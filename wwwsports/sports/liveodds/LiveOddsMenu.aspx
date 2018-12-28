<html>
<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">
	void Page_Load(Object sender,EventArgs e) {
		LiveOddsMenu menu = new LiveOddsMenu((string)Application["SoccerDBConnectionString"]);

		try {
			LiveoddsNameInfo.InnerHtml = menu.GetName();
			LiveoddsLinkInfo.InnerHtml = menu.GetLink();
		}	catch(NullReferenceException) {
			FormsAuthentication.SignOut();
			UpdateReturnMessage("<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>");
		}
	}

	void UpdateReturnMessage(string sMsg) {
		rtnMsg.Text = sMsg;
	}
</script>

<script language="JavaScript">
function goToPage(idx,pageURL) {
	if(pageURL!='' && pageURL!='0') {
		parent.content_frame.location.replace(pageURL);
	}
	if (LiveoddsMenuForm.LiveoddsAction.length==1) LiveoddsMenuForm.LiveoddsAction.value = '0';
	else LiveoddsMenuForm.LiveoddsAction[idx].value = '0';
}
</script>

<head>
	<META http-equiv="Content-Type" content="text/html; charset=big5">
	<LINK REL="stylesheet" HREF="/sportStyle.css" TYPE="text/css">
	<title>體育資訊 - 現場賠率</title>
</head>
<body>
<form name="LiveoddsMenuForm" method="post">
	<asp:Label id="rtnMsg" runat="server" />
	<table width="100%">
		<tr>
			<th align="left">現場賠率</th>
			<td align="left"><a href="../index.htm" target="_top">返回主頁</a></td>
			<td><a href="LiveOddsOrder.aspx" target="content_frame">修改排序</a></td>
			<span id="LiveoddsNameInfo" runat="server" />
		</tr>
		<tr>
			<td></td>
			<td><a href="LiveOddsMenu.aspx" target="menu_frame">更新選單</a></td>
			<td></td>
			<span id="LiveoddsLinkInfo" runat="server" />
		</tr>
	</table>
</form>
</body>
</html>