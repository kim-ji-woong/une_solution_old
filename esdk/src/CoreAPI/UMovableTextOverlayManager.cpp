#include "StdAfx.h"

#include <Ogre.h>
#include <OgreFontManager.h>
#include "UDB.h"
#include "UMovableTextOverlayManager.h"
#include "URectLayoutManager.h"

using namespace Ogre;

namespace UnE
{
	namespace Core
	{
		unsigned long nMvTextManagerCount = 0;
		typedef std::map<int, UMovableTextOverlay*> UTextOverlayList;
		UTextOverlayList gMovableTextList;

		UMovableTextOverlayManager::UMovableTextOverlayManager(SceneManager* pSceneMgr, Camera* pCam, String fontname)
			:mpSceneMgr(pSceneMgr)
			, mpCamera(pCam)
			, mFontName(fontname)
			, mpFont(NULL)
			, mpFontMaterial(NULL)
			, mpPoiGeometry(NULL)
			, mpNode(NULL)
			, mbIsReleased(false)
			, mbDepthSort(true)
			, mbDepthCheck(true)
		{
			mCreateCount = nMvTextManagerCount++;
			mID = StringConverter::toString(mCreateCount++);			

			_CreateGeometry();

			Root::getSingleton().addFrameListener(this);
		}

		UMovableTextOverlayManager::~UMovableTextOverlayManager(void)
		{
			ClearTextPOI();

			Root::getSingleton().removeFrameListener(this);
			
			_DestroyGeometry();
			
			MaterialManager::getSingleton().remove(mpFontMaterial->getHandle());
		}

		void UMovableTextOverlayManager::Release()
		{
			if (mbIsReleased)
				return;
			_DestroyGeometry();
			mbIsReleased = true;
		}

		void UMovableTextOverlayManager::Restore()
		{
			if (!mbIsReleased) return;
			_CreateGeometry();
			mbIsReleased = false;
		}

		UMovableTextOverlay* UMovableTextOverlayManager::AddTextPOI(Vector3 position, float charHeight, Ogre::ColourValue textcolor, String caption, float verticalAdjust)
		{
			int nCookie = UDB::GetNextCookie();

			UMovableTextOverlayAttributes *attrs = new UMovableTextOverlayAttributes("Attrs1", mpCamera, mFontName, charHeight, textcolor, "BorderPane");
			UMovableTextOverlay* pText = new UMovableTextOverlay(nCookie, NULL, mpCamera, position, caption, attrs);
			pText->enable(false); // make it invisible for now
			pText->setUpdateFrequency(0.01);// set update frequency to 0.01 seconds

			//pText->SetVerticalAdjust(verticalAdjust);
			mOvTextList.push_back(pText);
			return pText;
		}

		bool UMovableTextOverlayManager::RemoveTextPOI(UMovableTextOverlay* pPOI)
		{
			TextOverlays::iterator it = std::find(mOvTextList.begin(), mOvTextList.end(), pPOI);
			if (it != mOvTextList.end())
			{
				UMovableTextOverlay * poi = (*it);
				if (gMovableTextList.size() > 0)
				{
					UTextOverlayList::iterator iter = gMovableTextList.find(poi->GetID());
					if (iter != gMovableTextList.end())
					{
						UnE::Core::UMovableTextOverlay * pIcon = iter->second;
						iter = gMovableTextList.erase(iter);
					}
				}
				delete (*it);
				mOvTextList.erase(it);
				return true;
			}
			return false;
		}

		void UMovableTextOverlayManager::ClearTextPOI()
		{
			for (size_t i = 0; i < mOvTextList.size(); i++)
			{
				UMovableTextOverlay * poi = mOvTextList.at(i);
				if (gMovableTextList.size() > 0)
				{
					UTextOverlayList::iterator iter = gMovableTextList.find(poi->GetID());
					if (iter != gMovableTextList.end())
					{
						UnE::Core::UMovableTextOverlay * pIcon = iter->second;
						iter = gMovableTextList.erase(iter);
					}
				}
				delete poi;
			}
			mOvTextList.clear();
		}

		void UMovableTextOverlayManager::_CreateGeometry()
		{
			
		}

		void UMovableTextOverlayManager::_DestroyGeometry()
		{
			
		}

		bool UMovableTextOverlayManager::frameStarted(const Ogre::FrameEvent& evt)
		{	
			URectLayoutManager m(0, 0, mpCamera->getViewport()->getActualWidth(),
				mpCamera->getViewport()->getActualHeight());
			
			m.setDepth(0);

			int visible = 0;
			UMovableTextOverlay *p = 0;
			for (std::vector<UMovableTextOverlay*>::iterator i = mOvTextList.begin(); i<mOvTextList.end(); i++)
			{
				p = *i;
				p->update(evt.timeSinceLastFrame);
				
			}
			return true;
		}

		bool UMovableTextOverlayManager::frameEnded(const Ogre::FrameEvent& evt)
		{
			return true;
		}
	}
}
