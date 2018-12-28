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
rs.Open "delete from LOG_ADMINTASK", conn
rs.Open "delete from LOG_ALERTMSG", conn
rs.Open "delete from LOG_ALLODDS", conn
rs.Open "delete from LOG_ANALYSISBG", conn
rs.Open "delete from LOG_ANALYSISHISTORY", conn
rs.Open "delete from LOG_ANALYSISPLAYERS", conn
rs.Open "delete from LOG_ANALYSISRECENT", conn
rs.Open "delete from LOG_ANALYSISREMARKS", conn
rs.Open "delete from LOG_ANALYSISSTAT", conn
rs.Open "delete from LOG_BIGSMALLODDS", conn
rs.Open "delete from LOG_BLANKMESSAGE", conn
rs.Open "delete from LOG_CHARTRESEND", conn
rs.Open "delete from LOG_CORRECTSCORE", conn
rs.Open "delete from LOG_GOALDETAILS", conn
rs.Open "delete from LOG_LIVEGOAL", conn
rs.Open "delete from LOG_LIVEODDS", conn
rs.Open "delete from LOG_LIVEPLACE", conn
rs.Open "delete from LOG_MATCHLIST", conn
rs.Open "delete from LOG_MODIFYMATCH", conn
rs.Open "delete from LOG_OTHERGOAL", conn
rs.Open "delete from LOG_OTHERODDS", conn
rs.Open "delete from LOG_OTHERODDSMATCHMODIFY", conn
rs.Open "delete from LOG_RANK", conn
rs.Open "delete from LOG_REPORT", conn
rs.Open "delete from LOG_SCORERS", conn
rs.Open "delete from LOG_SPORTNEWS", conn
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
rs.Open "SET GENERATOR LOG_MODMAH_SEQ_GEN TO 0", conn
rs.Open "SET GENERATOR LOG_OTHGOAL_SEQ_GEN TO 0", conn
rs.Open "SET GENERATOR LOG_OTHODDS_SEQ_GEN TO 0", conn
rs.Open "SET GENERATOR LOG_MODOTH_SEQ_GEN TO 0", conn
rs.Open "SET GENERATOR LOG_RANK_SEQ_GEN TO 0", conn
rs.Open "SET GENERATOR LOG_REPORT_GEN TO 0", conn
rs.Open "SET GENERATOR LOG_SCORERS_SEQ_GEN TO 0", conn
rs.Open "SET GENERATOR LOG_SPTNEWS_SEQ_GEN TO 0", conn
conn.Close

conn.ConnectionString = "Provider=IbOleDb;Location=127.0.0.1;Data Source=D:/Projects/MessageDispatcher/db/DISPATCHER_DB.GDB;User ID=sysdba;Password=masterkey;Min Pool Size=1;Max Pool Size=5"
conn.Open
rs.Open "update MSGFLAG set LASTSEQ=0", conn
conn.Close

'Set connection object to null
Set rs = Nothing
Set conn = Nothing

'Log the SQL
dim filesys, filetxt, logFilePath
logFilePath = "D:\Projects\wwwsports\event_log\log_housekeep." & yyyy & mm & ".log"
Set filesys = CreateObject("Scripting.FileSystemObject")
Set filetxt = filesys.OpenTextFile(logFilePath, ForAppending, True)
filetxt.WriteLine(Date & " " & Time & "> LOG SOC History housekeeped.")
filetxt.Close