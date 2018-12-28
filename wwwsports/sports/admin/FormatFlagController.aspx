<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">
	void ChangeFlag(Object sender,EventArgs e) {
		string sLVPLFlag = (string)HttpContext.Current.Request.Form["lvplFlag"];

		try {
			if(sLVPLFlag.Equals("1")) {
				Application["EnableFormat_LVPL"] = true;
				rtnMsg.Text = "�ɰ�����]�w��web format";
			} else {
				Application["EnableFormat_LVPL"] = false;
				rtnMsg.Text = "�ɰ�����]�w��sender format";
			}
		} catch(NullReferenceException) {
			FormsAuthentication.SignOut();
			rtnMsg.Text = "<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>";
		}
	}
</script>

<html>
<head>
	<META http-equiv="Content-Type" content="text/html; charset=big5">
	<LINK REL="stylesheet" HREF="/sportStyle.css" TYPE="text/css">
	<title>��|��T - �o�e�~�|</title>
</head>
<body>
	<center>
	<form id="FormatControllerForm" method="post" runat="server">
		<table border="1" width="100%" style="font: 10pt verdana">
		<%
			if(Convert.ToInt32(Session["user_role"]) >= 988) {
		%>
			<tr style="background: #FFF0F5" align="center">
				<th>
					�T�����O
				</th>
				<th>
					Format by Web
				</th>
				<th>
					Format by Sender
				</th>
			</tr>

			<tr align="center">
				<td>
					�ɰ�����
				</td>
				<td>
					<input type="radio" name="lvplFlag" value="1" <%if((bool)Application["EnableFormat_LVPL"]) {%>checked<%}%>>
				</td>
				<td>
					<input type="radio" name="lvplFlag" value="2" <%if(!(bool)Application["EnableFormat_LVPL"]) {%>checked<%}%>>
				</td>
			</tr>

			<tr>
				<td colspan="3" align="right">
					<input type="button" id="ModifyBtn" value="�ק�" OnServerClick="ChangeFlag" runat="server">&nbsp;
					<input type="reset" value="���]">
				</td>
			</tr>
			<tr>
				<th colspan="3">
					<asp:Label id="rtnMsg" runat="server" />
				</th>
			</tr>
		<%
			} else {
		%>
			<tr>
				<th colspan="3" align="center">
					�S���ק��v���I
				</th>
			</tr>
		<%
			}
		%>
		</table>
		</form>
	</center>
</body>
</html>