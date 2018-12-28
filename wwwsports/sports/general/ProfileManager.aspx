<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">
	int iRecCount;

	void Page_Load(Object sender,EventArgs e) {
		ProfileManager displayProfile = new ProfileManager((string)Application["SoccerDBConnectionString"]);

		try {
			UserProfileInformation.InnerHtml = displayProfile.GetUserProfile();
			iRecCount = displayProfile.NumberOfRecords;
			UpdateHistoryMessage((string)Application["retrieveInfoMsg"] + "�{���p��(" + DateTime.Now.ToString("HH:mm:ss") + ")");
		} catch(NullReferenceException) {
			FormsAuthentication.SignOut();
			UpdateHistoryMessage("<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>");
		}
	}

	void UpdateUserProfile(Object sender,EventArgs e) {
		int[] Updated;
		ProfileManager updateProfile = new ProfileManager((string)Application["SoccerDBConnectionString"]);

		try {
			Updated = updateProfile.Update();
			Page_Load(sender,e);
			if((Updated[0] != -1) && (Updated[1] != -1)) {
				UpdateHistoryMessage("��s�ӤH�ɮסA�i�B�z" + Updated[0] + "���p�ɤΤ��B�z" + Updated[1] + "���p��(" + DateTime.Now.ToString("HH:mm:ss") + ")");
			}	else {
				UpdateHistoryMessage((string)Application["transErrorMsg"] + "(" + DateTime.Now.ToString("HH:mm:ss") + ")");
			}
		}	catch(NullReferenceException nullex) {
			FormsAuthentication.SignOut();
			UpdateHistoryMessage("<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>");
		}
	}

	void UpdateHistoryMessage(string sMsg) {
		historyMsg.Text = sMsg;
	}
</script>

<script language="JavaScript">
function selectAll() {
	<%
		int iSendChkIndex;
		string sSendChk_All;
	%>
	if(UserProfileForm.SelectAllLeague.checked == true) {
		<%
			for(iSendChkIndex=0;iSendChkIndex<iRecCount;iSendChkIndex++) {
				sSendChk_All = "selectedLeague[" + iSendChkIndex.ToString() + "]";
		%>
				UserProfileForm.<%=sSendChk_All%>.checked = true;
		<%
			}
		%>
	}
	else {
		<%
			for(iSendChkIndex=0;iSendChkIndex<iRecCount;iSendChkIndex++) {
				sSendChk_All = "selectedLeague[" + iSendChkIndex.ToString() + "]";
		%>
				UserProfileForm.<%=sSendChk_All%>.checked = false;
		<%
			}
		%>
	}
}
</script>

<html>
<head>
	<META http-equiv="Content-Type" content="text/html; charset=big5">
	<LINK REL="stylesheet" HREF="/sportStyle.css" TYPE="text/css">
</head>
<body>
	<form id="UserProfileForm" method="post" runat="server">
	<center>
		<font size="2"><b>�W�����:</b><asp:Label id="historyMsg" runat="server" /></font>
		<br>
		<table border="1" width="25%" style="font: 10pt verdana">
			<tr align="center" style="background-color:#00bfff">
				<th>
					�p��
				</th>
				<th>
					<input type="checkbox" name="SelectAllLeague" onClick="selectAll()">����
				</th>
			</tr>

			<span id="UserProfileInformation" runat="server" />

			<tr align="right">
				<td colspan="2">
					<input type="submit" id="SelectBtn" value="�x�s" OnServerClick="UpdateUserProfile" runat="server">
				</td>
			</tr>
		</table>
		<input type="hidden" name="RecordCount" value="<%=iRecCount.ToString()%>">
		</center>
	</form>
</body>
</html>