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
				UpdateReturnMessage("���\�s�W����(" + sNow + ")");
			}	else if(iUpdated == -99) {
				UpdateReturnMessage("����s�W����A�]��������w�s�b(" + sNow + ")");
			}	else if(iUpdated == 0) {
				UpdateReturnMessage("����s�W����A�]������W�٦��~(" + sNow + ")");
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
					�s�W����
				</th>
			</tr>

			<tr>
				<th align="center"><font color="red">*</font>����W��:</th>
				<td><input type="text" id="team" maxlength="4" runat="server"></td>
				<asp:RequiredFieldValidator
					id="teamVal"
					ControlToValidate="team" 
					InitialValue=""
					ErrorMessage="����W�٤���ť�"
					runat="server" />
			</tr>

			<tr>
				<th align="center"><font color="red">*</font>�����p��:</th>
				<td>
					<span id="TeamInformation" runat="server" />
				</td>
			</tr>

			<tr>
				<th align="center">���ݬw��:</th>
				<td><input type="text" name="continent" maxlength="4"></td>
			</tr>

			<tr>
				<th align="center">���ݰ�a:</th>
				<td><input type="text" name="country" maxlength="5"></td>
			</tr>

			<tr>
				<th align="center">���ݫ��]:</th>
				<td><input type="text" name="city" maxlength="5"></td>
			</tr>

			<tr>
				<th align="center">�D���W��:</th>
				<td><input type="text" name="venue" maxlength="10"></td>
			</tr>

			<tr align="right">
				<td colspan="2">
					<font size="2">(��<font color="red">*</font>�̥�����g)</font>
					<input type="submit" id="AddBtn" value="�s�W" OnServerClick="AddLeagueAction" runat="server">
					&nbsp;<input type="reset" value="���]">
				</td>
			</tr>
		</table>
		<asp:Label id="rtnMsg" runat="server" />
	</form>
	</center>
</body>
</html>