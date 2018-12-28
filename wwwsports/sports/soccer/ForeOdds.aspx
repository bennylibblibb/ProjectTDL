<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">
	int iRecCount;

	void Page_Load(Object sender,EventArgs e) {
		ForeOdds odds = new ForeOdds((string)Application["SoccerDBConnectionString"]);

		try {
			ForeOddsInformation.InnerHtml = odds.GetOdds();
			iRecCount = odds.NumberOfRecords;
			UpdateHistoryMessage((string)Application["retrieveInfoMsg"] + "�w������(" + DateTime.Now.ToString("HH:mm:ss") + ")");
		} catch(NullReferenceException) {
			FormsAuthentication.SignOut();
			UpdateHistoryMessage("<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>");
		}
	}

	void onUpdateOdds(Object sender,CommandEventArgs e) {
		int iUpdated = 0;
		string sNow;
		string sCmd;
		ForeOdds fodds = new ForeOdds((string)Application["SoccerDBConnectionString"]);

		sCmd = (string)e.CommandName;
		sNow = DateTime.Now.ToString("HH:mm:ss");
		try {
			iUpdated = fodds.UpdateForeOdds(sCmd);
			Page_Load(sender,e);
			if(sCmd.Equals("SEND")) {
				if(iUpdated > 0) {
					UpdateHistoryMessage("���\��s" + iUpdated.ToString() + "���w������(" + sNow + ")");
				}	else if(iUpdated == 0) {
					UpdateHistoryMessage("�S����s�w������(" + sNow + ")");
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
function onChecked(validate_index) {
	ForeOddsForm.MUSTSendChk[validate_index].checked = true;
}

function DateValidity() {
	<%
		int iIdx;
		string sInitDate;
		string sInitHandicap1;
		string sInitHandicap2;
		string sInitOdds;
		string s2ndDate;
		string s2ndHandicap1;
		string s2ndHandicap2;
		string s2ndOdds;
		for(iIdx = 0; iIdx < iRecCount; iIdx++) {
			sInitDate = "InitDate[" + iIdx.ToString() + "]";
			sInitHandicap1 = "ScoreHandicap1[" + iIdx.ToString() + "]";
			sInitHandicap2 = "ScoreHandicap2[" + iIdx.ToString() + "]";
			sInitOdds = "Odds[" + iIdx.ToString() + "]";
			s2ndDate = "LastDate[" + iIdx.ToString() + "]";
			s2ndHandicap1 = "LastScoreHandicap1[" + iIdx.ToString() + "]";
			s2ndHandicap2 = "LastScoreHandicap2[" + iIdx.ToString() + "]";
			s2ndOdds = "LastOdds[" + iIdx.ToString() + "]";
	%>
		if(ForeOddsForm.<%=sInitDate%>.value != '') {
			re=/\d{8,8}/
			x = ForeOddsForm.<%=sInitDate%>.value.search(re);
			if(x == -1) {
				ForeOddsForm.<%=sInitDate%>.value = '';
				alert('�����߲v��������ݬO8��Ʀr');
				return false;
			}
		} else {
			alert('�п�J�����߲v�����');
			return false;
		}

		if(ForeOddsForm.<%=s2ndDate%>.value != '') {
			re=/\d{8,8}/
			x = ForeOddsForm.<%=s2ndDate%>.value.search(re);
			if(x == -1) {
				ForeOddsForm.<%=s2ndDate%>.value = '';
				alert('��s�߲v��������ݬO8��Ʀr');
				return false;
			}
		} else {
			if(ForeOddsForm.<%=s2ndHandicap1%>.value!='' || ForeOddsForm.<%=s2ndHandicap2%>.value!='' || ForeOddsForm.<%=s2ndOdds%>.value!='') {
				alert('�п�J��s�߲v�����');
				return false;
			}
		}
	<%
		}
	%>
}

function ScoreHandicap1Validity(validate_index) {
	if(ForeOddsForm.ScoreHandicap1[validate_index].value != '') {
		var len = ForeOddsForm.ScoreHandicap1[validate_index].value.length;
		if(eval(len) == 1) {
			re=/\d/
			x = ForeOddsForm.ScoreHandicap1[validate_index].value.search(re);
			if(x == -1) {
				ForeOddsForm.ScoreHandicap1[validate_index].value = '';
				alert('�����T���y�Ʀr');
			} else {
				ForeOddsForm.MUSTSendChk[validate_index].checked = true;
			}
		} else if(eval(len) > 1) {
			re=/\d\x2e\d/
			x = ForeOddsForm.ScoreHandicap1[validate_index].value.search(re);
			if(x == -1) {
				ForeOddsForm.ScoreHandicap1[validate_index].value = '';
				alert('�����T���y�Ʀr');
			} else {
				ForeOddsForm.MUSTSendChk[validate_index].checked = true;
			}
		}
	} else {
		ForeOddsForm.MUSTSendChk[validate_index].checked = true;
	}
}

function ScoreHandicap2Validity(validate_index) {
	if(ForeOddsForm.ScoreHandicap2[validate_index].value != '') {
		var len = ForeOddsForm.ScoreHandicap2[validate_index].value.length;
		if(eval(len) == 1) {
			re=/\d/
			x = ForeOddsForm.ScoreHandicap2[validate_index].value.search(re);
			if(x == -1) {
				ForeOddsForm.ScoreHandicap2[validate_index].value = '';
				alert('�����T���y�Ʀr');
			} else {
				ForeOddsForm.MUSTSendChk[validate_index].checked = true;
			}
		} else if(eval(len) > 1) {
			re=/\d\x2e\d/
			x = ForeOddsForm.ScoreHandicap2[validate_index].value.search(re);
			if(x == -1) {
				ForeOddsForm.ScoreHandicap2[validate_index].value = '';
				alert('�����T���y�Ʀr');
			} else {
				ForeOddsForm.MUSTSendChk[validate_index].checked = true;
			}
		}
	} else {
		ForeOddsForm.MUSTSendChk[validate_index].checked = true;
	}
}

function OddsValidity(validate_index) {
	if(ForeOddsForm.Odds[validate_index].value != '') {
		var len = ForeOddsForm.Odds[validate_index].value.length;
		if(eval(len) == 1) {
			re=/\d/
			x = ForeOddsForm.Odds[validate_index].value.search(re);
			if(x == -1) {
				ForeOddsForm.Odds[validate_index].value = '';
				alert('�����T�߲v�Ʀr');
			} else {
				ForeOddsForm.MUSTSendChk[validate_index].checked = true;
			}
		} else if(eval(len) == 2) {
			re=/\d{2,2}/
			x = ForeOddsForm.Odds[validate_index].value.search(re);
			if(x == -1) {
				ForeOddsForm.Odds[validate_index].value = '';
				alert('�����T�߲v�Ʀr');
			} else {
				ForeOddsForm.MUSTSendChk[validate_index].checked = true;
			}
		}	else if(eval(len) == 3) {
			re=/\d\x2e\d/
			x = ForeOddsForm.Odds[validate_index].value.search(re);
			if(x == -1) {
				ForeOddsForm.Odds[validate_index].value = '';
				alert('�����T�߲v�Ʀr');
			} else {
				ForeOddsForm.MUSTSendChk[validate_index].checked = true;
			}
		}	else if(eval(len) == 4) {
			re=/\d\x2e\d{2,2}/
			x = ForeOddsForm.Odds[validate_index].value.search(re);
			if(x == -1) {
				re2=/\d{2,2}\x2e\d/
				y = ForeOddsForm.Odds[validate_index].value.search(re2);
				if(y == -1) {
					ForeOddsForm.Odds[validate_index].value = '';
					alert('�����T�߲v�Ʀr');
				} else {
					ForeOddsForm.MUSTSendChk[validate_index].checked = true;
				}
			} else {
				ForeOddsForm.MUSTSendChk[validate_index].checked = true;
			}
		}	else if(eval(len) > 4) {
			re=/\d\x2e\d{3,3}/
			x = ForeOddsForm.Odds[validate_index].value.search(re);
			if(x == -1) {
				re2=/\d{2,2}\x2e\d{2,2}/
				y = ForeOddsForm.Odds[validate_index].value.search(re2);
				if(y == -1) {
					re3=/\d{3,3}\x2e\d/
					z = ForeOddsForm.Odds[validate_index].value.search(re3);
					if(z == -1) {
						ForeOddsForm.Odds[validate_index].value = '';
						alert('�����T�߲v�Ʀr');
					} else {
						ForeOddsForm.MUSTSendChk[validate_index].checked = true;
					}
				} else {
					ForeOddsForm.MUSTSendChk[validate_index].checked = true;
				}
			} else {
				ForeOddsForm.MUSTSendChk[validate_index].checked = true;
			}
		}
	} else {
		ForeOddsForm.MUSTSendChk[validate_index].checked = true;
	}
}

function LastScoreHandicap1Validity(validate_index) {
	if(ForeOddsForm.LastScoreHandicap1[validate_index].value != '') {
		var len = ForeOddsForm.LastScoreHandicap1[validate_index].value.length;
		if(eval(len) == 1) {
			re=/\d/
			x = ForeOddsForm.LastScoreHandicap1[validate_index].value.search(re);
			if(x == -1) {
				ForeOddsForm.LastScoreHandicap1[validate_index].value = '';
				alert('�����T���y�Ʀr');
			} else {
				ForeOddsForm.MUSTSendChk[validate_index].checked = true;
			}
		} else if(eval(len) > 1) {
			re=/\d\x2e\d/
			x = ForeOddsForm.LastScoreHandicap1[validate_index].value.search(re);
			if(x == -1) {
				ForeOddsForm.LastScoreHandicap1[validate_index].value = '';
				alert('�����T���y�Ʀr');
			} else {
				ForeOddsForm.MUSTSendChk[validate_index].checked = true;
			}
		}
	} else {
		ForeOddsForm.MUSTSendChk[validate_index].checked = true;
	}
}

function LastScoreHandicap2Validity(validate_index) {
	if(ForeOddsForm.LastScoreHandicap2[validate_index].value != '') {
		var len = ForeOddsForm.LastScoreHandicap2[validate_index].value.length;
		if(eval(len) == 1) {
			re=/\d/
			x = ForeOddsForm.LastScoreHandicap2[validate_index].value.search(re);
			if(x == -1) {
				ForeOddsForm.LastScoreHandicap2[validate_index].value = '';
				alert('�����T���y�Ʀr');
			} else {
				ForeOddsForm.MUSTSendChk[validate_index].checked = true;
			}
		} else if(eval(len) > 1) {
			re=/\d\x2e\d/
			x = ForeOddsForm.LastScoreHandicap2[validate_index].value.search(re);
			if(x == -1) {
				ForeOddsForm.LastScoreHandicap2[validate_index].value = '';
				alert('�����T���y�Ʀr');
			} else {
				ForeOddsForm.MUSTSendChk[validate_index].checked = true;
			}
		}
	} else {
		ForeOddsForm.MUSTSendChk[validate_index].checked = true;
	}
}

function LastOddsValidity(validate_index) {
	if(ForeOddsForm.LastOdds[validate_index].value != '') {
		var len = ForeOddsForm.LastOdds[validate_index].value.length;
		if(eval(len) == 1) {
			re=/\d/
			x = ForeOddsForm.LastOdds[validate_index].value.search(re);
			if(x == -1) {
				ForeOddsForm.LastOdds[validate_index].value = '';
				alert('�����T�߲v�Ʀr');
			} else {
				ForeOddsForm.MUSTSendChk[validate_index].checked = true;
			}
		} else if(eval(len) == 2) {
			re=/\d{2,2}/
			x = ForeOddsForm.LastOdds[validate_index].value.search(re);
			if(x == -1) {
				ForeOddsForm.LastOdds[validate_index].value = '';
				alert('�����T�߲v�Ʀr');
			} else {
				ForeOddsForm.MUSTSendChk[validate_index].checked = true;
			}
		}	else if(eval(len) == 3) {
			re=/\d\x2e\d/
			x = ForeOddsForm.LastOdds[validate_index].value.search(re);
			if(x == -1) {
				ForeOddsForm.LastOdds[validate_index].value = '';
				alert('�����T�߲v�Ʀr');
			} else {
				ForeOddsForm.MUSTSendChk[validate_index].checked = true;
			}
		}	else if(eval(len) == 4) {
			re=/\d\x2e\d{2,2}/
			x = ForeOddsForm.LastOdds[validate_index].value.search(re);
			if(x == -1) {
				re2=/\d{2,2}\x2e\d/
				y = ForeOddsForm.LastOdds[validate_index].value.search(re2);
				if(y == -1) {
					ForeOddsForm.LastOdds[validate_index].value = '';
					alert('�����T�߲v�Ʀr');
				} else {
					ForeOddsForm.MUSTSendChk[validate_index].checked = true;
				}
			} else {
				ForeOddsForm.MUSTSendChk[validate_index].checked = true;
			}
		}	else if(eval(len) > 4) {
			re=/\d\x2e\d{3,3}/
			x = ForeOddsForm.LastOdds[validate_index].value.search(re);
			if(x == -1) {
				re2=/\d{2,2}\x2e\d{2,2}/
				y = ForeOddsForm.LastOdds[validate_index].value.search(re2);
				if(y == -1) {
					re3=/\d{3,3}\x2e\d/
					z = ForeOddsForm.LastOdds[validate_index].value.search(re3);
					if(z == -1) {
						ForeOddsForm.LastOdds[validate_index].value = '';
						alert('�����T�߲v�Ʀr');
					} else {
						ForeOddsForm.MUSTSendChk[validate_index].checked = true;
					}
				} else {
					ForeOddsForm.MUSTSendChk[validate_index].checked = true;
				}
			} else {
				ForeOddsForm.MUSTSendChk[validate_index].checked = true;
			}
		}
	} else {
		ForeOddsForm.MUSTSendChk[validate_index].checked = true;
	}
}

function selectAll() {
	<%
		int iSendChkIndex;
		string sSendChk_All;
	%>
	if(ForeOddsForm.SelectAllSend.checked == true) {
		<%
			for(iSendChkIndex=0;iSendChkIndex<iRecCount;iSendChkIndex++) {
				sSendChk_All = "MUSTSendChk[" + iSendChkIndex.ToString() + "]";
		%>
				ForeOddsForm.<%=sSendChk_All%>.checked = true;
		<%
			}
		%>
	} else {
		<%
			for(iSendChkIndex=0;iSendChkIndex<iRecCount;iSendChkIndex++) {
				sSendChk_All = "MUSTSendChk[" + iSendChkIndex.ToString() + "]";
		%>
				ForeOddsForm.<%=sSendChk_All%>.checked = false;
		<%
			}
		%>
	}
}
<%
} else {
%>
function onChecked(validate_index) {
	ForeOddsForm.MUSTSendChk.checked = true;
}

function DateValidity() {
	if(ForeOddsForm.InitDate.value != '') {
		re=/\d{8,8}/
		x = ForeOddsForm.InitDate.value.search(re);
		if(x == -1) {
			ForeOddsForm.InitDate.value = '';
			alert('������ݬO8��Ʀr');
			return false;
		}
	} else {
		if((ForeOddsForm.ScoreHandicap1.value == '') && (ForeOddsForm.ScoreHandicap2.value == '') && (ForeOddsForm.Odds.value == '')){
			alert('�п�J��s�߲v�����');
			return false;
		}
	}

	if(ForeOddsForm.LastDate.value != '') {
		re=/\d{8,8}/
		x = ForeOddsForm.LastDate.value.search(re);
		if(x == -1) {
			ForeOddsForm.LastDate.value = '';
			alert('������ݬO8��Ʀr');
			return false;
		}
	} else {
		if((ForeOddsForm.LastScoreHandicap1.value == '') && (ForeOddsForm.LastScoreHandicap2.value == '') && (ForeOddsForm.LastOdds.value == '')){
			alert('�п�J��s�߲v�����');
			return false;
		}
	}
}

function ScoreHandicap1Validity(validate_index) {
	if(ForeOddsForm.ScoreHandicap1.value != '') {
		var len = ForeOddsForm.ScoreHandicap1.value.length;
		if(eval(len) == 1) {
			re=/\d/
			x = ForeOddsForm.ScoreHandicap1.value.search(re);
			if(x == -1) {
				ForeOddsForm.ScoreHandicap1.value = '';
				alert('�����T���y�Ʀr');
			} else {
				ForeOddsForm.MUSTSendChk.checked = true;
			}
		} else if(eval(len) > 1) {
			re=/\d\x2e\d/
			x = ForeOddsForm.ScoreHandicap1.value.search(re);
			if(x == -1) {
				ForeOddsForm.ScoreHandicap1.value = '';
				alert('�����T���y�Ʀr');
			} else {
				ForeOddsForm.MUSTSendChk.checked = true;
			}
		}
	} else {
		ForeOddsForm.MUSTSendChk.checked = true;
	}
}

function ScoreHandicap2Validity(validate_index) {
	if(ForeOddsForm.ScoreHandicap2.value != '') {
		var len = ForeOddsForm.ScoreHandicap2.value.length;
		if(eval(len) == 1) {
			re=/\d/
			x = ForeOddsForm.ScoreHandicap2.value.search(re);
			if(x == -1) {
				ForeOddsForm.ScoreHandicap2.value = '';
				alert('�����T���y�Ʀr');
			} else {
				ForeOddsForm.MUSTSendChk.checked = true;
			}
		} else if(eval(len) > 1) {
			re=/\d\x2e\d/
			x = ForeOddsForm.ScoreHandicap2.value.search(re);
			if(x == -1) {
				ForeOddsForm.ScoreHandicap2.value = '';
				alert('�����T���y�Ʀr');
			} else {
				ForeOddsForm.MUSTSendChk.checked = true;
			}
		}
	} else {
		ForeOddsForm.MUSTSendChk.checked = true;
	}
}

function OddsValidity(validate_index) {
	if(ForeOddsForm.Odds.value != '') {
		var len = ForeOddsForm.Odds.value.length;
		if(eval(len) == 1) {
			re=/\d/
			x = ForeOddsForm.Odds.value.search(re);
			if(x == -1) {
				ForeOddsForm.Odds.value = '';
				alert('�����T�߲v�Ʀr');
			} else {
				ForeOddsForm.MUSTSendChk.checked = true;
			}
		} else if(eval(len) == 2) {
			re=/\d{2,2}/
			x = ForeOddsForm.Odds.value.search(re);
			if(x == -1) {
				ForeOddsForm.Odds.value = '';
				alert('�����T�߲v�Ʀr');
			} else {
				ForeOddsForm.MUSTSendChk.checked = true;
			}
		}	else if(eval(len) == 3) {
			re=/\d\x2e\d/
			x = ForeOddsForm.Odds.value.search(re);
			if(x == -1) {
				ForeOddsForm.Odds.value = '';
				alert('�����T�߲v�Ʀr');
			} else {
				ForeOddsForm.MUSTSendChk.checked = true;
			}
		}	else if(eval(len) == 4) {
			re=/\d\x2e\d{2,2}/
			x = ForeOddsForm.Odds.value.search(re);
			if(x == -1) {
				re2=/\d{2,2}\x2e\d/
				y = ForeOddsForm.Odds.value.search(re2);
				if(y == -1) {
					ForeOddsForm.Odds.value = '';
					alert('�����T�߲v�Ʀr');
				} else {
					ForeOddsForm.MUSTSendChk.checked = true;
				}
			} else {
				ForeOddsForm.MUSTSendChk.checked = true;
			}
		}	else if(eval(len) > 4) {
			re=/\d\x2e\d{3,3}/
			x = ForeOddsForm.Odds.value.search(re);
			if(x == -1) {
				re2=/\d{2,2}\x2e\d{2,2}/
				y = ForeOddsForm.Odds.value.search(re2);
				if(y == -1) {
					re3=/\d{3,3}\x2e\d/
					z = ForeOddsForm.Odds.value.search(re3);
					if(z == -1) {
						ForeOddsForm.Odds.value = '';
						alert('�����T�߲v�Ʀr');
					} else {
						ForeOddsForm.MUSTSendChk.checked = true;
					}
				} else {
					ForeOddsForm.MUSTSendChk.checked = true;
				}
			} else {
				ForeOddsForm.MUSTSendChk.checked = true;
			}
		}
	} else {
		ForeOddsForm.MUSTSendChk.checked = true;
	}
}

function LastScoreHandicap1Validity(validate_index) {
	if(ForeOddsForm.LastScoreHandicap1.value != '') {
		var len = ForeOddsForm.LastScoreHandicap1.value.length;
		if(eval(len) == 1) {
			re=/\d/
			x = ForeOddsForm.LastScoreHandicap1.value.search(re);
			if(x == -1) {
				ForeOddsForm.LastScoreHandicap1.value = '';
				alert('�����T���y�Ʀr');
			} else {
				ForeOddsForm.MUSTSendChk.checked = true;
			}
		} else if(eval(len) > 1) {
			re=/\d\x2e\d/
			x = ForeOddsForm.LastScoreHandicap1.value.search(re);
			if(x == -1) {
				ForeOddsForm.LastScoreHandicap1.value = '';
				alert('�����T���y�Ʀr');
			} else {
				ForeOddsForm.MUSTSendChk.checked = true;
			}
		}
	} else {
		ForeOddsForm.MUSTSendChk.checked = true;
	}
}

function LastScoreHandicap2Validity(validate_index) {
	if(ForeOddsForm.LastScoreHandicap2.value != '') {
		var len = ForeOddsForm.LastScoreHandicap2.value.length;
		if(eval(len) == 1) {
			re=/\d/
			x = ForeOddsForm.LastScoreHandicap2.value.search(re);
			if(x == -1) {
				ForeOddsForm.LastScoreHandicap2.value = '';
				alert('�����T���y�Ʀr');
			} else {
				ForeOddsForm.MUSTSendChk.checked = true;
			}
		} else if(eval(len) > 1) {
			re=/\d\x2e\d/
			x = ForeOddsForm.LastScoreHandicap2.value.search(re);
			if(x == -1) {
				ForeOddsForm.LastScoreHandicap2.value = '';
				alert('�����T���y�Ʀr');
			} else {
				ForeOddsForm.MUSTSendChk.checked = true;
			}
		}
	} else {
		ForeOddsForm.MUSTSendChk.checked = true;
	}
}

function LastOddsValidity(validate_index) {
	if(ForeOddsForm.LastOdds.value != '') {
		var len = ForeOddsForm.LastOdds.value.length;
		if(eval(len) == 1) {
			re=/\d/
			x = ForeOddsForm.LastOdds.value.search(re);
			if(x == -1) {
				ForeOddsForm.LastOdds.value = '';
				alert('�����T�߲v�Ʀr');
			} else {
				ForeOddsForm.MUSTSendChk.checked = true;
			}
		} else if(eval(len) == 2) {
			re=/\d{2,2}/
			x = ForeOddsForm.LastOdds.value.search(re);
			if(x == -1) {
				ForeOddsForm.LastOdds.value = '';
				alert('�����T�߲v�Ʀr');
			} else {
				ForeOddsForm.MUSTSendChk.checked = true;
			}
		}	else if(eval(len) == 3) {
			re=/\d\x2e\d/
			x = ForeOddsForm.LastOdds.value.search(re);
			if(x == -1) {
				ForeOddsForm.LastOdds.value = '';
				alert('�����T�߲v�Ʀr');
			} else {
				ForeOddsForm.MUSTSendChk.checked = true;
			}
		}	else if(eval(len) == 4) {
			re=/\d\x2e\d{2,2}/
			x = ForeOddsForm.LastOdds.value.search(re);
			if(x == -1) {
				re2=/\d{2,2}\x2e\d/
				y = ForeOddsForm.LastOdds.value.search(re2);
				if(y == -1) {
					ForeOddsForm.LastOdds.value = '';
					alert('�����T�߲v�Ʀr');
				} else {
					ForeOddsForm.MUSTSendChk.checked = true;
				}
			} else {
				ForeOddsForm.MUSTSendChk.checked = true;
			}
		}	else if(eval(len) > 4) {
			re=/\d\x2e\d{3,3}/
			x = ForeOddsForm.LastOdds.value.search(re);
			if(x == -1) {
				re2=/\d{2,2}\x2e\d{2,2}/
				y = ForeOddsForm.LastOdds.value.search(re2);
				if(y == -1) {
					re3=/\d{3,3}\x2e\d/
					z = ForeOddsForm.LastOdds.value.search(re3);
					if(z == -1) {
						ForeOddsForm.LastOdds.value = '';
						alert('�����T�߲v�Ʀr');
					} else {
						ForeOddsForm.MUSTSendChk.checked = true;
					}
				} else {
					ForeOddsForm.MUSTSendChk.checked = true;
				}
			} else {
				ForeOddsForm.MUSTSendChk.checked = true;
			}
		}
	} else {
		ForeOddsForm.MUSTSendChk.checked = true;
	}
}

function selectAll() {
	if(ForeOddsForm.SelectAllSend.checked == true) {
		ForeOddsForm.MUSTSendChk.checked = true;
	} else {
		ForeOddsForm.MUSTSendChk.checked = false;
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
	<form id="ForeOddsForm" method="post" runat="server" ONSUBMIT="return DateValidity()">
		<font size="2"><b>�W�����:</b><asp:Label id="historyMsg" runat="server" /></font><br>

		<table border="1" width="100%" style="font: 10pt verdana">
			<tr style="background-color:#ADFF2F">
				<th colspan="4"></th>
				<th colspan="4" style="background-color:#F5F5DC">������J���߲v</th>
				<th colspan="4" style="background-color:#B0C4DE">�C���s���߲v</th>
				<th></th>
			</tr>
			<tr style="background-color:#ADFF2F">
				<th>�ɨƤ��</th>
				<th>�p��</th>
				<th>�D��</th>
				<th>�ȶ�</th>
				<th style="background-color:#F5F5DC">�D��</th>
				<th style="background-color:#F5F5DC">���y</th>
				<th style="background-color:#F5F5DC">����</th>
				<th style="background-color:#F5F5DC">���</th>
				<th style="background-color:#B0C4DE">�D��</th>
				<th style="background-color:#B0C4DE">���y</th>
				<th style="background-color:#B0C4DE">����</th>
				<th style="background-color:#B0C4DE">���</th>
				<th>�j��ǰe<br><font size="1">(����<input type="checkbox" name="SelectAllSend" onClick="selectAll()">)</font></th>
			</tr>

			<span id="ForeOddsInformation" runat="server" />

			<tr>
				<td colspan="13" align="right">
					<!--
						Value of SendToPager is Path ID
						1: GOGO1 Sender1
						15: GOGO3Combo Asia 
					-->
					<input type="hidden" name="SendToPager" value="1">
					<input type="hidden" name="SendToPager" value="15">
					<asp:Button id="SendBtn" Text="�ǰe" CommandName="SEND" OnCommand="onUpdateOdds" runat="server" />
				</td>
			</tr>
		</table>
	</form>
</body>
</html>