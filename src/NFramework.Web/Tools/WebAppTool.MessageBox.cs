using System.Drawing;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NSoft.NFramework.Tools;
using RealWeb.Portal.Controls;

namespace NSoft.NFramework.Web.Tools
{
    public static partial class WebAppTool
    {
        /// <summary>
        /// 메시지를 <c>ITextControl</c>에 출력한다.
        /// </summary>
        /// <param name="control"><c>ITextControl</c></param>
        /// <param name="message">메시지</param>
        /// <param name="foreColor">색상</param>
        public static void DisplayMessage(this ITextControl control, string message, Color foreColor)
        {
            control.Text = message;

            if(control is WebControl)
            {
                WebControl webctrl = (WebControl)control;
                webctrl.ForeColor = foreColor;
                webctrl.Visible = true;
            }
        }

        /// <summary>
        /// 메시지를 <c>ITextControl</c>에 출력한다.
        /// </summary>
        /// <param name="control"><c>ITextControl</c></param>
        /// <param name="message">메시지</param>
        /// <param name="error">오류메시지여부</param>
        public static void DisplayMessage(this ITextControl control, string message, bool error = false)
        {
            DisplayMessage(control, message, error ? Color.Red : Color.Blue);
        }

        #region << MessageBox Methods >>

        /// <summary>
        /// 메시지 출력
        /// </summary>
        /// <param name="text">메시지</param>
        /// <param name="returnUrl">이동할 url</param>
        public static void MessageBox(string text, string returnUrl = "")
        {
            MessageBox(text, null, returnUrl);
        }

        /// <summary>
        /// 메시지 출력
        /// </summary>
        /// <param name="text"></param>
        /// <param name="messageType"></param>
        /// <param name="messageButton"></param>
        public static void MessageBox(string text, MessageType messageType, MessageButtons messageButton)
        {
            MessageBox(MessageBoxDisplayKind.JScriptAlert, string.Empty, text, messageType, messageButton);
        }

        /// <summary>
        /// 메시지 출력
        /// page 가 null 이 아니면 RegisterStartupScript 로 스크립트 추가되어지므로 호출후 Response.End()등으로 실행중지시 출력되어지지 않습니다.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="page"></param>
        public static void MessageBox(string text, Page page)
        {
            MessageBox(MessageBoxDisplayKind.JScriptAlert, string.Empty, text, MessageType.Normal, MessageButtons.Ok, page);
        }

        /// <summary>
        /// 메시지 출력
        /// ctl 가 null 이 아니면 RegisterStartupScript 로 스크립트 추가되어지므로 호출후 Response.End()등으로 실행중지시 출력되어지지 않습니다.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="ctl"></param>
        public static void MessageBox(string text, System.Web.UI.Control ctl)
        {
            MessageBox(MessageBoxDisplayKind.JScriptAlert, string.Empty, text, MessageType.Normal, MessageButtons.Ok, ctl);
        }

        /// <summary>
        /// 메시지 출력
        /// </summary>
        /// <param name="text">메시지</param>
        /// <param name="page">출력할 페이지</param>
        /// <param name="returnUrl">이동할 url</param>
        public static void MessageBox(string text, Page page, string returnUrl)
        {
            MessageBox(MessageBoxDisplayKind.JScriptAlert, string.Empty, text, page: page, returnUrl: returnUrl);
        }

        /// <summary>
        /// 메시지 출력
        /// page 가 null 이 아니면 RegisterStartupScript 로 스크립트 추가되어지므로 호출후 Response.End()등으로 실행중지시 출력되어지지 않습니다.
        /// </summary>
        /// <param name="messageBoxDisplayKind">출력타입</param>
        /// <param name="title">메시지 캡션</param>
        /// <param name="text">메시지</param>
        /// <param name="messageType">메시지타입</param>
        /// <param name="messageButton">버튼타입</param>
        /// <param name="page">렌더링되는 페이지</param>
        /// <param name="returnUrl">메시지출력후 이동할 url</param>
        /// <param name="endResponse">프로세스 종료여부</param>
        public static void MessageBox(MessageBoxDisplayKind messageBoxDisplayKind,
                                      string title,
                                      string text,
                                      MessageType messageType = MessageType.Normal,
                                      MessageButtons messageButton = MessageButtons.Ok,
                                      System.Web.UI.Page page = null,
                                      string returnUrl = "",
                                      bool endResponse = true)
        {
            switch(messageBoxDisplayKind)
            {
                case MessageBoxDisplayKind.Page:

                    var httpResponse = HttpContext.Current.Response;
                    var param = string.Format("Title={0}&Content={1}&MessageType={2}&MessageButton={3}&ReturnUrl={4}",
                                              title.UrlEncode(), text.UrlEncode(), (int)messageType, (int)messageButton, returnUrl.UrlEncode());
                    var url = UrlParamConcat(AppSettings.MessageBoxUrl, param);
                    httpResponse.Redirect(url, endResponse);
                    return;

                default:

                    #region

                    StringBuilder buffer = new StringBuilder();
                    try
                    {
                        buffer.Append(@"alert(");
                        buffer.Append(AntiXssTool.JavaScriptEncode((title.IsNotWhiteSpace() ? "[" + title + "]\n\n" : string.Empty) + text));
                        buffer.Append(@");");

                        // Alert이고 닫기 타입이 Close 이면 창을 닫는다.
                        if(messageButton == MessageButtons.Close)
                        {
                            buffer.Append(SR.CloseWindowJavascript);
                        }
                        else if(returnUrl.IsNotWhiteSpace())
                        {
                            buffer.AppendFormat("window.location.href={0};", AntiXssTool.JavaScriptEncode(returnUrl));
                        }

                        if(page != null)
                        {
                            //이미 추가되었다면
                            if(page.ClientScript.IsStartupScriptRegistered(page.GetType(), page + "_MessageBox"))
                                ScriptManager.RegisterStartupScript(page, page.GetType(), page + "_MessageBox", buffer.ToString(), true);
                            else
                                ScriptManager.RegisterStartupScript(page, page.GetType(), page + "_MessageBox", buffer.ToString(), true);
                        }
                        else
                        {
                            HttpContext.Current.Response.Output.Write(WrapScriptTag(buffer.ToString()));
                        }
                    }
                    finally
                    {
                        buffer = null;
                    }

                    #endregion

                    break;
            }
        }

        /// <summary>
        /// 메시지 출력
        /// ctl 가 null 이 아니면 RegisterStartupScript 로 스크립트 추가되어지므로 호출후 Response.End()등으로 실행중지시 출력되어지지 않습니다.
        /// </summary>
        /// <param name="messageBoxDisplayKind">출력타입</param>
        /// <param name="title">메시지 캡션</param>
        /// <param name="text">메시지</param>
        /// <param name="messageType">메시지타입</param>
        /// <param name="messageButton">버튼타입</param>
        /// <param name="ctl">렌더링되는 페이지</param>
        /// <param name="returnUrl">메시지출력후 이동할 url</param>
        /// <param name="endResponse">프로세스 종료여부</param>
        public static void MessageBox(MessageBoxDisplayKind messageBoxDisplayKind,
                                      string title,
                                      string text,
                                      MessageType messageType,
                                      MessageButtons messageButton,
                                      System.Web.UI.Control ctl,
                                      string returnUrl = "",
                                      bool endResponse = false)
        {
            MessageBox(messageBoxDisplayKind, title, text, messageType, messageButton, (ctl != null) ? ctl.Page : null, returnUrl, endResponse);
        }

        #endregion

        #region << MessageBox for Page >>

        public static void ShowMessageOfAccessDenied()
        {
            var title = GetGlobalResourceString(AppSettings.ResourceGlossary, "AccessFailed", "인증오류");
            var message = GetGlobalResourceString(AppSettings.ResourceMessages, "AccessFailed", "접근권한이 없습니다.");

            MessageBox(MessageBoxDisplayKind.Page, title, message, MessageType.Warning, MessageButtons.Ok, endResponse: false);
        }

        public static void ShowMessageOfLoginFail()
        {
            var title = GetGlobalResourceString(AppSettings.ResourceGlossary, "LoginFailed", "로그인 오류");
            var message = GetGlobalResourceString(AppSettings.ResourceMessages, "NotExistLoginInfo", "로그인 정보가 없습니다");

            MessageBox(MessageBoxDisplayKind.Page, title, message, MessageType.Warning, MessageButtons.Ok | MessageButtons.Login, endResponse: false);
        }

        #endregion
    }
}