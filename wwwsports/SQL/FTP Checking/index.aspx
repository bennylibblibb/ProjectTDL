<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">

	void Page_Load(Object sender,EventArgs e) {
		string sRole = (string)Session["user_role"];
		welcomeLabel.Text = "Welcome! " + (string)Session["user_name"];

		switch (sRole) {
			case "999":
				adminTable.Rows[1].Cells[1].InnerHtml = "<a href=\"admin/adminmenuframe.htm\">���o���</a>";
				adminTable.Rows[1].Cells[2].InnerHtml = "<a href=\"admin/admintaskframe.htm\">���o/�R��<br>��T</a>";
				adminTable.Rows[1].Cells[3].InnerHtml = "<a href=\"admin/cfginiadminframe.htm\">�t�γ]�w</a>";
				adminTable.Rows[1].Cells[4].InnerHtml = "<a href=\"jcpager/jcpageradminframe.htm\">���|��<br>�]�w</a>";
				break;
			case "988":
				adminTable.Rows[1].Cells[1].InnerHtml = "<a href=\"admin/adminmenuframe.htm\">���o���</a>";
				adminTable.Rows[1].Cells[2].InnerHtml = "<a href=\"admin/admintaskframe.htm\">���o/�R��<br>��T</a>";
				adminTable.Rows[1].Cells[3].InnerHtml = "<a href=\"admin/cfginiadminframe.htm\">�t�γ]�w</a>";
				adminTable.Rows[1].Cells[4].InnerHtml = "<a href=\"jcpager/jcpageradminframe.htm\">���|��<br>�]�w</a>";
				break;
			case "011":
				adminTable.Rows[1].Cells[1].InnerHtml = "<a href=\"admin/admintaskframe.htm\">���o/�R��<br>��T</a>";
				adminTable.Rows[1].Cells[2].InnerHtml = "<a href=\"admin/cfginiadminframe.htm\">�t�γ]�w</a>";
				adminTable.Rows[1].Cells[3].InnerHtml = "<a href=\"jcpager/jcpageradminframe.htm\">���|��<br>�]�w</a>";
				break;
			default:
				break;
		}
	}

	void LogOut(Object sender,EventArgs e) {
		Authenticator auLogout = new Authenticator((string)Application["SoccerDBConnectionString"]);
		auLogout.Logout((string)Session["user_name"]);
		FormsAuthentication.SignOut();
		Response.Redirect("../sports/index.aspx");
	}
</script>

<html>
<head>
	<META http-equiv="Content-Type" content="text/html; charset=big5">
	<LINK REL="stylesheet" HREF="/sportStyle.css" TYPE="text/css">
	<title>��|��T - �D��</title>
</head>
<body>
	<form method="post" runat="server">
		<h1>��|��T - �D�� <asp:Label id="welcomeLabel" runat="server" /></h1>
		<table align="center" width="100%">
			<tr>
				<td>
					<a href="monitor/monitorframe.htm">�t�Ϊ��p</a>
				</td>
				<td>
					<a href="soccerschedule/scheduleframe.htm">���y�ɵ{</a>
				</td>
				<td>
					<a href="soccer/soccerframe.htm">���y��T</a>
				</td>
				<td>
					<a href="othersoccer/othersoccerframe.htm">���y2��T</a>
				</td>
				<td>
					<a href="otherregion/otherregionframe.htm">��L�a��</a>
				</td>
				<td>
					<a href="horseracing/HorseRacingFrame.htm">�ɰ���T</a>
				</td>
				<td>
					<a href="sportnews/SportNewsFrame.htm">��L��T</a>
				</td>
				<td>
					<a href="liveodds/liveoddsframe.htm">�{���߲v</a>
				</td>
				<td>
					<a href="basketball/basketballframe.htm">�x�y��T</a>
				</td>
				<td>
					<asp:Button id="logout" Text="�n�X" OnClick="LogOut" runat="server" />
				</td>
			</tr>
		</table>

		<hr>
		<table id="adminTable" align="center" width="100%" border="1" style="background-color:#FFF8DC" runat="server">
			<tr align="center">
				<th>�޲z����:</th>
				<td>
					<a href="general/generalframe.htm">�@��]�w</a>
				</td>
				<td>
					<a href="general/alertframe.htm">�Y�ɰT��</a>
				</td>
				<td>
					<a href="general/specialalertframe.htm">�S�w�T��</a>
				</td>
				<td>
					<a href="general/gogo1chartframe.htm">GOGO 1<br>���ƹϪ�</a>
				</td>
				<td>
					<a href="general/gogo2chartframe.htm">GOGO 2<br>���ƹϪ�</a>
				</td>
				<td>
					<a href="general/ftpcheckframe.htm">FTP �ˬd</a>
				</td>
				<td>
					<a href="admin/UserManagerFrame.htm">�ϥΪ̳]�w</a>
				</td>
			</tr>
			<tr align="center">
				<td></td>
				<td></td>
				<td></td>
				<td></td>
				<td></td>
				<td></td>
			</tr>
		</table>
	</form>
</body>
</html>