<html>
<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">
	void Page_Load(Object sender,EventArgs e) {
		RegionMenu RegionMenu = new RegionMenu((string)Application["SoccerDBConnectionString"]);
		RegionMatchMenu menu = new RegionMatchMenu((string)Application["SoccerDBConnectionString"]);

		try {
			matchModifyInformation.InnerHtml = menu.Show();
			OtherRegionNameInfo.InnerHtml = RegionMenu.GetName();
			OtherRegionLinkInfo.InnerHtml = RegionMenu.GetLink();
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
function goToPage(idx,pageURL) {
	if(pageURL!='' && pageURL!='0') {
		parent.content_frame.location.replace(pageURL);
	}
	OtherRegionMenuForm.MslotAction.value = '0';
/*
	if (OtherRegionMenuForm.MslotAction.length==1) {
		OtherRegionMenuForm.MslotAction.value = '0';
	} else {
		OtherRegionMenuForm.MslotAction[idx].value = '0';
	}
*/
}

function goToMatchModify(matchModify) {
	if(matchModify!='' && matchModify!='0') {
		parent.content_frame.location.replace('RegionMatchModify.aspx?matchcnt=' + matchModify);
	}

	OtherRegionMenuForm.matchModify.value = '0';
}
</script>

<head>
	<META http-equiv="Content-Type" content="text/html; charset=big5">
	<LINK REL="stylesheet" HREF="/sportStyle.css" TYPE="text/css">
	<title>��|��T - ��L�a��</title>
</head>
<body>
<form name="OtherRegionMenuForm" method="post">
	<asp:Label id="rtnMsg" runat="server" />
	<table width="100%">
		<tr>
			<th align="left">��L�a��</th>
			<td align="left"><a href="../index.htm" target="_top">��^�D��</a></td>
<!--
			<td><font color="#D3D3D3">�D�����]�w</font></td>
-->
			<td>�ק��ɨ�</td>
			<span id="OtherRegionNameInfo" runat="server" />
		</tr>
		<tr>
			<td></td>
			<td><a href="RegionMenu.aspx" target="menu_frame">��s���</a></td>
<!--
			<td>
				<select name="MslotAction" onChange="goToPage(0,OtherRegionMenuForm.MslotAction[0].value)">
					<option value="0">�п��</option>
					<option value="ImportMslot.aspx">�פJ</option>
					<option value="MslotConfig.aspx">�]�w</option>
					<option value="DeleteMslot.aspx">�M��</option>
				</select>
			</td>
-->
			<td>
				<select name="matchModify" onChange="goToMatchModify(OtherRegionMenuForm.matchModify.value)">
					<option value="0">�п��</option>
					<span id="matchModifyInformation" runat="server" />
				</select>
			</td>
			<span id="OtherRegionLinkInfo" runat="server" />
		</tr>
	</table>
</form>
</body>
</html>