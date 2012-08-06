using System;
using NSoft.NFramework.LinqEx;
using NSoft.NFramework.Raven.Entities;
using NUnit.Framework;
using Raven.Client.Embedded;
using SharpTestsEx;

namespace NSoft.NFramework.Raven.GettingStarts {
    [TestFixture]
    public class ClientApiFixture {
        [Test]
        public void RunInMemoryTest() {
            using(var documentStore = new EmbeddableDocumentStore { RunInMemory = true }) {
                documentStore.Initialize();
            }
        }

        [Test]
        public void InitializeTest() {
            using(var documentStore = new EmbeddableDocumentStore { DataDirectory = "Data" }) {
                documentStore.Initialize();
                documentStore.DataDirectory.Should().Contain("Data");
            }
        }

        [Test]
        public void StoreTest() {
            using(var store = new EmbeddableDocumentStore { RunInMemory = true }.Initialize())
            using(var session = store.OpenSession()) {
                session.Store(new Company { Name = "Company 1", Region = "Korea" });
                session.Store(new Company { Name = "Company 2", Region = "Pusan" });
                session.SaveChanges();

                var companies = session.Query<Company>();

                companies.RunEach(Console.WriteLine);
            }
        }
    }
}