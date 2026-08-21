#include "StdAfx.h"
#include "EditSpinControl.h"

void InitSpinControl(int& rMin, int& rMax, int& rCurrent, /*int& rDelta, */CSpinButtonCtrl& rSpin, int nCurrentData)
{
	rMin		= 0;
	rMax		= 30000;
	rCurrent	= nCurrentData;
	//rDelta		= 1;
	rSpin.SetRange(rMin,rMax);
	rSpin.SetPos32(nCurrentData);
}

int OnDeltaposSpinControl(int& rMin, int& rMax, int& rCurrent, CSpinButtonCtrl& rSpin, CEdit& rEdit)
{
	int nPos = rSpin.GetPos32();
	int nRange = rMax - rMin;
	int nAdd;

	if (nPos < rCurrent)
	{
		if (rCurrent - nPos > nRange / 2)	// increase
		{
			nAdd = rMax - rCurrent + nPos - rMin;
		}
		else
		{
			nAdd = nPos - rCurrent;
		}
	}
	else if (nPos > rCurrent)
	{
		if (nPos - rCurrent > nRange / 2) // decrease
		{
			nAdd = nPos - rMax + rMin - rCurrent;
		}
		else
		{
			nAdd = nPos - rCurrent;
		}
	}
	else return 0;

	rCurrent = nPos;

	return nAdd;
}

void OnDeltaposSpinControlInt(int& rMin, int& rMax, int& rCurrent, CSpinButtonCtrl& rSpin, CEdit& rEdit)
{
	int nAdd = OnDeltaposSpinControl(rMin,rMax,rCurrent,rSpin,rEdit);
	if (nAdd == 0) return;

	CString strData;
	rEdit.GetWindowText(strData);
	const char* str = (char*)(LPCTSTR)strData;

	int nData;

	if (!Utility::StringManager::StrToInt(str,&nData))
	{
		nData = 0;
	}

	nData += nAdd;
	strData.Format("%d",nData);
	rEdit.SetWindowText((LPCTSTR)strData);
}

void OnDeltaposSpinControlDouble(int& rMin, int& rMax, int& rCurrent, CSpinButtonCtrl& rSpin, CEdit& rEdit)
{
	int nAdd = OnDeltaposSpinControl(rMin,rMax,rCurrent,rSpin,rEdit);
	if (nAdd == 0) return;

	CString strData;
	rEdit.GetWindowText(strData);
	const char* str = (char*)(LPCTSTR)strData;

	double dData;

	if (!Utility::StringManager::StrToDouble(str,&dData))
	{
		dData = 0.0;
	}

	dData += nAdd;
	strData.Format("%.1lf",dData);
	rEdit.SetWindowText((LPCTSTR)strData);
}
