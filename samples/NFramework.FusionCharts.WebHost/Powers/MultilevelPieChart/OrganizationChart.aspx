<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="OrganizationChart.aspx.cs" Inherits="NSoft.NFramework.FusionCharts.WebHost.Powers.MultilevelPieChart.OrganizationChart" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table style="border: 1px dotted #aaaaaa; height: 100%; width: 100%;">
        <tr>
            <td align="center" valign="middle" style="border: 1px dotted #dedede; height: 43px;">
                <h2>
                    Multi-level Pie Chart</h2>
            </td>
        </tr>
        <tr>
            <td align="center" valign="middle" style="border: 1px dotted #dedede; height: 43px;">
                <asp:UpdatePanel ID="SampleSelector" runat="server">
                    <ContentTemplate>
                        <asp:RadioButtonList ID="SampleList" runat="server" AutoPostBack="true" Width="100%" RepeatLayout="Flow" RepeatDirection="Horizontal">
                            <asp:ListItem Text="Organizations" Value="Organizations" Selected="true" />
                            <asp:ListItem Text="Keywords" Value="Keywords" />
                            <asp:ListItem Text="Friends Grouping" Value="Friends" />
                        </asp:RadioButtonList>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
        </tr>
        <tr>
            <td align="center" valign="top" style="border: 1px dotted #dedede;">
                <asp:UpdatePanel ID="FusionChartsUP" runat="server">
                    <ContentTemplate>
                        <asp:Panel ID="Panel1" runat="server" Height="100%" Width="100%">
                            <rcl:FusionChart ID="multiPieChart" runat="server" FileName="MultiLevelPie.swf" ChartId="multiPieChart" DataUrl="MultilevelPieChartDataProvider.ashx" Width="600" Height="600" DebugMode="false" />
                        </asp:Panel>
                    </ContentTemplate>
                </asp:UpdatePanel>
                <asp:UpdateProgress ID="UpdateProgress2" runat="server" DisplayAfter="50">
                    <ProgressTemplate>
                        <img id="Img2" src="~/images/loading.gif" runat="server" style="left: 564px; position: absolute; top: 327px; z-index: 5;" />
                    </ProgressTemplate>
                </asp:UpdateProgress>
            </td>
        </tr>
    </table>
</asp:Content>