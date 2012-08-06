using System.Collections.Generic;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;

namespace NSoft.NFramework.Data.NHibernateEx.Fluents {
    /// <summary>
    /// FluentNHibernate 의 Convention 기능을 활용하기 위한 기본 클래스입니다.
    /// </summary>
    public class FluentConventionBase : IFluentConvention, IClassConvention {
        private IList<string> _propertyWithClassNames = new List<string>();

        protected FluentConventionBase() : this(ConventionOptions.Default) {}

        protected FluentConventionBase(ConventionOptions options,
                                       IEnumerable<string> propertyWithClassNames = null,
                                       IDictionary<string, string> abbrNameMap = null) {
            Options = options ?? ConventionOptions.Pascal;

            if(propertyWithClassNames != null)
                _propertyWithClassNames = new List<string>(propertyWithClassNames);

            if(abbrNameMap != null)
                MappingContext.AbbrNameMap = abbrNameMap;
        }

        /// <summary>
        /// Convention Option
        /// </summary>
        public ConventionOptions Options { get; set; }

        /// <summary>
        /// 속성명과 컬럼명을 같은 값으로 유지하고자 하는 속성명 (Description, ExAttr 같은 것)
        /// 그 외의 것은 컬럼명이 EntityName + '_' + PropertyName 으로 매핑됩니다. (예: Company.Name 은 COMPANY_NAME)
        /// </summary>
        public IList<string> PropertyWithClassNames {
            get { return _propertyWithClassNames ?? (_propertyWithClassNames = new List<string>()); }
            set { _propertyWithClassNames = value; }
        }

        /// <summary>
        /// 컬럼명으로 매핑시에 약어로 매핑해야 할 이름(단어) 매핑이다. (예: Department-Dept, Locale-Loc, Configuration-Conf 등)
        /// </summary>
        public IDictionary<string, string> AbbrNameMap {
            get { return MappingContext.AbbrNameMap; }
            set { MappingContext.AbbrNameMap = value; }
        }

        /// <summary>
        /// Apply changes to the target
        /// </summary>
        public virtual void Apply(IClassInstance instance) {
            if(Options.DefaultLazy)
                instance.LazyLoad();

            if(Options.DynamicInsert)
                instance.DynamicInsert();

            if(Options.DynamicUpdate)
                instance.DynamicUpdate();
        }

        /// <summary>
        /// <paramref name="text"/>에 <see cref="AbbrNameMap"/>에 등록된 약어 변환 단어가 있더면 약어로 변환하여 반환합니다.
        /// </summary>
        /// <param name="text">원본 문자열</param>
        /// <returns></returns>
        public string GetAbbrName(string text) {
            return MappingContext.ToAbbrName(text);
        }
    }
}