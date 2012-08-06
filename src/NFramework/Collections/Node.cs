using System;
using System.Collections.Generic;

namespace NSoft.NFramework.Collections {
    /// <summary>
    /// Basic node for DataStructures
    /// </summary>
    /// <typeparam name="T">object type</typeparam>
    [Serializable]
    public class Node<T> : INode<T> {
        private T _itemValue;

        #region Constructors

        /// <summary>
        /// default constructor
        /// </summary>
        public Node() {
            _itemValue = default(T);
        }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="itemValue">Item Value</param>
        public Node(T itemValue) {
            _itemValue = itemValue;
        }

        #endregion

        #region INode<T> Members

        /// <summary>
        /// 노드가 가진 값
        /// </summary>
        public T Value {
            get { return _itemValue; }
            set { _itemValue = value; }
        }

        /// <summary>
        /// 이 노드와 관련된 노드들
        /// </summary>
        protected virtual NodeCollection<T> Nodes { get; set; }

        /// <summary>
        /// Associated nodes
        /// </summary>
        IList<INode<T>> INode<T>.Nodes {
            get { return Nodes as IList<INode<T>>; }
        }

        #endregion

        /// <summary>
        /// 노드가 가진 값을 반환한다.
        /// </summary>
        /// <returns>ItemValue의 정보, null 이면 "null"을 반환한다.</returns>
        public override string ToString() {
            return (Equals(_itemValue, null))
                       ? "null"
                       : _itemValue.ToString();
        }
    }
}