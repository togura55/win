// chatter.cpp : Defines the class behaviors for the application.
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
#include "chatter.h"
#include "setupdlg.h"
#include "resource.h"

#include "mainfrm.h"
#include "chatdoc.h"
#include "chatvw.h"
#include "Proginfo.h"
#include "Deviceinfo.h"
//#include "IPAddr.h"

//#include "iostream.h"
#include "winsock.h"

#ifdef _DEBUG
#undef THIS_FILE
static char BASED_CODE THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CChatterApp

BEGIN_MESSAGE_MAP(CChatterApp, CWinApp)
	//{{AFX_MSG_MAP(CChatterApp)
	ON_COMMAND(ID_OPEN_CONFIGDLG, OnConfigDlg)

	ON_COMMAND(ID_APP_ABOUT, OnAppAbout)
		// NOTE - the ClassWizard will add and remove mapping macros here.
		//    DO NOT EDIT what you see in these blocks of generated code!
	//}}AFX_MSG_MAP
	// Standard file based document commands
	ON_COMMAND(ID_FILE_NEW, CWinApp::OnFileNew)
	ON_COMMAND(ID_FILE_OPEN, CWinApp::OnFileOpen)
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CChatterApp construction

CChatterApp::CChatterApp()
{
	// プルダウンリスト表示用の既定のプログラムリスト
	m_pProgramInfo = new CObList();

	// ユーザー入力の通信パラメータの初期化
	m_nChannel = 0;
	m_strHandle = _T("");
	m_strServer = _T("");

	// デバイス情報
	// ToDo 本来ならDevice検知時に作成する
	m_pDeviceInfo = new CObList();
	
	CDeviceInfo	*pDI;

	pDI = new CDeviceInfo();
	m_pDeviceInfo->AddTail(pDI);

	char	ch[18];
	pDI->m_pchHdr = ch;
}

/////////////////////////////////////////////////////////////////////////////
// The one and only CChatterApp object

CChatterApp theApp;

/////////////////////////////////////////////////////////////////////////////
// CChatterApp initialization

BOOL CChatterApp::InitInstance()
{
	if (!AfxSocketInit())
	{
		AfxMessageBox(IDP_SOCKETS_INIT_FAILED);
		return FALSE;
	}

	// テスト用に番組データをCObListデータに格納
	SetProgramInfo();

	// Standard initialization
	// If you are not using these features and wish to reduce the size
	//  of your final executable, you should remove from the following
	//  the specific initialization routines you do not need.

//#ifdef _WIN32
//	Enable3dControls();
//#endif

	LoadStdProfileSettings();  // Load standard INI file options (including MRU)

	// Register the application's document templates.  Document templates
	//  serve as the connection between documents, frame windows and views.

	CSingleDocTemplate* pDocTemplate;
	pDocTemplate = new CSingleDocTemplate(
		IDR_MAINFRAME,
		RUNTIME_CLASS(CChatDoc),
		RUNTIME_CLASS(CMainFrame),       // main SDI frame window
		RUNTIME_CLASS(CChatView));
	AddDocTemplate(pDocTemplate);

	OnFileNew();

	return !(m_pMainWnd == NULL);
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
void CChatterApp::OnAppAbout()
{
	CAboutDlg aboutDlg;
	aboutDlg.DoModal();
}

/////////////////////////////////////////////////////////////////////////////
//
//  OnConfigDlg
//
void CChatterApp::OnConfigDlg()
{
	//CChatterApp* pApp = (CChatterApp*)AfxGetApp();
	CChatDoc* pCD = m_pChatDoc;
	CSetupDlg Dialog;

	BOOL bStat = TRUE;
	while(bStat == TRUE)
	{
		if (IDOK != Dialog.DoModal())
		{
			bStat = FALSE;
			break;
		}

		if (pCD->ConnectSocket(m_strHandle, m_strServer, m_nChannel))
		{
			bStat = FALSE;
			break;
		}

		if (AfxMessageBox(IDS_CHANGEADDRESS,MB_YESNO) == IDNO)
		{
			bStat = FALSE;
			break;
		}
	}
}

/////////////////////////////////////////////////////////////////////////////
// CChatterApp commands
// テスト用に番組データをCObListデータに格納
void CChatterApp::SetProgramInfo(void)
{
	CProgInfo*	pPI;
	int	i;

	for (i=0; i<3; i++)
	{
		pPI = new CProgInfo();
		m_pProgramInfo->AddTail(pPI);
	}

	POSITION pos = m_pProgramInfo->GetHeadPosition();

	pPI = (CProgInfo *)m_pProgramInfo->GetAt(pos);
	pPI->m_strTitle = _T("Ｊリーグ第１７節");	// 番組名
	pPI->m_strDesc = _T("Ｊリーグ第１７節 鹿島アントラーズ対ＦＣ東京　カシマスタジアム");	// 番組内容
	pPI->m_strCategory = _T("スポーツ");	// 番組カテゴリ
	pPI->m_strStation = _T("J SPORTS");	// 放送局名
	pPI->m_tiStart = ToCTime (2009, 10, 20, 23, 50, 0);		// 開始時間
	pPI->m_tiEnd = ToCTime (2009, 10, 21, 0, 50, 0);		// 終了時間
	pPI->m_iStat = 1;					// 録画要求状態
	pPI->m_iId = 0;						// リクエストID
	m_pProgramInfo->GetNext(pos);

	pPI = (CProgInfo *)m_pProgramInfo->GetAt(pos);
	pPI->m_strTitle = _T("映画「おくりびと」");
	pPI->m_strDesc = _T("人はだれでもいつか、おくりびと、おくられびととなる。第81回アカデミー賞外国語映画賞受賞作品");	// 番組内容
	pPI->m_strCategory = _T("映画");	// 番組カテゴリ
	pPI->m_strStation = _T("ＴＢＳ");	// 放送局名
	pPI->m_tiStart = ToCTime (2009, 11, 5, 21, 00, 0);		// 開始時間
	pPI->m_tiEnd = ToCTime (2009, 11, 5, 22, 54, 0);		// 終了時間
	pPI->m_iStat = 1;					// 録画要求状態
	pPI->m_iId = 0;						// リクエストID
	m_pProgramInfo->GetNext(pos);

	pPI = (CProgInfo *)m_pProgramInfo->GetAt(pos);
	pPI->m_strTitle = _T("天気予報");
	pPI->m_strDesc = _T("明日の天気");	// 番組内容
	pPI->m_strCategory = _T("情報");	// 番組カテゴリ
	pPI->m_strStation = _T("ＮＨＫ");	// 放送局名
	pPI->m_tiStart = ToCTime (2009, 10, 29, 5, 00, 0);		// 開始時間
	pPI->m_tiEnd = ToCTime (2009, 10, 29, 5, 05, 0);		// 終了時間
	pPI->m_iStat = 1;					// 録画要求状態
	pPI->m_iId = 0;						// リクエストID


	// For debug　手動入力が面倒なので、自動取得して入力する
	m_strHandle = _T("test");
    int retval = GetCurrentIPAddr(&m_strServer);
	TRACE("Handle:%s, IP Address:%s\n", m_strHandle, m_strServer);

}

/////////////////////////////////////////////////////////////////////////////
// GetCurrentIPAddr
// 開発時の使用目的で、Server, Clientが同じＰＣを使用している際に、自身のIP Address
//  がServer IP Addressと同じであることから、入力作業を省略するために、取得する。
int CChatterApp::GetCurrentIPAddr(CString *strIPAddr)
{
	int ret = 0;
	WSAData wsaData;
	char ac[80];

	{
		if (WSAStartup(MAKEWORD(1, 1), &wsaData) != 0) {
			return 1;
		}

		if (gethostname(ac, sizeof(ac)) == SOCKET_ERROR) {
			//cerr << "Error " << WSAGetLastError() <<
			//        " when getting local host name." << endl;
			return 1;
		}
		//cout << "Host name is " << ac << "." << endl;

		struct hostent *phe = gethostbyname(ac);
		if (phe == 0) {
			//cerr << "Yow! Bad host lookup." << endl;
			return 1;
		}

		for (int i = 0; phe->h_addr_list[i] != 0; ++i) {
			struct in_addr addr;
			memcpy(&addr, phe->h_addr_list[i], sizeof(struct in_addr));
			strIPAddr->Format("%s", inet_ntoa(addr));
		}

	}
	WSACleanup();

	return 0;
}



CTime CChatterApp::ToCTime(int y, int m, int d, int h, int mm, int s)
{
	CTime t(y, m, d, h, mm, s);

	return t;
}
