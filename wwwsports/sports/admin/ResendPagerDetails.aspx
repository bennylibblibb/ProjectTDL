<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">
	void Page_Load(Object sender,EventArgs e) {
		ResendPagerDetails pagerItem = new ResendPagerDetails((string)Application["SoccerDBConnectionString"]);
		try {
			ResendItemsInformation.InnerHtml = pagerItem.ShowItems();
		} catch(NullReferenceException) {
			FormsAuthentication.SignOut();
			rtnMsg.Text = "<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>";
		}
	}

	void ResendAction(Object sender,EventArgs e) {
		int iResult = 0;
		ResendPagerDetails resendItem = new ResendPagerDetails((string)Application["SoccerDBConnectionString"]);

		try {
			iResult = resendItem.Resend();
			if(iResult > 0) {
				rtnMsg.Text = "已重發" + iResult + "種資訊";
			}	else if(iResult == 0) {
				rtnMsg.Text = "沒有選擇資訊重發";
			}	else if(iResult == -99) {
				rtnMsg.Text = "沒有權限重發資訊";
			}	else {
				rtnMsg.Text = "重發資訊失敗，" + (string)Application["transErrorMsg"];
			}
		} catch(NullReferenceException) {
			FormsAuthentication.SignOut();
			rtnMsg.Text = "<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>";
		}
	}
</script>

<html>
<head>
	<META http-equiv="Content-Type" content="text/html; charset=big5">
	<LINK REL="stylesheet" HREF="/sportStyle.css" TYPE="text/css">
	<title>體育資訊 - 重發傳呼機資訊</title>
</head>
<body>
	<form id="ResendPagerDetailsForm" method="post" runat="server">
		<b><font color="#FF69B4">只重發傳呼機資訊，網頁數據不會重發</font></b>
		<center>
		<table border="1" width="100%" style="font: 10pt verdana">
			<span id="ResendItemsInformation" runat="server" />

			<tr>
				<td colspan="5"><asp:Label id="rtnMsg" runat="server" /></td>
			</tr>
			<tr>
				<td colspan="5">
					<input type="button" id="SendBtn" value="傳送" OnServerClick="ResendAction" runat="server">&nbsp;
					<input type="reset" value="重設">
				</td>
			</tr>
		</table>
		</center>
	</form>
</body>
</html>