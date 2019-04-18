<%@ Page language="c#" Codebehind="Default.aspx.cs" AutoEventWireup="false" Inherits="JC_SoccerWeb._Default" %>
<%@ Register TagPrefix="uc1" TagName="SignIn" Src="UserControl/SignIn.ascx" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>HKJC-ScoutsFeed-Login</title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<SCRIPT LANGUAGE="javascript" FOR="document" EVENT="onkeypress"> if ( event.keyCode==34) event.returnValue = false;</SCRIPT>
		<LINK href="CentaSmsStyle.css" type="text/css" rel="stylesheet">
		<script language="javascript" src="script.js"></script>
		<script type="text/javascript">   
        function change() {    
            var imgNode = document.getElementById("vimg");    
            imgNode.src = "CheckCode.asPx?t=" + (new Date()).valueOf();  // ??是 "WaterMark.ashx?t=" + (new Date()).valueOf(); ?句?可以改????，?里加???的??是?了防止??器?存的??    
        }    
         function Clear() 
			{ 
				 document.forms[0].SignIn1_UserID.value ="";
                 document.forms[0].SignIn1_txtValidate.value=""; 
				 document.forms[0].SignIn1_Password.value="";
				  document.getElementById("SignIn1_Message").innerHTML="";
				 }
		</script>
	</HEAD>
	<body OnLoad="document.forms[0].SignIn1_UserID.focus()">
		<form id="Reports" method="post" runat="server">
			<table cellSpacing="0" cellPadding="0" width="100%" border="0">
				<tr>
					<td class="top_bar01_bg">
                        <!--     <img src="resource/mango_logo_title.gif" width="359" height="49"> -->
					</td>
				</tr>
				<tr>
					<td class="top_bar02_bg"><IMG height="15" src="resource/spacer.gif" width="15"></td>
				</tr>
			</table>
			<table cellSpacing="0" cellPadding="0" width="100%" border="0">
				<tr width="100%">
					<td vAlign="top">
						<table class="admin-tan-border" height="100%" cellSpacing="0" cellPadding="0" width="100%"
							border="0">
							<tr>
								<td height="20"><FONT face="新細明體"></FONT></td>
							</tr>
							<tr valign="top">
								<td align="center" valign="middle"><br>
									<table width="600" border="0" cellpadding="0" cellspacing="0">
										<tr>
											<td width="299" align="center"> </td>
											<td width="1" class="line_bg"></td>
											<td width="300"><P>
													<uc1:SignIn id="SignIn1" runat="server"></uc1:SignIn></P>
											</td>
										</tr>
									</table>
									<br>
								</td>
							</tr>
							<tr bgcolor="#edecd1">
								<td class="bottom_bar_bg"><FONT face="新細明體"></FONT></td>
							</tr>
						</table>
					</td>
				</tr>
				<tr>
				</tr>
			</table>
			<TABLE id="Table10" width="100%">
				<TR valign="top">
					<TD valign="top" style="display:none">
						<P align="left"><FONT color="#000000"><STRONG>電訊數碼服務系統(下稱「服務系統」) 
									使用聲明<br>
									<br>Privacy policy</STRONG>
								<BR>
							</FONT>Privacy policy
The information contained in this document is confidential and cannot be disclosed to any third party without the
written permission of STATSCORE sp. z o.o. Disclosure, publication or dissemination of documents or any other
information contained in this document violates the law.
                            For more information, please contact our sales department at (<a href="sales@statscore.com"><FONT color="blue">sales@statscore.com</FONT></a>).
						    website (<a target="_blank" href="http://www.statscore.com"><FONT color="blue">www.statscore.com</FONT></a>).</P>
						<P style="LINE-HEIGHT: 150%" align="left"><IMG id="ctl00_image2" style="BORDER-TOP-WIDTH: 0px; BORDER-LEFT-WIDTH: 0px; BORDER-BOTTOM-WIDTH: 0px; WIDTH: 7px; HEIGHT: 7px; BORDER-RIGHT-WIDTH: 0px"
								src="resource/green.bmp"> <SPAN class="style9">Best viewed with Internet 
								Explorer 6 and a screen resolution of&nbsp;1024 X 768.<BR>
								<IMG id="ctl00_image3" style="BORDER-TOP-WIDTH: 0px; BORDER-LEFT-WIDTH: 0px; BORDER-BOTTOM-WIDTH: 0px; WIDTH: 7px; HEIGHT: 7px; BORDER-RIGHT-WIDTH: 0px"
									src="resource/yellow.bmp"> Copyright 2009 Mango Limited. All Rights 
								Reserved.<BR>
								<IMG id="ctl00_image4" style="BORDER-TOP-WIDTH: 0px; BORDER-LEFT-WIDTH: 0px; BORDER-BOTTOM-WIDTH: 0px; WIDTH: 7px; HEIGHT: 7px; BORDER-RIGHT-WIDTH: 0px"
									src="resource/images_41.gif"> Last Update：</SPAN><SPAN class="style2" style="COLOR: #cc3333">[12/07/2018]</SPAN></P>
					</TD>
				</TR>
			</TABLE>
			</TD></TR>
		</form>
	</body>
</HTML>
