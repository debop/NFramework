<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="CandleStickForm.aspx.cs" Inherits="NSoft.NFramework.FusionCharts.WebHost.Powers.CandleStocks.CandleStickForm" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table style="border: 1px dotted #aaaaaa; height: 100%; width: 100%;">
        <tr>
            <td align="center" valign="middle" style="border: 1px dotted #dedede; height: 43px;">
                <h2>
                    CandleStick Chart Samples</h2>
            </td>
        </tr>
        <tr>
            <td align="center" valign="middle" style="border: 1px dotted #dedede; height: 43px;">
                <asp:UpdatePanel ID="FactorySelector" runat="server">
                    <ContentTemplate>
                        <asp:RadioButtonList ID="dataOptions" runat="server" AutoPostBack="true" Width="100%" RepeatLayout="Flow" RepeatDirection="Horizontal">
                            <asp:ListItem Text="Basic Chart" Value="Basic" Selected="true" />
                            <asp:ListItem Text="Without Volume Chart" Value="WithoutVolume" />
                            <asp:ListItem Text="Line Plot" Value="LinePlot" />
                            <asp:ListItem Text="Bar Plot" Value="BarPlot" />
                            <asp:ListItem Text="With Missing Data" Value="MissingData" />
                            <asp:ListItem Text="Highlighting Data" Value="Highlighting" />
                            <asp:ListItem Text="With Trends & Indicators" Value="WithTrends" />
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
                            <rcl:FusionChart ID="candleStick" runat="server" FileName="CandleStick.swf" ChartId="candleStick" DataUrl="CandleStickData.ashx" Width="760" Height="500" DebugMode="false" />
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