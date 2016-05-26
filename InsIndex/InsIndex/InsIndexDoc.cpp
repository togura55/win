// InsIndexDoc.cpp : CInsIndexDoc クラスの実装
//

#include "stdafx.h"
#include "InsIndex.h"

#include "InsIndexDoc.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#endif


// CInsIndexDoc

IMPLEMENT_DYNCREATE(CInsIndexDoc, CDocument)

BEGIN_MESSAGE_MAP(CInsIndexDoc, CDocument)
END_MESSAGE_MAP()


// CInsIndexDoc コンストラクション/デストラクション

CInsIndexDoc::CInsIndexDoc()
{
	// TODO: この位置に 1 度だけ呼ばれる構築用のコードを追加してください。
	m_strDoc = _T("");
}

CInsIndexDoc::~CInsIndexDoc()
{
}

BOOL CInsIndexDoc::OnNewDocument()
{
	if (!CDocument::OnNewDocument())
		return FALSE;

	// TODO: この位置に再初期化処理を追加してください。
	// (SDI ドキュメントはこのドキュメントを再利用します。)

	return TRUE;
}




// CInsIndexDoc シリアル化

void CInsIndexDoc::Serialize(CArchive& ar)
{
	CString *pLine, strIndex = _T("  -AA");
	CStringList	m_strList;
	CFile *cf;
	int	iNumLines = 0;	// 行数
	int	iCnt = 0;		// インデックスの通番号

	cf = ar.GetFile();
	CString FileName = cf->GetFileName();

	if (ar.IsStoring())	// 書き込み
	{
		// TODO: 格納するコードをここに追加してください。

	}
	else	// 読み込み
	{
		// TODO: 読み込むコードをここに追加してください。
		CString line, str;
		int commentLine = 0;
		while(ar.ReadString(line))
		{
			pLine = new CString();
			commentLine = line.Find(_T("#"));

			if (line == "")		// 空白行の場合
			{
				
			}
			else if (commentLine == 0)	// コメント行の場合
			{
			}
			else				// 文字がある場合
			{
				str.Format(_T("%s%d"), strIndex, iCnt);
				line.Append(str);
				iCnt++;
			}

			pLine->SetString(line);
			m_strList.AddTail(*pLine);
			iNumLines++;
		}
		UpdateAllViews(NULL);

		// 整形したテキストをファイルに書き込む
		//
		CFile    file;
		int      err = 0;
		CString	strFileName = cf->GetFilePath();
		CString	strDestFileName = cf->GetFileName();

		strDestFileName.Insert(0, _T("id_"));	// 書き込みファイル名にプレフィックス文字列を追加
		strFileName.Replace(cf->GetFileName(), strDestFileName);

		if (!file.Open(strFileName, 
			CFile::modeReadWrite | CFile::shareExclusive | 
			CFile::modeCreate | CFile::modeNoTruncate)) err = 1;

		if(!err)	// 空のファイルの作成に成功、書き込みへ
		{
			CString str;
			
			for(POSITION pos = m_strList.GetHeadPosition(); pos != NULL;)
			{
				str = m_strList.GetNext(pos);
				str.Append(_T("\n"));				// 改行コードを付加
				file.Write(str, str.GetLength());	// 一行書き込み
			}
			file.Close();
		}
		else
		{
		}

	}

}

void CInsIndexDoc::DeleteContents() 
{
    TRACE("Entering CStudentDoc::DeleteContents\n");
    //while (m_studentList.GetHeadPosition()) {
    //    delete m_studentList.RemoveHead();
    //}
}

BOOL CInsIndexDoc::OnOpenDocument(LPCTSTR lpszPathName) 
{
    TRACE("Entering CStudentDoc::OnOpenDocument\n");
	if (!CDocument::OnOpenDocument(lpszPathName))
		return FALSE;
	
	// TODO: Add your specialized creation code here
	
	return TRUE;
}



// CInsIndexDoc 診断

#ifdef _DEBUG
void CInsIndexDoc::AssertValid() const
{
	CDocument::AssertValid();
}

void CInsIndexDoc::Dump(CDumpContext& dc) const
{
	CDocument::Dump(dc);
}
#endif //_DEBUG


// CInsIndexDoc コマンド
