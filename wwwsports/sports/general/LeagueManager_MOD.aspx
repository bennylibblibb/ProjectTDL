<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">
	void Page_Load(Object sender,EventArgs e) {
		ModifyLeague ModifyForm = new ModifyLeague((string)Application["SoccerDBConnectionString"]);
		try {
			LeagueInformation.InnerHtml = ModifyForm.GetLeagues();
		} catch(NullReferenceException) {
			FormsAuthentication.SignOut();
			UpdateReturnMessage("<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>");
		}
	}

	void ModifyLeagueAction(Object sender,EventArgs e) {
		int iUpdated = 0;
		string sLeagRefMsg = "";
		string sNow;
		ModifyLeague ModifyLeag = new ModifyLeague((string)Application["SoccerDBConnectionString"]);
		sNow = DateTime.Now.ToString("HH:mm:ss");
		try {
			iUpdated = ModifyLeag.Modify();
			if (iUpdated > 0) sLeagRefMsg = ModifyLeag.CheckLeagReference();
			Page_Load(sender,e);
			if(iUpdated > 0) {
				if (sLeagRefMsg.Length == 0) UpdateReturnMessage("�ק令�\(" + sNow + ")");
				else UpdateReturnMessage("�ק令�\(" + sNow + ") <br>"+sLeagRefMsg);
			}	else if(iUpdated == 0) {
				UpdateReturnMessage("�L�ĭק�(" + sNow + ")");
			}	else {
				UpdateReturnMessage((string)Application["transErrorMsg"] + "�A�ά����W�٤w�s�b�I (" + sNow + ")");
			}
		} catch(NullReferenceException nullex) {
			FormsAuthentication.SignOut();
			UpdateReturnMessage("<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>");
		}
	}

	void UpdateReturnMessage(string sMsg) {
		rtnMsg.Text = sMsg;
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
	document.ModifyLeagueForm.leagueName.value = Trim(document.ModifyLeagueForm.leagueName.value);
	if(document.ModifyLeagueForm.leagueName.value == '') {
		alert('�p�ɦW��(�Ȭw)����ť�');
		return false;
	}
	if(LengthOfString(document.ModifyLeagueForm.leagueName.value) > 10) {
		alert('�p�ɦW��(�Ȭw)����h��10�Ӧ줸');
		return false;
	}

	document.ModifyLeagueForm.alias.value = Trim(document.ModifyLeagueForm.alias.value);
	if(document.ModifyLeagueForm.alias.value == '') {
		alert('�p��²��(�Ȭw)����ť�');
		return false;
	}
	if(LengthOfString(document.ModifyLeagueForm.alias.value) > 6) {
		alert('�p��²��(�Ȭw)����h��6�Ӧ줸');
		return false;
	}

	document.ModifyLeagueForm.HKJCName.value = Trim(document.ModifyLeagueForm.HKJCName.value);
	if(document.ModifyLeagueForm.HKJCName.value == '') {
		alert('���|�W�٤���ť�');
		return false;
	}
	if(LengthOfString(document.ModifyLeagueForm.HKJCName.value) > 40) {
		alert('���|�W�٤���h��40�Ӧ줸');
		return false;
	}

	document.ModifyLeagueForm.HKJCNameAlias.value = Trim(document.ModifyLeagueForm.HKJCNameAlias.value);
	if(document.ModifyLeagueForm.HKJCNameAlias.value == '') {
		alert('���|²�٤���ť�');
		return false;
	}
	if(LengthOfString(document.ModifyLeagueForm.HKJCNameAlias.value) > 10) {
		alert('���|²�٤���h��10�Ӧ줸');
		return false;
	}

	document.ModifyLeagueForm.MCLeague.value = Trim(document.ModifyLeagueForm.MCLeague.value);
	if(document.ModifyLeagueForm.MCLeague.value == '') {
		alert('�D���W�٤���ť�');
		return false;
	}
	if(LengthOfString(document.ModifyLeagueForm.MCLeague.value) > 40) {
		alert('�D���W�٤���h��40�Ӧ줸');
		return false;
	}

	document.ModifyLeagueForm.engName.value = Trim(document.ModifyLeagueForm.engName.value);
	if(LengthOfString(document.ModifyLeagueForm.engName.value) > 50) {
		alert('�^��W�٤���h��50�Ӧ줸');
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
	<center>
	<form id="ModifyLeagueForm" method="post" ONSUBMIT="return CheckLength()" runat="server">
		<b>�W�����:</b><asp:Label id="rtnMsg" runat="server" />
		<table border="1" width="70%">
			<tr align="center" style="background-color:#6FAFB1">
				<th colspan=2><font color="#F0F8FF">�ק��p��</font></th>
			</tr>

			<span id="LeagueInformation" runat="server" />

			<tr align="right">
				<td align="center" style="background-color:#F5FFFA">
					<a href="LeagueManagerModifyFrame.htm" target="content_frame">��L�p��</a>
				</td>
				<td>
					<font size="2">
					<font color="red">#</font>�ק惡����ƪ��P�ɡA�t�η|�۰��ˬd���S��������T�w�Q�פJ
					</font>
					<input type="submit" id="modifyBtn" value="�ק�" OnServerClick="ModifyLeagueAction" runat="server">
					&nbsp;<input type="reset" value="���]">
				</td>
			</tr>
		</table>
	</form>
	</center>
</body>
</html>