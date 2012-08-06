
#if EXPERIMENTS

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using NUnit.Framework;
using Rhino.Mocks;

namespace NSoft.NFramework.Core
{
	[TestFixture]
	public class HttpHandlerMockTestCase
	{
		private MockRepository Mocks;

		private HttpContextBase _httpContext;
		private HttpResponseBase _httpResponse;

		private JavascriptHandlerImpl _handler;

		[TestFixtureSetUp]
		public void ClassSetUp()
		{
			Mocks = new MockRepository();

			_httpContext = Mocks.StrictMock<HttpContextBase>();
			_httpResponse = Mocks.StrictMock<HttpResponseBase>();

			Expect.Call(_httpContext.Response).Return(_httpResponse);

			_handler = new JavascriptHandlerImpl();
		}

		[Test]
		[Ignore("Rhino.Mocks 사용법에 문제가 있다.")]
		public void ProcessRequest_Should_Set_Correct_ContentType()
		{
			Expect.Call(_httpResponse.ContentType).Return("application/x-javascript");
			_handler.OnProcessRequest(_httpContext);

			Mocks.VerifyAll();
		}
	}
}

#endif