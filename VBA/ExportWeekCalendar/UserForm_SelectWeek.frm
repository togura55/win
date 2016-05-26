VERSION 5.00
Begin {C62A69F0-16DC-11CE-9E98-00AA00574A4F} UserForm_SelectWeek 
   Caption         =   "Select Weeks"
   ClientHeight    =   1995
   ClientLeft      =   45
   ClientTop       =   375
   ClientWidth     =   2685
   OleObjectBlob   =   "UserForm_SelectWeek.frx":0000
   StartUpPosition =   1  'オーナー フォームの中央
End
Attribute VB_Name = "UserForm_SelectWeek"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False

Option Explicit


' OK
Private Sub CommandButton1_Click()
    bStat = True
'    UserForm_SelectWeek
    Me.Hide
End Sub

' Cancel
Private Sub CommandButton2_Click()
    bStat = False
    Me.Hide
End Sub

' This Week
Private Sub OptionButton1_Click()
    iWeek = 0
End Sub

' Next Week
Private Sub OptionButton2_Click()
    iWeek = 1
End Sub

Private Sub UserForm_Click()

End Sub

Private Sub UserForm_Initialize()
    OptionButton1.Caption = "This Week"
    OptionButton2.Caption = "Next Week"
    CommandButton1.Caption = "OK"
    CommandButton2.Caption = "Cancel"
    OptionButton1.Value = True
    iWeek = 0
    bStat = True
End Sub
