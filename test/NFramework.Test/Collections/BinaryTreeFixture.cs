using System;
using NUnit.Framework;

namespace NSoft.NFramework.Collections {
    [TestFixture]
    public class BinaryTreeFixture : AbstractFixture {
        private const int MaxNodeSize = 0x2000;
        private readonly BinaryTree<int> BinTree = new BinaryTree<int>();

        [TestFixtureSetUp]
        public void Initialize() {
            const int maxNumber = MaxNodeSize * 1000;
            for(var i = 0; i < MaxNodeSize; i++)
                BinTree.Add(Rnd.Next(maxNumber));
        }

        [Test]
        public void ContainsTest() {
            BinTree.Add(MaxNodeSize + 1);
            Assert.IsTrue(BinTree.Contains(MaxNodeSize + 1));

            Console.WriteLine("Contains : " + BinTree.TraverseToString(TraversalMethod.InOrder));
        }

        [Test]
        public void RunRwBinaryTreeTest() {
            for(var i = 0; i < MaxNodeSize; i++) {
                var v = Rnd.Next(MaxNodeSize);

                switch(Rnd.Next(3)) {
                    case 0:
                        BinTree.Add(v);
                        break;
                    case 1:
                        BinTree.Remove(v);
                        break;
                    case 2:
                        BinTree.Contains(v);
                        break;
                }
            }
            Console.WriteLine(BinTree.TraverseToString(TraversalMethod.InOrder));
        }

        [Test]
        public void TraverseInOrderTest() {
            BinTree.Clear();
            BinTree.AddRange(6, 7, 1, 2, 3, 4, 5, 8, 9);
            Console.WriteLine("Count=" + BinTree.Count);
            Console.WriteLine(BinTree.TraverseToString(TraversalMethod.InOrder));
        }

        [Test]
        public void TraversePreOrderTest() {
            BinTree.Clear();
            BinTree.AddRange(6, 7, 1, 2, 3, 4, 5, 8, 9);
            Console.WriteLine(BinTree.TraverseToString(TraversalMethod.PreOrder));
        }

        [Test]
        public void TraversePostOrderTest() {
            BinTree.Clear();
            BinTree.AddRange(6, 7, 1, 2, 3, 4, 5, 8, 9);
            Console.WriteLine(BinTree.TraverseToString(TraversalMethod.PostOrder));
        }

        [Test]
        public void CopyToTest() {
            BinTree.Clear();
            BinTree.AddRange(6, 7, 1, 2, 3, 4, 5, 8, 9);

            var array = new int[BinTree.Count];
            BinTree.CopyTo(array, 0);

            foreach(int i in array)
                Console.Write(i + ",");
        }

        [Test]
        public void LocalEnumeratorTest() {
            BinTree.Clear();
            BinTree.AddRange(6, 7, 1, 2, 3, 4, 5, 8, 9);

            var node = BinTree.FindNode(6);
            Assert.IsNotNull(node);

            Console.WriteLine("Pre : ");
            foreach(BinaryTreeNode<int> childNode in BinTree.GetEnumerableByPreOrderedNode(node))
                Console.Write(childNode.Value + ",");

            Console.WriteLine("; In : ");
            foreach(BinaryTreeNode<int> childNode in BinTree.GetEnumerableByInOrderedNode(node))
                Console.Write(childNode.Value + ",");

            Console.WriteLine("; Post : ");
            foreach(BinaryTreeNode<int> childNode in BinTree.GetEnumerableByPostOrderedNode(node))
                Console.Write(childNode.Value + ",");

            Console.WriteLine("; Rev : ");
            foreach(BinaryTreeNode<int> childNode in BinTree.GetEnumerableByInRevOrderedNode(node))
                Console.Write(childNode.Value + ",");

            Console.WriteLine("");
        }
    }
}