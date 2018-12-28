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
					SportsParserInformation.InnerHtml	= "系統錯誤";
				if(sFlag.Equals("-100"))
					SportsParserInformation.InnerHtml	= "沒有檢視權限";	
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
					rtnMsg.Text = "<b>成功啟動設定</b>";		
					
					
					
				if(sFlag.Equals("0"))				
					rtnMsg.Text = "<b>成功暫停設定</b>";
					
			}else{
				rtnMsg.Text = "沒有更新設定";
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
	<title>體育資訊 - 系統設定</title>
</head>
<body>
	<form id="SportsParserConfigForm" method="post" runat="server">
		<h3>系統設定</h3>
		<asp:Label id="rtnMsg" runat="server" />
		
		<span id="AlertMessage" runat="server" />
		<table border="1" width="30%">
			<tr>
				<th align="right">允許FTP發送數據:</th>
				<td>
					<span id="SportsParserInformation" runat="server" />						
			    
				</td>
			</tr>
			<tr align="right">
				<td>
				</td>
				<td>
					<input type="button" id="SaveBtn" value="儲存" OnServerClick="ModifySportsParserCFGAction" runat="server">&nbsp;
					<input type="reset" value="重設">
				</td>
				
			</tr>
		</table>
		<br><font size="2">(更改系統設定將有1-2秒延誤, 請稍後重新整理網頁確定儲存值)</font>	
	</form>
</body>
</html>