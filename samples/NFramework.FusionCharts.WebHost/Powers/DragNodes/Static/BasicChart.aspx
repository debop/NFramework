<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="BasicChart.aspx.cs" Inherits="NSoft.NFramework.FusionCharts.WebHost.Powers.DragNodes.BasicChart" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div>
        <rcl:FusionChart ID="dragNode" runat="server" FileName="DragNode.swf" ChartId="dragNode" DataUrl="BasicChart.xml" Width="600" Height="450" />
    </div>
</asp:Content>