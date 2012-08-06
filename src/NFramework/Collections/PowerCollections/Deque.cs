using System;
using System.Collections.Generic;
using System.Text;

namespace NSoft.NFramework.Collections.PowerCollections {
    /// <summary>
    /// <para>The Deque class implements a type of list known as a Double Ended Queue. A Deque
    /// is quite similar to a List, in that items have indices (starting at 0), and the item at any
    /// index can be efficiently retrieved. The difference between a List and a Deque lies in the
    /// efficiency of inserting elements at the beginning. In a List, items can be efficiently added
    /// to the end, but inserting an item at the beginning of the List is slow, taking time 
    /// proportional to the size of the List. In a Deque, items can be added to the beginning 
    /// or end equally efficiently, regardless of the number of items in the Deque. As a trade-off
    /// for this increased flexibility, Deque is somewhat slower than List (but still constant time) when
    /// being indexed to get or retrieve elements. </para>
    /// </summary>
    /// <remarks>
    /// <para>The Deque class can also be used as a more flexible alternative to the Queue 
    /// and Stack classes. Deque is as efficient as Queue and Stack for adding or removing items, 
    /// but is more flexible: it allows access
    /// to all items in the queue, and allows adding or removing from either end.</para>
    /// <para>Deque is implemented as a ring buffer, which is grown as necessary. The size
    /// of the buffer is doubled whenever the existing capacity is too small to hold all the
    /// elements.</para>
    /// </remarks>
    /// <typeparam name="T">The type of items stored in the Deque.</typeparam>
    [Serializable]
    public class Deque<T> : ListBase<T> {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        // The initial size of the buffer.
        private const int INITIAL_SIZE = 8;

        // A ring buffer containing all the items in the deque. Shrinks or grows as needed.
        // Except temporarily during an add, there is always at least one empty item.
        private T[] _buffer;

        // Index of the first item (index 0) in the deque.
        // Always in the range 0 through buffer.Length - 1.
        private int _start;

        // Index just beyond the last item in the deque. If equal to start, the deque is empty.
        // Always in the range 0 through buffer.Length - 1.
        private int _end;

        // Holds the change stamp for the collection.
        private int _changeStamp;

        /// <summary>
        /// Must be called whenever there is a structural change in the tree. Causes
        /// changeStamp to be changed, which causes any in-progress enumerations
        /// to throw exceptions.
        /// </summary>
        private void StopEnumerations() {
            ++_changeStamp;
        }

        /// <summary>
        /// Checks the given stamp against the current change stamp. If different, the
        /// collection has changed during enumeration and an InvalidOperationException
        /// must be thrown
        /// </summary>
        /// <param name="startStamp">changeStamp at the start of the enumeration.</param>
        private void CheckEnumerationStamp(int startStamp) {
            Guard.Assert(startStamp == _changeStamp, Strings.ChangeDuringEnumeration);
        }

        /// <summary>
        /// Create a new Deque that is initially empty.
        /// </summary>
        public Deque() {}

        /// <summary>
        /// Create a new Deque initialized with the items from the passed collection,
        /// in order.
        /// </summary>
        /// <param name="collection">A collection of items to initialize the Deque with.</param>
        public Deque(IEnumerable<T> collection) {
            AddManyToBack(collection);
        }

        /// <summary>
        /// Gets the number of items currently stored in the Deque. The last item
        /// in the Deque has index Count-1.
        /// </summary>
        /// <remarks>Getting the count of items in the Deque takes a small constant
        /// amount of time.</remarks>
        /// <value>The number of items stored in this Deque.</value>
        public override sealed int Count {
            get {
                if(_end >= _start)
                    return _end - _start;

                return _end + _buffer.Length - _start;
            }
        }

        /// <summary>
        /// Copies all the items in the Deque into an array.
        /// </summary>
        /// <param name="array">Array to copy to.</param>
        /// <param name="arrayIndex">Starting index in <paramref name="array"/> to copy to.</param>
        public override sealed void CopyTo(T[] array, int arrayIndex) {
            array.ShouldNotBeNull("array");

            // This override is provided to give a more efficient implementation to CopyTo than
            // the default one provided by CollectionBase.

            if(_buffer == null || _buffer.Length == 0)
                return;

            var length = _buffer.Length;

            //var length = (_buffer == null) ? 0 : _buffer.Length;

            if(_start > _end) {
                Array.Copy(_buffer, _start, array, arrayIndex, length - _start);
                Array.Copy(_buffer, 0, array, arrayIndex + length - _start, _end);
            }
            else {
                if(_end > _start)
                    Array.Copy(_buffer, _start, array, arrayIndex, _end - _start);
            }
        }

        /// <summary>
        /// Gets or sets the capacity of the Deque. The Capacity is the number of
        /// items that this Deque can hold without expanding its internal buffer. Since
        /// Deque will automatically expand its buffer when necessary, in almost all cases
        /// it is unnecessary to worry about the capacity. However, if it is known that a
        /// Deque will contain exactly 1000 items eventually, it can slightly improve 
        /// efficiency to set the capacity to 1000 up front, so that the Deque does not
        /// have to expand automatically.
        /// </summary>
        /// <value>The number of items that this Deque can hold without expanding its
        /// internal buffer.</value>
        /// <exception cref="ArgumentOutOfRangeException">The capacity is being set
        /// to less than Count, or to too large a value.</exception>
        public int Capacity {
            get { return (_buffer == null) ? 0 : _buffer.Length - 1; }
            set {
                if(value < Count)
                    throw new ArgumentOutOfRangeException("value", Strings.CapacityLessThanCount);
                if(value > int.MaxValue - 1)
                    throw new ArgumentOutOfRangeException("value");
                if(value == Capacity)
                    return;

                var newBuffer = new T[value + 1];
                CopyTo(newBuffer, 0);

                _end = Count;
                _start = 0;
                _buffer = newBuffer;
            }
        }

        /// <summary>
        /// Trims the amount of memory used by the Deque by changing
        /// the Capacity to be equal to Count. If no more items will be added
        /// to the Deque, calling TrimToSize will reduce the amount of memory
        /// used by the Deque.
        /// </summary>
        public void TrimToSize() {
            Capacity = Count;
        }

        /// <summary>
        /// Removes all items from the Deque.
        /// </summary>
        /// <remarks>Clearing the Deque takes a small constant amount of time, regardless of
        /// how many items are currently in the Deque.</remarks>
        public override sealed void Clear() {
            StopEnumerations();
            _buffer = null;
            _start = _end = 0;
        }

        /// <summary>
        /// Gets or sets an item at a particular index in the Deque. 
        /// </summary>
        /// <remarks>Getting or setting the item at a particular index takes a small constant amount
        /// of time, no matter what index is used.</remarks>
        /// <param name="index">The index of the item to retrieve or change. The front item has index 0, and
        /// the back item has index Count-1.</param>
        /// <returns>The value at the indicated index.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The index is less than zero or greater than or equal
        /// to Count.</exception>
        public override sealed T this[int index] {
            get {
                var i = index + _start;
                if(i < _start) // handles both the case where index < 0, or the above addition overflow to a negative number.
                    throw new ArgumentOutOfRangeException("index");

                if(_end >= _start) {
                    if(i >= _end)
                        throw new ArgumentOutOfRangeException("index");
                    return _buffer[i];
                }

                var length = _buffer.Length;

                if(i >= length) {
                    i -= length;
                    if(i >= _end)
                        throw new ArgumentOutOfRangeException("index");
                }
                return _buffer[i];
            }

            set {
                // Like List<T>, we stop enumerations after a set operation. There is no
                // technical reason to do this, however.
                StopEnumerations();

                var i = index + _start;

                if(i < _start) // handles both the case where index < 0, or the above addition overflow to a negative number.
                    throw new ArgumentOutOfRangeException("index");

                if(_end >= _start) {
                    if(i >= _end)
                        throw new ArgumentOutOfRangeException("index");
                    _buffer[i] = value;
                }
                else {
                    var length = _buffer.Length;

                    if(i >= length) {
                        i -= length;
                        if(i >= _end)
                            throw new ArgumentOutOfRangeException("index");
                    }
                    _buffer[i] = value;
                }
            }
        }

        /// <summary>
        /// Enumerates all of the items in the list, in order. The item at index 0
        /// is enumerated first, then the item at index 1, and so on. If the items
        /// are added to or removed from the Deque during enumeration, the 
        /// enumeration ends with an InvalidOperationException.
        /// </summary>
        /// <returns>An IEnumerator&lt;T&gt; that enumerates all the
        /// items in the list.</returns>
        /// <exception cref="InvalidOperationException">The Deque has an item added or deleted during the enumeration.</exception>
        public override sealed IEnumerator<T> GetEnumerator() {
            var startStamp = _changeStamp;
            var count = Count;

            for(var i = 0; i < count; ++i) {
                yield return this[i];
                CheckEnumerationStamp(startStamp);
            }
        }

        /// <summary>
        /// Creates the initial buffer and initialized the Deque to contain one initial
        /// item.
        /// </summary>
        /// <param name="firstItem">First and only item for the Deque.</param>
        private void CreateInitialBuffer(T firstItem) {
            // The buffer hasn't been created yet.
            _buffer = new T[INITIAL_SIZE];
            _start = 0;
            _end = 1;
            _buffer[0] = firstItem;
        }

        /// <summary>
        /// Inserts a new item at the given index in the Deque. All items at indexes 
        /// equal to or greater than <paramref name="index"/> move up one index
        /// in the Deque.
        /// </summary>
        /// <remarks>The amount of time to insert an item in the Deque is proportional
        /// to the distance of index from the closest end of the Deque: 
        /// O(Min(<paramref name="index"/>, Count - <paramref name="index"/>)).
        /// Thus, inserting an item at the front or end of the Deque is always fast; the middle of
        /// of the Deque is the slowest place to insert.
        /// </remarks>
        /// <param name="index">The index in the Deque to insert the item at. After the
        /// insertion, the inserted item is located at this index. The
        /// front item in the Deque has index 0.</param>
        /// <param name="item">The item to insert at the given index.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is
        /// less than zero or greater than Count.</exception>
        public override sealed void Insert(int index, T item) {
            StopEnumerations();

            var count = Count;
            if(index < 0 || index > Count)
                throw new ArgumentOutOfRangeException("index");

            if(_buffer == null) {
                // The buffer hasn't been created yet.
                CreateInitialBuffer(item);
                return;
            }

            var length = _buffer.Length;
            int i; // The location the new item was placed at.

            if(index < count / 2) {
                // Inserting into the first half of the list. Move items with
                // lower index down in the buffer.
                _start -= 1;
                if(_start < 0)
                    _start += length;
                i = index + _start;
                if(i >= length) {
                    i -= length;
                    if(length - 1 > _start)
                        Array.Copy(_buffer, _start + 1, _buffer, _start, length - 1 - _start);
                    _buffer[length - 1] = _buffer[0]; // unneeded if end == 0, but doesn't hurt
                    if(i > 0)
                        Array.Copy(_buffer, 1, _buffer, 0, i);
                }
                else {
                    if(i > _start)
                        Array.Copy(_buffer, _start + 1, _buffer, _start, i - _start);
                }
            }
            else {
                // Inserting into the last half of the list. Move items with higher
                // index up in the buffer.
                i = index + _start;
                if(i >= length)
                    i -= length;
                if(i <= _end) {
                    if(_end > i)
                        Array.Copy(_buffer, i, _buffer, i + 1, _end - i);
                    _end += 1;
                    if(_end >= length)
                        _end -= length;
                }
                else {
                    if(_end > 0)
                        Array.Copy(_buffer, 0, _buffer, 1, _end);
                    _buffer[0] = _buffer[length - 1];
                    if(length - 1 > i)
                        Array.Copy(_buffer, i, _buffer, i + 1, length - 1 - i);
                    _end += 1;
                }
            }

            _buffer[i] = item;
            if(_start == _end)
                IncreaseBuffer();
        }

        /// <summary>
        /// Inserts a collection of items at the given index in the Deque. All items at indexes 
        /// equal to or greater than <paramref name="index"/> increase their indices in the Deque
        /// by the number of items inserted.
        /// </summary>
        /// <remarks>The amount of time to insert a collection in the Deque is proportional
        /// to the distance of index from the closest end of the Deque, plus the number of items
        /// inserted (M): 
        /// O(M + Min(<paramref name="index"/>, Count - <paramref name="index"/>)).
        /// </remarks>
        /// <param name="index">The index in the Deque to insert the collection at. After the
        /// insertion, the first item of the inserted collection is located at this index. The
        /// front item in the Deque has index 0.</param>
        /// <param name="collection">The collection of items to insert at the given index.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is
        /// less than zero or greater than Count.</exception>
        public void InsertRange(int index, IEnumerable<T> collection) {
            collection.ShouldNotBeNull("collection");

            StopEnumerations();

            var count = Count;
            if(index < 0 || index > Count)
                throw new ArgumentOutOfRangeException("index");

            // We need an ICollection, because we need the count of the collection.
            // If needed, copy the items to a temporary list.

            ICollection<T> coll = collection is ICollection<T>
                                      ? (ICollection<T>)collection
                                      : new List<T>(collection);

            if(coll.Count == 0)
                return; // nothing to do.

            // Create enough capacity in the list for the new items.
            if(Capacity < Count + coll.Count)
                Capacity = Count + coll.Count;

            var length = _buffer.Length;
            int s, d;

            if(index < count / 2) {
                // Inserting into the first half of the list. Move items with
                // lower index down in the buffer.
                s = _start;
                d = s - coll.Count;
                if(d < 0)
                    d += length;
                _start = d;
                var c = index;

                while(c > 0) {
                    var chunk = c;

                    if(length - d < chunk)
                        chunk = length - d;
                    if(length - s < chunk)
                        chunk = length - s;

                    Array.Copy(_buffer, s, _buffer, d, chunk);

                    c -= chunk;
                    if((d += chunk) >= length)
                        d -= length;
                    if((s += chunk) >= length)
                        s -= length;
                }
            }
            else {
                // Inserting into the last half of the list. Move items with higher
                // index up in the buffer.
                s = _end;
                d = s + coll.Count;
                if(d >= length)
                    d -= length;
                _end = d;
                var move = count - index; // number of items at end to move

                var c = move;
                while(c > 0) {
                    var chunk = c;
                    if(d > 0 && d < chunk)
                        chunk = d;
                    if(s > 0 && s < chunk)
                        chunk = s;
                    if((d -= chunk) < 0)
                        d += length;
                    if((s -= chunk) < 0)
                        s += length;

                    Array.Copy(_buffer, s, _buffer, d, chunk);
                    c -= chunk;
                }

                d -= coll.Count;
                if(d < 0)
                    d += length;
            }

            // Copy the items into the space vacated, which starts at d.
            foreach(T item in coll) {
                _buffer[d] = item;
                if(++d >= length)
                    d -= length;
            }
        }

        /// <summary>
        /// Removes the item at the given index in the Deque. All items at indexes 
        /// greater than <paramref name="index"/> move down one index
        /// in the Deque.
        /// </summary>
        /// <remarks>The amount of time to delete an item in the Deque is proportional
        /// to the distance of index from the closest end of the Deque: 
        /// O(Min(<paramref name="index"/>, Count - 1 - <paramref name="index"/>)).
        /// Thus, deleting an item at the front or end of the Deque is always fast; the middle of
        /// of the Deque is the slowest place to delete.
        /// </remarks>
        /// <param name="index">The index in the list to remove the item at. The
        /// first item in the list has index 0.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is
        /// less than zero or greater than or equal to Count.</exception>
        public override sealed void RemoveAt(int index) {
            StopEnumerations();

            var count = Count;

            if(index < 0 || index >= count)
                throw new ArgumentOutOfRangeException("index");

            var length = _buffer.Length;
            int i; // index of removed item
            if(index < count / 2) {
                // Removing in the first half of the list. Move items with
                // lower index up in the buffer.
                i = index + _start;

                if(i >= length) {
                    i -= length;

                    if(i > 0)
                        Array.Copy(_buffer, 0, _buffer, 1, i);

                    _buffer[0] = _buffer[length - 1];
                    if(length - 1 > _start)
                        Array.Copy(_buffer, _start, _buffer, _start + 1, length - 1 - _start);
                }
                else {
                    if(i > _start)
                        Array.Copy(_buffer, _start, _buffer, _start + 1, i - _start);
                }

                _buffer[_start] = default(T);
                _start += 1;
                if(_start >= length)
                    _start -= length;
            }
            else {
                // Removing in the second half of the list. Move items with
                // higher indexes down in the buffer.
                i = index + _start;
                if(i >= length)
                    i -= length;

                _end -= 1;
                if(_end < 0)
                    _end = length - 1;

                if(i <= _end) {
                    if(_end > i)
                        Array.Copy(_buffer, i + 1, _buffer, i, _end - i);
                }
                else {
                    if(length - 1 > i)
                        Array.Copy(_buffer, i + 1, _buffer, i, length - 1 - i);
                    _buffer[length - 1] = _buffer[0];
                    if(_end > 0)
                        Array.Copy(_buffer, 1, _buffer, 0, _end);
                }

                _buffer[_end] = default(T);
            }
        }

        /// <summary>
        /// Removes a range of items at the given index in the Deque. All items at indexes 
        /// greater than <paramref name="index"/> move down <paramref name="count"/> indices
        /// in the Deque.
        /// </summary>
        /// <remarks>The amount of time to delete <paramref name="count"/> items in the Deque is proportional
        /// to the distance to the closest end of the Deque: 
        /// O(Min(<paramref name="index"/>, Count - <paramref name="index"/> - <paramref name="count"/>)).
        /// </remarks>
        /// <param name="index">The index in the list to remove the range at. The
        /// first item in the list has index 0.</param>
        /// <param name="count">The number of items to remove.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is
        /// less than zero or greater than or equal to Count, or <paramref name="count"/> is less than zero
        /// or too large.</exception>
        public void RemoveRange(int index, int count) {
            StopEnumerations();

            var dequeCount = Count;

            if(index < 0 || index >= dequeCount)
                throw new ArgumentOutOfRangeException("index");
            if(count < 0 || count > dequeCount - index)
                throw new ArgumentOutOfRangeException("count");

            if(count == 0)
                return;

            var length = _buffer.Length;
            int s, d;
            if(index < dequeCount / 2) {
                // Removing in the first half of the list. Move items with
                // lower index up in the buffer.
                s = _start + index;
                if(s >= length)
                    s -= length;
                d = s + count;
                if(d >= length)
                    d -= length;

                int c = index;
                while(c > 0) {
                    int chunk = c;
                    if(d > 0 && d < chunk)
                        chunk = d;
                    if(s > 0 && s < chunk)
                        chunk = s;
                    if((d -= chunk) < 0)
                        d += length;
                    if((s -= chunk) < 0)
                        s += length;
                    Array.Copy(_buffer, s, _buffer, d, chunk);
                    c -= chunk;
                }

                // At this point, s == start
                for(c = 0; c < count; ++c) {
                    _buffer[s] = default(T);
                    if(++s >= length)
                        s -= length;
                }
                _start = s;
            }
            else {
                // Removing in the second half of the list. Move items with
                // higher indexes down in the buffer.
                var move = dequeCount - index - count;
                s = _end - move;
                if(s < 0)
                    s += length;
                d = s - count;
                if(d < 0)
                    d += length;

                var c = move;
                while(c > 0) {
                    int chunk = c;
                    if(length - d < chunk)
                        chunk = length - d;
                    if(length - s < chunk)
                        chunk = length - s;
                    Array.Copy(_buffer, s, _buffer, d, chunk);
                    c -= chunk;
                    if((d += chunk) >= length)
                        d -= length;
                    if((s += chunk) >= length)
                        s -= length;
                }

                // At this point, s == end.
                for(c = 0; c < count; ++c) {
                    if(--s < 0)
                        s += length;
                    _buffer[s] = default(T);
                }
                _end = s;
            }
        }

        /// <summary>
        /// Increase the amount of buffer space. When calling this method, the Deque
        /// must not be empty. If start and end are equal, that indicates a completely
        /// full Deque.
        /// </summary>
        private void IncreaseBuffer() {
            var length = _buffer.Length;

            var newBuffer = new T[length * 2];

            if(_start >= _end) {
                Array.Copy(_buffer, _start, newBuffer, 0, length - _start);
                Array.Copy(_buffer, 0, newBuffer, length - _start, _end);
                _end = _end + length - _start;
                _start = 0;
            }
            else {
                Array.Copy(_buffer, _start, newBuffer, 0, _end - _start);
                _end = _end - _start;
                _start = 0;
            }

            _buffer = newBuffer;
        }

        /// <summary>
        /// Adds an item to the front of the Deque. The indices of all existing items
        /// in the Deque are increased by 1. This method is 
        /// equivalent to <c>Insert(0, item)</c> but is a little more
        /// efficient.
        /// </summary>
        /// <remarks>Adding an item to the front of the Deque takes
        /// a small constant amount of time, regardless of how many items are in the Deque.</remarks>
        /// <param name="item">The item to add.</param>
        public void AddToFront(T item) {
            StopEnumerations();

            if(_buffer == null) {
                // The buffer hasn't been created yet.
                CreateInitialBuffer(item);
                return;
            }

            if(--_start < 0)
                _start += _buffer.Length;

            _buffer[_start] = item;

            if(_start == _end)
                IncreaseBuffer();
        }

        /// <summary>
        /// Adds a collection of items to the front of the Deque. The indices of all existing items
        /// in the Deque are increased by the number of items inserted. The first item in the added collection becomes the
        /// first item in the Deque. 
        /// </summary>
        /// <remarks>This method takes time O(M), where M is the number of items in the 
        /// <paramref name="collection"/>.</remarks>
        /// <param name="collection">The collection of items to add.</param>
        public void AddManyToFront(IEnumerable<T> collection) {
            collection.ShouldNotBeNull("collection");

            InsertRange(0, collection);
        }

        /// <summary>
        /// Adds an item to the back of the Deque. The indices of all existing items
        /// in the Deque are unchanged. This method is 
        /// equivalent to <c>Insert(Count, item)</c> but is a little more
        /// efficient.
        /// </summary>
        /// <remarks>Adding an item to the back of the Deque takes
        /// a small constant amount of time, regardless of how many items are in the Deque.</remarks>
        /// <param name="item">The item to add.</param>
        public void AddToBack(T item) {
            StopEnumerations();

            if(_buffer == null) {
                // The buffer hasn't been created yet.
                CreateInitialBuffer(item);
                return;
            }

            _buffer[_end] = item;

            if(++_end >= _buffer.Length)
                _end -= _buffer.Length;

            if(_start == _end)
                IncreaseBuffer();
        }

        /// <summary>
        /// Adds an item to the back of the Deque. The indices of all existing items
        /// in the Deque are unchanged. This method is 
        /// equivalent to <c>AddToBack(item)</c>.
        /// </summary>
        /// <remarks>Adding an item to the back of the Deque takes
        /// a small constant amount of time, regardless of how many items are in the Deque.</remarks>
        /// <param name="item">The item to add.</param>
        public override sealed void Add(T item) {
            AddToBack(item);
        }

        /// <summary>
        /// Adds a collection of items to the back of the Deque. The indices of all existing items
        /// in the Deque are unchanged. The last item in the added collection becomes the
        /// last item in the Deque.
        /// </summary>
        /// <remarks>This method takes time O(M), where M is the number of items in the 
        /// <paramref name="collection"/>.</remarks>
        /// <param name="collection">The collection of item to add.</param>
        public void AddManyToBack(IEnumerable<T> collection) {
            collection.ShouldNotBeNull("collection");

            foreach(T item in collection)
                AddToBack(item);
        }

        /// <summary>
        /// Removes an item from the front of the Deque. The indices of all existing items
        /// in the Deque are decreased by 1. This method is 
        /// equivalent to <c>RemoveAt(0)</c> but is a little more
        /// efficient.
        /// </summary>
        /// <remarks>Removing an item from the front of the Deque takes
        /// a small constant amount of time, regardless of how many items are in the Deque.</remarks>
        /// <returns>The item that was removed.</returns>
        /// <exception cref="InvalidOperationException">The Deque is empty.</exception>
        public T RemoveFromFront() {
            Guard.Assert(_start != _end, Strings.CollectionIsEmpty);
            StopEnumerations();

            T item = _buffer[_start];

            _buffer[_start] = default(T);

            if(++_start >= _buffer.Length)
                _start -= _buffer.Length;

            return item;
        }

        /// <summary>
        /// Removes an item from the back of the Deque. The indices of all existing items
        /// in the Deque are unchanged. This method is 
        /// equivalent to <c>RemoveAt(Count-1)</c> but is a little more
        /// efficient.
        /// </summary>
        /// <remarks>Removing an item from the back of the Deque takes
        /// a small constant amount of time, regardless of how many items are in the Deque.</remarks>
        /// <exception cref="InvalidOperationException">The Deque is empty.</exception>
        public T RemoveFromBack() {
            Guard.Assert(_start != _end, Strings.CollectionIsEmpty);
            StopEnumerations();

            if(--_end < 0)
                _end += _buffer.Length;

            T item = _buffer[_end];
            _buffer[_end] = default(T);

            return item;
        }

        /// <summary>
        /// Retreives the item currently at the front of the Deque. The Deque is 
        /// unchanged. This method is 
        /// equivalent to <c>deque[0]</c> (except that a different exception is thrown).
        /// </summary>
        /// <remarks>Retreiving the item at the front of the Deque takes
        /// a small constant amount of time, regardless of how many items are in the Deque.</remarks>
        /// <returns>The item at the front of the Deque.</returns>
        /// <exception cref="InvalidOperationException">The Deque is empty.</exception>
        public T GetAtFront() {
            Guard.Assert(_start != _end, Strings.CollectionIsEmpty);

            return _buffer[_start];
        }

        /// <summary>
        /// Retreives the item currently at the back of the Deque. The Deque is 
        /// unchanged. This method is 
        /// equivalent to <c>deque[deque.Count - 1]</c> (except that a different exception is thrown).
        /// </summary>
        /// <remarks>Retreiving the item at the back of the Deque takes
        /// a small constant amount of time, regardless of how many items are in the Deque.</remarks>
        /// <returns>The item at the back of the Deque.</returns>
        /// <exception cref="InvalidOperationException">The Deque is empty.</exception>
        public T GetAtBack() {
            Guard.Assert(_start != _end, Strings.CollectionIsEmpty);

            return (_end == 0)
                       ? _buffer[_buffer.Length - 1]
                       : _buffer[_end - 1];
        }

        /// <summary>
        /// Creates a new Deque that is a copy of this one.
        /// </summary>
        /// <remarks>Copying a Deque takes O(N) time, where N is the number of items in this Deque..</remarks>
        /// <returns>A copy of the current deque.</returns>
        public Deque<T> Clone() {
            return new Deque<T>(this);
        }

#if !SILVERLIGHT
        /// <summary>
        /// Makes a deep clone of this Deque. A new Deque is created with a clone of
        /// each element of this set, by calling ICloneable.Clone on each element. If T is
        /// a value type, then each element is copied as if by simple assignment.
        /// </summary>
        /// <remarks><para>If T is a reference type, it must implement
        /// ICloneable. Otherwise, an InvalidOperationException is thrown.</para>
        /// <para>Cloning the Deque takes time O(N), where N is the number of items in the Deque.</para></remarks>
        /// <returns>The cloned Deque.</returns>
        /// <exception cref="InvalidOperationException">T is a reference type that does not implement ICloneable.</exception>
        public Deque<T> CloneContents() {
            bool itemIsValueType;

            if(Util.IsCloneableType(typeof(T), out itemIsValueType) == false)
                throw new InvalidOperationException(string.Format(Strings.TypeNotCloneable, typeof(T).FullName));

            var clone = new Deque<T>();

            // Clone each item, and add it to the new ordered set.
            foreach(var item in this) {
                T itemClone;

                if(itemIsValueType)
                    itemClone = item;
                else {
                    itemClone = Equals(item, null) ? default(T) : (T)(((ICloneable)item).Clone());
                }

                clone.AddToBack(itemClone);
            }

            return clone;
        }
#endif

        /// <summary>
        /// Print out the internal state of the Deque for debugging.
        /// </summary>
        internal void Print() {
            if(IsDebugEnabled) {
                log.Debug("length={0}  start={1}  end={2}", _buffer.Length, _start, _end);

                var builder = new StringBuilder();

                for(int i = 0; i < _buffer.Length; ++i) {
                    builder.Append(i == _start ? "start-> " : "        ");

                    builder.Append(i == _end ? "end-> " : "      ");
                    builder.AppendFormat("{0,4} {1}", i, _buffer[i]).AppendLine();
                }

                log.Debug(builder.ToString());
            }
        }
    }
}