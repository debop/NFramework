using FluentNHibernate.Mapping;
using NSoft.NFramework.Data.NHibernateEx.Domain.UserTypes;

namespace NSoft.NFramework.Data.NHibernateEx.Domain.Fluents.TreeNodeMappings {
    public class LocaledMetadataTreeNodeMapping : ClassMap<LocaledMetadataTreeNode> {
        public LocaledMetadataTreeNodeMapping() {
            Id(x => x.Id).GeneratedBy.Native();

            Map(x => x.Code).Not.Nullable();
            Map(x => x.Name).Not.Nullable();
            Map(x => x.Description).Length(MappingContext.MaxStringLength);
            Map(x => x.ExAttr).Length(MappingContext.MaxStringLength);


            // Localed
            HasMany<LocaledMetadataTreeNodeLocale>(x => x.LocaleMap)
                .Table("LocaledMetadataTreeNodeLocale".AsNamingText())
                .Access.CamelCaseField(Prefix.Underscore)
                .Cascade.AllDeleteOrphan()
                .ForeignKeyConstraintName("FK_LocMetaTreeNode_Loc")
                .AsMap<CultureUserType>("LocaleKey".AsNamingText())
                .Component(loc => {
                               loc.Map(x => x.Name).Not.Nullable();
                               loc.Map(x => x.Description).Length(MappingContext.MaxStringLength);
                               loc.Map(x => x.ExAttr).Length(MappingContext.MaxStringLength);
                           });

            // Metadata
            HasMany<MetadataValue>(x => x.MetadataMap)
                .Table("LocaledMetadataTreeNodeMeta".AsNamingText())
                .Access.CamelCaseField(Prefix.Underscore)
                .Cascade.AllDeleteOrphan()
                .ForeignKeyConstraintName("FK_LocMetaTreeNode_Meta")
                .AsMap<string>("MetaKey".AsNamingText())
                .Component(m => {
                               m.Map(x => x.Value).Column("MetaValue".AsNamingText()).Length(MappingContext.MaxStringLength);
                               m.Map(x => x.ValueType).Column("MetaValueType".AsNamingText());
                           });

            // TreeNode 
            //
            References(x => x.Parent)
                .LazyLoad(Laziness.Proxy)
                .Fetch.Select()
                .Cascade.None()
                .LazyLoad(Laziness.Proxy);

            HasMany(x => x.Children)
                .Access.CamelCaseField(Prefix.Underscore)
                .ForeignKeyConstraintName("FK_LocMetaTreeNode_Child")
                .Inverse()
                .LazyLoad()
                .Cascade.AllDeleteOrphan()
                .AsSet();

            Component<TreeNodePosition>(x => x.NodePosition,
                                        cp => {
                                            cp.Map(x => x.Level).Column("TreeLevel");
                                            cp.Map(x => x.Order).Column("TreeOrder");
                                        });

            // IUpdateTimestampEntity
            Map(x => x.UpdateTimestamp).CustomType("Timestamp");
        }
    }
}