'GET VERSION ------------
Dim exePath, exeVer
exePath = Wscript.Arguments(0)
With CreateObject("Scripting.FileSystemObject")
    exePath = .GetAbsolutePathName(exePath)
    exeVer  = .GetFileVersion(exePath)
End With
exeVer = Left(exeVer, InStrRev(exeVer, ".") - 1)
exeVer = Replace(exeVer, ".", "-")
WScript.Echo exeVer
