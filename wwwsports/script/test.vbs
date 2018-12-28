Const ForReading = 1, ForWriting = 2, ForAppending = 8

'Log the SQL
dim filesys, filetxt, logFilePath
logFilePath = "D:\Projects\wwwsports\script\test.txt"
Set filesys = CreateObject("Scripting.FileSystemObject")
Set filetxt = filesys.OpenTextFile(logFilePath, ForAppending, True)
filetxt.WriteLine(Date & " " & Time & "> Test script.")
filetxt.Close