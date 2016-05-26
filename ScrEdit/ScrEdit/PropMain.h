#pragma once
#include "afxcmn.h"
#include "afxwin.h"
#include "afxcoll.h"


// CPropMain ダイアログ

class CPropMain : public CPropertyPage
{
	DECLARE_DYNAMIC(CPropMain)

public:
	CPropMain();
	virtual ~CPropMain();

// ダイアログ データ
	enum { IDD = IDD_PROPDLG_MAIN };

protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV サポート

	DECLARE_MESSAGE_MAP()
public:
	virtual void OnOK();

	CListBox m_xcListAS;
	virtual BOOL OnInitDialog();
	BOOL	UpdateASView(int);
private:
	CButton m_xcPbtnUp;
	CButton m_xcPbtnDw;
	CComboBox m_xcCombo_Cmd1;
	CComboBox m_xcCombo_Cmd2;
	CStringList m_strlCmd;
	void ResetAllControls(void);
	bool m_bAddEntryState;
public:
	afx_msg void OnBnClickedPbtnUpAs();
	afx_msg void OnBnClickedPbtnDwAs();
	afx_msg void OnLbnSelchangeListAs();
	CString m_xvEditCd;
	CString m_xvEditLcd;
	CString m_xvEditCmd1;
	CString m_xvEditCmd2;
	int m_xvCombo_Cmd1;
	int m_xvCombo_Cmd2;
	bool ShowASEntries(int);
	afx_msg void OnBnClickedPbtnAddentry();
	afx_msg void OnBnClickedPbtnWrite();
	afx_msg void OnBnClickedPbtnReflect();

private:
	int RetrieveInputData(void);
public:
	afx_msg void OnBnClickedPbtnCopy();
};
