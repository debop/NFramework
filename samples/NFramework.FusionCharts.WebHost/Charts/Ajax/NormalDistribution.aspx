<%@ Page Title="정규분포도" Async="true" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="NormalDistribution.aspx.cs" Inherits="NSoft.NFramework.FusionCharts.WebHost.Charts.Ajax.NormalDistribution" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table style="border: 1px dotted #aaaaaa; height: 100%; width: 100%;">
        <tr>
            <td align="center" valign="middle" style="border: 1px dotted #dedede; height: 43px;">
                <h2>
                    정규분포도</h2>
            </td>
        </tr>
        <tr>
            <td align="center" valign="middle" style="border: 1px dotted #dedede; height: 43px;">
                <asp:UpdatePanel ID="FactorySelector" runat="server">
                    <ContentTemplate>
                        평균:
                        <asp:TextBox ID="edAvg" runat="server" Text="0" Style="text-align: right;"></asp:TextBox>&nbsp; 표준편차:
                        <asp:TextBox ID="edStDev" runat="server" Text="1" Style="text-align: right;"></asp:TextBox>&nbsp;
                        <asp:Button ID="btnCalc" runat="server" Text="정규분포 계산하기" OnClick="btnCalc_Click" />
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
        </tr>
        <tr style="height: 100%; width: 100%;">
            <td valign="top" style="border: 1px dotted #dedede; height: 100%; width: 100%;">
                <asp:UpdatePanel ID="FusionChartsUP" runat="server">
                    <ContentTemplate>
                        <asp:Panel ID="Panel1" runat="server" Height="100%" Width="100%">
                            <rcl:FusionChart ID="normalDistributionChart" runat="server" FileName="MSLine.swf" ChartId="normalDistributionChart" DataUrl="NormalDistributionDataHandler.ashx"
                                             Width="100%" Height="500" DebugMode="false" />
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