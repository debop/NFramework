using System.Collections.Generic;
using NHibernate.Tool.hbm2ddl;
using NSoft.NFramework.Data.NHibernateEx.UnitOfWorks.MultipleUnitOfWorkArtifacts;
using NSoft.NFramework.InversionOfControl;
using NUnit.Framework;

namespace NSoft.NFramework.Data.NHibernateEx.UnitOfWorks {
    /// <summary>
    /// 복수의 SessionFactory를 UnitOfWork 패턴에서 한번에 사용할 수 있도록 하였습니다.
    /// </summary>
    public abstract class NHMultipleUnitOfWorkFactoryFixtureBase {
        private IUnitOfWork _unitOfWork;
        private List<SchemaExport> _schemas;

        [TestFixtureSetUp]
        public void FixtureSetUp() {
            OnFixtureSetUp();
        }

        [SetUp]
        public void SetUp() {
            OnSetUp();
        }

        [TearDown]
        public void TearDown() {
            OnTearDown();
        }

        [TestFixtureTearDown]
        public void FixtureTearDown() {
            OnFixtureTearDown();
        }

        protected virtual void OnFixtureSetUp() {
            InitializeIoC();

            _unitOfWork = UnitOfWork.Start();


            // create databases
            var sessions = new[]
                           {
                               UnitOfWork.GetCurrentSessionFor(typeof(DomainObjectFromDatabase1)),
                               UnitOfWork.GetCurrentSessionFor(typeof(DomainObjectFromDatabase2))
                           };

            for(int i = 0; i < sessions.Length; i++)
                _schemas[i].Execute(false, true, false, sessions[i].Connection, null);

            // insert test data and evict from session
            NHWith.Transaction(delegate {
                                   sessions[0].Evict(Repository<DomainObjectFromDatabase1>.Save(new DomainObjectFromDatabase1("foo")));
                                   sessions[1].Evict(Repository<DomainObjectFromDatabase2>.Save(new DomainObjectFromDatabase2("bar")));
                               });
        }

        protected virtual void OnSetUp() {}

        protected virtual void OnTearDown() {}

        protected virtual void OnFixtureTearDown() {
            _unitOfWork.Dispose();
            _schemas.ForEach(schema => schema.Drop(false, true));
            IoC.Reset();
        }

        protected virtual string WindsorConfigurationFilePath {
            get { return @".\UnitOfWorks\IoC.MultipleUnitOfWork.config"; }
        }

        protected NHMultipleUnitOfWorkFactory MultiUnitOfWorkFactory {
            get { return (NHMultipleUnitOfWorkFactory)IoC.Resolve<IUnitOfWorkFactory>(); }
        }

        private void InitializeIoC() {
            IoC.InitializeFromXmlFile(WindsorConfigurationFilePath);

            // load schemas to create databases
            _schemas = new List<SchemaExport>();

            foreach(var factory in MultiUnitOfWorkFactory) {
                _schemas.Add(new SchemaExport(factory.Configuration));
            }
        }
    }
}