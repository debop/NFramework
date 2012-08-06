<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="StackedChart.aspx.cs" Inherits="NSoft.NFramework.FusionCharts.WebHost.Charts.Stacked.StackedChart" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div>
        <center>
            <asp:RadioButtonList ID="ChartNameList" runat="server" AutoPostBack="true" Width="100%" RepeatLayout="Flow" RepeatDirection="Horizontal">
                <asp:ListItem Text="Stacked Column 2D" Value="StackedColumn2D" />
                <asp:ListItem Text="Stacked Column 3D" Value="StackedColumn3D" />
                <asp:ListItem Text="Stacked Area" Value="StackedArea2D" />
                <asp:ListItem Text="Stacked Bar 2D" Value="StackedBar2D" />
                <asp:ListItem Text="Stacked Bar 3D" Value="StackedBar3D" />
            </asp:RadioButtonList>
            <h2>FusionCharts <%= ChartNameList.SelectedItem.Text %> Examples</h2>
            <h4>Using pre-built Data.xml</h4>
            <rcl:FusionChart ID="staticChart" runat="server" FileName="Line.swf" ChartId="column3DChart" DataUrl="Data.xml" Width="500" Height="300" />
            <h4>Using DataProviderHandler</h4>
            <rcl:FusionChart ID="dynamicChart" runat="server" FileName="Line.swf" ChartId="column3DChart" DataUrl="~/Charts/DataHandlers/MultiSeriesChartDataHandler.ashx?FactoryId=2" Width="500" Height="300" />
        </center>
    </div>
    <script type="text/javascript">
        function popup(msg) {
            window.alert(msg);
        }
    </script>
</asp:Content>