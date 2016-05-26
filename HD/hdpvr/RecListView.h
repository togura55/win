#pragma once

// CRecListView �r���[

#include "ProgInfo.h"

class CRecListView : public CListView
{
	DECLARE_DYNCREATE(CRecListView)

public:
	CRecListView();           // ���I�����Ŏg�p����� protected �R���X�g���N�^
	virtual ~CRecListView();

// Overrides
public:
	virtual void OnInitialUpdate();
	virtual BOOL OnChildNotify(UINT message, WPARAM wParam, LPARAM lParam, LRESULT* pLResult);

public:
#ifdef _DEBUG
	virtual void AssertValid() const;
#ifndef _WIN32_WCE
	virtual void Dump(CDumpContext& dc) const;
#endif
#endif

protected:
	DECLARE_MESSAGE_MAP()
public:
	int ListInit(void);
	//int ListInitInsertItem(void);
	int ListInsertItem(void);
	bool RecListSetItem(CListCtrl*	m_pListCtrl, int* piItem, int iSubItem, CString strItem);
	int ProgramContentsForDisplay (CProgInfo* pPI, int iIndex);

};


