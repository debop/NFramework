<%@ Page Language="C#" AutoEventWireup="true" EnableViewState="false" EnableSessionState="False" CodeBehind="AspImageDownload.aspx.cs" Inherits="NSoft.NFramework.WebHost.Adapters.AspImageDownload" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>ControlAdapter 예제</title>
</head>
<body>
	<form id="form1" runat="server">
	<div>
		<asp:Label runat="server" Text="이미지 경로가 절대경로라면, 이미지 다운로드를 동시에 수행한다고 합니다." /><br />
		<asp:Image ImageUrl="~/Images/T72_0.jpg" width="800" height="441" runat="server"/><br />
		<asp:Image ImageUrl="~/Images/T72_1.jpg" width="800" height="533" runat="server"/><br />
		<asp:Image ImageUrl="~/Images/T72_2.jpg" width="700" height="466" runat="server"/><br />
		<asp:Image ImageUrl="~/Images/T72_3.jpg" width="800" height="533" runat="server"/><br />
		<asp:Image ImageUrl="~/Images/T72_4.jpg" width="800" height="533" runat="server"/><br />
		<asp:Image ImageUrl="~/Images/T72_5.jpg" width="800" height="533" runat="server"/><br />

		<hr />
<%--		<img src="http://localhost:3500/NFramework.Web/Images/T72_0.jpg" style="border-width:0px;" /><br />
		<img src="http://localhost:3500/NFramework.Web/Images/T72_1.jpg" style="border-width:0px;" /><br />
		<img src="http://localhost:3500/NFramework.Web/Images/T72_2.jpg" style="border-width:0px;" /><br />
		<img src="http://localhost:3500/NFramework.Web/Images/T72_3.jpg" style="border-width:0px;" /><br />
		<img src="http://localhost:3500/NFramework.Web/Images/T72_4.jpg" style="border-width:0px;" /><br />
		<img src="http://localhost:3500/NFramework.Web/Images/T72_5.jpg" style="border-width:0px;" /><br />--%>
		
	</div>
	</form>
</body>
</html>
