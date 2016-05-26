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


// CRecListView 診断

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


// CRecListView メッセージ ハンドラ
//
void CRecListView::OnInitialUpdate()
{

	CListView::OnInitialUpdate();

	// GetListCtrl() メンバ関数の呼び出しを通して、直接そのリスト
    // コントロールにアクセスすることによって ListView をアイテムで固定できます。
    {
		int          err = 0;
		CListCtrl    &listCtrl = GetListCtrl();
		CChatterApp* pApp = (CChatterApp*)AfxGetApp();
		pApp->m_pRecListCtrl = &listCtrl;	// 表示するリストコントロールを保存

        // レポートビューに設定
        //if (!err) if (listCtrl.SetView(LV_VIEW_DETAILS) == -1) err = 1;
		// リストのスタイル（レポート、１行選択等）
		listCtrl.SetExtendedStyle(listCtrl.GetExtendedStyle()|LVS_EX_SUBITEMIMAGES|LVS_EX_FULLROWSELECT);
		listCtrl.ModifyStyle(NULL,LVS_REPORT);

        // リストコントロールの初期化
        //if (!err) err = ListInit();
		ListInit();
        
        // リストアイテム（初期値）の挿入
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
//  リストビューの初期化処理
//
int CRecListView::ListInit(void)
{
	LVCOLUMN    lvc;
    int         i;
    TCHAR       caption[][256] = {_T("番組名"), _T("番組内容"), _T("ジャンル"), _T("放送局名"), _T("開始時間"), _T("終了時間")};
    const int   clmNum = sizeof caption /sizeof caption[0];
    CListCtrl   &listCtrl = GetListCtrl();
    int         err = 0;
    
    lvc.mask = LVCF_WIDTH | LVCF_TEXT | LVCF_SUBITEM;    // 有効フラグ
    for (i = 0; i < clmNum; i++)
    {
        lvc.iSubItem    = i;            // サブアイテム番号
        lvc.pszText     = caption[i];   // 見出しテキスト
        lvc.cx          = 150;          // 横幅
        if (listCtrl.InsertColumn(i, &lvc) == -1) {err = 1; break;}
    }
    
    return err;
}

/////////////////////////////////////////////////////////////////////////////
// CRecListView::ListInsertItem
//  リストビューにアイテムを挿入する処理
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

		// ListViewに表示するItemをセットする
		ProgramContentsForDisplay (pPI, iIndex);

		// 次の行へ
		pDI->m_pProgram->GetNext(pos);
	}
    
    return err;
}


/////////////////////////////////////////////////////////////////////////////
// CRecListView::RecListSetItem
//  LVITEMの作成とSetViewの呼び出し。
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

	// 先頭カラムの場合はInterItemをコール
	if (iSubItem == 0)
		*piIndex = pListCtrl->InsertItem(&lvi);
	else
		pListCtrl->SetItem(&lvi);

	return TRUE;
}

// **************************************************
// リストビューに表示するアイテムのフォーマットを作成
//
// **************************************************
int CRecListView::ProgramContentsForDisplay (CProgInfo* pPI, int iIndex)
{
	int	err = 0;
	int iSubItem = 0;
	CString	str;

	CChatterApp* pApp = (CChatterApp*)AfxGetApp();

	// 番組名
	RecListSetItem(pApp->m_pRecListCtrl, &iIndex, iSubItem++, pPI->m_strTitle);
	// 番組内容
	RecListSetItem(pApp->m_pRecListCtrl, &iIndex, iSubItem++, pPI->m_strDesc);
	// 番組カテゴリ
	RecListSetItem(pApp->m_pRecListCtrl, &iIndex, iSubItem++, pPI->m_strCategory);
	// 放送局名
	RecListSetItem(pApp->m_pRecListCtrl, &iIndex, iSubItem++, pPI->m_strStation);
	// 開始時間
	str.Format("%d/%d/%d %d:%d:%d",  pPI->m_tiStart.GetYear(),  pPI->m_tiStart.GetMonth(),  pPI->m_tiStart.GetDay(),  pPI->m_tiStart.GetHour(),  pPI->m_tiStart.GetMinute(),  pPI->m_tiStart.GetSecond()); 
	RecListSetItem(pApp->m_pRecListCtrl, &iIndex, iSubItem++, str);
	// 終了時間
	str.Format("%d/%d/%d %d:%d:%d",  pPI->m_tiEnd.GetYear(),  pPI->m_tiEnd.GetMonth(),  pPI->m_tiEnd.GetDay(),  pPI->m_tiEnd.GetHour(),  pPI->m_tiEnd.GetMinute(),  pPI->m_tiEnd.GetSecond()); 
	RecListSetItem(pApp->m_pRecListCtrl, &iIndex, iSubItem++, str);

    return err;
}