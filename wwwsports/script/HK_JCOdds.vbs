Const ForReading = 1, ForWriting = 2, ForAppending = 8
'Const TristateUseDefault = ¡V2, TristateTrue = ¡V1, TristateFalse  = 0

Const DB2_Host = "192.168.102.5"
Const DB2_DBName = "ASIASPT"
Const DB2_Username = "wgdb"
Const DB2_Pwd = "ibmdb2"
Const LogFile_Path = "C:/JCSports/Datafile/"

'Dim all variables
Dim DB2conn, numRecords

'Open file for logging
Dim fso, txtfile
Set fso = CreateObject("Scripting.FileSystemObject")
'Set txtfile = fso.OpenTextFile(LogFile_Path & "HK_JCOdds_Log.txt", ForAppending, True, TristateFalse)
Set txtfile = fso.OpenTextFile("HK_JCOdds_Log.txt", ForAppending, True, TristateFalse)

'Open Connection to DB2 database
Set DB2conn = CreateObject("adodb.connection")
DB2ConnectStr = "Provider=IBMDADB2;" & _
                  "Database="&DB2_DBName&";" & _
                  "HOSTNAME="&DB2_Host&";" & _
                  "PORT=50010;PROTOCOL=TCPIP;" & _
                  "uid="&DB2_Username&";" & _
                  "pwd="&DB2_Pwd&";"

DB2conn.Open DB2ConnectStr
'Open DB2 Recordset to exec SQL
set DB2rs = CreateObject("adodb.recordset")

composedMatchDateTime = CDbl(CurDateStr) + (CDbl(CurTimeStr)/10000)

' Check if any recent data exist, if yes then clear the housekeep table and housekeep the recent data
DB2rs.Open "SELECT COUNT(*) NUM_RECORDS FROM JC_ODDS_DETAILS WHERE (IMATCH_DATE+(CAST(IMATCH_TIME AS DOUBLE)/10000))<="&composedMatchDateTime, DB2conn
numRecords = DB2rs("NUM_RECORDS")
DB2rs.Close

If numRecords > 0 Then
	DB2rs.Open "DELETE FROM HK_JC_ODDS_DETAILS", DB2conn
	' Start housekeep recent data for JC_ODDS_DETAILS
	SQL = "INSERT INTO HK_JC_ODDS_DETAILS (SELECT * FROM JC_ODDS_DETAILS WHERE (IMATCH_DATE+(CAST(IMATCH_TIME AS DOUBLE)/10000))<="&composedMatchDateTime&")"
	DB2rs.Open SQL, DB2conn
	txtfile.WriteLine("Housekeeped table JC_ODDS_DETAILS")

	DB2rs.Open "DELETE FROM HK_JC_CRS_ODDS_DETAILS", DB2conn
	' Start housekeep recent data for JC_CRS_ODDS_DETAILS
	SQL = "INSERT INTO HK_JC_CRS_ODDS_DETAILS (select CRS.* from JC_CRS_ODDS_DETAILS CRS, JC_ODDS_DETAILS OD WHERE CRS.CMATCH_DAY_CODE = OD.CMATCH_DAY_CODE AND CRS.IMATCH_NO = OD.IMATCH_NO AND (OD.IMATCH_DATE+(CAST(OD.IMATCH_TIME AS DOUBLE)/10000))<="&composedMatchDateTime&")"
	DB2rs.Open SQL, DB2conn
	txtfile.WriteLine("Housekeeped table JC_CRS_ODDS_DETAILS")

	DB2rs.Open "DELETE FROM HK_JC_TTG_ODDS_DETAILS", DB2conn
	' Start housekeep recent data for JC_TTG_ODDS_DETAILS
	SQL = "INSERT INTO HK_JC_TTG_ODDS_DETAILS (select TTG.* from JC_TTG_ODDS_DETAILS TTG, JC_ODDS_DETAILS OD WHERE TTG.CMATCH_DAY_CODE = OD.CMATCH_DAY_CODE AND TTG.IMATCH_NO = OD.IMATCH_NO AND (OD.IMATCH_DATE+(CAST(OD.IMATCH_TIME AS DOUBLE)/10000))<="&composedMatchDateTime&")"
	DB2rs.Open SQL, DB2conn
	txtfile.WriteLine("Housekeeped table JC_TTG_ODDS_DETAILS")
End If


'free resource
'DB2rs.Close
Set DB2rs = Nothing

'close DB2 connection required objects
DB2conn.Close
Set DB2conn = Nothing

'close log file required objects
txtfile.Close
Set txtfile = Nothing
Set fso = Nothing

Public Function CurDateStr()
	Dim monthStr, dayStr
	
	If Len(Month(Now)) < 2 Then
		monthStr = "0" & Month(Now)
	Else
		monthStr = Month(Now)
	End If
	
	If Len(Day(Now)) < 2 Then
		dayStr = "0" & Day(Now)
	Else
		dayStr = Day(Now)
	End If
	
	CurDateStr = Year(Now) & monthStr & dayStr
End Function

Public Function CurTimeStr()
	Dim minStr
	
	If Len(Minute(Now)) < 2 Then
		minStr = "0" & Minute(Now)
	Else
		minStr = Minute(Now)
	End If
	
	CurTimeStr = Hour(Now) & minStr
End Function