using System;
using System.Collections.Generic;

namespace NSoft.NFramework.Collections {
    /// <summary>
    /// Graph 의 Vertex
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class Vertex<T> : ValueObjectBase {
        /// <summary>
        /// constructor
        /// </summary>
        public Vertex() : this(default(T)) {}

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="value">value of vertex</param>
        public Vertex(T value) {
            Value = value;
        }

        /// <summary>
        /// Value of this vertext
        /// </summary>
        public T Value { get; set; }

        private IList<Vertex<T>> _adjacents = new List<Vertex<T>>();

        /// <summary>
        /// Adjacent vertices
        /// </summary>
        public IList<Vertex<T>> Adjacents {
            get { return _adjacents; }
            set { _adjacents = value; }
        }

        public override int GetHashCode() {
            return HashTool.Compute(Value);
        }
    }
}