// ScrEditDlg.cpp : 実装ファイル
//

#include "stdafx.h"
#include "ScrEdit.h"
#include "ScrEditDlg.h"
#include "ReadScrDlg.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#endif


// アプリケーションのバージョン情報に使われる CAboutDlg ダイアログ

class CAboutDlg : public CDialog
{
public:
	CAboutDlg();

// ダイアログ データ
	enum { IDD = IDD_ABOUTBOX };

	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV サポート

// 実装
protected:
	DECLARE_MESSAGE_MAP()
};

CAboutDlg::CAboutDlg() : CDialog(CAboutDlg::IDD)
{
}

void CAboutDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
}

BEGIN_MESSAGE_MAP(CAboutDlg, CDialog)
END_MESSAGE_MAP()


// CScrEditDlg ダイアログ




CDlgScrEdit::CDlgScrEdit(CWnd* pParent /*=NULL*/)
//	: CPropertySheet(CDlgScrEdit::IDD, pParent)
	: CPropertySheet(_T(""), pParent)	// Edited for Propertysheet
{
	m_hIcon = AfxGetApp()->LoadIcon(IDR_MAINFRAME);

	m_pReadSrcDlg = NULL;

	static CString strMain, strConfig;
	CString str;


	// Invisible Help button
	this->m_psh.dwFlags &= ~PSH_HASHELP;
	m_propMain.m_psp.dwFlags &= ~PSP_HASHELP;
	m_propConfig.m_psp.dwFlags &= ~PSP_HASHELP;


	m_propMain.m_psp.dwFlags |= PSP_USETITLE;
	strMain.LoadStringW(IDS_MAIN_PROPMAIN);m_propMain.m_psp.pszTitle = strMain;		// 定数文字列として指定

	m_propConfig.m_psp.dwFlags |= PSP_USETITLE;
	strConfig.LoadStringW(IDS_CONFIG_PROPCONFIG);m_propConfig.m_psp.pszTitle = strConfig;	// 定数文字列として指定

	AddPage(&m_propMain);
    AddPage(&m_propConfig);

	str.LoadStringW(IDS_PROGRAMNAME);SetTitle(str, 0);

}

void CDlgScrEdit::DoDataExchange(CDataExchange* pDX)
{
	CPropertySheet::DoDataExchange(pDX);
}

BEGIN_MESSAGE_MAP(CDlgScrEdit, CPropertySheet)
	ON_WM_SYSCOMMAND()
	ON_WM_PAINT()
	ON_WM_QUERYDRAGICON()
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()


// CScrEditDlg メッセージ ハンドラ

BOOL CDlgScrEdit::OnInitDialog()
{
	CPropertySheet::OnInitDialog();

	// "バージョン情報..." メニューをシステム メニューに追加します。

	// IDM_ABOUTBOX は、システム コマンドの範囲内になければなりません。
	ASSERT((IDM_ABOUTBOX & 0xFFF0) == IDM_ABOUTBOX);
	ASSERT(IDM_ABOUTBOX < 0xF000);

	CMenu* pSysMenu = GetSystemMenu(FALSE);
	if (pSysMenu != NULL)
	{
		CString strAboutMenu;
		strAboutMenu.LoadString(IDS_ABOUTBOX);
		if (!strAboutMenu.IsEmpty())
		{
			pSysMenu->AppendMenu(MF_SEPARATOR);
			pSysMenu->AppendMenu(MF_STRING, IDM_ABOUTBOX, strAboutMenu);
		}
	}

	// このダイアログのアイコンを設定します。アプリケーションのメイン ウィンドウがダイアログでない場合、
	//  Framework は、この設定を自動的に行います。
	SetIcon(m_hIcon, TRUE);			// 大きいアイコンの設定
	SetIcon(m_hIcon, FALSE);		// 小さいアイコンの設定

	// TODO: 初期化をここに追加します。
	//{
	//	CDialogPropSheet   cPropSheet(_T(""), this);
	//	bool            main;
	//	unsigned int    config;
	//	CString         str;
	//    
	//	if (cPropSheet.DoModal() == IDOK)
	//	{
	//		main = (AfxGetApp()->GetProfileInt(_T("Option"), _T("Main"), 0)) ? true : false;
	//		config = AfxGetApp()->GetProfileInt(_T("Option"), _T("Config"), 0);

	//		//m_xvEditRes = (kind == 0) ? _T("テキスト") : 
	//		//    (kind == 1) ? _T("画像") : _T("画像とテキスト");
	//		//m_xvEditRes = m_xvEditRes +_T("が表示") 
	//		//    +(disp ? _T("されます。") : _T("されません。"));
	//		UpdateData(FALSE);
	//	}
	//}


	SendMessage (DM_SETDEFID, IDC_LIST_AS);	// Default focus set

	// ReadScr 子ダイアログウィンドウの表示
	if (!theApp.m_bDefaultScriptFile)
	{
		m_pReadSrcDlg = new CReadScrDlg(this);

		//if (m_pReadSrcDlg->DoModal() == IDOK){
		//}

		m_pReadSrcDlg->ShowWindow(true);
	}
	else
	{
		// Script Fileの読み込み
		theApp.ReadScrFile(theApp.m_strScriptFile);
	}

	return FALSE;  // フォーカスをコントロールに設定した場合を除き、TRUE を返します。
}

void CDlgScrEdit::OnSysCommand(UINT nID, LPARAM lParam)
{
	if ((nID & 0xFFF0) == IDM_ABOUTBOX)
	{
		CAboutDlg dlgAbout;
		dlgAbout.DoModal();
	}
	else
	{
		CPropertySheet::OnSysCommand(nID, lParam);
	}
}

// ダイアログに最小化ボタンを追加する場合、アイコンを描画するための
//  下のコードが必要です。ドキュメント/ビュー モデルを使う MFC アプリケーションの場合、
//  これは、Framework によって自動的に設定されます。

void CDlgScrEdit::OnPaint()
{
	if (IsIconic())
	{
		CPaintDC dc(this); // 描画のデバイス コンテキスト

		SendMessage(WM_ICONERASEBKGND, reinterpret_cast<WPARAM>(dc.GetSafeHdc()), 0);

		// クライアントの四角形領域内の中央
		int cxIcon = GetSystemMetrics(SM_CXICON);
		int cyIcon = GetSystemMetrics(SM_CYICON);
		CRect rect;
		GetClientRect(&rect);
		int x = (rect.Width() - cxIcon + 1) / 2;
		int y = (rect.Height() - cyIcon + 1) / 2;

		// アイコンの描画
		dc.DrawIcon(x, y, m_hIcon);
	}
	else
	{
		CPropertySheet::OnPaint();
	}
}

// ユーザーが最小化したウィンドウをドラッグしているときに表示するカーソルを取得するために、
//  システムがこの関数を呼び出します。
HCURSOR CDlgScrEdit::OnQueryDragIcon()
{
	return static_cast<HCURSOR>(m_hIcon);
}



