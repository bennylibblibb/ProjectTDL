<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">

	void Page_Load(Object sender,EventArgs e) {
		string sRole = (string)Session["user_role"];
		welcomeLabel.Text = "Welcome! " + (string)Session["user_name"];

		switch (sRole) {
			case "999":
				adminTable.Rows[1].Cells[1].InnerHtml = "<a href=\"admin/adminmenuframe.htm\">重發菜單</a>";
				adminTable.Rows[1].Cells[2].InnerHtml = "<a href=\"admin/admintaskframe.htm\">重發/刪除<br>資訊</a>";
				adminTable.Rows[1].Cells[3].InnerHtml = "<a href=\"admin/cfginiadminframe.htm\">系統設定</a>";
				adminTable.Rows[1].Cells[4].InnerHtml = "<a href=\"jcpager/jcpageradminframe.htm\">馬會機<br>設定</a>";
				break;
			case "988":
				adminTable.Rows[1].Cells[1].InnerHtml = "<a href=\"admin/adminmenuframe.htm\">重發菜單</a>";
				adminTable.Rows[1].Cells[2].InnerHtml = "<a href=\"admin/admintaskframe.htm\">重發/刪除<br>資訊</a>";
				adminTable.Rows[1].Cells[3].InnerHtml = "<a href=\"admin/cfginiadminframe.htm\">系統設定</a>";
				adminTable.Rows[1].Cells[4].InnerHtml = "<a href=\"jcpager/jcpageradminframe.htm\">馬會機<br>設定</a>";
				break;
			case "011":
				adminTable.Rows[1].Cells[1].InnerHtml = "<a href=\"admin/admintaskframe.htm\">重發/刪除<br>資訊</a>";
				adminTable.Rows[1].Cells[2].InnerHtml = "<a href=\"admin/cfginiadminframe.htm\">系統設定</a>";
				adminTable.Rows[1].Cells[3].InnerHtml = "<a href=\"jcpager/jcpageradminframe.htm\">馬會機<br>設定</a>";
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
	<title>體育資訊 - 主頁</title>
</head>
<body>
	<form method="post" runat="server">
		<h1>體育資訊 - 主頁 <asp:Label id="welcomeLabel" runat="server" /></h1>
		<table align="center" width="100%">
			<tr>
				<td>
					<a href="monitor/monitorframe.htm">系統狀況</a>
				</td>
				<td>
					<a href="soccerschedule/scheduleframe.htm">足球賽程</a>
				</td>
				<td>
					<a href="soccer/soccerframe.htm">足球資訊</a>
				</td>
				<td>
					<a href="othersoccer/othersoccerframe.htm">足球2資訊</a>
				</td>
				<td>
					<a href="otherregion/otherregionframe.htm">其他地區</a>
				</td>
				<td>
					<a href="horseracing/HorseRacingFrame.htm">賽馬資訊</a>
				</td>
				<td>
					<a href="sportnews/SportNewsFrame.htm">其他資訊</a>
				</td>
				<td>
					<a href="liveodds/liveoddsframe.htm">現場賠率</a>
				</td>
				<td>
					<a href="basketball/basketballframe.htm">籃球資訊</a>
				</td>
				<td>
					<asp:Button id="logout" Text="登出" OnClick="LogOut" runat="server" />
				</td>
			</tr>
		</table>

		<hr>
		<table id="adminTable" align="center" width="100%" border="1" style="background-color:#FFF8DC" runat="server">
			<tr align="center">
				<th>管理項目:</th>
				<td>
					<a href="general/generalframe.htm">一般設定</a>
				</td>
				<td>
					<a href="general/alertframe.htm">即時訊息</a>
				</td>
				<td>
					<a href="general/specialalertframe.htm">特定訊息</a>
				</td>
				<td>
					<a href="general/gogo1chartframe.htm">GOGO 1<br>指數圖表</a>
				</td>
				<td>
					<a href="general/gogo2chartframe.htm">GOGO 2<br>指數圖表</a>
				</td>
				<td>
					<a href="general/ftpcheckframe.htm">FTP 檢查</a>
				</td>
				<td>
					<a href="admin/UserManagerFrame.htm">使用者設定</a>
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