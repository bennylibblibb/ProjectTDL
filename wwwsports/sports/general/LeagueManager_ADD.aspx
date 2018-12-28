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
				UpdateReturnMessage("成功新增聯賽(" + sNow + ")");

				League.Value = "";
				Alias.Value = "";
				HKJCLeague.Value = "";
				HKJCLeagueAlias.Value = "";
				MCLeague.Value = "";
			}	else if(iUpdated == -1) {
				UpdateReturnMessage("新增失敗，<b>聯賽名稱(亞洲)</b>已存在！(" + sNow + ")");
			}	else if(iUpdated == -2) {
				UpdateReturnMessage("新增失敗，<b>聯賽簡稱(亞洲)</b>已存在！(" + sNow + ")");
			}	else if(iUpdated == -3) {
				UpdateReturnMessage("新增失敗，<b>馬會名稱</b>已存在！(" + sNow + ")");
			}	else if(iUpdated == -4) {
				UpdateReturnMessage("新增失敗，<b>馬會簡稱</b>已存在！(" + sNow + ")");
			}	else if(iUpdated == -5) {
				UpdateReturnMessage("新增失敗，<b>澳門名稱</b>已存在！(" + sNow + ")");
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
		alert('聯賽名稱(亞洲)不能多於10個位元');
		return false;
	}

	if(LengthOfString(document.AddLeagueForm.Alias.value) > 6) {
		alert('聯賽簡稱(亞洲)不能多於6個位元');
		return false;
	}

	if(LengthOfString(document.AddLeagueForm.HKJCLeague.value) > 40) {
		alert('馬會名稱不能多於40個位元');
		return false;
	}

	if(LengthOfString(document.AddLeagueForm.HKJCLeagueAlias.value) > 10) {
		alert('馬會簡稱不能多於10個位元');
		return false;
	}

	if(LengthOfString(document.AddLeagueForm.MCLeague.value) > 40) {
		alert('澳門名稱不能多於40個位元');
		return false;
	}

	if(LengthOfString(document.AddLeagueForm.EnglishLeague.value) > 50) {
		alert('英文名稱不能多於50個位元');
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
		<b>上次行動:</b><asp:Label id="rtnMsg" runat="server" />
		<table border="1" width="60%">
			<tr align="center" style="background-color:#6FAFB1">
				<th colspan="2">
					<font color="#F0F8FF">新增聯賽</font>
				</th>
			</tr>

			<tr align="center">
				<th><font color="red">*</font>聯賽名稱(亞洲):</th>
				<td align="left"><input type="text" id="League" maxlength="5" runat="server"></td>
				<asp:RequiredFieldValidator
					id="LeagueVal"
					ControlToValidate="League"
					InitialValue=""
					ErrorMessage="聯賽名稱(亞洲)不能空白"
					runat="server" />
			</tr>

			<tr align="center">
				<th><font color="red">*</font>聯賽簡稱(亞洲):</th>
				<td align="left"><input type="text" id="Alias" maxlength="3" size="10" runat="server"></td>
				<asp:RequiredFieldValidator
					id="AliasVal"
					ControlToValidate="Alias"
					InitialValue=""
					ErrorMessage="聯賽簡稱(亞洲)不能空白"
					runat="server" />
			</tr>

			<tr align="center">
				<th><font color="red">*</font>聯賽類別:</th>
				<td align="left">
					<select name="leaguetype">
						<option value="1">國內</option>
						<option value="2">歐洲</option>
						<option value="3">國際</option>
					</select>
				</td>
			</tr>

			<tr align="center">
				<th><font color="red">*</font>馬會名稱:</th>
				<td align="left"><input type="text" id="HKJCLeague" maxlength="20" size="30" runat="server"></td>
				<asp:RequiredFieldValidator
					id="HKJCLeagueVal"
					ControlToValidate="HKJCLeague"
					InitialValue=""
					ErrorMessage="馬會名稱不能空白"
					runat="server" />
			</tr>

			<tr align="center">
				<th><font color="red">*</font>馬會簡稱:</th>
				<td align="left"><input type="text" id="HKJCLeagueAlias" maxlength="5" size="10" runat="server"></td>
				<asp:RequiredFieldValidator
					id="HKJCLeagueAliasVal"
					ControlToValidate="HKJCLeagueAlias"
					InitialValue=""
					ErrorMessage="馬會簡稱不能空白"
					runat="server" />
			</tr>

			<tr align="center">
				<th><font color="red">*</font>澳門名稱:</th>
				<td align="left"><input type="text" id="MCLeague" maxlength="20" size="30" runat="server"></td>
				<asp:RequiredFieldValidator
					id="MCLeagueVal"
					ControlToValidate="MCLeague"
					InitialValue=""
					ErrorMessage="澳門名稱不能空白"
					runat="server" />
			</tr>

			<tr align="center">
				<th>英文名稱:</th>
				<td align="left"><input type="text" name="EnglishLeague" maxlength="50" size="40"></td>
			</tr>

			<tr align="center">
				<th>舉辦組織:</th>
				<td align="left"><input type="text" name="org" maxlength="20"></td>
			</tr>

			<tr align="right">
				<td colspan="2">
					<font size="2">(有<font color="red">*</font>者必須填寫)</font>
					<input type="submit" id="addBtn" value="新增" OnServerClick="AddLeagueAction" runat="server">
					&nbsp;<input type="reset" value="重設">
				</td>
			</tr>
		</table>
	</form>
	</center>

</body>
</html>