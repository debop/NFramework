<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Menu.ascx.cs" Inherits="NSoft.NFramework.StringResources.WebHost.Common.UserControls.Menu" %>
<table class="menu" width="100%">
    <tr>
        <td class="hborder">
        </td>
    </tr>
    <tr>
        <td style="width: 100%">
            <!-- SiteMapPath control -->
            <asp:SiteMapPath CssClass="hmenu" ID="SiteMapPath2" runat="server" Font-Names="Verdana" Font-Size="0.8em" PathSeparator=" : ">
                <PathSeparatorTemplate>
                    <asp:Image ID="imgMenu" runat="server" ImageAlign="Middle" ImageUrl="~/images/arrow.gif" />
                </PathSeparatorTemplate>
                <PathSeparatorStyle Font-Bold="True" ForeColor="#5D7B9D" />
                <CurrentNodeStyle ForeColor="#333333" />
                <NodeStyle Font-Bold="True" ForeColor="#7C6F57" />
                <RootNodeStyle Font-Bold="True" ForeColor="#5D7B9D" />
            </asp:SiteMapPath>
        </td>
    </tr>
    <tr>
        <td class="hborder">
        </td>
    </tr>  
</table>