<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">
	void Page_Load(Object sender,EventArgs e) {
		PagerMenuMenu menu = new PagerMenuMenu((string)Application["SoccerDBConnectionString"]);
		try {
			PagerMenuInformation.InnerHtml = menu.ShowMenu();
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
function goToMenu(idx, url) {
	if(url!='' && url!='0') {
		parent.content_frame.location.replace(url);
	}

	PagerMenuForm.PagerMenu[idx].value = '0';
}
</script>

<html>
<head>
	<META http-equiv="Content-Type" content="text/html; charset=big5">
	<LINK REL="stylesheet" HREF="/sportStyle.css" TYPE="text/css">
</head>
<body>
	<form id="PagerMenuForm" method="post" runat="server">
		<asp:Label id="rtnMsg" runat="server" />
		<table width="100%">
			<span id="PagerMenuInformation" runat="server" />
		</table>
	</form>
</body>
</html>