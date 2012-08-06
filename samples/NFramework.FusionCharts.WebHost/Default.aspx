<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="NSoft.NFramework.FusionCharts.WebHost._Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
    <head runat="server">
        <title></title>
    </head>
    <body>
        <form id="form1" runat="server">
            <h3>
                NFramework.FusionCharts Sample Web Application
            </h3>
            <div>
                IIS에 게시할 경우<br />
		
                1. Application Pool의 버전을 맞추고 (NET-2.0 / NET-4.0 용 구분됨)<br />
                2. Applicaiton Pool의 권한을 LocalSystem으로 해줘야 로그도 쓰고, Chart도 제대로 읽어온다
            </div>
        </form>
    </body>
</html>