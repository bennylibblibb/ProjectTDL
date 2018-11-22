<%@ Page language="c#" Codebehind="ChangePassword.aspx.cs" AutoEventWireup="false" Inherits="JC_SoccerWeb.ChangePassword" %>
<%@ Register TagPrefix="uc1" TagName="UserTabs" Src="UserControl/UserTabs.ascx" %>
<%@ Register TagPrefix="anthem" Namespace="Anthem" Assembly="Anthem" %>
<%@ Register TagPrefix="uc1" TagName="MenuTabs" Src="UserControl/MenuTabs.ascx" %>
<%@ Register TagPrefix="uc1" TagName="SendTabs" Src="UserControl/SendTabs.ascx" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>Telecom Digital MMS Services</title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<SCRIPT language="javascript" event="onkeypress" for="document"> if ( event.keyCode==34) event.returnValue = false;</SCRIPT>
		<LINK href="CentaSmsStyle.css" type="text/css" rel="stylesheet">
		<script language="JavaScript" src="CalendarUtrl/Popup.js" type="text/javascript"></script>
		<script type="text/javascript">   
    
            function Clear() 
			{
				 document.getElementById("txtPW2").value ="";
				  document.getElementById("txtPW").value =""; 
            } 
            
         
		</script>
	</HEAD>
	<body OnLoad="fmSms.txtPW.focus()">
		<form id="fmSms" method="post" runat="server">
			<table cellSpacing="0" cellPadding="0" width="100%" border="0">
				<tr>
					<td class="top_bar01_bg"><img src="resource/mango_logo_title.gif" width="359" height="49">
					</td>
				</tr>
				<tr>
					<td class="top_bar02_bg" vAlign="top" height="15">
						<P align="left"><FONT color="#ffffff"><IMG height="15" src="resource/spacer.gif" width="15"></FONT>
							用戶:&nbsp;<asp:label id="lbUser" runat="server"></asp:label></P>
					</td>
				</tr>
			</table>
			<table cellSpacing="0" cellPadding="0" width="100%" border="1" height="500" class="tan-border01">
				<tr width="100%">
					<td vAlign="top">
						<table class="tan-border01" id="table1" height="100%" cellSpacing="0" cellPadding="0" width="100%"
							border="0" style="BORDER-COLLAPSE: collapse">
							<tr>
								<td class="admin-table" style="HEIGHT: 10px" vAlign="top" height="2"><IMG height="15" src="resource/spacer.gif" width="15">
								</td>
								<td style="HEIGHT: 36px" vAlign="top"><FONT face="新細明體"></FONT></td>
							</tr>
							<TR vAlign="top">
								<TD style="WIDTH: 169px; HEIGHT: 100%" vAlign="top" class="left_bar_bg"><uc1:menutabs id="MenuTabs1" runat="server"></uc1:menutabs></TD>
								<TD vAlign="top" align="center">
									<TABLE class="tan-border01" cellSpacing="10" cellPadding="0" rules="none" align="left"
										border="0">
										<TR vAlign="middle" width="100%">
										<TR>
											<TD vAlign="baseline" align="left" height="1"><FONT face="新細明體"></FONT></TD>
											<TD align="left" height="1"><FONT face="新細明體">&nbsp;</FONT></TD>
											<TD align="left" height="1"></TD>
											<TD align="left" height="1"></TD>
											<TD align="left" width="15%" height="1"></TD>
										</TR>
										<TR width="100%">
											<TD vAlign="top" align="center" width="50%" colSpan="5" height="256"><FONT face="新細明體">
													<anthem:panel id="plSendMsg" runat="server">
														<TABLE id="Table2" style="WIDTH: 399px; HEIGHT: 182px" height="182" cellSpacing="0" cellPadding="0"
															width="399" border="0">
															<TR>
																<TD style="HEIGHT: 30px" width="14"><FONT face="新細明體"></FONT></TD>
																<TD style="HEIGHT: 30px">
																	<ASP:LABEL id="lbpw" runat="server">新密碼：</ASP:LABEL></TD>
															</TR>
															<TR>
																<TD style="HEIGHT: 30px" width="14"></TD>
																<TD style="HEIGHT: 30px">
																	<ASP:TEXTBOX id="txtPW" runat="server" TextMode="Password" MaxLength="11" CssClass="safari-midtextbox"
																		Width="200px"></ASP:TEXTBOX>
																	<ASP:REQUIREDFIELDVALIDATOR id="rfvPW" runat="server" ControlToValidate="txtPW" ErrorMessage="＊"></ASP:REQUIREDFIELDVALIDATOR></TD>
															</TR>
															<TR>
																<TD style="HEIGHT: 30px" width="14">&nbsp;</TD>
																<TD style="HEIGHT: 30px">
																	<ASP:LABEL id="lbConfirm" runat="server">確認密碼：</ASP:LABEL></TD>
															</TR>
															<TR vAlign="top">
																<TD style="HEIGHT: 35px" width="14">&nbsp;</TD>
																<TD style="HEIGHT: 35px">
																	<ASP:TEXTBOX id="txtPW2" runat="server" TextMode="Password" MaxLength="21" CssClass="safari-midtextbox"
																		Width="200px"></ASP:TEXTBOX>
																	<ASP:REQUIREDFIELDVALIDATOR id="rfvPW2" runat="server" ControlToValidate="txtPW2" ErrorMessage="＊"></ASP:REQUIREDFIELDVALIDATOR>
																	<ASP:COMPAREVALIDATOR id="cvPW" runat="server" ControlToValidate="txtPW2" ErrorMessage="Passwords Do Not Match. "
																		ControlToCompare="txtPW"></ASP:COMPAREVALIDATOR></TD>
															</TR>
															<TR>
																<TD style="HEIGHT: 25px" width="14">&nbsp;</TD>
																<TD style="HEIGHT: 25px">
																	<anthem:BUTTON id="Save" runat="server" Width="64px" Text="保存"></anthem:BUTTON>&nbsp;&nbsp;&nbsp;<INPUT id="btnCannel" onclick="Clear()" type="button" value="重置" name="btnCannel">&nbsp;&nbsp;
																	<anthem:LABEL id="lbMsg" runat="server" ForeColor="Red"></anthem:LABEL></TD>
															</TR>
														</TABLE>
													</anthem:panel></FONT></TD>
										</TR>
										<TR height="100%" width="100%">
											<TD colSpan="5"><FONT face="新細明體"></FONT></TD>
										</TR>
									</TABLE>
								</TD>
							</TR>
						</table>
					</td>
				</tr>
			</table>
		</form>
	</body>
</HTML>
