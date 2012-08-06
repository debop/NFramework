using System.Collections.Generic;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Helpers;
using NSoft.NFramework.Reflections;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.Data.NHibernateEx.Fluents {
    /// <summary>
    /// Fluent NHibernate Convention 과련 옵션입니다.
    /// 참고: http://wiki.fluentnhibernate.org/Convention_shortcut
    /// </summary>
    public class ConventionOptions {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// Pascal Naming 규칙에 따른 기본 Convention입니다.
        /// </summary>
        public static ConventionOptions Default = new ConventionOptions
                                                  {
                                                      DefaultLazy = true,
                                                      DynamicInsert = true,
                                                      DynamicUpdate = true,
                                                      PrimaryKeySurfix = "Id",
                                                      ForeignKeySurfix = "Id"
                                                  };

        /// <summary>
        /// Pascal Naming 규칙에 따른 기본 Convention입니다.
        /// </summary>
        public static ConventionOptions Pascal = Default;

        /// <summary>
        /// Pascal Naming 규칙에 따른 기본 Convention입니다.
        /// </summary>
        public static ConventionOptions Oracle = new ConventionOptions
                                                 {
                                                     DefaultLazy = true,
                                                     DynamicInsert = true,
                                                     DynamicUpdate = true,
                                                     PrimaryKeySurfix = "ID",
                                                     ForeignKeySurfix = "ID"
                                                 };

        /// <summary>
        /// Fluent NHibernate 에서 제공하는 Convention 설정에 따른 <see cref="IConvention"/> 인스턴스를 빌드하여 제공합니다.
        /// </summary>
        /// <param name="options">ConventionOptions 인스턴스</param>
        /// <returns>Convention 설정 정보를 기초로 만든 <see cref="IConvention"/>의 인스턴스 배열</returns>
        public static IList<IConvention> ToConventions(ConventionOptions options) {
            options.ShouldNotBeNull("optioons");

            if(IsDebugEnabled)
                log.Debug("ConventionOptions 정보로 IConvention[]을 빌드합니다...  " + options);

            var conventions = new List<IConvention>();

            conventions.Add((options.DefaultLazy) ? LazyLoad.Always() : LazyLoad.Never());

            if(options.DynamicInsert)
                conventions.Add(FluentNHibernate.Conventions.Helpers.DynamicInsert.AlwaysTrue());
            if(options.DynamicUpdate)
                conventions.Add(FluentNHibernate.Conventions.Helpers.DynamicUpdate.AlwaysTrue());

            if(options.TableNamePrefix.IsNotWhiteSpace())
                conventions.Add(Table.Is(x => options.TableNamePrefix + x.EntityType.Name));
            else if(options.TableNamePrefix.IsNotWhiteSpace())
                conventions.Add(Table.Is(x => x.EntityType.Name + options.TableNameSurfix));
            else
                conventions.Add(Table.Is(x => x.EntityType.Name));

            if(options.PrimaryKeyName.IsNotWhiteSpace())
                conventions.Add(PrimaryKey.Name.Is(x => options.PrimaryKeyName));
            else if(options.PrimaryKeySurfix.IsNotWhiteSpace())
                conventions.Add(PrimaryKey.Name.Is(x => x.EntityType.Name + options.PrimaryKeySurfix));

            if(options.ForeignKeySurfix.IsNotWhiteSpace()) {
                conventions.Add(ForeignKey.EndsWith(options.ForeignKeySurfix));

                conventions.Add(ConventionBuilder.HasMany.Always(x => x.Key.Column(x.EntityType.Name + options.ForeignKeySurfix)));
                conventions.Add(ConventionBuilder.Reference.Always(x => x.Column(x.Property.Name + options.ForeignKeySurfix)));

                conventions.Add(ConventionBuilder.HasManyToMany.Always(x => {
                                                                           x.Key.Column(x.EntityType.Name + options.PrimaryKeySurfix);
                                                                           x.Relationship.Column(x.ChildType.Name +
                                                                                                 options.ForeignKeySurfix);
                                                                       }));
            }

            return conventions;
        }

        /// <summary>
        /// LazyLoad를 기본으로 할 것인가?
        /// </summary>
        public bool DefaultLazy { get; set; }

        /// <summary>
        /// Entity 등록 시, 값이 있는 속성만으로 Query를 생성할 것인가를 설정하는 값의 기본 값
        /// </summary>
        public bool DynamicInsert { get; set; }

        /// <summary>
        /// Entity 갱신 시, 값이 있는 속성만으로 Query를 생성할 것인가를 설정하는 값의 기본 값
        /// </summary>
        public bool DynamicUpdate { get; set; }

        /// <summary>
        /// 테이블 명의 접두사 (예: "TBL_", "RAT_" 등)
        /// </summary>
        public string TableNamePrefix { get; set; }

        /// <summary>
        /// 테이블 명의 접미사 (예: "_TABLE" 등)
        /// </summary>
        public string TableNameSurfix { get; set; }

        /// <summary>
        /// Primary Key의 기본 값 (예: "Id"), 보통 <see cref="PrimaryKeySurfix"/>를 많이 사용한다.
        /// </summary>
        public string PrimaryKeyName { get; set; }

        /// <summary>
        /// Primary Key 의 명칭을 EntityName + PrimaryKeySurfix 로 설정하게 합니다. 
        /// (예: Surfix가 "Id" 일 경우, User의 Primary Key 는 "UserId" 가 되고, Company의 Primary Key 는 "CompanyId" 가 됩니다)
        /// </summary>
        public string PrimaryKeySurfix { get; set; }

        /// <summary>
        /// Foreign Key의 접미사를 지정합니다. (예: "_ID", "Id")
        /// </summary>
        public string ForeignKeySurfix { get; set; }

        public override string ToString() {
            return this.ObjectToString();
        }
    }
}