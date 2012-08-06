<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="LEDGaugeSample.aspx.cs" Inherits="NSoft.NFramework.FusionCharts.WebHost.Widgets.Guages.LED.LEDGaugeSample" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table style="border: 1px dotted #aaaaaa; height: 100%; width: 100%;">
        <tr>
            <td colspan="2" align="center" valign="middle" style="border: 1px dotted #dedede; height: 43px;">
                <h2>
                    LED Gauge Graphs</h2>
            </td>
        </tr>
        <tr>
            <td colspan="2" align="center" valign="middle" style="border: 1px dotted #dedede; height: 43px;">
                <asp:UpdatePanel ID="FactorySelector" runat="server">
                    <ContentTemplate>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
        </tr>
        <tr style="height: 100%; width: 100%;">
            <td align="center" valign="top" style="border: 1px dotted #dedede;">
                <asp:UpdatePanel ID="FusionChartsUP" runat="server">
                    <ContentTemplate>
                        <asp:Panel ID="Panel1" runat="server">
                            <rcl:FusionChart ID="gaugeEdit" runat="server" FileName="HLED.swf" ChartId="editChart" DataUrl="LED.xml" Width="440" Height="120" />
                        </asp:Panel>
                    </ContentTemplate>
                </asp:UpdatePanel>
                <asp:UpdateProgress ID="UpdateProgress2" runat="server" DisplayAfter="50">
                    <ProgressTemplate>
                        <img id="Img2" src="~/images/loading.gif" runat="server" style="left: 564px; position: absolute; top: 327px; z-index: 5;" />
                    </ProgressTemplate>
                </asp:UpdateProgress>
            </td>
            <td align="center" valign="top" style="border: 1px dotted #dedede;">
                <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                    <ContentTemplate>
                        <asp:Panel ID="Panel2" runat="server">
                            <rcl:FusionChart ID="gaugeReal" runat="server" FileName="HLED.swf" ChartId="realTimeChart" DataUrl="LEDGaugeData.ashx" Width="440" Height="120" DebugMode="false" />
                        </asp:Panel>
                    </ContentTemplate>
                </asp:UpdatePanel>
                <asp:UpdateProgress ID="UpdateProgress1" runat="server" DisplayAfter="50">
                    <ProgressTemplate>
                        <img id="Img2" src="~/images/loading.gif" runat="server" style="left: 564px; position: absolute; top: 327px; z-index: 5;" />
                    </ProgressTemplate>
                </asp:UpdateProgress>
            </td>
        </tr>
        <tr style="height: 100%; width: 100%;">
            <td align="center" valign="top" style="border: 1px dotted #dedede;">
                <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                    <ContentTemplate>
                        <asp:Panel ID="Panel3" runat="server">
                            <rcl:FusionChart ID="FusionChart1" runat="server" FileName="VLED.swf" ChartId="editChart" DataUrl="LED.xml" Width="140" Height="440" />
                        </asp:Panel>
                    </ContentTemplate>
                </asp:UpdatePanel>
                <asp:UpdateProgress ID="UpdateProgress3" runat="server" DisplayAfter="50">
                    <ProgressTemplate>
                        <img id="Img2" src="~/images/loading.gif" runat="server" style="left: 564px; position: absolute; top: 327px; z-index: 5;" />
                    </ProgressTemplate>
                </asp:UpdateProgress>
            </td>
            <td align="center" valign="top" style="border: 1px dotted #dedede;">
                <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                    <ContentTemplate>
                        <asp:Panel ID="Panel4" runat="server">
                            <rcl:FusionChart ID="FusionChart2" runat="server" FileName="VLED.swf" ChartId="realTimeChart" DataUrl="LEDGaugeData.ashx" Width="140" Height="440" DebugMode="false" />
                        </asp:Panel>
                    </ContentTemplate>
                </asp:UpdatePanel>
                <asp:UpdateProgress ID="UpdateProgress4" runat="server" DisplayAfter="50">
                    <ProgressTemplate>
                        <img id="Img2" src="~/images/loading.gif" runat="server" style="left: 564px; position: absolute; top: 327px; z-index: 5;" />
                    </ProgressTemplate>
                </asp:UpdateProgress>
            </td>
        </tr>
    </table>
</asp:Content>