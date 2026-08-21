#include "stdafx.h"


//////////////////////////////////////////////////////////////////////////
// Ogre
#include "Ogre.h"
#include "OgreSceneManager.h"
#include "OgreVector3.h"
#include "OgreFont.h"
#include "OgreFontManager.h"

//////////////////////////////////////////////////////////////////////////
// Core API
#include "UPOIManager.h"
#include "TextPOI.h"
#include "TextPOISet.h"


namespace UnE
{
	namespace Core
	{

		typedef std::map<int, UnE::Core::UIconPOI*> UIconPOIList;
		UIconPOIList gIconPOIList;

		typedef std::map<int, UnE::Core::UTextPOI*> UTextPOIList;
		UTextPOIList gTextPOIList;

		//---------------------------------------------------------------------
		UPOIManager::UPOIManager( HWND hWnd, Ogre::SceneManager* pSceneMgr, Ogre::Camera* pCamera )
		{
			m_hWnd = (int)hWnd;
			m_pSceneMgr = pSceneMgr;
			m_pCamera = pCamera;			
		}

		//---------------------------------------------------------------------
		UPOIManager::~UPOIManager()
		{

			ClearIconPOI();
			ClearTextPOI();
		}
		//---------------------------------------------------------------------
		UTextPOI* UPOIManager::AddTextPOI(Ogre::String fontname, Ogre::Vector3 position, float charHeight, Ogre::ColourValue textcolor, Ogre::String caption, float verticalAdjust)
		{
			UTextPOISet* pPOISet = NULL;

			if(mTextPOISetList.find(fontname) == mTextPOISetList.end())
			{			
				pPOISet = new UnE::Core::UTextPOISet(m_pSceneMgr, m_pCamera, fontname);
				mTextPOISetList[fontname] = pPOISet;
			}
			else
			{
				pPOISet = mTextPOISetList[fontname];
			}
			return pPOISet->AddTextPOI(position, charHeight, textcolor, caption, verticalAdjust);
		}
		//---------------------------------------------------------------------
		void UPOIManager::RemoveTextPOI(UnE::Core::UTextPOI* pPOI)
		{
			TextPOISetList::iterator it;
			for(it = mTextPOISetList.begin(); it != mTextPOISetList.end(); it++)
			{
				if(it->second->RemoveTextPOI(pPOI))
				{
					break;
				}
			}
		}
		//---------------------------------------------------------------------
		void UPOIManager::ClearTextPOI()
		{
			TextPOISetList::iterator it;
			for(it = mTextPOISetList.begin(); it != mTextPOISetList.end(); it++)
			{			
				it->second->ClearTextPOI();
				OGRE_DELETE(it->second);				
			}
			mTextPOISetList.clear();
		}
		
		//---------------------------------------------------------------------
		UIconPOI* UPOIManager::AddIconPOI(Ogre::SceneManager* mpSceneMgr, Ogre::Camera* mpCamera, Ogre::String iconPath, Ogre::String iconPath2 ,Ogre::Vector3 position, float width, float height, int depth)
		{
			UIconPOISet* pPOISet = NULL;
			Ogre::String fileName = iconPath;
			size_t pos = iconPath.rfind('\\');
			if(pos != Ogre::String::npos)
			{
				size_t size = iconPath.size();
				fileName = iconPath.substr(pos + 1, size - pos);
				Ogre::String filePath = iconPath.substr(0, pos);
				if(!Ogre::ResourceGroupManager::getSingleton().resourceExists("General", fileName))
				{
					Ogre::ResourceGroupManager::getSingleton().addResourceLocation(filePath,"FileSystem", "General");
				}
			}
			Ogre::String fileName2 = iconPath2;
			size_t pos2 = iconPath2.rfind('\\');
			if(pos2 != Ogre::String::npos)
			{
				size_t size = iconPath2.size();
				fileName2 = iconPath2.substr(pos2 + 1, size - pos2);
				Ogre::String filePath = iconPath.substr(0, pos2);
				if(!Ogre::ResourceGroupManager::getSingleton().resourceExists("General", fileName2))
				{
					Ogre::ResourceGroupManager::getSingleton().addResourceLocation(filePath,"FileSystem", "General");
				}
			}
			if(mIconPOISetList.find(iconPath) == mIconPOISetList.end())
			{			
				pPOISet = new UIconPOISet(mpSceneMgr, mpCamera, fileName, iconPath2, depth);
				mIconPOISetList[iconPath] = pPOISet;
			}
			else
			{
				pPOISet = mIconPOISetList[iconPath];
			}
			return pPOISet->AddIconPOI(position, width, height);
		}
		//---------------------------------------------------------------------
		UIconPOI* UPOIManager::AddIconPOI(Ogre::SceneManager* mpSceneMgr, Ogre::Camera* mpCamera, Ogre::String iconPath, Ogre::Vector3 position, float width, float height, int depth)
		{
			UIconPOISet* pPOISet = NULL;
			Ogre::String fileName = iconPath;
			size_t pos = iconPath.rfind('\\');
			if(pos != Ogre::String::npos)
			{
				size_t size = iconPath.size();
				fileName = iconPath.substr(pos + 1, size - pos);
				Ogre::String filePath = iconPath.substr(0, pos);
				if(!Ogre::ResourceGroupManager::getSingleton().resourceExists("General", fileName))
				{
					Ogre::ResourceGroupManager::getSingleton().addResourceLocation(filePath,"FileSystem", "General");
				}
			}			
			if(mIconPOISetList.find(iconPath) == mIconPOISetList.end())
			{			
				pPOISet = new UIconPOISet(mpSceneMgr, mpCamera, fileName, depth);
				mIconPOISetList[iconPath] = pPOISet;
			}
			else
			{
				pPOISet = mIconPOISetList[iconPath];
			}
			return pPOISet->AddIconPOI(position, width, height);
		}
		//---------------------------------------------------------------------
		void UPOIManager::RemoveIconPOI(UIconPOI* pPOI)
		{			
			IconPOISetList::iterator it;
			for(it = mIconPOISetList.begin(); it != mIconPOISetList.end(); it++)
			{
				if(it->second->RemoveIconPOI(pPOI))
				{
					break;
				}
			}						
		}
		//---------------------------------------------------------------------
		void UPOIManager::ClearIconPOI()
		{			
			IconPOISetList::iterator it;
			for(it = mIconPOISetList.begin(); it != mIconPOISetList.end(); it++)
			{
				OGRE_DELETE(it->second);
			}
			mIconPOISetList.clear();	
			
		}
		//---------------------------------------------------------------------
		
	}
}