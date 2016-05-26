// ScrEdit.cpp : アプリケーションのクラス動作を定義します。
//

#include "stdafx.h"
#include "ScrEdit.h"
#include "ScrEditDlg.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#endif


// CScrEditApp

BEGIN_MESSAGE_MAP(CScrEditApp, CWinApp)
	ON_COMMAND(ID_HELP, &CWinApp::OnHelp)
END_MESSAGE_MAP()


// CScrEditApp コンストラクション

CScrEditApp::CScrEditApp()
{
	// TODO: この位置に構築用コードを追加してください。
	// ここに InitInstance 中の重要な初期化処理をすべて記述してください。
	
	m_bDefaultScriptFile = FALSE;	// 既定のスクリプトファイルが設定されているか
	m_pDlgSrcEdit = NULL;

	m_pActivitySet = new CObList();

}


// 唯一の CScrEditApp オブジェクトです。

CScrEditApp theApp;


// CScrEditApp 初期化

/////////////////////////////////////////////////
// 
//
BOOL CScrEditApp::InitInstance()
{
	// アプリケーション マニフェストが visual スタイルを有効にするために、
	// ComCtl32.dll Version 6 以降の使用を指定する場合は、
	// Windows XP に InitCommonControlsEx() が必要です。さもなければ、ウィンドウ作成はすべて失敗します。
	INITCOMMONCONTROLSEX InitCtrls;
	InitCtrls.dwSize = sizeof(InitCtrls);
	// アプリケーションで使用するすべてのコモン コントロール クラスを含めるには、
	// これを設定します。
	InitCtrls.dwICC = ICC_WIN95_CLASSES;
	InitCommonControlsEx(&InitCtrls);

	CWinApp::InitInstance();

	AfxEnableControlContainer();

	// 標準初期化
	// これらの機能を使わずに最終的な実行可能ファイルの
	// サイズを縮小したい場合は、以下から不要な初期化
	// ルーチンを削除してください。
	// 設定が格納されているレジストリ キーを変更します。
	// TODO: 会社名または組織名などの適切な文字列に
	// この文字列を変更してください。
//	SetRegistryKey(_T("アプリケーション ウィザードで生成されたローカル アプリケーション"));
	ReadRegSettings();

	CDlgScrEdit dlg;
	m_pDlgSrcEdit = &dlg;
	m_pMainWnd = &dlg;
	INT_PTR nResponse = dlg.DoModal();
	if (nResponse == IDOK)
	{
		// TODO: ダイアログが <OK> で消された時のコードを
		//  記述してください。
	}
	else if (nResponse == IDCANCEL)
	{
		// TODO: ダイアログが <キャンセル> で消された時のコードを
		//  記述してください。
	}

	// ダイアログは閉じられました。アプリケーションのメッセージ ポンプを開始しないで
	//  アプリケーションを終了するために FALSE を返してください。
	return FALSE;
}


/////////////////////////////////////////////////
// レジストリに記録してあるプライベート情報の読み込み
//
bool CScrEditApp::ReadRegSettings(void)
{
	CString str, strConfig;
	DWORD	dwData = 0;

	// 設定が格納されているレジストリ キーを変更します。
	// TODO: この文字列を、会社名または組織名などの、
	// 適切な文字列に変更してください。
	str.LoadStringW(IDS_REGKEYNAME);SetRegistryKey(str);

	// サブキー下の指定したキーの値（文字列）の取得。アプリ名は暗黙で設定される
	strConfig.LoadStringW(IDS_REGSUBKEY_CONFIG);

	str.LoadStringW(IDS_REGKEY_USEDEFAULTSCRIPTFILE);
	dwData = GetProfileInt(strConfig, str, 0);
	if (dwData == 0) m_bDefaultScriptFile = false; else m_bDefaultScriptFile = true; 

	str.LoadStringW(IDS_REGKEY_SCRIPTFILE);
	m_strScriptFile = GetProfileString(strConfig, str, _T(""));

	return TRUE;
}

/////////////////////////////////////////////////
// レジストリへのプライベート情報の書き込み
//
bool CScrEditApp::WriteRegSettings(void)
{
	CString	str, strConfig;
	DWORD	dwData;
	
	strConfig.LoadStringW(IDS_REGSUBKEY_CONFIG);
	str.LoadStringW(IDS_REGKEY_SCRIPTFILE);
	WriteProfileString(strConfig, str,	m_strScriptFile);

	m_bDefaultScriptFile ? dwData = 1 : dwData = 0;
	str.LoadStringW(IDS_REGKEY_USEDEFAULTSCRIPTFILE);
	WriteProfileInt(strConfig, str, dwData);

	return TRUE;
}

/////////////////////////////////////////////////
// Read Script File Lines
//
bool CScrEditApp::ReadScrFile(CString strScriptFile)
{
	CStdioFile	stdFile;
	CString str, strTmp;
	CStringList	*pAS = NULL;
	bool bRes = false; 

	m_pActivitySet->RemoveAll();	// Clear all entries
	if(stdFile.Open(strScriptFile,CFile::modeRead)) 	//ファイルを正常に開けた場合
	{
		bool bCreateAS = TRUE;
		POSITION pos;
		while(stdFile.ReadString(strTmp))	// EOFまで１行づつ読む
		{
			if (!strTmp.IsEmpty())	// 空白行でない場合
			{
				if (bCreateAS)
				{
					pAS = new CStringList();
					pos = m_pActivitySet->AddTail(pAS);
				}
				pAS = (CStringList *)m_pActivitySet->GetAt(pos);

				pAS->AddTail(strTmp);

				bCreateAS = FALSE;	// ActiveSet increment flag OFF
			}
			else					// 空白行の場合
			{
				bCreateAS = TRUE;	// ActiveSet increment flag ON
			}
		}

		// Update ListView and highlighted the top row
		m_pDlgSrcEdit->m_propMain.UpdateASView(0);

		stdFile.Close();
		bRes = true;
	}
	else
	{
		str.LoadStringW(IDS_MSG_COULDNOTOPENFILE);AfxMessageBox(str);
		bRes = false;
	}

	return bRes;
}
