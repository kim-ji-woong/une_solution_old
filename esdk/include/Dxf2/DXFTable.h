#pragma once
#include <string>

namespace DXF
{
	namespace TABLES
	{
		class TableManager;

		class Table
		{
		public:
			Table(TableManager* pMgr);
			virtual ~Table(void);

		public:
			virtual void Init() = 0;
			virtual void Clear() {}
			virtual void Write(Utility::FileManager* pMgr);
			virtual void ReadDatai(int nCode, int nData) = 0;
			virtual void ReadDatad(int nCode, double dData) = 0;
			virtual void ReadDatas(int nCode, wchar_t* strData) = 0;

		public:
			int GetHandle();
			wchar_t* GetEntityName();
			TableManager* GetManager();

		protected:
			int m_nHandle;
			int m_nSoftPointer;
			std::wstring m_strEntityName;
			wchar_t* m_strDefSubClassName;
			wchar_t* m_strSubClassName;
			int m_nEntitySize;
			TableManager* m_pMgr;
		};
	}
}
