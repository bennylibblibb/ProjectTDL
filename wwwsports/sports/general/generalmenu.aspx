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
				<th>�@��]�w</th>
				<td><a href="../index.htm" target="_top">��^�D��</a></td>
				<td><a href="ProfileManager.aspx" target="content_frame">�ӤH�B�z<br>�p�ɶ���</a></td>
				<td><a href="SortLeague.aspx" target="content_frame">�ק�<br>�p�ɧǸ�</a></td>
				<td>�p�ɺ޲z</td>
				<td>����޲z</td>
				<td><a href="BlankMessageManager.aspx" target="content_frame">���N<br>[�S���T��]</a></td>
			</tr>
			<tr align="center">
				<td colspan="4"></td>
				<td>
					<select name="LeagueManager" onchange="selectLeagueMode(document.GeneralMenuForm.LeagueManager.value)">
						<option value="0">�п��</option>
						<option value="1">�s�W</option>
						<option value="2">�ק�</option>
						<option value="3">�R��</option>
					</select>
				</td>
				<td>
					<select name="TeamManager" onchange="selectTeamMode(document.GeneralMenuForm.TeamManager.value)">
						<option value="0">�п��</option>
						<option value="1">�s�W</option>
						<option value="2">�ק�</option>
						<option value="3">�R��</option>
					</select>
				</td>
			</tr>
		</table>
	</form>
</body>
</html>