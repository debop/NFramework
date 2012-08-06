using FluentNHibernate.Mapping;

namespace NSoft.NFramework.Data.NHibernateEx.Domain.Fluents.InheritanceMappings.TablePerSubclass {
    public class AnimalMapping : ClassMap<FAnimal> {
        public AnimalMapping() {
            Table("F_JOINED_SUBCLASS_ANIMAL");
            DynamicInsert();
            DynamicUpdate();
            LazyLoad();

            Id(x => x.Id).GeneratedBy.Native();

            Map(x => x.Name).Not.Nullable();
            Map(x => x.Age);
        }
    }

    public class DogMapping : SubclassMap<FDog> {
        public DogMapping() {
            Table("F_JOINED_SUBCLASS_DOG");
            DynamicInsert();
            DynamicUpdate();
            LazyLoad();

            KeyColumn("AnimalId");

            Map(x => x.IsPet);
        }
    }

    public class PigMapping : SubclassMap<FPig> {
        public PigMapping() {
            Table("F_JOINED_SUBCLASS_PIG");
            DynamicInsert();
            DynamicUpdate();
            LazyLoad();

            KeyColumn("AnimalId");

            Map(x => x.ForMeat);
        }
    }
}