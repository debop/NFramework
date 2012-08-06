using System;
using System.Collections;
using NSoft.NFramework.Reflections;

namespace NSoft.NFramework.Collections {
    /// <summary>
    /// Pascal Language에 있는 Set을 구현한 클래스입니다.
    /// </summary>
    /// <remarks>
    /// int, char 형의 집합을 구현한 것이다. enum의 [flags] attribute를 이용한 bit 연산을 수행해도 되지만,
    /// Delphi 의 Set 예약어처럼 편리하게 사용할 수 있다.
    /// </remarks>
    /// <example>
    /// <code>
    /// // alphabet 만
    /// PascalSet alphabetSet = new PascalSet('A', 'z'); // asciiSet.Union(chars.ToArray());
    /// // 모음
    /// PascalSet vowels = alphabetSet.Union('A', 'a', 'E', 'e', 'I', 'i', 'O', 'o', 'U', 'u');
    /// // 자음
    /// PascalSet consonants = vowels.Complement();
    /// 
    /// string contents = "Hey, realweb members. make money please.";
    /// 
    /// int contentLength = contents.Length;
    /// int vowelCount =0;
    /// int consonantCount = 0;
    /// int otherCount = 0;
    /// 
    /// for (int i = 0; i &lt; contentLength; i++)
    /// {
    /// 	char c = contents[i];
    /// 
    /// 	if (vowels.ContainsElement(c))              // 모음
    /// 		vowelCount++;
    /// 	else if (consonants.ContainsElement(c))     // 자음
    /// 		consonantCount++;
    /// 	else                                        // 기타
    /// 		otherCount++;
    /// }
    /// Console.Write("주어진 문장에는 {0}개의 모음, {1}개의 자음, {2}개의 비 알파벳 문자가 있습니다.", vowelCount, consonantCount, otherCount);
    /// 
    /// </code>  
    /// </example>
    [Serializable]
    public class PascalSet : ICollection {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// 지정된 두 PascalSet의 집합 범위(LowerBound, UpperBound)가 같은지 검사한다. (Same이, Equivalent, Equal과는 다르다)
        /// </summary>
        public static bool AreSimilar(PascalSet set1, PascalSet set2) {
            return (set1.LowerBound == set2.LowerBound) &&
                   (set1.UpperBound == set2.UpperBound);
        }

        /// <summary>
        /// 지정된 두 집합의 집합 범위가 같은지 검사한다.
        /// </summary>
        /// <exception cref="ArgumentException">두 집합이 같은 종류가 아닐 때</exception>
        public static void CheckSimilar(PascalSet set1, PascalSet set2) {
            set1.ShouldNotBeNull("set1");
            set2.ShouldNotBeNull("set2");

            if(!AreSimilar(set1, set2))
                throw new ArgumentException(SR.ErrNotSameType);
        }

        private readonly int _lowerBound;
        private readonly int _upperBound;

        // NOTE: 집합 범위를 만든다. 포함 여부를 Bit로 처리한다. 이렇게 하면 속도가 빠르다.
        private readonly BitArray _bitData;

        /// <summary>
        /// Initialize a new instance of PascalSet with lowerBound, upperBound
        /// </summary>
        /// <param name="lowerBound">하한값</param>
        /// <param name="upperBound">상한값</param>
        public PascalSet(int lowerBound, int upperBound) {
            if(IsDebugEnabled)
                log.Debug("Initialize a new instance of PascalSet with lowerBound=[{0}], upperBound=[{1}]",
                          lowerBound, upperBound);

            if(lowerBound > upperBound) {
                _lowerBound = upperBound;
                _upperBound = lowerBound;
            }
            else {
                _lowerBound = lowerBound;
                _upperBound = upperBound;
            }

            // 집합 범위를 만든다. 포함 여부를 Bit로 처리한다.
            _bitData = new BitArray(_upperBound - _lowerBound + 1);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="lowerBound">Lower bound.</param>
        /// <param name="upperBound">Upper bound.</param>
        public PascalSet(char lowerBound, char upperBound)
            : this(lowerBound, (int)upperBound) {}

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="lowerBound">Lower bound.</param>
        /// <param name="upperBound">Upper bound.</param>
        /// <param name="elements">element of this instance</param>
        public PascalSet(char lowerBound, char upperBound, char[] elements)
            : this(lowerBound, (int)upperBound) {
            AddElements(elements);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="lowerBound">Lower bound.</param>
        /// <param name="upperBound">Upper bound.</param>
        /// <param name="elements">element of this instance</param>
        public PascalSet(int lowerBound, int upperBound, int[] elements)
            : this(lowerBound, upperBound) {
            AddElements(elements);
        }

        /// <summary>
        /// Constructor for copying
        /// </summary>
        /// <param name="set">instance to copied</param>
        public PascalSet(PascalSet set) {
            set.ShouldNotBeNull("set");

            _lowerBound = set._lowerBound;
            _upperBound = set._upperBound;
            _bitData = new BitArray(set._bitData);
        }

        /// <summary>
        /// 하한
        /// </summary>
        public int LowerBound {
            get { return _lowerBound; }
        }

        /// <summary>
        /// 상한
        /// </summary>
        public int UpperBound {
            get { return _upperBound; }
        }

        /// <summary>
        /// 현재 집합에 지정된 배열 값들을 요소로 추가한다.
        /// </summary>
        /// <param name="values"></param>
        public virtual void AddElements(params int[] values) {
            if(values == null || values.Length == 0)
                return;

            SetElements(true, values);
        }

        /// <summary>
        /// Add elements to this instance.
        /// </summary>
        /// <param name="values">array of element to added.</param>
        public virtual void AddElements(params char[] values) {
            if(values == null || values.Length == 0)
                return;

            SetElements(true, values);
        }

        /// <summary>
        /// 현재 집합에 지정된 배열 값들을 요소에서 제거한다.
        /// </summary>
        /// <param name="values">array of element to removed.</param>
        public virtual void RemoveElements(params int[] values) {
            if(values == null || values.Length == 0)
                return;

            SetElements(false, values);
        }

        /// <summary>
        /// 현재 집합에 지정된 배열 값들을 요소에서 제거한다.
        /// </summary>
        /// <param name="values">array of element to removed.</param>
        public virtual void RemoveElements(params char[] values) {
            if(values == null || values.Length == 0)
                return;

            SetElements(false, values);
        }

        /// <summary>
        /// 현재 집합에 지정된 값들을 요소로 포함 시키거나, 제외시킨다.
        /// </summary>
        /// <param name="contains">포함시킬 것인가, 제외시킬 것인가</param>
        /// <param name="values">array of elements to set</param>
        protected virtual void SetElements(bool contains, params int[] values) {
            foreach(int value in values) {
                if(value >= _lowerBound && value <= _upperBound)
                    _bitData.Set(value - _lowerBound, contains);
                else
                    throw new ArgumentOutOfRangeException(
                        string.Format(SR.ErrValueIsOutOfRange,
                                      value, _lowerBound, _upperBound));
            }
        }

        /// <summary>
        /// 현재 집합에 지정된 값들을 요소로 포함 시키거나, 제외시킨다.
        /// </summary>
        /// <param name="contains">포함시킬 것인가, 제외시킬 것인가</param>
        /// <param name="values">array of elements to set</param>
        protected virtual void SetElements(bool contains, params char[] values) {
            foreach(char c in values) {
                int value = c;

                if(value >= _lowerBound && value <= _upperBound)
                    _bitData.Set(value - _lowerBound, contains);
                else
                    throw new ArgumentOutOfRangeException(
                        string.Format(SR.ErrValueIsOutOfRange,
                                      value, _lowerBound, _upperBound));
            }
        }

        /// <summary>
        /// 현재 집합이 지정한 값을 요소로 가지고 있는지 검사한다.
        /// </summary>
        public bool ContainsElement(int x) {
            if(x < _lowerBound || x > _upperBound)
                return false;

            return _bitData.Get(x - _lowerBound);
        }

        /// <summary>
        /// 현재 집합이 지정한 값을 요소로 가지고 있는지 검사한다.
        /// </summary>
        public bool ContainsElement(char x) {
            return ContainsElement((int)x);
        }

        /// <summary>
        /// 현재 집합과 지정된 집합의 합집합을 만든다
        /// </summary>
        /// <param name="set">집합</param>
        public virtual PascalSet Union(PascalSet set) {
            if(set == null)
                return Clone();

            CheckSimilar(this, set);

            var result = Clone();
            result._bitData.Or(set._bitData);

            return result;
        }

        /// <summary>
        /// 현재 집합과 지정된 배열의 값들을 요소로 가지는 집합의 합집합을 만든다.
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public virtual PascalSet Union(params int[] values) {
            return Union(new PascalSet(_lowerBound, _upperBound, values));
        }

        /// <summary>
        /// 현재 집합과 지정된 배열의 값들을 요소로 가지는 집합의 합집합을 만든다.
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public virtual PascalSet Union(params char[] values) {
            return Union(new PascalSet((char)_lowerBound, (char)_upperBound, values));
        }

        /// <summary>
        /// 합집합 연산자 (OR)
        /// </summary>
        public static PascalSet operator |(PascalSet lhs, PascalSet rhs) {
            if(lhs != null)
                return lhs.Union(rhs);

            if(rhs != null)
                return rhs;

            return null;
        }

        /// <summary>
        /// 현재 집합과 지정된 집합의 교집합을 만든다.
        /// </summary>
        public virtual PascalSet Intersection(PascalSet set) {
            // 요소가 없는 빈 집합을 반환한다.
            if(set == null)
                return new PascalSet(_lowerBound, _upperBound);

            CheckSimilar(this, set);

            var result = Clone();
            result._bitData.And(set._bitData);

            return result;
        }

        /// <summary>
        /// 현재 집합과 지정된 값을 요소로 가진 집합의 교집합을 구한다.
        /// </summary>
        public virtual PascalSet Intersection(params int[] values) {
            return Intersection(new PascalSet(_lowerBound, _upperBound, values));
        }

        /// <summary>
        /// 현재 집합과 지정된 값을 요소로 가진 집합의 교집합을 구한다.
        /// </summary>
        public virtual PascalSet Intersection(params char[] values) {
            return Intersection(new PascalSet((char)_lowerBound, (char)_upperBound, values));
        }

        /// <summary>
        /// 교집합 연산자 (AND)
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static PascalSet operator &(PascalSet lhs, PascalSet rhs) {
            if(lhs != null)
                return lhs.Intersection(rhs);

            if(rhs != null)
                return rhs.Intersection(lhs);

            return null;
        }

        /// <summary>
        /// 현재 집합에서 지정된 집합의 요소를 뺀 집합 (차집합) 을 만든다.
        /// </summary>
        public virtual PascalSet Difference(PascalSet set) {
            if(set == null)
                return Clone();

            CheckSimilar(this, set);

            var result = Clone();

            result._bitData.Xor(set._bitData).And(_bitData);

            return result;
        }

        /// <summary>
        ///  현재 집합에서 지정된 배열 값을 뺀 차집합을 만든다.
        /// </summary>
        public virtual PascalSet Difference(params int[] values) {
            return Difference(new PascalSet(_lowerBound, _upperBound, values));
        }

        /// <summary>
        ///  현재 집합에서 지정된 배열 값을 뺀 차집합을 만든다.
        /// </summary>
        public virtual PascalSet Difference(params char[] values) {
            return Difference(new PascalSet((char)_lowerBound, (char)_upperBound, values));
        }

        /// <summary>
        /// 차집합 연산자
        /// </summary>
        public static PascalSet operator -(PascalSet lhs, PascalSet rhs) {
            if(lhs != null)
                return lhs.Difference(rhs);

            return null;
        }

        /// <summary>
        /// 현재 집합의 여집합을 만든다.
        /// </summary>
        /// <returns>인스턴스의 여집합</returns>
        public PascalSet Complement() {
            var result = Clone();
            result._bitData.Not();

            return result;
        }

        /// <summary>
        /// Complement(여집합) 연산자
        /// </summary>
        /// <param name="lhs"></param>
        /// <returns></returns>
        public static PascalSet operator !(PascalSet lhs) {
            if(lhs != null)
                return lhs.Complement();

            return null;
        }

        /// <summary>
        /// 현재 집합과 지정된 집합의 XOR 연산을 수행한다. ( XOR = (A | B) - ( A &amp; B) )
        /// </summary>
        public virtual PascalSet ExclusiveOr(PascalSet set) {
            if(set == null)
                return Clone();

            CheckSimilar(this, set);

            var result = Clone();
            result._bitData.Xor(set._bitData);

            return result;
        }

        /// <summary>
        /// 현재 집합과 지정된 값들의 집합의 XOR 연산을 수행한다.
        /// </summary>
        public virtual PascalSet ExclusiveOr(params int[] values) {
            return ExclusiveOr(new PascalSet(_lowerBound, _upperBound, values));
        }

        /// <summary>
        /// 현재 집합과 지정된 값들의 집합의 XOR 연산을 수행한다.
        /// </summary>
        public virtual PascalSet ExclusiveOr(params char[] values) {
            return ExclusiveOr(new PascalSet((char)_lowerBound, (char)_upperBound, values));
        }

        /// <summary>
        /// XOR 연산자 ( XOR = (A | B) - ( A &amp; B) )
        /// </summary>
        /// <param name="lhs">집합</param>
        /// <param name="rhs">집합</param>
        /// <returns>두 집합의 XOR 결과 집합</returns>
        public static PascalSet operator ^(PascalSet lhs, PascalSet rhs) {
            if(lhs != null)
                return lhs.ExclusiveOr(rhs);

            if(rhs != null)
                return rhs.ExclusiveOr(lhs);

            return null;
        }

        /// <summary>
        /// 현재 집합이 지정된 집합의 부분집합인지 판단한다. (두 집합이 일치해도 부분집합이다)
        /// </summary>
        public virtual bool IsSubset(PascalSet set) {
            if(set == null)
                return false;

            CheckSimilar(this, set);

            // bit 로 포함되어 있기 때문
            const int INT_SiZE = 32;
            int arraySize = (_bitData.Length + INT_SiZE - 1) / INT_SiZE;

            var thisBits = new int[arraySize];
            var setBits = new int[arraySize];

            _bitData.CopyTo(thisBits, 0);
            set._bitData.CopyTo(setBits, 0);

            for(int i = 0; i < thisBits.Length; i++) {
                // 현재 집합의 원소가 지정된 집합의 원소가 아니라면 부분집합이 아니다.
                if((thisBits[i] & setBits[i]) != thisBits[i])
                    return false;
            }

            return true;
        }

        /// <summary>
        /// 현재 집합이 지정된 값을 가진 집합의 부분집합인지 판단한다. (두 집합이 일치해도 부분집합이다)
        /// </summary>
        public virtual bool IsSubset(params int[] values) {
            return IsSubset(new PascalSet(_lowerBound, _upperBound, values));
        }

        /// <summary>
        /// 현재 집합이 지정된 값을 가진 집합의 부분집합인지 판단한다. (두 집합이 일치해도 부분집합이다)
        /// </summary>
        public virtual bool IsSubset(params char[] values) {
            return IsSubset(new PascalSet((char)_lowerBound, (char)_upperBound, values));
        }

        /// <summary>
        /// 현재 집합이 지정된 집합의 참부분집합인지 판단한다. (두집합이 동치이면 False이다)
        /// </summary>
        public virtual bool IsProperSubset(PascalSet set) {
            if(set == null)
                return false;

            CheckSimilar(this, set);

            return IsSubset(set) && (Count < set.Count);
        }

        /// <summary>
        /// 현재 집합이 지정된 집합의 참부분집합인지 판단한다. (두집합이 동치이면 False이다)
        /// </summary>
        public virtual bool IsProperSubset(params int[] values) {
            return IsProperSubset(new PascalSet(_lowerBound, _upperBound, values));
        }

        /// <summary>
        /// 현재 집합이 지정된 집합의 참부분집합인지 판단한다. (두집합이 동치이면 False이다)
        /// </summary>
        public virtual bool IsProperSubset(params char[] values) {
            return IsProperSubset(new PascalSet((char)_lowerBound, (char)_upperBound, values));
        }

        /// <summary>
        /// 현재 집합이 지정된 집합의 모집합인지 판단한다.
        /// </summary>
        public virtual bool IsSuperset(PascalSet set) {
            CheckSimilar(this, set);

            return set.IsSubset(this);
        }

        /// <summary>
        /// 현재 집합이 지정된 값을 가진 집합의 모집합인지 판단한다.
        /// </summary>
        public virtual bool IsSuperset(params int[] values) {
            return IsSuperset(new PascalSet(_lowerBound, _upperBound, values));
        }

        /// <summary>
        /// 현재 집합이 지정된 값을 가진 집합의 모집합인지 판단한다.
        /// </summary>
        public virtual bool IsSuperset(params char[] values) {
            return IsSuperset(new PascalSet((char)_lowerBound, (char)_upperBound, values));
        }

        /// <summary>
        /// 현재 집합이 지정된 집합의 순모집합인지 판단한다. (두집합이 동치이면 False이다)
        /// </summary>
        public virtual bool IsProperSuperset(PascalSet set) {
            CheckSimilar(this, set);
            return set.IsSubset(this) && (Count > set.Count);
        }

        /// <summary>
        /// 현재 집합이 지정된 값을 가진 집합의 순모집합인지 판단한다. (두집합이 동치이면 False이다)
        /// </summary>
        public virtual bool IsProperSuperset(params int[] values) {
            return IsProperSuperset(new PascalSet(_lowerBound, _upperBound, values));
        }

        /// <summary>
        /// 현재 집합이 지정된 값을 가진 집합의 순모집합인지 판단한다. (두집합이 동치이면 False이다)
        /// </summary>
        public virtual bool IsProperSuperset(params char[] values) {
            return IsProperSuperset(new PascalSet((char)_lowerBound, (char)_upperBound, values));
        }

        /// <summary>
        /// 현재 집합과 지정된 집합이 같은지 검사한다.
        /// </summary>
        /// <param name="set"></param>
        /// <returns></returns>
        public bool Equals(PascalSet set) {
            if(set == null)
                return false;

            return (IsSubset(set) && (set.Count == Count));
        }

        /// <summary>
        /// 인스턴스를 복제한다.
        /// </summary>
        /// <returns></returns>
        public PascalSet Clone() {
            return new PascalSet(this);
        }

        /// <summary>
        /// 현재 집합을 문자열로 표현한다.
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            return this.CollectionToString();
        }

        /// <summary>
        /// 현재 집합의 요소들을 지정된 배열에 복사한다.
        /// </summary>
        /// <param name="array">요소가 저장된 대상 배열</param>
        /// <param name="startIndex">배열의 시작 인덱스</param>
        public void CopyTo(Array array, int startIndex) {
            array.ShouldNotBeNull("array");

            for(int i = _lowerBound; i <= _upperBound; i++)
                if(_bitData.Get(i))
                    array.SetValue(i, startIndex++);
        }

        /// <summary>
        /// 현재 집합의 요소 수
        /// </summary>
        public int Count {
            get {
                int count = 0;

                for(int i = 0; i < _bitData.Length; i++)
                    if(_bitData.Get(i))
                        count++;

                return count;
            }
        }

        /// <summary>
        /// Indicate that this instance is synchronized.
        /// </summary>
        public bool IsSynchronized {
            get { return false; }
        }

        /// <summary>
        /// synchronized root object.
        /// </summary>
        public object SyncRoot {
            get { return this; }
        }

        /// <summary>
        /// 집합 요소의 반복자
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetEnumerator() {
            // 중간중간 비어 있을 수 있으므로 예상되는 값을 넘어서면 중단한다.
            //
            int totalElements = Count;
            int itemsRequired = 0;

            for(int i = 0; i < _bitData.Length || itemsRequired < totalElements; i++) {
                if(_bitData.Get(i)) {
                    itemsRequired++;
                    yield return i + _lowerBound;
                }
            }
        }
    }
}