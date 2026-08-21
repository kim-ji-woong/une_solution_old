#pragma once

namespace Utility
{
	class Vertex3D;
}

namespace DXF
{
	namespace ENTITIES
	{
		class ArcDimension : public Dimension
		{
		public:
			ArcDimension(TABLES::TableManager* pTblMgr, BLOCKS::BlockManager* pBlkMgr, wchar_t* strLayerName);
			virtual ~ArcDimension(void);

		public:
			virtual void Write(Utility::FileManager* pMgr);
			virtual void Init();

		public:
			//void SetBlockName(char* strBlockName);
			//void SetDefinitionPoint(double x, double y, double z);	10,20,30
			//void SetTextMiddlePoint(double x, double y, double z);	11,21,31
			// pt1 : 호의 한쪽 끝점
			// pt2 : 호의 다른쪽 끝점
			// ptCenter : 호의 중점
			void SetTargetPoint(const Utility::Vertex3D& pt1, const Utility::Vertex3D& pt2, const Utility::Vertex3D& ptCenter);

		protected:
			Utility::Vertex3D m_ptTarget1;
			Utility::Vertex3D m_ptTarget2;
			Utility::Vertex3D m_ptTargetCenter;
		};
	}
}
