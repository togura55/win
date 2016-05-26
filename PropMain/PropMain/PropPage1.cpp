// PropPage1.cpp : 実装ファイル
//

#include "stdafx.h"
#include "PropMain.h"
#include "PropPage1.h"


// CPropPage1 ダイアログ

IMPLEMENT_DYNAMIC(CPropPage1, CPropertyPage)

CPropPage1::CPropPage1()
	: CPropertyPage(CPropPage1::IDD)
{

}

CPropPage1::~CPropPage1()
{
}

void CPropPage1::DoDataExchange(CDataExchange* pDX)
{
	CPropertyPage::DoDataExchange(pDX);
}


BEGIN_MESSAGE_MAP(CPropPage1, CPropertyPage)
END_MESSAGE_MAP()


// CPropPage1 メッセージ ハンドラ
