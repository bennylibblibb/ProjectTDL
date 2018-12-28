Const ForReading = 1, ForWriting = 2, ForAppending = 8

'Dim all variables
Dim conn, rs, yyyy, mm, dd, hh, nn, currentdate, currenttime, sSQL

'Open Connection to database
Set conn = CreateObject("adodb.connection")
'Open Recordset to hold data
set rs = CreateObject("adodb.recordset")

yyyy = Year(Date)
mm = Month(Date)
If mm < 10 Then
	mm = "0" & mm
End If
dd = Day(Date)
If dd < 10 Then
	dd = "0" & dd
End If
hh = Hour(Time)
If hh < 10 Then
	hh = "0" & hh
End If
nn = Minute(Time)
If nn < 10 Then
	nn = "0" & nn
End If
currentdate = yyyy & mm & dd
currenttime = hh & nn & ss
'connect to D:\SPORTS\DB\ASIA_SOCCER_DB.GDB@localhost
conn.ConnectionString = "Provider=IbOleDb;Location=127.0.0.1;Data Source=D:/Sports/db/ASIA_SOCCER_DB.GDB;User ID=sysdba;Password=masterkey;Min Pool Size=1;Max Pool Size=5"
conn.Open
'purge table record up to the latest one hour before
rs.Open "delete from LOG_ADMINTASK where timeflag <= CURRENT_TIMESTAMP-0.0417", conn
rs.Open "delete from LOG_ALERTMSG where timeflag <= CURRENT_TIMESTAMP-0.0417", conn
rs.Open "delete from LOG_ALLODDS where timeflag <= CURRENT_TIMESTAMP-0.0417", conn
rs.Open "delete from LOG_ANALYSISBG where timeflag <= CURRENT_TIMESTAMP-0.0417", conn
rs.Open "delete from LOG_ANALYSISHISTORY where timeflag <= CURRENT_TIMESTAMP-0.0417", conn
rs.Open "delete from LOG_ANALYSISPLAYERS where timeflag <= CURRENT_TIMESTAMP-0.0417", conn
rs.Open "delete from LOG_ANALYSISRECENT where timeflag <= CURRENT_TIMESTAMP-0.0417", conn
rs.Open "delete from LOG_ANALYSISREMARKS where timeflag <= CURRENT_TIMESTAMP-0.0417", conn
rs.Open "delete from LOG_ANALYSISSTAT where timeflag <= CURRENT_TIMESTAMP-0.0417", conn
rs.Open "delete from LOG_BIGSMALLODDS where timeflag <= CURRENT_TIMESTAMP-0.0417", conn
rs.Open "delete from LOG_BLANKMESSAGE where timeflag <= CURRENT_TIMESTAMP-0.0417", conn
rs.Open "delete from LOG_CHARTRESEND where timeflag <= CURRENT_TIMESTAMP-0.0417", conn
rs.Open "delete from LOG_CORRECTSCORE where timeflag <= CURRENT_TIMESTAMP-0.0417", conn
rs.Open "delete from LOG_GOALDETAILS where timeflag <= CURRENT_TIMESTAMP-0.0417", conn
rs.Open "delete from LOG_LIVEGOAL where timeflag <= CURRENT_TIMESTAMP-0.0417", conn
rs.Open "delete from LOG_LIVEODDS where timeflag <= CURRENT_TIMESTAMP-0.0417", conn
rs.Open "delete from LOG_LIVEPLACE where timeflag <= CURRENT_TIMESTAMP-0.0417", conn
rs.Open "delete from LOG_MATCHLIST where timeflag <= CURRENT_TIMESTAMP-0.0417", conn
rs.Open "delete from LOG_MENU where timeflag <= CURRENT_TIMESTAMP-0.0417", conn
rs.Open "delete from LOG_MODIFYMATCH where timeflag <= CURRENT_TIMESTAMP-0.0417", conn
rs.Open "delete from LOG_OTHERGOAL where timeflag <= CURRENT_TIMESTAMP-0.0417", conn
rs.Open "delete from LOG_OTHERODDS where timeflag <= CURRENT_TIMESTAMP-0.0417", conn
rs.Open "delete from LOG_OTHERODDSMATCHMODIFY where timeflag <= CURRENT_TIMESTAMP-0.0417", conn
rs.Open "delete from LOG_RANK where timeflag <= CURRENT_TIMESTAMP-0.0417", conn
rs.Open "delete from LOG_REPORT where timeflag <= CURRENT_TIMESTAMP-0.0417", conn
rs.Open "delete from LOG_SCORERS where timeflag <= CURRENT_TIMESTAMP-0.0417", conn
rs.Open "delete from LOG_SPORTNEWS where timeflag <= CURRENT_TIMESTAMP-0.0417", conn
rs.Open "delete from LOG_OTHERSOCCER where timeflag <= CURRENT_TIMESTAMP-0.0417", conn
rs.Open "delete from LOG_OTHERSOCCER_MODIFYMATCH where timeflag <= CURRENT_TIMESTAMP-0.0417", conn
rs.Open "delete from LOG_OTHERSOCCER_LIVEGOAL where timeflag <= CURRENT_TIMESTAMP-0.0417", conn
rs.Open "SET GENERATOR LOG_ADMIN_SEQ_GEN TO 0", conn
rs.Open "SET GENERATOR LOG_ALTMSG_SEQ_GEN TO 0", conn
rs.Open "SET GENERATOR LOG_ALLODDS_SEQ_GEN TO 0", conn
rs.Open "SET GENERATOR LOG_ANLYBG_GEN TO 0", conn
rs.Open "SET GENERATOR LOG_ANLYHIS_SEQ_GEN TO 0", conn
rs.Open "SET GENERATOR LOG_ANLYPLY_SEQ_GEN TO 0", conn
rs.Open "SET GENERATOR LOG_ANLYRECN_GEN TO 0", conn
rs.Open "SET GENERATOR LOG_ANLYREMK_GEN TO 0", conn
rs.Open "SET GENERATOR LOG_ANLYSTAT_GEN TO 0", conn
rs.Open "SET GENERATOR LOG_BSODDS_SEQ_GEN TO 0", conn
rs.Open "SET GENERATOR LOG_BLKMSG_SEQ_GEN TO 0", conn
rs.Open "SET GENERATOR LOG_CHART_SEQ_GEN TO 0", conn
rs.Open "SET GENERATOR LOG_CRS_SEQ_GEN TO 0", conn
rs.Open "SET GENERATOR LOG_GOALDTL_SEQ_GEN TO 0", conn
rs.Open "SET GENERATOR LOG_LVGOAL_SEQ_GEN TO 0", conn
rs.Open "SET GENERATOR LOG_LVODDS_SEQ_GEN TO 0", conn
rs.Open "SET GENERATOR LOG_LVPL_SEQ_GEN TO 0", conn
rs.Open "SET GENERATOR LOG_MATLIST_SEQ_GEN TO 0", conn
rs.Open "SET GENERATOR LOG_MENU_SEQ_GEN TO 0", conn
rs.Open "SET GENERATOR LOG_MODMAH_SEQ_GEN TO 0", conn
rs.Open "SET GENERATOR LOG_OTHGOAL_SEQ_GEN TO 0", conn
rs.Open "SET GENERATOR LOG_OTHODDS_SEQ_GEN TO 0", conn
rs.Open "SET GENERATOR LOG_MODOTH_SEQ_GEN TO 0", conn
rs.Open "SET GENERATOR LOG_RANK_SEQ_GEN TO 0", conn
rs.Open "SET GENERATOR LOG_REPORT_GEN TO 0", conn
rs.Open "SET GENERATOR LOG_SCORERS_SEQ_GEN TO 0", conn
rs.Open "SET GENERATOR LOG_SPTNEWS_SEQ_GEN TO 0", conn
rs.Open "SET GENERATOR LOG_OTHERSOCCER_SEQ_GEN TO 0", conn
rs.Open "SET GENERATOR LOG_OTHS_MODIFYMATCH_SEQ_GEN TO 0", conn
rs.Open "SET GENERATOR LOG_OTHSGOAL_SEQ_GEN TO 0", conn
rs.Open "SET GENERATOR LOG_OTHERSOCCER_SEQ_GEN TO 0", conn
rs.Open "SET GENERATOR LOG_OTHS_MODIFYMATCH_SEQ_GEN TO 0", conn
rs.Open "SET GENERATOR LOG_OTHSGOAL_SEQ_GEN TO 0", conn
conn.Close

'conn.ConnectionString = "Provider=IbOleDb;Location=127.0.0.1;Data Source=D:/MessageDispatcher/db/DISPATCHER_DB.GDB;User ID=sysdba;Password=masterkey;Min Pool Size=1;Max Pool Size=5"
'conn.Open
'rs.Open "update MSGFLAG set LASTSEQ=0", conn
'conn.Close

'Set connection object to null
Set rs = Nothing
Set conn = Nothing

'Log the SQL
dim filesys, filetxt, logFilePath
logFilePath = "D:\wwwsports\event_log\log_housekeep." & yyyy & mm & ".log"
Set filesys = CreateObject("Scripting.FileSystemObject")
Set filetxt = filesys.OpenTextFile(logFilePath, ForAppending, True)
filetxt.WriteLine(Date & " " & Time & "> LOG History housekeeped.")
filetxt.Close