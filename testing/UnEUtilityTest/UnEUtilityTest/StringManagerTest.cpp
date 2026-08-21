#include "stdafx.h"
#include <UnEUtility/Calendar.h>
#include <Windows.h>
#include <stdio.h>

using namespace System;
using namespace System::Text;
using namespace System::Collections::Generic;
using namespace Microsoft::VisualStudio::TestTools::UnitTesting;

using namespace UnE::Utility;

namespace UnEUtilityTest
{
	[TestClass]
	public ref class CalendarTest
	{
	private:
		TestContext^ testContextInstance;

	public: 
		/// <summary>
		///현재 테스트 실행에 대한 정보 및 기능을
		///제공하는 테스트 컨텍스트를 가져오거나 설정합니다.
		///</summary>
		property Microsoft::VisualStudio::TestTools::UnitTesting::TestContext^ TestContext
		{
			Microsoft::VisualStudio::TestTools::UnitTesting::TestContext^ get()
			{
				return testContextInstance;
			}
			System::Void set(Microsoft::VisualStudio::TestTools::UnitTesting::TestContext^ value)
			{
				testContextInstance = value;
			}
		};

		#pragma region Additional test attributes
		//
		//테스트를 작성할 때 다음 추가 특성을 사용할 수 있습니다.
		//
		//ClassInitialize를 사용하여 클래스의 첫 번째 테스트를 실행하기 전에 코드를 실행합니다.
		//[ClassInitialize()]
		//static void MyClassInitialize(TestContext^ testContext) {};
		//
		//ClassCleanup을 사용하여 클래스의 테스트를 모두 실행한 후에 코드를 실행합니다.
		//[ClassCleanup()]
		//static void MyClassCleanup() {};
		//
		//TestInitialize를 사용하여 각 테스트를 실행하기 전에 코드를 실행합니다.
		//[TestInitialize()]
		//void MyTestInitialize() {};
		//
		//TestCleanup을 사용하여 각 테스트를 실행하기 전에 코드를 실행합니다.
		//[TestCleanup()]
		//void MyTestCleanup() {};
		//
		#pragma endregion 

		[TestMethod]
		void TestCalendar()
		{
			//
			// TODO: 테스트 논리를 여기에 추가합니다.
			//
			if (!TestIncrease())
				return;
			if (!TestDiff())
				return;
		};

		bool ErrorMessage(String^ strMessage)
		{
			Assert::Inconclusive(strMessage);
			return false;
		}

		bool TestIncrease()
		{
			SYSTEMTIME t;
			GetLocalTime(&t);

			Calendar cal(t.wYear, t.wMonth, t.wDay);
			cal.IncreaseMonth(7);

			int nYear;
			unsigned int nMonth, nDay;
			cal.GetDate(nYear, nMonth, nDay);

			Calendar cal2(nYear, nMonth, nDay);
			cal2.IncreaseMonth(-7);

			int nYear2;
			unsigned int nMonth2, nDay2;
			cal2.GetDate(nYear2, nMonth2, nDay2);

			if (nYear2 != t.wYear || nMonth2 != t.wMonth || nDay2 != t.wDay)
			{
				wchar_t wstrError[512];
				swprintf(wstrError, 512, L"[TestIncrease] Calendar(%d, %d, %d).IncreaseMonth(7)의 결과 %d/%d/%d가 잘못되었거나, Calendar(%d, %d, %d).IncreaseMonth(-7)의 결과 %d/%d/%d가 잘못되었습니다.", 
					t.wYear, t.wMonth, t.wDay, nYear, nMonth, nDay, nYear, nMonth, nDay, nYear2, nMonth2, nDay2);
				return ErrorMessage(gcnew String(wstrError));
			}

			cal.SetDate(t.wYear, t.wMonth, t.wDay);
			cal.IncreaseDay(7);

			cal.GetDate(nYear, nMonth, nDay);

			cal2.SetDate(nYear, nMonth, nDay);
			cal2.IncreaseDay(-7);

			cal2.GetDate(nYear2, nMonth2, nDay2);

			if (nYear2 != t.wYear || nMonth2 != t.wMonth || nDay2 != t.wDay)
			{
				wchar_t wstrError[512];
				swprintf(wstrError, 512, L"[TestIncrease] Calendar(%d, %d, %d).IncreaseDay(7)의 결과 %d/%d/%d가 잘못되었거나, Calendar(%d, %d, %d).IncreaseDay(-7)의 결과 %d/%d/%d가 잘못되었습니다.", 
					t.wYear, t.wMonth, t.wDay, nYear, nMonth, nDay, nYear, nMonth, nDay, nYear2, nMonth2, nDay2);
				return ErrorMessage(gcnew String(wstrError));
			}

			return true;
		}

		bool TestDiff()
		{
			SYSTEMTIME t;
			GetLocalTime(&t);

			srand(GetTickCount());
			int nDiff = rand() % 1000;

			Calendar cal(t.wYear, t.wMonth, t.wDay);
			cal.IncreaseDay(nDiff);

			int nYear;
			unsigned int nMonth, nDay;
			cal.GetDate(nYear, nMonth, nDay);

			unsigned int nDiff2 = Calendar::GetDiffDay(t.wYear, t.wMonth, t.wDay, nYear, nMonth, nDay);

			if (nDiff != (int)nDiff2)
			{
				wchar_t wstrError[512];
				swprintf(wstrError, 512, L"[TestDiff] Calendar::GetDiffDay(%d, %d, %d, %d, %d, %d)의 결과가 %d가 아닙니다.(%d) GetDiffDay() / IncreaseDay() 둘 중 하나의 오류입니다.", 
					t.wYear, t.wMonth, t.wDay, nYear, nMonth, nDay, nDiff, nDiff2);
				return ErrorMessage(gcnew String(wstrError));
			}

			return true;
		}
	};
}
