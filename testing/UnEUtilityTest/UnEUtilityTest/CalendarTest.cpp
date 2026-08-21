#include "stdafx.h"
#include <UnEUtility/StringManager.h>
#include <string.h>

using namespace System;
using namespace System::Text;
using namespace System::Collections::Generic;
using namespace Microsoft::VisualStudio::TestTools::UnitTesting;

using namespace UnE::Utility;

namespace UnEUtilityTest
{
	[TestClass]
	public ref class UnitTest
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
		void TestStringManager()
		{
			//
			// TODO: 테스트 논리를 여기에 추가합니다.
			//
			if (!TestParseString())
				return;
			if (!TestGetToken())
				return;
			if (!TestStrToX())
				return;
			if (!TestHexToX())
				return;
		};

		bool ErrorMessage(String^ strMessage)
		{
			Assert::Inconclusive(strMessage);
			return false;
		}

		bool TestParseString()
		{
			wchar_t* str = L"\t Hello World !!! \t ";
			std::wstring strChanged = StringManager::ParseString(str);
			int nLen2 = strChanged.length();

			if (strChanged[0] != 'H')
			{
				return ErrorMessage(L"[TestParseString] 문자열의 시작 부분이 제대로 제거되지 않았습니다.");
			}

			int nLen = strChanged.length();

			if (strChanged[nLen-1] != '!')
			{
				return ErrorMessage(L"[TestParseString] 문자열의 마지막 부분이 제대로 제거되지 않았습니다.");
			}

			return true;
		}

		bool TestGetToken()
		{
			wchar_t* str = L"\t Hello World !!! \t ";
			std::wstring str2 = StringManager::ParseString(str);

			wchar_t strToken[256];

			for (int i=0;str2.length() > 0;i++)
			{
				str2 = StringManager::GetToken(str2.c_str(), strToken);

				if (i == 0)
				{
					if (wcscmp(strToken, L"Hello"))
					{
						return ErrorMessage(L"[TestGetToken] 첫번째 Token이 Hello가 아닙니다.");
					}
				}
				else if (i == 1)
				{
					if (wcscmp(strToken, L"World"))
					{
						return ErrorMessage(L"[TestGetToken] 두번째 Token이 World가 아닙니다.");
					}
				}
				else if (i == 2)
				{
					if (wcscmp(strToken, L"!!!"))
					{
						return ErrorMessage(L"[TestGetToken] 세번째 Token이 !!!가 아닙니다.");
					}
				}
				else
				{
					return ErrorMessage(L"[TestGetToken] 알 수 없는 Token입니다.");
				}
			}

			return true;
		}
		
		bool TestStrToX()
		{
			int nInt;
			unsigned int nUInt;
			__int64 nInt64;
			double dDouble;

			if (!StringManager::StrToInt(L"-123456", &nInt))
				return ErrorMessage(L"[TestStrToInt] 변환 실패");
			if (!StringManager::StrToInt(L"123456", &nInt))
				return ErrorMessage(L"[TestStrToInt] 변환 실패");
			if (StringManager::StrToInt(L" 123456", &nInt))
				return ErrorMessage(L"[TestStrToInt] 정상적인 오류가 발생하지 않음");
			if (StringManager::StrToInt(L"123456 ", &nInt))
				return ErrorMessage(L"[TestStrToInt] 정상적인 오류가 발생하지 않음");
			if (StringManager::StrToInt(L"123a456 ", &nInt))
				return ErrorMessage(L"[TestStrToInt] 정상적인 오류가 발생하지 않음");

			if (!StringManager::StrToUInt(L"-123456", &nUInt))
				return ErrorMessage(L"[StrToUInt] 변환 실패");
			if (!StringManager::StrToUInt(L"123456", &nUInt))
				return ErrorMessage(L"[StrToUInt] 변환 실패");
			if (StringManager::StrToUInt(L" 123456", &nUInt))
				return ErrorMessage(L"[StrToUInt] 정상적인 오류가 발생하지 않음");
			if (StringManager::StrToUInt(L"123456 ", &nUInt))
				return ErrorMessage(L"[StrToUInt] 정상적인 오류가 발생하지 않음");
			if (StringManager::StrToUInt(L"123a456 ", &nUInt))
				return ErrorMessage(L"[StrToUInt] 정상적인 오류가 발생하지 않음");

			if (!StringManager::StrToInt64(L"-123456", &nInt64))
				return ErrorMessage(L"[StrToInt64] 변환 실패");
			if (!StringManager::StrToInt64(L"123456", &nInt64))
				return ErrorMessage(L"[StrToInt64] 변환 실패");
			if (StringManager::StrToInt64(L" 123456", &nInt64))
				return ErrorMessage(L"[StrToInt64] 정상적인 오류가 발생하지 않음");
			if (StringManager::StrToInt64(L"123456 ", &nInt64))
				return ErrorMessage(L"[StrToInt64] 정상적인 오류가 발생하지 않음");
			if (StringManager::StrToInt64(L"123a456 ", &nInt64))
				return ErrorMessage(L"[StrToInt64] 정상적인 오류가 발생하지 않음");

			if (!StringManager::StrToDouble(L"-123456", &dDouble))
				return ErrorMessage(L"[StrToDouble] 변환 실패");
			if (!StringManager::StrToDouble(L"-123456.1245", &dDouble))
				return ErrorMessage(L"[StrToDouble] 변환 실패");
			if (!StringManager::StrToDouble(L"123456.234", &dDouble))
				return ErrorMessage(L"[StrToDouble] 변환 실패");
			if (StringManager::StrToDouble(L" 123456", &dDouble))
				return ErrorMessage(L"[StrToDouble] 정상적인 오류가 발생하지 않음");
			if (StringManager::StrToDouble(L"123456 ", &dDouble))
				return ErrorMessage(L"[StrToDouble] 정상적인 오류가 발생하지 않음");
			if (StringManager::StrToDouble(L"123a456 ", &dDouble))
				return ErrorMessage(L"[StrToDouble] 정상적인 오류가 발생하지 않음");

			return true;
		}

		bool TestHexToX()
		{
			int nInt;
			__int64 nInt64;

			if (!StringManager::HexToInt(L"12af8", &nInt))
				return ErrorMessage(L"[HexToInt] 변환 실패");
			if (!StringManager::HexToInt(L"12Af8", &nInt))
				return ErrorMessage(L"[HexToInt] 변환 실패");
			if (!StringManager::HexToInt(L"12aF8", &nInt))
				return ErrorMessage(L"[HexToInt] 변환 실패");
			if (StringManager::HexToInt(L"12aG8", &nInt))
				return ErrorMessage(L"[HexToInt] 정상적인 오류가 발생하지 않음");
			if (StringManager::HexToInt(L" 12af8", &nInt))
				return ErrorMessage(L"[HexToInt] 정상적인 오류가 발생하지 않음");
			if (StringManager::HexToInt(L"12ae8 ", &nInt))
				return ErrorMessage(L"[HexToInt] 정상적인 오류가 발생하지 않음");
			if (StringManager::HexToInt(L"12a f8", &nInt))
				return ErrorMessage(L"[HexToInt] 정상적인 오류가 발생하지 않음");

			if (!StringManager::HexToInt64(L"12af8", &nInt64))
				return ErrorMessage(L"[HexToInt64] 변환 실패");
			if (!StringManager::HexToInt64(L"12Af8", &nInt64))
				return ErrorMessage(L"[HexToInt64] 변환 실패");
			if (!StringManager::HexToInt64(L"12aF8", &nInt64))
				return ErrorMessage(L"[HexToInt64] 변환 실패");
			if (StringManager::HexToInt64(L"12aG8", &nInt64))
				return ErrorMessage(L"[HexToInt64] 정상적인 오류가 발생하지 않음");
			if (StringManager::HexToInt64(L" 12af8", &nInt64))
				return ErrorMessage(L"[HexToInt64] 정상적인 오류가 발생하지 않음");
			if (StringManager::HexToInt64(L"12ae8 ", &nInt64))
				return ErrorMessage(L"[HexToInt64] 정상적인 오류가 발생하지 않음");
			if (StringManager::HexToInt64(L"12a f8", &nInt64))
				return ErrorMessage(L"[HexToInt64] 정상적인 오류가 발생하지 않음");

			return true;
		}
	};
}
