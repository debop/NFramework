using System;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.FusionCharts.WebHost.Charts.Ajax {
    public partial class MathExpr : System.Web.UI.Page {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        public const string DefaultExpression = @"sin(x)";

        protected void Page_Load(object sender, EventArgs e) {
            if(IsPostBack == false) {
                edExpression.Text = DefaultExpression;
                edLowerX.Text = "-10";
                edUpperX.Text = "10";
                DrawGraph("sin(x)", -10, 10);
            }
        }

        protected void btnEvaluate_Click(object sender, EventArgs e) {
            var expr = edExpression.Text.AsText(DefaultExpression);
            var lower = edLowerX.Text.AsDouble(-10);
            var upper = edUpperX.Text.AsDouble(10);

            DrawGraph(expr, lower, upper);
        }

        protected void DrawGraph(string expression, double lower, double upper) {
            if(IsDebugEnabled)
                log.Debug("그래프 작성용 정보... Expr=[{0}], Lower=[{1}], Upper=[{2}]", expression, lower, upper);

            if(expression.IsWhiteSpace())
                expression = DefaultExpression;

            //
            //! 수식은 특수문자가 많기때문에 Base64 로 인코딩합니다.
            //
            graphChart.DataUrl = string.Format("MathExprEvaluator.ashx?Expr={0}&Lower={1}&Upper={2}", expression.Base64Encode(), lower,
                                               upper);
        }
    }
}