<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">
	void Page_Load(Object sender,EventArgs e) {
		BSKModifyResult result = new BSKModifyResult((string)Application["BasketballDBConnectionString"]);
		try {
			ResultInformation.InnerHtml = result.GetDetails();
			UpdateReturnMessage((string)Application["retrieveInfoMsg"] + "�x�y�ɪG(" + DateTime.Now.ToString("HH:mm:ss") + ")");
		} catch(NullReferenceException) {
			FormsAuthentication.SignOut();
			UpdateReturnMessage("<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>");
		}
	}

	void onModify(Object sender,EventArgs e) {
		BSKModifyResult result = new BSKModifyResult((string)Application["BasketballDBConnectionString"]);
		try {
			int iUpd = result.Modify();
			Page_Load(sender, e);
			if(iUpd > 0) {
				UpdateReturnMessage("���\��s�x�y�ɪG(" + DateTime.Now.ToString("HH:mm:ss") + ")");
			} else if(iUpd == 0) {
				UpdateReturnMessage("�S����s�x�y�ɪG(" + DateTime.Now.ToString("HH:mm:ss") + ")");
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
function HostScoreChanged() {
	var total = 0;
	for(i = 0; i < BSKModifyResultForm.HostScore.length; i++) {
		total += Number(BSKModifyResultForm.HostScore[i].value);
	}
	BSKModifyResultForm.TotalHostScore.value = total;
}

function GuestScoreChanged() {
	var total = 0;
	for(i = 0; i < BSKModifyResultForm.GuestScore.length; i++) {
		total += Number(BSKModifyResultForm.GuestScore[i].value);
	}
	BSKModifyResultForm.TotalGuestScore.value = total;
}

function checkScore() {
	pass = true;
	score_re = /^\d{0,2}$/

	for(i = 0; i < BSKModifyResultForm.HostScore.length; i++) {
		re_val = BSKModifyResultForm.HostScore[i].value.search(score_re)
		if(re_val == -1) {
			alert('��ƥu�����Ʀr');
			BSKModifyResultForm.HostScore[i].select();
			pass = false;
		}
	}

	for(i = 0; i < BSKModifyResultForm.GuestScore.length; i++) {
		re_val = BSKModifyResultForm.GuestScore[i].value.search(score_re)
		if(re_val == -1) {
			alert('��ƥu�����Ʀr');
			BSKModifyResultForm.GuestScore[i].select();
			pass = false;
		}
	}

	return pass;
}

function showBubbleText() {
	document.getElementById('bubbleText').style.visibility = "visible";
	document.getElementById('bubbleText').style.position = "absolute";
	document.getElementById('bubbleText').style.left = event.clientX - 180;
	document.getElementById('bubbleText').style.top = event.clientY - 20;
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
	<form id="BSKModifyResultForm" method="post" runat="server" onSubmit="return checkScore()">
		<asp:Label id="rtnMsg" runat="server" />
		<table border="1" width="60%">
			<span id="ResultInformation" runat="server" />

			<tr>
				<td></td>
				<td align="left">
					<input type="submit" id="SaveBtn" value="�ק�" OnServerClick="onModify" runat="server" onmouseover="showBubbleText()" onmouseout="hideBubbleText()">&nbsp;
					<input type="reset" value="���]">
				</td>
			</tr>
			<tr>
				<th colspan="2" align="left">
					�w�ק諸�ƾڤ��|�o�e�A�Ф��O���o�ɪG��GOGO���ΰ��|��<br>
					<font color="#FF0000">���o/�R����T -> ���o�ǩI����T -> GOGO �x�y -> �ɪG</font><br>
					<font color="#FF0000">���o/�R����T -> ���o�ǩI����T -> ���| �x�y -> �ɪG</font><br>
					<font color="#FF0000">���o/�R����T -> ���o�ǩI����T -> Combo �x�y -> �ɪG</font>
				</th>
			</tr>
		</table>
		<span id="bubbleText" style="visibility:hidden"><font color="#FF0000"><b>�ХJ���[�ݤU�C����</b></font></span>
	</form>
	</center>
</body>
</html>