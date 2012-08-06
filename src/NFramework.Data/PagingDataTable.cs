using System;
using System.Data;

namespace NSoft.NFramework.Data {
    /// <summary>
    /// <see cref="DataTable"/>을 래핑하여, 전체 정보 중, 특정 Paging에 해당하는 정보만을 표현하는 DataTable입니다.
    /// </summary>
    public class PagingDataTable : IPagingInfo, IDisposable {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="table">DataTable 인스턴스</param>
        /// <param name="pageIndex">Page Index (0부터 시작)</param>
        /// <param name="pageSize">Page Size (보통 10)</param>
        /// <param name="totalItemCount">실제 DB에 있는 전체 Item의 수</param>
        public PagingDataTable(DataTable table, int pageIndex, int pageSize, int totalItemCount) {
            table.ShouldNotBeNull("table");
            pageIndex.ShouldBePositiveOrZero("pageIndex");

            _table = table;

            PageIndex = pageIndex;
            PageSize = (pageSize > 0) ? pageSize : totalItemCount;
            TotalItemCount = totalItemCount;
            TotalPageCount = (int)Math.Ceiling(totalItemCount / (double)PageSize);

            if(IsDebugEnabled)
                log.Debug("PagingDataTable이 생성되었습니다!!! " + ToString());
        }

        /// <summary>
        /// PagingDataTable을 DataTable로 변환하는 명시적 변환자
        /// </summary>
        /// <param name="pagingDataTable"></param>
        /// <returns></returns>
        public static explicit operator DataTable(PagingDataTable pagingDataTable) {
            return pagingDataTable.Table;
        }

        private DataTable _table;

        /// <summary>
        /// 실제 Data를 가진 DataTable 입니다.
        /// </summary>
        public DataTable Table {
            get { return _table; }
        }

        /// <summary>
        /// 현재 Page 번호 (0 부터 시작)
        /// </summary>
        public int PageIndex { get; private set; }

        /// <summary>
        /// Page 당 Item (레코드) 의 수 (예: 한 페이지에 10개의 Item이면 PageSize는 10이다.)
        /// </summary>
        public int PageSize { get; private set; }

        /// <summary>
        /// 전체 Page 수
        /// </summary>
        public int TotalPageCount { get; private set; }

        /// <summary>
        /// 전체 Item (레코드)의 갯수
        /// </summary>
        public int TotalItemCount { get; private set; }

        public override string ToString() {
            var tableName = (_table != null) ? _table.TableName : "NULL";

            return
                string.Format(
                    "PagingDataTable# Table=[{0}], PageIndex=[{1}], PageSize=[{2}], TotalPageCount=[{3}], TotalItemCount=[{4}]",
                    tableName, PageIndex, PageSize, TotalPageCount, TotalItemCount);
        }

        #region << IDisposable >>

        /// <summary>
        /// 리소스 해제 여부
        /// </summary>
        public bool IsDisposed { get; protected set; }

        /// <summary>
        /// 소멸자
        /// </summary>
        ~PagingDataTable() {
            Disposable(false);
        }

        /// <summary>
        /// 리소스 해제
        /// </summary>
        public void Dispose() {
            Disposable(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 리소스 해제
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Disposable(bool disposing) {
            if(IsDisposed)
                return;

            if(disposing) {
                if(_table != null)
                    With.TryAction(() => _table.Dispose(),
                                   null,
                                   () => _table = null);

                if(IsDebugEnabled)
                    log.Debug("PagingTable이 Dispose 되었습니다.");
            }

            IsDisposed = true;
        }

        #endregion
    }
}