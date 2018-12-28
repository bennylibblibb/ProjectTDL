<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">
	int iUpindex;

	void Page_Load(Object sender,EventArgs e) {
		BSKGroupRank AddRankLeag = new BSKGroupRank((string)Application["BasketballDBConnectionString"]);

		try {
			RankLeagInformation.InnerHtml = AddRankLeag.GetRankLeag();
			leaguename.Text = AddRankLeag.SetLeaglong;
			iUpindex = AddRankLeag.NumberOfRecords;
			UpdateHistoryMessage((string)Application["retrieveInfoMsg"] + "小組排名(" + DateTime.Now.ToString("HH:mm:ss") + ")");
		} catch(NullReferenceException) {
			FormsAuthentication.SignOut();
			UpdateHistoryMessage("<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>");
		}
	}

	void onAddLeague(Object sender,EventArgs e) {
		int iUpdated = 0;
		string sNow;
		BSKGroupRank AddRankLeag = new BSKGroupRank((string)Application["BasketballDBConnectionString"]);
		sNow = DateTime.Now.ToString("HH:mm:ss");
		try {
			iUpdated = AddRankLeag.AddRankLeag();
			Page_Load(sender,e);
			if(iUpdated > 0) {
				UpdateHistoryMessage("成功發送"+ iUpdated.ToString()+"小組排名(" + sNow + ")");
			} else {
				UpdateHistoryMessage("沒有發送(" + sNow + ")");
			}
		} catch(NullReferenceException nullex) {
			FormsAuthentication.SignOut();
			UpdateHistoryMessage("<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>");
		}
	}

	void UpdateHistoryMessage(string sMsg) {
		historyMsg.Text = sMsg;
	}
</script>

<script language="JavaScript">
function  CheckValue() {
	order_re = /^\d{0,2}$/
<%
	if(iUpindex >1 ) {
%>

<%
	int iIndex;
	string sRankDataIndex,sRankIdIndex,sWinIndex,sLossIndex;
	for(iIndex=0;iIndex<iUpindex;iIndex++) {
		sRankDataIndex = "DATA[" + iIndex.ToString() + "]";
		sLossIndex = "LOSS["+iIndex.ToString()+"]";
		sWinIndex = "WIN["+iIndex.ToString()+"]";
		sRankIdIndex = "RankNumber["+iIndex.ToString()+"]";
%>

	re_val = BSKGroupRankForm.<%=sRankIdIndex%>.value.search(order_re)
	if(re_val == -1) {
		alert('排名只接受數字');
		BSKGroupRankForm.<%=sRankIdIndex%>.value = '';
		return false;
	}
	if(BSKGroupRankForm.<%=sWinIndex%>.value!="") {
		re_val = BSKGroupRankForm.<%=sWinIndex%>.value.search(order_re)
		if(re_val == -1) {
			alert('勝只接受數字!');
			BSKGroupRankForm.<%=sWinIndex%>.value = '';
			return false;
		}
	}

	if(BSKGroupRankForm.<%=sLossIndex%>.value!="") {
		re_val = BSKGroupRankForm.<%=sLossIndex%>.value.search(order_re)
		if(re_val == -1) {
			alert('負只接受數字!');
			BSKGroupRankForm.<%=sLossIndex%>.value = '';
			return false;
			}
	}

	if(BSKGroupRankForm.<%=sRankDataIndex%>.value!="") {
		re_val = BSKGroupRankForm.<%=sRankDataIndex%>.value.search(order_re)
		if(re_val == -1) {
			alert('連勝/負只接受數字!');
			BSKGroupRankForm.<%=sRankDataIndex%>.value = '';
			return false;
		}
	}
<%
	}
%>
<%
	} else {
		if(iUpindex ==1 ) {
%>
	reoder = BSKGroupRankForm.RankNumber.value.search(order_re)
	if(reoder == -1) {
		alert('排名只接受數字');
		BSKGroupRankForm.RankNumber.value = '';
		return false;
	}

	if(BSKGroupRankForm.WIN.value!="") {
		reoder = BSKGroupRankForm.WIN.value.search(order_re)
		if(reoder == -1) {
			alert('勝只接受數字!');
			BSKGroupRankForm.WIN.value = '';
			return false;
			}
	}

	if(BSKGroupRankForm.LOSS.value!="") {
		reoder = BSKGroupRankForm.LOSS.value.search(order_re)
		if(reoder == -1) {
			alert('負只接受數字!');
			BSKGroupRankForm.LOSS.value = '';
			return false;
			}
	}

	if(BSKGroupRankForm.DATA.value!="") {
		reoder = BSKGroupRankForm.DATA.value.search(order_re)
		if(reoder == -1)  {
			alert('連勝/負只接受數字!');
			BSKGroupRankForm.DATA.value = '';
			return false;
		}
	}
<%
	}
}
%>
}
</script>

<html>
<head>
	<META http-equiv="Content-Type" content="text/html; charset=big5">
	<LINK REL="stylesheet" HREF="/sportStyle.css" TYPE="text/css">
</head>
<body>
	<form id="BSKGroupRankForm" method="post" runat="server" ONSUBMIT="return CheckValue()">
		<font size="2"><b>上次行動:</b><asp:Label id="historyMsg" runat="server" /></font><br>

		<table border="1" width="60%" style="font: 10pt verdana">
		<tr style="background-color:#22c471">
		<th colspan=6 align="left"><font color=#aa4471>
		<asp:Label id="leaguename" runat="server" />
		</font>排名</th>
		<tr>
			<th>排名</th>
			<th>球隊</th>
			<th>勝/負</th>
			<th>連勝/敗</th>
		</tr>
		<span id="RankLeagInformation" runat="server" />
		<tr>
			<td colspan="4" align="right">
				<!--
					Value of SendToPager is Device ID defined in DEVICE_TYPE
				-->
				<input type="checkbox" name="SendToPager" value="1" checked>GOGO1&nbsp;
				<input type="checkbox" name="SendToPager" value="1" checked>GOGO2&nbsp;
				<input type="checkbox" name="SendToPager" value="3" checked>馬會機&nbsp;
				<!--
				<input type="checkbox" name="SendToPager" value="4" checked>JCCombo&nbsp;
				<input type="checkbox" name="SendToPager" value="5" checked>GOGO3Combo&nbsp;
				-->
				<input type="submit" id="SaveBtn" value="傳送" OnServerClick="onAddLeague" runat="server">&nbsp;
				<input type="reset" value="重設">
			</td>
		</table>
	</form>
</body>
</html>