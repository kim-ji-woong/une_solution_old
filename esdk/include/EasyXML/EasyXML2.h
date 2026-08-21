#pragma once

#include <string>
#include "Tree.h"

#import <msxml3.dll>
using namespace MSXML2;

namespace UnE
{
	class TreeData
	{
	public:
		TreeData();
		TreeData(const TreeData& rhs);
		TreeData(const wchar_t* str);
		~TreeData();
		void operator= (const TreeData& rhs);
		void operator= (const wchar_t* str);
		const wchar_t* GetStrData();

	public:
		int nDataType;

	protected:
		std::wstring m_strData;
	};

	class EasyXML2
	{
	public:
		enum DataType {ROOT = 0, ELEMENT, ATTRIBUTE, TEXT, ATTRIBUTE_TEXT, COMMENT};

	public:
		EasyXML2(void);
		virtual ~EasyXML2(void);

	public:
		bool OpenXMLFileA(const char* strPath, bool bRead);
		bool OpenXMLFile(const wchar_t* strPath, bool bRead);

		// 읽기 전용 함수
		// 파일이 아닌 메모리에서 읽는다.
		bool OpenXMLString(const wchar_t* strXML);
		void CloseXMLFile(void);
		bool IsOpen();
		bool Save();
		bool SaveAsA(const char* strPath);
		bool SaveAs(const wchar_t* strPath);

		// Return 값 : strXML의 길이
		size_t GetXMLString(std::wstring& strXML) const;
		size_t GetXMLStringA(std::string& strXML) const;
	//#ifdef UNICODE
	//	size_t GetXMLString(std::wstring& strXML) const;
	//#else
	//	size_t GetXMLString(std::string& strXML) const;
	//#endif
		/*size_t GetXMLString(std::string& strXML) const;
		size_t GetXMLString(std::wstring& strXML) const;*/

		DWORD_PTR GetRootNode();
		DWORD_PTR GetChildNode(DWORD_PTR nodeID);
		bool GetChildNodeDataA(DWORD_PTR nodeID, char* strData);
		bool GetChildNodeData(DWORD_PTR nodeID, wchar_t* strData);
		DWORD_PTR GetParentNode(DWORD_PTR nodeID);
		DWORD_PTR GetNextNode(DWORD_PTR nodeID);
		DWORD_PTR GetPrevNode(DWORD_PTR nodeID);
		DWORD_PTR InsertDataA(DWORD_PTR nodeID, LONG nodeType, const char* strData);
		DWORD_PTR InsertData(DWORD_PTR nodeID, LONG nodeType, const wchar_t* strData);
		DWORD_PTR InsertAttributeDataA(DWORD_PTR nodeID, const char* strAttrName, const char* strAttrData);
		DWORD_PTR InsertAttributeData(DWORD_PTR nodeID, const wchar_t* strAttrName, const wchar_t* strAttrData);
		DWORD_PTR InsertBeforeA(DWORD_PTR nodeID, LONG nodeType, const char* strData);
		DWORD_PTR InsertBefore(DWORD_PTR nodeID, LONG nodeType, const wchar_t* strData);
		DWORD_PTR InsertBeforeAttributeA(DWORD_PTR nodeID, const char* strAttrName, const char* strAttrData);
		DWORD_PTR InsertBeforeAttribute(DWORD_PTR nodeID, const wchar_t* strAttrName, const wchar_t* strAttrData);
		DWORD_PTR InsertAfterA(DWORD_PTR nodeID, LONG nodeType, const char* strData);
		DWORD_PTR InsertAfter(DWORD_PTR nodeID, LONG nodeType, const wchar_t* strData);
		DWORD_PTR InsertAfterAttributeA(DWORD_PTR nodeID, const char* strAttrName, const char* strAttrData);
		DWORD_PTR InsertAfterAttribute(DWORD_PTR nodeID, const wchar_t* strAttrName, const wchar_t* strAttrData);
		bool SetNodeDataA(DWORD_PTR nodeID, const char* strData);
		bool SetNodeData(DWORD_PTR nodeID, const wchar_t* strData);
		bool GetNodeDataA(DWORD_PTR nodeID, char* pStrData, DataType* pNodeType);
		bool GetNodeData(DWORD_PTR nodeID, wchar_t* pStrData, DataType* pNodeType);
		bool RemoveNode(DWORD_PTR nodeID);
		void GetErrorMessageA(char* pStrError);
		void GetErrorMessage(wchar_t* pStrError);
		void SetStyleSheetA(const char* strStyleSheet);
		void SetStyleSheet(const wchar_t* strStyleSheet);
		void GetStyleSheetA(char* strStyleSheet);
		void GetStyleSheet(wchar_t* strStyleSheet);
		void SetIndent(bool bIndent);
		bool GetIndent() const;
		void SetLocaleA(const char* strLocale);
		void SetLocale(const wchar_t* strLocale);
		void GetLocaleA(char* strLocale) const;
		void GetLocale(wchar_t* strLocale) const;

		DWORD_PTR MakeElementA(DWORD_PTR nodeID, const char* strElement, const char* strText = 0);
		DWORD_PTR MakeElement(DWORD_PTR nodeID, const wchar_t* strElement, const wchar_t* strText = 0);
		DWORD_PTR MakeAttributeA(DWORD_PTR nodeID, const char* strAttrName, const char* strAttrData);
		DWORD_PTR MakeAttribute(DWORD_PTR nodeID, const wchar_t* strAttrName, const wchar_t* strAttrData);
		DWORD_PTR MakeTextA(DWORD_PTR nodeID, const char* strText);
		DWORD_PTR MakeText(DWORD_PTR nodeID, const wchar_t* strText);

		DWORD_PTR GetChildNodeCount(DWORD_PTR nNodeID, DataType type);
		DWORD_PTR FindNodeTreeA(DWORD_PTR nParentID, const char* strNodeName, DataType type);
		DWORD_PTR FindNodeTree(DWORD_PTR nParentID, const wchar_t* strNodeName, DataType type);

	protected:
		void ClearError();
		// pTreeNod : 삽입할 부모 노드
		// data : 삽입될 자식 노드의 데이터
		// data의 타입이 ATTRIBUTE일 경우, InsertNodeData() 대신 InsertAttributeNodeData()를 사용한다.
		bool InsertNodeData(Tree<TreeData>::Node* pTreeNod, TreeData data);
		bool InsertNodeData(MSXML2::IXMLDOMElementPtr pElement, TreeData data);
		bool InsertAttributeNodeData(Tree<TreeData>::Node* pTreeNod, TreeData data, const wchar_t* strAttrData);
		bool InsertAttributeNodeData(MSXML2::IXMLDOMElementPtr pElement, TreeData data, const wchar_t* strAttrData);
		bool GetAttributeNameAndValue(const wchar_t* strSource, wchar_t* strName, wchar_t* strValue);
		// rIndex : 같은 이름을 가진 노드가 여러개 있을 경우 몇 번째 노드의 것인지 알려준다.
		void GetIndex(Tree<TreeData>::Node* pNod, int& rIndex);
		// pParent : Attribute의 텍스트일 경우 해당하는 노드가 없다.
		//           이 경우에는 pParent가 true가 되고, 그 부모 노드인 Attribute이 리턴된다.
		void FindNode(MSXML2::IXMLDOMNodePtr& pNod, MSXML2::IXMLDOMNodePtr& pParentNode, Tree<TreeData>::Node* pTreeNode, bool* pParent);
		bool FindAttribute(MSXML2::IXMLDOMNodePtr& pNod, const wchar_t* strItem);
		bool ReadElement(void* pData, Tree<TreeData>::Node* pTreeNod);
		bool ReadAttribute(void* pData, Tree<TreeData>::Node* pTreeNod);
		bool ReadText(void* pData, Tree<TreeData>::Node* pTreeNod);
		// pTreeNod : 추가될 노드 뒤에 놓여진 노드
		// data : 삽입될 자식 노드의 데이터
		// data의 타입이 ATTRIBUTE일 경우, InsertBeforeNodeData() 대신 InsertBeforeAttributeNodeData()를 사용한다.
		bool InsertBeforeNodeData(Tree<TreeData>::Node* pTreeNod, TreeData data);
		bool InsertBeforeNodeData(MSXML2::IXMLDOMNode* pNod, MSXML2::IXMLDOMNode* pParentNode, TreeData data);
		bool InsertBeforeAttributeNodeData(Tree<TreeData>::Node* pTreeNod, TreeData data, const wchar_t* strAttrData);
		bool InsertBeforeAttributeNodeData(MSXML2::IXMLDOMNode* pNod, MSXML2::IXMLDOMNode* pParentNode, TreeData data, const wchar_t* strAttrData);
		// pTreeNod : 추가될 노드 앞에 놓여진 노드
		// data : 삽입될 자식 노드의 데이터
		// data의 타입이 ATTRIBUTE일 경우, InsertAfterNodeData() 대신 InsertAfterAttributeNodeData()를 사용한다.
		bool InsertAfterNodeData(Tree<TreeData>::Node* pTreeNod, TreeData data);
		bool InsertAfterNodeData(MSXML2::IXMLDOMNode* pNod, MSXML2::IXMLDOMNode* pParentNode, TreeData data);
		bool InsertAfterAttributeNodeData(Tree<TreeData>::Node* pTreeNod, TreeData data, const wchar_t* strAttrData);
		bool InsertAfterAttributeNodeData(MSXML2::IXMLDOMNode* pNod, MSXML2::IXMLDOMNode* pParentNode, TreeData data, const wchar_t* strAttrData);
		bool ChangeXMLData(Tree<TreeData>::Node* pTreeNod, const wchar_t* strData);
		bool RemoveData(Tree<TreeData>::Node* pTreeNod);

	protected:
		wchar_t m_strPath[_MAX_PATH];
		wchar_t m_strError[256];
		wchar_t m_strStyleSheet[_MAX_PATH];
		wchar_t m_strLocale[32];
		MSXML2::IXMLDOMDocumentPtr m_pDoc;
		Tree<TreeData> m_tree;
		// XML 저장시에 한줄이 아니라 Tree 형태의 여러줄로 저장할 것인가?
		// [2009/9/7] 김지웅
		bool m_bIndent;
		// 쓰기 모드로 열렸는가?
		bool m_writeMode;
	};
}
