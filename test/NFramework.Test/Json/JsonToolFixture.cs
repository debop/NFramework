using System;
using NSoft.NFramework.LinqEx;
using NSoft.NFramework.UnitTesting;
using NUnit.Framework;
using Newtonsoft.Json;
using SharpTestsEx;

namespace NSoft.NFramework.Json {
    [Microsoft.Silverlight.Testing.Tag("JSON")]
    [TestFixture]
    public class JsonToolFixture : AbstractFixture {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        public const int ThreadCount = 5;

        private static readonly UserInfo user = UserInfo.GetSample();
        private static readonly JsonSerializerSettings _serializerSettings = JsonTool.DefaultJsonSerializerSettings;

        [Test]
        public void SerializeAsText_DeserializeFromText() {
            TestTool.RunTasks(ThreadCount,
                              () => {
                                  var serialized = JsonTool.SerializeAsText(user);
                                  var deserializedUser = JsonTool.DeserializeFromText<UserInfo>(serialized);
                                  VerifySerializer(user, deserializedUser);
                              });
        }

        [Test]
        public void SerializeAsBytes_DeserializeFromBytes() {
            TestTool.RunTasks(ThreadCount,
                              () => {
                                  var serialized = JsonTool.SerializeAsBytes(user);
                                  var deserializedUser = JsonTool.DeserializeFromBytes<UserInfo>(serialized);
                                  VerifySerializer(user, deserializedUser);
                              });
        }

        [Test]
        public void SerializeWithCompress_DeserializeWithDecompress() {
            TestTool.RunTasks(ThreadCount,
                              () => {
                                  var serialized = JsonTool.SerializeWithCompress(user);
                                  var deserializedUser = JsonTool.DeserializeWithDecompress<UserInfo>(serialized);
                                  VerifySerializer(user, deserializedUser);
                              });
        }

        public static void VerifySerializer(UserInfo source, UserInfo deserialized) {
            TestTool.RunTasks(ThreadCount,
                              () => {
                                  Assert.IsNotNull(deserialized);
                                  Assert.AreEqual(source.UserName, deserialized.UserName);
                                  Assert.AreEqual(source.FavoriteMovies.Count, deserialized.FavoriteMovies.Count);

                                  deserialized.HomeAddr.Proeprties.Contains("home").Should().Be.True();
                                  deserialized.OfficeAddr.Proeprties.Contains("office").Should().Be.True();
                              });
        }

        [Test]
        public void JsonMapping() {
            TestTool.RunTasks(ThreadCount,
                              () => {
                                  var source = user;
                                  var target = (UserInfo)JsonTool.Map(source, typeof(UserInfo), _serializerSettings);
                                  Assert.IsNotNull(target);

                                  VerifyMappingObject(source,
                                                      target,
                                                      (s, t) => Assert.AreEqual(s.UserName, t.UserName),
                                                      (s, t) => Assert.AreEqual(s.Description, t.Description),
                                                      (s, t) => Assert.AreEqual(s.ByteArray, t.ByteArray),
                                                      (s, t) => Assert.AreEqual(s.FavoriteMovies.Count, t.FavoriteMovies.Count),
                                                      (s, t) => Assert.AreEqual(s.HomeAddr.Phone, t.HomeAddr.Phone));
                              });
        }

        [Test]
        public void JsonMappingGeneric() {
            TestTool.RunTasks(ThreadCount,
                              () => {
                                  var source = user;
                                  var target = JsonTool.Map<UserInfo>(source, _serializerSettings);

                                  Assert.IsNotNull(target);

                                  VerifyMappingObject(source,
                                                      target,
                                                      (s, t) => Assert.AreEqual(s.UserName, t.UserName),
                                                      (s, t) => Assert.AreEqual(s.Description, t.Description),
                                                      (s, t) => Assert.AreEqual(s.ByteArray, t.ByteArray),
                                                      (s, t) => Assert.AreEqual(s.FavoriteMovies.Count, t.FavoriteMovies.Count),
                                                      (s, t) => Assert.AreEqual(s.HomeAddr.Phone, t.HomeAddr.Phone));
                              });
        }

        [Test]
        public void JsonTryMapping() {
            TestTool.RunTasks(ThreadCount,
                              () => {
                                  var source = user;
                                  object target;

                                  var mapping = JsonTool.TryMap(source, source.GetType(), _serializerSettings, out target);

                                  Assert.IsTrue(mapping);
                                  var targetUser = target as UserInfo;

                                  Assert.IsNotNull(targetUser);

                                  VerifyMappingObject(source,
                                                      targetUser,
                                                      (s, t) => Assert.AreEqual(s.UserName, t.UserName),
                                                      (s, t) => Assert.AreEqual(s.Description, t.Description),
                                                      (s, t) => Assert.AreEqual(s.ByteArray, t.ByteArray),
                                                      (s, t) => Assert.AreEqual(s.FavoriteMovies.Count, t.FavoriteMovies.Count),
                                                      (s, t) => Assert.AreEqual(s.HomeAddr.Phone, t.HomeAddr.Phone));
                              });
        }

        [Test]
        public void JsonTryMappingGeneric() {
            TestTool.RunTasks(ThreadCount,
                              () => {
                                  var source = user;
                                  UserInfo target;

                                  var mapping = JsonTool.TryMap<UserInfo>(source, _serializerSettings, out target);

                                  Assert.IsTrue(mapping);
                                  Assert.IsNotNull(target);

                                  VerifyMappingObject(source,
                                                      target,
                                                      (s, t) => Assert.AreEqual(s.UserName, t.UserName),
                                                      (s, t) => Assert.AreEqual(s.Description, t.Description),
                                                      (s, t) => Assert.AreEqual(s.ByteArray, t.ByteArray),
                                                      (s, t) => Assert.AreEqual(s.FavoriteMovies.Count, t.FavoriteMovies.Count),
                                                      (s, t) => Assert.AreEqual(s.HomeAddr.Phone, t.HomeAddr.Phone));
                              });
        }

        private static void VerifyMappingObject<S, T>(S source, T target, params Action<S, T>[] verifyFuncs) {
            if(IsDebugEnabled)
                log.Debug("Verify Mapping Object...");

            verifyFuncs.RunEach(func => func(source, target));
        }
    }
}