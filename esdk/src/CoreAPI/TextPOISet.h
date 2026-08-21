#pragma once

#include "CoreAPI.h"
#include "TextPOI.h"

namespace UnE
{
	namespace Core
	{
		typedef std::vector<UTextPOI*> TextPOIList; 
		class CORE_API UTextPOISet : public Ogre::FrameListener
		{
		public:
			UTextPOISet(Ogre::SceneManager* pSceneMgr, Ogre::Camera* pCam, Ogre::String fontname);
			~UTextPOISet(void);

			void Release();
			void Restore();

			UTextPOI* AddTextPOI(Ogre::Vector3 position, float charHeight, Ogre::ColourValue textcolor, Ogre::String caption, float verticalAdjust = 0);
			bool RemoveTextPOI(UTextPOI* pPOI);
			void ClearTextPOI();

			void SetUseDepthSort(bool bDepthSort){mbDepthSort = bDepthSort;}
			void SetDepthCheck(bool bDepthCheck){mbDepthCheck = bDepthCheck;}

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

			TextPOIList mTextPOIList;
			bool mbDepthSort;
			bool mbDepthCheck;
		};
	}
}