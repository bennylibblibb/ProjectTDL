<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">
	void Page_Load(Object sender,EventArgs e) {
		ModifyTeamContent ModifyForm = new ModifyTeamContent((string)Application["SoccerDBConnectionString"]);

		try {
			TeamInformation.InnerHtml = ModifyForm.GetTeam();
		}	catch(NullReferenceException) {
			FormsAuthentication.SignOut();
			UpdateReturnMessage("<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>");
		}
	}

	void ModifyTeamAction(Object sender,EventArgs e) {
		int iUpdated = 0;
		string sTeamRefMsg = "";
		string sNow;
		ModifyTeamContent teamModify = new ModifyTeamContent((string)Application["SoccerDBConnectionString"]);
		sNow = DateTime.Now.ToString("HH:mm:ss");
		try {
			iUpdated = teamModify.Modify();
			if (iUpdated > 0) sTeamRefMsg = teamModify.CheckLeagReference();
			Page_Load(sender,e);
			if(iUpdated > 0) {
				if (sTeamRefMsg.Length == 0) UpdateReturnMessage("修改成功(" + sNow + ")");
				else UpdateReturnMessage("修改成功(" + sNow + ") <br>"+ sTeamRefMsg);
			}	else if(iUpdated == 0) {
				UpdateReturnMessage("無效修改(" + sNow + ")");
			}	else {
				UpdateReturnMessage((string)Application["transErrorMsg"] + "，或相關名稱已存在！ (" + sNow + ")");
			}
		}	catch(NullReferenceException nullex) {
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
	document.ModifyTeamContentForm.AsiaName.value = Trim(document.ModifyTeamContentForm.AsiaName.value);
	if(document.ModifyTeamContentForm.AsiaName.value == '') {
		alert('隊伍名稱(亞洲)不能空白');
		return false;
	}
	if(LengthOfString(document.ModifyTeamContentForm.AsiaName.value) > 6) {
		alert('隊伍名稱(亞洲)不能多於6個位元');
		return false;
	}

	document.ModifyTeamContentForm.HKJCName.value = Trim(document.ModifyTeamContentForm.HKJCName.value);
	if(document.ModifyTeamContentForm.HKJCName.value == '') {
		alert('馬會全名不能空白');
		return false;
	}
	if(LengthOfString(document.ModifyTeamContentForm.HKJCName.value) > 20) {
		alert('馬會全名不能多於20個位元');
		return false;
	}

	document.ModifyTeamContentForm.HKJCNameAlias.value = Trim(document.ModifyTeamContentForm.HKJCNameAlias.value);
	if(document.ModifyTeamContentForm.HKJCNameAlias.value == '') {
		alert('馬會簡稱不能空白');
		return false;
	}
	if(LengthOfString(document.ModifyTeamContentForm.HKJCNameAlias.value) > 6) {
		alert('馬會簡稱不能多於6個位元');
		return false;
	}

	document.ModifyTeamContentForm.MCTeam.value = Trim(document.ModifyTeamContentForm.MCTeam.value);
	if(document.ModifyTeamContentForm.MCTeam.value == '') {
		alert('澳門名稱不能空白');
		return false;
	}
	if(LengthOfString(document.ModifyTeamContentForm.MCTeam.value) > 20) {
		alert('澳門名稱不能多於20個位元');
		return false;
	}

	document.ModifyTeamContentForm.EnglishName.value = Trim(document.ModifyTeamContentForm.EnglishName.value);
	if(LengthOfString(document.ModifyTeamContentForm.EnglishName.value) > 50) {
		alert('英文名稱不能多於50個位元');
		return false;
	}

	document.ModifyTeamContentForm.venue.value = Trim(document.ModifyTeamContentForm.venue.value);
	if(LengthOfString(document.ModifyTeamContentForm.venue.value) > 20) {
		alert('主場名稱不能多於20個位元');
		return false;
	}

	document.ModifyTeamContentForm.continent.value = Trim(document.ModifyTeamContentForm.continent.value);
	if(LengthOfString(document.ModifyTeamContentForm.continent.value) > 8) {
		alert('所屬洲份不能多於8個位元');
		return false;
	}

	document.ModifyTeamContentForm.country.value = Trim(document.ModifyTeamContentForm.country.value);
	if(LengthOfString(document.ModifyTeamContentForm.country.value) > 10) {
		alert('所屬國家不能多於10個位元');
		return false;
	}

	document.ModifyTeamContentForm.city.value = Trim(document.ModifyTeamContentForm.city.value);
	if(LengthOfString(document.ModifyTeamContentForm.city.value) > 10) {
		alert('所屬城巿不能多於10個位元');
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
	<form id="ModifyTeamContentForm" method="post" ONSUBMIT="return CheckLength()" runat="server">
		<b>上次行動:</b><asp:Label id="rtnMsg" runat="server" />
		<table border="1" width="100%">
			<span id="TeamInformation" runat="server" />
			<tr align="right">
				<td align="center" style="background-color:#F5FFFA">
					<a href="TeamManagerModifyFrame.htm" target="content_frame">其他隊伍</a>
				</td>
				<td>
					<font size="2">
					<font color="red">#</font>修改此項資料的同時，系統會自動檢查有沒有相關資訊已被匯入&nbsp;
					<font color="red">*</font>必須選擇
					</font>
					<input type="submit" id="ModifyBtn" value="修改" OnServerClick="ModifyTeamAction" runat="server">
					&nbsp;<input type="reset" value="重設">
				</td>
			</tr>
		</table>
	</form>
	</center>
</body>
</html>