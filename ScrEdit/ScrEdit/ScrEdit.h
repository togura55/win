// ScrEdit.h : PROJECT_NAME アプリケーションのメイン ヘッダー ファイルです。
//

#pragma once

#ifndef __AFXWIN_H__
	#error "PCH に対してこのファイルをインクルードする前に 'stdafx.h' をインクルードしてください"
#endif

#include "resource.h"		// メイン シンボル
#include "ScrEditDlg.h"


// CScrEditApp:
// このクラスの実装については、ScrEdit.cpp を参照してください。
//

class CScrEditApp : public CWinApp
{
public:
	CScrEditApp();

// オーバーライド
	public:
	virtual BOOL InitInstance();

// 実装
	bool	m_bDefaultScriptFile;
	CObList	*m_pActivitySet;

	CDlgScrEdit *m_pDlgSrcEdit;
	CString	m_strScriptFile;

	DECLARE_MESSAGE_MAP()
public:
	bool ReadRegSettings(void);
	bool WriteRegSettings(void);
	bool ReadScrFile(CString);
};

extern CScrEditApp theApp;