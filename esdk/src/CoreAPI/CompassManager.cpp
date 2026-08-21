#include "stdafx.h"
#include "CoreAPI.h"
#include "CompassManager.h"
#include "io.h"


#include <algorithm>  
#ifndef max 
#define max(a,b) (((a) > (b)) ? (a) : (b)) 
#endif 
#ifndef min 
#define min(a,b) (((a) < (b)) ? (a) : (b)) 
#endif 

#include <gdiplus.h> 
#undef max 
#undef min 


using namespace Ogre;


namespace UnE
{
	namespace Core
	{
		CCompassManager::CCompassManager(HWND hWindow)
			:mpSceneManager(NULL)
			, mpCompassEntity(NULL)
			, mpCompassNode(NULL)
			, mpCompassOverlay(NULL)
			, mbIsReleased(false)
			, mScale(0.010f)
			, mbVisible(true)
			, mbCameraLinkage(true)
			, mAzimuth(0)
			, mbInvAzimuth(false)			
			, mbHas2DImage(false)
			, m2DWidth(150)
			, m2DHeight(150)
			, m2DGap(20)
		{

			//FILE * fout = fopen("c:\\temp\\tttttt.txt","w");

			m_hWnd = hWindow;


			WndCtx * pCtx = GetWndContext(m_hWnd);

			/*fputc('1', fout);
			fclose(fout);*/

			mOrthoHeight = 80.0f;
			mpSceneManager = pCtx->sceneMgr;
			mpCamera = pCtx->camera;
			float nearDist = mpCamera->getNearClipDistance();

			float y = 20.0f;
			float x = (y + 0.4) * mpCamera->getAspectRatio();
			mPosition = Vector3(x, y, -10);
			/*fout = fopen("c:\\temp\\tttttt.txt", "a");
			fputc('2', fout);
			fclose(fout);*/

			_Create();

			/*fout = fopen("c:\\temp\\tttttt.txt", "a");
			fputc('11', fout);
			fclose(fout);*/
			Root::getSingleton().addFrameListener(this);
			mpSceneManager->addRenderQueueListener(this);

			/*fout = fopen("c:\\temp\\tttttt.txt", "a");
			fputc('12', fout);
			fclose(fout);*/
		}

		CCompassManager::~CCompassManager(void)
		{
			mpSceneManager->removeRenderQueueListener(this);
			Root::getSingleton().removeFrameListener(this);
			_Destroy();
		}

		void CCompassManager::_Create()
		{
			int nID = (int)m_hWnd;

			Ogre::String szEntityName = "CompassModelEntity";
			mpCompassEntity = mpSceneManager->createEntity(szEntityName, "CompassObj.mesh", "Popular");

			//mpCompassEntity-("Compass", "Popular");

			mpCompassEntity->setRenderQueueGroup(RENDER_QUEUE_OVERLAY);
			
			mpCompassEntity->setVisible(true);
			mpCompassEntity->setCastShadows(false);
			
			bool bHasVP = Root::getSingleton().getRenderSystem()->getCapabilities()->hasCapability(Ogre::RSC_VERTEX_PROGRAM);
			bool bHasFP = Root::getSingleton().getRenderSystem()->getCapabilities()->hasCapability(Ogre::RSC_FRAGMENT_PROGRAM);
			if (bHasVP && bHasFP)
			{
				mpCompassEntity->getSubEntity(0)->setMaterialName("_07_-_Default", "Popular" );
				mpCompassEntity->getSubEntity(1)->setMaterialName("_02_-_Default", "Popular");
				mpCompassEntity->getSubEntity(2)->setMaterialName("_01_-_Default", "Popular");
				mpCompassEntity->getSubEntity(3)->setMaterialName("_03_-_Default", "Popular");
			}
			////mpCompassEntity->setVisibilityFlags(1 << 0);

			Ogre::String szSceneNodeName = "CompassNode";
			mpCompassNode = mpSceneManager->createSceneNode(szSceneNodeName);
			mpCompassNode->attachObject(mpCompassEntity);
			mpCompassNode->setPosition(mPosition);			
			mpCompassNode->setScale(mScale, mScale, mScale);

	
			mpCompassNode->setVisible(true);

			Ogre::String szOverlayName = "CompassOverlay";
			
			mpCompassOverlay = OverlayManager::getSingleton().create(szOverlayName);
			mpCompassOverlay->add3D(mpCompassNode);
			mpCompassOverlay->setZOrder(100);

			if (mbVisible)
			{
				mpCompassOverlay->show();
				if (mpCompassEntity)
					mpCompassEntity->setVisible(true);
			}
			else
			{
				mpCompassOverlay->hide();
				if (mpCompassEntity)
					mpCompassEntity->setVisible(false);
			}
		}

		void CCompassManager::_Destroy()
		{

			mpCompassOverlay->remove3D(mpCompassNode);
			OverlayManager::getSingleton().destroy(mpCompassOverlay);
			mpCompassNode->removeAllChildren();
			mpSceneManager->destroyEntity(mpCompassEntity->getName());
			mpSceneManager->destroySceneNode(mpCompassNode->getName());
			mpCompassEntity = NULL;
			mpCompassNode = NULL;
			mpCompassOverlay = NULL;
		}

		void CCompassManager::Release()
		{
			if (mbIsReleased) return;
			_Destroy();
			mbIsReleased = true;
		}

		void CCompassManager::Restore()
		{
			if (!mbIsReleased)
				return;

			WndCtx * pCtx = GetWndContext(m_hWnd);
			mpSceneManager = pCtx->sceneMgr;
			mpCamera = pCtx->camera;

			_Create();
			mbIsReleased = false;
		}

		void CCompassManager::SetVisible(bool bVisible)
		{
			if (mbVisible != bVisible)
			{
				mbVisible = bVisible;
				if (mbVisible)
				{
					mpCompassOverlay->show();
					if (mpCompassEntity)
						mpCompassEntity->setVisible(true);
				}
				else
				{
					mpCompassOverlay->hide();
					if (mpCompassEntity)
						mpCompassEntity->setVisible(false);
				}
			}
		}

		bool CCompassManager::SetAzimuth(float azimuth)
		{
			if (mAzimuth != azimuth)
			{
				mAzimuth = azimuth;
				return true;
			}
			return false;
		}

		void CCompassManager::_UpdateCompass()
		{
			if (mbVisible)
			{
				if (mpCompassNode == NULL)
					return;
				if (mpCamera == NULL )
					return;
				Ogre::Camera* pCam = mpCamera;
				float nearDist = pCam->getNearClipDistance();
				float width = pCam->getOrthoWindowWidth();
				float height = pCam->getOrthoWindowHeight();
				
				float asp = pCam->getAspectRatio();
				pCam->getViewport()->getWidth() * 0.5f;
				float y = 3.6f;
				float x = 0.0f;
				if (asp > 1.0f)
				{
					x = (y + 0.05f) * pCam->getAspectRatio();
				}
				else
				{
					x = y * pCam->getAspectRatio();
				}
				
				mPosition = Vector3(x, y, -10);				
				mpCompassNode->setPosition(mPosition);

				float azimuth = mAzimuth;
				if (mbInvAzimuth)
				{
					azimuth *= -1;
				}
				Quaternion qAzimuth = Quaternion(Radian(Degree(180 - azimuth)), Vector3::UNIT_Y);
				if (mbCameraLinkage)
				{
					Quaternion qCamOrient = pCam->getOrientation().UnitInverse();
					qAzimuth = qCamOrient * qAzimuth;
				}
				else
				{
					Quaternion qPitch = Quaternion(Radian(Degree(90)), Vector3::UNIT_X);
					qAzimuth = qPitch * qAzimuth;
				}
				mpCompassNode->setOrientation(qAzimuth);
			}
		}

		bool CCompassManager::frameStarted(const Ogre::FrameEvent& evt)
		{	
			//if (Root::getSingleton()._getCurrentSceneManager() == mpSceneManager)
				_UpdateCompass();
			//else
			//	SetVisible(false);
			return true;
		}

		bool CCompassManager::frameEnded(const Ogre::FrameEvent& evt)
		{
			return true;
		}

		void CCompassManager::renderQueueStarted(Ogre::uint8 queueGroupId, const Ogre::String& invocation, bool& skipThisInvocation)
		{
			if (queueGroupId == RENDER_QUEUE_OVERLAY)
			{
				/*Ogre::RenderSystem* pRenderSys = Ogre::Root::getSingleton().getRenderSystem();
				Ogre::Camera* pCamera = pRenderSys->_getViewport()->getCamera();
				mPorjType = pCamera->getProjectionType();
				pCamera->setProjectionType(PT_ORTHOGRAPHIC);
				mNearDist = pCamera->getNearClipDistance();
				pCamera->setNearClipDistance(5);
				mSaveOrthoHeight = pCamera->getOrthoWindowHeight();
				pCamera->setOrthoWindowHeight(mOrthoHeight);
				mFov = pCamera->getFOVy();
				pCamera->setFOVy(Radian(Degree(45)));
*/

				/*if (Root::getSingleton()._getCurrentSceneManager() == mpSceneManager)
					_UpdateCompass();*/
				//else
				//	SetVisible(false);
			}
		}

		void CCompassManager::renderQueueEnded(Ogre::uint8 queueGroupId, const Ogre::String& invocation, bool& repeatThisInvocation)
		{
			if (queueGroupId == RENDER_QUEUE_OVERLAY)
			{
				/*Ogre::RenderSystem* pRenderSys = Ogre::Root::getSingleton().getRenderSystem();
				Ogre::Camera* pCamera = pRenderSys->_getViewport()->getCamera();
				pCamera->setProjectionType(mPorjType);
				pCamera->setNearClipDistance(mNearDist);
				pCamera->setOrthoWindowHeight(mSaveOrthoHeight);
				pCamera->setFOVy(mFov);*/
			}
		}

		void CCompassManager::Draw2D(CDC* pDC, CRect rt)
		{
			if (mbHas2DImage && mbVisible)
			{
				WCHAR* szWchar;
				szWchar = new WCHAR[strlen(m2DImagePass.c_str()) + 1];
				_swprintf(szWchar, L"%s", CA2W(m2DImagePass.c_str()));
				Gdiplus::Graphics grp(pDC->m_hDC);
				Gdiplus::Image img(szWchar);
				delete[] szWchar;

				int posX = rt.Width() - m2DWidth - m2DGap;
				int posY = m2DGap;

				float azimuth = mAzimuth;
				if (mbInvAzimuth)
				{
					azimuth *= -1;
				}
				Gdiplus::Matrix mat;
				mat.RotateAt(azimuth, Gdiplus::PointF(posX + m2DWidth / 2, posY + m2DHeight / 2));
				grp.SetTransform(&mat);
				grp.DrawImage(&img, posX, posY, m2DWidth, m2DHeight);
			}
		}

		void CCompassManager::Set2DImagePath(std::string strPath)
		{
			if (_access(strPath.c_str(), 0) == 0)
			{
				m2DImagePass = strPath.c_str();
				mbHas2DImage = true;
			}
		}
	}
}