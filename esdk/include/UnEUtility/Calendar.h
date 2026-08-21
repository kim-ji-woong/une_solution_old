#pragma once

namespace UnE
{
	namespace Utility
	{
		class Calendar
		{
		public:
			Calendar(void);
			Calendar(int nYear, unsigned int nMonth, unsigned int nDay);
			virtual ~Calendar(void);

		public:
			void SetDate(int nYear, unsigned int nMonth, unsigned int nDay);
			void GetDate(int& rYear, unsigned int& rMonth, unsigned int& rDay);
			// nMonth 개월만큼 날짜를 증가시킨다.
			// nMonth가 음수일 경우 날짜가 감소한다.
			void IncreaseMonth(int nMonth);
			// nDay 일만큼 날짜를 증가시킨다.
			// nDay가 음수일 경우 날짜가 감소한다.
			void IncreaseDay(int nDay);

		public:
			// 윤년인가?
			static bool IsLeapYear(int nYear);
			static unsigned int GetLastDay(int nYear, unsigned int nMonth);
			// 두 날짜가 몇일의 차이가 나는지 알려준다.
			static unsigned int GetDiffDay(int nYear1, int nMonth1, int nDay1, int nYear2, int nMonth2, int nDay2);
			// 특정 날짜(nYear/nMonth/nDay)로부터 연말(혹은 연초)까지 며칠이 남았는지 알려준다.
			// toEnd : true이면 연말, false이면 연초
			static unsigned int GetDay(int nYear, int nMonth, int nDay, bool toEnd);
			
		protected:
			// 두 해 사이의 날짜를 알려준다.
			// nYear1이 더 오래전이며, nYear1과 nYear2 사이의 해들만 계산한다.
			// GetDiffDayYear(1990, 1994)이면 1991 ~ 1993의 날짜들만 더해서 알려준다.
			static unsigned int GetDiffDayYear(int nYear1, int nYear2);
			// 두 달 사이의 날짜를 알려준다.
			// nMonth1이 더 오래전이며, nMonth1과 nMonth2 사이의 달들만 계산한다.
			// GetDiffDayMonth(1, 4)이면 2 ~ 3의 날짜들만 더해서 알려준다.
			static unsigned int GetDiffDayMonth(int nYear, int nMonth1, int nMonth2);

		protected:
			void CalcMonthNDay(int nYear, unsigned int nMonth, unsigned int& rDay);
			int GetDayForNext(int nYear, unsigned int nMonth, unsigned int nDay);

		protected:
			// m_nYear가 0보다 작으면 BC
			int m_nYear;
			unsigned int m_nMonth;
			unsigned int m_nDay;

			static unsigned int m_arrMonth[12];
		};
	}
}
