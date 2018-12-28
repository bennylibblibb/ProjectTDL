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
	<title>�p�ɷj�M</title>
</head>
<body>
	<form id="SearchLeagueForm" method="post" runat="server">
		<b>�п�J������d�ߡG</b><input name="keyword" maxlength="10" size="10">
		<input type="submit" id="searchBtn" value="�d��" OnServerClick="SearchLeague" runat="server">&nbsp;
		<input type="checkbox" name="searchType" value="A" checked>�Ȭw&nbsp;
		<input type="checkbox" name="searchType" value="J" checked>���|&nbsp;
		<input type="checkbox" name="searchType" value="M" checked>�D��
		<input type="hidden" name="action" value="<%=sAction%>">
		<asp:Label id="rtnMsg" runat="server" />
		<table border="1" width="100%">
			<tr style="background-color:#6FAFB1; color=#F0F8FF">
				<th>�Ȭw�W��</th>
				<th>�Ȭw²��</th>
				<th>���|�W��</th>
				<th>���|²��</th>
				<th>�D���W��</th>
			</tr>
			<span id="SearchLeagueInformation" runat="server" />
		</table>

	</form>
</body>
</html>