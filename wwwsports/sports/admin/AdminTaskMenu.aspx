<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">
	void Page_Load(Object sender,EventArgs e) {
		string sMenuItems = "";
		string sWebItems = "";
		AdminTaskMenu menu = new AdminTaskMenu((string)Application["SoccerDBConnectionString"]);
		try {
			sMenuItems = menu.Show();
			sWebItems = menu.WebShow();
			ResendInformation.InnerHtml = sMenuItems;
			DeletePagerInformation.InnerHtml = sMenuItems;
			DeleteAllInformation.InnerHtml = sMenuItems;
			DeleteWebInformation.InnerHtml = sWebItems;
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
function goToResend(selectedItem) {
	if(selectedItem!='' && selectedItem!='0') {
		parent.content_frame.location.replace('ResendPagerDetails.aspx?svc=' + selectedItem);
	}

	AdminMenuForm.ResendMenu.value = '0';
}

function goToDelPager(selectedItem) {
	if(selectedItem!='' && selectedItem!='0') {
		parent.content_frame.location.replace('DeletePagerDetails.aspx?svc=' + selectedItem);
	}

	AdminMenuForm.DelPagerMenu.value = '0';
}

function goToDelAll(selectedItem) {
	if(selectedItem!='' && selectedItem!='0') {
		parent.content_frame.location.replace('DeleteAllData.aspx?svc=' + selectedItem);
	}

	AdminMenuForm.DelAllMenu.value = '0';
}

function goToDelWeb(selectedItem) {
	if(selectedItem!='' && selectedItem!='0') {
		parent.content_frame.location.replace('DelWebDetails.aspx?svc=' + selectedItem);
	}

	AdminMenuForm.DelWebMenu.value = '0';
}
</script>

<html>
<head>
	<META http-equiv="Content-Type" content="text/html; charset=big5">
	<LINK REL="stylesheet" HREF="/sportStyle.css" TYPE="text/css">
</head>
<body>
	<form id="AdminMenuForm" method="post" runat="server">
		<asp:Label id="rtnMsg" runat="server" />
		<table width="100%">
			<tr align="left">
				<th>���o/�R����T</th>
				<td><a href="../index.htm" target="_top">��^�D��</a></td>
				<td>���o�ǩI����T</td>
				<td>�R���ǩI����T</td>
				<td>�R���ǩI���κ�����T</td>
				<td>�R��������T</td>
			</tr>
			<tr align="left">
				<td></td>
				<td></td>
				<td>
					<select name="ResendMenu" onChange="goToResend(AdminMenuForm.ResendMenu.value)">
						<option value="0">�п��</option>
						<span id="ResendInformation" runat="server" />
					</select>
				</td>
				<td>
					<select name="DelPagerMenu" onChange="goToDelPager(AdminMenuForm.DelPagerMenu.value)">
						<option value="0">�п��</option>
						<span id="DeletePagerInformation" runat="server" />
					</select>
				</td>
				<td>
					<select name="DelAllMenu" onChange="goToDelAll(AdminMenuForm.DelAllMenu.value)">
						<option value="0">�п��</option>
						<span id="DeleteAllInformation" runat="server" />
					</select>
				</td>
				<td>
					<select name="DelWebMenu" onChange="goToDelWeb(AdminMenuForm.DelWebMenu.value)">
						<option value="0">�п��</option>
						<span id="DeleteWebInformation" runat="server" />
					</select>
				</td>
			</tr>
		</table>
	</form>
</body>
</html>