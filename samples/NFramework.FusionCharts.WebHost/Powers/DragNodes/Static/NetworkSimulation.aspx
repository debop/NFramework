<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="NetworkSimulation.aspx.cs" Inherits="NSoft.NFramework.FusionCharts.WebHost.Powers.DragNodes.NetworkSimulation" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div>
        <rcl:FusionChart ID="dragNode" runat="server" FileName="DragNode.swf" ChartId="dragNode" DataUrl="NetworkSimulation.xml" Width="700" Height="600" />
    </div>
</asp:Content>