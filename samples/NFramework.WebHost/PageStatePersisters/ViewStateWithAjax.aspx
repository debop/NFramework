<%@ Page Title="" Language="C#" MasterPageFile="~/PageStatePersisters/ViewState.Master" AutoEventWireup="true" CodeBehind="ViewStateWithAjax.aspx.cs" Inherits="NSoft.NFramework.WebHost.PageStatePersisters.ViewStateWithAjax" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
	<asp:UpdatePanel ID="viewStateUpdatePanel" runat="server" UpdateMode="Always">
		<ContentTemplate>
			<asp:Label ID="lblCurrentTime" runat="server" Text="현재시간" /><br />
			<asp:Label ID="lblLargeStr" runat="server" Text="" /><br />
			
			<asp:Button ID="btnSubmit" runat="server" Text="Submit" OnClick="btnSubmit_Click" />
		</ContentTemplate>
	</asp:UpdatePanel>
	
</asp:Content>
