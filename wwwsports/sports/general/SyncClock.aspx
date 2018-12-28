<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">
	void OnSyncClock(Object sender,EventArgs e) {
		int iUpdated = 0;
		string sNow;
		GOGO2Chart chart = new GOGO2Chart();
		sNow = DateTime.Now.ToString("HH:mm:ss");
		try {
			if(chart.SyncClock()) {
				UpdateReturnMessage("���\�շǮ���(" + sNow + ")");
			}	else {
				UpdateReturnMessage((string)Application["transErrorMsg"] + "(" + sNow + ")");
			}
		} catch(NullReferenceException nullex) {
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
	<center>
	<form id="SyncClockForm" method="post" runat="server">
		<h2>�ծնǩI���������P���A���P�B�A�T�w�շǮ����ܡH</h2>
		<input type="hidden" name="handlecode" value="sync_clock">
		<input type="submit" id="updBtn" value="�շ�" OnServerClick="OnSyncClock" runat="server"><br>
		<asp:Label id="rtnMsg" runat="server" />
	</form>
	</center>
</body>
</html>