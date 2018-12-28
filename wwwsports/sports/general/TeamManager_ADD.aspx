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
				UpdateReturnMessage("成功新增隊伍(" + sNow + ")");

				team.Value = "";
				HKJCTeam.Value = "";
				HKJCTeamAlias.Value = "";
				MCTeam.Value = "";
			}	else if(iUpdated == -1) {
				UpdateReturnMessage("新增失敗，<b>隊伍名稱(亞洲)</b>已存在！(" + sNow + ")");
			}	else if(iUpdated == -2) {
				UpdateReturnMessage("新增失敗，<b>馬會全名</b>已存在！(" + sNow + ")");
			}	else if(iUpdated == -3) {
				UpdateReturnMessage("新增失敗，<b>馬會簡稱</b>已存在！(" + sNow + ")");
			}	else if(iUpdated == -4) {
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
		alert('隊伍名稱(亞洲)不能多於6個位元');
		return false;
	}

	if(LengthOfString(document.AddTeamForm.HKJCTeam.value) > 20) {
		alert('馬會全名不能多於20個位元');
		return false;
	}

	if(LengthOfString(document.AddTeamForm.HKJCTeamAlias.value) > 6) {
		alert('馬會簡稱不能多於6個位元');
		return false;
	}

	if(LengthOfString(document.AddTeamForm.MCTeam.value) > 20) {
		alert('澳門名稱不能多於20個位元');
		return false;
	}

	document.AddTeamForm.EnglishTeam.value = Trim(document.AddTeamForm.EnglishTeam.value);
	if(LengthOfString(document.AddTeamForm.EnglishTeam.value) > 50) {
		alert('英文名稱不能多於50個位元');
		return false;
	}

	document.AddTeamForm.venue.value = Trim(document.AddTeamForm.venue.value);
	if(LengthOfString(document.AddTeamForm.venue.value) > 20) {
		alert('主場名稱不能多於20個位元');
		return false;
	}

	document.AddTeamForm.continent.value = Trim(document.AddTeamForm.continent.value);
	if(LengthOfString(document.AddTeamForm.continent.value) > 8) {
		alert('所屬洲份不能多於8個位元');
		return false;
	}

	document.AddTeamForm.country.value = Trim(document.AddTeamForm.country.value);
	if(LengthOfString(document.AddTeamForm.country.value) > 10) {
		alert('所屬國家不能多於10個位元');
		return false;
	}

	document.AddTeamForm.city.value = Trim(document.AddTeamForm.city.value);
	if(LengthOfString(document.AddTeamForm.city.value) > 10) {
		alert('所屬城巿不能多於10個位元');
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
		<b>上次行動:</b><asp:Label id="rtnMsg" runat="server" />
		<table border="1" width="60%">
			<tr align="center" style="background-color:#FFA07A">
				<th colspan="2">
					<font color="#FFFAF0">新增隊伍</font>
				</th>
			</tr>

			<tr>
				<th align="center"><font color="red">*</font>隊伍名稱(亞洲):</th>
				<td><input type="text" id="team" maxlength="10" size="10" runat="server"></td>
				<asp:RequiredFieldValidator
					id="teamVal"
					ControlToValidate="team"
					InitialValue=""
					ErrorMessage="隊伍名稱(亞洲)不能空白"
					runat="server" />
			</tr>

			<tr>
				<th align="center"><font color="red">*</font>所屬聯賽:</th>
				<td>
					<span id="TeamInformation" runat="server" />
				</td>
			</tr>

			<tr>
				<th align="center"><font color="red">*</font>馬會全名:</th>
				<td><input type="text" id="HKJCTeam" onChange=copyText() maxlength="20" size="20" runat="server"></td>
				<asp:RequiredFieldValidator
					id="HKJCTeamVal"
					ControlToValidate="HKJCTeam"
					InitialValue=""
					ErrorMessage="馬會全名不能空白"
					runat="server" />
			</tr>

			<tr>
				<th align="center"><font color="red">*</font>馬會簡稱:</th>
				<td><input type="text" id="HKJCTeamAlias" maxlength="10" size="10" runat="server"></td>
				<asp:RequiredFieldValidator
					id="HKJCTeamAliasVal"
					ControlToValidate="HKJCTeamAlias"
					InitialValue=""
					ErrorMessage="馬會簡稱不能空白"
					runat="server" />
			</tr>

			<tr>
				<th align="center"><font color="red">*</font>澳門名稱:</th>
				<td><input type="text" id="MCTeam" maxlength="20" size="20" runat="server"></td>
				<asp:RequiredFieldValidator
					id="MCTeamVal"
					ControlToValidate="MCTeam"
					InitialValue=""
					ErrorMessage="澳門名稱不能空白"
					runat="server" />
			</tr>

			<tr>
				<th align="center">英文名稱:</th>
				<td><input type="text" name="EnglishTeam" maxlength="50" size="40"></td>
			</tr>

			<tr>
				<th align="center">主場名稱:</th>
				<td><input type="text" name="venue" maxlength="10" size="10"></td>
			</tr>

			<tr>
				<th align="center">所屬洲份:</th>
				<td><input type="text" name="continent" maxlength="4" size="10"></td>
			</tr>

			<tr>
				<th align="center">所屬國家:</th>
				<td><input type="text" name="country" maxlength="5" size="10"></td>
			</tr>

			<tr>
				<th align="center">所屬城巿:</th>
				<td><input type="text" name="city" maxlength="5" size="10"></td>
			</tr>

			<tr align="right">
				<td colspan="2">
					<font size="2">(有<font color="red">*</font>者必須填寫)</font>
					<input type="submit" id="AddBtn" value="新增" OnServerClick="AddLeagueAction" runat="server">
					&nbsp;<input type="reset" value="重設">
				</td>
			</tr>
		</table>
	</form>
	</center>
</body>
</html>