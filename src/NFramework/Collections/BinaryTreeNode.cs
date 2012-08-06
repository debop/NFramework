using System;

namespace NSoft.NFramework.Collections {
    /// <summary>
    /// Binary Tree Node
    /// </summary>
    /// <typeparam name="T">Type of Item that contains in BinaryTreeNode </typeparam>
    [Serializable]
    public class BinaryTreeNode<T> : Node<T> {
        /// <summary>
        /// Left Node
        /// </summary>
        public const int LEFT_INDEX = 0;

        /// <summary>
        /// Right Node
        /// </summary>
        public const int RIGHT_INDEX = 1;

        /// <summary>
        /// default constructor
        /// </summary>
        /// <param name="value">node value</param>
        public BinaryTreeNode(T value) : base(value) {}

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="value">node value</param>
        /// <param name="leftNode">left node</param>
        /// <param name="rightNode">right node</param>
        public BinaryTreeNode(T value, Node<T> leftNode, Node<T> rightNode) {
            Value = value;
            Nodes[LEFT_INDEX] = leftNode;
            Nodes[RIGHT_INDEX] = rightNode;
        }

        /// <summary>
        /// child nodes	(left, right만 있다)
        /// </summary>
        protected override NodeCollection<T> Nodes {
            get { return base.Nodes ?? (base.Nodes = new NodeCollection<T>(2)); }
        }

        /// <summary>
        /// left child node
        /// </summary>
        //[CLSCompliant(false)]
        public BinaryTreeNode<T> Left {
            get { return Nodes[LEFT_INDEX] as BinaryTreeNode<T>; }
            set { Nodes[LEFT_INDEX] = value; }
        }

        /// <summary>
        /// right child node
        /// </summary>
        //[CLSCompliant(false)]
        public BinaryTreeNode<T> Right {
            get { return Nodes[RIGHT_INDEX] as BinaryTreeNode<T>; }
            set { Nodes[RIGHT_INDEX] = value; }
        }
    }
}