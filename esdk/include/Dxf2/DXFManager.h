#pragma once

#include <string>

namespace DXF
{
	enum ArrowType {FILL_TRIANGLE = 0, SMALL_DOT = 3, SLASH, TRIANGLE, TWO_LINE, CIRCLE_ARROW, NONE};

	namespace BLOCKS
	{
		class BlockManager;
	}

	namespace CLASSES
	{
		class ClassManager;
	}

	namespace ENTITIES
	{
		class EntityManager;
	}

	namespace HEADER
	{
		class CHeader;
	}

	namespace OBJECTS
	{
		class ObjectManager;
	}

	namespace TABLES
	{
		class TableManager;
	}
	/*typedef union _DataType
	{
	int nData;
	double dData;
	} DataType;

	typedef struct _DXFData1
	{
	int nCode;
	DataType vData;
	} DXFData1;

	typedef struct _DXFData2
	{
	int nCode;
	std::string str;
	} DXFData2;*/
	typedef struct _DXFData
	{
		int nCode;
		int nData;
		double dData;
		std::wstring str;
	} DXFData;

	class DXFManager
	{
	public:
		DXFManager(void);
		~DXFManager(void);

	public:
		void SetBlockManager(BLOCKS::BlockManager* pBlkMgr);
		void SetClassManager(CLASSES::ClassManager* pClsMgr);
		void SetEntityManager(ENTITIES::EntityManager* pEntMgr);
		void SetHeaderManager(HEADER::CHeader* pHdrMgr);
		void SetObjectManager(OBJECTS::ObjectManager* pObjMgr);
		void SetTableManager(TABLES::TableManager* pTblMgr);
		bool SaveFile(wchar_t* strPath);
		bool OpenFile(wchar_t* strPath);
		wchar_t* GetErrorMessage();
		void ClearError();
		BLOCKS::BlockManager* GetBlockManager() {return m_pBlkMgr;}
		CLASSES::ClassManager* GetClassManager() {return m_pClsMgr;}
		ENTITIES::EntityManager* GetEntityManager() {return m_pEntMgr;}
		HEADER::CHeader* GetHeaderManager() {return m_pHdrMgr;}
		OBJECTS::ObjectManager* GetObjectManager() {return m_pObjMgr;}
		TABLES::TableManager* GetTableManager() {return m_pTblMgr;}

		short Get16BitHandle();
		int Get32BitHandle();

	protected:
		std::wstring m_strError;
		std::wstring m_strPath;
		BLOCKS::BlockManager* m_pBlkMgr;
		CLASSES::ClassManager* m_pClsMgr;
		ENTITIES::EntityManager* m_pEntMgr;
		HEADER::CHeader* m_pHdrMgr;
		OBJECTS::ObjectManager* m_pObjMgr;
		TABLES::TableManager* m_pTblMgr;

	private:
		bool m_isFirstBlkMgr, m_isFirstClsMgr, m_isFirstEntMgr;
		bool m_isFirstHdrMgr, m_isFirstObjMgr, m_isFirstTblMgr;

		short m_n16BitHandle;
		int m_n32BitHandle;
		wchar_t m_strErrorBuf[256];
	};
}
