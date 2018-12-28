<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">
	string sAction;

	void Page_Load(Object sender,EventArgs e) {
		sAction = (string)Request.QueryString["action"];
		string sKeyParam = (string)Request.QueryString["keyParam"];
		if(sKeyParam != null) {
			if(!sKeyParam.Trim().Equals("")) {
				SearchTeam st = new SearchTeam((string)Application["SoccerDBConnectionString"]);
				try {
					SearchTeamInformation.InnerHtml = st.ShowTeams(sKeyParam);
				} catch(NullReferenceException) {
					FormsAuthentication.SignOut();
					UpdateReturnMessage("<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>");
				}
			}
		}
	}

	void SearchTeam(Object sender,EventArgs e) {
		SearchTeam query = new SearchTeam((string)Application["SoccerDBConnectionString"]);
		try {
			SearchTeamInformation.InnerHtml = query.ShowTeams();
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
	<title>����j�M</title>
</head>
<body>
	<form id="SearchTeamForm" method="post" runat="server">
		<b>�п�J������d�ߡG</b><input name="keyword" maxlength="10" size="10">
		<input type="submit" id="searchBtn" value="�d��" OnServerClick="SearchTeam" runat="server">&nbsp;
		<input type="checkbox" name="searchType" value="A" checked>�Ȭw&nbsp;
		<input type="checkbox" name="searchType" value="J" checked>���|&nbsp;
		<input type="checkbox" name="searchType" value="M" checked>�D��
		<input type="hidden" name="action" value="<%=sAction%>">
		<table border="1" width="100%">
			<tr style="background-color:#FFA07A; color=#FFFAF0">
				<th>�Ȭw�W��</th>
				<th>���|�W��</th>
				<th>���|²��</th>
				<th>�D���W��</th>
			</tr>

			<span id="SearchTeamInformation" runat="server" />
		</table>
		<asp:Label id="rtnMsg" runat="server" />
	</form>
</body>
</html>