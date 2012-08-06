using System;
using AutoMapper;
using NSoft.NFramework.Caching.Domain.Model;
using NSoft.NFramework.Data.NHibernateEx;
using NUnit.Framework;

namespace NSoft.NFramework.Caching.Domain {
    /// <summary>
    /// AutoMapper 를 이용하여, Association 을 가진 엔티티도 DTO로 매핑이 가능합니다. 
    /// 즉 Parent-Child 를 ParentDTO-ChildDTO 로 매핑이 가능합니다.
    /// </summary>
    [TestFixture]
    public class DtoMappingFixture : NHRepositoryTestFixtureBase {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        protected override void OnTestFixtureSetUp() {
            base.OnTestFixtureSetUp();

            Mapper.CreateMap<Parent, ParentDTO>();
            Mapper.CreateMap<Child, ChildDTO>();
            Mapper.AssertConfigurationIsValid();
        }

        [Test]
        public void MapToDto() {
            UnitOfWork.CurrentSession.Clear();

            var parent = Repository<Parent>.Get(parentsInDB[0].Id);

            Assert.IsNotNull(parent);
            Assert.AreEqual(parentsInDB[0].Name, parent.Name);
            Assert.AreEqual(2, parentsInDB[0].Children.Count);
            Assert.AreEqual(2, parent.Children.Count);

            var parentDTO = Mapper.Map<Parent, ParentDTO>(parent);
            Assert.IsNotNull(parentDTO);
            Assert.AreEqual(parentsInDB[0].Name, parentDTO.Name);
            Assert.AreEqual(2, parentDTO.Children.Count);

            foreach(var childDto in parentDTO.Children)
                Console.WriteLine(childDto);
        }
    }
}