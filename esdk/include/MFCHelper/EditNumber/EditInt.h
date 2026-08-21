#pragma once

// Int형 Data(정수)만 입력받는다.
class CEditInt : public CEdit
{
	DECLARE_DYNAMIC(CEditInt)

public:
	CEditInt();
	virtual ~CEditInt();

public:
	void SetData(int nData);
	int GetData() const;
	void InitText();
	// Return 값 : 0(int), 1(double)
	int GetClassType() const;

	// 최대 혹은 최소값을 가지는가?
	void SetMaxUse(bool bPermit);
	void SetMinUse(bool bPermit);
	bool GetMaxUse() const;
	bool GetMinUse() const;
	// bMax : true이면 최대값
	//        false이면 최소값
	void SetData(int nData, bool bMax);
	int GetData(bool bMax) const;
	void SetPermitMinus(bool bPermit);
	bool GetPermitMinus() const;

protected:
	DECLARE_MESSAGE_MAP()

protected:

	// 최대 혹은 최소값을 가지는가?
	bool m_bMax, m_bMin;
	// 음수를 허용할 것인가?
	bool m_bPermitMinus;
	CString m_strPrev;
	bool m_bChanged;
	int m_nData;
	int m_nDataMax, m_nDataMin;

private:
	int m_nClassType;

public:
	afx_msg void OnEnChange();
};