#pragma once



// CMyPropSheet

class CMyPropSheet : public CPropertySheet
{
	DECLARE_DYNAMIC(CMyPropSheet)

public:
	CMyPropSheet(UINT nIDCaption, CWnd* pParentWnd = NULL, UINT iSelectPage = 0);
	CMyPropSheet(LPCTSTR pszCaption, CWnd* pParentWnd = NULL, UINT iSelectPage = 0);
	virtual ~CMyPropSheet();

protected:
	DECLARE_MESSAGE_MAP()
};


