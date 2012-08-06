using System;
using System.Linq;
using NSoft.NFramework.Json;
using NSoft.NFramework.LinqEx;
using NSoft.NFramework.Reflections;
using NSoft.NFramework.Threading;
using NSoft.NFramework.Tools;
using NUnit.Framework;
using SharpTestsEx;
using EnumerableTool = NSoft.NFramework.LinqEx.EnumerableTool;

namespace NSoft.NFramework.DataServices.Messages {
    [TestFixture]
    public class MessageFixture : AbstractDataServiceFixture {
        #region << logger >>

        [NonSerialized] private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        [NonSerialized] private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        public static Random Rnd = new ThreadSafeRandom();

        public const int RequestItemCount = 10;
        public const int ResultRowCount = 10;

        [Test]
        public void CreateRequestMessageTest() {
            var request = new RequestMessage();
            request.Should().Not.Be.Null();

            request.AddItem("GetUsers", ResponseFormatKind.ResultSet, null, null).Should().Not.Be.Null();
        }

        [Test]
        public void Request_Build_Serialize_Deserialize() {
            var original = GetRequestMessage();

            var requestText = JsonTool.SerializeAsText(original);
            requestText.Should().Not.Be.Empty();

            var loaded = JsonTool.DeserializeFromText<RequestMessage>(requestText);

            AssertRequest(original, loaded);
        }

        [Test]
        public void Request_Build_Serialize_Deserialize_With_Compress() {
            var original = GetRequestMessage();

            var requestBytes = JsonTool.SerializeWithCompress(original);
            requestBytes.Should().Not.Be.Null();
            requestBytes.Length.Should().Be.GreaterThan(0);

            var loaded = JsonTool.DeserializeWithDecompress<RequestMessage>(requestBytes);

            AssertRequest(original, loaded);
        }

        [Test]
        public void Response_Build_Serialize_Deserialize() {
            var original = GetResponseMessage();

            var responseBytes = JsonByteSerializer<ResponseMessage>.Instance.Serialize(original);
            responseBytes.Should().Not.Be.Empty();

            var loaded = JsonByteSerializer<ResponseMessage>.Instance.Deserialize(responseBytes);

            AssertResponse(original, loaded);
        }

        [Test]
        public void Response_Build_Serialize_Deserialize_With_Compress() {
            var original = GetResponseMessage();

            var requestBytes = JsonTool.SerializeWithCompress(original);
            requestBytes.Should().Not.Be.Null();
            requestBytes.Length.Should().Be.GreaterThan(0);

            var loaded = JsonTool.DeserializeWithDecompress<ResponseMessage>(requestBytes);

            AssertResponse(original, loaded);
        }

        [Test]
        public void Response_GetMappedObject() {
            var original = GetResponseMessage();
            var responseText = JsonTool.SerializeAsText(original);
            var loaded = JsonTool.DeserializeFromText<ResponseMessage>(responseText);

            AssertResponse(original, loaded);

            Action<object> @VerifyMappedObject =
                x => {
                    x.Should().Not.Be.Null();
                    x.Should().Be.InstanceOf<ResultEntity>();

                    var result = (ResultEntity)x;
                    result.Name.Should().Not.Be.Empty();
                    result.Description.Should().Not.Be.Empty();
                    result.CreateAt.Should().Be.LessThan(DateTime.Now);
                    result.CreateAt.Should().Be.GreaterThan(DateTime.MinValue);
                };

            foreach(var item in original.Items)
                EnumerableTool.RunEach(item.ResultSet.GetMappedObjects(typeof(ResultEntity)), x => @VerifyMappedObject(x));

            foreach(var item in loaded.Items)
                item.ResultSet.GetMappedObjects(typeof(ResultEntity)).RunEach(x => @VerifyMappedObject(x));

            var enumerator1 = original.Items.GetEnumerator();
            var enumerator2 = loaded.Items.GetEnumerator();

            try {
                while(enumerator1.MoveNext() && enumerator2.MoveNext()) {
                    var set1 = enumerator1.Current.ResultSet;
                    var set2 = enumerator2.Current.ResultSet;

                    var mapObjects1 = set1.GetMappedObjects(typeof(ResultEntity)).ToList<ResultEntity>();
                    var mapObjects2 = set2.GetMappedObjects(typeof(ResultEntity)).ToList<ResultEntity>();

                    Assert.AreEqual((int)mapObjects1.Count, (int)mapObjects2.Count, "Objecct Count different");

                    var count = mapObjects1.Count;

                    for(int i = 0; i < count; i++)
                        Assert.AreEqual((object)mapObjects1[i], mapObjects2[i], "mapObject1=[{0}], mapObject2=[{1}]", mapObjects1[i],
                                        mapObjects2[i]);
                }
            }
            finally {
                enumerator1.Dispose();
                enumerator2.Dispose();
            }
        }

        [Test]
        public void JsonFormatTest() {
            var original = GetResponseMessage();
            var responseText = JsonTool.SerializeAsText(original);
            var loaded = JsonTool.DeserializeFromText<ResponseMessage>(responseText);
        }

        public void AssertRequest(RequestMessage original, RequestMessage loaded) {
            loaded.Should().Should().Not.Be.Null();

            loaded.MessageId.Should().Be(original.MessageId);
            loaded.Items.Count.Should().Be(original.Items.Count);
            loaded.PrepareStatements.Count.Should().Be(original.PrepareStatements.Count);
            loaded.PostscriptStatements.Count.Should().Be(original.PostscriptStatements.Count);

            for(int i = 0; i < original.Items.Count; i++) {
                loaded.Items[i].Id.Should().Be(original.Items[i].Id);
                loaded.Items[i].Parameters.Count.Should().Be(original.Items[i].Parameters.Count);
                loaded.Items[i].PrepareStatements.Count.Should().Be(original.Items[i].PrepareStatements.Count);
                loaded.Items[i].PostscriptStatements.Count.Should().Be(original.Items[i].PostscriptStatements.Count);
            }
        }

        public void AssertResponse(ResponseMessage original, ResponseMessage loaded) {
            loaded.Should().Should().Not.Be.Null();

            loaded.MessageId.Should().Be(original.MessageId);
            loaded.Items.Count.Should().Be(original.Items.Count);
            loaded.CreatedUtcTime.Should().Be(original.CreatedUtcTime);

            for(int i = 0; i < original.Items.Count; i++) {
                loaded.Items[i].Id.Should().Be(original.Items[i].Id);
                loaded.Items[i].ResultSet.Count.Should().Be(original.Items[i].ResultSet.Count);
            }
        }

        public RequestMessage GetRequestMessage() {
            var request = new RequestMessage();

            for(int i = 0; i < RequestItemCount; i++) {
                var item = new RequestItem
                           {
                               Method = "Request Body " + i,
                               RequestMethod = RequestMethodKind.Method,
                               ResponseFormat = ResponseFormatKind.ResultSet
                           };

                Enumerable
                    .Range(0, 5)
                    .RunEach(p => {
                                 var parameter = new RequestParameter("PARAM_" + p, p);
                                 item.Parameters.Add(parameter);

                                 item.PrepareStatements.Add("Prepare Statements " + p);
                                 item.PostscriptStatements.Add("Postscript Statements " + p);
                             });

                request.Items.Add(item);
            }

            request.PrepareStatements.Add("Request Prepare");
            request.PostscriptStatements.Add("Request Postscript");

            return request;
        }

        public ResponseMessage GetResponseMessage() {
            const string Text = "동해물과 백두산이 마르고 닳도록";
            var FieldValue = Text.Replicate(100);

            var request = GetRequestMessage();
            var response = new ResponseMessage() { CreatedUtcTime = DateTime.UtcNow };

            request.Items
                .RunEach(requestItem => {
                             var resultSet = new ResultSet();

                             for(int i = 0; i < ResultRowCount; i++) {
                                 var row = ResultEntity.ToResultRow(ResultEntity.CreateResultEntity());
                                 resultSet.Add(row);
                             }

                             //for (var i = 0; i < 10; i++)
                             //{
                             //    var row = new ResultRow();

                             //    for (var j = 0; j < 10; j++)
                             //        row.AddValue("Field" + j, FieldValue);

                             //    resultSet.Add(row);
                             //}

                             var responseItem = new ResponseItem(requestItem)
                                                {
                                                    ResultSet = resultSet,
                                                    ExecutionTime = TimeSpan.FromMilliseconds(500)
                                                };

                             response.Items.Add(responseItem);
                         });

            return response;
        }

        [Serializable]
        public class ResultEntity : IEquatable<ResultEntity> {
            public static ResultEntity CreateResultEntity() {
                var id = Guid.NewGuid();

                return new ResultEntity
                       {
                           Id = id,
                           Name = "Entity Name of " + id,
                           Description = "Description of Result Entity - " + id,
                           Number = Rnd.Next(1000),
                           DoubleNum = Rnd.NextDouble(),
                           CreateAt = DateTime.Now
                       };
            }

            public static ResultRow ToResultRow(ResultEntity resultEntity) {
                var accessor = DynamicAccessorFactory.CreateDynamicAccessor<ResultEntity>();

                return new ResultRow(accessor.GetPropertyNameValueCollection(resultEntity).ToDictionary(p => p.Key, p => p.Value));
            }

            public Guid Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }

            public int Number { get; set; }
            public double DoubleNum { get; set; }

            public DateTime CreateAt { get; set; }

            public bool Equals(ResultEntity other) {
                return (other != null) && GetHashCode().Equals(other.GetHashCode());
            }

            public override bool Equals(object obj) {
                return (obj != null) && (obj is ResultEntity) && Equals((ResultEntity)obj);
            }

            public override int GetHashCode() {
                //! NOTE: JSON 포맷에서는 DateTime 비교를 항상 JsonDateTime 포맷으로 해야합니다.
                //
                return HashTool.Compute(Id,
                                        Name,
                                        Description,
                                        Number,
                                        Description,
                                        CreateAt.ToJsonDateTime());
            }

            public override string ToString() {
                return this.ObjectToString();
            }
        }
    }
}