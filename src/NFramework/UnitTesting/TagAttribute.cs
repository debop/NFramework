using System;

#if !SILVERLIGHT

namespace Microsoft.Silverlight.Testing {
    /// <summary>
    /// Silverlight 테스팅 시에 NUnit의 Category 역할을 하도록 하는 Attribute 입니다. Tag에 해당되는 단위 테스트들만 테스트를 수행하도록 할 수 있습니다.
    /// </summary>
    [Serializable]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class TagAttribute : Attribute {
        public TagAttribute(string tag) {
            Tag = tag;
        }

        public string Tag { get; private set; }
    }
}

#endif