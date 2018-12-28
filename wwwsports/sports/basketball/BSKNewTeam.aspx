<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">
	void Page_Load(Object sender,EventArgs e) {
		BSKNewTeam team = new BSKNewTeam((string)Application["BasketballDBConnectionString"]);
		try {
			TeamInformation.InnerHtml = team.GetLeagues();
		} catch(NullReferenceException) {
			FormsAuthentication.SignOut();
			UpdateReturnMessage("<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>");
		}
	}

	void AddLeagueAction(Object sender,EventArgs e) {
		int iUpdated = 0;
		string sNow;
		BSKNewTeam teamUpdate = new BSKNewTeam((string)Application["BasketballDBConnectionString"]);
		sNow = DateTime.Now.ToString("HH:mm:ss");
		try {
			iUpdated = teamUpdate.Add(team.Value);
			Page_Load(sender,e);
			if(iUpdated > 0) {
				UpdateReturnMessage("成功新增隊伍(" + sNow + ")");
			}	else if(iUpdated == -99) {
				UpdateReturnMessage("不能新增隊伍，因為此隊伍已存在(" + sNow + ")");
			}	else if(iUpdated == 0) {
				UpdateReturnMessage("不能新增隊伍，因為隊伍名稱有誤(" + sNow + ")");
			}	else {
				UpdateReturnMessage((string)Application["transErrorMsg"] + "(" + sNow + ")");
			}
		} catch(NullReferenceException nullex) {
			FormsAuthentication.SignOut();
			UpdateReturnMessage("<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>");
		}	finally {
	    team.Value = "";
    }
	}

	void UpdateReturnMessage(string sMsg) {
		rtnMsg.Text = sMsg;
	}
</script>

<html>
<head>
	<META http-equiv="Content-Type" content="text/html; charset=big5">
	<LINK REL="stylesheet" HREF="/sportStyle.css" TYPE="text/css">
</head>
<body>
	<center>
	<form id="BSKNewTeamForm" method="post" runat="server">
		<table border="1" width="40%">
			<tr align="center">
				<th colspan="2">
					新增隊伍
				</th>
			</tr>

			<tr>
				<th align="center"><font color="red">*</font>隊伍名稱:</th>
				<td><input type="text" id="team" maxlength="4" runat="server"></td>
				<asp:RequiredFieldValidator
					id="teamVal"
					ControlToValidate="team" 
					InitialValue=""
					ErrorMessage="隊伍名稱不能空白"
					runat="server" />
			</tr>

			<tr>
				<th align="center"><font color="red">*</font>所屬聯賽:</th>
				<td>
					<span id="TeamInformation" runat="server" />
				</td>
			</tr>

			<tr>
				<th align="center">所屬洲份:</th>
				<td><input type="text" name="continent" maxlength="4"></td>
			</tr>

			<tr>
				<th align="center">所屬國家:</th>
				<td><input type="text" name="country" maxlength="5"></td>
			</tr>

			<tr>
				<th align="center">所屬城巿:</th>
				<td><input type="text" name="city" maxlength="5"></td>
			</tr>

			<tr>
				<th align="center">主場名稱:</th>
				<td><input type="text" name="venue" maxlength="10"></td>
			</tr>

			<tr align="right">
				<td colspan="2">
					<font size="2">(有<font color="red">*</font>者必須填寫)</font>
					<input type="submit" id="AddBtn" value="新增" OnServerClick="AddLeagueAction" runat="server">
					&nbsp;<input type="reset" value="重設">
				</td>
			</tr>
		</table>
		<asp:Label id="rtnMsg" runat="server" />
	</form>
	</center>
</body>
</html>