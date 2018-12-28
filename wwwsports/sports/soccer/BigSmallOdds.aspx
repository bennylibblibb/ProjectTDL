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
			UpdateHistoryMessage((string)Application["retrieveInfoMsg"] + "�j�p�L(" + DateTime.Now.ToString("HH:mm:ss") + ")");
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
				UpdateHistoryMessage("���\�ק�j�p�L(" + sNow + ")");
			}	else if(iUpdated == 0) {
				UpdateHistoryMessage("�S���ק�j�p�L(" + sNow + ")");
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
				UpdateHistoryMessage("���\�ץX�j�p�L(" + sNow + ")");
			}	else if(iUpdated == 0) {
				UpdateHistoryMessage("�S���ץX�j�p�L(" + sNow + ")");
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
		alert('�Ǹ��u�����Ʀr');
		BigSmallOddsForm.orderID[idx].value = '';
	}
}

function onBigOddsChange(idx) {
	if(BigSmallOddsForm.bigodds[idx].value != '') {
		if(isNaN(BigSmallOddsForm.bigodds[idx].value)) {
			BigSmallOddsForm.bigodds[idx].value = '';
			alert('�����T�߲v�Ʀr');
		}
	}
}

function onSmallOddsChange(idx) {
	if(BigSmallOddsForm.smallodds[idx].value != '') {
		if(isNaN(BigSmallOddsForm.smallodds[idx].value)) {
			BigSmallOddsForm.smallodds[idx].value = '';
			alert('�����T�߲v�Ʀr');
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
		alert('�Ǹ��u�����Ʀr');
		BigSmallOddsForm.orderID.value = '';
	}
}

function onBigOddsChange(idx) {
	if(BigSmallOddsForm.bigodds.value != '') {
		if(isNaN(BigSmallOddsForm.bigodds.value)) {
			BigSmallOddsForm.bigodds.value = '';
			alert('�����T�߲v�Ʀr');
		}
	}
}

function onSmallOddsChange(idx) {
	if(BigSmallOddsForm.smallodds.value != '') {
		if(isNaN(BigSmallOddsForm.smallodds.value)) {
			BigSmallOddsForm.smallodds.value = '';
			alert('�����T�߲v�Ʀr');
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
				alert('�����T�ץX�Ʀr');
			}
		} else if(eval(len) == 2) {
			re=/\d{2,2}/
			x = BigSmallOddsForm.importsize.value.search(re);
			if(x == -1) {
				BigSmallOddsForm.importsize.value = '1';
				alert('�����T�ץX�Ʀr');
			}
		}
	} else {
		BigSmallOddsForm.importsize.value = '1';
		alert('�����T�ץX�Ʀr');
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
		<font size="2"><b>�W�����:</b><asp:Label id="historyMsg" runat="server" /></font><br>

		<table border="1" width="100%" style="font: 10pt verdana">
			<tr align="right" style="background-color:#FFE4E1">
				<td align="left" colspan="5">
					<input type="button" name="clearOrder" value="�M���Ǹ�" onClick="ClearOrderNo()">
				</td>
				<td colspan="5">
					<b>�ץX�]�w:</b>�C<input name="importsize" maxlength="1" size="1" value="<%=iExportSize%>" onChange="onSizeChanged()">���ɨƬ��@���T��
				</td>
			</tr>
			<tr align="center" style="background-color:#FFE4E1">
				<th>�Ǹ�</th>
				<th>�p��</th>
				<th>�D��</th>
				<th>�ȶ�</th>
				<th>�ɨƤ��</th>
				<th>����ʧ@</th>
				<th>�`�J�y</th>
				<th>�߲v</th>
				<th>���p</th>
				<th>�ץX(<font size="1">����<input type="checkbox" name="ExportAll" value="1" onClick="selectAll()"></font>)</th>
			</tr>
			<span id="BSOddsModifyInformation" runat="server" />
			<tr>
				<td colspan="9" align="right">
					<font color="red">�x�s�ƾڤ��|�o�e��ǩI���A�p�n�o�e�A�жץX�ƾڡA�M��b<font color="blue">��L��T->���y��T2->�j�p�L</font>�o�e��ǩI��</font>&nbsp;
					<input type="submit" id="SaveBtn" value="�x�s" OnServerClick="ModifyBigSmallOdds" runat="server">&nbsp;
					<input type="reset" value="���]">
				</td>
				<td align="center">
					<input type="submit" id="ExportBtn" value="�ץX" OnServerClick="ExportBigSmallOdds" runat="server">
				</td>
			</tr>
			<tr>
				<td colspan="3" align="left">
					<font color="#DDA0DD">����O���w�s�b�y�j�p�L�z��Ʈw</font>
				</td>
				<td colspan="7" align="right">
					�b<font color="blue">��L��T->���y��T2->�j�p�L</font>��i�ק�j�p�L�ƾڡA���ƾڤ��|�л\��o��
				</td>
			</tr>
		</table>
	</form>
</body>
</html>