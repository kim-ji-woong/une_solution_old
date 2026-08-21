#pragma once
#include "DXFTable.h"
#include "Vertex.h"

namespace DXF
{
	namespace TABLES
	{
		class TableManager;

		class View : public Table
		{
		public:
			class Entity
			{
			public:
				Entity(View* pTable, wchar_t* strViewName, double dViewHeight, double dViewWidth, double dViewCenterPoint[2], const Utility::Vertex3D& vViewDirection, double dTargetPoint[3]);

			public:
				void Write(Utility::FileManager* pMgr);

			protected:
				wchar_t m_strViewName[256];
				View* m_pParent;
				int m_nHandle;
				int m_nDictionaryHandle;
				int m_nFlag;
				double m_dViewHeight;
				double m_dViewCenterPoint[2];	// X, Y 평면에서
				double m_dViewWidth;
				Utility::Vertex3D m_vViewDirection;
				double m_dTargetPoint[3];
				double m_dLensLength;
				double m_dFrontPlane;
				double m_dBackPlane;
				double m_dTwistAngle;
				int m_nViewMode;
				int m_nRenderMode;
				bool m_bAssociatedUCS;
			};

		public:
			View(TableManager* pMgr);
			virtual ~View(void);
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
