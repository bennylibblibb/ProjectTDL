<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">
	void Page_Load(Object sender,EventArgs e) {
		AddTeam AddForm = new AddTeam((string)Application["SoccerDBConnectionString"]);
		try {
			TeamInformation.InnerHtml = AddForm.GetLeagues();
		} catch(NullReferenceException) {
			FormsAuthentication.SignOut();
			UpdateReturnMessage("<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>");
		}
	}

	void AddLeagueAction(Object sender,EventArgs e) {
		int iUpdated = 0;
		string sNow;
		AddTeam teamUpdate = new AddTeam((string)Application["SoccerDBConnectionString"]);
		sNow = DateTime.Now.ToString("HH:mm:ss");
		try {
			iUpdated = teamUpdate.Add(team.Value, HKJCTeam.Value, HKJCTeamAlias.Value, MCTeam.Value);
			Page_Load(sender,e);
			if(iUpdated > 0) {
				UpdateReturnMessage("���\�s�W����(" + sNow + ")");

				team.Value = "";
				HKJCTeam.Value = "";
				HKJCTeamAlias.Value = "";
				MCTeam.Value = "";
			}	else if(iUpdated == -1) {
				UpdateReturnMessage("�s�W���ѡA<b>����W��(�Ȭw)</b>�w�s�b�I(" + sNow + ")");
			}	else if(iUpdated == -2) {
				UpdateReturnMessage("�s�W���ѡA<b>���|���W</b>�w�s�b�I(" + sNow + ")");
			}	else if(iUpdated == -3) {
				UpdateReturnMessage("�s�W���ѡA<b>���|²��</b>�w�s�b�I(" + sNow + ")");
			}	else if(iUpdated == -4) {
				UpdateReturnMessage("�s�W���ѡA<b>�D���W��</b>�w�s�b�I(" + sNow + ")");
			}	else {
				UpdateReturnMessage((string)Application["transErrorMsg"] + "(" + sNow + ")");
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
	if(LengthOfString(document.AddTeamForm.team.value) > 6) {
		alert('����W��(�Ȭw)����h��6�Ӧ줸');
		return false;
	}

	if(LengthOfString(document.AddTeamForm.HKJCTeam.value) > 20) {
		alert('���|���W����h��20�Ӧ줸');
		return false;
	}

	if(LengthOfString(document.AddTeamForm.HKJCTeamAlias.value) > 6) {
		alert('���|²�٤���h��6�Ӧ줸');
		return false;
	}

	if(LengthOfString(document.AddTeamForm.MCTeam.value) > 20) {
		alert('�D���W�٤���h��20�Ӧ줸');
		return false;
	}

	document.AddTeamForm.EnglishTeam.value = Trim(document.AddTeamForm.EnglishTeam.value);
	if(LengthOfString(document.AddTeamForm.EnglishTeam.value) > 50) {
		alert('�^��W�٤���h��50�Ӧ줸');
		return false;
	}

	document.AddTeamForm.venue.value = Trim(document.AddTeamForm.venue.value);
	if(LengthOfString(document.AddTeamForm.venue.value) > 20) {
		alert('�D���W�٤���h��20�Ӧ줸');
		return false;
	}

	document.AddTeamForm.continent.value = Trim(document.AddTeamForm.continent.value);
	if(LengthOfString(document.AddTeamForm.continent.value) > 8) {
		alert('���ݬw������h��8�Ӧ줸');
		return false;
	}

	document.AddTeamForm.country.value = Trim(document.AddTeamForm.country.value);
	if(LengthOfString(document.AddTeamForm.country.value) > 10) {
		alert('���ݰ�a����h��10�Ӧ줸');
		return false;
	}

	document.AddTeamForm.city.value = Trim(document.AddTeamForm.city.value);
	if(LengthOfString(document.AddTeamForm.city.value) > 10) {
		alert('���ݫ��]����h��10�Ӧ줸');
		return false;
	}

	return true;
}

function copyText() {
	document.AddTeamForm.HKJCTeamAlias.value = document.AddTeamForm.HKJCTeam.value.substring(0,3);
}
</script>

<html>
<head>
	<META http-equiv="Content-Type" content="text/html; charset=big5">
	<LINK REL="stylesheet" HREF="/sportStyle.css" TYPE="text/css">
</head>
<body>
	<center>
	<form id="AddTeamForm" method="post" ONSUBMIT="return CheckLength()" runat="server">
		<b>�W�����:</b><asp:Label id="rtnMsg" runat="server" />
		<table border="1" width="60%">
			<tr align="center" style="background-color:#FFA07A">
				<th colspan="2">
					<font color="#FFFAF0">�s�W����</font>
				</th>
			</tr>

			<tr>
				<th align="center"><font color="red">*</font>����W��(�Ȭw):</th>
				<td><input type="text" id="team" maxlength="10" size="10" runat="server"></td>
				<asp:RequiredFieldValidator
					id="teamVal"
					ControlToValidate="team"
					InitialValue=""
					ErrorMessage="����W��(�Ȭw)����ť�"
					runat="server" />
			</tr>

			<tr>
				<th align="center"><font color="red">*</font>�����p��:</th>
				<td>
					<span id="TeamInformation" runat="server" />
				</td>
			</tr>

			<tr>
				<th align="center"><font color="red">*</font>���|���W:</th>
				<td><input type="text" id="HKJCTeam" onChange=copyText() maxlength="20" size="20" runat="server"></td>
				<asp:RequiredFieldValidator
					id="HKJCTeamVal"
					ControlToValidate="HKJCTeam"
					InitialValue=""
					ErrorMessage="���|���W����ť�"
					runat="server" />
			</tr>

			<tr>
				<th align="center"><font color="red">*</font>���|²��:</th>
				<td><input type="text" id="HKJCTeamAlias" maxlength="10" size="10" runat="server"></td>
				<asp:RequiredFieldValidator
					id="HKJCTeamAliasVal"
					ControlToValidate="HKJCTeamAlias"
					InitialValue=""
					ErrorMessage="���|²�٤���ť�"
					runat="server" />
			</tr>

			<tr>
				<th align="center"><font color="red">*</font>�D���W��:</th>
				<td><input type="text" id="MCTeam" maxlength="20" size="20" runat="server"></td>
				<asp:RequiredFieldValidator
					id="MCTeamVal"
					ControlToValidate="MCTeam"
					InitialValue=""
					ErrorMessage="�D���W�٤���ť�"
					runat="server" />
			</tr>

			<tr>
				<th align="center">�^��W��:</th>
				<td><input type="text" name="EnglishTeam" maxlength="50" size="40"></td>
			</tr>

			<tr>
				<th align="center">�D���W��:</th>
				<td><input type="text" name="venue" maxlength="10" size="10"></td>
			</tr>

			<tr>
				<th align="center">���ݬw��:</th>
				<td><input type="text" name="continent" maxlength="4" size="10"></td>
			</tr>

			<tr>
				<th align="center">���ݰ�a:</th>
				<td><input type="text" name="country" maxlength="5" size="10"></td>
			</tr>

			<tr>
				<th align="center">���ݫ��]:</th>
				<td><input type="text" name="city" maxlength="5" size="10"></td>
			</tr>

			<tr align="right">
				<td colspan="2">
					<font size="2">(��<font color="red">*</font>�̥�����g)</font>
					<input type="submit" id="AddBtn" value="�s�W" OnServerClick="AddLeagueAction" runat="server">
					&nbsp;<input type="reset" value="���]">
				</td>
			</tr>
		</table>
	</form>
	</center>
</body>
</html>