<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">
	void Page_Load(Object sender,EventArgs e) {
		DeleteAllData pagerItem = new DeleteAllData((string)Application["SoccerDBConnectionString"]);
		try {
			DeleteAllInformation.InnerHtml = pagerItem.ShowItems();
		} catch(NullReferenceException) {
			FormsAuthentication.SignOut();
			rtnMsg.Text = "<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>";
		}
	}

	void DeleteAllAction(Object sender,EventArgs e) {
		int iResult = 0;
		DeleteAllData allDelete = new DeleteAllData((string)Application["SoccerDBConnectionString"]);

		try {
			iResult = allDelete.Delete();
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
	<title>��|��T - �R���ǩI���κ�����T</title>
</head>
<body>
	<form id="DeleteAllDataForm" method="post" runat="server">
		<b><font color="#F3A0A0">�R���Ҧ��ǩI���κ�����T�A<font color="#FF0000">�����ƾڱN�����٭�I</font></font></b>
		<center>
		<table border="1" width="100%" style="font: 10pt verdana">
			<span id="DeleteAllInformation" runat="server" />

			<tr>
				<td colspan="5"><asp:Label id="rtnMsg" runat="server" /></td>
			</tr>
			<tr>
				<td colspan="5">
					<input type="button" id="SendBtn" value="�ǰe" OnServerClick="DeleteAllAction" runat="server">&nbsp;
					<input type="reset" value="���]">
				</td>
			</tr>
		</table>
		</center>
	</form>
</body>
</html>