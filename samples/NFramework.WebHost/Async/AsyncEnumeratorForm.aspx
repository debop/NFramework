<%@ Page Async="true" Language="C#" AutoEventWireup="true" CodeBehind="AsyncEnumeratorForm.aspx.cs" Inherits="NSoft.NFramework.WebHost.Async.AsyncEnumeratorForm" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>AsyncEnumerator in Wintellect Power Threading Library</title>
</head>
<body>
	<form id="form1" runat="server">
	<div>
		<table style="width: 371px">
			<tr>
				<td style="width: 167px">
					<asp:Label ID="Label1" runat="server" Text="Server willing-to-wait time:"></asp:Label>
				</td>
				<td style="width: 182px">
					<asp:TextBox ID="m_txtServerTime" runat="server" Width="41px">2000</asp:TextBox>
				</td>
			</tr>
			<tr>
				<td style="width: 167px">
					<asp:CheckBox ID="m_chkSimFailure" runat="server" Text="Simulate request failure" />
				</td>
				<td style="width: 182px">
					<asp:Button ID="m_btnAPMGetImages" runat="server" Text="APM Get Images" Width="125px" OnClick="m_btnAPMGetImages_Click" />
				</td>
			</tr>
		</table>
		<br />
		<asp:Label ID="m_lblPic1" runat="server" Text="(Image 1 size goes here)"></asp:Label>
		<br />
		<asp:Label ID="m_lblPic2" runat="server" Text="(Image 2 size goes here)"></asp:Label>
		&nbsp;</div>
	</form>
</body>
</html>
