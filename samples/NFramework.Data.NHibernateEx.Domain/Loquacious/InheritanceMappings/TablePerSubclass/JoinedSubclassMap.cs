using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace NSoft.NFramework.Data.NHibernateEx.Domain.Loquacious.InheritanceMappings.TablePerSubclass {
    public class CAnimalMap : ClassMapping<CAnimal> {
        public CAnimalMap() {
            Table("C_JOINED_SUB_ANIMAL");
            Lazy(false);
            DynamicInsert(true);
            DynamicUpdate(true);

            Id(x => x.Id, c => {
                              c.Generator(Generators.Native);
                              c.Column("AnimalId");
                          });

            Property(x => x.Name, c => c.NotNullable(true));
            Property(x => x.Age);
        }
    }

    public class CDogMap : JoinedSubclassMapping<CDog> {
        public CDogMap() {
            Table("C_JOINED_SUB_DOG");
            Lazy(false);
            DynamicInsert(true);
            DynamicUpdate(true);

            Key(km => {
                    km.Column("DogId");
                    km.ForeignKey("FK_CAnimal_CDog");
                });

            Property(x => x.IsPet);
        }
    }

    public class CPigMap : JoinedSubclassMapping<CPig> {
        public CPigMap() {
            Table("C_JOINED_SUB_PIG");
            Lazy(false);
            DynamicInsert(true);
            DynamicUpdate(true);

            Key(km => {
                    km.Column("PigId");
                    km.ForeignKey("FK_CAnimal_CPig");
                });

            Property(x => x.ForMeat);
        }
    }
}