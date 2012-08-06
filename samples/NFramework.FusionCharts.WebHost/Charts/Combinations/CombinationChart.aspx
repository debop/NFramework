<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="CombinationChart.aspx.cs" Inherits="NSoft.NFramework.FusionCharts.WebHost.Charts.Combinations.CombinationChart" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div>
        <center>
            <asp:RadioButtonList ID="ChartNameList" runat="server" AutoPostBack="true" Width="100%" RepeatLayout="Flow" RepeatDirection="Horizontal">
                <asp:ListItem Text="2D Single Y Combination " Value="MSCombi2D" />
                <asp:ListItem Text="3D Single Y Combination" Value="MSCombi3D" />
                <asp:ListItem Text="Column 3D + Line Single Y" Value="MSColumnLine3D" />
                <asp:ListItem Text="2D Dual Y Combination" Value="MSCombiDY2D" />
                <asp:ListItem Text="Column 3D + Line Dual Y" Value="MSColumn3DLineDY" />
                <asp:ListItem Text="Stacked Column 3D + Line Dual Y" Value="StackedColumn3DLineDY" />
                <asp:ListItem Text="Muti-series Stacked Column" Value="MSStackedColumn2DLineDY" />
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
            <br />
            <br />
            <asp:Label ID="lblChartName" runat="server" />
        </center>
    </div>
    <script type="text/javascript">
        function PopUp(msg) {
            window.alert(msg);
        }
    </script>
</asp:Content>