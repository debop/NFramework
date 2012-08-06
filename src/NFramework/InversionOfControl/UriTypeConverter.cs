using System;
using Castle.Core.Configuration;
using Castle.MicroKernel.SubSystems.Conversion;

namespace NSoft.NFramework.InversionOfControl {
    /// <summary>
    /// Castle.Windsor 환경설정에서 parameters 의 값으로 URI 형태의 문자열을 형변환을 수행한다.
    /// </summary>
    [Serializable]
    public class UriTypeConverter : AbstractTypeConverter {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// 형변환이 가능한지 검사한다.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public override bool CanHandleType(Type type) {
            return type.IsAssignableFrom(typeof(Uri));
        }

        /// <summary>
        /// 환경설정에 정의된 값을 이용하여 URI 형으로 변환하여 반환한다.
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="targetType"></param>
        /// <returns></returns>
        public override object PerformConversion(IConfiguration configuration, Type targetType) {
            return PerformConversion(configuration.Value, targetType);
        }

        /// <summary>
        /// 지정된 값을 이용하여 URI 형으로 변환하여 반환한다.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <returns></returns>
        public override object PerformConversion(string value, Type targetType) {
            if(IsDebugEnabled)
                log.Debug("Convert specified string to Uri instance. value=[{0}]", value);

            return new Uri(value);
        }
    }
}