// InsIndexDoc.cpp : CInsIndexDoc �N���X�̎���
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


// CInsIndexDoc �R���X�g���N�V����/�f�X�g���N�V����

CInsIndexDoc::CInsIndexDoc()
{
	// TODO: ���̈ʒu�� 1 �x�����Ă΂��\�z�p�̃R�[�h��ǉ����Ă��������B
	m_strDoc = _T("");
}

CInsIndexDoc::~CInsIndexDoc()
{
}

BOOL CInsIndexDoc::OnNewDocument()
{
	if (!CDocument::OnNewDocument())
		return FALSE;

	// TODO: ���̈ʒu�ɍď�����������ǉ����Ă��������B
	// (SDI �h�L�������g�͂��̃h�L�������g���ė��p���܂��B)

	return TRUE;
}




// CInsIndexDoc �V���A����

void CInsIndexDoc::Serialize(CArchive& ar)
{
	CString *pLine, strIndex = _T("  -AA");
	CStringList	m_strList;
	CFile *cf;
	int	iNumLines = 0;	// �s��
	int	iCnt = 0;		// �C���f�b�N�X�̒ʔԍ�

	cf = ar.GetFile();
	CString FileName = cf->GetFileName();

	if (ar.IsStoring())	// ��������
	{
		// TODO: �i�[����R�[�h�������ɒǉ����Ă��������B

	}
	else	// �ǂݍ���
	{
		// TODO: �ǂݍ��ރR�[�h�������ɒǉ����Ă��������B
		CString line, str;
		int commentLine = 0;
		while(ar.ReadString(line))
		{
			pLine = new CString();
			commentLine = line.Find(_T("#"));

			if (line == "")		// �󔒍s�̏ꍇ
			{
				
			}
			else if (commentLine == 0)	// �R�����g�s�̏ꍇ
			{
			}
			else				// ����������ꍇ
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

		// ���`�����e�L�X�g���t�@�C���ɏ�������
		//
		CFile    file;
		int      err = 0;
		CString	strFileName = cf->GetFilePath();
		CString	strDestFileName = cf->GetFileName();

		strDestFileName.Insert(0, _T("id_"));	// �������݃t�@�C�����Ƀv���t�B�b�N�X�������ǉ�
		strFileName.Replace(cf->GetFileName(), strDestFileName);

		if (!file.Open(strFileName, 
			CFile::modeReadWrite | CFile::shareExclusive | 
			CFile::modeCreate | CFile::modeNoTruncate)) err = 1;

		if(!err)	// ��̃t�@�C���̍쐬�ɐ����A�������݂�
		{
			CString str;
			
			for(POSITION pos = m_strList.GetHeadPosition(); pos != NULL;)
			{
				str = m_strList.GetNext(pos);
				str.Append(_T("\n"));				// ���s�R�[�h��t��
				file.Write(str, str.GetLength());	// ��s��������
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



// CInsIndexDoc �f�f

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


// CInsIndexDoc �R�}���h
