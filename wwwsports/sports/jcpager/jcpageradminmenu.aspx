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

	//JCPagerAdminMenuForm.JCPagerResend.value = '0';
	//JCPagerAdminMenuForm.JCPagerDelete.value = '0';
	JCPagerAdminMenuForm.JCPagerReload.value = '0';
}
</script>

<html>
<head>
	<META http-equiv="Content-Type" content="text/html; charset=big5">
	<LINK REL="stylesheet" HREF="/sportStyle.css" TYPE="text/css">
	<title>��|��T - ���|���]�w</title>
</head>
<body>
	<form id="JCPagerAdminMenuForm" method="post" runat="server">
		<asp:Label id="rtnMsg" runat="server" />
		<table width="100%">
			<tr align="left">
				<th>���|���]�w</th>
				<td><a href="../index.htm" target="_top">��^�D��</a></td>
				<td><a href="JCMatchLeakage.aspx" target="content_frame">�˵���|�ɨ�</a></td>
				<td><a href="HKJCSchedule.aspx" target="content_frame">�]�w�ɵ{</a></td>
				<td><a href="HKJCModifyName.aspx" target="content_frame">���|���ɨ�</a></td>
				<%
					if(AllowDisplay) {
				%>
<!--
				<td>���o��T</td>
				<td>�R����T</td>
-->
				<td>���s���J</td>

				<%
					}
				%>
			</tr>
			<tr align="left">
				<td colspan="4"></td>
				<%
					if(AllowDisplay) {
				%>
<!--
				<td>
					<select name="JCPagerResend" onChange="goToURL(JCPagerAdminMenuForm.JCPagerResend.value)">
						<option value="0">�п��</option>
						<option value="../admin/ResendHKJC.aspx?appID=09">���|WAP</option>
					</select>
				</td>
				<td>
					<select name="JCPagerDelete" onChange="goToURL(JCPagerAdminMenuForm.JCPagerDelete.value)">
						<option value="0">�п��</option>
						<option value="../admin/DeleteHKJC.aspx?appID=09">���|WAP</option>
					</select>
				</td>
-->
				<td></td>
				<td>
					<select name="JCPagerReload" onChange="goToURL(JCPagerAdminMenuForm.JCPagerReload.value)">
						<option value="0">�п��</option>
						<option value="ReloadHKJC.aspx?appID=08">���|��</option>
<!--
						<option value="ReloadHKJC.aspx?appID=09">���|WAP</option>
-->
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