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
			UpdateHistoryMessage((string)Application["retrieveInfoMsg"] + "�p�ձƦW(" + DateTime.Now.ToString("HH:mm:ss") + ")");
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
				UpdateHistoryMessage("���\�o�e"+ iUpdated.ToString()+"�p�ձƦW(" + sNow + ")");
			} else {
				UpdateHistoryMessage("�S���o�e(" + sNow + ")");
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
		alert('�ƦW�u�����Ʀr');
		BSKGroupRankForm.<%=sRankIdIndex%>.value = '';
		return false;
	}
	if(BSKGroupRankForm.<%=sWinIndex%>.value!="") {
		re_val = BSKGroupRankForm.<%=sWinIndex%>.value.search(order_re)
		if(re_val == -1) {
			alert('�ӥu�����Ʀr!');
			BSKGroupRankForm.<%=sWinIndex%>.value = '';
			return false;
		}
	}

	if(BSKGroupRankForm.<%=sLossIndex%>.value!="") {
		re_val = BSKGroupRankForm.<%=sLossIndex%>.value.search(order_re)
		if(re_val == -1) {
			alert('�t�u�����Ʀr!');
			BSKGroupRankForm.<%=sLossIndex%>.value = '';
			return false;
			}
	}

	if(BSKGroupRankForm.<%=sRankDataIndex%>.value!="") {
		re_val = BSKGroupRankForm.<%=sRankDataIndex%>.value.search(order_re)
		if(re_val == -1) {
			alert('�s��/�t�u�����Ʀr!');
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
		alert('�ƦW�u�����Ʀr');
		BSKGroupRankForm.RankNumber.value = '';
		return false;
	}

	if(BSKGroupRankForm.WIN.value!="") {
		reoder = BSKGroupRankForm.WIN.value.search(order_re)
		if(reoder == -1) {
			alert('�ӥu�����Ʀr!');
			BSKGroupRankForm.WIN.value = '';
			return false;
			}
	}

	if(BSKGroupRankForm.LOSS.value!="") {
		reoder = BSKGroupRankForm.LOSS.value.search(order_re)
		if(reoder == -1) {
			alert('�t�u�����Ʀr!');
			BSKGroupRankForm.LOSS.value = '';
			return false;
			}
	}

	if(BSKGroupRankForm.DATA.value!="") {
		reoder = BSKGroupRankForm.DATA.value.search(order_re)
		if(reoder == -1)  {
			alert('�s��/�t�u�����Ʀr!');
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
		<font size="2"><b>�W�����:</b><asp:Label id="historyMsg" runat="server" /></font><br>

		<table border="1" width="60%" style="font: 10pt verdana">
		<tr style="background-color:#22c471">
		<th colspan=6 align="left"><font color=#aa4471>
		<asp:Label id="leaguename" runat="server" />
		</font>�ƦW</th>
		<tr>
			<th>�ƦW</th>
			<th>�y��</th>
			<th>��/�t</th>
			<th>�s��/��</th>
		</tr>
		<span id="RankLeagInformation" runat="server" />
		<tr>
			<td colspan="4" align="right">
				<!--
					Value of SendToPager is Device ID defined in DEVICE_TYPE
				-->
				<input type="checkbox" name="SendToPager" value="1" checked>GOGO1&nbsp;
				<input type="checkbox" name="SendToPager" value="1" checked>GOGO2&nbsp;
				<input type="checkbox" name="SendToPager" value="3" checked>���|��&nbsp;
				<!--
				<input type="checkbox" name="SendToPager" value="4" checked>JCCombo&nbsp;
				<input type="checkbox" name="SendToPager" value="5" checked>GOGO3Combo&nbsp;
				-->
				<input type="submit" id="SaveBtn" value="�ǰe" OnServerClick="onAddLeague" runat="server">&nbsp;
				<input type="reset" value="���]">
			</td>
		</table>
	</form>
</body>
</html>