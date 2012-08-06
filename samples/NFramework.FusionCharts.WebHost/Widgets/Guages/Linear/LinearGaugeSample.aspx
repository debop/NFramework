<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="LinearGaugeSample.aspx.cs" Inherits="NSoft.NFramework.FusionCharts.WebHost.Widgets.Guages.Linear.LinearGaugeSample" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <!-- FusionCharts.js 는 rcl:Chart 에서 참조하게 하므로 불필요하다. -->
    <%--<script type="text/javascript" src="FusionResourceFile.axd?File=FusionCharts.js"></script>--%>

    <script language="javascript">
        function FC_ChartUpdated(DomId) {
            if (DomId == "editChart") {
                var chartRef = getChartFromId(DomId);
                var pointerValue = chartRef.getData(1);
                var divToUpdate = document.getElementById("contentDiv");
                divToUpdate.innerHTML = "<span>Your satisfaction index: <b>" + Math.round(pointerValue) + "%</b></span>";
            }
        }
    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table style="border: 1px dotted #aaaaaa; height: 100%; width: 100%;">
        <tr>
            <td colspan="2" align="center" valign="middle" style="border: 1px dotted #dedede; height: 43px;">
                <h2>
                    Horizontal Linear Gauges</h2>
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
                            <rcl:FusionChart ID="gaugeEdit" runat="server" FileName="HLinearGauge.swf" ChartId="editChart" DataUrl="LinearGauge.xml" Width="350" Height="120" />
                            <div id="contentDiv">
                                <span>Please drag the pointer above to indicate your satisfaction index.</span>
                            </div>
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
                            <rcl:FusionChart ID="gaugeReal" runat="server" FileName="HLinearGauge.swf" ChartId="realTimeChart" DataUrl="LinearGaugeData.ashx" Width="350" Height="120" />
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
    </table>
</asp:Content>