<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">
	void Page_Load(Object sender,EventArgs e) {
		DelWebDetails pagerItem = new DelWebDetails((string)Application["SoccerDBConnectionString"]);
		try {
			DeleteWebInformation.InnerHtml = pagerItem.ShowItems();
		} catch(NullReferenceException) {
			FormsAuthentication.SignOut();
			rtnMsg.Text = "<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>";
		}
	}

	void DeleteWebAction(Object sender,EventArgs e) {
		int iResult = 0;
		DelWebDetails deleteItem = new DelWebDetails((string)Application["SoccerDBConnectionString"]);

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
	<title>��|��T - �R��������T</title>
</head>
<body>
	<form id="DeleteWebForm" method="post" runat="server">
		<b><font color="#F3A0A0">�R���Ҧ�������T�A<font color="#FF0000">�����ƾڱN�����٭�I</font></font></b>
		<center>
		<table border="1" width="100%" style="font: 10pt verdana">
			<span id="DeleteWebInformation" runat="server" />

			<tr>
				<td colspan="5"><asp:Label id="rtnMsg" runat="server" /></td>
			</tr>
			<tr>
				<td colspan="5">
					<input type="button" id="SendBtn" value="�R��" OnServerClick="DeleteWebAction" runat="server">&nbsp;
					<input type="reset" value="���]">
				</td>
			</tr>
		</table>
		</center>
	</form>
</body>
</html>