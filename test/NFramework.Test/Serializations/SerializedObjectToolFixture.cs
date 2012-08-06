using System;
using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.Serializations {
    [TestFixture]
    public class SerializedObjectToolFixture {
        [Test]
        public void ParseSerializationMethod() {
            foreach(var method in Enum.GetValues(typeof(SerializationMethod))) {
                method.AsText().AsEnum(SerializationMethod.None).Should().Be(method);
            }
        }
    }
}