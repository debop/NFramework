<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="NSoft.NFramework.FusionCharts.WebHost.Widgets.Gantt.Default" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div style="text-align: center">
        <h2>
            FusionWidgets Gantt Chart Basic Examples</h2>
        <h4>
            Basic example using pre-built Data.xml</h4>
        <rcl:FusionChart ID="staticChart" runat="server" FileName="Gantt.swf" ChartId="gantt2" DataUrl="MultiTasks.xml" Width="800" Height="500" DebugMode="false" />
        <br />
        <h4>
            Basic example using DataXmlProvider</h4>
        <rcl:FusionChart ID="dynamicChart" runat="server" FileName="Gantt.swf" ChartId="gantt2" DataUrl="~/Widgets/Gantt/GanttDataProvider.ashx" Width="800" Height="500" DebugMode="true" />
    </div>
</asp:Content>