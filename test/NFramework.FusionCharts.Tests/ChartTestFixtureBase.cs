using System;
using NSoft.NFramework.Threading;
using NSoft.NFramework.Xml;
using NUnit.Framework;

namespace NSoft.NFramework.FusionCharts {
    /// <summary>
    /// Fusion Chart Unit Test를 위한 Test Class의 추상클래스입니다.
    /// </summary>
    [TestFixture]
    public abstract class ChartTestFixtureBase {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        protected static readonly Random rnd = new ThreadSafeRandom();

        public static void ValidateChartXml(IChart chart) {
            var xmlText = chart.GetDataXml(true);

            if(IsDebugEnabled) {
                log.Debug("Chart Xml:");
                log.Debug(xmlText);
            }

            var doc = new XmlDoc(xmlText);

            Assert.IsTrue(doc.IsValidDocument);
        }
    }
}