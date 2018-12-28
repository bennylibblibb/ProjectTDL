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
				UpdateReturnMessage("���\�s�W�p��(" + sNow + ")");
			}	else if(iUpdated == -99) {
				UpdateReturnMessage("����s�W�p�ɡA�]�����p�ɤw�s�b(" + sNow + ")");
			}	else if(iUpdated == 0) {
				UpdateReturnMessage("����s�W�p�ɡA�]���p�ɦW�٦��~(" + sNow + ")");
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
					�s�W�p��
				</th>
			</tr>

			<tr align="center">
				<th><font color="red">*</font>�p�ɥ��W:</th>
				<td align="left"><input type="text" id="League" maxlength="9" runat="server"></td>
				<asp:RequiredFieldValidator
					id="LeagueVal"
					ControlToValidate="League" 
					InitialValue=""
					ErrorMessage="�p�ɥ��W����ť�"
					runat="server" />
			</tr>

			<tr align="center">
				<th><font color="red">*</font>�p��²��:</th>
				<td align="left"><input type="text" id="Alias" maxlength="4" runat="server"></td>
				<asp:RequiredFieldValidator
					id="AliasVal"
					ControlToValidate="Alias" 
					InitialValue=""
					ErrorMessage="�p��²�٤���ť�"
					runat="server" />
			</tr>

			<tr align="center">
				<th><font color="red">*</font>���O:</th>
				<td align="left">
					<select name="leaguetype">
						<option value="0">�ɨ��p��</option>
<!--
						<option value="1">�p�ձƦW</option>
						<option value="2">�ӤH�έp</option>
-->
					</select>
				</td>
			</tr>
<!--
			<tr align="center">
				<th>�|���´:</th>
				<td align="left"><input type="text" name="org" maxlength="20"></td>
			</tr>
-->

			<tr align="right">
				<td colspan="2">
					<font size="2">(��<font color="red">*</font>�̥�����g)</font>
					<input type="submit" id="addBtn" value="�s�W" OnServerClick="onAddLeague" runat="server">
					&nbsp;<input type="reset" value="���]">
				</td>
			</tr>
		</table>
		<asp:Label id="rtnMsg" runat="server" />
	</form>
	</center>
</body>
</html>