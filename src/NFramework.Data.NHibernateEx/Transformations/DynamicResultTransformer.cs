using System;
using System.Threading;
using NHibernate;
using NHibernate.Transform;
using NSoft.NFramework.Reflections;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.Data.NHibernateEx {
    /// <summary>
    /// <see cref="ICriteria"/> 등을 실행해서 나온 결과(object[]) 를 지정된 형식으로 빌드한다.
    /// 이때 지정된 형식의 속성값에 값을 설정하기 때문에, 관련없는 속성에는 값이 설정되지 않는다.
    /// </summary>
    public class DynamicResultTransformer<T> : IResultTransformer {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private static readonly Type _entityType = typeof(T);

        private static readonly object _syncLock = new object();
        private static IDynamicAccessor<T> _accessor;

        /// <summary>
        /// Dynamic Method를 이용하여, 지정된 타입의 객체의 속성, 필드 정보를 조회/설정할 수 있는 접근자
        /// </summary>
        protected static IDynamicAccessor<T> DynamicAccessor {
            get {
                if(_accessor == null)
                    lock(_syncLock)
                        if(_accessor == null) {
                            var accessor = DynamicAccessorFactory.CreateDynamicAccessor<T>(new MapPropertyOptions
                                                                                           {
                                                                                               SuppressException = false,
                                                                                               IgnoreCase = true
                                                                                           });
                            Thread.MemoryBarrier();
                            _accessor = accessor;
                        }

                return _accessor;
            }
        }

        ///<summary>
        /// 변환할 데이타의 컬렉션  
        ///</summary>
        ///<param name="collection">원본 컬렉션</param>
        ///<returns>변환할 데이타의 컬렉션</returns>
        public System.Collections.IList TransformList(System.Collections.IList collection) {
            return collection;
        }

        /// <summary>
        /// NHibernate ResultSet으로부터 나온 object[]를 지정된 형식의 인스턴스의 속성값에 매칭시킨다.
        /// </summary>
        /// <param name="tuple">결과 값</param>
        /// <param name="aliases">컬럼 명</param>
        /// <returns></returns>
        public object TransformTuple(object[] tuple, string[] aliases) {
            if(IsDebugEnabled)
                log.Debug("결과값을 새로운 인스턴스의 속성값으로 설정합니다. Target Type=[{0}], tuple=[{1}], aliases=[{2}]",
                          _entityType.FullName, tuple.CollectionToString(), aliases.CollectionToString());

            T result;

            try {
                result = ActivatorTool.CreateInstance<T>();

                for(var i = 0; i < aliases.Length; i++)
                    DynamicAccessor.SetPropertyValue(result, aliases[i], tuple[i]);

                if(IsDebugEnabled)
                    log.Debug("새로운 엔티티를 생성하고, NHibernate ResultSet을 엔티티의 속성 값에 설정했습니다. result=[{0}]", result);
            }
            catch(Exception ex) {
                if(log.IsErrorEnabled) {
                    log.Error("Fail to transform to instance. type=[{0}]", _entityType.FullName);
                    log.Error(ex);
                }

                return tuple;
            }

            return result;
        }
    }
}