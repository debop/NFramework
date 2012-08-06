using System;

namespace NSoft.NFramework {
    /// <summary>
    /// DDD에서 Value Object 나타내는 기본 인터페이스
    /// </summary>
    public interface IValueObject : IEquatable<IValueObject> {}
}