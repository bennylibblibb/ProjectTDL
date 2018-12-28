<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">
	void Page_Load(Object sender,EventArgs e) {
		SportNewsMenu NewsMenu = new SportNewsMenu((string)Application["SoccerDBConnectionString"]);

		try {
			SportNewsInformation.InnerHtml = NewsMenu.GetMenu();
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
function goToNews(idx,selectedNews) {
	if(selectedNews!='' && selectedNews!='0') {
		parent.content_frame.location.replace(selectedNews);
	}
	SportNewsMenuForm.SportNewsID[idx].value = '0';
}
</script>

<script language="JavaScript">
function goToDisplayCfg(idx,selectedDisplay) {
	if(selectedDisplay!='' && selectedDisplay!='0') {
		parent.content_frame.location.replace(selectedDisplay);
	}
	SportNewsMenuForm.SportNewsID[idx].value = '0';
}
</script>

<html>
<head>
	<META http-equiv="Content-Type" content="text/html; charset=big5">
	<LINK REL="stylesheet" HREF="/sportStyle.css" TYPE="text/css">
</head>
<body>
	<form id="SportNewsMenuForm" method="post" runat="server">
		<asp:Label id="rtnMsg" runat="server" />
		<table width="100%">
			<span id="SportNewsInformation" runat="server" />
		</table>
	</form>
</body>
</html>