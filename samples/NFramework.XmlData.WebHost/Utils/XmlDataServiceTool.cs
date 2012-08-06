using NSoft.NFramework.Serializations.Serializers;

namespace NSoft.NFramework.XmlData.WebHost {
    public static class XmlDataServiceTool {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// 원하는 <see cref="ISerializer"/>를 빌드합니다.
        /// </summary>
        /// <param name="compress">압축할 것인가?</param>
        /// <param name="security">보안을 적용할 것인가?</param>
        /// <returns></returns>
        public static ISerializer GetSerializer(bool compress, bool security) {
            if(IsDebugEnabled)
                log.Debug("Create Serializer. compress=[{0}], security=[{1}]", compress, security);

            // ISerializer serializer = new BinarySerializer();
            ISerializer serializer = new CloneSerializer();

            if(compress)
                serializer = new CompressSerializer(serializer);

            if(security)
                serializer = new EncryptSerializer(serializer);

            return serializer;
        }
    }
}