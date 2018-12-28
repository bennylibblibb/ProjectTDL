Const ForReading = 1, ForWriting = 2, ForAppending = 8
Dim filesys, fileEvtTxt, fileErrTxt, eventLogPath, errorLogPath
eventLogPath = "D:\wwwsports\event_log\web_hk_evt." & Year(Date) & Month(Date) & Day(Date) & ".log"
errorLogPath = "D:\wwwsports\error_log\web_hk_err." & Year(Date) & Month(Date) & Day(Date) & ".log"
Set filesys = CreateObject("Scripting.FileSystemObject")

'Dim all variables
Dim yyyy, mm, dd, hh, nn, prune_date, prune_time, PruneDateTime
Dim MatchCount, sSQL
Dim rootConn, rootRS, pruneRS

PruneDateTime = DateAdd("d",-2,Date)
yyyy = Year(PruneDateTime)
mm = Month(PruneDateTime)
If mm < 10 Then
	mm = "0" & mm
End If
dd = Day(PruneDateTime)
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
prune_date = yyyy & mm & dd
prune_time = hh & nn & ss

On Error Resume Next

'Open Connection to database
Set rootConn = CreateObject("adodb.connection")
'Open Recordset to hold data
set rootRS = CreateObject("adodb.recordset")
set pruneRS = CreateObject("adodb.recordset")
'connect to D:\SPORTS\DB\ASIA_SOCCER_DB.GDB@localhost
rootConn.ConnectionString = "Provider=IbOleDb;Location=127.0.0.1;Data Source=D:/Sports/db/ASIA_SOCCER_DB.GDB;User ID=sysdba;Password=masterkey;Min Pool Size=5;Max Pool Size=100"
rootConn.Open

If Err.Number <> 0 Then
	writeErrorLog("Create connection error: " & Err.Description & " [" & Err.Number & "]" )
	Err.Clear
End If

'Select Match Count(key)
sSQL = "SELECT MATCH_CNT, MATCHDATE, MATCHTIME, LEAGUE, HOST, GUEST FROM GAMEINFO where (MATCHDATE + MATCHTIME/10000)<'" & (prune_date + prune_time/10000) & "' order by MATCH_CNT"
writeEventLog("Prepare housekeep SQL: " & sSQL)
rootRS.Open sSQL, rootConn

If Err.Number <> 0 Then
	writeErrorLog("Prepare housekeep items SQL error: " & Err.Description & " [" & Err.Number & "]" )
	Err.Clear
End If

If Not rootRS.EOF Then
	rootRS.MoveFirst
	Do While Not rootRS.EOF
		MatchCount = rootRS("MATCH_CNT")
		writeEventLog("Housekeeping Match <ID: " & MatchCount & ", Date: " & rootRS("MATCHDATE") & ", Time: " & rootRS("MATCHTIME") & ", Alias: " & Trim(rootRS("LEAGUE")) & ", Host: " & Trim(rootRS("HOST")) & ", Guest: " & Trim(rootRS("GUEST")) & ">")
		'sSQL = "DELETE FROM ANALYSISINFO WHERE MATCH_CNT=" & MatchCount
		'pruneRS.Open sSQL, rootConn
		sSQL = "DELETE FROM ANALYSIS_BG_INFO WHERE IMATCH_CNT=" & MatchCount
		pruneRS.Open sSQL, rootConn
		sSQL = "DELETE FROM ANALYSIS_HISTORY_INFO WHERE IMATCH_CNT=" & MatchCount
		pruneRS.Open sSQL, rootConn
		sSQL = "DELETE FROM ANALYSIS_REMARK_INFO WHERE IMATCH_CNT=" & MatchCount
		pruneRS.Open sSQL, rootConn
		sSQL = "DELETE FROM ANALYSIS_STAT_INFO WHERE IMATCH_CNT=" & MatchCount
		pruneRS.Open sSQL, rootConn
		sSQL = "DELETE FROM ANALYSIS_RECENT_INFO WHERE IMATCH_CNT=" & MatchCount
		pruneRS.Open sSQL, rootConn
		sSQL = "DELETE FROM BIGSMALLODDS_INFO WHERE IMATCH_CNT=" & MatchCount
		pruneRS.Open sSQL, rootConn
		sSQL = "DELETE FROM CORRECTSCORE_INFO WHERE IMATCH_CNT=" & MatchCount
		pruneRS.Open sSQL, rootConn
		sSQL = "DELETE FROM LIVEODDS_INFO WHERE IMATCH_CNT=" & MatchCount
		pruneRS.Open sSQL, rootConn
		sSQL = "DELETE FROM NEWSINFO_MATCHLIST WHERE IMATCH_CNT=" & MatchCount
		pruneRS.Open sSQL, rootConn
		sSQL = "DELETE FROM RESULTINFO WHERE MATCH_CNT=" & MatchCount
		pruneRS.Open sSQL, rootConn
		sSQL = "DELETE FROM TIMEOFGAME_INFO WHERE IMATCH_CNT=" & MatchCount
		pruneRS.Open sSQL, rootConn
		sSQL = "DELETE FROM GOALINFO WHERE MATCH_CNT=" & MatchCount
		pruneRS.Open sSQL, rootConn
		sSQL = "DELETE FROM GAMEINFO WHERE MATCH_CNT=" & MatchCount
		pruneRS.Open sSQL, rootConn
		rootRS.MoveNext
	Loop
Else
	writeEventLog("No match to housekeep, cut off: " & prune_date & " " & prune_time)
End If

sSQL = "DELETE FROM OTHERODDSINFO WHERE (MATCHDATE + MATCHTIME/10000)<'" & (prune_date + prune_time/10000) & "'"
pruneRS.Open sSQL, rootConn
sSQL = "DELETE FROM OTHERRESINFO WHERE (MATCHDATE + MATCHTIME/10000)<'" & (prune_date + prune_time/10000) & "'"
pruneRS.Open sSQL, rootConn
rootRS.Close

If Err.Number <> 0 Then
	writeErrorLog("Housekeeping match error: " & Err.Description & " [" & Err.Number & "]" )
	Err.Clear
End If

'Set connection object to null
Set rootRS = Nothing
Set pruneRS = Nothing
Set rootConn = Nothing

Sub writeEventLog(statement)
Set fileEvtTxt = filesys.OpenTextFile(eventLogPath, ForAppending, True)
fileEvtTxt.WriteLine(Time & "> " & statement)
fileEvtTxt.Close
End Sub

Sub writeErrorLog(statement)
Set fileErrTxt = filesys.OpenTextFile(errorLogPath, ForAppending, True)
fileErrTxt.WriteLine(Time & "> " & statement)
fileErrTxt.Close
End Sub