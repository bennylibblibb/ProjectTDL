<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">
	void Page_Load(Object sender,EventArgs e) {
		DeleteAllData pagerItem = new DeleteAllData((string)Application["SoccerDBConnectionString"]);
		try {
			DeleteAllInformation.InnerHtml = pagerItem.ShowItems();
		} catch(NullReferenceException) {
			FormsAuthentication.SignOut();
			rtnMsg.Text = "<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>";
		}
	}

	void DeleteAllAction(Object sender,EventArgs e) {
		int iResult = 0;
		DeleteAllData allDelete = new DeleteAllData((string)Application["SoccerDBConnectionString"]);

		try {
			iResult = allDelete.Delete();
			Page_Load(sender,e);
			if(iResult > 0) {
				rtnMsg.Text = "已刪除" + iResult + "種資訊";
			}	else if(iResult == 0) {
				rtnMsg.Text = "沒有選擇資訊刪除";
			}	else if(iResult == -99) {
				rtnMsg.Text = "沒有權限刪除資訊";
			} else {
				rtnMsg.Text = "刪除資訊失敗，" + (string)Application["transErrorMsg"];
			}
		}	catch(NullReferenceException) {
			FormsAuthentication.SignOut();
			rtnMsg.Text = "<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>";
		}
	}
</script>

<html>
<head>
	<META http-equiv="Content-Type" content="text/html; charset=big5">
	<LINK REL="stylesheet" HREF="/sportStyle.css" TYPE="text/css">
	<title>體育資訊 - 刪除傳呼機及網頁資訊</title>
</head>
<body>
	<form id="DeleteAllDataForm" method="post" runat="server">
		<b><font color="#F3A0A0">刪除所有傳呼機及網頁資訊，<font color="#FF0000">相關數據將不能還原！</font></font></b>
		<center>
		<table border="1" width="100%" style="font: 10pt verdana">
			<span id="DeleteAllInformation" runat="server" />

			<tr>
				<td colspan="5"><asp:Label id="rtnMsg" runat="server" /></td>
			</tr>
			<tr>
				<td colspan="5">
					<input type="button" id="SendBtn" value="傳送" OnServerClick="DeleteAllAction" runat="server">&nbsp;
					<input type="reset" value="重設">
				</td>
			</tr>
		</table>
		</center>
	</form>
</body>
</html>