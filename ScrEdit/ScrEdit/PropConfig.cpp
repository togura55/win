// PropConfig.cpp : �����t�@�C��
//

#include "stdafx.h"
#include "ScrEdit.h"
#include "ScrEditDlg.h"
#include "ReadScrDlg.h"
#include "PropConfig.h"


// CPropConfig �_�C�A���O

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


// CPropConfig ���b�Z�[�W �n���h��

void CPropConfig::OnOK()
{
	// TODO: �����ɓ���ȃR�[�h��ǉ����邩�A�������͊�{�N���X���Ăяo���Ă��������B
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

	// TODO:  �����ɏ�������ǉ����Ă�������
	CString str;

	str.LoadStringW(IDS_CONFIG_TXTOPENSCRIPTFILE);SetDlgItemText(IDC_TXT_OPENSCRIPTFILE, str);
	str.LoadStringW(IDS_CONFIG_PBTNOPENSCRIPTFILE);SetDlgItemText(IDC_PBTN_OPENSCRIPTFILE, str);

	return TRUE;  // return TRUE unless you set the focus to a control
	// ��O : OCX �v���p�e�B �y�[�W�͕K�� FALSE ��Ԃ��܂��B
}

void CPropConfig::OnStnClickedTxtOpenscriptfile()
{
	// TODO: �����ɃR���g���[���ʒm�n���h�� �R�[�h��ǉ����܂��B
}
