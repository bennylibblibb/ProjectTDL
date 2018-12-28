<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>

<%@ Import Namespace="TDL.IO"%>

<script language="C#" runat="server">
	void Page_Load(Object sender,EventArgs e) {
		AlertSpecialMessage alert = new AlertSpecialMessage((string)Application["SportAppCfgConnectionString"]);
		HeaderInformation.InnerHtml = alert.GetMessagePage();
		ReloadPageMessage((string)Application["retrieveInfoMsg"] + "特定訊息(" + DateTime.Now.ToString("HH:mm:ss") + ")");
	}	

	void SendAlert(Object sender,EventArgs e) {
		int i = 0;
		int iUpdated = 0;
		string sNow;
		string sCodeStatus = "";
		string[] arrSendToPager;
		AlertSpecialMessage AlerMessage = new AlertSpecialMessage((string)Application["SportAppCfgConnectionString"]);
		sNow = DateTime.Now.ToString("HH:mm:ss");
/*		
		Files myLog;
		myLog = new Files();
		myLog.FilePath = "D:\\Projects\\wwwsports\\error_log\\AlertSpcMsg_error.";
		myLog.SetFileName(0,"log");
*/
		try {
			int[] arrCode = AlerMessage.SendRemotingMessage();
			arrSendToPager = AlerMessage.GetSendToPager();
			Page_Load(sender,e);
/*			
			myLog.Open();
			myLog.Write("arrCode[6] = "+arrCode[6].ToString());
			myLog.Close();
*/				
			if(arrCode[6] > 0) {
				for (i = 0; i < arrSendToPager.Length; i++) {
					if (arrSendToPager[i] == "2") {
						if (arrCode[0]>=0 && arrCode[3]==100000)
							iUpdated++;
					} else if (arrSendToPager[i] == "5") {
						if (arrCode[1]>=0 && arrCode[4]==100000)
							iUpdated++;
					} else if (arrSendToPager[i] == "11") {
						if (arrCode[2]>=0 && arrCode[5]==100000)
							iUpdated++;
					}
				}
/*				
				myLog.Open();
				myLog.Write("iUpdated = "+iUpdated.ToString());
				myLog.Close();
*/				
				if (i == iUpdated) {
					sCodeStatus = "成功發送特定訊息 (" + sNow + ")";
				} else {
					if(arrCode[0] == -1) {
						sCodeStatus += "<br><font color=\"red\"><b>寫入GOGO機特定訊息錯誤！</b></font>";
					}
					if(arrCode[1] == -1) {
						sCodeStatus += "<br><font color=\"red\"><b>寫入馬會機特定訊息錯誤！</b></font>";
					}
					if(arrCode[2] == -1) {
						sCodeStatus += "<br><font color=\"red\"><b>寫入JC Combo特定訊息錯誤！</b></font>";
					}
					if(arrCode[3] == -1) {
						sCodeStatus += "<br><font color=\"red\"><b>傳送GOGO機特定訊息錯誤，系統問題！</b></font>";
					}
					if(arrCode[3] == -2) {
						sCodeStatus += "<br><font color=\"red\"><b>傳送GOGO機特定訊息錯誤，連接失敗！</b></font>";
					}
					if(arrCode[3] == -3) {
						sCodeStatus += "<br><font color=\"red\"><b>沒有GOGO機特定訊息可傳送！</b></font>";
					}
					if(arrCode[4] == -1) {
						sCodeStatus += "<br><font color=\"red\"><b>傳送馬會機特定訊息錯誤，系統問題！</b></font>";
					}
					if(arrCode[4] == -2) {
						sCodeStatus += "<br><font color=\"red\"><b>傳送馬會機特定訊息錯誤，連接失敗！</b></font>";
					}
					if(arrCode[4] == -3) {
						sCodeStatus += "<br><font color=\"red\"><b>沒有馬會機特定訊息可傳送！</b></font>";
					}
					if(arrCode[5] == -1) {
						sCodeStatus += "<br><font color=\"red\"><b>傳送JC Combo特定訊息錯誤，系統問題！</b></font>";
					}
					if(arrCode[5] == -2) {
						sCodeStatus += "<br><font color=\"red\"><b>傳送JC Combo特定訊息錯誤，連接失敗！</b></font>";
					}
					if(arrCode[5] == -3) {
						sCodeStatus += "<br><font color=\"red\"><b>沒有JC Combo特定訊息可傳送！</b></font>";
					}
					if(arrCode[5] == -4) {
						sCodeStatus += "<br><font color=\"red\"><b>Invalid AppID or MsgID for JC Combo!</b></font>";
					}
				}
			} else if (arrCode[6] == -2 ) {
				sCodeStatus = "<font color=\"red\"><b>請選擇 Message Type！</b></font>";
			} else {
				sCodeStatus = "<br><font color=\"red\"><b>" + (string)Application["transErrorMsg"] + "</b></font> (" + sNow + ")";
			}
			ReloadPageMessage(sCodeStatus);
			
		}	catch(NullReferenceException nullex) {
			FormsAuthentication.SignOut();
			ReloadPageMessage("<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>");
		}
	}
	
	void ReloadPageMessage(string sMsg) {
		PageMsg.Text = sMsg;
	}
</script>

<script language="JavaScript">
function checkPagers() {
	if(!document.AlertSpecialMessageForm.SendToPager[0].checked && !document.AlertSpecialMessageForm.SendToPager[1].checked && !document.AlertSpecialMessageForm.SendToPager[2].checked) {
		alert("請選擇發送目標!");
		return false;
	} else if (document.AlertSpecialMessageForm.alertTypeRadio[1].checked && document.AlertSpecialMessageForm.PagerMessageType.value == "0") {
		alert("請選擇 Message Type！");
		return false;
	} else if (document.AlertSpecialMessageForm.alertTypeRadio[0].checked) {
		if (document.AlertSpecialMessageForm.SendToPager[0].checked && document.AlertSpecialMessageForm.SendToPager[1].checked) {
			alert("Only choose one device for individual Header ID");
			return false;
		} else if (document.AlertSpecialMessageForm.SendToPager[0].checked && document.AlertSpecialMessageForm.SendToPager[2].checked) {
			alert("Only choose one device for individual Header ID");
			return false;
		} else if (document.AlertSpecialMessageForm.SendToPager[1].checked && document.AlertSpecialMessageForm.SendToPager[2].checked) {
	    	alert("Only choose one device for individual Header ID");
			return false;
		}
	} else
		return true;
}
</script>
<script language="JavaScript">
function detect() {    
	if (document.AlertSpecialMessageForm.alertTypeRadio[0].checked) {
		document.AlertSpecialMessageForm.HeaderID.disabled = false;
		document.AlertSpecialMessageForm.PagerMessageType.disabled = true;
		if (document.AlertSpecialMessageForm.SendToPager[0].checked && document.AlertSpecialMessageForm.SendToPager[1].checked) {
			alert("Only choose one device for individual Header ID");
		} else if (document.AlertSpecialMessageForm.SendToPager[0].checked && document.AlertSpecialMessageForm.SendToPager[2].checked) {
			alert("Only choose one device for individual Header ID");
		} else if (document.AlertSpecialMessageForm.SendToPager[1].checked && document.AlertSpecialMessageForm.SendToPager[2].checked) {
	    	alert("Only choose one device for individual Header ID");
		}
	} else if (document.AlertSpecialMessageForm.alertTypeRadio[1].checked) {
		document.AlertSpecialMessageForm.HeaderID.disabled = true;
		document.AlertSpecialMessageForm.PagerMessageType.disabled = false;
	}
    else
        alert("You should choose either one of them");
}

function filter() {
	// Only Combo
	if (document.AlertSpecialMessageForm.PagerMessageType.value == "AnalysisHistory" || document.AlertSpecialMessageForm.PagerMessageType.value == "AnalysisPlayers") {
		document.AlertSpecialMessageForm.SendToPager[0].checked = false;
		document.AlertSpecialMessageForm.SendToPager[1].checked = false;
		document.AlertSpecialMessageForm.SendToPager[2].checked = false;
		document.AlertSpecialMessageForm.SendToPager[0].disabled = true;
		document.AlertSpecialMessageForm.SendToPager[1].disabled = true;
	}
	// HKJC and Combo
/*
	else if (document.AlertSpecialMessageForm.PagerMessageType.value == "AnalysisPlayers") {
		document.AlertSpecialMessageForm.SendToPager[0].checked = false;
		document.AlertSpecialMessageForm.SendToPager[1].checked = false;
		document.AlertSpecialMessageForm.SendToPager[2].checked = false;
		document.AlertSpecialMessageForm.SendToPager[0].disabled = true;
		document.AlertSpecialMessageForm.SendToPager[1].disabled = false;
	}
*/
	// Otherwise
	else {
		document.AlertSpecialMessageForm.SendToPager[0].checked = false;
		document.AlertSpecialMessageForm.SendToPager[1].checked = false;
		document.AlertSpecialMessageForm.SendToPager[2].checked = false;
		document.AlertSpecialMessageForm.SendToPager[0].disabled = false;
		document.AlertSpecialMessageForm.SendToPager[1].disabled = false;
	}
}
</script>

<html>
<head>
	<META http-equiv="Content-Type" content="text/html; charset=big5">
	<LINK REL="stylesheet" HREF="/sportStyle.css" TYPE="text/css">
	<title>體育資訊 - 特定訊息</title>	
</head>

<body>
	<form id="AlertSpecialMessageForm" method="post" Onsubmit="return checkPagers()" runat="server">
		<font size="2"><b>上次行動:</b><asp:Label id="PageMsg" runat="server" /></font><br>
		<table border="1" width="60%" style="font: 10pt verdana">
			<tr style="background-color:#FFF0F5">
				<td align="left">
					訊息類別:
				</td>
				<td align="left">
					訊息內容:
				</td>
			</tr>
			<tr>
				<td align="left">
					<span id="HeaderInformation" runat="server" />
					<!--<textarea name="alertMsg" rows=10 cols=40></textarea>-->
				</td>
			</tr>
			<tr>
				<td colspan="2" align="right">
					<!--
						Value of SendToPager is Path ID
						1: GOGO1 Sender1		<input type="checkbox" name="SendToPager" value="1">GOGO1&nbsp;
						2: GOGO2 Handler1
						5: HKJC Handler1
						11: JCCombo MISC   
						20: GOGO3Combo MISC		<input type="checkbox" name="SendToPager" value="20">GOGO3Combo&nbsp;
					-->
					
					<input type="checkbox" name="SendToPager" value="2">GOGO2&nbsp;
					<input type="checkbox" name="SendToPager" value="5">馬會機&nbsp;
					<input type="checkbox" name="SendToPager" value="11">JCCombo&nbsp;
				</td>
			</tr>
			<tr>
				<td colspan="2" align="right">
					<input type="submit" id="SendBtn" value="傳送" OnServerClick="SendAlert" runat="server">&nbsp;
					<input type="reset" value="重設">
				</td>
			</tr>
		</table>
	</form>
</body>
</html>