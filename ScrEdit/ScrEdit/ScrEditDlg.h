// ScrEditDlg.h : �w�b�_�[ �t�@�C��
//

#pragma once
#include "afxcmn.h"
#include "PropMain.h"
#include "PropConfig.h"


// CScrEditDlg �_�C�A���O
//class CDlgScrEdit : public CDialog
class CDlgScrEdit : public CPropertySheet
{
// �R���X�g���N�V����
public:
	CDlgScrEdit(CWnd* pParent = NULL);	// �W���R���X�g���N�^

// �_�C�A���O �f�[�^
	enum { IDD = IDD_SCREDIT_DIALOG };

protected:
	virtual void DoDataExchange(CDataExchange* pDX);	// DDX/DDV �T�|�[�g


// ����
protected:
	HICON m_hIcon;

	// �������ꂽ�A���b�Z�[�W���蓖�Ċ֐�
	virtual BOOL OnInitDialog();
	afx_msg void OnSysCommand(UINT nID, LPARAM lParam);
	afx_msg void OnPaint();
	afx_msg HCURSOR OnQueryDragIcon();
	DECLARE_MESSAGE_MAP()


public:
	afx_msg void OnTcnSelchangeTabScredit(NMHDR *pNMHDR, LRESULT *pResult);
	
	CDialog	*m_pReadSrcDlg;
	CPropMain	m_propMain;
    CPropConfig	m_propConfig;

};
