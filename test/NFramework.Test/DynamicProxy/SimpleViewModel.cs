using System;

namespace NSoft.NFramework.DynamicProxy {
    /// <summary>
    /// NOTE: Proxy에 의해 Interceptor가 수행되기 위해서는 해당 속성 또는 메소드가 virtual 이어야 합니다.
    /// </summary>
    [Serializable]
    public class SimpleViewModel : ValueObjectBase {
        public virtual string Name { get; set; }

        public virtual int? Age { get; set; }

        public virtual string Description { get; set; }

        public override int GetHashCode() {
            return HashTool.Compute(Name);
        }
    }
}