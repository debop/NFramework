<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="ScrollChart.aspx.cs" Inherits="NSoft.NFramework.FusionCharts.WebHost.Charts.Scroll.ScrollChart" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div>
        <center>
            <asp:RadioButtonList ID="ChartNameList" runat="server" AutoPostBack="true" Width="100%" RepeatLayout="Flow" RepeatDirection="Horizontal">
                <asp:ListItem Text="Scroll Column 2D" Value="ScrollColumn2D" />
                <asp:ListItem Text="Scroll Line 2D" Value="ScrollLine2D" />
                <asp:ListItem Text="Scroll Area 2D" Value="ScrollArea2D" />
                <asp:ListItem Text="Scroll Stacked Column 2D" Value="ScrollStackedColumn2D" />
                <asp:ListItem Text="Scroll Combination 2D" Value="ScrollCombi2D" />
                <asp:ListItem Text="Scroll Combination Dual Y 2D" Value="ScrollCombiDY2D" />
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
            <rcl:FusionChart ID="dynamicChart" runat="server" FileName="Line.swf" ChartId="column3DChart" DataUrl="~/Charts/DataHandlers/CombinationChartDataHandler.ashx?FactoryId=2" Width="500" Height="300" DebugMode="false" />
        </center>
    </div>
    <script type="text/javascript">
        function PopUp(msg) {
            window.alert(msg);
        }
    </script>
</asp:Content>