using System;
using System.Collections;
using System.Drawing;
using System.Linq;
using System.Web.Services;
using NCalc;
using NSoft.NFramework.FusionCharts.Charts;
using NSoft.NFramework.LinqEx;
using NSoft.NFramework.Threading;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.FusionCharts.WebHost.Charts.Ajax {
    /// <summary>
    /// MathExprEvaluator의 요약 설명입니다.
    /// </summary>
    [WebService(Namespace = "http://ws.realweb21.com/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class MathExprEvaluator : NSoft.NFramework.FusionCharts.Web.DataXmlHandlerBase {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// X축 변량의 갯수
        /// </summary>
        private const int VarCount = 500;

        private static readonly Random rnd = new ThreadSafeRandom();

        public string Expression { get; set; }
        public double Lower { get; set; }
        public double Upper { get; set; }

        public override IChart BuildFusionChart() {
            ParseParameters();

            var chart = new SingleSeriesChart
                        {
                            Caption = "사용자 정의 그래프",
                            SubCaption = Expression,
                            Palette = rnd.Next(1, 5),
                            XAxisName = "X",
                            YAxisName = "Y",
                            //NumVisiblePlot = VarCount / 10,
                            LabelStep = Math.Min(VarCount / 50, 50),
                            BaseFontAttr = { Font = "맑은 고딕", FontSize = "16" },
                            BorderAttr = { Show = true },
                            ShowLabels = true,
                            RotateLabels = false,
                            ShowValues = false,
                            BackgroundAttr =
                            {
                                BgColor = Color.WhiteSmoke,
                                BgAlpha = 100
                            },
                            ShowShadow = true,
                        };

            chart.Categories.FontAttr.FontSize = "8";

            BuildExpressionGraph(chart);

            return chart;
        }

        private void ParseParameters() {
            //! 수식은 특수문자가 있을 수 있기때문에 Base64 Encoding/Decoding으로 값을 전달합니다.
            //
            Expression = Request["Expr"].AsText().Base64Decode().ToText();
            Lower = Request["Lower"].AsDouble(-100);
            Upper = Request["Upper"].AsDouble(100);

            if(log.IsDebugEnabled)
                log.Debug("ParsedParameters... Expression={0}, Lower={1}, Upper={2}", Expression, Lower, Upper);

            if(Lower > Upper) {
                var temp = Lower;
                Lower = Upper;
                Upper = temp;
            }
            if(Math.Abs(Upper - Lower) < double.Epsilon)
                Upper += 100;
        }

        private void BuildExpressionGraph(SingleSeriesChart chart) {
            var expr = new NCalc.Expression(Expression, EvaluateOptions.IgnoreCase | EvaluateOptions.IterateParameters);

            var step = (Upper - Lower) / VarCount;

            var x = EnumerableTool.Step(Lower, Upper, step).ToArray();
            expr.Parameters["x"] = x;

            var y = ((IEnumerable)expr.Evaluate()).ToListUnsafe<double>();
            for(var i = 0; i < x.Length; i++) {
                chart.SetElements.Add(new ValueSetElement(y[i])
                                      {
                                          Label = x[i].ToString("#.0")
                                      });
            }
        }
    }
}