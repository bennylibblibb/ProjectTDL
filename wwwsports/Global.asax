<%@ Import Namespace="SportsItemHandler"%>
<%@ Import Namespace="SportsUtil"%>
<%@ Import Namespace="System.Collections"%>
<%@ Import Namespace="System.Collections.Specialized"%>
<%@ Import Namespace="System.Configuration"%>
<%@ Import Namespace="System.Globalization"%>

<script language="C#" runat="server">
	void Application_Start(Object sender, EventArgs E) {
		//Variables declaration
		int iIndex = 0;
		string sSoccerConn = "";
		string sBasketballConn = "";
		string sReplicateConn = "";
		string sJCScheduleConn = "";
		string sSpt2SocConn = "";
		string sJCSoccerConn = "";
		string sMDSoccerConn = "";
		string sSptAppCfgConn = "";
		string sHKJCNBAConn = "";
		string sHKJCSOCConn = "";
		string sJCCOMBOConn = "";
		string sEventPath = "";
		string sErrorPath = "";
		string sDescPath = "";
		string sJCDescPath = "";
		string sJCMenuPath = "";
		string sJCMatchLeakagePath = "";
		string sNbaINIPath = "";
		string sJCNbaINIPath = "";
		string sSoccerINIPath = "";
		string sWebServAppINIPath = "";
		string sIPMuxINIPath = "";
		string sGOGOMenuPath = "";
		string sMenuSection = "";
		string sXMLINIEnable = "";
		string sXMLConfigPath = "";
		string sBC2GOGOObjURI = "";
		string sBC2HKJCObjURI = "";
		string sIPMUXAdptBCStr = "";
		string sSportsParserFTPAddress = "";
		string sSportsParserFTPPort = "";
		string sSportsParserFTPMode = "";
		string sParserXMLPath = "";
		string sParserEnable = "";
		string sJCPrepSystemPath = "";
		string sComboNbaINIPath = "";
		ArrayList configSetting = new ArrayList();
		ArrayList iniItemsList = new ArrayList();
		string[] oddsItems;
		string[] lvoddsMatchItems;
		string[] matchItems;
		string[] goalItems;
		string[] weatherItems;
		string[] fieldItems;
		string[] commonMsgItems;
		string[] msgType;
		string[] positionItems;
		string[] adminActionArray;
		string[] QueueItemsArray;
		string[] RemotingItemsArray;
		string[] HorseRaceTrackArray;
		string[] HorseRaceClassArray;
		string[] HorseRaceStatusArray;
		string[] SOCScoreHandiArray;
		string[] NotifyMessageArray;
		string[] weekArray;
		string[] hdaStatusArray;
		string[] BSKAlertArray;
		string[] BSKMatchStatusArray;
		string[] BSKPositionArray;
		//byte[] ComboAlertHexArray;
		NameValueCollection songNVC;
		NameValueCollection statusNVC;
		NameValueCollection timeNVC;
		NameValueCollection HKJCadminNVC;
		DBEntity dbCollector;
		pathEntity pathCollector;
		songEntity songCollector;
		iniEntity iniCollector;
		matchStatusEntity matchStatusCollector;
		matchTimeEntity matchTimeCollector;
		HKJCTaskEntity adminTaskCollector;
		channelEntity channelCollector;
		BCStrEntity BCStrCollector;
		FTPEntity FTPCollector;
		//Load Connection String
		configSetting = (ArrayList) ConfigurationSettings.GetConfig("DBConfig");
		if(configSetting != null) {
			dbCollector = (DBEntity) configSetting[0];
			sSoccerConn = dbCollector.CONNSTR;
			dbCollector = (DBEntity) configSetting[1];
			sBasketballConn = dbCollector.CONNSTR;
			dbCollector = (DBEntity) configSetting[2];
			sReplicateConn = dbCollector.CONNSTR;
			dbCollector = (DBEntity) configSetting[3];
			sJCScheduleConn = dbCollector.CONNSTR;
			dbCollector = (DBEntity) configSetting[4];
			sSpt2SocConn = dbCollector.CONNSTR;
			dbCollector = (DBEntity) configSetting[5];
			sJCSoccerConn = dbCollector.CONNSTR;
			dbCollector = (DBEntity) configSetting[6];
			sMDSoccerConn = dbCollector.CONNSTR;
			dbCollector = (DBEntity) configSetting[7];
			sSptAppCfgConn = dbCollector.CONNSTR;
			dbCollector = (DBEntity) configSetting[8];
			sHKJCNBAConn = dbCollector.CONNSTR;
			dbCollector = (DBEntity) configSetting[9];
			sHKJCSOCConn = dbCollector.CONNSTR;
			dbCollector = (DBEntity) configSetting[10];
			sJCCOMBOConn = dbCollector.CONNSTR;
		}
		
		Application["SoccerDBConnectionString"] = sSoccerConn;
		Application["BasketballDBConnectionString"] = sBasketballConn;
		Application["RepDBConnectionString"] = sReplicateConn;
		Application["HKJCDBConnectionString"] = sJCScheduleConn;
		Application["GOGO2SOCDBConnectionString"] = sSpt2SocConn;
		Application["JCSOCCERDBConnectionString"] = sJCSoccerConn;
		Application["MSGDISPATCHERConnectionString"] = sMDSoccerConn;
		Application["SportAppCfgConnectionString"] = sSptAppCfgConn;
		Application["HKJCNBAConnectionString"] = sHKJCNBAConn;
		Application["HKJCSOCConnectionString"] = sHKJCSOCConn;
		Application["JCCOMBODBConnectionString"] = sJCCOMBOConn;
		configSetting.Clear();

		//Load filePath Tag
		configSetting = (ArrayList) ConfigurationSettings.GetConfig("filePath");
		if(configSetting != null) {
			pathCollector = (pathEntity) configSetting[0];
			sEventPath = pathCollector.PATH;
			pathCollector = (pathEntity) configSetting[1];
			sErrorPath = pathCollector.PATH;
			pathCollector = (pathEntity) configSetting[2];
			sDescPath = pathCollector.PATH;
			pathCollector = (pathEntity) configSetting[3];
			sSoccerINIPath = pathCollector.PATH;
			pathCollector = (pathEntity) configSetting[4];
			sWebServAppINIPath = pathCollector.PATH;
			pathCollector = (pathEntity) configSetting[5];
			sIPMuxINIPath = pathCollector.PATH;
			pathCollector = (pathEntity) configSetting[6];
			sGOGOMenuPath = pathCollector.PATH;
			pathCollector = (pathEntity) configSetting[7];
			sXMLConfigPath = pathCollector.PATH;
			sXMLINIEnable = pathCollector.TYPE;
			pathCollector = (pathEntity) configSetting[8];
			sJCDescPath = pathCollector.PATH;
			pathCollector = (pathEntity) configSetting[9];
			sJCMenuPath = pathCollector.PATH;
			pathCollector = (pathEntity) configSetting[10];
			sJCMatchLeakagePath = pathCollector.PATH;
			pathCollector = (pathEntity) configSetting[11];
			sNbaINIPath = pathCollector.PATH;
			pathCollector = (pathEntity) configSetting[12];
			sJCNbaINIPath = pathCollector.PATH;
			pathCollector = (pathEntity) configSetting[13];
			sParserXMLPath = pathCollector.PATH;
			sParserEnable = pathCollector.TYPE;
			pathCollector = (pathEntity) configSetting[14];
			sJCPrepSystemPath = pathCollector.PATH;
			pathCollector = (pathEntity) configSetting[15];
			sComboNbaINIPath = pathCollector.PATH;
		}
		Application["EventFilePath"] = sEventPath;
		Application["ErrorFilePath"] = sErrorPath;
		Application["DescFilePath"] = sDescPath;
		Application["SoccerINIFilePath"] = sSoccerINIPath;
		Application["WebServAppINIFilePath"] = sWebServAppINIPath;
		Application["IPMuxINIFilePath"] = sIPMuxINIPath;
		Application["GOGOMenuFilePath"] = sGOGOMenuPath;
		Application["INIEnableItem"] = sXMLINIEnable;
		Application["XMLConfigPath"] = sXMLConfigPath;
		Application["JCDescFilePath"] = sJCDescPath;
		Application["JCMenuFilePath"] = sJCMenuPath;
		Application["JCMatchLeakageFilePath"] = sJCMatchLeakagePath;
		Application["NbaINIFilePath"] = sNbaINIPath;
		Application["JCNbaINIFilePath"] = sJCNbaINIPath;
		Application["ParserXMLFilePath"] = sParserXMLPath;
		Application["ParserEnableItem"] = sParserEnable;
		Application["JCPrepSystemPathFilePath"] = sJCPrepSystemPath;
		Application["ComboNbaINIFilePath"] = sComboNbaINIPath;
		configSetting.Clear();

		//Load songType Tag
		configSetting = (ArrayList) ConfigurationSettings.GetConfig("songType");
		songNVC = new NameValueCollection();
		if(configSetting != null) {
			foreach(object o in configSetting) {
				songCollector = (songEntity) o;
				songNVC.Add(songCollector.ID,songCollector.NAME);
			}
		}
		Application["songItems"] = songNVC;
		configSetting.Clear();

		//Load messageType Tag
		configSetting = (ArrayList) ConfigurationSettings.GetConfig("messageType");
		msgType = new string[configSetting.Count];
		if(configSetting != null) {
			iIndex = 0;
			foreach(string s in configSetting) {
				msgType[iIndex] = s;
				iIndex++;
			}
		}
		Application["messageType"] = msgType;
		configSetting.Clear();

		//Load commonMessageItems Tag
		configSetting = (ArrayList) ConfigurationSettings.GetConfig("commonMessageItems");
		commonMsgItems = new string[configSetting.Count];
		if(configSetting != null) {
			iIndex = 0;
			foreach(string s in configSetting) {
				commonMsgItems[iIndex] = s;
				iIndex++;
			}
		}
		Application["sessionExpiredMsg"] = commonMsgItems[0];
		Application["accessErrorMsg"] = commonMsgItems[1];
		Application["retrieveInfoMsg"] = commonMsgItems[2];
		Application["transErrorMsg"] = commonMsgItems[3];
		configSetting.Clear();

		//Load oddsItems Tag
		configSetting = (ArrayList) ConfigurationSettings.GetConfig("oddsItems");
		oddsItems = new string[configSetting.Count];
		if(configSetting != null) {
			iIndex = 0;
			foreach(string s in configSetting) {
				oddsItems[iIndex] = s;
				iIndex++;
			}
		}
		Application["oddsItemsArray"] = oddsItems;
		configSetting.Clear();

		//Load lvoddsMatchItems Tag
		configSetting = (ArrayList) ConfigurationSettings.GetConfig("lvoddsMatchItems");
		lvoddsMatchItems = new string[configSetting.Count];
		if(configSetting != null) {
			iIndex = 0;
			foreach(string s in configSetting) {
				lvoddsMatchItems[iIndex] = s;
				iIndex++;
			}
		}
		Application["lvoddsMatchItemsArray"] = lvoddsMatchItems;
		configSetting.Clear();

		//Load matchItems Tag
		configSetting = (ArrayList) ConfigurationSettings.GetConfig("matchItems");
		matchItems = new string[configSetting.Count];
		if(configSetting != null) {
			iIndex = 0;
			foreach(string s in configSetting) {
				matchItems[iIndex] = s;
				iIndex++;
			}
		}
		Application["matchItemsArray"] = matchItems;
		configSetting.Clear();

		//Load goalItems Tag
		configSetting = (ArrayList) ConfigurationSettings.GetConfig("goalItems");
		goalItems = new string[configSetting.Count];
		if(configSetting != null) {
			iIndex = 0;
			foreach(string s in configSetting) {
				goalItems[iIndex] = s;
				iIndex++;
			}
		}
		Application["goalItemsArray"] = goalItems;
		configSetting.Clear();

		//Load weatherItems Tag
		configSetting = (ArrayList) ConfigurationSettings.GetConfig("weatherItems");
		weatherItems = new string[configSetting.Count];
		if(configSetting != null) {
			iIndex = 0;
			foreach(string s in configSetting) {
				weatherItems[iIndex] = s;
				iIndex++;
			}
		}
		Application["weatherItemsArray"] = weatherItems;
		configSetting.Clear();

		//Load fieldItems Tag
		configSetting = (ArrayList) ConfigurationSettings.GetConfig("fieldItems");
		fieldItems = new string[configSetting.Count];
		if(configSetting != null) {
			iIndex = 0;
			foreach(string s in configSetting) {
				fieldItems[iIndex] = s;
				iIndex++;
			}
		}
		Application["fieldItemsArray"] = fieldItems;
		configSetting.Clear();

		//Load INIFileInformation Tag
		configSetting = (ArrayList) ConfigurationSettings.GetConfig("INIFileInformation");
		if(configSetting != null) {
			iniCollector = (iniEntity) configSetting[0];
			sMenuSection = iniCollector.SECTION;
			if(iniCollector.KEYS != null) {
				string[] menuKeys = new string[iniCollector.KEYS.Count];
				iIndex = 0;
				foreach(string s in iniCollector.KEYS) {
					menuKeys[iIndex] = s;
					iIndex++;
				}
				Application["MenuKeys"] = menuKeys;
			}
		}
		Application["MenuSection"] = sMenuSection;
		configSetting.Clear();

		//Load positionItems Tag
		configSetting = (ArrayList) ConfigurationSettings.GetConfig("positionItems");
		positionItems = new string[configSetting.Count];
		if(configSetting != null) {
			iIndex = 0;
			foreach(string s in configSetting) {
				positionItems[iIndex] = s;
				iIndex++;
			}
		}
		Application["positionItemsArray"] = positionItems;
		configSetting.Clear();

		//Load adminActionItems Tag
		configSetting = (ArrayList) ConfigurationSettings.GetConfig("adminActionItems");
		adminActionArray = new string[configSetting.Count];
		if(configSetting != null) {
			iIndex = 0;
			foreach(string s in configSetting) {
				adminActionArray[iIndex] = s;
				iIndex++;
			}
		}
		Application["adminActionArray"] = adminActionArray;
		configSetting.Clear();

		//Load matchStatusType Tag
		configSetting = (ArrayList) ConfigurationSettings.GetConfig("matchStatusType");
		statusNVC = new NameValueCollection();
		if(configSetting != null) {
			foreach(object o in configSetting) {
				matchStatusCollector = (matchStatusEntity) o;
				statusNVC.Add(matchStatusCollector.ID,matchStatusCollector.NAME);
			}
		}
		Application["matchStatusItems"] = statusNVC;
		configSetting.Clear();

		//Load matchTimeType Tag
		configSetting = (ArrayList) ConfigurationSettings.GetConfig("matchTimeType");
		timeNVC = new NameValueCollection();
		if(configSetting != null) {
			foreach(object o in configSetting) {
				matchTimeCollector = (matchTimeEntity) o;
				timeNVC.Add(matchTimeCollector.ID,matchTimeCollector.NAME);
			}
		}
		Application["matchTimeItems"] = timeNVC;
		configSetting.Clear();

		//Load HKJCAdminTask Tag
		configSetting = (ArrayList) ConfigurationSettings.GetConfig("HKJCAdminTask");
		HKJCadminNVC = new NameValueCollection();
		if(configSetting != null) {
			foreach(object o in configSetting) {
				adminTaskCollector = (HKJCTaskEntity) o;
				HKJCadminNVC.Add(adminTaskCollector.NAME,adminTaskCollector.ID);
			}
		}
		Application["HKJCAdminTaskItems"] = HKJCadminNVC;
		configSetting.Clear();

		//Load QueueInfo Tag
		configSetting = (ArrayList)ConfigurationSettings.GetConfig("QueueInfo");
		QueueItemsArray = new string[configSetting.Count];
		if(configSetting != null) {
			iIndex = 0;
			foreach(string s in configSetting) {
				QueueItemsArray[iIndex] = s;
				iIndex++;
			}
		}
		Application["QueueItems"] = QueueItemsArray;
		configSetting.Clear();

		//Load RemotingInfo Tag
		configSetting = (ArrayList)ConfigurationSettings.GetConfig("RemotingInfo");
		RemotingItemsArray = new string[configSetting.Count];
		if(configSetting != null) {
			iIndex = 0;
			foreach(string s in configSetting) {
				RemotingItemsArray[iIndex] = s;
				iIndex++;
			}
		}
		Application["RemotingItems"] = RemotingItemsArray;
		configSetting.Clear();

		//Load HorseRaceTrack Tag
		configSetting = (ArrayList)ConfigurationSettings.GetConfig("HorseRaceTrack");
		HorseRaceTrackArray = new string[configSetting.Count];
		if(configSetting != null) {
			iIndex = 0;
			foreach(string s in configSetting) {
				HorseRaceTrackArray[iIndex] = s;
				iIndex++;
			}
		}
		Application["HorseRaceTracks"] = HorseRaceTrackArray;
		configSetting.Clear();

		//Load HorseRaceClass Tag
		configSetting = (ArrayList)ConfigurationSettings.GetConfig("HorseRaceClass");
		HorseRaceClassArray = new string[configSetting.Count];
		if(configSetting != null) {
			iIndex = 0;
			foreach(string s in configSetting) {
				HorseRaceClassArray[iIndex] = s;
				iIndex++;
			}
		}
		Application["HorseRaceClasses"] = HorseRaceClassArray;
		configSetting.Clear();

		//Load HorseRaceStatus Tag
		configSetting = (ArrayList)ConfigurationSettings.GetConfig("HorseRaceStatus");
		HorseRaceStatusArray = new string[configSetting.Count];
		if(configSetting != null) {
			iIndex = 0;
			foreach(string s in configSetting) {
				HorseRaceStatusArray[iIndex] = s;
				iIndex++;
			}
		}
		Application["HorseRaceStatuses"] = HorseRaceStatusArray;
		configSetting.Clear();

		//Load SOCScoreHandi Tag
		configSetting = (ArrayList) ConfigurationSettings.GetConfig("SOCScoreHandi");
		SOCScoreHandiArray = new string[configSetting.Count];
		if(configSetting != null) {
			iIndex = 0;
			foreach(string s in configSetting) {
				SOCScoreHandiArray[iIndex] = s;
				iIndex++;
			}
		}
		Application["SOCScoreHandiArray"] = SOCScoreHandiArray;
		configSetting.Clear();

		//Load NotifyMessageType Tag
		configSetting = (ArrayList)ConfigurationSettings.GetConfig("NotifyMessageType");
		NotifyMessageArray = new string[configSetting.Count];
		if(configSetting != null) {
			iIndex = 0;
			foreach(string s in configSetting) {
				NotifyMessageArray[iIndex] = s;
				iIndex++;
			}
		}
		Application["NotifyMessageTypes"] = NotifyMessageArray;
		configSetting.Clear();

		//Load WeekType Tag
		configSetting = (ArrayList)ConfigurationSettings.GetConfig("WeekType");
		weekArray = new string[configSetting.Count];
		if(configSetting != null) {
			iIndex = 0;
			foreach(string s in configSetting) {
				weekArray[iIndex] = s;
				iIndex++;
			}
		}
		Application["WeekItems"] = weekArray;
		configSetting.Clear();

		//Load HKJC_HKJCOddsStatus Tag
		configSetting = (ArrayList)ConfigurationSettings.GetConfig("HKJCOddsStatus");
		hdaStatusArray = new string[configSetting.Count];
		if(configSetting != null) {
			iIndex = 0;
			foreach(string s in configSetting) {
				hdaStatusArray[iIndex] = s;
				iIndex++;
			}
		}
		Application["HKJCOddsStatus"] = hdaStatusArray;
		configSetting.Clear();

		//Load BSKAlertItems Tag
		configSetting = (ArrayList) ConfigurationSettings.GetConfig("BSKAlertItems");
		BSKAlertArray = new string[configSetting.Count];
		if(configSetting != null) {
			iIndex = 0;
			foreach(string s in configSetting) {
				BSKAlertArray[iIndex] = s;
				iIndex++;
			}
		}
		Application["BSKAlertArray"] = BSKAlertArray;
		configSetting.Clear();

		//Load BSKMatchStatusItems Tag
		configSetting = (ArrayList) ConfigurationSettings.GetConfig("BSKMatchStatusItems");
		BSKMatchStatusArray = new string[configSetting.Count];
		if(configSetting != null) {
			iIndex = 0;
			foreach(string s in configSetting) {
				BSKMatchStatusArray[iIndex] = s;
				iIndex++;
			}
		}
		Application["BSKMatchStatusArray"] = BSKMatchStatusArray;
		configSetting.Clear();

		//Load BSKPositionItems Tag
		configSetting = (ArrayList) ConfigurationSettings.GetConfig("BSKPositionItems");
		BSKPositionArray = new string[configSetting.Count];
		if(configSetting != null) {
			iIndex = 0;
			foreach(string s in configSetting) {
				BSKPositionArray[iIndex] = s;
				iIndex++;
			}
		}
		Application["BSKPositionArray"] = BSKPositionArray;
		configSetting.Clear();

		//Load BroadcastChannel Tag
		configSetting = (ArrayList) ConfigurationSettings.GetConfig("BroadcastChannel");
		if(configSetting != null) {
			channelCollector = (channelEntity) configSetting[0];
			sBC2GOGOObjURI = channelCollector.URI;
			channelCollector = (channelEntity) configSetting[1];
			sBC2HKJCObjURI = channelCollector.URI;
		}
		Application["Broadcast2GOGOObjURI"] = sBC2GOGOObjURI;
		Application["Broadcast2HKJCObjURI"] = sBC2HKJCObjURI;
		configSetting.Clear();

		//Load BroadcastOption Tag
		configSetting = (ArrayList) ConfigurationSettings.GetConfig("BroadcastOption");
		if(configSetting != null) {
			BCStrCollector = (BCStrEntity) configSetting[0];
			sIPMUXAdptBCStr = BCStrCollector.MSG;
		}
		Application["IPMUXAdptBCStr"] = sIPMUXAdptBCStr;
		configSetting.Clear();

		/*
		//Load ComboAlertHexString Tag
		configSetting = (ArrayList) ConfigurationSettings.GetConfig("ComboAlertHexString");
		ComboAlertHexArray = new byte[configSetting.Count];
		if(configSetting != null) {
			iIndex = 0;
			foreach(string bstr in configSetting) {
				ComboAlertHexArray[iIndex] = byte.Parse(bstr,NumberStyles.HexNumber);
				iIndex++;
			}
		}
		Application["ComboAlertHexString"] = ComboAlertHexArray;
		configSetting.Clear();
		*/

		//Load FTPChannel Tag
		configSetting = (ArrayList) ConfigurationSettings.GetConfig("FTPChannel");
		if(configSetting != null) {
			FTPCollector = (FTPEntity) configSetting[0];
			sSportsParserFTPAddress = FTPCollector.Address;
			sSportsParserFTPPort = FTPCollector.Port;
			sSportsParserFTPMode = FTPCollector.Mode;

		}

		Application["SportsParserFTPAddress"] = sSportsParserFTPAddress;
		Application["SportsParserFTPPort"] = sSportsParserFTPPort;
		Application["SportsParserFTPMode"] = sSportsParserFTPMode;

		configSetting.Clear();
	}

	void Application_End(Object sender, EventArgs E) {
		// Clean up application resources here
		Application["SoccerDBConnectionString"] = null;
		Application["BasketballDBConnectionString"] = null;
		Application["RepDBConnectionString"] = null;
		Application["HKJCDBConnectionString"] = null;
		Application["GOGO2SOCDBConnectionString"] = null;
		Application["JCSOCCERDBConnectionString"] = null;
		Application["MSGDISPATCHERConnectionString"] = null;
		Application["SportAppCfgConnectionString"] = null;
		Application["HKJCNBAConnectionString"] = null;
		Application["HKJCSOCConnectionString"] = null;
		Application["JCCOMBODBConnectionString"] = null;
		Application["EventFilePath"] = null;
		Application["ErrorFilePath"] = null;
		Application["DescFilePath"] = null;
		Application["SoccerINIFilePath"] = null;
		Application["WebServAppINIFilePath"] = null;
		Application["IPMuxINIFilePath"] = null;
		Application["GOGOMenuFilePath"] = null;
		Application["NbaINIFilePath"] = null;
		Application["JCNbaINIFilePath"] = null;
		Application["oddsItemsArray"] = null;
		Application["lvoddsMatchItemsArray"] = null;
		Application["matchItemsArray"] = null;
		Application["goalItemsArray"] = null;
		Application["weatherItemsArray"] = null;
		Application["fieldItemsArray"] = null;
		Application["songItems"] = null;
		Application["MenuSection"] = null;
		Application["MenuKeys"] = null;
		Application["positionItemsArray"] = null;
		Application["adminActionArray"] = null;
		Application["matchStatusItems"] = null;
		Application["matchTimeItems"] = null;
		Application["HKJCAdminTaskItems"] = null;
		Application["INIEnableItem"] = null;
		Application["XMLConfigPath"] = null;
		Application["JCDescFilePath"] = null;
		Application["JCMenuFilePath"] = null;
		Application["JCMatchLeakageFilePath"] = null;
		Application["QueueItems"] = null;
		Application["RemotingItems"] = null;
		Application["HorseRaceTracks"] = null;
		Application["HorseRaceClasses"] = null;
		Application["HorseRaceStatuses"] = null;
		Application["SOCScoreHandiArray"] = null;
		Application["NotifyMessageTypes"] = null;
		Application["WeekItems"] = null;
		Application["HKJCOddsStatus"] = null;
		Application["BSKAlertArray"] = null;
		Application["BSKMatchStatusArray"] = null;
		Application["BSKPositionArray"] = null;
		Application["Broadcast2GOGOObjURI"] = null;
		Application["Broadcast2HKJCObjURI"] = null;
		Application["IPMUXAdptBCStr"] = null;
		//Application["ComboAlertString"] = null;
		Application["SportsParserFTPAddress"] = null;
		Application["SportsParserFTPPort"] = null;
		Application["SportsParserFTPMode"] = null;
		Application["ParserXMLFilePath"] = null;		
		Application["ParserEnableItem"] = null;
		Application["JCPrepSystemPathFilePath"] = null;
		Application["ComboNbaINIFilePath"] = null;
	}
</script>
