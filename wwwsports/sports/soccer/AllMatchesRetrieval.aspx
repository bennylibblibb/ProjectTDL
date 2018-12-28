<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>
<%@ Import Namespace="System.Collections"%>

<script language="C#" runat="server">
	int iRecCount;
	ArrayList leagueList;

	void Page_Load(Object sender,EventArgs e) {
		AllMatches matchInfo = new AllMatches((string)Application["SoccerDBConnectionString"]);

		try {
			MatchesInformation.InnerHtml = matchInfo.GetAllMatches();
			iRecCount = matchInfo.NumberOfRecords;
			leagueList = matchInfo.LeagueList;
			UpdateHistoryMessage((string)Application["retrieveInfoMsg"] + "所有賽事(" + DateTime.Now.ToString("HH:mm:ss") + ")");
		} catch(NullReferenceException) {
			FormsAuthentication.SignOut();
			UpdateHistoryMessage("<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>");
		}
	}

	void GetMatchesAction(Object sender,CommandEventArgs e) {
		int iUpdated = 0;
		string sNow;
		string sCmd;
		AllMatches matchUpd = new AllMatches((string)Application["SoccerDBConnectionString"]);

		sCmd = (string)e.CommandName;
		sNow = DateTime.Now.ToString("HH:mm:ss");
		try {
			iUpdated = matchUpd.UpdateMatches(sCmd);
			Page_Load(sender,e);
			if(sCmd.Equals("SEND")) {
				if(iUpdated > 0) {
					UpdateHistoryMessage("成功更新" + iUpdated.ToString() + "場賽事資訊(" + sNow + ")");
				}	else if(iUpdated == 0) {
					UpdateHistoryMessage("沒有更新賽事(" + sNow + ")");
				}	else {
					UpdateHistoryMessage((string)Application["transErrorMsg"] + "(" + sNow + ")");
				}
			}	else if(sCmd.Equals("SORT")) {
				if(iUpdated > 0) {
					UpdateHistoryMessage("成功更新賽事序號(" + sNow + ")");
				}	else if(iUpdated == 0) {
					UpdateHistoryMessage("沒有序號更新(" + sNow + ")");
				}	else {
					UpdateHistoryMessage((string)Application["transErrorMsg"] + "(" + sNow + ")");
				}
			}	else if(sCmd.Equals("HIDE")) {
				if(iUpdated > 0) {
					UpdateHistoryMessage("隱藏" + iUpdated.ToString() + "場賽事(" + sNow + ")");
				}	else if(iUpdated == 0) {
					UpdateHistoryMessage("沒有賽事隱藏(" + sNow + ")");
				}	else {
					UpdateHistoryMessage((string)Application["transErrorMsg"] + "(" + sNow + ")");
				}
			}	else if(sCmd.Equals("SHOW")) {
				if(iUpdated > 0) {
					UpdateHistoryMessage("顯示所有賽事(" + sNow + ")");
				} else if(iUpdated == 0) {
					UpdateHistoryMessage("沒有賽事顯示(" + sNow + ")");
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
<%
	if(iRecCount > 1) {
%>
function ClearOrderNo() {
	<%
		for(int iClear=0;iClear<iRecCount;iClear++) {
	%>
		AllMatchesForm.orderID[<%=iClear.ToString()%>].value = '';
	<%
		}
	%>
}

function onOrder_IDChanged(validate_index) {
	order_re = /^\d{0,3}$/
	re_val = AllMatchesForm.orderID[validate_index].value.search(order_re)
	if(re_val == -1) {
		alert('序號只接受數字');
		AllMatchesForm.orderID[validate_index].value = '';
	}
}

function UpHandicap1Validity(validate_index) {
	if(AllMatchesForm.UpHandicap1[validate_index].value != '') {
		var len = AllMatchesForm.UpHandicap1[validate_index].value.length;
		if(eval(len) == 1) {
			re=/\d/
			x = AllMatchesForm.UpHandicap1[validate_index].value.search(re);
			if(x == -1) {
				AllMatchesForm.UpHandicap1[validate_index].value = '';
				alert('不正確讓球數字');
			} else {
				AllMatchesForm.MUSTSendChk[validate_index].checked = true;
			}
		} else if(eval(len) > 1) {
			re=/\d\x2e\d/
			x = AllMatchesForm.UpHandicap1[validate_index].value.search(re);
			if(x == -1) {
				AllMatchesForm.UpHandicap1[validate_index].value = '';
				alert('不正確讓球數字');
			} else {
				AllMatchesForm.MUSTSendChk[validate_index].checked = true;
			}
		}
	} else {
		AllMatchesForm.MUSTSendChk[validate_index].checked = true;
	}
}

function UpHandicap2Validity(validate_index) {
	if(AllMatchesForm.UpHandicap2[validate_index].value != '') {
		var len = AllMatchesForm.UpHandicap2[validate_index].value.length;
		if(eval(len) == 1) {
			re=/\d/
			x = AllMatchesForm.UpHandicap2[validate_index].value.search(re);
			if(x == -1) {
				AllMatchesForm.UpHandicap2[validate_index].value = '';
				alert('不正確讓球數字');
			} else {
				AllMatchesForm.MUSTSendChk[validate_index].checked = true;
			}
		} else if(eval(len) > 1) {
			re=/\d\x2e\d/
			x = AllMatchesForm.UpHandicap2[validate_index].value.search(re);
			if(x == -1) {
				AllMatchesForm.UpHandicap2[validate_index].value = '';
				alert('不正確讓球數字');
			} else {
				AllMatchesForm.MUSTSendChk[validate_index].checked = true;
			}
		}
	} else {
		AllMatchesForm.MUSTSendChk[validate_index].checked = true;
	}
}

function UpOddsValidity(validate_index) {
	if(AllMatchesForm.UpOdds[validate_index].value != '') {
		var len = AllMatchesForm.UpOdds[validate_index].value.length;
		if(eval(len) == 1) {
			re=/\d/
			x = AllMatchesForm.UpOdds[validate_index].value.search(re);
			if(x == -1) {
				AllMatchesForm.UpOdds[validate_index].value = '';
				alert('不正確賠率數字');
			} else {
				AllMatchesForm.MUSTSendChk[validate_index].checked = true;
			}
		} else if(eval(len) == 2) {
			re=/\d{2,2}/
			x = AllMatchesForm.UpOdds[validate_index].value.search(re);
			if(x == -1) {
				AllMatchesForm.UpOdds[validate_index].value = '';
				alert('不正確賠率數字');
			} else {
				AllMatchesForm.MUSTSendChk[validate_index].checked = true;
			}
		}	else if(eval(len) == 3) {
			re=/\d\x2e\d/
			x = AllMatchesForm.UpOdds[validate_index].value.search(re);
			if(x == -1) {
				AllMatchesForm.UpOdds[validate_index].value = '';
				alert('不正確賠率數字');
			} else {
				AllMatchesForm.MUSTSendChk[validate_index].checked = true;
			}
		}	else if(eval(len) == 4) {
			re=/\d\x2e\d{2,2}/
			x = AllMatchesForm.UpOdds[validate_index].value.search(re);
			if(x == -1) {
				re2=/\d{2,2}\x2e\d/
				y = AllMatchesForm.UpOdds[validate_index].value.search(re2);
				if(y == -1) {
					AllMatchesForm.UpOdds[validate_index].value = '';
					alert('不正確賠率數字');
				} else {
					AllMatchesForm.MUSTSendChk[validate_index].checked = true;
				}
			} else {
				AllMatchesForm.MUSTSendChk[validate_index].checked = true;
			}
		}	else if(eval(len) > 4) {
			re=/\d\x2e\d{3,3}/
			x = AllMatchesForm.UpOdds[validate_index].value.search(re);
			if(x == -1) {
				re2=/\d{2,2}\x2e\d{2,2}/
				y = AllMatchesForm.UpOdds[validate_index].value.search(re2);
				if(y == -1) {
					re3=/\d{3,3}\x2e\d/
					z = AllMatchesForm.UpOdds[validate_index].value.search(re3);
					if(z == -1) {
						AllMatchesForm.UpOdds[validate_index].value = '';
						alert('不正確賠率數字');
					} else {
						AllMatchesForm.MUSTSendChk[validate_index].checked = true;
					}
				} else {
					AllMatchesForm.MUSTSendChk[validate_index].checked = true;
				}
			} else {
				AllMatchesForm.MUSTSendChk[validate_index].checked = true;
			}
		}
	} else {
		AllMatchesForm.MUSTSendChk[validate_index].checked = true;
	}
}

function BSTTSValidity(validate_index) {
	if(AllMatchesForm.BSTTS[validate_index].value != '') {
		var len = AllMatchesForm.BSTTS[validate_index].value.length;
		if(eval(len) == 1) {
			re=/\d/
			x = AllMatchesForm.BSTTS[validate_index].value.search(re);
			if(x == -1) {
				AllMatchesForm.BSTTS[validate_index].value = '';
				alert('不正確總入球格式');
			} else {
				AllMatchesForm.MUSTSendChk[validate_index].checked = true;
			}
		} else if(eval(len) == 2) {
			re=/\d{2,2}/
			x = AllMatchesForm.BSTTS[validate_index].value.search(re);
			if(x == -1) {
				AllMatchesForm.BSTTS[validate_index].value = '';
				alert('不正確總入球格式');
			} else {
				AllMatchesForm.MUSTSendChk[validate_index].checked = true;
			}
		} else if(eval(len) == 3) {
			re=/\d\x2e\d/
			x = AllMatchesForm.BSTTS[validate_index].value.search(re);
			if(x == -1) {
				AllMatchesForm.BSTTS[validate_index].value = '';
				alert('不正確總入球格式');
			} else {
				AllMatchesForm.MUSTSendChk[validate_index].checked = true;
			}
		} else if(eval(len) > 3) {
			re=/\d\x2e\d\x2f\d/
			x = AllMatchesForm.BSTTS[validate_index].value.search(re);
			if(x == -1) {
				re=/\d\x2f\d\x2e\d/
				y = AllMatchesForm.BSTTS[validate_index].value.search(re);
				if(y == -1) {
					AllMatchesForm.BSTTS[validate_index].value = '';
					alert('不正確總入球格式');
				} else {
					AllMatchesForm.MUSTSendChk[validate_index].checked = true;
				}
			} else {
				AllMatchesForm.MUSTSendChk[validate_index].checked = true;
			}
		}
	} else {
		AllMatchesForm.MUSTSendChk[validate_index].checked = true;
	}
}

function DownOddsValidity(validate_index) {
	if(AllMatchesForm.DownOdds[validate_index].value != '') {
		var len = AllMatchesForm.DownOdds[validate_index].value.length;
		if(eval(len) == 1) {
			re=/\d/
			x = AllMatchesForm.DownOdds[validate_index].value.search(re);
			if(x == -1) {
				AllMatchesForm.DownOdds[validate_index].value = '';
				alert('不正確賠率數字');
			} else {
				AllMatchesForm.MUSTSendChk[validate_index].checked = true;
			}
		} else if(eval(len) == 2) {
			re=/\d{2,2}/
			x = AllMatchesForm.DownOdds[validate_index].value.search(re);
			if(x == -1) {
				AllMatchesForm.DownOdds[validate_index].value = '';
				alert('不正確賠率數字');
			} else {
				AllMatchesForm.MUSTSendChk[validate_index].checked = true;
			}
		}	else if(eval(len) == 3) {
			re=/\d\x2e\d/
			x = AllMatchesForm.DownOdds[validate_index].value.search(re);
			if(x == -1) {
				AllMatchesForm.DownOdds[validate_index].value = '';
				alert('不正確賠率數字');
			} else {
				AllMatchesForm.MUSTSendChk[validate_index].checked = true;
			}
		}	else if(eval(len) == 4) {
			re=/\d\x2e\d{2,2}/
			x = AllMatchesForm.DownOdds[validate_index].value.search(re);
			if(x == -1) {
				re2=/\d{2,2}\x2e\d/
				y = AllMatchesForm.DownOdds[validate_index].value.search(re2);
				if(y == -1) {
					AllMatchesForm.DownOdds[validate_index].value = '';
					alert('不正確賠率數字');
				} else {
					AllMatchesForm.MUSTSendChk[validate_index].checked = true;
				}
			} else {
				AllMatchesForm.MUSTSendChk[validate_index].checked = true;
			}
		}	else if(eval(len) > 4) {
			re=/\d\x2e\d{3,3}/
			x = AllMatchesForm.DownOdds[validate_index].value.search(re);
			if(x == -1) {
				re2=/\d{2,2}\x2e\d{2,2}/
				y = AllMatchesForm.DownOdds[validate_index].value.search(re2);
				if(y == -1) {
					re3=/\d{3,3}\x2e\d/
					z = AllMatchesForm.DownOdds[validate_index].value.search(re3);
					if(z == -1) {
						AllMatchesForm.DownOdds[validate_index].value = '';
						alert('不正確賠率數字');
					} else {
						AllMatchesForm.MUSTSendChk[validate_index].checked = true;
					}
				} else {
					AllMatchesForm.MUSTSendChk[validate_index].checked = true;
				}
			} else {
				AllMatchesForm.MUSTSendChk[validate_index].checked = true;
			}
		}
	} else {
		AllMatchesForm.MUSTSendChk[validate_index].checked = true;
	}
}

function EuroWinOddsValidity(validate_index) {
	if(AllMatchesForm.EuroWinOdds[validate_index].value != '') {
		var len = AllMatchesForm.EuroWinOdds[validate_index].value.length;
		if(eval(len) == 1) {
			re=/\d/
			x = AllMatchesForm.EuroWinOdds[validate_index].value.search(re);
			if(x == -1) {
				AllMatchesForm.EuroWinOdds[validate_index].value = '';
				alert('不正確賠率數字');
			} else {
				AllMatchesForm.MUSTSendChk[validate_index].checked = true;
			}
		} else if(eval(len) == 2) {
			re=/\d{2,2}/
			x = AllMatchesForm.EuroWinOdds[validate_index].value.search(re);
			if(x == -1) {
				AllMatchesForm.EuroWinOdds[validate_index].value = '';
				alert('不正確賠率數字');
			} else {
				AllMatchesForm.MUSTSendChk[validate_index].checked = true;
			}
		} else if(eval(len) == 3) {
			re=/\d\x2e\d/
			x = AllMatchesForm.EuroWinOdds[validate_index].value.search(re);
			if(x == -1) {
				AllMatchesForm.EuroWinOdds[validate_index].value = '';
				alert('不正確賠率數字');
			} else {
				AllMatchesForm.MUSTSendChk[validate_index].checked = true;
			}
		} else if(eval(len) > 3) {
			re=/\d\x2e\d{2,2}/
			x = AllMatchesForm.EuroWinOdds[validate_index].value.search(re);
			if(x == -1) {
				re2=/\d{2,2}\x2e\d/
				y = AllMatchesForm.EuroWinOdds[validate_index].value.search(re2);
				if(y == -1) {
					AllMatchesForm.EuroWinOdds[validate_index].value = '';
					alert('不正確賠率數字');
				} else {
					AllMatchesForm.MUSTSendChk[validate_index].checked = true;
				}
			} else {
				AllMatchesForm.MUSTSendChk[validate_index].checked = true;
			}
		}
	} else {
		AllMatchesForm.MUSTSendChk[validate_index].checked = true;
	}
}

function EuroLossOddsValidity(validate_index) {
	if(AllMatchesForm.EuroLossOdds[validate_index].value != '') {
		var len = AllMatchesForm.EuroLossOdds[validate_index].value.length;
		if(eval(len) == 1) {
			re=/\d/
			x = AllMatchesForm.EuroLossOdds[validate_index].value.search(re);
			if(x == -1) {
				AllMatchesForm.EuroLossOdds[validate_index].value = '';
				alert('不正確賠率數字');
			} else {
				AllMatchesForm.MUSTSendChk[validate_index].checked = true;
			}
		} else if(eval(len) == 2) {
			re=/\d{2,2}/
			x = AllMatchesForm.EuroLossOdds[validate_index].value.search(re);
			if(x == -1) {
				AllMatchesForm.EuroLossOdds[validate_index].value = '';
				alert('不正確賠率數字');
			} else {
				AllMatchesForm.MUSTSendChk[validate_index].checked = true;
			}
		} else if(eval(len) == 3) {
			re=/\d\x2e\d/
			x = AllMatchesForm.EuroLossOdds[validate_index].value.search(re);
			if(x == -1) {
				AllMatchesForm.EuroLossOdds[validate_index].value = '';
				alert('不正確賠率數字');
			} else {
				AllMatchesForm.MUSTSendChk[validate_index].checked = true;
			}
		} else if(eval(len) > 3) {
			re=/\d\x2e\d{2,2}/
			x = AllMatchesForm.EuroLossOdds[validate_index].value.search(re);
			if(x == -1) {
				re2=/\d{2,2}\x2e\d/
				y = AllMatchesForm.EuroLossOdds[validate_index].value.search(re2);
				if(y == -1) {
					AllMatchesForm.EuroLossOdds[validate_index].value = '';
					alert('不正確賠率數字');
				} else {
					AllMatchesForm.MUSTSendChk[validate_index].checked = true;
				}
			} else {
				AllMatchesForm.MUSTSendChk[validate_index].checked = true;
			}
		}
	} else {
		AllMatchesForm.MUSTSendChk[validate_index].checked = true;
	}
}

function EuroDrawOddsValidity(validate_index) {
	if(AllMatchesForm.EuroDrawOdds[validate_index].value != '') {
		var len = AllMatchesForm.EuroDrawOdds[validate_index].value.length;
		if(eval(len) == 1) {
			re=/\d/
			x = AllMatchesForm.EuroDrawOdds[validate_index].value.search(re);
			if(x == -1) {
				AllMatchesForm.EuroDrawOdds[validate_index].value = '';
				alert('不正確賠率數字');
			} else {
				AllMatchesForm.MUSTSendChk[validate_index].checked = true;
			}
		} else if(eval(len) == 2) {
			re=/\d{2,2}/
			x = AllMatchesForm.EuroDrawOdds[validate_index].value.search(re);
			if(x == -1) {
				AllMatchesForm.EuroDrawOdds[validate_index].value = '';
				alert('不正確賠率數字');
			} else {
				AllMatchesForm.MUSTSendChk[validate_index].checked = true;
			}
		} else if(eval(len) == 3) {
			re=/\d\x2e\d/
			x = AllMatchesForm.EuroDrawOdds[validate_index].value.search(re);
			if(x == -1) {
				AllMatchesForm.EuroDrawOdds[validate_index].value = '';
				alert('不正確賠率數字');
			} else {
				AllMatchesForm.MUSTSendChk[validate_index].checked = true;
			}
		} else if(eval(len) > 3) {
			re=/\d\x2e\d{2,2}/
			x = AllMatchesForm.EuroDrawOdds[validate_index].value.search(re);
			if(x == -1) {
				re2=/\d{2,2}\x2e\d/
				y = AllMatchesForm.EuroDrawOdds[validate_index].value.search(re2);
				if(y == -1) {
					AllMatchesForm.EuroDrawOdds[validate_index].value = '';
					alert('不正確賠率數字');
				} else {
					AllMatchesForm.MUSTSendChk[validate_index].checked = true;
				}
			} else {
				AllMatchesForm.MUSTSendChk[validate_index].checked = true;
			}
		}
	} else {
		AllMatchesForm.MUSTSendChk[validate_index].checked = true;
	}
}

function onStatusChanged(validate_index) {
	AllMatchesForm.MUSTSendChk[validate_index].checked = true;
}

function onChecked(validate_index) {
	AllMatchesForm.MUSTSendChk[validate_index].checked = true;
}

function selectAll(iType) {
	<%
		int iSendChkIndex;
		string sSendChk_All;
	%>
	if(iType == 0) {
		if(AllMatchesForm.SelectAllSend.checked == true) {
			<%
				for(iSendChkIndex=0;iSendChkIndex<iRecCount;iSendChkIndex++) {
					sSendChk_All = "MUSTSendChk[" + iSendChkIndex.ToString() + "]";
			%>
					AllMatchesForm.<%=sSendChk_All%>.checked = true;
			<%
				}
			%>
		} else {
			<%
				for(iSendChkIndex=0;iSendChkIndex<iRecCount;iSendChkIndex++) {
					sSendChk_All = "MUSTSendChk[" + iSendChkIndex.ToString() + "]";
			%>
					AllMatchesForm.<%=sSendChk_All%>.checked = false;
			<%
				}
			%>
		}
	} else if(iType == 1) {
		if(AllMatchesForm.SelectAllHide.checked == true) {
			<%
				for(iSendChkIndex=0;iSendChkIndex<iRecCount;iSendChkIndex++) {
					sSendChk_All = "hiddenChk[" + iSendChkIndex.ToString() + "]";
			%>
					AllMatchesForm.<%=sSendChk_All%>.checked = true;
			<%
				}
			%>
		} else {
			<%
				for(iSendChkIndex=0;iSendChkIndex<iRecCount;iSendChkIndex++) {
					sSendChk_All = "hiddenChk[" + iSendChkIndex.ToString() + "]";
			%>
					AllMatchesForm.<%=sSendChk_All%>.checked = false;
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
	AllMatchesForm.orderID.value = '';
}

function onOrder_IDChanged(validate_index) {
	order_re = /^\d{0,3}$/
	re_val = AllMatchesForm.orderID.value.search(order_re)
	if(re_val == -1) {
		alert('序號只接受數字');
		AllMatchesForm.orderID.value = '';
	}
}

function UpHandicap1Validity(validate_index) {
	if(AllMatchesForm.UpHandicap1.value != '') {
		var len = AllMatchesForm.UpHandicap1.value.length;
		if(eval(len) == 1) {
			re=/\d/
			x = AllMatchesForm.UpHandicap1.value.search(re);
			if(x == -1) {
				AllMatchesForm.UpHandicap1.value = '';
				alert('不正確讓球數字');
			} else {
				AllMatchesForm.MUSTSendChk.checked = true;
			}
		} else if(eval(len) > 1) {
			re=/\d\x2e\d/
			x = AllMatchesForm.UpHandicap1.value.search(re);
			if(x == -1) {
				AllMatchesForm.UpHandicap1.value = '';
				alert('不正確讓球數字');
			} else {
				AllMatchesForm.MUSTSendChk.checked = true;
			}
		}
	} else {
		AllMatchesForm.MUSTSendChk.checked = true;
	}
}

function UpHandicap2Validity(validate_index) {
	if(AllMatchesForm.UpHandicap2.value != '') {
		var len = AllMatchesForm.UpHandicap2.value.length;
		if(eval(len) == 1) {
			re=/\d/
			x = AllMatchesForm.UpHandicap2.value.search(re);
			if(x == -1) {
				AllMatchesForm.UpHandicap2.value = '';
				alert('不正確讓球數字');
			} else {
				AllMatchesForm.MUSTSendChk.checked = true;
			}
		} else if(eval(len) > 1) {
			re=/\d\x2e\d/
			x = AllMatchesForm.UpHandicap2.value.search(re);
			if(x == -1) {
				AllMatchesForm.UpHandicap2.value = '';
				alert('不正確讓球數字');
			} else {
				AllMatchesForm.MUSTSendChk.checked = true;
			}
		}
	} else {
		AllMatchesForm.MUSTSendChk.checked = true;
	}
}

function UpOddsValidity(validate_index) {
	if(AllMatchesForm.UpOdds.value != '') {
		var len = AllMatchesForm.UpOdds.value.length;
		if(eval(len) == 1) {
			re=/\d/
			x = AllMatchesForm.UpOdds.value.search(re);
			if(x == -1) {
				AllMatchesForm.UpOdds.value = '';
				alert('不正確賠率數字');
			} else {
				AllMatchesForm.MUSTSendChk.checked = true;
			}
		} else if(eval(len) == 2) {
			re=/\d{2,2}/
			x = AllMatchesForm.UpOdds.value.search(re);
			if(x == -1) {
				AllMatchesForm.UpOdds.value = '';
				alert('不正確賠率數字');
			} else {
				AllMatchesForm.MUSTSendChk.checked = true;
			}
		}	else if(eval(len) == 3) {
			re=/\d\x2e\d/
			x = AllMatchesForm.UpOdds.value.search(re);
			if(x == -1) {
				AllMatchesForm.UpOdds.value = '';
				alert('不正確賠率數字');
			} else {
				AllMatchesForm.MUSTSendChk.checked = true;
			}
		}	else if(eval(len) == 4) {
			re=/\d\x2e\d{2,2}/
			x = AllMatchesForm.UpOdds.value.search(re);
			if(x == -1) {
				re2=/\d{2,2}\x2e\d/
				y = AllMatchesForm.UpOdds.value.search(re2);
				if(y == -1) {
					AllMatchesForm.UpOdds.value = '';
					alert('不正確賠率數字');
				} else {
					AllMatchesForm.MUSTSendChk.checked = true;
				}
			} else {
				AllMatchesForm.MUSTSendChk.checked = true;
			}
		}	else if(eval(len) > 4) {
			re=/\d\x2e\d{3,3}/
			x = AllMatchesForm.UpOdds.value.search(re);
			if(x == -1) {
				re2=/\d{2,2}\x2e\d{2,2}/
				y = AllMatchesForm.UpOdds.value.search(re2);
				if(y == -1) {
					re3=/\d{3,3}\x2e\d/
					z = AllMatchesForm.UpOdds.value.search(re3);
					if(z == -1) {
						AllMatchesForm.UpOdds.value = '';
						alert('不正確賠率數字');
					} else {
						AllMatchesForm.MUSTSendChk.checked = true;
					}
				} else {
					AllMatchesForm.MUSTSendChk.checked = true;
				}
			} else {
				AllMatchesForm.MUSTSendChk.checked = true;
			}
		}
	} else {
		AllMatchesForm.MUSTSendChk.checked = true;
	}
}

function BSTTSValidity(validate_index) {
	if(AllMatchesForm.BSTTS.value != '') {
		var len = AllMatchesForm.BSTTS.value.length;
		if(eval(len) == 1) {
			re=/\d/
			x = AllMatchesForm.BSTTS.value.search(re);
			if(x == -1) {
				AllMatchesForm.BSTTS.value = '';
				alert('不正確總入球格式');
			} else {
				AllMatchesForm.MUSTSendChk.checked = true;
			}
		} else if(eval(len) == 2) {
			re=/\d{2,2}/
			x = AllMatchesForm.BSTTS.value.search(re);
			if(x == -1) {
				AllMatchesForm.BSTTS.value = '';
				alert('不正確總入球格式');
			} else {
				AllMatchesForm.MUSTSendChk.checked = true;
			}
		} else if(eval(len) == 3) {
			re=/\d\x2e\d/
			x = AllMatchesForm.BSTTS.value.search(re);
			if(x == -1) {
				AllMatchesForm.BSTTS.value = '';
				alert('不正確總入球格式');
			} else {
				AllMatchesForm.MUSTSendChk.checked = true;
			}
		} else if(eval(len) > 3) {
			re=/\d\x2e\d\x2f\d/
			x = AllMatchesForm.BSTTS.value.search(re);
			if(x == -1) {
				re=/\d\x2f\d\x2e\d/
				y = AllMatchesForm.BSTTS.value.search(re);
				if(y == -1) {
					AllMatchesForm.BSTTS.value = '';
					alert('不正確總入球格式');
				} else {
					AllMatchesForm.MUSTSendChk.checked = true;
				}
			} else {
				AllMatchesForm.MUSTSendChk.checked = true;
			}
		}
	} else {
		AllMatchesForm.MUSTSendChk.checked = true;
	}
}

function DownOddsValidity(validate_index) {
	if(AllMatchesForm.DownOdds.value != '') {
		var len = AllMatchesForm.DownOdds.value.length;
		if(eval(len) == 1) {
			re=/\d/
			x = AllMatchesForm.DownOdds.value.search(re);
			if(x == -1) {
				AllMatchesForm.DownOdds.value = '';
				alert('不正確賠率數字');
			} else {
				AllMatchesForm.MUSTSendChk.checked = true;
			}
		} else if(eval(len) == 2) {
			re=/\d{2,2}/
			x = AllMatchesForm.DownOdds.value.search(re);
			if(x == -1) {
				AllMatchesForm.DownOdds.value = '';
				alert('不正確賠率數字');
			} else {
				AllMatchesForm.MUSTSendChk.checked = true;
			}
		}	else if(eval(len) == 3) {
			re=/\d\x2e\d/
			x = AllMatchesForm.DownOdds.value.search(re);
			if(x == -1) {
				AllMatchesForm.DownOdds.value = '';
				alert('不正確賠率數字');
			} else {
				AllMatchesForm.MUSTSendChk.checked = true;
			}
		}	else if(eval(len) == 4) {
			re=/\d\x2e\d{2,2}/
			x = AllMatchesForm.DownOdds.value.search(re);
			if(x == -1) {
				re2=/\d{2,2}\x2e\d/
				y = AllMatchesForm.DownOdds.value.search(re2);
				if(y == -1) {
					AllMatchesForm.DownOdds.value = '';
					alert('不正確賠率數字');
				} else {
					AllMatchesForm.MUSTSendChk.checked = true;
				}
			} else {
				AllMatchesForm.MUSTSendChk.checked = true;
			}
		}	else if(eval(len) > 4) {
			re=/\d\x2e\d{3,3}/
			x = AllMatchesForm.DownOdds.value.search(re);
			if(x == -1) {
				re2=/\d{2,2}\x2e\d{2,2}/
				y = AllMatchesForm.DownOdds.value.search(re2);
				if(y == -1) {
					re3=/\d{3,3}\x2e\d/
					z = AllMatchesForm.DownOdds.value.search(re3);
					if(z == -1) {
						AllMatchesForm.DownOdds.value = '';
						alert('不正確賠率數字');
					} else {
						AllMatchesForm.MUSTSendChk.checked = true;
					}
				} else {
					AllMatchesForm.MUSTSendChk.checked = true;
				}
			} else {
				AllMatchesForm.MUSTSendChk.checked = true;
			}
		}
	} else {
		AllMatchesForm.MUSTSendChk.checked = true;
	}
}

function EuroWinOddsValidity(validate_index) {
	if(AllMatchesForm.EuroWinOdds.value != '') {
		var len = AllMatchesForm.EuroWinOdds.value.length;
		if(eval(len) == 1) {
			re=/\d/
			x = AllMatchesForm.EuroWinOdds.value.search(re);
			if(x == -1) {
				AllMatchesForm.EuroWinOdds.value = '';
				alert('不正確賠率數字');
			} else {
				AllMatchesForm.MUSTSendChk.checked = true;
			}
		} else if(eval(len) == 2) {
			re=/\d{2,2}/
			x = AllMatchesForm.EuroWinOdds.value.search(re);
			if(x == -1) {
				AllMatchesForm.EuroWinOdds.value = '';
				alert('不正確賠率數字');
			} else {
				AllMatchesForm.MUSTSendChk.checked = true;
			}
		} else if(eval(len) == 3) {
			re=/\d\x2e\d/
			x = AllMatchesForm.EuroWinOdds.value.search(re);
			if(x == -1) {
				AllMatchesForm.EuroWinOdds.value = '';
				alert('不正確賠率數字');
			} else {
				AllMatchesForm.MUSTSendChk.checked = true;
			}
		} else if(eval(len) > 3) {
			re=/\d\x2e\d{2,2}/
			x = AllMatchesForm.EuroWinOdds.value.search(re);
			if(x == -1) {
				re2=/\d{2,2}\x2e\d/
				y = AllMatchesForm.EuroWinOdds.value.search(re2);
				if(y == -1) {
					AllMatchesForm.EuroWinOdds.value = '';
					alert('不正確賠率數字');
				} else {
					AllMatchesForm.MUSTSendChk.checked = true;
				}
			} else {
				AllMatchesForm.MUSTSendChk.checked = true;
			}
		}
	} else {
		AllMatchesForm.MUSTSendChk.checked = true;
	}
}

function EuroLossOddsValidity(validate_index) {
	if(AllMatchesForm.EuroLossOdds.value != '') {
		var len = AllMatchesForm.EuroLossOdds.value.length;
		if(eval(len) == 1) {
			re=/\d/
			x = AllMatchesForm.EuroLossOdds.value.search(re);
			if(x == -1) {
				AllMatchesForm.EuroLossOdds.value = '';
				alert('不正確賠率數字');
			} else {
				AllMatchesForm.MUSTSendChk.checked = true;
			}
		} else if(eval(len) == 2) {
			re=/\d{2,2}/
			x = AllMatchesForm.EuroLossOdds.value.search(re);
			if(x == -1) {
				AllMatchesForm.EuroLossOdds.value = '';
				alert('不正確賠率數字');
			} else {
				AllMatchesForm.MUSTSendChk.checked = true;
			}
		} else if(eval(len) == 3) {
			re=/\d\x2e\d/
			x = AllMatchesForm.EuroLossOdds.value.search(re);
			if(x == -1) {
				AllMatchesForm.EuroLossOdds.value = '';
				alert('不正確賠率數字');
			} else {
				AllMatchesForm.MUSTSendChk.checked = true;
			}
		} else if(eval(len) > 3) {
			re=/\d\x2e\d{2,2}/
			x = AllMatchesForm.EuroLossOdds.value.search(re);
			if(x == -1) {
				re2=/\d{2,2}\x2e\d/
				y = AllMatchesForm.EuroLossOdds.value.search(re2);
				if(y == -1) {
					AllMatchesForm.EuroLossOdds.value = '';
					alert('不正確賠率數字');
				} else {
					AllMatchesForm.MUSTSendChk.checked = true;
				}
			} else {
				AllMatchesForm.MUSTSendChk.checked = true;
			}
		}
	} else {
		AllMatchesForm.MUSTSendChk.checked = true;
	}
}

function EuroDrawOddsValidity(validate_index) {
	if(AllMatchesForm.EuroDrawOdds.value != '') {
		var len = AllMatchesForm.EuroDrawOdds.value.length;
		if(eval(len) == 1) {
			re=/\d/
			x = AllMatchesForm.EuroDrawOdds.value.search(re);
			if(x == -1) {
				AllMatchesForm.EuroDrawOdds.value = '';
				alert('不正確賠率數字');
			} else {
				AllMatchesForm.MUSTSendChk.checked = true;
			}
		} else if(eval(len) == 2) {
			re=/\d{2,2}/
			x = AllMatchesForm.EuroDrawOdds.value.search(re);
			if(x == -1) {
				AllMatchesForm.EuroDrawOdds.value = '';
				alert('不正確賠率數字');
			} else {
				AllMatchesForm.MUSTSendChk.checked = true;
			}
		} else if(eval(len) == 3) {
			re=/\d\x2e\d/
			x = AllMatchesForm.EuroDrawOdds.value.search(re);
			if(x == -1) {
				AllMatchesForm.EuroDrawOdds.value = '';
				alert('不正確賠率數字');
			} else {
				AllMatchesForm.MUSTSendChk.checked = true;
			}
		} else if(eval(len) > 3) {
			re=/\d\x2e\d{2,2}/
			x = AllMatchesForm.EuroDrawOdds.value.search(re);
			if(x == -1) {
				re2=/\d{2,2}\x2e\d/
				y = AllMatchesForm.EuroDrawOdds.value.search(re2);
				if(y == -1) {
					AllMatchesForm.EuroDrawOdds.value = '';
					alert('不正確賠率數字');
				} else {
					AllMatchesForm.MUSTSendChk.checked = true;
				}
			} else {
				AllMatchesForm.MUSTSendChk.checked = true;
			}
		}
	} else {
		AllMatchesForm.MUSTSendChk.checked = true;
	}
}

function onStatusChanged(validate_index) {
	AllMatchesForm.MUSTSendChk.checked = true;
}

function onChecked(validate_index) {
	AllMatchesForm.MUSTSendChk.checked = true;
}

function selectAll(iType) {
	if(iType == 0) {
		if(AllMatchesForm.SelectAllSend.checked == true) {
			AllMatchesForm.MUSTSendChk.checked = true;
		} else {
			AllMatchesForm.MUSTSendChk.checked = false;
		}
	} else if(iType == 1) {
		if(AllMatchesForm.SelectAllHide.checked == true) {
			AllMatchesForm.hiddenChk.checked = true;
		} else {
			AllMatchesForm.hiddenChk.checked = false;
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
					if(AllMatchesForm.<%=sLeagChkItem%>.length > 1) {
						for(j = 0; j < AllMatchesForm.<%=sLeagChkItem%>.length; j++) {
							if(AllMatchesForm.leagGpChk[leagGp].checked == true) {
								AllMatchesForm.MUSTSendChk[AllMatchesForm.<%=sLeagChkItem%>[j].value].checked = true;
							} else {
								AllMatchesForm.MUSTSendChk[AllMatchesForm.<%=sLeagChkItem%>[j].value].checked = false;
							}
						}
					} else {
						if(AllMatchesForm.leagGpChk[leagGp].checked == true) {
							<%
								if(iRecCount > 1) {
							%>
								AllMatchesForm.MUSTSendChk[AllMatchesForm.<%=sLeagChkItem%>.value].checked = true;
							<%
								} else {
							%>
								AllMatchesForm.MUSTSendChk.checked = true;
							<%
								}
							%>
						} else {
							<%
								if(iRecCount > 1) {
							%>
								AllMatchesForm.MUSTSendChk[AllMatchesForm.<%=sLeagChkItem%>.value].checked = false;
							<%
								} else {
							%>
								AllMatchesForm.MUSTSendChk.checked = false;
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
				if(AllMatchesForm.LeagueChk_0.length > 1) {
					for(j = 0; j < AllMatchesForm.LeagueChk_0.length; j++) {
						if(AllMatchesForm.leagGpChk.checked == true) {
							AllMatchesForm.MUSTSendChk[AllMatchesForm.LeagueChk_0[j].value].checked = true;
						} else {
							AllMatchesForm.MUSTSendChk[AllMatchesForm.LeagueChk_0[j].value].checked = false;
						}
					}
				} else {
					if(AllMatchesForm.leagGpChk.checked == true) {
						AllMatchesForm.MUSTSendChk.checked = true;
					} else {
						AllMatchesForm.MUSTSendChk.checked = false;
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
					if(AllMatchesForm.<%=sLeagChkItem%>.length > 1) {
						for(j = 0; j < AllMatchesForm.<%=sLeagChkItem%>.length; j++) {
							if(AllMatchesForm.leagGpHide[leagGp].checked == true) {
								AllMatchesForm.hiddenChk[AllMatchesForm.<%=sLeagChkItem%>[j].value].checked = true;
							} else {
								AllMatchesForm.hiddenChk[AllMatchesForm.<%=sLeagChkItem%>[j].value].checked = false;
							}
						}
					} else {
						if(AllMatchesForm.leagGpHide[leagGp].checked == true) {
							<%
								if(iRecCount > 1) {
							%>
								AllMatchesForm.hiddenChk[AllMatchesForm.<%=sLeagChkItem%>.value].checked = true;
							<%
								} else {
							%>
								AllMatchesForm.hiddenChk.checked = true;
							<%
								}
							%>
						} else {
							<%
								if(iRecCount > 1) {
							%>
								AllMatchesForm.hiddenChk[AllMatchesForm.<%=sLeagChkItem%>.value].checked = false;
							<%
								} else {
							%>
								AllMatchesForm.hiddenChk.checked = false;
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
				if(AllMatchesForm.LeagueChk_0.length > 1) {
					for(j = 0; j < AllMatchesForm.LeagueChk_0.length; j++) {
						if(AllMatchesForm.leagGpHide.checked == true) {
							AllMatchesForm.hiddenChk[AllMatchesForm.LeagueChk_0[j].value].checked = true;
						} else {
							AllMatchesForm.hiddenChk[AllMatchesForm.LeagueChk_0[j].value].checked = false;
						}
					}
				} else {
					if(AllMatchesForm.leagGpHide.checked == true) {
						AllMatchesForm.hiddenChk.checked = true;
					} else {
						AllMatchesForm.hiddenChk.checked = false;
					}
				}
	<%
			}
		}
	%>
}

function ReSort(sortType) {
	parent.content_frame.location.replace('AllMatchesRetrieval.aspx?sort=' + sortType);
}

function ShowHiddenMatches() {
	matchWinFeature="Resizable=Yes,ScrollBars=Yes,MenuBar=No,Directories=No,ToolBar=No,Location=No,Status=No, width=400, height=680";
	matchWindow = window.open('HiddenMatch.aspx?type=odds', 'HiddenMatch' , matchWinFeature);
	matchWindow.focus();
}
</script>

<html>
<head>
	<META http-equiv="Content-Type" content="text/html; charset=big5">
	<LINK REL="stylesheet" HREF="/sportStyle.css" TYPE="text/css">
</head>
<body>
	<form id="AllMatchesForm" method="post" runat="server">
		<font size="2"><b>上次行動:</b><asp:Label id="historyMsg" runat="server" /></font><br>

		<table border="1" width="100%" style="font: 10pt verdana">
		<tr style="background-color:#FFC39C; color:#191970">
			<th colspan="14" align="left">
				<%
					if(leagueList != null && leagueList.Count > 0) {
				%>
					選擇隱藏:&nbsp;
				<%
						if(leagueList.Count > 1) {
							for(int i = 0; i < leagueList.Count; i++) {
				%>
							<input type="checkbox" name="leagGpHide" value="<%=i.ToString()%>" onClick="SelectLeagueToHide(AllMatchesForm.leagGpHide[<%=i.ToString()%>].value)"><%=leagueList[i]%>&nbsp;&nbsp;
				<%
							}
						} else {
				%>
							<input type="checkbox" name="leagGpHide" value="0" onClick="SelectLeagueToHide(AllMatchesForm.leagGpHide.value)"><%=leagueList[0]%>&nbsp;&nbsp;
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
					選擇傳送:&nbsp;
				<%
						if(leagueList.Count > 1) {
							for(int i = 0; i < leagueList.Count; i++) {
				%>
							<input type="checkbox" name="leagGpChk" value="<%=i.ToString()%>" onClick="SelectLeagueToSend(AllMatchesForm.leagGpChk[<%=i.ToString()%>].value)"><%=leagueList[i]%>&nbsp;&nbsp;
				<%
							}
						} else {
				%>
							<input type="checkbox" name="leagGpChk" value="0" onClick="SelectLeagueToSend(AllMatchesForm.leagGpChk.value)"><%=leagueList[0]%>&nbsp;&nbsp;
				<%
						}
					}
				%>
			</th>
		</tr>

		<tr style="background-color:#FFF0F5">
			<th colspan="6" align="left">
				<input type="button" name="clearOrder" value="清除序號" onClick="ClearOrderNo()">&nbsp;&nbsp;
				順序依
				<select name="sortType" onChange="ReSort(AllMatchesForm.sortType.value)">
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
			<th colspan="2">上盤</th>
			<th colspan="2">大小盤</th>
			<th>歐盤</th>
			<th></th>
			<th style="background-color:#FFFAF0; color:#CD5C5C">全選<input type="checkbox" name="SelectAllSend" onClick="selectAll(0)"></th>
			<th style="background-color:#FFC39C; color:#191970">全選<input type="checkbox" name="SelectAllHide" onClick="selectAll(1)"></th>
		</tr>

		<tr style="background-color:#FFF0F5">
			<th>排序</th>
			<th>聯賽</th>
			<th>主隊</th>
			<th>客隊</th>
			<th>中立</th>
			<th>主讓</th>
			<th>讓球</th>
			<th>賠率</th>
			<th>總入球</th>
			<th>賠率</th>
			<th>勝/負  和</th>
			<th>響</th>
			<th style="background-color:#FFFAF0; color:#CD5C5C">強制傳送</th>
			<th style="background-color:#FFC39C; color:#191970">隱藏</th>
		</tr>

		<span id="MatchesInformation" runat="server" />

		<tr>
			<td style="background-color:#FFFAF0" colspan="13" align="right">
				<!--
					Value of SendToPager is Device ID defined in DEVICE_TYPE
				-->
				<input type="checkbox" name="SendToPager" value="1" checked>GOGO1&nbsp;
				<input type="checkbox" name="SendToPager" value="2" checked>GOGO2&nbsp;
				<input type="checkbox" name="SendToPager" value="5" checked>GOGO3Combo&nbsp;
				<asp:Button id="SendBtn" Text="傳送" CommandName="SEND" OnCommand="GetMatchesAction" runat="server" />
			</td>
			<td></td>
		</tr>
		<tr>
			<td colspan="11"><asp:Button id="SortBtn" Text="儲存序號" CommandName="SORT" tabindex="500" OnCommand="GetMatchesAction" runat="server" /></td>
			<td style="background-color:#FFC39C" colspan="2" align="right"><asp:Button id="ShowBtn" Text="顯示所有" CommandName="SHOW" OnCommand="GetMatchesAction" runat="server" /></td>
			<td style="background-color:#FFC39C" align="center"><asp:Button id="HideBtn" Text="隱藏" CommandName="HIDE" OnCommand="GetMatchesAction" runat="server" /></td>
		</tr>

		<tr>
			<th align="right" colspan="14" style="background-color:#FFC39C; color:#191970">
				<a href="javascript:ShowHiddenMatches()">已隱藏的賽事</a>
			</th>
		</tr>
		</table>
	</form>
</body>
</html>