namespace NSoft.NFramework.FusionCharts.Charts {
    /// <summary>
    /// Multi-Series Stacked Chart 등에서 DataSet이 내부에 여러개의 DataSet을 가진다.
    /// </summary>
    public class DataSetCollection : CollectionElement<DataSetElement> {
        public DataSetCollection()
            : base("dataset") {}
    }
}