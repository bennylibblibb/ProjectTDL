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
	<title>體育資訊 - 其他地區</title>
</head>
<body>
<form name="OtherRegionMenuForm" method="post">
	<asp:Label id="rtnMsg" runat="server" />
	<table width="100%">
		<tr>
			<th align="left">其他地區</th>
			<td align="left"><a href="../index.htm" target="_top">返回主頁</a></td>
<!--
			<td><font color="#D3D3D3">澳門網設定</font></td>
-->
			<td>修改賽事</td>
			<span id="OtherRegionNameInfo" runat="server" />
		</tr>
		<tr>
			<td></td>
			<td><a href="RegionMenu.aspx" target="menu_frame">更新選單</a></td>
<!--
			<td>
				<select name="MslotAction" onChange="goToPage(0,OtherRegionMenuForm.MslotAction[0].value)">
					<option value="0">請選擇</option>
					<option value="ImportMslot.aspx">匯入</option>
					<option value="MslotConfig.aspx">設定</option>
					<option value="DeleteMslot.aspx">清除</option>
				</select>
			</td>
-->
			<td>
				<select name="matchModify" onChange="goToMatchModify(OtherRegionMenuForm.matchModify.value)">
					<option value="0">請選擇</option>
					<span id="matchModifyInformation" runat="server" />
				</select>
			</td>
			<span id="OtherRegionLinkInfo" runat="server" />
		</tr>
	</table>
</form>
</body>
</html>