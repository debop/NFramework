<%@ Page Title="사용자 함수 그래프" Language="C#"  Async="true" MasterPageFile="~/MasterPage.Master" CodeBehind="MathExpr.aspx.cs" Inherits="NSoft.NFramework.FusionCharts.WebHost.Charts.Ajax.MathExpr" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table style="border: 1px dotted #aaaaaa; height: 100%; width: 100%;">
        <tr>
            <td align="center" valign="middle" style="border: 1px dotted #dedede; height: 43px;">
                <h2>사용자 정의 함수 그래프</h2>
            </td>
        </tr>
        <tr>
            <td align="center" valign="middle" style="border: 1px dotted #dedede; height: 80px;">
                <asp:UpdatePanel ID="FactorySelector" runat="server">
                    <ContentTemplate>
                        함수 식<asp:TextBox ID="edExpression" runat="server" Text="sin(x)" Style="text-align: left;" Width="100%"></asp:TextBox><br/><br />
                        변량 Lower:<asp:TextBox ID="edLowerX" runat="server" Text="-100" Style="text-align: right;"></asp:TextBox>&nbsp;&nbsp;&nbsp;
                        변량 Upper:<asp:TextBox ID="edUpperX" runat="server" Text="100" Style="text-align: right;"></asp:TextBox>&nbsp;&nbsp;&nbsp;
                        <asp:Button ID="btnEvaluate" runat="server" Text="함수 그래프 그리기" OnClick="btnEvaluate_Click" />
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
        </tr>
        <tr style="height: 100%; width: 100%;">
            <td valign="top" style="border: 1px dotted #dedede; height: 100%; width: 100%;">
                <asp:UpdatePanel ID="FusionChartPanel" runat="server">
                    <ContentTemplate>
                        <asp:Panel ID="Panel1" runat="server" Height="100%" Width="100%">
                            <rcl:FusionChart ID="graphChart" runat="server" FileName="Line.swf" ChartId="graphChart" DataUrl="MathExprEvaluator.ashx" Width="100%" Height="500" DebugMode="false" />
                        </asp:Panel>
                    </ContentTemplate>
                </asp:UpdatePanel>
                <asp:UpdateProgress ID="UpdateProgress" runat="server" DisplayAfter="50">
                    <ProgressTemplate>
                        <img id="Img2" src="~/images/loading.gif" runat="server" style="left: 564px; position: absolute; top: 327px; z-index: 5;" />
                    </ProgressTemplate>
                </asp:UpdateProgress>
            </td>
        </tr>
    </table>
</asp:Content>