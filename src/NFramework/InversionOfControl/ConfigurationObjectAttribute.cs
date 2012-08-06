using System;

namespace NSoft.NFramework.InversionOfControl {
    /// <summary>
    /// IoC 환경설정에 쓰이는 Class임을 나타내는 Attribute이다.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ConfigurationObjectAttribute : Attribute {}
}