using NSoft.NFramework.Data.NHibernateEx.UnitOfWorks.MultipleUnitOfWorkArtifacts;
using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.Data.NHibernateEx.UnitOfWorks {
    /// <summary>
    /// 복수개의 Database Source를 하나의 UnitOfWork에서 사용할 수 있도록 해주는 NHMultipleUnitOfWorkFactory에 대한 테스트입니다.
    /// </summary>
    [TestFixture]
    public class NHMultipleUnitOfWorkFactoryFixture : NHMultipleUnitOfWorkFactoryFixtureBase {
        [Test]
        public void SchemaGeneratedTest() {
            Repository<DomainObjectFromDatabase1>.Count().Should().Be(1);
            Repository<DomainObjectFromDatabase2>.Count().Should().Be(1);
        }

        [Test]
        public void ResolveMultipleUnitOfWorkFacotryTest() {
            var session1 = UnitOfWork.UnitOfWorkFactory.GetCurrentSessionFor<DomainObjectFromDatabase1>();
            var session2 = UnitOfWork.UnitOfWorkFactory.GetCurrentSessionFor<DomainObjectFromDatabase2>();

            Assert.IsNotNull(session1);
            Assert.IsNotNull(session2);
            Assert.AreNotEqual(session1, session2);

            Repository<DomainObjectFromDatabase1>.Session.Should().Not.Be.Null();
            Repository<DomainObjectFromDatabase2>.Session.Should().Not.Be.Null();

            Repository<DomainObjectFromDatabase1>.Count().Should().Be.GreaterThanOrEqualTo(1);
            Repository<DomainObjectFromDatabase2>.Count().Should().Be.GreaterThanOrEqualTo(1);
        }

        [Test]
        public void TransactionFlushTest() {
            const int ItemCount = 100;

            for(var i = 0; i < ItemCount; i++) {
                Repository<DomainObjectFromDatabase1>.SaveOrUpdate(new DomainObjectFromDatabase1("database1_transactional_flush_" + i));
                Repository<DomainObjectFromDatabase2>.SaveOrUpdate(new DomainObjectFromDatabase2("database2_transactional_flush_" + i));
            }
            UnitOfWork.Current.TransactionalFlush();
            UnitOfWork.Current.Clear();

            Repository<DomainObjectFromDatabase1>.Count().Should().Be.GreaterThanOrEqualTo(ItemCount);
            Repository<DomainObjectFromDatabase2>.Count().Should().Be.GreaterThanOrEqualTo(ItemCount);

            Repository<DomainObjectFromDatabase1>.DeleteAll();
            Repository<DomainObjectFromDatabase2>.DeleteAll();

            UnitOfWork.Current.TransactionalFlush();
            UnitOfWork.Current.Clear();

            Repository<DomainObjectFromDatabase1>.Count().Should().Be(0);
            Repository<DomainObjectFromDatabase2>.Count().Should().Be(0);
        }
    }
}