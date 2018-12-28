<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">
	string sAppID;
	void Page_Load(Object sender,EventArgs e) {
		sAppID = Request.QueryString["appID"];
	}

	void DeleteAction(Object sender,EventArgs e) {
		int iResult = 0;
		HKJCAdmin hkjc = new HKJCAdmin();

		try {
			iResult = hkjc.NotifyProcess("D", sAppID);
			if(iResult > 0) {
				rtnMsg.Text = "�w�R��" + iResult + "�ظ�T";
			}	else if(iResult == 0) {
				rtnMsg.Text = "�S����ܸ�T�R��";
			}	else if(iResult == -99) {
				rtnMsg.Text = "�S���v���R����T";
			}	else {
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
	<title>��|��T - �R�����|���y</title>
</head>
<body>
	<form id="HKJCDeleteForm" method="post" runat="server">
		<b><font color="#FF0000">�R��</font>���|���y
		<%
			if(sAppID.Equals("05")) {
		%>
			(���y2 -> ���a)
		<%
			} else if(sAppID.Equals("08")) {
		%>
			(���|��)
		<%
			} else {
		%>
			(���|WAP)
		<%
			}
		%>
		��T</b>
		<table border="1" width="100%" style="font: 10pt verdana">
			<tr>
				<td><input type="checkbox" name="delete" value="clr_tbl_odds">�Ҧ��߲v</td>
				<td></td>
				<td></td>
				<td></td>
				<td></td>
			</tr>
			<tr>
				<td colspan="5"><asp:Label id="rtnMsg" runat="server" /></td>
			</tr>
			<tr>
				<td colspan="5">
					<input type="button" id="SendBtn" value="�R��" OnServerClick="DeleteAction" runat="server">&nbsp;
					<input type="reset" value="���]">
				</td>
			</tr>
		</table>
	</form>
</body>
</html>