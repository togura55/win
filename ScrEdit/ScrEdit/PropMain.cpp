// PropMain.cpp : 実装ファイル
//

#include "stdafx.h"
#include "ScrEdit.h"
#include "PropMain.h"


// CPropMain ダイアログ

IMPLEMENT_DYNAMIC(CPropMain, CPropertyPage)

CPropMain::CPropMain()
	: CPropertyPage(CPropMain::IDD)
	, m_xvEditCd(_T(""))
	, m_xvEditLcd(_T(""))
	, m_xvEditCmd1(_T(""))
	, m_xvEditCmd2(_T(""))
	, m_xvCombo_Cmd1(0)
	, m_xvCombo_Cmd2(0)
	, m_bAddEntryState(true)
{

}

CPropMain::~CPropMain()
{
}

void CPropMain::DoDataExchange(CDataExchange* pDX)
{
	CPropertyPage::DoDataExchange(pDX);
	DDX_Control(pDX, IDC_LIST_AS, m_xcListAS);
	DDX_Control(pDX, IDC_PBTN_ASUP, m_xcPbtnUp);
	DDX_Control(pDX, IDC_PBTN_ASDW, m_xcPbtnDw);
	DDX_Text(pDX, IDC_EDIT_CD, m_xvEditCd);
	DDX_Text(pDX, IDC_EDIT_LCD, m_xvEditLcd);
	DDX_Text(pDX, IDC_EDIT_CMD1, m_xvEditCmd1);
	DDX_Text(pDX, IDC_EDIT_CMD2, m_xvEditCmd2);
	DDX_Control(pDX, IDC_COMBO_CMD1, m_xcCombo_Cmd1);
	DDX_Control(pDX, IDC_COMBO_CMD2, m_xcCombo_Cmd2);
	DDX_CBIndex(pDX, IDC_COMBO_CMD1, m_xvCombo_Cmd1);
	DDX_CBIndex(pDX, IDC_COMBO_CMD2, m_xvCombo_Cmd2);
}


BEGIN_MESSAGE_MAP(CPropMain, CPropertyPage)
	ON_BN_CLICKED(IDC_PBTN_ASUP, &CPropMain::OnBnClickedPbtnUpAs)
	ON_BN_CLICKED(IDC_PBTN_ASDW, &CPropMain::OnBnClickedPbtnDwAs)
	ON_LBN_SELCHANGE(IDC_LIST_AS, &CPropMain::OnLbnSelchangeListAs)
	ON_BN_CLICKED(IDC_PBTN_ADDENTRY, &CPropMain::OnBnClickedPbtnAddentry)
	ON_BN_CLICKED(IDC_PBTN_WRITE, &CPropMain::OnBnClickedPbtnWrite)
	ON_BN_CLICKED(IDC_PBTN_REFRECT, &CPropMain::OnBnClickedPbtnReflect)
	ON_BN_CLICKED(IDC_PBTN_COPY, &CPropMain::OnBnClickedPbtnCopy)
END_MESSAGE_MAP()


// CPropMain メッセージ ハンドラ

/////////////////////////////////////////////////
//  OK button handler
//
void CPropMain::OnOK()
{
	// TODO: ここに特定なコードを追加するか、もしくは基本クラスを呼び出してください。

	
	
//    UpdateData();
//    AfxGetApp()->WriteProfileInt(_T("Option"), _T("kind"), m_xvRadioKind);
    
	CPropertyPage::OnOK();
}

/////////////////////////////////////////////////
// 
//
BOOL CPropMain::OnInitDialog()
{
	CPropertyPage::OnInitDialog();

	// TODO:  ここに初期化を追加してください
	
	CString str;
	str.LoadStringW(IDS_COMMAND_PUT);m_strlCmd.AddTail(str);
	str.LoadStringW(IDS_COMMAND_MPUT);m_strlCmd.AddTail(str);
	str.LoadStringW(IDS_COMMAND_GET);m_strlCmd.AddTail(str);
	str.LoadStringW(IDS_COMMAND_MGET);m_strlCmd.AddTail(str);
	str.LoadStringW(IDS_COMMAND_DEL);m_strlCmd.AddTail(str);
	str.LoadStringW(IDS_COMMAND_MKDIR);m_strlCmd.AddTail(str);
	str.LoadStringW(IDS_COMMAND_RMDIR);m_strlCmd.AddTail(str);
	str.LoadStringW(IDS_COMMAND_BYE);m_strlCmd.AddTail(str);
	str.LoadStringW(IDS_COMMAND_QUIT);m_strlCmd.AddTail(str);
	m_strlCmd.AddTail(_T(""));	// Set blank string in the last

    {
        int        err = 0, cbErr = 0, i;

		POSITION pos = m_strlCmd.GetHeadPosition();
		for (i=0; i<m_strlCmd.GetCount(); i++)
		{
			str = m_strlCmd.GetNext(pos);
			cbErr = m_xcCombo_Cmd1.InsertString(-1, str);
			cbErr = m_xcCombo_Cmd2.InsertString(-1, str);
            if (cbErr == CB_ERR || cbErr == CB_ERRSPACE) err = 1;
		}
    }

	m_xcPbtnUp.SetIcon(AfxGetApp()->LoadIcon(IDI_PROPUP));
	m_xcPbtnDw.SetIcon(AfxGetApp()->LoadIcon(IDI_PROPDOWN));

	str.LoadStringW(IDS_MAIN_OPERATIONS);SetDlgItemText(IDC_GB_OPS, str);
	str.LoadStringW(IDS_MAIN_ADDENTRY);SetDlgItemText(IDC_PBTN_ADDENTRY, str);
	str.LoadStringW(IDS_MAIN_REFRECT);SetDlgItemText(IDC_PBTN_REFRECT, str);
	str.LoadStringW(IDS_MAIN_WRITE);SetDlgItemText(IDC_PBTN_WRITE, str);
	str.LoadStringW(IDS_MAIN_COPY);SetDlgItemText(IDC_PBTN_COPY, str);

	return TRUE;  // return TRUE unless you set the focus to a control
	// 例外 : OCX プロパティ ページは必ず FALSE を返します。
}

/////////////////////////////////////////////////
// 
//
BOOL CPropMain::UpdateASView(int nSelect)
{
	POSITION pos = theApp.m_pActivitySet->GetHeadPosition();
	CString	str;
	CStringList *strl;
	int        err = 0, lbErr = 0;

	// 既に表示されているリストをクリアする
	m_xcListAS.ResetContent();

	while(pos)
	{
		if (!err)
		{
			strl = (CStringList *)theApp.m_pActivitySet->GetAt(pos);
			lbErr = m_xcListAS.InsertString(-1, strl->GetHead().TrimLeft());
			if (lbErr == LB_ERR || lbErr == LB_ERRSPACE) err = 1;
		}
		theApp.m_pActivitySet->GetNext(pos);	// if end of list, pos = NULL
	}
	
	if (!err)
	{
		m_xcListAS.SetCurSel(nSelect);	// 指定行をハイライト表示
		ShowASEntries(nSelect);			// コントロールをリセット
	}

	return TRUE;
}

/////////////////////////////////////////////////
// UP button handler
//
void CPropMain::OnBnClickedPbtnUpAs()
{
	CStringList *strl = NULL;
	int	iIndex = m_xcListAS.GetCurSel();	// Get the current highlighted line
	CObList *pAS = theApp.m_pActivitySet;

	if (iIndex != 0)
	{
		POSITION pos = pAS->FindIndex(iIndex-1);	// Set POS at upper floor
		strl = (CStringList *)pAS->GetAt(pos);		// Evacuation
		pAS->SetAt(pAS->FindIndex(iIndex-1), (CStringList *)pAS->GetAt(pAS->FindIndex(iIndex)));
		pAS->SetAt(pAS->FindIndex(iIndex), strl);	// Copy 
		
		UpdateASView(iIndex-1);			// AS List Viewを再描画
	}
}

/////////////////////////////////////////////////
// Down button handler
//
void CPropMain::OnBnClickedPbtnDwAs()
{
	CStringList *strl = NULL;
	int	iIndex = m_xcListAS.GetCurSel();	// Get the current highlighted line
	CObList *pAS = theApp.m_pActivitySet;

	if (iIndex != pAS->GetCount()-1)
	{
		POSITION pos = pAS->FindIndex(iIndex+1);	// Set POS at lower floor
		strl = (CStringList *)pAS->GetAt(pos);		// Evacuation
		pAS->SetAt(pAS->FindIndex(iIndex+1), (CStringList *)pAS->GetAt(pAS->FindIndex(iIndex)));
		pAS->SetAt(pAS->FindIndex(iIndex), strl);	// Copy 
		
		UpdateASView(iIndex+1);			// AS List Viewを再描画
	}
}

/////////////////////////////////////////////////
// AS-List-Selected event handler
//
void CPropMain::OnLbnSelchangeListAs()
{
	int	iIndex = m_xcListAS.GetCurSel();
	ShowASEntries(iIndex);
}

/////////////////////////////////////////////////
//  選択されたASの詳細情報を右ペインに表示する
//
bool CPropMain::ShowASEntries(int iIndex)
{
	CString str, strBye, strQuit, str2;
	CStringList *strl = NULL;
	CObList *pAS = theApp.m_pActivitySet;
	POSITION pos, pos2;

	// Once, clear/reset all controls
	ResetAllControls();

	// 最上行、最下行の場合の上下ボタン非表示の処理
	if (iIndex == 0) GetDlgItem(IDC_PBTN_ASUP)->EnableWindow(false);
	else if (iIndex == pAS->GetCount()-1) GetDlgItem(IDC_PBTN_ASDW)->EnableWindow(false);


	strl = (CStringList *)pAS->GetAt(pAS->FindIndex(iIndex));
	
	pos = strl->GetHeadPosition();
	strBye.LoadStringW(IDS_COMMAND_BYE);
	strQuit.LoadStringW(IDS_COMMAND_QUIT);

	// AddEntryでstr=空白行が来た場合の処理
	str = strl->GetNext(pos).TrimLeft();
	if (str.IsEmpty())
	{
		ResetAllControls();
	}

	// bye, quit の列が来た場合の処理
	else if (str.Find(strBye, 0) == 0 || str.Find(strQuit, 0) == 0 )
	{
		pos2 = m_strlCmd.GetHeadPosition();
		for (int i=0; i<m_strlCmd.GetCount(); i++)
		{
			str2 = m_strlCmd.GetNext(pos2);
			if (str.Find(str2, 0) == 0)
			{
				m_xvCombo_Cmd1 = i;	// Combo box
				GetDlgItem(IDC_TXT_CD)->EnableWindow(false);
				GetDlgItem(IDC_EDIT_CD)->EnableWindow(false);
				GetDlgItem(IDC_TXT_LCD)->EnableWindow(false);
				GetDlgItem(IDC_EDIT_LCD)->EnableWindow(false);
				GetDlgItem(IDC_EDIT_CMD1)->EnableWindow(false);
				GetDlgItem(IDC_COMBO_CMD2)->EnableWindow(false);
				GetDlgItem(IDC_EDIT_CMD2)->EnableWindow(false);
				break;
			}
		}
	}

	// 以外のコマンド列が来た場合
	else
	{
		// cd
		str2.LoadStringW(IDS_COMMAND_CD);
		if (str.Find(str2, 0) == 0)
		{
			str.Delete(0, str2.GetLength());
			m_xvEditCd = str.TrimLeft();
		}
		else
		{
		}

		// lcd
		if (pos)
		{
			str = strl->GetNext(pos).TrimLeft();
			str2.LoadStringW(IDS_COMMAND_LCD);
			if (str.Find(str2, 0) == 0)
			{
				str.Delete(0, str2.GetLength());
				m_xvEditLcd = str.TrimLeft();
			}
			else
			{
			}
		}
		else
			goto End;

		//Commands1
		if(pos)
		{
			str = strl->GetNext(pos).TrimLeft();
			pos2 = m_strlCmd.GetHeadPosition();
			for (int i=0; i<m_strlCmd.GetCount(); i++)
			{
				str2 = m_strlCmd.GetNext(pos2);
				if (str.Find(str2, 0) == 0)
				{
					str.Delete(0, str2.GetLength());
					m_xvEditCmd1 = str.TrimLeft();
					m_xvCombo_Cmd1 = i;	// Set position of pull down list
					break;
				}
				else
				{
					// ToDo: プルダウンにない場合の処理
					// remの場合
					// 空白行の場合
					// 登録されていないコマンドの場合
				}
			}
		}
		else
			goto End;

		//Commands2
		if(pos)
		{
			str = strl->GetNext(pos).TrimLeft();
			pos2 = m_strlCmd.GetHeadPosition();
			for (int i=0; i<m_strlCmd.GetCount(); i++)
			{
				str2 = m_strlCmd.GetNext(pos2);
				if (str.Find(str2, 0) == 0)
				{
					str.Delete(0, str2.GetLength());
					m_xvEditCmd2 = str.TrimLeft();
					m_xvCombo_Cmd2 = i;	// Set position of pull down list
					break;
				}
				else
				{
					// ToDo: プルダウンにない場合の処理
				}
			}
		}
		else
			goto End;

	}

End:
	UpdateData(FALSE);

	return TRUE;
}

/////////////////////////////////////////////////
// AddEntry button event handler
//
void CPropMain::OnBnClickedPbtnAddentry()
{
	POSITION pos = NULL;
	CObList *pAS = theApp.m_pActivitySet;
	CStringList	*pStrl;
	CString str;

	// 追加button時の処理
	if (m_bAddEntryState)
	{
		// byeの前に１エントリ追加します。
		pos = pAS->GetTailPosition();
		if (pos)
		{
			pStrl = new CStringList();
			pStrl->AddTail(_T(""));
			pos = pAS->InsertBefore(pos, pStrl);

			// Clear/reset all controls and Visible
			ResetAllControls();

			UpdateASView(pAS->GetCount()-2);
		}

		// 上下ボタンをDiable
		GetDlgItem(IDC_PBTN_ASUP)->EnableWindow(false);
		GetDlgItem(IDC_PBTN_ASDW)->EnableWindow(false);

		// ボタンの表示を変更
		str.LoadStringW(IDS_MAIN_UNDO);SetDlgItemText(IDC_PBTN_ADDENTRY, str);
	}
	// 取りやめbutton時の処理
	else
	{
		int	iIndex = m_xcListAS.GetCurSel();	// Get the current highlighted line
		pos = pAS->FindIndex(iIndex);
		if (pos)
		{
			pAS->RemoveAt(pos);
			UpdateASView(0);		// Go top line, acceptable?
		}
		// ボタンの表示を変更
		str.LoadStringW(IDS_MAIN_ADDENTRY);SetDlgItemText(IDC_PBTN_ADDENTRY, str);
	}

	m_bAddEntryState = !m_bAddEntryState;	// 状態の反転

}

/////////////////////////////////////////////////
// Reflect button event handler
//
void CPropMain::OnBnClickedPbtnReflect()
{
	// 右ペインに表示されている情報を選択されているASに反映する
	int	iIndex = m_xcListAS.GetCurSel();	// Get the current highlighted line
	CStringList *strl = NULL;
	CObList *pAS = theApp.m_pActivitySet;

	CString str, str2, str3;
	int i = 0;
	POSITION pos = pAS->FindIndex(iIndex);
	strl = (CStringList *)pAS->GetAt(pos);

	UpdateData();

	// IDS_COMMAND_CD
	if(m_xvEditCd.IsEmpty())
	{
		str.LoadStringW(IDS_COMMAND_CD);
		str2.LoadStringW(IDS_MSG_MAININVALIDINPUT);str3.Format(str2, str);AfxMessageBox(str3);
		goto End;
	}
	str.Empty();
	str2.LoadStringW(IDS_COMMAND_CD);
	str.Format(_T("%s %s"), str2, m_xvEditCd);
	if (strl->FindIndex(i) == NULL)
	{
		strl->AddTail(str);
		i++;
	}
	else
		strl->SetAt(strl->FindIndex(i++), str);

	// IDS_COMMAND_LCD
	if(m_xvEditLcd.IsEmpty())
	{
		str.LoadStringW(IDS_COMMAND_LCD);
		str2.LoadStringW(IDS_MSG_MAININVALIDINPUT);str3.Format(str2, str);AfxMessageBox(str3);
		goto End;
	}
	str.Empty();
	str2.LoadStringW(IDS_COMMAND_LCD);
	str.Format(_T("%s %s"), str2, m_xvEditLcd);
	if (strl->FindIndex(i) == NULL)
	{
		strl->AddTail(str);
		i++;
	}
	else
		strl->SetAt(strl->FindIndex(i++), str);

	// IDC_COMBO_CMD1, IDC_EDIT_CMD1
	str.Empty();
	m_xcCombo_Cmd1.GetLBText(m_xvCombo_Cmd1, str2);
	if(str2.IsEmpty() && m_xvEditCmd1.IsEmpty())
		goto End;
	str.Format(_T("%s %s"), str2, m_xvEditCmd1);
	if (strl->FindIndex(i) == NULL)
	{
		strl->AddTail(str);
		i++;
	}
	else
		strl->SetAt(strl->FindIndex(i++), str);

	// IDC_COMBO_CMD2, IDC_EDIT_CMD2
	str.Empty();
	m_xcCombo_Cmd2.GetLBText(m_xvCombo_Cmd2, str2);
	if(str2.IsEmpty() && m_xvEditCmd2.IsEmpty())
		goto End;
	str.Format(_T("%s %s"), str2, m_xvEditCmd2);
	if (strl->FindIndex(i) == NULL)
	{
		strl->AddTail(str);
		i++;
	}
	else
		strl->SetAt(strl->FindIndex(i++), str);
End:
	UpdateASView(iIndex);			// AS List Viewを再描画
}


/////////////////////////////////////////////////
// Write button event handler
//
void CPropMain::OnBnClickedPbtnWrite()
{
	CStdioFile    stdFile;
	CString       wstr, rstr, str, str2;
	LPTSTR        rstrBuf = NULL;
	int           err = 0;

	// (1)読み書き用にオープン
	if (!err)
	{
		if (!stdFile.Open(theApp.m_strScriptFile, 
			CFile::modeReadWrite | CFile::shareExclusive | 
			CFile::modeCreate)) err = 1;
	}
	// (2)書き込み
	if (!err)
	{
		CObList *pAS = theApp.m_pActivitySet;

		POSITION pos = NULL, posAS = NULL;
		CString	str;
		CStringList *strl;
		int        err = 0, lbErr = 0;

		posAS = pAS->GetHeadPosition();
		wstr.Empty();
		while(posAS)
		{
			strl = (CStringList *)pAS->GetAt(posAS);
			pos = strl->GetHeadPosition();
			while(pos)
			{
				wstr.Append(strl->GetAt(pos));
				wstr.Append(_T("\n"));
				strl->GetNext(pos);
			}
			wstr.Append(_T("\n"));
			pAS->GetNext(posAS);	// if end of list, pos = NULL
		}
		TRY {stdFile.WriteString(wstr);}
		CATCH (CFileException, eP) {err = 1;}
		END_CATCH

		str.LoadStringW(IDS_MSG_WRITESUCCESS);str2.Format(str, theApp.m_strScriptFile);
		AfxMessageBox(str2, MB_ICONINFORMATION | MB_OK, 0);

		GetDlgItem(IDC_PBTN_WRITE)->EnableWindow(true);
	}
}

/////////////////////////////////////////////////
// Copy button event handler
//
void CPropMain::OnBnClickedPbtnCopy()
{
	CStringList *pNewStrl = NULL, *pStrl=NULL;
	int	iSelected = m_xcListAS.GetCurSel();	// Get the current highlighted line
	CObList *pAS = theApp.m_pActivitySet;
	POSITION pos = NULL;
	CString str;

	pStrl = (CStringList *)pAS->GetAt(pAS->FindIndex(iSelected));
	pos = pStrl->GetHeadPosition();

	// 現在の行の後ろに１エントリ追加します。
	pNewStrl = new CStringList();
	while(pos)
	{
		str = pStrl->GetNext(pos);	// Get data at pos, then inc pos
		pNewStrl->AddTail(str);		// Add data at tail
	}

	pos = pAS->InsertAfter(pAS->FindIndex(iSelected), pNewStrl);

	// Clear/reset all controls and Visible
	ResetAllControls();

	UpdateASView(iSelected+1);	// Display data of new line
}

/////////////////////////////////////////////////
// Reset all control state on Prop sheet to the initial one
//
void CPropMain::ResetAllControls(void)
{
	GetDlgItem(IDC_TXT_CD)->EnableWindow(true);
	GetDlgItem(IDC_EDIT_CD)->EnableWindow(true);
	GetDlgItem(IDC_TXT_LCD)->EnableWindow(true);
	GetDlgItem(IDC_EDIT_LCD)->EnableWindow(true);
	GetDlgItem(IDC_COMBO_CMD1)->EnableWindow(true);
	GetDlgItem(IDC_EDIT_CMD1)->EnableWindow(true);
	GetDlgItem(IDC_COMBO_CMD2)->EnableWindow(true);
	GetDlgItem(IDC_EDIT_CMD2)->EnableWindow(true);
	GetDlgItem(IDC_PBTN_ASUP)->EnableWindow(true);
	GetDlgItem(IDC_PBTN_ASDW)->EnableWindow(true);
	GetDlgItem(IDC_PBTN_WRITE)->EnableWindow(false);
	m_xvEditCd.Empty();
	m_xvEditLcd.Empty();
	m_xvEditCmd1.Empty();
	m_xvEditCmd2.Empty();
	m_xvCombo_Cmd1 = m_strlCmd.GetCount()-1;
	m_xvCombo_Cmd2 = m_strlCmd.GetCount()-1;
}



int CPropMain::RetrieveInputData(void)
{
	return 0;
}

