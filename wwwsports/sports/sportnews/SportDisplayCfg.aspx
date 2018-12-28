<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>

<%@ Import Namespace="TDL.IO"%>

<script language="C#" runat="server">
	void Page_Load(Object sender,EventArgs e) {
		SportDisplayCfg alert = new SportDisplayCfg((string)Application["SoccerDBConnectionString"]);
		HeaderInformation.InnerHtml = alert.GetMessagePage();
		ReloadPageMessage((string)Application["retrieveInfoMsg"] + "�����]�w(" + DateTime.Now.ToString("HH:mm:ss") + ")");
	}	

	void changeDisplayName(Object sender,EventArgs e) {
		int iUpdated = 0;
		int iCheckTeam = 0;
		string sLeagRefMsg = "";
		string sNow;
/*
		Files myLog;
		myLog = new Files();
		myLog.FilePath = "D:\\Projects\\wwwsports\\error_log\\AlertSpcMsg_error.";
		myLog.SetFileName(0,"log");
		
		myLog.Open();
		myLog.Write("iUpdated = "+iUpdated.ToString());
		myLog.Close();
*/
		SportDisplayCfg ModifyDisplayName = new SportDisplayCfg((string)Application["SoccerDBConnectionString"]);
		sNow = DateTime.Now.ToString("HH:mm:ss");
		try {
			iUpdated = ModifyDisplayName.ModifyName();
				//errorReturnMessage("The Updated Number is <font color=\"red\">" +iUpdated+ "</font>");
				//errorMessage("The Check Team Number is <font color=\"red\">" +iCheckTeam+ "</font>");
			Page_Load(sender,e);
			if(iUpdated > 0) {
				ReloadPageMessage("�ק令�\ (" + sNow + ")");
			} else if (iUpdated == -1){
				//errorMessage("The second Check Team Number is <font color=\"red\">" +iCheckTeam+ "</font>");
				ReloadPageMessage("<font color=\"red\">�L�ĭק�</font>: �S�� all_menu.ini �ɮצs�b (" + sNow + ")");
			} else if (iUpdated == -2){
				FormsAuthentication.SignOut();
				ReloadPageMessage("<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>");
			} else if (iUpdated == -3){
				ReloadPageMessage("<font color=\"red\">�L�ĭק�</font>: �s�����T���w�s�b (" + sNow + ")");
			}
		}
		catch(NullReferenceException nullex) {
			FormsAuthentication.SignOut();
			ReloadPageMessage("<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>");
		}
	}
	
	void blankTextBox (Object sender,EventArgs e) {
		HttpContext.Current.Request.Form["NewMsgName"] = "";
	}
	
	void ReloadPageMessage(string sMsg) {
		PageMsg.Text = sMsg;
	}
</script>

<script language="JavaScript">
function checkPagers() {
	
//	if(!document.SportDisplayCfgForm.SendToPager[0].checked && !document.SportDisplayCfgForm.SendToPager[1].checked && !document.SportDisplayCfgForm.SendToPager[2].checked) {
//		alert("�п�ܵo�e�ؼ�!");
//		return false;
//	} else
	if (document.SportDisplayCfgForm.OldMsgName.value == "0") {
		alert("�п�� Message Type�I");
		return false;
	} else if (document.SportDisplayCfgForm.NewMsgName.value == '') {
		alert('�s�����T������ť�');
		return false;
	} else if (document.SportDisplayCfgForm.NewMsgName.value.length != 2) {
		alert('�s�����T����������2�Ӧ줸');
		return false; 
	}
	return true;
}
</script>

<html>
<head>
	<META http-equiv="Content-Type" content="text/html; charset=big5">
	<LINK REL="stylesheet" HREF="/sportStyle.css" TYPE="text/css">
	<title>��|��T - �����]�w</title>	
</head>

<body>
	<form id="SportDisplayCfgForm" method="post" Onsubmit="return checkPagers()" runat="server">
		<font size="2"><b>�W�����:</b><asp:Label id="PageMsg" runat="server" /></font><br>
		<table border="1" width="40%" style="font: 10pt verdana">
			<tr style="background-color:#FFF0F5">
				<td align="left">
					�ª����T��:
				</td>
				<td align="left">
					�s�����T��:
				</td>
			</tr>
			<tr>
				<td align="left">
					<span id="HeaderInformation" runat="server" />
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
					
					<input type="hidden" name="SendToPager" value="1">GOGO2&nbsp;
					<input type="hidden" name="SendToPager" value="2">WinWin&nbsp;
					<input type="hidden" name="SendToPager" value="3">���|��&nbsp;
					<input type="hidden" name="SendToPager" value="4">JCCombo&nbsp;
				</td>
			</tr>
			<tr>
				<td colspan="2" align="right">
					<input type="submit" id="SaveBtn" value="��s" OnServerClick="changeDisplayName" runat="server">&nbsp;
					<input type="reset" value="���]" OnServerClick="blankTextBox" runat="server">
				</td>
			</tr>
		</table>
	</form>
</body>
</html>