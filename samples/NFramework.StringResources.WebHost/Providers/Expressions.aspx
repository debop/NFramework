<%@ Page Title="Expressions" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Expressions.aspx.cs" Inherits="NSoft.NFramework.StringResources.WebHost.Providers.Expressions" %>
<asp:Content ID="Content1" ContentPlaceHolderID="contentMain" runat="server">
    <h1>Localization Expressions</h1>
    <p>
        This page illustrates the use of localization expressions that draw local and global
        resources from the database using a custom resource provider.
    </p>
    <p>
        <!-- DbProvider인 경우 local resource도 DB에서 가져온다 -->
        Implicit Expression (local page resources):
        <asp:Label ID="lblHelloLocal" runat="server" Text="<%$ Resources:lblHelloLocalResource1.Text %>" />
        <br />
        Explicit Expression (local resources):
        <asp:Label ID="Label1" runat="server" Text="<%$ Resources:lblHelloLocalResource1.Text %>" ></asp:Label>
        <br />
        Explicit Expression (global resources):
        <asp:Label ID="lblHelloGlobal" runat="server" Text="<%$ RwResources:CommonTerms, Hello %>" meta:localize="false"></asp:Label>
    </p>
</asp:Content>