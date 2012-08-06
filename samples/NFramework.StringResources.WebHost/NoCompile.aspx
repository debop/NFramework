<%@ Page Title="No Compile" CompilationMode="Never" Language="C#" MasterPageFile="~/Site.Master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="contentMain" runat="server">
    <h1>
        Localization Expression (No Compile)</h1>
    <p>
        컴파일없이 리소스 정보를 읽어와서, 화면에 뿌려줍니다.
        <br />
        CompilationMode="Never" 이면 behind code 나 server script는 사용하지 못하지만, ExpressionBuilder를
        이용한 작업은 가능합니다. 즉 다국어용 정적 페이지는 이방식을 사용하면 됩니다.
    </p>
    <p>
        Implicit Expression (local page resources):
        <asp:Label ID="lblHelloLocal" runat="server" meta:resourcekey="lblHelloLocalResource1"
                   Text="안녕하세요 (기본)"></asp:Label>
        <br />
        Explicit Expression (local page resources):
        <asp:Label ID="lblHelloLocal2" runat="server" Text="<%$ Resources:lblHelloLocalResource1.Text %>">
        </asp:Label>
        <br />
        Explicit Expression (global resources):
        <asp:Label ID="lblHelloGlobal" runat="server" Text="<%$ Resources:CommonTerms, Hello %>">
        </asp:Label>
    </p>
</asp:Content>