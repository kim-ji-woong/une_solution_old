#pragma once

#include "CoreAPI.h"
#include "IconPOI.h"


namespace UnE

{
	namespace Core
	{
		typedef std::vector<UIconPOI*> IconPOIList; 
		class CORE_API UIconPOISet : public Ogre::FrameListener
		{
		public:
			UIconPOISet(Ogre::SceneManager* pSceneMgr, Ogre::Camera* pCam, Ogre::String iconPath, int depth = 0);
			UIconPOISet(Ogre::SceneManager* pSceneMgr, Ogre::Camera* pCam, Ogre::String iconPath, Ogre::String iconPath2, int depth = 0);
			~UIconPOISet(void);

			void Release();
			void Restore();

			UIconPOI* AddIconPOI(Ogre::Vector3 position, float width, float height);
			bool RemoveIconPOI(UIconPOI* pPOI);
			void ClearIconPOI();



			void SetUseDepthSort(bool bDepthSort){mbDepthSort = bDepthSort;}
			void SetDepthCheck(bool bDepthCheck){mbDepthCheck = bDepthCheck;}

		protected:
			void _CreateGeometry();
			void _DestroyGeometry();
			virtual bool frameStarted(const Ogre::FrameEvent& evt);
			virtual bool frameEnded(const Ogre::FrameEvent& evt);

		protected:
			static unsigned long mCreateCount;
			Ogre::String mID;
			Ogre::SceneManager* mpSceneMgr;
			Ogre::Camera* mpCamera;
			Ogre::String mIconPath;
			Ogre::MaterialPtr mpIconMaterial;
			Ogre::ManualObject* mpPoiGeometry;
			Ogre::SceneNode* mpNode;
			bool mbIsReleased;

			IconPOIList mIconPOIList;
			bool mbDepthSort;
			bool mbDepthCheck;

			int mDepth;
		};
	}
}
