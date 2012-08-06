<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MultiFileForm.aspx.cs" Inherits="NSoft.NFramework.WebHost.HttpHandlers.MultiFileForm" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>Multi File Handler</title>
	<link type="text/css" rel="Stylesheet" href="MultiFileHttpHandler.axd?S=Set.Css&T=text/css&V=1" />
	<!-- 외부사이트에서 javascript 정보를 다운로드 받습니다 -->
	<script type="text/javascript" src="MultiFileHttpHandler.axd?T=text/javascript&V=3&F=http://alexgorbatchev.com/pub/sh/current/scripts/shCore.js|http://alexgorbatchev.com/pub/sh/current/scripts/shBrushCSharp.js">
	</script>
</head>
<body>
	<form id="form1" runat="server">
	<div>
		<h1>HttpMultiFileHandler 테스트 페이지</h1>
		<p>이 글이 빨강색으로 보인다면, 두 개의 CSS 파일이 하나로 합쳐져서 로드되었다는 뜻입니다. - </p>

		<script type="text/javascript" src="MultiFileHttpHandler.axd?S=Set.Javascript&T=text/javascript&V=2&F=~/Scripts/Js3.js|~/Scripts/Js4.js">
		</script>

		<p id="jquery">
		</p>
		
		<script type="text/javascript">
			$(function() {
				$("#jquery").html("jQuery also loaded, cool!");
			});    
		</script>
	</div>
	</form>
</body>
</html>
