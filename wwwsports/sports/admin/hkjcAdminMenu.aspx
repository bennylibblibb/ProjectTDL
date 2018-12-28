<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">
	bool AllowDisplay;

	void Page_Load(Object sender,EventArgs e) {
		string sRole = "";
		AllowDisplay = false;
		try {
			sRole = (string)Session["user_role"];
			if(Convert.ToInt32(sRole) >= 11) {
				AllowDisplay = true;
			}
		} catch(NullReferenceException) {
			FormsAuthentication.SignOut();
			UpdateReturnMessage("<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>");
		}
	}

	void UpdateReturnMessage(string sMsg) {
		rtnMsg.Text = sMsg;
	}
</script>

<script language="JavaScript">
function goToURL(url) {
	if(url!='' && url!='0') {
		parent.content_frame.location.replace(url);
	}

	HKJCAdminMenuForm.HKJCResend.value = '0';
	HKJCAdminMenuForm.HKJCDelete.value = '0';
}
</script>

<html>
<head>
	<META http-equiv="Content-Type" content="text/html; charset=big5">
	<LINK REL="stylesheet" HREF="/sportStyle.css" TYPE="text/css">
	<title>��|��T - ���a�]�w</title>
</head>
<body>
	<form id="HKJCAdminMenuForm" method="post" runat="server">
		<asp:Label id="rtnMsg" runat="server" />
		<table width="80%">
			<tr align="left">
				<th>���a�]�w</th>
				<td><a href="../index.htm" target="_top">��^�D��</a></td>
				<td><a href="HKJCSchedule.aspx" target="content_frame">�]�w�ɵ{</a></td>
				<%
					if(AllowDisplay) {
				%>
				<td>���o��T</td>
				<td>�R����T</td>
				<%
					}
				%>
			</tr>
			<tr align="left">
				<td colspan="3"></td>
				<%
					if(AllowDisplay) {
				%>
				<td>
					<select name="HKJCResend" onChange="goToURL(HKJCAdminMenuForm.HKJCResend.value)">
						<option value="0">�п��</option>
						<option value="ResendHKJC.aspx?appID=05">���y2 -> ���a</option>
					</select>
				</td>
				<td>
					<select name="HKJCDelete" onChange="goToURL(HKJCAdminMenuForm.HKJCDelete.value)">
						<option value="0">�п��</option>
						<option value="DeleteHKJC.aspx?appID=05">���y2 -> ���a</option>
					</select>
				</td>
				<%
					}
				%>
			</tr>
		</table>
	</form>
</body>
</html>