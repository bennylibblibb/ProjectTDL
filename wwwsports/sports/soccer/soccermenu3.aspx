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
				<th>���y</th>
				<td><a href="redirectToMenu2.htm" target="menu_frame">&lt;&lt;</a></td>
				<td><a href="../index.htm" target="_top">��^�D��</a></td>
				<td>�ק��ɪG</td>
				<td>�s�W�ɪG</td>
			</tr>
			<tr align="left">
				<td></td><td></td>
				<td><a href="redirectToMenu3.htm" target="menu_frame">��s���</a></td>
				<td>
					<select name="ModifyResult" onChange="goToModifyResult(SoccerMenuForm.ModifyResult.value)">
						<option value="0">�п��</option>
						<option value="ModifyResultFrame.aspx?tar=gogo1">GOGO1��</option>
						<option value="ModifyResultFrame.aspx?tar=gogo2">GOGO2��</option>
						<option value="ModifyResultFrame.aspx?tar=hkjc">���|��</option>
						<option value="ModifyResultFrame.aspx?tar=jccombo">JCCombo</option>
					</select>
				</td>
			</tr>
		</table>
	</form>
</body>
</html>