using System;
using System.Linq;
using FluentNHibernate.Testing;
using NHibernate.Linq;
using NSoft.NFramework.Data.NHibernateEx;
using NSoft.NFramework.Data.NHibernateEx.Domain;
using NSoft.NFramework.Data.NHibernateEx.ForTesting;
using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.Data.SQLite.NHibernateEx.Domains {
    [TestFixture]
    public class MappingTestCase : FluentDomainTestCaseBase {
        [Test]
        public void SimpleEntity() {
            Repository<Company>.DeleteAll();
            UnitOfWork.Current.TransactionalFlush();
            UnitOfWork.CurrentSession.Clear();

            var company = new Company("NSoft") { Name = "리얼웹" };

            Repository<Company>.SaveOrUpdate(company);

            UnitOfWork.Current.TransactionalFlush();
            UnitOfWork.CurrentSession.Clear();

            var savedCompany = UnitOfWork.CurrentSession.Query<Company>().Single(c => c.Code == "NSoft");

            Assert.AreEqual(company.Code, savedCompany.Code);
            Assert.AreEqual(company.Name, savedCompany.Name);
        }

        [Test]
        public void CompanyTest() {
            UnitOfWork.CurrentSession.Specification<Company>()
                .CheckProperty(x => x.Code, "NSoft")
                .CheckProperty(x => x.Name, "리얼웹")
                .VerifyTheMappings();
        }

        [Test]
        public void PagingEntities() {
            Repository<Company>.DeleteAll();
            UnitOfWork.Current.TransactionalFlush();
            UnitOfWork.CurrentSession.Clear();


            var company = new Company("RW") { Name = "RealWeb" };
            Repository<Company>.SaveOrUpdate(company);

            for(int i = 0; i < 10; i++)
                Repository<Company>.SaveOrUpdate(new Company("RW_" + i) { Name = "RealWeb_" + i });

            UnitOfWork.Current.TransactionalFlush();
            UnitOfWork.CurrentSession.Clear();

            var savedCompany = Repository<Company>.FindAll(1, 5);
            savedCompany.Count.Should().Be(5);

            Repository<Company>.DeleteAll();
            UnitOfWork.Current.TransactionalFlush();
            UnitOfWork.CurrentSession.Clear();
        }

        [Test]
        public void OneToOneTest() {
            var project = new FluentProject
                          {
                              Code = "PRJ-" + Guid.NewGuid().ToString(),
                              Name = "프로젝트 명"
                          };

            project.AddLocale(English, new FluentProjectLocale { Name = "FluentProject Name" });

            project.Field.Name = project.Name + " - Field";
            project.Field.AddLocale(English, new FluentProjectFieldLocale { Name = "FluentProjectField Name" });

            Repository<FluentProject>.SaveOrUpdate(project);

            UnitOfWork.CurrentSession.Flush();
            UnitOfWork.Current.Clear();

            project = UnitOfWork.CurrentSession.Query<FluentProject>().FirstOrDefault();
            Assert.IsNotNull(project);

            var projects = UnitOfWork.CurrentSession.Local<FluentProject>().ToList();
            Assert.Greater(projects.Count, 0);

            Repository<FluentProject>.Delete(project);
            UnitOfWork.Current.TransactionalFlush();

            var projectCount = UnitOfWork.CurrentSession.Query<FluentProject>().Count();
            Assert.AreEqual(0, projectCount);
        }

        [Test]
        public void Project_ProjectField_One_To_One_Test() {
            var project = new FluentProject
                          {
                              Code = "PRJ-" + Guid.NewGuid(),
                              Name = "프로젝트 명"
                          };

            project.AddLocale(English, new FluentProjectLocale { Name = "FluentProject Name" });

            project.Field.Name = project.Name + " - Field";
            project.Field.AddLocale(English, new FluentProjectFieldLocale { Name = "FluentProjectField Name" });

            UnitOfWork.CurrentSession.Specification<FluentProject>()
                .VerifyTheMappings(project);
        }
    }
}