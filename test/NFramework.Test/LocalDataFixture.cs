using System;
using System.Threading;
using NSoft.NFramework.UnitTesting;
using NUnit.Framework;

namespace NSoft.NFramework {
    /// <summary>
    /// 비동기 방식에서는 사용하면 안됩니다.
    /// </summary>
    [TestFixture]
    public class LocalDataFixture : AbstractFixture {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;
        private static readonly bool IsInfoEnabled = log.IsInfoEnabled;

        #endregion

        [Test]
        public void ThreadTest() {
            TestTool.RunTasks(5,
                              CanSaveAndLoadValueType,
                              NotYetSetValueIsNull,
                              SaveAndLoadRefenceType,
                              GetOrAdd);
        }

        [Test]
        public void CanSaveAndLoadValueType() {
            const string key = @"LocalDataFixture.Value.Key";
            var value = Guid.NewGuid().ToString();

            Local.Data[key] = value;

            if(IsDebugEnabled)
                log.Debug("Key=[{0}], Value=[{1}]", key, Local.Data[key]);

            Assert.AreEqual(value, Local.Data[key]);
        }

        [Test]
        public void NotYetSetValueIsNull() {
            var key = Guid.NewGuid().ToString();

            Assert.IsNull(Local.Data[key]);
            Local.Data[key] = key;

            Assert.AreEqual(key, Local.Data[key]);
        }

        [Test]
        public void SaveAndLoadRefenceType() {
            const string key = @"LocalDataFixture.Reference.Key";

            var user = new User
                       {
                           Name = "user",
                           Password = "P" + Thread.CurrentThread.ManagedThreadId
                       };

            Local.Data[key] = user;

            Thread.Sleep(5);

            var storedUser = Local.Data[key] as User;

            Assert.IsNotNull(storedUser);
            Assert.AreEqual(user.Name, storedUser.Name);
            Assert.AreEqual(user.Password, storedUser.Password);

            if(IsDebugEnabled)
                log.Debug("User=[{0}], Stored User=[{1}]", user, storedUser);


            object aUser;

            if(Local.Data.TryGetValue(key, out aUser)) {
                storedUser = aUser as User;

                Assert.IsNotNull(storedUser);
                Assert.AreEqual(user.Name, storedUser.Name);
                Assert.AreEqual(user.Password, storedUser.Password);

                if(IsDebugEnabled)
                    log.Debug("User=[{0}], Stored User=[{1}]", user, storedUser);
            }
            else {
                Assert.Fail("Local.Data에 자료가 없습니다. key=" + key);
            }
        }

        [Test]
        public void GetOrAdd() {
            const string key = @"LocalDataFixture.Reference.Key";

            var user = Local.Data.GetOrAdd(key,
                                           () => new User
                                                 {
                                                     Name = "user",
                                                     Password = "P" + Thread.CurrentThread.ManagedThreadId
                                                 });

            Thread.Sleep(5);

            var storedUser = Local.Data[key] as User;

            Assert.AreEqual(user, storedUser);

            Local.Data.SetValue<User>(key, (u) => u.Password = "123");

            Assert.AreEqual("123", user.Password);
        }

        public class User : IEquatable<User> {
            public string Name { get; set; }

            public string Password { get; set; }

            public bool Equals(User other) {
                return (other != null) && GetHashCode().Equals(other.GetHashCode());
            }

            public override bool Equals(object obj) {
                return (obj != null) && (obj is User) && Equals((User)obj);
            }

            public override int GetHashCode() {
                return HashTool.Compute(Name, Password);
            }

            public override string ToString() {
                return string.Format("User# Name={0}, Password={1}", Name, Password);
            }
        }
    }
}