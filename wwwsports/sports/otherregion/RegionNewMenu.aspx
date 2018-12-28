<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">
	string sRegion;
	void Page_Load(Object sender,EventArgs e) {
		RegionNewMenu RegionLeagueMenu = new RegionNewMenu((string)Application["SoccerDBConnectionString"]);
		try {
			LeagueInformation.InnerHtml = RegionLeagueMenu.Show();
			sRegion = RegionLeagueMenu.Region;
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
function goToLeague(pageURL) {
	if(pageURL!='' && pageURL!='0') {
		parent.content_frame.location.replace(pageURL);
	}

	RegionMenuForm.regionMenuLeague.value = '0';
}
</script>

<html>
<head>
	<META http-equiv="Content-Type" content="text/html; charset=big5">
	<LINK REL="stylesheet" HREF="/sportStyle.css" TYPE="text/css">
</head>
<body>
	<form id="RegionMenuForm" method="post" runat="server">
	<center>
		<asp:Label id="rtnMsg" runat="server" />
		<table width="50%">
			<tr align="center">
				<th>新增賽事至<%=sRegion%>，請選擇賽事所屬聯賽:</th>
				<td>
					<select name="regionMenuLeague" onChange="goToLeague(RegionMenuForm.regionMenuLeague.value)">
						<option value="0">請選擇</option>
						<span id="LeagueInformation" runat="server" />
					</select>
				</td>
			</tr>
		</table>
	</center>
	</form>
</body>
</html>