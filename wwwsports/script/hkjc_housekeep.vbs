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
conn.ConnectionString = "Provider=IbOleDb;Location=127.0.0.1;Data Source=D:/Sports/db/ASIA_SOCCER_DB.GDB;User ID=sysdba;Password=masterkey;Min Pool Size=5;Max Pool Size=100"
conn.Open
rs.Open "delete from HKJCSOCTTG_INFO where IMATCH_CNT in (select IMATCH_CNT from HKJCSOCCER_INFO where MATCHDATETIME < CURRENT_TIMESTAMP)", conn
rs.Open "delete from HKJCSOCHDA_INFO where IMATCH_CNT in (select IMATCH_CNT from HKJCSOCCER_INFO where MATCHDATETIME < CURRENT_TIMESTAMP)", conn
rs.Open "delete from HKJCSOCCRS_INFO where IMATCH_CNT in (select IMATCH_CNT from HKJCSOCCER_INFO where MATCHDATETIME < CURRENT_TIMESTAMP)", conn
conn.Close
conn.Open
rs.Open "delete from HKJCSOCCER_INFO where MATCHDATETIME < CURRENT_TIMESTAMP", conn
conn.Close

'Set connection object to null
Set rs = Nothing
Set conn = Nothing

'Log the SQL
dim filesys, filetxt, logFilePath
logFilePath = "D:\Projects\wwwsports_gogo2\event_log\hkjc_housekeep." & yyyy & mm & ".log"
'logFilePath = "D:\projects\wwwsports_gogo2\event_log\hkjc_housekeep." & yyyy & mm & ".log"
Set filesys = CreateObject("Scripting.FileSystemObject")
Set filetxt = filesys.OpenTextFile(logFilePath, ForAppending, True)
filetxt.WriteLine(Date & " " & Time & "> HKJC DATA housekeeped.")
filetxt.Close