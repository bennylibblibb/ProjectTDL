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
				<th>使用者設定</th>
				<td><a href="../index.htm" target="_top">返回主頁</a></td>
				<td><a href="UserManager.aspx?act=M" target="content_frame">修改密碼</a></td>
				<%
					if(Convert.ToInt32(sRole) >= 999) {
				%>
					<td><a href="UserManager.aspx?act=A" target="content_frame">新增使用者</a></td>
					<td><a href="UserManager.aspx?act=D" target="content_frame">刪除使用者</a></td>
				<%
					}
				%>
			</tr>
		</table>
	</form>
</body>
</html>