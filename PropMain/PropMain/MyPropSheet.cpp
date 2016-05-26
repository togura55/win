// MyPropSheet.cpp : 実装ファイル
//

#include "stdafx.h"
#include "PropMain.h"
#include "MyPropSheet.h"


// CMyPropSheet

IMPLEMENT_DYNAMIC(CMyPropSheet, CPropertySheet)

CMyPropSheet::CMyPropSheet(UINT nIDCaption, CWnd* pParentWnd, UINT iSelectPage)
	:CPropertySheet(nIDCaption, pParentWnd, iSelectPage)
{

}

CMyPropSheet::CMyPropSheet(LPCTSTR pszCaption, CWnd* pParentWnd, UINT iSelectPage)
	:CPropertySheet(pszCaption, pParentWnd, iSelectPage)
{

}

CMyPropSheet::~CMyPropSheet()
{
}


BEGIN_MESSAGE_MAP(CMyPropSheet, CPropertySheet)
END_MESSAGE_MAP()


// CMyPropSheet メッセージ ハンドラ
