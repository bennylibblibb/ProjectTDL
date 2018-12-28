case 'B': pstrSQL->Add("delete from ANALYSIS_REMARK_INFO where IMATCH_CNT=(select match_cnt from gameinfo where leaglong='"+leag+"' and host='"+host+"' and guest='"+guest+"')");
pstrSQL->Add("update PLAYERS_INFO set IROSTER=0 where CTEAM_ID=(select TEAM_ID from TEAMINFO where TEAMNAME='"+host+"')");
pstrSQL->Add("update PLAYERS_INFO set IROSTER=0 where CTEAM_ID=(select TEAM_ID from TEAMINFO where TEAMNAME='"+guest+"')");
pstrSQL->Add("delete from ANALYSIS_HISTORY_INFO where IMATCH_CNT=(select match_cnt from gameinfo where leaglong='"+leag+"' and host='"+host+"' and guest='"+guest+"')");
pstrSQL->Add("delete from ANALYSIS_BG_INFO where IMATCH_CNT=(select match_cnt from gameinfo where leaglong='"+leag+"' and host='"+host+"' and guest='"+guest+"')");
break;
case 'H': pstrSQL->Add("delete from ANALYSIS_HISTORY_INFO where IMATCH_CNT=(select match_cnt from gameinfo where leaglong='"+leag+"' and host='"+host+"' and guest='"+guest+"')");
break;
case 'P': pstrSQL->Add("update PLAYERS_INFO set IROSTER=0 where CTEAM_ID=(select TEAM_ID from TEAMINFO where TEAMNAME='"+host+"')");
pstrSQL->Add("update PLAYERS_INFO set IROSTER=0 where CTEAM_ID=(select TEAM_ID from TEAMINFO where TEAMNAME='"+guest+"')");
break;
case 'R': pstrSQL->Add("delete from ANALYSIS_REMARK_INFO where IMATCH_CNT=(select match_cnt from gameinfo where leaglong='"+leag+"' and host='"+host+"' and guest='"+guest+"')");
break;

Remark: BigSmallOdds
pstrSQL->Add("delete from BIGSMALLODDS_INFO where IMATCH_CNT=(select match_cnt from gameinfo where leaglong='"+leag+"' and host='"+host+"' and guest='"+guest+"')");