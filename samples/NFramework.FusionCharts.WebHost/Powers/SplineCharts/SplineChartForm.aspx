<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="SplineChartForm.aspx.cs" Inherits="NSoft.NFramework.FusionCharts.WebHost.Powers.SplineCharts.SplineChartForm" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table style="border: 1px dotted #aaaaaa; height: 100%; width: 100%;">
        <tr>
            <td align="center" valign="middle" style="border: 1px dotted #dedede; height: 43px;">
                <h2>
                    Spline Chart</h2>
            </td>
        </tr>
        <tr>
            <td align="center" valign="middle" style="border: 1px dotted #dedede; height: 43px;">
                <asp:UpdatePanel ID="FactorySelector" runat="server">
                    <ContentTemplate>
                        <asp:RadioButtonList ID="ChartNameList" runat="server" AutoPostBack="true" Width="100%" RepeatLayout="Flow" RepeatDirection="Horizontal">
                            <asp:ListItem Text="Spline" Value="Spline" Selected="true" />
                            <asp:ListItem Text="Spline Area" Value="SplineArea" />
                            <asp:ListItem Text="Multi-Series Spline" Value="MSSpline" />
                            <asp:ListItem Text="Multi-Series Spline Area" Value="MSSplineArea" />
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
                            <rcl:fusionchart id="chartSpline" runat="server" filename="Spline.swf" chartid="chartSpline" dataurl="SingleSeriesData.ashx" width="600" height="300" debugmode="false" />
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