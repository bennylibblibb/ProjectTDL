<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">
	void Page_Load(Object sender,EventArgs e) {
		ResendPagerDetails pagerItem = new ResendPagerDetails((string)Application["SoccerDBConnectionString"]);
		try {
			ResendItemsInformation.InnerHtml = pagerItem.ShowItems();
		} catch(NullReferenceException) {
			FormsAuthentication.SignOut();
			rtnMsg.Text = "<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>";
		}
	}

	void ResendAction(Object sender,EventArgs e) {
		int iResult = 0;
		ResendPagerDetails resendItem = new ResendPagerDetails((string)Application["SoccerDBConnectionString"]);

		try {
			iResult = resendItem.Resend();
			if(iResult > 0) {
				rtnMsg.Text = "�w���o" + iResult + "�ظ�T";
			}	else if(iResult == 0) {
				rtnMsg.Text = "�S����ܸ�T���o";
			}	else if(iResult == -99) {
				rtnMsg.Text = "�S���v�����o��T";
			}	else {
				rtnMsg.Text = "���o��T���ѡA" + (string)Application["transErrorMsg"];
			}
		} catch(NullReferenceException) {
			FormsAuthentication.SignOut();
			rtnMsg.Text = "<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>";
		}
	}
</script>

<html>
<head>
	<META http-equiv="Content-Type" content="text/html; charset=big5">
	<LINK REL="stylesheet" HREF="/sportStyle.css" TYPE="text/css">
	<title>��|��T - ���o�ǩI����T</title>
</head>
<body>
	<form id="ResendPagerDetailsForm" method="post" runat="server">
		<b><font color="#FF69B4">�u���o�ǩI����T�A�����ƾڤ��|���o</font></b>
		<center>
		<table border="1" width="100%" style="font: 10pt verdana">
			<span id="ResendItemsInformation" runat="server" />

			<tr>
				<td colspan="5"><asp:Label id="rtnMsg" runat="server" /></td>
			</tr>
			<tr>
				<td colspan="5">
					<input type="button" id="SendBtn" value="�ǰe" OnServerClick="ResendAction" runat="server">&nbsp;
					<input type="reset" value="���]">
				</td>
			</tr>
		</table>
		</center>
	</form>
</body>
</html>