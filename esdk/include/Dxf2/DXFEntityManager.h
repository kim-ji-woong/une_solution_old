#pragma once
#include "DXFHatch.h"

class FileManager;

namespace DXF
{
	namespace BLOCKS
	{
		class BlockManager;
	}

	namespace ENTITIES
	{
		class Entity;
		class MText;

		class EntityManager : public SectionManager
		{
		public:
			EntityManager(void);
			virtual ~EntityManager(void);

		public:
			void AddEntity(Entity* pEnt);
			void Write(Utility::FileManager* pMgr);
			void Clear();
			// pID : Entity 정보를 담고 있는 링크드 리스트 노드의 포인터
			Entity* GetEntity(void*& pID);
			// nHandle을 소유한 Entity를 찾아낸다.
			Entity* GetEntity(int nHandle);

			Hatch::HatchPatternGroup* HatchPatternGroups();

			void AddTempMText(MText* pText, int nHandle);
			MText* FindFirstMText(MText* pText);
			// MText 임시 객체들을 삭제한다.
			void RemoveTempMText(BLOCKS::BlockManager* pBlockManager);

		public:
			virtual void ReadDatai(int nCode, int nData);
			virtual void ReadDatad(int nCode, double dData);
			virtual void ReadDatas(int nCode, wchar_t* strData);

			unsigned int GetAllEntityCount()
			{
				return m_list.size();
			}

		protected:
			void MakeHatchPattern();

		protected:
			std::list<Entity*> m_list;
			Entity* m_pEntity;

			int m_nHatchPatternSize;
			Hatch::HatchPatternGroup m_arrHatchPatternGroup[21];

		private:
			std::list<Entity*>::iterator m_entIter;
			// MText는 하나의 Text를 여러객체가 나눠서 보유할 수 있다.
			// Text의 첫번째 요소객체(MText)가 나머지 연결될 객체정보를 가지고 있게 되는데
			// 연결될 MText 객체가 나타나면 첫번째 객체에 Text를 연결시켜야 하므로
			// 연결될 MText가 첫번째 MText를 찾아갈 수 있어야 한다.
			// Key ; 연결될 MText ID
			// Value : 첫번째 MText
			std::map<int, MText*> m_mapTextOwner;
			std::list<MText*> m_tempMTextList;
		};
	}
}
