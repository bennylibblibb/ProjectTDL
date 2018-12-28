Const ForReading = 1, ForWriting = 2, ForAppending = 8

'Dim all variables
Dim conn, rs

'Open Connection to database
'Note the 2nd line holds a DSN
Set conn = CreateObject("adodb.connection")
conn.Open "Soccer","SYSDBA","masterkey"

'Open Recordset to hold data
set rs = CreateObject("adodb.recordset")
rs.Open "UPDATE Header SET Host = 'Go Go Pager' WHERE Header = 721", conn


'close all objects
'rs.Close
conn.Close
Set rs = Nothing
Set conn = Nothing
