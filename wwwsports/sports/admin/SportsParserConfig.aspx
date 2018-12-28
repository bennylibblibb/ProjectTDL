<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">

	
	void Page_Load(Object sender,EventArgs e) {
		string sFlag;
		
		try {		
			SportsParserConfig SportsParsercfg = new SportsParserConfig();
			sFlag = SportsParsercfg.GetXMLConfig((String)Application["ParserXMLFilePath"],(String)Application["ParserEnableItem"]);													
			
			if(sFlag.Equals("1")){
				sFlag = "<input type=\"checkbox\" name=\"SportsParserEnable\" value=\"1\" checked>";
		  }else if(sFlag.Equals("0")){
				sFlag = "<input type=\"checkbox\" name=\"SportsParserEnable\" value=\"1\">";									
			}
			
			SportsParserInformation.InnerHtml = sFlag;
			
			if(!sFlag.Equals("-99"))
				SportsParserInformation.InnerHtml = sFlag;
			else{
				if(sFlag.Equals("-99"))
					SportsParserInformation.InnerHtml	= "�t�ο��~";
				if(sFlag.Equals("-100"))
					SportsParserInformation.InnerHtml	= "�S���˵��v��";	
			}		
					
		} catch(NullReferenceException) {
			FormsAuthentication.SignOut();
			rtnMsg.Text = "<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>";
		}
		
	}

	void ModifySportsParserCFGAction(Object sender,EventArgs e) {
		string sFlag;
		SportsParserConfig SportsParserupd = new SportsParserConfig();
		try {
			
			sFlag = SportsParserupd.SetXMLConfig((String)Application["ParserXMLFilePath"],(String)Application["ParserEnableItem"]);			
			Page_Load(sender,e);	
			
			if(!sFlag.Equals("-99")){
				if(sFlag.Equals("1"))				
					rtnMsg.Text = "<b>���\�Ұʳ]�w</b>";		
					
					
					
				if(sFlag.Equals("0"))				
					rtnMsg.Text = "<b>���\�Ȱ��]�w</b>";
					
			}else{
				rtnMsg.Text = "�S����s�]�w";
			}
						
			//SportsParserConfigForm.SportsParserEnable.Checked=true;					
			
			
		}	catch(NullReferenceException) {
			FormsAuthentication.SignOut();
			rtnMsg.Text = "<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>";
		}
	}
</script>

<html>
<head>
	<META http-equiv="Content-Type" content="text/html; charset=big5">
	<LINK REL="stylesheet" HREF="/sportStyle.css" TYPE="text/css">
	<title>��|��T - �t�γ]�w</title>
</head>
<body>
	<form id="SportsParserConfigForm" method="post" runat="server">
		<h3>�t�γ]�w</h3>
		<asp:Label id="rtnMsg" runat="server" />
		
		<span id="AlertMessage" runat="server" />
		<table border="1" width="30%">
			<tr>
				<th align="right">���\FTP�o�e�ƾ�:</th>
				<td>
					<span id="SportsParserInformation" runat="server" />						
			    
				</td>
			</tr>
			<tr align="right">
				<td>
				</td>
				<td>
					<input type="button" id="SaveBtn" value="�x�s" OnServerClick="ModifySportsParserCFGAction" runat="server">&nbsp;
					<input type="reset" value="���]">
				</td>
				
			</tr>
		</table>
		<br><font size="2">(���t�γ]�w�N��1-2���~, �еy�᭫�s��z�����T�w�x�s��)</font>	
	</form>
</body>
</html>