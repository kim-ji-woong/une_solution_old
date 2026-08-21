#pragma once


// CEditDouble
// Double형 Data(실수)만 입력받는다.
class CEditDouble : public CEdit
{
	DECLARE_DYNAMIC(CEditDouble)

public:
	CEditDouble();
	virtual ~CEditDouble();

public:
	void SetData(double dData);
	double GetData() const;
	// nSize : 소수점 몇자리까지 표시할 것인가?
	void InitText(unsigned int nSize = 1);
	// Return 값 : 0(int), 1(double)
	int GetClassType() const;
// 최대 혹은 최소값을 가지는가?
	void SetMaxUse(bool bPermit);
	void SetMinUse(bool bPermit);
	bool GetMaxUse() const;
	bool GetMinUse() const;
	// bMax : true이면 최대값
	//        false이면 최소값
	void SetData(double dData, bool bMax);
	double GetData(bool bMax) const;

protected:
	DECLARE_MESSAGE_MAP()

protected:

	// 최대 혹은 최소값을 가지는가?
	bool m_bMax, m_bMin;
	// 음수를 허용할 것인가?
	bool m_bPermitMinus;
	CString m_strPrev;
	bool m_bChanged;
	double m_dData;
	double m_dDataMax, m_dDataMin;
private:
	int m_nClassType;

public:
	afx_msg void OnEnChange();
};


