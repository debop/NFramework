<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="MultiSeriesStackedChart.aspx.cs" Inherits="NSoft.NFramework.FusionCharts.WebHost.Charts.Stacked.MultiSeriesStackedChart" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div>
        <center>
            <asp:RadioButtonList ID="ChartNameList" runat="server" AutoPostBack="true" Width="100%" RepeatLayout="Flow" RepeatDirection="Horizontal">
                <asp:ListItem Text="Multi-series Stacked Column 2D" Value="MSStackedColumn2D" />
                <asp:ListItem Text="Multi-series Stacked Column 2D Line Dual Y" Value="MSStackedColumn2DLineDY" />
            </asp:RadioButtonList>
            <h2>
                FusionCharts
                <%= ChartNameList.SelectedItem.Text %>
                Examples</h2>
            <h4>
                Using pre-built Data.xml</h4>
            <rcl:FusionChart ID="staticChart" runat="server" FileName="MSStackedColumn2D.swf" ChartId="column3DChart" DataUrl="MultiSeriesStackedData.xml" Width="500" Height="300" />
            <h4>
                Using DataProviderHandler</h4>
            <rcl:FusionChart ID="dynamicChart" runat="server" FileName="MSStackedColumn2D.swf" ChartId="column3DChart" DataUrl="~/Charts/DataHandlers/MultiSeriesStackedChartDataHandler.ashx?FactoryId=2" Width="500" Height="300" />
        </center>
    </div>
    <script type="text/javascript">
        function popup(msg) {
            window.alert(msg);
        }
    </script>
</asp:Content>