using System;
using System.Collections.Generic;
using System.Linq;

namespace NSoft.NFramework.LinqEx {
    public static partial class EnumerableTool {
        #region << Convert To Stack / Queue >>

        /// <summary>
        /// <paramref name="sequence"/>에 있는 요소들을 Stack에 넣어서 반환한다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sequence"></param>
        /// <returns></returns>
        public static Stack<T> ToStack<T>(this IEnumerable<T> sequence) {
            return new Stack<T>(sequence);
        }

        /// <summary>
        /// 지정된 시퀀스를 큐에 담아 반환한다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sequence"></param>
        /// <returns></returns>
        public static Queue<T> ToQueue<T>(this IEnumerable<T> sequence) {
            return new Queue<T>(sequence);
        }

        #endregion

        #region << BinaryTree >>

        // TODO: Node<T> 와 NSoft.NFramework.Collections.Node<T> 를 합쳐야 한다.

        /// <summary>
        /// A Node in the traversal.
        /// </summary>
        /// <typeparam name="T">Type of item in node.</typeparam>
        public class Node<T> {
            public Node(int level, Node<T> parent, T item) {
                Level = level;
                Parent = parent;
                Item = item;
            }

            /// <summary>
            /// Level
            /// </summary>
            public int Level { get; set; }

            /// <summary>
            /// Parent node.
            /// </summary>
            public Node<T> Parent { get; set; }

            /// <summary>
            /// Item
            /// </summary>
            public T Item { get; set; }
        }

        /// <summary>
        /// 계층 구조 순서대로 Scan을 수행한다.
        /// </summary>
        /// <typeparam name="T">Scan할 요소의 수형</typeparam>
        /// <param name="source">Scan 대상</param>
        /// <param name="startWith">계층 구조에서 시작할 위치를 지정해주는 함수 (보통 계층구조상의 Root 요소를 반환하는 함수)</param>
        /// <param name="connectBy">첫번째 요소가 두번째 요소의 부모인지 판단하는 함수 (두 요소사이에 계층구조가 있음을 파악한다)</param>
        /// <param name="canYieldLevel">계층구조상의 지정된 Level 정보를 반환할 것인가를 결정하는 함수</param>
        /// <param name="parent">부모 노드</param>
        /// <returns>계층 구조상의 순서대로 제공되는 리스트</returns>
        public static IEnumerable<Node<T>> ByHierarchy<T>(this IEnumerable<T> source,
                                                          Func<T, bool> startWith,
                                                          Func<T, T, bool> connectBy,
                                                          Func<int, bool> canYieldLevel = null,
                                                          Node<T> parent = null) {
            source.ShouldNotBeNull("source");
            startWith.ShouldNotBeNull("startWith");
            connectBy.ShouldNotBeNull("connectBy");

            canYieldLevel = canYieldLevel ?? (_ => true);
            var level = (parent == null) ? 0 : parent.Level + 1;
            var items = source.Where(startWith);

            foreach(var item in items) {
                var newNode = new Node<T>(level, parent, item);

                if(canYieldLevel(level))
                    yield return newNode;

                var item1 = item;
                foreach(var childNode in ByHierarchy<T>(source,
                                                        possibleChild => connectBy(item1, possibleChild),
                                                        connectBy,
                                                        canYieldLevel,
                                                        newNode))
                    yield return childNode;
            }
        }

        /// <summary>
        /// Scans a source in reverse hierarchical order.
        /// </summary>
        /// <typeparam name="T">Scan할 요소의 수형</typeparam>
        /// <param name="source">Scan 대상</param>
        /// <param name="startWith">계층 구조에서 시작할 위치를 지정해주는 함수 (보통 계층구조상의 Root 요소를 반환하는 함수)</param>
        /// <param name="connectBy">첫번째 요소가 두번째 요소의 부모인지 판단하는 함수 (두 요소사이에 계층구조가 있음을 파악한다)</param>
        /// <param name="canYieldLevel">계층구조상의 지정된 Level 정보를 반환할 것인가를 결정하는 함수</param>
        /// <param name="parent">부모 노드</param>
        /// <returns>계층 구조상의 순서대로 제공되는 리스트</returns>
        public static IEnumerable<Node<T>> ByReverseHierarchy<T>(this IEnumerable<T> source,
                                                                 Func<T, bool> startWith,
                                                                 Func<T, T, bool> connectBy,
                                                                 Func<int, bool> canYieldLevel = null,
                                                                 Node<T> parent = null) {
            source.ShouldNotBeNull("source");
            startWith.ShouldNotBeNull("startWith");
            connectBy.ShouldNotBeNull("connectBy");

            canYieldLevel = canYieldLevel ?? (_ => true);
            var level = (parent == null) ? 0 : parent.Level + 1;
            var items = source.Where(startWith);

            foreach(var item in items) {
                var newNode = new Node<T>(level, parent, item);

                var possibleParent = item;
                foreach(var childNode in ByReverseHierarchy(source,
                                                            possibleChild => connectBy(possibleParent, possibleChild),
                                                            connectBy,
                                                            canYieldLevel,
                                                            newNode))
                    yield return childNode;

                if(canYieldLevel(level))
                    yield return newNode;
            }
        }

        /// <summary>
        /// BinaryTree를 PreOrder 방식으로 탐색한다 (Self, Left, Right 순으로 탐색)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">시작 노드</param>
        /// <param name="getLeft">노드의 왼쪽 Child Node를 구하는 메소드</param>
        /// <param name="getRight">노드의 오른쪽 Child Node를 구하는 메소드</param>
        /// <param name="canYieldLevel">소스를 반환할 수 있는 레벨인가 판단하는 함수</param>
        /// <param name="level">노드 레벨</param>
        /// <returns></returns>
        public static IEnumerable<T> BinaryTreePreOrderScan<T>(this T source,
                                                               Func<T, T> getLeft,
                                                               Func<T, T> getRight,
                                                               Func<int, bool> canYieldLevel = null,
                                                               int level = 0) {
            source.ShouldNotBeNull("source");
            getLeft.ShouldNotBeNull("getLeft");
            getRight.ShouldNotBeNull("getRight");

            canYieldLevel = canYieldLevel ?? (_ => true);

            if(canYieldLevel(level))
                yield return source;

            var left = getLeft(source);
            if(Equals(left, default(T)) == false)
                foreach(T subNode in left.BinaryTreePreOrderScan(getLeft, getRight, canYieldLevel, level + 1))
                    yield return subNode;

            var right = getRight(source);
            if(Equals(right, default(T)) == false)
                foreach(T subNode in right.BinaryTreePreOrderScan(getLeft, getRight, canYieldLevel, level + 1))
                    yield return subNode;
        }

        /// <summary>
        /// BinaryTree를 InOrder 방식으로 탐색한다 (Left, Self, Right 순으로 탐색) : 오름차순 정렬 순서대로 가져온다. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">시작 노드</param>
        /// <param name="getLeft">노드의 왼쪽 Child Node를 구하는 메소드</param>
        /// <param name="getRight">노드의 오른쪽 Child Node를 구하는 메소드</param>
        /// <param name="canYieldLevel">소스를 반환할 수 있는 레벨인가 판단하는 함수</param>
        /// <param name="level">노드 레벨</param>
        /// <returns></returns>
        public static IEnumerable<T> BinaryTreeInOrderScan<T>(this T source,
                                                              Func<T, T> getLeft,
                                                              Func<T, T> getRight,
                                                              Func<int, bool> canYieldLevel = null,
                                                              int level = 0) {
            source.ShouldNotBeNull("source");
            getLeft.ShouldNotBeNull("getLeft");
            getRight.ShouldNotBeNull("getRight");

            canYieldLevel = canYieldLevel ?? (_ => true);

            var left = getLeft(source);
            if(Equals(left, default(T)) == false)
                foreach(T subNode in left.BinaryTreeInOrderScan(getLeft, getRight, canYieldLevel, level + 1))
                    yield return subNode;

            if(canYieldLevel(level))
                yield return source;

            var right = getRight(source);
            if(Equals(right, default(T)) == false)
                foreach(T subNode in right.BinaryTreeInOrderScan(getLeft, getRight, canYieldLevel, level + 1))
                    yield return subNode;
        }

        /// <summary>
        /// BinaryTree를 PostOrder 방식으로 탐색한다 (Left, Right, Self 순으로 탐색)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">시작 노드</param>
        /// <param name="getLeft">노드의 왼쪽 Child Node를 구하는 메소드</param>
        /// <param name="getRight">노드의 오른쪽 Child Node를 구하는 메소드</param>
        /// <param name="canYieldLevel">소스를 반환할 수 있는 레벨인가 판단하는 함수</param>
        /// <param name="level">노드 레벨</param>
        /// <returns></returns>
        public static IEnumerable<T> BinaryTreePostOrderScan<T>(this T source,
                                                                Func<T, T> getLeft,
                                                                Func<T, T> getRight,
                                                                Func<int, bool> canYieldLevel = null,
                                                                int level = 0) {
            source.ShouldNotBeNull("source");
            getLeft.ShouldNotBeNull("getLeft");
            getRight.ShouldNotBeNull("getRight");

            canYieldLevel = canYieldLevel ?? (_ => true);

            var left = getLeft(source);
            if(Equals(left, default(T)) == false)
                foreach(T subNode in left.BinaryTreePostOrderScan(getLeft, getRight, canYieldLevel, level + 1))
                    yield return subNode;

            var right = getRight(source);
            if(Equals(right, default(T)) == false)
                foreach(T subNode in right.BinaryTreePostOrderScan(getLeft, getRight, canYieldLevel, level + 1))
                    yield return subNode;

            if(canYieldLevel(level))
                yield return source;
        }

        /// <summary>
        /// BinaryTree를 InReverseOrder 방식으로 탐색한다 (Right, Self, Left 순으로 탐색) : 내림차순 정렬 순서대로 가져온다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">시작 노드</param>
        /// <param name="getLeft">노드의 왼쪽 Child Node를 구하는 메소드</param>
        /// <param name="getRight">노드의 오른쪽 Child Node를 구하는 메소드</param>
        /// <param name="canYieldLevel">소스를 반환할 수 있는 레벨인가 판단하는 함수</param>
        /// <param name="level">노드 레벨</param>
        /// <returns></returns>
        public static IEnumerable<T> BinaryTreeInRevOrderScan<T>(this T source,
                                                                 Func<T, T> getLeft,
                                                                 Func<T, T> getRight,
                                                                 Func<int, bool> canYieldLevel = null,
                                                                 int level = 0) {
            source.ShouldNotBeNull("source");
            getLeft.ShouldNotBeNull("getLeft");
            getRight.ShouldNotBeNull("getRight");

            canYieldLevel = canYieldLevel ?? (_ => true);

            var right = getRight(source);
            if(Equals(right, default(T)) == false)
                foreach(T subNode in right.BinaryTreeInRevOrderScan(getLeft, getRight, canYieldLevel, level + 1))
                    yield return subNode;

            if(canYieldLevel(level))
                yield return source;

            var left = getLeft(source);
            if(Equals(left, default(T)) == false)
                foreach(T subNode in left.BinaryTreeInRevOrderScan(getLeft, getRight, canYieldLevel, level + 1))
                    yield return subNode;
        }

        #endregion

        #region << Graph Navigation >>

        /// <summary>
        /// Breadth-First Scan for Adjacent Graph
        /// </summary>
        /// <typeparam name="T">탐색할 Node의 수형</typeparam>
        /// <param name="source">Graph의 시작 Point</param>
        /// <param name="getAdjacent">현재 Node와 연결된 인접한 노드들을 찾아주는 함수</param>
        /// <returns>Breadth-Firth Algorithm으로 탐색한 노드들의 컬렉션</returns>
        public static IEnumerable<T> GraphBreadthFirstScan<T>(this T source, Func<T, IEnumerable<T>> getAdjacent) {
            source.ShouldNotBeNull("source");
            getAdjacent.ShouldNotBeNull("getAdjacent");

            var toScan = new Queue<T>(new[] { source });
            var scanned = new HashSet<T>();

            while(toScan.Count > 0) {
                T current = toScan.Dequeue();

                yield return current;
                scanned.Add(current);

                foreach(T item in getAdjacent(current)) {
                    if(scanned.Contains(item) == false)
                        toScan.Enqueue(item);
                }
            }
        }

        /// <summary>
        /// Depth-First Scan for Adjacent Graph
        /// </summary>
        /// <typeparam name="T">탐색할 Node의 수형</typeparam>
        /// <param name="source">Graph의 시작 Point</param>
        /// <param name="getAdjacent">현재 Node와 연결된 인접한 노드들을 찾아주는 함수</param>
        /// <returns>Depth-Firth Algorithm으로 탐색한 노드들의 컬렉션</returns>
        public static IEnumerable<T> GraphDepthFirstScan<T>(this T source, Func<T, IEnumerable<T>> getAdjacent) {
            source.ShouldNotBeNull("source");
            getAdjacent.ShouldNotBeNull("getAdjacent");

            var toScan = new Stack<T>(new[] { source });
            var scanned = new HashSet<T>();

            while(toScan.Count > 0) {
                T current = toScan.Pop();

                yield return current;
                scanned.Add(current);

                foreach(T item in getAdjacent(current)) {
                    if(scanned.Contains(item) == false)
                        toScan.Push(item);
                }
            }
        }

        #endregion
    }
}