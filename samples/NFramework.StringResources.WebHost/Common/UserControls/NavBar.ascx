<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="NavBar.ascx.cs" Inherits="NSoft.NFramework.StringResources.WebHost.Common.UserControls.NavBar" %>
<table style="width: 100%" cellpadding="0" cellspacing="0">
    <tr>
        <td style="width: 100%">
            <asp:SiteMapDataSource ID="SiteMapDataSource1" runat="server" />
            <asp:TreeView ID="TreeView1" DataSourceID="SiteMapDataSource1" runat="server" ExpandDepth="2" ImageSet="Simple" EnableTheming="true">
                <LevelStyles>
                    <asp:TreeNodeStyle Font-Bold="True" Font-Underline="False" />
                </LevelStyles>
                <ParentNodeStyle Font-Bold="False" />
                <HoverNodeStyle Font-Underline="True" ForeColor="#5555DD" />
                <SelectedNodeStyle
                    Font-Underline="True" HorizontalPadding="0px" VerticalPadding="0px" ForeColor="#5555DD" />
                <NodeStyle Font-Names="Tahoma" Font-Size="10pt" ForeColor="Black" HorizontalPadding="0px"
                           NodeSpacing="0px" VerticalPadding="0px" />
            </asp:TreeView>
        </td>
    </tr>
    <tr style="height: 6px">
        <td style="width: 100%">
        </td>
    </tr>
    <tr>
        <td class="hborder" style="height: 19px">
        </td>
    </tr>
</table>