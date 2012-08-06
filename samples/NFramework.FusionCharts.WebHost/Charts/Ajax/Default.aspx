<%@ Page Title="Ajax enabled chart sample" Language="C#" MasterPageFile="~/MasterPage.Master"
         AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="NSoft.NFramework.FusionCharts.WebHost.Charts.Ajax.Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table style="border: 1px dotted #aaaaaa;" width="778px">
        <tr>
            <td align="center" valign="middle" colspan="2">
                <h2>
                    FusionCharts Example - UpdatePanel #1</h2>
                <h4>
                    Using ASP.NET Ajax</h4>
                <h5>
                    Please select a factory</h5>
            </td>
        </tr>
        <tr>
            <td align="center" valign="middle" colspan="2" style="border: 1px dotted #dedede; height: 43px;">
                <asp:UpdatePanel ID="FactorySelector" runat="server">
                    <ContentTemplate>
                        <asp:RadioButtonList ID="RadioButtonList1" runat="server" AutoPostBack="True" Height="40px"
                                             OnSelectedIndexChanged="RadioButtonList1_SelectedIndexChanged" Width="400px"
                                             RepeatDirection="Horizontal" Style="font-family: Verdana; font-size: 14px; font-weight: bold;"
                                             ForeColor="#404040">
                        </asp:RadioButtonList>
                    </ContentTemplate>
                </asp:UpdatePanel>
                &nbsp;
            </td>
        </tr>
        <tr>
            <td valign="middle" style="border-bottom: #cdcdcd 1px solid; border-left: #cdcdcd 1px dotted; border-right: #cdcdcd 1px dotted; border-top: #cdcdcd 1px dotted; height: 299px; width: 320px;"
                align="center">
                <asp:UpdatePanel ID="GridUP" runat="server">
                    <ContentTemplate>
                        <div style="height: 350px; overflow: auto; overflow-x: hidden; overflow-y: auto; width: 318px;">
                            <asp:GridView ID="GridView1" runat="server" BackColor="White" BorderColor="#DEDFDE"
                                          BorderStyle="None" BorderWidth="1px" CellPadding="4" ForeColor="Black" GridLines="Vertical"
                                          Height="350px" Width="300px" AutoGenerateColumns="False">
                                <FooterStyle BackColor="#CCCC99" />
                                <RowStyle BackColor="#EFEFEF" Font-Names="Verdana" Font-Size="10px" />
                                <PagerStyle BackColor="#F7F7DE" ForeColor="Black" HorizontalAlign="Right" />
                                <SelectedRowStyle BackColor="#CE5D5A" Font-Bold="True" ForeColor="White" />
                                <HeaderStyle BackColor="#E0E0E0" Font-Bold="True" ForeColor="#404040" Font-Names="Verdana"
                                             Font-Size="11px" />
                                <AlternatingRowStyle BackColor="White" />
                                <Columns>
                                    <asp:BoundField DataField="DatePro" DataFormatString="{0:yyyy-MM-dd}" HeaderText="Date">
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Quantity" HeaderText="Quantity">
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:BoundField>
                                </Columns>
                            </asp:GridView>
                        </div>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="RadioButtonList1" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>
                <asp:UpdateProgress ID="UpdateProgress1" runat="server" DisplayAfter="50">
                    <ProgressTemplate>
                        <img id="Img1" style="left: 153px; position: absolute; top: 324px; z-index: 5;" src="~/images/loading.gif"
                             runat="server" />
                    </ProgressTemplate>
                </asp:UpdateProgress>
            </td>
            <td valign="middle" style="border: 1px dotted #dedede; height: 350px; width: 440px;">
                <asp:UpdatePanel ID="FusionChartsUP" runat="server">
                    <ContentTemplate>
                        <asp:Panel ID="Panel1" runat="server" Height="350px" Width="445px">
                            <rcl:FusionChart ID="factoryOutputChart" runat="server" FileName="Line.swf" ChartId="FactoryOutputChart1"
                                             DataUrl="FactoryDataHandler.ashx" Width="445" Height="350" />
                        </asp:Panel>
                    </ContentTemplate>
                </asp:UpdatePanel>
                <asp:UpdateProgress ID="UpdateProgress2" runat="server" DisplayAfter="50">
                    <ProgressTemplate>
                        <img id="Img2" src="~/images/loading.gif" runat="server" style="left: 564px; position: absolute; top: 327px; z-index: 5;" />
                    </ProgressTemplate>
                </asp:UpdateProgress>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:UpdatePanel ID="DescriptionUP" runat="server">
                    <ContentTemplate>
                        <asp:Label ID="lblFactory" runat="server"></asp:Label>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
        </tr>
    </table>
    <script type="text/javascript">
        function PopUp(msg) {
            window.alert(msg);
        }
    </script>
</asp:Content>