 <%@ Page language="c#" Codebehind="default.aspx.cs" AutoEventWireup="false" Inherits="testascx.WebForm1" %>
<%@ Register TagPrefix="uc1" TagName="sayhello" Src="sayhello.ascx" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
<HEAD>
<title>WebForm1</title>
<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
<meta name="CODE_LANGUAGE" Content="C#">
<meta name="vs_defaultClientScript" content="JavaScript"> 
</HEAD>
<body MS_POSITIONING="GridLayout" leftmargin=0 topmargin=0>
<form id="Form121" method="post" runat="server">
<!-- 给属性LabText赋值如下格式 -->
<uc1:sayhello id="Sayhello1" LabText="我是在html里面的" runat="server"></uc1:sayhello>
</form>

</HTML>
