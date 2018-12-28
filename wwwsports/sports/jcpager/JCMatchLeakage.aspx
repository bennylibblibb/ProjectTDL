<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>
<%@ Import Namespace="System.Diagnostics"%>

<script language="C#" runat="server">
	void Page_Load(Object sender,EventArgs e) {
		JCMatchLeakage match = new JCMatchLeakage();
		try {
			JCMatchLeakageInformation.InnerHtml = match.ShowLeakageFiles();
			UpdateReturnMessage((string)Application["retrieveInfoMsg"] + "馬會機遺漏賽事(" + DateTime.Now.ToString("HH:mm:ss") + ")");
		} catch(NullReferenceException) {
			FormsAuthentication.SignOut();
			UpdateReturnMessage("<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>");
		}
	}

	void UpdateReturnMessage(string sMsg) {
		rtnMsg.Text = sMsg;
	}

	void GenerateMatchLeakage(Object sender,EventArgs e) {
		Process.Start("D:\\Sports\\script\\Seek_HKJCMatch_Leakage.vbs");
		//Process.Start("D:\\wwwsports\\sports\\jcpager\\.vbs");
		Page_Load(sender, e);
	}
</script>

<html>
<head>
	<META http-equiv="Content-Type" content="text/html; charset=big5">
	<LINK REL="stylesheet" HREF="/sportStyle.css" TYPE="text/css">
</head>
<body>
	<form id="JCMatchLeakageForm" method="post" runat="server">
		<table width="100%" border="1">
			<tr align="left" style="background-color:#90EE90">
				<td align="center" width="20%">
					<input type="submit" id="RefreshBtn" value="更新版面" OnServerClick="Page_Load" runat="server">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
					<input type="button" id="GenBtn" value="產生檔案" OnServerClick="GenerateMatchLeakage" runat="server">
				</td>
				<th><asp:Label id="rtnMsg" runat="server" /></th>
			</tr>

			<span id="JCMatchLeakageInformation" runat="server" />
		</table>
	</form>
</body>
</html>