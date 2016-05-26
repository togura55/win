#pragma once


// CPropConfig ダイアログ

class CPropConfig : public CPropertyPage
{
	DECLARE_DYNAMIC(CPropConfig)

public:
	CPropConfig();
	virtual ~CPropConfig();

// ダイアログ データ
	enum { IDD = IDD_PROPDLG_CONFIG };

protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV サポート

	DECLARE_MESSAGE_MAP()
public:
	virtual void OnOK();
	afx_msg void OnBnClickedPbtnOpenscriptfile();
	virtual BOOL OnInitDialog();
	afx_msg void OnStnClickedTxtOpenscriptfile();
};
