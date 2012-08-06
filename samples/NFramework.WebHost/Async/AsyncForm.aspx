<%@ Page Language="C#" ValidateRequest="false" EnableViewState="true" AutoEventWireup="true" CodeBehind="AsyncForm.aspx.cs" Inherits="NSoft.NFramework.WebHost.Async.AsyncForm" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>비동기 웹 폼 예제</title>
</head>
<body>
	<form id="form1" runat="server">
	<asp:Button ID="btnReload" runat="server" Text="Reload All" Width="125px" OnClick="btnReload_Click" /><br />
	<asp:Label ID="Label1" runat="server">Label1</asp:Label><br />
	<asp:Label ID="Label2" runat="server">Label2</asp:Label><br />
	<hr />
	<asp:TextBox ID="ResultTextBox1" runat="server" ReadOnly="true" TextMode="MultiLine"  Rows="25" Columns="100" />
	<asp:TextBox ID="ResultTextBox2" runat="server" ReadOnly="true" TextMode="MultiLine" Rows="25" Columns="100" />
	
	</form>
</body>
</html>
