<%@ Register TagPrefix="uc1" TagName="SendTabs" Src="UserControl/SendTabs.ascx" %>
<%@ Register TagPrefix="uc1" TagName="MenuTabs" Src="UserControl/MenuTabs.ascx" %>
<%@ Register TagPrefix="anthem" Namespace="Anthem" Assembly="Anthem" %>
<%@ Register TagPrefix="uc1" TagName="UserTabs" Src="UserControl/UserTabs.ascx" %>
<%@ Page language="c#" Codebehind="UserMaintenance.aspx.cs" AutoEventWireup="false" Inherits="JC_SoccerWeb.UserMaintenance" %>
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
        function change() {    
            var imgNode = document.getElementById("vimg");    
            imgNode.src = "CheckCode.asPx?t=" + (new Date()).valueOf();  // 234, 261??是 "WaterMark.ashx?t=" + (new Date()).valueOf(); ?句?可以改????，?里加???的??是?了防止??器?存的??    
        }  
        
        	function ConfirmSendontime() 
			{
				if (confirm("Are you sure?")) 
				{
					return true;
				} 
            } 
            
            function Clear() 
			{
				 document.getElementById("txtSmsContent").innerHTML ="";
				  document.getElementById("tbNum").innerHTML ="";
           
            } 
            
            function ShowInputMiddle(pagePath, winAttrs) {
				window.showModalDialog(pagePath, '', winAttrs);
	        } 
	        
	          function   Focuss(  id,clear)    
		   {     
		   if(clear=='All')
		   {
		   if(id=='txtUserID1')
		   { 
		      document.getElementById("txtUserID1").value =""; 
		      document.getElementById("txtUserName1").value =""; 
		      document.getElementById("txtPwd1").value =""; 
		      document.getElementById("txtEmail1").value ="";  
		      document.getElementById("txtDepart1").value =""; 
		   } 
		   } 
		   document.getElementById(id).focus();
         }   

	        function  focusload()
  { 
 if (document.getElementById('txtUserID1')!=null)
 { 
   document.getElementById("txtUserID1").focus();
 }
 }
		</script>
	</HEAD>
	<body OnLoad="focusload()">
	<form id="fmSms" method="post" runat="server">
        <table cellSpacing="0" cellPadding="0" width="100%" border="0">
        <tr>
            <td class="top_bar01_bg">
                <img src="resource/mango_logo_title.gif" width="359" height="49">
            </td>
        </tr>
        <tr>
            <td class="top_bar02_bg" vAlign="top" height="15">
                <P align="left">
                    <FONT color="#ffffff">
                        <IMG height="15" src="resource/spacer.gif" width="15">
                    </FONT>
                    用戶:&nbsp;<asp:label id="lbUser" runat="server"></asp:label>
                </P>
            </td>
        </tr>
    </table>
    <table cellSpacing="0" cellPadding="0" width="100%" border="0">
        <tr width="100%">
            <td vAlign="top">
                <table class="admin-tan-border" id="table1" cellSpacing="0" cellPadding="0"
                       width="100%" border="0">
                    <tr>
                        <td class="admin-table" style="HEIGHT: 10px" vAlign="top" height="2">
                            <IMG height="15" src="resource/spacer.gif" width="15">
                        </td>
                        <td   vAlign="top"> 
                                <uc1:menutabs id="MenuTabs1" runat="server"></uc1:menutabs>
                        </td>
                    </tr>
                    <TR vAlign="top">
                        <TD vAlign="top" class="left_bar_bg">
                            <uc1:sendtabs id="SendTabs1" runat="server"></uc1:sendtabs> 
                        </TD>
                        <TD vAlign="top" class="tan-border02" height="1">
    <anthem:panel id="panel1" runat="server" Visible="false">
										<TABLE class="tan-border02" style="WIDTH: 100%; BORDER-COLLAPSE: collapse; HEIGHT: 380px"
											cellSpacing="10" cellPadding="0" rules="none" align="left" border="0">
											<TR>
												<TD vAlign="baseline" align="left" height="1"></TD>
												<TD align="left" height="1"></TD>
												<TD align="left" height="1"><FONT face="新細明體"></FONT></TD>
												<TD align="left" height="1"><FONT face="新細明體">&nbsp;</FONT></TD>
												<TD align="left" width="20%" height="1"></TD>
											</TR>
											<TR vAlign="middle" width="100%">
												<TD vAlign="baseline" align="left" height="1"><STRONG>查詢用戶：</STRONG>
												</TD>
												<TD align="left" height="1">用戶：
													<anthem:DropDownList id="dplUsers" runat="server" Width="80px" AutoCallBack="True">
														<asp:ListItem Value="All" Selected="True">All</asp:ListItem>
													</anthem:DropDownList></TD>
												<TD align="left" height="1">日期從：
													<asp:TextBox id="txtStartDate" onclick="fPopUpDlg('CalendarUtrl/calendar.htm', document.fmSms.txtStartDate, 'winpop', 234, 261);return false"
														runat="server" Width="80px" ReadOnly="True"></asp:TextBox>
													<asp:RequiredFieldValidator id="rfv" runat="server" ControlToValidate="dplUsers" ErrorMessage="*"></asp:RequiredFieldValidator></TD>
												<TD align="left" height="1">到:
													<asp:TextBox id="txtEndDate" onclick="fPopUpDlg('CalendarUtrl/calendar.htm', document.fmSms.txtEndDate, 'winpop', 234, 261);return false"
														runat="server" Width="80px" ReadOnly="True"></asp:TextBox></TD>
												<TD align="left" width="20%" height="1"><FONT face="新細明體">
														<anthem:Button id="btnSearch" runat="server" Text="查詢"></anthem:Button></FONT></TD>
											<TR>
												<TD vAlign="baseline" align="left" height="1"></TD>
												<TD align="left" height="1"></TD>
												<TD align="left" height="1"></TD>
												<TD align="left" height="1"></TD>
												<TD align="left" width="20%" height="1"></TD>
											</TR>
											<TR width="100%">
												<TD vAlign="top" align="left" width="50%" colSpan="5" height="256">
													<anthem:DataGrid id="dgData" runat="server" Width="100%" AutoGenerateColumns="False" AllowPaging="True">
														<PagerStyle Mode="NumericPages"></PagerStyle>
														<HeaderStyle Font-Bold="True"></HeaderStyle>
														<Columns>
															<asp:TemplateColumn HeaderText="ID">
																<HeaderStyle  Width="40px" HorizontalAlign="Left" Wrap="false" CssClass="grid-header" VerticalAlign="Middle"></HeaderStyle>
																<ItemStyle Width="40px" CssClass="grid-item"></ItemStyle>
																<ItemTemplate>
																	<%#this.dgData.CurrentPageIndex * this.dgData.PageSize + Container.ItemIndex + 1%>
																</ItemTemplate>
															</asp:TemplateColumn>
															<asp:TemplateColumn HeaderText="用戶ＩＤ">
																<HeaderStyle Width="80px" Wrap="False" HorizontalAlign="Left" CssClass="grid-header" VerticalAlign="Middle"></HeaderStyle>
																<ItemStyle CssClass="grid-item" Width="80px" ></ItemStyle>
																<ItemTemplate>
																	<%# DataBinder.Eval(Container, "DataItem.USER_ID")%>
																</ItemTemplate>
															</asp:TemplateColumn>
															<asp:TemplateColumn HeaderText="用戶名">
																<HeaderStyle Width="80px" Wrap="False" HorizontalAlign="Left" CssClass="grid-header" VerticalAlign="Middle"></HeaderStyle>
																<ItemStyle CssClass="grid-item" Width="80px"></ItemStyle>
																<ItemTemplate>
																	<%# DataBinder.Eval(Container, "DataItem.USER_NAME")%>
																</ItemTemplate>
															</asp:TemplateColumn>
															<asp:TemplateColumn HeaderText="權限(共享組別)">
																<HeaderStyle Width="100px" Wrap="False" HorizontalAlign="Left" CssClass="grid-header" VerticalAlign="Middle"></HeaderStyle>
																<ItemStyle CssClass="grid-item" Width="100px"></ItemStyle>
																<ItemTemplate>
																	<%# DataBinder.Eval(Container, "DataItem.SHAREDGP_RIGHT")%>
																</ItemTemplate>
															</asp:TemplateColumn>
															<asp:TemplateColumn HeaderText="Email">
																<HeaderStyle Width="80px" Wrap="False" HorizontalAlign="Left" CssClass="grid-header" VerticalAlign="Middle"></HeaderStyle>
																<ItemStyle CssClass="grid-item" Width="80px"></ItemStyle>
																<ItemTemplate>
																	<%# DataBinder.Eval(Container, "DataItem.USER_EMAIL")%>
																</ItemTemplate>
															</asp:TemplateColumn>
															<asp:TemplateColumn HeaderText="部門">
																<HeaderStyle Width="80px" Wrap="False" HorizontalAlign="Left" CssClass="grid-header" VerticalAlign="Middle"></HeaderStyle>
																<ItemStyle CssClass="grid-item" Width="80px"></ItemStyle>
																<ItemTemplate>
																	<%# DataBinder.Eval(Container, "DataItem.DEPARTMENT")%>
																</ItemTemplate>
															</asp:TemplateColumn>
															<asp:TemplateColumn HeaderText="建立日期">
																<HeaderStyle HorizontalAlign="Left" Width="160px" CssClass="grid-header" VerticalAlign="Middle"></HeaderStyle>
																<ItemStyle CssClass="grid-item" Width="160px"></ItemStyle>
																<ItemTemplate>
																	<%# DataBinder.Eval(Container, "DataItem.CREATE_DATE")%>
																</ItemTemplate>
															</asp:TemplateColumn>
														</Columns>
													</anthem:DataGrid></TD>
											</TR>
											<TR width="100%">
												<TD colSpan="5"><FONT face="新細明體"></FONT></TD>
											</TR>
										</TABLE>
									</anthem:panel>
                                    <anthem:panel id="panel2" runat="server" Visible="false">
										<TABLE class="tan-border02" style="WIDTH: 100%; BORDER-COLLAPSE: collapse; HEIGHT: 380px"
											cellSpacing="10" cellPadding="0" rules="none" align="left" border="0">
											<TR>
												<TD style="HEIGHT: 4px" vAlign="top" align="left" colSpan="4" height="4"><FONT face="新細明體">&nbsp;</FONT></TD>
											</TR>
											<TR vAlign="top" width="100%">
												<TD style="HEIGHT: 30px" align="left" colSpan="4" height="30"><FONT face="新細明體" size="4"><STRONG>&nbsp;&nbsp;&nbsp; 
															新增用戶： </STRONG></FONT>
												</TD>
												</tr>
										
											
											
											<TR width="100%">
												<TD style="WIDTH: 98px; HEIGHT: 24px" align="right" width="98" height="24"><FONT face="新細明體">
														<asp:Label id="lbUserID1" runat="server">用戶ＩＤ：</asp:Label>
														<asp:RequiredFieldValidator id="Requiredfieldvalidator21" runat="server" Width="10px" ControlToValidate="txtUserID1"
															ErrorMessage="*"></asp:RequiredFieldValidator></FONT></TD>
												<TD style="WIDTH: 162px; HEIGHT: 24px" vAlign="top" align="left" width="162" height="24"><FONT face="新細明體">
														<anthem:TextBox id="txtUserID1" runat="server" Width="120px"></anthem:TextBox></FONT></TD>
												<TD style="WIDTH: 126px; HEIGHT: 24px" align="left" height="24">
													<asp:Label id="lbUserName1" runat="server">用戶名：</asp:Label>
													<asp:RequiredFieldValidator id="Requiredfieldvalidator51" runat="server" Width="10px" ControlToValidate="txtUserName1"
														ErrorMessage="*"></asp:RequiredFieldValidator></TD>
												<TD style="HEIGHT: 24px" align="left" height="24">
													<anthem:TextBox id="txtUserName1" runat="server" Width="120px"></anthem:TextBox></TD>
											</TR>
											<TR>
												<TD style="WIDTH: 98px; HEIGHT: 24px" vAlign="baseline" align="right" width="98" height="24">
													<P>
														<asp:Label id="lbPwd1" runat="server">用戶密碼：</asp:Label>
														<asp:RequiredFieldValidator id="Requiredfieldvalidator41" runat="server" Width="10px" ControlToValidate="txtPwd1"
															ErrorMessage="*"></asp:RequiredFieldValidator></P>
												</TD>
												<TD style="WIDTH: 162px; HEIGHT: 24px" vAlign="top" align="left" width="162" height="24">
													<anthem:TextBox id="txtPwd1" runat="server" Width="120px" TextMode="Password">123</anthem:TextBox></TD>
												<TD style="WIDTH: 126px; HEIGHT: 24px" align="left" height="24">
													<asp:Label id="Label21" runat="server">權限(共享組別)：</asp:Label></TD>
												<TD style="HEIGHT: 24px" align="left" height="24">
													<anthem:DropDownList id="dplRight1" runat="server" Width="120px">
														<asp:ListItem Value="Y" Selected="True">有</asp:ListItem>
														<asp:ListItem Value="N">無</asp:ListItem>
													</anthem:DropDownList></TD>
											</TR>  
											<TR>
												<TD style="WIDTH: 98px; HEIGHT: 13px" vAlign="baseline" align="right" width="98" height="13"><FONT face="新細明體">
														<asp:Label id="lbEmail" runat="server">用戶Email：</asp:Label>
														<asp:RegularExpressionValidator id="rvEmail" runat="server" Width="10px" ControlToValidate="txtEmail1" ErrorMessage="*"
															ValidationExpression="\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"></asp:RegularExpressionValidator></FONT></TD>
												<TD style="WIDTH: 162px; HEIGHT: 13px" vAlign="top" align="left" width="162" height="13"><FONT face="新細明體">
														<anthem:TextBox id="txtEmail1" runat="server" Width="120px"></anthem:TextBox></FONT></TD>
												<TD style="WIDTH: 126px; HEIGHT: 13px" align="left" height="13">
													<asp:Label id="Label3" runat="server" Width="110px">用戶Department：</asp:Label>
													<asp:RequiredFieldValidator id="Requiredfieldvalidator7" runat="server" Width="10px" ControlToValidate="txtDepart1"
														ErrorMessage="*"></asp:RequiredFieldValidator></TD>
												<TD style="HEIGHT: 13px" align="left" height="13">
													<anthem:TextBox id="txtDepart1" runat="server" Width="120px"></anthem:TextBox></TD>
											</TR>
											<TR>
												<TD style="WIDTH: 98px" vAlign="baseline" align="left" width="98" height="1"><FONT face="新細明體"></FONT></TD>
												<TD style="WIDTH: 162px" vAlign="top" align="left" width="162" height="1"><FONT face="新細明體"></FONT></TD>
												<TD style="WIDTH: 126px" align="left" height="1"><FONT face="新細明體"></FONT></TD>
												<TD align="left" height="1"><FONT face="新細明體">
														<anthem:BUTTON id="btnNewUser" runat="server" Text="保存"></anthem:BUTTON>&nbsp;
														<anthem:Label id="lbMsg" runat="server" ForeColor="Red"></anthem:Label></FONT></TD>
											</TR>
											<TR>
												<TD style="WIDTH: 98px" vAlign="baseline" align="left" width="98" height="100"></TD>
												<TD style="WIDTH: 162px" vAlign="top" align="left" width="162" height="100"><FONT face="新細明體"></FONT></TD>
												<TD style="WIDTH: 126px" align="left" height="100"><FONT face="新細明體"></FONT></TD>
												<TD align="left" height="100"></TD>
											</TR>
										</TABLE>
									</anthem:panel>
                                    <anthem:panel id="panel3" runat="server" Visible="false">
										<TABLE class="tan-border02" style="WIDTH: 100%; BORDER-COLLAPSE: collapse; HEIGHT: 380px"
											cellSpacing="10" cellPadding="0" rules="none" align="left" border="0">
											<TR>
												<TD style="HEIGHT: 4px" vAlign="top" align="left" colSpan="4" height="4"><FONT face="新細明體">&nbsp;</FONT></TD>
											</TR>
											<TR vAlign="top" width="100%">
												<TD style="HEIGHT: 30px" align="left" colSpan="4" height="30"><FONT face="新細明體" size="4"><STRONG>&nbsp;&nbsp;&nbsp; 
															更新用戶：
															<anthem:DropDownList id="dplUsersU" runat="server" Width="80px" AutoCallBack="True">
																<asp:ListItem Value="All" Selected="True">All</asp:ListItem>
															</anthem:DropDownList>
															<anthem:Button id="btnSearchU" runat="server" Visible="False" Text="選定"></anthem:Button></STRONG></FONT></TD>
											<TR width="100%">
												<TD style="WIDTH: 98px; HEIGHT: 24px" align="right" width="98" height="24"><FONT face="新細明體">
														<asp:Label id="lbUserIDU" runat="server">用戶ＩＤ：</asp:Label>
														<asp:RequiredFieldValidator id="Requiredfieldvalidator12" runat="server" Width="10px" ControlToValidate="txtUserIDU"
															ErrorMessage="*"></asp:RequiredFieldValidator></FONT></TD>
												<TD style="WIDTH: 162px; HEIGHT: 24px" vAlign="top" align="left" width="162" height="24"><FONT face="新細明體">
														<anthem:TextBox id="txtUserIDU" runat="server" Width="120px" ReadOnly="True"></anthem:TextBox></FONT></TD>
												<TD style="WIDTH: 126px; HEIGHT: 24px" align="left" height="24">
													<asp:Label id="lbUserNameU" runat="server">用戶名：</asp:Label>
													<asp:RequiredFieldValidator id="Requiredfieldvalidator8" runat="server" Width="10px" ControlToValidate="txtUserNameU"
														ErrorMessage="*"></asp:RequiredFieldValidator></TD>
												<TD style="HEIGHT: 24px" align="left" height="24">
													<anthem:TextBox id="txtUserNameU" runat="server" Width="120px"></anthem:TextBox></TD>
											</TR>
											<TR>
												<TD style="WIDTH: 98px; HEIGHT: 24px" align="right" width="98" height="24">
													<asp:Label id="lbPwdU" runat="server" Visible="true">用戶密碼：</asp:Label>
													<asp:RequiredFieldValidator id="rfvPwdU" runat="server" Width="10px" ControlToValidate="txtPwdU" ErrorMessage="*"></asp:RequiredFieldValidator></TD>
												<TD style="WIDTH: 162px; HEIGHT: 24px" vAlign="top" align="left" width="162" height="24">
													<anthem:TextBox id="txtPwdU" runat="server" Width="120px"></anthem:TextBox></TD>
												<TD style="WIDTH: 126px; HEIGHT: 24px" align="left" height="24">
													<asp:Label id="lbRightU" runat="server">權限(共享組別)：</asp:Label></TD>
												<TD style="HEIGHT: 24px" align="left" height="24">
													<anthem:DropDownList id="dplRightU" runat="server" Width="120px">
														<asp:ListItem Value="Y" Selected="True">有</asp:ListItem>
														<asp:ListItem Value="N">無</asp:ListItem>
													</anthem:DropDownList></TD>
											</TR>
											<TR>
												<TD style="WIDTH: 98px; HEIGHT: 13px" vAlign="baseline" align="right" width="98" height="13"><FONT face="新細明體">
														<asp:Label id="lbEmailU" runat="server">用戶Email：</asp:Label>
														<asp:RegularExpressionValidator id="rvEmailU" runat="server" Width="10px" ControlToValidate="txtEmailU" ErrorMessage="*"
															ValidationExpression="\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"></asp:RegularExpressionValidator></FONT></TD>
												<TD style="WIDTH: 162px; HEIGHT: 13px" vAlign="top" align="left" width="162" height="13"><FONT face="新細明體">
														<anthem:TextBox id="txtEmailU" runat="server" Width="120px"></anthem:TextBox></FONT></TD>
												<TD style="WIDTH: 126px; HEIGHT: 13px" align="left" height="13">
													<asp:Label id="Label1" runat="server" Width="110px">用戶Department：</asp:Label>
													<asp:RequiredFieldValidator id="Requiredfieldvalidator11" runat="server" Width="10px" ControlToValidate="txtDepartU"
														ErrorMessage="*"></asp:RequiredFieldValidator></TD>
												<TD style="HEIGHT: 13px" align="left" height="13">
													<anthem:TextBox id="txtDepartU" runat="server" Width="120px"></anthem:TextBox></TD>
											</TR>
											<TR>
												<TD style="WIDTH: 98px" vAlign="baseline" align="left" width="98" height="1"><FONT face="新細明體"></FONT></TD>
												<TD style="WIDTH: 162px" vAlign="top" align="left" width="162" height="1"><FONT face="新細明體"></FONT></TD>
												<TD style="WIDTH: 126px" align="left" height="1"><FONT face="新細明體"></FONT></TD>
												<TD align="left" height="1"><FONT face="新細明體">
														<anthem:BUTTON id="btnNewUserU" runat="server" Text="保存"></anthem:BUTTON>&nbsp;
														<anthem:Label id="lbMsgU" runat="server" ForeColor="Red"></anthem:Label></FONT></TD>
											</TR>
											<TR>
												<TD style="WIDTH: 98px" vAlign="baseline" align="left" width="98" height="100"></TD>
												<TD style="WIDTH: 162px" vAlign="top" align="left" width="162" height="100"><FONT face="新細明體"></FONT></TD>
												<TD style="WIDTH: 126px" align="left" height="100"><FONT face="新細明體"></FONT></TD>
												<TD align="left" height="100"></TD>
											</TR>
										</TABLE>
									</anthem:panel>
                                    <anthem:panel id="panel4" runat="server" Visible="false">
										<TABLE class="tan-border02" style="WIDTH: 100%; BORDER-COLLAPSE: collapse; HEIGHT: 380px"
											cellSpacing="10" cellPadding="0" rules="none" align="left" border="0">
											<TR>
												<TD style="HEIGHT: 20px" vAlign="top" align="left" width="98" colSpan="4"></TD>
											</TR>
											<TR vAlign="top" width="100%">
												<TD style="HEIGHT: 4px" vAlign="top" align="left" colSpan="4" height="4"><FONT face="新細明體" size="4"><STRONG>&nbsp;&nbsp;&nbsp;刪除用戶：
														</STRONG></FONT>
													<anthem:DropDownList id="dplUsersD" runat="server" Width="80px">
														<asp:ListItem Value="All" Selected="True">All</asp:ListItem>
													</anthem:DropDownList>
													<anthem:Button id="btnDelete" runat="server" Text="刪除"></anthem:Button>
													<anthem:Label id="lbMsgD" runat="server" ForeColor="Red"></anthem:Label></TD>
											<TR>
												<TD style="WIDTH: 98px; HEIGHT: 21px" vAlign="top" align="left" width="98" colSpan="4"
													height="21"><FONT face="新細明體"></FONT></TD>
											</TR>
											<TR>
												<TD style="WIDTH: 98px" vAlign="baseline" align="left" width="98" height="100"></TD>
												<TD style="WIDTH: 162px" vAlign="top" align="left" width="162" height="100"><FONT face="新細明體"></FONT></TD>
												<TD style="WIDTH: 126px" align="left" height="100"><FONT face="新細明體"></FONT></TD>
												<TD align="left" height="100"></TD>
											</TR>
										</TABLE>
									</anthem:panel>    
    
                        </TD>
                    </TR>
                </table>
            </td>
        </tr>

    </table>   
</form>
</body>
</HTML>