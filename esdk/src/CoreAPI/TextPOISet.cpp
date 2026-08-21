#include "StdAfx.h"

#include <Ogre.h>
#include <OgreFontManager.h>
#include "UDB.h"
#include "TextPOISet.h"

using namespace Ogre;

namespace UnE
{
	namespace Core
	{

		typedef std::map<int, UnE::Core::UTextPOI*> UTextPOIList;
		extern UTextPOIList gTextPOIList;

		struct TextPoiCompair{
			bool operator()(UTextPOI* a, UTextPOI* b) const 
			{
				return (a->Get2DPosition(false).z > b->Get2DPosition(false).z);
			}
		};


		UTextPOISet::UTextPOISet(SceneManager* pSceneMgr, Camera* pCam, String fontname)
			:mpSceneMgr(pSceneMgr)
			,mpCamera(pCam)
			,mFontName(fontname)
			,mpFont(NULL)
			,mpFontMaterial(NULL)
			,mpPoiGeometry(NULL)
			,mpNode(NULL)
			,mbIsReleased(false)
			,mbDepthSort(true)
			,mbDepthCheck(true)
		{
			mCreateCount = UnE::Core::UDB::GetNextCookie();
			mID = StringConverter::toString(mCreateCount++);
			mpFont = (Ogre::Font *)Ogre::FontManager::getSingleton().getByName("AritaSB", "Popular").getPointer();
			if (!mpFont)
				throw Exception(Exception::ERR_ITEM_NOT_FOUND, "Could not find font " + mFontName, "TextPOISet::TextPOISet");
			if(!mpFont->isLoaded())
				mpFont->load();

			mpFontMaterial = mpFont->getMaterial()->clone("TextPOIMaterial_" + mID);
			if(!mpFontMaterial->isLoaded())
				mpFontMaterial->load();
			mpFontMaterial->getTechnique(0)->getPass(0)->setDepthCheckEnabled(true);
			mpFontMaterial->getTechnique(0)->getPass(0)->setLightingEnabled(false);
			mpFontMaterial->getTechnique(0)->getPass(0)->setVertexColourTracking(TVC_DIFFUSE);
			mpFontMaterial->getTechnique(0)->getPass(0)->setLightingEnabled(false);
			mpFontMaterial->getTechnique(0)->getPass(0)->setDepthWriteEnabled(false);
				
			_CreateGeometry();

			Root::getSingleton().addFrameListener(this);
		}

		UTextPOISet::~UTextPOISet(void)
		{
			ClearTextPOI();
			Root::getSingleton().removeFrameListener(this);
			_DestroyGeometry();
			MaterialManager::getSingleton().remove(mpFontMaterial->getHandle());
		}

		void UTextPOISet::Release()
		{
			if(mbIsReleased) return;
			_DestroyGeometry();
			mbIsReleased = true;
		}

		void UTextPOISet::Restore()
		{
			if(!mbIsReleased) return;
			_CreateGeometry();
			mbIsReleased = false;
		}

		UTextPOI* UTextPOISet::AddTextPOI(Vector3 position, float charHeight, Ogre::ColourValue textcolor, String caption, float verticalAdjust)
		{
			UTextPOI* pText = new UTextPOI(mpSceneMgr, mpCamera, mpFont, position, charHeight, textcolor, caption);
			pText->SetVerticalAdjust(verticalAdjust);
			mTextPOIList.push_back(pText);
			return pText;
		}

		bool UTextPOISet::RemoveTextPOI(UTextPOI* pPOI)
		{
			TextPOIList::iterator it = std::find(mTextPOIList.begin(), mTextPOIList.end(), pPOI);
			if(it != mTextPOIList.end())
			{
				UTextPOI * poi = (*it);
				if( gTextPOIList.size() > 0)
				{
					UTextPOIList::iterator iter = gTextPOIList.find(poi->GetID());
					if(iter  != gTextPOIList.end() )
					{
						UnE::Core::UTextPOI * pIcon = iter->second;
						iter = gTextPOIList.erase(iter);
					}				
				}
				delete (*it);
				mTextPOIList.erase(it);
				return true;
			}
			return false;
		}

		void UTextPOISet::ClearTextPOI()
		{
			for(size_t i = 0; i < mTextPOIList.size(); i++)
			{
				UTextPOI * poi = mTextPOIList.at(i);			
				if( gTextPOIList.size() > 0)
				{
					UTextPOIList::iterator iter = gTextPOIList.find(poi->GetID());
					if(iter  != gTextPOIList.end() )
					{
						UnE::Core::UTextPOI * pIcon = iter->second;
						iter = gTextPOIList.erase(iter);
					}				
				}
				delete poi;
			}
			mTextPOIList.clear();
		}

		void UTextPOISet::_CreateGeometry()
		{
			mpPoiGeometry = mpSceneMgr->createManualObject("TextPOIGeometry_" + mID);
			mpPoiGeometry->setUseIdentityProjection(true);
			mpPoiGeometry->setUseIdentityView(true);
			mpPoiGeometry->begin(mpFontMaterial->getName());
			mpPoiGeometry->position(0,0,0);
			mpPoiGeometry->colour(1,1,1);
			mpPoiGeometry->textureCoord(0,0);
			mpPoiGeometry->position(0,1,0);
			mpPoiGeometry->colour(1,1,1);
			mpPoiGeometry->textureCoord(0,1);
			mpPoiGeometry->position(1,0,0);
			mpPoiGeometry->colour(1,1,1);
			mpPoiGeometry->textureCoord(1,0);
			mpPoiGeometry->triangle(0,1,2);
			mpPoiGeometry->end();

			AxisAlignedBox aabInf;
			aabInf.setInfinite();
			mpPoiGeometry->setBoundingBox(aabInf);
			mpPoiGeometry->setRenderQueueGroup(RENDER_QUEUE_OVERLAY - 2);
			mpPoiGeometry->setCastShadows(false);
			mpPoiGeometry->setVisible(false);
			

			mpNode = mpSceneMgr->getRootSceneNode()->createChildSceneNode("TextPOISetNode_" + mID);
			mpNode->attachObject(mpPoiGeometry);
			Vector3 rootScale = mpSceneMgr->getRootSceneNode()->getScale();
			mpNode->setScale(Vector3(1,1,1) / rootScale);
		}

		void UTextPOISet::_DestroyGeometry()
		{
			mpNode->detachAllObjects();
			mpSceneMgr->destroySceneNode(mpNode->getName());
			mpSceneMgr->destroyManualObject(mpPoiGeometry->getName());
		}

		bool UTextPOISet::frameStarted(const Ogre::FrameEvent& evt)
		{
			if(mbDepthSort)
			{
				for(size_t i = 0; i < mTextPOIList.size(); i++)
				{
					mTextPOIList.at(i)->_Update2DPosition();
				}
				std::sort(mTextPOIList.begin(), mTextPOIList.end(), TextPoiCompair());
			}

			mpPoiGeometry->beginUpdate(0);

			unsigned short idxbase = 0;
			for(size_t i = 0; i < mTextPOIList.size(); i++)
			{
				UTextPOI* pPoi = mTextPOIList.at(i);

				
				

				Vector3 v2DPos = pPoi->Get2DPosition(!mbDepthSort);
				if(!mbDepthCheck) v2DPos.z = -1;
				ColourValue color = pPoi->GetColor();

				pPoi->_UpdateGeometry();

				
				TextPOIGeoMetryData* pData = pPoi->GetPOIGeometryData();
				if(!pPoi->IsDraw() || !pPoi->GetVisible()) continue;

				/*if( mpCamera != NULL)
				{
				Ogre::Vector3 vCamPos = mpCamera->getPosition();
				Ogre::Vector3 vPoiPos = pPoi->Get3DPosition();
				Ogre::Vector3 v3 = vCamPos - vPoiPos;
				float fLength = v3.length();
				if( fLength > 650.0f )
				{
				continue;
				}
				}*/

				size_t capsize = pPoi->GetCaptionSize();
				for(size_t j = 0; j < capsize; j++)
				{
					for(int k = 0; k < 4; k++)
					{
						mpPoiGeometry->position(pData[j].coners[k] + v2DPos);
						mpPoiGeometry->colour(color);
						mpPoiGeometry->textureCoord(pData[j].texCoords[k]);
					}
					for(int k = 0; k < 6; k++)
					{
						mpPoiGeometry->index(idxbase + pData[j].indexs[k]);
					}
					idxbase += 4;
				}

				
			}

			mpPoiGeometry->end();

			if(mTextPOIList.empty())
				mpPoiGeometry->setVisible(false);
			else mpPoiGeometry->setVisible(true);


			
			
			return true;
		}

		bool UTextPOISet::frameEnded(const Ogre::FrameEvent& evt)
		{
			return true;
		}
	}
}
