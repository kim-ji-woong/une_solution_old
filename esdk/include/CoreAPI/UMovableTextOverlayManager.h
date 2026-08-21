#pragma once

#include "CoreAPI.h"
#include <map>
#include "Ogre.h"
#include "UMovableTextOverlay.h"


namespace UnE
{
	namespace Core
	{

		typedef std::vector<UMovableTextOverlay*> TextOverlays;
		
		class CORE_API UMovableTextOverlayManager : public Ogre::FrameListener
		{
		private:
			
		public:
			UMovableTextOverlayManager(Ogre::SceneManager* pSceneMgr, Ogre::Camera* pCam, Ogre::String fontname);
			virtual ~UMovableTextOverlayManager();

			void Release();
			void Restore();

			UMovableTextOverlay* AddTextPOI(Ogre::Vector3 position, float charHeight, Ogre::ColourValue textcolor, Ogre::String caption, float verticalAdjust = 0);
			bool RemoveTextPOI(UMovableTextOverlay* pPOI);
			void ClearTextPOI();

			//void SetUseDepthSort(bool bDepthSort){ mbDepthSort = bDepthSort; }
			//void SetDepthCheck(bool bDepthCheck){ mbDepthCheck = bDepthCheck; }

		protected:
			void _CreateGeometry();
			void _DestroyGeometry();
			virtual bool frameStarted(const Ogre::FrameEvent& evt);
			virtual bool frameEnded(const Ogre::FrameEvent& evt);

		protected:
			unsigned long mCreateCount;
			Ogre::String mID;
			Ogre::SceneManager* mpSceneMgr;
			Ogre::Camera* mpCamera;
			Ogre::Font*	mpFont;
			Ogre::String mFontName;

			Ogre::MaterialPtr mpFontMaterial;
			Ogre::ManualObject* mpPoiGeometry;
			Ogre::SceneNode* mpNode;
			bool mbIsReleased;

			TextOverlays mOvTextList;
			bool mbDepthSort;
			bool mbDepthCheck;
		};
	}
}

