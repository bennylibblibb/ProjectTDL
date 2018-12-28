<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">
	string sRole;

	void Page_Load(Object sender,EventArgs e) {
		try {
			sRole = (string)Session["user_role"];
		} catch(NullReferenceException) {
			FormsAuthentication.SignOut();
			UpdateReturnMessage("<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>");
		}
	}

	void UpdateReturnMessage(string sMsg) {
		rtnMsg.Text = sMsg;
	}
</script>

<html>
<head>
	<META http-equiv="Content-Type" content="text/html; charset=big5">
	<LINK REL="stylesheet" HREF="/sportStyle.css" TYPE="text/css">
</head>
<body>
	<form id="UserMgrMenuForm" method="post" runat="server">
		<asp:Label id="rtnMsg" runat="server" />
		<table width="50%">
			<tr align="left">
				<th>�ϥΪ̳]�w</th>
				<td><a href="../index.htm" target="_top">��^�D��</a></td>
				<td><a href="UserManager.aspx?act=M" target="content_frame">�ק�K�X</a></td>
				<%
					if(Convert.ToInt32(sRole) >= 999) {
				%>
					<td><a href="UserManager.aspx?act=A" target="content_frame">�s�W�ϥΪ�</a></td>
					<td><a href="UserManager.aspx?act=D" target="content_frame">�R���ϥΪ�</a></td>
				<%
					}
				%>
			</tr>
		</table>
	</form>
</body>
</html>