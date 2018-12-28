<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">
	void Page_Load(Object sender,EventArgs e) {
		ModifyResultContent result = new ModifyResultContent();
		try {
			ResultContentInformation.InnerHtml = result.GetDetails();
			UpdateReturnMessage((string)Application["retrieveInfoMsg"] + "�ɪG�Ա�" + "(" + DateTime.Now.ToString("HH:mm:ss") + ")");
		} catch(NullReferenceException) {
			FormsAuthentication.SignOut();
			UpdateReturnMessage("<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>");
		}
	}

	void onModify(Object sender,EventArgs e) {
		ModifyResultContent result = new ModifyResultContent();
		try {
			int iUpd = result.Modify();
			Page_Load(sender, e);
			if(iUpd > 0) {
				UpdateReturnMessage("���\��s���y�ɪG(" + DateTime.Now.ToString("HH:mm:ss") + ")");
			} else if(iUpd == 0) {
				UpdateReturnMessage("�S����s���y�ɪG(" + DateTime.Now.ToString("HH:mm:ss") + ")");
			} else {
				UpdateReturnMessage((string)Application["transErrorMsg"] + "(" + DateTime.Now.ToString("HH:mm:ss") + ")");
			}
		} catch(NullReferenceException) {
			FormsAuthentication.SignOut();
			UpdateReturnMessage("<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>");
		}
	}

	void UpdateReturnMessage(string sMsg) {
		rtnMsg.Text = sMsg;
	}
</script>

<script language="JavaScript">
function checkScore() {
	pass = true;
	ttscore_re = /^\d{1,2}$/
	score_re = /^\d{0,2}$/
	time_re = /^\d{0,3}$/

	re_val = ModifyResultContentForm.HostTotalGoal.value.search(ttscore_re)
	if(re_val == -1) {
		alert('�D���`��ƥu�����Ʀr');
		ModifyResultContentForm.HostTotalGoal.select();
		pass = false;
	}

	re_val = ModifyResultContentForm.GuestTotalGoal.value.search(ttscore_re)
	if(re_val == -1) {
		alert('�ȶ��`��ƥu�����Ʀr');
		ModifyResultContentForm.GuestTotalGoal.select();
		pass = false;
	}

	for(i = 0; i < ModifyResultContentForm.MatchTime.length; i++) {
		re_val = ModifyResultContentForm.MatchTime[i].value.search(time_re)
		if(re_val == -1) {
			alert('�ɶ��u�����Ʀr');
			ModifyResultContentForm.MatchTime[i].select();
			pass = false;
		}
	}

	for(i = 0; i < ModifyResultContentForm.HostGoal.length; i++) {
		re_val = ModifyResultContentForm.HostGoal[i].value.search(score_re)
		if(re_val == -1) {
			alert('�D����ƥu�����Ʀr');
			ModifyResultContentForm.HostGoal[i].select();
			pass = false;
		}
	}

	for(i = 0; i < ModifyResultContentForm.GuestGoal.length; i++) {
		re_val = ModifyResultContentForm.GuestGoal[i].value.search(score_re)
		if(re_val == -1) {
			alert('�ȶ���ƥu�����Ʀr');
			ModifyResultContentForm.GuestGoal[i].select();
			pass = false;
		}
	}

	return pass;
}

function showBubbleText() {
	document.getElementById('bubbleText').style.visibility = "visible";
	document.getElementById('bubbleText').style.position = "absolute";
	document.getElementById('bubbleText').style.left = event.clientX - 180;
	document.getElementById('bubbleText').style.top = event.clientY + 150;
}

function hideBubbleText() {
	document.getElementById('bubbleText').style.visibility = "hidden";
}
</script>

<html>
<head>
	<META http-equiv="Content-Type" content="text/html; charset=big5">
	<LINK REL="stylesheet" HREF="/sportStyle.css" TYPE="text/css">
</head>
<body>
	<center>
	<form id="ModifyResultContentForm" method="post" runat="server" onSubmit="return checkScore()">
		<font size="2"><b>�W�����:</b><asp:Label id="rtnMsg" runat="server" /></asp:Label></font>
		<table border="1" width="80%" style="font: 10pt verdana">
			<span id="bubbleText" style="visibility:hidden"><font color="#FF0000"><b>�ХJ���[�ݤU�C����</b></font></span>
			<span id="ResultContentInformation" runat="server" />

			<tr>
				<td colspan="5"></td>
				<td align="center">
					<input type="submit" id="SaveBtn" value="�ק�" OnServerClick="onModify" runat="server" onmouseover="showBubbleText()" onmouseout="hideBubbleText()">&nbsp;
					<input type="reset" value="���]">
				</td>
			</tr>
			<tr>
				<th colspan="6" align="left">
					�w�ק諸�ƾڤ��|�o�e�A�Ф��O���o�ɪG��GOGO1���BGOGO2���ΰ��|��<br>
					<font color="#FF0000">���o/�R����T -> ���o�ǩI����T -> GOGO1 Sender -> �ɪG</font><br>
					<font color="#FF0000">���o/�R����T -> ���o�ǩI����T -> GOGO2 Handler 1 -> �ɪG</font><br>
					<font color="#FF0000">���o/�R����T -> ���o�ǩI����T -> ���| Handler 1 -> �ɪG</font><br>
					<font color="#FF0000">���o/�R����T -> ���o�ǩI����T -> JC Combo Handler -> �ɪG</font><br>
					<font color="#CD5C5C">�p���R���@��ƾڡA�Ч⨺��ƾڪ�<font color="#000080">�ɶ�</font>�B<font color="#000080">�D�����</font>��<font color="#000080">�ȶ����</font>��šA�A��<font color="#000000">�ק�</font></font>
				</th>
			</tr>
		</table>
	</form>
	</center>
</body>
</html>