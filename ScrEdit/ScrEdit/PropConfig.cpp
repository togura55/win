// PropConfig.cpp : 実装ファイル
//

#include "stdafx.h"
#include "ScrEdit.h"
#include "ScrEditDlg.h"
#include "ReadScrDlg.h"
#include "PropConfig.h"


// CPropConfig ダイアログ

IMPLEMENT_DYNAMIC(CPropConfig, CPropertyPage)

CPropConfig::CPropConfig()
	: CPropertyPage(CPropConfig::IDD)
{

}

CPropConfig::~CPropConfig()
{
}

void CPropConfig::DoDataExchange(CDataExchange* pDX)
{
	CPropertyPage::DoDataExchange(pDX);
}


BEGIN_MESSAGE_MAP(CPropConfig, CPropertyPage)
	ON_BN_CLICKED(IDC_PBTN_OPENSCRIPTFILE, &CPropConfig::OnBnClickedPbtnOpenscriptfile)
	ON_STN_CLICKED(IDC_TXT_OPENSCRIPTFILE, &CPropConfig::OnStnClickedTxtOpenscriptfile)
END_MESSAGE_MAP()


// CPropConfig メッセージ ハンドラ

void CPropConfig::OnOK()
{
	// TODO: ここに特定なコードを追加するか、もしくは基本クラスを呼び出してください。
//	UpdateData();
//	AfxGetApp()->WriteProfileInt(_T("Option"), _T("kind"), m_xvRadioKind);
    
    CPropertyPage::OnOK();
}

void CPropConfig::OnBnClickedPbtnOpenscriptfile()
{
	if (theApp.m_pDlgSrcEdit->m_pReadSrcDlg != NULL)
		theApp.m_pDlgSrcEdit->m_pReadSrcDlg = new CReadScrDlg(this);

	theApp.m_pDlgSrcEdit->m_pReadSrcDlg->ShowWindow(true);
}

BOOL CPropConfig::OnInitDialog()
{
	CPropertyPage::OnInitDialog();

	// TODO:  ここに初期化を追加してください
	CString str;

	str.LoadStringW(IDS_CONFIG_TXTOPENSCRIPTFILE);SetDlgItemText(IDC_TXT_OPENSCRIPTFILE, str);
	str.LoadStringW(IDS_CONFIG_PBTNOPENSCRIPTFILE);SetDlgItemText(IDC_PBTN_OPENSCRIPTFILE, str);

	return TRUE;  // return TRUE unless you set the focus to a control
	// 例外 : OCX プロパティ ページは必ず FALSE を返します。
}

void CPropConfig::OnStnClickedTxtOpenscriptfile()
{
	// TODO: ここにコントロール通知ハンドラ コードを追加します。
}
