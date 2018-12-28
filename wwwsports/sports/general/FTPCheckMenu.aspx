<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">
	void Page_Load(Object sender,EventArgs e) {
		string sCheckOption = "";
		FTPCheckMenu checkMenu = new FTPCheckMenu((string)Application["SoccerDBConnectionString"]);
		try {
			sCheckOption = checkMenu.Show();
			CheckInformation.InnerHtml = sCheckOption;
		} catch(NullReferenceException) {
			FormsAuthentication.SignOut();
			UpdateReturnMessage("<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>");
		}
	}

	void UpdateReturnMessage(string sMsg) {
		rtnMsg.Text = sMsg;
	}
</script>

<script language="JavaScript">
function goToCheck(sSelectedCheck) {
	if(sSelectedCheck!='' && sSelectedCheck!='0') {
		if(sSelectedCheck == '1') {
			parent.content_frame.location.replace('FTPScheduleCheck.aspx');
		} else if(sSelectedCheck == '2') {
			parent.content_frame.location.replace('FTPMatchCheck.aspx');
		} else if(sSelectedCheck == '3') {
			parent.content_frame.location.replace('FTPOtherMatchCheck.aspx');
		}
	}

	FTPCheckMenuForm.checkMenuOption.value = '0';
}
</script>

<html>
<head>
	<META http-equiv="Content-Type" content="text/html; charset=big5">
	<LINK REL="stylesheet" HREF="/sportStyle.css" TYPE="text/css">
</head>
<body>
	<form id="FTPCheckMenuForm" method="post" runat="server">
		<asp:Label id="rtnMsg" runat="server" />
		<table width="60%">
			<tr align="center">
				<th>FTP 檢查</th>
				<td><a href="../index.htm" target="_top">返回主頁</a></td>
				<td>檢查選擇</td>
			</tr>
			<tr align="center">
				<td></td>
				<td><a href="redirectToFTPMenu.htm" target="menu_frame">更新選單</a></td>
				<td>
					<select name="checkMenuOption" onChange="goToCheck(FTPCheckMenuForm.checkMenuOption.value)">
						<option value="0">請選擇</option>
						<span id="CheckInformation" runat="server" />
					</select>
				</td>
			</tr>
		</table>
	</form>
</body>
</html>