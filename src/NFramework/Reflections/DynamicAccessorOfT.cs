using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Fasterflect;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.Reflections {
    /// <summary>
    /// Dynamic Method를 이용하여, 객체의 속성, 필드 정보를 조회/설정할 수 있는 Class
    /// </summary>
    /// <remarks>
    /// <see cref="DynamicAccessor{T}"/>를 생성할 때 직접하지 말고 <see cref="DynamicAccessorFactory.CreateDynamicAccessor{T}()"/>를 사용하세요.
    /// </remarks>
    /// <typeparam name="T">대상 객체의 형식</typeparam>
    /// <see cref="IDynamicAccessor{T}"/>
    /// <see cref="DynamicAccessorFactory"/>
    /// <see cref="DynamicAccessor"/>
    [Serializable]
    public class DynamicAccessor<T> : IDynamicAccessor<T> {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private readonly object _syncLock = new object();
        private static readonly Type targetType = typeof(T);

        protected readonly Dictionary<string, MemberGetter> FieldGetterMap = new Dictionary<string, MemberGetter>();
        protected readonly Dictionary<string, MemberSetter> FieldSetterMap = new Dictionary<string, MemberSetter>();

        protected readonly Dictionary<string, MemberGetter> PropertyGetterMap = new Dictionary<string, MemberGetter>();
        protected readonly Dictionary<string, MemberSetter> PropertySetterMap = new Dictionary<string, MemberSetter>();

        /// <summary>
        /// 예외 발생 시 rethrow를 시킬것인가 억제할 것인가?
        /// </summary>
        public bool SuppressError { get; set; }

        /// <summary>
        /// 속성명/필드명의 대소문자 구분 여부
        /// </summary>
        public bool IgnoreCase { get; set; }

        public DynamicAccessor() : this(false, false) {}

        public DynamicAccessor(bool suppressError = false, bool ignoreCase = false) {
            if(log.IsInfoEnabled)
                log.Info("DynamicAccessor<{0}>를 생성합니다. suppressError=[{1}], ignoreCase=[{2}]", typeof(T).Name, suppressError, ignoreCase);


            SuppressError = suppressError;
            IgnoreCase = ignoreCase;


            // 미리 준비하도록 합니다.
            Task.Factory.StartNew(() => {
                                      var ignoredPropertyMap = PropertyMap;
                                      var ignoredFieldMap = FieldMap;
                                  },
                                  TaskCreationOptions.LongRunning);
        }

        public Type TargetType {
            get { return targetType; }
        }

        /// <summary>
        /// 지정한 인스턴스의 필드 값을 가져온다.
        /// </summary>
        /// <param name="target">{T}의 인스턴스</param>
        /// <param name="fieldName">필드명</param>
        /// <returns>인스턴스의 필드명에 해당하는 값</returns>
        /// <exception cref="InvalidOperationException">해당 인스턴스가 필드 명에 해당하는 필드가 없을 때</exception>
        public object GetFieldValue(T target, string fieldName) {
            var fName = GetFieldName(fieldName);
            var fieldGetter = FieldGetterMap.GetOrAddFieldGetter(TargetType, fName);

            return (fieldGetter != null) ? fieldGetter(target) : null;
        }

        /// <summary>
        /// 지정한 인스턴스의 필드 값을 가져온다.
        /// </summary>
        /// <param name="target">{T}의 인스턴스</param>
        /// <param name="fieldName">필드 명</param>
        /// <param name="fieldValue">필드 값</param>
        /// <returns>필드 값 조회 여부</returns>
        public bool TryGetFieldValue(T target, string fieldName, out object fieldValue) {
            var fName = GetFieldName(fieldName);

            var fieldGetter = With.TryFunction(() => FieldGetterMap.GetOrAddFieldGetter(TargetType, fName));

            fieldValue = (fieldGetter != null) ? fieldGetter(target) : null;
            return (fieldGetter != null);
        }

        /// <summary>
        /// 지정한 인스턴스의 필드 값을 설정한다.
        /// </summary>
        /// <param name="target">대상 객체</param>
        /// <param name="fieldName">필드 명</param>
        /// <param name="fieldValue">필드 값</param>
        /// <exception cref="InvalidOperationException">해당 인스턴스가 필드 명에 해당하는 필드가 없을 때</exception>
        public virtual void SetFieldValue(T target, string fieldName, object fieldValue) {
            if(IsDebugEnabled)
                log.Debug("필드 값을 설정합니다... TargetType=[{0}], fieldName=[{1}], fieldValue=[{2}]", TargetType.Name, fieldName, fieldValue);

            var fName = GetFieldName(fieldName);

            var @exceptionAction =
                SuppressError
                    ? (Action<Exception>)(ex => { })
                    : ex => {
                          if(log.IsWarnEnabled) {
                              log.Warn("필드[{0}]에 값[{1}] 설정에 실패했습니다.", fieldName, fieldValue);
                              log.Warn(ex);
                          }
                      };

            var fieldSetter = FieldSetterMap.GetOrAddFieldSetter(TargetType, fName, @exceptionAction);

            if(fieldSetter != null)
                With.TryAction(() => fieldSetter(target, ConvertTool.IsNullOrDbNull(fieldValue) ? null : fieldValue), @exceptionAction);
        }

        /// <summary>
        /// 지정한 인스턴스의 속성 값을 가져온다.
        /// </summary>
        /// <param name="target">대상 객체</param>
        /// <param name="propertyName">속성 명</param>
        /// <returns>대상 객체의 속성명에 해당하는 속성의 값</returns>
        /// <exception cref="InvalidOperationException">해당 인스턴스가 속성명에 해당하는 속성이 정의되어 있지 않을 때</exception>
        public object GetPropertyValue(T target, string propertyName) {
            var pName = GetPropertyName(propertyName);

            var propertyGetter = PropertyGetterMap.GetOrAddPropertyGetter(TargetType, pName);
            return (propertyGetter != null) ? propertyGetter(target) : null;
        }

        /// <summary>
        /// 지정한 인스턴스의 속성 명에 해당하는 값을 조회합니다.
        /// </summary>
        /// <param name="target">대상 인스턴스</param>
        /// <param name="propertyName">속성 명</param>
        /// <param name="propertyValue">속성 값</param>
        /// <returns>조회 성공 여부</returns>
        public bool TryGetPropertyValue(T target, string propertyName, out object propertyValue) {
            var pName = GetPropertyName(propertyName);

            var propertyGetter = With.TryFunction(() => PropertyGetterMap.GetOrAddPropertyGetter(TargetType, pName));
            propertyValue = (propertyGetter != null) ? propertyGetter(target) : null;
            return (propertyGetter != null);
        }

        /// <summary>
        /// 지정한 인스턴스의 속성 값을 설정한다.
        /// </summary>
        /// <param name="target">대상 객체</param>
        /// <param name="propertyName">속성 명</param>
        /// <param name="propertyValue">설정할 속성 값</param>
        /// <exception cref="InvalidOperationException">해당 인스턴스가 속성명에 해당하는 속성이 정의되어 있지 않을 때</exception>
        public virtual void SetPropertyValue(T target, string propertyName, object propertyValue) {
            if(IsDebugEnabled)
                log.Debug("속성 값을 설정합니다... TargetType=[{0}], propertyName=[{1}], propertyValue=[{2}]", TargetType.Name, propertyName,
                          propertyValue);

            var pName = GetPropertyName(propertyName);

            var @exceptionAction =
                SuppressError
                    ? (Action<Exception>)(ex => { })
                    : ex => {
                          if(log.IsWarnEnabled) {
                              log.Warn("속성[{0}]에 값[{1}] 설정에 실패했습니다.", pName, propertyValue);
                              log.Warn(ex);
                          }
                      };

            var propertySetter = PropertySetterMap.GetOrAddPropertySetter(TargetType, pName, @exceptionAction);

            if(propertySetter != null)
                With.TryAction(() => propertySetter(target, ConvertTool.IsNullOrDbNull(propertyValue) ? null : propertyValue),
                               @exceptionAction);
        }

        /// <summary>
        /// Indexer
        /// </summary>
        /// <param name="target"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public object this[T target, string propertyName] {
            get { return GetPropertyValue(target, propertyName); }
            set { SetPropertyValue(target, propertyName, value); }
        }

        private IDictionary<string, FieldInfo> _fieldMap;
        private IDictionary<string, PropertyInfo> _propertyMap;

        /// <summary>
        /// 필드 정보
        /// </summary>
        public IDictionary<string, FieldInfo> FieldMap {
            get {
                if(_fieldMap == null)
                    lock(_syncLock)
                        if(_fieldMap == null) {
                            var map = TargetType.RetriveFieldMap();
                            System.Threading.Thread.MemoryBarrier();
                            _fieldMap = map;
                        }

                return _fieldMap;
            }
        }

        /// <summary>
        /// 속성 정보
        /// </summary>
        public IDictionary<string, PropertyInfo> PropertyMap {
            get {
                if(_propertyMap == null)
                    lock(_syncLock)
                        if(_propertyMap == null) {
                            var map = TargetType.RetrievePropertyMap();
                            System.Threading.Thread.MemoryBarrier();
                            _propertyMap = map;
                        }

                return _propertyMap;
            }
        }

        /// <summary>
        /// Public Property들의 속성명들을 가져온다
        /// </summary>
        /// <returns>해당 형식의 Public 속성명 리스트</returns>
        public IList<string> GetPropertyNames() {
            return PropertyMap.Keys.ToList();
        }

        /// <summary>
        /// 해당 형식의 지정된 BindingFlag에 의해 조사된 속성명들을 반환한다.
        /// </summary>
        /// <param name="bindingFlags">Reflection BindingFlags</param>
        /// <returns>속성명 리스트</returns>
        public IList<string> GetPropertyNames(BindingFlags bindingFlags) {
            return TargetType.Properties(bindingFlags).Select(pi => pi.Name).ToList();
        }

        /// <summary>
        /// Public Field들의 속성명을 가져온다
        /// </summary>
        /// <returns>Collection of Field names</returns>
        public IList<string> GetFieldNames() {
            return FieldMap.Keys.ToList();
        }

        /// <summary>
        /// 해당 형식의 지정된 BindingFlag에 의해 조사된 필드명들을 반환한다.
        /// </summary>
        /// <param name="bindingFlags"></param>
        /// <returns>Collectio of Field names</returns>
        public IList<string> GetFieldNames(BindingFlags bindingFlags) {
            return TargetType.Fields(bindingFlags).Select(fi => fi.Name).ToList();
        }

        /// <summary>
        /// 지정된 속성의 형식을 반환한다.
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <returns>Type of Field</returns>
        /// <exception cref="InvalidOperationException">필드가 존재하지 않을 때</exception>
        public Type GetFieldType(string fieldName) {
            var fi = FieldMap.GetValue(GetFieldName(fieldName));
            return (fi != null) ? fi.FieldType : null;
        }

        /// <summary>
        /// 지정된 속성의 형식을 반환한다.
        /// </summary>
        /// <param name="propertyName">Property name</param>
        /// <returns>Type of Property</returns>
        /// <exception cref="InvalidOperationException">속성이 존재하지 않을 때</exception>
        public Type GetPropertyType(string propertyName) {
            var pi = PropertyMap.GetValue(GetPropertyName(propertyName));
            return (pi != null) ? pi.PropertyType : null;
        }

        protected string GetFieldName(string fieldName) {
            return (IgnoreCase)
                       ? FieldMap.Keys.FirstOrDefault(name => name.EqualTo(fieldName)) ?? fieldName
                       : fieldName;
        }

        protected string GetPropertyName(string propertyName) {
            return (IgnoreCase)
                       ? PropertyMap.Keys.FirstOrDefault(name => name.EqualTo(propertyName)) ?? propertyName
                       : propertyName;
        }
    }
}