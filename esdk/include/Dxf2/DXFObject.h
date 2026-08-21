#pragma once

namespace DXF
{
	namespace OBJECTS
	{
		class ObjectManager;

		class Object
		{
		public:
			Object(ObjectManager* pMgr);
			virtual ~Object(void);
			friend class ObjectManager;

		public:
			virtual void Write(Utility::FileManager* pMgr);

			virtual void ReadDatai(int nCode, int nData) {}
			virtual void ReadDatad(int nCode, double dData) {}
			virtual void ReadDatas(int nCode, wchar_t* strData) {}

		public:
			void AddData(DXFData& rData);
			void AddData(int nCode, void* pData);
			wchar_t* GetEntityType();
			int GetHandle();
			int GetDictionaryHandle();
			void SetDictionaryHandle(int nDictionaryHandle);

		protected:
			int m_nHandle;
			int m_nDictionaryHandle;
			std::list<DXFData> m_list;
			std::wstring m_strEntityType;
			ObjectManager* m_pMgr;
		};
	}
}
