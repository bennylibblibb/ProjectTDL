<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">
	string sAppID;
	void Page_Load(Object sender,EventArgs e) {
		sAppID = Request.QueryString["appID"];
	}

	void ResendAction(Object sender,EventArgs e) {
		int iResult = 0;
		HKJCAdmin hkjc = new HKJCAdmin();

		try {
			iResult = hkjc.NotifyProcess("R", sAppID);
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
	<title>��|��T - ���o���|���y</title>
</head>
<body>
	<form id="HKJCResendForm" method="post" runat="server">
		<b><font color="#FF1493">���o</font>���|���y
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
				<td><input type="checkbox" name="resend" value="resend_odds">�Ҧ��߲v</td>
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
					<input type="button" id="SendBtn" value="���o" OnServerClick="ResendAction" runat="server">&nbsp;
					<input type="reset" value="���]">
				</td>
			</tr>
		</table>
	</form>
</body>
</html>