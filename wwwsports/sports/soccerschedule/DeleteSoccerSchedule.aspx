<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">
	void Page_Load(Object sender,EventArgs e) {
		DeleteSoccerSchedule match = new DeleteSoccerSchedule((string)Application["SoccerDBConnectionString"]);

		try {
			ScheduleInformation.InnerHtml = match.getMatch();
			UpdateHistoryMessage((string)Application["retrieveInfoMsg"] + "賽程刪除(" + DateTime.Now.ToString("HH:mm:ss") + ")");
		} catch(NullReferenceException) {
			FormsAuthentication.SignOut();
			UpdateHistoryMessage("<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>");
		}
	}

	void DeleteScheduleAction(Object sender,EventArgs e) {
		int iUpdated = 0;
		string sNow;
		DeleteSoccerSchedule schedule = new DeleteSoccerSchedule((string)Application["SoccerDBConnectionString"]);

		sNow = DateTime.Now.ToString("HH:mm:ss");
		try {
			iUpdated = schedule.Delete();
			if(iUpdated > 0) {
				UpdateHistoryMessage("成功刪除賽程(" + sNow + ")");
			}	else if(iUpdated == 0) {
				UpdateHistoryMessage("沒有賽程被刪除(" + sNow + ")");
			}	else if(iUpdated == -99) {
				UpdateHistoryMessage("賽程已存在，沒有刪除(" + sNow + ")");
			}	else {
				UpdateHistoryMessage((string)Application["transErrorMsg"] + "(" + sNow + ")");
			}
			Response.Redirect("ListSoccerSchedule.aspx?leagID=0000");
		}	catch(NullReferenceException nullex) {
			FormsAuthentication.SignOut();
			UpdateHistoryMessage("<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>");
		}
	}

	void BackToSchedule(Object sender,EventArgs e) {
		Response.Redirect("ListSoccerSchedule.aspx?leagID=0000");
	}

	void UpdateHistoryMessage(string sMsg) {
		historyMsg.Text = sMsg;
	}
</script>

<html>
<head>
	<META http-equiv="Content-Type" content="text/html; charset=big5">
	<LINK REL="stylesheet" HREF="/sportStyle.css" TYPE="text/css">
</head>
<body>
	<form id="DeleteScheduleForm" method="post" runat="server">
		<h2><font color="red">你確定刪除下列賽程嗎？</font></h2>
		<font size="2"><b>上次行動:</b><asp:Label id="historyMsg" runat="server" /></font><br>

		<table border="1" width="60%">
			<span id="ScheduleInformation" runat="server" />
			<tr>
				<td align="right">
					<input type="submit" id="DeleteBtn" value="刪除" OnServerClick="DeleteScheduleAction" runat="server">&nbsp;
					<input type="submit" id="BackBtn" value="返回" OnServerClick="BackToSchedule" runat="server">
				</td>
			</tr>
	</form>
</body>
</html>