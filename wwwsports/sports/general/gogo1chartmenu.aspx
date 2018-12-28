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
	<title>體育資訊 - 第1代GOGO機指數圖表</title>
</head>
<body>
	<form id="ChartMenuForm" method="post" runat="server">
		<asp:Label id="rtnMsg" runat="server" />
		<table width="50%">
			<tr>
				<th>GOGO1 指數圖表</th>
				<td><a href="../index.htm" target="_top">返回主頁</a></td>
				<td>圖表數據</td>
				<td><a href="BackupChartData.aspx" target="content_frame">備份還原</a></td>
			</tr>
			<tr align="left">
					<td></td>
					<td><a href="redirectToGoGo1Menu.htm" target="menu_frame">更新選單</a></td>
					<td>
						<select name="chartMatch" onChange="goToMatch(ChartMenuForm.chartMatch.value)">
							<option value="0">請選擇</option>
							<span id="chatMatchInformation" runat="server" />
						</select>
					</td>
					<td></td>
				</tr>
		</table>
	</form>
</body>
</html>