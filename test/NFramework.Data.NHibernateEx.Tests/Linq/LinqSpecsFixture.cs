using System.Linq;
using NSoft.NFramework.Data.NHibernateEx.Domain;
using NSoft.NFramework.Data.NHibernateEx.Linq.Specifications;
using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.Data.NHibernateEx.Linq {
    //
    // NOTE: http://linqspecs.codeplex.com 의 LinqSpecs 은 LINQ를 이용하여 질의어를 정의하고, 이를 NHibernate의 LINQ와 결합하여 사용할 수 있도록 했습니다.
    //
    [TestFixture]
    public class LinqSpecsFixture : NHRepositoryTestFixtureBase {
        [Test]
        public void ParentByNameTest() {
            var parents =
                Repository<Parent>
                    .Query()
                    .Where(new ParentNameBy("Parent1").IsSatisfiedBy())
                    .ToList();

            parents.Should().Not.Be.Null();
            parents.Count.Should().Be(1);
        }

        [Test]
        public void ParentByChildNameTest() {
            var parents =
                Repository<Parent>
                    .Query()
                    .Where(new ParentHasChild("child1-of-parent4").IsSatisfiedBy())
                    .ToList();

            parents.Should().Not.Be.Null();
            parents.Count.Should().Be(1);
        }
    }
}