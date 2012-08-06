using Castle.Windsor;
using NSoft.NFramework.FusionCharts.Charts;
using NSoft.NFramework.InversionOfControl;
using NUnit.Framework;

namespace NSoft.NFramework.FusionCharts.Themes {
    // NOTE: 같은 수형의 Attribute를 DI를 하려고 했더니, 문제가 발생한다.
    // NOTE: SingleSeries는 baseFont, outCnvBaseFont 둘다 설정했지만, 
    // NOTE: MultiSeries는 둘다 설정하지 않았는데도, baseFont로 설정해버린다... Castle DI에서 문제가 발생했다.
    //       우선은 어쩔 수 없이, 속성 설정 시 prefix를 재확인해서 설정한다.
    /// <summary>
    /// 
    /// </summary>
    [TestFixture]
    public class ChartIoCTestFixture : ChartTestFixtureBase {
        public const string DefaultConfiguration = @"file://Themes/IoC.Charts.Default.config";

        [TestFixtureSetUp]
        public void ClassSetUp() {
            if(IoC.IsInitialized == false)
                IoC.Initialize(new WindsorContainer(DefaultConfiguration));
        }

        [TestCase("SingleSeriesChart")]
        [TestCase("MultiSeriesChart")]
        public void ResolveChartComponent(string componentId) {
            var chart = IoC.Resolve<IChart>(componentId);

            Assert.IsNotNull(chart);
            Assert.IsInstanceOf<FusionChartBase>(chart);

            var fusionChart = chart as FusionChartBase;

            Assert.IsNotNull(fusionChart);
            fusionChart.Caption = componentId;

            // ssChart.BaseFontAttr.FontSize = "16";
            ValidateChartXml(fusionChart);
        }

        [Test]
        public void ResolveAllComponents() {
            var charts = IoC.ResolveAll<IChart>();
            Assert.IsNotNull(charts);

            foreach(var chart in charts) {
                Assert.IsNotNull(chart);
                Assert.IsInstanceOf<IChart>(chart);

                ValidateChartXml(chart);
            }
        }
    }
}