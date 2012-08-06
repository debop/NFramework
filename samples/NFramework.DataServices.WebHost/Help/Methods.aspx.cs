using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NSoft.NFramework.Data;
using NSoft.NFramework.InversionOfControl;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.DataServices.WebHost.Help {
    public partial class Methods : System.Web.UI.Page {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private string _dbName;
        private IAdoRepository _repository;

        protected void Page_Load(object sender, EventArgs e) {
            lblError.Text = string.Empty;

            if(IsPostBack)
                return;
            try {
                InitializeComponents();

                Task.Factory.StartNew(BindDataSource).Wait();
            }
            catch(Exception ex) {
                if(log.IsErrorEnabled) {
                    log.Error("예외가 발생했습니다.");
                    log.Error(ex);
                }

                lblError.Text = "Error: " + ex.Message;
            }
        }

        private void InitializeComponents() {
            _dbName = Request[HttpParams.Product].AsText(AdoTool.DefaultDatabaseName);

            var repositoryId = @"AdoRepository";

            if(_dbName.IsNotWhiteSpace())
                repositoryId = string.Concat(repositoryId, ".", _dbName);

            if(IsDebugEnabled)
                log.Debug(@"기본 AdoRepository를 Resolve합니다. repositoryId=[{0}]", repositoryId);

            _repository = IoC.Resolve<IAdoRepository>(repositoryId);
        }

        private void BindDataSource() {
            var queryTable = new SortedDictionary<string, string>(_repository.QueryProvider.GetQueries());

            lblProductName.Text = _dbName;
            rptMethods.DataSource = queryTable;
            rptMethods.DataBind();
        }
    }
}