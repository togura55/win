// InsIndexView.cpp : CInsIndexView クラスの実装
//

#include "stdafx.h"
#include "InsIndex.h"

#include "InsIndexDoc.h"
#include "InsIndexView.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#endif


// CInsIndexView

IMPLEMENT_DYNCREATE(CInsIndexView, CFormView)

BEGIN_MESSAGE_MAP(CInsIndexView, CFormView)
	// 標準印刷コマンド
	ON_COMMAND(ID_FILE_PRINT, &CFormView::OnFilePrint)
	ON_COMMAND(ID_FILE_PRINT_DIRECT, &CFormView::OnFilePrint)
	ON_COMMAND(ID_FILE_PRINT_PREVIEW, &CFormView::OnFilePrintPreview)
END_MESSAGE_MAP()

// CInsIndexView コンストラクション/デストラクション

CInsIndexView::CInsIndexView()
	: CFormView(CInsIndexView::IDD)
{
    TRACE("Entering CInsIndexView constructor\n");
	// TODO: 構築コードをここに追加します。

}

CInsIndexView::~CInsIndexView()
{
}

BOOL CInsIndexView::PreCreateWindow(CREATESTRUCT& cs)
{
	// TODO: この位置で CREATESTRUCT cs を修正して Window クラスまたはスタイルを
	//  修正してください。

	return CFormView::PreCreateWindow(cs);
}

// CInsIndexView 描画

void CInsIndexView::OnDraw(CDC* /*pDC*/)
{
	CInsIndexDoc* pDoc = GetDocument();
	ASSERT_VALID(pDoc);
	if (!pDoc)
		return;

	// TODO: この場所にネイティブ データ用の描画コードを追加します。
}


// CInsIndexView 印刷

BOOL CInsIndexView::OnPreparePrinting(CPrintInfo* pInfo)
{
	// 既定の印刷準備
	return DoPreparePrinting(pInfo);
}

void CInsIndexView::OnBeginPrinting(CDC* /*pDC*/, CPrintInfo* /*pInfo*/)
{
	// TODO: 印刷前の特別な初期化処理を追加してください。
}

void CInsIndexView::OnEndPrinting(CDC* /*pDC*/, CPrintInfo* /*pInfo*/)
{
	// TODO: 印刷後の後処理を追加してください。
}


// CInsIndexView 診断

#ifdef _DEBUG
void CInsIndexView::AssertValid() const
{
	CFormView::AssertValid();
}

void CInsIndexView::Dump(CDumpContext& dc) const
{
	CFormView::Dump(dc);
}

CInsIndexDoc* CInsIndexView::GetDocument() const // デバッグ以外のバージョンはインラインです。
{
	ASSERT(m_pDocument->IsKindOf(RUNTIME_CLASS(CInsIndexDoc)));
	return (CInsIndexDoc*)m_pDocument;
}
#endif //_DEBUG


// CInsIndexView メッセージ ハンドラ
