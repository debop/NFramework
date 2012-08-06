using System;
using System.Linq;
using FluentNHibernate.Testing;
using NHibernate.Linq;
using NSoft.NFramework.Data.DevartOracle.NH.Domains.Models;
using NSoft.NFramework.Data.NHibernateEx;
using NSoft.NFramework.Data.NHibernateEx.ForTesting;
using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.Data.DevartOracle.NH.Domains {
    [TestFixture]
    public class MappingTestCase : FluentDomainTestCaseBase {
        [Test]
        public void SimpleEntity() {
            Repository<Company>.DeleteAll();
            UnitOfWork.Current.Flush();
            UnitOfWork.Current.Clear();

            var company = new Company("NSoft") { Name = "리얼웹" };

            Repository<Company>.SaveOrUpdate(company);

            UnitOfWork.Current.Flush();
            UnitOfWork.Current.Clear();

            var savedCompany = UnitOfWork.CurrentSession.Query<Company>().Where(c => c.Code == "NSoft").Single();

            Assert.AreEqual(company.Code, savedCompany.Code);
            Assert.AreEqual(company.Name, savedCompany.Name);

            Repository<Company>.DeleteAll();
            UnitOfWork.Current.Flush();
            UnitOfWork.Current.Clear();
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
            UnitOfWork.Current.Flush();
            UnitOfWork.Current.Clear();

            var company = new Company("RW") { Name = "RealWeb" };
            Repository<Company>.SaveOrUpdate(company);

            for(int i = 0; i < 10; i++)
                Repository<Company>.SaveOrUpdate(new Company("RW_" + i.ToString()) { Name = "RealWeb_" + i.ToString() });

            UnitOfWork.Current.Flush();
            UnitOfWork.Current.Clear();

            var savedCompany = Repository<Company>.FindAll(1, 5);
            savedCompany.Count.Should().Be(5);

            Repository<Company>.DeleteAll();
            UnitOfWork.Current.TransactionalFlush();
            UnitOfWork.Current.Clear();
        }

        [Test]
        public void OneToOneTest() {
            var project = new Project
                          {
                              Code = "PRJ-" + Guid.NewGuid().ToString(),
                              Name = "프로젝트 명"
                          };

            project.AddLocale(English, new ProjectLocale { Name = "Project Name" });

            project.Field.Name = project.Name + " - Field";
            project.Field.AddLocale(English, new ProjectFieldLocale { Name = "ProjectField Name" });

            Repository<Project>.SaveOrUpdate(project);

            UnitOfWork.CurrentSession.Flush();
            UnitOfWork.Current.Clear();

            project = UnitOfWork.CurrentSession.Query<Project>().FirstOrDefault();
            Assert.IsNotNull(project);

            var projects = UnitOfWork.CurrentSession.Local<Project>().ToList();
            Assert.Greater(projects.Count, 0);

            Repository<Project>.Delete(project);
            UnitOfWork.Current.TransactionalFlush();

            var projectCount = UnitOfWork.CurrentSession.Query<Project>().Count();
            Assert.AreEqual(0, projectCount);
        }

        [Test]
        public void Project_ProjectField_One_To_One_Test() {
            var project = new Project
                          {
                              Code = "PRJ-" + Guid.NewGuid(),
                              Name = "프로젝트 명"
                          };

            project.AddLocale(English, new ProjectLocale { Name = "Project Name" });

            project.Field.Name = project.Name + " - Field";
            project.Field.AddLocale(English, new ProjectFieldLocale { Name = "ProjectField Name" });

            UnitOfWork.CurrentSession.Specification<Project>()
                .VerifyTheMappings(project);
        }
    }
}