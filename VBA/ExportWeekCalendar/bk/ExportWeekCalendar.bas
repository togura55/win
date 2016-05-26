Attribute VB_Name = "ExportWeekCalendar"
Public iWeek As Integer
Public bStat As Boolean


Sub ExportWeekCalendar()
    Dim dtExport As Date
    Dim strStart As String
    Dim strEnd As String
    Dim objFSO 'As FileSystemObject
    Dim stmCSVFile 'As TextStream
    Const CSV_FILE_NAME = "c:\Data\thismonth.csv" '' エクスポートするファイル名を指定してください。
    Dim colAppts As Items
    Dim objAppt 'As AppointmentItem
    Dim strLine As String
    Dim dStart As Date, dEnd As Date

'    Show WeekSelectDialog
    UserForm_SelectWeek.Show

    If bStat = True Then
        
    Call dThisWeek(dStart, dEnd)

'    dtExport = Now ' 来月の予定をエクスポートする場合は Now の代わりに DateAdd("m",1,Now) を使用します。
    ' 月単位ではなく任意の単位にする場合は以下の記述を変更します。
'    strStart = Year(Now) & "/" & Month(Now) & "/1 00:00"
    strStart = Year(dStart) & "/" & Month(dStart) & "/" & Day(dStart) & " 00:00"
    ' エクスポートする範囲の最後の日の次の日を strEnd に指定します。
'    strEnd = DateAdd("m", 1, CDate(strStart)) & " 00:00"
    strEnd = Year(dEnd) & "/" & Month(dEnd) & "/" & Day(dEnd) & " 00:00"
    Set objFSO = CreateObject("Scripting.FileSystemObject")
    Set stmCSVFile = objFSO.CreateTextFile(CSV_FILE_NAME, True)
    ' CSV ファイルのヘッダです。出力するフィールドを増減する場合はこちらも変更してください。
    stmCSVFile.WriteLine """件名"",""場所"",""開始日時"",""終了日時"",""分類項目"",""主催者"",""必須出席者"",""任意出席者"""
    Set colAppts = Application.Session.GetDefaultFolder(olFolderCalendar).Items
    colAppts.Sort "[Start]"
    colAppts.IncludeRecurrences = True
    Set objAppt = colAppts.Find("[Start] < """ & strEnd & """ AND [End] >= """ & strStart & """")
    While Not objAppt Is Nothing
        strLine = """" & objAppt.Subject & _
            """,""" & objAppt.Location & _
            """,""" & objAppt.Start & _
            """,""" & objAppt.End & _
            """,""" & objAppt.Categories & _
            """,""" & objAppt.Organizer & _
            """,""" & objAppt.RequiredAttendees & _
            """,""" & objAppt.OptionalAttendees & _
            """"
        stmCSVFile.WriteLine strLine
        Set objAppt = colAppts.FindNext
    Wend
    stmCSVFile.Close
    
    End If
    
End Sub


Sub dThisWeek(dStart As Date, dEnd As Date)

  '今週の期間を取得


  Dim sToday As String
  Dim dToday As Date
  Dim iOffset As Integer

  'dToday = Date '基準日
'  dToday = "2008/09/10" '基準日
    iOffset = 0
    If iWeek = 1 Then
        iOffset = 7
    End If
    
  dToday = Now + iOffset '基準日
  sToday = Format(dToday, "aaa") '基準日を曜日に変換

'  MsgBox "基準日 --> " & dToday

  Select Case sToday

  Case "月"
'　MsgBox "今週 -- >" & dToday & " ～" & dToday + 6
    dStart = dToday
    dEnd = dToday + 6
  Case "火"
'　MsgBox "今週 -- >" & dToday - 1 & " ～" & dToday + 5
    dStart = dToday - 1
    dEnd = dToday + 5
  Case "水"
'　MsgBox "今週 -- >" & dToday - 2 & " ～" & dToday + 4
    dStart = dToday - 2
    dEnd = dToday + 4
  Case "木"
'　MsgBox "今週 -- >" & dToday - 3 & " ～" & dToday + 3
    dStart = dToday - 3
    dEnd = dToday + 3
  Case "金"
'　MsgBox "今週 -- >" & dToday - 4 & " ～" & dToday + 2
    dStart = dToday - 4
    dEnd = dToday + 2
   Case "土"
'　MsgBox "今週 -- >" & dToday - 5 & " ～" & dToday + 1
    dStart = dToday - 5
    dEnd = dToday + 1
  Case "日"
'　MsgBox "今週 -- >" & dToday - 6 & " ～" & dToday + 0
    dStart = dToday - 6
    dEnd = dToday

  End Select

End Sub




