<%@ Page EnableViewState="false"%>

<script language="JavaScript">
function selectLeagueMode(selectedItem) {
	if((selectedItem !="") && (selectedItem != "0")) {
		if(selectedItem == "1")
			parent.content_frame.location.replace('LeagueManager_ADD.aspx');
		if(selectedItem == "2")
			parent.content_frame.location.replace('LeagueManagerModifyFrame.htm');
		if(selectedItem == "3")
			parent.content_frame.location.replace('LeagueManagerDeleteFrame.htm');
	}
	document.GeneralMenuForm.LeagueManager.value = "0";
}

function selectTeamMode(selectedItem) {
	if((selectedItem !="") && (selectedItem != "0")) {
		if(selectedItem == "1")
			parent.content_frame.location.replace('TeamManager_ADD.aspx');
		if(selectedItem == "2")
			parent.content_frame.location.replace('TeamManagerModifyFrame.htm');
		if(selectedItem == "3")
			parent.content_frame.location.replace('TeamManagerDeleteFrame.htm');
	}
	document.GeneralMenuForm.TeamManager.value = "0";
}
</script>

<html>
<head>
	<META http-equiv="Content-Type" content="text/html; charset=big5">
	<LINK REL="stylesheet" HREF="/sportStyle.css" TYPE="text/css">
</head>
<body>
	<form id="GeneralMenuForm" method="post" runat="server">
		<table width="70%">
			<tr align="center">
				<th>一般設定</th>
				<td><a href="../index.htm" target="_top">返回主頁</a></td>
				<td><a href="ProfileManager.aspx" target="content_frame">個人處理<br>聯賽項目</a></td>
				<td><a href="SortLeague.aspx" target="content_frame">修改<br>聯賽序號</a></td>
				<td>聯賽管理</td>
				<td>隊伍管理</td>
				<td><a href="BlankMessageManager.aspx" target="content_frame">取代<br>[沒有訊息]</a></td>
			</tr>
			<tr align="center">
				<td colspan="4"></td>
				<td>
					<select name="LeagueManager" onchange="selectLeagueMode(document.GeneralMenuForm.LeagueManager.value)">
						<option value="0">請選擇</option>
						<option value="1">新增</option>
						<option value="2">修改</option>
						<option value="3">刪除</option>
					</select>
				</td>
				<td>
					<select name="TeamManager" onchange="selectTeamMode(document.GeneralMenuForm.TeamManager.value)">
						<option value="0">請選擇</option>
						<option value="1">新增</option>
						<option value="2">修改</option>
						<option value="3">刪除</option>
					</select>
				</td>
			</tr>
		</table>
	</form>
</body>
</html>