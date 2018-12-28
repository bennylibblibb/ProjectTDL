Const ForReading = 1, ForWriting = 2, ForAppending = 8
Dim filesys, fileEvtTxt, fileErrTxt, eventLogPath, errorLogPath
eventLogPath = "D:\Projects\wwwsports\event_log\failover_evt." & Year(Date) & Month(Date) & Day(Date) & ".log"
errorLogPath = "D:\Projects\wwwsports\error_log\failover_err." & Year(Date) & Month(Date) & Day(Date) & ".log"
Set filesys = CreateObject("Scripting.FileSystemObject")

'Dim all variables
Dim yyyy, mm, dd, hh, nn, current_date, current_time
Dim MatchCount, sSQL
Dim liveDBConn, bkupDBConn, liveRS, pruneRS

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
current_date = yyyy & mm & dd
current_time = hh & nn & ss

On Error Resume Next

'Open Connection to database
Set liveDBConn = CreateObject("adodb.connection")
'Open Recordset to hold data
set liveRS = CreateObject("adodb.recordset")
set pruneRS = CreateObject("adodb.recordset")
'connect to D:\SPORTS\DB\ASIA_SOCCER_DB.GDB@localhost
liveDBConn.ConnectionString = "Provider=IbOleDb;Location=127.0.0.1;Data Source=D:/Sports/db/ASIA_SOCCER_DB.GDB;User ID=sysdba;Password=masterkey;Min Pool Size=5;Max Pool Size=100"
liveDBConn.Open

If Err.Number <> 0 Then
	writeErrorLog("Create Live DB connection error: " & Err.Description & " [" & Err.Number & "]" )
	Err.Clear
End If

'Select soccer schedule from live DB
sSQL = "SELECT IMATCH_CNT, CLEAGUEALIAS, CLEAGUE, CHOST, CGUEST, MATCHDATETIME, CFIELD, CHOST_HANDI, CSTATUS FROM SOCCERSCHEDULE order by IMATCH_CNT, MATCHDATETIME"
liveRS.Open sSQL, liveDBConn

If Err.Number <> 0 Then
	writeErrorLog("Prepare SQL error: " & Err.Description & " [" & Err.Number & "]" )
	Err.Clear
End If

If Not liveRS.EOF Then
	liveRS.MoveFirst
	Do While Not liveRS.EOF
		MatchCount = liveRS("IMATCH_CNT")
		writeEventLog("Match <ID: " & MatchCount & ", Date: " & liveRS("MATCHDATETIME") & ", Alias: " & Trim(liveRS("CLEAGUEALIAS")) & ", Host: " & Trim(liveRS("CHOST")) & ", Guest: " & Trim(liveRS("CGUEST")) & ">")
		'sSQL = "DELETE FROM ANALYSISINFO WHERE MATCH_CNT=" & MatchCount
		'pruneRS.Open sSQL, liveDBConn
		liveRS.MoveNext
	Loop
Else
	writeEventLog("Empty soccer schedule")
End If

'sSQL = "DELETE FROM OTHERODDSINFO WHERE (MATCHDATE + MATCHTIME/10000)<'" & (current_date + current_time/10000) & "'"
'pruneRS.Open sSQL, liveDBConn
'sSQL = "DELETE FROM OTHERRESINFO WHERE (MATCHDATE + MATCHTIME/10000)<'" & (current_date + current_time/10000) & "'"
'pruneRS.Open sSQL, liveDBConn
'liveRS.Close

'If Err.Number <> 0 Then
	'writeErrorLog("Housekeeping SOC match error: " & Err.Description & " [" & Err.Number & "]" )
	'Err.Clear
'End If

'Set connection object to null
Set liveRS = Nothing
'Set pruneRS = Nothing
Set liveDBConn = Nothing

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