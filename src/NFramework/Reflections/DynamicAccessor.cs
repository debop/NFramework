using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Fasterflect;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.Reflections {
    /// <summary>
    /// 인스턴스의 필드와 속성 정보에 동적으로 조작이 가능하도록 한다. (Reflection보다 속도가 빠르다.)
    /// </summary>
    [Serializable]
    public class DynamicAccessor : IDynamicAccessor {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private readonly object _syncLock = new object();

        protected readonly ConcurrentDictionary<string, MemberGetter> FieldGetterMap = new ConcurrentDictionary<string, MemberGetter>();
        protected readonly ConcurrentDictionary<string, MemberSetter> FieldSetterMap = new ConcurrentDictionary<string, MemberSetter>();

        protected readonly ConcurrentDictionary<string, MemberGetter> PropertyGetterMap =
            new ConcurrentDictionary<string, MemberGetter>();

        protected readonly ConcurrentDictionary<string, MemberSetter> PropertySetterMap =
            new ConcurrentDictionary<string, MemberSetter>();

        public DynamicAccessor(Type targetType) : this(targetType, false, false) {}

        public DynamicAccessor(Type targetType, bool suppressError = false, bool ignoreCase = false) {
            targetType.ShouldNotBeNull("targetType");

            if(log.IsInfoEnabled)
                log.Info("DynamicAccessor를 생성합니다. targetType=[{0}], suppressError=[{1}], ignoreCase=[{2}]",
                         targetType.FullName, suppressError, ignoreCase);

            TargetType = targetType;
            SuppressError = suppressError;
            IgnoreCase = ignoreCase;

            // 미리 준비하도록 합니다.
            Task.Factory.StartNew(() => {
                                      var ignoredPropertyMap = PropertyMap;
                                      var ignoredFieldMap = FieldMap;
                                  },
                                  TaskCreationOptions.LongRunning);
        }

        /// <summary>
        /// 예외 발생 시 rethrow를 시킬것인가 억제할 것인가?
        /// </summary>
        public bool SuppressError { get; set; }

        /// <summary>
        /// 속성명/필드명의 대소문자 구분 여부
        /// </summary>
        public bool IgnoreCase { get; set; }

        /// <summary>
        /// 동적으로 정보에 접근하고자하는 수형
        /// </summary>
        public Type TargetType { get; private set; }

        /// <summary>
        /// 지정한 인스턴스의 필드 값을 가져온다.
        /// </summary>
        /// <param name="target">인스턴스</param>
        /// <param name="fieldName">필드명</param>
        /// <returns>필드 값</returns>
        public object GetFieldValue(object target, string fieldName) {
            var fName = GetFieldName(fieldName);
            var fieldGetter = FieldGetterMap.GetOrAddFieldGetter(TargetType, fName);
            return (fieldGetter != null) ? fieldGetter(target) : null;
        }

        /// <summary>
        /// 인스턴스의 지정한 필드명의 값을 가져옵니다. 해당 필드가 없다면, false를 반환합니다.
        /// </summary>
        /// <param name="target">인스턴스</param>
        /// <param name="fieldName">필드명</param>
        /// <param name="fieldValue">필드 값</param>
        /// <returns>조회 성공 여부</returns>
        public bool TryGetFieldValue(object target, string fieldName, out object fieldValue) {
            var fName = GetFieldName(fieldName);

            var fieldGetter = With.TryFunction(() => FieldGetterMap.GetOrAddFieldGetter(TargetType, fName));

            fieldValue = (fieldGetter != null) ? fieldGetter(target) : null;
            return (fieldGetter != null);
        }

        /// <summary>
        /// 지정한 인스턴스의 필드 값을 설정한다.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="fieldName"></param>
        /// <param name="fieldValue"></param>
        public virtual void SetFieldValue(object target, string fieldName, object fieldValue) {
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
                With.TryAction(() => fieldSetter(target, ConvertTool.IsNullOrDbNull(fieldValue) ? null : fieldValue),
                               @exceptionAction);
        }

        /// <summary>
        /// 지정한 인스턴스의 속성 값을 가져온다.
        /// </summary>
        /// <param name="target">인스턴스</param>
        /// <param name="propertyName">속성 명</param>
        /// <returns>속성 값</returns>
        public object GetPropertyValue(object target, string propertyName) {
            var pName = GetPropertyName(propertyName);
            var propertyGetter = PropertyGetterMap.GetOrAddPropertyGetter(TargetType, pName);
            return (propertyGetter != null) ? propertyGetter(target) : null;
        }

        /// <summary>
        /// 인스턴스의 지정한 속성 명의 값을 가져옵니다. 해당 속성이 없다면, false를 반환합니다.
        /// </summary>
        /// <param name="target">인스턴스</param>
        /// <param name="propertyName">속성 명</param>
        /// <param name="propertyValue">속성 값</param>
        /// <returns>조회 여부</returns>
        public bool TryGetPropertyValue(object target, string propertyName, out object propertyValue) {
            var pName = GetPropertyName(propertyName);

            var propertyGetter = With.TryFunction(() => PropertyGetterMap.GetOrAddPropertyGetter(TargetType, pName));

            propertyValue = (propertyGetter != null) ? propertyGetter(target) : null;
            return (propertyGetter != null);
        }

        /// <summary>
        /// 지정한 인스턴스의 속성 값을 설정한다.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="propertyName"></param>
        /// <param name="propertyValue"></param>
        public virtual void SetPropertyValue(object target, string propertyName, object propertyValue) {
            if(IsDebugEnabled)
                log.Debug("속성 값을 설정합니다... TargetType=[{0}], propertyName=[{1}], propertyValue=[{2}]", TargetType.Name, propertyName,
                          propertyValue);

            var pName = GetPropertyName(propertyName);

            var @exceptionAction =
                SuppressError
                    ? (Action<Exception>)(ex => { })
                    : ex => {
                          if(log.IsWarnEnabled) {
                              log.Warn("속성[{0}]에 값[{1}] 설정에 실패했습니다.", propertyName, propertyValue);
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
        public object this[object target, string propertyName] {
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
        /// Get property names
        /// </summary>
        /// <returns></returns>
        public IList<string> GetPropertyNames() {
            return PropertyMap.Keys.ToList();
        }

        /// <summary>
        /// Get field names
        /// </summary>
        /// <returns></returns>
        public IList<string> GetFieldNames() {
            return FieldMap.Keys.ToList();
        }

        /// <summary>
        /// 지정된 속성의 형식을 반환한다.
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <returns>Type of Field</returns>
        /// <exception cref="InvalidOperationException">필드가 존재하지 않을 때</exception>
        public Type GetFieldType(string fieldName) {
            fieldName.ShouldNotBeWhiteSpace("fieldName");

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
            propertyName.ShouldNotBeWhiteSpace("propertyName");

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