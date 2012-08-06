using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Castle.Windsor;

namespace NSoft.NFramework.InversionOfControl {
    /// <summary>
    /// 여러 상황을 동시에 지원하기 위해, 다중 Container를 등록해 놓고, 일시적으로 다른 Container를 이용할 수 있도록 한다.
    /// </summary>
    /// <remarks>
    /// IoC에서 Parameter로 줄 때 http://www.castleproject.org/container/documentation/trunk/usersguide/arrayslistsanddicts.html 를 참고하세요
    /// </remarks>
    [Serializable]
    public sealed class ContainerSelector {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// 기본 Container의 자식 Container중에 해당 이름을 가진 Container를 사용한다.
        /// </summary>
        /// <param name="name">Container 명</param>
        /// <returns></returns>
        /// <example>
        /// <code>
        ///	using(ContainerSelector.Enter(name))
        ///	{
        ///		Assert.AreEqual(name + "Compressor", Compressor.DefaultCompressor.GetType().Name);
        ///	}
        /// </code>
        /// </example>
        public static IDisposable UseContainer(string name) {
            return IoC.Resolve<ContainerSelector>().Enter(name);
        }

        private readonly ConcurrentDictionary<string, IWindsorContainer> _containers =
            new ConcurrentDictionary<string, IWindsorContainer>();

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="containers">자식 container 정보</param>
        public ContainerSelector(IDictionary containers) {
            if(containers != null)
                foreach(DictionaryEntry entry in containers) {
                    if(entry.Value != null)
                        Register(entry.Key.ToString(), new WindsorContainer(entry.Value.ToString()));
                }
        }

        /// <summary>
        /// 새로운 Container를 등록한다.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="container"></param>
        public void Register(string name, IWindsorContainer container) {
            name.ShouldNotBeWhiteSpace("name");
            container.ShouldNotBeNull("container");

            if(IsDebugEnabled)
                log.Debug("자식 컨테이너를 등록합니다... name=[{0}], container name=[{1}]", name, container.Name);

            _containers.AddOrUpdate(name, container, (key, value) => container);
        }

        /// <summary>
        /// 자식 Container들의 이름 목록
        /// </summary>
        public ICollection<string> ContainerNames {
            get { return _containers.Keys; }
        }

        /// <summary>
        /// 지정된 이름을 가진 자식 Container를 사용합니다.
        /// </summary>
        /// <param name="name">자식 컨테이너 명</param>
        /// <returns></returns>
        public IDisposable Enter(string name) {
            name.ShouldNotBeWhiteSpace("name");
            Guard.Assert(_containers.ContainsKey(name), "컨테이너명[{0}]으로 등록된 컨테이너가 없습니다.", name);

            return IoC.UseLocalContainer(_containers[name]);
        }
    }
}