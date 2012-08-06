<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Header.ascx.cs" Inherits="NSoft.NFramework.StringResources.WebHost.Common.UserControls.Header" %>
<table class="header">
    <tr>
        <td align="right" style="height: 20px">
            <asp:HyperLink ID="lnkSiteMap" NavigateUrl="~/" runat="server" Text="<%$ Resources:Glossary, SiteMap %>" />
            &nbsp;|&nbsp;
            <asp:HyperLink ID="lnkContact" NavigateUrl="~/" runat="server" Text="<%$ Resources:Glossary, HomePage %>" />
            &nbsp;|&nbsp;
            <asp:RadioButtonList ID="localeKeys" runat="server" AutoPostBack="True" RepeatDirection="Horizontal" OnSelectedIndexChanged="LocaleKey_Changed">
                <asp:ListItem Text="Korean" Value="ko-KR" />
                <asp:ListItem Text="English" Value="en-US" />
            </asp:RadioButtonList>
        </td>
    </tr>
    <tr>
        <td>
            <asp:Image ID="imgFlag" ImageUrl='<%$ Resources:Config, FlagImage %>' runat="server" />
            <h3>
                <asp:Localize ID="locWelcome" Text='<%$ Resources:Glossary, Welcome %>' runat="server" />
            </h3>
        </td>
    </tr>
</table>