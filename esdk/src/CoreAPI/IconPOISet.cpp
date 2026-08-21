#include "StdAfx.h"
#include <Ogre.h>
#include "IconPOISet.h"

#include "VisiblityMask.h"
#include "UDB.h"



using namespace Ogre;

namespace UnE
{
	namespace Core
	{

		typedef std::map<int, UnE::Core::UIconPOI*> UIconPOIList;
		extern UIconPOIList gIconPOIList;

		struct IconPoiCompair{
			bool operator()(UIconPOI* a, UIconPOI* b) const 
			{
				return (a->Get2DPosition(false).z > b->Get2DPosition(false).z);
			}
		};

		unsigned long UIconPOISet::mCreateCount = 0;
		UIconPOISet::UIconPOISet(SceneManager* pSceneMgr, Camera* pCam, String iconPath, String iconPath2, int depth)
			:mpSceneMgr(pSceneMgr)
			,mpCamera(pCam)
			,mIconPath(iconPath)
			,mpIconMaterial(NULL)
			,mpPoiGeometry(NULL)
			,mpNode(NULL)
			,mbIsReleased(false)
			,mbDepthSort(true)
			,mbDepthCheck(true)
			,mDepth(depth)
		{
			mCreateCount = UnE::Core::UDB::GetNextCookie();
			mID = StringConverter::toString(mCreateCount);
			mpIconMaterial = MaterialManager::getSingleton().create("IconPOISetMaterial_" + mID, "General");
			mpIconMaterial->getTechnique(0)->getPass(0)->setLightingEnabled(false);
			mpIconMaterial->getTechnique(0)->getPass(0)->createTextureUnitState(iconPath);
			mpIconMaterial->getTechnique(0)->getPass(0)->createTextureUnitState(iconPath2, 1);
			mpIconMaterial->getTechnique(0)->getPass(0)->setSceneBlending(SBT_TRANSPARENT_ALPHA);
			mpIconMaterial->getTechnique(0)->getPass(0)->setVertexColourTracking(TVC_AMBIENT);

			_CreateGeometry();

			Root::getSingleton().addFrameListener(this);
		}

		UIconPOISet::UIconPOISet( Ogre::SceneManager* pSceneMgr, Ogre::Camera* pCam, Ogre::String iconPath, int depth /*= 0*/ ):mpSceneMgr(pSceneMgr)
			,mpCamera(pCam)
			,mIconPath(iconPath)
			,mpIconMaterial(NULL)
			,mpPoiGeometry(NULL)
			,mpNode(NULL)
			,mbIsReleased(false)
			,mbDepthSort(true)
			,mbDepthCheck(true)
			,mDepth(depth)
		{
			mCreateCount = UnE::Core::UDB::GetNextCookie();
			mID = StringConverter::toString(mCreateCount);
			mpIconMaterial = MaterialManager::getSingleton().create("IconPOISetMaterial_" + mID, "General");
			mpIconMaterial->getTechnique(0)->getPass(0)->setLightingEnabled(false);
			mpIconMaterial->getTechnique(0)->getPass(0)->createTextureUnitState(iconPath);
			mpIconMaterial->getTechnique(0)->getPass(0)->setSceneBlending(SBT_TRANSPARENT_ALPHA);
			mpIconMaterial->getTechnique(0)->getPass(0)->setVertexColourTracking(TVC_AMBIENT);

			_CreateGeometry();

			Root::getSingleton().addFrameListener(this);
		}

		UIconPOISet::~UIconPOISet(void)
		{			
			Root::getSingleton().removeFrameListener(this);
			ClearIconPOI();
			_DestroyGeometry();
			MaterialManager::getSingleton().remove(mpIconMaterial->getHandle());
		}

		void UIconPOISet::Release()
		{
			if(mbIsReleased) return;
			_DestroyGeometry();
			mbIsReleased = true;
		}

		void UIconPOISet::Restore()
		{
			if(!mbIsReleased) return;
			_CreateGeometry();
			mbIsReleased = false;
		}

		UIconPOI* UIconPOISet::AddIconPOI(Vector3 position, float width, float height)
		{
			UIconPOI* pIcon = new UIconPOI(mpSceneMgr, mpCamera, position, width, height);
			mIconPOIList.push_back(pIcon);
			return pIcon;
		}

		bool UIconPOISet::RemoveIconPOI(UIconPOI* pPOI)
		{
			IconPOIList::iterator it = std::find(mIconPOIList.begin(), mIconPOIList.end(), pPOI);
			if(it != mIconPOIList.end())
			{
				UIconPOI * poi = *it;
				if( gIconPOIList.size() > 0)
				{
					UIconPOIList::iterator iter = gIconPOIList.find(poi->GetID());
					if(iter  != gIconPOIList.end() )
					{
						UnE::Core::UIconPOI * pIcon = iter->second;
						iter = gIconPOIList.erase(iter);
					}				
				}		
				delete poi;
				mIconPOIList.erase(it);
				return true;
			}
			return false;
		}

		void UIconPOISet::ClearIconPOI()
		{
			for(size_t i = 0; i < mIconPOIList.size(); i++)
			{
				UIconPOI* pPOI = mIconPOIList.at(i);
				if( gIconPOIList.size() > 0)
				{
					UIconPOIList::iterator iter = gIconPOIList.find(pPOI->GetID());
					if(iter  != gIconPOIList.end() )
					{
						UnE::Core::UIconPOI * pIcon = iter->second;
						iter = gIconPOIList.erase(iter);
					}				
				}
				delete pPOI;
			}
			mIconPOIList.clear();
		}

		void UIconPOISet::_CreateGeometry()
		{
			mpPoiGeometry = mpSceneMgr->createManualObject("IconPOIGeometry_" + mID);
			mpPoiGeometry->setUseIdentityProjection(true);
			mpPoiGeometry->setUseIdentityView(true);
			mpPoiGeometry->begin(mpIconMaterial->getName());
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
			mpPoiGeometry->setRenderQueueGroup(RENDER_QUEUE_OVERLAY);
			mpPoiGeometry->setVisibilityFlags(VIM_VIRTUALCAMERA);
			mpPoiGeometry->setCastShadows(false);
			mpPoiGeometry->setVisible(false);

			mpNode = mpSceneMgr->getRootSceneNode()->createChildSceneNode("IconPOISetNode_" + mID);
			mpNode->attachObject(mpPoiGeometry);
			Vector3 rootScale = mpSceneMgr->getRootSceneNode()->getScale();
			mpNode->setScale(Vector3(1,1,1) / rootScale);
		}

		void UIconPOISet::_DestroyGeometry()
		{
			mpNode->detachAllObjects();
			mpSceneMgr->destroySceneNode(mpNode->getName());
			mpSceneMgr->destroyManualObject(mpPoiGeometry->getName());
		}

		bool UIconPOISet::frameStarted(const Ogre::FrameEvent& evt)
		{
			if(mbDepthSort)
			{
				for(size_t i = 0; i < mIconPOIList.size(); i++)
				{
					mIconPOIList.at(i)->_Update();
				}
				std::sort(mIconPOIList.begin(), mIconPOIList.end(), IconPoiCompair());
			}

			if(mIconPOIList.empty())
			{
				mpPoiGeometry->setVisible(false);

			}
			else
			{
				mpPoiGeometry->beginUpdate(0);

				unsigned short idxbase = 0;
				for(size_t i = 0; i < mIconPOIList.size(); i++)
				{
					UIconPOI* pPoi = mIconPOIList.at(i);
					IconPOIGeoMetryData data = pPoi->GetGeometryData(!mbDepthSort);
					ColourValue color = pPoi->GetColor(evt.timeSinceLastFrame);
					if(!data.bDraw || !pPoi->GetVisible()) continue;
					for(int j = 0; j < 4; j++)
					{
						if(mbDepthCheck)
							mpPoiGeometry->position(data.coners[j]);
						else
							mpPoiGeometry->position(data.coners[j].x, data.coners[j].y, -1);
						mpPoiGeometry->colour(color);
						mpPoiGeometry->textureCoord(data.texCoords[j]);
					}
					for(int j = 0; j < 6; j++)
					{
						mpPoiGeometry->index(idxbase + data.indexs[j]);
					}
					idxbase += 4;
				}

				mpPoiGeometry->end();
				mpPoiGeometry->setVisible(true);
			}

			return true;
		}

		bool UIconPOISet::frameEnded(const Ogre::FrameEvent& evt)
		{
			return true;
		}
	}
}
