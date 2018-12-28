<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">
	string sDevice;

	void Page_Load(Object sender,EventArgs e) {
		string sTarget = (string)Request.QueryString["target"];
		switch(sTarget) {
			case "gogo1":
				sDevice = "GOGO1��";
				break;
			case "gogo2":
				sDevice = "GOGO2��";
				break;
			case "hkjc":
				sDevice = "���|��";
				break;
			case "jccombo":
				sDevice = "JCCombo";
				break;
			default:
				sDevice = "ERROR";
				break;
		}

		ModifyResultMenu menu = new ModifyResultMenu();
		try {
			ResultInformation.InnerHtml = menu.Show();
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
function goToModifyResultContent(url) {
	if(url!='' && url!='0') {
		parent.subcontent_frame.location.replace(url);
	}

	ModifyResultForm.MatchID.value = '0';
}
</script>

<html>
<head>
	<META http-equiv="Content-Type" content="text/html; charset=big5">
	<LINK REL="stylesheet" HREF="/sportStyle.css" TYPE="text/css">
</head>
<body>
	<center>
	<form id="ModifyResultForm" method="post" runat="server">
		<asp:Label id="rtnMsg" runat="server" />
		<table border="1" width="50%">
			<tr style="background-color:#F5F5DC">
				<th align="right">�п��<font color="#800080"><%=sDevice%></font>�ɪG�@�ק�G</th>
				<td>
					<select name="MatchID" onChange="goToModifyResultContent(ModifyResultForm.MatchID.value)">
						<option value="0">�п��</option>
						<span id="ResultInformation" runat="server" />
					</select>
				</td>
			</tr>
		</table>
	</form>
	</center>
</body>
</html>