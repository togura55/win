#include "stdafx.h"
#include "RecListView.h"
#include "Proginfo.h"
#include "chatter.h"

// CRecListView

IMPLEMENT_DYNCREATE(CRecListView, CListView)

CRecListView::CRecListView()
{

}

CRecListView::~CRecListView()
{
}

BEGIN_MESSAGE_MAP(CRecListView, CListView)
END_MESSAGE_MAP()


// CRecListView �f�f

#ifdef _DEBUG
void CRecListView::AssertValid() const
{
	CListView::AssertValid();
}

#ifndef _WIN32_WCE
void CRecListView::Dump(CDumpContext& dc) const
{
	CListView::Dump(dc);
}
#endif
#endif //_DEBUG


// CRecListView ���b�Z�[�W �n���h��
//
void CRecListView::OnInitialUpdate()
{

	CListView::OnInitialUpdate();

	// GetListCtrl() �����o�֐��̌Ăяo����ʂ��āA���ڂ��̃��X�g
    // �R���g���[���ɃA�N�Z�X���邱�Ƃɂ���� ListView ���A�C�e���ŌŒ�ł��܂��B
    {
		int          err = 0;
		CListCtrl    &listCtrl = GetListCtrl();
		CChatterApp* pApp = (CChatterApp*)AfxGetApp();
		pApp->m_pRecListCtrl = &listCtrl;	// �\�����郊�X�g�R���g���[����ۑ�

        // ���|�[�g�r���[�ɐݒ�
        //if (!err) if (listCtrl.SetView(LV_VIEW_DETAILS) == -1) err = 1;
		// ���X�g�̃X�^�C���i���|�[�g�A�P�s�I�𓙁j
		listCtrl.SetExtendedStyle(listCtrl.GetExtendedStyle()|LVS_EX_SUBITEMIMAGES|LVS_EX_FULLROWSELECT);
		listCtrl.ModifyStyle(NULL,LVS_REPORT);

        // ���X�g�R���g���[���̏�����
        //if (!err) err = ListInit();
		ListInit();
        
        // ���X�g�A�C�e���i�����l�j�̑}��
        //if (!err) err = 
		//ListInsertItem(pDI);

    }
}


BOOL CRecListView::OnChildNotify(UINT message, WPARAM wParam, LPARAM lParam, LRESULT* pResult)
{
	if(message == WM_NOTIFY)
	{
		NMHDR* phdr = (NMHDR*)lParam;

		// these 3 notifications are only sent by virtual listviews
		switch(phdr->code)
		{
		case LVN_GETDISPINFO:
			//{
			//	NMLVDISPINFO* pLvdi;

			//	pLvdi = (NMLVDISPINFO*)lParam;
			//	GetDispInfo(&pLvdi->item);
			//}
			//if(pResult != NULL)
			//{
			//	*pResult = 0;
			//}
			break;
		case LVN_ODCACHEHINT:
			//{
			//	NMLVCACHEHINT* pHint = (NMLVCACHEHINT*)lParam;

			//	PrepareCache(pHint->iFrom, pHint->iTo);
			//}
			//if(pResult != NULL)
			//{
			//	*pResult = 0;
			//}
			break;
		case LVN_ODFINDITEM:
			//{
			//	NMLVFINDITEM* pFindItem = (NMLVFINDITEM*)lParam;
			//	int i = FindItem(pFindItem->iStart, &pFindItem->lvfi);
			//	if(pResult != NULL)
			//	{
			//		*pResult = i;
			//	}
			//}
			break;
		default:
			return CListView::OnChildNotify(message, wParam, lParam, pResult);
		}
	}
	else
		return CListView::OnChildNotify(message, wParam, lParam, pResult);

	return TRUE;
}

/////////////////////////////////////////////////////////////////////////////
// CRecListView::ListInit
//  ���X�g�r���[�̏���������
//
int CRecListView::ListInit(void)
{
	LVCOLUMN    lvc;
    int         i;
    TCHAR       caption[][256] = {_T("�ԑg��"), _T("�ԑg���e"), _T("�W������"), _T("�����ǖ�"), _T("�J�n����"), _T("�I������")};
    const int   clmNum = sizeof caption /sizeof caption[0];
    CListCtrl   &listCtrl = GetListCtrl();
    int         err = 0;
    
    lvc.mask = LVCF_WIDTH | LVCF_TEXT | LVCF_SUBITEM;    // �L���t���O
    for (i = 0; i < clmNum; i++)
    {
        lvc.iSubItem    = i;            // �T�u�A�C�e���ԍ�
        lvc.pszText     = caption[i];   // ���o���e�L�X�g
        lvc.cx          = 150;          // ����
        if (listCtrl.InsertColumn(i, &lvc) == -1) {err = 1; break;}
    }
    
    return err;
}

/////////////////////////////////////////////////////////////////////////////
// CRecListView::ListInsertItem
//  ���X�g�r���[�ɃA�C�e����}�����鏈��
//
int CRecListView::ListInsertItem(CDeviceInfo *pDI)
{
    int         i, index = 0;
    int         err = 0, iNum;
	CString		str;

	CProgInfo*	pPI;
	//CChatterApp* pApp = (CChatterApp*)AfxGetApp();
	POSITION pos = pDI->m_pProgram->GetHeadPosition();

	iNum = (int)pDI->m_iNumProg;

    for (i = 0; i < iNum; i++)
    {
		int iSubItem = 0, iIndex = 0;
		pPI = (CProgInfo *)pDI->m_pProgram->GetAt(pos);

		// ListView�ɕ\������Item���Z�b�g����
		ProgramContentsForDisplay (pPI, iIndex);

		// ���̍s��
		pDI->m_pProgram->GetNext(pos);
	}
    
    return err;
}


/////////////////////////////////////////////////////////////////////////////
// CRecListView::RecListSetItem
//  LVITEM�̍쐬��SetView�̌Ăяo���B
//
bool CRecListView::RecListSetItem(CListCtrl* pListCtrl, int* piIndex, int iSubItem, CString strItem)
{
	LVITEM       lvi;
    int          size = 0;

	lvi.mask = LVIF_TEXT;
	lvi.iItem = (int)*piIndex;
    lvi.iSubItem = iSubItem;
	size = strItem.GetLength()+1;
	lvi.pszText = new TCHAR[size];
	_tcscpy_s(lvi.pszText, size, strItem);

	// �擪�J�����̏ꍇ��InterItem���R�[��
	if (iSubItem == 0)
		*piIndex = pListCtrl->InsertItem(&lvi);
	else
		pListCtrl->SetItem(&lvi);

	return TRUE;
}

// **************************************************
// ���X�g�r���[�ɕ\������A�C�e���̃t�H�[�}�b�g���쐬
//
// **************************************************
int CRecListView::ProgramContentsForDisplay (CProgInfo* pPI, int iIndex)
{
	int	err = 0;
	int iSubItem = 0;
	CString	str;

	CChatterApp* pApp = (CChatterApp*)AfxGetApp();

	// �ԑg��
	RecListSetItem(pApp->m_pRecListCtrl, &iIndex, iSubItem++, pPI->m_strTitle);
	// �ԑg���e
	RecListSetItem(pApp->m_pRecListCtrl, &iIndex, iSubItem++, pPI->m_strDesc);
	// �ԑg�J�e�S��
	RecListSetItem(pApp->m_pRecListCtrl, &iIndex, iSubItem++, pPI->m_strCategory);
	// �����ǖ�
	RecListSetItem(pApp->m_pRecListCtrl, &iIndex, iSubItem++, pPI->m_strStation);
	// �J�n����
	str.Format("%d/%d/%d %d:%d:%d",  pPI->m_tiStart.GetYear(),  pPI->m_tiStart.GetMonth(),  pPI->m_tiStart.GetDay(),  pPI->m_tiStart.GetHour(),  pPI->m_tiStart.GetMinute(),  pPI->m_tiStart.GetSecond()); 
	RecListSetItem(pApp->m_pRecListCtrl, &iIndex, iSubItem++, str);
	// �I������
	str.Format("%d/%d/%d %d:%d:%d",  pPI->m_tiEnd.GetYear(),  pPI->m_tiEnd.GetMonth(),  pPI->m_tiEnd.GetDay(),  pPI->m_tiEnd.GetHour(),  pPI->m_tiEnd.GetMinute(),  pPI->m_tiEnd.GetSecond()); 
	RecListSetItem(pApp->m_pRecListCtrl, &iIndex, iSubItem++, str);

    return err;
}