#pragma once
#include "Vertex.h"

namespace DXF
{
	namespace ENTITIES
	{
		typedef struct _Group102
		{
			std::wstring strName;
			int nHandleCode;
			int nHandle;
		} Group102;

		class EntityManager;

		class Entity
		{
		public:
			Entity(void);
			virtual ~Entity(void);

		public:
			void SetEntityType(wchar_t* strEntityType);
			int GetHandle();
			void SetSoftPointer(int nPointer);
			void SetSubClass(wchar_t* strClassName);
			void SetOwnLayer(wchar_t* strLayer);
			bool IsSupoorted();
			void SetLineWeight(float fLineWeight);
			void SetLineWidth(float fLineWidth);
			void Add102Group(wchar_t* strGroupName, int nHandleCode, int nHandle);
			void Remove102Group(wchar_t* strGroupName);
			void Set102Handle(wchar_t* strGroupName, int nHandle);
			void SetColorIndex(int nColorIndex);
			int GetColorIndex();
			wchar_t* GetEntityType();
			wchar_t* GetOwnLayer();
			void SetLineType(TABLES::LType::Entity* pLineType);
			TABLES::LType::Entity* GetLineType();
			// nIndex : X축(0), Y축(1), Z축(2)
			void SetAxisVector(int nIndex, Utility::Vertex3D& rVector);
			// nIndex : X축(0), Y축(1), Z축(2)
			void SetAxisVector(int nIndex, double x, double y, double z);
			// nIndex : X축(0), Y축(1), Z축(2)
			bool GetAxisVector(int nIndex, double& x, double& y, double& z);
			void SetManager(EntityManager* pMgr);

		public:
			// WCS 좌표계에서의 좌표(rCoordX,rCoordY,rCoordZ)를 OCS 좌표계의 좌표로 바꾼다.
			// vNormal은 WCS에서의 좌표가 위치한 평면의 법선 벡터이다.
			static void WCSToOCS(double& rCoordX, double& rCoordY, double& rCoordZ, const Utility::Vertex3D& vNormal);
			// OCS 좌표계에서의 좌표(rCoordX,rCoordY,rCoordZ)를 WCS 좌표계의 좌표로 바꾼다.
			// vNormal은 WCS에서의 좌표가 위치한 평면의 법선 벡터이다.
			static void OCSToWCS(double& rCoordX, double& rCoordY, double& rCoordZ, const Utility::Vertex3D& vNormal);

		public:
			virtual void Write(Utility::FileManager* pMgr);
			virtual void Init() = 0;
			virtual bool ReadDatai(int nCode, int nData);
			virtual bool ReadDatad(int nCode, double dData);
			virtual bool ReadDatas(int nCode, wchar_t* strData);
			virtual void SetHandle(int nHandle);

		protected:
			std::wstring m_strEntityType;	// 0
			int m_nHandle;					// 5
			int m_nSoftPointer;			// 330
			std::wstring m_strSubClassName;	// 100
			std::wstring m_strDefaultSubClassName;	// 100
			std::wstring m_strOwnLayer;		// 8
			float m_fLineWeight;			// 선 가중치
			float m_fLineWidth;				// 선 두께
			bool m_bNotSupported;
			std::list<Group102> m_list102;	// 102 Group Code에 대한 리스트
			int m_nColorIndex;				// CAD에서 정의된 색상 번호
			TABLES::LType::Entity* m_pLineType;	// 0이면 ByLayer
			Utility::Vertex3D m_vecAxis[3];		// 축별 방향 벡터
			EntityManager* m_pMgr;
		};
	}
}
