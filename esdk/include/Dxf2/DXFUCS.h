#pragma once
#include "DXFTable.h"
#include "Vertex.h"

namespace DXF
{
	namespace TABLES
	{
		class Utility::FileManager;
		class TableManager;

		class UCS :	public Table
		{
		public:
			class Entity
			{
			public:
				Entity(UCS* pTable, wchar_t* strUCSName, double dCoordOrigin[3], const Utility::Vertex3D& vDirectionX, const Utility::Vertex3D& vDirectionY);

			public:
				void Write(Utility::FileManager* pMgr);

			protected:
				wchar_t m_strUCSName[256];
				UCS* m_pParent;
				int m_nHandle;
				int m_nFlag;
				double m_dOriginCoord[3];
				Utility::Vertex3D m_vDirection[2];		// X, Y축별 방향 벡터
				int m_nConstant;			// 항상 0
				double m_dElevation;
			};

		public:
			UCS(TableManager* pMgr);
			virtual ~UCS(void);
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