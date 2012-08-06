using System;
using System.Drawing;
using System.Web.Services;
using NSoft.NFramework.FusionCharts.Powers;
using NSoft.NFramework.Threading;

namespace NSoft.NFramework.FusionCharts.WebHost.Powers.MultilevelPieChart {
    /// <summary>
    /// 다단계 파이차트용 Data 제공자입니다.
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class MultilevelPieChartDataProvider : NSoft.NFramework.FusionCharts.Web.DataXmlHandlerBase {
        public const string HoverTextFormat = "<b><u>{0}</u></b><br>{1}<br>Joined: {2}<br>More detail can come here...";

        private static readonly Random rnd = new ThreadSafeRandom();

        #region << DataXmlHandlerBase >>

        /// <summary>
        /// 원하는 Chart를 빌드합니다.
        /// </summary>
        public override IChart BuildFusionChart() {
            var option = Request["Option"].AsText("Organizations");

            var chart = new MultilevelPie();
            SetChartAttributes(chart);
            chart.Caption = option + " Chart";

            switch(option) {
                case "Friends":
                    BuildFriendsChart(chart);
                    break;

                case "Keywords":
                    BuildKeywordsChart(chart);
                    break;

                default:
                    BuildOrganizationsChart(chart);
                    break;
            }

            return chart;
        }

        #endregion

        private static void SetChartAttributes(MultilevelPie chart) {
            chart.Palette = rnd.Next(1, 6);
            chart.BaseFontAttr.FontSize = "12";
            chart.BaseFontAttr.IsBold = true;
            chart.ShowLabels = true;
            chart.PieFillAlpha = 34;
            chart.PieBorderThickness = 3;
            chart.HoverFillColor = "#FDCEDA".FromHtml();
            chart.PieBorderColor = Color.White;

            // 이걸 해줘야 ToolText의 Html 이 먹는다.
            chart.Styles.Definition.Add(new FontStyle("myHtmlFont") { IsHTML = true });
            chart.Styles.Definition.Add(new FontStyle("captionFont") { Size = 16 });

            chart.Styles.AddApply("ToolTip", "myHtmlFont");
            chart.Styles.AddApply("Caption", "captionFont");
        }

        private static void BuildFriendsChart(MultilevelPie chart) {}
        private static void BuildKeywordsChart(MultilevelPie chart) {}

        private static void BuildOrganizationsChart(MultilevelPie chart) {
            // CEO
            var ceo = CreateMultilevelCategoryElement("CEO", "Chief Enterprise Officer", "Mr. Lucas Smith", 1988);
            chart.SetRootCategory(ceo);

            var cto = CreateMultilevelCategoryElement("CTO", "Chief Technology Officer", "Mr Ed Harley", 1988);
            ceo.AddChildCategory(cto);

            var pm = CreateMultilevelCategoryElement("Proj. Manager", "Project Manager", "Mr. Daniel Edwards", null)
                .AddChildCategory(CreateMultilevelCategoryElement("Design", "Design  Team", "정경선, 고이정", null))
                .AddChildCategory(CreateMultilevelCategoryElement("Coding", "Coding Team", "권진석, 신동규", null))
                .AddChildCategory(CreateMultilevelCategoryElement("Validataion", "Validataion Team", "오선영", null));

            cto.AddChildCategory(pm);

            var qa
                = CreateMultilevelCategoryElement("Q & A Manager", "Q & A Manager", "김홍준", null)
                    .AddChildCategory(CreateMultilevelCategoryElement("Testing Team", "Testing Team", "Nik Arora", null));
            cto.AddChildCategory(qa);

            var arch
                = CreateMultilevelCategoryElement("Architect", "System Architecture Manager", "배성혁", null)
                    .AddChildCategory(CreateMultilevelCategoryElement("Study", "System Requirement Study Team", "이인철", null))
                    .AddChildCategory(CreateMultilevelCategoryElement("Design", "Design", "Ravi Srivastav", null));
            cto.AddChildCategory(arch);

            var cfo = CreateMultilevelCategoryElement("CFO", "Chief Financial Officer", "한윤근", 1987);
            ceo.AddChildCategory(cfo);

            var payables
                = CreateMultilevelCategoryElement("Payables", "Accounts Payable Team")
                    .AddChildCategory(CreateMultilevelCategoryElement("Salary", "Team for salary accounts maintenance and upkeep"))
                    .AddChildCategory(CreateMultilevelCategoryElement("Purchase", "Team for accounting of purchase of S/W and godds"))
                    .AddChildCategory(CreateMultilevelCategoryElement("Other", "Other Payments"));
            cfo.AddChildCategory(payables);

            var receivables
                = CreateMultilevelCategoryElement("Receivables", "Accounts receivables Team")
                    .AddChildCategory(CreateMultilevelCategoryElement("SW1", "Online Software Receipts Accounts"))
                    .AddChildCategory(CreateMultilevelCategoryElement("SW2", "Physical Software Receipts Accounts"))
                    .AddChildCategory(CreateMultilevelCategoryElement("SER1", "Services Collection"))
                    .AddChildCategory(CreateMultilevelCategoryElement("SER2", "Services Collection (Physical)"))
                    .AddChildCategory(CreateMultilevelCategoryElement("OTR", "Subscription and Other Collections"));
            cfo.AddChildCategory(receivables);

            var cio = CreateMultilevelCategoryElement("CIO", "Chief Information Officer", "Mr. David Brown", 1992);
            ceo.AddChildCategory(cio);

            var pr = CreateMultilevelCategoryElement("PR", "PR Team")
                .AddChildCategory(CreateMultilevelCategoryElement("Packaging", "Packaging Staff"))
                .AddChildCategory(CreateMultilevelCategoryElement("Inv Rel.", "Investor Relations Upkeep"))
                .AddChildCategory(CreateMultilevelCategoryElement("Marketing", "Marketing & Sales"));
            cio.AddChildCategory(pr);

            var hr = CreateMultilevelCategoryElement("HR", "HR Team")
                .AddChildCategory(CreateMultilevelCategoryElement("Selection", "Selection of Candidates"))
                .AddChildCategory(CreateMultilevelCategoryElement("Deploying", "Deploying at required site"));
            cio.AddChildCategory(hr);
        }

        private static MultilevelCategoryElement CreateMultilevelCategoryElement(string label, string title, string name, int? hierYear) {
            return CreateMultilevelCategoryElement(label,
                                                   string.Format(HoverTextFormat, title, name, hierYear));
        }

        private static MultilevelCategoryElement CreateMultilevelCategoryElement(string label, string hoverText) {
            return new MultilevelCategoryElement
                   {
                       Label = label,
                       HoverText = hoverText
                   };
        }
    }
}