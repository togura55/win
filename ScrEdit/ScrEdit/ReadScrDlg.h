#pragma once


// CReadScrDlg �_�C�A���O

class CReadScrDlg : public CDialog
{
	DECLARE_DYNAMIC(CReadScrDlg)

public:
	CReadScrDlg(CWnd* pParent = NULL);   // �W���R���X�g���N�^
	virtual ~CReadScrDlg();

// �_�C�A���O �f�[�^
	enum { IDD = IDD_READSCRIPT };

protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV �T�|�[�g

	DECLARE_MESSAGE_MAP()
public:
	afx_msg void OnEnChangeEdit1();
	afx_msg void OnBnClickedPbtnReadscriptReference();
	afx_msg void OnBnClickedOk();
	CString m_xvEditReadScriptFile;
private:
	BOOL m_xvCB_UseDef;
public:
	afx_msg void OnBnClickedCbReadscriptDefault();
	virtual BOOL OnInitDialog();
protected:
	virtual void OnCancel();
	virtual void OnOK();
	virtual void PostNcDestroy();
public:
	afx_msg void OnStnClickedTxtReadscriptDesc();
};
