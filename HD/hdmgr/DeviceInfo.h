#pragma once

#include "ProgInfo.h"

// CDeviceInfo �R�}���h �^�[�Q�b�g

#pragma once

class CDeviceInfo : public CObject
{
public:
	CDeviceInfo();
	virtual ~CDeviceInfo();

	// Attribute
	int		m_iDeviceID;		// �f�o�C�XID
	CString	m_strDeviceDesc;	// �f�o�C�X�̐���
	CObList	*m_pProgram;		// �v���O�����̃��X�g
	int		m_iNumProg;			// �v���O������
	char	*m_pchHdr;			// Header�p�P�b�g

};


