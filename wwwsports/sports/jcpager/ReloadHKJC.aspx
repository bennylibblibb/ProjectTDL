<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">
	string sAppID;
	void Page_Load(Object sender,EventArgs e) {
		sAppID = Request.QueryString["appID"];
	}

	void SendAction(Object sender,EventArgs e) {
		int iResult = 0;
		HKJCAdmin hkjc = new HKJCAdmin();

		try {
			iResult = hkjc.NotifyProcess("L", sAppID);
			if(iResult > 0) {
				rtnMsg.Text = "�w���s���J" + iResult + "�ظ�T";
			}	else if(iResult == 0) {
				rtnMsg.Text = "�S����ܸ�T���s���J";
			}	else if(iResult == -99) {
				rtnMsg.Text = "�S���v�����s���J��T";
			}	else {
				rtnMsg.Text = "���s���J��T���ѡA" + (string)Application["transErrorMsg"];
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
	<title>��|��T - �������|���y</title>
</head>
<body>
	<form id="HKJCReloadForm" method="post" runat="server">
		<b><font color="#32CD32">���s���J</font>���|���y
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
				<!--
				<td><input type="checkbox" name="reload" value="reload_to_pager">���|���ƾڮw�ζǰe�차�|��[ID:98]</td>
				<td></td>
				-->
				<!--
				<td><input type="checkbox" name="reload" value="reload_to_db2">���|���y�ƾڮw</td>
				-->
				<td><input type="checkbox" name="reload" value="sync_team">����W��[ID:96]</td>
				<td></td>
				<td></td>
			</tr>
			<tr>
				<td colspan="5"><asp:Label id="rtnMsg" runat="server" /></td>
			</tr>
			<tr>
				<td colspan="5">
					<input type="button" id="SendBtn" value="���s���J" OnServerClick="SendAction" runat="server">&nbsp;
					<input type="reset" value="���]">
				</td>
			</tr>
		</table>
	</form>
</body>
</html>