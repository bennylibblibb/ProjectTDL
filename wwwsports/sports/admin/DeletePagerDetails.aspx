<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">
	void Page_Load(Object sender,EventArgs e) {
		DeletePagerDetails pagerItem = new DeletePagerDetails((string)Application["SoccerDBConnectionString"]);
		try {
			DeletePagerInformation.InnerHtml = pagerItem.ShowItems();
		} catch(NullReferenceException) {
			FormsAuthentication.SignOut();
			rtnMsg.Text = "<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>";
		}
	}

	void DeleteAction(Object sender,EventArgs e) {
		int iResult = 0;
		DeletePagerDetails deleteItem = new DeletePagerDetails((string)Application["SoccerDBConnectionString"]);

		try {
			iResult = deleteItem.Delete();
			Page_Load(sender,e);
			if(iResult > 0) {
				rtnMsg.Text = "�w�R��" + iResult + "�ظ�T";
			}	else if(iResult == 0) {
				rtnMsg.Text = "�S����ܸ�T�R��";
			}	else if(iResult == -99) {
				rtnMsg.Text = "�S���v���R����T";
			} else {
				rtnMsg.Text = "�R����T���ѡA" + (string)Application["transErrorMsg"];
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
	<title>��|��T - �R���ǩI����T</title>
</head>
<body>
	<form id="DeletePagerDetailsForm" method="post" runat="server">
		<b><font color="#FF7F50">�u�R���ǩI����T�A�����ƾڤ��|�R��</font></b>
		<center>
		<table border="1" width="100%" style="font: 10pt verdana">
			<span id="DeletePagerInformation" runat="server" />

			<tr>
				<td colspan="5"><asp:Label id="rtnMsg" runat="server" /></td>
			</tr>
			<tr>
				<td colspan="5">
					<input type="button" id="SendBtn" value="�ǰe" OnServerClick="DeleteAction" runat="server">&nbsp;
					<input type="reset" value="���]">
				</td>
			</tr>
		</table>
		</center>
	</form>
</body>
</html>