<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">
	void Page_Load(Object sender,EventArgs e) {
		
	}

	void UpdateReturnMessage(string sMsg) {
		rtnMsg.Text = sMsg;
	}
</script>

<script language="JavaScript">
function goToModifyResult(url) {
	if(url!='' && url!='0') {
		parent.content_frame.location.replace(url);
	}

	SoccerMenuForm.ModifyResult.value = '0';
}
</script>

<html>
<head>
	<META http-equiv="Content-Type" content="text/html; charset=big5">
	<LINK REL="stylesheet" HREF="/sportStyle.css" TYPE="text/css">
</head>
<body>
	<form id="SoccerMenuForm" method="post" runat="server">
		<asp:Label id="rtnMsg" runat="server" />
		<table width="40%">
			<tr align="left">
				<th>足球</th>
				<td><a href="redirectToMenu2.htm" target="menu_frame">&lt;&lt;</a></td>
				<td><a href="../index.htm" target="_top">返回主頁</a></td>
				<td>修改賽果</td>
				<td>新增賽果</td>
			</tr>
			<tr align="left">
				<td></td><td></td>
				<td><a href="redirectToMenu3.htm" target="menu_frame">更新選單</a></td>
				<td>
					<select name="ModifyResult" onChange="goToModifyResult(SoccerMenuForm.ModifyResult.value)">
						<option value="0">請選擇</option>
						<option value="ModifyResultFrame.aspx?tar=gogo1">GOGO1機</option>
						<option value="ModifyResultFrame.aspx?tar=gogo2">GOGO2機</option>
						<option value="ModifyResultFrame.aspx?tar=hkjc">馬會機</option>
						<option value="ModifyResultFrame.aspx?tar=jccombo">JCCombo</option>
					</select>
				</td>
			</tr>
		</table>
	</form>
</body>
</html>