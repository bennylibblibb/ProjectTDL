Const ForReading = 1, ForWriting = 2, ForAppending = 8

'Dim all variables
Dim conn, rs

'Open Connection to database
Set conn = CreateObject("adodb.connection")
'Open Recordset to hold data
set rs = CreateObject("adodb.recordset")

'connection for <192.168.102.34>:D:\SPORTS\DB\ASIA_SOCCER_DB.GDB
conn.ConnectionString = "Provider=IbOleDb;Location=192.168.102.34;Data Source=D:/Sports/db/ASIA_SOCCER_DB.GDB;User ID=sysdba;Password=masterkey;Min Pool Size=5;Max Pool Size=100"
conn.Open
'reset comq_gen value
rs.Open "SET GENERATOR COMQ_GEN TO 1", conn
conn.Close

'connection for <192.168.102.34>:D:\SPORTS\REPDB\ASIA_SOCCER_REPDB.GDB
conn.ConnectionString = "Provider=IbOleDb;Location=192.168.102.34;Data Source=D:/Sports/Repdb/ASIA_SOCCER_REPDB.GDB;User ID=sysdba;Password=masterkey;Min Pool Size=5;Max Pool Size=100"
conn.Open
'reset rep_pager_gen value
rs.Open "SET GENERATOR REP_PAGER_GEN TO 1", conn
'reset rep_web_gen value
rs.Open "SET GENERATOR REP_WEB_GEN TO 1", conn
conn.Close

Set rs = Nothing
Set conn = Nothing
