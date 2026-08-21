#include "stdafx.h"
#include <UBML/Writer.h>
#include <UBML/Reader.h>
#include <UBML/UData.h>

using namespace System;
using namespace System::Text;
using namespace System::Collections::Generic;
using namespace Microsoft::VisualStudio::TestTools::UnitTesting;

using namespace UnE::UBML;

namespace UBMLTest
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
		void TestUBML()
		{
			if (!TestWriteBinary())
				return;
			if (!TestReadBinary())
				return;
		}

		bool TestReadBinary()
		{
			wchar_t* strFilePath = L"test.ubml";

			Reader reader;
			if (!reader.ReadFile(strFilePath))
			{
				String^ strError = gcnew String(reader.GetErrorMessage().c_str());
				Assert::Inconclusive(gcnew String("[TestReadBinary] Fail : ") + strError);
				return false;
			}

			//bool isArray;
			unsigned int nElementCount = reader.GetElementCount();

			for (unsigned int i=0;i<nElementCount;i++)
			{
				const Element* pElement = reader.GetElement(i);
				if (pElement == 0) continue;

				//wstring strTag = pElement->GetTagString();
				//wcout << L"[" << strTag << L"]" << endl;
		
				unsigned int nDataCount = pElement->GetDataCount();

				for (unsigned int j=0;j<nDataCount;j++)
				{
					const UData* pData = pElement->GetData(j);
					if (pData == 0) continue;

					if (pData->GetClassType() == UData::ELEMENT)
					{
						//if (!PrintElement((const Element*)pData, 1))
						//	return false;
					}
					else
					{
						//if (!PrintSegment((const Segment*)pData, 1))
						//	return false;
					}
				}
			}

			if (!reader.ToXML(L"reader.xml"))
			{
				String^ strError = gcnew String(reader.GetErrorMessage().c_str());
				Assert::Inconclusive(gcnew String("[TestReadBinary] Fail : ") + strError);
				return false;
			}

			return true;
		}

		//[TestMethod]
		bool TestWriteBinary()
		{
			//
			// TODO: 테스트 논리를 여기에 추가합니다.
			//
			wchar_t* strFilePath = L"test.ubml";

			Element* pElement = new Element;
			pElement->MakeTag(L"AB", 123);

			int nData1 = 10;
			short arr2[10] = {1, 2, 3, 4, -5, 6, 7, 8, 9, 10};
			float fData3 = 0.01f;
			char* str4 = "1부터 10까지";

			Segment* seg[4] = {new Segment, new Segment, new Segment, new Segment};

			seg[0]->SetType(__LONG);
			seg[1]->SetType(__SHORT_ARR);
			seg[2]->SetType(__FLOAT);
			seg[3]->SetType(__CHAR_ARR);

			seg[0]->AddData(nData1);
	
			for (int i=0;i<10;i++)
			{
				seg[1]->AddData(arr2[i]);
			}

			seg[2]->AddData(fData3);
	
			int nLen = (int)strlen(str4);

			for (int i=0;i<nLen;i++)
			{
				seg[3]->AddData(str4[i]);
			}

			for (int i=0;i<4;i++)
			{
				pElement->AddData(seg[i]);
			}

			Writer writer;
			writer.AddElement(pElement);
			bool isSuccess = writer.WriteFile(strFilePath);

			if (!isSuccess)
			{
				String^ strError = gcnew String(writer.GetErrorMessage().c_str());
				Assert::Inconclusive(gcnew String("[TestWriteBinary] Fail : ") + strError);
				return false;
			}

			if (!writer.ToXML(L"writer.xml"))
			{
				String^ strError = gcnew String(writer.GetErrorMessage().c_str());
				Assert::Inconclusive(gcnew String("[TestWriteBinary] Fail : ") + strError);
				return false;
			}

			return true;
		};
	};
}
