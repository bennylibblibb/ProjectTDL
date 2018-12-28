<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">
	int iRecCount;

	void Page_Load(Object sender,EventArgs e) {
		JCComboPlayerPreview matchPreview = new JCComboPlayerPreview((string)Application["SoccerDBConnectionString"]);
		try {
			AnalysisPreivewInformation.InnerHtml = matchPreview.PreviewMatches();
			iRecCount = matchPreview.NumberOfRecords;
			UpdateHistoryMessage((string)Application["retrieveInfoMsg"] + "�ɨƤ��R(" + DateTime.Now.ToString("HH:mm:ss") + ")");
		} catch(NullReferenceException) {
			FormsAuthentication.SignOut();
			UpdateHistoryMessage("<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>");
		}
	}

	void onSendAnalysis(Object sender,EventArgs e) {
		int iUpdated = 0;
		string sNow;
		JCComboPlayerPreview previewAnly = new JCComboPlayerPreview((string)Application["SoccerDBConnectionString"]);
		sNow = DateTime.Now.ToString("HH:mm:ss");
		try {
			iUpdated = previewAnly.Send();
			Page_Load(sender,e);
			if(iUpdated > 0) {
				UpdateHistoryMessage("���\�o�e�ɨƤ��R(" + sNow + ")");
			}	else if(iUpdated == 0) {
				UpdateHistoryMessage("�S���o�e�ɨƤ��R(" + sNow + ")");
			} else {
				UpdateHistoryMessage((string)Application["transErrorMsg"] + "(" + sNow + ")");
			}
		}	catch(NullReferenceException nullex) {
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
	if(iRecCount > 1) {
%>
		function DeviceCheck() {
			<%
				int iActIdx;
				string sAction;
		
				for (iActIdx=0;iActIdx<iRecCount;iActIdx++) {
					sAction = "Action[" + iActIdx.ToString() + "]";
			%>
					if (JCComboPlayerPreviewForm.<%=sAction%>.value == 'D') {
						JCComboPlayerPreviewForm.SendToPager[0].checked = true;
						JCComboPlayerPreviewForm.SendToPager[1].checked = true;
						JCComboPlayerPreviewForm.SendToPager[2].checked = true;
					} else {
						if ((JCComboPlayerPreviewForm.SendToPager[0].checked == true) || (JCComboPlayerPreviewForm.SendToPager[1].checked == true)) {
							JCComboPlayerPreviewForm.SendToPager[0].checked = true;
							JCComboPlayerPreviewForm.SendToPager[1].checked = true;
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
			if (JCComboPlayerPreviewForm.AllBG.checked == true) {
				<%
					for(iBGIdx=0;iBGIdx<iRecCount;iBGIdx++) {
						sBG = "analysis_bg[" + iBGIdx.ToString() + "]";
				%>
						JCComboPlayerPreviewForm.<%=sBG%>.checked = true;
				<%
					}
				%>
			} else {
				<%
					for(iBGIdx=0;iBGIdx<iRecCount;iBGIdx++) {
						sBG = "analysis_bg[" + iBGIdx.ToString() + "]";
				%>
						JCComboPlayerPreviewForm.<%=sBG%>.checked = false;
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
			if (JCComboPlayerPreviewForm.AllHistory.checked == true) {
				<%
					for(iHistoryIdx=0;iHistoryIdx<iRecCount;iHistoryIdx++) {
						sHistory = "analysis_history[" + iHistoryIdx.ToString() + "]";
				%>
						JCComboPlayerPreviewForm.<%=sHistory%>.checked = true;
				<%
					}
				%>
			} else {
				<%
					for(iHistoryIdx=0;iHistoryIdx<iRecCount;iHistoryIdx++) {
						sHistory = "analysis_history[" + iHistoryIdx.ToString() + "]";
				%>
						JCComboPlayerPreviewForm.<%=sHistory%>.checked = false;
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
			if (JCComboPlayerPreviewForm.AllPlayers.checked == true) {
				<%
					for (iPlayerIdx=0;iPlayerIdx<iRecCount;iPlayerIdx++) {
						sPlayers = "analysis_players[" + iPlayerIdx.ToString() + "]";
				%>
						JCComboPlayerPreviewForm.<%=sPlayers%>.checked = true;
				<%
					}
				%>
			} else {
				<%
					for(iPlayerIdx=0;iPlayerIdx<iRecCount;iPlayerIdx++) {
						sPlayers = "analysis_players[" + iPlayerIdx.ToString() + "]";
				%>
						JCComboPlayerPreviewForm.<%=sPlayers%>.checked = false;
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
			if (JCComboPlayerPreviewForm.AllRemarks.checked == true) {
				<%
					for (iRemarksIdx=0;iRemarksIdx<iRecCount;iRemarksIdx++) {
						sRemarks = "analysis_remarks[" + iRemarksIdx.ToString() + "]";
				%>
						JCComboPlayerPreviewForm.<%=sRemarks%>.checked = true;
				<%
					}
				%>
			} else {
				<%
					for (iRemarksIdx=0;iRemarksIdx<iRecCount;iRemarksIdx++) {
						sRemarks = "analysis_remarks[" + iRemarksIdx.ToString() + "]";
				%>
						JCComboPlayerPreviewForm.<%=sRemarks%>.checked = false;
				<%
					}
				%>
			}
		}
		
		function selectAllItems(checkedIdx) {
			if (JCComboPlayerPreviewForm.allitems[checkedIdx].checked == true) {
				JCComboPlayerPreviewForm.analysis_bg[checkedIdx].checked = true;
				JCComboPlayerPreviewForm.analysis_history[checkedIdx].checked = true;
				JCComboPlayerPreviewForm.analysis_players[checkedIdx].checked = true;
				JCComboPlayerPreviewForm.analysis_remarks[checkedIdx].checked = true;
			} else {
				JCComboPlayerPreviewForm.analysis_bg[checkedIdx].checked = false;
				JCComboPlayerPreviewForm.analysis_history[checkedIdx].checked = false;
				JCComboPlayerPreviewForm.analysis_players[checkedIdx].checked = false;
				JCComboPlayerPreviewForm.analysis_remarks[checkedIdx].checked = false;
			}
		}
		
		function OnActionChanged(checkedIdx) {
			if (JCComboPlayerPreviewForm.Action[checkedIdx].value == 'D') {
				document.JCComboPlayerPreviewForm.SendToPager[0].checked = true;
				document.JCComboPlayerPreviewForm.SendToPager[1].checked = true;
				document.JCComboPlayerPreviewForm.SendToPager[2].checked = true;
				//alert('�нT�wGOGO1,GOGO2�ΰ��|���w����I');
			}
		}
<%
	} else {
%>
		function DeviceCheck() {
			if (JCComboPlayerPreviewForm.Action.value == 'D') {
				JCComboPlayerPreviewForm.SendToPager[0].checked = true;
				JCComboPlayerPreviewForm.SendToPager[1].checked = true;
				JCComboPlayerPreviewForm.SendToPager[2].checked = true;
			} else {
				if ((JCComboPlayerPreviewForm.SendToPager[0].checked == true) || (JCComboPlayerPreviewForm.SendToPager[1].checked == true)) {
					JCComboPlayerPreviewForm.SendToPager[0].checked = true;
					JCComboPlayerPreviewForm.SendToPager[1].checked = true;
				}
			}
		}
		
		function selectAllBG() {
			if (JCComboPlayerPreviewForm.AllBG.checked == true) {
				JCComboPlayerPreviewForm.analysis_bg.checked = true;
			} else {
				JCComboPlayerPreviewForm.analysis_bg.checked = false;
			}
		}
		
		function selectAllHistory() {
			if(JCComboPlayerPreviewForm.AllHistory.checked == true) {
				JCComboPlayerPreviewForm.analysis_history.checked = true;
			} else {
				JCComboPlayerPreviewForm.analysis_history.checked = false;
			}
		}
		
		function selectAllPlayers() {
			if (JCComboPlayerPreviewForm.AllPlayers.checked == true) {
				JCComboPlayerPreviewForm.analysis_players.checked = true;
			} else {
				JCComboPlayerPreviewForm.analysis_players.checked = false;
			}
		}
		
		function selectAllRemarks() {
			if (JCComboPlayerPreviewForm.AllRemarks.checked == true) {
				JCComboPlayerPreviewForm.analysis_remarks.checked = true;
			} else {
				JCComboPlayerPreviewForm.analysis_remarks.checked = false;
			}
		}
		
		function selectAllItems(checkedIdx) {
			if (JCComboPlayerPreviewForm.allitems.checked == true) {
				JCComboPlayerPreviewForm.analysis_bg.checked = true;
				JCComboPlayerPreviewForm.analysis_history.checked = true;
				JCComboPlayerPreviewForm.analysis_players.checked = true;
				JCComboPlayerPreviewForm.analysis_remarks.checked = true;
			} else {
				JCComboPlayerPreviewForm.analysis_bg.checked = false;
				JCComboPlayerPreviewForm.analysis_history.checked = false;
				JCComboPlayerPreviewForm.analysis_players.checked = false;
				JCComboPlayerPreviewForm.analysis_remarks.checked = false;
			}
		}
		
		function OnActionChanged(checkedIdx) {
			if (JCComboPlayerPreviewForm.Action.value == 'D') {
				document.JCComboPlayerPreviewForm.SendToPager[0].checked = true;
				document.JCComboPlayerPreviewForm.SendToPager[1].checked = true;
				document.JCComboPlayerPreviewForm.SendToPager[2].checked = true;
				//alert('�нT�wGOGO1,GOGO2�ΰ��|���w����I');
			}
		}
<%
	}
%>

function detect() {    
	if (document.JCComboPlayerPreviewForm.alertTypeRadio[0].checked) {
		document.JCComboPlayerPreviewForm.reference.value = '�ȩw�}�e�A�u�ѰѦ�';
	} else if (document.JCComboPlayerPreviewForm.alertTypeRadio[1].checked) {
		document.JCComboPlayerPreviewForm.reference.value = '�{���}�e�A�u�ѰѦ�';
	}
}
</script>

<html>
<head>
	<META http-equiv="Content-Type" content="text/html; charset=big5">
	<LINK REL="stylesheet" HREF="/sportStyle.css" TYPE="text/css">
</head>
<body>
	<form id="JCComboPlayerPreviewForm" method="post" runat="server" onsubmit="DeviceCheck()">
		<font size="2"><b>�W�����:</b><asp:Label id="historyMsg" runat="server" /></font><br>

		<table border="1" width="100%" style="font: 10pt verdana">
		<tr style="background-color:#2F4F4F" align="center">
			<th><font color="#F0F8FF">�ɨƮɶ�</font></th>
			<th><font color="#F0F8FF">�p��</font></th>
			<th><font color="#F0F8FF">�D��</font></th>
			<th><font color="#F0F8FF">�ȶ�</font></th>
			<th><font color="#F0F8FF">����ʧ@</font></th>
			<th><font color="#F0F8FF">�X���W��<br>����<input type="checkbox" name="AllPlayers" value="1" onClick="selectAllPlayers()"></font></th>
		</tr>
		<span id="AnalysisPreivewInformation" runat="server" />
		<tr>
			<td colspan="6" align="right">
				<!--
					Value of SendToPager is Device ID defined in DEVICE_TYPE
				-->
				<input type="checkbox" name="SendToPager" value="4" checked>JCCombo&nbsp;
				<input type="submit" id="SendBtn" value="�o�e" OnServerClick="onSendAnalysis" runat="server">&nbsp;
				<input type="reset" value="���]">
			</td>
		</table>
	</form>
</body>
</html>