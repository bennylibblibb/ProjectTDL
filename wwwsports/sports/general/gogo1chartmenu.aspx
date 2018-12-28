<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">
	void Page_Load(Object sender,EventArgs e) {
		SoccerMenuAnalysis MenuAnalysis = new SoccerMenuAnalysis((string)Application["SoccerDBConnectionString"]);
		try {
			chatMatchInformation.InnerHtml = MenuAnalysis.Show();
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
function goToMatch(selectedMatch) {
	if(selectedMatch!='' && selectedMatch!='0') {
		parent.content_frame.location.replace('Gogo1ChartData.aspx?matchcount=' + selectedMatch);
	}

	ChartMenuForm.chartMatch.value = '0';
}
</script>

<html>
<head>
	<META http-equiv="Content-Type" content="text/html; charset=big5">
	<LINK REL="stylesheet" HREF="/sportStyle.css" TYPE="text/css">
	<title>��|��T - ��1�NGOGO�����ƹϪ�</title>
</head>
<body>
	<form id="ChartMenuForm" method="post" runat="server">
		<asp:Label id="rtnMsg" runat="server" />
		<table width="50%">
			<tr>
				<th>GOGO1 ���ƹϪ�</th>
				<td><a href="../index.htm" target="_top">��^�D��</a></td>
				<td>�Ϫ�ƾ�</td>
				<td><a href="BackupChartData.aspx" target="content_frame">�ƥ��٭�</a></td>
			</tr>
			<tr align="left">
					<td></td>
					<td><a href="redirectToGoGo1Menu.htm" target="menu_frame">��s���</a></td>
					<td>
						<select name="chartMatch" onChange="goToMatch(ChartMenuForm.chartMatch.value)">
							<option value="0">�п��</option>
							<span id="chatMatchInformation" runat="server" />
						</select>
					</td>
					<td></td>
				</tr>
		</table>
	</form>
</body>
</html>