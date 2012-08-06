<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="MultiSeriesChart.aspx.cs" Inherits="NSoft.NFramework.FusionCharts.WebHost.Charts.MultiSeries.MultiSeriesChart" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div>
        <center>
            <asp:RadioButtonList ID="ChartNameList" runat="server" AutoPostBack="true" Width="100%" RepeatLayout="Flow" RepeatDirection="Horizontal">
                <asp:ListItem Text="Multi-series Column 2D" Value="MSColumn2D" />
                <asp:ListItem Text="Multi-series Column 3D" Value="MSColumn3D" />
                <asp:ListItem Text="Multi-series Line" Value="MSLine" />
                <asp:ListItem Text="Multi-series Area" Value="MSArea" />
                <asp:ListItem Text="Multi-series Bar 2D" Value="MSBar2D" />
                <asp:ListItem Text="Multi-series Bar 3D" Value="MSBar3D" />
            </asp:RadioButtonList>
            <h2>
                FusionCharts
                <%= ChartNameList.SelectedItem.Text %>
                Examples</h2>
            <h4>
                Using pre-built Data.xml</h4>
            <rcl:FusionChart ID="staticChart" runat="server" FileName="Line.swf" ChartId="column3DChart" DataUrl="Data.xml" Width="500" Height="300" />
            <h4>
                Using DataProviderHandler</h4>
            <rcl:FusionChart ID="dynamicChart" runat="server" FileName="Line.swf" ChartId="column3DChart" DataUrl="~/Charts/DataHandlers/MultiSeriesChartDataHandler.ashx?FactoryId=2" Width="500" Height="300" />
        </center>
    </div>
    <script type="text/javascript">
        function PopUp(msg) {
            window.alert(msg);
        }
    </script>
</asp:Content>