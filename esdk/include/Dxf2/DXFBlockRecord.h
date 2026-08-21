#pragma once

namespace DXF
{
	namespace TABLES
	{
		class TableManager;

		class BlockRecord :	public Table
		{
		public:
			class Entity
			{
			public:
				Entity(BlockRecord* pTable, wchar_t* strAppName, int nBlockHandle, int nLayoutHandle, ArrowType type = FILL_TRIANGLE);

			public:
				void Write(Utility::FileManager* pMgr) const;

			public:
				void SetArrowType(ArrowType type);
				ArrowType GetArrowType() const;
				int GetBlockHandle() const;
				const wchar_t* GetAppName() const;

			protected:
				ArrowType m_arrowType;
				wchar_t m_strAppName[256];
				BlockRecord* m_pParent;
				int m_nBlockHandle;
				int m_nLayoutHandle;
			};

		public:
			BlockRecord(BLOCKS::BlockManager* pBlkMgr, OBJECTS::ObjectManager* pObjMgr, TableManager* pMgr);
			virtual ~BlockRecord(void);
			friend class Entity;

		public:
			void Init();
			void Write(Utility::FileManager* pMgr);
			void AddEntity(const Entity& rEntity);
			OBJECTS::ObjectManager* GetObjectManager() {return m_pObjMgr;}
			int GetBlockRecordHandle(ArrowType type);
			bool GetBlockHandle(const wchar_t* strAppName, int* pBlockHandle);

		public:
			virtual void ReadDatai(int nCode, int nData) {}
			virtual void ReadDatad(int nCode, double dData) {}
			virtual void ReadDatas(int nCode, wchar_t* strData) {}

		protected:
			std::list<Entity> m_list;
			BLOCKS::BlockManager* m_pBlkMgr;
			OBJECTS::ObjectManager* m_pObjMgr;
		};
	}
}
