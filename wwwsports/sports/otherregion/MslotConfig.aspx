<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">
	void Page_Load(Object sender,EventArgs e) {
		string sFlag;
		MslotConfig mslotcfg = new MslotConfig();
		try {
			MslotInformation.InnerHtml = mslotcfg.GetXMLConfig((String)Application["XMLConfigPath"],(String)Application["INIEnableItem"]);
		} catch(NullReferenceException) {
			FormsAuthentication.SignOut();
			rtnMsg.Text = "<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>";
		}
	}

	void ModifyMslotCFGAction(Object sender,EventArgs e) {
		string sFlag;
		MslotConfig mslotupd = new MslotConfig();
		try {
			sFlag = mslotupd.SetXMLConfig((String)Application["XMLConfigPath"],(String)Application["INIEnableItem"]);
			Page_Load(sender,e);
			rtnMsg.Text = "更新設定成功";
		}	catch(NullReferenceException) {
			FormsAuthentication.SignOut();
			rtnMsg.Text = "<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>";
		}
	}
</script>

<html>
<head>
	<META http-equiv="Content-Type" content="text/html; charset=big5">
	<LINK REL="stylesheet" HREF="/sportStyle.css" TYPE="text/css">
	<title>體育資訊 - 其他地區</title>
</head>
<body>
	<form id="MslotConfigForm" method="post" runat="server">
		<h3>澳門網設定</h3>
		<asp:Label id="rtnMsg" runat="server" />
		<table border="1" width="30%">
			<tr>
				<th align="right">允許自動發送澳門網數據:</th>
				<td>
					<span id="MslotInformation" runat="server" />
				</td>
			</tr>
			<tr align="right">
				<td colspan=2>
					<input type="button" id="SaveBtn" value="儲存" OnServerClick="ModifyMslotCFGAction" runat="server">&nbsp;
					<input type="reset" value="重設">
				</td>
			</tr>
		</table>
	</form>
</body>
</html>