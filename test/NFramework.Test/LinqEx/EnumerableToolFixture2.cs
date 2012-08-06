using System.Linq;
using NSoft.NFramework.Collections;
using NSoft.NFramework.UnitTesting;
using NUnit.Framework;

namespace NSoft.NFramework.LinqEx.Collections {
    [Microsoft.Silverlight.Testing.Tag("Linq")]
    [TestFixture]
    public class EnumerableToolFixture2 : AbstractFixture {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        private const int MaxSize = 512;
        private const int MaxNum = MaxSize * MaxSize;
        private const int MaxDepth = 5;
        private const int MinAdjacent = 5;
        private const int MaxAdjacent = 10;

        private readonly Vertex<int> _startVertex = new Vertex<int>(1);

#if !SILVERLIGHT
        private readonly BinaryTree<int> _tree = new BinaryTree<int>();

        protected override void OnFixtureSetUp() {
            base.OnFixtureSetUp();

            for(int i = 0; i < MaxSize; i++)
                _tree.Add(Rnd.Next(MaxNum));

            BuildGraph(_startVertex, 0);
        }

        [Test]
        public void ByHierarchy() {
            var nodes = _tree.InOrderNode.ByHierarchy(node => node == _tree.Root,
                                                      (parent, child) => (parent.Left == child) || (parent.Right == child));

            Assert.IsTrue(nodes.Any());

            var nodeArray = nodes.ToArray();
            CollectionAssert.AllItemsAreUnique(nodeArray);
            CollectionAssert.AllItemsAreNotNull(nodeArray);
        }

        [Test]
        public void BinaryTreePreOrderScan() {
            var nodes = _tree.Root.BinaryTreePreOrderScan(n => n.Left, n => n.Right);

            Assert.IsNotNull(nodes);
            Assert.IsTrue(nodes.Any());
            CollectionAssert.AllItemsAreUnique(nodes.ToArray());
        }

        [Test]
        public void BinaryTreeInOrderScan() {
            var nodes = _tree.Root.BinaryTreeInOrderScan(n => n.Left, n => n.Right);

            Assert.IsNotNull(nodes);
            Assert.IsTrue(nodes.Any());
            CollectionAssert.AllItemsAreUnique(nodes.ToArray());
        }

        [Test]
        public void BinaryTreePostOrderScan() {
            var nodes = _tree.Root.BinaryTreePostOrderScan(n => n.Left, n => n.Right);

            Assert.IsNotNull(nodes);
            Assert.IsTrue(nodes.Any());
            CollectionAssert.AllItemsAreUnique(nodes.ToArray());
        }

        [Test]
        public void BinaryTreeInRevOrderScan() {
            var nodes = _tree.Root.BinaryTreeInRevOrderScan(n => n.Left, n => n.Right);

            Assert.IsNotNull(nodes);
            Assert.IsTrue(nodes.Any());
            CollectionAssert.AllItemsAreUnique(nodes.ToArray());
        }
#endif

        [Test]
        public void GraphBreadFirstSearch() {
            var vertices = _startVertex.GraphBreadthFirstScan(v => v.Adjacents).ToList();

            Assert.IsTrue(vertices.Any());
            Assert.GreaterOrEqual(vertices.Count, MinAdjacent * MaxDepth);
        }

        [Test]
        public void GraphDepthFirstSearch() {
            var vertices = _startVertex.GraphDepthFirstScan(v => v.Adjacents).ToList();

            Assert.IsTrue(vertices.Any());
            Assert.GreaterOrEqual(vertices.Count, MinAdjacent * MaxDepth);
        }

        [Test]
        public void Between() {
            int[] numbers = { 1, 2, 3, 4, 5, 6, 7 };

            var subNumbers = numbers.Where(n => n.Between(4, 9));
            Assert.AreEqual(4, subNumbers.Count());
            Assert.IsTrue(subNumbers.SequenceEqual(new[] { 4, 5, 6, 7 }));
        }

        [Test]
        public void InTest() {
            int[] numbers = { 1, 2, 3, 4, 5, 6, 7 };

            var evens = numbers.Where(n => n % 2 == 0);
            var even = numbers.Where(n => n.In(evens));

            Assert.AreEqual(3, evens.Count());
            Assert.AreEqual(3, even.Count());
        }

        [Test]
        public void Summation() {}

        [Test]
        public void SumWithSelector() {
            // 짝수이어야 합니다.
            const int NumberCount = 1000;

            var numbers = Enumerable.Range(1, NumberCount);
            var sum = (double)numbers.Sum();

            // normailize와 같다.
            double norm = numbers.Sum(n => n / sum);

            Assert.AreEqual(1, norm);
        }

        [Test]
        public void MultiThread_Test() {
            TestTool.RunTasks(5,
                              () => {
#if !SILVERLIGHT
                                  ByHierarchy();
                                  BinaryTreePreOrderScan();
                                  BinaryTreeInOrderScan();
                                  BinaryTreePostOrderScan();
                                  BinaryTreeInRevOrderScan();
#endif
                                  GraphBreadFirstSearch();
                                  GraphDepthFirstSearch();

                                  Between();
                                  InTest();
                                  Summation();
                                  SumWithSelector();
                              });
        }

        private void BuildGraph(Vertex<int> startVertex, int depth) {
            if(depth > MaxDepth)
                return;

            int adjacentCount = Rnd.Next(MinAdjacent, MaxAdjacent);

            for(int i = 0; i < adjacentCount; i++) {
                startVertex.Adjacents.Add(new Vertex<int>(startVertex.Value + 1));
                BuildGraph(startVertex.Adjacents[i], depth + 1);
            }
        }
    }
}