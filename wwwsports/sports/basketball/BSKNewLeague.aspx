<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">
	void onAddLeague(Object sender,EventArgs e) {
		int iUpdated = 0;
		string sNow;
		BSKNewLeague leag = new BSKNewLeague((string)Application["BasketballDBConnectionString"]);
		sNow = DateTime.Now.ToString("HH:mm:ss");
		try {
			iUpdated = leag.Add(League.Value,Alias.Value);
			if(iUpdated > 0) {
				UpdateReturnMessage("成功新增聯賽(" + sNow + ")");
			}	else if(iUpdated == -99) {
				UpdateReturnMessage("不能新增聯賽，因為此聯賽已存在(" + sNow + ")");
			}	else if(iUpdated == 0) {
				UpdateReturnMessage("不能新增聯賽，因為聯賽名稱有誤(" + sNow + ")");
			}	else {
				UpdateReturnMessage((string)Application["transErrorMsg"] + "(" + sNow + ")");
			}
		} catch(NullReferenceException nullex) {
			FormsAuthentication.SignOut();
			UpdateReturnMessage("<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>");
		} finally {
			League.Value = "";
			Alias.Value = "";
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
	<form id="BSKNewLeagueForm" method="post" runat="server">
		<table border="1" width="40%">
			<tr align="center">
				<th colspan=2>
					新增聯賽
				</th>
			</tr>

			<tr align="center">
				<th><font color="red">*</font>聯賽全名:</th>
				<td align="left"><input type="text" id="League" maxlength="9" runat="server"></td>
				<asp:RequiredFieldValidator
					id="LeagueVal"
					ControlToValidate="League" 
					InitialValue=""
					ErrorMessage="聯賽全名不能空白"
					runat="server" />
			</tr>

			<tr align="center">
				<th><font color="red">*</font>聯賽簡稱:</th>
				<td align="left"><input type="text" id="Alias" maxlength="4" runat="server"></td>
				<asp:RequiredFieldValidator
					id="AliasVal"
					ControlToValidate="Alias" 
					InitialValue=""
					ErrorMessage="聯賽簡稱不能空白"
					runat="server" />
			</tr>

			<tr align="center">
				<th><font color="red">*</font>類別:</th>
				<td align="left">
					<select name="leaguetype">
						<option value="0">賽事聯賽</option>
<!--
						<option value="1">小組排名</option>
						<option value="2">個人統計</option>
-->
					</select>
				</td>
			</tr>
<!--
			<tr align="center">
				<th>舉辦組織:</th>
				<td align="left"><input type="text" name="org" maxlength="20"></td>
			</tr>
-->

			<tr align="right">
				<td colspan="2">
					<font size="2">(有<font color="red">*</font>者必須填寫)</font>
					<input type="submit" id="addBtn" value="新增" OnServerClick="onAddLeague" runat="server">
					&nbsp;<input type="reset" value="重設">
				</td>
			</tr>
		</table>
		<asp:Label id="rtnMsg" runat="server" />
	</form>
	</center>
</body>
</html>