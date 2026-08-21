#pragma once

#define MAX_LINE_TYPE_DATA 12

namespace DXF
{
	namespace TABLES
	{
		class TableManager;

		class LType : public Table
		{
		public:
			class Entity
			{
			public:
				class Data
				{
				public:
					double m_dLength;
					short m_nType;
				};

			public:
				Entity(LType* pTable, wchar_t* strLineType, wchar_t* strAnnotation = (wchar_t*)L"");

			public:
				void Write(Utility::FileManager* pMgr);
				void AddData(double dLength, short nType = 0);
				wchar_t* GetTypeName();
				bool GetData(void*& pID, double* pLength, short* pType);
				int GetHandle();

			protected:
				std::list<Data> m_list;
				std::wstring m_strLineType;
				LType* m_pParent;
				int m_nHandle;
				int m_nFlag;
				wchar_t m_strAnnotation[256];
				int m_nAlignCode;	// 항상 65
				int m_nLineTypeSize;// 선 종류의 개수
				double m_dPatternLength;

			private:
				std::list<Data>::iterator m_dataIter;
			};

		public:
			LType(TableManager* pMgr);
			virtual ~LType(void);
			friend class Entity;

		public:
			void Init();
			void Write(Utility::FileManager* pMgr);
			void AddEntity(int nFactor, unsigned short nStyle, wchar_t* strTypeName, wchar_t* strAnnotation = (wchar_t*)L"");
			void AddEntity(Entity& rEntity);
			Entity* GetEntity(wchar_t* strTypeName);
			Entity* GetEntityFromID(void*& pID);

		public:
			virtual void ReadDatai(int nCode, int nData);
			virtual void ReadDatad(int nCode, double dData);
			virtual void ReadDatas(int nCode, wchar_t* strData);
			virtual void Clear();

		protected:
			std::list<Entity> m_list;
			Entity* m_pEntity;
			double m_dTempData;

		private:
			std::wstring m_strTypeName;
			std::list<Entity>::iterator m_entIter;
		};
	}
}
