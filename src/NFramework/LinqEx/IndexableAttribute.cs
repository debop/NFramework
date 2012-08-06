using System;

namespace NSoft.NFramework.LinqEx {
    /// <summary>
    /// LINQ 의 검색시에 속도를 빠르게 하기 위해 인덱싱해야 할 속성에 지정한다.
    /// </summary>
    /// <remarks>
    /// http://www.codeplex.com/i4o
    /// </remarks>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class IndexableAttribute : Attribute {}
}