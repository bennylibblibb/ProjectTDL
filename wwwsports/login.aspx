<%@ Page EnableViewState="false"%>

<%@ Import Namespace="System.Web.Security"%>
<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">
	void OnLogIn (Object sender, EventArgs e) {
		int iAccessCode = CustomAuthenticate(UserName.Value,passwd.Value);
		if(iAccessCode > 0) {
			FormsAuthentication.SetAuthCookie(UserName.Value,false);
			FormsAuthentication.RedirectFromLoginPage(UserName.Value,false);
		}	else {
			if(iAccessCode == -1) {
				rtnMsg.Text = "登入失敗：名稱和密碼不符合，或使用者不存在。";
			} else if(iAccessCode == -2) {
				rtnMsg.Text = "登入失敗：未到登入時間。";
			} else if(iAccessCode == -3) {
				rtnMsg.Text = "登入失敗：已過登入時間。";
			} else if(iAccessCode == -4) {
				rtnMsg.Text = "登入失敗：登入時間已過期。";
			} else if(iAccessCode == -5) {
				rtnMsg.Text = "登入失敗：沒有權限。";
			} else if(iAccessCode == -99) {
				rtnMsg.Text = "登入失敗：系統有誤。";
			}	else {
				rtnMsg.Text = "登入失敗：原因不明。";
			}
		}
	}

	int CustomAuthenticate(string username, string password) {
		string[] arrUserInfo;
		Authenticator au = new Authenticator((string)Application["SoccerDBConnectionString"]);
		try {
			arrUserInfo = au.UserAuthenticate(username,password);
			if(au.AccessCode > 0) {
				Session["user_id"] = arrUserInfo[0];
				Session["user_role"] = arrUserInfo[1];
				Session["user_name"] = username;
				Session["user_sortType"] = "0";
			}

			return au.AccessCode;
    } catch(Exception) {
	    return -99;
    }
  }
</script>

<html>
<head>
	<META http-equiv="Content-Type" content="text/html; charset=big5">
	<LINK REL="stylesheet" HREF="sportStyle.css" TYPE="text/css">
	<title>體育資訊 - 登入</title>
</head>
<body OnLoad="document.forms[0].UserName.focus()">
	<br><br><br><br><br><br>
	<center>
	<form method="post" runat="server">
		<table background="img/sports-info.jpg" height="264" width="439">
			<tr height="90">
				<td>
				</td>
			</tr>
			<tr height="35">
				<th align="right">名稱:</th>
				<td><input type="text" id="UserName" maxlength="6" runat="server"></td>
				<asp:RequiredFieldValidator
					id="nameVal"
					ControlToValidate="UserName" 
					InitialValue=""
					ErrorMessage="名稱不能空白"
					runat="server" />
			</tr>
			<tr height="30">
				<th  align="right">密碼:</th>
				<td><input type="password" id="passwd" maxlength="20" runat="server"></td>
				<asp:RequiredFieldValidator
					id="pwdVal"
					ControlToValidate="passwd" 
					InitialValue=""
					ErrorMessage="密碼不能空白"
					runat="server" />
			</tr>
			<tr align="center" height="30">
				<td colspan=2>
					<input type=submit value="登入" OnServerClick="OnLogIn" runat="server">
					<input type=reset value="重設">
				</td>
			</tr>
			<tr height="79">
				<td>
				</td>
			</tr>
		</table>
		<asp:Label id="rtnMsg" runat="server" />
		</form>
	</center>
</body>
</html>