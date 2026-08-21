#pragma once

namespace DXF
{
	namespace TABLES
	{
		class TableManager;

		class AppID : public Table
		{
		public:
			class Entity
			{
			public:
				Entity(AppID* pTable, wchar_t* strAppName);

			public:
				void Write(Utility::FileManager* pMgr);

			protected:
				wchar_t m_strAppName[256];
				AppID* m_pParent;
				int m_nHandle;
			};

		public:
			AppID(TableManager* pMgr);
			virtual ~AppID(void);
			friend class Entity;

		public:
			void Init();
			void Write(Utility::FileManager* pMgr);

		public:
			virtual void ReadDatai(int nCode, int nData) {}
			virtual void ReadDatad(int nCode, double dData) {}
			virtual void ReadDatas(int nCode, wchar_t* strData) {}

		protected:
			std::list<Entity> m_list;
		};
	}
}
