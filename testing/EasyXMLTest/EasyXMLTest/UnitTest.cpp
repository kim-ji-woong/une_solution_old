#include "stdafx.h"
#include <EasyXML/EasyXML2.h>
#include <string.h>

using namespace System;
using namespace System::Text;
using namespace System::Collections::Generic;
using namespace Microsoft::VisualStudio::TestTools::UnitTesting;

using namespace UnE;

namespace EasyXMLTest
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
		void TestEasyXML()
		{
			//
			// TODO: 테스트 논리를 여기에 추가합니다.
			//
			if (!TestWrite())
				return;
			if (!TestRead())
				return;
			if (!TestRemove())
				return;
		};

		bool ErrorMessage(String^ strMessage, EasyXML2& rXML)
		{
			wchar_t wstrError[256] = L"";
			rXML.GetErrorMessage(wstrError);

			String^ strError = gcnew String(wstrError);
			Assert::Inconclusive(gcnew String(strMessage + strError));
			return false;
		}

		bool TestWrite()
		{
			EasyXML2 xml;

			if (!xml.OpenXMLFile(L"test.xml", false))
			{
				return ErrorMessage("[TestWrite] Fail : ", xml);
			}

			if (!TestInsert(xml))
				return false;
			if (!TestSetData(xml))
				return false;

			return true;
		}

		bool TestRead()
		{
			EasyXML2 xml;

			if (!xml.OpenXMLFile(L"test.xml", true))
			{
				return ErrorMessage("[TestRead] Fail : ", xml);
			}

			DWORD_PTR nNodeID = xml.GetRootNode();

			if (nNodeID == 0)
			{
				return ErrorMessage("[TestRead] GetRootNode Fail : ", xml);
			}

			wchar_t strData[256] = L"";
			long nNodeType;

			if (!xml.GetNodeData(nNodeID, strData, &nNodeType))
			{
				return ErrorMessage("[TestRead] Root Node의 Data를 가져올 수 없습니다. : ", xml);
			}

			if (nNodeType != EasyXML2::ROOT)
			{
				return ErrorMessage("[TestRead] Root Node의 Data Type이 ROOT가 아닙니다. : ", xml);
			}

			if (wcslen(strData) == 0)
			{
				return ErrorMessage("[TestRead] Root Node의 Element Text를 얻어올 수 없습니다. : ", xml);
			}

			if (!TestReadElement(xml, nNodeID))
				return false;

			return true;
		}

		bool TestRemove()
		{
			EasyXML2 xml;
			xml.OpenXMLFile(L"test.xml", true);
			
			DWORD_PTR nNodeID = xml.GetRootNode();

			int nResult = 0;	// Element(1), Attribute(2), 4(Text)

			if (!TestReadElement(xml, nNodeID))
				return false;

			return true;
		}

		bool TestRemoveElement(EasyXML2& rXML, DWORD_PTR nNodeID, int& rResult)
		{
			wchar_t strData[256] = L"";
			long nNodeType;

			nNodeID = rXML.GetChildNode(nNodeID);

			while (nNodeID)
			{
				rXML.GetNodeData(nNodeID, strData, &nNodeType);
			
				if (nNodeType == EasyXML2::ELEMENT)
				{
					if ((rResult & 1) != 1)
					{
						DWORD_PTR tempID = rXML.GetNextNode(nNodeID);

						if (!rXML.RemoveNode(nNodeID))
						{
							return ErrorMessage("[TestRemoveElement] Element Node를 삭제할 수 없습니다. : ", rXML);
						}

						rResult |= 1;
						if (rResult == 7) return true;

						nNodeID = tempID;
						continue;
					}

					if (!TestRemoveElement(rXML, nNodeID, rResult))
						return false;
				}
				else if (nNodeType == EasyXML2::ATTRIBUTE)
				{
					if ((rResult & 2) != 2)
					{
						DWORD_PTR tempID = rXML.GetNextNode(nNodeID);

						DWORD_PTR nAttrTextID = rXML.GetChildNode(nNodeID);

						if (rXML.RemoveNode(nAttrTextID))
						{
							return ErrorMessage("[TestRemoveElement] AttrText Node는 단독으로 삭제될 수 없습니다. : ", rXML);
						}

						if (!rXML.RemoveNode(nNodeID))
						{
							return ErrorMessage("[TestRemoveElement] Attr Node를 삭제할 수 없습니다. : ", rXML);
						}

						rResult |= 2;
						if (rResult == 7) return true;

						nNodeID = tempID;
						continue;
					}
				}
				else if (nNodeType == EasyXML2::TEXT)
				{
					if ((rResult & 4) != 4)
					{
						DWORD_PTR tempID = rXML.GetNextNode(nNodeID);

						if (!rXML.RemoveNode(nNodeID))
						{
							return ErrorMessage("[TestRemoveElement] Text Node를 삭제할 수 없습니다. : ", rXML);
						}

						rResult |= 4;
						if (rResult == 7) return true;

						nNodeID = tempID;
						continue;
					}
				}

				nNodeID = rXML.GetNextNode(nNodeID);
			}

			return true;
		}

		bool TestReadElement(EasyXML2& rXML, DWORD_PTR nNodeID)
		{
			wchar_t strData[256] = L"";
			long nNodeType;

			nNodeID = rXML.GetChildNode(nNodeID);

			while (nNodeID)
			{
				if (!rXML.GetNodeData(nNodeID, strData, &nNodeType))
				{
					return ErrorMessage("[TestReadElement] Node의 Data를 가져올 수 없습니다. : ", rXML);
				}

				if (nNodeType == EasyXML2::ELEMENT)
				{
					if (wcslen(strData) == 0)
					{
						return ErrorMessage("[TestReadElement] Element의 Text를 얻어올 수 없습니다. : ", rXML);
					}

					if (!TestReadElement(rXML, nNodeID))
						return false;
				}
				else if (nNodeType == EasyXML2::ATTRIBUTE)
				{
					if (wcslen(strData) == 0)
					{
						return ErrorMessage("[TestReadElement] Attr Name을 얻어올 수 없습니다. : ", rXML);
					}

					DWORD_PTR nAttrTextID = rXML.GetChildNode(nNodeID);

					if (nAttrTextID == 0)
					{
						return ErrorMessage("[TestReadElement] Attr Text Node를 얻어올 수 없습니다. : ", rXML);
					}

					if (!rXML.GetNodeData(nAttrTextID, strData, &nNodeType))

					if (nNodeType != EasyXML2::ATTRIBUTE_TEXT)
					{
						return ErrorMessage("[TestReadElement] Attr Node 아래에 AttrText Node가 아닌 다른 Node가 존재합니다. : ", rXML);
					}
				}
				else if (nNodeType == EasyXML2::TEXT)
				{
				}
				else
				{
					return ErrorMessage("[TestReadElement] 알려지지 않은 Node입니다. : ", rXML);
				}

				nNodeID = rXML.GetNextNode(nNodeID);
			}

			return true;
		}

		bool TestInsert(EasyXML2& rXML)
		{
			DWORD_PTR nNodeID = rXML.GetRootNode();
			
			DWORD_PTR nChildID = rXML.InsertData(nNodeID, EasyXML2::ELEMENT, L"FirstChild");
			if (nChildID == 0)
			{
				return ErrorMessage("[TestInsert] Element 생성 Fail : ", rXML);
			}

			DWORD_PTR nAttrID = rXML.InsertAttributeData(nNodeID, L"attrName", L"attrData");
			if (nAttrID == 0)
			{
				return ErrorMessage("[TestInsert] Attribute 생성 Fail : ", rXML);
			}

			DWORD_PTR nTextID = rXML.InsertData(nChildID, EasyXML2::TEXT, L"Test Text");
			if (nTextID == 0)
			{
				return ErrorMessage("[TestInsert] Text 생성 Fail : ", rXML);
			}

			if (!TestInsertAfter(rXML))
				return false;

			if (!TestInsertBefore(rXML))
				return false;

			return true;
		}

		bool TestInsertAfter(EasyXML2& rXML)
		{
			DWORD_PTR nRootID = rXML.GetRootNode();
			DWORD_PTR nNodeID = rXML.GetChildNode(nRootID);
			
			DWORD_PTR nChildID = rXML.InsertAfter(nNodeID, EasyXML2::ELEMENT, L"SecondChild");
			if (nChildID == 0)
			{
				return ErrorMessage("[TestInsertAfter] Element 생성 Fail : ", rXML);
			}

			DWORD_PTR nNextID = rXML.GetNextNode(nNodeID);

			wchar_t strData[256];
			long nNodeType;

			while (nNextID)
			{
				rXML.GetNodeData(nNextID, strData, &nNodeType);

				if (nNodeType == EasyXML2::ATTRIBUTE)
				{
					DWORD_PTR nAttrID = rXML.InsertAfterAttribute(nNextID, L"attrName2", L"attrData2");
					if (nAttrID == 0)
					{
						return ErrorMessage("[TestInsertAfter] Attribute 생성 Fail : ", rXML);
					}

					break;
				}

				nNextID = rXML.GetNextNode(nNextID);
			}

			DWORD_PTR nTextID = rXML.InsertAfter(nNodeID, EasyXML2::TEXT, L"After Test Text");
			if (nTextID == 0)
			{
				return ErrorMessage("[TestInsertAfter] Text 생성 Fail : ", rXML);
			}

			return true;
		}

		bool TestInsertBefore(EasyXML2& rXML)
		{
			DWORD_PTR nRootID = rXML.GetRootNode();
			DWORD_PTR nNodeID = rXML.GetChildNode(nRootID);
			
			DWORD_PTR nChildID = rXML.InsertBefore(nNodeID, EasyXML2::ELEMENT, L"_0ThChild");
			if (nChildID == 0)
			{
				return ErrorMessage("[TestInsertBefore] Element 생성 Fail : ", rXML);
			}

			DWORD_PTR nNextID = rXML.GetNextNode(nNodeID);

			wchar_t strData[256];
			long nNodeType;

			while (nNextID)
			{
				rXML.GetNodeData(nNextID, strData, &nNodeType);

				if (nNodeType == EasyXML2::ATTRIBUTE)
				{
					DWORD_PTR nAttrID = rXML.InsertBeforeAttribute(nNextID, L"attrName0", L"attrData0");
					if (nAttrID == 0)
					{
						return ErrorMessage("[TestInsertBefore] Attribute 생성 Fail : ", rXML);
					}

					break;
				}

				nNextID = rXML.GetNextNode(nNextID);
			}

			DWORD_PTR nTextID = rXML.InsertBefore(nNodeID, EasyXML2::TEXT, L"Before Test Text");
			if (nTextID == 0)
			{
				return ErrorMessage("[TestInsertBefore] Text 생성 Fail : ", rXML);
			}

			return true;
		}

		bool TestSetData(EasyXML2& rXML)
		{
			DWORD_PTR nRootID = rXML.GetRootNode();

			if (!rXML.SetNodeData(nRootID, L"RootElementChanged"))
			{
				return ErrorMessage("[TestSetData] Root Node 설정 Fail : ", rXML);
			}

			DWORD_PTR nChildID = rXML.GetChildNode(nRootID);

			wchar_t strData[256] = L"";
			long nNodeType;
			bool checkAttr = false, checkElement = false, checkText = false;

			while (nChildID)
			{
				if (!rXML.GetNodeData(nChildID, strData, &nNodeType))
				{
					return ErrorMessage("[TestSetData] GetNodeData Fail : ", rXML);
				}

				if (nNodeType == EasyXML2::ATTRIBUTE && !checkAttr)
				{
					if (!rXML.SetNodeData(nChildID, L"AttrNameChanged"))
					{
						return ErrorMessage("[TestSetData] Attr Node 설정 Fail : ", rXML);
					}

					DWORD_PTR nAttrTextID = rXML.GetChildNode(nChildID);

					if (!rXML.SetNodeData(nAttrTextID, L"AttrDataChanged"))
					{
						return ErrorMessage("[TestSetData] AttrText Node 설정 Fail : ", rXML);
					}

					checkAttr = true;
				}
				else if (nNodeType == EasyXML2::ELEMENT && !checkElement)
				{
					if (!rXML.SetNodeData(nChildID, L"ChildElementChanged"))
					{
						return ErrorMessage("[TestSetData] Element Node 설정 Fail : ", rXML);
					}

					checkElement = true;
				}
				else if (nNodeType == EasyXML2::TEXT && !checkText)
				{
					if (!rXML.SetNodeData(nChildID, L"Child Text Changed"))
					{
						return ErrorMessage("[TestSetData] Text Node 설정 Fail : ", rXML);
					}

					checkText = true;
				}

				nChildID = rXML.GetNextNode(nChildID);
			}

			return true;
		}
	};
}
