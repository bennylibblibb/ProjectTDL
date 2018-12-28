<html>
<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">
	void Page_Load(Object sender,EventArgs e) {
		LiveOddsOrder order = new LiveOddsOrder((string)Application["SoccerDBConnectionString"]);

		try {
			LiveoddsNameInfo.InnerHtml = order.ShowRegion();
			UpdateHistoryMessage((string)Application["retrieveInfoMsg"] + "�U�a�Ǹ�(" + DateTime.Now.ToString("HH:mm:ss") + ")");
		}	catch(NullReferenceException) {
			FormsAuthentication.SignOut();
			UpdateHistoryMessage("<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>");
		}
	}

	void Reordered(Object sender,EventArgs e) {
		string sNow;
		LiveOddsOrder modify = new LiveOddsOrder((string)Application["SoccerDBConnectionString"]);
		sNow = DateTime.Now.ToString("HH:mm:ss");
		try {
			if(modify.ReOrder()) {
				UpdateHistoryMessage("���\�ק�Ǹ� (" + sNow + ")");
			} else {
				UpdateHistoryMessage((string)Application["transErrorMsg"] + "(" + sNow + ")");
			}
			Page_Load(sender,e);
		}	catch(NullReferenceException nullex) {
			FormsAuthentication.SignOut();
			UpdateHistoryMessage("<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>");
		}
	}

	void UpdateHistoryMessage(string sMsg) {
		rtnMsg.Text = sMsg;
	}
</script>

<script language="JavaScript">
function OrderChanged(idx) {
	if(LiveoddsOrderForm.order[idx].value != '') {
		var len = LiveoddsOrderForm.order[idx].value.length;
		if(eval(len) == 1) {
			re=/\d/
			x = LiveoddsOrderForm.order[idx].value.search(re);
			if(x == -1) {
				alert('�����T�Ǹ�, �Ǹ����ݬO�Ʀr!');
				LiveoddsOrderForm.order[idx].value = '1';
			}
		} else if(eval(len) == 2) {
			re=/\d{2,2}/
			x = LiveoddsOrderForm.order[idx].value.search(re);
			if(x == -1) {
				alert('�����T�Ǹ�, �Ǹ����ݬO�Ʀr!');
				LiveoddsOrderForm.order[idx].value = '1';
			}
		} else if(eval(len) == 3) {
			re=/\d{3,3}/
			x = LiveoddsOrderForm.order[idx].value.search(re);
			if(x == -1) {
				alert('�����T�Ǹ�, �Ǹ����ݬO�Ʀr!');
				LiveoddsOrderForm.order[idx].value = '1';
			}
		}
	} else {
		alert('�Ǹ����ݬO�Ʀr!');
		LiveoddsOrderForm.order[idx].value = '1';
	}
}
</script>

<head>
	<META http-equiv="Content-Type" content="text/html; charset=big5">
	<LINK REL="stylesheet" HREF="/sportStyle.css" TYPE="text/css">
	<title>��|��T - �{���߲v</title>
</head>
<body>
<form id="LiveoddsOrderForm" method="post" runat="server">
	<h1>�ק�U�a�{���߲v���Ƨ�</h1>
	<asp:Label id="rtnMsg" runat="server" />
	<table border="1" width="25%" style="font: 10pt verdana">
		<tr style="background-color:#DAA520">
			<th>�Ǹ�</th>
			<th>�a��/���q</th>
		</tr>
		<span id="LiveoddsNameInfo" runat="server" />
		<tr>
			<td colspan="2" align="right">
				<!--
					Value of SendToPager is Path ID
					2: GOGO2 Handler1
					9: GOGO1 Sender2
					15: GOGO3Combo Asia
				-->
				<input type="hidden" name="SendToPager" value="2">
				<input type="hidden" name="SendToPager" value="9">
				<input type="hidden" name="SendToPager" value="15">
				<input type="submit" id="SendBtn" value="�o�e" OnServerClick="Reordered" runat="server">
			</td>
		</tr>
	</table>
</form>
</body>
</html>