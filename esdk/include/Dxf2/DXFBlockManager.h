#pragma once

namespace DXF
{
	namespace ENTITIES
	{
		class MText;
	}

	namespace BLOCKS
	{
		class BlockManager : public SectionManager
		{
		public:
			BlockManager(void);
			virtual ~BlockManager(void);

		public:
			virtual void ReadDatai(int nCode, int nData);
			virtual void ReadDatad(int nCode, double dData);
			virtual void ReadDatas(int nCode, wchar_t* strData);

		public:
			// 특정 순서의 Block 정보를 알아낸다.
			// 만일 해당 Index의 Block이 존재하지 않으면 false를 리턴한다.
			// nIndex : 몇 번째 Block인가?
			// pBlockHandle : 해당 Block의 핸들
			// strBlockName : 해당 Block의 이름
			bool GetBlockInfo(wchar_t* strBlockName, int* pBlockHandle, int nIndex);
			const BlockData* GetBlockData(const wchar_t* strBlockName);
			BlockData* AddBlock(TABLES::TableManager* pTblMgr, wchar_t* strBlockName, wchar_t* strLayerName, double dBasePointX = 0.0, double dBasePointY = 0.0, double dBasePointZ = 0.0, wchar_t* strRefPath = (wchar_t*)L"");
			void Write(Utility::FileManager* pMgr);
			// MText 임시 객체들을 삭제한다.
			void RemoveTempMText(std::list<ENTITIES::MText*>& tempMTextList);

		protected:
			void Init();

		protected:
			wchar_t m_strDefaultLayerName[256];
			std::list<BlockData> m_list;
			ENTITIES::Entity* m_pEntity;
			BlockData* m_pBlock;

			// 현재 Entity의 데이터를 읽고 있는가?
			// true이면 그렇다.(m_pEntity)
			// false이면 Block의 데이터를 읽고 있다.(m_pBlock)
			bool m_bEntityRead;
		};
	}
}
