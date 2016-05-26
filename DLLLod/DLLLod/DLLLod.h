
// DLLLod.h : PROJECT_NAME アプリケーションのメイン ヘッダー ファイルです。
//

#pragma once

#ifndef __AFXWIN_H__
	#error "PCH に対してこのファイルをインクルードする前に 'stdafx.h' をインクルードしてください"
#endif

#include "resource.h"		// メイン シンボル


// CDLLLodApp:
// このクラスの実装については、DLLLod.cpp を参照してください。
//

class CDLLLodApp : public CWinAppEx
{
public:
	CDLLLodApp();

// オーバーライド
	public:
	virtual BOOL InitInstance();

// 実装

	DECLARE_MESSAGE_MAP()
};

extern CDLLLodApp theApp;