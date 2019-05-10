<%@ Control Language="c#" AutoEventWireup="false" Codebehind="SignIn.ascx.cs" Inherits="JC_SoccerWeb.UserControl.SignIn" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<table cellSpacing="0" cellPadding="0" width="320" border="0">
	<TR>
		<TD colSpan="2">&nbsp;</TD>
	</TR>
	<%--<TR>
		<TD></TD>
		<TD><FONT face="新細明體">&nbsp;</FONT></TD>
	</TR>--%>
	<tr>
		<TD><FONT face="新細明體"></FONT></TD>
		<td><span class="header-gray" style="HEIGHT:20px;">用戶登錄</span></td>
	</tr>
    <TR>
		<TD>&nbsp;</TD>
		<TD>&nbsp;</TD>
	</TR>
    <TR>
		<TD>&nbsp;</TD>
		<TD>&nbsp;</TD>
	</TR>
	<tr> 
        <TD>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</TD>
		<td>
			<span>用戶 ID:</span>
			<asp:RequiredFieldValidator id="RequiredFieldValidator1" runat="server" ErrorMessage="*" ControlToValidate="UserID"></asp:RequiredFieldValidator>
		</td>
	</tr>
	<tr>
		<TD><FONT face="新細明體"></FONT></TD>
		<td><FONT face="新細明體"></FONT><FONT face="新細明體"></FONT>
		<%--	<br>--%>
			<asp:TextBox id="UserID" columns="9" width="200px" cssclass="safari-midtextbox" runat="server"></asp:TextBox>
		</td>
	</tr>
	<tr>
		<TD></TD>
		<td><FONT face="新細明體"></FONT>
			<br>
			<span class="font-size: 16px;">密碼:</span>
			<asp:RequiredFieldValidator id="RequiredFieldValidator2" runat="server" ErrorMessage="*" ControlToValidate="Password"></asp:RequiredFieldValidator>
		</td>
	</tr>
	<tr>
		<TD></TD>
		<td>
		<%--	<br>--%>
			<asp:TextBox id="Password" columns="9" width="200px" textmode="password" cssclass="safari-midtextbox"
				runat="server"></asp:TextBox>
		</td>
	</tr>
	<TR>
		<TD></TD>
		<TD style="display:none"><STRONG><SPAN class="bold">驗證碼</SPAN></STRONG> :
			<asp:TextBox id="txtValidate" runat="server" width="65px" cssclass="safari-midtextbox" columns="9" Text='<%# Request.Cookies["CheckCodeS"].Value %>' ></asp:TextBox>
			<IMG id='vimg' src="CheckCode.aspx" alt='"Click To Refresh"' OnClick="change()">
			<asp:RequiredFieldValidator id="RequiredFieldValidator3" runat="server"  Enabled="false" ControlToValidate="txtValidate" ErrorMessage="*"></asp:RequiredFieldValidator>
		</TD>
	</TR>
	<tr>
		<TD>
		</TD>
		<td><FONT face="新細明體"></FONT>
			<br>
			<asp:checkbox id="RememberCheckbox" class="Normal" Text="在這部電腦上記住" runat="server" /></td>
	</tr>
	<tr>
		<TD>
		</TD>
		<td>
			<br>
			<asp:Button id="BtnSignin" runat="server" Text="登入" Width="78"></asp:Button>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
			<INPUT id="btnCCannel" style="WIDTH:78px" onclick="Clear()" type="button" value="重置" name="btnCCannel">
			<br>
		</td>
	</tr>
	<TR>
		<TD></TD>
		<TD>
			<asp:label class="NormalRed" id="Message" runat="server" Width="280px" ForeColor="Red" Height="30px"></asp:label></TD>
	</TR>
	<TR>
		<TD></TD>
		<TD></TD>
	</TR>
</table>
