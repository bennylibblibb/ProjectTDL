<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">
	void Page_Load(Object sender,EventArgs e) {
		BlankMessageManager BlankMessageForm = new BlankMessageManager((string)Application["SoccerDBConnectionString"]);
		try {
			BlankMessageInformation.InnerHtml = BlankMessageForm.GetMessage();
		} catch(NullReferenceException) {
			FormsAuthentication.SignOut();
			UpdateReturnMessage("<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>");
		}
	}

	void updateBlankMessage(Object sender,EventArgs e) {
		int iUpdated = 0;
		string sNow;
		BlankMessageManager BlankMsg = new BlankMessageManager((string)Application["SoccerDBConnectionString"]);
		sNow = DateTime.Now.ToString("HH:mm:ss");
		try {
			iUpdated = BlankMsg.SetMessage();
			Page_Load(sender,e);
			if(iUpdated > 0) {
				UpdateReturnMessage("修改成功(" + sNow + ")");
			}	else if(iUpdated == 0) {
				UpdateReturnMessage("修改無效(" + sNow + ")");
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

<html>
<head>
	<META http-equiv="Content-Type" content="text/html; charset=big5">
	<LINK REL="stylesheet" HREF="/sportStyle.css" TYPE="text/css">
</head>
<body>
	<center>
	<form id="BlankMessageForm" method="post" runat="server">
		<table border="1" width="30%">
			<tr align="center" style="background-color:#F5F5DC">
				<th colspan=2>更新訊息以取代[沒有訊息]</th>
			</tr>

			<span id="BlankMessageInformation" runat="server" />

			<tr align="right">
				<td colspan="2">
					<!--
						Value of SendToPager is Device ID defined in DEVICE_TYPE
					-->
					<input type="hidden" name="SendToPager" value="1">
					<input type="hidden" name="SendToPager" value="2">
					<input type="hidden" name="SendToPager" value="5">
					<input type="submit" id="updBtn" value="更新" OnServerClick="updateBlankMessage" runat="server">
					&nbsp;<input type="reset" value="重設">
				</td>
			</tr>
		</table>
		<asp:Label id="rtnMsg" runat="server" />
	</form>
	</center>
</body>
</html>