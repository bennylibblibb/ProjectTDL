<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">
	void Page_Load(Object sender,EventArgs e) {
		HorseRacingMenu RaceMenu = new HorseRacingMenu((string)Application["SoccerDBConnectionString"]);
		try {
			RaceInformation.InnerHtml = RaceMenu.GetMenu();
		}	catch(NullReferenceException) {
			FormsAuthentication.SignOut();
			UpdateReturnMessage("<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>");
		}
	}

	void UpdateReturnMessage(string sMsg) {
		rtnMsg.Text = sMsg;
	}
</script>

<script language="JavaScript">
function goToRace(selectedRace) {
	if(selectedRace!='' && selectedRace!='0') {
		parent.content_frame.location.replace('HorseLivePlace.aspx?RaceID=' + selectedRace);
	}

	HorseRacingMenuForm.HorseRacingLivePlace.value = '0';
}
</script>

<html>
<head>
	<META http-equiv="Content-Type" content="text/html; charset=big5">
	<LINK REL="stylesheet" HREF="/sportStyle.css" TYPE="text/css">
</head>
<body>
	<form id="HorseRacingMenuForm" method="post" runat="server">
		<asp:Label id="rtnMsg" runat="server" />
		<table width="50%">
			<tr align="left">
				<th>賽馬資訊</th>
				<td><a href="../index.htm" target="_top">返回主頁</a></td>
				<td><a href="HorseRacingInfo.aspx" target="content_frame">新增賽事</a></td>
				<td>現場走位</td>
			</tr>
			<tr align="left">
				<th></th>
				<td colspan="2">
					<a href="redirectToMenu.htm" target="menu_frame">更新選單</a>
				</td>
				<td colspan="2">
					<select name="HorseRacingLivePlace" onChange="goToRace(HorseRacingMenuForm.HorseRacingLivePlace.value)">
						<option value="0">請選擇</option>
						<span id="RaceInformation" runat="server" />
					</select>
				</td>
			</tr>
		</table>
	</form>
</body>
</html>