<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="RadarChartForm.aspx.cs" Inherits="NSoft.NFramework.FusionCharts.WebHost.Powers.Radar.RadarChartForm" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table style="border: 1px dotted #aaaaaa; height: 100%; width: 100%;">
        <tr>
            <td align="center" valign="middle" style="border: 1px dotted #dedede; height: 43px;">
                <h2>
                    Radar Chart Samples</h2>
            </td>
        </tr>
        <tr>
            <td align="center" valign="middle" style="border: 1px dotted #dedede; height: 43px;">
                <asp:UpdatePanel ID="FactorySelector" runat="server">
                    <ContentTemplate>
                        <asp:RadioButtonList ID="dataOptions" runat="server" AutoPostBack="true" Width="100%" RepeatLayout="Flow" RepeatDirection="Horizontal">
                            <asp:ListItem Text="Sample 1" Value="1" Selected="true" />
                            <asp:ListItem Text="Sample 2" Value="2" />
                            <asp:ListItem Text="Sample 3" Value="3" />
                        </asp:RadioButtonList>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
        </tr>
        <tr style="height: 100%; width: 100%;">
            <td align="center" valign="top" style="border: 1px dotted #dedede; height: 100%; width: 100%;">
                <asp:UpdatePanel ID="FusionChartsUP" runat="server">
                    <ContentTemplate>
                        <asp:Panel ID="Panel1" runat="server" Height="100%" Width="100%">
                            <rcl:FusionChart ID="radarChart" runat="server" FileName="Radar.swf" ChartId="radarChart" DataUrl="RadarChartDataProvider.ashx" Width="760" Height="500" DebugMode="false" />
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