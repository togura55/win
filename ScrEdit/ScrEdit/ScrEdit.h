// ScrEdit.h : PROJECT_NAME �A�v���P�[�V�����̃��C�� �w�b�_�[ �t�@�C���ł��B
//

#pragma once

#ifndef __AFXWIN_H__
	#error "PCH �ɑ΂��Ă��̃t�@�C�����C���N���[�h����O�� 'stdafx.h' ���C���N���[�h���Ă�������"
#endif

#include "resource.h"		// ���C�� �V���{��
#include "ScrEditDlg.h"


// CScrEditApp:
// ���̃N���X�̎����ɂ��ẮAScrEdit.cpp ���Q�Ƃ��Ă��������B
//

class CScrEditApp : public CWinApp
{
public:
	CScrEditApp();

// �I�[�o�[���C�h
	public:
	virtual BOOL InitInstance();

// ����
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