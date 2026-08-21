#pragma once
#include "DXFDictionary.h"
#include "DXFSectionManager.h"

namespace DXF
{
	class DXFManager;

	namespace BLOCKS
	{
		class BlockManager;
	}

	namespace OBJECTS
	{
		class Layout;

		class ObjectManager : public SectionManager
		{
		public:
			ObjectManager(BLOCKS::BlockManager* pBlkMgr);
			virtual ~ObjectManager(void);

		public:
			// nBlockHandle과 연관된 Layout 객체의 핸들을 리턴한다.
			int GetLayoutHandle(int nBlockHandle);
			void Clear();
			void Write(Utility::FileManager* pMgr);
			void AddObject(Object* pObj);
			void SetBlockManager(BLOCKS::BlockManager* pBlkMgr);
			// pID : Object 정보를 담고 있는 링크드 리스트 노드의 포인터
			Object* GetObject(void*& pID);

			void SetDXFManager(DXFManager* pMgr);
			DXFManager* GetManager();

		public:
			virtual void ReadDatai(int nCode, int nData);
			virtual void ReadDatad(int nCode, double dData);
			virtual void ReadDatas(int nCode, wchar_t* strData);

		protected:
			void Init();
			template <class T>
			T* GetGroupDictionary(int nDictionaryHandle);
			Dictionary* GetLayoutDictionary(int nDictionaryHandle);
			Dictionary* GetMLineStyleDictionary(int nDictionaryHandle);
			Dictionary* GetPlotSettingDictionary(int nDictionaryHandle);
			Dictionary* GetPlotStyleNameDictionary(int nDictionaryHandle);
			Layout* GetDefaultLayout(int nDictionaryHandle, wchar_t* strLayoutName, int nOrder, bool bPrimary = false);

		protected:
			BLOCKS::BlockManager* m_pBlkMgr;
			std::list<Object*> m_list;
			Object* m_pObject;
			bool m_hasLayout;

		private:
			DXFManager* m_pMgr;
			std::list<Object*>::iterator m_objIter;
		};
	}
}
