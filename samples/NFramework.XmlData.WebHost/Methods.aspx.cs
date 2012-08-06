using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NSoft.NFramework.Data;
using NSoft.NFramework.InversionOfControl;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.XmlData.WebHost {
    public partial class Methods : System.Web.UI.Page {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private string _dbName;
        private IAdoRepository _repository;

        protected void Page_Load(object sender, EventArgs e) {
            lblError.Text = string.Empty;
            try {
                InitializeComponents();

                if(!IsPostBack) {
                    Task.Factory.StartNew(BindDataSource, TaskCreationOptions.PreferFairness).Wait();
                    // BindDataSource();
                }
            }
            catch(Exception ex) {
                if(log.IsErrorEnabled)
                    log.ErrorException("예외가 발생했습니다.", ex);

                lblError.Text = "Error: " + ex.Message;
            }
        }

        private void InitializeComponents() {
            _dbName = Request[XmlDataServiceFacade.PARAM_PRODUCT].AsText(AdoTool.DefaultDatabaseName);

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