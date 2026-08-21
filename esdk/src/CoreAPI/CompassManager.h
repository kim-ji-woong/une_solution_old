#pragma once

#include "Ogre.h"


namespace UnE
{
	namespace Core
	{
		class CCompassManager : public Ogre::FrameListener, public Ogre::RenderQueueListener
		{
		public:
			CCompassManager(HWND hWindow);
			~CCompassManager(void);

			void Release();
			void Restore();
			void SetVisible(bool bVisible);
			bool SetAzimuth(float azimuth);
			float GetAzimuth(){ return mAzimuth; }
			void SetCameraLinkage(bool bLink){ mbCameraLinkage = bLink; }
			bool GetCameraLinkage(){ return mbCameraLinkage; }
			void Draw2D(CDC* pDC, CRect rt);
			void Set2DImagePath(std::string strPath);

		protected:
			void _Create();
			void _Destroy();
			virtual void renderQueueStarted(Ogre::uint8 queueGroupId, const Ogre::String& invocation, bool& skipThisInvocation);
			virtual void renderQueueEnded(Ogre::uint8 queueGroupId, const Ogre::String& invocation, bool& repeatThisInvocation);
			void _UpdateCompass();
			virtual bool frameStarted(const Ogre::FrameEvent& evt);
			virtual bool frameEnded(const Ogre::FrameEvent& evt);

		protected:
			Ogre::SceneManager* mpSceneManager;
			Ogre::Camera *  mpCamera;
			Ogre::Entity*		mpCompassEntity;
			Ogre::SceneNode*	mpCompassNode;
			Ogre::Overlay*		mpCompassOverlay;

			bool mbIsReleased;
			float mScale;
			bool mbVisible;
			Ogre::Vector3 mPosition;
			Ogre::ProjectionType mPorjType;
			float mNearDist;
			Ogre::Radian mFov;
			bool mbCameraLinkage;

			float mAzimuth;
			float mOrthoHeight;
			float mSaveOrthoHeight;
			bool mbInvAzimuth;

			Ogre::String m2DImagePass;
			bool mbHas2DImage;
			int m2DWidth;
			int m2DHeight;
			int m2DGap;

			HWND m_hWnd;
		};
	}
}
