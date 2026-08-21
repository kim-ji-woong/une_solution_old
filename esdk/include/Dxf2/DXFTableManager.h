#pragma once

#include "DXFTable.h"
#include "DXFSectionManager.h"

class FileManager;

namespace DXF
{
	namespace TABLES
	{
		class TableManager : public SectionManager
		{
		public:
			TableManager(void);
			virtual ~TableManager(void);

		public:
			void Write(Utility::FileManager* pMgr);
			void AddTable(Table* pTable);
			// nIndex번째 Layout 객체의 Handle을 얻어온다.
			int GetLayoutHandle(int nIndex);
			Layer* GetLayer();
			VPort* GetVPort();
			LType* GetLType();
			Style* GetStyle();
			DimStyle* GetDimStyle();
			BlockRecord* GetBlockRecord();

			wchar_t* GetDimBlockName();

		public:
			virtual void Clear();
			virtual void ReadDatai(int nCode, int nData);
			virtual void ReadDatad(int nCode, double dData);
			virtual void ReadDatas(int nCode, wchar_t* strData);
			// Handle Code(5)가 정수가 아닌 문자열일 경우에도 읽을수 있는가?
			virtual bool ReadStringHandle();

		protected:
			void Init();

		protected:
			std::list<Table*> m_list;
			Table* m_pTable;

		private:
			bool m_isNewTable;
			wchar_t m_strDimBlockName[256];
			int m_nDimBlockIndex;
		};
	}
}
