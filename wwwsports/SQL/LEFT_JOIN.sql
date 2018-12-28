Remark: Big/Small Odds
SELECT bs.IMATCH_CNT, bs.IORDER_NO, game.MATCH_CNT, game.LEAGUE, game.HOST, game.GUEST, game.MATCHDATE, game.MATCHTIME, bs.CSCORE, bs.CBIGODDS, bs.CSMALLODDS, bs.CSTATUS 
FROM GAMEINFO game 
INNER JOIN LEAGINFO leag ON game.LEAGUE = leag.alias and leag.leag_id in (select cleag_id from userprofile_info where iuser_id=1) 
LEFT OUTER JOIN BIGSMALLODDS_INFO bs ON bs.IMATCH_CNT = game.MATCH_CNT 
ORDER BY bs.IORDER_NO, leag.leag_order, game.MATCHDATE, game.MATCHTIME

Remark: Rank
SELECT leag.ALIAS, rank.RANK, team.TEAMNAME, rank.GAMES, rank.SCORE 
FROM LEAGINFO leag 
LEFT OUTER JOIN TEAMINFO team ON team.TEAM_ID in (select TEAM_ID from ID_INFO where LEAG_ID='001') 
LEFT OUTER JOIN LEAGRANKINFO rank ON rank.LEAG_ID=leag.LEAG_ID and rank.LEAG_ID='001' and rank.TEAM=team.TEAMNAME 
ORDER BY rank.RANK