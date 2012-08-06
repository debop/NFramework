using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;

namespace NSoft.NFramework.Collections.PowerCollections {
    /// <summary>
    /// The base implementation for various collections classes that use hash tables
    /// as part of their implementation. This class should not (and can not) be 
    /// used directly by end users; it's only for internal use by the collections package. The Hash
    /// does not handle duplicate values.
    /// </summary>
    /// <remarks>
    /// The Hash manages items of type T, and uses a IComparer&lt;ItemTYpe&gt; that
    /// hashes compares items to hash items into the table.  
    ///</remarks>
    [Serializable]
    public class Hash<T> : IEnumerable<T> 
#if !SILVERLIGHT
                           , ISerializable, IDeserializationCallback
#endif
    {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        // NOTE: If you add new member variables, you very well may need to change the serialization
        // code to serialize that member.
        private IEqualityComparer<T> _equalityComparer; // interface for comparing elements

        private int _count; // The count of elements in the table.

        private int _usedSlots;
                    // Includes real elements and deleted elements with the collision bit on. Used to determine when we need to resize.

        private int _totalSlots; // Size of the table. Always a power of two.
        private float _loadFactor = 0.70F; // maximal load factor for the table.
        private int _thresholdGrow; // floor(totalSlots * loadFactor);
        private int _thresholdShrink; // thresholdGrow / 3.
        private int _hashMask; // Mask to convert hash values to the size of the table.
        private int _secondaryShift; // Shift to get the secondary skip value.

        private Slot[] _table; // The hash table.

        private int _changeStamp; // An integer that is changed every time the table structurally changes.
        // Used so that enumerations throw an exception if the tree is changed
        // during enumeration.

        private const int MINSIZE = 16; // minimum number of slots.

#if !SILVERLIGHT
        private SerializationInfo _serializationInfo; // Info used during deserialization.
#endif

        /// <summary>
        /// The structure that has each slot in the hash table. Each slot has three parts:
        /// 1. The collision bit. Indicates whether some item visited this slot but had to
        /// keep looking because the slot was full. 
        /// 2. 31-bit full hash value of the item. If zero, the slot is empty.
        /// 3. The item itself.
        /// </summary>
        private struct Slot {
            private uint hash_collision; // Lower 31 bits: the hash value. Top bit: the collision bit. 
            public T item; // The item.

            /// <summary>
            /// The full hash value associated with the value in this slot, or zero
            /// if the slot is empty.
            /// </summary>
            public int HashValue {
                get { return (int)(hash_collision & 0x7FFFFFFF); }
                set {
                    Debug.Assert((value & 0x80000000) == 0); // make sure sign bit isn't set.
                    hash_collision = (uint)value | (hash_collision & 0x80000000);
                }
            }

            /// <summary>
            /// Is this slot empty?
            /// </summary>
            public bool Empty {
                get { return HashValue == 0; }
            }

            /// <summary>
            /// Clear this slot, leaving the collision bit alone.
            /// </summary>
            public void Clear() {
                HashValue = 0;
                item = default(T); // Done to avoid keeping things alive that shouldn't be.
            }

            /// <summary>
            /// The "Collision" bit indicates that some value hit this slot and
            /// collided, so had to try another slot.
            /// </summary>
            public bool Collision {
                get { return (hash_collision & 0x80000000) != 0; }
                set {
                    if(value)
                        hash_collision |= 0x80000000;
                    else
                        hash_collision &= 0x7FFFFFFF;
                }
            }
        }

        /// <summary>
        /// Constructor. Create a new hash table.
        /// </summary>
        /// <param name="equalityComparer">The comparer to use to compare items. </param>
        public Hash(IEqualityComparer<T> equalityComparer) {
            equalityComparer.ShouldNotBeNull("equalityComparer");
            _equalityComparer = equalityComparer;
        }

        /// <summary>
        /// Gets the current enumeration stamp. Call CheckEnumerationStamp later
        /// with this value to throw an exception if the hash table is changed.
        /// </summary>
        /// <returns>The current enumeration stamp.</returns>
        internal int GetEnumerationStamp() {
            return _changeStamp;
        }

        /// <summary>
        /// Must be called whenever there is a structural change in the tree. Causes
        /// changeStamp to be changed, which causes any in-progress enumerations
        /// to throw exceptions.
        /// </summary>
        internal void StopEnumerations() {
            ++_changeStamp;
        }

        /// <summary>
        /// Checks the given stamp against the current change stamp. If different, the
        /// collection has changed during enumeration and an InvalidOperationException
        /// must be thrown
        /// </summary>
        /// <param name="startStamp">changeStamp at the start of the enumeration.</param>
        internal void CheckEnumerationStamp(int startStamp) {
            Guard.Assert(_changeStamp == startStamp, Strings.ChangeDuringEnumeration);
        }

        /// <summary>
        /// Gets the full hash code for an item.
        /// </summary>
        /// <param name="item">Item to get hash code for.</param>
        /// <returns>The full hash code. It is never zero.</returns>
        private int GetFullHash(T item) {
            var hash = (uint)Util.GetHashCode(item, _equalityComparer);

            // The .NET framework tends to produce pretty bad hash codes.
            // Scramble them up to be much more random!
            hash += ~(hash << 15);
            hash ^= (hash >> 10);
            hash += (hash << 3);
            hash ^= (hash >> 6);
            hash += ~(hash << 11);
            hash ^= (hash >> 16);
            hash &= 0x7FFFFFFF;

            if(hash == 0)
                hash = 0x7FFFFFFF; // Make sure it isn't zero.

            return (int)hash;
        }

        /// <summary>
        /// Get the initial bucket number and skip amount from the full hash value.
        /// </summary>
        /// <param name="hash">The full hash value.</param>
        /// <param name="initialBucket">Returns the initial bucket. Always in the range 0..(totalSlots - 1).</param>
        /// <param name="skip">Returns the skip values. Always odd in the range 0..(totalSlots - 1).</param>
        private void GetHashValuesFromFullHash(int hash, out int initialBucket, out int skip) {
            initialBucket = hash & _hashMask;

            // The skip value must be relatively prime to the table size. Since the table size is a 
            // power of two, any odd number is relatively prime, so oring in 1 will do it.
            skip = ((hash >> _secondaryShift) & _hashMask) | 1;
        }

        /// <summary>
        /// Gets the full hash value, initial bucket number, and skip amount for an item.
        /// </summary>
        /// <param name="item">Item to get hash value of.</param>
        /// <param name="initialBucket">Returns the initial bucket. Always in the range 0..(totalSlots - 1).</param>
        /// <param name="skip">Returns the skip values. Always odd in the range 0..(totalSlots - 1).</param>
        /// <returns>The full hash value. This is never zero.</returns>
        private int GetHashValues(T item, out int initialBucket, out int skip) {
            int hash = GetFullHash(item);
            GetHashValuesFromFullHash(hash, out initialBucket, out skip);
            return hash;
        }

        /// <summary>
        /// Make sure there are enough slots in the hash table that <paramref name="additionalItems"/>
        /// items can be inserted into the table.
        /// </summary>
        /// <param name="additionalItems">Number of additional items we are inserting.</param>
        private void EnsureEnoughSlots(int additionalItems) {
            StopEnumerations();

            if(_usedSlots + additionalItems > _thresholdGrow) {
                // We need to expand the table. Figure out to what size.

                var newSize = Math.Max(_totalSlots, MINSIZE);

                while((int)(newSize * _loadFactor) < _usedSlots + additionalItems) {
                    newSize *= 2;

                    Guard.Assert(newSize > 0, Strings.CollectionTooLarge);
                    //if(newSize <= 0)
                    //{
                    //    // Must have overflowed the size of an int. Hard to believe we didn't run out of memory first.
                    //    throw new InvalidOperationException(Strings.CollectionTooLarge);
                    //}
                }

                ResizeTable(newSize);
            }
        }

        /// <summary>
        /// Check if the number of items in the table is small enough that
        /// we should shrink the table again.
        /// </summary>
        private void ShrinkIfNeeded() {
            if(_count < _thresholdShrink) {
                int newSize;

                if(_count > 0) {
                    newSize = MINSIZE;
                    while((int)(newSize * _loadFactor) < _count)
                        newSize *= 2;
                }
                else {
                    // We've removed all the elements. Shrink to zero.
                    newSize = 0;
                }

                ResizeTable(newSize);
            }
        }

        /// <summary>
        /// Given the size of a hash table, compute the "secondary shift" value -- the shift
        /// that is used to determine the skip amount for collision resolution.
        /// </summary>
        /// <param name="newSize">The new size of the table.</param>
        /// <returns>The secondary skip amount.</returns>
        private static int GetSecondaryShift(int newSize) {
            var x = newSize - 2; // x is of the form 0000111110 -- a single string of 1's followed by a single zero.
            var secondaryShift = 0;

            // Keep shifting x until it is the set of bits we want to extract: it be the highest bits possible,
            // but can't overflow into the sign bit.
            while((x & 0x40000000) == 0) {
                x <<= 1;
                ++secondaryShift;
            }

            return secondaryShift;
        }

        /// <summary>
        /// Resize the hash table to the given new size, moving all items into the
        /// new hash table.
        /// </summary>
        /// <param name="newSize">The new size of the hash table. Must be a power
        /// of two.</param>
        private void ResizeTable(int newSize) {
            Slot[] oldTable = _table; // Move all the items from this table to the new table.

            Guard.Assert((newSize & (newSize - 1)) == 0); // Check newSize is a power of two.

            _totalSlots = newSize;
            _thresholdGrow = (int)(_totalSlots * _loadFactor);
            _thresholdShrink = _thresholdGrow / 3;

            if(_thresholdShrink <= MINSIZE)
                _thresholdShrink = 1;

            _hashMask = newSize - 1;
            _secondaryShift = GetSecondaryShift(newSize);
            _table = _totalSlots > 0 ? new Slot[_totalSlots] : null;

            if(oldTable != null && _table != null) {
                foreach(Slot oldSlot in oldTable) {
                    int bucket, skip;

                    var hash = oldSlot.HashValue;
                    GetHashValuesFromFullHash(hash, out bucket, out skip);

                    // Find an empty bucket.
                    while(!_table[bucket].Empty) {
                        // The slot is used, but isn't our item. Set the collision bit and keep looking.
                        _table[bucket].Collision = true;
                        bucket = (bucket + skip) & _hashMask;
                    }

                    // We found an empty bucket. 
                    _table[bucket].HashValue = hash;
                    _table[bucket].item = oldSlot.item;
                }
            }

            _usedSlots = _count; // no deleted elements have the collision bit on now.
        }

        /// <summary>
        /// Get the number of items in the hash table.
        /// </summary>
        /// <value>The number of items stored in the hash table.</value>
        public int ElementCount {
            get { return _count; }
        }

        /// <summary>
        /// Get the number of slots in the hash table. Exposed internally
        /// for testing purposes.
        /// </summary>
        /// <value>The number of slots in the hash table.</value>
        public int SlotCount {
            get { return _totalSlots; }
        }

        /// <summary>
        /// Get or change the load factor. Changing the load factor may cause
        /// the size of the table to grow or shrink accordingly.
        /// </summary>
        /// <value></value>
        public float LoadFactor {
            get { return _loadFactor; }
            set {
                // Don't allow hopelessly inefficient load factors.
                value.ShouldBeInRange<float>(0.25F, 0.95000001F, "value");

                StopEnumerations();

                var maybeExpand = value < _loadFactor; // May need to expand or shrink the table -- which?

                // Update loadFactor and thresholds.
                _loadFactor = value;
                _thresholdGrow = (int)(_totalSlots * _loadFactor);
                _thresholdShrink = _thresholdGrow / 3;

                if(_thresholdShrink <= MINSIZE)
                    _thresholdShrink = 1;

                // Possibly expand or shrink the table.
                if(maybeExpand)
                    EnsureEnoughSlots(0);
                else
                    ShrinkIfNeeded();
            }
        }

        /// <summary>
        /// Insert a new item into the hash table. If a duplicate item exists, can replace or
        /// do nothing.
        /// </summary>
        /// <param name="item">The item to insert.</param>
        /// <param name="replaceOnDuplicate">If true, duplicate items are replaced. If false, nothing
        /// is done if a duplicate already exists.</param>
        /// <param name="previous">If a duplicate was found, returns it (whether replaced or not).</param>
        /// <returns>True if no duplicate existed, false if a duplicate was found.</returns>
        public bool Insert(T item, bool replaceOnDuplicate, out T previous) {
            int bucket, skip;

            var emptyBucket = -1; // If >= 0, an empty bucket we can use for a true insert
            var duplicateMightExist = true; // If true, still the possibility that a duplicate exists.

            EnsureEnoughSlots(1); // Ensure enough room to insert. Also stops enumerations.

            var hash = GetHashValues(item, out bucket, out skip);

            for(;;) {
                if(_table[bucket].Empty) {
                    // Record the location of the first empty bucket seen. This is where the item will
                    // go if no duplicate exists.
                    if(emptyBucket == -1)
                        emptyBucket = bucket;

                    if(!duplicateMightExist || !_table[bucket].Collision) {
                        // There can't be a duplicate further on, because a bucket with the collision bit
                        // clear was found (here or earlier). We have the place to insert.
                        break;
                    }
                }
                else if(_table[bucket].HashValue == hash && _equalityComparer.Equals(_table[bucket].item, item)) {
                    // We found a duplicate item. Replace it if requested to.
                    previous = _table[bucket].item;
                    if(replaceOnDuplicate)
                        _table[bucket].item = item;
                    return false;
                }
                else {
                    // The slot is used, but isn't our item. 
                    if(!_table[bucket].Collision) {
                        // Since the collision bit is off, we can't have a duplicate. 
                        if(emptyBucket >= 0) {
                            // We already have an empty bucket to use.
                            break;
                        }

                        // Keep searching for an empty bucket to place the item.
                        _table[bucket].Collision = true;
                        duplicateMightExist = false;
                    }
                }

                bucket = (bucket + skip) & _hashMask;
            }

            // We found an empty bucket. Insert the new item.
            _table[emptyBucket].HashValue = hash;
            _table[emptyBucket].item = item;

            ++_count;
            if(_table[emptyBucket].Collision == false)
                ++_usedSlots;
            previous = default(T);
            return true;
        }

        /// <summary>
        /// Deletes an item from the hash table. 
        /// </summary>
        /// <param name="item">Item to search for and delete.</param>
        /// <param name="itemDeleted">If true returned, the actual item stored in the hash table (must be 
        /// equal to <paramref name="item"/>, but may not be identical.</param>
        /// <returns>True if item was found and deleted, false if item wasn't found.</returns>
        public bool Delete(T item, out T itemDeleted) {
            int bucket, skip;

            StopEnumerations();

            if(_count == 0) {
                itemDeleted = default(T);
                return false;
            }

            var hash = GetHashValues(item, out bucket, out skip);

            for(;;) {
                if(_table[bucket].HashValue == hash && _equalityComparer.Equals(_table[bucket].item, item)) {
                    // Found the item. Remove it.
                    itemDeleted = _table[bucket].item;
                    _table[bucket].Clear();
                    --_count;
                    if(!_table[bucket].Collision)
                        --_usedSlots;
                    ShrinkIfNeeded();
                    return true;
                }

                if(!_table[bucket].Collision) {
                    // No collision bit, so we can stop searching. No such element.
                    itemDeleted = default(T);
                    return false;
                }

                bucket = (bucket + skip) & _hashMask;
            }
        }

        /// <summary>
        /// Find an item in the hash table. If found, optionally replace it with the
        /// finding item.
        /// </summary>
        /// <param name="find">Item to find.</param>
        /// <param name="replace">If true, replaces the equal item in the hash table
        /// with <paramref name="item"/>.</param>
        /// <param name="item">Returns the equal item found in the table, if true was returned.</param>
        /// <returns>True if the item was found, false otherwise.</returns>
        public bool Find(T find, bool replace, out T item) {
            int bucket, skip;

            if(_count == 0) {
                item = default(T);
                return false;
            }

            var hash = GetHashValues(find, out bucket, out skip);

            for(;;) {
                if(_table[bucket].HashValue == hash && _equalityComparer.Equals(_table[bucket].item, find)) {
                    // Found the item.  
                    item = _table[bucket].item;

                    if(replace)
                        _table[bucket].item = find;
                    return true;
                }

                if(!_table[bucket].Collision) {
                    // No collision bit, so we can stop searching. No such element.
                    item = default(T);
                    return false;
                }

                bucket = (bucket + skip) & _hashMask;
            }
        }

        /// <summary>
        /// Enumerate all of the items in the hash table. The items
        /// are enumerated in a haphazard, unpredictable order.
        /// </summary>
        /// <returns>An IEnumerator&lt;T&gt; that enumerates the items
        /// in the hash table.</returns>
        public IEnumerator<T> GetEnumerator() {
            if(_count > 0) {
                var startStamp = _changeStamp;

                foreach(var slot in _table.Where(s => !s.Empty)) {
                    yield return slot.item;
                    CheckEnumerationStamp(startStamp);
                }
            }
        }

        /// <summary>
        /// Enumerate all of the items in the hash table. The items
        /// are enumerated in a haphazard, unpredictable order.
        /// </summary>
        /// <returns>An IEnumerator that enumerates the items
        /// in the hash table.</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        /// <summary>
        /// Creates a clone of this hash table.
        /// </summary>
        /// <param name="cloneItem">If non-null, this function is applied to each item when cloning. It must be the 
        /// case that this function does not modify the hash code or equality function.</param>
        /// <returns>A shallow clone that contains the same items.</returns>
        public Hash<T> Clone(Func<T, T> cloneItem) {
            var clone = new Hash<T>(_equalityComparer);

            clone._count = _count;
            clone._usedSlots = _usedSlots;
            clone._totalSlots = _totalSlots;
            clone._loadFactor = _loadFactor;
            clone._thresholdGrow = _thresholdGrow;
            clone._thresholdShrink = _thresholdShrink;
            clone._hashMask = _hashMask;
            clone._secondaryShift = _secondaryShift;

            if(_table != null) {
                clone._table = (Slot[])_table.Clone();

                if(cloneItem != null) {
                    for(int i = 0; i < _table.Length; ++i) {
                        if(_table[i].Empty == false)
                            _table[i].item = cloneItem(_table[i].item);
                    }
                }
            }

            return clone;
        }

#if !SILVERLIGHT

        #region Serialization

        /// <summary>
        /// Serialize the hash table. Called from the serialization infrastructure.
        /// </summary>
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context) {
            if(info == null)
                throw new ArgumentNullException("info");

            info.AddValue("equalityComparer", _equalityComparer, typeof(IEqualityComparer<T>));
            info.AddValue("loadFactor", _loadFactor, typeof(float));

            var items = new T[_count];
            var i = 0;

            foreach(var slot in _table.Where(slot => !slot.Empty)) {
                items[i++] = slot.item;
            }

            info.AddValue("items", items, typeof(T[]));
        }

        /// <summary>
        /// Called on deserialization. We cannot deserialize now, because hash codes
        /// might not be correct now. We do real deserialization in the OnDeserialization call.
        /// </summary>
        protected Hash(SerializationInfo serInfo, StreamingContext context) {
            // Save away the serialization info for use later. We can't be sure of hash codes
            // being stable until the entire object graph is deserialized, so we wait until then
            // to deserialize.
            _serializationInfo = serInfo;
        }

        /// <summary>
        /// Deserialize the hash table. Called from the serialization infrastructure when 
        /// the object graph has finished deserializing.
        /// </summary>
        void IDeserializationCallback.OnDeserialization(object sender) {
            if(_serializationInfo == null)
                return;

            _loadFactor = _serializationInfo.GetSingle("loadFactor");
            _equalityComparer = (IEqualityComparer<T>)_serializationInfo.GetValue("equalityComparer", typeof(IEqualityComparer<T>));

            var items = (T[])_serializationInfo.GetValue("items", typeof(T[]));
            EnsureEnoughSlots(items.Length);

            T dummy;
            foreach(T item in items)
                Insert(item, true, out dummy);

            _serializationInfo = null;
        }

        #endregion Serialization

#endif

#if DEBUG
        /// <summary>
        /// Print out basic stats about the hash table.
        /// </summary>
        public void PrintStats() {
            if(IsDebugEnabled) {
                log.Debug("count={0}  usedSlots={1}  totalSlots={2}", _count, _usedSlots, _totalSlots);
                log.Debug("loadFactor={0}  thresholdGrow={1}  thresholdShrink={2}", _loadFactor, _thresholdGrow, _thresholdShrink);
                log.Debug("hashMask={0:X}  secondaryShift={1}", _hashMask, _secondaryShift);
            }
        }

        /// <summary>
        /// Print out the state of the hash table and each of the slots. Each slot looks like:
        ///     Slot    4: C 4513e41e hello
        /// where the "C" indicates the collision bit is on
        /// the next hex number is the hash value
        /// followed by ToString() on the item.
        /// </summary>
        public void Print() {
            if(IsDebugEnabled) {
                PrintStats();
                for(int i = 0; i < _totalSlots; ++i)
                    log.Debug("Slot {0,4:X}: {1} {2,8:X} {3}", i, _table[i].Collision ? "C" : " ",
                              _table[i].HashValue, _table[i].Empty ? "<empty>" : _table[i].item.ToString());
            }
        }

        /// <summary>
        /// Check that everything appears to be OK in the hash table.
        /// </summary>
        public void Validate() {
            Guard.Assert(_count <= _usedSlots);
            Guard.Assert(_count <= _totalSlots);
            Guard.Assert(_usedSlots <= _totalSlots);
            Guard.Assert(_usedSlots <= _thresholdGrow);
            Guard.Assert((int)(_totalSlots * _loadFactor) == _thresholdGrow);

            if(_thresholdShrink > 1)
                Guard.Assert(_thresholdGrow / 3 == _thresholdShrink);
            else
                Guard.Assert(_thresholdGrow / 3 <= MINSIZE);

            if(_totalSlots > 0) {
                Guard.Assert((_totalSlots & (_totalSlots - 1)) == 0); // totalSlots is a power of two.
                Guard.Assert(_totalSlots - 1 == _hashMask);
                Guard.Assert(GetSecondaryShift(_totalSlots) == _secondaryShift);
                Guard.Assert(_totalSlots == _table.Length);
            }

            // Traverse the table. Make sure that count and usedSlots are right, and that
            // each slot looks reasonable.
            int expectedCount = 0, expectedUsed = 0;
            if(_table != null) {
                for(var i = 0; i < _totalSlots; ++i) {
                    var slot = _table[i];
                    if(slot.Empty) {
                        // Empty slot
                        if(slot.Collision)
                            ++expectedUsed;
                        Guard.Assert(object.Equals(default(T), slot.item));
                    }
                    else {
                        // not empty.
                        ++expectedCount;
                        ++expectedUsed;
                        Guard.Assert(slot.HashValue != 0);
                        int initialBucket;
                        int skip;
                        Guard.Assert(GetHashValues(slot.item, out initialBucket, out skip) == slot.HashValue);
                        if(initialBucket != i)
                            Guard.Assert(_table[initialBucket].Collision);
                    }
                }
            }

            Guard.Assert(expectedCount == _count);
            Guard.Assert(expectedUsed == _usedSlots);
        }

#endif
    }
}