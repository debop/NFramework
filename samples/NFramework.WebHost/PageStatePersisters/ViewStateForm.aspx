<%@ Page Language="C#" AutoEventWireup="true" EnableViewState="true" CodeBehind="ViewStateForm.aspx.cs" Inherits="NSoft.NFramework.WebHost.PageStatePersisters.ViewStateForm" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
		<asp:Label ID="lblCurrentTime" runat="server" Text="현재시간"/><br />
		<asp:Label ID="lblLargeStr" runat="server" Text="" /><br />
		<asp:Button ID="btnSubmit" runat="server" Text="Submit" OnClick="btnSubmit_Click" />
    </div>
    </form>
</body>
</html>
