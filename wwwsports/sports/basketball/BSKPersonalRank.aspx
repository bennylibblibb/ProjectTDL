<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">
	string m_JavaScript;

	void Page_Load(Object sender,EventArgs e) {
	string sRankPres = "平均";
		BSKPersonalRank AddRankPresonal = new BSKPersonalRank((string)Application["BasketballDBConnectionString"]);

		try {
			RankPresInformation.InnerHtml = AddRankPresonal.GetRankPres();
			rankname.Text = AddRankPresonal.SetRankName;
			rankpresonalname.Text = sRankPres + AddRankPresonal.SetRankName;
			m_JavaScript = AddRankPresonal.GetJavaScript();
			UpdateHistoryMessage((string)Application["retrieveInfoMsg"] + "個人統計(" + DateTime.Now.ToString("HH:mm:ss") + ")");
		} catch(NullReferenceException) {
			FormsAuthentication.SignOut();
			UpdateHistoryMessage("<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>");
		}
	}

	void onAddPresonalRank(Object sender,EventArgs e) {
		int iUpdated = 0;
		string sNow;
		BSKPersonalRank AddRankPresonal = new BSKPersonalRank((string)Application["BasketballDBConnectionString"]);
		sNow = DateTime.Now.ToString("HH:mm:ss");
		try {

			iUpdated = AddRankPresonal.AddRankPres();
			Page_Load(sender,e);
			if(iUpdated > 0) {
				UpdateHistoryMessage("成功改動" + iUpdated.ToString() + "個人統計(" + sNow + ")");
			} else {
				UpdateHistoryMessage("沒有改動(" + sNow + ")");
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
function  CheckValue() {
<%
	int iIndex;
	string sRankDataIndex,sTeamIndex,sPlayerIndex;
	for(iIndex=0;iIndex<3;iIndex++) {
		sRankDataIndex = "RANKDATA[" + iIndex.ToString() + "]";
		sTeamIndex = "Team[" + iIndex.ToString() + "]";
		sPlayerIndex= "player" + iIndex.ToString();
%>

	if(isNaN(RankLeagueForm.<%=sRankDataIndex%>.value)) {
		alert('數據只能為數字!');
		RankLeagueForm.<%=sRankDataIndex%>.value = '';
		return false;
	}

	if((RankLeagueForm.<%=sTeamIndex%>.value!="")&&(RankLeagueForm.<%=sPlayerIndex%>.value=="")) {
		alert('球隊被選,球員必須被選!');
		return false;
	}

	if((RankLeagueForm.<%=sTeamIndex%>.value!="")&&(RankLeagueForm.<%=sPlayerIndex%>.value=="暫無球員")) {
		alert('球隊被選,球員必須被選!');
		return false;
	}
<%
}
%>
}

<%=m_JavaScript%>

function emptyList(select) {
	while(select.options.length) select.options[0] = null;
}
function fillList( select, arr ) {
	for ( i = 0; i < arr.length; i++ ) {
		option = new Option( arr[i], arr[i] );
		select.options[select.length] = option;
	}
	select.selectedIndex=0;
}

function changeList0( select ) {
	list = lists[select.options[select.selectedIndex].value];
	emptyList( select.form.player0 );
	fillList( select.form.player0, list );
}
function changeList1( select ) {
	list = lists[select.options[select.selectedIndex].value];
	emptyList( select.form.player1 );
	fillList( select.form.player1, list );
}
function changeList2( select ) {
	list = lists[select.options[select.selectedIndex].value];
	emptyList( select.form.player2 );
	fillList( select.form.player2, list );
}
</script>
<html>
<head>
	<META http-equiv="Content-Type" content="text/html; charset=big5">
	<LINK REL="stylesheet" HREF="/sportStyle.css" TYPE="text/css">
</head>
<body>
	<form id="RankLeagueForm" method="post" runat="server" ONSUBMIT="return CheckValue()">
		<font size="2"><b>上次行動:</b><asp:Label id="historyMsg" runat="server" /></font><br>

		<table border="1" width="60%" style="font: 10pt verdana">
		<tr style="background-color:#77cc22">
		<th colspan=4 align="left"><font color=#aa4471>
		<asp:Label id="rankname" runat="server" />
		</font>榜</th>
		<tr>
			<th>名次</th>
			<th>球隊</th>
			<th>球員</th>
			<th>
				<asp:Label id="rankpresonalname" runat="server" />
			</th>
		</tr>
		<span id="RankPresInformation" runat="server" />
		<tr>
			<td colspan="4" align="right">
				<!--
					Value of SendToPager is Device ID defined in DEVICE_TYPE
				-->
				<input type="checkbox" name="SendToPager" value="1" checked>GOGO1&nbsp;
				<input type="checkbox" name="SendToPager" value="1" checked>GOGO2&nbsp;
				<input type="checkbox" name="SendToPager" value="3" checked>馬會機&nbsp;
				<!--
				<input type="checkbox" name="SendToPager" value="4" checked>JCCombo&nbsp;
				<input type="checkbox" name="SendToPager" value="5" checked>GOGO3Combo&nbsp;
				-->
				<input type="submit" id="SaveBtn" value="傳送" OnServerClick="onAddPresonalRank" runat="server">&nbsp;
				<input type="reset" value="重設">
			</td>
		</table>
	</form>
</body>
</html>