#pragma once

// CProgInfo �R�}���h �^�[�Q�b�g

class CProgInfo : public CObject
{
public:
	CProgInfo();
	virtual ~CProgInfo();

// Attribute
	CString		m_strTitle;		// �ԑg��
	CString		m_strDesc;		// �ԑg���e
	CString		m_strCategory;	// �ԑg�J�e�S��
	CString		m_strStation;	// �����ǖ�
	CTime		m_tiStart;		// �J�n����
	CTime		m_tiEnd;		// �I������
	int			m_iStat;		// �^��v�����
	int			m_iId;			// ���N�G�X�gID

	char		m_chHdr[8];	// �w�b�_�[�� �Œ蒷

};


