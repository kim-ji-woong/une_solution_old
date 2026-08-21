#pragma once
#include "EditNumber/EditDouble.h"

// CEditDoubleEx

class CEditDoubleEx : public CEditDouble
{
	DECLARE_DYNAMIC(CEditDoubleEx)

public:
	CEditDoubleEx();
	virtual ~CEditDoubleEx();

public:
	void SetMinimum(double dMin);
	double GetMinimum() const;

protected:
	double m_dMin;

protected:
	DECLARE_MESSAGE_MAP()
public:
	afx_msg void OnEnChange();
};


