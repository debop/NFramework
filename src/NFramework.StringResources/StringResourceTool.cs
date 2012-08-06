using System;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.StringResources {
    /// <summary>
    /// 문자열 리소스에 대한 Helper Class 입니다.
    /// </summary>
    public static class StringResourceTool {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// 기본 리소스 파일명 또는 APPLICATION 명 (string.Empty)
        /// </summary>
        /// <value>DEFAULT</value>
        public static readonly string DEFAULT_ASSEMBLY_NAME = string.Empty;

        /// <summary>
        /// AssemblyName 키
        /// </summary>
        public const string CLASS_ASSEMBLY_NAME = "assemblyName";

        /// <summary>
        /// 리소스 명의 키
        /// </summary>
        public const string CLASS_RESOURCE_NAME = "resourceName";

        /// <summary>
        /// Class key 구분자 ('|')
        /// </summary>
        public const char CLASS_KEY_DELIMITER = '|';

        /// <summary>
        /// Expression 구분자 (',')
        /// </summary>
        public const char EXPRESSION_DELIMITER = ',';

        /// <summary>
        /// Parameterized key start parameter ("${")
        /// </summary>
        public const string PARAM_REF_START = @"${";

        /// <summary>
        /// Parameterized key end parameter ("}")
        /// </summary>
        public const string PARAM_REF_END = "}";

        /// <summary>
        /// 예: &lt;%$ ExtResources: AssemblyName|ResourceFilename, ResourceKey %&gt; 형태를 파싱한다.
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="classKey"></param>
        /// <param name="resourceKey"></param>
        public static void ParseExpression(string expression, out string classKey, out string resourceKey) {
            ParseClassKey(expression, EXPRESSION_DELIMITER, out classKey, out resourceKey);
        }

        /// <summary>
        /// ResourceProviderFactory에 ClassKey로 입력되는 값은 [AssemblyName|]ResourceName 형식이다.
        /// </summary>
        /// <param name="classKey">리소스가 존재하는 위치를 나타내는 문자열 [AssemblyName|]ResourceFileName</param>
        /// <param name="assemblyName">리소스가 정의된 Assembly 명</param>
        /// <param name="resourceFilename">리소스 파일명</param>
        public static void ParseClassKey(string classKey, out string assemblyName, out string resourceFilename) {
            ParseClassKey(classKey, CLASS_KEY_DELIMITER, out assemblyName, out resourceFilename);
        }

        /// <summary>
        /// ResourceProviderFactory에 ClassKey로 입력되는 값은 [AssemblyName|]ResourceType 형식이다.
        /// 이 값을 파싱하여 구분한다.
        /// </summary>
        /// <param name="classKey">리소스가 존재하는 위치를 나타내는 문자열 [AssemblyName|]ResourceFileName</param>
        /// <param name="delimiter">assemblyName과 resourceFileName 사이의 구분자</param>
        /// <param name="assemblyName">리소스가 정의된 Assembly 명</param>
        /// <param name="resourceFilename">리소스 파일명</param>
        public static void ParseClassKey(string classKey, char delimiter, out string assemblyName, out string resourceFilename) {
            if(IsDebugEnabled)
                log.Debug("ClassKey를 파싱합니다. classKey=[{0}], delimiter=[{1}]", classKey, delimiter);

            if(classKey.IsNotWhiteSpace())
                classKey = classKey.TrimStart(CLASS_KEY_DELIMITER);

            if(classKey.IndexOf(delimiter) > 0) {
                var textArray = classKey.Split(new[] { delimiter }, StringSplitOptions.RemoveEmptyEntries);

                if(textArray.Length == 1) {
                    assemblyName = DEFAULT_ASSEMBLY_NAME;
                    resourceFilename = textArray[0].Trim();
                }
                else if(textArray.Length == 2) {
                    assemblyName = textArray[0].Trim();
                    resourceFilename = textArray[1].Trim();
                }
                else {
                    throw new ArgumentException("Invalid classkey=" + classKey);
                }
            }
            else if(classKey.IsNotWhiteSpace()) {
                assemblyName = DEFAULT_ASSEMBLY_NAME;
                resourceFilename = classKey.Trim();
            }
            else {
                throw new ArgumentException("Invalid classkey=" + classKey);
            }

            if(IsDebugEnabled)
                log.Debug("ClassKey 파싱을 완료했습니다. assemblyName=[{0}], resourceFilename=[{1}]", assemblyName, resourceFilename);
        }
    }
}