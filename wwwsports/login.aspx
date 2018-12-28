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
				rtnMsg.Text = "�n�J���ѡG�W�٩M�K�X���ŦX�A�ΨϥΪ̤��s�b�C";
			} else if(iAccessCode == -2) {
				rtnMsg.Text = "�n�J���ѡG����n�J�ɶ��C";
			} else if(iAccessCode == -3) {
				rtnMsg.Text = "�n�J���ѡG�w�L�n�J�ɶ��C";
			} else if(iAccessCode == -4) {
				rtnMsg.Text = "�n�J���ѡG�n�J�ɶ��w�L���C";
			} else if(iAccessCode == -5) {
				rtnMsg.Text = "�n�J���ѡG�S���v���C";
			} else if(iAccessCode == -99) {
				rtnMsg.Text = "�n�J���ѡG�t�Φ��~�C";
			}	else {
				rtnMsg.Text = "�n�J���ѡG��]�����C";
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
	<title>��|��T - �n�J</title>
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
				<th align="right">�W��:</th>
				<td><input type="text" id="UserName" maxlength="6" runat="server"></td>
				<asp:RequiredFieldValidator
					id="nameVal"
					ControlToValidate="UserName" 
					InitialValue=""
					ErrorMessage="�W�٤���ť�"
					runat="server" />
			</tr>
			<tr height="30">
				<th  align="right">�K�X:</th>
				<td><input type="password" id="passwd" maxlength="20" runat="server"></td>
				<asp:RequiredFieldValidator
					id="pwdVal"
					ControlToValidate="passwd" 
					InitialValue=""
					ErrorMessage="�K�X����ť�"
					runat="server" />
			</tr>
			<tr align="center" height="30">
				<td colspan=2>
					<input type=submit value="�n�J" OnServerClick="OnLogIn" runat="server">
					<input type=reset value="���]">
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