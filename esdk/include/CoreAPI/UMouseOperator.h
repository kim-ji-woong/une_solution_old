#ifndef MouseManipulator_h__
#define MouseManipulator_h__


#pragma once

#include "UBaseOperator.h"
#include "UVector3.h"
#include "UPlane.h"
#include "URay.h"


namespace UnE
{
	namespace Core
	{
		class Camera;
		class UEntity;
		class USceneNode;

		class CORE_API MouseOperator : public UBaseOperator
		{
			friend class UBaseView;
		protected:
			void Zoom(int zDelta);
			void Pan( CPoint pt, bool bMove = false);

			void Orbit(CPoint pt, bool bMove = false);
			//void Orbit( CPoint pt1, CPoint pt2 );
			void Orbit( float fAngleX, float fAngleY );
			void SetOrbitCenter(UnE::Math::Vector3 vCenter);
			void SetOrbitRadius(Real fLength);

			BOOL Pick(CPoint pt, UnE::Math::Vector3& vec,unsigned int mask);


		

		public:
			MouseOperator(HWND	m_hWnd);
			MouseOperator();
			virtual ~MouseOperator(void);	

			CPoint		 GetSavedPoint() const { return mSavePt; }
			UINT		 GetSavedFlags() const { return mSaveFlag; }
			void		 SavePoint(UINT nFlag, CPoint pt);

			virtual BOOL OnMouseWheel(UINT nFlags, short zDelta, CPoint pt);
			virtual BOOL OnMouseMove (UINT nFlags, CPoint pt);
			virtual BOOL OnMButtonUp(UINT nFlags, CPoint point);
			virtual BOOL OnMButtonDown(UINT nFlags, CPoint point);
			virtual BOOL OnLButtonUp(UINT nFlags, CPoint point);
			virtual BOOL OnLButtonDown(UINT nFlags, CPoint point);
			virtual BOOL OnRButtonUp(UINT nFlags, CPoint point);
			virtual BOOL OnRButtonDown(UINT nFlags, CPoint point);

			virtual BOOL OnSelect(UINT nFlags, CPoint point);
			virtual BOOL OnSelectNode(UINT nFlags, CPoint point);
			virtual BOOL OnDelete(UINT nFlags, CPoint point);
			void ClearSelect();

			virtual void Reset();

			HWND GetHWnd() const;
			void SetHWnd(HWND val);
			void SelectPanPlane(CPoint point, UnE::Math::Vector3& vec);


			////  beta function ////
			void CreateSphere();

			void RemoveTexture();
			void SetTexture(std::string& szPath);

			void SetZoomObject(std::string szObjectName);
			bool GetObjectPoint(std::string szObjectName, UnE::Math::Vector3& vPos);

			UnE::Math::Vector3 GetLastPoistion();
			
			UEntity * SelectedObject() const { return m_pSelectedObject; }
			
			USceneNode * SelectNode() const { return m_pSelectNode; }

			BOOL Get3DPosition(CPoint pt, UnE::Math::Vector3& vec);

			BOOL Get2DPosition(UnE::Math::Vector3 vec, CPoint& pt);

			void TargetZoom( UnE::Math::Vector3 target, float dist );

			int	OnSelectPOI (int x, int y);

			BOOL CheckInFrustum(float x, float y, float z);

		protected:

			bool m_bMouseOrbitMode;
			bool m_bMousePanMode;
			bool m_bUseTrackBall;

			float mOrbitRadius;
			float mPickDistance;


			HWND	m_hWnd;
			CPoint m_PtPrev;
			CPoint m_PtCurr;
			
			UnE::Math::Vector3 m_vPrev;
			UnE::Math::Vector3 m_vCurrent;
			UnE::Math::Vector3 mOrbitCenter;
			UnE::Math::Vector3 m_PickPoint;
			UnE::Math::Ray     m_pickRay;
			

			UnE::Math::Quaternion mPrvQuat;

			UnE::Core::Camera * mCamera;

		
			UnE::Math::Plane mSelectPanPlane;
			UnE::Math::Vector3 mSelectPanPt;

			CPoint  mSavePt;
			
			UINT	mSaveFlag;
			bool	bSavePt;

			std::vector<void*> mSelectedEntity;

			UEntity * m_pSelectedObject;
			
			USceneNode * m_pSelectNode;

			virtual UOpType GetType();

		private:
			UnE::Math::Vector3 m_vCameraPos;
			UnE::Math::Vector3 m_vCameraDir;		

			void SetAnimation(UnE::Math::Vector3 vPos, UnE::Math::Vector3 vDir);
		};

	}
}



#endif // MouseManipulator_h__
