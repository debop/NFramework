<%@ Page Language="C#" AutoEventWireup="true"  CodeBehind="HostingEnvironments.aspx.cs" Inherits="NSoft.NFramework.WebHost.Enviroments.HostingEnvironments" %>
<%@ OutputCache Duration="60" VaryByParam="*" VaryByControl="*" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>Hosting Environment 정보</title>
</head>
<body>
	<form id="form1" runat="server">
	<div>
		<asp:Label ID="txtHostingEnvironment" runat="server" EnableViewState="true" />
	</div>
	</form>
</body>
</html>
