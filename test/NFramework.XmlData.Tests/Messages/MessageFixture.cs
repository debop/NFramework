using System;
using NSoft.NFramework.UnitTesting;
using NSoft.NFramework.Xml;
using NUnit.Framework;

namespace NSoft.NFramework.XmlData.Messages {
    [TestFixture]
    public class MessageFixture {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        private static XdsRequestDocument GetRequestDocument() {
            var requestDocument = new XdsRequestDocument();
            requestDocument.Transaction = true;
            int reqId = requestDocument.AddStoredProc("SP_NAME", XmlDataResponseKind.DataSet);

            Assert.AreEqual(0, reqId);

            var request = requestDocument[reqId];
            request.AddParamArray("ID", "NAME");
            request.AddValue(1, "배성혁");
            request.AddValue(2, "sunghyouk");

            Assert.AreEqual(2, request.Values.Count);

            reqId = requestDocument.AddQuery("QUERY_NAME", XmlDataResponseKind.Scalar);
            var request2 = requestDocument[reqId];
            request2.AddParamArray("NAME");
            request2.AddValue("배성혁");
            request2.AddValue("sunghyouk bae");

            Assert.AreEqual(2, request2.Values.Count);

            request2.PreQueries.AddQuery("DELETE Employees where LastName='Bae'");
            request2.PostQueries.AddQuery("DELETE Employees where LastName='Bae'");

            Assert.AreEqual(1, request2.PreQueries.Count);
            Assert.AreEqual(1, request2.PostQueries.Count);

            return requestDocument;
        }

        private static XdsResponseDocument GetResponseDocument() {
            var responseDoc = new XdsResponseDocument();
            var index = responseDoc.AddResponseItem(new XdsResponseItem(XmlDataResponseKind.DataSet, 1, 1));
            responseDoc[index].Fields.AddFieldArray("USER_ID", "USER_NAME", "USER_PWD");
            responseDoc[index].Records.AddColumnArray("debop", "배성혁", "비밀번호");
            responseDoc[index].Records.AddColumnArray("mskwon", "권미숙", "비밀번호");

            responseDoc.Errors.AddError(new InvalidOperationException("테스트용 에러 메시지 입니다."));

            Assert.AreEqual(1, responseDoc.Responses.Count);
            Assert.AreEqual(1, responseDoc.Errors.Count);

            return responseDoc;
        }

        /// <summary>
        /// 요청정보를 생성 -> 직렬화 -> (전송) -> 역직렬화 -> 요청정보 (원본과 비교) 
        /// </summary>
        [Test]
        public void Request_Build_Send_Receive() {
            var original = GetRequestDocument();

            // 서버에서 Stream을 받아 XdsRequestDocument로 역직렬화를 수행한다.
            //
            var received =
                GetRequestDocument()
                    .ToStream(XmlTool.XmlEncoding)
                    .ToRequestDocument(XmlTool.XmlEncoding);

            Assert.IsNotNull(received);
            Assert.AreEqual(original.Transaction, received.Transaction);
            Assert.AreEqual(original.Requests.Count, received.Requests.Count);
        }

        /// <summary>
        /// 응답 정보를 Build->직렬화-> (전송) -> 역직렬화 -> 원본과 비교
        /// </summary>
        [Test]
        public void Response_Build_Send_Receive() {
            var original = GetResponseDocument();

            var received =
                GetResponseDocument()
                    .ToStream(XmlTool.XmlEncoding)
                    .ToResponseDocument(XmlTool.XmlEncoding);

            Assert.IsNotNull(received);
            Assert.AreEqual(original.HasError, received.HasError);
            Assert.AreEqual(original.Responses.Count, received.Responses.Count);
        }

        [Test]
        public void MultiThreadMixingTest() {
            TestTool.RunTasks(3,
                              () => {
                                  // 여기에 overriding하여 여러 테스트 함수를 호출하게 한다.);
                                  using(new OperationTimer("MultiThreadMixingTest")) {
                                      Request_Build_Send_Receive();
                                      Response_Build_Send_Receive();
                                  }
                              });
        }
    }
}