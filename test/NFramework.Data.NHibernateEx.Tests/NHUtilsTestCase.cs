using System;
using System.Linq;
using NHibernate;
using NSoft.NFramework.Data.NHibernateEx.Domain;
using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.Data.NHibernateEx {
    [TestFixture]
    public class NHUtilsTestCase : NHRepositoryTestFixtureBase {
        // NOTE: Stateless 작업은 실제 DB 파일이 있어야 합니다. 
        protected override DatabaseEngine GetDatabaseEngine() {
            return DatabaseEngine.MsSql2005;
        }

        [Test]
        public void IsMappedEntity_Generic() {
            Assert.IsTrue(UnitOfWork.CurrentSessionFactory.IsMappedEntity<Parent>());
            Assert.IsTrue(UnitOfWork.CurrentSessionFactory.IsMappedEntity<User>());

            Assert.IsFalse(UnitOfWork.CurrentSessionFactory.IsMappedEntity<ParentDTO>());
            Assert.IsFalse(UnitOfWork.CurrentSessionFactory.IsMappedEntity<IParent>());
        }

        [Test]
        public void IsMappedEntity_Type() {
            Assert.IsTrue(UnitOfWork.CurrentSessionFactory.IsMappedEntity(typeof(Parent)));
            Assert.IsTrue(UnitOfWork.CurrentSessionFactory.IsMappedEntity(typeof(User)));

            Assert.IsFalse(UnitOfWork.CurrentSessionFactory.IsMappedEntity(typeof(ParentDTO)));
            Assert.IsFalse(UnitOfWork.CurrentSessionFactory.IsMappedEntity(typeof(IParent)));

            // Parent의 Proxy일 수 있습니다.
            //
            var parent = Repository<Parent>.Query().First();

            Assert.IsTrue(UnitOfWork.CurrentSessionFactory.IsMappedEntity(parent.GetType()), "[{0}] 은 SessionFactory에 매핑되지 않았습니다.",
                          parent.GetType());
        }

        [Test]
        public void IsMappedEntity_Object() {
            // Parent의 Proxy일 수 있습니다.
            //
            var parent = Repository<Parent>.Query().First();

            Assert.IsTrue(UnitOfWork.CurrentSessionFactory.IsMappedEntity(parent), "[{0}] 은 SessionFactory에 매핑되지 않았습니다.",
                          parent.GetType());
        }

        [Test]
        public void CopyAndToTransientTest() {
            // Copy Parent 
            //
            var parent = Repository<Parent>.Query().First();
            Assert.IsNotNull(parent);
            Assert.IsNotNull(parent.Children);
            parent.Children.Count.Should().Be.GreaterThan(0);

            var transient = NHTool.CopyAndToTransient(parent);

            transient.IsTransient.Should().Be.True();
            transient.IsSaved.Should().Be.False();
            transient.Id.Should().Be(default(Guid));

            transient.Name.Should().Be(parent.Name);

            // 일반 속성만 복사되고, Reference 정보는 복사하지 않습니다.
            transient.Children.Count.Should().Be(0);

            // Copy children
            //
            var child = parent.Children[0];
            child.Should().Not.Be.Null();
            child.Parent.Should().Be(parent);

            var transientChild = NHTool.CopyAndToTransient(child);

            transientChild.Should().Not.Be.Null();
            transientChild.Id.Should().Be(default(Guid));
            transientChild.IsSaved.Should().Be.False();
            transientChild.IsTransient.Should().Be.True();

            transientChild.Name.Should().Be(child.Name);

            transientChild.Parent.Should().Be.Null();
        }

        [Test]
        public void LocalTest() {
            var id = parentsInDB[0].Id;
            var parentFromSession = Repository<Parent>.Get(id);

            var parents = UnitOfWork.CurrentSession.Local<Parent>().ToList();
            Assert.Greater(parents.Count, 0);

            foreach(var parent in parents)
                Assert.IsTrue(NHibernateUtil.IsInitialized(parent));
        }

        [Test]
        public void InitializeEntities_Test() {
            UnitOfWork.CurrentSessionFactory.Evict(typeof(User));
            UnitOfWork.CurrentSession.Clear();

            var users = Repository<User>.Query().ToList();
            users.InitializeEntities();

            foreach(var user in users)
                Assert.IsTrue(NHibernateUtil.IsPropertyInitialized(user, "Company"), "User의 Company 속성이 초기화되지 않았습니다");
        }

        [Test]
        public void InitializeEntity_Test() {
            UnitOfWork.CurrentSessionFactory.Evict(typeof(User));
            UnitOfWork.CurrentSession.Clear();

            var user = Repository<User>.Query().First();
            user.InitializeEntity();

            Assert.IsTrue(NHibernateUtil.IsPropertyInitialized(user, "Company"), "User의 Company 속성이 초기화되지 않았습니다");
        }
    }
}