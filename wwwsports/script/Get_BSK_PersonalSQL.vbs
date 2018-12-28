Const ForReading = 1, ForWriting = 2, ForAppending = 8
Dim filesys, fileEvtTxt, fileErrTxt, eventLogPath, errorLogPath
eventLogPath = "D:\Projects\wwwsports\event_log\BSKPersonalRankSQL." & Year(Date) & Month(Date) & Day(Date) & ".sql"
errorLogPath = "D:\Projects\wwwsports\error_log\BSKPersonalRankSQL_err." & Year(Date) & Month(Date) & Day(Date) & ".log"
Set filesys = CreateObject("Scripting.FileSystemObject")

'Dim all variables
Dim yyyy, mm, dd, hh, nn
Dim TeamID, sSQL
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

Dim sPlayerSQL, PlayerID
'Select Player Info
sSQL = "SELECT IRANK_TYPE, CPLAYER_ID, IRANK, CRANK_DATA FROM RANK_PERSONAL_INFO order by IRANK_TYPE, CPLAYER_ID"
'writeEventLog("Prepare BSK personal rank info SQL: " & sSQL)
rootRS.Open sSQL, rootConn

If Err.Number <> 0 Then
	writeErrorLog("Prepare BSK Perspnal Rank info SQL error: " & Err.Description & " [" & Err.Number & "]" )
	Err.Clear
End If

If Not rootRS.EOF Then
	rootRS.MoveFirst
	Do While Not rootRS.EOF
		sPlayerSQL = "INSERT INTO RANKPERSONAL_INFO (CLEAG_ID, CTEAM_ID, CCHI_NAME, IRANK, CRANK_DATA) VALUES ("

		PlayerID = rootRS("CPLAYER_ID")

		'Double check updated CLEAG_ID
		If rootRS("IRANK_TYPE") == 0 Then
			sPlayerSQL = sPlayerSQL & "'007', "
		Else If rootRS("IRANK_TYPE") == 1 Then
			sPlayerSQL = sPlayerSQL & "'008', "
		Else If rootRS("IRANK_TYPE") == 2 Then
			sPlayerSQL = sPlayerSQL & "'009', "
		End If

		sSQL = "SELECT CTEAM_ID, CPLAYER_NAME FROM PLAYER_INFO where CPLAYER_ID='" & PlayerID & "'"
		updateRS.Open sSQL, rootConn
		sPlayerSQL = sPlayerSQL & "'" & updateRS("CTEAM_ID") & "', '" & updateRS("CPLAYER_NAME") & "', "
		updateRS.Close

		If IsNull(rootRS("IRANK")) Then
			sPlayerSQL = sPlayerSQL & "null, "
		Else
			sPlayerSQL = sPlayerSQL & rootRS("IRANK") & ", "
		End If

		If IsNull(rootRS("CRANK_DATA")) Then
			sPlayerSQL = sPlayerSQL & "null);"
		Else
			sPlayerSQL = sPlayerSQL & rootRS("CRANK_DATA") & ");"
		End If
		writeEventLog(sPlayerSQL)
		rootRS.MoveNext
	Loop
Else
	writeEventLog("No BSK personal rank to log.")
End If

If Err.Number <> 0 Then
	writeErrorLog("Update BSK personal rank error: " & Err.Description & " [" & Err.Number & "]" )
	Err.Clear
End If

'Set connection object to null
Set rootRS = Nothing
Set updateRS = Nothing
Set rootConn = Nothing

Sub writeEventLog(statement)
Set fileEvtTxt = filesys.OpenTextFile(eventLogPath, ForAppending, True)
fileEvtTxt.WriteLine(statement)
fileEvtTxt.Close
End Sub

Sub writeErrorLog(statement)
Set fileErrTxt = filesys.OpenTextFile(errorLogPath, ForAppending, True)
fileErrTxt.WriteLine(Time & "> " & statement)
fileErrTxt.Close
End Sub