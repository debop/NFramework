using System.Collections.Generic;

namespace NSoft.NFramework.Collections {
    /// <summary>
    /// Node that has object.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface INode<T> {
        /// <summary>
        /// Value that current node contains 
        /// </summary>
        T Value { get; set; }

        /// <summary>
        /// Associated nodes
        /// </summary>
        IList<INode<T>> Nodes { get; }
    }
}