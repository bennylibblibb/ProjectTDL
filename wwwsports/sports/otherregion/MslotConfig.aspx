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
			rtnMsg.Text = "��s�]�w���\";
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
	<title>��|��T - ��L�a��</title>
</head>
<body>
	<form id="MslotConfigForm" method="post" runat="server">
		<h3>�D�����]�w</h3>
		<asp:Label id="rtnMsg" runat="server" />
		<table border="1" width="30%">
			<tr>
				<th align="right">���\�۰ʵo�e�D�����ƾ�:</th>
				<td>
					<span id="MslotInformation" runat="server" />
				</td>
			</tr>
			<tr align="right">
				<td colspan=2>
					<input type="button" id="SaveBtn" value="�x�s" OnServerClick="ModifyMslotCFGAction" runat="server">&nbsp;
					<input type="reset" value="���]">
				</td>
			</tr>
		</table>
	</form>
</body>
</html>