#pragma once

namespace DXF
{
	namespace OBJECTS
	{
		class ObjectManager;

		class Dictionary : public Object
		{
		public:
			class Entry
			{
			public:
				Entry() {m_pObj = 0;}

			public:
				Object* m_pObj;
				std::wstring m_strEntryName;
				int m_nHandleCode;			// 350 or 360
			};

		public:
			Dictionary(ObjectManager* pMgr);
			virtual ~Dictionary(void);

		public:
			virtual void Write(Utility::FileManager* pMgr);

		public:
			// nHandleCode : 350 or 360
			void AddEntry(Object* pObj, wchar_t* strEntryName, int nHandleCode);
			// nBlockHandle과 연관된 Layout 객체의 핸들을 리턴한다.
			int GetLayoutHandle(int nBlockHandle);

		protected:
			void Init();

		public:
			static wchar_t* GetSubClassName();

		protected:
			std::list<Entry> m_listEntry;

		protected:
			static std::wstring m_strSubClassName;
		};
	}
}
