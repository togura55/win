
// DLLLodDlg.cpp : 実装ファイル
//

#include "stdafx.h"
#include "DLLLod.h"
#include "DLLLodDlg.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#endif


// CDLLLodDlg ダイアログ




CDLLLodDlg::CDLLLodDlg(CWnd* pParent /*=NULL*/)
	: CDialog(CDLLLodDlg::IDD, pParent)
{
	m_hIcon = AfxGetApp()->LoadIcon(IDR_MAINFRAME);
}

void CDLLLodDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
}

BEGIN_MESSAGE_MAP(CDLLLodDlg, CDialog)
	ON_WM_PAINT()
	ON_WM_QUERYDRAGICON()
	//}}AFX_MSG_MAP
	ON_BN_CLICKED(IDC_BUTTON1, &CDLLLodDlg::OnBnClickedButton1)
END_MESSAGE_MAP()


// CDLLLodDlg メッセージ ハンドラ

BOOL CDLLLodDlg::OnInitDialog()
{
	CDialog::OnInitDialog();

	// このダイアログのアイコンを設定します。アプリケーションのメイン ウィンドウがダイアログでない場合、
	//  Framework は、この設定を自動的に行います。
	SetIcon(m_hIcon, TRUE);			// 大きいアイコンの設定
	SetIcon(m_hIcon, FALSE);		// 小さいアイコンの設定

	// TODO: 初期化をここに追加します。

	return TRUE;  // フォーカスをコントロールに設定した場合を除き、TRUE を返します。
}

// ダイアログに最小化ボタンを追加する場合、アイコンを描画するための
//  下のコードが必要です。ドキュメント/ビュー モデルを使う MFC アプリケーションの場合、
//  これは、Framework によって自動的に設定されます。

void CDLLLodDlg::OnPaint()
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
		CDialog::OnPaint();
	}
}

// ユーザーが最小化したウィンドウをドラッグしているときに表示するカーソルを取得するために、
//  システムがこの関数を呼び出します。
HCURSOR CDLLLodDlg::OnQueryDragIcon()
{
	return static_cast<HCURSOR>(m_hIcon);
}



typedef BSTR (__stdcall GETVERS)( BSTR *);
typedef BSTR (__stdcall FINDUSB)( int *);

void CDLLLodDlg::OnBnClickedButton1()
{
	HINSTANCE	hInst;
	GETVERS		*pfnDllFuncGetVers;
	FINDUSB		*pfnDllFuncFindUSB;

	hInst = ::LoadLibrary(_T("USBMeter") );	//DLLを読み込みます。
	if( hInst == NULL )
	{
		MessageBox(_T("ＤＬＬをロードできませんでした。") );
		return;
	}

	pfnDllFuncGetVers = (GETVERS *)::GetProcAddress( hInst, "_GetVers@4" );	//関数のアドレスを取得します。
	pfnDllFuncFindUSB = (FINDUSB *)::GetProcAddress( hInst, "_FindUSB@4" );	//関数のアドレスを取得します。
	if( pfnDllFuncGetVers == NULL)
	{
		MessageBox(_T("関数を取得できませんでした。") );
		::FreeLibrary( hInst );       //DLLを解放します。
		return;
	}

	BSTR str;			// Srring type compatibility w/ VB
	BSTR strParam;
	int index = 0;

//	static TCHAR s[MAX_PATH +1];
//strParam = s;

//	static TCHAR strParam[MAX_PATH +1];

	str = pfnDllFuncGetVers( &strParam );	//呼び出し！
	str = pfnDllFuncFindUSB( &index );	//呼び出し！

	::FreeLibrary( hInst ); 
}
