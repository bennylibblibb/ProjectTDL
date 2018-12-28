<%@ Page EnableViewState="false" codepage="950"%>

<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">
	string sLeagID;

	void Page_Load(Object sender,EventArgs e) {
		sLeagID = (string)Request.QueryString["leagID"];
	}
</script>

<html>
<head>
	<META http-equiv="Content-Type" content="text/html; charset=big5">
	<LINK REL="stylesheet" HREF="/sportStyle.css" TYPE="text/css">
	<title>體育資訊 - 聯賽修改</title>
</head>
<frameset border="0" rows="60%,40%">
	<frame name="leagmod_frame" src="LeagueManager_MOD.aspx?leagID=<%=sLeagID%>" noresize>
	<frame name="teamlink_frame" src="LeagueManager_ListTeam.aspx?leagID=<%=sLeagID%>" noresize>
	</frameset>
	<noframes>
		This information on this page is displayed in frames.
		Your browser can't view frames. Please check Perferences in IE setting.
	</noframes>
</frameset>
</html>