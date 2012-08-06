using System;
using System.Windows.Forms;

namespace NSoft.NFramework.DataServices.WinForm {
    public partial class Form1 : Form {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        public new const string ProductName = "Northwind";

        public Form1() {
            InitializeComponent();
        }

        private void btnExecute_Click(object sender, EventArgs e) {
            var method = txtMethod.Text;
            if(method.IsWhiteSpace())
                return;

            lblResultCount.Text = string.Empty;

            Cursor = Cursors.WaitCursor;

            try {
                ExecuteMethod(method,
                              ProductName,
                              responseMsg => {
                                  if(responseMsg.HasError)
                                      MessageBox.Show(this, responseMsg.AggregateErrors.Select(x => x.Message).Join("\n"), "Error");

                                  if(responseMsg.Items.Count > 0) {
                                      var resultSet = responseMsg.Items[0].ResultSet;
                                      BindResultSet(resultSet, listView);

                                      lblResultCount.Text = resultSet.Count.AsText();
                                      if(IsDebugEnabled)
                                          log.Debug("정보를 조회했습니다. 레코드 수=[{0}]", responseMsg.Items[0].ResultSet.Count);
                                  }
                              });
            }
            catch(Exception ex) {
                MessageBox.Show(ex.Message);
            }

            Cursor = Cursors.Arrow;
        }

        public static void ExecuteMethod(string method, string productName, Action<ResponseMessage> callback) {
            ResponseMessage responseMsg = null;

            if(IsDebugEnabled)
                log.Debug("정보를 조회합니다... method=[{0}]", method);


            try {
                var requestMsg = new RequestMessage();
                requestMsg.AddItem(method, ResponseFormatKind.ResultSet);

                using(var dataService = new NSoft.NFramework.DataServices.Clients.WebDataService.DataService()) {
                    responseMsg = ClientTool.Execute(dataService, requestMsg, ProductName);
                }
            }
            catch(Exception ex) {
                if(log.IsErrorEnabled)
                    log.ErrorException("서버에 DataService 요청을 처리하는데 예외가 발생했습니다.", ex);
                throw;
            }
            if(callback != null)
                callback(responseMsg);
        }

        public static void BindResultSet(ResultSet resultSet, ListView listView) {
            listView.BeginUpdate();
            try {
                listView.Clear();

                listView.Columns.Add("No");

                foreach(var name in resultSet.FieldNames)
                    listView.Columns.Add(name);

                var i = 0;
                foreach(var row in resultSet) {
                    var item = new ListViewItem(i.ToString());
                    foreach(var name in resultSet.FieldNames)
                        item.SubItems.Add(row[name].AsText());

                    listView.Items.Add(item);
                    i++;
                }
            }
            finally {
                listView.EndUpdate();
            }
        }
    }
}