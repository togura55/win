#pragma once


// CPropPage1 �_�C�A���O

class CPropPage1 : public CPropertyPage
{
	DECLARE_DYNAMIC(CPropPage1)

public:
	CPropPage1();
	virtual ~CPropPage1();

// �_�C�A���O �f�[�^
	enum { IDD = IDD_PROPDLG_1 };

protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV �T�|�[�g

	DECLARE_MESSAGE_MAP()
};
