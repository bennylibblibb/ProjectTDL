<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">
	int iTotalGp;
	string sAction;

	void Page_Load(Object sender,EventArgs e) {
		string sRole = (string)Session["user_role"];
		if(sRole == null) sRole = "0";
		sAction = (string)Request.QueryString["act"];
		if(sAction == null) sAction = "M";
		UserManager user = new UserManager((string)Application["SoccerDBConnectionString"]);

		try {
			if(sAction.Trim().Equals("A")) {
				if(Convert.ToInt32(sRole) >= 999) {
					UserInformation.InnerHtml = user.ShowForm(sAction);
					UpdateHistoryMessage((string)Application["retrieveInfoMsg"] + "�s�W�ϥΪ�(" + DateTime.Now.ToString("HH:mm:ss") + ")");
				} else {
					UpdateHistoryMessage("�S���v��(" + DateTime.Now.ToString("HH:mm:ss") + ")");
				}
			} else if(sAction.Trim().Equals("D")) {
				if(Convert.ToInt32(sRole) >= 999) {
					UserInformation.InnerHtml = user.ShowForm(sAction);
					UpdateHistoryMessage((string)Application["retrieveInfoMsg"] + "�R���ϥΪ�(" + DateTime.Now.ToString("HH:mm:ss") + ")");
				} else {
					UpdateHistoryMessage("�S���v��(" + DateTime.Now.ToString("HH:mm:ss") + ")");
				}
			} else {
				UserInformation.InnerHtml = user.ShowForm("M");
				UpdateHistoryMessage((string)Application["retrieveInfoMsg"] + "�ק�K�X(" + DateTime.Now.ToString("HH:mm:ss") + ")");
			}
			iTotalGp = user.TotalGp;
		} catch(NullReferenceException) {
			FormsAuthentication.SignOut();
			UpdateHistoryMessage("<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>");
		}
	}

	void UpdateUser(Object sender,EventArgs e) {
		int iResult;
		UserManager userAdmin = new UserManager((string)Application["SoccerDBConnectionString"]);

		try {
			iResult = userAdmin.Update();
			if(iResult > 0) {
				UpdateHistoryMessage("�x�s���\");
			}	else if(iResult == 0) {
				UpdateHistoryMessage("�ۦP�O���w�s�b�A�S���x�s");
			}	else if(iResult == -10) {
				UpdateHistoryMessage("�ק�K�X�X��!");
			}	else if(iResult == -11) {
				UpdateHistoryMessage("�ϥΪ̤��s�b���±K�X���۲šA����ק�K�X");
			}	else if(iResult == -20) {
				UpdateHistoryMessage("�s�W�{�ǥX��!");
			}	else if(iResult == -21) {
				UpdateHistoryMessage("�ϥΪ̤w�s�b�A����s�W");
			}	else if(iResult == -30) {
				UpdateHistoryMessage("�R���{�ǥX��!");
			}	else if(iResult == -31) {
				UpdateHistoryMessage("�ϥΪ̤��s�b�A����R��");
			}	else if(iResult == -40) {
				UpdateHistoryMessage("�S������ʧ@�A�Х��n�X");
			}	else if(iResult == -99) {
				UpdateHistoryMessage("�S���v��");
			} else {
				UpdateHistoryMessage((string)Application["accessErrorMsg"]);
			}
			//Page_Load(sender, e);
		} catch (Exception ex) {
			UpdateHistoryMessage((string)Application["accessErrorMsg"]);
		}
	}

	void UpdateHistoryMessage(string sMsg) {
		rtnMsg.Text = sMsg;
	}
</script>

<script language="JavaScript">
<%
if(sAction.Equals("M")) {	//JavaScript used in Modify
%>
function checkField() {
	if(UserManagerForm.newpasswd1.value == UserManagerForm.newpasswd2.value) {
		if(UserManagerForm.oldpasswd.value.length >= 4) {
			if(UserManagerForm.newpasswd1.value.length >= 4) {
				re = /\s{1,}/
				re_val = UserManagerForm.oldpasswd.value.search(re)
				if(re_val > -1) {
					alert('�±K�X�u����a-z�BA-Z�B0-9 �� _');
					UserManagerForm.oldpasswd.value = '';
					return false;
				}

				re = /\W{1,}/
				re_val = UserManagerForm.oldpasswd.value.search(re)
				if(re_val > -1) {
					alert('�±K�X�u����a-z�BA-Z�B0-9 �� _');
					UserManagerForm.oldpasswd.value = '';
					return false;
				}

				re = /\s{1,}/
				re_val = UserManagerForm.newpasswd1.value.search(re)
				if(re_val > -1) {
					alert('�s�K�X�u����a-z�BA-Z�B0-9 �� _');
					UserManagerForm.newpasswd1.value = '';
					UserManagerForm.newpasswd2.value = '';
					return false;
				}

				re = /\W{1,}/
				re_val = UserManagerForm.newpasswd1.value.search(re)
				if(re_val > -1) {
					alert('�s�K�X�u����a-z�BA-Z�B0-9 �� _');
					UserManagerForm.newpasswd1.value = '';
					UserManagerForm.newpasswd2.value = '';
					return false;
				}
			} else {
				alert('�s�K�X�̤֭n4�Ӧr');
				UserManagerForm.newpasswd1.value = '';
				UserManagerForm.newpasswd2.value = '';
				return false;
			}
		} else {
			alert('�±K�X�̤֭n4�Ӧr');
			UserManagerForm.oldpasswd.value = '';
			return false;
		}
	} else {
		alert('�s�K�X�P�T�{�K�X���۲šA�ЦA��J');
		UserManagerForm.newpasswd1.value = '';
		UserManagerForm.newpasswd2.value = '';
		return false;
	}
}
<%
} else if(sAction.Equals("A")) {	//JavaScript used in Add
%>
function checkField() {
	if(UserManagerForm.username.value.length >= 3) {
		if(UserManagerForm.oldpasswd.value.length >= 4) {
			re = /\s{1,}/
			re_val = UserManagerForm.username.value.search(re)
			if(re_val > -1) {
				alert('�W�٥u����a-z�BA-Z�B0-9 �� _');
				UserManagerForm.username.value = '';
				return false;
			}

			re = /\W{1,}/
			re_val = UserManagerForm.username.value.search(re)
			if(re_val > -1) {
				alert('�W�٥u����a-z�BA-Z�B0-9 �� _');
				UserManagerForm.username.value = '';
				return false;
			}

			re = /\s{1,}/
			re_val = UserManagerForm.oldpasswd.value.search(re)
			if(re_val > -1) {
				alert('�K�X�u����a-z�BA-Z�B0-9 �� _');
				UserManagerForm.oldpasswd.value = '';
				return false;
			}

			re = /\W{1,}/
			re_val = UserManagerForm.oldpasswd.value.search(re)
			if(re_val > -1) {
				alert('�K�X�u����a-z�BA-Z�B0-9 �� _');
				UserManagerForm.oldpasswd.value = '';
				return false;
			}

			chk = 0;
			<%
				for(int i = 0; i < iTotalGp; i++) {
			%>
				if(UserManagerForm.group[<%=i.ToString()%>].checked == true) {
					chk++;
				}
			<%
				}
			%>
			if(chk <= 0) {
				alert('�̤֭n��ܤ@�Ӹs��');
				UserManagerForm.group[0].checked = true;
				GpCheck();
				return false;
			}
		} else {
			alert('�K�X�̤֭n4�Ӧr');
			UserManagerForm.oldpasswd.value = '';
			return false;
		}
	} else {
		alert('�W�ٳ̤֭n3�Ӧr');
		UserManagerForm.username.value = '';
		return false;
	}
}

function GpCheck() {
	if(UserManagerForm.group[0].checked == true) {
	<%
		for(int i = 1; i < iTotalGp; i++) {
	%>
		UserManagerForm.group[<%=i.ToString()%>].disabled = true;
	<%
		}
	%>
	} else {
	<%
		for(int i = 1; i < iTotalGp; i++) {
	%>
		UserManagerForm.group[<%=i.ToString()%>].disabled = false;
	<%
		}
	%>
	}
}
<%
} else if(sAction.Equals("D")) {	//JavaScript used in Delete
%>
function checkField() {
	if(confirm("�T�w�R��?")) {
		return true;
	} else {
		return false;
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
	<title>��|��T - �ϥΪ̳]�w</title>
</head>
<body>
	<center>
	<form id="UserManagerForm" method="post" runat="server" ONSUBMIT="return checkField()">
		<asp:Label id="rtnMsg" runat="server" />
		<table width="50%" border="1">
			<span id="UserInformation" runat="server" />

			<tr align="right">
				<td colspan="2">
					<input type="submit" id="SaveBtn" value="�x�s" OnServerClick="UpdateUser" runat="server">
				</td>
			</tr>
		</table>
		</form>
	</center>
</body>
</html>