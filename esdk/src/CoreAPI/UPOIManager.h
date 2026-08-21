#ifndef __UNE_CORE_API_UPOIMANAGER_H_INCLUDED__
#define __UNE_CORE_API_UPOIMANAGER_H_INCLUDED__

#pragma once


#include "CoreAPI.h"
#include "TextPOISet.h"
#include "IconPOISet.h"

namespace UnE
{
	namespace Core
	{		

		typedef std::map<Ogre::String, UnE::Core::UIconPOISet*> IconPOISetList;
		typedef std::map<Ogre::String, UnE::Core::UTextPOISet*> TextPOISetList;

		class CORE_API UPOIManager
		{
		protected:
			int					m_hWnd;
			Ogre::SceneManager* mpSceneMgr;
			Ogre::Camera* mpCamera;

			IconPOISetList mIconPOISetList;
			TextPOISetList mTextPOISetList;
			
			Ogre::SceneManager* m_pSceneMgr;
			Ogre::Camera*		m_pCamera;

		public:
			UPOIManager(HWND hWnd, Ogre::SceneManager* pSceneMgr, Ogre::Camera* pCamera);
			virtual ~UPOIManager();			

			//////////////////////////////////////////////////////////////////////////
			// TEXT POI
			UnE::Core::UTextPOI* AddTextPOI(Ogre::String fontname, Ogre::Vector3 position, 
				float charHeight, Ogre::ColourValue textcolor,
				Ogre::String caption, float verticalAdjust
				);			
			void RemoveTextPOI(UnE::Core::UTextPOI* pPOI);
			void ClearTextPOI();
			
			//////////////////////////////////////////////////////////////////////////
			// ICON POI
			UnE::Core::UIconPOI* AddIconPOI(Ogre::SceneManager* mpSceneMgr, Ogre::Camera* mpCamera,
				Ogre::String iconPath, Ogre::String iconPath2, Ogre::Vector3 position,
				float width, float height, int depth
				);
			UnE::Core::UIconPOI* AddIconPOI(Ogre::SceneManager* mpSceneMgr, Ogre::Camera* mpCamera,
				Ogre::String iconPath, Ogre::Vector3 position,
				float width, float height, int depth
				);
			void RemoveIconPOI(UIconPOI* pPOI);
			void ClearIconPOI();

	
			
		};
		
	}
}

#endif//__UNE_CORE_API_UPOIMANAGER_H_INCLUDED__
