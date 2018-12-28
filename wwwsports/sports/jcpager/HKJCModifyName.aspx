<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">
	void Page_Load(Object sender,EventArgs e) {
		string sNow;
		sNow = DateTime.Now.ToString("HH:mm:ss");
		HKJCModifyName ModifyForm = new HKJCModifyName();
		try {
			ChangeNameInformation.InnerHtml = ModifyForm.GetLeaguesName();
			UpdateReturnMessage("�ק��p�ɶ���W�� (" + sNow + ")");
		} catch(NullReferenceException) {
			UpdateReturnMessage("<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>");
		}
	}

	void changeTeamName(Object sender,EventArgs e) {
		int iUpdated = 0;
		int iCheckTeam = 0;
		string sLeagRefMsg = "";
		string sNow;
		HKJCModifyName ModifyTeamName = new HKJCModifyName();
		sNow = DateTime.Now.ToString("HH:mm:ss");
		try {
			iCheckTeam = ModifyTeamName.CheckHostName() + ModifyTeamName.CheckGuestName();
			if (iCheckTeam > 0) {
				iUpdated = ModifyTeamName.ModifyName();
				//errorReturnMessage("The Updated Number is <font color=\"red\">" +iUpdated+ "</font>");
				//errorMessage("The Check Team Number is <font color=\"red\">" +iCheckTeam+ "</font>");
				Page_Load(sender,e);
				if(iUpdated > 0) {
					UpdateReturnMessage("�ק令�\ (" + sNow + ")");
				}
				else {
					UpdateReturnMessage((string)Application["accessErrorMsg"] + " (" + sNow + ")");
				}
			}
			else {
				//errorMessage("The second Check Team Number is <font color=\"red\">" +iCheckTeam+ "</font>");
				UpdateReturnMessage("<font color=\"red\">�L�ĭק�</font>: �S���¶�������W�� (" + sNow + ")");
			}
		}
		catch(NullReferenceException nullex) {
			FormsAuthentication.SignOut();
			UpdateReturnMessage("<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>");
		}
	}
	
	void blankTextBox (Object sender,EventArgs e) {
		HttpContext.Current.Request.Form["oldTeam"] = "";
		HttpContext.Current.Request.Form["newTeam"] = "";
	}

	void UpdateReturnMessage(string sMsg) {
		rtnMsg.Text = sMsg;
	}
	
	void errorReturnMessage(string sErrMsg) {
		errorMsg.Text = sErrMsg;
	}
	
	void errorMessage(string sErMsg) {
		errMsg.Text = sErMsg;
	}
	
</script>

<script language="JavaScript">
function Trim(str) {
	if(str.length > 0) {
		//Remove leading space
		var ch = str.substring(0, 1);
		while(ch == ' ') {
			str = str.substring(1, str.length);
			if(str.length > 0) {
      	ch = str.substring(0, 1);
      } else {
      	str = '';
      	break;
      }
		}

		//Remove trail space
		ch = str.substring(str.length-1, str.length);
		while(ch == ' ') {
			str = str.substring(0, str.length-1);
			if(str.length > 0) {
				ch = str.substring(str.length-1, str.length);
			} else {
				str = '';
      	break;
			}
		}

	} else {
		str = '';
	}
	return str;
}

function LengthOfString(s) {
	len = 0;
	for(i = 0; i < s.length; i++) {
		if((s.charCodeAt(i) <= 127) && (s.charCodeAt(i) >= 0)) {
			len++;
		} else {
			len = len + 2;
		}
	}
	return len;
}

function CheckLength() {

	document.ChangeNameForm.oldTeam.value = Trim(document.ChangeNameForm.oldTeam.value);
	if(document.ChangeNameForm.oldTeam.value == '') {
		alert('�¶���W�٤���ť�');
		return false;
	}
	if(LengthOfString(document.ChangeNameForm.oldTeam.value) > 6) {
		alert('�¶���W�٤���h��6�Ӧ줸');
		return false;
	}

	document.ChangeNameForm.newTeam.value = Trim(document.ChangeNameForm.newTeam.value);
	if(document.ChangeNameForm.newTeam.value == '') {
		alert('�s����W�٤���ť�');
		return false;
	}
	if(LengthOfString(document.ChangeNameForm.newTeam.value) > 6) {
		alert('�s����W�٤���h��6�Ӧ줸');
		return false;
	}

	return true;
}
</script>

<html>
<head>
	<META http-equiv="Content-Type" content="text/html; charset=big5">
	<LINK REL="stylesheet" HREF="/sportStyle.css" TYPE="text/css">
</head>
<body>
	<form id="ChangeNameForm" method="post"  ONSUBMIT="return CheckLength()" runat="server">
		<font size="2"><b>�W�����: </b><asp:Label id="rtnMsg" runat="server" /></font><br>

<!-- show error
		<font size="2"><b>Show MESSAGE:</b><asp:Label id="errorMsg" runat="server" /></font><br>
		<font size="2"><b>Show MESSAGE:</b><asp:Label id="errMsg" runat="server" /></font><br>
-->

		<table border="1" width="70%" style="font: 10pt verdana">
				<tr style="background-color:#90EE90; color:#191970">
					<td colspan="3" align="left">
						<font>�¶���W��:</font><input type="text" id="oldTeam" maxlength="6" size="6" runat="server">
						<font>�s����W��:</font><input type="text" id="newTeam" maxlength="6" size="6" runat="server">
						<input type="submit" id="SaveBtn" value="��s" OnServerClick="changeTeamName" runat="server">&nbsp;
						<input type="reset" value="���]" OnServerClick="blankTextBox" runat="server">
					</td>
				</tr>
				<tr style="background-color:#FFF0F5">
					<th>�p�ɦW��</th><th>�D��</th><th>�ȶ�</th>
				</tr>

				<span id="ChangeNameInformation" runat="server" />
		</table>
	</form>
</body>
</html>
