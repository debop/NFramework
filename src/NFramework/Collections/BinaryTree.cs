using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NSoft.NFramework.Reflections;

namespace NSoft.NFramework.Collections {
    /// <summary>
    /// 이진 트리를 구성한다.
    /// </summary>
    /// <remarks>
    /// 이진 트리에서는 <c>BinaryTreeNode</c>(노드)의 Value (값)을 중복을 허용하지 않는다.<br/>
    /// 중복된 노드 값이 들어 왔을 때는 무시한다.
    /// </remarks>
    /// <typeparam name="T">이진 트리를 구성하는 노드 <c>BinaryTreeNode</c> 의 값의 수형</typeparam>
    [Serializable]
    public class BinaryTree<T> : ICollection<T>, ICollection {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private BinaryTreeNode<T> _root;
        private int _count;

        /// <summary>
        /// 트리 구성시 순서를 정하기 'T' 수형에 대한 위해 비교 인터페이스가 필요하다.
        /// </summary>
        private readonly IComparer<T> _comparer = Comparer<T>.Default;

        /// <summary>
        /// 기본 생성자
        /// </summary>
        public BinaryTree() : this(null, null) {}

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="comparer">노드의 값을 비교하기 위한 비교자</param>
        public BinaryTree(IComparer<T> comparer) : this((IEnumerable<T>)null, comparer) {}

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="collection">노드를 구성할 값들의 컬렉션</param>
        public BinaryTree(IEnumerable<T> collection) : this(collection, (IComparer<T>)null) {}

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="collection">노드를 구성할 값들의 컬렉션</param>
        /// <param name="comparer">노드의 값을 비교하기 위한 비교자</param>
        public BinaryTree(IEnumerable<T> collection, IComparer<T> comparer) {
            _comparer = comparer ?? Comparer<T>.Default;

            if(collection != null)
                AddRange(collection);
        }

        /// <summary>
        /// 트리가 비었는지 검사 (Root가 널이면 빈 트리이다.)
        /// </summary>
        /// <value></value>
        public bool IsEmpty {
            get { return object.Equals(_root, null); }
        }

        /// <summary>
        /// Root node of this binary tree
        /// </summary>
        public BinaryTreeNode<T> Root {
            get { return _root; }
        }

        /// <summary>
        /// PreOrder (자신->Left->Right 순)으로 탐색이 가능한 Value에 대한 반복자를 제공한다.
        /// </summary>
        public virtual IEnumerable<T> PreOrder {
            get { return PreOrderNode.Select(node => node.Value); }
        }

        /// <summary>
        /// PreOrder 방식의 이진트리 노드에 대한 반복자를 만든다. (자신,Left,Right 순)
        /// </summary>
        /// <value></value>
        public virtual IEnumerable<BinaryTreeNode<T>> PreOrderNode {
            get { return GetEnumerableByPreOrderedNode(_root); }
        }

        /// <summary>
        /// PreOrder 방식의 이진트리 노드에 대한 반복자를 만든다. (자신,Left,Right 순)
        /// </summary>
        /// <param name="root">시작 노드</param>
        public virtual IEnumerable<BinaryTreeNode<T>> GetEnumerableByPreOrderedNode(BinaryTreeNode<T> root) {
            var toVisit = new Stack<BinaryTreeNode<T>>(Count);
            var current = root;

            if(current != null)
                toVisit.Push(current);

            while(toVisit.Count != 0) {
                current = toVisit.Pop();

                if(current.Right != null)
                    toVisit.Push(current.Right);
                if(current.Left != null)
                    toVisit.Push(current.Left);

                yield return current;
            }
        }

        /// <summary>
        /// InOrder (Left->자신->Right 순) 으로 Value 탐색이 가능한 반복자를 제공한다.
        /// </summary>
        public virtual IEnumerable<T> InOrder {
            get { return InOrderNode.Select(node => node.Value); }
        }

        /// <summary>
        /// InOrder 방법으로 이진트리 노드 탐색의 반복자를 만든다. (Left,자신, Right 순)
        /// </summary>
        /// <value></value>
        public virtual IEnumerable<BinaryTreeNode<T>> InOrderNode {
            get { return GetEnumerableByInOrderedNode(_root); }
        }

        /// <summary>
        /// InOrder 방법으로 이진트리 노드 탐색의 반복자를 만든다. (Left,자신, Right 순)
        /// </summary>
        /// <param name="root">시작 노드</param>
        public virtual IEnumerable<BinaryTreeNode<T>> GetEnumerableByInOrderedNode(BinaryTreeNode<T> root) {
            var toVisit = new Stack<BinaryTreeNode<T>>(_count);

            for(var current = root; current != null || toVisit.Count != 0; current = current.Right) {
                // 가장 왼쪽에 있는 노드를 찾는다.
                while(current != null) {
                    toVisit.Push(current);
                    current = current.Left;
                }

                current = toVisit.Pop();
                yield return current;
            }
        }

        /// <summary>
        /// PostOrder (Left->Right->자신 순)으로 Value 탐색이 가능한 반복자를 제공한다.
        /// </summary>
        public virtual IEnumerable<T> PostOrder {
            get { return PostOrderNode.Select(node => node.Value); }
        }

        /// <summary>
        /// PostOrder (Left->Right->자신 순)으로 이진트리 노드 탐색이 가능한 반복자를 제공한다.
        /// </summary>
        /// <value></value>
        public virtual IEnumerable<BinaryTreeNode<T>> PostOrderNode {
            get { return GetEnumerableByPostOrderedNode(_root); }
        }

        /// <summary>
        /// 지정된 노드를 기준으로 PostOrder (Left->Right->자신 순)으로 이진트리 노드 탐색이 가능한 반복자를 제공한다.
        /// </summary>
        /// <param name="root">시작 노드</param>
        /// <returns>이진트리 노드 반복자</returns>
        public virtual IEnumerable<BinaryTreeNode<T>> GetEnumerableByPostOrderedNode(BinaryTreeNode<T> root) {
            // 두개의 stack을 사용한다.
            // 하나는 방문해야 할 노드를 보관, 하나는 방문 여부를 보관

            var toVisit = new Stack<BinaryTreeNode<T>>(Count);
            var hasBeenProcessed = new Stack<bool>(Count);

            var current = root;

            if(current != null) {
                toVisit.Push(current);
                hasBeenProcessed.Push(false);
                current = current.Left;
            }

            while(toVisit.Count != 0) {
                if(current != null) {
                    toVisit.Push(current);
                    hasBeenProcessed.Push(false);
                    current = current.Left;
                }
                else {
                    var processed = hasBeenProcessed.Pop();
                    var node = toVisit.Pop();

                    if(!processed) {
                        // 처리 안했으면, right node 로 간다.
                        toVisit.Push(node);
                        hasBeenProcessed.Push(true); // 처리된 것으로 간주한다.
                        current = node.Right;
                    }
                    else
                        yield return node;
                }
            }
        }

        /// <summary>
        /// 지정된 노드를 기준으로 InRevOrder( Right->자신->Left 순)으로 값 탐색이 가능한 반복자를 제공한다.
        /// </summary>
        public virtual IEnumerable<T> InRevOrder {
            get { return InRevOrderNode.Select(node => node.Value); }
        }

        /// <summary>
        /// 지정된 노드를 기준으로 InRevOrder( Right->자신->Left 순)으로 이진트리 노드 탐색이 가능한 반복자를 제공한다.
        /// </summary>
        public virtual IEnumerable<BinaryTreeNode<T>> InRevOrderNode {
            get { return GetEnumerableByInRevOrderedNode(_root); }
        }

        /// <summary>
        /// 지정된 노드를 기준으로 InRevOrder( Right->자신->Left 순)으로 이진트리 노드 탐색이 가능한 반복자를 제공한다.
        /// </summary>
        /// <param name="root">시작 노드</param>
        /// <returns>이진트리 노드 반복자</returns>
        public virtual IEnumerable<BinaryTreeNode<T>> GetEnumerableByInRevOrderedNode(BinaryTreeNode<T> root) {
            var toVisit = new Stack<BinaryTreeNode<T>>(_count);

            for(var current = root; current != null || toVisit.Count != 0; current = current.Left) {
                // 가장 오른쪽에 있는 노드를 찾는다.
                while(current != null) {
                    toVisit.Push(current);
                    current = current.Right;
                }

                current = toVisit.Pop();
                yield return current;
            }
        }

        /// <summary>
        /// 이진트리를 복제한다.
        /// </summary>
        /// <returns></returns>
        public BinaryTree<T> Clone() {
            return new BinaryTree<T>(this);
        }

        /// <summary>
        /// 탐색 방법에 따른 값에 대한 노드 값에 대한 반복자를 제공한다.
        /// </summary>
        public virtual IEnumerable<T> GetValueEnumerable(TraversalMethod method) {
            switch(method) {
                case TraversalMethod.PreOrder:
                    return PreOrder;
                case TraversalMethod.InOrder:
                    return InOrder;
                case TraversalMethod.PostOrder:
                    return PostOrder;
                case TraversalMethod.InRevOrder:
                    return InRevOrder;
                default:
                    return InOrder;
            }
        }

        /// <summary>
        /// 탐색 방법에 따른 노드에 대한 반복자를 반환한다.
        /// </summary>
        public virtual IEnumerable<BinaryTreeNode<T>> GetNodeEnumerable(TraversalMethod method) {
            switch(method) {
                case TraversalMethod.PreOrder:
                    return PreOrderNode;
                case TraversalMethod.InOrder:
                    return InOrderNode;
                case TraversalMethod.PostOrder:
                    return PostOrderNode;
                case TraversalMethod.InRevOrder:
                    return InRevOrderNode;
                default:
                    return InOrderNode;
            }
        }

        /// <summary>
        /// 배열의 값을 이진 트리에 추가한다.
        /// </summary>
        public virtual void AddRange(params T[] values) {
            if(values != null)
                foreach(T value in values)
                    Add(value);
        }

        /// <summary>
        /// 값들을 이진 트리에 추가한다.
        /// </summary>
        /// <param name="values"></param>
        public void AddRange(IEnumerable<T> values) {
            if(values != null)
                foreach(T value in values)
                    Add(value);
        }

        /// <summary>
        /// 지정된 값을 가진 새로운 노드를 Tree에 추가한다.
        /// </summary>
        /// <remarks>
        /// 중복을 허용하지 않는다.
        /// </remarks>
        /// <param name="value">이진트리 노드의 값</param>
        public virtual void Add(T value) {
            var newNode = new BinaryTreeNode<T>(value);
            int result;

            var current = _root;
            BinaryTreeNode<T> parent = null;

            while(current != null) {
                result = _comparer.Compare(current.Value, value);

                if(result == 0) // 중복 허용 않음!!!
                    return;

                // 현재 값이 추가된 값보다 크면 왼쪽으로, 크면 오른쪽으로 탐색한다.
                parent = current;
                current = (result > 0) ? current.Left : current.Right;
            }

            _count++;

            if(parent == null)
                _root = newNode;
            else {
                result = _comparer.Compare(parent.Value, value);

                if(result > 0)
                    parent.Left = newNode;
                else
                    parent.Right = newNode;
            }
        }

        /// <summary>
        /// 트리에서 노드들을 모두 제거한다.
        /// </summary>
        public void Clear() {
            _root = null;
            _count = 0;
        }

        /// <summary>
        /// 지정된 값을 가진 노드가 있는지 검사한다.
        /// </summary>
        /// <param name="value">찾을 값</param>
        /// <returns>존재 여부</returns>
        public bool Contains(T value) {
            return (FindNode(value) != null);
        }

        /// <summary>
        /// 이진트리에 있는 노드의 값들을 InOrder 탐색 방식으로 1차원 배열에 복사한다.
        /// </summary>
        /// <param name="array">값을 담을 1차원 배열</param>
        /// <param name="arrayIndex">값을 담을 배열의 첫번째 위치</param>
        public void CopyTo(T[] array, int arrayIndex) {
            CopyTo(array, arrayIndex, TraversalMethod.InOrder);
        }

        /// <summary>
        /// 이진트리에 있는 노드의 값들을 1차원 배열에 복사한다.
        /// </summary>
        /// <param name="array">값을 담을 1차원 배열</param>
        /// <param name="arrayIndex">값을 담을 배열의 첫번째 위치</param>
        /// <param name="method">탐색 방법</param>
        /// <exception cref="ArgumentOutOfRangeException">값을 담을 배열의 주어진 크기가 작을때</exception>
        public void CopyTo(T[] array, int arrayIndex, TraversalMethod method) {
            array.ShouldNotBeNull("array");

            Guard.Assert(_count + arrayIndex <= array.Length, @"arrayIndex is larger than length of array to copy space");

            foreach(T value in GetValueEnumerable(method))
                array[arrayIndex++] = value;
        }

        /// <summary>
        /// 이진트리에 있는 노드의 수
        /// </summary>
        public int Count {
            get { return _count; }
        }

        /// <summary>
        /// 읽기 전용인가?
        /// </summary>
        /// <value></value>
        public bool IsReadOnly {
            get { return false; }
        }

        /// <summary>
        /// 지정된 값을 가진 노드를 삭제한다.
        /// </summary>
        /// <param name="value">삭제할 값</param>
        /// <returns>삭제 여부</returns>
        public virtual bool Remove(T value) {
            if(IsEmpty)
                return false;

            BinaryTreeNode<T> current;
            BinaryTreeNode<T> parent;

            // 노드를 찾는다.
            FindNode(value, out current, out parent);

            //해당노드가 없다면
            if(current == null)
                return false;

            _count--;

            int result;

            // CASE 1 :
            //          현재 노드가 Right Child가 없을 때, 현재 노드의 Left child가 부모 노드에 속한다.
            if(current.Right == null) {
                if(parent == null)
                    _root = current.Left;
                else {
                    result = _comparer.Compare(parent.Value, current.Value);

                    // parent.Value > node.Value 이므로, 왼쪽으로 붙는다.
                    if(result > 0)
                        parent.Left = current.Left;
                        // parent 값이 더 작으므로, 오른쪽에 붙는다.
                    else if(result < 0)
                        parent.Right = current.Left;
                }
            }
                // CASE 2 :
                //          현재 노드의 Right child의 Left child가 없다면, 
                //          현재 노드의 Right child가 현재 노드를 대신하고,
                //          현재 노드의 Left child는 현재 노드의 Right child의 Left child로 된다.
            else if(current.Right.Left == null) {
                current.Right.Left = current.Left;

                if(parent == null)
                    _root = current.Right;
                else {
                    result = _comparer.Compare(parent.Value, current.Value);

                    if(result > 0)
                        parent.Left = current.Right;
                    else if(result < 0)
                        parent.Right = current.Right;
                }
            }
                // CASE 3 : 
                //          현재 노드의 Right child에게 Left child가 있다면,
                //          현재 노드를 Right child의 자손중에 가장 왼쪽에 있는 노드로 교체한다.
            else {
                var lmParent = current.Right;
                var leftMost = GetLeftMostChild(current.Right.Left);

                // 최좌측 노드의 오른쪽 자식 노드들은 부모 노드의 왼쪽 자식으로 등록된다.
                lmParent.Left = leftMost.Right;

                // 최좌측 노드를 현재 노드로 교체한다.
                leftMost.Left = current.Left;
                leftMost.Right = current.Right;

                if(parent == null)
                    _root = leftMost;
                else {
                    result = _comparer.Compare(parent.Value, current.Value);

                    if(result > 0)
                        parent.Left = leftMost;
                    else if(result < 0)
                        parent.Right = leftMost;
                }
            }

            current.Left = current.Right = null;
            current = null;

            return true;
        }

        /// <summary>
        /// 지정된 값을 가진 노드를 검색하여 반환한다.
        /// </summary>
        /// <returns>검색된 노드, 검색 실패시에는 null을 반환한다.</returns>
        public virtual BinaryTreeNode<T> FindNode(T value) {
            if(IsEmpty)
                return null;

            var current = _root;

            while(current != null) {
                var result = _comparer.Compare(current.Value, value);

                if(result == 0) // 찾았다!!!
                    return current;

                current = (result > 0) ? current.Left : current.Right;
            }
            return null;
        }

        /// <summary>
        /// 지정된 값을 가진 노드를 검색한다.
        /// </summary>
        /// <param name="value">검색할 값</param>
        /// <param name="current">검색 값을 가진 노드</param>
        /// <param name="parent">찾은 노드의 부모 노드</param>
        public virtual void FindNode(T value, out BinaryTreeNode<T> current, out BinaryTreeNode<T> parent) {
            current = null;
            parent = null;

            if(IsEmpty)
                return;

            current = _root;
            int result = _comparer.Compare(current.Value, value);

            while(result != 0) {
                parent = current;
                current = (result > 0) ? current.Left : current.Right;

                if(current == null)
                    return;

                result = _comparer.Compare(current.Value, value);
            }
        }

        /// <summary>
        /// 트리에서 가장 왼쪽에 있는 노드를 구한다.
        /// </summary>
        /// <returns>노드</returns>
        public BinaryTreeNode<T> GetLeftMostChild() {
            return GetLeftMostChild(_root);
        }

        /// <summary>
        /// 기준노드를 루트로 볼 때 가장 왼쪽에 있는 노드를 구한다.
        /// </summary>
        /// <param name="current">기준 노드</param>
        /// <returns>노드, 없을 시에는 null 반환</returns>
        public BinaryTreeNode<T> GetLeftMostChild(BinaryTreeNode<T> current) {
            if(current == null)
                return null;

            var leftMost = current;

            while(current != null) {
                leftMost = current;
                current = current.Left;
            }

            return leftMost;
        }

        /// <summary>
        /// 트리의 가장 오른쪽에 있는 노드를 구한다.
        /// </summary>
        /// <returns>노드, 없을 시에는 null 반환</returns>
        public BinaryTreeNode<T> GetRightMostChild() {
            return GetRightMostChild(_root);
        }

        /// <summary>
        /// 기준노드를 루트로 볼 때 가장 오른쪽에 있는 노드를 구한다.
        /// </summary>
        /// <param name="current"></param>
        /// <returns>노드, 없을 시에는 null 반환</returns>
        public BinaryTreeNode<T> GetRightMostChild(BinaryTreeNode<T> current) {
            if(current == null)
                return null;

            var rightMost = current;

            while(current != null) {
                rightMost = current;
                current = current.Right;
            }

            return rightMost;
        }

        /// <summary>
        /// 탐색 방법에 따라 탐색하면서 값들을 집합형식으로 표현한 문자열을 반환한다.
        /// </summary>
        /// <param name="method">탐색 방법</param>
        /// <returns>노드 값을 집합으로 표현한 문자열 </returns>
        public string TraverseToString(TraversalMethod method) {
            return ToString(method);
        }

        /// <summary>
        /// 노드들의 Value를 지정된 컬렉션에 담는다.
        /// </summary>
        public virtual void GetTreeDataList(IList<T> list) {
            GetTreeDataList(list, TraversalMethod.InOrder);
        }

        /// <summary>
        /// 노드들을 지정된 탐색 방법에 따라 지정된 컬렉션에 담는다.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="method"></param>
        public virtual void GetTreeDataList(IList<T> list, TraversalMethod method) {
            // 탐색 방법에 따라 얻는다.
            foreach(T data in GetValueEnumerable(method))
                list.Add(data);
        }

        /// <summary>
        /// InOrder 방식으로 노드들의 값을 집합형식의 문자열로 만듦
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            return ToString(TraversalMethod.InOrder);
        }

        /// <summary>
        /// 지정된 탐색 방법으로 노드들의 값을 집합형식의 문자열로 만듦
        /// </summary>
        /// <param name="method">탐색 방법</param>
        /// <returns>문자열</returns>
        public virtual string ToString(TraversalMethod method) {
            if(IsEmpty)
                return ReflectionTool.CollectionToString(null);

            return GetValueEnumerable(method).CollectionToString();
        }

        /// <summary>
        /// Return enumerator by by InOrder traversal method.
        /// </summary>
        /// <returns>Enumerator of {T}</returns>
        public IEnumerator<T> GetEnumerator() {
            return GetValueEnumerable(TraversalMethod.InOrder).GetEnumerator();
        }

        /// <summary>
        /// Return enumerator by the specified traversal method.
        /// </summary>
        /// <param name="method">Traversal method</param>
        /// <returns>Enumerator of {T}</returns>
        public IEnumerator<T> GetEnumerator(TraversalMethod method) {
            return GetValueEnumerable(method).GetEnumerator();
        }

        /// <summary>
        /// Return binary tree node enumerator by InOrder traversal method.
        /// </summary>
        /// <returns>Enumerator of <see cref="BinaryTreeNode{T}"/></returns>
        public IEnumerator<BinaryTreeNode<T>> GetNodeEnumerator() {
            return GetNodeEnumerator(TraversalMethod.InOrder);
        }

        /// <summary>
        /// Return binary tree node enumerator by the specified traversal method.
        /// </summary>
        /// <param name="method"></param>
        /// <returns>Enumerator of <see cref="BinaryTreeNode{T}"/></returns>
        public IEnumerator<BinaryTreeNode<T>> GetNodeEnumerator(TraversalMethod method) {
            return GetNodeEnumerable(method).GetEnumerator();
        }

        void ICollection.CopyTo(Array array, int index) {
            array.ShouldNotBeNull("array");
            index.ShouldBePositiveOrZero("index");

            if(array.Length - index < _count)
                throw new ArgumentOutOfRangeException("index",
                                                      "array size is smaller than binary tree size or index is greater than proper offset.");

            var values = new T[_count];

            CopyTo(values, 0);
            Array.Copy(values, 0, array, index, array.Length - index);
        }

        int ICollection.Count {
            get { return _count; }
        }

        bool ICollection.IsSynchronized {
            get { return false; }
        }

        object ICollection.SyncRoot {
            get { return null; }
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return InOrder.GetEnumerator();
        }
    }
}