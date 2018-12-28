<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">
	void SendAlert(Object sender,EventArgs e) {
		int iResult = 0;
		AlertMessage alert = new AlertMessage((string)Application["SoccerDBConnectionString"]);

		try {
			iResult = alert.Send();
			if(iResult > 0) {
				rtnMsg.Text = "即時訊息已發送";
			}	else if(iResult == 0) {
				rtnMsg.Text = "沒有即時訊息可發送";
			} else {
				rtnMsg.Text = "發送即時訊息失敗，" + (string)Application["transErrorMsg"];
			}
		}	catch(NullReferenceException) {
			FormsAuthentication.SignOut();
			rtnMsg.Text = "<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>";
		}
	}
</script>

<script language="JavaScript">
function checkPagers() {
	if(document.AlertMessageForm.SendToPager[0].checked == false && document.AlertMessageForm.SendToPager[1].checked == false && document.AlertMessageForm.SendToPager[2].checked == false && document.AlertMessageForm.SendToPager[3].checked == false && document.AlertMessageForm.SendToPager[4].checked == false) {
		alert('請選擇發送目標!');
		return false;
	} else if (document.AlertMessageForm.SendToPager[2].checked == true || document.AlertMessageForm.SendToPager[3].checked == true || document.AlertMessageForm.SendToPager[4].checked == true) {
		var question = confirm('是否確定發送的資料相關於馬會賽事？')
		if (question == true)
			return true;
		else
			return false;
	} else {
		return true;
	}
}
</script>

<html>
<head>
	<META http-equiv="Content-Type" content="text/html; charset=big5">
	<LINK REL="stylesheet" HREF="/sportStyle.css" TYPE="text/css">
	<title>體育資訊 - 即時訊息</title>
</head>
<body>
	<form id="AlertMessageForm" method="post" Onsubmit="return checkPagers()" runat="server">
		<b><font color="#8B4513">如使用者在傳呼機的足球1有選擇總訊息提示，他們便會收到即時訊息</font></font></b>
		<table border="1" width="50%" style="font: 10pt verdana">
			<tr>
				<td align="center">
					<textarea name="alertMsg" rows=6 cols=40></textarea>
				</td>
			</tr>
			<tr>
				<td><asp:Label id="rtnMsg" runat="server" /></td>
			</tr>
			<tr align="right">
				<td>
					<!--
						Value of SendToPager is Path ID
						1: GOGO1 Sender1
						2: GOGO2 Handler1
						5: HKJC Handler1
						11: JCCombo MISC
						20: GOGO3Combo MISC
					-->
					<input type="checkbox" name="SendToPager" value="1">GOGO1&nbsp;
					<input type="checkbox" name="SendToPager" value="2">GOGO2&nbsp;
					<input type="checkbox" name="SendToPager" value="5">馬會機&nbsp;
					<input type="checkbox" name="SendToPager" value="11">JCCombo&nbsp;
					<input type="checkbox" name="SendToPager" value="20">GOGO3Combo&nbsp;
				</td>
			</tr>
			<tr>
				<td align="right">
					<input type="submit" id="SendBtn" value="傳送" OnServerClick="SendAlert" runat="server">&nbsp;
					<input type="reset" value="重設">
				</td>
			</tr>
		</table>
	</form>
</body>
</html>