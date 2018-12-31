<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">
	int m_MsgCnt;
	int m_MatchRecordCnt;
	void Page_Load(Object sender,EventArgs e) {
		SportNews newsInfo = new SportNews(ConfigurationManager.AppSettings["SoccerDBConnectionString"]);

		try {
			NewsInformation.InnerHtml = newsInfo.GetMessage();
			m_MsgCnt = newsInfo.MessageCount;
			m_MatchRecordCnt = newsInfo.MatchRecordCount;
			UpdateHistoryMessage(ConfigurationManager.AppSettings["retrieveInfoMsg"] + "其他資訊(" + DateTime.Now.ToString("HH:mm:ss") + ")");
		} catch(NullReferenceException) {
			FormsAuthentication.SignOut();
			UpdateHistoryMessage("<a href=\"/sports/index.htm\" target=\"_top\">" + ConfigurationManager.AppSettings["sessionExpiredMsg"] + "</a>");
		}
	}

	void SendNews(Object sender,EventArgs e) {
		int iUpdated = 0;
		string sNow;
		SportNews NewsSave = new SportNews(ConfigurationManager.AppSettings["SoccerDBConnectionString"]);

		sNow = DateTime.Now.ToString("HH:mm:ss");
		try {
			iUpdated = NewsSave.SendNews();
			Page_Load(sender,e);
			if(iUpdated > 0) {
				UpdateHistoryMessage("成功更新其他資訊(" + sNow + ")");
			}	else if (iUpdated == 0) {
				UpdateHistoryMessage("沒有更新其他資訊(" + sNow + ")");
			}	else {
				UpdateHistoryMessage(ConfigurationManager.AppSettings["transErrorMsg"] + "(" + sNow + ")");
			}
		}	catch(NullReferenceException nullex) {
			FormsAuthentication.SignOut();
			UpdateHistoryMessage("<a href=\"/sports/index.htm\" target=\"_top\">" + ConfigurationManager.AppSettings["sessionExpiredMsg"] + "</a>");
		}
	}

	void UpdateHistoryMessage(string sMsg) {
		historyMsg.Text = sMsg;
	}
</script>

<script language="JavaScript">
function LengthOfString(s) {
	len = 0;
	for(i = 0; i < s.length; i++) {
		if((s.charCodeAt(i) <= 255) && (s.charCodeAt(i) >= 0)) {
			len++;
		} else {
			len = len + 2;
		}
	}
	return len;
}

<%
if(m_MsgCnt > 1) {
%>
	function DeviceCheck() {
		<%
			int iActIdx;
			string sAction;
			string sMustSend;
	
			for(iActIdx=0;iActIdx<m_MsgCnt;iActIdx++) {
				sAction = "Action[" + iActIdx.ToString() + "]";
				sMustSend = "mustSend[" + iActIdx.ToString() + "]";
		%>
				if(NewsInfoForm.<%=sAction%>.value == 'D') {
					NewsInfoForm.SendToPager[0].checked = true;
					NewsInfoForm.SendToPager[1].checked = true;
					NewsInfoForm.SendToPager[2].checked = true;
				} else {
					if(NewsInfoForm.SendToPager[1].checked == true) {
						NewsInfoForm.SendToPager[0].checked = true;
					}
				}
				
				if (NewsInfoForm.<%=sMustSend%>.disabled == true) {
					NewsInfoForm.<%=sMustSend%>.disabled = false;
				}
		<%
			}
		%>
		
		if (NewsInfoForm.AppID.value >= 409 && NewsInfoForm.AppID.value <= 443) {
			if (NewsInfoForm.SportMsg0.value == '') {
				alert ('Could not be empty content');
				NewsInfoForm.mustSend[0].disabled = true;
				NewsInfoForm.mustSend[1].disabled = true;
				NewsInfoForm.mustSend[2].disabled = true;
				NewsInfoForm.mustSend[3].disabled = true;
				if (NewsInfoForm.AppID.value >= 441 && NewsInfoForm.AppID.value <= 443) {
					NewsInfoForm.mustSend[4].disabled = true;
				}
				return false;
			} else if (NewsInfoForm.SportMsg1.value == '') {
				alert ('Could not be empty content');
				NewsInfoForm.mustSend[0].disabled = true;
				NewsInfoForm.mustSend[1].disabled = true;
				NewsInfoForm.mustSend[2].disabled = true;
				NewsInfoForm.mustSend[3].disabled = true;
				if (NewsInfoForm.AppID.value >= 441 && NewsInfoForm.AppID.value <= 443) {
					NewsInfoForm.mustSend[4].disabled = true;
				}
				return false;
			} else if (NewsInfoForm.SportMsg2.value == '') {
				alert ('Could not be empty content');	
				NewsInfoForm.mustSend[0].disabled = true;
				NewsInfoForm.mustSend[1].disabled = true;
				NewsInfoForm.mustSend[2].disabled = true;
				NewsInfoForm.mustSend[3].disabled = true;
				if (NewsInfoForm.AppID.value >= 441 && NewsInfoForm.AppID.value <= 443) {
					NewsInfoForm.mustSend[4].disabled = true;
				}
				return false;
			} else if (NewsInfoForm.SportMsg3.value == '') {
				alert ('Could not be empty content');
				NewsInfoForm.mustSend[0].disabled = true;
				NewsInfoForm.mustSend[1].disabled = true;
				NewsInfoForm.mustSend[2].disabled = true;
				NewsInfoForm.mustSend[3].disabled = true;
				if (NewsInfoForm.AppID.value >= 441 && NewsInfoForm.AppID.value <= 443) {
					NewsInfoForm.mustSend[4].disabled = true;
				}
				return false;
			} else {
				return true;
			}
		} else {
			return true;
		}
	}
	
	function ActionChange(idx) {
		NewsInfoForm.mustSend[idx].checked = true;
		if(NewsInfoForm.Action[idx].value == 'D') {
			NewsInfoForm.SendToPager[0].checked = true;
			NewsInfoForm.SendToPager[1].checked = true;
			NewsInfoForm.SendToPager[2].checked = true;
			//alert('請確定GOGO1,GOGO2及馬會機已選取！');
		}
	}
	
	function MsgChange(idx) {
		<%
			for(int i=0; i<m_MsgCnt; i++) {
		%>
			str = document.NewsInfoForm.SportMsg<%=i%>.value;
			strlen = LengthOfString(str);
			if(strlen > 400) {
				alert('訊息內容不能多於400個立元');
			}
			NewsInfoForm.mustSend[idx].checked = true;
		<%
			}
		%>
	}
	
	function selectAll() {
		<%
			int iSendChkIndex;
			string sSendChk_All;
		%>
		if(NewsInfoForm.AllSend.checked == true) {
			<%
				for(iSendChkIndex=0;iSendChkIndex<m_MsgCnt;iSendChkIndex++) {
					sSendChk_All = "mustSend[" + iSendChkIndex.ToString() + "]";
			%>
					NewsInfoForm.<%=sSendChk_All%>.checked = true;
			<%
				}
			%>
		} else if(NewsInfoForm.AllSend.checked == false) {
			<%
				for(iSendChkIndex=0;iSendChkIndex<m_MsgCnt;iSendChkIndex++) {
					sSendChk_All = "mustSend[" + iSendChkIndex.ToString() + "]";
			%>
					NewsInfoForm.<%=sSendChk_All%>.checked = false;
			<%
				}
			%>
		}
	}
<%
} else {
%>
	function DeviceCheck() {
		if(NewsInfoForm.Action.value == 'D') {
			NewsInfoForm.SendToPager[0].checked = true;
			NewsInfoForm.SendToPager[1].checked = true;
			NewsInfoForm.SendToPager[2].checked = true;
		} else {
			if(NewsInfoForm.SendToPager[1].checked == true) {
				NewsInfoForm.SendToPager[0].checked = true;
			}
		}
				
		if (NewsInfoForm.mustSend[0].disabled == true) {
			NewsInfoForm.mustSend[0].disabled = false;
		}
		if (NewsInfoForm.mustSend[1].disabled == true) {
			NewsInfoForm.mustSend[1].disabled = false;
		}
		if (NewsInfoForm.mustSend[2].disabled == true) {
			NewsInfoForm.mustSend[2].disabled = false
		}
		if (NewsInfoForm.mustSend[3].disabled == true) {
			NewsInfoForm.mustSend[3].disabled = false;
		}
		
		if (NewsInfoForm.AppID.value >= 409 && NewsInfoForm.AppID.value <= 443) {
			if (NewsInfoForm.SportMsg0.value == '') {
				alert ('Could not be empty content');
				NewsInfoForm.mustSend[0].disabled = true;
				NewsInfoForm.mustSend[1].disabled = true;
				NewsInfoForm.mustSend[2].disabled = true;
				NewsInfoForm.mustSend[3].disabled = true;
				if (NewsInfoForm.AppID.value >= 441 && NewsInfoForm.AppID.value <= 443) {
					NewsInfoForm.mustSend[4].disabled = true;
				}
				return false;
			} else if (NewsInfoForm.SportMsg1.value == '') {
				alert ('Could not be empty content');
				NewsInfoForm.mustSend[0].disabled = true;
				NewsInfoForm.mustSend[1].disabled = true;
				NewsInfoForm.mustSend[2].disabled = true;
				NewsInfoForm.mustSend[3].disabled = true;
				if (NewsInfoForm.AppID.value >= 441 && NewsInfoForm.AppID.value <= 443) {
					NewsInfoForm.mustSend[4].disabled = true;
				}
				return false;
			} else if (NewsInfoForm.SportMsg2.value == '') {
				alert ('Could not be empty content');	
				NewsInfoForm.mustSend[0].disabled = true;
				NewsInfoForm.mustSend[1].disabled = true;
				NewsInfoForm.mustSend[2].disabled = true;
				NewsInfoForm.mustSend[3].disabled = true;
				if (NewsInfoForm.AppID.value >= 441 && NewsInfoForm.AppID.value <= 443) {
					NewsInfoForm.mustSend[4].disabled = true;
				}
				return false;
			} else if (NewsInfoForm.SportMsg3.value == '') {
				alert ('Could not be empty content');
				NewsInfoForm.mustSend[0].disabled = true;
				NewsInfoForm.mustSend[1].disabled = true;
				NewsInfoForm.mustSend[2].disabled = true;
				NewsInfoForm.mustSend[3].disabled = true;
				if (NewsInfoForm.AppID.value >= 441 && NewsInfoForm.AppID.value <= 443) {
					NewsInfoForm.mustSend[4].disabled = true;
				}
				return false;
			} else {
				return true;
			}
		} else {
			return true;
		}
	}
	
	function ActionChange(idx) {
		NewsInfoForm.mustSend.checked = true;
		if(NewsInfoForm.Action.value == 'D') {
			NewsInfoForm.SendToPager[0].checked = true;
			NewsInfoForm.SendToPager[1].checked = true;
			NewsInfoForm.SendToPager[2].checked = true;
			//alert('請確定GOGO1,GOGO2及馬會機已選取！');
		}
	}
	
	function MsgChange(idx) {
		str = document.NewsInfoForm.SportMsg0.value;
		strlen = LengthOfString(str);
		if(strlen > 400) {
			alert('訊息內容不能多於400個位元');
		}
		NewsInfoForm.mustSend.checked = true;
	}
	
	function selectAll() {
		if(NewsInfoForm.AllSend.checked == true) {
			NewsInfoForm.mustSend.checked = true;
		} else if(NewsInfoForm.AllSend.checked == false) {
			NewsInfoForm.mustSend.checked = false;
		}
	}
<%
}
%>

<%
if(m_MatchRecordCnt > 1) {
%>
	function selectAllMatches() {
		<%
			int iChkIndex;
			string sChk_All;
		%>
		if(NewsInfoForm.selectedAllMatches.checked == true) {
			<%
				for(iChkIndex=0;iChkIndex<m_MatchRecordCnt;iChkIndex++) {
					sChk_All = "selectedMatch[" + iChkIndex.ToString() + "]";
			%>
					NewsInfoForm.<%=sChk_All%>.checked = true;
			<%
				}
			%>
		} else if(NewsInfoForm.selectedAllMatches.checked == false) {
			<%
				for(iChkIndex=0;iChkIndex<m_MatchRecordCnt;iChkIndex++) {
					sChk_All = "selectedMatch[" + iChkIndex.ToString() + "]";
			%>
					NewsInfoForm.<%=sChk_All%>.checked = false;
			<%
				}
			%>
		}
	}
<%
} else {
%>
	function selectAllMatches() {
		if(NewsInfoForm.selectedAllMatches.checked == true) {
			NewsInfoForm.selectedMatch.checked = true;
		} else if(NewsInfoForm.selectedAllMatches.checked == false) {
			NewsInfoForm.selectedMatch.checked = false;
		}
	}
<%
}
%>
</script>

<html>
<head>
	<META http-equiv="Content-Type" content="text/html; charset=big5">
	<LINK REL="stylesheet" HREF="/sportStyle.css" TYPE="text/css">
</head>
<body>
	<form id="NewsInfoForm" method="post" onsubmit="return DeviceCheck()" runat="server">
		<font size="2"><b>上次行動:</b><asp:Label id="historyMsg" runat="server" /></font><br>

		<table border="1" width="80%" style="font: 10pt verdana">
		<tr>
			<th colspan="3" align="left">訊息不能多於400個位元</th>
			<th align="center">全選<input type="checkbox" name="AllSend" value="1" onClick="selectAll()"></th>
		</tr>
		<span id="NewsInformation" runat="server" />
		<tr>
			<td colspan="4" align="center">
				<font color="red"><b>在刪除資料時，請確定所有傳呼機已選取，以避免傳呼機資料有錯誤！</b></font>&nbsp;&nbsp;&nbsp;
		</tr>
		<tr>
			<td colspan="4" align="right">
				<!--
					Value of SendToPager is Device ID defined in DEVICE_TYPE
				-->
				<input type="checkbox" name="SendToPager" value="1" checked>GOGO1&nbsp;
				<input type="checkbox" name="SendToPager" value="2" checked>GOGO2&nbsp;
				<input type="checkbox" name="SendToPager" value="3" checked>馬會機&nbsp;
				<input type="checkbox" name="SendToPager" value="4" checked>JCCombo&nbsp;
				<input type="submit" id="SendBtn" value="發送" OnServerClick="SendNews" runat="server">&nbsp;
				<input type="reset" value="重設">
			</td>
		</tr>
		</table>
	</form>
</body>
</html>