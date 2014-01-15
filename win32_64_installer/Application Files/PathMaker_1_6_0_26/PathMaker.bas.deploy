Attribute VB_Name = "PathMaker"
Public Sub ExportPathMakerFile()
    GlobalVar.runningMacro = True

    Util.CheckIfSaveVSDFile
    
    Dim pg As Page, newPg As Page
    Dim sh As Shape, shp As Shape, newShp As Shape
    Dim vStencil As Visio.Document
    Dim i As Integer, j As Integer
    Dim appVisio As Visio.Application
    Dim currentDoc As Visio.Document: Set currentDoc = Visio.ActiveDocument
    Dim currentWin As Visio.Window: Set currentWin = Visio.ActiveWindow
    Dim fileLoc As String
    
    Set appVisio = CreateObject("Visio.Application")
    appVisio.ShowChanges = True
    RegEx.SetRegEx
    
    On Error Resume Next
    
    fileLoc = RegistryUtil.regQuery_A_Key(RegistryUtil.HKEY_LOCAL_MACHINE, "Software\Convergys", "InstalledPath") + "\PMConversion.vst"
    
    Dim newDoc As Visio.Document: Set newDoc = appVisio.Documents.Add(fileLoc)
    Dim newWin As Visio.Window: Set newWin = appVisio.ActiveWindow
    
    appVisio.ShowChanges = True
    
    For Each pg In currentDoc.Pages
        Set newPg = newDoc.Pages.Add
        newPg.name = pg.name
        newPg.Background = False
        
        currentWin.Activate
        ActiveWindow.Page = pg
         
        'clear these so they don't popup windows when we drop the new shapes
        For Each shp In pg.Shapes
            If StartWith(shp.name, "Off-page reference") Then
                shp.Cells("EventDrop").Formula = "=OPENTEXTWIN()"
            End If
        Next shp
        
        ActiveWindow.SelectAll
         
        If ActiveWindow.Selection.Count >= 1 Then
            If InStr(pg.Shapes.Item(1).name, "Document Title") = 0 And ActiveWindow.Selection.Count > 1 Then
                ActiveWindow.Selection.Group
            End If
         
            ActiveWindow.Selection.Copy (visCopyPasteNoTranslate)
            If InStr(pg.Shapes.Item(1).name, "Document Title") = 0 Then
                ActiveWindow.Selection.Ungroup
            End If
             
            newWin.Activate
            appVisio.ActiveWindow.Page = newPg
            newPg.Paste (visCopyPasteNoTranslate)
            newWin.SelectAll
            If InStr(newPg.Shapes.Item(1).name, "Document Title") = 0 Then
                newPg.Shapes(1).Ungroup
            End If
             
            If InStr(pg.Shapes.Item(1).name, "Document Title") = 0 Then
                currentWin.Activate
                ActiveWindow.Page = pg
            End If
         End If
         
         'and put them back
         For Each shp In pg.Shapes
             If StartWith(shp.name, "Off-page reference") Then
                 shp.Cells("EventDrop").Formula = "RUNADDONWARGS(""OPC"",""/CMD=1"")"
             End If
         Next shp

         
         newWin.Activate
         appVisio.ActiveWindow.Page = newPg
    Next pg
        
    appVisio.ActiveWindow.DeselectAll
    newDoc.Pages.Item("PMPlaceHolder").Delete True
    
    currentWin.Activate
    ActiveWindow.DeselectAll
    newWin.Activate
    
    Dim outPath As String
    Dim curDate As String
    Dim client As String, project As String
    Dim pagCurrent As Page, shaCurrent As Shape

    client = "'- not specified --"
    project = "'- not specified --"
    
    ' loop through each page
    For Each pagCurrent In ActiveDocument.Pages
        For Each shaCurrent In pagCurrent.Shapes
            If shaCurrent.CellExists("Prop.TitleLine1", False) Then
                client = shaCurrent.Cells("Prop.TitleLine1").Formula
                project = shaCurrent.Cells("Prop.TitleLine2").Formula
            End If
        Next
        
    Next

    curDate = Format(Now(), "yyyy-mm-dd_hh-mm-ss")
    outPath = ActiveDocument.path & Util.RemoveSurroundingQuotes(client) & "_" & Util.RemoveSurroundingQuotes(project) & "_Client_CallFlow_" & curDate & ".vui"
    
    newDoc.SaveAs outPath
    newDoc.Close
    Set newDoc = Nothing
    
    appVisio.Quit
    Set appVisio = Nothing
    
    MsgBox "PathMaker VUI is saved in: " & outPath, vbOKOnly, "CID"
    
    GlobalVar.runningMacro = False
    
End Sub


