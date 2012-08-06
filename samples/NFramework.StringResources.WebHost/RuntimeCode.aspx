<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="RuntimeCode.aspx.cs" Inherits="NSoft.NFramework.StringResources.WebApp.RuntimeCode" %>
<asp:Content ID="Content1" ContentPlaceHolderID="contentMain" runat="server">
    <h1>
        Runtime Resource Access</h1>
    <p>
        이 페이지는 로칼, 글로발 리소스 정보를 얻기 위해 Page 객체의 Localization API 메소드를 이용하여 동적으로 리소스 값을 설정하는
        예를 보여줍니다. 리소스 정보는 사용자 리소스 제공자를 이용하여 데이타베이스로부터 얻습니다.
    </p>
    <p>
        GetLocalResourceObject():
        <!-- 동적 리소스를 얻기 위해 암시적으로 얻는 것을 금지한다.-->
        <asp:Label ID="lblHelloLocal" runat="server" meta:localize="false" Text="안녕하세요(기본)">
        </asp:Label>
        <br />
        GetGlobalResourceObject():
        <asp:Label ID="lblHelloGlobal" runat="server" meta:localize="false" Text="안녕하세요(기본)">
        </asp:Label>
    </p>
</asp:Content>