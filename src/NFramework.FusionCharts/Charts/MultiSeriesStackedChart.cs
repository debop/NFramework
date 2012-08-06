using System;
using System.Collections.Generic;
using System.Xml;

namespace NSoft.NFramework.FusionCharts.Charts {
    /// <summary>
    /// DataSet이 DataSet을 가진다.
    /// </summary>
    public class MultiSeriesStackedChart : MultiSeriesChart {
        private IList<DataSetCollection> _dataSetCollections;

        public virtual IList<DataSetCollection> DataSetCollections {
            get { return _dataSetCollections ?? (_dataSetCollections = new List<DataSetCollection>()); }
            set { _dataSetCollections = value; }
        }

        [Obsolete("MultiSeriesStacked 는 DataSetCollection을 사용하셔야 합니다.")]
        protected new IList<DataSetElement> DataSets {
            get { return base.DataSets; }
        }

        protected override void GenerateXmlElements(XmlWriter writer) {
            base.GenerateXmlElements(writer);

            if(_dataSetCollections != null && _dataSetCollections.Count > 0)
                foreach(var dataSetCollection in _dataSetCollections)
                    dataSetCollection.WriteXmlElement(writer);
        }
    }
}