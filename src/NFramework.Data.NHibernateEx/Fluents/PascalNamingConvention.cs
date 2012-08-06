using System.Collections.Generic;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.AcceptanceCriteria;
using FluentNHibernate.Conventions.Inspections;
using FluentNHibernate.Conventions.Instances;
using NSoft.NFramework.LinqEx;

namespace NSoft.NFramework.Data.NHibernateEx.Fluents {
    /// <summary>
    /// Pascal Naming 규칙에 따른 RDBMS 명명규칙을 정의한 FluentNHibernate Convention 클래스입니다.
    /// 참고: http://wiki.fluentnhibernate.org/Conventions#Writing_Your_Own_Conventions
    /// </summary>
    public class PascalNamingConvention : FluentConventionBase,
                                          IIdConvention,
                                          ICompositeIdentityConvention,
                                          IHasManyConvention,
                                          IHasManyToManyConvention,
                                          IPropertyConvention,
                                          IReferenceConvention,
                                          IComponentConvention,
                                          IConventionAcceptance<IClassInspector> {
        /// <summary>
        /// 기본 생성자
        /// </summary>
        public PascalNamingConvention() : this(null, null) {}

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="propertyWithClassNames">컬럼명을 클래스명 + 속성명 으로 표현할 속성명 컬렉션</param>
        /// <param name="abbrNameMap">약어 맵</param>
        public PascalNamingConvention(IEnumerable<string> propertyWithClassNames = null, IDictionary<string, string> abbrNameMap = null)
            : base(ConventionOptions.Pascal, propertyWithClassNames, abbrNameMap) {
            MappingContext.NamingRule = NamingRuleKind.Pascal;
        }

        /// <summary>
        /// 클래스 인스턴스에 대한 Convention 적용
        /// </summary>
        public override void Apply(IClassInstance instance) {
            base.Apply(instance);

            var tableName = string.Concat(NHTool.EntityWrapperChar,
                                          Options.TableNamePrefix.AsText(),
                                          GetAbbrName(instance.EntityType.Name),
                                          Options.TableNameSurfix.AsText(),
                                          NHTool.EntityWrapperChar);
            instance.Table(tableName);
        }

        /// <summary>
        /// 클래스 Identifier에 대한 Convention 적용
        /// </summary>
        public void Apply(IIdentityInstance instance) {
            instance.Column(GetAbbrName(instance.EntityType.Name) + Options.PrimaryKeySurfix);
        }

        /// <summary>
        /// Composite Identifier에 대한 Convention 적용
        /// </summary>
        public void Apply(ICompositeIdentityInstance instance) {
            instance.KeyManyToOnes.RunEach(x => x.ForeignKey(GetAbbrName(x.Name) + Options.ForeignKeySurfix));
        }

        /// <summary>
        /// one-to-many 관계에 해당하는 부분의 Convention 적용
        /// </summary>
        public void Apply(IOneToManyCollectionInstance instance) {
            instance.Key.Column(GetAbbrName(instance.EntityType.Name) + Options.PrimaryKeySurfix);
        }

        /// <summary>
        /// many-to-many 관계에 해당하는 매핑 부분에 Convention을 적용합니다.
        /// </summary>
        public void Apply(IManyToManyCollectionInstance instance) {
            instance.Key.Column(GetAbbrName(instance.EntityType.Name) + Options.PrimaryKeySurfix);
            instance.Relationship.Column(GetAbbrName(instance.ChildType.Name) + Options.ForeignKeySurfix);
        }

        /// <summary>
        /// 속성에 대해 Convention을 적용합니다.
        /// </summary>
        public void Apply(IPropertyInstance instance) {
            if(PropertyWithClassNames.Contains(instance.Property.Name))
                instance.Column(GetAbbrName(instance.EntityType.Name) + GetAbbrName(instance.Name));
            else
                instance.Column(GetAbbrName(instance.Name));
        }

        /// <summary>
        /// many-to-one 관계에 대한 Convention을 적용합니다.
        /// </summary>
        public void Apply(IManyToOneInstance instance) {
            instance.Column(GetAbbrName(instance.Property.Name) + Options.ForeignKeySurfix);
        }

        /// <summary>
        /// component에 대해 convention 을 적용합니다.
        /// </summary>
        public void Apply(IComponentInstance instance) {
            instance.Properties.RunEach(p => p.Column(GetAbbrName(instance.Name) + GetAbbrName(p.Name)));
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