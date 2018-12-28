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
				if (sLeagRefMsg.Length == 0) UpdateReturnMessage("修改成功(" + sNow + ")");
				else UpdateReturnMessage("修改成功(" + sNow + ") <br>"+sLeagRefMsg);
			}	else if(iUpdated == 0) {
				UpdateReturnMessage("無效修改(" + sNow + ")");
			}	else {
				UpdateReturnMessage((string)Application["transErrorMsg"] + "，或相關名稱已存在！ (" + sNow + ")");
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
		alert('聯賽名稱(亞洲)不能空白');
		return false;
	}
	if(LengthOfString(document.ModifyLeagueForm.leagueName.value) > 10) {
		alert('聯賽名稱(亞洲)不能多於10個位元');
		return false;
	}

	document.ModifyLeagueForm.alias.value = Trim(document.ModifyLeagueForm.alias.value);
	if(document.ModifyLeagueForm.alias.value == '') {
		alert('聯賽簡稱(亞洲)不能空白');
		return false;
	}
	if(LengthOfString(document.ModifyLeagueForm.alias.value) > 6) {
		alert('聯賽簡稱(亞洲)不能多於6個位元');
		return false;
	}

	document.ModifyLeagueForm.HKJCName.value = Trim(document.ModifyLeagueForm.HKJCName.value);
	if(document.ModifyLeagueForm.HKJCName.value == '') {
		alert('馬會名稱不能空白');
		return false;
	}
	if(LengthOfString(document.ModifyLeagueForm.HKJCName.value) > 40) {
		alert('馬會名稱不能多於40個位元');
		return false;
	}

	document.ModifyLeagueForm.HKJCNameAlias.value = Trim(document.ModifyLeagueForm.HKJCNameAlias.value);
	if(document.ModifyLeagueForm.HKJCNameAlias.value == '') {
		alert('馬會簡稱不能空白');
		return false;
	}
	if(LengthOfString(document.ModifyLeagueForm.HKJCNameAlias.value) > 10) {
		alert('馬會簡稱不能多於10個位元');
		return false;
	}

	document.ModifyLeagueForm.MCLeague.value = Trim(document.ModifyLeagueForm.MCLeague.value);
	if(document.ModifyLeagueForm.MCLeague.value == '') {
		alert('澳門名稱不能空白');
		return false;
	}
	if(LengthOfString(document.ModifyLeagueForm.MCLeague.value) > 40) {
		alert('澳門名稱不能多於40個位元');
		return false;
	}

	document.ModifyLeagueForm.engName.value = Trim(document.ModifyLeagueForm.engName.value);
	if(LengthOfString(document.ModifyLeagueForm.engName.value) > 50) {
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
	<form id="ModifyLeagueForm" method="post" ONSUBMIT="return CheckLength()" runat="server">
		<b>上次行動:</b><asp:Label id="rtnMsg" runat="server" />
		<table border="1" width="70%">
			<tr align="center" style="background-color:#6FAFB1">
				<th colspan=2><font color="#F0F8FF">修改聯賽</font></th>
			</tr>

			<span id="LeagueInformation" runat="server" />

			<tr align="right">
				<td align="center" style="background-color:#F5FFFA">
					<a href="LeagueManagerModifyFrame.htm" target="content_frame">其他聯賽</a>
				</td>
				<td>
					<font size="2">
					<font color="red">#</font>修改此項資料的同時，系統會自動檢查有沒有相關資訊已被匯入
					</font>
					<input type="submit" id="modifyBtn" value="修改" OnServerClick="ModifyLeagueAction" runat="server">
					&nbsp;<input type="reset" value="重設">
				</td>
			</tr>
		</table>
	</form>
	</center>
</body>
</html>