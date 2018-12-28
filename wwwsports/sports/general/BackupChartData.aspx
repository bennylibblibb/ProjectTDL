<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">
	void BackupRestoreAction(Object sender,EventArgs e) {
		string sMode;
		BackupChartData chartBackup = new BackupChartData((string)Application["SoccerDBConnectionString"]);

		try {
			sMode = chartBackup.BackupRestore();
			if(sMode.Equals("1")) {
				rtnMsg.Text = "�Ϫ�ƾڤw�ƥ�";
			}	else if(sMode.Equals("2")) {
				rtnMsg.Text = "�Ϫ�ƾڤw�٭�";
			}	else if(sMode.Equals("0")) {
				rtnMsg.Text = "�S����ܳƥ�/�٭�Ϫ�ƾ�";
			}	else {
				rtnMsg.Text = "�ƥ�/�٭�Ϫ�ƾڥ��ѡA" + (string)Application["transErrorMsg"];
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
	<title>��|��T - �ƥ�/�٭�Ϫ�ƾ�</title>
</head>
<body>
	<form id="BackupRestoreForm" method="post" runat="server">
		<b><font color="#C71585">�b�R���ǩI���ƾڤαq��|��T�������o�ƾګ�A���٭�w�ƥ����Ϫ�ƾ�</font></b>
		<table border="1" width="50%" style="font: 10pt verdana">
			<tr>
				<td>
					<input type="radio" name="chartaction" value="1">�ƥ��Ϫ�ƾ�<br>
					<input type="radio" name="chartaction" value="2">�٭�Ϫ�ƾ�
				</td>
			</tr>
			<tr>
				<td><asp:Label id="rtnMsg" runat="server" /></td>
			</tr>
			<tr>
				<td>
					<input type="button" id="SendBtn" value="�ǰe" OnServerClick="BackupRestoreAction" runat="server">&nbsp;
					<input type="reset" value="���]">
				</td>
			</tr>
		</table>
	</form>
</body>
</html>