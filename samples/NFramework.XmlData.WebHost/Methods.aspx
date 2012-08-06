<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Methods.aspx.cs" Inherits="NSoft.NFramework.XmlData.WebHost.Methods" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
    <head id="Head1" runat="server">
        <title>XmlDataService에서 제공되는 Method 정보</title>
        <meta http-equiv="Keyword" content="NSoft.NFramework,XmlDataService" />
        <style>
            BODY {
                background-color: White;
                color: SteelBlue;
                font-family: 맑은 고딕, Lucida Console;
                font-size: 9pt;
                left-margin: 2pt;
                top-margin: 2pt;
            }
        </style>
    </head>
    <body>
        <form id="form1" runat="server">
            <h1>
                Method Infomation</h1>
            <h3>
                Product:
                <asp:Label ID="lblProductName" runat="server" /></h3>
            <asp:Repeater ID="rptMethods" runat="server">
                <HeaderTemplate>
                    <table border="0" cellpadding="2" cellspacing="0">
                    <tr>
                        <th>
                            Method Name
                        </th>
                        <th>
                            Method Body
                        </th>
                    </tr>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td>
                            <asp:Label ID="lblMethodName" runat="server"><%# Eval("Key") %></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="lblMethodBody" runat="server"><%# Eval("Value") %></asp:Label>
                        </td>
                    </tr>
                </ItemTemplate>
                <AlternatingItemTemplate>
                    <tr style="background: Beige;">
                        <td>
                            <asp:Label ID="lblMethodName2" runat="server"><%# Eval("Key") %></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="lblMethodBody2" runat="server"><%# Eval("Value") %></asp:Label>
                        </td>
                    </tr>
                </AlternatingItemTemplate>
                <FooterTemplate>
                </table>
                </FooterTemplate>
            </asp:Repeater>
        </form>
        <br />
        <div>
            Usage :
            <ul>
                <li><a href="Methods.aspx?Product=Northwind">Use Northwind : Product=Northwind</a></li>
                <li><a href="Methods.aspx?Product=Pubs">Use Pubs : Product=Pubs</a></li>
            </ul>
        </div>
        <div>
            <asp:Label ID="lblError" runat="server" Font-Bold="true" ForeColor="Red"></asp:Label>
        </div>
    </body>
</html>