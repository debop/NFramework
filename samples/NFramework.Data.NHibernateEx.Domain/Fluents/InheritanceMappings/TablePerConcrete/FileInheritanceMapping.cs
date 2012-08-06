using FluentNHibernate.Mapping;

namespace NSoft.NFramework.Data.NHibernateEx.Domain.Fluents.InheritanceMappings.TablePerConcrete {
    /// <summary>
    /// Table Per Concrete class (union-subclass)
    /// </summary>
    public class FileInheritanceMapping : ClassMap<FileBase> {
        public FileInheritanceMapping() {
            //! NOTE: union-subclass 를 수행합니다. ( ConcreteClass 마다 Table을 가짐 )
            UseUnionSubclassForInheritanceMapping();

            DynamicInsert();
            DynamicUpdate();
            LazyLoad();

            Id(x => x.Id).Column("FileId").GeneratedBy.Assigned();

            Map(x => x.Name).Column("FileName").Not.Nullable();
            Map(x => x.Size).Column("FileSize");
        }
    }

    public class IssueFileMapping : SubclassMap<IssueFile> {
        public IssueFileMapping() {
            Table("IssueFile");

            Map(x => x.IssueCode);
        }
    }

    public class RiskFileMapping : SubclassMap<RiskFile> {
        public RiskFileMapping() {
            Table("RiskFile");

            Map(x => x.RiskCode);
        }
    }
}