Const ForReading = 1, ForWriting = 2, ForAppending = 8
Dim filesys, fileEvtTxt, fileErrTxt, eventLogPath, errorLogPath
eventLogPath = "D:\Projects\wwwsports\event_log\BSKData_evt." & Year(Date) & Month(Date) & Day(Date) & ".log"
errorLogPath = "D:\Projects\wwwsports\error_log\BSKData_err." & Year(Date) & Month(Date) & Day(Date) & ".log"
Set filesys = CreateObject("Scripting.FileSystemObject")

'Dim all variables
Dim yyyy, mm, dd, hh, nn
Dim LeagueID, sSQL
Dim rootConn, rootRS, updateRS

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

On Error Resume Next

'Open Connection to database
Set rootConn = CreateObject("adodb.connection")
'Open Recordset to hold data
set rootRS = CreateObject("adodb.recordset")
set updateRS = CreateObject("adodb.recordset")
'connect to D:\SPORTS\DB\ASIA_SOCCER_DB.GDB@localhost
rootConn.ConnectionString = "Provider=IbOleDb;Location=127.0.0.1;Data Source=D:/NBASports/db/ASIA_NBA.GDB;User ID=sysdba;Password=masterkey;Min Pool Size=5;Max Pool Size=100"
rootConn.Open

If Err.Number <> 0 Then
	writeErrorLog("Create BSK connection error: " & Err.Description & " [" & Err.Number & "]" )
	Err.Clear
End If

'Select Team ID
sSQL = "SELECT CLEAG_ID, CLEAGUE FROM LEAGUE_INFO order by CLEAG_ID"
writeEventLog("Prepare BSK LEAGUE ID SQL: " & sSQL)
rootRS.Open sSQL, rootConn

If Err.Number <> 0 Then
	writeErrorLog("Prepare BSK LEAGUE IDs SQL error: " & Err.Description & " [" & Err.Number & "]" )
	Err.Clear
End If

If Not rootRS.EOF Then
	rootRS.MoveFirst
	Do While Not rootRS.EOF
		LeagueID = rootRS("CLEAG_ID")
		writeEventLog("BSK League <ID: " & LeagueID & ", League " & Trim(rootRS("CLEAGUE")) & ">")
		sSQL = "UPDATE RANK_LEAGUE_INFO SET CLEAG_ID='0" & LeagueID & "' WHERE CLEAG_ID='" & LeagueID & "'"
		updateRS.Open sSQL, rootConn
		sSQL = "UPDATE PLAYER_INFO SET CLEAG_ID='0" & LeagueID & "' WHERE CLEAG_ID='" & LeagueID & "'"
		updateRS.Open sSQL, rootConn
		sSQL = "UPDATE IDMAP_INFO SET CLEAG_ID='0" & LeagueID & "' WHERE CLEAG_ID='" & LeagueID & "'"
		updateRS.Open sSQL, rootConn
		sSQL = "UPDATE LEAGUE_INFO SET CLEAG_ID='0" & LeagueID & "' WHERE CLEAG_ID='" & LeagueID & "'"
		updateRS.Open sSQL, rootConn
		rootRS.MoveNext
	Loop
Else
	writeEventLog("No BSK match to update.")
End If

If Err.Number <> 0 Then
	writeErrorLog("Update BSK match error: " & Err.Description & " [" & Err.Number & "]" )
	Err.Clear
End If

'Set connection object to null
Set rootRS = Nothing
Set updateRS = Nothing
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