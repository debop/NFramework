using System.Threading;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework {
    /// <summary>
    /// Singleton Pattern을 구현할 때 많이 사용하는 방법은 두가지가 있다. 
    /// <list type="table">
    ///		<listheader>
    ///			<term>구현방법</term>
    ///			<description>설명</description>
    ///		</listheader>
    ///		<item>
    ///			<term>double-check locking pattern</term>
    ///			<description>Instance를 반환하기 전, 객체를 Locking한 후 두번에 걸쳐서 singleton 객체가 null인지를 검사한다.</description>
    ///			<term>static pattern</term>
    ///			<description>static 변수에 직접 생성해 버린다. - 간단하지만 어떤에러가 있을 지 모른다.</description>
    ///		</item>
    /// </list>
    /// <see cref="SingletonTool{T}"/>는 위의 double-check locking pattern을 변형하여 더욱 정교하고, Thread-safe 한 Singleton Pattern을 구현한다.
    /// </summary>
    /// <remarks>
    /// 단 기본생성자가 제공되어야 합니다.
    /// </remarks>
    /// <example>
    ///		아래 예는 일반적인 Singleton 객체를 ThreadSafeSingletonFactory{T} 를 이용하여 만드는 방법이다.
    ///		<code>
    ///		public static class SomeSingleton
    ///		{
    ///			public static SomeSingleton Instance
    ///			{
    ///				get
    ///				{
    ///					return SingletonTool&lt;SomeSingleton&gt;.Instance;
    ///				}
    ///			}
    ///		}
    ///		</code>
    /// </example>
    public class SingletonTool<T> where T : class {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        private static T _instance;
        private static readonly object _syncLock = new object();

        /// <summary>
        /// Thread-Safe 한 Singleton 인스턴스	(단 기본생성자가 제공되어야 합니다.)
        /// </summary>
        /// <example>
        /// <code>
        /// // Thread-safe 한 Singleton Pattern을 구현하려면, 구현하려는 class를 만들고
        /// // 해당 class의 instance를 반환하는 property를 만든 후 SingletonTool를 사용해
        /// // 아래와 같이 만들면 된다.
        /// public static class SomeSingleton
        /// {
        /// 	public static SomeSingleton Instance
        /// 	{
        /// 		get
        /// 		{
        /// 			return SingletonTool&lt;SomeSingleton&gt;.Instance;
        /// 		}
        /// 	}
        /// }
        /// </code>
        /// </example>
        public static T Instance {
            get {
                // double-check locking pattern
                //
                if(ReferenceEquals(_instance, null)) {
                    lock(_syncLock) {
                        if(ReferenceEquals(_instance, null)) {
                            var result = ActivatorTool.CreateInstance<T>();

                            // 캐시 메모리를 주 메모리로 올린다.
                            // 즉 result 객체에 대한 메모리를 주메모리로 올려질 때까지 기다린다.
                            //
                            Thread.MemoryBarrier();

                            _instance = result;

                            if(log.IsInfoEnabled)
                                log.Info("Singleton 패턴으로 수형[{0}]의 인스턴스를 생성했습니다.", typeof(T).FullName);
                        }
                    }
                }

                return _instance;
            }
        }
    }
}