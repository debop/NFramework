using System;
using System.Collections;
using System.Collections.Generic;
using NSoft.NFramework.Reflections;

namespace NSoft.NFramework.Collections {
    /// <summary>
    /// 무한 집합에 대한 클래스이다.
    /// </summary>
    /// <remarks>
    /// 집합 연산을 제공하는 컬렉션이다.
    /// </remarks>
    /// <typeparam name="T">요소 타입</typeparam>
    [Serializable]
    public class Set<T> : ICollection<T>, IEnumerable {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private readonly List<T> _internalList = new List<T>();

        #region << Constructors >>

        /// <summary>
        /// 기본 생성자
        /// </summary>
        public Set() {}

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="items">요소들</param>
        public Set(params T[] items) {
            if(items != null)
                _internalList.AddRange(items);
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="collection">요소 컬렉션</param>
        public Set(IEnumerable<T> collection) {
            if(collection != null)
                _internalList.AddRange(collection);
        }

        #endregion

        /// <summary>
        /// 요소 수
        /// </summary>
        public int Count {
            get { return _internalList.Count; }
        }

        /// <summary>
        /// 빈 집합 여부
        /// </summary>
        public bool IsEmpty {
            get { return (Count == 0); }
        }

        /// <summary>
        /// Indexer
        /// </summary>
        /// <param name="index">위치 인덱스</param>
        /// <returns>요소</returns>
        public T this[int index] {
            get { return _internalList[index]; }
        }

        /// <summary>
        /// 요소 추가 - 중복된 요소는 무시한다.
        /// </summary>
        /// <param name="item">요소</param>
        public void Add(T item) {
            if(Contains(item) == false)
                _internalList.Add(item);
        }

        /// <summary>
        /// 요소들을 추가한다. - 중복은 무시한다.
        /// </summary>
        /// <param name="items"></param>
        public void Add(params T[] items) {
            if(items != null)
                foreach(T item in items)
                    Add(item);
        }

        /// <summary>
        /// 지정된 컬렉션의 요소들을 추가한다.
        /// </summary>
        /// <param name="collection"></param>
        public void AddRange(IEnumerable<T> collection) {
            if(collection != null)
                foreach(T item in collection)
                    Add(item);
        }

        /// <summary>
        /// 모든 요소를 제거한다.
        /// </summary>
        public void Clear() {
            _internalList.Clear();
        }

        /// <summary>
        /// 요소 존재 여부
        /// </summary>
        public bool Contains(T item) {
            return _internalList.Contains(item);
        }

        /// <summary>
        /// 지정된 요소들을 모두 가지고 있는지 검사한다.
        /// </summary>
        /// <param name="items"></param>
        /// <returns>인스턴스가 지정된 모든 요소를 가지고 있으면 true, 아니면 false</returns>
        public bool ContainsRange(params T[] items) {
            if(items == null)
                return false;

            foreach(T item in items)
                if(Contains(item) == false)
                    return false;

            return true;
        }

        /// <summary>
        /// 현재 집합이 지정된 컬렉션의 모든 요소를 가지고 있는지 검사.
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public bool ContainsRange(IEnumerable<T> collection) {
            if(collection == null)
                return false;

            foreach(T item in collection)
                if(Contains(item) == false)
                    return false;

            return true;
        }

        /// <summary>
        /// 요소 제거
        /// </summary>
        public bool Remove(T item) {
            return _internalList.Remove(item);
        }

        /// <summary>
        /// 요소들을 제거한다.
        /// </summary>
        /// <param name="items"></param>
        public void RemoveRange(params T[] items) {
            if(items == null)
                return;

            foreach(T item in items)
                if(Contains(item))
                    Remove(item);
        }

        /// <summary>
        /// 현재 집합에서 지정된 컬렉션의 요소들을 제거한다.
        /// </summary>
        /// <param name="collection"></param>
        public void RemoveRange(IEnumerable<T> collection) {
            if(collection == null)
                return;

            foreach(T item in collection)
                if(Contains(item))
                    Remove(item);
        }

        /// <summary>
        /// 현재 집합에서 지정된 컬렉션의 요소만 남겨두고 다 제거한다. - 교집합
        /// </summary>
        /// <param name="collection">요소 컬렉션</param>
        public void RetainRange(IEnumerable<T> collection) {
            if(collection == null) {
                _internalList.Clear();
            }
            else {
                var tmpSet = new Set<T>(collection);

                foreach(T item in this)
                    if(!tmpSet.Contains(item))
                        Remove(item);
            }
        }

        /// <summary>
        /// 요소 정렬
        /// </summary>
        public void Sort() {
            _internalList.Sort();
        }

        /// <summary>
        /// 요소 정렬
        /// </summary>
        /// <param name="comparer"></param>
        public void Sort(IComparer<T> comparer) {
            _internalList.Sort(comparer);
        }

        /// <summary>
        /// 요소를 1차원 배열로 만들어 반환한다.
        /// </summary>
        /// <returns>요소의 1차원 배열</returns>
        public virtual T[] ToArray() {
            return _internalList.ToArray();
        }

        #region Union (합집합)

        /// <summary>
        /// 합집합
        /// </summary>
        /// <remarks>현 집합과 지정한 집합의 합집합을 만든다.</remarks>
        public Set<T> Union(Set<T> set) {
            var resultSet = Clone();

            if(set != null && set.Count > 0)
                resultSet.AddRange(set);

            return resultSet;
        }

        /// <summary>
        /// 현 집합과 지정된 요소들의 집합의 합집합을 만든다.
        /// </summary>
        /// <param name="items">요소들</param>
        /// <returns>합집합</returns>
        public Set<T> Union(params T[] items) {
            if(items == null || items.Length == 0)
                return Clone();

            return Union(new Set<T>(items));
        }

        /// <summary>
        /// 합집합 연산자
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static Set<T> operator |(Set<T> lhs, Set<T> rhs) {
            if(lhs != null)
                return lhs.Union(rhs);

            if(rhs != null)
                return rhs.Union(lhs);

            return new Set<T>();
        }

        #endregion

        #region Intersection (교집합)

        /// <summary>
        /// 교집합
        /// </summary>
        public Set<T> Intersection(Set<T> set) {
            var resultSet = new Set<T>();

            if(set != null && set.Count > 0) {
                foreach(T item in this)
                    if(set.Contains(item))
                        resultSet.Add(item);
            }
            return resultSet;
        }

        /// <summary>
        /// 교집합
        /// </summary>
        public Set<T> Intersection(params T[] items) {
            if(items == null || items.Length == 0)
                return new Set<T>();

            return Intersection(new Set<T>(items));
        }

        /// <summary>
        /// 교집합 연산자
        /// </summary>
        public static Set<T> operator &(Set<T> lhs, Set<T> rhs) {
            if(lhs != null)
                return lhs.Intersection(rhs);
            if(rhs != null)
                return rhs.Intersection(lhs);

            return new Set<T>();
        }

        #endregion

        #region Difference (차집합)

        /// <summary>
        /// 현재 집합에서 지정된 집합을 뺀 차집합
        /// </summary>
        /// <param name="set"></param>
        /// <returns></returns>
        public Set<T> Difference(Set<T> set) {
            if(set == null || set.Count == 0)
                return Clone();

            var resultSet = new Set<T>();

            foreach(T item in this)
                if(!set.Contains(item))
                    resultSet.Add(item);

            return resultSet;
        }

        /// <summary>
        /// 현재 RwSet에서 배열의 요소들을 뺀 차집합
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public Set<T> Difference(params T[] items) {
            if(items == null || items.Length == 0)
                return Clone();

            return Difference(new Set<T>(items));
        }

        /// <summary>
        /// 차집합 연산자
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static Set<T> operator -(Set<T> lhs, Set<T> rhs) {
            return (lhs == null) ? null : lhs.Difference(rhs);
        }

        #endregion

        #region ExclusiveOr (여집합)

        /// <summary>
        /// 요소가 두집합중에 한 군데에만 속해 있는 집합
        /// </summary>
        public Set<T> ExclusiveOr(Set<T> set) {
            if(set == null || set.Count == 0)
                return Clone();

            var union = Union(set);
            var intersect = Intersection(set);

            return union.Difference(intersect);
        }

        /// <summary>
        /// 요소가 두집합중에 한 군데에만 속해 있는 집합
        /// </summary>
        public Set<T> ExclusiveOr(params T[] items) {
            if(items == null || items.Length == 0)
                return Clone();
            return ExclusiveOr(new Set<T>(items));
        }

        /// <summary>
        /// ExclusiveOr (XOR) 연산자
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static Set<T> operator ^(Set<T> lhs, Set<T> rhs) {
            if(lhs == null)
                return rhs;

            if(rhs == null)
                return lhs.ExclusiveOr(rhs);

            return new Set<T>();
        }

        #endregion

        #region IsSubset

        /// <summary>
        /// 현재 집합이 지정된 집합의 subset인지 판단한다. - 동치이면 True
        /// </summary>
        public virtual bool IsSubset(Set<T> set) {
            if(set == null)
                return false;
            if(set.Count < Count)
                return false;

            foreach(T item in this)
                if(!set.Contains(item))
                    return false;

            return true;
        }

        /// <summary>
        /// 현재 집합이 지정된 집합의 부분집합인지 판단한다. - 동치이면 True
        /// </summary>
        public virtual bool IsSubset(params T[] items) {
            items.ShouldNotBeNull("items");
            return IsSubset(new Set<T>(items));
        }

        /// <summary>
        /// 현재 집합이 지정된 집합의 참부분집합인지 판단한다. (동치이면 순 부분집합이 아니다.)
        /// </summary>
        public virtual bool IsProperSubset(Set<T> set) {
            return (IsSubset(set) && (Count < set.Count));
        }

        /// <summary>
        /// 현재 집합이 지정된 요소들로 이루어진 집합의 참부분집합인지 판단한다. (동치이면 순 부분집합이 아니다.)
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public virtual bool IsProperSubset(params T[] items) {
            if(items == null)
                return true;
            if(Count < items.Length)
                return IsProperSubset(new Set<T>(items));

            return false;
        }

        #endregion

        #region IsSuperset

        /// <summary>
        /// 현재 집합이 지정된 집합의 Superset인지 판다.
        /// </summary>
        /// <param name="set"></param>
        /// <returns></returns>
        public virtual bool IsSuperset(Set<T> set) {
            if(set == null)
                return true;
            if(set.Count > Count)
                return false;

            foreach(T item in set)
                if(!Contains(item))
                    return false;

            return true;
        }

        /// <summary>
        /// 현재 집합이 지정된 배열의 요소를 가진 집합의 부모집합인지 검사
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public virtual bool IsSuperset(params T[] items) {
            if(items == null)
                return true;

            return IsSuperset(new Set<T>(items));
        }

        /// <summary>
        /// 현집합이 지정된 집합의 순부모집합인지 검사
        /// </summary>
        /// <param name="set"></param>
        /// <returns></returns>
        public virtual bool IsProperSuperset(Set<T> set) {
            if(set == null)
                return true;

            return IsSuperset(set) && (Count > set.Count);
        }

        /// <summary>
        /// 현재 집합이 지정된 배열의 집합의 순 부모집합인지 검사 (동치이면 안된다);
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public virtual bool IsProperSuperset(params T[] items) {
            if(items == null)
                return true;
            if(Count > items.Length)
                return IsProperSuperset(new Set<T>(items));

            return false;
        }

        #endregion

        /// <summary>
        /// 두 집합이 동치인지 판단
        /// </summary>
        public static bool operator ==(Set<T> lhs, Set<T> rhs) {
            if(Equals(lhs, null) && Equals(rhs, null))
                return true;
            if(!Equals(lhs, null))
                return lhs.Equals(rhs);
            if(!Equals(rhs, null))
                return rhs.Equals(lhs);
            return false;
        }

        /// <summary>
        /// 두 집합이 동치가 아니면 True, 동치이면 False를 반환
        /// </summary>
        public static bool operator !=(Set<T> lhs, Set<T> rhs) {
            // null 값과 비교할 때는 단순 연산자를 쓰지 말고 
            // object.Equals 함수를 이용하라.

            if(!Equals(lhs, null))
                return !lhs.Equals(rhs);
            if(!Equals(rhs, null))
                return !rhs.Equals(lhs);
            return false;
        }

        /// <summary>
        /// 현재 집합이 지정된 개체와 동치인지 판단
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj) {
            if(obj != null && obj is Set<T>) {
                var set = (Set<T>)obj;
                return IsSubset(set) && (Count == set.Count);
            }

            return false;
        }

        /// <summary>
        /// Return hash code of this instance.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() {
            return ToString().GetHashCode();
        }

        /// <summary>
        /// Set 요소를 나열한다.
        /// </summary>
        public override string ToString() {
            return ToString(false);
        }

        /// <summary>
        /// Set 요소를 나열한다.
        /// </summary>
        /// <param name="sorted">정렬 여부</param>
        public string ToString(bool sorted) {
            ICollection<T> collection;

            if(IsEmpty)
                collection = null;
            else {
                List<T> clonedList;

                if(sorted) {
                    clonedList = new List<T>(_internalList);
                    clonedList.Sort();
                }
                else
                    clonedList = _internalList;

                collection = clonedList;
            }

            return collection.CollectionToString();
            //return RwCollection.AsString(clonedList.InOrderEnumerable);
        }

        #region ICollection<T> Members

        /// <summary>
        /// 요소들을 대상 배열에 복사한다.
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(T[] array, int arrayIndex) {
            array.ShouldNotBeNull("array");
            arrayIndex.ShouldBePositiveOrZero("arrayIndex");

            if(_internalList.Count + arrayIndex > array.Length)
                throw new ArgumentOutOfRangeException("arrayIndex");

            foreach(T item in _internalList) {
                array[arrayIndex++] = item;
            }
        }

        /// <summary>
        /// 읽기 전용 여부
        /// </summary>
        public virtual bool IsReadOnly {
            get { return false; }
        }

        #endregion

        #region IEnumerable<T> Members

        /// <summary>
        /// 반복자
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator() {
            return _internalList.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        #endregion

        #region ICloneable Members

        /// <summary>
        /// 인스턴스 복제
        /// </summary>
        /// <returns></returns>
        public virtual Set<T> Clone() {
            return new Set<T>(this);
        }

        #endregion
    }
}