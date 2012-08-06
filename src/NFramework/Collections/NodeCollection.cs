using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NSoft.NFramework.Reflections;

namespace NSoft.NFramework.Collections {
    /// <summary>
    /// Collection of <see cref="Node{T}"/>
    /// </summary>
    /// <typeparam name="N">Node Value Type</typeparam>
    [Serializable]
    public class NodeCollection<N> : List<Node<N>> {
        /// <summary>
        /// default constructor
        /// </summary>
        public NodeCollection() {}

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="list">N 타입의 값을 가지는 노드의 리스트 객체</param>
        public NodeCollection(IEnumerable<Node<N>> list) : base(list) {}

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="count">컬렉션에 새로 만들 노드의 갯수</param>
        public NodeCollection(int count)
            : base(count) {
            // capacity 만큼 자리만 마련하는게 아니라 Node<T> 를 기본 생성자로 인스턴스를 생성해서 넣는다.
            //
            for(var i = 0; i < count; i++)
                Add(new Node<N>());
        }

        /// <summary>
        /// Find Node contains the specified value.
        /// </summary>
        /// <param name="value">searching value</param>
        /// <returns>node that contains the value. if not exist return null.</returns>
        public Node<N> FindByValue(N value) {
            IComparer<N> valueComparer = Comparer<N>.Default;

            return this.FirstOrDefault(node => valueComparer.Compare(node.Value, value) == 0);
        }

        /// <summary>
        /// Find all nodes that contains the specified value.
        /// </summary>
        /// <param name="value">seaching value</param>
        /// <returns>Collection of Node that contains the specified value</returns>
        public NodeCollection<N> FindAllByValue(N value) {
            var result = new NodeCollection<N>();

            IComparer<N> valueComparer = Comparer<N>.Default;

            foreach(var node in this.Where(node => valueComparer.Compare(node.Value, value) == 0))
                result.Add(node);

            return result;
        }

        /// <summary>
        /// Return Node informations
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            return ((IEnumerable)this).CollectionToString();
        }
    }
}