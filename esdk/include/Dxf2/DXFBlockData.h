#pragma once

namespace DXF
{
	namespace BLOCKS
	{
		class BlockManager;

		class BlockData
		{
		public:
			class Block
			{
			public:
				void SetData(int nBlockHandle, int nHandle, wchar_t* strBlockName, wchar_t* strLayerName, double dBasePointX = 0.0, double dBasePointY = 0.0, double dBasePointZ = 0.0, wchar_t* strRefPath = (wchar_t*)L"");
				void SetDefinedData(DXFManager* pDXFMgr, ArrowType type, int nBlockHandle, wchar_t* strBlockName, wchar_t* strLayerName, double dBasePointX = 0.0, double dBasePointY = 0.0, double dBasePointZ = 0.0, wchar_t* strRefPath = (wchar_t*)L"");
				void Write(Utility::FileManager* pMgr);
				void GetBlockName(wchar_t* strBlockName) const;

			protected:
				void SetSmallDotData(DXFManager* pDXFMgr, std::list<DXFData>& rList, int nBlockHandle, wchar_t* strLayerName);
				void SetSlashData(DXFManager* pDXFMgr, std::list<DXFData>& rList, int nBlockHandle, wchar_t* strLayerName);
				void SetTriangleData(DXFManager* pDXFMgr, std::list<DXFData>& rList, int nBlockHandle, wchar_t* strLayerName);
				void SetTwoLineData(DXFManager* pDXFMgr, std::list<DXFData>& rList, int nBlockHandle, wchar_t* strLayerName);
				void SetCircleArrowData(DXFManager* pDXFMgr, std::list<DXFData>& rList, int nBlockHandle, wchar_t* strLayerName);

			protected:
				std::list<DXFData> m_list;
			};

			class EndBlock
			{
			public:
				void SetData(int nBlockHandle, int nHandle, wchar_t* strLayerName);
				void Write(Utility::FileManager* pMgr);

			protected:
				std::list<DXFData> m_list;
			};

		public:
			BlockData(BlockManager* pMgr);
			BlockData(const BlockData& rhs);
			void operator= (const BlockData& rhs);
			virtual ~BlockData(void);

		public:
			virtual void ReadDatai(int nCode, int nData);
			virtual void ReadDatad(int nCode, double dData);
			virtual void ReadDatas(int nCode, wchar_t* strData);

		public:
			void SetData(wchar_t* strBlockName, wchar_t* strLayerName, double dBasePointX = 0.0, double dBasePointY = 0.0, double dBasePointZ = 0.0, wchar_t* strRefPath = (wchar_t*)L"");
			void SetDefinedData(ArrowType type, wchar_t* strLayerName, double dBasePointX = 0.0, double dBasePointY = 0.0, double dBasePointZ = 0.0, wchar_t* strRefPath = (wchar_t*)L"");
			void Write(Utility::FileManager* pMgr);
			int GetBlockHandle();
			void GetBlockName(wchar_t* strBlockName) const;
			void AddEntity(ENTITIES::Entity* pEntity);
			void RemoveEntity(ENTITIES::Entity* pEntity);
			// pID : Entity 정보가 담긴 링크드 리스트 노드의 포인터
			ENTITIES::Entity* GetEntity(void*& pID);
			void SetBlockHandle(int nBlockHandle);
			void GetInsertPoint(double& insertX, double& insertY, double& insertZ);

		private:
			void Copy(const BlockData& rhs);
			void Clear();

		protected:
			std::list<ENTITIES::Entity*> m_listEntity;
			int m_nBlockHandle;
			Block m_blk;
			EndBlock m_endBlock;
			std::wstring m_strLayerName;
			std::wstring m_strBlockName;
			BlockManager* m_pMgr;

		private:
			int* m_pRefCount;
			double m_dArrInsert[3];

			std::list<ENTITIES::Entity*>::iterator m_entIter;
		};
	}
}
