<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>
<%@ Import Namespace="System.Collections"%>

<script language="C#" runat="server">
	int iRecCount;
	string sRegion;
	string sRegionID;
	string sTotalOdds;
	ArrayList leagueList;

	void Page_Load(Object sender,EventArgs e) {
		RegionModify regionInfo = new RegionModify((string)Application["SoccerDBConnectionString"]);

		try {
			MatchesInformation.InnerHtml = regionInfo.GetAllMatches();
			iRecCount = regionInfo.NumberOfRecords;
			sRegion = regionInfo.Region;
			sTotalOdds = regionInfo.TotalOdds;
			leagueList = regionInfo.LeagueList;
			sRegionID = regionInfo.RegionID;
			UpdateHistoryMessage((string)Application["retrieveInfoMsg"] + sRegion + "賽事(" + DateTime.Now.ToString("HH:mm:ss") + ")");
		} catch(NullReferenceException) {
			FormsAuthentication.SignOut();
			UpdateHistoryMessage("<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>");
		}
	}

	void GetMatchesAction(Object sender,CommandEventArgs e) {
		int iUpdated = 0;
		string sCmd, sNow;
		RegionModify matchUpd = new RegionModify((string)Application["SoccerDBConnectionString"]);

		sCmd = (string)e.CommandName;
		sNow = DateTime.Now.ToString("HH:mm:ss");
		try {
			iUpdated = matchUpd.UpdateMatches(sCmd);
			Page_Load(sender,e);
			if(sCmd.Equals("SEND")) {
				if(iUpdated > 0) {
					UpdateHistoryMessage("成功更新" + iUpdated.ToString() + "場" + sRegion + "賽事(" + sNow + ")");
				}	else if(iUpdated == 0) {
					UpdateHistoryMessage("沒有更新" + sRegion + "賽事(" + sNow + ")");
				}	else {
					UpdateHistoryMessage((string)Application["transErrorMsg"] + "(" + sNow + ")");
				}
			}	else if(sCmd.Equals("SORT")) {
				if(iUpdated > 0) {
					UpdateHistoryMessage("成功更新" + sRegion + "賽事序號(" + sNow + ")");
				}	else if(iUpdated == 0) {
					UpdateHistoryMessage("沒有序號更新(" + sNow + ")");
				}	else {
					UpdateHistoryMessage((string)Application["transErrorMsg"] + "(" + sNow + ")");
				}
			}	else if(sCmd.Equals("HIDE")) {
				if(iUpdated > 0) {
					UpdateHistoryMessage("隱藏" + iUpdated.ToString() + "場" + sRegion + "賽事(" + sNow + ")");
				}	else if(iUpdated == 0) {
					UpdateHistoryMessage("沒有" + sRegion + "賽事隱藏(" + sNow + ")");
				}	else {
					UpdateHistoryMessage((string)Application["transErrorMsg"] + "(" + sNow + ")");
				}
			}	else if(sCmd.Equals("SHOW")) {
				if(iUpdated > 0) {
					UpdateHistoryMessage("顯示所有" + sRegion + "賽事(" + sNow + ")");
				} else if(iUpdated == 0) {
					UpdateHistoryMessage("沒有" + sRegion + "賽事顯示(" + sNow + ")");
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

	void SaveTotalOdds(Object sender,EventArgs e) {
		int iUpdated = 0;
		string sNow;
		RegionModify oddsUpd = new RegionModify((string)Application["SoccerDBConnectionString"]);

		sNow = DateTime.Now.ToString("HH:mm:ss");
		try {
			iUpdated = oddsUpd.UpdateTotalOdds();
			Page_Load(sender,e);
			if(iUpdated > 0) {
				UpdateHistoryMessage("成功更新" + sRegion + "賠率總數(" + sNow + ")");
			}	else if(iUpdated == 0) {
				UpdateHistoryMessage("沒有更新" + sRegion + "賠率總數(" + sNow + ")");
			}	else {
				UpdateHistoryMessage((string)Application["transErrorMsg"] + "(" + sNow + ")");
			}
		} catch(NullReferenceException nullex) {
			FormsAuthentication.SignOut();
			UpdateHistoryMessage("<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>");
		}
	}
</script>

<script language="JavaScript">
function DeviceCheck() {
	if(RegionMatchesForm.SendToPager[1].checked == true) {
		RegionMatchesForm.SendToPager[0].checked = true;
	}
}

<%
	if(iRecCount > 1) {
%>
function ClearOrderNo() {
	<%
		for(int iClear=0;iClear<iRecCount;iClear++) {
	%>
		RegionMatchesForm.orderID[<%=iClear.ToString()%>].value = '';
	<%
		}
	%>
}

function onOrder_IDChanged(validate_index) {
	order_re = /^\d{0,3}$/
	re_val = RegionMatchesForm.orderID[validate_index].value.search(order_re)
	if(re_val == -1) {
		alert('序號只接受數字');
		RegionMatchesForm.orderID[validate_index].value = '';
	}
}

function Handicap1Validity(validate_index) {
	if(RegionMatchesForm.Handicap1[validate_index].value != '') {
		var len = RegionMatchesForm.Handicap1[validate_index].value.length;
		if(eval(len) == 1) {
			re=/\d/
			x = RegionMatchesForm.Handicap1[validate_index].value.search(re);
			if(x == -1) {
				RegionMatchesForm.Handicap1[validate_index].value = '';
				alert('不正確讓球數字');
			} else {
				RegionMatchesForm.MUSTSendChk[validate_index].checked = true;
			}
		}	else if(eval(len) > 1) {
			re=/\d\x2e\d/
			x = RegionMatchesForm.Handicap1[validate_index].value.search(re);
			if(x == -1) {
				RegionMatchesForm.Handicap1[validate_index].value = '';
				alert('不正確讓球數字');
			}	else {
				RegionMatchesForm.MUSTSendChk[validate_index].checked = true;
			}
		}
	} else {
		RegionMatchesForm.MUSTSendChk[validate_index].checked = true;
	}
}

function Handicap2Validity(validate_index) {
	if(RegionMatchesForm.Handicap2[validate_index].value != '') {
		var len = RegionMatchesForm.Handicap2[validate_index].value.length;
		if(eval(len) == 1) {
			re=/\d/
			x = RegionMatchesForm.Handicap2[validate_index].value.search(re);
			if(x == -1) {
				RegionMatchesForm.Handicap2[validate_index].value = '';
				alert('不正確讓球數字');
			}	else {
				RegionMatchesForm.MUSTSendChk[validate_index].checked = true;
			}
		} else if(eval(len) > 1) {
			re=/\d\x2e\d/
			x = RegionMatchesForm.Handicap2[validate_index].value.search(re);
			if(x == -1) {
				RegionMatchesForm.Handicap2[validate_index].value = '';
				alert('不正確讓球數字');
			}	else {
				RegionMatchesForm.MUSTSendChk[validate_index].checked = true;
			}
		}
	} else {
		RegionMatchesForm.MUSTSendChk[validate_index].checked = true;
	}
}

function UpOddsValidity(validate_index) {
	if(RegionMatchesForm.UpOdds[validate_index].value != '') {
		var len = RegionMatchesForm.UpOdds[validate_index].value.length;
		if(eval(len) == 1) {
			re=/\d/
			x = RegionMatchesForm.UpOdds[validate_index].value.search(re);
			if(x == -1) {
				RegionMatchesForm.UpOdds[validate_index].value = '';
				alert('不正確賠率數字');
			}	else {
				<%
					if(!sTotalOdds.Equals("")) {
				%>
					RegionMatchesForm.DownOdds[validate_index].value = <%=sTotalOdds%> - RegionMatchesForm.UpOdds[validate_index].value;
					RegionMatchesForm.DownOdds[validate_index].value = Math.round(RegionMatchesForm.DownOdds[validate_index].value*1000)/1000;
				<%
					}
				%>
				RegionMatchesForm.MUSTSendChk[validate_index].checked = true;
			}
		} else if(eval(len) > 1) {
			re=/\d\x2e\d{1,3}/
			x = RegionMatchesForm.UpOdds[validate_index].value.search(re);
			if(x == -1) {
				RegionMatchesForm.UpOdds[validate_index].value = '';
				alert('不正確賠率數字');
			}	else {
				<%
					if(!sTotalOdds.Equals("")) {
				%>
					RegionMatchesForm.DownOdds[validate_index].value = <%=sTotalOdds%> - RegionMatchesForm.UpOdds[validate_index].value;
					RegionMatchesForm.DownOdds[validate_index].value = Math.round(RegionMatchesForm.DownOdds[validate_index].value*1000)/1000;
				<%
					}
				%>
				RegionMatchesForm.MUSTSendChk[validate_index].checked = true;
			}
		}
	}	else {
		RegionMatchesForm.MUSTSendChk[validate_index].checked = true;
	}
}

function DownOddsValidity(validate_index) {
	if(RegionMatchesForm.DownOdds[validate_index].value != '') {
		var len = RegionMatchesForm.DownOdds[validate_index].value.length;
		if(eval(len) == 1) {
			re=/\d/
			x = RegionMatchesForm.DownOdds[validate_index].value.search(re);
			if(x == -1) {
				RegionMatchesForm.DownOdds[validate_index].value = '';
				alert('不正確賠率數字');
			}	else {
				RegionMatchesForm.MUSTSendChk[validate_index].checked = true;
			}
		}	else if(eval(len) > 1) {
			re=/\d\x2e\d{1,3}/
			x = RegionMatchesForm.DownOdds[validate_index].value.search(re);
			if(x == -1) {
				RegionMatchesForm.DownOdds[validate_index].value = '';
				alert('不正確賠率數字');
			}	else {
				RegionMatchesForm.MUSTSendChk[validate_index].checked = true;
			}
		}
	} else {
		RegionMatchesForm.MUSTSendChk[validate_index].checked = true;
	}
}

function EuroWinOddsValidity(validate_index) {
	if(RegionMatchesForm.EuroWinOdds[validate_index].value != '') {
		var len = RegionMatchesForm.EuroWinOdds[validate_index].value.length;
		if(eval(len) == 1) {
			re=/\d/
			x = RegionMatchesForm.EuroWinOdds[validate_index].value.search(re);
			if(x == -1) {
				RegionMatchesForm.EuroWinOdds[validate_index].value = '';
				alert('不正確賠率數字');
			}	else {
				RegionMatchesForm.MUSTSendChk[validate_index].checked = true;
			}
		}	else if(eval(len) == 2) {
			re=/\d{2,2}/
			x = RegionMatchesForm.EuroWinOdds[validate_index].value.search(re);
			if(x == -1) {
				RegionMatchesForm.EuroWinOdds[validate_index].value = '';
				alert('不正確賠率數字');
			}	else {
				RegionMatchesForm.MUSTSendChk[validate_index].checked = true;
			}
		} else if(eval(len) == 3) {
			re=/\d\x2e\d/
			x = RegionMatchesForm.EuroWinOdds[validate_index].value.search(re);
			if(x == -1) {
				RegionMatchesForm.EuroWinOdds[validate_index].value = '';
				alert('不正確賠率數字');
			}	else {
				RegionMatchesForm.MUSTSendChk[validate_index].checked = true;
			}
		} else if(eval(len) > 3) {
			re=/\d\x2e\d{2,2}/
			x = RegionMatchesForm.EuroWinOdds[validate_index].value.search(re);
			if(x == -1) {
				re2=/\d{2,2}\x2e\d/
				y = RegionMatchesForm.EuroWinOdds[validate_index].value.search(re2);
				if(y == -1) {
					RegionMatchesForm.EuroWinOdds[validate_index].value = '';
					alert('不正確賠率數字');
				} else {
					RegionMatchesForm.MUSTSendChk[validate_index].checked = true;
				}
			}	else {
				RegionMatchesForm.MUSTSendChk[validate_index].checked = true;
			}
		}
	} else {
		RegionMatchesForm.MUSTSendChk[validate_index].checked = true;
	}
}

function EuroLossOddsValidity(validate_index) {
	if(RegionMatchesForm.EuroLossOdds[validate_index].value != '') {
		var len = RegionMatchesForm.EuroLossOdds[validate_index].value.length;
		if(eval(len) == 1) {
			re=/\d/
			x = RegionMatchesForm.EuroLossOdds[validate_index].value.search(re);
			if(x == -1) {
				RegionMatchesForm.EuroLossOdds[validate_index].value = '';
				alert('不正確賠率數字');
			}	else {
				RegionMatchesForm.MUSTSendChk[validate_index].checked = true;
			}
		}	else if(eval(len) == 2) {
			re=/\d{2,2}/
			x = RegionMatchesForm.EuroLossOdds[validate_index].value.search(re);
			if(x == -1) {
				RegionMatchesForm.EuroLossOdds[validate_index].value = '';
				alert('不正確賠率數字');
			}	else {
				RegionMatchesForm.MUSTSendChk[validate_index].checked = true;
			}
		} else if(eval(len) == 3) {
			re=/\d\x2e\d/
			x = RegionMatchesForm.EuroLossOdds[validate_index].value.search(re);
			if(x == -1) {
				RegionMatchesForm.EuroLossOdds[validate_index].value = '';
				alert('不正確賠率數字');
			}	else {
				RegionMatchesForm.MUSTSendChk[validate_index].checked = true;
			}
		} else if(eval(len) > 3) {
			re=/\d\x2e\d{2,2}/
			x = RegionMatchesForm.EuroLossOdds[validate_index].value.search(re);
			if(x == -1) {
				re2=/\d{2,2}\x2e\d/
				y = RegionMatchesForm.EuroLossOdds[validate_index].value.search(re2);
				if(y == -1) {
					RegionMatchesForm.EuroLossOdds[validate_index].value = '';
					alert('不正確賠率數字');
				} else {
					RegionMatchesForm.MUSTSendChk[validate_index].checked = true;
				}
			}	else {
				RegionMatchesForm.MUSTSendChk[validate_index].checked = true;
			}
		}
	} else {
		RegionMatchesForm.MUSTSendChk[validate_index].checked = true;
	}
}

function EuroDrawOddsValidity(validate_index) {
	if(RegionMatchesForm.EuroDrawOdds[validate_index].value != '') {
		var len = RegionMatchesForm.EuroDrawOdds[validate_index].value.length;
		if(eval(len) == 1) {
			re=/\d/
			x = RegionMatchesForm.EuroDrawOdds[validate_index].value.search(re);
			if(x == -1) {
				RegionMatchesForm.EuroDrawOdds[validate_index].value = '';
				alert('不正確賠率數字');
			}	else {
				RegionMatchesForm.MUSTSendChk[validate_index].checked = true;
			}
		}	else if(eval(len) == 2) {
			re=/\d{2,2}/
			x = RegionMatchesForm.EuroDrawOdds[validate_index].value.search(re);
			if(x == -1) {
				RegionMatchesForm.EuroDrawOdds[validate_index].value = '';
				alert('不正確賠率數字');
			}	else {
				RegionMatchesForm.MUSTSendChk[validate_index].checked = true;
			}
		} else if(eval(len) == 3) {
			re=/\d\x2e\d/
			x = RegionMatchesForm.EuroDrawOdds[validate_index].value.search(re);
			if(x == -1) {
				RegionMatchesForm.EuroDrawOdds[validate_index].value = '';
				alert('不正確賠率數字');
			}	else {
				RegionMatchesForm.MUSTSendChk[validate_index].checked = true;
			}
		} else if(eval(len) > 3) {
			re=/\d\x2e\d{2,2}/
			x = RegionMatchesForm.EuroDrawOdds[validate_index].value.search(re);
			if(x == -1) {
				re2=/\d{2,2}\x2e\d/
				y = RegionMatchesForm.EuroDrawOdds[validate_index].value.search(re2);
				if(y == -1) {
					RegionMatchesForm.EuroDrawOdds[validate_index].value = '';
					alert('不正確賠率數字');
				} else {
					RegionMatchesForm.MUSTSendChk[validate_index].checked = true;
				}
			}	else {
				RegionMatchesForm.MUSTSendChk[validate_index].checked = true;
			}
		}
	} else {
		RegionMatchesForm.MUSTSendChk[validate_index].checked = true;
	}
}

function onStatusChanged(validate_index) {
	RegionMatchesForm.MUSTSendChk[validate_index].checked = true;
}

function onChecked(validate_index) {
	RegionMatchesForm.MUSTSendChk[validate_index].checked = true;
}

function selectAll(iType) {
	<%
		int iSendChkIndex;
		string sSendChk_All;
	%>
	if(iType == 0) {
		if(RegionMatchesForm.SelectAllSend.checked == true) {
			<%
				for(iSendChkIndex=0;iSendChkIndex<iRecCount;iSendChkIndex++) {
					sSendChk_All = "MUSTSendChk[" + iSendChkIndex.ToString() + "]";
			%>
					RegionMatchesForm.<%=sSendChk_All%>.checked = true;
			<%
				}
			%>
		}	else {
			<%
				for(iSendChkIndex=0;iSendChkIndex<iRecCount;iSendChkIndex++) {
					sSendChk_All = "MUSTSendChk[" + iSendChkIndex.ToString() + "]";
			%>
					RegionMatchesForm.<%=sSendChk_All%>.checked = false;
			<%
				}
			%>
		}
	} else if(iType == 1) {
		if(RegionMatchesForm.SelectAllHide.checked == true) {
			<%
				for(iSendChkIndex=0;iSendChkIndex<iRecCount;iSendChkIndex++) {
					sSendChk_All = "hiddenChk[" + iSendChkIndex.ToString() + "]";
			%>
					RegionMatchesForm.<%=sSendChk_All%>.checked = true;
			<%
				}
			%>
		}	else {
			<%
				for(iSendChkIndex=0;iSendChkIndex<iRecCount;iSendChkIndex++) {
					sSendChk_All = "hiddenChk[" + iSendChkIndex.ToString() + "]";
			%>
					RegionMatchesForm.<%=sSendChk_All%>.checked = false;
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
	RegionMatchesForm.orderID.value = '';
}

function onOrder_IDChanged(validate_index) {
	order_re = /^\d{0,3}$/
	re_val = RegionMatchesForm.orderID.value.search(order_re)
	if(re_val == -1) {
		alert('序號只接受數字');
		RegionMatchesForm.orderID.value = '';
	}
}

function Handicap1Validity(validate_index) {
	if(RegionMatchesForm.Handicap1.value != '') {
		var len = RegionMatchesForm.Handicap1.value.length;
		if(eval(len) == 1) {
			re=/\d/
			x = RegionMatchesForm.Handicap1.value.search(re);
			if(x == -1) {
				RegionMatchesForm.Handicap1.value = '';
				alert('不正確讓球數字');
			}	else {
				RegionMatchesForm.MUSTSendChk.checked = true;
			}
		}	else if(eval(len) > 1) {
			re=/\d\x2e\d/
			x = RegionMatchesForm.Handicap1.value.search(re);
			if(x == -1) {
				RegionMatchesForm.Handicap1.value = '';
				alert('不正確讓球數字');
			}	else {
				RegionMatchesForm.MUSTSendChk.checked = true;
			}
		}
	} else {
		RegionMatchesForm.MUSTSendChk.checked = true;
	}
}

function Handicap2Validity(validate_index) {
	if(RegionMatchesForm.Handicap2.value != '') {
		var len = RegionMatchesForm.Handicap2.value.length;
		if(eval(len) == 1) {
			re=/\d/
			x = RegionMatchesForm.Handicap2.value.search(re);
			if(x == -1) {
				RegionMatchesForm.Handicap2.value = '';
				alert('不正確讓球數字');
			}	else {
				RegionMatchesForm.MUSTSendChk.checked = true;
			}
		}	else if(eval(len) > 1) {
			re=/\d\x2e\d/
			x = RegionMatchesForm.Handicap2.value.search(re);
			if(x == -1) {
				RegionMatchesForm.Handicap2.value = '';
				alert('不正確讓球數字');
			}	else {
				RegionMatchesForm.MUSTSendChk.checked = true;
			}
		}
	} else {
		RegionMatchesForm.MUSTSendChk.checked = true;
	}
}

function UpOddsValidity(validate_index) {
	if(RegionMatchesForm.UpOdds.value != '') {
		var len = RegionMatchesForm.UpOdds.value.length;
		if(eval(len) == 1) {
			re=/\d/
			x = RegionMatchesForm.UpOdds.value.search(re);
			if(x == -1) {
				RegionMatchesForm.UpOdds.value = '';
				alert('不正確賠率數字');
			}	else {
				<%
					if(!sTotalOdds.Equals("")) {
				%>
					RegionMatchesForm.DownOdds.value = <%=sTotalOdds%> - RegionMatchesForm.UpOdds.value;
					RegionMatchesForm.DownOdds.value = Math.round(RegionMatchesForm.DownOdds.value*1000)/1000;
				<%
					}
				%>
				RegionMatchesForm.MUSTSendChk.checked = true;
			}
		}	else if(eval(len) > 1) {
			re=/\d\x2e\d{1,3}/
			x = RegionMatchesForm.UpOdds.value.search(re);
			if(x == -1) {
				RegionMatchesForm.UpOdds.value = '';
				alert('不正確賠率數字');
			}	else {
				<%
					if(!sTotalOdds.Equals("")) {
				%>
					RegionMatchesForm.DownOdds.value = <%=sTotalOdds%> - RegionMatchesForm.UpOdds.value;
					RegionMatchesForm.DownOdds.value = Math.round(RegionMatchesForm.DownOdds.value*1000)/1000;
				<%
					}
				%>
				RegionMatchesForm.MUSTSendChk.checked = true;
			}
		}
	}	else {
		RegionMatchesForm.MUSTSendChk.checked = true;
	}
}

function DownOddsValidity(validate_index) {
	if(RegionMatchesForm.DownOdds.value != '') {
		var len = RegionMatchesForm.DownOdds.value.length;
		if(eval(len) == 1) {
			re=/\d/
			x = RegionMatchesForm.DownOdds.value.search(re);
			if(x == -1) {
				RegionMatchesForm.DownOdds.value = '';
				alert('不正確賠率數字');
			}	else {
				RegionMatchesForm.MUSTSendChk.checked = true;
			}
		}	else if(eval(len) > 1) {
			re=/\d\x2e\d{1,3}/
			x = RegionMatchesForm.DownOdds.value.search(re);
			if(x == -1) {
				RegionMatchesForm.DownOdds.value = '';
				alert('不正確賠率數字');
			}	else {
				RegionMatchesForm.MUSTSendChk.checked = true;
			}
		}
	} else {
		RegionMatchesForm.MUSTSendChk.checked = true;
	}
}

function EuroWinOddsValidity(validate_index) {
	if(RegionMatchesForm.EuroWinOdds.value != '') {
		var len = RegionMatchesForm.EuroWinOdds.value.length;
		if(eval(len) == 1) {
			re=/\d/
			x = RegionMatchesForm.EuroWinOdds.value.search(re);
			if(x == -1) {
				RegionMatchesForm.EuroWinOdds.value = '';
				alert('不正確賠率數字');
			}	else {
				RegionMatchesForm.MUSTSendChk.checked = true;
			}
		}	else if(eval(len) == 2) {
			re=/\d{2,2}/
			x = RegionMatchesForm.EuroWinOdds.value.search(re);
			if(x == -1) {
				RegionMatchesForm.EuroWinOdds.value = '';
				alert('不正確賠率數字');
			}	else {
				RegionMatchesForm.MUSTSendChk.checked = true;
			}
		} else if(eval(len) == 3) {
			re=/\d\x2e\d/
			x = RegionMatchesForm.EuroWinOdds.value.search(re);
			if(x == -1) {
				RegionMatchesForm.EuroWinOdds.value = '';
				alert('不正確賠率數字');
			}	else {
				RegionMatchesForm.MUSTSendChk.checked = true;
			}
		} else if(eval(len) > 3) {
			re=/\d\x2e\d{2,2}/
			x = RegionMatchesForm.EuroWinOdds.value.search(re);
			if(x == -1) {
				re2=/\d{2,2}\x2e\d/
				y = RegionMatchesForm.EuroWinOdds.value.search(re2);
				if(y == -1) {
					RegionMatchesForm.EuroWinOdds.value = '';
					alert('不正確賠率數字');
				} else {
					RegionMatchesForm.MUSTSendChk.checked = true;
				}
			}	else {
				RegionMatchesForm.MUSTSendChk.checked = true;
			}
		}
	} else {
		RegionMatchesForm.MUSTSendChk.checked = true;
	}
}

function EuroLossOddsValidity(validate_index) {
	if(RegionMatchesForm.EuroLossOdds.value != '') {
		var len = RegionMatchesForm.EuroLossOdds.value.length;
		if(eval(len) == 1) {
			re=/\d/
			x = RegionMatchesForm.EuroLossOdds.value.search(re);
			if(x == -1) {
				RegionMatchesForm.EuroLossOdds.value = '';
				alert('不正確賠率數字');
			}	else {
				RegionMatchesForm.MUSTSendChk.checked = true;
			}
		}	else if(eval(len) == 2) {
			re=/\d{2,2}/
			x = RegionMatchesForm.EuroLossOdds.value.search(re);
			if(x == -1) {
				RegionMatchesForm.EuroLossOdds.value = '';
				alert('不正確賠率數字');
			}	else {
				RegionMatchesForm.MUSTSendChk.checked = true;
			}
		} else if(eval(len) == 3) {
			re=/\d\x2e\d/
			x = RegionMatchesForm.EuroLossOdds.value.search(re);
			if(x == -1) {
				RegionMatchesForm.EuroLossOdds.value = '';
				alert('不正確賠率數字');
			}	else {
				RegionMatchesForm.MUSTSendChk.checked = true;
			}
		} else if(eval(len) > 3) {
			re=/\d\x2e\d{2,2}/
			x = RegionMatchesForm.EuroLossOdds.value.search(re);
			if(x == -1) {
				re2=/\d{2,2}\x2e\d/
				y = RegionMatchesForm.EuroLossOdds.value.search(re2);
				if(y == -1) {
					RegionMatchesForm.EuroLossOdds.value = '';
					alert('不正確賠率數字');
				} else {
					RegionMatchesForm.MUSTSendChk.checked = true;
				}
			}	else {
				RegionMatchesForm.MUSTSendChk.checked = true;
			}
		}
	}	else {
		RegionMatchesForm.MUSTSendChk.checked = true;
	}
}

function EuroDrawOddsValidity(validate_index) {
	if(RegionMatchesForm.EuroDrawOdds.value != '') {
		var len = RegionMatchesForm.EuroDrawOdds.value.length;
		if(eval(len) == 1) {
			re=/\d/
			x = RegionMatchesForm.EuroDrawOdds.value.search(re);
			if(x == -1) {
				RegionMatchesForm.EuroDrawOdds.value = '';
				alert('不正確賠率數字');
			}	else {
				RegionMatchesForm.MUSTSendChk.checked = true;
			}
		}	else if(eval(len) == 2) {
			re=/\d{2,2}/
			x = RegionMatchesForm.EuroDrawOdds.value.search(re);
			if(x == -1) {
				RegionMatchesForm.EuroDrawOdds.value = '';
				alert('不正確賠率數字');
			}	else {
				RegionMatchesForm.MUSTSendChk.checked = true;
			}
		} else if(eval(len) == 3) {
			re=/\d\x2e\d/
			x = RegionMatchesForm.EuroDrawOdds.value.search(re);
			if(x == -1) {
				RegionMatchesForm.EuroDrawOdds.value = '';
				alert('不正確賠率數字');
			}	else {
				RegionMatchesForm.MUSTSendChk.checked = true;
			}
		} else if(eval(len) > 3) {
			re=/\d\x2e\d{2,2}/
			x = RegionMatchesForm.EuroDrawOdds.value.search(re);
			if(x == -1) {
				re2=/\d{2,2}\x2e\d/
				y = RegionMatchesForm.EuroDrawOdds.value.search(re2);
				if(y == -1) {
					RegionMatchesForm.EuroDrawOdds.value = '';
					alert('不正確賠率數字');
				} else {
					RegionMatchesForm.MUSTSendChk.checked = true;
				}
			}	else {
				RegionMatchesForm.MUSTSendChk.checked = true;
			}
		}
	}	else {
		RegionMatchesForm.MUSTSendChk.checked = true;
	}
}

function onStatusChanged(validate_index) {
	RegionMatchesForm.MUSTSendChk.checked = true;
}

function onChecked(validate_index) {
	RegionMatchesForm.MUSTSendChk.checked = true;
}

function selectAll(iType) {
	if(iType == 0) {
		if(RegionMatchesForm.SelectAllSend.checked == true) {
			RegionMatchesForm.MUSTSendChk.checked = true;
		}	else {
			RegionMatchesForm.MUSTSendChk.checked = false;
		}
	}	else if(iType == 1) {
		if(RegionMatchesForm.SelectAllHide.checked == true) {
			RegionMatchesForm.hiddenChk.checked = true;
		}	else {
			RegionMatchesForm.hiddenChk.checked = false;
		}
	}
}
<%
	}
%>

function CheckTotalOdds() {
	if(RegionMatchesForm.totalOdds.value != '') {
		var len = RegionMatchesForm.totalOdds.value.length;
		if(eval(len) == 1) {
			re=/\d/
			x = RegionMatchesForm.totalOdds.value.search(re);
			if(x == -1) {
				RegionMatchesForm.totalOdds.value = '';
				alert('不正確賠率總數');
			}
		} else if(eval(len) > 1) {
			re=/\d\x2e\d{1,3}/
			x = RegionMatchesForm.totalOdds.value.search(re);
			if(x == -1) {
				RegionMatchesForm.totalOdds.value = '';
				alert('不正確賠率總數');
			}
		}
	}
}

function SelectLeagueToSend(leagGp) {
	<%
		if(leagueList != null && leagueList.Count > 0) {
			string sLeagChkItem = "";
			if(leagueList.Count > 1) {
				for(int i = 0; i < leagueList.Count; i++) {
					sLeagChkItem = "LeagueChk_" + i.ToString();
	%>
				if(leagGp == '<%=i.ToString()%>') {
					if(RegionMatchesForm.<%=sLeagChkItem%>.length > 1) {
						for(j = 0; j < RegionMatchesForm.<%=sLeagChkItem%>.length; j++) {
							if(RegionMatchesForm.leagGpChk[leagGp].checked == true) {
								RegionMatchesForm.MUSTSendChk[RegionMatchesForm.<%=sLeagChkItem%>[j].value].checked = true;
							} else {
								RegionMatchesForm.MUSTSendChk[RegionMatchesForm.<%=sLeagChkItem%>[j].value].checked = false;
							}
						}
					} else {
						if(RegionMatchesForm.leagGpChk[leagGp].checked == true) {
							<%
								if(iRecCount > 1) {
							%>
								RegionMatchesForm.MUSTSendChk[RegionMatchesForm.<%=sLeagChkItem%>.value].checked = true;
							<%
								} else {
							%>
								RegionMatchesForm.MUSTSendChk.checked = true;
							<%
								}
							%>
						} else {
							<%
								if(iRecCount > 1) {
							%>
								RegionMatchesForm.MUSTSendChk[RegionMatchesForm.<%=sLeagChkItem%>.value].checked = false;
							<%
								} else {
							%>
								RegionMatchesForm.MUSTSendChk.checked = false;
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
				if(RegionMatchesForm.LeagueChk_0.length > 1) {
					for(j = 0; j < RegionMatchesForm.LeagueChk_0.length; j++) {
						if(RegionMatchesForm.leagGpChk.checked == true) {
							RegionMatchesForm.MUSTSendChk[RegionMatchesForm.LeagueChk_0[j].value].checked = true;
						} else {
							RegionMatchesForm.MUSTSendChk[RegionMatchesForm.LeagueChk_0[j].value].checked = false;
						}
					}
				} else {
					if(RegionMatchesForm.leagGpChk.checked == true) {
						RegionMatchesForm.MUSTSendChk.checked = true;
					} else {
						RegionMatchesForm.MUSTSendChk.checked = false;
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
					if(RegionMatchesForm.<%=sLeagChkItem%>.length > 1) {
						for(j = 0; j < RegionMatchesForm.<%=sLeagChkItem%>.length; j++) {
							if(RegionMatchesForm.leagGpHide[leagGp].checked == true) {
								RegionMatchesForm.hiddenChk[RegionMatchesForm.<%=sLeagChkItem%>[j].value].checked = true;
							} else {
								RegionMatchesForm.hiddenChk[RegionMatchesForm.<%=sLeagChkItem%>[j].value].checked = false;
							}
						}
					} else {
						if(RegionMatchesForm.leagGpHide[leagGp].checked == true) {
							<%
								if(iRecCount > 1) {
							%>
								RegionMatchesForm.hiddenChk[RegionMatchesForm.<%=sLeagChkItem%>.value].checked = true;
							<%
								} else {
							%>
								RegionMatchesForm.hiddenChk.checked = true;
							<%
								}
							%>
						} else {
							<%
								if(iRecCount > 1) {
							%>
								RegionMatchesForm.hiddenChk[RegionMatchesForm.<%=sLeagChkItem%>.value].checked = false;
							<%
								} else {
							%>
								RegionMatchesForm.hiddenChk.checked = false;
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
				if(RegionMatchesForm.LeagueChk_0.length > 1) {
					for(j = 0; j < RegionMatchesForm.LeagueChk_0.length; j++) {
						if(RegionMatchesForm.leagGpHide.checked == true) {
							RegionMatchesForm.hiddenChk[RegionMatchesForm.LeagueChk_0[j].value].checked = true;
						} else {
							RegionMatchesForm.hiddenChk[RegionMatchesForm.LeagueChk_0[j].value].checked = false;
						}
					}
				} else {
					if(RegionMatchesForm.leagGpHide.checked == true) {
						RegionMatchesForm.hiddenChk.checked = true;
					} else {
						RegionMatchesForm.hiddenChk.checked = false;
					}
				}
	<%
			}
		}
	%>
}

function ReSort(sortType) {
	parent.content_frame.location.replace('RegionModify.aspx?RegionID=<%=sRegionID%>&sort=' +sortType);
}

function ShowHiddenMatches() {
	matchWinFeature="Resizable=Yes,ScrollBars=Yes,MenuBar=No,Directories=No,ToolBar=No,Location=No,Status=No, width=400, height=680";
	matchWindow = window.open('../soccer/HiddenMatch.aspx?type=otherodds&regionID=<%=sRegionID%>', 'HiddenMatch' , matchWinFeature);
	matchWindow.focus();
}
</script>

<html>
<head>
	<META http-equiv="Content-Type" content="text/html; charset=big5">
	<LINK REL="stylesheet" HREF="/sportStyle.css" TYPE="text/css">
</head>
<body>
	<form id="RegionMatchesForm" method="post" runat="server" onsubmit="DeviceCheck()">
		<font size="2"><b>上次行動:</b><asp:Label id="historyMsg" runat="server" /></font><br>

		<table border="1" width="100%" style="font: 10pt verdana">
		<tr style="background-color:#FFC39C; color:#191970">
			<th colspan="11" align="left">
				<%
					if(leagueList != null && leagueList.Count > 0) {
				%>
					選擇隱藏:&nbsp;
				<%
						if(leagueList.Count > 1) {
							for(int i = 0; i < leagueList.Count; i++) {
				%>
							<input type="checkbox" name="leagGpHide" value="<%=i.ToString()%>" onClick="SelectLeagueToHide(RegionMatchesForm.leagGpHide[<%=i.ToString()%>].value)"><%=leagueList[i]%>&nbsp;&nbsp;
				<%
							}
						} else {
				%>
							<input type="checkbox" name="leagGpHide" value="0" onClick="SelectLeagueToHide(RegionMatchesForm.leagGpHide.value)"><%=leagueList[0]%>&nbsp;&nbsp;
				<%
						}
					}
				%>
			</th>
		</tr>

		<tr style="background-color:#FFFAF0; color:#CD5C5C">
			<th colspan="11" align="left">
				<%
					if(leagueList != null && leagueList.Count > 0) {
				%>
					選擇傳送:&nbsp;
				<%
						if(leagueList.Count > 1) {
							for(int i = 0; i < leagueList.Count; i++) {
				%>
							<input type="checkbox" name="leagGpChk" value="<%=i.ToString()%>" onClick="SelectLeagueToSend(RegionMatchesForm.leagGpChk[<%=i.ToString()%>].value)"><%=leagueList[i]%>&nbsp;&nbsp;
				<%
							}
						} else {
				%>
							<input type="checkbox" name="leagGpChk" value="0" onClick="SelectLeagueToSend(RegionMatchesForm.leagGpChk.value)"><%=leagueList[0]%>&nbsp;&nbsp;
				<%
						}
					}
				%>
			</th>
		</tr>

		<tr style="background-color:#DDA0DD">
			<th colspan="6" align="left">
				<input type="button" name="clearOrder" value="清除序號" onClick="ClearOrderNo()">&nbsp;&nbsp;
				順序依
				<select name="sortType" onChange="ReSort(RegionMatchesForm.sortType.value)">
				<%
					if(Session["user_sortType"].ToString().Equals("0")) {
				%>
					<option value="0">聯賽</option>
					<option value="1">序號</option>
				<%
					} else {
				%>
					<option value="1">序號</option>
					<option value="0">聯賽</option>
				<%
					}
				%>
				</select>
			</th>
			<th colspan="2">亞洲盤</th>
			<th>歐洲盤</th>
			<th style="background-color:#FFFAF0; color:#CD5C5C">全選<input type="checkbox" name="SelectAllSend" onClick="selectAll(0)"></th>
			<th style="background-color:#FFC39C; color:#191970">全選<input type="checkbox" name="SelectAllHide" onClick="selectAll(1)"></th>
		</tr>

		<tr style="background-color:#DDA0DD">
			<th>排序</th>
			<th>聯賽</th>
			<th>主隊</th>
			<th>客隊</th>
			<th>中立</th>
			<th>主讓</th>
			<th>讓球</th>
			<th>賠率</th>
			<th>歐盤(勝/負 和)</th>
			<th style="background-color:#FFFAF0; color:#CD5C5C">強制傳送</th>
			<th style="background-color:#FFC39C; color:#191970">隱藏</th>
		</tr>

		<span id="MatchesInformation" runat="server" />

		<tr>
			<td colspan="10" style="background-color:#FFFAF0" align="right">
				<!--
					Value of SendToPager is Device ID defined in DEVICE_TYPE
				-->
				<input type="checkbox" name="SendToPager" value="1" checked>GOGO1&nbsp;
				<input type="checkbox" name="SendToPager" value="2" checked>GOGO2&nbsp;
				<!--
				<input type="checkbox" name="SendToPager" value="5" checked>GOGO3Combo&nbsp;
				-->
				<asp:Button id="SendBtn" Text="傳送" CommandName="SEND" OnCommand="GetMatchesAction" runat="server" />
			</td>
		</tr>
		<tr>
			<td colspan="9"><asp:Button id="SortBtn" Text="儲存序號" CommandName="SORT" tabindex="500" OnCommand="GetMatchesAction" runat="server" /></td>
			<td style="background-color:#FFC39C" align="right"><asp:Button id="ShowBtn" Text="顯示所有" CommandName="SHOW" OnCommand="GetMatchesAction" runat="server" /></td>
			<td style="background-color:#FFC39C" align="center"><asp:Button id="HideBtn" Text="隱藏" CommandName="HIDE" OnCommand="GetMatchesAction" runat="server" /></td>
		</tr>

		<tr>
			<th align="right" colspan="11" style="background-color:#FFC39C; color:#191970">
				<a href="javascript:ShowHiddenMatches()">已隱藏的賽事</a>
			</th>
		</tr>

		<tr align="left">
			<th><%=sRegion%>賠率總數:</th>
			<td colspan="10">
				<input type="text" name="totalOdds" value="<%=sTotalOdds%>" size="5" maxlength="5" onChange="CheckTotalOdds()">&nbsp;
				<input type="submit" id="SaveBtn" value="儲存" OnServerClick="SaveTotalOdds" runat="server">
			</td>
		</tr>
		</table>
	</form>
</body>
</html>