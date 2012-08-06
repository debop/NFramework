using FluentNHibernate.Mapping;

namespace NSoft.NFramework.Data.NHibernateEx.Domain {
    /// <summary>
    /// Join Mapping 에 대한 예제입니다.
    /// </summary>
    public class PersonMap : ClassMap<Person> {
        public PersonMap() {
            Table("Person");

            Id(x => x.Id).GeneratedBy.Native();

            Map(x => x.Name).Not.Nullable();

            Join("PersonAddress",
                 m => {
                     m.Fetch.Join();
                     m.KeyColumn("PersonId");

                     m.Map(p => p.City);
                     m.Map(p => p.Country);
                     m.Map(p => p.Zip);
                 });
        }
    }
}