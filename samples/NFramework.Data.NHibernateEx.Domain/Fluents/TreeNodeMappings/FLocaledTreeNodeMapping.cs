using FluentNHibernate.Mapping;
using NSoft.NFramework.Data.NHibernateEx.Domain.UserTypes;

namespace NSoft.NFramework.Data.NHibernateEx.Domain.Fluents.TreeNodeMappings {
    public class FLocaledTreeNodeMapping : ClassMap<FLocaledTreeNode> {
        public FLocaledTreeNodeMapping() {
            Id(x => x.Id).GeneratedBy.Native();

            Map(x => x.Code).Not.Nullable();
            Map(x => x.Name).Not.Nullable();
            Map(x => x.Description).Length(MappingContext.MaxStringLength);
            Map(x => x.ExAttr).Length(MappingContext.MaxStringLength);


            // Localed
            HasMany<FLocaledTreeNodeLocale>(x => x.LocaleMap)
                .Table("FLocaledTreeNodeLocale".AsNamingText())
                .Access.CamelCaseField(Prefix.Underscore)
                .Cascade.AllDeleteOrphan()
                .AsMap<CultureUserType>("LocaleKey".AsNamingText())
                .Component(loc => {
                               loc.Map(x => x.Name).Not.Nullable();
                               loc.Map(x => x.Description).Length(MappingContext.MaxStringLength);
                               loc.Map(x => x.ExAttr).Length(MappingContext.MaxStringLength);
                           });


            // TreeNode 
            //
            References(x => x.Parent)
                .LazyLoad(Laziness.Proxy)
                .Fetch.Select()
                .Cascade.SaveUpdate();


            HasMany(x => x.Children)
                .Access.CamelCaseField(Prefix.Underscore)
                .Inverse()
                .LazyLoad()
                .Cascade.AllDeleteOrphan()
                .AsSet();

            Component<TreeNodePosition>(x => x.NodePosition,
                                        cp => {
                                            cp.Map(x => x.Level).Column("TreeLevel".AsNamingText());
                                            cp.Map(x => x.Order).Column("TreeOrder".AsNamingText());
                                        });

            // IUpdateTimestampEntity
            Map(x => x.UpdateTimestamp).CustomType("Timestamp");
        }
    }
}