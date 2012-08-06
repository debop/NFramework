using System.Web;

namespace NSoft.NFramework.Web.HttpModules {
    /// <summary>
    /// 일반적인 HttpModule의 추상 클래스이며, 
    /// HttpModule 테스트를 수행하기 쉽도록 하기 위해 <see cref="HttpContextWrapper"/>를 이용하도록 하였다.
    /// </summary>
    /// <remarks>
    /// AbstractHttpModule을 상속받아 구현한 모듈은 웹 Application이 없어도 Mocking을 통해 쉽게 테스트가 가능합니다.
    /// </remarks>
    public abstract class AbstractHttpModule : IHttpModule {
        /// <summary>
        /// 모듈을 초기화하고 요청을 처리할 수 있도록 준비합니다.
        /// </summary>
        /// <param name="context">ASP.NET 응용 프로그램 내의 모든 응용 프로그램 개체에 공통되는 메서드, 속성 및 이벤트에 액세스할 수 있도록 하는 <see cref="T:System.Web.HttpApplication"/>입니다.</param>
        public virtual void Init(HttpApplication context) {
            context.BeginRequest += (s, e) => OnBeginRequest(new HttpContextWrapper(((HttpApplication)s).Context));
            context.Error += (s, e) => OnError(new HttpContextWrapper(((HttpApplication)s).Context));
            context.EndRequest += (s, e) => OnEndRequest(new HttpContextWrapper(((HttpApplication)s).Context));
        }

        /// <summary>
        /// <see cref="T:System.Web.IHttpModule"/>을 구현하는 모듈에서 사용하는 리소스(메모리 제외)를 삭제합니다.
        /// </summary>
        public virtual void Dispose() {}

        /// <summary>
        /// HttpApplication이 사용자 요청을 받기 시작했을 때 호출되는 메소드입니다.
        /// </summary>
        /// <param name="context"></param>
        public virtual void OnBeginRequest(HttpContextBase context) {}

        /// <summary>
        /// HttpApplication에서 예외가 발생했을 때 호출되는 메소드입니다.
        /// </summary>
        /// <param name="context"></param>
        public virtual void OnError(HttpContextBase context) {}

        /// <summary>
        /// HttpApplication이 사용자 요청을 처리 완료 했을 때 호출되는 메소드입니다.
        /// </summary>
        /// <param name="context"></param>
        public virtual void OnEndRequest(HttpContextBase context) {}
    }
}