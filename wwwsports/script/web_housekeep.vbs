Const ForReading = 1, ForWriting = 2, ForAppending = 8

'Dim all variables
Dim conn, rs, yyyy, mm, dd, hh, nn, currentdate, currenttime, sSQL
Dim MatchCount, CurrentDateTime

'Open Connection to database
Set conn = CreateObject("adodb.connection")
'Open Recordset to hold data
set rs = CreateObject("adodb.recordset")

CurrentDateTime = DateAdd("d",-2,Date)

yyyy = Year(CurrentDateTime)
mm = Month(Date)
If mm < 10 Then
	mm = "0" & mm
End If
dd = Day(CurrentDateTime)
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
conn.ConnectionString = "Provider=IbOleDb;Location=127.0.0.1;Data Source=D:/Sports/db/ASIA_SOCCER_DB.GDB;User ID=sysdba;Password=masterkey;Min Pool Size=5;Max Pool Size=100"
conn.Open

'Select Match Count(key)
sSQL = "SELECT MATCH_CNT FROM GAMEINFO where (MATCHDATE + MATCHTIME/10000)<'" & (currentdate + currenttime/10000) & "'"
rs.Open sSQL, conn
MatchCount = rs("MATCH_CNT")
rs.Close

rs.Open "delete from GAMEINFO where ACT='D'", conn
rs.Open "delete from GOALINFO where ACT='D'", conn
sSQL = "delete from GAMEINFO where (MATCHDATE + MATCHTIME/10000)<'" & (currentdate + currenttime/10000) & "'"
rs.Open sSQL, conn
sSQL = "delete from GOALINFO where (MATCHDATE + MATCHTIME/10000)<'" & (currentdate + currenttime/10000) & "'"
rs.Open sSQL, conn
conn.Close

'Set connection object to null
Set rs = Nothing
Set conn = Nothing

'Log the SQL
dim filesys, filetxt, logFilePath
logFilePath = "D:\WebSource\event_log\web_housekeep." & yyyy & mm & ".log"
'logFilePath = "C:\projects\wwwsports\script\web_housekeep." & yyyy & mm & ".log"
Set filesys = CreateObject("Scripting.FileSystemObject")
Set filetxt = filesys.OpenTextFile(logFilePath, ForAppending, True)
filetxt.WriteLine(Date & " " & Time & "> GAMEINFO and GOALINFO housekeeped")
filetxt.Close