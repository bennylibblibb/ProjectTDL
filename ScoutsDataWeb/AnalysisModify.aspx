<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">
	void Page_Load(Object sender,EventArgs e) {
	 
		string sAnalysisOption = "";
		 SoccerMenuAnalysis MenuAnalysis = new SoccerMenuAnalysis(ConfigurationManager.AppSettings["SoccerDBConnectionString"]);
		 try {  
			sAnalysisOption = MenuAnalysis.Show();
			AnalysisModifyInformation2.InnerHtml = sAnalysisOption; 

		} catch(NullReferenceException) {
		 }

        
        //AnalysisModify matchAnalysis = new AnalysisModify(ConfigurationManager.AppSettings["SoccerDBConnectionString"]);
       AnalysisModify matchAnalysis = new AnalysisModify(ConfigurationManager.AppSettings["SoccerDBConnectionString"]);
		try {
			AnalysisModifyInformation.InnerHtml = matchAnalysis.GetMatches();
			UpdateHistoryMessage(ConfigurationManager.AppSettings["retrieveInfoMsg"] + "賽事分析(" + DateTime.Now.ToString("HH:mm:ss") + ")");
		} catch(NullReferenceException) {
			FormsAuthentication.SignOut();
			UpdateHistoryMessage("<a href=\"/sports/index.htm\" target=\"_top\">" + ConfigurationManager.AppSettings["sessionExpiredMsg"]+ "</a>");
		}
	}

	void onSaveMatchAnalysis(Object sender,EventArgs e) {
		int iUpdated = 0;
		string sNow;
		//AnalysisModify modifyAnly = new AnalysisModify(ConfigurationManager.AppSettings["SoccerDBConnectionString"]);
        AnalysisModify modifyAnly = new AnalysisModify(ConfigurationManager.AppSettings["SoccerDBConnectionString"]);
		sNow = DateTime.Now.ToString("HH:mm:ss");
		try {
			iUpdated = modifyAnly.Modify();
			Page_Load(sender,e);
			if(iUpdated > 0) {
				UpdateHistoryMessage("成功修改賽事分析(" + sNow + ")");
			}	else if(iUpdated == 0) {
				UpdateHistoryMessage("沒有修改賽事分析(" + sNow + ")");
			}	else {
				UpdateHistoryMessage(ConfigurationManager.AppSettings["transErrorMsg"] + "(" + sNow + ")");
			}
		} catch(NullReferenceException nullex) {
			FormsAuthentication.SignOut();
			UpdateHistoryMessage("<a href=\"/sports/index.htm\" target=\"_top\">" + ConfigurationManager.AppSettings["sessionExpiredMsg"]+ "</a>");
		}
	}

	void UpdateHistoryMessage(string sMsg) {
		historyMsg.Text = sMsg;
	}

     
</script>

<script language="JavaScript">

    
    function goToAnalysisModify(selectedMatchModify) {
       // alert(selectedMatchModify);
	if(selectedMatchModify!='' && selectedMatchModify!='0') {
		//parent.content_frame.location.replace('AnalysisModify.aspx?matchcnt=' + selectedMatchModify);
          window.location.replace('AnalysisModify.aspx?matchcnt=' + selectedMatchModify);
	}

	AnalysisModifyForm.soccerMenuAnalysisModify.value = '0';
    }

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

function checkRemarkByte() {
	str = document.AnalysisModifyForm.remarks.value;
	strlen = LengthOfString(str);
	if(strlen > 300) {
		alert('附加訊息不能多於300個位元');
	}
}

function onHostScoreChanged(validate_index) {
	re = /^\d{0,2}$/
	re_val = AnalysisModifyForm.hostscore[validate_index].value.search(re)
	if(re_val == -1) {
		alert('比數只接受數字');
		AnalysisModifyForm.hostscore[validate_index].value = '0';
	}
}

function onGuestScoreChanged(validate_index) {
	re = /^\d{0,2}$/
	re_val = AnalysisModifyForm.guestscore[validate_index].value.search(re)
	if(re_val == -1) {
		alert('比數只接受數字');
		AnalysisModifyForm.guestscore[validate_index].value = '0';
	}
}
</script>

<html>
<head>
	<META http-equiv="Content-Type" content="text/html; charset=big5">
	<LINK REL="stylesheet" HREF="/sportStyle.css" TYPE="text/css">
     <title>分析</title>
</head>
<body>
	<form id="AnalysisModifyForm" method="post" runat="server">
		<font size="2"><b>上次行動:</b><asp:Label id="historyMsg" runat="server" /></font><br>
        
					<select name="soccerMenuAnalysisModify" onChange="goToAnalysisModify(AnalysisModifyForm.soccerMenuAnalysisModify.value)">
						<option value="0">請選擇</option>
						<span id="AnalysisModifyInformation2" runat="server" />
					</select>
		<table border="1" width="70%" style="font: 10pt verdana">
		<span id="AnalysisModifyInformation" runat="server" />
		<tr>
			<td colspan="3" align="right">
				<input type="submit" id="SaveBtn" value="儲存" OnServerClick="onSaveMatchAnalysis" runat="server">&nbsp;
				<input type="reset" value="重設">
			</td>
		</table>
	</form>
</body>
</html>