
#if EXPERIMENTS

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using NUnit.Framework;
using Rhino.Mocks;

namespace NSoft.NFramework.Core
{
	[TestFixture]
	public class HttpModuleMockTestCase
	{
		private MockRepository _repository;
		private HttpContextBase _httpContext;
		private HttpRequestBase _httpRequest;
		private HttpResponseBase _httpResponse;


		[TestFixtureSetUp]
		public void ClassSetUp()
		{
			_repository = new MockRepository();
			_httpContext = _repository.StrictMock<HttpContextBase>();
			_httpRequest = _repository.StrictMock<HttpRequestBase>();
			_httpResponse = _repository.StrictMock<HttpResponseBase>();
		}

		[Test]
		[Ignore("Rhino.Mocks 사용법을 더 익혀야 될 듯.")]
		public void OnBeginRequest_Should_Redirect_When_Requesting_Url_Start_With_WWW()
		{
			using (_repository.Record())
			{
				SetupResult.For(_httpContext.Request).Return(_httpRequest);
				SetupResult.For(_httpContext.Response).Return(_httpResponse);

				Expect.Call(_httpRequest.Url).Return(new Uri("http://www.mysite.com"));

				Expect.Call(_httpResponse.StatusCode).Return((int)HttpStatusCode.MovedPermanently);

				var module = new RemoveW3HttpModuleImpl();
				module.OnBeginRequest(_httpContext);
			}
			_repository.ReplayAll();

			_repository.VerifyAll();

		}
	}
}
#endif