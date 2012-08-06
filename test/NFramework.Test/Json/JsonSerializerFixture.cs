using System;
using System.Collections.Generic;
using System.Linq;
using NSoft.NFramework.LinqEx;
using NUnit.Framework;

namespace NSoft.NFramework.Json {
    [Microsoft.Silverlight.Testing.Tag("JSON")]
    [TestFixture]
    public class JsonSerializerFixture : AbstractFixture {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private static readonly IList<ISerializer<UserInfo>> serializers =
            new List<ISerializer<UserInfo>>
            {
                new JsonByteSerializer<UserInfo>(),
                new CompressJsonSerializer<UserInfo>(),
                new BsonSerializer<UserInfo>(),
                new CompressBsonSerializer<UserInfo>(),
            };

        [Test]
        public void All_Serializer_Test_AsParallel() {
            var user = UserInfo.GetSample();

            serializers
#if !SILVERLIGHT
                .AsParallel()
                .WithExecutionMode(ParallelExecutionMode.ForceParallelism)
                .WithDegreeOfParallelism(4)
#endif
                .RunEach(serializer =>
                         JsonSerializerVerifier(serializer,
                                                user,
                                                (source, target) => Assert.AreEqual(source.UserName, target.UserName),
                                                (source, target) => Assert.AreEqual(source.Description, target.Description),
                                                (source, target) => Assert.AreEqual(source.ByteArray, target.ByteArray),
                                                (source, target) =>
                                                Assert.AreEqual(source.FavoriteMovies.Count, target.FavoriteMovies.Count),
                                                (source, target) => Assert.AreEqual(source.HomeAddr.Phone, target.HomeAddr.Phone)));
        }

        [Test]
        public void All_Serializer_Test() {
            var user = UserInfo.GetSample();

            serializers
                .RunEach(serializer =>
                         JsonSerializerVerifier(serializer,
                                                user,
                                                (source, target) => Assert.AreEqual(source.UserName, target.UserName),
                                                (source, target) => Assert.AreEqual(source.Description, target.Description),
                                                (source, target) => Assert.AreEqual(source.ByteArray, target.ByteArray),
                                                (source, target) =>
                                                Assert.AreEqual(source.FavoriteMovies.Count, target.FavoriteMovies.Count),
                                                (source, target) => Assert.AreEqual(source.HomeAddr.Phone, target.HomeAddr.Phone)));
        }

        public static void JsonSerializerVerifier<T>(ISerializer<T> serializer, T graph, params Action<T, T>[] assertActions) {
            if(IsDebugEnabled)
                log.Debug("Json 방식의 Serializer를 테스트합니다. Serializer=" + serializer.GetType());

            var serializedBytes = serializer.Serialize(graph);
            var deserializedGraph = serializer.Deserialize(serializedBytes);

            assertActions.RunEach(assert => assert(graph, deserializedGraph));
        }
    }
}