<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CompressiblePage.aspx.cs" Inherits="NSoft.NFramework.WebHost.Compression.CompressiblePage" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>쿠키, 뷰스테이트를 압축하여 저장하는 페이지입니다.</title>
</head>
<body>
	<form id="form1" runat="server">
	<div>
		<asp:GridView ID="GridView1" runat="server" AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False" CellPadding="4" DataKeyNames="OrderID,ProductID" DataSourceID="SqlDataSource1" ForeColor="#333333" GridLines="None">
			<FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
			<Columns>
				<asp:CommandField ShowSelectButton="True" />
				<asp:BoundField DataField="OrderID" HeaderText="OrderID" ReadOnly="True" SortExpression="OrderID" />
				<asp:BoundField DataField="ProductID" HeaderText="ProductID" ReadOnly="True" SortExpression="ProductID" />
				<asp:BoundField DataField="ProductName" HeaderText="ProductName" SortExpression="ProductName" />
				<asp:BoundField DataField="UnitPrice" HeaderText="UnitPrice" SortExpression="UnitPrice" />
				<asp:BoundField DataField="Quantity" HeaderText="Quantity" SortExpression="Quantity" />
				<asp:BoundField DataField="Discount" HeaderText="Discount" SortExpression="Discount" />
				<asp:BoundField DataField="ExtendedPrice" HeaderText="ExtendedPrice" ReadOnly="True" SortExpression="ExtendedPrice" />
			</Columns>
			<RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
			<EditRowStyle BackColor="#999999" />
			<SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
			<PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
			<HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
			<AlternatingRowStyle BackColor="White" ForeColor="#284775" />
		</asp:GridView>
	</div>
	<asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:Northwind %>" SelectCommand="SELECT * FROM [Order Details Extended]"></asp:SqlDataSource>
	<asp:ObjectDataSource ID="ObjectDataSource1" runat="server"></asp:ObjectDataSource>
	<div>
		<table>
			<tr>
				<td>
					Property
				</td>
				<td>
					Value
				</td>
			</tr>
			<tr>
				<td>
					Name
				</td>
				<td>
					<asp:Label ID="lblName" runat="server" />
				</td>
			</tr>
			<tr>
				<td>
					Address
				</td>
				<td>
					<asp:Label ID="lblAddress" runat="server" />
				</td>
			</tr>
			<tr>
				<td>
					Email
				</td>
				<td>
					<asp:Label ID="lblEmail" runat="server" />
				</td>
			</tr>
			<tr>
				<td>
					Account
				</td>
				<td>
					<asp:Label ID="lblAccount" runat="server" />
				</td>
			</tr>
		</table>
		<asp:Repeater ID="rptFavorites" runat="server">
			<HeaderTemplate>
				Favorite Movies
				<hr />
			</HeaderTemplate>
			<ItemTemplate>
				<asp:Label ID="itemFavorites" runat="server"><%# Container.DataItem %></asp:Label><br />
				<%--<asp:Label id="lblFavoritesNo" runat="server"><%# Eval("Key") %></asp:Label>
				<asp:Label ID="lblFavorites" runat="server"><%# Eval("Value") %></asp:Label><br />--%>
			</ItemTemplate>
			<FooterTemplate>
				<hr />
			</FooterTemplate>
		</asp:Repeater>
		&nbsp;
		<asp:Label ID="lblMessage" runat="server" /><br />
		<asp:Button ID="btnRefresh" runat="server" Text="Refresh" />
	</div>
	</form>
</body>
</html>
