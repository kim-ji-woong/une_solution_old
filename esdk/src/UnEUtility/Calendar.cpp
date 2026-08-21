#include "stdafx.h"
#include "Calendar.h"
#include <windows.h>

BEGIN_NS(UnE)
BEGIN_NS(Utility)

unsigned int Calendar::m_arrMonth[12] = {31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31};

Calendar::Calendar(void)
{
	// Default는 오늘의 날짜
	SYSTEMTIME t;
	::GetLocalTime(&t);

	m_nYear	 = t.wYear;
	m_nMonth = t.wMonth;
	m_nDay	 = t.wDay;
}

Calendar::Calendar(int nYear, unsigned int nMonth, unsigned int nDay)
{
	SetDate(nYear,nMonth,nDay);
}

Calendar::~Calendar(void)
{
}

void Calendar::SetDate(int nYear, unsigned int nMonth, unsigned int nDay)
{
	m_nYear	 = nYear;
	m_nMonth = nMonth;
	m_nDay	 = nDay;
}

void Calendar::GetDate(int& rYear, unsigned int& rMonth, unsigned int& rDay)
{
	rYear	= m_nYear;
	rMonth	= m_nMonth;
	rDay	= m_nDay;
}

// nMonth 개월만큼 날짜를 증가시킨다.
// nMonth가 음수일 경우 날짜가 감소한다.
void Calendar::IncreaseMonth(int nMonth)
{
	if (nMonth == 0) return;

	int nAddYear, nNewMonth;

	if (nMonth > 0)
	{
		nAddYear  = (m_nMonth + nMonth - 1) / 12;
		nNewMonth = (m_nMonth + nMonth) % 12;
		if (nNewMonth == 0) nNewMonth = 12;
	}
	else
	{
		int nTemp = m_nMonth + nMonth;

		if (nTemp > 0) nAddYear = 0;
		else nAddYear = nTemp / 12 - 1;

		nNewMonth = ((1 - nAddYear) * 12 + nTemp) % 12;
		if (nNewMonth == 0) nNewMonth = 12;
	}

	m_nYear = m_nYear + nAddYear;
	m_nMonth = nNewMonth;
	
	CalcMonthNDay(m_nYear,m_nMonth,m_nDay);
}

// nDay 일만큼 날짜를 증가시킨다.
// nDay가 음수일 경우 날짜가 감소한다.
void Calendar::IncreaseDay(int nDay)
{
	if (nDay == 0) return;

	if (nDay > 0)
	{
		for (;;)
		{
			int nNextDay = GetDayForNext(m_nYear,m_nMonth,m_nDay);

			if (nDay < nNextDay)
			{
				m_nDay += nDay;
				return;
			}
			else if (nDay == nNextDay)
			{
				m_nDay = 1;
				IncreaseMonth(1);
				return;
			}
			else// if (nDay > nNextDay)
			{
				m_nDay = 1;
				nDay -= nNextDay;
				IncreaseMonth(1);
			}
		}
	}
	else
	{
		for (;;)
		{
			int nPrevDay = m_nDay;

			if (-nDay < nPrevDay)
			{
				m_nDay += nDay;
				return;
			}
			else if (-nDay == nPrevDay)
			{
				IncreaseMonth(-1);
				m_nDay = GetLastDay(m_nYear,m_nMonth);
				return;
			}
			else// if (-nDay > nPrevDay)
			{
				IncreaseMonth(-1);
				m_nDay = GetLastDay(m_nYear,m_nMonth);
				nDay += nPrevDay;
			}
		}
	}
}

unsigned int Calendar::GetLastDay(int nYear, unsigned int nMonth)
{
	if (nMonth == 1 || nMonth == 3 || nMonth == 5 || nMonth == 7 || nMonth == 8 || nMonth == 10 || nMonth == 12)
	{
		return 31;
	}
	else if (nMonth == 2)
	{
		if (IsLeapYear(nYear)) return 29;
		else return 28;
	}

	return 30;
}

int Calendar::GetDayForNext(int nYear, unsigned int nMonth, unsigned int nDay)
{
	if (nMonth == 1 || nMonth == 3 || nMonth == 5 || nMonth == 7 || nMonth == 8 || nMonth == 10 || nMonth == 12)
	{
		return 32 - nDay;
	}
	else if (nMonth == 2)
	{
		if (IsLeapYear(nYear)) return 30 - nDay;
		else return 29 - nDay;
	}
	
	return 31 - nDay;
}

void Calendar::CalcMonthNDay(int nYear, unsigned int nMonth, unsigned int& rDay)
{
	if (rDay <= 28) return;

	if (rDay == 29)
	{
		if (nMonth != 2) return;
		else
		{
			if (IsLeapYear(nYear)) return;
			else rDay = 28;
		}
	}
	else if (rDay == 30)
	{
		if (nMonth != 2) return;
		else
		{
			if (IsLeapYear(nYear)) rDay = 29;
			else rDay = 28;
		}
	}
	else// if (rDay == 31)
	{
		if (nMonth == 1 || nMonth == 3 || nMonth == 5 || nMonth == 7 || nMonth == 8 || nMonth == 10 || nMonth == 12) return;
		else if (nMonth == 2)
		{
			if (IsLeapYear(nYear)) rDay = 29;
			else rDay = 28;
		}
		else rDay = 30;
	}
}

// 윤년인가?
bool Calendar::IsLeapYear(int nYear)
{
	if (nYear % 4 == 0)			// 4년에 한번씩 윤년
	{
		if (nYear % 100 == 0)	// 100년마다 윤년 건너뜀
		{
			if (nYear % 400 == 0) return true;	// 그러나, 400년째는 윤년 인정
			else return false;
		}
		else return true;
	}

	return false;
}

// 두 날짜가 몇일의 차이가 나는지 알려준다.
unsigned int Calendar::GetDiffDay(int nYear1, int nMonth1, int nDay1, int nYear2, int nMonth2, int nDay2)
{
	unsigned int nDayCount = 0;
	
	if (nYear1 < nYear2)
	{
		nDayCount = GetDiffDayYear(nYear1, nYear2);
		nDayCount += GetDay(nYear1, nMonth1, nDay1, true);
		nDayCount += GetDay(nYear2, nMonth2, nDay2, false) + 1;
	}
	else if (nYear1 > nYear2)
	{
		nDayCount = GetDiffDayYear(nYear2, nYear1);
		nDayCount += GetDay(nYear2, nMonth2, nDay2, true);
		nDayCount += GetDay(nYear1, nMonth1, nDay1, false) + 1;
	}
	else
	{
		if (nMonth1 < nMonth2)
		{
			nDayCount = GetDiffDayMonth(nYear1, nMonth1, nMonth2);
			
			if (nMonth1 == 2)
			{
				unsigned int nMonth = IsLeapYear(nYear1) ? 29 : 28;
				nDayCount += nMonth - nDay1;
			}
			else
				nDayCount += m_arrMonth[nMonth1-1] - nDay1;

			nDayCount += nDay2;
		}
		else if (nMonth1 > nMonth2)
		{
			nDayCount = GetDiffDayMonth(nYear1, nMonth2, nMonth1);
			
			if (nMonth2 == 2)
			{
				unsigned int nMonth = IsLeapYear(nYear1) ? 29 : 28;
				nDayCount += nMonth - nDay2;
			}
			else
				nDayCount += m_arrMonth[nMonth2-1] - nDay2;

			nDayCount += nDay1;
		}
		else// if (nMonth1 == nMonth2)
		{
			if (nDay1 < nDay2)
				nDayCount = nDay2 - nDay1;
			else
				nDayCount = nDay1 - nDay2;
		}
	}

	return nDayCount;
}

// 두 해 사이의 날짜를 알려준다.
// nYear1이 더 오래전이며, nYear1과 nYear2 사이의 해들만 계산한다.
// GetDiffDayYear(1990, 1994)이면 1991 ~ 1993의 날짜들만 더해서 알려준다.
unsigned int Calendar::GetDiffDayYear(int nYear1, int nYear2)
{
	unsigned int nDayCount = 0;

	for (int i=nYear1+1;i<nYear2;i++)
	{
		if (IsLeapYear(i))
			nDayCount += 366;
		else
			nDayCount += 365;
	}

	return nDayCount;
}

// 두 달 사이의 날짜를 알려준다.
// nMonth1이 더 오래전이며, nMonth1과 nMonth2 사이의 달들만 계산한다.
// GetDiffDayMonth(1, 4)이면 2 ~ 3의 날짜들만 더해서 알려준다.
unsigned int Calendar::GetDiffDayMonth(int nYear, int nMonth1, int nMonth2)
{
	unsigned int nDayCount = 0;

	for (int i=nMonth1;i<nMonth2-1;i++)
	{
		if (i == 1)	// 2월
		{
			nDayCount += IsLeapYear(nYear) ? 29 : 28;
		}
		else
			nDayCount += m_arrMonth[i];
	}

	return nDayCount;
}

// 특정 날짜(nYear/nMonth/nDay)로부터 연말(혹은 연초)까지 며칠이 남았는지 알려준다.
// toEnd : true이면 연말, false이면 연초
unsigned int Calendar::GetDay(int nYear, int nMonth, int nDay, bool toEnd)
{
	unsigned int nDayCount = 0;

	if (toEnd)
	{
		if (nMonth > 2)
		{
			nDayCount = m_arrMonth[nMonth-1] - nDay;

			for (int i=nMonth;i<12;i++)
			{
				nDayCount += m_arrMonth[i];
			}
		}
		else
		{
			unsigned int nMonthDay;

			for (int i=nMonth-1;i<12;i++)
			{
				if (i == 1)	// 2월
					nMonthDay = IsLeapYear(nYear) ? 29 : 28;
				else
					nMonthDay = m_arrMonth[i];

				if (i == nMonth - 1)
				{
					nDayCount = nMonthDay - nDay;
				}
				else
				{
					nDayCount += nMonthDay;
				}
			}
		}
	}
	else
	{
		if (nMonth > 2)
		{
			unsigned int nMonthDay;
			nDayCount = (unsigned int)nDay - 1;

			for (int i=nMonth-2;i>=0;i--)
			{
				if (i == 1)	// 2월
					nMonthDay = IsLeapYear(nYear) ? 29 : 28;
				else
					nMonthDay = m_arrMonth[i];

				nDayCount += nMonthDay;
			}
		}
		else
		{
			nDayCount = (unsigned int)nDay - 1;

			for (int i=nMonth-2;i>=0;i--)
			{
				nDayCount += m_arrMonth[i];
			}
		}
	}
	
	return nDayCount;
}

END_NS
END_NS
