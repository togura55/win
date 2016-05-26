#pragma once


// CPropPage1 ダイアログ

class CPropPage1 : public CPropertyPage
{
	DECLARE_DYNAMIC(CPropPage1)

public:
	CPropPage1();
	virtual ~CPropPage1();

// ダイアログ データ
	enum { IDD = IDD_PROPDLG_1 };

protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV サポート

	DECLARE_MESSAGE_MAP()
};
