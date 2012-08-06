<%@ Page Title="No Compile" Language="C#" MasterPageFile="~/Site.Master" CompilationMode="Never" %>
<asp:Content ID="Content1" ContentPlaceHolderID="contentMain" runat="server">
    <h1>
        Localization Expressions (No Compile)</h1>
    <p>
        This page illustrates the use of localization expressions that draw local and global
        resources from the database using a custom resource provider on a page that is not
        compiled.
    </p>
    <p>
        Implicit Expression (local page resources):
        <asp:Label ID="lblHelloLocal" runat="server" Text="<%$ Resources:lblHelloLocalResource1.Text %>"></asp:Label>
        <br />
        Explicit Expression (local resources):
        <asp:Label ID="Label1" runat="server" Text="<%$ Resources:lblHelloLocalResource1.Text %>"
                   meta:localize="false"></asp:Label>
        <br />
        Explicit Expression (global resources):
        <asp:Label ID="lblHelloGlobal" runat="server" Text="<%$ RwResources:CommonTerms, Hello %>"
                   meta:localize="false"></asp:Label>
    </p>
</asp:Content>