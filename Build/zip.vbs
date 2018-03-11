'MAKE ZIP ARCHIVE ------------
Dim inputDir, outputFile
inputDir = Wscript.Arguments(0)
outputFile = Wscript.Arguments(1)

With CreateObject("Scripting.FileSystemObject")
    outputFile = .GetAbsolutePathName(outputFile)
    inputDir   = .GetAbsolutePathName(inputDir)
    With .CreateTextFile(outputFile, True)
        .Write Chr(80) & Chr(75) & Chr(5) & Chr(6) & String(18, chr(0))
    End With
End With

With CreateObject("Shell.Application")
    .NameSpace(outputFile).CopyHere .NameSpace(inputDir).Items
    Do Until .NameSpace(outputFile).Items.Count = .NameSpace(inputDir).Items.Count
        WScript.Sleep 1000
    Loop
End With
