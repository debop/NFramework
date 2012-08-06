using System;
using System.Reflection;
using Castle.Core.Configuration;
using Castle.MicroKernel.SubSystems.Conversion;
using NSoft.NFramework.Reflections;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.InversionOfControl {
    /// <summary>
    /// Castle.Windsor의 환경설정상의 property value를 인스턴스 속성에 지정하는 작업을 수행한다.
    /// </summary>
    [Serializable]
    public class ConfigurationObjectConverter : AbstractTypeConverter {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// <see cref="ConfigurationObjectAttribute"/> 가 정의된 Class에 대해서 Windsor의 Prameters 값을 변환해준다.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public override bool CanHandleType(Type type) {
            return type.IsDefined(typeof(ConfigurationObjectAttribute), true);
        }

        /// <summary>
        /// Castle.Windsor 환경설정 정보에 있는 속성정보를 Instance의 속성 값으로 매핑한다.
        /// </summary>
        /// <param name="configuration">Castle configuration object</param>
        /// <param name="targetType">target type</param>
        /// <returns></returns>
        public override object PerformConversion(IConfiguration configuration, Type targetType) {
            configuration.ShouldNotBeNull("configuration");
            targetType.ShouldNotBeNull("targetType");

            if(IsDebugEnabled)
                log.Debug("Perform conversion configuration value to property. configuration={0}, targetType={1}",
                          configuration, targetType);

            var instance = ActivatorTool.CreateInstance(targetType);

            var accessor = DynamicAccessorFactory.CreateDynamicAccessor(targetType, false);

            const BindingFlags flags = BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

            foreach(var itemConfig in configuration.Children) {
                var propInfo = targetType.GetProperty(itemConfig.Name, flags);

                // convert from string to object
                var value = Context.Composition.PerformConversion(itemConfig.Value, propInfo.PropertyType);
                accessor.SetPropertyValue(instance, itemConfig.Name, value);
            }
            return instance;
        }

        /// <summary>
        /// Castle.Windsor 환경설정 정보에 있는 값을 해당 형식의 생성자에 제공하여 인스턴스를 빌드한다.
        /// </summary>
        /// <param name="value">인스턴싱할때 생성자에 제공할 인자 값</param>
        /// <param name="targetType">인스턴싱할 형식</param>
        /// <returns></returns>
        public override object PerformConversion(string value, Type targetType) {
            targetType.ShouldNotBeNull("targetType");

            if(IsDebugEnabled)
                log.Debug("Perform conversion configuration value to property. value:{0}, targetType:{1}", value, targetType);

            return ActivatorTool.CreateInstance(targetType, new object[] { value });
        }
    }
}