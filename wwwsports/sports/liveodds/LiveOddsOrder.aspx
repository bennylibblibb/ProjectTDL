<html>
<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">
	void Page_Load(Object sender,EventArgs e) {
		LiveOddsOrder order = new LiveOddsOrder((string)Application["SoccerDBConnectionString"]);

		try {
			LiveoddsNameInfo.InnerHtml = order.ShowRegion();
			UpdateHistoryMessage((string)Application["retrieveInfoMsg"] + "各地序號(" + DateTime.Now.ToString("HH:mm:ss") + ")");
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
				UpdateHistoryMessage("成功修改序號 (" + sNow + ")");
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
				alert('不正確序號, 序號必需是數字!');
				LiveoddsOrderForm.order[idx].value = '1';
			}
		} else if(eval(len) == 2) {
			re=/\d{2,2}/
			x = LiveoddsOrderForm.order[idx].value.search(re);
			if(x == -1) {
				alert('不正確序號, 序號必需是數字!');
				LiveoddsOrderForm.order[idx].value = '1';
			}
		} else if(eval(len) == 3) {
			re=/\d{3,3}/
			x = LiveoddsOrderForm.order[idx].value.search(re);
			if(x == -1) {
				alert('不正確序號, 序號必需是數字!');
				LiveoddsOrderForm.order[idx].value = '1';
			}
		}
	} else {
		alert('序號必需是數字!');
		LiveoddsOrderForm.order[idx].value = '1';
	}
}
</script>

<head>
	<META http-equiv="Content-Type" content="text/html; charset=big5">
	<LINK REL="stylesheet" HREF="/sportStyle.css" TYPE="text/css">
	<title>體育資訊 - 現場賠率</title>
</head>
<body>
<form id="LiveoddsOrderForm" method="post" runat="server">
	<h1>修改各地現場賠率之排序</h1>
	<asp:Label id="rtnMsg" runat="server" />
	<table border="1" width="25%" style="font: 10pt verdana">
		<tr style="background-color:#DAA520">
			<th>序號</th>
			<th>地區/公司</th>
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
				<input type="submit" id="SendBtn" value="發送" OnServerClick="Reordered" runat="server">
			</td>
		</tr>
	</table>
</form>
</body>
</html>