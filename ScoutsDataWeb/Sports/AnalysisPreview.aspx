<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>
<%@ Register TagPrefix="uc1" TagName="MenuTabs" Src="~/UserControl/MenuTabs.ascx" %>
<script language="C#" runat="server">
    int iRecCount;

    void Page_Load(Object sender,EventArgs e) {
        AnalysisPreview matchPreview = new AnalysisPreview(ConfigurationManager.AppSettings["SoccerDBConnectionString"]);
        try {
            AnalysisPreivewInformation.InnerHtml = matchPreview.PreviewMatches();
            iRecCount = matchPreview.NumberOfRecords;
            UpdateHistoryMessage(ConfigurationManager.AppSettings["retrieveInfoMsg"] + "�ɨƤ��R(" + DateTime.Now.ToString("HH:mm:ss") + ")");
            this.Title =matchPreview.m_Title;
        } catch(NullReferenceException) {
            FormsAuthentication.SignOut();
            UpdateHistoryMessage("<a href=\"/sports/index.htm\" target=\"_top\">" + ConfigurationManager.AppSettings["sessionExpiredMsg"] + "</a>");
        }
    }

    void onSendAnalysis(Object sender,EventArgs e) {
        int iUpdated = 0;
        string sNow;
        AnalysisPreview previewAnly = new AnalysisPreview(ConfigurationManager.AppSettings["SoccerDBConnectionString"]);
        sNow = DateTime.Now.ToString("HH:mm:ss");
        try {
            iUpdated = previewAnly.Send();
            Page_Load(sender,e);
            if(iUpdated > 0) {
                UpdateHistoryMessage("���\�o�e�ɨƤ��R(" + sNow + ")");
            }   else if(iUpdated == 0) {
                UpdateHistoryMessage("�S���o�e�ɨƤ��R(" + sNow + ")");
            } else {
                UpdateHistoryMessage(ConfigurationManager.AppSettings["transErrorMsg"] + "(" + sNow + ")");
            }
        }   catch(NullReferenceException nullex) {
            FormsAuthentication.SignOut();
            UpdateHistoryMessage("<a href=\"/sports/index.htm\" target=\"_top\">" + ConfigurationManager.AppSettings["sessionExpiredMsg"] + "</a>");
        }
    }

    void UpdateHistoryMessage(string sMsg) {
        historyMsg.Text = sMsg;
    }
</script>

<script language="JavaScript">
<%
	if(iRecCount > 1) {
%>
function DeviceCheck() {
	<%
		int iActIdx;
		string sAction;

		for(iActIdx=0;iActIdx<iRecCount;iActIdx++) {
			sAction = "Action[" + iActIdx.ToString() + "]";
	%>
			if(AnalysisPreviewForm.<%=sAction%>.value == 'D') {
				AnalysisPreviewForm.SendToPager[0].checked = true;
				AnalysisPreviewForm.SendToPager[1].checked = true;
				AnalysisPreviewForm.SendToPager[2].checked = true;
			} else {
				if((AnalysisPreviewForm.SendToPager[0].checked == true) || (AnalysisPreviewForm.SendToPager[1].checked == true)) {
					AnalysisPreviewForm.SendToPager[0].checked = true;
					AnalysisPreviewForm.SendToPager[1].checked = true;
				}
			}
	<%
		}
	%>
}

function selectAllBG() {
	<%
		int iBGIdx;
		string sBG;
	%>
	if(AnalysisPreviewForm.AllBG.checked == true) {
		<%
			for(iBGIdx=0;iBGIdx<iRecCount;iBGIdx++) {
				sBG = "analysis_bg[" + iBGIdx.ToString() + "]";
		%>
				AnalysisPreviewForm.<%=sBG%>.checked = true;
		<%
			}
		%>
	}	else {
		<%
			for(iBGIdx=0;iBGIdx<iRecCount;iBGIdx++) {
				sBG = "analysis_bg[" + iBGIdx.ToString() + "]";
		%>
				AnalysisPreviewForm.<%=sBG%>.checked = false;
		<%
			}
		%>
	}
}

function selectAllHistory() {
	<%
		int iHistoryIdx;
		string sHistory;
	%>
	if(AnalysisPreviewForm.AllHistory.checked == true) {
		<%
			for(iHistoryIdx=0;iHistoryIdx<iRecCount;iHistoryIdx++) {
				sHistory = "analysis_history[" + iHistoryIdx.ToString() + "]";
		%>
				AnalysisPreviewForm.<%=sHistory%>.checked = true;
		<%
			}
		%>
	}	else {
		<%
			for(iHistoryIdx=0;iHistoryIdx<iRecCount;iHistoryIdx++) {
				sHistory = "analysis_history[" + iHistoryIdx.ToString() + "]";
		%>
				AnalysisPreviewForm.<%=sHistory%>.checked = false;
		<%
			}
		%>
	}
}

function selectAllPlayers() {
	<%
		int iPlayerIdx;
		string sPlayers;
	%>
	if(AnalysisPreviewForm.AllPlayers.checked == true) {
		<%
			for(iPlayerIdx=0;iPlayerIdx<iRecCount;iPlayerIdx++) {
				sPlayers = "analysis_players[" + iPlayerIdx.ToString() + "]";
		%>
				AnalysisPreviewForm.<%=sPlayers%>.checked = true;
		<%
			}
		%>
	}	else {
		<%
			for(iPlayerIdx=0;iPlayerIdx<iRecCount;iPlayerIdx++) {
				sPlayers = "analysis_players[" + iPlayerIdx.ToString() + "]";
		%>
				AnalysisPreviewForm.<%=sPlayers%>.checked = false;
		<%
			}
		%>
	}
}

function selectAllRemarks() {
	<%
		int iRemarksIdx;
		string sRemarks;
	%>
	if(AnalysisPreviewForm.AllRemarks.checked == true) {
		<%
			for(iRemarksIdx=0;iRemarksIdx<iRecCount;iRemarksIdx++) {
				sRemarks = "analysis_remarks[" + iRemarksIdx.ToString() + "]";
		%>
				AnalysisPreviewForm.<%=sRemarks%>.checked = true;
		<%
			}
		%>
	}	else {
		<%
			for(iRemarksIdx=0;iRemarksIdx<iRecCount;iRemarksIdx++) {
				sRemarks = "analysis_remarks[" + iRemarksIdx.ToString() + "]";
		%>
				AnalysisPreviewForm.<%=sRemarks%>.checked = false;
		<%
			}
		%>
	}
}

function selectAllItems(checkedIdx) {
	if(AnalysisPreviewForm.allitems[checkedIdx].checked == true) {
		AnalysisPreviewForm.analysis_bg[checkedIdx].checked = true;
		AnalysisPreviewForm.analysis_history[checkedIdx].checked = true;
		AnalysisPreviewForm.analysis_players[checkedIdx].checked = true;
		AnalysisPreviewForm.analysis_remarks[checkedIdx].checked = true;
	} else {
		AnalysisPreviewForm.analysis_bg[checkedIdx].checked = false;
		AnalysisPreviewForm.analysis_history[checkedIdx].checked = false;
		AnalysisPreviewForm.analysis_players[checkedIdx].checked = false;
		AnalysisPreviewForm.analysis_remarks[checkedIdx].checked = false;
	}
}

function OnActionChanged(checkedIdx) {
	if(AnalysisPreviewForm.Action[checkedIdx].value == 'D') {
		document.AnalysisPreviewForm.SendToPager[0].checked = true;
		document.AnalysisPreviewForm.SendToPager[1].checked = true;
		document.AnalysisPreviewForm.SendToPager[2].checked = true;
		//alert('�нT�wGOGO1,GOGO2�ΰ��|���w����I');
	}
}
<%
}
else {
%>
function DeviceCheck() {
	if(AnalysisPreviewForm.Action.value == 'D') {
		AnalysisPreviewForm.SendToPager[0].checked = true;
		AnalysisPreviewForm.SendToPager[1].checked = true;
		AnalysisPreviewForm.SendToPager[2].checked = true;
	} else {
		if((AnalysisPreviewForm.SendToPager[0].checked == true) || (AnalysisPreviewForm.SendToPager[1].checked == true)) {
			AnalysisPreviewForm.SendToPager[0].checked = true;
			AnalysisPreviewForm.SendToPager[1].checked = true;
		}
	}
}

function selectAllBG() {
	if(AnalysisPreviewForm.AllBG.checked == true) {
		AnalysisPreviewForm.analysis_bg.checked = true;
	}	else {
		AnalysisPreviewForm.analysis_bg.checked = false;
	}
}

function selectAllHistory() {
	if(AnalysisPreviewForm.AllHistory.checked == true) {
		AnalysisPreviewForm.analysis_history.checked = true;
	}	else {
		AnalysisPreviewForm.analysis_history.checked = false;
	}
}

function selectAllPlayers() {
	if(AnalysisPreviewForm.AllPlayers.checked == true) {
		AnalysisPreviewForm.analysis_players.checked = true;
	}	else {
		AnalysisPreviewForm.analysis_players.checked = false;
	}
}

function selectAllRemarks() {
	if(AnalysisPreviewForm.AllRemarks.checked == true) {
		AnalysisPreviewForm.analysis_remarks.checked = true;
	}	else {
		AnalysisPreviewForm.analysis_remarks.checked = false;
	}
}

function selectAllItems(checkedIdx) {
	if(AnalysisPreviewForm.allitems.checked == true) {
		AnalysisPreviewForm.analysis_bg.checked = true;
		AnalysisPreviewForm.analysis_history.checked = true;
		AnalysisPreviewForm.analysis_players.checked = true;
		AnalysisPreviewForm.analysis_remarks.checked = true;
	}	else {
		AnalysisPreviewForm.analysis_bg.checked = false;
		AnalysisPreviewForm.analysis_history.checked = false;
		AnalysisPreviewForm.analysis_players.checked = false;
		AnalysisPreviewForm.analysis_remarks.checked = false;
	}
}

function OnActionChanged(checkedIdx) {
	if(AnalysisPreviewForm.Action.value == 'D') {
		document.AnalysisPreviewForm.SendToPager[0].checked = true;
		document.AnalysisPreviewForm.SendToPager[1].checked = true;
		document.AnalysisPreviewForm.SendToPager[2].checked = true;
		//alert('�нT�wGOGO1,GOGO2�ΰ��|���w����I');
	}
}
<%
	}
%>
</script>

<html>
<head runat="server">
	<META http-equiv="Content-Type" content="text/html; charset=big5">
	<LINK href="../CentaSmsStyle.css" type="text/css" rel="stylesheet">
    <LINK REL="stylesheet" HREF="/sportStyle.css" TYPE="text/css">
      <title>�o�e���R</title>
</head>
<body>
	<form id="AnalysisPreviewForm" method="post" runat="server" onsubmit="DeviceCheck()">
	 <uc1:menutabs id="MenuTabs1" runat="server" Visible="false" ></uc1:menutabs>
        <font size="2"><b>�W�����:</b><asp:Label id="historyMsg" runat="server" /></font><br>

		<table border="1" width="100%" style="font: 10pt verdana">
		<tr style="background-color:#2F4F4F" align="center">
			<th colspan="5"></th>
			<th colspan="4"><font color="#F0F8FF">�o�e����</font></th>
			<th></th>
		</tr>
		<tr style="background-color:#2F4F4F" align="center">
			<th><font color="#F0F8FF">�ɨƮɶ�</font></th>
			<th><font color="#F0F8FF">�p��</font></th>
			<th><font color="#F0F8FF">�D��</font></th>
			<th><font color="#F0F8FF">�ȶ�</font></th>
			<th><font color="#F0F8FF">����ʧ@</font></th>
			<th><font color="#F0F8FF">�򥻸�T<br>����<input type="checkbox" name="AllBG" value="1" onClick="selectAllBG()"></font></th>
			<th><font color="#F0F8FF">���ɩ��Z<br>����<input type="checkbox" name="AllHistory" value="1" onClick="selectAllHistory()"></font></th>
			<th><font color="#F0F8FF">�X���W��<br>����<input type="checkbox" name="AllPlayers" value="1" onClick="selectAllPlayers()"></font></th>
			<th><font color="#F0F8FF">���[��T<br>����<input type="checkbox" name="AllRemarks" value="1" onClick="selectAllRemarks()"></font></th>
			<th></th>
		</tr>
		<span id="AnalysisPreivewInformation" runat="server" />
		<tr>
			<th colspan="5" align="left">
				<font color="red">�b�R����ƮɡA�нT�wGOGO1,GOGO2�ΰ��|���w�P�ɿ���A�H�קK�ǩI����Ʀ����~�I</font>
			</th>
			<td colspan="5" align="right">
				<!--
					Value of SendToPager is Device ID defined in DEVICE_TYPE
				-->
				<input type="checkbox" name="SendToPager" value="1" checked>GOGO1&nbsp;
				<input type="checkbox" name="SendToPager" value="2" checked>GOGO2&nbsp;
				<input type="checkbox" name="SendToPager" value="3" checked>���|��&nbsp;
				<input type="checkbox" name="SendToPager" value="4" checked>Combo
				<input type="submit" id="SendBtn" value="�o�e" OnServerClick="onSendAnalysis" runat="server">&nbsp;
				<input type="reset" value="���]">
			</td>
		</table>
	</form>
</body>
</html>