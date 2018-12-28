<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">
	string sAppID;
	void Page_Load(Object sender,EventArgs e) {
		sAppID = Request.QueryString["appID"];
	}

	void SendAction(Object sender,EventArgs e) {
		int iResult = 0;
		HKJCAdmin hkjc = new HKJCAdmin();

		try {
			iResult = hkjc.NotifyProcess("L", sAppID);
			if(iResult > 0) {
				rtnMsg.Text = "已重新載入" + iResult + "種資訊";
			}	else if(iResult == 0) {
				rtnMsg.Text = "沒有選擇資訊重新載入";
			}	else if(iResult == -99) {
				rtnMsg.Text = "沒有權限重新載入資訊";
			}	else {
				rtnMsg.Text = "重新載入資訊失敗，" + (string)Application["transErrorMsg"];
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
	<title>體育資訊 - 重載馬會足球</title>
</head>
<body>
	<form id="HKJCReloadForm" method="post" runat="server">
		<b><font color="#32CD32">重新載入</font>馬會足球
		<%
			if(sAppID.Equals("05")) {
		%>
			(足球2 -> 本地)
		<%
			} else if(sAppID.Equals("08")) {
		%>
			(馬會機)
		<%
			} else {
		%>
			(馬會WAP)
		<%
			}
		%>
		資訊</b>
		<table border="1" width="100%" style="font: 10pt verdana">
			<tr>
				<!--
				<td><input type="checkbox" name="reload" value="reload_to_pager">馬會機數據庫及傳送到馬會機[ID:98]</td>
				<td></td>
				-->
				<!--
				<td><input type="checkbox" name="reload" value="reload_to_db2">馬會足球數據庫</td>
				-->
				<td><input type="checkbox" name="reload" value="sync_team">隊伍名稱[ID:96]</td>
				<td></td>
				<td></td>
			</tr>
			<tr>
				<td colspan="5"><asp:Label id="rtnMsg" runat="server" /></td>
			</tr>
			<tr>
				<td colspan="5">
					<input type="button" id="SendBtn" value="重新載入" OnServerClick="SendAction" runat="server">&nbsp;
					<input type="reset" value="重設">
				</td>
			</tr>
		</table>
	</form>
</body>
</html>