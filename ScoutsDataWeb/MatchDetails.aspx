<%@ Register TagPrefix="anthem" Namespace="Anthem" Assembly="Anthem" %>
<%@ Page language="c#" Codebehind="MatchDetails.aspx.cs" AutoEventWireup="false" Inherits="JC_SoccerWeb.MatchDetails" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<TITLE>Telecom Digital SMS Services</TITLE>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		 <LINK href="CentaSmsStyle.css" type="text/css" rel="stylesheet">
		<script type="text/javascript">   
		 
		</script>
	</HEAD>
	<body >
		<FORM id="Form1" method="post" runat="server">
			<TABLE align="center">
				 <TR>
					<TD align="left" style="height:40px"> </TD>
				</TR>
				<TR align="center">
					<TD align="center">
                       <anthem:DataGrid id="eventDetails" runat="server" Width="100%"   AllowPaging="false" AutoGenerateColumns="true" >
                                                <PagerStyle Mode="NumericPages"></PagerStyle><HeaderStyle Font-Bold="True"></HeaderStyle><EditItemStyle ></EditItemStyle><ItemStyle Height="34px" CssClass="grid-item"></ItemStyle>
			</anthem:DataGrid>
					</TD>
				</TR>
				 <TR>
					<TD align="left"> </TD>
				</TR>
				 
			</TABLE>
		</FORM>
	</body>
</HTML>
