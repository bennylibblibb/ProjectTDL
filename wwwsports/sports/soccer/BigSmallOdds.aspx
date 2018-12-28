<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">
	int iRecCount;
	int iExportSize;

	void Page_Load(Object sender,EventArgs e) {
		BigSmallOdds bsOdds = new BigSmallOdds((string)Application["SoccerDBConnectionString"]);
		try {
			BSOddsModifyInformation.InnerHtml = bsOdds.GetMatch();
			iRecCount = bsOdds.NumberOfRecords;
			iExportSize = bsOdds.ExportSize;
			UpdateHistoryMessage((string)Application["retrieveInfoMsg"] + "大小盤(" + DateTime.Now.ToString("HH:mm:ss") + ")");
		} catch(NullReferenceException) {
			FormsAuthentication.SignOut();
			UpdateHistoryMessage("<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>");
		}
	}

	void ModifyBigSmallOdds(Object sender,EventArgs e) {
		int iUpdated = 0;
		string sNow;
		BigSmallOdds cOdds = new BigSmallOdds((string)Application["SoccerDBConnectionString"]);
		sNow = DateTime.Now.ToString("HH:mm:ss");
		try {
			iUpdated = cOdds.Modify();
			Page_Load(sender,e);
			if(iUpdated > 0) {
				UpdateHistoryMessage("成功修改大小盤(" + sNow + ")");
			}	else if(iUpdated == 0) {
				UpdateHistoryMessage("沒有修改大小盤(" + sNow + ")");
			}	else {
				UpdateHistoryMessage((string)Application["transErrorMsg"] + "(" + sNow + ")");
			}
		} catch(NullReferenceException nullex) {
			FormsAuthentication.SignOut();
			UpdateHistoryMessage("<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>");
		}
	}

	void ExportBigSmallOdds(Object sender,EventArgs e) {
		int iUpdated = 0;
		string sNow;
		BigSmallOdds odds = new BigSmallOdds((string)Application["SoccerDBConnectionString"]);
		sNow = DateTime.Now.ToString("HH:mm:ss");
		try {
			iUpdated = odds.Export();
			Page_Load(sender,e);
			if(iUpdated > 0) {
				UpdateHistoryMessage("成功匯出大小盤(" + sNow + ")");
			}	else if(iUpdated == 0) {
				UpdateHistoryMessage("沒有匯出大小盤(" + sNow + ")");
			}	else {
				UpdateHistoryMessage((string)Application["transErrorMsg"] + "(" + sNow + ")");
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
<%
	if(iRecCount > 0) {
%>
function selectAll() {
	<%
		int iIdx;
		string sExportItem;
	%>
	if(BigSmallOddsForm.ExportAll.checked == true) {
		<%
			for(iIdx=0;iIdx<iRecCount;iIdx++) {
				sExportItem = "exported[" + iIdx.ToString() + "]";
		%>
				BigSmallOddsForm.<%=sExportItem%>.checked = true;
		<%
			}
		%>
	}	else {
		<%
			for(iIdx=0;iIdx<iRecCount;iIdx++) {
				sExportItem = "exported[" + iIdx.ToString() + "]";
		%>
				BigSmallOddsForm.<%=sExportItem%>.checked = false;
		<%
			}
		%>
	}
}

function ClearOrderNo() {
	<%
		for(int iClear=0;iClear<iRecCount;iClear++) {
	%>
		BigSmallOddsForm.orderID[<%=iClear.ToString()%>].value = '';
	<%
		}
	%>
}

function onOrderNoChange(idx) {
	order_re = /^\d{0,3}$/
	re_val = BigSmallOddsForm.orderID[idx].value.search(order_re)
	if(re_val == -1) {
		alert('序號只接受數字');
		BigSmallOddsForm.orderID[idx].value = '';
	}
}

function onBigOddsChange(idx) {
	if(BigSmallOddsForm.bigodds[idx].value != '') {
		if(isNaN(BigSmallOddsForm.bigodds[idx].value)) {
			BigSmallOddsForm.bigodds[idx].value = '';
			alert('不正確賠率數字');
		}
	}
}

function onSmallOddsChange(idx) {
	if(BigSmallOddsForm.smallodds[idx].value != '') {
		if(isNaN(BigSmallOddsForm.smallodds[idx].value)) {
			BigSmallOddsForm.smallodds[idx].value = '';
			alert('不正確賠率數字');
		}
	}
}
<%
	} else {
%>
function selectAll() {
	if(BigSmallOddsForm.ExportAll.checked == true) {
		BigSmallOddsForm.exported.checked = true;
	}	else {
		BigSmallOddsForm.exported.checked = false;
	}
}

function ClearOrderNo() {
	BigSmallOddsForm.orderID.value = '';
}

function onOrderNoChange(idx) {
	order_re = /^\d{0,3}$/
	re_val = BigSmallOddsForm.orderID.value.search(order_re)
	if(re_val == -1) {
		alert('序號只接受數字');
		BigSmallOddsForm.orderID.value = '';
	}
}

function onBigOddsChange(idx) {
	if(BigSmallOddsForm.bigodds.value != '') {
		if(isNaN(BigSmallOddsForm.bigodds.value)) {
			BigSmallOddsForm.bigodds.value = '';
			alert('不正確賠率數字');
		}
	}
}

function onSmallOddsChange(idx) {
	if(BigSmallOddsForm.smallodds.value != '') {
		if(isNaN(BigSmallOddsForm.smallodds.value)) {
			BigSmallOddsForm.smallodds.value = '';
			alert('不正確賠率數字');
		}
	}
}
<%
	}
%>

function onSizeChanged() {
	if(BigSmallOddsForm.importsize.value != '') {
		var len = BigSmallOddsForm.importsize.value.length;
		if(eval(len) == 1) {
			re=/\d/
			x = BigSmallOddsForm.importsize.value.search(re);
			if(x == -1) {
				BigSmallOddsForm.importsize.value = '1';
				alert('不正確匯出數字');
			}
		} else if(eval(len) == 2) {
			re=/\d{2,2}/
			x = BigSmallOddsForm.importsize.value.search(re);
			if(x == -1) {
				BigSmallOddsForm.importsize.value = '1';
				alert('不正確匯出數字');
			}
		}
	} else {
		BigSmallOddsForm.importsize.value = '1';
		alert('不正確匯出數字');
	}
}
</script>

<html>
<head>
	<META http-equiv="Content-Type" content="text/html; charset=big5">
	<LINK REL="stylesheet" HREF="/sportStyle.css" TYPE="text/css">
</head>
<body>
	<form id="BigSmallOddsForm" method="post" runat="server">
		<font size="2"><b>上次行動:</b><asp:Label id="historyMsg" runat="server" /></font><br>

		<table border="1" width="100%" style="font: 10pt verdana">
			<tr align="right" style="background-color:#FFE4E1">
				<td align="left" colspan="5">
					<input type="button" name="clearOrder" value="清除序號" onClick="ClearOrderNo()">
				</td>
				<td colspan="5">
					<b>匯出設定:</b>每<input name="importsize" maxlength="1" size="1" value="<%=iExportSize%>" onChange="onSizeChanged()">場賽事為一條訊息
				</td>
			</tr>
			<tr align="center" style="background-color:#FFE4E1">
				<th>序號</th>
				<th>聯賽</th>
				<th>主隊</th>
				<th>客隊</th>
				<th>賽事日期</th>
				<th>執行動作</th>
				<th>總入球</th>
				<th>賠率</th>
				<th>狀況</th>
				<th>匯出(<font size="1">全選<input type="checkbox" name="ExportAll" value="1" onClick="selectAll()"></font>)</th>
			</tr>
			<span id="BSOddsModifyInformation" runat="server" />
			<tr>
				<td colspan="9" align="right">
					<font color="red">儲存數據不會發送到傳呼機，如要發送，請匯出數據，然後在<font color="blue">其他資訊->足球資訊2->大小盤</font>發送到傳呼機</font>&nbsp;
					<input type="submit" id="SaveBtn" value="儲存" OnServerClick="ModifyBigSmallOdds" runat="server">&nbsp;
					<input type="reset" value="重設">
				</td>
				<td align="center">
					<input type="submit" id="ExportBtn" value="匯出" OnServerClick="ExportBigSmallOdds" runat="server">
				</td>
			</tr>
			<tr>
				<td colspan="3" align="left">
					<font color="#DDA0DD">紫色記錄已存在『大小盤』資料庫</font>
				</td>
				<td colspan="7" align="right">
					在<font color="blue">其他資訊->足球資訊2->大小盤</font>亦可修改大小盤數據，但數據不會覆蓋到這裡
				</td>
			</tr>
		</table>
	</form>
</body>
</html>