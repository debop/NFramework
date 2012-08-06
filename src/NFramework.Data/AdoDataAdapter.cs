using System;
using System.Data;
using System.Data.Common;

namespace NSoft.NFramework.Data {
    /// <summary>
    /// 상속이 불가능한 DataAdapter의 protected 메소드를 사용하기 위해, Delegate를 이용하여, 
    /// 범위를 지정할 수 있는 Fill 메소드를 사용할 수 있도록 하였습니다.
    /// </summary>
    public sealed class AdoDataAdapter : IDisposable {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private readonly FillDataSetCommand _doFillDataSetCommand;
        private readonly FillDataTablesCommand _doFillDataTablesCommand;
        private readonly DisposeCommand _doDisposeCommand;

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="adapter">래핑될 실제 <see cref="DataAdapter"/> 인스턴스</param>
        public AdoDataAdapter(DataAdapter adapter) {
            adapter.ShouldNotBeNull("adapter");

            if(IsDebugEnabled)
                log.Debug("AdoDataAdapter를 생성합니다... 내부적으로 System.Data.Common.DataAdapter의 protected 메소드들을 활용합니다");

            DataAdapter = adapter;

            _doFillDataSetCommand = (FillDataSetCommand)Delegate.CreateDelegate(typeof(FillDataSetCommand), DataAdapter, "Fill");
            _doFillDataTablesCommand =
                (FillDataTablesCommand)Delegate.CreateDelegate(typeof(FillDataTablesCommand), DataAdapter, "Fill");
            _doDisposeCommand = (DisposeCommand)Delegate.CreateDelegate(typeof(DisposeCommand), DataAdapter, "Dispose");

            _doFillDataSetCommand.ShouldNotBeNull("_doFillDataSetCommand");
            _doFillDataTablesCommand.ShouldNotBeNull("_doFillDataTablesCommand");
        }

        /// <summary>
        /// 실제 DbDataAdapter의 인스턴스
        /// </summary>
        public DataAdapter DataAdapter { get; private set; }

        /// <summary>
        /// <paramref name="dataReader"/>를 읽어서, <see cref="DataTable"/>을 빌드하여 <paramref name="dataSet"/>에 추가합니다.
        /// </summary>
        public int Fill(DataSet dataSet, string tableName, IDataReader dataReader, int firstResult = 0, int maxResults = 0) {
            dataSet.ShouldNotBeNull("dataSet");
            dataReader.ShouldNotBeNull("dataReader");

            return _doFillDataSetCommand(dataSet, tableName ?? "Table" + dataSet.Tables.Count + 1, dataReader, firstResult, maxResults);
        }

        /// <summary>
        /// <paramref name="dataReader"/>를 읽어서, <paramref name="dataTables"/> 에 결과 셋을 채웁니다.
        /// </summary>
        public int Fill(DataTable[] dataTables, IDataReader dataReader, int firstResult = 0, int maxResults = 0) {
            dataTables.ShouldNotBeEmpty("dataTables");
            dataReader.ShouldNotBeNull("dataReader");

            return _doFillDataTablesCommand(dataTables, dataReader, firstResult, maxResults);
        }

        public bool IsDisposed { get; private set; }

        /// <summary>
        /// 관리되지 않는 리소스의 확보, 해제 또는 다시 설정과 관련된 응용 프로그램 정의 작업을 수행합니다.
        /// </summary>
        public void Dispose() {
            if(IsDisposed)
                return;

            With.TryAction(() => _doDisposeCommand());

            if(IsDebugEnabled)
                log.Debug("AdoDataAdapter 인스턴스의 리소스를 해제했습니다.");

            IsDisposed = true;
        }

        //
        // NOTE: protected method를 사용하기 위해 delegate로 정의해둡니다. Delegate.CreateDelegate()
        //

        private delegate int FillDataSetCommand(
            DataSet dataSet, string srcTable, IDataReader dataReader, int startRecord, int maxRecords);

        private delegate int FillDataTablesCommand(DataTable[] dataTables, IDataReader dataReader, int startRecord, int maxRecords);

        private delegate void DisposeCommand();
    }
}