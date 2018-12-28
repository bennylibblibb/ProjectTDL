<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">
	string sAppID;
	void Page_Load(Object sender,EventArgs e) {
		sAppID = Request.QueryString["appID"];
	}

	void DeleteAction(Object sender,EventArgs e) {
		int iResult = 0;
		HKJCAdmin hkjc = new HKJCAdmin();

		try {
			iResult = hkjc.NotifyProcess("D", sAppID);
			if(iResult > 0) {
				rtnMsg.Text = "已刪除" + iResult + "種資訊";
			}	else if(iResult == 0) {
				rtnMsg.Text = "沒有選擇資訊刪除";
			}	else if(iResult == -99) {
				rtnMsg.Text = "沒有權限刪除資訊";
			}	else {
				rtnMsg.Text = "刪除資訊失敗，" + (string)Application["transErrorMsg"];
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
	<title>體育資訊 - 刪除馬會足球</title>
</head>
<body>
	<form id="HKJCDeleteForm" method="post" runat="server">
		<b><font color="#FF0000">刪除</font>馬會足球
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
				<td><input type="checkbox" name="delete" value="clr_tbl_odds">所有賠率</td>
				<td></td>
				<td></td>
				<td></td>
				<td></td>
			</tr>
			<tr>
				<td colspan="5"><asp:Label id="rtnMsg" runat="server" /></td>
			</tr>
			<tr>
				<td colspan="5">
					<input type="button" id="SendBtn" value="刪除" OnServerClick="DeleteAction" runat="server">&nbsp;
					<input type="reset" value="重設">
				</td>
			</tr>
		</table>
	</form>
</body>
</html>