#pragma once
#include "DXFTable.h"
#include "Vertex.h"

namespace DXF
{
	namespace TABLES
	{
		class TableManager;

		class VPort : public Table
		{
		public:
			class Entity
			{
			public:
				Entity(int nHandle);
				Entity(VPort* pTable, wchar_t* strVPortName, int nUCSHandle = 0, 
					Utility::Vertex3D vUCSOrigin = Utility::Vertex3D(0,0,0), 
					Utility::Vertex3D vAxisX = Utility::Vertex3D(1,0,0), 
					Utility::Vertex3D vAxisY = Utility::Vertex3D(0,1,0));

			public:
				void Write(Utility::FileManager* pMgr);
				void SetViewportName(wchar_t* strViewportName);
				void SetViewportCenter(double dX, double dY);
				void SetUCSAxis(const Utility::Vertex3D& vAxisX, const Utility::Vertex3D& vAxisY);
				wchar_t* GetVPortName();
				void GetCenterPoint(double* pX, double* pY);
				void GetUCSAxisX(double* pX, double* pY, double* pZ);
				void GetUCSAxisY(double* pX, double* pY, double* pZ);
				void SetViewportHeight(double dHeight);
				// dAspect : 뷰의 너비/ 뷰의 높이
				void SetViewportAspect(double dAspect);
				double GetViewportHeight();
				double GetViewportAspect();
				int GetHandle();

			protected:
				std::wstring m_strVPortName;
				VPort* m_pParent;
				int m_nHandle;
				int m_nFlag;
				double m_dBL[2];
				double m_dTR[2];
				double m_dCenter[2];
				double m_dSnapBasePoint[2];
				double m_dSnapSpace[2];
				double m_dGridSpace[2];
				double m_dViewDirection[3]; // View Direction from Target Point
				double m_dTargetPoint[3];
				double m_dViewHeight;
				double m_dAspect;
				double m_dLensLength;
				double m_dFrontPlane;
				double m_dBackPlane;
				double m_dSnapAngle;
				double m_dTwistAngle;
				int m_nViewMode;
				int m_nCircleZoomPercent;
				int m_nFastZoomSetting;
				int m_nIconSetting;
				bool m_bSnapOnOff;
				bool m_bGridOnOff;
				int m_nSnapStyle;
				int m_nSnapIsopair;
				int m_nRenderMode;
				int m_nUCSVP;
				double m_dUCSOrigin[3];
				Utility::Vertex3D m_vUCSAxis[2];	// X, Y축에 관한 UCS 벡터
				int m_nUCSHandle;		// 사용자 정의 UCS가 존재하는 경우 해당 UCS의 Handle
										// 존재하지 않으면 사용하지 않는다.(Code 345)
				int m_nOrthographicType;
				double m_dElevation;
			};

		public:
			VPort(TableManager* pMgr);
			virtual ~VPort(void);
			friend class Entity;

		public:
			void Clear();
			void Init();
			void Write(Utility::FileManager* pMgr);
			Entity* GetActiveEntity();
			// pID : Viewport 정보를 담고 있는 링크드 리스트 노드의 포인터
			Entity* GetEntity(void*& pID);

		public:
			virtual void ReadDatai(int nCode, int nData);
			virtual void ReadDatad(int nCode, double dData);
			virtual void ReadDatas(int nCode, wchar_t* strData);

		protected:
			std::list<Entity> m_list;
			Entity* m_pEntity;

		private:
			double m_dArrTemp[6];
			std::list<Entity>::iterator m_entIter;
		};
	}
}
