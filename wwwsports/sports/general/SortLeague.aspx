<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">
	int iRecCount;

	void Page_Load(Object sender,EventArgs e) {
		LeagueSort leagueOrder = new LeagueSort((string)Application["SoccerDBConnectionString"]);
		try {
			LeagueInformation.InnerHtml = leagueOrder.GetLeagues();
			iRecCount = leagueOrder.NumberOfRecords;
			UpdateHistoryMessage((string)Application["retrieveInfoMsg"] + "聯賽序號(" + DateTime.Now.ToString("HH:mm:ss") + ")");
		} catch(NullReferenceException) {
			FormsAuthentication.SignOut();
			UpdateHistoryMessage("<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>");
		}
	}
	
	void UpdateLeagueSort(Object sender,EventArgs e) {
		int iUpdated = 0;
		LeagueSort leagueOrder = new LeagueSort((string)Application["SoccerDBConnectionString"]);

		try {
			iUpdated = leagueOrder.Sort();
			Page_Load(sender,e);
			if(iUpdated != -1) {
				UpdateHistoryMessage("更新聯賽序號(" + DateTime.Now.ToString("HH:mm:ss") + ")");
			}	else {
				UpdateHistoryMessage((string)Application["transErrorMsg"] + "(" + DateTime.Now.ToString("HH:mm:ss") + ")");
			}
		} catch(NullReferenceException nullex) {
			FormsAuthentication.SignOut();
			UpdateHistoryMessage("<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>");
		}
	}

	void UpdateHistoryMessage(string sMsg) {
		historyMsg.Text = sMsg;
	}
</script>

<script language="JavaScript">
function OrderValidator(validate_index) {
	order_re = /^\d{0,2}$/
	re_val = SortLeagueForm.leag_order[validate_index].value.search(order_re)
	if(re_val == -1) {
		alert('序號只接受數字');
		SortLeagueForm.leag_order[validate_index].value = '';
	}
}
</script>

<html>
<head>
	<META http-equiv="Content-Type" content="text/html; charset=big5">
	<LINK REL="stylesheet" HREF="/sportStyle.css" TYPE="text/css">
</head>
<body>
	<form id="SortLeagueForm" method="post" runat="server">
	<center>
		<font size="2"><b>上次行動:</b><asp:Label id="historyMsg" runat="server" /></font><br>

		<table border="1" width="30%" style="font: 10pt verdana">
		<tr align="center" style="background-color:#00bfff">
			<th>
				聯賽序號
			</th>
			<th>
				聯賽簡稱
			</th>
			<th>
				聯賽全名
			</th>
		</tr>

		<span id="LeagueInformation" runat="server" />

		<tr>
			<td colspan="3">
				<input type="submit" id="SortBtn" value="儲存" OnServerClick="UpdateLeagueSort" runat="server">
			</td>
		</tr>
		</table>
		<input type="hidden" name="RecordCount" value="<%=iRecCount.ToString()%>">
		</center>
	</form>
</body>
</html>