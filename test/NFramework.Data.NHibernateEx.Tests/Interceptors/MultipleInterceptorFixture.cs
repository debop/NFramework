using System;
using System.IO;
using NHibernate;
using NSoft.NFramework.InversionOfControl;
using NUnit.Framework;

namespace NSoft.NFramework.Data.NHibernateEx.Interceptors {
    [TestFixture]
    public class MultipleInterceptorFixture : NHRepositoryTestFixtureBase {
        protected override string ContainerFilePath {
            get {
                return
                    Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Interceptors\IoC.NHibernate.config"));
            }
        }

        [Test]
        public void ResolveInterceptor() {
            if(IoC.IsNotInitialized)
                Assert.Fail("MultiInterceptor를 사용하기 위해서는 ContainerFilePath를 제대로 설정하세요.");

            var interceptor = IoC.Resolve<IInterceptor>();

            Assert.IsInstanceOf<MultipleInterceptor>(interceptor);
        }

        [Test]
        public void PropertyChangedProxy() {
            // PropertyChangedInterceptor에서 속성값 변경 시 로그를 찍는지 검사하세요.
            base.parentsInDB[0].Age += 1;
        }
    }
}