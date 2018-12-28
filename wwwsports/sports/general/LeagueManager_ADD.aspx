<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">
	void AddLeagueAction(Object sender,EventArgs e) {
		int iUpdated = 0;
		string sNow;
		AddLeague AddLeag = new AddLeague((string)Application["SoccerDBConnectionString"]);
		sNow = DateTime.Now.ToString("HH:mm:ss");
		try {
			iUpdated = AddLeag.Add(League.Value.Trim(),Alias.Value.Trim(),HKJCLeague.Value.Trim(),HKJCLeagueAlias.Value.Trim(),MCLeague.Value.Trim());
			if(iUpdated > 0) {
				UpdateReturnMessage("���\�s�W�p��(" + sNow + ")");

				League.Value = "";
				Alias.Value = "";
				HKJCLeague.Value = "";
				HKJCLeagueAlias.Value = "";
				MCLeague.Value = "";
			}	else if(iUpdated == -1) {
				UpdateReturnMessage("�s�W���ѡA<b>�p�ɦW��(�Ȭw)</b>�w�s�b�I(" + sNow + ")");
			}	else if(iUpdated == -2) {
				UpdateReturnMessage("�s�W���ѡA<b>�p��²��(�Ȭw)</b>�w�s�b�I(" + sNow + ")");
			}	else if(iUpdated == -3) {
				UpdateReturnMessage("�s�W���ѡA<b>���|�W��</b>�w�s�b�I(" + sNow + ")");
			}	else if(iUpdated == -4) {
				UpdateReturnMessage("�s�W���ѡA<b>���|²��</b>�w�s�b�I(" + sNow + ")");
			}	else if(iUpdated == -5) {
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
	if(LengthOfString(document.AddLeagueForm.League.value) > 10) {
		alert('�p�ɦW��(�Ȭw)����h��10�Ӧ줸');
		return false;
	}

	if(LengthOfString(document.AddLeagueForm.Alias.value) > 6) {
		alert('�p��²��(�Ȭw)����h��6�Ӧ줸');
		return false;
	}

	if(LengthOfString(document.AddLeagueForm.HKJCLeague.value) > 40) {
		alert('���|�W�٤���h��40�Ӧ줸');
		return false;
	}

	if(LengthOfString(document.AddLeagueForm.HKJCLeagueAlias.value) > 10) {
		alert('���|²�٤���h��10�Ӧ줸');
		return false;
	}

	if(LengthOfString(document.AddLeagueForm.MCLeague.value) > 40) {
		alert('�D���W�٤���h��40�Ӧ줸');
		return false;
	}

	if(LengthOfString(document.AddLeagueForm.EnglishLeague.value) > 50) {
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
	<form id="AddLeagueForm" method="post" ONSUBMIT="return CheckLength()" runat="server">
		<b>�W�����:</b><asp:Label id="rtnMsg" runat="server" />
		<table border="1" width="60%">
			<tr align="center" style="background-color:#6FAFB1">
				<th colspan="2">
					<font color="#F0F8FF">�s�W�p��</font>
				</th>
			</tr>

			<tr align="center">
				<th><font color="red">*</font>�p�ɦW��(�Ȭw):</th>
				<td align="left"><input type="text" id="League" maxlength="5" runat="server"></td>
				<asp:RequiredFieldValidator
					id="LeagueVal"
					ControlToValidate="League"
					InitialValue=""
					ErrorMessage="�p�ɦW��(�Ȭw)����ť�"
					runat="server" />
			</tr>

			<tr align="center">
				<th><font color="red">*</font>�p��²��(�Ȭw):</th>
				<td align="left"><input type="text" id="Alias" maxlength="3" size="10" runat="server"></td>
				<asp:RequiredFieldValidator
					id="AliasVal"
					ControlToValidate="Alias"
					InitialValue=""
					ErrorMessage="�p��²��(�Ȭw)����ť�"
					runat="server" />
			</tr>

			<tr align="center">
				<th><font color="red">*</font>�p�����O:</th>
				<td align="left">
					<select name="leaguetype">
						<option value="1">�ꤺ</option>
						<option value="2">�ڬw</option>
						<option value="3">���</option>
					</select>
				</td>
			</tr>

			<tr align="center">
				<th><font color="red">*</font>���|�W��:</th>
				<td align="left"><input type="text" id="HKJCLeague" maxlength="20" size="30" runat="server"></td>
				<asp:RequiredFieldValidator
					id="HKJCLeagueVal"
					ControlToValidate="HKJCLeague"
					InitialValue=""
					ErrorMessage="���|�W�٤���ť�"
					runat="server" />
			</tr>

			<tr align="center">
				<th><font color="red">*</font>���|²��:</th>
				<td align="left"><input type="text" id="HKJCLeagueAlias" maxlength="5" size="10" runat="server"></td>
				<asp:RequiredFieldValidator
					id="HKJCLeagueAliasVal"
					ControlToValidate="HKJCLeagueAlias"
					InitialValue=""
					ErrorMessage="���|²�٤���ť�"
					runat="server" />
			</tr>

			<tr align="center">
				<th><font color="red">*</font>�D���W��:</th>
				<td align="left"><input type="text" id="MCLeague" maxlength="20" size="30" runat="server"></td>
				<asp:RequiredFieldValidator
					id="MCLeagueVal"
					ControlToValidate="MCLeague"
					InitialValue=""
					ErrorMessage="�D���W�٤���ť�"
					runat="server" />
			</tr>

			<tr align="center">
				<th>�^��W��:</th>
				<td align="left"><input type="text" name="EnglishLeague" maxlength="50" size="40"></td>
			</tr>

			<tr align="center">
				<th>�|���´:</th>
				<td align="left"><input type="text" name="org" maxlength="20"></td>
			</tr>

			<tr align="right">
				<td colspan="2">
					<font size="2">(��<font color="red">*</font>�̥�����g)</font>
					<input type="submit" id="addBtn" value="�s�W" OnServerClick="AddLeagueAction" runat="server">
					&nbsp;<input type="reset" value="���]">
				</td>
			</tr>
		</table>
	</form>
	</center>

</body>
</html>