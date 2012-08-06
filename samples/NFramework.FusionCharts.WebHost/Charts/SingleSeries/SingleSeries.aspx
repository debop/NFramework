<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="SingleSeries.aspx.cs" Inherits="NSoft.NFramework.FusionCharts.WebHost.Charts.SingleSeries.SingleSeries" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div>
        <center>
            <asp:RadioButtonList ID="ChartNameList" runat="server" AutoPostBack="true" Width="100%" RepeatLayout="Flow" RepeatDirection="Horizontal">
                <asp:ListItem Text="Column2D" Value="Column2D" />
                <asp:ListItem Text="Column3D" Value="Column3D" />
                <asp:ListItem Text="Pie2D" Value="Pie2D" />
                <asp:ListItem Text="Pie3D" Value="Pie3D" />
                <asp:ListItem Text="Line" Value="Line" />
                <asp:ListItem Text="Bar2D" Value="Bar2D" />
                <asp:ListItem Text="Area2D" Value="Area2D" />
                <asp:ListItem Text="Doughnut2D" Value="Doughnut2D" />
                <asp:ListItem Text="Doughnut3D" Value="Doughnut3D" />
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
            <rcl:FusionChart ID="dynamicChart" runat="server" FileName="Line.swf" ChartId="column3DChart" DataUrl="~/Charts/DataHandlers/SingleSeriesChartDataHandler.ashx?FactoryId=2" Width="500" Height="300" />
        </center>
    </div>
    <script type="text/javascript">
        function PopUp(msg) {
            window.alert(msg);
        }
    </script>
</asp:Content>