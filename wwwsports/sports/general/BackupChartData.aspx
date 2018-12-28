<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">
	void BackupRestoreAction(Object sender,EventArgs e) {
		string sMode;
		BackupChartData chartBackup = new BackupChartData((string)Application["SoccerDBConnectionString"]);

		try {
			sMode = chartBackup.BackupRestore();
			if(sMode.Equals("1")) {
				rtnMsg.Text = "圖表數據已備份";
			}	else if(sMode.Equals("2")) {
				rtnMsg.Text = "圖表數據已還原";
			}	else if(sMode.Equals("0")) {
				rtnMsg.Text = "沒有選擇備份/還原圖表數據";
			}	else {
				rtnMsg.Text = "備份/還原圖表數據失敗，" + (string)Application["transErrorMsg"];
			}
		}	catch(NullReferenceException) {
			FormsAuthentication.SignOut();
			rtnMsg.Text = "<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>";
		}
	}
</script>

<html>
<head>
	<META http-equiv="Content-Type" content="text/html; charset=big5">
	<LINK REL="stylesheet" HREF="/sportStyle.css" TYPE="text/css">
	<title>體育資訊 - 備份/還原圖表數據</title>
</head>
<body>
	<form id="BackupRestoreForm" method="post" runat="server">
		<b><font color="#C71585">在刪除傳呼機數據及從體育資訊網頁重發數據後，請還原已備份的圖表數據</font></b>
		<table border="1" width="50%" style="font: 10pt verdana">
			<tr>
				<td>
					<input type="radio" name="chartaction" value="1">備份圖表數據<br>
					<input type="radio" name="chartaction" value="2">還原圖表數據
				</td>
			</tr>
			<tr>
				<td><asp:Label id="rtnMsg" runat="server" /></td>
			</tr>
			<tr>
				<td>
					<input type="button" id="SendBtn" value="傳送" OnServerClick="BackupRestoreAction" runat="server">&nbsp;
					<input type="reset" value="重設">
				</td>
			</tr>
		</table>
	</form>
</body>
</html>