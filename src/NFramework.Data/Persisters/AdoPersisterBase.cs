using System;
using System.Threading;
using NSoft.NFramework.Data.Mappers;
using NSoft.NFramework.Reflections;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.Data.Persisters {
    ///<summary>
    /// 데이터 소스로부터 Persistent Object 를 빌드합니다.
    ///</summary>
    ///<typeparam name="TDataSource">Type of dataReader dataReader</typeparam>
    ///<typeparam name="TPersist">Type of Persistent object</typeparam>
    public abstract class AdoPersisterBase<TDataSource, TPersist> : IAdoPersister<TDataSource, TPersist> {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        private static readonly object _syncLock = new object();

        #region << DynamicAccessor >>

        private static IDynamicAccessor<TPersist> _dynamicAccessor;

        public static IDynamicAccessor<TPersist> DynamicAccessor {
            get {
                if(_dynamicAccessor == null)
                    lock(_syncLock)
                        if(_dynamicAccessor == null) {
                            var accessor = DynamicAccessorFactory.CreateDynamicAccessor<TPersist>(MapPropertyOptions.Safety);
                            Thread.MemoryBarrier();
                            _dynamicAccessor = accessor;
                        }

                return _dynamicAccessor;
            }
        }

        #endregion

        /// <summary>
        /// 생성자
        /// </summary>
        protected AdoPersisterBase() : this(new TrimNameMapper()) {}

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="nameMapper">name mapper</param>
        /// <param name="persistentFactory">Persistent object 생성 delegate</param>
        protected AdoPersisterBase(INameMapper nameMapper, Func<TPersist> persistentFactory = null) {
            nameMapper.ShouldNotBeNull("nameMapper");

            NameMapper = nameMapper;
            FactoryFunction = persistentFactory ?? ActivatorTool.CreateInstance<TPersist>;
        }

        /// <summary>
        /// 컬럼명:속성명의 Name Mapper를 나타냅니다.
        /// </summary>
        public INameMapper NameMapper { get; set; }

        /// <summary>
        /// Persistence 객체를 생성하는 함수
        /// </summary>
        public Func<TPersist> FactoryFunction { get; set; }

        /// <summary>
        /// DataSource로부터 새로운 Persistent object를 빌드합니다.
        /// </summary>
        /// <param name="data">데이타 소스</param>
        /// <returns>Persistent object</returns>
        public abstract TPersist Persist(TDataSource data);
    }
}