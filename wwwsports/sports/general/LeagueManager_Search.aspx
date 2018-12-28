<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">
	string sAction;

	void Page_Load(Object sender,EventArgs e) {
		sAction = (string)Request.QueryString["action"];
		string sKeyParam = (string)Request.QueryString["keyParam"];
		if(sKeyParam != null) {
			if(!sKeyParam.Trim().Equals("")) {
				SearchLeague sl = new SearchLeague((string)Application["SoccerDBConnectionString"]);
				try {
					SearchLeagueInformation.InnerHtml = sl.ShowLeagues(sKeyParam);
				} catch(NullReferenceException) {
					FormsAuthentication.SignOut();
					UpdateReturnMessage("<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>");
				}
			}
		}
	}

	void SearchLeague(Object sender,EventArgs e) {
		SearchLeague query = new SearchLeague((string)Application["SoccerDBConnectionString"]);
		try {
			SearchLeagueInformation.InnerHtml = query.ShowLeagues();
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
function setFocus() {
	window.opener.focus();
}
</script>

<html>
<head>
	<META http-equiv="Content-Type" content="text/html; charset=big5">
	<LINK REL="stylesheet" HREF="/sportStyle.css" TYPE="text/css">
	<title>聯賽搜尋</title>
</head>
<body>
	<form id="SearchLeagueForm" method="post" runat="server">
		<b>請輸入關鍵詞查詢：</b><input name="keyword" maxlength="10" size="10">
		<input type="submit" id="searchBtn" value="查詢" OnServerClick="SearchLeague" runat="server">&nbsp;
		<input type="checkbox" name="searchType" value="A" checked>亞洲&nbsp;
		<input type="checkbox" name="searchType" value="J" checked>馬會&nbsp;
		<input type="checkbox" name="searchType" value="M" checked>澳門
		<input type="hidden" name="action" value="<%=sAction%>">
		<asp:Label id="rtnMsg" runat="server" />
		<table border="1" width="100%">
			<tr style="background-color:#6FAFB1; color=#F0F8FF">
				<th>亞洲名稱</th>
				<th>亞洲簡稱</th>
				<th>馬會名稱</th>
				<th>馬會簡稱</th>
				<th>澳門名稱</th>
			</tr>
			<span id="SearchLeagueInformation" runat="server" />
		</table>

	</form>
</body>
</html>