using System;
using NSoft.NFramework.Compressions;
using NSoft.NFramework.Compressions.Compressors;
using NSoft.NFramework.UnitTesting;
using NUnit.Framework;

namespace NSoft.NFramework.InversionOfControl {
    /// <summary>
    /// 다중의 환경설정에 대해 동시에 사용가능한지 TEST한다.
    /// </summary>
    [TestFixture]
    public class ContainerSelectorFixture {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        private const string DefaultCompressorId = @"DefaultCompressor";

        [TestFixtureSetUp]
        public void ClassSetUp() {
            if(IoC.IsNotInitialized)
                IoC.Initialize();
        }

        [TestFixtureTearDown]
        public void ClassTearDown() {
            IoC.Reset();
        }

        [Test]
        public void ResolveTest() {
            var containerSelector = IoC.Resolve<ContainerSelector>();

            Assert.IsNotNull(containerSelector);
            Assert.Greater(containerSelector.ContainerNames.Count, 0,
                           "환경설정에서 ContainerSelector에 Container 정보가 없습니다.");

            foreach(var name in IoC.Resolve<ContainerSelector>().ContainerNames)
                Console.WriteLine("등록된 Container Name: " + name);
        }

        public ContainerSelector ContainerSelector {
            get { return IoC.Resolve<ContainerSelector>(); }
        }

        /// <summary>
        /// Main Container에서는 Default가 SharpGZipCompressor이고, ContainerSelector를 이용하여 다른 Compressor를 이용할 수 있다.
        /// </summary>
        [Test]
        public void LoadLocalContainers() {
            // Main Container : DefaultCompressor로 SharpGZipCompressor가 등록되어 있다.
            Assert.IsInstanceOf<SharpGZipCompressor>(IoC.Resolve<ICompressor>(DefaultCompressorId));
            // Assert.IsInstanceOfType(typeof(SharpGZipCompressor), IoC.Resolve<ICompressor>(DefaultCompressorId));

            // ContainerSelector는 특정 상황에서 다른 configuration을 이용할 수 있다.
            //
            // 테스트시에 ContainerSelector에는 Deflate, SevenZip을 사용하게끔 만들었다. 
            // 즉 ContainerSelector를 이용하면 특정상황에서만 기본 설정이 아닌 다른 설정에 따라 Compressor를 변경 시킬 수 있다.

            foreach(var name in ContainerSelector.ContainerNames)
                using(ContainerSelector.Enter(name)) {
                    Compressor.InnerCompressor = IoC.Resolve<ICompressor>(DefaultCompressorId);

                    Assert.IsNotNull(Compressor.InnerCompressor);
                    Assert.AreEqual(name + "Compressor", Compressor.InnerCompressor.GetType().Name);
                }
        }

        // TODO: 새로운 클래스로 구현해야 합니다.
        [Test]
        [Ignore("새로운 클래스로 구현해야 합니다.")]
        public void EnumParameterConversion() {
//#pragma warning disable 0618
//			var provider = IoC.Resolve<HashProvider>();
//#pragma warning restore 0618
//			Assert.IsNotNull(provider);
//			Console.WriteLine("Provider Type : " + provider.ProviderType);
        }

        [Test]
        public void ThreadTest() {
            TestTool.RunTasks(15,
                              () => {
                                  ResolveTest();
                                  LoadLocalContainers();
                                  EnumParameterConversion();
                              });
        }
    }
}