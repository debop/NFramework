<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Client.aspx.cs" Inherits="NSoft.NFramework.StringResources.WebHost.UnitOfWorkTesting.Client" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
    <head runat="server">
        <title></title>
    </head>
    <body>
        <form id="form1" runat="server">
            <div>
                Client 입니다.<br />
                UnitOfWork.IsStarted = <%= UnitOfWork.IsStarted %>
            </div>
        </form>
    </body>
</html>