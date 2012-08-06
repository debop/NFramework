using System.Collections.Generic;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.AcceptanceCriteria;
using FluentNHibernate.Conventions.Inspections;
using FluentNHibernate.Conventions.Instances;
using NSoft.NFramework.LinqEx;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.Data.NHibernateEx.Fluents {
    /// <summary>
    /// Oracle DB에서 사용되는 명명규칙을 기준으로 FluentNHibernate Convention 을 구현합니다.
    /// 참고: http://wiki.fluentnhibernate.org/Conventions#Writing_Your_Own_Conventions
    /// 참고: http://www.oracle-base.com/articles/misc/NamingConventions.php
    /// </summary>
    public class OracleNamingConvention : FluentConventionBase,
                                          IIdConvention,
                                          ICompositeIdentityConvention,
                                          IHasManyConvention,
                                          IHasManyToManyConvention,
                                          IPropertyConvention,
                                          IReferenceConvention,
                                          IComponentConvention,
                                          IConventionAcceptance<IClassInspector> {
        public const string Delimiter = "_";

        public OracleNamingConvention() : this(null, null) {}

        public OracleNamingConvention(IEnumerable<string> propertyWithClassNames = null, IDictionary<string, string> abbrNameMap = null)
            : base(ConventionOptions.Oracle, propertyWithClassNames, abbrNameMap) {
            MappingContext.NamingRule = NamingRuleKind.Oracle;
        }

        /// <summary>
        /// Apply changes to the target
        /// </summary>
        public override void Apply(IClassInstance instance) {
            base.Apply(instance);

            var tablePrefix = Options.TableNamePrefix ?? string.Empty;
            if(tablePrefix.IsNotWhiteSpace() && tablePrefix.EndsWith(Delimiter) == false)
                tablePrefix += Delimiter;

            var tableSurfix = Options.TableNameSurfix ?? string.Empty;
            if(tableSurfix.IsNotWhiteSpace() && tableSurfix.StartsWith(Delimiter) == false)
                tableSurfix = Delimiter + tableSurfix;

            var tableName = string.Concat(NHTool.EntityWrapperChar,
                                          tablePrefix,
                                          GetAbbrName(instance.EntityType.Name).ToOracleNaming(),
                                          tableSurfix,
                                          NHTool.EntityWrapperChar);

            instance.Table(tableName);
        }

        /// <summary>
        /// Apply changes to the target
        /// </summary>
        public void Apply(IIdentityInstance instance) {
            instance.Column(GetAbbrName(instance.EntityType.Name).ToOracleNaming() + Delimiter + Options.PrimaryKeySurfix);
        }

        /// <summary>
        /// Apply changes to the target
        /// </summary>
        public void Apply(ICompositeIdentityInstance instance) {
            instance.KeyManyToOnes.RunEach(
                x => x.ForeignKey(GetAbbrName(x.Name).ToOracleNaming() + Delimiter + Options.ForeignKeySurfix));
        }

        /// <summary>
        /// Apply changes to the target
        /// </summary>
        public void Apply(IOneToManyCollectionInstance instance) {
            instance.Key.Column(GetAbbrName(instance.EntityType.Name).ToOracleNaming() + Delimiter + Options.PrimaryKeySurfix);
        }

        /// <summary>
        /// Apply changes to the target
        /// </summary>
        public void Apply(IManyToManyCollectionInstance instance) {
            instance.Key.Column(GetAbbrName(instance.EntityType.Name).ToOracleNaming() + Delimiter + Options.PrimaryKeySurfix);
            instance.Relationship.Column(GetAbbrName(instance.ChildType.Name).ToOracleNaming() + Delimiter + Options.ForeignKeySurfix);
        }

        /// <summary>
        /// Apply changes to the target
        /// </summary>
        public void Apply(IPropertyInstance instance) {
            if(PropertyWithClassNames.Contains(instance.Property.Name))
                instance.Column(GetAbbrName(instance.EntityType.Name).ToOracleNaming() + Delimiter +
                                GetAbbrName(instance.Name).ToOracleNaming());

            else
                instance.Column(GetAbbrName(instance.Name).ToOracleNaming());
        }

        /// <summary>
        /// Apply changes to the target
        /// </summary>
        public void Apply(IManyToOneInstance instance) {
            instance.Column(GetAbbrName(instance.Property.Name).ToOracleNaming() + Delimiter + Options.ForeignKeySurfix);
        }

        /// <summary>
        /// Apply changes to the target
        /// </summary>
        public void Apply(IComponentInstance instance) {
            instance.Properties.RunEach(
                p => p.Column(GetAbbrName(instance.Name).ToOracleNaming() + Delimiter + GetAbbrName(p.Name).ToOracleNaming()));
        }

        /// <summary>
        /// Whether this convention will be applied to the target.
        /// </summary>
        /// <param name="criteria">Instace that could be supplied</param>
        /// <returns>
        /// Apply on this target?
        /// </returns>
        public void Accept(IAcceptanceCriteria<IClassInspector> criteria) {
            // Nothing to do.
        }
                                          }
}