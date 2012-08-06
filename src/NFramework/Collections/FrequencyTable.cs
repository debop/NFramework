using System;
using System.Collections;
using System.Collections.Generic;
using NSoft.NFramework.Reflections;

namespace NSoft.NFramework.Collections {

    #region Enums

    /// <summary>
    /// Enumeration defining the various orders of sorting
    /// </summary>
    public enum FrequencyTableSortOrder {
        /// <summary>
        /// 값을 기준으로 오름차순
        /// </summary>
        Value_Ascending,

        /// <summary>
        /// 값 기준의 내림차순
        /// </summary>
        Value_Descending,

        /// <summary>
        /// 빈도 기준의 오름차순
        /// </summary>
        Frequency_Ascending,

        /// <summary>
        /// 빈도 기준의 내림차순
        /// </summary>
        Frequency_Descending,

        /// <summary>
        /// 정렬 없음
        /// </summary>
        None
    }

    /// <summary>
    /// Enumeration defining the literal frequency analysis
    /// </summary>
    public enum TextAnalyzeMode {
        /// <summary>
        /// 모든 문자
        /// </summary>
        AllCharacters,

        /// <summary>
        /// 숫자 제외
        /// </summary>
        NoNumerals,

        /// <summary>
        /// 특수문자 제외
        /// </summary>
        NoSpecialCharacters,

        /// <summary>
        /// 글자만
        /// </summary>
        LettersOnly,

        /// <summary>
        /// 숫자만
        /// </summary>
        NumeralsOnly,

        /// <summary>
        /// 특수문자만
        /// </summary>
        SpecialCharactersOnly
    }

    #endregion

    /// <summary>
    /// A generic frequency table (빈도수 검사를 할때 유용한 Class입니다)
    /// </summary>
    /// <typeparam name="T">요수의 수형</typeparam>
    [Serializable]
    public class FrequencyTable<T> : IEnumerable<FrequencyTableEntry<T>> where T : IComparable<T> {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        #region << Fields >>

        // store the value and frequencies
        // it is possible to use a dictionary like this
        // Dictionary<T, FrequencyTableEntry<T>>
        private readonly Dictionary<T, int> _entries;
        // number of elements in _entries
        private int _length;
        // number of elements counted (actually the sample size)
        private int _count;
        // store the user-defined tag
        // store the description
        // highest frequency
        private int _high;
        // mode
        private T _mode;

        #endregion

        #region << Constructors >>

        /// <summary>
        /// Default constructor
        /// </summary>
        public FrequencyTable() {
            _entries = new Dictionary<T, int>();
            _length = 0;
            _count = 0;
            Description = "";
            Tag = null;
            _high = 0;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="initialCapacity">Initial buffer size</param>
        public FrequencyTable(int initialCapacity) {
            _entries = new Dictionary<T, int>(initialCapacity);
            _length = initialCapacity;
            _count = 0;
            Description = "";
            Tag = null;
            _high = 0;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="samples">요소 열거자</param>
        public FrequencyTable(IEnumerable<T> samples) : this() {
            foreach(T item in samples)
                Add(item);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="text">분석할 정보</param>
        /// <param name="mode">분석 모드</param>
        /// <exception cref="ArgumentException"><paramref name="text"/>가 문자열이 아닐 경우</exception>
        public FrequencyTable(T text, TextAnalyzeMode mode)
            : this() {
            if(typeof(string).IsInstanceOfType(text) == false)
                throw new ArgumentException("text argument must be string.", "text");

            AnalyseString(text, mode);
        }

        #endregion

        #region << Properties >>

        /// <summary>
        /// actual number of entries
        /// </summary>
        public int Length {
            get { return _length; }
        }

        /// <summary>
        /// the sample size
        /// </summary>
        public int SampleSize {
            get { return _count; }
        }

        /// <summary>
        /// user defined tag
        /// </summary>
        public object Tag { get; set; }

        /// <summary>
        /// Description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Returns the scarcest value ( actually the first occurence of the lowest frequency is considered )
        /// </summary>
        public T ScarcestValue {
            get {
                // the largest possible frequency is _count
                int f = _count + 1;
                var v = default(T);

                foreach(T item in _entries.Keys) {
                    if(_entries[item] < f) {
                        v = item;
                        f = _entries[item];
                    }
                }
                return v;
            }
        }

        /// <summary>
        /// return the most Frequency value
        /// </summary>
        public T Mode {
            get { return _mode; }
        }

        /// <summary>
        /// return the highest observed frequency
        /// </summary>
        public int HighestFrequency {
            get { return _high; }
        }

        /// <summary>
        /// return the lowest observed frequency
        /// </summary>
        public int SmallestFrequency {
            get {
                var f = GetTableAsArray(FrequencyTableSortOrder.Frequency_Ascending);
                return f[0].AbsoluteFrequency;
            }
        }

        #endregion

        #region << Private Methods >>

        private void AnalyseString(T text, TextAnalyzeMode mode) {
            // character strings
            const string SpecialChars = @"""!?%&/()=?@<>|?.;:-_#'*+~껙 ";
            const string Numbers = "0123456789";

            // Adding the entries according to mode
            switch(mode) {
                case TextAnalyzeMode.AllCharacters:
                    foreach(char v in text.ToString())
                        Add((T)Convert.ChangeType(v, text.GetType(), null));
                    break;

                case TextAnalyzeMode.LettersOnly:
                    foreach(var v in text.ToString()) {
                        if((SpecialChars.IndexOf(v) == -1) & (Numbers.IndexOf(v) == -1))
                            Add((T)Convert.ChangeType(v, text.GetType(), null));
                    }
                    break;
                case TextAnalyzeMode.NoNumerals:
                    foreach(var v in text.ToString()) {
                        if(Numbers.IndexOf(v) == -1)
                            Add((T)Convert.ChangeType(v, text.GetType(), null));
                    }
                    break;
                case TextAnalyzeMode.NoSpecialCharacters:
                    foreach(var v in text.ToString()) {
                        if(SpecialChars.IndexOf(v) == -1)
                            Add((T)Convert.ChangeType(v, text.GetType(), null));
                    }
                    break;
                case TextAnalyzeMode.NumeralsOnly:
                    foreach(var v in text.ToString()) {
                        if(Numbers.IndexOf(v) != -1)
                            Add((T)Convert.ChangeType(v, text.GetType(), null));
                    }
                    break;
                case TextAnalyzeMode.SpecialCharactersOnly:
                    foreach(var v in text.ToString()) {
                        if(SpecialChars.IndexOf(v) != -1)
                            Add((T)Convert.ChangeType(v, text.GetType(), null));
                    }
                    break;
            }
        }

        #endregion

        #region << Public Methods >>

        /// <summary>
        /// Add the specified value.
        /// </summary>
        /// <param name="value">value to add.</param>
        public void Add(T value) {
            if(_entries.ContainsKey(value)) {
                // update the frequency
                _entries[value]++;
                // update mode and highest frequency
                if(_entries[value] > _high) {
                    _high = _entries[value];
                    _mode = value;
                }
                // add 1 to sample size
                _count++;
            }
            else {
                // add a new entry - frequency is one
                _entries.Add(value, 1);
                // add 1 to table length
                _length++;
                // add 1 to sample size
                _count++;
            }
        }

        /// <summary>
        /// Add text to analyzed
        /// </summary>
        /// <param name="text">text to analyzed</param>
        /// <param name="mode">analyze mode</param>
        public void Add(T text, TextAnalyzeMode mode) {
            if(typeof(T) != typeof(string))
                throw new ArgumentException();

            AnalyseString(text, mode);
        }

        /// <summary>
        /// Remove the specified value
        /// </summary>
        /// <param name="value">value to remove.</param>
        /// <exception cref="InvalidOperationException">value not exists.</exception>
        public void Remove(T value) {
            if(_entries.ContainsKey(value)) {
                // Update length and sample size
                _count = _count - _entries[value];
                _length--;
                // Remove the entry
                _entries.Remove(value);
            }
            else
                throw new InvalidOperationException();
        }

        /// <summary>
        /// Get array of frequency table entry
        /// </summary>
        /// <returns></returns>
        public FrequencyTableEntry<T>[] GetTableAsArray() {
            var _output = new FrequencyTableEntry<T>[_length];
            int i = 0;
            foreach(FrequencyTableEntry<T> entry in this) {
                _output[i] = entry;
                i++;
            }
            return _output;
        }

        ///<summary>
        /// Get array of frequency table entry
        ///</summary>
        ///<param name="order"></param>
        ///<returns></returns>
        public FrequencyTableEntry<T>[] GetTableAsArray(FrequencyTableSortOrder order) {
            FrequencyTableEntry<T>[] _output = null;
            switch(order) {
                case FrequencyTableSortOrder.None:
                    _output = GetTableAsArray();
                    break;
                case FrequencyTableSortOrder.Frequency_Ascending:
                    _output = SortFrequencyTable<T>.SortTable(GetTableAsArray(),
                                                              FrequencyTableSortOrder.Frequency_Ascending);
                    break;
                case FrequencyTableSortOrder.Frequency_Descending:
                    _output = SortFrequencyTable<T>.SortTable(GetTableAsArray(),
                                                              FrequencyTableSortOrder.Frequency_Descending);
                    break;
                case FrequencyTableSortOrder.Value_Ascending:
                    _output = SortFrequencyTable<T>.SortTable(GetTableAsArray(), FrequencyTableSortOrder.Value_Ascending);
                    break;
                case FrequencyTableSortOrder.Value_Descending:
                    _output = SortFrequencyTable<T>.SortTable(GetTableAsArray(),
                                                              FrequencyTableSortOrder.Value_Descending);
                    break;
            }
            return _output;
        }

        #endregion

        #region IEnumerable<FrequencyTableEntry<T>> Members

        ///<summary>
        ///Returns an enumerator that iterates through the collection.
        ///</summary>
        ///
        ///<returns>
        ///A <see cref="T:System.Collections.Generic.IEnumerator`1"></see> that can be used to iterate through the collection.
        ///</returns>
        public IEnumerator<FrequencyTableEntry<T>> GetEnumerator() {
            foreach(T key in _entries.Keys) {
                var int_f = _entries[key];
                var dbl_f = (double)int_f;

                // fill the structure
                yield return new FrequencyTableEntry<T>(key, int_f, dbl_f / _count, dbl_f / _count * 100.0);
            }
        }

        #endregion

        #region IEnumerable Members

        ///<summary>
        ///Returns an enumerator that iterates through a collection.
        ///</summary>
        ///
        ///<returns>
        ///An <see cref="T:System.Collections.IEnumerator"></see> object that can be used to iterate through the collection.
        ///</returns>
        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        #endregion

        #region << SortFrequencyTable<K> >>

        /// <summary>
        /// Implements the Quicksort-Algorithm
        /// </summary>
        private class SortFrequencyTable<K> where K : IComparable<K> {
            /// <summary>
            /// the array to sort
            /// </summary>
            private static FrequencyTableEntry<K>[] a;

            /// <summary>
            /// Sorts a FrequencyTableEntry-Array using a quicksort algorithm
            /// </summary>
            /// <param name="input">The Array to sort</param>
            /// <param name="order">The sort order</param>
            /// <returns>The sorted array</returns>
            public static FrequencyTableEntry<K>[] SortTable(FrequencyTableEntry<K>[] input,
                                                             FrequencyTableSortOrder order) {
                a = new FrequencyTableEntry<K>[input.Length];
                input.CopyTo(a, 0);
                switch(order) {
                    case FrequencyTableSortOrder.Value_Ascending:
                        SortByValueAscending(0, a.Length - 1);
                        break;
                    case FrequencyTableSortOrder.Value_Descending:
                        SortByValueDescending(0, a.Length - 1);
                        break;
                    case FrequencyTableSortOrder.Frequency_Ascending:
                        SortByFrequencyAscending(0, a.Length - 1);
                        break;
                    case FrequencyTableSortOrder.Frequency_Descending:
                        SortByFrequencyDescending(0, a.Length - 1);
                        break;
                    case FrequencyTableSortOrder.None:
                        break;
                }
                return a;
            }

            /// <summary>
            /// The Quicksort-Method
            /// </summary>
            /// <param name="l">lower bound</param>
            /// <param name="u">upper bound</param>
            private static void SortByValueAscending(int l, int u) {
                int i = l;
                int j = u;
                var v = a[(l + u) / 2].Value;
                while(i <= j) {
                    while(a[i].Value.CompareTo(v) < 0)
                        i++;
                    while(a[j].Value.CompareTo(v) > 0)
                        j--;
                    if(i <= j) {
                        Swap(i, j);
                        i++;
                        j--;
                    }
                }
                if(l < j)
                    SortByValueAscending(l, j);
                if(i < u)
                    SortByValueAscending(i, u);
            }

            /// <summary>
            /// The Quicksort-Method
            /// </summary>
            /// <param name="l">lower bound</param>
            /// <param name="u">upper bound</param>
            private static void SortByValueDescending(int l, int u) {
                int i = l;
                int j = u;
                var v = a[(l + u) / 2].Value;
                while(i <= j) {
                    while(a[i].Value.CompareTo(v) > 0)
                        i++;
                    while(a[j].Value.CompareTo(v) < 0)
                        j--;
                    if(i <= j) {
                        Swap(i, j);
                        i++;
                        j--;
                    }
                }
                if(l < j)
                    SortByValueDescending(l, j);
                if(i < u)
                    SortByValueDescending(i, u);
            }

            /// <summary>
            /// The Quicksort-Method
            /// </summary>
            /// <param name="l">lower bound</param>
            /// <param name="u">upper bound</param>
            private static void SortByFrequencyAscending(int l, int u) {
                int i = l;
                int j = u;
                int v = a[(l + u) / 2].AbsoluteFrequency;
                while(i <= j) {
                    while(a[i].AbsoluteFrequency < v)
                        i++;
                    while(a[j].AbsoluteFrequency > v)
                        j--;
                    if(i <= j) {
                        Swap(i, j);
                        i++;
                        j--;
                    }
                }
                if(l < j)
                    SortByFrequencyAscending(l, j);
                if(i < u)
                    SortByFrequencyAscending(i, u);
            }

            /// <summary>
            /// The Quicksort-Method
            /// </summary>
            /// <param name="l">lower bound</param>
            /// <param name="u">upper bound</param>
            private static void SortByFrequencyDescending(int l, int u) {
                int i = l;
                int j = u;
                int v = a[(l + u) / 2].AbsoluteFrequency;
                while(i <= j) {
                    while(a[i].AbsoluteFrequency > v)
                        i++;
                    while(a[j].AbsoluteFrequency < v)
                        j--;
                    if(i <= j) {
                        Swap(i, j);
                        i++;
                        j--;
                    }
                }
                if(l < j)
                    SortByFrequencyDescending(l, j);
                if(i < u)
                    SortByFrequencyDescending(i, u);
            }

            /// <summary>
            /// Swaps two array-elements
            /// </summary>
            /// <param name="i">First element</param>
            /// <param name="j">Second element</param>
            private static void Swap(int i, int j) {
                var temp = a[i];
                a[i] = a[j];
                a[j] = temp;
            }
        }

        #endregion

        /// <summary>
        /// Return informatio of this instance.
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            return "FrequencyTable# " + this.CollectionToString();
        }
    }
}