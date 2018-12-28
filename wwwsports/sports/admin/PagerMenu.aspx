<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">
	void Page_Load(Object sender,EventArgs e) {
		PagerMenu menu = new PagerMenu((string)Application["SoccerDBConnectionString"]);
		try {
			MenuInformation.InnerHtml = menu.GetMenuItems();
		}	catch(NullReferenceException) {
			FormsAuthentication.SignOut();
			rtnMsg.Text = "<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>";
		}
	}

	void SendMenuAction(Object sender,EventArgs e) {
		int iResult = 0;
		PagerMenu sendMenu = new PagerMenu((string)Application["SoccerDBConnectionString"]);

		try {
			iResult = sendMenu.Resend();
			Page_Load(sender,e);
			if(iResult > 0) {
				rtnMsg.Text = "�w�o�e���";
			}	else if(iResult == 0) {
				rtnMsg.Text = "�S����ܵ��@���o";
			}	else if(iResult == -99) {
				rtnMsg.Text = "�S���v�����o���";
			}	else {
				rtnMsg.Text = "���o��楢��";
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
	<title>��|��T - ���o���</title>
</head>
<body>
	<center>
	<b><font color="red">�o�e�e�Ъ`�N�t�Ϊ��p</font></b>
	<form id="ResendMenuForm" method="post" runat="server">
		<table border="1" width="100%" style="font: 10pt verdana">
			<span id="MenuInformation" runat="server" />
			<tr>
				<td colspan="2">
					<input type="button" id="SendBtn" value="�ǰe" OnServerClick="SendMenuAction" runat="server">&nbsp;
					<input type="reset" value="���]">
				</td>
			</tr>
		</table>
		<asp:Label id="rtnMsg" runat="server" />
		</form>
	</center>
</body>
</html>