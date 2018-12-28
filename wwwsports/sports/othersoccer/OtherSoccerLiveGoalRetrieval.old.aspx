<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>
<%@ Import Namespace="System.Collections"%>

<script language="C#" runat="server">
	int iRecCount;
	ArrayList leagueList;

	void Page_Load(Object sender,EventArgs e) {
		OtherSoccerLiveGoal goalInfo = new OtherSoccerLiveGoal((string)Application["SoccerDBConnectionString"]);

		try {
			GoalInformation.InnerHtml = goalInfo.GetLiveGoals();
			iRecCount = goalInfo.NumberOfRecords;
			leagueList = goalInfo.LeagueList;
			UpdateHistoryMessage((string)Application["retrieveInfoMsg"] + "�{�����(" + DateTime.Now.ToString("HH:mm:ss") + ")");
		} catch(NullReferenceException) {
			FormsAuthentication.SignOut();
			UpdateHistoryMessage("<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>");
		}
	}

	void LiveGoalAction(Object sender,CommandEventArgs e) {
		int iUpdated = 0;
		string sCmd, sNow;
		OtherSoccerLiveGoal LiveGoalUpd = new OtherSoccerLiveGoal((string)Application["SoccerDBConnectionString"]);

		sCmd = (string)e.CommandName;
		sNow = DateTime.Now.ToString("HH:mm:ss");
		try {
			iUpdated = LiveGoalUpd.UpdateLiveGoals(sCmd);
			Page_Load(sender,e);
			if(sCmd.Equals("SEND")) {
				if(iUpdated > 0) {
					UpdateHistoryMessage("���\��s" + iUpdated.ToString() + "�����(" + sNow + ")");
				}	else if(iUpdated == 0) {
					UpdateHistoryMessage("�S����s���(" + sNow + ")");
				}	else {
					UpdateHistoryMessage((string)Application["transErrorMsg"] + "(" + sNow + ")");
				}
			}	else if(sCmd.Equals("SORT")) {
				if(iUpdated > 0) {
					UpdateHistoryMessage("��s��ƧǸ�(" + sNow + ")");
				}	else if(iUpdated == 0) {
					UpdateHistoryMessage("�S���Ǹ���s(" + sNow + ")");
				}	else {
					UpdateHistoryMessage((string)Application["transErrorMsg"] + "(" + sNow + ")");
				}
			}	else if(sCmd.Equals("HIDE")) {
				if(iUpdated > 0) {
					UpdateHistoryMessage("����" + iUpdated.ToString() + "�����(" + sNow + ")");
				}	else if(iUpdated == 0) {
					UpdateHistoryMessage("�S���������(" + sNow + ")");
				}	else {
					UpdateHistoryMessage((string)Application["transErrorMsg"] + "(" + sNow + ")");
				}
			}	else if(sCmd.Equals("SHOW")) {
				if(iUpdated > 0) {
					UpdateHistoryMessage("��ܩҦ����(" + sNow + ")");
				}	else if(iUpdated == 0) {
					UpdateHistoryMessage("�S��������(" + sNow + ")");
				}	else {
					UpdateHistoryMessage((string)Application["transErrorMsg"] + "(" + sNow + ")");
				}
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
function openOneGoal(matchcnt, oneGoalWinName) {
	oneGoalWinFeature="Resizable=Yes,ScrollBars=Yes,MenuBar=No,Directories=No,ToolBar=No,Location=No,Status=No, width=380, height=110";
	oneGoalWindow = window.open('OtherSoccerPerLiveGoal.aspx?matchcount=' + matchcnt,oneGoalWinName,oneGoalWinFeature);
	oneGoalWindow.focus();
}

<%
	if(iRecCount > 1) {
%>
function ClearOrderNo() {
	<%
		for(int iClear=0;iClear<iRecCount;iClear++) {
	%>
		LiveGoalForm.orderID[<%=iClear.ToString()%>].value = '';
	<%
		}
	%>
}

function onOrder_IDChanged(validate_index) {
	order_re = /^\d{0,3}$/
	re_val = LiveGoalForm.orderID[validate_index].value.search(order_re)
	if(re_val == -1) {
		alert('�Ǹ��u�����Ʀr');
		LiveGoalForm.orderID[validate_index].value = '';
	}
}

function onStatusChanged(validate_index) {
	LiveGoalForm.MUSTSendChk[validate_index].checked = true;
}

function onHost_ScoreChanged(validate_index) {
	host_re = /^\d{0,2}$/
	re_val = LiveGoalForm.hostScore[validate_index].value.search(host_re)
	if(re_val == -1) {
		alert('��ƥu�����Ʀr');
		LiveGoalForm.hostScore[validate_index].value = '0';
	}	else {
		LiveGoalForm.MUSTSendChk[validate_index].checked = true;
	}
}

function onHost_VChanged(validate_index) {
	if(LiveGoalForm.hostValidity[validate_index].value == 0) {
		LiveGoalForm.alertChk[validate_index].checked = true;
		LiveGoalForm.song[validate_index].value = '0112';
	} else if(LiveGoalForm.hostValidity[validate_index].value == 1) {
		LiveGoalForm.alertChk[validate_index].checked = false;
		LiveGoalForm.song[validate_index].value = '0112';
	} else {
		LiveGoalForm.alertChk[validate_index].checked = true;
		LiveGoalForm.song[validate_index].value = '0096';
	}
	LiveGoalForm.MUSTSendChk[validate_index].checked = true;
}

function onGuest_ScoreChanged(validate_index) {
	guest_re = /^\d{0,2}$/
	re_val = LiveGoalForm.guestScore[validate_index].value.search(guest_re)
	if(re_val == -1) {
		alert('��ƥu�����Ʀr');
		LiveGoalForm.guestScore[validate_index].value = '0';
	}	else {
		LiveGoalForm.MUSTSendChk[validate_index].checked = true;
	}
}

function onGuest_VChanged(validate_index) {
	if(LiveGoalForm.guestValidity[validate_index].value == 0) {
		LiveGoalForm.alertChk[validate_index].checked = true;
		LiveGoalForm.song[validate_index].value = '0112';
	} else if(LiveGoalForm.guestValidity[validate_index].value == 1) {
		LiveGoalForm.alertChk[validate_index].checked = false;
		LiveGoalForm.song[validate_index].value = '0112';
	} else {
		LiveGoalForm.alertChk[validate_index].checked = true;
		LiveGoalForm.song[validate_index].value = '0096';
	}
	LiveGoalForm.MUSTSendChk[validate_index].checked = true;
}

function onSongChanged(validate_index) {
	LiveGoalForm.MUSTSendChk[validate_index].checked = true;
}

function onRemarkChanged(validate_index) {
	LiveGoalForm.MUSTSendChk[validate_index].checked = true;
}

function onAlertClicked(validate_index) {
	LiveGoalForm.MUSTSendChk[validate_index].checked = true;
}

function selectAll(iType) {
	<%
		int iSendChkIndex;
		string sSendChk_All;
	%>
	if(iType == 0) {
		if(LiveGoalForm.SelectAllSend.checked == true) {
			<%
				for(iSendChkIndex=0;iSendChkIndex<iRecCount;iSendChkIndex++) {
					sSendChk_All = "MUSTSendChk[" + iSendChkIndex.ToString() + "]";
			%>
					LiveGoalForm.<%=sSendChk_All%>.checked = true;
			<%
				}
			%>
		}	else {
			<%
				for(iSendChkIndex=0;iSendChkIndex<iRecCount;iSendChkIndex++) {
					sSendChk_All = "MUSTSendChk[" + iSendChkIndex.ToString() + "]";
			%>
					LiveGoalForm.<%=sSendChk_All%>.checked = false;
			<%
				}
			%>
		}
	}	else if(iType == 1) {
		if(LiveGoalForm.SelectAllHide.checked == true) {
			<%
				for(iSendChkIndex=0;iSendChkIndex<iRecCount;iSendChkIndex++) {
					sSendChk_All = "hiddenChk[" + iSendChkIndex.ToString() + "]";
			%>
					LiveGoalForm.<%=sSendChk_All%>.checked = true;
			<%
				}
			%>
		}	else {
			<%
				for(iSendChkIndex=0;iSendChkIndex<iRecCount;iSendChkIndex++) {
					sSendChk_All = "hiddenChk[" + iSendChkIndex.ToString() + "]";
			%>
					LiveGoalForm.<%=sSendChk_All%>.checked = false;
			<%
				}
			%>
		}
	}
}
<%
} else {
%>
function ClearOrderNo() {
	LiveGoalForm.orderID.value = '';
}

function onOrder_IDChanged(validate_index) {
	order_re = /^\d{0,3}$/
	re_val = LiveGoalForm.orderID.value.search(order_re)
	if(re_val == -1) {
		alert('�Ǹ��u�����Ʀr');
		LiveGoalForm.orderID.value = '';
	}
}

function onStatusChanged(validate_index) {
	LiveGoalForm.MUSTSendChk.checked = true;
}

function onHost_ScoreChanged(validate_index) {
	host_re = /^\d{0,2}$/
	re_val = LiveGoalForm.hostScore.value.search(host_re)
	if(re_val == -1) {
		alert('��ƥu�����Ʀr');
		LiveGoalForm.hostScore.value = '0';
	}	else {
		LiveGoalForm.MUSTSendChk.checked = true;
	}
}

function onHost_VChanged(validate_index) {
	if(LiveGoalForm.hostValidity.value == 0) {
		LiveGoalForm.alertChk.checked = true;
		LiveGoalForm.song.value = '0112';
	} else if(LiveGoalForm.hostValidity.value == 1) {
		LiveGoalForm.alertChk.checked = false;
		LiveGoalForm.song.value = '0112';
	} else {
		LiveGoalForm.alertChk.checked = true;
		LiveGoalForm.song.value = '0096';
	}
	LiveGoalForm.MUSTSendChk.checked = true;
}

function onGuest_ScoreChanged(validate_index) {
	guest_re = /^\d{0,2}$/
	re_val = LiveGoalForm.guestScore.value.search(guest_re)
	if(re_val == -1) {
		alert('��ƥu�����Ʀr');
		LiveGoalForm.guestScore.value = '0';
	}	else {
		LiveGoalForm.MUSTSendChk.checked = true;
	}
}

function onGuest_VChanged(validate_index) {
	if(LiveGoalForm.guestValidity.value == 0) {
		LiveGoalForm.alertChk.checked = true;
		LiveGoalForm.song.value = '0112';
	} else if(LiveGoalForm.guestValidity.value == 1) {
		LiveGoalForm.alertChk.checked = false;
		LiveGoalForm.song.value = '0112';
	} else {
		LiveGoalForm.alertChk.checked = true;
		LiveGoalForm.song.value = '0096';
	}
	LiveGoalForm.MUSTSendChk.checked = true;
}

function onSongChanged(validate_index) {
	LiveGoalForm.MUSTSendChk.checked = true;
}

function onRemarkChanged(validate_index) {
	LiveGoalForm.MUSTSendChk.checked = true;
}

function onAlertClicked(validate_index) {
	LiveGoalForm.MUSTSendChk.checked = true;
}

function selectAll(iType) {
	if(iType == 0) {
		if(LiveGoalForm.SelectAllSend.checked == true) {
			LiveGoalForm.MUSTSendChk.checked = true;
		}	else {
			LiveGoalForm.MUSTSendChk.checked = false;
		}
	}	else if(iType == 1) {
		if(LiveGoalForm.SelectAllHide.checked == true) {
			LiveGoalForm.hiddenChk.checked = true;
		}	else {
			LiveGoalForm.hiddenChk.checked = false;
		}
	}
}
<%
	}
%>

function SelectLeagueToSend(leagGp) {
	<%
		if(leagueList != null && leagueList.Count > 0) {
			string sLeagChkItem = "";
			if(leagueList.Count > 1) {
				for(int i = 0; i < leagueList.Count; i++) {
					sLeagChkItem = "LeagueChk_" + i.ToString();
	%>
				if(leagGp == '<%=i.ToString()%>') {
					if(LiveGoalForm.<%=sLeagChkItem%>.length > 1) {
						for(j = 0; j < LiveGoalForm.<%=sLeagChkItem%>.length; j++) {
							if(LiveGoalForm.leagGpChk[leagGp].checked == true) {
								LiveGoalForm.MUSTSendChk[LiveGoalForm.<%=sLeagChkItem%>[j].value].checked = true;
							} else {
								LiveGoalForm.MUSTSendChk[LiveGoalForm.<%=sLeagChkItem%>[j].value].checked = false;
							}
						}
					} else {
						if(LiveGoalForm.leagGpChk[leagGp].checked == true) {
							<%
								if(iRecCount > 1) {
							%>
								LiveGoalForm.MUSTSendChk[LiveGoalForm.<%=sLeagChkItem%>.value].checked = true;
							<%
								} else {
							%>
								LiveGoalForm.MUSTSendChk.checked = true;
							<%
								}
							%>
						} else {
							<%
								if(iRecCount > 1) {
							%>
								LiveGoalForm.MUSTSendChk[LiveGoalForm.<%=sLeagChkItem%>.value].checked = false;
							<%
								} else {
							%>
								LiveGoalForm.MUSTSendChk.checked = false;
							<%
								}
							%>
						}
					}
				}
		<%
				}
			} else {
		%>
				if(LiveGoalForm.LeagueChk_0.length > 1) {
					for(j = 0; j < LiveGoalForm.LeagueChk_0.length; j++) {
						if(LiveGoalForm.leagGpChk.checked == true) {
							LiveGoalForm.MUSTSendChk[LiveGoalForm.LeagueChk_0[j].value].checked = true;
						} else {
							LiveGoalForm.MUSTSendChk[LiveGoalForm.LeagueChk_0[j].value].checked = false;
						}
					}
				} else {
					if(LiveGoalForm.leagGpChk.checked == true) {
						LiveGoalForm.MUSTSendChk.checked = true;
					} else {
						LiveGoalForm.MUSTSendChk.checked = false;
					}
				}
	<%
			}
		}
	%>
}

function SelectLeagueToHide(leagGp) {
	<%
		if(leagueList != null && leagueList.Count > 0) {
			string sLeagChkItem = "";
			if(leagueList.Count > 1) {
				for(int i = 0; i < leagueList.Count; i++) {
					sLeagChkItem = "LeagueChk_" + i.ToString();
	%>
				if(leagGp == '<%=i.ToString()%>') {
					if(LiveGoalForm.<%=sLeagChkItem%>.length > 1) {
						for(j = 0; j < LiveGoalForm.<%=sLeagChkItem%>.length; j++) {
							if(LiveGoalForm.leagGpHide[leagGp].checked == true) {
								LiveGoalForm.hiddenChk[LiveGoalForm.<%=sLeagChkItem%>[j].value].checked = true;
							} else {
								LiveGoalForm.hiddenChk[LiveGoalForm.<%=sLeagChkItem%>[j].value].checked = false;
							}
						}
					} else {
						if(LiveGoalForm.leagGpHide[leagGp].checked == true) {
							<%
								if(iRecCount > 1) {
							%>
								LiveGoalForm.hiddenChk[LiveGoalForm.<%=sLeagChkItem%>.value].checked = true;
							<%
								} else {
							%>
								LiveGoalForm.hiddenChk.checked = true;
							<%
								}
							%>
						} else {
							<%
								if(iRecCount > 1) {
							%>
								LiveGoalForm.hiddenChk[LiveGoalForm.<%=sLeagChkItem%>.value].checked = false;
							<%
								} else {
							%>
								LiveGoalForm.hiddenChk.checked = false;
							<%
								}
							%>
						}
					}
				}
		<%
				}
			} else {
		%>
				if(LiveGoalForm.LeagueChk_0.length > 1) {
					for(j = 0; j < LiveGoalForm.LeagueChk_0.length; j++) {
						if(LiveGoalForm.leagGpHide.checked == true) {
							LiveGoalForm.hiddenChk[LiveGoalForm.LeagueChk_0[j].value].checked = true;
						} else {
							LiveGoalForm.hiddenChk[LiveGoalForm.LeagueChk_0[j].value].checked = false;
						}
					}
				} else {
					if(LiveGoalForm.leagGpHide.checked == true) {
						LiveGoalForm.hiddenChk.checked = true;
					} else {
						LiveGoalForm.hiddenChk.checked = false;
					}
				}
	<%
			}
		}
	%>
}

function ReSort(sortType) {
	parent.content_frame.location.replace('OtherSoccerLiveGoalRetrieval.aspx?sort=' + sortType);
}

function ShowHiddenMatches() {
	matchWinFeature="Resizable=Yes,ScrollBars=Yes,MenuBar=No,Directories=No,ToolBar=No,Location=No,Status=No, width=400, height=680";
	matchWindow = window.open('OtherSoccerHiddenMatch.aspx?type=goal', 'HiddenMatch' , matchWinFeature);
	matchWindow.focus();
}
</script>

<html>
<head>
	<META http-equiv="Content-Type" content="text/html; charset=big5">
	<LINK REL="stylesheet" HREF="/sportStyle.css" TYPE="text/css">
</head>
<body>
	<form id="LiveGoalForm" method="post" runat="server">
		<font size="2"><b>�W�����:</b><asp:Label id="historyMsg" runat="server" /></font><br>

		<table border="1" width="100%" style="font: 10pt verdana">
		<tr style="background-color:#FFC39C; color:#191970">
			<th colspan="14" align="left">
				<%
					if(leagueList != null && leagueList.Count > 0) {
				%>
					�������:&nbsp;
				<%
						if(leagueList.Count > 1) {
							for(int i = 0; i < leagueList.Count; i++) {
				%>
							<input type="checkbox" name="leagGpHide" value="<%=i.ToString()%>" onClick="SelectLeagueToHide(LiveGoalForm.leagGpHide[<%=i.ToString()%>].value)"><%=leagueList[i]%>&nbsp;&nbsp;
				<%
							}
						} else {
				%>
							<input type="checkbox" name="leagGpHide" value="0" onClick="SelectLeagueToHide(LiveGoalForm.leagGpHide.value)"><%=leagueList[0]%>&nbsp;&nbsp;
				<%
						}
					}
				%>
			</th>
		</tr>

		<tr style="background-color:#FFFAF0; color:#CD5C5C">
			<th colspan="14" align="left">
				<%
					if(leagueList != null && leagueList.Count > 0) {
				%>
					��ܶǰe:&nbsp;
				<%
						if(leagueList.Count > 1) {
							for(int i = 0; i < leagueList.Count; i++) {
				%>
							<input type="checkbox" name="leagGpChk" value="<%=i.ToString()%>" onClick="SelectLeagueToSend(LiveGoalForm.leagGpChk[<%=i.ToString()%>].value)"><%=leagueList[i]%>&nbsp;&nbsp;
				<%
							}
						} else {
				%>
							<input type="checkbox" name="leagGpChk" value="0" onClick="SelectLeagueToSend(LiveGoalForm.leagGpChk.value)"><%=leagueList[0]%>&nbsp;&nbsp;
				<%
						}
					}
				%>
			</th>
		</tr>

		<tr style="background-color:#ADADD6">
			<th colspan="5" align="left">
				<input type="button" name="clearOrder" value="�M���Ǹ�" onClick="ClearOrderNo()">&nbsp;&nbsp;
				���Ǩ�
				<select name="sortType" onChange="ReSort(LiveGoalForm.sortType.value)">
				<%
					if(Session["user_sortType"].ToString().Equals("0")) {
				%>
					<option value="0">�p��</option>
					<option value="1">�Ǹ�</option>
				<%
					} else {
				%>
					<option value="1">�Ǹ�</option>
					<option value="0">�p��</option>
				<%
					}
				%>
				</select>
			</th>
			<th colspan="4">�{�����</th>
			<th colspan="3"></th>
			<th style="background-color:#FFFAF0; color:#CD5C5C">����<input type="checkbox" name="SelectAllSend" onClick="selectAll(0)"></th>
			<th style="background-color:#FFC39C; color:#191970">����<input type="checkbox" name="SelectAllHide" onClick="selectAll(1)"></th>
		</tr>

		<tr style="background-color:#ADADD6">
			<th>�Ƨ�</th>
			<th>���</th>
			<th>�ɶ�</th>
			<th>�ɬq</th>
			<th>�p��</th>
			<th colspan="2">�D��</th>
			<th colspan="2">�ȶ�</th>
			<th>�q��</th>
			<th>�Ƶ�</th>
			<th>�J�y<br>����</th>
			<th style="background-color:#FFFAF0; color:#CD5C5C">�j��<br>�ǰe</th>
			<th style="background-color:#FFC39C; color:#191970">����</th>
		</tr>

		<span id="GoalInformation" runat="server" />

		<tr>
			<td colspan="7" align="left">
				��<font color="blue">�p��</font>�i�J��Ƥp�e���F
				��<font color="blue">�i��ɶ�</font>�ץ��ɨƶi�椧�ɶ��C
			</td>
			<td style="background-color:#FFFAF0" colspan="6" align="right">
				<!--
					Value of SendToPager is Device ID defined in DEVICE_TYPE
				-->
				<input type="checkbox" name="SendToPager" value="1" checked>GOGO1,GOGO2&nbsp;
				<input type="checkbox" name="SendToPager" value="3" checked>���|��&nbsp;
				<input type="checkbox" name="SendToPager" value="5" checked>GOGO3Combo&nbsp;
				<asp:Button id="SendBtn" Text="�ǰe" CommandName="SEND" OnCommand="LiveGoalAction" runat="server" />
			</td>
			<td></td>
		</tr>
		<tr>
			<td colspan="10"><asp:Button id="SortBtn" Text="�x�s�Ǹ�" CommandName="SORT" tabindex="500" OnCommand="LiveGoalAction" runat="server" /></td>
			<td style="background-color:#FFC39C" colspan="3" align="right"><asp:Button id="ShowBtn" Text="��ܩҦ�" CommandName="SHOW" OnCommand="LiveGoalAction" runat="server" /></td>
			<td style="background-color:#FFC39C" align="center"><asp:Button id="HideBtn" Text="����" CommandName="HIDE" OnCommand="LiveGoalAction" runat="server" /></td>
		</tr>

		<tr>
			<th align="right" colspan="13" style="background-color:#FFC39C; color:#191970">
				<a href="javascript:ShowHiddenMatches()">�w���ê��ɨ�</a>
			</th>
		</tr>
		</table>
	</form>
</body>
</html>