<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="DrawingPad.aspx.cs" Inherits="NSoft.NFramework.FusionCharts.WebHost.Widgets.DrawingPad.DrawingPad" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div>
        <rcl:FusionChart ID="drawingPad" runat="server" FileName="DrawingPad.swf" ChartId="drawingPad" DataUrl="DrawingPad.xml" Width="100%" Height="500" />
    </div>
</asp:Content>