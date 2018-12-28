<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">
	void Page_Load(Object sender,EventArgs e) {
		PagerMenu menu = new PagerMenu((string)Application["SoccerDBConnectionString"]);
		try {
			MenuInformation.InnerHtml = menu.GetMenuItems();
		}	catch(NullReferenceException) {
			FormsAuthentication.SignOut();
			rtnMsg.Text = "<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>";
		}
	}

	void SendMenuAction(Object sender,EventArgs e) {
		int iResult = 0;
		PagerMenu sendMenu = new PagerMenu((string)Application["SoccerDBConnectionString"]);

		try {
			iResult = sendMenu.Resend();
			Page_Load(sender,e);
			if(iResult > 0) {
				rtnMsg.Text = "已發送菜單";
			}	else if(iResult == 0) {
				rtnMsg.Text = "沒有選擇菜單作重發";
			}	else if(iResult == -99) {
				rtnMsg.Text = "沒有權限重發菜單";
			}	else {
				rtnMsg.Text = "重發菜單失敗";
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
	<title>體育資訊 - 重發菜單</title>
</head>
<body>
	<center>
	<b><font color="red">發送前請注意系統狀況</font></b>
	<form id="ResendMenuForm" method="post" runat="server">
		<table border="1" width="100%" style="font: 10pt verdana">
			<span id="MenuInformation" runat="server" />
			<tr>
				<td colspan="2">
					<input type="button" id="SendBtn" value="傳送" OnServerClick="SendMenuAction" runat="server">&nbsp;
					<input type="reset" value="重設">
				</td>
			</tr>
		</table>
		<asp:Label id="rtnMsg" runat="server" />
		</form>
	</center>
</body>
</html>