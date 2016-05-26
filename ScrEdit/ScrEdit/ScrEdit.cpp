// ScrEdit.cpp : �A�v���P�[�V�����̃N���X������`���܂��B
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


// CScrEditApp �R���X�g���N�V����

CScrEditApp::CScrEditApp()
{
	// TODO: ���̈ʒu�ɍ\�z�p�R�[�h��ǉ����Ă��������B
	// ������ InitInstance ���̏d�v�ȏ��������������ׂċL�q���Ă��������B
	
	m_bDefaultScriptFile = FALSE;	// ����̃X�N���v�g�t�@�C�����ݒ肳��Ă��邩
	m_pDlgSrcEdit = NULL;

	m_pActivitySet = new CObList();

}


// �B��� CScrEditApp �I�u�W�F�N�g�ł��B

CScrEditApp theApp;


// CScrEditApp ������

/////////////////////////////////////////////////
// 
//
BOOL CScrEditApp::InitInstance()
{
	// �A�v���P�[�V���� �}�j�t�F�X�g�� visual �X�^�C����L���ɂ��邽�߂ɁA
	// ComCtl32.dll Version 6 �ȍ~�̎g�p���w�肷��ꍇ�́A
	// Windows XP �� InitCommonControlsEx() ���K�v�ł��B�����Ȃ���΁A�E�B���h�E�쐬�͂��ׂĎ��s���܂��B
	INITCOMMONCONTROLSEX InitCtrls;
	InitCtrls.dwSize = sizeof(InitCtrls);
	// �A�v���P�[�V�����Ŏg�p���邷�ׂẴR���� �R���g���[�� �N���X���܂߂�ɂ́A
	// �����ݒ肵�܂��B
	InitCtrls.dwICC = ICC_WIN95_CLASSES;
	InitCommonControlsEx(&InitCtrls);

	CWinApp::InitInstance();

	AfxEnableControlContainer();

	// �W��������
	// �����̋@�\���g�킸�ɍŏI�I�Ȏ��s�\�t�@�C����
	// �T�C�Y���k���������ꍇ�́A�ȉ�����s�v�ȏ�����
	// ���[�`�����폜���Ă��������B
	// �ݒ肪�i�[����Ă��郌�W�X�g�� �L�[��ύX���܂��B
	// TODO: ��Ж��܂��͑g�D���Ȃǂ̓K�؂ȕ������
	// ���̕������ύX���Ă��������B
//	SetRegistryKey(_T("�A�v���P�[�V���� �E�B�U�[�h�Ő������ꂽ���[�J�� �A�v���P�[�V����"));
	ReadRegSettings();

	CDlgScrEdit dlg;
	m_pDlgSrcEdit = &dlg;
	m_pMainWnd = &dlg;
	INT_PTR nResponse = dlg.DoModal();
	if (nResponse == IDOK)
	{
		// TODO: �_�C�A���O�� <OK> �ŏ����ꂽ���̃R�[�h��
		//  �L�q���Ă��������B
	}
	else if (nResponse == IDCANCEL)
	{
		// TODO: �_�C�A���O�� <�L�����Z��> �ŏ����ꂽ���̃R�[�h��
		//  �L�q���Ă��������B
	}

	// �_�C�A���O�͕����܂����B�A�v���P�[�V�����̃��b�Z�[�W �|���v���J�n���Ȃ���
	//  �A�v���P�[�V�������I�����邽�߂� FALSE ��Ԃ��Ă��������B
	return FALSE;
}


/////////////////////////////////////////////////
// ���W�X�g���ɋL�^���Ă���v���C�x�[�g���̓ǂݍ���
//
bool CScrEditApp::ReadRegSettings(void)
{
	CString str, strConfig;
	DWORD	dwData = 0;

	// �ݒ肪�i�[����Ă��郌�W�X�g�� �L�[��ύX���܂��B
	// TODO: ���̕�������A��Ж��܂��͑g�D���Ȃǂ́A
	// �K�؂ȕ�����ɕύX���Ă��������B
	str.LoadStringW(IDS_REGKEYNAME);SetRegistryKey(str);

	// �T�u�L�[���̎w�肵���L�[�̒l�i������j�̎擾�B�A�v�����͈ÖقŐݒ肳���
	strConfig.LoadStringW(IDS_REGSUBKEY_CONFIG);

	str.LoadStringW(IDS_REGKEY_USEDEFAULTSCRIPTFILE);
	dwData = GetProfileInt(strConfig, str, 0);
	if (dwData == 0) m_bDefaultScriptFile = false; else m_bDefaultScriptFile = true; 

	str.LoadStringW(IDS_REGKEY_SCRIPTFILE);
	m_strScriptFile = GetProfileString(strConfig, str, _T(""));

	return TRUE;
}

/////////////////////////////////////////////////
// ���W�X�g���ւ̃v���C�x�[�g���̏�������
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
	if(stdFile.Open(strScriptFile,CFile::modeRead)) 	//�t�@�C���𐳏�ɊJ�����ꍇ
	{
		bool bCreateAS = TRUE;
		POSITION pos;
		while(stdFile.ReadString(strTmp))	// EOF�܂łP�s�Âǂ�
		{
			if (!strTmp.IsEmpty())	// �󔒍s�łȂ��ꍇ
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
			else					// �󔒍s�̏ꍇ
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
