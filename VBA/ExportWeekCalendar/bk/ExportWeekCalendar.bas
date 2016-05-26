Attribute VB_Name = "ExportWeekCalendar"
Public iWeek As Integer
Public bStat As Boolean


Sub ExportWeekCalendar()
    Dim dtExport As Date
    Dim strStart As String
    Dim strEnd As String
    Dim objFSO 'As FileSystemObject
    Dim stmCSVFile 'As TextStream
    Const CSV_FILE_NAME = "c:\Data\thismonth.csv" '' �G�N�X�|�[�g����t�@�C�������w�肵�Ă��������B
    Dim colAppts As Items
    Dim objAppt 'As AppointmentItem
    Dim strLine As String
    Dim dStart As Date, dEnd As Date

'    Show WeekSelectDialog
    UserForm_SelectWeek.Show

    If bStat = True Then
        
    Call dThisWeek(dStart, dEnd)

'    dtExport = Now ' �����̗\����G�N�X�|�[�g����ꍇ�� Now �̑���� DateAdd("m",1,Now) ���g�p���܂��B
    ' ���P�ʂł͂Ȃ��C�ӂ̒P�ʂɂ���ꍇ�͈ȉ��̋L�q��ύX���܂��B
'    strStart = Year(Now) & "/" & Month(Now) & "/1 00:00"
    strStart = Year(dStart) & "/" & Month(dStart) & "/" & Day(dStart) & " 00:00"
    ' �G�N�X�|�[�g����͈͂̍Ō�̓��̎��̓��� strEnd �Ɏw�肵�܂��B
'    strEnd = DateAdd("m", 1, CDate(strStart)) & " 00:00"
    strEnd = Year(dEnd) & "/" & Month(dEnd) & "/" & Day(dEnd) & " 00:00"
    Set objFSO = CreateObject("Scripting.FileSystemObject")
    Set stmCSVFile = objFSO.CreateTextFile(CSV_FILE_NAME, True)
    ' CSV �t�@�C���̃w�b�_�ł��B�o�͂���t�B�[���h�𑝌�����ꍇ�͂�������ύX���Ă��������B
    stmCSVFile.WriteLine """����"",""�ꏊ"",""�J�n����"",""�I������"",""���ލ���"",""��Î�"",""�K�{�o�Ȏ�"",""�C�ӏo�Ȏ�"""
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

  '���T�̊��Ԃ��擾


  Dim sToday As String
  Dim dToday As Date
  Dim iOffset As Integer

  'dToday = Date '���
'  dToday = "2008/09/10" '���
    iOffset = 0
    If iWeek = 1 Then
        iOffset = 7
    End If
    
  dToday = Now + iOffset '���
  sToday = Format(dToday, "aaa") '�����j���ɕϊ�

'  MsgBox "��� --> " & dToday

  Select Case sToday

  Case "��"
'�@MsgBox "���T -- >" & dToday & " �`" & dToday + 6
    dStart = dToday
    dEnd = dToday + 6
  Case "��"
'�@MsgBox "���T -- >" & dToday - 1 & " �`" & dToday + 5
    dStart = dToday - 1
    dEnd = dToday + 5
  Case "��"
'�@MsgBox "���T -- >" & dToday - 2 & " �`" & dToday + 4
    dStart = dToday - 2
    dEnd = dToday + 4
  Case "��"
'�@MsgBox "���T -- >" & dToday - 3 & " �`" & dToday + 3
    dStart = dToday - 3
    dEnd = dToday + 3
  Case "��"
'�@MsgBox "���T -- >" & dToday - 4 & " �`" & dToday + 2
    dStart = dToday - 4
    dEnd = dToday + 2
   Case "�y"
'�@MsgBox "���T -- >" & dToday - 5 & " �`" & dToday + 1
    dStart = dToday - 5
    dEnd = dToday + 1
  Case "��"
'�@MsgBox "���T -- >" & dToday - 6 & " �`" & dToday + 0
    dStart = dToday - 6
    dEnd = dToday

  End Select

End Sub




