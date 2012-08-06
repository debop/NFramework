<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Footer.ascx.cs" Inherits="NSoft.NFramework.StringResources.WebHost.Common.UserControls.Footer" %>
<div id="footer" class="footer">
    <table class="frame">
        <tr>
            <td style="text-align: left">
                <asp:Localize ID="locCopyright" Text="Copyright (c)2007 리얼웹" runat="server" meta:localize="true" meta:resourcekey="locCopyrightResource1" />
            </td>
            <td style="text-align: right">
                <asp:HyperLink ID="lnkDesign" NavigateUrl="http://www.realweb21.com" runat="server" meta:localize="true" meta:resourcekey="lnkDesignResource1">리얼웹</asp:HyperLink>
                |
                <asp:HyperLink ID="lnkDasBlond" NavigateUrl="http://debop.egloos.com" runat="server" meta:localize="true" meta:resourcekey="lnkDasBlondResource1">디밥의 블로그</asp:HyperLink>
                |
                <asp:HyperLink ID="lnkBook" NavigateUrl="http://www.amazon.com" runat="server" meta:localize="true" meta:resourcekey="lnkBookResource1">아마존 서점</asp:HyperLink>
            </td>
        </tr>
    </table>
</div>