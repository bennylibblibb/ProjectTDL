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
					UpdateHistoryMessage((string)Application["retrieveInfoMsg"] + "新增使用者(" + DateTime.Now.ToString("HH:mm:ss") + ")");
				} else {
					UpdateHistoryMessage("沒有權限(" + DateTime.Now.ToString("HH:mm:ss") + ")");
				}
			} else if(sAction.Trim().Equals("D")) {
				if(Convert.ToInt32(sRole) >= 999) {
					UserInformation.InnerHtml = user.ShowForm(sAction);
					UpdateHistoryMessage((string)Application["retrieveInfoMsg"] + "刪除使用者(" + DateTime.Now.ToString("HH:mm:ss") + ")");
				} else {
					UpdateHistoryMessage("沒有權限(" + DateTime.Now.ToString("HH:mm:ss") + ")");
				}
			} else {
				UserInformation.InnerHtml = user.ShowForm("M");
				UpdateHistoryMessage((string)Application["retrieveInfoMsg"] + "修改密碼(" + DateTime.Now.ToString("HH:mm:ss") + ")");
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
				UpdateHistoryMessage("儲存成功");
			}	else if(iResult == 0) {
				UpdateHistoryMessage("相同記錄已存在，沒有儲存");
			}	else if(iResult == -10) {
				UpdateHistoryMessage("修改密碼出錯!");
			}	else if(iResult == -11) {
				UpdateHistoryMessage("使用者不存在或舊密碼不相符，不能修改密碼");
			}	else if(iResult == -20) {
				UpdateHistoryMessage("新增程序出錯!");
			}	else if(iResult == -21) {
				UpdateHistoryMessage("使用者已存在，不能新增");
			}	else if(iResult == -30) {
				UpdateHistoryMessage("刪除程序出錯!");
			}	else if(iResult == -31) {
				UpdateHistoryMessage("使用者不存在，不能刪除");
			}	else if(iResult == -40) {
				UpdateHistoryMessage("沒有執行動作，請先登出");
			}	else if(iResult == -99) {
				UpdateHistoryMessage("沒有權限");
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
					alert('舊密碼只接受a-z、A-Z、0-9 及 _');
					UserManagerForm.oldpasswd.value = '';
					return false;
				}

				re = /\W{1,}/
				re_val = UserManagerForm.oldpasswd.value.search(re)
				if(re_val > -1) {
					alert('舊密碼只接受a-z、A-Z、0-9 及 _');
					UserManagerForm.oldpasswd.value = '';
					return false;
				}

				re = /\s{1,}/
				re_val = UserManagerForm.newpasswd1.value.search(re)
				if(re_val > -1) {
					alert('新密碼只接受a-z、A-Z、0-9 及 _');
					UserManagerForm.newpasswd1.value = '';
					UserManagerForm.newpasswd2.value = '';
					return false;
				}

				re = /\W{1,}/
				re_val = UserManagerForm.newpasswd1.value.search(re)
				if(re_val > -1) {
					alert('新密碼只接受a-z、A-Z、0-9 及 _');
					UserManagerForm.newpasswd1.value = '';
					UserManagerForm.newpasswd2.value = '';
					return false;
				}
			} else {
				alert('新密碼最少要4個字');
				UserManagerForm.newpasswd1.value = '';
				UserManagerForm.newpasswd2.value = '';
				return false;
			}
		} else {
			alert('舊密碼最少要4個字');
			UserManagerForm.oldpasswd.value = '';
			return false;
		}
	} else {
		alert('新密碼與確認密碼不相符，請再輸入');
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
				alert('名稱只接受a-z、A-Z、0-9 及 _');
				UserManagerForm.username.value = '';
				return false;
			}

			re = /\W{1,}/
			re_val = UserManagerForm.username.value.search(re)
			if(re_val > -1) {
				alert('名稱只接受a-z、A-Z、0-9 及 _');
				UserManagerForm.username.value = '';
				return false;
			}

			re = /\s{1,}/
			re_val = UserManagerForm.oldpasswd.value.search(re)
			if(re_val > -1) {
				alert('密碼只接受a-z、A-Z、0-9 及 _');
				UserManagerForm.oldpasswd.value = '';
				return false;
			}

			re = /\W{1,}/
			re_val = UserManagerForm.oldpasswd.value.search(re)
			if(re_val > -1) {
				alert('密碼只接受a-z、A-Z、0-9 及 _');
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
				alert('最少要選擇一個群組');
				UserManagerForm.group[0].checked = true;
				GpCheck();
				return false;
			}
		} else {
			alert('密碼最少要4個字');
			UserManagerForm.oldpasswd.value = '';
			return false;
		}
	} else {
		alert('名稱最少要3個字');
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
	if(confirm("確定刪除?")) {
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
	<title>體育資訊 - 使用者設定</title>
</head>
<body>
	<center>
	<form id="UserManagerForm" method="post" runat="server" ONSUBMIT="return checkField()">
		<asp:Label id="rtnMsg" runat="server" />
		<table width="50%" border="1">
			<span id="UserInformation" runat="server" />

			<tr align="right">
				<td colspan="2">
					<input type="submit" id="SaveBtn" value="儲存" OnServerClick="UpdateUser" runat="server">
				</td>
			</tr>
		</table>
		</form>
	</center>
</body>
</html>