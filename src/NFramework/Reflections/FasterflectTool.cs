using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using Fasterflect;

namespace NSoft.NFramework.Reflections {
    /// <summary>
    /// Reflection을 더 빠르게 수행해 Fasterflect 라이브러리를 쉽게 이용할 수 있는 Helper Class 입니다.
    /// 참고 : http://fasterflect.codeplex.com
    /// </summary>
    public static partial class FasterflectTool {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// Reflection을 위한 기본 Binding Flags (Static 만 빼고 다 있다!!!)
        /// </summary>
        public const BindingFlags DefaultFlags =
            BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase;

        /// <summary>
        /// 특정 수형의 필드 정보를 조회합니다.
        /// </summary>
        /// <param name="targetType"></param>
        /// <returns></returns>
        public static IDictionary<string, FieldInfo> RetriveFieldMap(this Type targetType) {
            if(IsDebugEnabled)
                log.Debug("형식[{0}]의 필드 정보를 조회합니다...", targetType.FullName);

            var fieldMap = new ConcurrentDictionary<string, FieldInfo>();

            foreach(var fieldInfo in targetType.Fields()) {
                var fi = fieldInfo;
                if(fieldMap.ContainsKey(fi.Name) == false)
                    fieldMap.AddOrUpdate(fi.Name, fi, (k, v) => fi);
            }

            return fieldMap;
        }

        /// <summary>
        /// 특정 수형의 속성 정보를 조회합니다.
        /// </summary>
        /// <param name="targetType"></param>
        /// <returns></returns>
        public static IDictionary<string, PropertyInfo> RetrievePropertyMap(this Type targetType) {
            if(IsDebugEnabled)
                log.Debug("형식[{0}]의 속성 정보를 조회합니다...", targetType.FullName);

            var propertyMap = new ConcurrentDictionary<string, PropertyInfo>();

            foreach(var propertyInfo in targetType.Properties()) {
                var pi = propertyInfo;
                if(propertyMap.ContainsKey(pi.Name) == false)
                    propertyMap.AddOrUpdate(pi.Name, pi, (k, v) => pi);
            }

            return propertyMap;
        }

        /// <summary>
        /// 특정 수형의 필드 값를 조회하기 위한 <see cref="MemberGetter"/>를 가져옵니다. 없으면 새로 추가합니다.
        /// </summary>
        /// <param name="fieldGetters">필드 값을 조회하는 MemberGetter들의 Dictionary</param>
        /// <param name="targetType">대상 형식</param>
        /// <param name="fieldName">필드명</param>
        /// <param name="exceptionAction">예외 시 수행할 Action</param>
        /// <returns></returns>
        public static MemberGetter GetOrAddFieldGetter(this IDictionary<string, MemberGetter> fieldGetters,
                                                       Type targetType,
                                                       string fieldName,
                                                       Action<Exception> exceptionAction = null) {
            fieldGetters.ShouldNotBeNull("fieldGetters");

            MemberGetter fieldGetter;

            if(fieldGetters.TryGetValue(fieldName, out fieldGetter))
                return fieldGetter;

            try {
                fieldGetter = targetType.DelegateForGetFieldValue(fieldName, DefaultFlags);

                if(fieldGetters is ConcurrentDictionary<string, MemberGetter>) {
                    ((ConcurrentDictionary<string, MemberGetter>)fieldGetters).AddOrUpdate(fieldName, fieldGetter, (k, v) => fieldGetter);
                }
                else {
                    lock(fieldGetters)
                        if(fieldGetters.ContainsKey(fieldName) == false) {
                            fieldGetters.Add(fieldName, fieldGetter);

                            if(IsDebugEnabled)
                                log.Debug("수형[{0}]의 필드[{1}]의 MemberGetter를 만들어서 캐시합니다...", targetType.FullName, fieldName);
                        }
                }
            }
            catch(Exception ex) {
                if(exceptionAction != null) {
                    exceptionAction(ex);
                }
                else if(log.IsWarnEnabled) {
                    log.Warn("수형[{0}]의 필드[{1}]의 MemberGetter를 만들어서 캐시하는데 예외가 발생했습니다!!! 무시합니다.", targetType.FullName, fieldName);
                    log.Warn(ex);
                }
            }

            return fieldGetter;
        }

        /// <summary>
        /// 특정 수형의 필드 값을 설정하기 위한 <see cref="MemberSetter"/>를 가져옵니다. 없으면 새로 추가합니다.
        /// </summary>
        /// <param name="fieldSetters">필드 값을 조회하는 MemberGetter들의 Dictionary</param>
        /// <param name="targetType">대상 형식</param>
        /// <param name="fieldName">필드명</param>
        /// <param name="exceptionAction">예외 시 수행할 Action</param>
        /// <returns></returns>
        public static MemberSetter GetOrAddFieldSetter(this IDictionary<string, MemberSetter> fieldSetters,
                                                       Type targetType,
                                                       string fieldName,
                                                       Action<Exception> exceptionAction = null) {
            fieldSetters.ShouldNotBeNull("fieldSetters");
            targetType.ShouldNotBeNull("targetType");
            fieldName.ShouldNotBeWhiteSpace("fieldName");

            MemberSetter fieldSetter;

            if(fieldSetters.TryGetValue(fieldName, out fieldSetter))
                return fieldSetter;

            try {
                fieldSetter = targetType.DelegateForSetFieldValue(fieldName, DefaultFlags);

                if(fieldSetters is ConcurrentDictionary<string, MemberSetter>) {
                    ((ConcurrentDictionary<string, MemberSetter>)fieldSetters).AddOrUpdate(fieldName, fieldSetter, (k, v) => fieldSetter);
                }
                else {
                    lock(fieldSetters)
                        if(fieldSetters.ContainsKey(fieldName) == false) {
                            fieldSetters.Add(fieldName, fieldSetter);

                            if(IsDebugEnabled)
                                log.Debug("수형[{0}]의 필드[{1}]의 MemberSetter를 만들어서 캐시합니다", targetType.FullName, fieldName);
                        }
                }
            }
            catch(Exception ex) {
                if(exceptionAction != null)
                    exceptionAction(ex);

                else if(log.IsWarnEnabled) {
                    log.Warn("수형[{0}]의 필드[{1}]의 MemberSetter를 만들어서 캐시하는데 예외가 발생했습니다!!! 무시합니다.", targetType.FullName, fieldName);
                    log.Warn(ex);
                }
            }

            return fieldSetter;
        }

        /// <summary>
        /// 특정 수형의 속성 값을 가져오기 위한 <see cref="MemberGetter"/>를 가져옵니다. 없으면 새로 추가합니다.
        /// </summary>
        /// <param name="propertyGetters"></param>
        /// <param name="targetType">대상 객체</param>
        /// <param name="propertyName">조회할 속성명</param>
        /// <param name="exceptionAction">예외 발생 시 처리할 Action</param>
        /// <returns></returns>
        public static MemberGetter GetOrAddPropertyGetter(this IDictionary<string, MemberGetter> propertyGetters,
                                                          Type targetType,
                                                          string propertyName,
                                                          Action<Exception> exceptionAction = null) {
            propertyGetters.ShouldNotBeNull("propertyGetters");
            targetType.ShouldNotBeNull("targetType");
            propertyName.ShouldNotBeEmpty("propertyName");

            MemberGetter propertyGetter;

            if(propertyGetters.TryGetValue(propertyName, out propertyGetter))
                return propertyGetter;

            try {
                propertyGetter = targetType.DelegateForGetPropertyValue(propertyName, DefaultFlags);

                if(propertyGetters is ConcurrentDictionary<string, MemberGetter>) {
                    ((ConcurrentDictionary<string, MemberGetter>)propertyGetters).AddOrUpdate(propertyName, propertyGetter,
                                                                                              (k, v) => propertyGetter);
                }
                else {
                    lock(propertyGetters)
                        if(propertyGetters.ContainsKey(propertyName) == false) {
                            propertyGetters.Add(propertyName, propertyGetter);

                            if(IsDebugEnabled)
                                log.Debug("수형[{0}]의 속성[{1}]의 MemberGetter를 만들어서 캐시합니다...", targetType.FullName, propertyName);
                        }
                }
            }
            catch(Exception ex) {
                if(exceptionAction != null) {
                    exceptionAction(ex);
                }
                else {
                    if(log.IsWarnEnabled) {
                        log.Warn("수형[{0}]의 속성[{1}]의 MemberGetter를 만들어서 캐시하는데 예외가 발생했습니다!!! 무시합니다.", targetType.FullName, propertyName);
                        log.Warn(ex);
                    }
                }
            }

            return propertyGetter;
        }

        /// <summary>
        /// 특정 수형의 속성 값을 설정하기 위한 <see cref="MemberSetter"/>를 가져옵니다. 없으면 새로추가합니다.
        /// </summary>
        /// <param name="propertySetters">속성 설정 Setter들의 Dictionary</param>
        /// <param name="targetType">대상 형식</param>
        /// <param name="propertyName">속성 명</param>
        /// <param name="exceptionAction">예외 시 실행할 Action</param>
        /// <returns></returns>
        public static MemberSetter GetOrAddPropertySetter(this IDictionary<string, MemberSetter> propertySetters,
                                                          Type targetType,
                                                          string propertyName,
                                                          Action<Exception> exceptionAction = null) {
            propertySetters.ShouldNotBeNull("propertySetters");
            targetType.ShouldNotBeNull("targetType");
            propertyName.ShouldNotBeWhiteSpace("propertyName");

            MemberSetter propertySetter;

            if(propertySetters.TryGetValue(propertyName, out propertySetter))
                return propertySetter;

            try {
                propertySetter = targetType.DelegateForSetPropertyValue(propertyName, DefaultFlags);

                if(propertySetters is ConcurrentDictionary<string, MemberSetter>) {
                    ((ConcurrentDictionary<string, MemberSetter>)propertySetters).AddOrUpdate(propertyName, propertySetter,
                                                                                              (k, v) => propertySetter);
                }
                else {
                    lock(propertySetters) {
                        if(propertySetters.ContainsKey(propertyName) == false) {
                            propertySetters.Add(propertyName, propertySetter);

                            if(IsDebugEnabled)
                                log.Debug("수형[{0}]의 속성[{1}]의 MemberSetter를 만들어서 캐시합니다.", targetType.FullName, propertyName);
                        }
                    }
                }
            }
            catch(Exception ex) {
                if(exceptionAction != null)
                    exceptionAction(ex);
                else {
                    if(log.IsWarnEnabled) {
                        log.Warn("수형[{0}]의 속성[{1}]의 MemberSetter를 만들어서 캐시하는데 예외가 발생했습니다!!! 무시합니다.", targetType.FullName, propertyName);
                        log.Warn(ex);
                    }
                }
            }

            return propertySetter;
        }

        /// <summary>
        /// Class 멤버명을 명명규칙에 따라 변경하게 됩니다.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="namingRule"></param>
        /// <returns></returns>
        public static string GetMemberName(this string name, MemberNamingRule namingRule = MemberNamingRule.CamelCaseUndercore) {
            name.ShouldNotBeWhiteSpace("name");

            var converted = name;

            switch(namingRule) {
                case MemberNamingRule.CamelCase:
                    converted = string.Concat(name[0].ToString().ToLower(), name.Substring(1));
                    break;

                case MemberNamingRule.CamelCaseUndercore:
                    converted = string.Concat("_", name[0].ToString().ToLower(), name.Substring(1));
                    break;

                case MemberNamingRule.CamelCase_M_Underscore:
                    converted = string.Concat("m_", name[0].ToString().ToLower(), name.Substring(1));
                    break;

                case MemberNamingRule.PascalCase:
                    converted = name;
                    break;

                case MemberNamingRule.PascalCaseUnderscore:
                    converted = string.Concat("_", name[0].ToString().ToUpper(), name.Substring(1));
                    break;

                case MemberNamingRule.PascalCase_M_Underscore:
                    converted = string.Concat("m_", name[0].ToString().ToUpper(), name.Substring(1));
                    break;

                default:
                    converted = string.Concat("_", name[0].ToString().ToLower(), name.Substring(1));
                    break;
            }

            if(IsDebugEnabled)
                log.Debug("멤버 변수명을 명명규칙에 따라 빌드했습니다. name=[{0}], result=[{1}], namingRule=[{2}]", name, converted, namingRule);

            return converted;
        }
    }
}