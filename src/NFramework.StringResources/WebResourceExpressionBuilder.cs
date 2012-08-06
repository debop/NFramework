using System;
using System.CodeDom;
using System.Globalization;
using System.Web.Compilation;
using System.Web.UI;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.StringResources {
    /// <summary>
    /// 웹 리소스 정보를 추출하기 위해 표현식을 만드는 클래스입니다.
    /// </summary>
    public class WebResourceExpressionBuilder : ResourceExpressionBuilder {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// 외부 Resource Assembly로부터 지정한 리소스 키에 해당하는 값을 가져온다.
        /// </summary>
        public static object GetGlobalResourceObject(string classKey, string resourceKey, CultureInfo culture = null) {
            culture = CultureTool.GetOrCurrentUICulture(culture);

            if(IsDebugEnabled)
                log.Debug("전역 리소스 값을 로드합니다... classKey=[{0}], resourceKey=[{1}], culture=[{2}]",
                          classKey, resourceKey, culture);

            var value = ResourceProvider.GetObject(classKey, resourceKey, culture);

            if(IsDebugEnabled)
                log.Debug("전역 리소스 값을 로드했습니다!!! classKey=[{0}], resourceKey=[{1}], culture=[{2}], value=[{3}]",
                          classKey, resourceKey, culture, value);

            return value;
        }

        /// <summary>
        /// Database 로부터 지역 리소스 정보를 얻는다.
        /// </summary>
        /// <param name="classKey"></param>
        /// <param name="resourceKey"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public static object GetLocalResourceObject(string classKey, string resourceKey, CultureInfo culture = null) {
            culture = culture ?? CultureInfo.CurrentCulture;

            if(IsDebugEnabled)
                log.Debug("Get Local Resource object. classKey=[{0}], resourceKey=[{1}], culture=[{2}]",
                          classKey, resourceKey, culture);

            var value = ResourceProvider.GetObject(classKey, resourceKey, culture);

            if(IsDebugEnabled)
                log.Debug("Get Local Resource object. value=[{0}]", value);

            return value;
        }

        ///<summary>
        /// 리소스 파일에서 값을 반환합니다.
        ///</summary>
        ///
        ///<returns>
        /// 구문 분석한 식과 관련된 <see cref="T:System.Object" />입니다. 구문 분석한 식은 클래스 이름과 리소스 키를 포함합니다.
        ///</returns>
        ///
        ///<param name="target">
        /// 식이 포함된 개체입니다.
        /// </param>
        ///<param name="entry">
        /// 식에 의해 바인딩된 속성에 대한 정보를 나타내는 개체입니다.
        /// </param>
        ///<param name="parsedData">
        /// <see cref="ParseExpression"/> 메서드에 의해 반환된 구문 분석한 데이터가 들어 있는 개체입니다.
        ///</param>
        ///<param name="context">
        /// 식을 계산하는 데 필요한 컨텍스트 정보입니다.
        ///</param>
        public override object EvaluateExpression(object target,
                                                  BoundPropertyEntry entry,
                                                  object parsedData,
                                                  ExpressionBuilderContext context) {
            if(IsDebugEnabled)
                log.Debug("표현식을 평가합니다... target=[{0}], entry=[{1}], parsedData=[{2}], context=[{3}]", target, entry, parsedData, context);

            var field = parsedData as ExpressionField;
            Guard.Assert(field != null, "파싱된 정보가 ExpressionField 형식이 아닙니다. parseData=[{0}]", parsedData);

            return ResourceProvider.GetObject(field.ClassKey, field.ResourceKey);
        }

        ///<summary>
        /// 구문 분석된 식을 나타내는 개체를 반환합니다.
        ///</summary>
        ///<returns>
        /// 구문 분석한 식을 나타내는 <see cref="T:System.Object" />입니다.
        ///</returns>
        ///<param name="expression">선언적 식의 값입니다.</param>
        ///<param name="propertyType">식에 의해 바인딩된 속성의 형식입니다.</param>
        ///<param name="context">식을 계산하는 데 필요한 컨텍스트 정보입니다.</param>
        ///<exception cref="T:System.Web.HttpException">리소스 식이 없거나 유효하지 않은 경우</exception>
        public override object ParseExpression(string expression, Type propertyType, ExpressionBuilderContext context) {
            expression.ShouldNotBeWhiteSpace("expression");

            if(IsDebugEnabled)
                log.Debug("표현식을 파싱합니다... expression=[{0}], propertyType=[{1}], context=[{2}]", expression, propertyType, context);

            string classKey;
            string resourceKey;

            //+ 예 : <%$ ExternalResources : AssemblyName|ResourceFileName, ResourceKey %> 를 파싱한다.
            //
            StringResourceTool.ParseClassKey(expression, ',', out classKey, out resourceKey);
            var field = new ExpressionField(classKey, resourceKey);

            // 실제로 값이 있는지 검사한다.
            if(ResourceProvider.GetObject(field.ClassKey, field.ResourceKey) == null)
                throw new InvalidOperationException("지정한 리소스를 찾을 수 없습니다. resource key=" + field.ResourceKey);

            return field;
        }

        ///<summary>
        /// 페이지 실행 중에 계산할 코드 식을 반환합니다.
        /// </summary>
        ///<returns>
        /// 메서드를 호출하는 <see cref="T:System.CodeDom.CodeExpression" />입니다.
        ///</returns>
        ///<param name="entry">개체의 속성 이름입니다.</param>
        ///<param name="parsedData">식의 구문 분석된 값입니다.</param>
        ///<param name="context">컨트롤 또는 페이지에 대한 속성입니다.</param>
        public override CodeExpression GetCodeExpression(BoundPropertyEntry entry, object parsedData,
                                                         ExpressionBuilderContext context) {
            var field = parsedData as ExpressionField;
            field.ShouldNotBeNull("filed");

            if(IsDebugEnabled)
                log.Debug("Get code expression for NO Compiled ASP.NET page. entry=[{0}], parsedData=[{1}], context=[{2}]", entry,
                          parsedData, context);

            // GetGlobalResourceObject를 호출한다.
            //
            return
                new CodeMethodInvokeExpression(
                    new CodeTypeReferenceExpression(typeof(WebResourceExpressionBuilder)),
                    "GetGlobalResourceObject",
                    new CodePrimitiveExpression(field.ClassKey),
                    new CodePrimitiveExpression(field.ResourceKey)
                    );
        }

        ///<summary>
        ///비컴파일 기능을 사용하는 페이지에서 식이 계산될 수 있는지 여부를 나타내는 값을 반환합니다.
        ///</summary>
        ///<returns>
        ///모든 경우에 true를 반환합니다.
        ///</returns>
        public override bool SupportsEvaluate {
            get { return true; }
        }
    }
}