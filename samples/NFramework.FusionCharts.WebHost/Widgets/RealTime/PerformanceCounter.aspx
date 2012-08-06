<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="PerformanceCounter.aspx.cs" Inherits="NSoft.NFramework.FusionCharts.WebHost.Widgets.RealTime.PerformanceCounter" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div>
        <center>
            <asp:RadioButtonList ID="ChartNameList" runat="server" AutoPostBack="true" Width="100%" RepeatLayout="Flow" RepeatDirection="Horizontal">
                <asp:ListItem Text="RealTime Line" Value="RealTimeLine" />
                <asp:ListItem Text="RealTime Column" Value="RealTimeColumn" />
                <asp:ListItem Text="RealTime Area" Value="RealTimeArea" />
            </asp:RadioButtonList>
            <h2>FusionCharts <%= ChartNameList.SelectedItem.Text %> Examples</h2>
            <rcl:FusionChart ID="staticChart" runat="server" FileName="RealTimeLine.swf" ChartId="column3DChart" DataUrl="PerformanceCounter.xml" Width="600" Height="400" />
        </center>
    </div>
</asp:Content>