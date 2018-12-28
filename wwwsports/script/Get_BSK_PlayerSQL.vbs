Const ForReading = 1, ForWriting = 2, ForAppending = 8
Dim filesys, fileEvtTxt, fileErrTxt, eventLogPath, errorLogPath
eventLogPath = "D:\Projects\wwwsports\event_log\BSKPlayerSQL." & Year(Date) & Month(Date) & Day(Date) & ".sql"
errorLogPath = "D:\Projects\wwwsports\error_log\BSKPlayerSQL_err." & Year(Date) & Month(Date) & Day(Date) & ".log"
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

Dim sPlayerSQL
'Select Player Info
sSQL = "SELECT CTEAM_ID, CPLAYER_NO, CPLAYER_NAME, CPLAYER_POSITION, CPLAYER_COUNTRY FROM PLAYER_INFO order by CTEAM_ID, CPLAYER_NO, CPLAYER_NAME"
'writeEventLog("Prepare BSK Palyer's info SQL: " & sSQL)
rootRS.Open sSQL, rootConn

If Err.Number <> 0 Then
	writeErrorLog("Prepare BSK Player's info SQL error: " & Err.Description & " [" & Err.Number & "]" )
	Err.Clear
End If

If Not rootRS.EOF Then
	rootRS.MoveFirst
	Do While Not rootRS.EOF
		sPlayerSQL = "INSERT INTO PLAYERS_INFO (CTEAM_ID, IPLAYER_NO, IPOS, CCHI_NAME, CCOUNTRY, IROSTER) VALUES ('" & rootRS("CTEAM_ID") & "', "

		If IsNull(rootRS("CPLAYER_NO")) Then
			sPlayerSQL = sPlayerSQL & "null, "
		Else
			sPlayerSQL = sPlayerSQL & rootRS("CPLAYER_NO") & ", "
		End If

		If IsNull(rootRS("CPLAYER_POSITION")) Then
			sPlayerSQL = sPlayerSQL & "null, "
		Else
			sPlayerSQL = sPlayerSQL & rootRS("CPLAYER_POSITION") & ", "
		End If

		If IsNull(rootRS("CPLAYER_NAME")) Then
			sPlayerSQL = sPlayerSQL & "null, "
		Else
			sPlayerSQL = sPlayerSQL & "'" & rootRS("CPLAYER_NAME") & "', "
		End If

		If IsNull(rootRS("CPLAYER_COUNTRY")) Then
			sPlayerSQL = sPlayerSQL & "null, "
		Else
			sPlayerSQL = sPlayerSQL & "'" & rootRS("CPLAYER_COUNTRY") & "', "
		End If

		sPlayerSQL = sPlayerSQL & "0);"
		writeEventLog(sPlayerSQL)
		rootRS.MoveNext
	Loop
Else
	writeEventLog("No BSK players to log.")
End If

If Err.Number <> 0 Then
	writeErrorLog("Update BSK players error: " & Err.Description & " [" & Err.Number & "]" )
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