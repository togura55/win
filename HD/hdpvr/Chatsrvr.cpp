// chatsrvr.cpp : Defines the class behaviors for the application.
//
// This is a part of the Microsoft Foundation Classes C++ library.
// Copyright (C) 1992-1998 Microsoft Corporation
// All rights reserved.
//
// This source code is only intended as a supplement to the
// Microsoft Foundation Classes Reference and related
// electronic documentation provided with the library.
// See these sources for detailed information regarding the
// Microsoft Foundation Classes product.

#include "stdafx.h"
#include "chatsrvr.h"

#include "mainfrm.h"
#include "srvrdoc.h"
#include "srvrvw.h"
#include "Proginfo.h"
#include "RecListView.h"


#ifdef _DEBUG
#undef THIS_FILE
static char BASED_CODE THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CServerApp

BEGIN_MESSAGE_MAP(CServerApp, CWinApp)
	//{{AFX_MSG_MAP(CServerApp)
	ON_COMMAND(ID_APP_ABOUT, OnAppAbout)
		// NOTE - the ClassWizard will add and remove mapping macros here.
		//    DO NOT EDIT what you see in these blocks of generated code!
	//}}AFX_MSG_MAP
	// Standard file based document commands
	ON_COMMAND(ID_FILE_NEW, CWinApp::OnFileNew)
	ON_COMMAND(ID_FILE_OPEN, CWinApp::OnFileOpen)
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CServerApp construction

CServerApp::CServerApp()
{
	m_pProgramInfo = new CObList();
}

/////////////////////////////////////////////////////////////////////////////
// The one and only CServerApp object

CServerApp theApp;

/////////////////////////////////////////////////////////////////////////////
// CServerApp initialization

BOOL CServerApp::InitInstance()
{
	if (!AfxSocketInit())
	{
		AfxMessageBox(IDP_SOCKETS_INIT_FAILED);
		return FALSE;
	}

	// Read registry settings
	ReadRegSettings();

	// 本デバイスの固有情報を保存
	m_iDeviceID = 1;					// デバイスID
	m_strDeviceDesc = _T("HDPVR001");	// デバイスの説明

	// Standard initialization
	// If you are not using these features and wish to reduce the size
	//  of your final executable, you should remove from the following
	//  the specific initialization routines you do not need.

	// Initialize static members of CSocketThread

//#ifdef _WIN32
//	Enable3dControls();
//#endif

	LoadStdProfileSettings();  // Load standard INI file options (including MRU)

	// Register the application's document templates.  Document templates
	//  serve as the connection between documents, frame windows and views.

	CSingleDocTemplate* pDocTemplate;
	pDocTemplate = new CSingleDocTemplate(
		IDR_MAINFRAME,
		RUNTIME_CLASS(CServerDoc),
		RUNTIME_CLASS(CMainFrame),       // main SDI frame window
		RUNTIME_CLASS(CServerView));
	AddDocTemplate(pDocTemplate);

	// create a new (empty) document
	OnFileNew();

	return TRUE;
}

int CServerApp::ExitInstance()
{
	WriteRegSettings();

	return CWinApp::ExitInstance();
}

/////////////////////////////////////////////////////////////////////////////
// CAboutDlg dialog used for App About

class CAboutDlg : public CDialog
{
public:
	CAboutDlg();

// Dialog Data
	//{{AFX_DATA(CAboutDlg)
	enum { IDD = IDD_ABOUTBOX };
	//}}AFX_DATA

// Implementation
protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//{{AFX_MSG(CAboutDlg)
		// No message handlers
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
};

CAboutDlg::CAboutDlg() : CDialog(CAboutDlg::IDD)
{
	//{{AFX_DATA_INIT(CAboutDlg)
	//}}AFX_DATA_INIT
}

void CAboutDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CAboutDlg)
	//}}AFX_DATA_MAP
}

BEGIN_MESSAGE_MAP(CAboutDlg, CDialog)
	//{{AFX_MSG_MAP(CAboutDlg)
		// No message handlers
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

// App command to run the dialog
void CServerApp::OnAppAbout()
{
	CAboutDlg aboutDlg;
	aboutDlg.DoModal();
}

// **************************************************
// レジストリに記録してあるプライベート情報の読み込み
//
// **************************************************
bool CServerApp::ReadRegSettings(void)
{
	// 設定が格納されているレジストリ キーを変更します。
	// TODO: この文字列を、会社名または組織名などの、
	// 適切な文字列に変更してください。
	SetRegistryKey(_T("Arugo"));

	// サブキー下の指定したキーの値（文字列）の取得。アプリ名は暗黙で設定される
	m_iNumRecProg = GetProfileInt(_T("RecPrograms"), _T("NumRec"), 0);
	int i;
	CProgInfo*	pPI;

	CString str;
	for (i=0; i<m_iNumRecProg;  i++)
	{
		pPI = new CProgInfo();
		m_pProgramInfo->AddTail(pPI);	// リストアイテムを１つ追加

		str.Format("%s\\%d", _T("RecPrograms"), i);
		pPI->m_strTitle = GetProfileString(str, _T("Title"), _T(""));
		pPI->m_strDesc = GetProfileString(str, _T("Description"), _T(""));
		pPI->m_strCategory = GetProfileString(str, _T("Category"), _T(""));
		pPI->m_strStation = GetProfileString(str, _T("Station"), _T(""));
		pPI->m_tiStart = CTime(GetProfileIntA(str, _T("StartTime64T"), 0));
		pPI->m_tiEnd = CTime(GetProfileIntA(str, _T("EndTime64T"), 0));

		pPI->m_iStat = 0;	// For the temporary
		pPI->m_iId = 0;		// For the temporary
	}

	return TRUE;
}

// **************************************************
// レジストリへのプライベート情報の書き込み
//
// **************************************************
bool CServerApp::WriteRegSettings(void)
{
	int i;
	CString	str;
	CServerApp* pApp = (CServerApp*)AfxGetApp();
	POSITION pos = pApp->m_pProgramInfo->GetHeadPosition();
	CProgInfo*	pPI;
	
	pApp->WriteProfileInt(_T("RecPrograms"), _T("NumRec"), pApp->m_iNumRecProg);

	for (i=0; i<pApp->m_iNumRecProg; i++)
	{
		pPI = (CProgInfo *)pApp->m_pProgramInfo->GetAt(pos);
		str.Format("%s\\%d", _T("RecPrograms"), i);
		pApp->WriteProfileStringA(str, "Title",			pPI->m_strTitle);
		pApp->WriteProfileStringA(str, "Description",	pPI->m_strDesc);
		pApp->WriteProfileStringA(str, "Category",		pPI->m_strCategory);
		pApp->WriteProfileStringA(str, "Station",		pPI->m_strStation);
		pApp->WriteProfileInt(str, "StartTime64T",	(int)pPI->m_tiStart.GetTime());
		pApp->WriteProfileInt(str, "EndTime64T",	(int)pPI->m_tiEnd.GetTime());

		//pApp->WriteProfileInt(str, "Status",	(int)iPI->m_iStat);
		//pApp->WriteProfileInt(str, "ID",		(int)iPI->m_iId);

		pApp->m_pProgramInfo->GetNext(pos);	// 次のリストへ
	}

	return TRUE;
}

