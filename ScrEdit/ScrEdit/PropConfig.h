#pragma once


// CPropConfig �_�C�A���O

class CPropConfig : public CPropertyPage
{
	DECLARE_DYNAMIC(CPropConfig)

public:
	CPropConfig();
	virtual ~CPropConfig();

// �_�C�A���O �f�[�^
	enum { IDD = IDD_PROPDLG_CONFIG };

protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV �T�|�[�g

	DECLARE_MESSAGE_MAP()
public:
	virtual void OnOK();
	afx_msg void OnBnClickedPbtnOpenscriptfile();
	virtual BOOL OnInitDialog();
	afx_msg void OnStnClickedTxtOpenscriptfile();
};
