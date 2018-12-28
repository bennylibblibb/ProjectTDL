<%@ Page EnableViewState="false" codepage="950"%>

<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">
	string sTarget;

	void Page_Load(Object sender,EventArgs e) {
		sTarget = (string)Request.QueryString["tar"];
	}
</script>

<html>
<head>
	<META http-equiv="Content-Type" content="text/html; charset=big5">
	<LINK REL="stylesheet" HREF="/sportStyle.css" TYPE="text/css">
	<title>體育資訊 - 足球 - 修改賽果</title>
</head>
<frameset border="0" rows="18%,*">
	<frame name="submenu_frame" src="ModifyResultMenu.aspx?target=<%=sTarget%>" noresize>
	<frame name="subcontent_frame" src="..\blank.htm" noresize>
	</frameset>
	<noframes>
		This information on this page is displayed in frames.
		Your browser can't view frames. Please check Perferences in IE setting.
	</noframes>
</frameset>
</html>