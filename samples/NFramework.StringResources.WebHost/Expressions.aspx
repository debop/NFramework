<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Expressions.aspx.cs" Inherits="NSoft.NFramework.StringResources.WebHost.Expressions" meta:resourcekey="PageResource1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="contentMain" runat="server">
    <h1>
        <asp:Localize ID="locTitle" runat="server" meta:resourcekey="locTitleResource1" Text="
    지역화 Expression 예제
        "></asp:Localize>
    </h1>
    <p>
        <asp:Localize ID="locContent" runat="server" meta:resourcekey="locContentResource1">
            사용자 리소스 프로바이더를 이용하여 데이터베이스로부터 광역 및 지역 리소스 정보를 가져와서 사용할 수 있도록 한다.
        </asp:Localize>
    </p>
    <p>
        Implicit Expression (local page resources):
        <asp:Label ID="lblHelloLocal" runat="server" meta:resourcekey="lblHelloLocalResource2" Text="<%$ Resources:lblHelloLocalResource1.Text %>"></asp:Label>
        <br />
        Explicit Expression (local page resources):
        <asp:Label ID="lblHelloLocal2" runat="server" meta:resourcekey="lblHelloLocal2Resource1" Text="<%$ Resources:lblHelloLocalResource1.Text %>"></asp:Label>
        <br />
        Explicit Expression (global resources):
        <asp:Label ID="lblHelloGlobal" runat="server" meta:resourcekey="lblHelloGlobalResource1" Text="<%$ Resources:CommonTerms, Hello %>"></asp:Label>
        <br />
        Explicit Expression (custom expression, external resources):
        <asp:Label ID="Label1" runat="server" meta:localize="false" Text="<%$ Resources:CommonTerms, Hello %>"></asp:Label>
    </p>
</asp:Content>