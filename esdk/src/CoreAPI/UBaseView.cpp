
#include "stdafx.h"
//////////////////////////////////////////////////////////////////////////
// System header
#include <list>
#include <map>
#include <string>
#include <vector>

//////////////////////////////////////////////////////////////////////////
// Poco header

#include <Poco/Timer.h>




//////////////////////////////////////////////////////////////////////////
// Ogre headers
#include <OgreCommon.h>
#include <OgreException.h>
#include <OgreConfigFile.h>
#include <OgreRoot.h>
#include <OgreCamera.h>
#include <OgreViewport.h>
#include <OgreSceneManager.h>
#include <OgreRenderWindow.h>
#include <OgreEntity.h>
#include <OgreSubEntity.h>
#include <OgreWindowEventUtilities.h>
#include <OgreLogManager.h>
#include <OgreRenderSystem.h>
#include <OgreResourceBackgroundQueue.h>
#include <OgreManualObject.h>
#include <OgreStaticGeometry.h>
#include <OgreMeshManager.h>

#include <OgreSubMesh.h>
#include <OgreRay.h>
#include <OgreMaterialManager.h>

#include <OgreOverlay.h>
#include <OgreAny.h>
#include <OgreHardwareOcclusionQuery.h>
#include <OgreInstanceManager.h>
#include <OgreInstancedEntity.h>
#include <OgreFont.h>
#include <OgreFontManager.h>
//////////////////////////////////////////////////////////////////////////
// Une Core Header
#include "UDB.h"
#include "UObject.h"
#include "UBaseOperator.h"
#include "UBaseView.h"
#include "UBaseDriver.h"

#include "UEntity.h"
#include "UMaterial.h"
#include "UAssimpLoader.h"
#include "UMouseOperator.h"
#include "UPOIManager.h"
#include "IconPOISet.h"

#include "UBaseModel.h"
#include "UScene.h"

#include "UMovableTextOverlayManager.h"

#include "DynamicLines.h"
#include "CompassManager.h"


DWORD WINAPI LoadResourceThread(PVOID pArg);
//////////////////////////////////////////////////////////////////////////

using namespace Ogre;
using namespace UnE::Core;


static std::string UMATERIAL_GENERAL_RESOURCE_GROUP		= Ogre::ResourceGroupManager::DEFAULT_RESOURCE_GROUP_NAME;
static std::string UMATERIAL_WIREFRAME_RESOURCE_GROUP	= "WireFrame";
static std::string UMATERIAL_HIDDEN_RESOURCE_GROUP		= "HiddenLine";
static std::string UMATERIAL_POINT_RESOURCE_GROUP		= "Point";
static std::string UMATERIAL_SELECT_RESOURCE_GROUP		= "Selected";



void __stdcall RenderTimer(HWND hWnd, UINT uMsg, UINT_PTR idEvent, DWORD dwTime)
{
	DWORD nCountPre = GetTickCount();

	UBaseDriver::Instance().RenderAllView();

	DWORD nCountPost = GetTickCount();
	DWORD nCurCnt = nCountPost - nCountPre;
	TRACE1("RENDER TIMER : %d\n", nCurCnt);
	if( nCurCnt <= 16)
		nCurCnt = 16;

	KillTimer(g_WndCtx->hWnd, 9999);
	//SetTimer(m_WndCtx->hWnd, 9999, 10, &RenderTimer);
	//gRenderTimer.setPeriodicInterval(nCurCnt);
	//gRenderTimer.restart();	
}

namespace UnE
{

	namespace Core
	{
		//////////////////////////////////////////////////////////////////////////
		// Globa Variable
		
		// Main DB
		UDB pDB;
		
		// Global Rendering Timer
		Poco::Timer gRenderTimer;

		// Global Entity Render Contex
		RenderableContext gEntityRenderContext; 
		

		typedef std::map<int, UnE::Core::UIconPOI*> UIconPOIList;
		extern UIconPOIList gIconPOIList;

		typedef std::map<int, UnE::Core::UTextPOI*> UTextPOIList;
		extern UTextPOIList gTextPOIList;

		typedef std::map<int, UnE::Core::UMovableTextOverlay*> UTextOverlayList;
		extern UTextOverlayList gMovableTextList;

		std::vector<Ogre::String> mMaterialList;

		static HKEY& GetRootKey()
		{
			return UBaseDriver::Instance().GetRegistry();
		};
		//////////////////////////////////////////////////////////////////////////
		// Local function declaration 
		static Vector3 GetInitPoistion()
		{
			Vector3 vecInitPos;
			vecInitPos.x = ReadProfileData(GetRootKey(), "InitPosition0", 0.0f);
			vecInitPos.y = ReadProfileData(GetRootKey(), "InitPosition1", 40.0f);
			vecInitPos.z = ReadProfileData(GetRootKey(), "InitPosition2", -300.0f);
			return vecInitPos;
		}

		static Vector3 GetInitLookAt()
		{
			Vector3 vecInitLookAt;
			vecInitLookAt.x = ReadProfileData(GetRootKey(), "InitLookAt0", 0.0f);
			vecInitLookAt.y = ReadProfileData(GetRootKey(), "InitLookAt1", 40.0f);
			vecInitLookAt.z = ReadProfileData(GetRootKey(), "InitLookAt2", 300.0f);
			return vecInitLookAt;
		}

		static float GetNearClipDistance()
		{
			return ReadProfileData(GetRootKey(), "NearClipDist", 5.0f);
		}

		static size_t GetNumMipmap()
		{
			return ReadProfileData(GetRootKey(), "NumMipmap", (DWORD)5);
		}

		static ColourValue GetBackgroundColor()
		{
			ColourValue cBack;
			//cBack.a = ReadProfileData(GetRootReg(), "BackcolorA", 1.0f);
			cBack.r = ReadProfileData(GetRootKey(), "BackcolorR", 255.0f);
			cBack.g = ReadProfileData(GetRootKey(), "BackcolorG", 255.0f);
			cBack.b = ReadProfileData(GetRootKey(), "BackcolorB", 255.0f);
			return cBack;
		}

		//////////////////////////////////////////////////////////////////////////
		// BeginRenderEvent Implementation			
		class BeginRenderEvent 
		{
		public:
			BeginRenderEvent() 
			{				
			}

			~BeginRenderEvent(){}

			void InvokeRender(Poco::Timer& timer)
			{
				UBaseDriver::Instance().RenderAllView();
			}
		};

		

		//////////////////////////////////////////////////////////////////////////
		// AnimationRenderer Implementation
		class AnimationRenderer : Ogre::FrameListener
		{
		public:
			virtual bool frameRenderingQueued(const Ogre::FrameEvent& evt)
			{
				
				UDB::GetUDB()->GetAnimationManager()->Animate(evt.timeSinceLastEvent);
				return true;

		
			}
		};


		//////////////////////////////////////////////////////////////////////////
		// Global Variable

		// Animation Frame Listener
		AnimationRenderer aniRenderer;
		// Timer Render Listener
		BeginRenderEvent beginRenderer;				
		// Poco Timer Callback function
		Poco::TimerCallback<BeginRenderEvent> renderCallback(beginRenderer, &BeginRenderEvent::InvokeRender);
		

		//////////////////////////////////////////////////////////////////////////
		// ViewOgreTechniqueSwitcher Implemenation
		class ViewOgreTechniqueSwitcher : public Ogre::RenderQueue::RenderableListener
		{
		public:
			const static int SOLIDMATCOUNT = 10;

			ViewOgreTechniqueSwitcher(const Ogre::ColourValue& bgColour) 
				:backColour(bgColour), active(false)
			{
				// get default materials
				wireSelectionMaterial = Ogre::MaterialManager::getSingleton().getByName("GesWireSel");
				if (wireSelectionMaterial.isNull())
				{
					wireSelectionMaterial = Ogre::MaterialManager::getSingleton().create("GesWireSel", 
						Ogre::ResourceGroupManager::DEFAULT_RESOURCE_GROUP_NAME);
					Ogre::Pass* p = wireSelectionMaterial->getTechnique(0)->getPass(0);
					p->setLightingEnabled(false);
					p->setPolygonMode(Ogre::PM_WIREFRAME);
					p->setCullingMode(Ogre::CULL_NONE);
					wireSelectionMaterial->load();
				}
				solidSelectionMaterial = Ogre::MaterialManager::getSingleton().getByName("GesFlatSel");
				if (solidSelectionMaterial.isNull())
				{
					solidSelectionMaterial = Ogre::MaterialManager::getSingleton().create("GesFlatSel", 
						Ogre::ResourceGroupManager::DEFAULT_RESOURCE_GROUP_NAME);
					Ogre::Pass* p = solidSelectionMaterial->getTechnique(0)->getPass(0);
					p->setLightingEnabled(false);
					// need a depth bias
					p->setDepthBias(2);
					// need to colour this red, and alpha blend
					Ogre::TextureUnitState* t = p->createTextureUnitState();
					t->setColourOperationEx(Ogre::LBX_SOURCE1, Ogre::LBS_MANUAL, Ogre::LBS_CURRENT,
						Ogre::ColourValue::Red);
					t->setAlphaOperation(Ogre::LBX_SOURCE1, Ogre::LBS_MANUAL, Ogre::LBS_CURRENT, 0.2f);
					p->setSceneBlending(Ogre::SBT_TRANSPARENT_ALPHA);
					p->setDepthWriteEnabled(false);

					p = solidSelectionMaterial->getTechnique(0)->createPass();
					//p->setPolygonMode(Ogre::PM_WIREFRAME);
					p->setPolygonMode(Ogre::PM_SOLID);
					p->setLightingEnabled(false);
					p->setDepthWriteEnabled(false);
					p->setDepthBias(10);
					solidSelectionMaterial->load();
				}
				for (int i = 0; i < SOLIDMATCOUNT; ++i)
				{
					Ogre::StringUtil::StrStreamType str;
					str << "GesSolidCol" << i;
					solidColourMaterials[i] = Ogre::MaterialManager::getSingleton().getByName(str.str());
					if (solidColourMaterials[i].isNull())
					{
						solidColourMaterials[i] = Ogre::MaterialManager::getSingleton().create(str.str(), 
							Ogre::ResourceGroupManager::DEFAULT_RESOURCE_GROUP_NAME);
						Ogre::Pass* p = solidColourMaterials[i]->getTechnique(0)->getPass(0);
						p->setAmbient(backColour * 0.5f);
						//p->setLightingEnabled(false);
						// need to colour this, use hue range
						// distribute equally across hue spectrum
						// Solid colours should be a bit less saturated
						Ogre::ColourValue col;
						col.setHSB((float)i/(float)(SOLIDMATCOUNT-1), 0.25, 1.0);

						Ogre::TextureUnitState* t = p->createTextureUnitState();
						t->setColourOperationEx(Ogre::LBX_MODULATE, Ogre::LBS_MANUAL, Ogre::LBS_CURRENT,
							col);
						solidColourMaterials[i]->load();
					}
					str.str(Ogre::StringUtil::BLANK);
					str << "GesWireCol" << i;
					wireColourMaterials[i] = Ogre::MaterialManager::getSingleton().getByName(str.str());
					if (wireColourMaterials[i].isNull())
					{
						wireColourMaterials[i] = Ogre::MaterialManager::getSingleton().create(str.str(), 
							Ogre::ResourceGroupManager::DEFAULT_RESOURCE_GROUP_NAME);
						Ogre::Pass* p = wireColourMaterials[i]->getTechnique(0)->getPass(0);
						p->setLightingEnabled(false);
						p->setPolygonMode(Ogre::PM_WIREFRAME);
						p->setCullingMode(Ogre::CULL_NONE);
						// need to colour this, use hue range
						// distribute equally across hue spectrum
						Ogre::ColourValue col;
						col.setHSB((float)i/(float)(SOLIDMATCOUNT-1), 0.5, 1.0);

						Ogre::TextureUnitState* t = p->createTextureUnitState();
						t->setColourOperationEx(Ogre::LBX_SOURCE1, Ogre::LBS_MANUAL, Ogre::LBS_CURRENT,
							col);
						wireColourMaterials[i]->load();
					}
					str.str(Ogre::StringUtil::BLANK);
					str << "GesHiddenLineCol" << i;
					hiddenLineColourMaterials[i] = Ogre::MaterialManager::getSingleton().getByName(str.str());
					if (hiddenLineColourMaterials[i].isNull())
					{
						hiddenLineColourMaterials[i] = Ogre::MaterialManager::getSingleton().create(str.str(), 
							Ogre::ResourceGroupManager::DEFAULT_RESOURCE_GROUP_NAME);
						Ogre::Pass* p = hiddenLineColourMaterials[i]->getTechnique(0)->getPass(0);
						// Hidden line passes should have an initial pass setting the
						// background colour, then a wire pass over the top
						p->setLightingEnabled(false);
						Ogre::TextureUnitState* t = p->createTextureUnitState();
						t->setColourOperationEx(Ogre::LBX_SOURCE1, Ogre::LBS_MANUAL, Ogre::LBS_CURRENT,
							backColour);


						p = hiddenLineColourMaterials[i]->getTechnique(0)->createPass();
						p->setLightingEnabled(false);
						p->setDepthBias(10);
						p->setPolygonMode(Ogre::PM_WIREFRAME);
						// need to colour this, use hue range
						// distribute equally across hue spectrum
						Ogre::ColourValue col;
						col.setHSB((float)i/(float)(SOLIDMATCOUNT-1), 0.5, 1.0);

						t = p->createTextureUnitState();
						t->setColourOperationEx(Ogre::LBX_SOURCE1, Ogre::LBS_MANUAL, Ogre::LBS_CURRENT,
							col);
						hiddenLineColourMaterials[i]->load();
					}
				}
			}
			~ViewOgreTechniqueSwitcher() 
			{
			}

			//---------------------------------------------------------------------
			bool renderableQueued(Ogre::Renderable* rend, Ogre::uint8 groupID, 
				Ogre::ushort priority, Ogre::Technique** ppTech, Ogre::RenderQueue* pQueue)
			{
				
				if((groupID < Ogre::RENDER_QUEUE_1) || (groupID > Ogre::RENDER_QUEUE_9))
					return true;

				// Use pointer-based any_cast to try to cast
				const RenderableContext* ctx = Ogre::any_cast<RenderableContext>(&rend->getUserAny());

				// Always render normally
				if (ctx && ctx->ignoreViewDetail)
					return true;
				
				// use renderable pointer to determine what colour for now
				// may want to use a categorisation in future
				Ogre::RenderOperation op;
				rend->getRenderOperation(op);
				int colourIndex = ((int)op.vertexData->vertexCount) % SOLIDMATCOUNT;

				switch(detail)
				{
				case UPolygonMode::ePM_TEXTURED:
					// use default technique
					// queue an extra renderable if selected
					if (ctx && ctx->selected)
					{
						// queue selection pass just after this group
						pQueue->getQueueGroup(groupID + 1)->addRenderable(rend, 
							solidSelectionMaterial->getSupportedTechnique(0), priority);

					}
					return true;
				case UPolygonMode::ePM_SHADED:
					// change to flat shading
					*ppTech = solidColourMaterials[colourIndex]->getSupportedTechnique(0);
					// queue an extra renderable if selected
					if (ctx && ctx->selected)
					{
						// queue selection pass just after this group
						pQueue->getQueueGroup(groupID + 1)->addRenderable(rend, 
							solidSelectionMaterial->getSupportedTechnique(0), priority);
					}
					return true;
				case UPolygonMode::ePM_WIREFRAME:
					// return a solid-coloured technique depending on selection
					if (ctx && ctx->selected)
					{
						*ppTech = wireSelectionMaterial->getSupportedTechnique(0);
					}
					else
					{
						*ppTech = wireColourMaterials[colourIndex]->getSupportedTechnique(0);
					}
					return true;
				case UPolygonMode::ePM_HIDDENLINE:
					// Change to hidden line removal
					*ppTech = hiddenLineColourMaterials[colourIndex]->getSupportedTechnique(0);
					// queue an extra renderable if selected
					if (ctx && ctx->selected)
					{
						// queue selection pass just after this group
						pQueue->getQueueGroup(groupID + 1)->addRenderable(rend, 
							solidSelectionMaterial->getSupportedTechnique(0), priority);
					}
					return true;
				};
				return true;
			}

			Ogre::MaterialPtr wireColourMaterials[SOLIDMATCOUNT];
			Ogre::MaterialPtr solidColourMaterials[SOLIDMATCOUNT];
			Ogre::MaterialPtr hiddenLineColourMaterials[SOLIDMATCOUNT];
			Ogre::MaterialPtr wireSelectionMaterial;
			Ogre::MaterialPtr solidSelectionMaterial;
			UPolygonMode detail;
			Ogre::ColourValue backColour;
			bool active;
		};	


		//////////////////////////////////////////////////////////////////////////
		// Material Technique Switcher		
		ViewOgreTechniqueSwitcher* mTechSwitcher = NULL;

		//////////////////////////////////////////////////////////////////////////
		UBaseView::UBaseView( HWND hWnd )
		{
			m_hWnd = hWnd;
			m_bInitDisplay = false;		

			m_bChangingRenderSystem = false;

			m_curMode = UPolygonMode::ePM_SHADED;

			m_fFontHeight = 18.0f;

			m_bRender = false;

			mpCompass = NULL;
		
			UBaseDriver::Instance().Add(this);

			m_pModel = new UBaseModel(hWnd);
			m_pModel->SetBaseView(this);

			m_fIconHeight = 32.0f;
			m_fIconWidth = 32.0f;

			m_rTextColor = 1.0f;
			m_gTextColor = 1.0f;
			m_bTextColor = 0.0f;

			m_bEnableGradient = true;
			m_rBackUpper = 1.0f;
			m_gBackUpper = 1.0f;
			m_bBackUpper = 1.0f;
			m_rBackBottom = 0.4f;
			m_gBackBottom = 0.8f;
			m_bBackBottom = 0.9f;	

			m_fTextLODDist = 400.0f;
			m_bLODText = true;
			
			m_bEnableFloorGradient = false;
			m_rFloorTop = 1.0f;
			m_gFloorTop = 1.0f;
			m_bFloorTop = 1.0f;
			m_rFloorBtm = 0.4f;
			m_gFloorBtm = 0.8f;
			m_bFloorBtm = 0.9f;
			
		}


		UBaseView::~UBaseView()
		{
			UBaseDriver::Instance().Remove(this);
			
			if( m_pModel != NULL)
				delete m_pModel;
		}


		bool UBaseView::IsInitWindow()
		{
			return m_bInitDisplay;
		}


		void UBaseView::OnChangeRenderer()
		{
			gRenderTimer.stop();

			m_bChangingRenderSystem = true;			

			if( m_Root != NULL)
			{
				CWnd * pWnd = CWnd::FromHandle(m_hWnd);
				if( pWnd != NULL)
				{
					CRect rect;
					pWnd->GetClientRect(rect);
					CreateRenderWindow(rect.Width() + 1, rect.Height() + 1,  "Main", "play");

					WndCtx * pCtx = GetWndContext(m_hWnd);
					
					for(unsigned int i = 0 ; i < mCameraList.size(); i++)
					{
						if( pCtx != NULL)
							mCameraList[i]->pInternal = pCtx->camera;
					}
				}
				ResetOperator();
			}
			m_bChangingRenderSystem = false;
		}
		
		
		bool UBaseView::CreateSubWindow(HWND hParent, int nWidth, int nHeight, std::string camName )
		{
			Ogre::NameValuePairList parms;
			parms["parentWindowHandle"] = Ogre::StringConverter::toString(HandleToLong(hParent));
			parms["externalWindowHandle"] = Ogre::StringConverter::toString(HandleToLong(m_hWnd));
			parms["FSAA"] = "8";

			// Create main Render Window
			RenderWindow * mWindow = m_Root->createRenderWindow("Sub", nWidth, nHeight, false, &parms);
			if( mWindow == NULL)
				return false;
			
			WndCtx * pWndCtx = GetWndContext(m_hWnd);
			if( pWndCtx == NULL)
			{
				pWndCtx = new WndCtx();
				AddWndContext(pWndCtx);
			}	
			pWndCtx->bSubWindow = true;
			pWndCtx->hWnd = m_hWnd;

			pWndCtx->renderWnd = mWindow;
			pWndCtx->renderWnd->setAutoUpdated(false);
			pWndCtx->renderWnd->setVSyncEnabled(true);

			// Create the SceneManager, in this case a generic one
			pWndCtx->sceneMgr = m_Root->createSceneManager(ST_GENERIC , "SubSceneManager");
	

			// Create scene query
			pWndCtx->ray = new Ogre::Ray();
			pWndCtx->raySceneQuery = pWndCtx->sceneMgr->createRayQuery(*(pWndCtx->ray));

			// Create the camera
			pWndCtx->camera = pWndCtx->sceneMgr->createCamera(camName);	

			// Position it at 500 in Z direction
			Ogre::Vector3 vecInit = GetInitPoistion();	
			pWndCtx->camera->setPosition(vecInit);
			pWndCtx->camera->setFixedYawAxis(true);

			// Look back along -Z
			Ogre::Vector3 vecLook = GetInitLookAt();	
			pWndCtx->camera->lookAt(vecLook);

			// Set clip distance
			Real nearClipDistance = GetNearClipDistance();
			pWndCtx->camera->setNearClipDistance(nearClipDistance);

			// Create one viewport, entire window
			Ogre::Viewport* vp = mWindow->addViewport(pWndCtx->camera);	
			vp->setOverlaysEnabled(false);

			// Set background color in viewport
			Ogre::ColourValue colorBack = GetBackgroundColor();
			vp->setBackgroundColour(colorBack);

			// Alter the camera aspect ratio to match the viewport
			pWndCtx->camera->setAspectRatio(Ogre::Real(vp->getActualWidth()) / Ogre::Real(vp->getActualHeight()));
			pWndCtx->viewport = vp;

			// Set default mipmap level (NB some APIs ignore this)
			int numDefaultMipmap  = GetNumMipmap();
			//Ogre::TextureManager::getSingleton().setDefaultNumMipmaps(numDefaultMipmap);

			// initialise all resource groups

			pWndCtx->rootNode = pWndCtx->sceneMgr->getRootSceneNode();
			pWndCtx->orientaion = new Quaternion(pWndCtx->rootNode->getOrientation());
					
			// Set ambient light
			pWndCtx->sceneMgr->setAmbientLight(Ogre::ColourValue(0.8f, 0.8f, 0.8f));

			//m_SubWndCtx->aabb.setExtents(Ogre::Vector3::ZERO, Ogre::Vector3::UNIT_X);
			pWndCtx->poiManger = new UPOIManager(m_hWnd, pWndCtx->sceneMgr, pWndCtx->camera);
			pWndCtx->objectManager = new UObjectManager();
			
			pWndCtx->mvTextManager = new UMovableTextOverlayManager(pWndCtx->sceneMgr, pWndCtx->camera, "AritaSB");

			// Create background
			CreateBackgroundPane();

			m_bInitDisplay = true;
			return true;
		}

		bool UBaseView::ReCreateRenderWindow( int nWidth, int nHeight, std::string title , std::string camName )
		{
			return false;
		}

		Ogre::HardwareOcclusionQuery* mHOQ = NULL;

		bool UBaseView::CreateRenderWindow(int nWidth, int nHeight, std::string title , std::string camName)
		{
			if( m_bInitDisplay == true)
				return true;			

			Ogre::NameValuePairList parms;
			parms["externalWindowHandle"] = Ogre::StringConverter::toString(HandleToLong(m_hWnd));
			parms["FSAA"] = "8";

			// Create main Render Window
			RenderWindow * mWindow = m_Root->createRenderWindow(title, nWidth, nHeight, false, &parms);
			if( mWindow == NULL)
				return false;
			
			WndCtx* pWndCtx = GetWndContext(m_hWnd);
			if( pWndCtx == NULL)
			{
				pWndCtx = new WndCtx();	
				AddWndContext(pWndCtx);
			}	

			pWndCtx->bSubWindow = false;
			pWndCtx->hWnd = m_hWnd;

			pWndCtx->renderWnd = mWindow;
			pWndCtx->renderWnd->setAutoUpdated(false);
			pWndCtx->renderWnd->setVSyncEnabled(true);

			// Create the SceneManager, in this case a generic one
			pWndCtx->sceneMgr = m_Root->createSceneManager(ST_GENERIC , "DefaultSceneManager");
			bool bShow = true;
			//m_WndCtx->sceneMgr->setOption("ShowOctree", &bShow );
			// Create scene query
			pWndCtx->ray = new Ogre::Ray();
			pWndCtx->raySceneQuery = pWndCtx->sceneMgr->createRayQuery(*(pWndCtx->ray));

			// Create the camera
			pWndCtx->camera = pWndCtx->sceneMgr->createCamera(camName);	

			// Position it at 500 in Z direction
			Ogre::Vector3 vecInit = GetInitPoistion();	
			pWndCtx->camera->setPosition(vecInit);
			pWndCtx->camera->setFixedYawAxis(true);

			// Look back along -Z
			Ogre::Vector3 vecLook = GetInitLookAt();	
			pWndCtx->camera->lookAt(vecLook);

			// Set clip distance
			Real nearClipDistance = GetNearClipDistance();
			pWndCtx->camera->setNearClipDistance(nearClipDistance);

			// Create one viewport, entire window
			Ogre::Viewport* vp = mWindow->addViewport(pWndCtx->camera);	
			
			//vp->setOverlaysEnabled(false);

			// Set background color in viewport
			Ogre::ColourValue colorBack = GetBackgroundColor();
			vp->setBackgroundColour(colorBack);

			// Alter the camera aspect ratio to match the viewport
			pWndCtx->camera->setAspectRatio(Ogre::Real(vp->getActualWidth()) / Ogre::Real(vp->getActualHeight()));
			pWndCtx->viewport = vp;

			// Set default mipmap level (NB some APIs ignore this)
			int numDefaultMipmap  = GetNumMipmap();
			Ogre::TextureManager::getSingleton().setDefaultNumMipmaps(numDefaultMipmap);

			// initialise all resource groups

			pWndCtx->rootNode = pWndCtx->sceneMgr->getRootSceneNode();
			pWndCtx->orientaion = new Quaternion(pWndCtx->rootNode->getOrientation());
			
			// add Animation frame listener 
			m_Root->addFrameListener((Ogre::FrameListener*)&aniRenderer);

			// Set ambient light
			pWndCtx->sceneMgr->setAmbientLight(Ogre::ColourValue(0.8f, 0.8f, 0.8f));

			// Create background
			CreateBackgroundPane();

			if( mTechSwitcher == NULL)
				mTechSwitcher = new ViewOgreTechniqueSwitcher(Ogre::ColourValue(0.5f, 0.5f, 0.5f));
			mTechSwitcher->detail = ePM_TEXTURED;
			m_curMode = ePM_TEXTURED;

			pWndCtx->poiManger = new UPOIManager(m_hWnd, pWndCtx->sceneMgr, pWndCtx->camera);
			pWndCtx->objectManager = new UObjectManager();
			
			pWndCtx->mvTextManager = new UMovableTextOverlayManager(pWndCtx->sceneMgr, pWndCtx->camera, "AritaSB");
			
			m_bInitDisplay = true;

			// Set Rendering Timer
			SetTimer(m_hWnd, 9999, 10,  &RenderTimer );
			return true;
		}

		bool UBaseView::DisposeWindow()
		{
			m_bInitDisplay = false;
			return true;
		}

		bool UBaseView::Dispose()
		{
			gRenderTimer.stop();
			if (m_Root != NULL)
			{
				if (mTechSwitcher != NULL)
				{
					delete mTechSwitcher;
					mTechSwitcher = NULL;
				}
				// int flag
				m_bInitDisplay = false;

				// !!!! SET NO RENDER !!!!
				m_bRender = true;
				// wait rendering thread
				Sleep(200);


				WndCtx * pWndCtx = GetWndContext(m_hWnd);

				// delete root init orientaion
				delete pWndCtx->orientaion;

				// delete ray & scene query
				delete pWndCtx->ray;
				OGRE_DELETE(pWndCtx->raySceneQuery);

				delete pWndCtx->poiManger;
				// delete all viewport
				pWndCtx->renderWnd->removeAllViewports();

				if (mpCompass != NULL)
				{
					((CCompassManager*)mpCompass)->Release();
					delete mpCompass;
					mpCompass = NULL;
				}

				// main wnd destory
				pWndCtx->renderWnd->destroy();
				// Clear Scene
				pWndCtx->sceneMgr->destroyAllEntities();
				pWndCtx->sceneMgr->destroyAllLights();
				pWndCtx->sceneMgr->destroyAllManualObjects();
				pWndCtx->sceneMgr->destroyAllMovableObjects();
				pWndCtx->sceneMgr->destroyAllAnimations();
				pWndCtx->sceneMgr->destroyAllAnimationStates();
				pWndCtx->sceneMgr->destroyAllEntities();
				pWndCtx->sceneMgr->getRootSceneNode()->removeAndDestroyAllChildren();
				pWndCtx->sceneMgr->clearScene();		
				// destory all camera
				pWndCtx->sceneMgr->destroyAllCameras();		
				// destory scene manager
				m_Root->destroySceneManager(pWndCtx->sceneMgr);

				UnE::Core::UObjectManager * pObjManager = pWndCtx->objectManager;
				delete pObjManager;

				UnE::Core::UMovableTextOverlayManager* pMvTextManager = pWndCtx->mvTextManager;
				delete pMvTextManager;
				

				m_bRender = false;
			}
			return true;
		}
		
		bool UBaseView::RenderScene()
		{
			if(m_bRender == false)
			{
				m_bRender = true;
				if( m_bInitDisplay == true &&  m_bChangingRenderSystem  == false)
				{					
					//if( m_Root != NULL)
					{
						//mHOQ = m_renderSystem->createHardwareOcclusionQuery();
						//mHOQ->beginOcclusionQuery(); 
						WndCtx * pCtx = GetWndContext(m_hWnd);
						m_Root->_fireFrameStarted();
						pCtx->camera->getSceneManager()->getRenderQueue()->setRenderableListener(mTechSwitcher);
						pCtx->renderWnd->update();

						m_Root->_updateAllRenderTargets();
						pCtx->camera->getSceneManager()->getRenderQueue()->setRenderableListener(0);

						m_Root->_fireFrameEnded();
					}
				}
				m_bRender = false;
			}	
			return true;
		}

		void UBaseView::ChangeDisplaySize( int nWidth, int nHeight )
		{	
			// guard min size
			if( nWidth > 0 && nHeight > 0)
			{
				WndCtx * pCtx = GetWndContext(m_hWnd);
				if( pCtx != NULL)
				{
					m_nScrHeight = nHeight;
					m_nScrWidth = nWidth;
					// set window resized
					pCtx->renderWnd->windowMovedOrResized();
					
					// set aspect ratio on viewport
					pCtx->camera->setAspectRatio((Ogre::Real)nWidth / (Ogre::Real)nHeight);	
					
				}	
			}	
		}

		HRESULT UBaseView::WindowProc(UINT message, WPARAM wParam, LPARAM lParam)
		{
			return TRUE;
		}


		DWORD WINAPI LoadResourceThread(PVOID pArg)
		{
			UBaseView * pFrm = (UBaseView*)pArg;
			pFrm->LoadDefultResource();	
			return 0;
		}

		void UBaseView::LoadDefultResource()
		{
			UMaterialManager::Instance().LoadDefultResource();
		}

		bool UBaseView::RefreshWindow( )
		{
			return true;
		}

		bool UBaseView::RenderOneFrame( )
		{
			if(m_bRender == false)
			{
				m_bRender = true;
				if( m_bInitDisplay == true &&  m_bChangingRenderSystem  == false)
				{

					if( m_Root != NULL)
					{
						WndCtx * pCtx = GetWndContext(m_hWnd);
						if( pCtx == NULL)
						{
							m_bRender = false;
							return false;
						}
						m_Root->_fireFrameStarted();
						
						//mPlane->setVisible(true);
						pCtx->camera->getSceneManager()->getRenderQueue()->setRenderableListener(mTechSwitcher);
						pCtx->renderWnd->update();

						m_Root->_updateAllRenderTargets();

						pCtx->camera->getSceneManager()->getRenderQueue()->setRenderableListener(0);
						m_Root->_fireFrameEnded();

						//m_Root->renderOneFrame();
					}	
				}
				m_bRender = false;
			}

			return true;
		}
		
		void UBaseView::AddOperator( UBaseOperator* pOperator )
		{
			if( pOperator != NULL)
			{
				std::list<UBaseOperator* >::iterator iter;
				iter = std::find( m_OperatorList.begin(), m_OperatorList.end(), pOperator);
				if( iter == m_OperatorList.end())
				{
					pOperator->SetTargetView(this);
					m_OperatorList.push_back(pOperator);
				}
				
			}
		}
		void UBaseView::ResetOperator()
		{
			std::list<UBaseOperator* >::iterator iter;
			for( iter = m_OperatorList.begin(); iter!= m_OperatorList.end(); iter++)
			{
				(*iter)->Reset();
			}
		}
		void UBaseView::RemoveOperator( UBaseOperator* pOperator )
		{
			if( pOperator != NULL)
			{
				std::list<UBaseOperator* >::iterator iter;
				iter = std::find( m_OperatorList.begin(), m_OperatorList.end(), pOperator);
				if( iter != m_OperatorList.end())
				{
					pOperator->SetTargetView(NULL);
					m_OperatorList.erase(iter);
				}
			}
		}

		void UBaseView::MoveCameraRelative( UnE::Math::Vector3& vCamMoveRel )
		{
			WndCtx * pCtx = GetWndContext(m_hWnd);
			Vector3 vecMove( vCamMoveRel.x, vCamMoveRel.y, vCamMoveRel.z);
			pCtx->camera->moveRelative(vecMove);
		}

		float UBaseView::GetCameraPitch()
		{
			WndCtx * pCtx = GetWndContext(m_hWnd);
			Vector3 camdir = pCtx->camera->getDirection();
			return -(Ogre::Math::ATan2(Ogre::Math::Abs(camdir.x) + Ogre::Math::Abs(camdir.z), camdir.y).valueRadians() - Ogre::Math::HALF_PI);	
		}

		UnE::Math::Vector3 UBaseView::GetCameraPosition()
		{
			WndCtx * pCtx = GetWndContext(m_hWnd);
			Vector3 campos = pCtx->camera->getRealPosition();
			return UnE::Math::Vector3(campos.x, campos.y, campos.z);
		}

		UnE::Math::Quaternion UBaseView::GetCameraOrientaion()
		{
			WndCtx * pCtx = GetWndContext(m_hWnd);
			Quaternion qOri = pCtx->camera->getRealOrientation();
			return UnE::Math::Quaternion(qOri.w, qOri.x, qOri.y, qOri.z);
		}

		UnE::Math::Vector3 UBaseView::GetCameraDirection()
		{
			WndCtx * pCtx = GetWndContext(m_hWnd);
			Vector3 camdir = pCtx->camera->getRealDirection();
			return UnE::Math::Vector3(camdir.x, camdir.y, camdir.z);
		}

		void UBaseView::SetCameraDirection(UnE::Math::Vector3& vCamDir)
		{
			WndCtx * pCtx = GetWndContext(m_hWnd);
			Vector3 campos(vCamDir.x, vCamDir.y, vCamDir.z);
			pCtx->camera->setDirection(campos);
		}

		void UBaseView::SetCameraPitch(const float fPitch )
		{
			WndCtx * pCtx = GetWndContext(m_hWnd);
			pCtx->camera->pitch(Radian(fPitch));
		}

		void UBaseView::SetCameraYaw(const float fYaw )
		{
			WndCtx * pCtx = GetWndContext(m_hWnd);
			pCtx->camera->yaw(Radian(fYaw));
		}

		void UBaseView::SetCameraPosition( UnE::Math::Vector3& vCamPos )
		{
			WndCtx * pCtx = GetWndContext(m_hWnd);
			Vector3 campos(vCamPos.x, vCamPos.y, vCamPos.z);
			pCtx->camera->setPosition(campos);
		}

		void UBaseView::SetCameraOrientation(UnE::Math::Quaternion& vCamOrient )
		{
			WndCtx * pCtx = GetWndContext(m_hWnd);
			Quaternion qOrient(vCamOrient.w, vCamOrient.x, vCamOrient.y, vCamOrient.z );
			pCtx->camera->setOrientation(qOrient);
		}


		UnE::Math::Vector3 UBaseView::GetCameraRight()
		{
			WndCtx * pCtx = GetWndContext(m_hWnd);
			Vector3 camRight = pCtx->camera->getRight();
			return UnE::Math::Vector3(camRight.x, camRight.y, camRight.z);
		}


		UnE::Core::Camera* UBaseView::CreateCamera()
		{
			if( m_hWnd == NULL)
				return NULL;

			char buf[513];
			sprintf_s(buf, "Camera_%d", (int)m_hWnd);
			std::string szCamName = std::string(buf);
			UnE::Core::Camera * pObj = new UnE::Core::Camera(szCamName);

			WndCtx * pCtx = GetWndContext(m_hWnd);
			pObj->pInternal = (pCtx->camera);
			mCameraList.push_back(pObj);
			return pObj;

		}

		int UBaseView::GetViewportHeight()
		{
			UnE::Core::WndCtx * pCtx = GetWndContext(m_hWnd);
			if( pCtx->hWnd == m_hWnd )
			{
				return pCtx->viewport->getActualHeight();
			}
			return 0;
		}

		int UBaseView::GetViewportWidth()
		{
			UnE::Core::WndCtx * pCtx = GetWndContext(m_hWnd);
			if( pCtx->hWnd == m_hWnd )
			{
				return pCtx->viewport->getActualWidth();
			}
			return 0;
		}


		void UBaseView::CreateCircle()
		{	
			
		}


		void UBaseView::CreateBackgroundPane()
		{
			WndCtx * pCTX = GetWndContext(m_hWnd);
			Ogre::String szName = "BackgroundCild";
			// CHECK NODE
			try
			{
				Node * snode = pCTX->sceneMgr->getSceneNode(szName);
				if( snode != NULL)
					return;
			}
			catch(Exception)
			{
			}

			//////////////////////////////////////////////////////////
			// Set Background Material
			Ogre::String lNameOfMaterial = szName + "_Material";
			Ogre::String lResourceGroup = Ogre::ResourceGroupManager::DEFAULT_RESOURCE_GROUP_NAME;
			if(!MaterialManager::getSingleton().resourceExists(lNameOfMaterial))
			{
				Ogre::MaterialPtr myPathMaterial = Ogre::MaterialManager::getSingleton().create(lNameOfMaterial,lResourceGroup); 
				// SET NO SHADOW
				myPathMaterial->setReceiveShadows(false); 
				// USE COLOR MODE
				myPathMaterial->getTechnique(0)->setLightingEnabled(false); 
				// DEPTH CHECK DISABLE
				myPathMaterial->getTechnique(0)->getPass(0)->setDepthCheckEnabled(false);
				myPathMaterial->getTechnique(0)->getPass(0)->setDepthWriteEnabled(false);
			}

			// Create a manual object for 2D	
			Ogre::SceneNode * pNode = pCTX->sceneMgr->getRootSceneNode()->createChildSceneNode(szName);	
			pNode->setInheritOrientation(false);
			pNode->setInheritScale(false);


			// Create Manual Object
			ManualObject* manual = pCTX->sceneMgr->createManualObject("BackgroundRect");



			manual->setUseIdentityProjection(true);
			manual->setUseIdentityView(true);
			manual->begin(lNameOfMaterial, RenderOperation::OT_TRIANGLE_LIST);
			{
				manual->position(-1.0f, -1.0f, 0.0);
				
				manual->colour(m_rBackBottom, m_gBackBottom, m_bBackBottom); // bottom 1

				manual->position( 1.0f, -1.0f, 0.0);
				manual->colour(m_rBackBottom, m_gBackBottom, m_bBackBottom); // bottom 2

				manual->position( 1.0f,  1.0f, 0.0);

				if( m_bEnableGradient == false)
				{
					manual->colour(m_rBackBottom, m_gBackBottom, m_bBackBottom); // top2
				}
				else
				{
					manual->colour(m_rBackUpper, m_gBackUpper, m_bBackUpper); // top2
				}				

				manual->position(-1.0f,  1.0f, 0.0);
				if( m_bEnableGradient == false)
				{
					manual->colour(m_rBackBottom, m_gBackBottom, m_bBackBottom); // top1
				}
				else
				{
					manual->colour(m_rBackUpper, m_gBackUpper, m_bBackUpper); // top1
				}

				manual->triangle(2, 3, 0);
				manual->triangle(2, 0, 1);
			}
			manual->end();
			// Use infinite AAB to always stay visible
			AxisAlignedBox aabInf;
			aabInf.setInfinite();
			manual->setBoundingBox(aabInf);
			// Render just before overlays
			manual->setRenderQueueGroup(RENDER_QUEUE_BACKGROUND);

			gEntityRenderContext.selected = false;
			gEntityRenderContext.ignoreViewDetail = true;
			manual->setUserAny(Ogre::Any(gEntityRenderContext));

			// Attach to scene
			pNode->attachObject(manual); 
		}
		
		void UBaseView::ChangeViewMode( UPolygonMode mode )
		{
			m_curMode = mode;
			if( mTechSwitcher != NULL)
				mTechSwitcher->detail = mode;	
		}

		void UBaseView::ShowOctree( BOOL bShow )
		{
			WndCtx * pCTX = GetWndContext(m_hWnd);
			if( pCTX != NULL)
			{
				if( pCTX->sceneMgr )
				{
					bool show = bShow == TRUE ? true : false;
					pCTX->sceneMgr->setOption("ShowOctree", &show);
				}
			}
		}

		void UBaseView::SetFitView()
		{
			WndCtx * pCTX = GetWndContext(m_hWnd);
			Ogre::AxisAlignedBox aab = pCTX->aabb;
			if(!aab.isNull())
			{

				Ogre::Camera* tCamera = pCTX->camera;				
				float asp = tCamera->getAspectRatio();

				float width = aab.getMaximum().x - aab.getMinimum().x;
				float height = aab.getMaximum().y - aab.getMinimum().y;				
				float depth = aab.getMaximum().z - aab.getMinimum().z;

				float len = (width/height > asp) ? width/asp : height;
				len += len * 0.1f;
				len *= 0.5f;

				Ogre::Radian fov = tCamera->getFOVy() * 0.5f;

				len = len / Ogre::Math::Tan(fov);
				len *= 0.5f;

				Ogre::Vector3 center = aab.getCenter();
				
				center.z -= (len + (aab.getMaximum().z - aab.getMinimum().z) * 0.5f);
				Ogre::Vector3 newDir = aab.getCenter() - center;
				float flength = newDir.length();
				center.y = flength;
				tCamera->setPosition(center);
				tCamera->setDirection(newDir);

				tCamera->pitch(-Ogre::Radian(Ogre::Math::HALF_PI / 2.0f));
				
				MouseOperator * pOp = GetMouseOperator();
				pOp->SetOrbitCenter(UnE::Math::Vector3(aab.getCenter().x, aab.getCenter().y, aab.getCenter().z));
				pOp->Orbit(-30.0f, 0.0f);
			}
		}

		void UBaseView::SetFrontView()
		{		
			WndCtx * pCTX = GetWndContext(m_hWnd);
			Ogre::AxisAlignedBox aab = pCTX->aabb;
			if(!aab.isNull())
			{
				
				Ogre::Camera* tCamera = pCTX->camera;				
				float asp = tCamera->getAspectRatio();
				
				float width = aab.getMaximum().x - aab.getMinimum().x;
				float height = aab.getMaximum().y - aab.getMinimum().y;				
				float depth = aab.getMaximum().z - aab.getMinimum().z;

				float len = (width/height > asp) ? width/asp : height;
				//len += len * 0.1f;
				len *= 0.5f;

				Ogre::Radian fov = tCamera->getFOVy() * 0.5f;

				len = len / Ogre::Math::Tan(fov);
				len *= 0.1f;

				Ogre::Vector3 center = aab.getCenter();
					
				center.z -= (len + (aab.getMaximum().z - aab.getMinimum().z) * 0.5f);
				Ogre::Vector3 newDir = Vector3::UNIT_Z;

				tCamera->setPosition(center);
				tCamera->setDirection(newDir);
			}
		}

		void UBaseView::SetTopView()
		{
			WndCtx * pCTX = GetWndContext(m_hWnd);
			Ogre::AxisAlignedBox aab = pCTX->aabb;
			if(!aab.isNull())
			{
				Ogre::Camera* tCamera = pCTX->camera;
				float asp = tCamera->getAspectRatio();

				float width = aab.getMaximum().x - aab.getMinimum().x;
				float height = aab.getMaximum().z - aab.getMinimum().z;
				
				float len = (width/height > asp) ? width/asp : height;
				len += len * 0.1f;
				len *= 0.5f;
				
				Ogre::Radian fov = tCamera->getFOVy() * 0.5f;
				len = len / Ogre::Math::Tan(fov);
				
				Ogre::Vector3 center = aab.getCenter();
				center.y += (len + (aab.getMaximum().y - aab.getMinimum().y) * 0.5f);
				
				Ogre::Vector3 newDir = Ogre::Vector3::UNIT_Z;

				tCamera->setPosition(center);
				tCamera->setDirection(newDir);
				tCamera->pitch(-Ogre::Radian(Ogre::Math::HALF_PI));
			}
		}

		void UBaseView::SetLeftView()
		{
			WndCtx * pCTX = GetWndContext(m_hWnd);
			Ogre::AxisAlignedBox aab = pCTX->aabb;
			if(!aab.isNull())
			{			
				Ogre::Camera* tCamera = pCTX->camera;
				float asp = tCamera->getAspectRatio();
				float width = aab.getMaximum().z - aab.getMinimum().z;
				float height = aab.getMaximum().y - aab.getMinimum().y;
				float len = (width/height > asp) ? width/asp : height;
				//len += len * 0.1f;
				len *= 0.5f;
				Ogre::Radian fov = tCamera->getFOVy() * 0.5f;
				
				len *= 0.5f;
				len = len / Ogre::Math::Tan(fov);
				Ogre::Vector3 center = aab.getCenter();
				center.x -= len + (aab.getMaximum().x - aab.getMinimum().x) * 0.5f;
				Ogre::Vector3 newDir = Ogre::Vector3::UNIT_X;

				tCamera->setPosition(center);
				tCamera->setDirection(newDir);
			}
		}

		void UBaseView::SetRightView()
		{
			WndCtx * pCTX = GetWndContext(m_hWnd);
			Ogre::AxisAlignedBox aab = pCTX->aabb;
			if(!aab.isNull())
			{				
				Ogre::Camera* tCamera = pCTX->camera;
				float asp = tCamera->getAspectRatio();
				float width = aab.getMaximum().z - aab.getMinimum().z;
				float height = aab.getMaximum().y - aab.getMinimum().y;
				float len = (width/height > asp) ? width/asp : height;
				//len += len * 0.1f;
				len *= 0.5f;
				Ogre::Radian fov = tCamera->getFOVy() * 0.5f;
				
				len = len / Ogre::Math::Tan(fov);
				
				len *= 0.5f;
				Ogre::Vector3 center = aab.getCenter();
				center.x += len + (aab.getMaximum().x - aab.getMinimum().x) * 0.5f;
				Ogre::Vector3 newDir = -Ogre::Vector3::UNIT_X;

				tCamera->setPosition(center);
				tCamera->setDirection(newDir);
			}
		}
		void UBaseView::SetFixView()
		{
			WndCtx * pCTX = GetWndContext(m_hWnd);
			Ogre::AxisAlignedBox aab = pCTX->aabb;
			if(!aab.isNull())
			{
				Ogre::Camera* tCamera = pCTX->camera;
				//tCamera->setPosition(Ogre::Vector3(-11.4f, 390.0f,-850.0f));

				//POSITION : -25.78327,395.6898,-898.6788
				tCamera->setPosition(Ogre::Vector3(-25.78327f, 421.6898f,-885.5657f));
				tCamera->setDirection(Ogre::Vector3(0.0f, -0.5f, 0.846f ));
			}
		}

		void UBaseView::SetHomeView( float zoomFactor /*= 0.5f*/ )
		{				
			WndCtx * pCTX = GetWndContext(m_hWnd);
			Ogre::AxisAlignedBox aab = pCTX->aabb;
			if(!aab.isNull())
			{
				Ogre::Camera* tCamera = pCTX->camera;				
				float asp = tCamera->getAspectRatio();

				float width = aab.getMaximum().x - aab.getMinimum().x;
				float height = aab.getMaximum().y - aab.getMinimum().y;				
				float depth = aab.getMaximum().z - aab.getMinimum().z;

				float len = (width/height > asp) ? width/asp : height;
				//len += len * 0.1f;
				len *= 0.5f;

				Ogre::Radian fov = tCamera->getFOVy() * 0.5f;

				len = len / Ogre::Math::Tan(fov);
				len *= zoomFactor;

				Ogre::Vector3 center = aab.getCenter();

				center.z -= (len + (aab.getMaximum().z - aab.getMinimum().z) * 0.5f);
				Ogre::Vector3 newDir = Vector3::UNIT_Z;

				center.y *= 2.5f;
				center.x += width*0.14f;
				tCamera->setPosition(center);
				tCamera->setDirection(newDir);

				MouseOperator * pOp = GetMouseOperator();
				pOp->SetOrbitCenter(UnE::Math::Vector3(center.x, center.y, center.z));
				pOp->Orbit(0.0f, -30.0f);
			}			
		}

		void UBaseView::SetRearView()
		{
			WndCtx * pCTX = GetWndContext(m_hWnd);
			Ogre::AxisAlignedBox aab = pCTX->aabb;
			if(!aab.isNull())
			{				
				Ogre::Camera* tCamera = pCTX->camera;				
				float asp = tCamera->getAspectRatio();

				float width = aab.getMaximum().x - aab.getMinimum().x;
				float height = aab.getMaximum().y - aab.getMinimum().y;				
				float depth = aab.getMaximum().z - aab.getMinimum().z;

				float len = (width/height > asp) ? width/asp : height;
				//len += len * 0.1f;
				len *= 0.5f;

				Ogre::Radian fov = tCamera->getFOVy() * 0.5f;

				len = len / Ogre::Math::Tan(fov);
				len *= 0.1f;

				Ogre::Vector3 center = aab.getCenter();

				center.z += (len + (aab.getMaximum().z - aab.getMinimum().z) * 0.5f);
				Ogre::Vector3 newDir = -Vector3::UNIT_Z;

				tCamera->setPosition(center);
				tCamera->setDirection(newDir);
			}
		}
		//////////////////////////////////////////////////////////////////////////
		// ICON POI		
		int UBaseView::AddIconPOI( std::string szIconPath, float x, float y, float z, bool bVisible )
		{
			WndCtx * pCtx = GetWndContext(m_hWnd);
			if( pCtx != NULL)
			{
				Ogre::Vector3 pos = Ogre::Vector3(x, y, z);
				UIconPOI * pIcon = pCtx->poiManger->AddIconPOI(pCtx->sceneMgr, pCtx->camera, szIconPath,pos, m_fIconWidth, m_fIconHeight, 0);
				pIcon->SetVisible(bVisible);
				int nCookie = UDB::GetNextCookie();
				pIcon->SetID(nCookie);
				gIconPOIList.insert(std::make_pair(nCookie, pIcon));
				return nCookie;
			}
			return 0;
		}
		int UBaseView::AddIconPOI( std::string szIconPath, float x, float y, float z, float fwidth, float fHeight, bool bVisible /*= true*/ )
		{
			WndCtx * pCtx = GetWndContext(m_hWnd);
			if( pCtx != NULL)
			{
				Ogre::Vector3 pos = Ogre::Vector3(x, y, z);
				UIconPOI * pIcon = pCtx->poiManger->AddIconPOI(pCtx->sceneMgr, pCtx->camera, szIconPath,pos, fwidth, fHeight, 0);
				pIcon->SetVisible(bVisible);
				int nCookie = UDB::GetNextCookie();
				pIcon->SetID(nCookie);
				gIconPOIList.insert(std::make_pair(nCookie, pIcon));
				return nCookie;
			}
			return 0;
		}

		//////////////////////////////////////////////////////////////////////////
		void UBaseView::UpdateIcon( int hIcon, std::string szIconPath )
		{
			WndCtx * pCtx = GetWndContext(m_hWnd);
			if( pCtx != NULL)
			{
				if( gIconPOIList.size() > 0)
				{
					UIconPOIList::iterator iter = gIconPOIList.find(hIcon);
					if(iter  == gIconPOIList.end() )
					{
						return;
					}					
					UnE::Core::UIconPOI * pIcon = iter->second;
					gIconPOIList.erase(iter);

					Ogre::Vector3 vPos = pIcon->Get3DPosition();
					bool bVisible = pIcon->GetVisible();
					
					pCtx->poiManger->RemoveIconPOI(pIcon);

					pIcon = pCtx->poiManger->AddIconPOI(pCtx->sceneMgr, pCtx->camera, szIconPath,vPos, m_fIconWidth, m_fIconHeight, 0);
					pIcon->SetVisible(bVisible);
					pIcon->SetID(hIcon);
					gIconPOIList.insert(std::make_pair(hIcon, pIcon));						
				}
			}
		}
		//////////////////////////////////////////////////////////////////////////
		UnE::Core::MouseOperator* UBaseView::GetMouseOperator()
		{
			std::list<UBaseOperator* >::iterator iter;
			UBaseOperator* pOperator = NULL;
			if(m_OperatorList.size() == 0)
				return NULL;

			for( iter= m_OperatorList.begin(); iter!= m_OperatorList.end(); iter++)
			{
				pOperator = *iter;
				if(pOperator->GetType() == 1)
				{
					UnE::Core::MouseOperator * pMouseOp = (MouseOperator*)pOperator;
					if( pMouseOp->GetType() == UOpType::eOp_Mouse)
					{
						break;
					}
				}					
				pOperator = NULL;
			}

			if( pOperator == NULL)
				return NULL;

			UnE::Core::MouseOperator * pMouseOp = (MouseOperator*)pOperator;
			return pMouseOp;
		}

		int UBaseView::AddIconPOI( std::string szIconPath )
		{
			WndCtx * pCtx = GetWndContext(m_hWnd);
			if( pCtx != NULL)
			{
				UnE::Core::MouseOperator * pMouseOp = GetMouseOperator();
				UnE::Math::Vector3 vPos = pMouseOp->GetLastPoistion();				
				Ogre::Vector3 pos = Ogre::Vector3(vPos.x, vPos.y, vPos.z);
				UIconPOI * pIcon = pCtx->poiManger->AddIconPOI(pCtx->sceneMgr, pCtx->camera, szIconPath,pos, m_fIconWidth, m_fIconHeight, 0);
				if( pIcon != NULL)
				{
					pIcon->SetVisible(true);
					int nCookie = UDB::GetNextCookie();
					pIcon->SetID(nCookie);
					gIconPOIList.insert(std::make_pair(nCookie, pIcon));
					return nCookie;
				}			    
			}
			return -1;
		}

		
		void UBaseView::RemovePOI(float x, float y, float z)
		{
			WndCtx * pCtx = GetWndContext(m_hWnd);
			if( pCtx != NULL)
			{
				if( gIconPOIList.size() > 0)
				{
					Ogre::Vector3 hPos = Ogre::Vector3(x, y, z);
					UIconPOIList::iterator iter = gIconPOIList.begin();
					for( ; iter != gIconPOIList.end(); iter++)
					{
						Ogre::Vector3 vpos = iter->second->Get3DPosition();
						if( vpos.positionCloses(hPos, 0.01f) == true)
						{
							UnE::Core::UIconPOI * pIcon = iter->second;
							iter = gIconPOIList.erase(iter);
							pCtx->poiManger->RemoveIconPOI(pIcon);
							return;
						}
					}					
				}				
			}
		}

		void UBaseView::RemovePOI(int nIconID )
		{
			WndCtx * pCtx = GetWndContext(m_hWnd);
			if( pCtx != NULL)
			{
				if( gIconPOIList.size() > 0)
				{
					UIconPOIList::iterator iter = gIconPOIList.find(nIconID);
					if(iter  == gIconPOIList.end() )
					{
						return;
					}					
					UnE::Core::UIconPOI * pIcon = iter->second;
					iter = gIconPOIList.erase(iter);
					pCtx->poiManger->RemoveIconPOI(pIcon);
				}				
			}
		}

		void UBaseView::RemovePOI()
		{
			WndCtx * pCtx = GetWndContext(m_hWnd);
			if( pCtx != NULL)
			{
				UnE::Core::MouseOperator * pMouseOp = GetMouseOperator();
				float x = (float)(pMouseOp->mSavePt.x);
				float y = (float)(pMouseOp->mSavePt.y);
				UIconPOIList::iterator it;
				for(it = gIconPOIList.begin(); it != gIconPOIList.end(); it++)
				{
					UnE::Core::UIconPOI * pIcon = it->second;
					std::pair<bool, float> rpair = pIcon->Pick(x, y);
					if(rpair.first == true)
					{						
						RemovePOI(pIcon->GetID());
						break;
					}
				}
			}
		}

		//////////////////////////////////////////////////////////////////////////
		// TEXT POI
		int UBaseView::AddTextPOI(std::string szText )
		{
			WndCtx * pCtx = GetWndContext(m_hWnd);
			if( pCtx != NULL)
			{
				Ogre::String szFont = "AritaSB";
				UnE::Core::MouseOperator * pMouseOp = GetMouseOperator();
				UnE::Math::Vector3 vPos = pMouseOp->GetLastPoistion();				
				Ogre::Vector3 m3DPosition = Ogre::Vector3(vPos.x, vPos.y, vPos.z);
				Ogre::String text = szText.c_str();	

				float mFontHeight = m_fFontHeight;
				//Ogre::ColourValue textColor = Ogre::ColourValue(1.0f, 0.0f, 0.0f);				
				Ogre::ColourValue textColor = Ogre::ColourValue(m_rTextColor, m_gTextColor, m_bTextColor);	
				UTextPOI* pTextPOI = pCtx->poiManger->AddTextPOI(szFont, m3DPosition, mFontHeight, textColor, text, mFontHeight);
				if( pTextPOI != NULL)
				{
					pTextPOI->ToggleLODDist(m_bLODText);
					pTextPOI->SetLODDist(m_fTextLODDist);
					pTextPOI->SetColor(textColor);
					pTextPOI->SetCharHeight(mFontHeight);
					pTextPOI->SetTextAlignment(UTextPOI::H_CENTER, UTextPOI::V_ABOVE);
					pTextPOI->_UpdateGeometry();
					pTextPOI->SetVisible(true);
					int nCookie = UDB::GetNextCookie();
					pTextPOI->SetID(nCookie);
					gTextPOIList.insert(std::make_pair(nCookie, pTextPOI));
					return nCookie;
				}				
			}
			return -1;
		}

		int UBaseView::AddTextPOI2(std::string szText, float x, float y, float z, bool bVisible /*= true*/)
		{
			WndCtx * pCtx = GetWndContext(m_hWnd);
			if (pCtx != NULL)
			{
				Ogre::String szFont = "AritaSB";
				Ogre::String text = szText.c_str();
				Ogre::Vector3 m3DPosition = Ogre::Vector3(x, y, z);

				float mFontHeight = m_fFontHeight;

				Ogre::ColourValue textColor = Ogre::ColourValue(m_rTextColor, m_gTextColor, m_bTextColor);

				UMovableTextOverlay * pTextPOI = pCtx->mvTextManager->AddTextPOI(m3DPosition, mFontHeight, textColor, text, mFontHeight);
				if (pTextPOI != NULL)
				{
					pTextPOI->enable(bVisible);					
					int nCookie = pTextPOI->GetID();
					gMovableTextList.insert(std::make_pair(nCookie, pTextPOI));
					return nCookie;
				}
			}
			return -1;
		}
		
		bool UBaseView::MoveTextPOI2(int hText, float x, float y, float z)
		{
			WndCtx * pCtx = GetWndContext(m_hWnd);
			if (pCtx != NULL)
			{
				if (gMovableTextList.size() > 0)
				{
					UTextOverlayList::iterator iter = gMovableTextList.find(hText);
					if (iter == gMovableTextList.end())
					{
						return false;
					}
					UnE::Core::UMovableTextOverlay * pIcon = iter->second;
					pIcon->SetPoistion(x, y, z);
					return true;
				}
			}
			return false;
		}

		void UBaseView::RemoveTextPOI2(int hText)
		{
			WndCtx * pCtx = GetWndContext(m_hWnd);
			if (pCtx != NULL)
			{
				if (gMovableTextList.size() > 0)
				{
					UTextOverlayList::iterator iter = gMovableTextList.find(hText);
					if (iter == gMovableTextList.end())
					{
						return;
					}

					UnE::Core::UMovableTextOverlay * pIcon = iter->second;
					pIcon->enable(false);
					pCtx->mvTextManager->RemoveTextPOI(pIcon);					
					//gMovableTextList.erase(iter);					

				}
			}
		}

		void UBaseView::ShowTextPOI2(int hText, bool bShow)
		{
			WndCtx * pCtx = GetWndContext(m_hWnd);
			if (pCtx != NULL)
			{
				if (gMovableTextList.size() > 0)
				{
					UTextOverlayList::iterator iter = gMovableTextList.find(hText);
					if (iter == gMovableTextList.end())
					{
						return;
					}
					UnE::Core::UMovableTextOverlay * pText = iter->second;
					pText->enable(bShow);
				}
			}
		}

		int UBaseView::AddTextPOI( std::string szText, float x, float y, float z, bool bVisible /*= true*/ )
		{
			WndCtx * pCtx = GetWndContext(m_hWnd);
			if( pCtx != NULL)
			{
				Ogre::String szFont = "AritaSB";
				Ogre::String text = szText.c_str();
				Ogre::Vector3 m3DPosition = Ogre::Vector3(x, y, z);

				float mFontHeight = m_fFontHeight;
	
				Ogre::ColourValue textColor = Ogre::ColourValue(m_rTextColor, m_gTextColor, m_bTextColor);	
				
				UTextPOI* pTextPOI = pCtx->poiManger->AddTextPOI(szFont, m3DPosition, mFontHeight, textColor, text, mFontHeight);
				if( pTextPOI != NULL)
				{
					pTextPOI->ToggleLODDist(m_bLODText);
					pTextPOI->SetLODDist(m_fTextLODDist);
					pTextPOI->SetColor(textColor);
					pTextPOI->SetCharHeight(mFontHeight);
					pTextPOI->SetTextAlignment(UTextPOI::H_CENTER, UTextPOI::V_ABOVE);
					pTextPOI->_UpdateGeometry();
					pTextPOI->SetVisible(bVisible);
					int nCookie = UDB::GetNextCookie();
					pTextPOI->SetID(nCookie);
					gTextPOIList.insert(std::make_pair(nCookie, pTextPOI));
					return nCookie;
				}				
			}
			return -1;
		}

		void UBaseView::RemoveTextPOI( int nID )
		{
			WndCtx * pCtx = GetWndContext(m_hWnd);
			if( pCtx != NULL)
			{
				if( gTextPOIList.size() > 0)
				{
					UTextPOIList::iterator iter = gTextPOIList.find(nID);
					if(iter  == gTextPOIList.end() )
					{
						return;
					}					
					UnE::Core::UTextPOI * pText = iter->second;
					iter = gTextPOIList.erase(iter);
					pCtx->poiManger->RemoveTextPOI(pText);
				}				
			}
		}

		int UBaseView::ShowZoneName( float x, float y, float z, std::string szAlias )
		{
			WndCtx * pCtx = GetWndContext(m_hWnd);
			if( pCtx != NULL )
			{				
				Ogre::String szFont = "AritaSB";
				Ogre::String text = szAlias.c_str();
				Ogre::Vector3 m3DPosition = Ogre::Vector3( x,  y,  z );

				float mFontHeight = m_fFontHeight;
				Ogre::ColourValue textColor = Ogre::ColourValue(m_rTextColor, m_gTextColor, m_bTextColor);				

				UTextPOI* pTextPOI = pCtx->poiManger->AddTextPOI(szFont, m3DPosition, mFontHeight, textColor, text, mFontHeight);
				if( pTextPOI != NULL)
				{
					pTextPOI->ToggleLODDist(m_bLODText);
					pTextPOI->SetLODDist(m_fTextLODDist);
					pTextPOI->SetColor(textColor);
					pTextPOI->SetCharHeight(mFontHeight);
					pTextPOI->SetTextAlignment(UTextPOI::H_CENTER, UTextPOI::V_ABOVE);
					pTextPOI->_UpdateGeometry();
					pTextPOI->SetVisible(true);
					int nCookie = UDB::GetNextCookie();
					pTextPOI->SetID(nCookie);
					gTextPOIList.insert(std::make_pair(nCookie, pTextPOI));
					return nCookie;
				}					
			}	
			return -1;
		}

		int UBaseView::ShowObjectName(UObject* pObj, std::string szAlias )
		{
			WndCtx * pCtx = GetWndContext(m_hWnd);
			if( pCtx != NULL && pObj != NULL)
			{
				std::string szName = pObj->GetName();
				Ogre::SceneNode* pNode = pCtx->sceneMgr->getSceneNode(szName.c_str());				
				if( pNode != NULL)
				{
					Ogre::AxisAlignedBox box = pNode->_getWorldAABB();

					Ogre::Vector3 vpos = box.getMaximum();
					Ogre::Vector3 v2 = box.getMinimum();
					Ogre::Vector3 v3 = (vpos + v2 ) * 0.5f;

					return AddTextPOI(szAlias.c_str(), v3.x, vpos.y, v3.z );					
				}
			}
			return -1;
		}		

		void UBaseView::ClearViewData()
		{
			WndCtx * pCtx = GetWndContext(m_hWnd);
			if( pCtx != NULL)
			{	
				if (mpCompass != NULL)
				{
					((CCompassManager*)mpCompass)->Release();
				}
			

				if( pCtx->poiManger != NULL)
				{
					delete pCtx->poiManger;
				}
				

				pCtx->poiManger = new UPOIManager(m_hWnd, pCtx->sceneMgr, pCtx->camera);

				pCtx->sceneMgr->destroyAllEntities();
				pCtx->sceneMgr->destroyAllLights();
				pCtx->sceneMgr->destroyAllManualObjects();
				pCtx->sceneMgr->destroyAllAnimationStates();
				pCtx->sceneMgr->destroyAllStaticGeometry();
				pCtx->sceneMgr->getRootSceneNode()->removeAndDestroyAllChildren();
				
				Ogre::MeshManager::getSingleton().unloadUnreferencedResources(false);
				Ogre::MaterialManager::getSingleton().unloadUnreferencedResources(false);
								
				pCtx->objectManager->ClearAll();

				pCtx->mvTextManager->ClearTextPOI();

				pCtx->aabb = Ogre::AxisAlignedBox();

				CreateBackgroundPane();
				if (mpCompass != NULL)
				{
					((CCompassManager*)mpCompass)->Restore();
				}				
			}

			
		}
		
		void UBaseView::DrawTempLine(std::vector< UnE::Math::Vector3 >& vecPoints)
		{
			WndCtx * pCtx = GetWndContext(m_hWnd);
			if( pCtx != NULL)
			{
				Ogre::SceneNode *lnode = NULL;				
				try
				{
					lnode = dynamic_cast<Ogre::SceneNode*>(pCtx->sceneMgr->getRootSceneNode()->getChild("lines"));

				}				
				catch (CException* e)
				{
					lnode = NULL;
				}

				if( lnode == NULL)
				{
					DynamicLines *lines = new DynamicLines(RenderOperation::OT_LINE_LIST);
					for (unsigned int i=0; i<vecPoints.size(); i++)
					{
						lines->addPoint(vecPoints[i].x, vecPoints[i].y, vecPoints[i].z);
					}
					lines->update();
					Ogre::SceneNode *linesNode = pCtx->sceneMgr->getRootSceneNode()->createChildSceneNode("lines");
					linesNode->attachObject(lines);
				}
				else
				{
					DynamicLines *lines = dynamic_cast<DynamicLines*>(lnode->getAttachedObject(0));				
					if (lines->getNumPoints()!= vecPoints.size())
					{					
						lines->clear();
						for (unsigned int i=0; i<vecPoints.size(); ++i)
						{
							lines->addPoint(vecPoints[i].x, vecPoints[i].y, vecPoints[i].z);
						}
					}
					else
					{
						// Just values have changed, use 'setPoint' instead of 'addPoint'
						for (unsigned int i=0; i<vecPoints.size(); ++i) 
						{
							lines->setPoint(i,Ogre::Vector3(vecPoints[i].x, vecPoints[i].y, vecPoints[i].z));
						}
					}
					lines->update();
				}
			}
		}	

		void UBaseView::StopRendering()
		{
			m_bRender = true;
		}

		void UBaseView::ResumeRendering()
		{
			m_bRender = false;
		}

		void UBaseView::ShowIconPOI( int hIcon, bool bShow )
		{
			WndCtx * pCtx = GetWndContext(m_hWnd);
			if( pCtx != NULL)
			{
				if( gIconPOIList.size() > 0)
				{
					UIconPOIList::iterator iter = gIconPOIList.find(hIcon);
					if(iter  == gIconPOIList.end() )
					{
						return;
					}					
					UnE::Core::UIconPOI * pIcon = iter->second;
					pIcon->SetVisible(bShow);
				}				
			}
		}		

		void UBaseView::ShowTextPOI( int hText, bool bShow )
		{
			WndCtx * pCtx = GetWndContext(m_hWnd);
			if( pCtx != NULL)
			{
				if( gTextPOIList.size() > 0)
				{
					UTextPOIList::iterator iter = gTextPOIList.find(hText);
					if(iter == gTextPOIList.end() )
					{
						return;
					}					
					UnE::Core::UTextPOI * pText = iter->second;
					pText->SetVisible(bShow);
				}				
			}
		}

		

		bool UBaseView::SaveScreenShot( std::string path )
		{
			WndCtx * pCtx = GetWndContext(m_hWnd);
			if( pCtx != NULL)
			{
				Ogre::Viewport* pViewport = pCtx->camera->getViewport();
				if (pViewport == 0)
					return false;

				pViewport->getTarget()->update();
				pViewport->getTarget()->writeContentsToFile(path.c_str());
				return true;
			}
			return false;
		}

		void UBaseView::SelectIconPOI( int hIcon , bool bSelect)
		{
			WndCtx * pCtx = GetWndContext(m_hWnd);
			if( pCtx != NULL)
			{
				if( gIconPOIList.size() > 0)
				{
					UIconPOIList::iterator iter = gIconPOIList.find(hIcon);
					if(iter  == gIconPOIList.end() )
					{
						return;
					}					
					UnE::Core::UIconPOI * pIcon = iter->second;
					if( bSelect == true)
					{						
						pIcon->SetHilightMode(1);
					}
					else
					{
						if( pIcon->IsEnabled() == true)
						{
							pIcon->SetHilightMode(0);
						}
						else
						{
							pIcon->SetHilightMode(3);
						}					
					}
				}
			}		
		}

		void UBaseView::ClearSelectedPIO()
		{
			WndCtx * pCtx = GetWndContext(m_hWnd);
			if( pCtx != NULL)
			{
				UIconPOIList::iterator it;
				for(it = gIconPOIList.begin(); it != gIconPOIList.end(); it++)
				{
					UnE::Core::UIconPOI * pIcon = it->second;
					if( pIcon != NULL)
					{
						//if( pIcon->GetHilightMode() == 1)
						if( pIcon->IsEnabled() == true)
						{
							pIcon->SetHilightMode(0);
						}
						else
						{
							pIcon->SetHilightMode(3);
						}
					}													
				}
			}
		}

		void UBaseView::SetPickSize( int hIcon, int width, int height )
		{
			WndCtx * pCtx = GetWndContext(m_hWnd);
			if( pCtx != NULL)
			{
				if( gIconPOIList.size() > 0)
				{
					UIconPOIList::iterator iter = gIconPOIList.find(hIcon);
					if(iter  == gIconPOIList.end() )
					{
						return;
					}					
					UnE::Core::UIconPOI * pIcon = iter->second;
					pIcon->SetPickSize(width, height);					
				}
			}	
		}

		bool UBaseView::IsIconPOISelected( int hIcon )
		{
			WndCtx * pCtx = GetWndContext(m_hWnd);
			if( pCtx != NULL)
			{
				if( gIconPOIList.size() > 0)
				{
					UIconPOIList::iterator iter = gIconPOIList.find(hIcon);
					if(iter  == gIconPOIList.end() )
					{
						return false;
					}					
					UnE::Core::UIconPOI * pIcon = iter->second;
					if( pIcon->GetHilightMode() != 0)
						return true;
					
				}
			}	
			return false;
		}

		bool UBaseView::MoveIconPOI( int hIcon, float x, float y, float z )
		{
			WndCtx * pCtx = GetWndContext(m_hWnd);
			if( pCtx != NULL)
			{
				if( gIconPOIList.size() > 0)
				{
					UIconPOIList::iterator iter = gIconPOIList.find(hIcon);
					if(iter  == gIconPOIList.end() )
					{
						return false;
					}					
					UnE::Core::UIconPOI * pIcon = iter->second;
					pIcon->Set3DPosition(Ogre::Vector3(x, y, z));
					return true;
				}
			}	
			return false;
		}

		bool UBaseView::MoveTextPOI( int hText, float x, float y, float z )
		{
			WndCtx * pCtx = GetWndContext(m_hWnd);
			if( pCtx != NULL)
			{
				if( gIconPOIList.size() > 0)
				{
					UTextPOIList::iterator iter = gTextPOIList.find(hText);
					if(iter  == gTextPOIList.end() )
					{
						return false;
					}					
					UnE::Core::UTextPOI * pIcon = iter->second;
					pIcon->Set3DPosition(Ogre::Vector3(x, y, z));
					return true;
				}
			}	
			return false;
		}

		void UBaseView::EnablePOI( int hIcon, bool bEnable )
		{
			WndCtx * pCtx = GetWndContext(m_hWnd);
			if( pCtx != NULL)
			{
				if( gIconPOIList.size() > 0)
				{
					UIconPOIList::iterator iter = gIconPOIList.find(hIcon);
					if(iter  == gIconPOIList.end() )
					{
						return;
					}		
					UnE::Core::UIconPOI * pIcon = iter->second;
					if( bEnable == false)
					{
						if( pIcon->GetHilightMode() != 3)
						{
							pIcon->SetDisableColor(Ogre::ColourValue(0.8f, 0.8f, 0.8f, 0.8f));
							pIcon->SetHilightMode(3);
							pIcon->Enabled(false);
						}						
					}		
					else
					{
						if( pIcon->GetHilightMode() == 3)						
							pIcon->SetHilightMode(0);
						pIcon->Enabled(true);
					}
				}
			}			
		}

		float clamp(float fValue, float under, float upper)
		{
			if( under < 0.0f)
			{
				fValue = 0.0f;
			}
			if( upper > 1.0f)
			{
				fValue = 1.0f;
			}
			return fValue;
		}

		void UBaseView::SetTextPOIColor( float fred, float fgreen, float fbule )
		{
			m_rTextColor = clamp(fred, 0.0f, 1.0f);
			m_gTextColor = clamp(fgreen, 0.0f, 1.0f);
			m_bTextColor = clamp(fbule, 0.0f, 1.0f);
		}

		void UBaseView::SetIconPOISize( float nWidth, float nHeight )
		{
			m_fIconHeight = nHeight;
			m_fIconWidth = nWidth;
		}

		float UBaseView::GetPOIDistance( int hIcon )
		{
			WndCtx * pCtx = GetWndContext(m_hWnd);
			if( pCtx != NULL)
			{
				if( gIconPOIList.size() > 0)
				{
					UIconPOIList::iterator iter = gIconPOIList.find(hIcon);
					if(iter  == gIconPOIList.end() )
					{
						return -1.0f;
					}					
					UnE::Core::UIconPOI * pIcon = iter->second;
					Ogre::Vector3 p1 = pIcon->Get3DPosition();
					Ogre::Vector3 p2 = pCtx->camera->getPosition();
					float f = p1.distance(p2);
					return f;
				}
			}
			return -1.0f;
		}

		void UBaseView::EnableGraient( bool bEnabled )
		{
			m_bEnableGradient = bEnabled;
		}

		void UBaseView::SetBackUpperColor( float r, float g, float b )
		{
			m_rBackUpper = r;
			m_gBackUpper = g;
			m_bBackUpper = b;
		}

		void UBaseView::SetBackBottomColor( float r, float g, float b )
		{
			m_rBackBottom = r;
			m_gBackBottom = g;
			m_bBackBottom = b;
		}


		std::string UBaseView::CloneSceneNode( std::string& srcName, std::string& parentName, float tx, float ty, float tz, bool bVisible )
		{
			WndCtx * pCtx = GetWndContext(m_hWnd);
			if( pCtx != NULL)
			{				
				Ogre::SceneManager *  mSceneMgr = (Ogre::SceneManager*)(pCtx->sceneMgr);
				Ogre::SceneNode * sNode = (Ogre::SceneNode * )mSceneMgr->getRootSceneNode()->getChild(srcName);
				Ogre::SceneNode * parentNode = (Ogre::SceneNode * )mSceneMgr->getRootSceneNode()->getChild(parentName);

				if( sNode != NULL && parentNode != NULL)
				{
					int nCount = 0;
					int nCookie = UDB::GetNextCookie();
					

					Ogre::AxisAlignedBox aabox = sNode->_getWorldAABB();
					Ogre::Vector3 vCenter = sNode->_getWorldAABB().getCenter();
					
					Ogre::SceneNode* newNode = parentNode->createChildSceneNode(srcName + Ogre::StringConverter::toString(nCookie));			

					if(newNode != NULL)
					{
						Ogre::SceneNode::ObjectIterator iter = sNode->getAttachedObjectIterator();
						while (iter.hasMoreElements())
						{
							Ogre::Entity* temp = ((Ogre::Entity*)(iter.getNext()));
							
							Ogre::Entity* newE = temp->clone(temp->getName()+Ogre::StringConverter::toString(nCookie)+ "_" + Ogre::StringConverter::toString(nCount));
							newNode->attachObject(newE);
							nCount++;
						}
						
						newNode->_setDerivedPosition(sNode->_getDerivedPosition());
						//newNode->showBoundingBox(true);
						newNode->_updateBounds();
						//Ogre::AxisAlignedBox aabox = sNode->_getWorldAABB();
						//Ogre::Vector3 vCenter = sNode->_getWorldAABB().getCenter();
						//newNode->_setDerivedPosition(vCenter);
						
						
						newNode->translate(tx, ty, tz);
						
						
						newNode->setVisible(bVisible);


						parentNode->_updateBounds();


						UnE::Core::UBaseModel* pModel = UnE::Core::UDB::GetBaseModel((int)m_hWnd);
						UnE::Core::USceneNodeManager * pUSeneMan = pModel->GetSecneManager();

						std::string szName = std::string(newNode->getName());
						UnE::Core::USceneNode * pUParent = pUSeneMan->FindSceneNode(parentName);
						UnE::Core::USceneNode * pUNode = pUParent->CreateChild(szName);
						pUNode->SetAliasName(newNode->getName());
						pUNode->SetTag(newNode);					

						return std::string(newNode->getName());
					}
				}				
			}			
			return std::string("");
		}


		std::string UBaseView::CloneSceneNode( std::string& srcName, float tx, float ty, float tz, bool bVisible )
		{
			WndCtx * pCtx = GetWndContext(m_hWnd);
			if( pCtx != NULL)
			{				
				Ogre::SceneManager *  mSceneMgr = (Ogre::SceneManager*)(pCtx->sceneMgr);
				//mSceneMgr->showBoundingBoxes(true);
				Ogre::SceneNode * sNode = (Ogre::SceneNode * )mSceneMgr->getRootSceneNode()->getChild(srcName);
				if( sNode != NULL)
				{
					int nCount = 0;
					int nCookie = UDB::GetNextCookie();

					Ogre::AxisAlignedBox aabox = sNode->_getWorldAABB();
					Ogre::Vector3 vCenter = sNode->_getWorldAABB().getCenter();
					Ogre::SceneNode* newNode = mSceneMgr->getRootSceneNode()->createChildSceneNode(srcName + Ogre::StringConverter::toString(nCookie));

					if(newNode != NULL)
					{
						
						

						Ogre::SceneNode::ObjectIterator iter = sNode->getAttachedObjectIterator();
						while (iter.hasMoreElements())
						{
							Ogre::Entity* temp = ((Ogre::Entity*)(iter.getNext()));
							
							Ogre::Entity* newE = temp->clone(temp->getName()+Ogre::StringConverter::toString(nCookie)+ "_" + Ogre::StringConverter::toString(nCount));
							newNode->attachObject(newE);
							nCount++;
						}
						
						//newNode->showBoundingBox(true);
						newNode->_updateBounds();

						

						newNode->translate(tx, ty, tz);
						newNode->setVisible(bVisible);

						UnE::Core::UBaseModel* pModel = UnE::Core::UDB::GetBaseModel((int)m_hWnd);
						UnE::Core::USceneNodeManager * pUSeneMan = pModel->GetSecneManager();

						std::string szName = std::string(newNode->getName());;
						UnE::Core::USceneNode * pUNode = pUSeneMan->GetRootSceneNode()->CreateChild(szName);
						pUNode->SetAliasName(newNode->getName());
						pUNode->SetTag(newNode);

						return std::string(newNode->getName());
					}
				}				
			}			
			return std::string("");
		}


		void UBaseView::CreateScenePane(std::string szName, int type, float x, float y, float z, bool bVisible )
		{
			WndCtx * pCTX = GetWndContext(m_hWnd);
			// CHECK NODE
			try
			{
				Node * snode = pCTX->sceneMgr->getSceneNode(szName);
				if( snode != NULL)
					return;
			}
			catch(Exception)
			{
			}

			//////////////////////////////////////////////////////////
			// Set Background Material
			Ogre::String lNameOfMaterial = szName + "_Material";
			Ogre::String lResourceGroup ="Popular";
			if(!MaterialManager::getSingleton().resourceExists(lNameOfMaterial))
			{
				Ogre::MaterialPtr myPathMaterial = Ogre::MaterialManager::getSingleton().create(lNameOfMaterial,lResourceGroup); 
				// SET NO SHADOW
				myPathMaterial->setReceiveShadows(false); 
				myPathMaterial->getTechnique(0)->setLightingEnabled(true);  
				myPathMaterial->getTechnique(0)->getPass(0)->setDiffuse(0.8f,0.8f, 0.8f, 1.0f);
				myPathMaterial->getTechnique(0)->getPass(0)->setAmbient(Ogre::ColourValue(0.8f,0.8f,0.8f, 0.1f));
				myPathMaterial->getTechnique(0)->getPass(0)->setCullingMode(CULL_NONE);
				myPathMaterial->getTechnique(0)->getPass(0)->setDepthWriteEnabled(false);
				
				
				//myPathMaterial->getTechnique(0)->getPass(0)->setSceneBlending(SBF_SOURCE_ALPHA, SBF_DEST_ALPHA);
				myPathMaterial->getTechnique(0)->getPass(0)->setSceneBlending(SBT_TRANSPARENT_ALPHA);
				Ogre::TextureUnitState * pUnit = myPathMaterial->getTechnique(0)->getPass(0)->createTextureUnitState("3d03.png");
				pUnit->setNumMipmaps(0);
			}

			// Create a manual object for 2D	
			Ogre::SceneNode * pNode = pCTX->sceneMgr->getRootSceneNode()->createChildSceneNode(szName);	

			// Create Manual Object
			Ogre::String lNameOfManual= szName + "_Manual";
			ManualObject* manual = pCTX->sceneMgr->createManualObject(lNameOfManual);

			manual->begin(lNameOfMaterial, RenderOperation::OT_TRIANGLE_LIST);
			{
				manual->position(-0.5f, 0.0f, -0.5f);
				manual->textureCoord(0.0f, 1.0f); // bottom 1				

				manual->position( 0.5f, 0.0 , -0.5f);
				manual->textureCoord(1.0f, 1.0f); // bottom 2

				manual->position( 0.5f, 0.0 ,  0.5f);
				manual->textureCoord(1.0f, 0.0f); // top 2	

				manual->position(-0.5f,  0.0 , 0.5f);
				manual->textureCoord(0.0f, 0.0f); // top 1	

				manual->triangle(0, 3, 2);
				manual->triangle(0, 2, 1);
			}
			manual->end();

			gEntityRenderContext.selected = false;
			gEntityRenderContext.ignoreViewDetail = true;
			manual->setUserAny(Ogre::Any(gEntityRenderContext));

			Ogre::String lNameOfMesh = szName + "_Mesh";
			Ogre::MeshPtr pMesh = manual->convertToMesh(lNameOfMesh);
			Ogre::Entity* lEntity = pCTX->sceneMgr->createEntity(lNameOfMesh, pMesh);
			// Attach to scene
			pNode->attachObject(lEntity);
			pNode->setVisible(bVisible);
			pNode->setPosition(x, y, z);

			UnE::Core::UBaseModel* pModel = UnE::Core::UDB::GetBaseModel((int)m_hWnd);
			UnE::Core::USceneNodeManager * pUSeneMan = pModel->GetSecneManager();

			UnE::Core::USceneNode * pUNode = pUSeneMan->GetRootSceneNode()->CreateChild(szName);
			pUNode->SetAliasName(pNode->getName());
			pUNode->SetTag(pNode);

		}

		void UBaseView::CreateSceneNode( std::string szName, float nWidth, float nLength, float nHeight, float tx, float ty, float tz, bool bVisible )

		//void UBaseView::CreateSceneNode( std::string szName, int type, float x, float y, float z, bool bVisible )
		{
			WndCtx * pCTX = GetWndContext(m_hWnd);
			// CHECK NODE
			try
			{
				Node * snode = pCTX->sceneMgr->getSceneNode(szName);
				if( snode != NULL)
					return;
			}
			catch(Exception)
			{
			}

			//////////////////////////////////////////////////////////
			// Set Background Material
			Ogre::String lNameOfMaterial = szName + "_Material";
			Ogre::String lResourceGroup ="Popular";
			if(!MaterialManager::getSingleton().resourceExists(lNameOfMaterial))
			{
				Ogre::MaterialPtr myPathMaterial = Ogre::MaterialManager::getSingleton().create(lNameOfMaterial,lResourceGroup); 
				// SET NO SHADOW
				myPathMaterial->setReceiveShadows(false); 
				myPathMaterial->getTechnique(0)->setLightingEnabled(true);  
				myPathMaterial->getTechnique(0)->getPass(0)->setDiffuse(0.8f,0.8f, 0.8f, 1.0f);
				myPathMaterial->getTechnique(0)->getPass(0)->setAmbient(Ogre::ColourValue(0.8f,0.8f,0.8f, 0.1f));
				myPathMaterial->getTechnique(0)->getPass(0)->setCullingMode(CULL_NONE);
				//myPathMaterial->getTechnique(0)->getPass(0)->setDepthWriteEnabled(false);


				//myPathMaterial->getTechnique(0)->getPass(0)->setSceneBlending(SBF_SOURCE_ALPHA, SBF_DEST_ALPHA);
				//myPathMaterial->getTechnique(0)->getPass(0)->setSceneBlending(SBT_TRANSPARENT_ALPHA);
				Ogre::TextureUnitState * pUnit = myPathMaterial->getTechnique(0)->getPass(0)->createTextureUnitState("3d03.png");
				pUnit->setNumMipmaps(0);
			}

			// Create a manual object for 2D	
			Ogre::SceneNode * pNode = pCTX->sceneMgr->getRootSceneNode()->createChildSceneNode(szName);	

			// Create Manual Object
			Ogre::String lNameOfManual= szName + "_Manual";
			ManualObject* cube = pCTX->sceneMgr->createManualObject(lNameOfManual);

			float x = nWidth / 2.0f;
			float y = nHeight;
			float z = nLength / 2.0f;
			cube->begin(lNameOfMaterial, RenderOperation::OT_TRIANGLE_LIST);
			{
				////////////////////////////////////////////////
				cube->position(x, y, -z);cube->normal(0.408248,-0.816497,0.408248);cube->textureCoord(1,0);
				cube->position(-x,0.0, -z);cube->normal(-0.408248,-0.816497,-0.408248);cube->textureCoord(0,1);
				cube->position(x,0.0, -z);cube->normal(0.666667,-0.333333,-0.666667);cube->textureCoord(1,1);
				cube->position(-x,y, -z);cube->normal(-0.666667,-0.333333,0.666667);cube->textureCoord(0,0);
				cube->position(x,y, z);cube->normal(0.666667,0.333333,0.666667);cube->textureCoord(1,0);
				cube->position(-x,y,-z);cube->normal(-0.666667,-0.333333,0.666667);cube->textureCoord(0,1);
				cube->position(x,y,-z);cube->normal(0.408248,-0.816497,0.408248);cube->textureCoord(1,1);
				cube->position(-x,y,z);cube->normal(-0.408248,0.816497,0.408248);cube->textureCoord(0,0);
				cube->position(-x,0.0, z);cube->normal(-0.666667,0.333333,-0.666667);cube->textureCoord(0,1);
				cube->position(-x,0.0,-z);cube->normal(-0.408248,-0.816497,-0.408248);cube->textureCoord(1,1);
				cube->position(-x,y, -z);cube->normal(-0.666667,-0.333333,0.666667);cube->textureCoord(1,0);
				cube->position(x,0.0, -z);cube->normal(0.666667,-0.333333,-0.666667);cube->textureCoord(0,1);
				cube->position(x,0.0, z);cube->normal(0.408248,0.816497,-0.408248);cube->textureCoord(1,1);
				cube->position(x,y ,-z);cube->normal(0.408248,-0.816497,0.408248);cube->textureCoord(0,0);
				cube->position(x,0.0,-z);cube->normal(0.666667,-0.333333,-0.666667);cube->textureCoord(1,0);
				cube->position(-x,0.0,-z);cube->normal(-0.408248,-0.816497,-0.408248);cube->textureCoord(0,0);
				cube->position(-x,y, z);cube->normal(-0.408248,0.816497,0.408248);cube->textureCoord(1,0);
				cube->position(x,0.0, z);cube->normal(0.408248,0.816497,-0.408248);cube->textureCoord(0,1);
				cube->position(-x,0.0,z);cube->normal(-0.666667,0.333333,-0.666667);cube->textureCoord(1,1);
				cube->position(x,y, z);cube->normal(0.666667,0.333333,0.666667);cube->textureCoord(0,0);

				cube->triangle(0,1,2);      cube->triangle(3,1,0);
				cube->triangle(4,5,6);      cube->triangle(4,7,5);
				cube->triangle(8,9,10);      cube->triangle(10,7,8);
				cube->triangle(4,11,12);   cube->triangle(4,13,11);
				cube->triangle(14,8,12);   cube->triangle(14,15,8);
				cube->triangle(16,17,18);   cube->triangle(16,19,17);				

			}
			cube->end();

			gEntityRenderContext.selected = false;
			gEntityRenderContext.ignoreViewDetail = true;
			

			Ogre::String lNameOfMesh = szName + "_Mesh";
			Ogre::MeshPtr pMesh = cube->convertToMesh(lNameOfMesh);
			Ogre::Entity* lEntity = pCTX->sceneMgr->createEntity(lNameOfMesh, pMesh);
			lEntity->setUserAny(Ogre::Any(gEntityRenderContext));
			// Attach to scene
			pNode->attachObject(lEntity);
			pNode->setVisible(bVisible);
			pNode->setPosition(tx, ty, tz);

			UnE::Core::UBaseModel* pModel = UnE::Core::UDB::GetBaseModel((int)m_hWnd);
			UnE::Core::USceneNodeManager * pUSeneMan = pModel->GetSecneManager();

			UnE::Core::USceneNode * pUNode = pUSeneMan->GetRootSceneNode()->CreateChild(szName);
			pUNode->SetAliasName(pNode->getName());
			pUNode->SetTag(pNode);

		}

		void UBaseView::RemoveSceneNode(std::string szSceneName)
		{
			WndCtx * pWndCtx = GetWndContext(m_hWnd);
			if( pWndCtx != NULL)
			{
				if( szSceneName == "")
					szSceneName = std::string("TestCube");

				// make unique name
				Ogre::String lNameOfMaterial = szSceneName + "_Material";
				Ogre::String lNameOfMesh = szSceneName + "_Mesh";
				try
				{
					Ogre::Node * node = pWndCtx->sceneMgr->getRootSceneNode()->getChild(szSceneName);
					if( node != NULL)
					{
						// delete entity
						pWndCtx->sceneMgr->destroyEntity(lNameOfMesh);
						// delete & remove all child
						pWndCtx->sceneMgr->getRootSceneNode()->removeAndDestroyChild(szSceneName);
					}
				}
				catch(Ogre::ItemIdentityException e)
				{
				}		
			}

			
		}

		void UBaseView::SetTextPOILOD(int nID, bool bToogle, float dist)
		{
			WndCtx * pCtx = GetWndContext(m_hWnd);
			if (pCtx != NULL)
			{
				if (gTextPOIList.size() > 0)
				{
					UTextPOIList::iterator iter = gTextPOIList.find(nID);
					if (iter == gTextPOIList.end())
					{
						return;
					}
					UnE::Core::UTextPOI * pText = iter->second;
					pText->ToggleLODDist(bToogle);
					pText->SetLODDist(dist);
				}
			}
		}

		void UBaseView::CreateCompass(float fAzumith)
		{
			WndCtx * pWndCtx = GetWndContext(m_hWnd);
			if (pWndCtx != NULL)
			{
				if (mpCompass == NULL)
				{
					CCompassManager* pCompass = new CCompassManager(m_hWnd);
					mpCompass = (void*)pCompass;
					((CCompassManager*)mpCompass)->SetVisible(false);
				}

				if (mpCompass != NULL)
				{					
					((CCompassManager*)mpCompass)->SetAzimuth(fAzumith + 180);
					((CCompassManager*)mpCompass)->SetVisible(true);
				}
			}
		}

		float xys[] = {
			750854.75, 726518.75,
			759865.872881356, 726518.75,
			768876.995762712, 726518.75,
			777888.118644068, 726518.75,
			786899.241525424, 726518.75,
			795910.36440678, 726518.75,
			804921.487288136, 726518.75,
			813932.610169492, 726518.75,
			822943.733050847, 726518.75,
			831954.855932203, 726518.75,
			840965.978813559, 726518.75,
			849977.101694915, 726518.75,
			858988.224576271, 726518.75,
			867999.347457627, 726518.75,
			877010.470338983, 726518.75,
			886021.593220339, 726518.75,
			895032.716101695, 726518.75,
			904043.838983051, 726518.75,
			913054.961864407, 726518.75,
			922066.084745763, 726518.75,
			931077.207627119, 726518.75,
			940088.330508475, 726518.75,
			949099.453389831, 726518.75,
			958110.576271186, 726518.75,
			967121.699152542, 726518.75,
			976132.822033898, 726518.75,
			985143.944915254, 726518.75,
			994155.06779661, 726518.75,
			1003166.19067797, 726518.75,
			1012177.31355932, 726518.75,
			1021188.43644068, 726518.75,
			1030199.55932203, 726518.75,
			1039210.68220339, 726518.75,
			1048221.80508475, 726518.75,
			1057232.9279661, 726518.75,
			1066244.05084746, 726518.75,
			1075255.17372881, 726518.75,
			1084266.29661017, 726518.75,
			1093277.41949153, 726518.75,
			1102288.54237288, 726518.75,
			1111299.66525424, 726518.75,
			1120310.78813559, 726518.75,
			1129321.91101695, 726518.75,
			1138333.03389831, 726518.75,
			1147344.15677966, 726518.75,
			1156355.27966102, 726518.75,
			1165366.40254237, 726518.75,
			1174377.52542373, 726518.75,
			1183388.64830508, 726518.75,
			1192399.77118644, 726518.75,
			1201410.8940678, 726518.75,
			1210422.01694915, 726518.75,
			1219433.13983051, 726518.75,
			1228444.26271186, 726518.75,
			1237455.38559322, 726518.75,
			1246466.50847458, 726518.75,
			1255477.63135593, 726518.75,
			1264488.75423729, 726518.75,
			1273499.87711864, 726518.75,
			1282511, 726518.75,
			746079.25, 712192.625,
			755090.372881356, 712192.625,
			764101.495762712, 712192.625,
			773112.618644068, 712192.625,
			782123.741525424, 712192.625,
			791134.86440678, 712192.625,
			800145.987288136, 712192.625,
			809157.110169492, 712192.625,
			818168.233050847, 712192.625,
			827179.355932203, 712192.625,
			836190.478813559, 712192.625,
			845201.601694915, 712192.625,
			854212.724576271, 712192.625,
			863223.847457627, 712192.625,
			872234.970338983, 712192.625,
			881246.093220339, 712192.625,
			890257.216101695, 712192.625,
			899268.338983051, 712192.625,
			908279.461864407, 712192.625,
			917290.584745763, 712192.625,
			926301.707627119, 712192.625,
			935312.830508475, 712192.625,
			944323.953389831, 712192.625,
			953335.076271186, 712192.625,
			962346.199152542, 712192.625,
			971357.322033898, 712192.625,
			980368.444915254, 712192.625,
			989379.56779661, 712192.625,
			998390.690677966, 712192.625,
			1007401.81355932, 712192.625,
			1016412.93644068, 712192.625,
			1025424.05932203, 712192.625,
			1034435.18220339, 712192.625,
			1043446.30508475, 712192.625,
			1052457.4279661, 712192.625,
			1061468.55084746, 712192.625,
			1070479.67372881, 712192.625,
			1079490.79661017, 712192.625,
			1088501.91949153, 712192.625,
			1097513.04237288, 712192.625,
			1106524.16525424, 712192.625,
			1115535.28813559, 712192.625,
			1124546.41101695, 712192.625,
			1133557.53389831, 712192.625,
			1142568.65677966, 712192.625,
			1151579.77966102, 712192.625,
			1160590.90254237, 712192.625,
			1169602.02542373, 712192.625,
			1178613.14830508, 712192.625,
			1187624.27118644, 712192.625,
			1196635.3940678, 712192.625,
			1205646.51694915, 712192.625,
			1214657.63983051, 712192.625,
			1223668.76271186, 712192.625,
			1232679.88559322, 712192.625,
			1241691.00847458, 712192.625,
			1250702.13135593, 712192.625,
			1259713.25423729, 712192.625,
			1268724.37711864, 712192.625,
			1277735.5, 712192.625,
			749263, 693091.25,
			759106.076530612, 693091.25,
			768949.153061225, 693091.25,
			778792.229591837, 693091.25,
			788635.306122449, 693091.25,
			798478.382653061, 693091.25,
			808321.459183673, 693091.25,
			818164.535714286, 693091.25,
			828007.612244898, 693091.25,
			837850.68877551, 693091.25,
			847693.765306122, 693091.25,
			857536.841836735, 693091.25,
			867379.918367347, 693091.25,
			877222.994897959, 693091.25,
			887066.071428571, 693091.25,
			896909.147959184, 693091.25,
			906752.224489796, 693091.25,
			916595.301020408, 693091.25,
			926438.37755102, 693091.25,
			936281.454081633, 693091.25,
			946124.530612245, 693091.25,
			955967.607142857, 693091.25,
			965810.683673469, 693091.25,
			975653.760204082, 693091.25,
			985496.836734694, 693091.25,
			995339.913265306, 693091.25,
			1005182.98979592, 693091.25,
			1015026.06632653, 693091.25,
			1024869.14285714, 693091.25,
			1034712.21938776, 693091.25,
			1044555.29591837, 693091.25,
			1054398.37244898, 693091.25,
			1064241.44897959, 693091.25,
			1074084.5255102, 693091.25,
			1083927.60204082, 693091.25,
			1093770.67857143, 693091.25,
			1103613.75510204, 693091.25,
			1113456.83163265, 693091.25,
			1123299.90816327, 693091.25,
			1133142.98469388, 693091.25,
			1142986.06122449, 693091.25,
			1152829.1377551, 693091.25,
			1162672.21428571, 693091.25,
			1172515.29081633, 693091.25,
			1182358.36734694, 693091.25,
			1192201.44387755, 693091.25,
			1202044.52040816, 693091.25,
			1211887.59693878, 693091.25,
			1221730.67346939, 693091.25,
			1231573.75, 693091.25,
			1276898.5, 589131.25,
			1276898.5, 582086.626588983,
			1276898.5, 575042.003177966,
			1276898.5, 567997.379766949,
			1276898.5, 560952.756355932,
			1276898.5, 553908.132944915,
			1276898.5, 546863.509533898,
			1276898.5, 539818.886122881,
			1276898.5, 532774.262711864,
			1276898.5, 525729.639300847,
			1276898.5, 518685.015889831,
			1276898.5, 511640.392478814,
			1276898.5, 504595.769067797,
			1276898.5, 497551.14565678,
			1276898.5, 490506.522245763,
			1276898.5, 483461.898834746,
			1276898.5, 476417.275423729,
			1276898.5, 469372.652012712,
			1276898.5, 462328.028601695,
			1276898.5, 455283.405190678,
			1276898.5, 448238.781779661,
			1276898.5, 441194.158368644,
			1276898.5, 434149.534957627,
			1276898.5, 427104.91154661,
			1276898.5, 420060.288135593,
			1276898.5, 413015.664724576,
			1276898.5, 405971.041313559,
			1276898.5, 398926.417902542,
			1276898.5, 391881.794491525,
			1276898.5, 384837.171080509,
			1276898.5, 377792.547669492,
			1276898.5, 370747.924258475,
			1276898.5, 363703.300847458,
			1276898.5, 356658.677436441,
			1276898.5, 349614.054025424,
			1276898.5, 342569.430614407,
			1276898.5, 335524.80720339,
			1276898.5, 328480.183792373,
			1276898.5, 321435.560381356,
			1276898.5, 314390.936970339,
			1276898.5, 307346.313559322,
			1276898.5, 300301.690148305,
			1276898.5, 293257.066737288,
			1276898.5, 286212.443326271,
			1276898.5, 279167.819915254,
			1276898.5, 272123.196504237,
			1276898.5, 265078.57309322,
			1276898.5, 258033.949682203,
			1276898.5, 250989.326271186,
			1276898.5, 243944.70286017,
			1276898.5, 236900.079449153,
			1276898.5, 229855.456038136,
			1276898.5, 222810.832627119,
			1276898.5, 215766.209216102,
			1276898.5, 208721.585805085,
			1276898.5, 201676.962394068,
			1276898.5, 194632.338983051,
			1276898.5, 187587.715572034,
			1276898.5, 180543.092161017,
			1276898.5, 173498.46875,
			1259868.5, 594168.4375,
			1259868.5, 586981.753707627,
			1259868.5, 579795.069915254,
			1259868.5, 572608.386122881,
			1259868.5, 565421.702330509,
			1259868.5, 558235.018538136,
			1259868.5, 551048.334745763,
			1259868.5, 543861.65095339,
			1259868.5, 536674.967161017,
			1259868.5, 529488.283368644,
			1259868.5, 522301.599576271,
			1259868.5, 515114.915783898,
			1259868.5, 507928.231991525,
			1259868.5, 500741.548199153,
			1259868.5, 493554.86440678,
			1259868.5, 486368.180614407,
			1259868.5, 479181.496822034,
			1259868.5, 471994.813029661,
			1259868.5, 464808.129237288,
			1259868.5, 457621.445444915,
			1259868.5, 450434.761652542,
			1259868.5, 443248.077860169,
			1259868.5, 436061.394067797,
			1259868.5, 428874.710275424,
			1259868.5, 421688.026483051,
			1259868.5, 414501.342690678,
			1259868.5, 407314.658898305,
			1259868.5, 400127.975105932,
			1259868.5, 392941.291313559,
			1259868.5, 385754.607521186,
			1259868.5, 378567.923728814,
			1259868.5, 371381.239936441,
			1259868.5, 364194.556144068,
			1259868.5, 357007.872351695,
			1259868.5, 349821.188559322,
			1259868.5, 342634.504766949,
			1259868.5, 335447.820974576,
			1259868.5, 328261.137182203,
			1259868.5, 321074.45338983,
			1259868.5, 313887.769597458,
			1259868.5, 306701.085805085,
			1259868.5, 299514.402012712,
			1259868.5, 292327.718220339,
			1259868.5, 285141.034427966,
			1259868.5, 277954.350635593,
			1259868.5, 270767.66684322,
			1259868.5, 263580.983050847,
			1259868.5, 256394.299258475,
			1259868.5, 249207.615466102,
			1259868.5, 242020.931673729,
			1259868.5, 234834.247881356,
			1259868.5, 227647.564088983,
			1259868.5, 220460.88029661,
			1259868.5, 213274.196504237,
			1259868.5, 206087.512711864,
			1259868.5, 198900.828919491,
			1259868.5, 191714.145127119,
			1259868.5, 184527.461334746,
			1259868.5, 177340.777542373,
			1259868.5, 170154.09375,
			1244202.75, 594168.4375,
			1244202.75, 586946.351694915,
			1244202.75, 579724.265889831,
			1244202.75, 572502.180084746,
			1244202.75, 565280.094279661,
			1244202.75, 558058.008474576,
			1244202.75, 550835.922669492,
			1244202.75, 543613.836864407,
			1244202.75, 536391.751059322,
			1244202.75, 529169.665254237,
			1244202.75, 521947.579449153,
			1244202.75, 514725.493644068,
			1244202.75, 507503.407838983,
			1244202.75, 500281.322033898,
			1244202.75, 493059.236228814,
			1244202.75, 485837.150423729,
			1244202.75, 478615.064618644,
			1244202.75, 471392.978813559,
			1244202.75, 464170.893008475,
			1244202.75, 456948.80720339,
			1244202.75, 449726.721398305,
			1244202.75, 442504.63559322,
			1244202.75, 435282.549788136,
			1244202.75, 428060.463983051,
			1244202.75, 420838.378177966,
			1244202.75, 413616.292372881,
			1244202.75, 406394.206567797,
			1244202.75, 399172.120762712,
			1244202.75, 391950.034957627,
			1244202.75, 384727.949152542,
			1244202.75, 377505.863347458,
			1244202.75, 370283.777542373,
			1244202.75, 363061.691737288,
			1244202.75, 355839.605932203,
			1244202.75, 348617.520127119,
			1244202.75, 341395.434322034,
			1244202.75, 334173.348516949,
			1244202.75, 326951.262711864,
			1244202.75, 319729.17690678,
			1244202.75, 312507.091101695,
			1244202.75, 305285.00529661,
			1244202.75, 298062.919491525,
			1244202.75, 290840.833686441,
			1244202.75, 283618.747881356,
			1244202.75, 276396.662076271,
			1244202.75, 269174.576271186,
			1244202.75, 261952.490466102,
			1244202.75, 254730.404661017,
			1244202.75, 247508.318855932,
			1244202.75, 240286.233050847,
			1244202.75, 233064.147245763,
			1244202.75, 225842.061440678,
			1244202.75, 218619.975635593,
			1244202.75, 211397.889830509,
			1244202.75, 204175.804025424,
			1244202.75, 196953.718220339,
			1244202.75, 189731.632415254,
			1244202.75, 182509.546610169,
			1244202.75, 175287.460805085,
			1244202.75, 168065.375,
			721005.25, 672386.1875,
			712508.413793103, 672386.1875,
			704011.577586207, 672386.1875,
			695514.74137931, 672386.1875,
			687017.905172414, 672386.1875,
			678521.068965517, 672386.1875,
			670024.232758621, 672386.1875,
			661527.396551724, 672386.1875,
			653030.560344828, 672386.1875,
			644533.724137931, 672386.1875,
			636036.887931034, 672386.1875,
			627540.051724138, 672386.1875,
			619043.215517241, 672386.1875,
			610546.379310345, 672386.1875,
			602049.543103448, 672386.1875,
			593552.706896552, 672386.1875,
			585055.870689655, 672386.1875,
			576559.034482759, 672386.1875,
			568062.198275862, 672386.1875,
			559565.362068966, 672386.1875,
			551068.525862069, 672386.1875,
			542571.689655172, 672386.1875,
			534074.853448276, 672386.1875,
			525578.017241379, 672386.1875,
			517081.181034483, 672386.1875,
			508584.344827586, 672386.1875,
			500087.50862069, 672386.1875,
			491590.672413793, 672386.1875,
			483093.836206897, 672386.1875,
			474597, 672386.1875,
			722438, 656627.5,
			713743.551724138, 656627.5,
			705049.103448276, 656627.5,
			696354.655172414, 656627.5,
			687660.206896552, 656627.5,
			678965.75862069, 656627.5,
			670271.310344828, 656627.5,
			661576.862068966, 656627.5,
			652882.413793103, 656627.5,
			644187.965517241, 656627.5,
			635493.517241379, 656627.5,
			626799.068965517, 656627.5,
			618104.620689655, 656627.5,
			609410.172413793, 656627.5,
			600715.724137931, 656627.5,
			592021.275862069, 656627.5,
			583326.827586207, 656627.5,
			574632.379310345, 656627.5,
			565937.931034483, 656627.5,
			557243.482758621, 656627.5,
			548549.034482759, 656627.5,
			539854.586206897, 656627.5,
			531160.137931034, 656627.5,
			522465.689655172, 656627.5,
			513771.24137931, 656627.5,
			505076.793103448, 656627.5,
			496382.344827586, 656627.5,
			487687.896551724, 656627.5,
			478993.448275862, 656627.5,
			470299, 656627.5,
			722438, 639436.25,
			713694.155172414, 639436.25,
			704950.310344828, 639436.25,
			696206.465517241, 639436.25,
			687462.620689655, 639436.25,
			678718.775862069, 639436.25,
			669974.931034483, 639436.25,
			661231.086206897, 639436.25,
			652487.24137931, 639436.25,
			643743.396551724, 639436.25,
			634999.551724138, 639436.25,
			626255.706896552, 639436.25,
			617511.862068966, 639436.25,
			608768.017241379, 639436.25,
			600024.172413793, 639436.25,
			591280.327586207, 639436.25,
			582536.482758621, 639436.25,
			573792.637931034, 639436.25,
			565048.793103448, 639436.25,
			556304.948275862, 639436.25,
			547561.103448276, 639436.25,
			538817.25862069, 639436.25,
			530073.413793103, 639436.25,
			521329.568965517, 639436.25,
			512585.724137931, 639436.25,
			503841.879310345, 639436.25,
			495098.034482759, 639436.25,
			486354.189655172, 639436.25,
			477610.344827586, 639436.25,
			468866.5, 639436.25,
			636202.75, 60068.5625,
			645949.330508475, 60068.5625,
			655695.911016949, 60068.5625,
			665442.491525424, 60068.5625,
			675189.072033898, 60068.5625,
			684935.652542373, 60068.5625,
			694682.233050847, 60068.5625,
			704428.813559322, 60068.5625,
			714175.394067797, 60068.5625,
			723921.974576271, 60068.5625,
			733668.555084746, 60068.5625,
			743415.13559322, 60068.5625,
			753161.716101695, 60068.5625,
			762908.296610169, 60068.5625,
			772654.877118644, 60068.5625,
			782401.457627119, 60068.5625,
			792148.038135593, 60068.5625,
			801894.618644068, 60068.5625,
			811641.199152542, 60068.5625,
			821387.779661017, 60068.5625,
			831134.360169492, 60068.5625,
			840880.940677966, 60068.5625,
			850627.521186441, 60068.5625,
			860374.101694915, 60068.5625,
			870120.68220339, 60068.5625,
			879867.262711864, 60068.5625,
			889613.843220339, 60068.5625,
			899360.423728814, 60068.5625,
			909107.004237288, 60068.5625,
			918853.584745763, 60068.5625,
			928600.165254237, 60068.5625,
			938346.745762712, 60068.5625,
			948093.326271186, 60068.5625,
			957839.906779661, 60068.5625,
			967586.487288136, 60068.5625,
			977333.06779661, 60068.5625,
			987079.648305085, 60068.5625,
			996826.228813559, 60068.5625,
			1006572.80932203, 60068.5625,
			1016319.38983051, 60068.5625,
			1026065.97033898, 60068.5625,
			1035812.55084746, 60068.5625,
			1045559.13135593, 60068.5625,
			1055305.71186441, 60068.5625,
			1065052.29237288, 60068.5625,
			1074798.87288136, 60068.5625,
			1084545.45338983, 60068.5625,
			1094292.03389831, 60068.5625,
			1104038.61440678, 60068.5625,
			1113785.19491525, 60068.5625,
			1123531.77542373, 60068.5625,
			1133278.3559322, 60068.5625,
			1143024.93644068, 60068.5625,
			1152771.51694915, 60068.5625,
			1162518.09745763, 60068.5625,
			1172264.6779661, 60068.5625,
			1182011.25847458, 60068.5625,
			1191757.83898305, 60068.5625,
			1201504.41949153, 60068.5625,
			1211251, 60068.5625,
			631045.25, 43307.0625,
			640922.953389831, 43307.0625,
			650800.656779661, 43307.0625,
			660678.360169492, 43307.0625,
			670556.063559322, 43307.0625,
			680433.766949153, 43307.0625,
			690311.470338983, 43307.0625,
			700189.173728814, 43307.0625,
			710066.877118644, 43307.0625,
			719944.580508475, 43307.0625,
			729822.283898305, 43307.0625,
			739699.987288136, 43307.0625,
			749577.690677966, 43307.0625,
			759455.394067797, 43307.0625,
			769333.097457627, 43307.0625,
			779210.800847458, 43307.0625,
			789088.504237288, 43307.0625,
			798966.207627119, 43307.0625,
			808843.911016949, 43307.0625,
			818721.61440678, 43307.0625,
			828599.31779661, 43307.0625,
			838477.021186441, 43307.0625,
			848354.724576271, 43307.0625,
			858232.427966102, 43307.0625,
			868110.131355932, 43307.0625,
			877987.834745763, 43307.0625,
			887865.538135593, 43307.0625,
			897743.241525424, 43307.0625,
			907620.944915254, 43307.0625,
			917498.648305085, 43307.0625,
			927376.351694915, 43307.0625,
			937254.055084746, 43307.0625,
			947131.758474576, 43307.0625,
			957009.461864407, 43307.0625,
			966887.165254237, 43307.0625,
			976764.868644068, 43307.0625,
			986642.572033898, 43307.0625,
			996520.275423729, 43307.0625,
			1006397.97881356, 43307.0625,
			1016275.68220339, 43307.0625,
			1026153.38559322, 43307.0625,
			1036031.08898305, 43307.0625,
			1045908.79237288, 43307.0625,
			1055786.49576271, 43307.0625,
			1065664.19915254, 43307.0625,
			1075541.90254237, 43307.0625,
			1085419.6059322, 43307.0625,
			1095297.30932203, 43307.0625,
			1105175.01271186, 43307.0625,
			1115052.71610169, 43307.0625,
			1124930.41949153, 43307.0625,
			1134808.12288136, 43307.0625,
			1144685.82627119, 43307.0625,
			1154563.52966102, 43307.0625,
			1164441.23305085, 43307.0625,
			1174318.93644068, 43307.0625,
			1184196.63983051, 43307.0625,
			1194074.34322034, 43307.0625,
			1203952.04661017, 43307.0625,
			1213829.75, 43307.0625,
			634913.25, 29124.25,
			644659.830508475, 29124.25,
			654406.411016949, 29124.25,
			664152.991525424, 29124.25,
			673899.572033898, 29124.25,
			683646.152542373, 29124.25,
			693392.733050847, 29124.25,
			703139.313559322, 29124.25,
			712885.894067797, 29124.25,
			722632.474576271, 29124.25,
			732379.055084746, 29124.25,
			742125.63559322, 29124.25,
			751872.216101695, 29124.25,
			761618.796610169, 29124.25,
			771365.377118644, 29124.25,
			781111.957627119, 29124.25,
			790858.538135593, 29124.25,
			800605.118644068, 29124.25,
			810351.699152542, 29124.25,
			820098.279661017, 29124.25,
			829844.860169492, 29124.25,
			839591.440677966, 29124.25,
			849338.021186441, 29124.25,
			859084.601694915, 29124.25,
			868831.18220339, 29124.25,
			878577.762711864, 29124.25,
			888324.343220339, 29124.25,
			898070.923728814, 29124.25,
			907817.504237288, 29124.25,
			917564.084745763, 29124.25,
			927310.665254237, 29124.25,
			937057.245762712, 29124.25,
			946803.826271186, 29124.25,
			956550.406779661, 29124.25,
			966296.987288136, 29124.25,
			976043.56779661, 29124.25,
			985790.148305085, 29124.25,
			995536.728813559, 29124.25,
			1005283.30932203, 29124.25,
			1015029.88983051, 29124.25,
			1024776.47033898, 29124.25,
			1034523.05084746, 29124.25,
			1044269.63135593, 29124.25,
			1054016.21186441, 29124.25,
			1063762.79237288, 29124.25,
			1073509.37288136, 29124.25,
			1083255.95338983, 29124.25,
			1093002.53389831, 29124.25,
			1102749.11440678, 29124.25,
			1112495.69491525, 29124.25,
			1122242.27542373, 29124.25,
			1131988.8559322, 29124.25,
			1141735.43644068, 29124.25,
			1151482.01694915, 29124.25,
			1161228.59745763, 29124.25,
			1170975.1779661, 29124.25,
			1180721.75847458, 29124.25,
			1190468.33898305, 29124.25,
			1200214.91949153, 29124.25,
			1209961.5, 29124.25
		};

		bool UBaseView::CreateTree()
		{
			WndCtx * pWndCtx = GetWndContext(m_hWnd);
			if (pWndCtx != NULL)
			{

				std::string names1[] = { "Popular", "yhTree_A", "yhTree_A_001.mesh" };
				std::string names2[] = { "Popular", "yhTree_B", "yhTree_B_001.mesh" };

				int nCookie = UDB::GetNextCookie();
				bool badd = false;
				int nArrCount = sizeof(xys) / sizeof(float);

				Ogre::StaticGeometry* field = pWndCtx->sceneMgr->createStaticGeometry(names1[1] + Ogre::StringConverter::toString(nCookie));
				for (int i = 0; i < nArrCount; i += 2)
				{
					float tx = xys[i] / 1000.0f;
					float ty = 0.0f;
					float tz = -xys[i + 1] / 1000.0f;
					
					if (badd == false)
					{
						Ogre::Entity * pEntity = pWndCtx->sceneMgr->createEntity(names2[2]);
						Ogre::AxisAlignedBox box = pEntity->getBoundingBox();
						Ogre::MaterialPtr material = static_cast<Ogre::MaterialPtr>(MaterialManager::getSingleton().getByName(names2[1]));
						pEntity->setMaterial(material);
						pEntity->setCastShadows(false);							
						field->addEntity(pEntity, Ogre::Vector3(tx , ty + (box.getCenter().y / 2.0f) - 1.0f, tz), Quaternion::IDENTITY, Ogre::Vector3(2.3f, 2.8f, 2.3f));
					}
					else

					{
						Ogre::Entity * pEntity = pWndCtx->sceneMgr->createEntity(names1[2]);
						Ogre::AxisAlignedBox box = pEntity->getBoundingBox();
						Ogre::MaterialPtr material = static_cast<Ogre::MaterialPtr>(MaterialManager::getSingleton().getByName(names1[1]));

						pEntity->setMaterial(material);
						pEntity->setCastShadows(false);
						field->addEntity(pEntity, Ogre::Vector3(tx , ty + (box.getCenter().y / 2.0f) - 1.0f, tz ), Quaternion::IDENTITY, Ogre::Vector3(2.3f, 2.8f, 2.3f));

					}
					badd = !badd;
						
				}
				field->build();

				CreatePath();

				return true;
			}
			return false;
		}
		float ddd4[] = {
			75846.50, 694994.50, 80862.50, 695037.12, 85878.75, 695079.75, 90895.00, 695122.38, 95911.50, 695165.00, 100928.25, 695207.69, 105945.25, 695250.38, 110962.75, 695293.06, 115980.50, 695335.81, 120998.75, 695378.62, 126017.50, 695421.44, 131037.00, 695464.31, 136057.00, 695507.19, 141077.75, 695550.19, 146099.25, 695593.25, 151121.75, 695636.31, 156145.00, 695679.50, 161169.25, 695722.75, 166194.50, 695766.06, 171220.75, 695809.50, 176248.25, 695853.00, 181277.00, 695896.56, 186306.75, 695940.25, 191338.00, 695984.06, 196370.75, 696027.94, 201404.75, 696071.94, 206440.25, 696116.06, 211477.25, 696160.31, 216516.00, 696204.69, 221556.25, 696249.12, 226598.25, 696293.75, 231642.25, 696338.56, 236687.75, 696383.44, 241735.50, 696428.50, 246785.00, 696473.75, 251836.75, 696519.06, 256890.25, 696564.62, 261946.25, 696610.31, 267004.25, 696656.19, 272064.50, 696702.25, 277127.25, 696748.44, 282192.25, 696794.88, 287259.50, 696841.44, 292329.50, 696888.25, 297402.00, 696935.25, 302477.25, 696982.44, 307555.00, 697029.81, 312635.75, 697077.44, 317719.00, 697125.25, 322805.25, 697173.31, 327894.25, 697221.62, 332986.50, 697270.12, 338081.75, 697318.88, 343180.00, 697367.88, 348281.50, 697417.06, 353386.00, 697466.56, 358494.00, 697516.31, 363605.25, 697566.31, 368720.00, 697616.56, 373838.00, 697667.06, 378959.75, 697717.88, 384085.00, 697768.94, 389213.75, 697820.31, 394346.25, 697872.00, 399482.75, 697923.94, 404622.75, 697976.12, 409766.75, 698028.69, 414914.75, 698081.50, 420066.75, 698134.69, 425222.75, 698188.12, 430382.75, 698241.88, 435547.00, 698296.00, 440715.50, 698350.44, 445888.25, 698405.19, 451065.50, 698460.31, 456247.00, 698515.75, 461433.00, 698571.56, 466623.50, 698627.69, 471818.75, 698684.19, 477018.50, 698741.06, 482223.00, 698798.25, 487432.25, 698855.88, 492646.50, 698913.88, 497865.50, 698972.12, 503089.25, 699030.88, 505833.00, 699061.88, 513552.00, 699150.12, 518790.00, 699211.88, 524032.00, 699276.12, 529277.00, 699344.12, 534525.00, 699416.75, 539775.25, 699494.62, 545027.50, 699579.00, 550281.00, 699670.62, 555535.50, 699770.38, 560790.25, 699879.38, 566044.75, 699998.38, 571298.50, 700128.12, 576551.25, 700270.12, 581802.50, 700424.62, 587051.50, 700593.12, 592298.00, 700776.12, 597541.25, 700974.62, 602781.00, 701189.75, 608016.75, 701422.25, 613247.75, 701673.12, 618473.50, 701943.25, 623694.00, 702233.62, 628908.25, 702544.88, 634115.75, 702878.38, 639316.50, 703234.75, 644509.50, 703615.00, 649694.50, 704020.12, 654871.00, 704450.88, 660038.25, 704908.12, 665196.25, 705393.12, 670344.00, 705906.62, 675481.25, 706449.38, 680607.50, 707022.62, 685722.25, 707627.00, 690825.00, 708263.62, 695915.25, 708933.12, 700992.50, 709636.88, 706056.25, 710375.50, 711106.00, 711149.88, 716141.25, 711961.12, 721161.25, 712810.12, 726166.25, 713697.62, 731155.00, 714624.62, 736127.25, 715592.12, 741082.75, 716601.12, 746020.50, 717652.38, 750940.50, 718746.88, 755842.00, 719885.38, 760724.75, 721069.12, 765587.75, 722298.62, 770431.00, 723575.38, 775253.75, 724899.62, 780055.75, 726272.88, 784836.00, 727695.62, 789594.75, 729169.12, 794330.75, 730694.12, 799044.00, 732271.50, 803733.75, 733902.25, 808399.75, 735587.38, 813041.25, 737327.62, 817658.25, 739124.00, 822249.50, 740977.38, 826815.00, 742888.88, 831354.00, 744859.12, 835866.25, 746889.25, 840869.75, 749226.62, 844808.25, 751132.38, 849238.25, 753345.38, 853641.25, 755617.62, 858018.00, 757947.88, 862369.00, 760334.88, 866695.25, 762777.38, 870996.75, 765274.12, 875274.50, 767823.62, 879528.75, 770424.75, 883760.50, 773076.25, 887970.00, 775776.75, 892158.00, 778524.88, 896325.00, 781319.62, 900471.75, 784159.38, 904598.50, 787043.12, 908706.00, 789969.38, 912794.75, 792936.88, 916865.75, 795944.25, 920919.25, 798990.38, 924955.75, 802074.12, 928975.75, 805193.75, 932980.25, 808348.38, 936969.50, 811536.38, 940944.25, 814756.75, 944904.75, 818008.00, 948852.25, 821289.00, 952786.75, 824598.38, 956709.00, 827934.75, 960619.50, 831296.88, 964519.25, 834683.62, 968408.25, 838093.62, 972287.25, 841525.50, 976157.25, 844978.00, 980018.25, 848449.88, 983871.25, 851939.88, 987716.50, 855446.50, 991554.75, 858968.62, 995386.75, 862505.00, 999213.00, 866054.25, 1003033.75, 869615.12, 1006850.00, 873186.38, 1010662.25, 876766.62, 1014470.75, 880354.50, 1018276.50, 883948.88, 1022080.00, 887548.50, 1025881.50, 891151.88, 1029682.00, 894757.88, 1033482.00, 898365.25, 1019459.25, 897256.25, 1033496.00, 885486.88, 1033482.00, 898365.25
		};
		float ddd5[] = {
			413597.25, 357314.88, 414235.75, 371925.94, 414867.50, 386546.75, 415485.50, 401187.00, 415787.25, 408517.50, 416083.25, 415856.50, 416372.25, 423205.25, 416653.50, 430565.00, 416926.50, 437936.94, 417190.00, 445322.25, 417443.25, 452722.25, 417685.75, 460138.00, 417916.00, 467570.88, 418133.75, 475022.00, 418337.75, 482492.62, 418527.50, 489984.00, 418701.75, 497497.25, 418859.75, 505033.62, 419001.00, 512594.50, 419124.25, 520180.88, 419229.00, 527794.06, 419314.00, 535435.25, 419349.00, 539266.75, 419378.75, 543105.75, 419403.25, 546952.31, 419422.25, 550806.69, 419435.50, 554668.94, 419443.50, 558539.31, 419445.50, 562417.88, 419441.75, 566304.81, 419432.25, 570200.25, 419416.25, 574104.44, 419394.50, 578017.44, 419366.25, 581939.38, 419331.75, 585870.50, 419290.75, 589810.94, 419243.25, 593760.81, 419188.75, 597720.25, 419127.75, 601689.44, 419059.75, 605668.50, 418984.75, 609657.69, 418902.50, 613657.06, 418813.25, 617666.75, 418716.50, 621687.00, 418612.25, 625717.88, 418500.75, 629759.56, 418381.25, 633812.25, 418254.25, 637876.00, 418119.25, 641951.06, 417976.50, 646037.56, 417825.50, 650135.62, 417666.75, 654236.19, 417502.75, 658364.19, 417355.25, 662477.81, 417294.75, 664527.44, 417247.25, 666569.38, 417215.75, 668601.50, 417203.25, 670621.75, 417212.50, 672627.94, 417247.00, 674618.00, 417309.25, 676589.81, 417351.75, 677568.19, 417402.25, 678541.19, 417461.50, 679508.56, 417529.50, 680470.06, 417606.75, 681425.38, 417693.50, 682374.31, 417790.25, 683316.50, 417897.50, 684251.75, 418015.50, 685179.81, 418144.50, 686100.31, 418285.00, 687013.12, 418437.25, 687917.88, 418602.00, 688814.38, 418779.25, 689702.25, 418969.50, 690581.38, 419173.00, 691451.44, 419390.25, 692312.12, 419621.75, 693163.19, 419867.50, 694004.38, 420128.25, 694835.44, 420404.25, 695656.06, 420696.00, 696466.06, 421003.50, 697265.06, 421327.50, 698052.88, 421736.00, 698977.75, 422026.00, 699593.88, 422400.75, 700346.88, 422792.25, 701088.25, 423200.50, 701818.12, 423625.25, 702536.62, 424066.25, 703244.00, 424523.25, 703940.12, 424996.25, 704625.38, 425484.75, 705299.62, 425989.00, 705963.12, 426508.25, 706615.88, 427042.75, 707258.12, 427592.25, 707889.88, 428156.50, 708511.38, 428735.25, 709122.62, 429328.25, 709723.62, 429935.25, 710314.75, 430556.50, 710896.00, 431191.50, 711467.38, 431840.00, 712029.12, 432501.75, 712581.38, 433177.00, 713124.25, 433865.00, 713657.75, 434566.00, 714182.12, 435279.50, 714697.25, 436005.25, 715203.38, 436743.50, 715700.88, 437493.75, 716189.38, 438255.75, 716669.38, 439029.75, 717140.88, 439814.75, 717603.88, 440611.25, 718058.50, 441419.00, 718505.12, 442237.25, 718943.50, 443066.50, 719374.00, 443906.25, 719796.62, 444756.25, 720211.50, 445616.25, 720618.88, 446486.50, 721018.62, 447366.25, 721411.00, 448255.50, 721796.12, 449154.25, 722174.00, 450062.25, 722544.88, 450979.00, 722908.88, 451904.75, 723265.88, 452839.00, 723616.38, 453781.75, 723960.12, 454732.50, 724297.38, 455691.50, 724628.25, 456658.25, 724952.88, 457632.50, 725271.38, 458614.25, 725583.88, 459603.25, 725890.38, 460599.50, 726191.12, 461602.50, 726486.12, 463628.25, 727059.62, 465679.25, 727611.62, 467753.75, 728143.00, 469850.50, 728654.88, 471967.75, 729147.88, 474104.25, 729623.12, 476258.25, 730081.62, 478428.00, 730524.00, 480612.25, 730951.25, 482809.50, 731364.38, 485018.00, 731764.12, 487236.50, 732151.62, 489463.25, 732527.50, 491696.75, 732892.88, 493935.50, 733248.62, 500408.50, 734228.62, 502912.50, 734592.62, 507399.75, 735226.38, 511883.75, 735836.50, 516364.25, 736423.75, 520841.25, 736988.75, 525314.25, 737532.25, 529783.25, 738054.88, 534247.75, 738557.12, 538707.50, 739039.88, 543162.50, 739503.75, 547612.50, 739949.38, 552057.25, 740377.50, 556496.50, 740788.62, 560929.75, 741183.62, 565357.25, 741563.00, 569778.50, 741927.38, 574193.25, 742277.62, 578601.50, 742614.38, 583002.75, 742938.12, 587396.75, 743249.62, 591783.50, 743549.62, 596162.50, 743838.62, 600533.75, 744117.50, 604897.00, 744386.75, 609251.75, 744647.12, 613598.25, 744899.12, 617935.75, 745143.62, 622264.25, 745381.38, 626583.75, 745612.75, 630893.75, 745838.62, 635194.00, 746059.38, 639484.25, 746276.12, 643764.50, 746489.12, 648034.25, 746699.38, 652293.50, 746907.38, 656541.75, 747113.62, 660779.00, 747319.12, 665005.00, 747524.38, 669219.50, 747730.00, 673422.25, 747936.62, 677612.75, 748145.12, 681791.25, 748356.12, 685957.25, 748570.12, 690110.75, 748787.75, 694251.00, 749009.88, 698378.25, 749237.12, 702492.25, 749470.12, 706592.25, 749709.62, 710678.75, 749956.12, 714751.00, 750210.38, 718809.00, 750473.00, 722852.50, 750744.75, 726881.25, 751026.25, 730894.75, 751318.12, 734893.25, 751621.12, 738876.25, 751935.88, 742843.75, 752263.12, 746795.00, 752603.38, 750730.25, 752957.25, 754649.00, 753325.62, 758551.25, 753709.12, 762436.50, 754108.38, 766304.75, 754524.00, 770155.75, 754956.75, 773989.25, 755407.12, 777804.75, 755876.12, 781602.50, 756364.00, 785381.75, 756871.62, 789142.75, 757399.88, 792885.00, 757949.00, 796608.25, 758519.88, 800312.25, 759113.25, 803997.00, 759729.62, 807662.25, 760369.75, 811307.25, 761034.38, 814932.50, 761723.88, 818537.25, 762439.38, 822121.75, 763181.12, 825685.00, 763949.88, 829227.50, 764746.38, 830990.75, 765155.38, 832748.75, 765571.38, 834501.25, 765994.88, 836248.50, 766425.50, 837990.25, 766863.62, 839726.50, 767309.25, 841457.25, 767762.50, 843182.75, 768223.38, 844902.50, 768692.12, 846616.75, 769168.75, 848325.25, 769653.12, 850028.25, 770145.62, 851725.50, 770646.38, 853417.00, 771155.12, 855103.00, 771672.12, 856783.25, 772197.62, 858457.50, 772731.38, 860126.00, 773273.75, 861788.75, 773824.75, 863445.75, 774384.38, 865096.75, 774952.75, 866741.75, 775530.00, 868380.75, 776116.12, 870014.00, 776711.38, 871641.25, 777315.75, 873262.25, 777929.12, 874877.25, 778552.00, 876486.25, 779184.12, 878089.00, 779825.62, 879685.75, 780476.62, 881276.25, 781137.38, 882860.50, 781807.62, 884438.75, 782487.75, 886010.50, 783177.62, 887576.00, 783877.62, 889135.25, 784587.50, 890688.00, 785307.50, 892234.25, 786037.62, 893774.50, 786778.12, 895308.00, 787528.88, 896835.25, 788290.12, 898355.75, 789061.88, 899870.00, 789844.25, 901377.50, 790637.25, 902878.50, 791441.12, 904372.75, 792255.62, 905860.50, 793081.25, 907341.50, 793917.88, 908816.00, 794765.50, 910283.50, 795624.38, 911744.50, 796494.50, 913198.50, 797376.00, 914645.75, 798268.88, 916086.25, 799173.38, 917519.75, 800089.38, 918946.50, 801017.12, 920366.25, 801956.50, 921779.00, 802907.88, 923184.75, 803871.12, 924583.50, 804846.38, 925975.25, 805833.62, 927359.75, 806833.25, 928737.25, 807845.12, 930107.75, 808869.12, 931471.00, 809905.75, 932827.00, 810954.88, 934176.00, 812016.62, 935517.50, 813090.88, 936851.75, 814178.12, 938178.75, 815278.12, 939498.50, 816391.00, 940810.75, 817516.88, 942115.75, 818655.88, 943413.25, 819808.12, 944703.25, 820973.62, 945985.75, 822152.38, 947261.00, 823344.62, 948528.50, 824550.38, 949788.25, 825769.88, 951040.75, 827002.88, 952285.50, 828249.62, 953522.75, 829510.38, 954752.00, 830784.88, 955973.75, 832073.50, 957187.75, 833376.12, 958393.75, 834693.12, 959592.25, 836024.12, 960782.75, 837369.62, 961965.50, 838729.62, 963140.25, 840104.00, 964370.25, 841568.62, 965466.25, 842896.62, 966617.50, 844314.62, 967761.25, 845746.38, 968897.75, 847191.88, 970027.00, 848650.25, 971149.50, 850121.38, 972265.25, 851605.00, 973374.75, 853100.38, 974477.75, 854607.38, 975574.75, 856125.50, 976666.25, 857654.38, 977751.75, 859193.38, 978832.25, 860742.62, 979907.25, 862301.12, 980977.50, 863868.88, 982043.00, 865445.50, 983104.00, 867030.38, 985213.25, 870223.50, 987307.25, 873445.38, 989387.25, 876692.62, 991455.25, 879962.12, 993512.75, 883250.62, 995561.75, 886555.12, 997604.00, 889872.25, 999641.25, 893198.88, 1003706.50, 899868.38, 991862.75, 893781.88, 1004920.25, 886871.38, 1003706.50, 899868.38
		};
		float ddd3[] = {
			1854170.75, 654306.94,
			1824366.25, 655023.50,
			1809444.75, 655380.12,
			1794500.75, 655734.81,
			1779525.75, 656087.00,
			1764511.75, 656435.88,
			1756988.75, 656608.94,
			1749452.25, 656780.94,
			1741902.75, 656951.81, 1734339.25, 657121.50, 1726759.75, 657289.88, 1719164.25, 657456.88, 1711551.75, 657622.38, 1703920.25, 657786.44, 1696270.25, 657948.81, 1688599.75, 658109.56, 1680908.25, 658268.50, 1673194.75, 658425.56, 1665457.75, 658580.75, 1657697.25, 658733.88, 1649911.75, 658884.94, 1642099.75, 659033.81, 1634261.75, 659180.44, 1626395.25, 659324.69, 1618500.25, 659466.56, 1610575.75, 659605.94, 1602619.75, 659742.69, 1594632.75, 659876.81, 1586613.00, 660008.19, 1578559.50, 660136.75, 1570471.75, 660262.44, 1562348.25, 660385.12, 1554188.25, 660504.75, 1545990.75, 660621.19, 1537754.75, 660734.44, 1529479.75, 660844.44, 1521164.25, 660951.00, 1512807.50, 661054.06, 1504408.50, 661153.62, 1495966.25, 661249.56, 1487479.75, 661341.75, 1478948.50, 661430.19, 1470370.75, 661514.75, 1461746.25, 661595.38, 1453073.75, 661671.94, 1444352.25, 661744.44, 1439972.75, 661779.12, 1435580.75, 661812.69, 1431176.25, 661845.25, 1426758.50, 661876.75, 1422328.00, 661907.12, 1417884.25, 661936.38, 1413427.50, 661964.56, 1408957.25, 661991.62, 1404473.75, 662017.56, 1399976.75, 662042.31, 1395465.75, 662066.00, 1390941.25, 662088.44, 1386402.75, 662109.75, 1381011.25, 662133.50, 1372712.25, 662174.62, 1368143.25, 662213.00, 1363586.75, 662272.75, 1359052.25, 662361.94, 1356796.25, 662420.00, 1354549.25, 662488.50, 1352312.50, 662568.31, 1350087.25, 662660.44, 1347874.25, 662765.94, 1345675.50, 662885.81, 1343491.50, 663021.06, 1341323.75, 663172.69, 1339173.25, 663341.62, 1337041.25, 663528.94, 1334929.00, 663735.56, 1332837.75, 663962.62, 1330768.25, 664211.00, 1328722.25, 664481.75, 1326700.75, 664775.88, 1324704.75, 665094.31, 1322735.75, 665438.19, 1320794.25, 665808.38, 1318882.25, 666205.94, 1317000.75, 666631.81, 1315150.75, 667087.06, 1313333.25, 667572.69, 1311549.75, 668089.62, 1309801.25, 668638.94, 1308940.75, 668926.00, 1308089.50, 669221.56, 1307247.25, 669525.69, 1306414.75, 669838.50, 1305591.75, 670160.19, 1304778.75, 670490.81, 1303975.75, 670830.50, 1303182.75, 671179.38, 1302399.75, 671537.62, 1301627.75, 671905.31, 1300865.75, 672282.62, 1300114.75, 672669.56, 1299374.25, 673066.38, 1298645.00, 673473.12, 1297926.75, 673889.94, 1297220.00, 674317.00, 1296524.50, 674754.31, 1295840.75, 675202.12, 1295168.50, 675660.50, 1294508.25, 676129.50, 1293798.00, 676656.25, 1293223.75, 677100.19, 1292599.75, 677601.94, 1291987.25, 678114.44, 1291387.25, 678637.62, 1290798.25, 679171.50, 1290221.25, 679715.81, 1289655.75, 680270.62, 1289101.25, 680835.81, 1288558.25, 681411.25, 1288026.25, 681996.81, 1287505.50, 682592.50, 1286995.25, 683198.19, 1286496.00, 683813.75, 1286007.25, 684439.12, 1285529.25, 685074.25, 1285061.50, 685719.00, 1284604.25, 686373.31, 1284156.75, 687037.06, 1283719.75, 687710.19, 1283292.25, 688392.62, 1282875.00, 689084.19, 1282467.25, 689784.88, 1282069.25, 690494.56, 1281680.75, 691213.12, 1281301.25, 691940.56, 1280931.25, 692676.75, 1280570.75, 693421.31, 1279875.75, 694936.69, 1279216.25, 696485.38, 1278590.75, 698066.81, 1277998.75, 699680.25, 1277439.00, 701325.00, 1276910.75, 703000.38, 1276413.00, 704705.62, 1275944.75, 706439.88, 1275505.50, 708202.62, 1275093.75, 709993.12, 1274708.75, 711810.38, 1274349.75, 713653.88, 1274015.75, 715522.88, 1273705.75, 717416.75, 1273418.75, 719334.62, 1273154.25, 721275.62, 1272910.75, 723239.38, 1272687.50, 725224.88, 1272483.75, 727231.38, 1272298.25, 729258.38, 1272130.75, 731304.88, 1271979.25, 733370.50, 1271843.75, 735454.12, 1271723.25, 737555.38, 1271616.25, 739673.25, 1271522.00, 741807.12, 1271439.75, 743956.38, 1271368.75, 746120.00, 1271307.75, 748297.62, 1271255.75, 750488.12, 1271212.00, 752691.12, 1271175.75, 754905.62, 1271121.00, 759366.75, 1271084.25, 763865.62, 1271058.25, 768396.38, 1271035.25, 772953.25, 1271008.00, 777530.38, 1270968.75, 782122.12, 1270910.50, 786722.25, 1270825.25, 791325.38, 1270706.25, 795925.38, 1270545.50, 800516.38, 1270447.25, 802806.88, 1270335.75, 805092.88, 1270210.00, 807373.88, 1270069.25, 809648.88, 1269912.50, 811917.38, 1269739.00, 814178.62, 1269547.50, 816431.62, 1269337.25, 818676.12, 1269107.25, 820911.00, 1268856.75, 823135.62, 1268584.50, 825349.38, 1268289.75, 827551.50, 1267971.75, 829741.12, 1267629.25, 831917.62, 1267261.25, 834080.38, 1266867.25, 836228.38, 1266446.00, 838361.25, 1265996.75, 840478.00, 1265518.25, 842578.00, 1265009.75, 844660.50, 1264470.75, 846724.75, 1263899.50, 848770.12, 1263295.75, 850795.75, 1262658.00, 852801.00, 1261985.75, 854785.12, 1261278.00, 856747.38, 1260533.75, 858687.00, 1259752.00, 860603.38, 1258931.75, 862495.62, 1258021.50, 864470.12, 1257173.00, 866205.12, 1256234.25, 868021.62, 1255256.50, 869812.88, 1254241.25, 871578.62, 1253188.75, 873319.38, 1252099.75, 875034.88, 1250975.75, 876725.38, 1249817.25, 878391.12, 1248624.75, 880031.88, 1247399.25, 881648.12, 1246141.75, 883239.62, 1244853.00, 884806.75, 1243533.75, 886349.38, 1242184.75, 887867.88, 1240806.75, 889362.12, 1239401.00, 890832.38, 1237967.75, 892278.62, 1236508.50, 893701.00, 1235023.25, 895099.62, 1233513.50, 896474.62, 1231979.75, 897826.00, 1230422.75, 899154.00, 1228843.75, 900458.62, 1227243.00, 901740.12, 1225621.75, 902998.38, 1223980.25, 904233.62, 1222320.25, 905445.88, 1220641.75, 906635.38, 1218945.75, 907802.12, 1217233.25, 908946.25, 1215505.25, 910067.88, 1213762.00, 911167.12, 1212004.75, 912244.12, 1210234.00, 913298.88, 1208450.75, 914331.50, 1206656.25, 915342.12, 1204850.25, 916330.88, 1203034.75, 917297.88, 1201209.75, 918243.12, 1199376.25, 919166.88, 1197535.25, 920069.12, 1195687.75, 920950.12, 1193833.75, 921809.62, 1191995.75, 922638.88, 1190111.75, 923465.50, 1188244.25, 924262.12, 1186372.25, 925038.12, 1184496.25, 925793.75, 1182616.00, 926529.12, 1180731.75, 927244.88, 1178843.25, 927940.62, 1176951.00, 928617.12, 1175054.75, 929274.38, 1173154.25, 929912.62, 1171250.25, 930532.00, 1169342.25, 931132.88, 1167430.75, 931715.50, 1165515.25, 932280.12, 1163596.25, 932826.62, 1161673.25, 933355.75, 1159747.00, 933867.38, 1157817.25, 934361.88, 1155883.75, 934839.50, 1153947.00, 935300.38, 1152006.75, 935744.62, 1150063.25, 936172.88, 1148116.50, 936584.88, 1146166.25, 936981.25, 1144213.25, 937362.00, 1142256.75, 937727.38, 1140297.25, 938077.75, 1138334.50, 938413.12, 1136369.00, 938733.88, 1134400.25, 939040.38, 1132428.75, 939332.50, 1130454.50, 939610.75, 1128477.25, 939875.25, 1126497.25, 940126.25, 1124514.50, 940363.88, 1122529.25, 940588.62, 1120541.25, 940800.50, 1118550.25, 940999.88, 1114561.75, 941361.50, 1110563.00, 941675.62, 1106555.00, 941943.88, 1102538.00, 942168.38, 1098512.25, 942350.62, 1094478.25, 942492.62, 1090436.25, 942596.38, 1086387.00, 942663.38, 1082330.25, 942695.50, 1078267.00, 942694.75, 1074197.25, 942662.88, 1070121.75, 942601.75, 1066040.25, 942513.12, 1061953.50, 942398.88, 1057862.00, 942260.88, 1053766.00, 942100.88, 1066770.00, 931642.00, 1066770.00, 952115.50, 1053766.00, 942100.88
		};
		float ddd2[] =
		{
			1301184.50, 453544.75,
			1301321.75, 456526.56,
			1301458.75, 459508.25,
			1301594.50, 462489.75,
			1301728.75, 465470.69,
			1301861.25, 468451.12,
			1301991.25, 471430.75,
			1302118.25, 474409.56,
			1302241.75, 477387.38,
			1302361.25, 480364.00,
			1302476.25, 483339.25,
			1302586.75, 486313.12,
			1302691.25, 489285.31,
			1302790.25, 492255.81,
			1302882.50, 495224.38, 
			1302968.00, 498190.94,
			1303046.25, 501155.31,
			1303116.25, 504117.31, 1303178.00, 507076.88, 1303230.75, 510033.75, 1303274.25, 512987.94, 1303308.00, 515939.19, 1303331.25, 518887.38, 1303343.50, 521832.31, 1303344.50, 524773.94, 1303333.75, 527712.00, 1303310.25, 530646.50, 1303274.25, 533577.12, 1303225.00, 536503.88, 1303161.75, 539426.50, 1303084.25, 542344.94, 1302991.75, 545258.94, 1302884.00, 548168.50, 1302760.50, 551073.31, 1302620.75, 553973.38, 1302464.25, 556868.44, 1302290.25, 559758.44, 1302098.50, 562643.12, 1301888.50, 565522.50, 1301659.75, 568396.25, 1301411.75, 571264.38, 1301144.00, 574126.62, 1300855.75, 576982.94, 1300547.25, 579833.06, 1300217.25, 582676.94, 1299865.25, 585514.44, 1299491.25, 588345.31, 1299094.75, 591169.50, 1298674.75, 593986.88, 1298231.00, 596797.19, 1297763.25, 599600.44, 1297270.50, 602396.31, 1296752.75, 605184.75, 1296209.25, 607965.62, 1295639.75, 610738.81, 1295043.25, 613504.06, 1294419.75, 616261.31, 1293768.75, 619010.44, 1293089.25, 621751.19, 1292381.25, 624483.50, 1291644.25, 627207.19, 1290877.25, 629922.12, 1290080.25, 632628.19, 1289252.75, 635325.25, 1288131.25, 638814.81, 1287503.75, 640691.56, 1286582.25, 643360.69, 1285629.25, 646020.38, 1284645.75, 648670.69, 1283631.75, 651311.56, 1282587.75, 653943.06, 1281513.75, 656565.06, 1280411.00, 659177.62, 1279279.25, 661780.75, 1278119.00, 664374.38, 1276930.75, 666958.50, 1275714.50, 669533.12, 1274470.75, 672098.25, 1273200.25, 674653.88, 1271903.25, 677199.94, 1270579.75, 679736.44, 1269230.75, 682263.44, 1267855.75, 684780.88, 1266456.00, 687288.69, 1265031.25, 689786.94, 1263582.25, 692275.56, 1262109.50, 694754.62, 1260612.75, 697224.00, 1259093.00, 699683.88, 1257550.25, 702133.88, 1255985.25, 704574.38, 1254397.75, 707005.25, 1252788.75, 709426.38, 1251158.25, 711837.88, 1249507.00, 714239.62, 1247835.00, 716631.62, 1246142.75, 719013.88, 1244430.75, 721386.38, 1242699.00, 723749.38, 1240948.25, 726102.38, 1239178.75, 728445.62, 1237391.00, 730779.12, 1235585.25, 733102.88, 1233761.75, 735416.88, 1231921.25, 737720.88, 1230063.75, 740015.25, 1228189.75, 742299.62, 1226299.75, 744574.38, 1224394.00, 746839.12, 1222472.75, 749093.88, 1220536.75, 751338.88, 1218585.75, 753574.12, 1216621.00, 755799.25, 1214642.25, 758014.62, 1212649.75, 760219.88, 1210644.50, 762415.38, 1208626.25, 764600.88, 1206595.75, 766776.38, 1204553.25, 768941.88, 1202499.25, 771097.38, 1200434.25, 773243.00, 1198358.00, 775378.62, 1196271.25, 777504.12, 1194174.75, 779619.62, 1192068.25, 781725.12, 1189952.75, 783820.62, 1187827.75, 785906.00, 1185694.50, 787981.25, 1183553.00, 790046.50, 1181403.75, 792101.62, 1179246.75, 794146.62, 1177082.75, 796181.62, 1174912.25, 798206.38, 1172735.25, 800220.88, 1170552.25, 802225.38, 1168363.75, 804219.75, 1166170.00, 806203.88, 1163691.25, 808428.38, 1161768.25, 810141.62, 1159560.75, 812095.38, 1157349.25, 814039.25, 1155133.25, 815973.38, 1152913.00, 817897.88, 1150688.75, 819813.12, 1148460.75, 821719.12, 1146228.75, 823615.88, 1143992.75, 825503.88, 1141753.25, 827383.12, 1139509.75, 829253.88, 1137262.75, 831116.12, 1135012.00, 832970.12, 1132757.75, 834816.12, 1130500.25, 836654.12, 1128239.50, 838484.38, 1125975.25, 840307.12, 1123707.75, 842122.38, 1121437.25, 843930.38, 1119163.75, 845731.38, 1116887.25, 847525.38, 1114607.75, 849312.62, 1112325.25, 851093.38, 1110040.25, 852867.62, 1107752.25, 854635.62, 1105461.75, 856397.62, 1103168.75, 858153.50, 1100873.25, 859903.62, 1098575.25, 861648.38, 1096274.75, 863387.38, 1093972.25, 865121.38, 1091667.25, 866850.12, 1089360.25, 868573.88, 1087051.25, 870293.00, 1084740.25, 872007.38, 1082427.25, 873717.38, 1080112.25, 875423.12, 1077795.75, 877124.62, 1075477.25, 878822.38, 1073157.25, 880516.12, 1070835.75, 882206.38, 1068512.75, 883893.12, 1066188.25, 885576.50, 1063862.50, 887256.88, 1061535.25, 888934.12, 1059207.00, 890608.75, 1056877.50, 892280.62, 1054547.00, 893950.12, 1052215.50, 895617.12, 1049883.00, 897282.12, 1047549.75, 898945.12, 1050186.00, 885073.50, 1062019.50, 900196.00, 1047549.75, 898945.12
		};
		
		float ddd[] = { // type 3
			971174.75, 899043.38, 
			969464.25, 897523.50, 
			967753.25, 896005.50, 
			966041.00, 894489.50, 
			964327.75, 892975.62, 
			962613.25, 891464.25, 
			960897.50, 889955.38,
			959180.25, 888449.00, 
			957461.75, 886945.50,
			955741.75, 885444.88,
			954020.00, 883947.62, 952296.75, 882453.38, 950571.75, 880962.75, 948844.75, 879475.62, 947116.00, 877992.38, 945385.25, 876512.88, 943652.50, 875037.50, 941917.50, 873566.38, 940180.25, 872099.62, 938440.75, 870637.38, 936699.00, 869179.75, 934954.50, 867727.00, 933207.75, 866279.38, 931458.25, 864836.75, 929706.00, 863399.50, 927951.00, 861967.62, 926193.25, 860541.50, 924432.50, 859121.12, 922668.75, 857706.62, 920901.75, 856298.38, 919131.75, 854896.12, 917358.50, 853500.50, 915581.75, 852111.38, 913801.75, 850728.88, 912018.25, 849353.38, 910231.25, 847984.88, 908440.50, 846623.50, 906646.00, 845269.50, 904847.75, 843923.00, 903045.50, 842584.12, 901239.50, 841253.12, 899429.25, 839930.12, 897615.00, 838615.12, 895796.25, 837308.38, 893973.75, 836010.12, 892146.50, 834720.62, 890314.75, 833439.62, 888478.75, 832167.62, 886638.00, 830904.75, 884792.50, 829651.12, 882942.25, 828406.75, 881087.25, 827171.88, 879227.50, 825946.88, 877362.50, 824731.50, 875492.50, 823526.25, 873617.25, 822331.12, 871737.00, 821146.25, 869851.25, 819971.88, 867960.25, 818808.12, 866063.50, 817655.25, 864161.25, 816513.12, 862253.75, 815382.38, 860340.25, 814262.62, 858420.75, 813154.38, 856495.75, 812057.62, 854564.75, 810972.62, 852627.50, 809899.50, 850684.25, 808838.38, 848734.75, 807789.38, 846779.25, 806752.88, 844817.00, 805728.88, 842848.75, 804717.38, 840873.75, 803718.75, 838892.00, 802733.12, 836903.75, 801760.62, 834455.25, 800585.50, 832907.25, 799855.50, 830898.75, 798922.88, 828883.50, 798003.50, 826861.75, 797096.88, 824833.50, 796203.00, 822799.00, 795321.62, 820758.25, 794452.38, 818711.75, 793595.12, 816658.75, 792749.62, 814600.25, 791915.88, 812535.75, 791093.38, 810465.75, 790282.12, 808390.00, 789481.62, 806308.75, 788692.12, 804222.50, 787912.88, 802130.75, 787144.12, 800034.00, 786385.38, 797932.25, 785636.50, 795825.25, 784897.38, 793713.75, 784167.62, 791597.75, 783447.00, 789476.75, 782735.50, 787351.75, 782032.75, 785222.00, 781338.62, 783088.25, 780652.88, 780950.25, 779975.12, 778808.25, 779305.50, 776662.25, 778643.62, 774512.25, 777989.12, 772358.75, 777341.88, 770201.75, 776701.88, 768041.25, 776068.62, 765877.25, 775442.12, 763710.25, 774822.00, 761539.75, 774208.12, 759366.25, 773600.12, 757190.00, 772998.12, 755010.75, 772401.62, 752828.75, 771810.38, 750644.25, 771224.38, 748457.25, 770643.38, 746267.75, 770067.12, 744076.25, 769495.25, 738483.00, 768141.00, 748268.00, 763625.50, 745247.50, 776573.50, 738483.00, 768141.00
		};

		void UBaseView::HideAllPath()
		{
			WndCtx * pWndCtx = GetWndContext(m_hWnd);
			if (pWndCtx != NULL)
			{
				Ogre::SceneNode* node = pWndCtx->sceneMgr->getSceneNode("MissionPath3");
				if (node != NULL)
				{
					node->setVisible(false);
				}
				Ogre::SceneNode* node2 = pWndCtx->sceneMgr->getSceneNode("MissionPath2");
				if (node2 != NULL)
				{
					node2->setVisible(false);
				}
				Ogre::SceneNode* node3 = pWndCtx->sceneMgr->getSceneNode("MissionPath1");
				if (node3 != NULL)
				{
					node3->setVisible(false);
				}
			}
		}

		void UBaseView::ShowPath(int nType)
		{
			WndCtx * pWndCtx = GetWndContext(m_hWnd);
			if (pWndCtx != NULL)
			{
				if (nType == 1 || nType == 3)
				{
					Ogre::SceneNode* node = pWndCtx->sceneMgr->getSceneNode("MissionPath1");
					if (node != NULL)
					{
						node->setVisible(true);
					}
				}
				if (nType == 2)
				{
					Ogre::SceneNode* node = pWndCtx->sceneMgr->getSceneNode("MissionPath2");
					if (node != NULL)
					{
						node->setVisible(true);
					}
				}
				if (nType == 3)
				{
					Ogre::SceneNode* node = pWndCtx->sceneMgr->getSceneNode("MissionPath3");
					if (node != NULL)
					{
						node->setVisible(true);
					}
				}				
			}
		}

		void UBaseView::CreatePath()
		{
			WndCtx * pWndCtx = GetWndContext(m_hWnd);
			if (pWndCtx != NULL)
			{

				int mID = UDB::GetNextCookie();
				if (!MaterialManager::getSingleton().resourceExists("AlphaMaterial"))
				{
					MaterialPtr material = MaterialManager::getSingleton().create("AlphaMaterial", "General");
					material->getTechnique(0)->getPass(0)->setLightingEnabled(false);
					material->getTechnique(0)->getPass(0)->setCullingMode(CULL_NONE);
					material->getTechnique(0)->getPass(0)->setSceneBlending(SBF_SOURCE_ALPHA, SBF_ONE_MINUS_SOURCE_ALPHA);
					material->setReceiveShadows(false);
				}

				////////////////////////////////////////////////////////////////////////////////////
				// type 3
				Ogre::SceneNode * mpMissionPathNode = pWndCtx->sceneMgr->getRootSceneNode()->createChildSceneNode("MissionPath3");

				ManualObject *mpRepetPathObject = pWndCtx->sceneMgr->createManualObject("RepetPathObject");
				mpRepetPathObject->begin("AlphaMaterial", RenderOperation::OT_TRIANGLE_LIST);
				mpRepetPathObject->position(0, 0, 0);
				mpRepetPathObject->colour(1, 1, 1);
				mpRepetPathObject->position(100, 0, 0);
				mpRepetPathObject->colour(1, 1, 1);
				mpRepetPathObject->position(100, 0, 100);
				mpRepetPathObject->colour(1, 1, 1);
				mpRepetPathObject->triangle(0, 1, 2);
				mpRepetPathObject->end();
				mpRepetPathObject->setVisible(false);
				mpRepetPathObject->setCastShadows(false);
				mpRepetPathObject->setRenderQueueGroup(90);
				mpMissionPathNode->attachObject(mpRepetPathObject);

				mpRepetPathObject->beginUpdate(0);

				Ogre::ColourValue mRepetPathColor = Ogre::ColourValue(1.0f, 0.0f, 0.0f);
				float rad = 2.0f;
				int count = 0;
				int nArrCount = sizeof(ddd) / sizeof(float);
				for (int i = 2; i < nArrCount; i += 2)
				{
					float tx1 = ddd[i - 2] / 1000.0f;
					float ty1 = 20.0f;
					float tz1 = -ddd[i - 1] / 1000.0f;

					float tx2 = ddd[i] / 1000.0f;
					float ty2 = 20.0f;
					float tz2 = -ddd[i + 1] / 1000.0f;
					Vector3 vStart = Vector3(tx1, ty1, tz1);
					Vector3 vEnd = Vector3(tx2, ty2, tz2);
					Vector3 vCross = (vEnd - vStart).normalisedCopy().crossProduct(Vector3::UNIT_Y) * rad;

					mpRepetPathObject->position(vStart + vCross + Vector3::UNIT_Y * 11);
					mpRepetPathObject->colour(mRepetPathColor);
					mpRepetPathObject->position(vStart - vCross + Vector3::UNIT_Y * 11);
					mpRepetPathObject->colour(mRepetPathColor);
					mpRepetPathObject->position(vEnd - vCross + Vector3::UNIT_Y * 11);
					mpRepetPathObject->colour(mRepetPathColor);
					mpRepetPathObject->position(vEnd + vCross + Vector3::UNIT_Y * 11);
					mpRepetPathObject->colour(mRepetPathColor);
					count += 4;
					mpRepetPathObject->index(count - 4);
					mpRepetPathObject->index(count - 3);
					mpRepetPathObject->index(count - 2);
					mpRepetPathObject->index(count - 4);
					mpRepetPathObject->index(count - 2);
					mpRepetPathObject->index(count - 1);
				}
				mpRepetPathObject->end();
				mpRepetPathObject->setVisible(true);
				mpMissionPathNode->setVisible(false);

				////////////////////////////////////////////////////////////////////////////////////
				// type1
				Ogre::SceneNode * mpMissionPathNode2 = pWndCtx->sceneMgr->getRootSceneNode()->createChildSceneNode("MissionPath1");

				ManualObject *mpRepetPathObject2 = pWndCtx->sceneMgr->createManualObject("RepetPathObject2");
				mpRepetPathObject2->begin("AlphaMaterial", RenderOperation::OT_TRIANGLE_LIST);
				mpRepetPathObject2->position(0, 0, 0);
				mpRepetPathObject2->colour(1, 1, 1);
				mpRepetPathObject2->position(100, 0, 0);
				mpRepetPathObject2->colour(1, 1, 1);
				mpRepetPathObject2->position(100, 0, 100);
				mpRepetPathObject2->colour(1, 1, 1);
				mpRepetPathObject2->triangle(0, 1, 2);
				mpRepetPathObject2->end();
				mpRepetPathObject2->setVisible(false);
				mpRepetPathObject2->setCastShadows(false);
				mpRepetPathObject2->setRenderQueueGroup(90);
				mpMissionPathNode2->attachObject(mpRepetPathObject2);

				mpRepetPathObject2->beginUpdate(0);

				count = 0;
				nArrCount = sizeof(ddd2) / sizeof(float);
				for (int i = 2; i < nArrCount; i += 2)
				{
					float tx1 = ddd2[i - 2] / 1000.0f;
					float ty1 = 20.0f;
					float tz1 = -ddd2[i - 1] / 1000.0f;

					float tx2 = ddd2[i] / 1000.0f;
					float ty2 = 20.0f;
					float tz2 = -ddd2[i + 1] / 1000.0f;
					Vector3 vStart = Vector3(tx1, ty1, tz1);
					Vector3 vEnd = Vector3(tx2, ty2, tz2);
					Vector3 vCross = (vEnd - vStart).normalisedCopy().crossProduct(Vector3::UNIT_Y) * rad;

					mpRepetPathObject2->position(vStart + vCross + Vector3::UNIT_Y * 11);
					mpRepetPathObject2->colour(mRepetPathColor);
					mpRepetPathObject2->position(vStart - vCross + Vector3::UNIT_Y * 11);
					mpRepetPathObject2->colour(mRepetPathColor);
					mpRepetPathObject2->position(vEnd - vCross + Vector3::UNIT_Y * 11);
					mpRepetPathObject2->colour(mRepetPathColor);
					mpRepetPathObject2->position(vEnd + vCross + Vector3::UNIT_Y * 11);
					mpRepetPathObject2->colour(mRepetPathColor);
					count += 4;
					mpRepetPathObject2->index(count - 4);
					mpRepetPathObject2->index(count - 3);
					mpRepetPathObject2->index(count - 2);
					mpRepetPathObject2->index(count - 4);
					mpRepetPathObject2->index(count - 2);
					mpRepetPathObject2->index(count - 1);
				}
				mpRepetPathObject2->end();
				mpRepetPathObject2->setVisible(true);


				ManualObject *mpRepetPathObject3 = pWndCtx->sceneMgr->createManualObject("RepetPathObject3");
				mpRepetPathObject3->begin("AlphaMaterial", RenderOperation::OT_TRIANGLE_LIST);
				mpRepetPathObject3->position(0, 0, 0);
				mpRepetPathObject3->colour(1, 1, 1);
				mpRepetPathObject3->position(100, 0, 0);
				mpRepetPathObject3->colour(1, 1, 1);
				mpRepetPathObject3->position(100, 0, 100);
				mpRepetPathObject3->colour(1, 1, 1);
				mpRepetPathObject3->triangle(0, 1, 2);
				mpRepetPathObject3->end();
				mpRepetPathObject3->setVisible(false);
				mpRepetPathObject3->setCastShadows(false);
				mpRepetPathObject3->setRenderQueueGroup(90);
				mpMissionPathNode2->attachObject(mpRepetPathObject3);

				mpRepetPathObject3->beginUpdate(0);

				count = 0;
				nArrCount = sizeof(ddd3) / sizeof(float);
				for (int i = 2; i < nArrCount; i += 2)
				{
					float tx1 = ddd3[i - 2] / 1000.0f;
					float ty1 = 20.0f;
					float tz1 = -ddd3[i - 1] / 1000.0f;

					float tx2 = ddd3[i] / 1000.0f;
					float ty2 = 20.0f;
					float tz2 = -ddd3[i + 1] / 1000.0f;
					Vector3 vStart = Vector3(tx1, ty1, tz1);
					Vector3 vEnd = Vector3(tx2, ty2, tz2);
					Vector3 vCross = (vEnd - vStart).normalisedCopy().crossProduct(Vector3::UNIT_Y) * rad;

					mpRepetPathObject3->position(vStart + vCross + Vector3::UNIT_Y * 11);
					mpRepetPathObject3->colour(mRepetPathColor);
					mpRepetPathObject3->position(vStart - vCross + Vector3::UNIT_Y * 11);
					mpRepetPathObject3->colour(mRepetPathColor);
					mpRepetPathObject3->position(vEnd - vCross + Vector3::UNIT_Y * 11);
					mpRepetPathObject3->colour(mRepetPathColor);
					mpRepetPathObject3->position(vEnd + vCross + Vector3::UNIT_Y * 11);
					mpRepetPathObject3->colour(mRepetPathColor);
					count += 4;
					mpRepetPathObject3->index(count - 4);
					mpRepetPathObject3->index(count - 3);
					mpRepetPathObject3->index(count - 2);
					mpRepetPathObject3->index(count - 4);
					mpRepetPathObject3->index(count - 2);
					mpRepetPathObject3->index(count - 1);
				}
				mpRepetPathObject3->end();
				mpRepetPathObject3->setVisible(true);

				ManualObject *mpRepetPathObject4 = pWndCtx->sceneMgr->createManualObject("RepetPathObject4");
				mpRepetPathObject4->begin("AlphaMaterial", RenderOperation::OT_TRIANGLE_LIST);
				mpRepetPathObject4->position(0, 0, 0);
				mpRepetPathObject4->colour(1, 1, 1);
				mpRepetPathObject4->position(100, 0, 0);
				mpRepetPathObject4->colour(1, 1, 1);
				mpRepetPathObject4->position(100, 0, 100);
				mpRepetPathObject4->colour(1, 1, 1);
				mpRepetPathObject4->triangle(0, 1, 2);
				mpRepetPathObject4->end();
				mpRepetPathObject4->setVisible(false);
				mpRepetPathObject4->setCastShadows(false);
				mpRepetPathObject4->setRenderQueueGroup(90);
				mpMissionPathNode2->attachObject(mpRepetPathObject4);

				mpRepetPathObject4->beginUpdate(0);

				count = 0;
				nArrCount = sizeof(ddd4) / sizeof(float);
				for (int i = 2; i < nArrCount; i += 2)
				{
					float tx1 = ddd4[i - 2] / 1000.0f;
					float ty1 = 20.0f;
					float tz1 = -ddd4[i - 1] / 1000.0f;

					float tx2 = ddd4[i] / 1000.0f;
					float ty2 = 20.0f;
					float tz2 = -ddd4[i + 1] / 1000.0f;
					Vector3 vStart = Vector3(tx1, ty1, tz1);
					Vector3 vEnd = Vector3(tx2, ty2, tz2);
					Vector3 vCross = (vEnd - vStart).normalisedCopy().crossProduct(Vector3::UNIT_Y) * rad;

					mpRepetPathObject4->position(vStart + vCross + Vector3::UNIT_Y * 11);
					mpRepetPathObject4->colour(mRepetPathColor);
					mpRepetPathObject4->position(vStart - vCross + Vector3::UNIT_Y * 11);
					mpRepetPathObject4->colour(mRepetPathColor);
					mpRepetPathObject4->position(vEnd - vCross + Vector3::UNIT_Y * 11);
					mpRepetPathObject4->colour(mRepetPathColor);
					mpRepetPathObject4->position(vEnd + vCross + Vector3::UNIT_Y * 11);
					mpRepetPathObject4->colour(mRepetPathColor);
					count += 4;
					mpRepetPathObject4->index(count - 4);
					mpRepetPathObject4->index(count - 3);
					mpRepetPathObject4->index(count - 2);
					mpRepetPathObject4->index(count - 4);
					mpRepetPathObject4->index(count - 2);
					mpRepetPathObject4->index(count - 1);
				}
				mpRepetPathObject4->end();
				mpRepetPathObject4->setVisible(true);

				ManualObject *mpRepetPathObject5 = pWndCtx->sceneMgr->createManualObject("RepetPathObject5");
				mpRepetPathObject5->begin("AlphaMaterial", RenderOperation::OT_TRIANGLE_LIST);
				mpRepetPathObject5->position(0, 0, 0);
				mpRepetPathObject5->colour(1, 1, 1);
				mpRepetPathObject5->position(100, 0, 0);
				mpRepetPathObject5->colour(1, 1, 1);
				mpRepetPathObject5->position(100, 0, 100);
				mpRepetPathObject5->colour(1, 1, 1);
				mpRepetPathObject5->triangle(0, 1, 2);
				mpRepetPathObject5->end();
				mpRepetPathObject5->setVisible(false);
				mpRepetPathObject5->setCastShadows(false);
				mpRepetPathObject5->setRenderQueueGroup(90);
				mpMissionPathNode2->attachObject(mpRepetPathObject5);

				mpRepetPathObject5->beginUpdate(0);

				count = 0;
				nArrCount = sizeof(ddd5) / sizeof(float);
				for (int i = 2; i < nArrCount; i += 2)
				{
					float tx1 = ddd5[i - 2] / 1000.0f;
					float ty1 = 20.0f;
					float tz1 = -ddd5[i - 1] / 1000.0f;

					float tx2 = ddd5[i] / 1000.0f;
					float ty2 = 20.0f;
					float tz2 = -ddd5[i + 1] / 1000.0f;
					Vector3 vStart = Vector3(tx1, ty1, tz1);
					Vector3 vEnd = Vector3(tx2, ty2, tz2);
					Vector3 vCross = (vEnd - vStart).normalisedCopy().crossProduct(Vector3::UNIT_Y) * rad;

					mpRepetPathObject5->position(vStart + vCross + Vector3::UNIT_Y * 11);
					mpRepetPathObject5->colour(mRepetPathColor);
					mpRepetPathObject5->position(vStart - vCross + Vector3::UNIT_Y * 11);
					mpRepetPathObject5->colour(mRepetPathColor);
					mpRepetPathObject5->position(vEnd - vCross + Vector3::UNIT_Y * 11);
					mpRepetPathObject5->colour(mRepetPathColor);
					mpRepetPathObject5->position(vEnd + vCross + Vector3::UNIT_Y * 11);
					mpRepetPathObject5->colour(mRepetPathColor);
					count += 4;
					mpRepetPathObject5->index(count - 4);
					mpRepetPathObject5->index(count - 3);
					mpRepetPathObject5->index(count - 2);
					mpRepetPathObject5->index(count - 4);
					mpRepetPathObject5->index(count - 2);
					mpRepetPathObject5->index(count - 1);
				}
				mpRepetPathObject5->end();
				mpRepetPathObject5->setVisible(true);

				mpMissionPathNode2->setVisible(false);


				float type21[] = {
					414991.75, 356185.00, 415187.00, 357739.50, 415382.25, 359294.12, 415577.25, 360848.75, 415772.25, 362403.50, 415967.25, 363958.44, 416161.75, 365513.56, 416356.00, 367068.94, 416550.00, 368624.56, 416743.75, 370180.50, 416936.75, 371736.75, 417129.50, 373293.50, 417321.75, 374850.56, 417513.25, 376408.12, 417704.25, 377966.25, 417894.50, 379524.88, 418084.25, 381084.12, 418273.00, 382643.88, 418461.25, 384204.44, 418648.25, 385765.62, 418834.75, 387327.62, 419020.00, 388890.31, 419204.25, 390453.88, 419387.75, 392018.25, 419570.00, 393583.56, 419751.00, 395149.81, 419931.00, 396717.00, 420109.75, 398285.25, 420287.00, 399854.50, 420463.25, 401424.88, 420637.75, 402996.44, 420811.25, 404569.12, 420983.00, 406143.00, 421153.25, 407718.12, 421322.00, 409294.56, 421489.25, 410872.31, 421654.75, 412451.44, 421818.50, 414032.00, 421980.50, 415613.94, 422140.75, 417197.38, 422299.25, 418782.38, 422455.75, 420368.88, 422610.25, 421957.00, 422763.00, 423546.75, 422913.50, 425138.25, 423062.00, 426731.38, 423208.25, 428326.38, 423352.50, 429923.06, 423494.25, 431521.62, 423634.00, 433122.00, 423771.25, 434724.38, 423906.25, 436328.62, 424039.00, 437934.94, 424169.00, 439543.25, 424296.75, 441153.62, 424421.75, 442766.12, 424544.25, 444380.75, 424664.00, 445997.56, 424781.00, 447616.62, 424895.50, 449237.88, 425007.00, 450861.50, 425115.75, 452487.50, 425221.75, 454115.75, 425324.50, 455746.50, 425424.50, 457379.75, 425521.25, 459015.44, 425615.25, 460653.69, 425706.00, 462294.50, 425793.50, 463937.94, 425877.75, 465584.00, 425958.75, 467232.75, 426036.75, 468884.31, 426111.00, 470538.62, 426182.00, 472195.62, 426249.50, 473855.62, 426313.50, 475518.44, 426374.00, 477184.19, 426431.00, 478852.88, 426484.25, 480524.62, 426533.75, 482199.38, 426579.75, 483877.25, 426621.75, 485558.25, 426659.75, 487242.38, 426694.25, 488929.69, 426724.75, 490620.25, 426751.00, 492314.12, 426773.50, 494011.25, 426791.75, 495711.81, 426806.00, 497415.75, 426816.25, 499123.06, 426822.00, 500833.88, 426823.50, 502548.25, 426820.75, 504266.12, 426813.75, 505987.62, 426802.25, 507712.75, 426786.50, 509441.50, 426766.00, 511174.00, 426741.25, 512910.19, 426711.50, 514650.25, 426677.50, 516394.06, 426638.75, 518141.75, 426595.00, 519893.38, 426546.75, 521648.88, 426493.50, 523408.44, 426435.50, 525172.00, 426372.50, 526939.56, 426304.50, 528711.31, 426231.75, 530487.12, 426153.75, 532267.12, 426070.50, 534051.38, 425982.25, 535839.88, 425888.75, 537632.62, 425790.00, 539429.75, 425686.00, 541231.25, 425576.75, 543037.12, 425462.00, 544847.44, 425341.75, 546662.25, 425216.00, 548481.62, 425084.75, 550305.56, 424948.00, 552134.06, 424805.50, 553967.25, 424657.50, 555805.06, 424503.75, 557647.69, 424344.00, 559495.00, 424178.75, 561347.12, 424007.25, 563204.12, 423830.25, 565065.94, 423647.00, 566932.69, 423457.75, 568804.44, 423262.75, 570681.12, 423061.25, 572562.88, 422853.75, 574449.75, 422640.25, 576341.69, 422420.25, 578238.75, 422194.00, 580141.00, 421961.50, 582048.56, 421722.75, 583961.31, 421477.25, 585879.38, 421225.50, 587802.81, 420967.00, 589731.62, 420702.25, 591665.88, 420430.50, 593605.56, 420152.25, 595550.81, 419867.50, 597501.56, 419575.75, 599457.88, 419277.25, 601419.81, 418971.75, 603387.44, 418659.50, 605360.69, 418340.25, 607339.75, 418014.00, 609324.56, 417680.75, 611315.19, 417340.50, 613311.69, 416992.75, 615314.06, 416638.25, 617322.38, 416276.25, 619336.62, 415935.25, 621203.88, 415530.75, 623382.94, 415149.00, 625413.56, 414763.50, 627447.25, 414375.75, 629482.69, 413987.75, 631518.44, 413600.75, 633553.12, 413216.50, 635585.31, 412837.00, 637613.62, 412463.75, 639636.62, 412098.50, 641652.94, 411742.75, 643661.19, 411398.50, 645659.88, 411067.00, 647647.69, 410750.25, 649623.19, 410450.00, 651585.00, 410167.75, 653531.62, 409905.00, 655461.81, 409663.75, 657374.00, 409445.75, 659266.88, 409252.25, 661139.06, 409085.50, 662989.06, 408946.75, 664815.50, 408837.75, 666617.06, 408760.25, 668392.25, 408715.75, 670139.69, 408706.25, 671857.94, 408733.50, 673545.62, 408798.75, 675201.38, 408904.00, 676823.75, 409050.75, 678411.38, 409241.00, 679962.81, 409476.00, 681476.62, 409757.50, 682951.50, 410087.50, 684385.94, 410467.50, 685778.62, 410899.25, 687128.12, 411384.25, 688433.00, 411924.25, 689691.88, 412521.25, 690903.38, 413176.25, 692066.00, 413891.75, 693178.50, 414668.75, 694239.31, 415935.25, 695710.94, 416415.00, 696200.56, 417385.25, 697099.69, 418418.50, 697946.00, 419512.50, 698740.94, 420665.25, 699485.88, 421874.75, 700182.38, 423138.75, 700831.88, 424455.50, 701435.88, 425822.75, 701995.88, 427238.25, 702513.25, 428700.25, 702989.50, 430206.75, 703426.12, 431755.50, 703824.50, 433344.50, 704186.25, 434971.75, 704512.75, 436635.00, 704805.38, 438332.50, 705065.88, 440062.25, 705295.38, 441821.75, 705495.62, 443609.25, 705668.00, 445422.75, 705813.88, 447260.25, 705934.88, 449119.25, 706032.38, 450998.25, 706107.75, 452894.75, 706162.62, 454807.25, 706198.50, 456733.00, 706216.62, 458670.50, 706218.75, 460617.50, 706206.12, 462572.00, 706180.38, 464531.75, 706142.88, 466495.00, 706094.88, 468459.50, 706038.38, 470423.25, 705974.38, 472384.00, 705904.62, 474340.25, 705830.38, 476289.50, 705753.38, 478229.75, 705674.75, 480159.00, 705596.25, 482075.00, 705519.12, 483976.25, 705445.12, 485860.25, 705375.38, 487725.00, 705311.62, 489568.25, 705255.12, 491388.50, 705207.62, 493183.50, 705170.25, 495194.00, 705142.12, 496689.75, 705131.88, 498405.75, 705129.62, 500106.25, 705134.12, 495118.25, 709152.38, 495092.25, 701359.62, 500106.25, 705134.12
				};
				float type22[] = {
					73279.50, 690036.19, 76154.00, 689952.94, 79028.25, 689869.75, 81902.50, 689786.62, 84776.75, 689703.62, 87651.00, 689620.75, 90525.25, 689538.06, 93399.50, 689455.62, 96273.75, 689373.44, 99148.00, 689291.56, 102022.25, 689210.00, 104896.50, 689128.81, 107770.75, 689048.06, 110644.75, 688967.75, 113519.00, 688887.94, 116393.00, 688808.62, 119267.25, 688729.88, 122141.25, 688651.75, 125015.25, 688574.25, 127889.25, 688497.44, 130763.25, 688421.31, 133637.25, 688345.94, 136511.25, 688271.38, 139385.00, 688197.62, 142259.00, 688124.75, 145132.75, 688052.81, 148006.50, 687981.75, 150880.25, 687911.69, 153754.00, 687842.62, 156627.75, 687774.62, 159501.25, 687707.75, 162374.75, 687641.94, 165248.25, 687577.31, 168121.75, 687513.94, 170995.25, 687451.75, 173868.50, 687390.81, 176741.75, 687331.25, 179615.00, 687273.00, 182488.25, 687216.19, 185361.25, 687160.75, 188234.50, 687106.81, 191107.50, 687054.31, 193980.25, 687003.44, 196853.25, 686954.06, 199726.00, 686906.38, 202598.75, 686860.31, 205471.50, 686815.88, 208344.00, 686773.25, 211216.50, 686732.38, 214089.00, 686693.25, 216961.25, 686656.00, 219833.50, 686620.62, 222705.75, 686587.19, 225578.00, 686555.69, 228450.00, 686526.19, 231322.00, 686498.69, 234193.75, 686473.25, 237065.50, 686449.94, 239937.25, 686428.75, 242808.75, 686409.75, 245680.25, 686392.94, 248551.75, 686378.44, 251423.00, 686366.19, 254294.25, 686356.25, 257165.25, 686348.69, 260036.25, 686343.56, 262907.25, 686340.88, 265778.00, 686340.62, 268648.75, 686342.88, 271519.25, 686347.75, 274389.75, 686355.19, 277260.25, 686365.19, 280130.50, 686377.94, 283000.75, 686393.38, 285870.75, 686411.50, 288740.50, 686432.50, 291610.25, 686456.25, 294480.00, 686482.81, 297349.75, 686512.38, 300219.00, 686544.75, 303088.25, 686580.19, 305957.50, 686618.56, 308826.50, 686660.00, 311695.50, 686704.56, 314564.25, 686752.19, 317433.00, 686802.94, 320301.50, 686856.94, 323170.00, 686914.19, 326038.25, 686974.62, 328906.25, 687038.44, 331774.25, 687105.56, 334642.25, 687176.06, 337509.75, 687249.94, 340377.25, 687327.31, 343244.75, 687408.19, 346112.00, 687492.56, 348979.25, 687580.56, 351846.00, 687672.12, 354712.75, 687767.31, 357579.50, 687866.19, 360446.00, 687968.81, 363312.25, 688075.12, 366178.50, 688185.31, 369044.50, 688299.25, 371910.25, 688417.12, 374776.00, 688538.88, 377641.50, 688664.56, 380507.00, 688794.25, 383372.25, 688927.94, 386237.25, 689065.69, 389102.00, 689207.50, 391966.75, 689353.44, 394831.25, 689503.56, 397695.50, 689657.94, 400559.50, 689816.50, 403423.50, 689979.38, 406287.25, 690146.56, 409151.00, 690318.06, 412014.25, 690494.00, 414877.50, 690674.38, 417740.50, 690859.19, 419565.50, 690979.31, 423466.25, 691242.31, 426328.50, 691440.38, 429191.00, 691642.62, 432053.00, 691848.88, 434915.00, 692058.94, 437776.75, 692272.69, 440638.50, 692490.00, 443500.00, 692710.56, 446361.50, 692934.38, 449222.75, 693161.25, 452083.75, 693390.94, 454945.00, 693623.38, 457805.75, 693858.38, 460666.75, 694095.75, 463527.50, 694335.38, 466388.00, 694577.06, 469248.75, 694820.69, 472109.25, 695066.06, 474969.50, 695313.00, 477830.00, 695561.44, 480690.25, 695811.12, 483550.50, 696061.94, 486410.75, 696313.69, 489271.00, 696566.19, 492131.25, 696819.44, 494991.25, 697073.06, 497851.50, 697327.06, 500711.50, 697581.25, 496304.75, 700550.62, 497031.75, 693813.06, 500711.50, 697581.25
				};
				float type23[] = {
					1304984.25, 452227.88, 1305248.50, 456632.75, 1305511.75, 461036.81, 1305771.75, 465439.00, 1306028.25, 469838.50, 1306279.25, 474234.25, 1306523.50, 478625.50, 1306759.75, 483011.25, 1306986.25, 487390.62, 1307202.50, 491762.69, 1307406.25, 496126.50, 1307597.00, 500481.19, 1307772.75, 504825.88, 1307932.25, 509159.50, 1308074.50, 513481.31, 1308197.75, 517790.31, 1308301.00, 522085.62, 1308382.75, 526366.25, 1308441.75, 530631.44, 1308476.25, 534880.12, 1308485.50, 539111.44, 1308467.75, 543324.50, 1308422.00, 547518.38, 1308346.50, 551692.12, 1308240.25, 555844.88, 1308101.75, 559975.69, 1307929.50, 564083.62, 1307722.50, 568167.88, 1307479.25, 572227.44, 1307198.25, 576261.38, 1306878.25, 580268.81, 1306518.25, 584248.88, 1306116.25, 588200.62, 1305671.50, 592123.06, 1305182.25, 596015.44, 1304647.50, 599876.69, 1304065.75, 603705.94, 1303435.50, 607502.31, 1302755.75, 611264.88, 1302024.75, 614992.75, 1301241.50, 618684.94, 1300404.50, 622340.62, 1299512.25, 625958.81, 1298563.75, 629538.62, 1297557.75, 633079.19, 1296492.25, 636579.50, 1295366.50, 640038.69, 1294178.75, 643455.88, 1292928.25, 646830.12, 1291613.00, 650160.50, 1290232.00, 653446.06, 1288783.75, 656686.00, 1287267.25, 659879.31, 1285680.75, 663025.06, 1283946.75, 666260.38, 1282293.00, 669170.62, 1280491.25, 672169.94, 1278619.25, 675120.75, 1276677.00, 678023.56, 1274666.25, 680878.81, 1272587.25, 683686.94, 1270441.75, 686448.56, 1268229.75, 689163.94, 1265953.25, 691833.69, 1263612.50, 694458.25, 1261208.75, 697038.06, 1258742.75, 699573.62, 1256215.25, 702065.25, 1253627.75, 704513.62, 1250981.25, 706919.12, 1248276.00, 709282.12, 1245513.50, 711603.12, 1242694.50, 713882.88, 1239820.25, 716121.38, 1236891.25, 718319.38, 1233908.75, 720477.38, 1230873.25, 722595.62, 1227786.50, 724674.88, 1224648.75, 726715.38, 1221461.50, 728717.62, 1218225.25, 730682.12, 1214941.25, 732609.25, 1211610.25, 734499.62, 1208233.25, 736353.62, 1204811.25, 738171.75, 1201345.25, 739954.38, 1197836.25, 741702.12, 1194285.00, 743415.38, 1190692.50, 745094.50, 1187059.75, 746740.12, 1183387.75, 748352.62, 1179677.25, 749932.62, 1175929.75, 751480.25, 1172145.50, 752996.25, 1168325.75, 754481.00, 1164471.75, 755935.00, 1160583.75, 757358.75, 1156663.25, 758752.62, 1152711.25, 760117.12, 1148728.50, 761452.62, 1144716.00, 762759.75, 1140674.50, 764038.88, 1136605.25, 765290.62, 1132509.25, 766515.12, 1128387.00, 767713.12, 1124239.75, 768885.12, 1120068.75, 770031.38, 1115874.25, 771152.38, 1111657.75, 772248.75, 1107420.25, 773320.88, 1103162.25, 774369.12, 1098885.25, 775394.12, 1094589.75, 776396.38, 1090276.75, 777376.12, 1085947.25, 778333.88, 1081602.75, 779270.25, 1077243.25, 780185.62, 1072870.25, 781080.62, 1068484.75, 781955.38, 1064087.50, 782810.62, 1059679.75, 783646.62, 1055261.75, 784464.12, 1050835.25, 785263.38, 1046401.00, 786044.88, 1041959.75, 786809.12, 1037512.50, 787556.62, 1033060.25, 788287.62, 1028604.00, 789002.88, 1024144.50, 789702.75, 1019683.00, 790387.62, 1015220.25, 791058.12, 1010757.25, 791714.62, 1006295.00, 792357.50, 1001834.25, 792987.38, 997376.50, 793604.62, 992922.00, 794209.62, 988472.25, 794803.12, 984027.75, 795385.38, 979589.75, 795956.75, 975159.00, 796517.88, 970736.75, 797069.38, 966323.75, 797611.38, 961921.00, 798144.62, 957529.50, 798669.38, 953150.00, 799186.12, 948783.75, 799695.38, 944431.50, 800197.75, 940094.25, 800693.50, 935772.75, 801183.12, 931468.50, 801667.25, 925504.25, 802332.88, 922914.00, 802620.38, 918665.25, 803090.12, 914434.75, 803555.62, 910222.75, 804016.62, 906028.25, 804473.62, 901851.25, 804926.62, 897691.25, 805375.50, 893547.75, 805820.50, 889420.25, 806261.75, 885308.75, 806699.25, 881212.75, 807133.12, 877131.50, 807563.62, 873065.00, 807990.50, 869012.75, 808414.12, 864974.25, 808834.50, 860949.25, 809251.75, 856937.00, 809665.88, 852937.75, 810077.12, 848950.25, 810485.38, 844975.00, 810891.00, 841011.25, 811293.88, 837058.25, 811694.12, 833116.00, 812091.88, 829184.25, 812487.38, 825262.00, 812880.38, 821349.50, 813271.12, 817445.75, 813659.88, 813551.00, 814046.62, 809664.50, 814431.25, 805785.75, 814814.12, 801914.75, 815195.25, 798050.75, 815574.62, 794193.25, 815952.50, 790342.25, 816328.88, 786497.25, 816703.88, 782657.50, 817077.62, 778823.00, 817450.12, 774993.25, 817821.38, 771168.00, 818191.88, 767346.50, 818561.25, 763528.75, 818929.88, 759713.75, 819297.62, 755902.00, 819664.88, 752092.25, 820031.50, 748284.75, 820397.62, 744478.75, 820763.62, 740673.75, 821129.12, 754439.25, 828911.25, 752168.00, 811256.88, 740673.75, 821129.12
				};
				float type24[] = {
					1854641.25, 651852.62, 1848908.75, 651386.38, 1843176.75, 650920.50, 1837444.75, 650455.25, 1831712.75, 649991.00, 1825980.75, 649528.00, 1820249.25, 649066.56, 1814517.75, 648607.00, 1808786.75, 648149.75, 1803055.75, 647695.00, 1797325.75, 647243.12, 1791595.25, 646794.38, 1785865.75, 646349.19, 1780136.75, 645907.81, 1774407.75, 645470.56, 1768679.75, 645037.75, 1762951.75, 644609.75, 1757224.75, 644186.75, 1751498.75, 643769.25, 1745772.75, 643357.44, 1740048.25, 642951.69, 1734323.75, 642552.25, 1728600.75, 642159.56, 1722878.25, 641773.81, 1717156.75, 641395.38, 1711435.75, 641024.62, 1705716.25, 640661.81, 1699997.25, 640307.25, 1694279.75, 639961.25, 1688562.75, 639624.19, 1682847.75, 639296.38, 1677133.25, 638978.06, 1671419.75, 638669.62, 1665707.75, 638371.31, 1659997.25, 638083.56, 1654287.75, 637806.62, 1648579.75, 637540.81, 1642873.25, 637286.44, 1637167.75, 637043.81, 1631463.75, 636813.31, 1625761.75, 636595.19, 1620060.75, 636389.81, 1614361.75, 636197.44, 1608664.25, 636018.44, 1602968.25, 635853.12, 1597273.75, 635701.81, 1591581.75, 635564.81, 1585890.75, 635442.44, 1580201.75, 635335.00, 1574514.75, 635242.88, 1568829.25, 635166.25, 1563146.25, 635105.62, 1557464.75, 635061.12, 1551785.25, 635033.25, 1546108.00, 635022.19, 1540432.75, 635028.31, 1534759.50, 635051.88, 1529088.50, 635093.31, 1523419.75, 635152.88, 1517753.25, 635230.88, 1512088.75, 635327.62, 1506426.75, 635443.50, 1500767.00, 635578.75, 1495109.75, 635733.69, 1489454.75, 635908.69, 1483802.25, 636104.06, 1478152.25, 636320.12, 1472504.75, 636557.12, 1466860.00, 636815.50, 1461217.75, 637095.44, 1455578.25, 637397.38, 1449941.25, 637721.50, 1444306.75, 638068.25, 1438675.50, 638437.94, 1433046.75, 638830.81, 1427421.00, 639247.25, 1421798.00, 639687.50, 1416178.00, 640151.94, 1410560.75, 640640.88, 1404946.75, 641154.56, 1399335.75, 641693.44, 1393727.75, 642257.75, 1388122.75, 642847.81, 1382521.25, 643464.00, 1376922.75, 644106.56, 1371327.25, 644775.81, 1365735.25, 645472.12, 1360146.25, 646195.75, 1354560.75, 646947.06, 1348978.75, 647726.38, 1343400.25, 648534.00, 1337824.75, 649370.25, 1332253.25, 650235.44, 1326685.00, 651129.88, 1321120.25, 652053.94, 1315559.25, 653007.88, 1310001.75, 653992.00, 1304448.25, 655006.69, 1300966.75, 655658.81, 1293351.75, 657128.12, 1287809.00, 658233.06, 1282269.25, 659365.56, 1276732.75, 660524.06, 1271199.25, 661707.12, 1265668.25, 662913.25, 1260139.75, 664140.94, 1254613.75, 665388.75, 1249089.75, 666655.12, 1243567.50, 667938.62, 1238047.00, 669237.81, 1232528.25, 670551.06, 1227010.25, 671877.06, 1221493.75, 673214.19, 1215978.25, 674561.00, 1210463.25, 675916.00, 1204949.00, 677277.75, 1199435.00, 678644.69, 1193921.00, 680015.38, 1188407.25, 681388.38, 1182892.75, 682762.06, 1177378.25, 684135.06, 1171862.75, 685505.88, 1166346.50, 686873.00, 1160829.25, 688234.94, 1155310.75, 689590.19, 1149790.75, 690937.31, 1144269.25, 692274.81, 1138745.75, 693601.19, 1133220.25, 694914.94, 1127692.75, 696214.56, 1122162.50, 697498.62, 1116629.75, 698765.62, 1111094.25, 700014.12, 1105555.50, 701242.50, 1100013.75, 702449.38, 1094468.50, 703633.25, 1088919.75, 704792.62, 1083367.25, 705926.00, 1077810.50, 707031.88, 1072249.75, 708108.88, 1067715.50, 708963.88, 1061114.75, 710170.38, 1055540.50, 711154.50, 1049962.00, 712108.12, 1044379.25, 713031.88, 1038792.00, 713926.00, 1033200.75, 714791.38, 1027605.25, 715628.38, 1022006.00, 716437.62, 1016402.75, 717219.50, 1010795.75, 717974.62, 1005184.75, 718703.62, 999570.25, 719406.88, 993952.00, 720085.00, 988330.25, 720738.62, 982705.25, 721368.00, 977076.50, 721973.88, 971444.75, 722556.88, 965809.50, 723117.38, 960171.25, 723655.88, 954529.75, 724173.00, 948885.50, 724669.25, 943238.25, 725145.25, 937588.25, 725601.38, 931935.25, 726038.38, 926279.75, 726456.50, 920621.50, 726856.38, 914960.75, 727238.88, 909297.50, 727604.12, 903632.00, 727952.62, 897964.25, 728285.38, 892294.00, 728602.38, 886621.75, 728904.50, 880947.25, 729192.12, 875271.00, 729465.88, 869592.75, 729726.38, 863912.75, 729973.88, 858230.75, 730209.12, 852547.25, 730432.62, 846862.00, 730644.88, 841175.25, 730846.38, 835487.00, 731037.75, 829797.25, 731219.62, 824106.50, 731392.12, 818414.25, 731556.38, 812721.25, 731712.38, 807026.75, 731861.00, 801331.25, 732002.62, 795635.25, 732137.88, 789938.00, 732267.12, 784240.00, 732391.12, 778541.50, 732510.38, 772842.25, 732625.25, 767142.50, 732736.38, 761442.25, 732844.38, 755741.75, 732949.62, 750040.75, 733052.75, 744339.75, 733154.38, 754122.75, 738904.50, 753234.75, 725215.50, 744963.75, 733154.38
				};
				////////////////////////////////////////////////////////////////////////////////////
				// type 2
				Ogre::SceneNode * mpMissionPathNode3 = pWndCtx->sceneMgr->getRootSceneNode()->createChildSceneNode("MissionPath2");

				ManualObject *mpRepetPathObject7 = pWndCtx->sceneMgr->createManualObject("RepetPathObject7");
				mpRepetPathObject7->begin("AlphaMaterial", RenderOperation::OT_TRIANGLE_LIST);
				mpRepetPathObject7->position(0, 0, 0);
				mpRepetPathObject7->colour(1, 1, 1);
				mpRepetPathObject7->position(100, 0, 0);
				mpRepetPathObject7->colour(1, 1, 1);
				mpRepetPathObject7->position(100, 0, 100);
				mpRepetPathObject7->colour(1, 1, 1);
				mpRepetPathObject7->triangle(0, 1, 2);
				mpRepetPathObject7->end();
				mpRepetPathObject7->setVisible(false);
				mpRepetPathObject7->setCastShadows(false);
				mpRepetPathObject7->setRenderQueueGroup(90);
				mpMissionPathNode3->attachObject(mpRepetPathObject7);

				mpRepetPathObject7->beginUpdate(0);

				count = 0;
				nArrCount = sizeof(type21) / sizeof(float);
				for (int i = 2; i < nArrCount; i += 2)
				{
					float tx1 = type21[i - 2] / 1000.0f;
					float ty1 = 20.0f;
					float tz1 = -type21[i - 1] / 1000.0f;

					float tx2 = type21[i] / 1000.0f;
					float ty2 = 20.0f;
					float tz2 = -type21[i + 1] / 1000.0f;
					Vector3 vStart = Vector3(tx1, ty1, tz1);
					Vector3 vEnd = Vector3(tx2, ty2, tz2);
					Vector3 vCross = (vEnd - vStart).normalisedCopy().crossProduct(Vector3::UNIT_Y) * rad;

					mpRepetPathObject7->position(vStart + vCross + Vector3::UNIT_Y * 11);
					mpRepetPathObject7->colour(mRepetPathColor);
					mpRepetPathObject7->position(vStart - vCross + Vector3::UNIT_Y * 11);
					mpRepetPathObject7->colour(mRepetPathColor);
					mpRepetPathObject7->position(vEnd - vCross + Vector3::UNIT_Y * 11);
					mpRepetPathObject7->colour(mRepetPathColor);
					mpRepetPathObject7->position(vEnd + vCross + Vector3::UNIT_Y * 11);
					mpRepetPathObject7->colour(mRepetPathColor);
					count += 4;
					mpRepetPathObject7->index(count - 4);
					mpRepetPathObject7->index(count - 3);
					mpRepetPathObject7->index(count - 2);
					mpRepetPathObject7->index(count - 4);
					mpRepetPathObject7->index(count - 2);
					mpRepetPathObject7->index(count - 1);
				}
				mpRepetPathObject7->end();
				mpRepetPathObject7->setVisible(true);
												
				//std::string szName = std::string(mID + "RepetPathObject8");
				ManualObject *mpRepetPathObject8 = pWndCtx->sceneMgr->createManualObject("RepetPathObject8");
				mpRepetPathObject8->begin("AlphaMaterial", RenderOperation::OT_TRIANGLE_LIST);
				mpRepetPathObject8->position(0, 0, 0);
				mpRepetPathObject8->colour(1, 1, 1);
				mpRepetPathObject8->position(100, 0, 0);
				mpRepetPathObject8->colour(1, 1, 1);
				mpRepetPathObject8->position(100, 0, 100);
				mpRepetPathObject8->colour(1, 1, 1);
				mpRepetPathObject8->triangle(0, 1, 2);
				mpRepetPathObject8->end();
				mpRepetPathObject8->setVisible(false);
				mpRepetPathObject8->setCastShadows(false);
				mpRepetPathObject8->setRenderQueueGroup(90);
				mpMissionPathNode3->attachObject(mpRepetPathObject8);

				mpRepetPathObject8->beginUpdate(0);

				count = 0;
				nArrCount = sizeof(type22) / sizeof(float);
				for (int i = 2; i < nArrCount; i += 2)
				{
					float tx1 = type22[i - 2] / 1000.0f;
					float ty1 = 20.0f;
					float tz1 = -type22[i - 1] / 1000.0f;

					float tx2 = type22[i] / 1000.0f;
					float ty2 = 20.0f;
					float tz2 = -type22[i + 1] / 1000.0f;
					Vector3 vStart = Vector3(tx1, ty1, tz1);
					Vector3 vEnd = Vector3(tx2, ty2, tz2);
					Vector3 vCross = (vEnd - vStart).normalisedCopy().crossProduct(Vector3::UNIT_Y) * rad;

					mpRepetPathObject8->position(vStart + vCross + Vector3::UNIT_Y * 11);
					mpRepetPathObject8->colour(mRepetPathColor);
					mpRepetPathObject8->position(vStart - vCross + Vector3::UNIT_Y * 11);
					mpRepetPathObject8->colour(mRepetPathColor);
					mpRepetPathObject8->position(vEnd - vCross + Vector3::UNIT_Y * 11);
					mpRepetPathObject8->colour(mRepetPathColor);
					mpRepetPathObject8->position(vEnd + vCross + Vector3::UNIT_Y * 11);
					mpRepetPathObject8->colour(mRepetPathColor);
					count += 4;
					mpRepetPathObject8->index(count - 4);
					mpRepetPathObject8->index(count - 3);
					mpRepetPathObject8->index(count - 2);
					mpRepetPathObject8->index(count - 4);
					mpRepetPathObject8->index(count - 2);
					mpRepetPathObject8->index(count - 1);
				}
				mpRepetPathObject8->end();
				mpRepetPathObject8->setVisible(true);

				ManualObject *mpRepetPathObject9 = pWndCtx->sceneMgr->createManualObject("RepetPathObject9");
				mpRepetPathObject9->begin("AlphaMaterial", RenderOperation::OT_TRIANGLE_LIST);
				mpRepetPathObject9->position(0, 0, 0);
				mpRepetPathObject9->colour(1, 1, 1);
				mpRepetPathObject9->position(100, 0, 0);
				mpRepetPathObject9->colour(1, 1, 1);
				mpRepetPathObject9->position(100, 0, 100);
				mpRepetPathObject9->colour(1, 1, 1);
				mpRepetPathObject9->triangle(0, 1, 2);
				mpRepetPathObject9->end();
				mpRepetPathObject9->setVisible(false);
				mpRepetPathObject9->setCastShadows(false);
				mpRepetPathObject9->setRenderQueueGroup(90);
				mpMissionPathNode3->attachObject(mpRepetPathObject9);

				mpRepetPathObject9->beginUpdate(0);

				count = 0;
				nArrCount = sizeof(type23) / sizeof(float);
				for (int i = 2; i < nArrCount; i += 2)
				{
					float tx1 = type23[i - 2] / 1000.0f;
					float ty1 = 20.0f;
					float tz1 = -type23[i - 1] / 1000.0f;

					float tx2 = type23[i] / 1000.0f;
					float ty2 = 20.0f;
					float tz2 = -type23[i + 1] / 1000.0f;
					Vector3 vStart = Vector3(tx1, ty1, tz1);
					Vector3 vEnd = Vector3(tx2, ty2, tz2);
					Vector3 vCross = (vEnd - vStart).normalisedCopy().crossProduct(Vector3::UNIT_Y) * rad;

					mpRepetPathObject9->position(vStart + vCross + Vector3::UNIT_Y * 11);
					mpRepetPathObject9->colour(mRepetPathColor);
					mpRepetPathObject9->position(vStart - vCross + Vector3::UNIT_Y * 11);
					mpRepetPathObject9->colour(mRepetPathColor);
					mpRepetPathObject9->position(vEnd - vCross + Vector3::UNIT_Y * 11);
					mpRepetPathObject9->colour(mRepetPathColor);
					mpRepetPathObject9->position(vEnd + vCross + Vector3::UNIT_Y * 11);
					mpRepetPathObject9->colour(mRepetPathColor);
					count += 4;
					mpRepetPathObject9->index(count - 4);
					mpRepetPathObject9->index(count - 3);
					mpRepetPathObject9->index(count - 2);
					mpRepetPathObject9->index(count - 4);
					mpRepetPathObject9->index(count - 2);
					mpRepetPathObject9->index(count - 1);
				}
				mpRepetPathObject9->end();
				mpRepetPathObject9->setVisible(true);

				ManualObject *mpRepetPathObject10 = pWndCtx->sceneMgr->createManualObject("RepetPathObject10");
				mpRepetPathObject10->begin("AlphaMaterial", RenderOperation::OT_TRIANGLE_LIST);
				mpRepetPathObject10->position(0, 0, 0);
				mpRepetPathObject10->colour(1, 1, 1);
				mpRepetPathObject10->position(100, 0, 0);
				mpRepetPathObject10->colour(1, 1, 1);
				mpRepetPathObject10->position(100, 0, 100);
				mpRepetPathObject10->colour(1, 1, 1);
				mpRepetPathObject10->triangle(0, 1, 2);
				mpRepetPathObject10->end();
				mpRepetPathObject10->setVisible(false);
				mpRepetPathObject10->setCastShadows(false);
				mpRepetPathObject10->setRenderQueueGroup(90);
				mpMissionPathNode3->attachObject(mpRepetPathObject10);

				mpRepetPathObject10->beginUpdate(0);

				count = 0;
				nArrCount = sizeof(type24) / sizeof(float);
				for (int i = 2; i < nArrCount; i += 2)
				{
					float tx1 = type24[i - 2] / 1000.0f;
					float ty1 = 20.0f;
					float tz1 = -type24[i - 1] / 1000.0f;

					float tx2 = type24[i] / 1000.0f;
					float ty2 = 20.0f;
					float tz2 = -type24[i + 1] / 1000.0f;
					Vector3 vStart = Vector3(tx1, ty1, tz1);
					Vector3 vEnd = Vector3(tx2, ty2, tz2);
					Vector3 vCross = (vEnd - vStart).normalisedCopy().crossProduct(Vector3::UNIT_Y) * rad;

					mpRepetPathObject10->position(vStart + vCross + Vector3::UNIT_Y * 11);
					mpRepetPathObject10->colour(mRepetPathColor);
					mpRepetPathObject10->position(vStart - vCross + Vector3::UNIT_Y * 11);
					mpRepetPathObject10->colour(mRepetPathColor);
					mpRepetPathObject10->position(vEnd - vCross + Vector3::UNIT_Y * 11);
					mpRepetPathObject10->colour(mRepetPathColor);
					mpRepetPathObject10->position(vEnd + vCross + Vector3::UNIT_Y * 11);
					mpRepetPathObject10->colour(mRepetPathColor);
					count += 4;
					mpRepetPathObject10->index(count - 4);
					mpRepetPathObject10->index(count - 3);
					mpRepetPathObject10->index(count - 2);
					mpRepetPathObject10->index(count - 4);
					mpRepetPathObject10->index(count - 2);
					mpRepetPathObject10->index(count - 1);
				}
				mpRepetPathObject10->end();
				mpRepetPathObject10->setVisible(true);

				mpMissionPathNode3->setVisible(false);
			}

		}

		bool UBaseView::AddComponent(int nCompType, float tx, float ty, float tz)
		{
			WndCtx * pWndCtx = GetWndContext(m_hWnd);
			if (pWndCtx != NULL)
			{				
				if (nCompType == 1)
				{
					std::string names1[] = { "Popular", "yhTree_A", "yhTree_A_001.mesh" };
					int nCookie = UDB::GetNextCookie();

					Ogre::StaticGeometry* field = pWndCtx->sceneMgr->createStaticGeometry(names1[1] + Ogre::StringConverter::toString(nCookie));
					for (int i = 0; i < 4; i++)
					{
						for (int j = 0; j < 50; j++)
						{
							Ogre::Entity * pEntity = pWndCtx->sceneMgr->createEntity(names1[2]);
							Ogre::AxisAlignedBox box = pEntity->getBoundingBox();
							Ogre::MaterialPtr material = static_cast<Ogre::MaterialPtr>(MaterialManager::getSingleton().getByName(names1[1]));

							pEntity->setMaterial(material);
							pEntity->setCastShadows(false);

							field->addEntity(pEntity, Ogre::Vector3(tx + i * 10.0f, ty+ (box.getCenter().y / 2.0f) - 1.0f, tz + j * 10.0f), Quaternion::IDENTITY, Ogre::Vector3(2.0f, 2.4f, 2.0f));
						}
					}
					field->build();
				}
				else if (nCompType == 2)
				{
					std::string names2[] = { "Popular", "yhTree_B", "yhTree_B_001.mesh" };
					int nCookie = UDB::GetNextCookie();
					Ogre::StaticGeometry* field = pWndCtx->sceneMgr->createStaticGeometry(names2[1] + Ogre::StringConverter::toString(nCookie));
					for (int i = 0; i < 4; i++)
					{
						for (int j = 0; j < 50; j++)
						{
							Ogre::Entity * pEntity = pWndCtx->sceneMgr->createEntity(names2[2]);
							Ogre::AxisAlignedBox box = pEntity->getBoundingBox();
							Ogre::MaterialPtr material = static_cast<Ogre::MaterialPtr>(MaterialManager::getSingleton().getByName(names2[1]));
							pEntity->setMaterial(material);
							pEntity->setCastShadows(false);

							field->addEntity(pEntity, Ogre::Vector3(tx + i * 10.0f, ty + (box.getCenter().y / 2.0f) - 1.0f, tz + j * 10.0f), Quaternion::IDENTITY, Ogre::Vector3(2.0f, 2.4f, 2.0f));
						}
					}
					field->build();
				}				
				return true;
			}
			return false;
		}

		int UBaseView::CheckScenePosition(std::string szName, int type, float value)
		{
			WndCtx * pWndCtx = GetWndContext(m_hWnd);
			int nResult = -999;
			if (pWndCtx != NULL)
			{
				Ogre::SceneNode* node = pWndCtx->sceneMgr->getSceneNode(szName);
				Ogre::AxisAlignedBox aabb = node->_getWorldAABB();
				Ogre::Vector3 v = aabb.getCenter();
				if (type == 0) // x방향
				{
					if (v.x > value)
					{
						nResult = -1;
					}
					else
					{
						nResult = 1;
					}
					return nResult;
				}
				else if (type == 1) // z방향
				{
					if (v.z> value)
					{
						nResult = -1;
					}
					else
					{
						nResult = 1;
					}
					return nResult;
				}
				else if (type == 2) // y방향
				{
					if (v.y> value)
					{
						nResult = -1;
					}
					else
					{
						nResult = 1;
					}
					return nResult;
				}
				
			}
			return nResult;
		}

		/*void UBaseView::CreateSubCompass(float fAzumith)
		{
			WndCtx * pWndCtx = GetWndContext(m_hWnd);
			if (pWndCtx != NULL)
			{
				if (mpCompassSub == NULL)
				{
					mpCompassSub = new CCompassManager(m_hWnd);
					((CCompassManager*)mpCompassSub)->SetVisible(false);
				}
				if (mpCompassSub != NULL)
				{
					((CCompassManager*)mpCompassSub)->SetAzimuth(fAzumith);
					((CCompassManager*)mpCompassSub)->SetVisible(true);
				}
			
			}
		}*/


		void UBaseView::SetFloorTopColor(float r, float g, float b)
		{
			m_rFloorTop = r;
			m_gFloorTop = g;
			m_bFloorTop = b;

		}

		void UBaseView::SetFloorBottomColor(float r, float g, float b)
		{
			m_rFloorBtm = r;
			m_gFloorBtm = g;
			m_bFloorBtm = b;
		}

		void UBaseView::SetFloorEnableGradient(bool bEnable)
		{
			m_bEnableFloorGradient = bEnable;
		}

		void UBaseView::CreateFloor(float x, float y, float z)
		{
			WndCtx * pCTX = GetWndContext(m_hWnd);
			Ogre::String szName = "FloorRect";
			// CHECK NODE
			try
			{
				Node * snode = pCTX->sceneMgr->getSceneNode(szName);
				if (snode != NULL)
					return;
			}
			catch (Exception)
			{
			}
			
			UnE::Core::UBaseModel * pDoc = UDB::GetBaseModel((int)m_hWnd);
			pDoc->CreateSceneManager();

			ClearViewData();

			Ogre::MeshManager::getSingleton().unloadUnreferencedResources(false);
			Ogre::MaterialManager::getSingleton().unloadUnreferencedResources(false);
			
			CreateBackgroundPane();

			//////////////////////////////////////////////////////////
			// Set Background Material
			Ogre::String lNameOfMaterial = szName + "_Material";
			Ogre::String lResourceGroup = Ogre::ResourceGroupManager::DEFAULT_RESOURCE_GROUP_NAME;
			if (!MaterialManager::getSingleton().resourceExists(lNameOfMaterial))
			{
				Ogre::MaterialPtr myPathMaterial = Ogre::MaterialManager::getSingleton().create(lNameOfMaterial, lResourceGroup);
				// SET NO SHADOW
				myPathMaterial->setReceiveShadows(false);
				// USE COLOR MODE
				myPathMaterial->getTechnique(0)->setLightingEnabled(false);
				// DEPTH CHECK DISABLE
				myPathMaterial->getTechnique(0)->getPass(0)->setDepthCheckEnabled(false);
				myPathMaterial->getTechnique(0)->getPass(0)->setDepthWriteEnabled(false);
			}

			Ogre::SceneNode * pNode = pCTX->sceneMgr->getRootSceneNode()->createChildSceneNode(szName);

			// Create Manual Object
			ManualObject* manual = pCTX->sceneMgr->createManualObject("ManualFloorRect");


			float width = x ;
			float height = y;
			float elev = z;

			//manual->setUseIdentityProjection(true);
			//manual->setUseIdentityView(true);
			manual->begin(lNameOfMaterial, RenderOperation::OT_TRIANGLE_LIST);
			{
				manual->position(0.0,  0.0, 0.0);

				manual->colour(m_rFloorTop, m_gFloorTop, m_bFloorTop); // bottom 1

				manual->position(width, 0.0, 0.0);
				manual->colour(m_rFloorTop, m_gFloorTop, m_bFloorTop); // bottom 2

				manual->position(width, 0.0, -height);

				if (m_bEnableFloorGradient == false)
				{
					manual->colour(m_rFloorTop, m_gFloorTop, m_bFloorTop); // top2
				}
				else
				{
					manual->colour(m_rFloorBtm, m_rFloorBtm, m_rFloorBtm); // top2
				}

				manual->position(0.0, 0.0, -height);
				if (m_bEnableFloorGradient == false)
				{
					manual->colour(m_rFloorTop, m_gFloorTop, m_bFloorTop); // top1
				}
				else
				{
					manual->colour(m_rFloorBtm, m_rFloorBtm, m_rFloorBtm); // top1
				}

				manual->triangle(2, 3,0);
				manual->triangle(2, 0, 1);
			}
			manual->end();
		
			gEntityRenderContext.selected = false;
			gEntityRenderContext.ignoreViewDetail = true;
			manual->setUserAny(Ogre::Any(gEntityRenderContext));

			// Attach to scene
			pNode->attachObject(manual);
			pNode->_updateBounds();

			pCTX->aabb.merge(pNode->_getWorldAABB());
		}

		void UBaseView::SetOriginMaterial()
		{

		}

		void SetMaterial(Ogre::SceneNode* pNode, Ogre::MaterialPtr pMat)
		{
			int cnt = pNode->numChildren();
			for (int i = 0; i < cnt; ++i)
			{
				Ogre::SceneNode* pChild = (Ogre::SceneNode*)pNode->getChild(i);
				int nAttach = pChild->numAttachedObjects();
				for (int k = 0; k < nAttach; ++k)
				{
					Ogre::MovableObject* obj = pChild->getAttachedObject(k);
					if (obj->getMovableType() == "Entity")
					{
						Ogre::Entity* pEntity = (Ogre::Entity*)obj;
						pEntity->setMaterial(pMat);
						break;
					}
				}

				//SetMaterial(pChild, pMat);
			}
		}

		void UBaseView::SetTempMaterial(bool earthquake)
		{
			//char buff[13];
			//sprintf(buff, "%d%d%d%d", r, g, b, a);
			//Ogre::String matName = buff;
			//Ogre::MaterialPtr matPtr;
			//
			//if (!MaterialManager::getSingleton().resourceExists(matName))
			//{
			//	Ogre::MaterialPtr matPtr = Ogre::MaterialManager::getSingleton().create(matName, "General");
 		//		matPtr->setReceiveShadows(true);
			//	matPtr->getTechnique(0)->setLightingEnabled(true);
			//	matPtr->getTechnique(0)->getPass(0)->setDepthCheckEnabled(true);
			//	matPtr->getTechnique(0)->getPass(0)->setDepthWriteEnabled(false);
			//	float R = (float)r / 255.0f;
			//	float G = (float)g / 255.0f;
			//	float B = (float)b / 255.0f;
			//	float A = (float)a / 255.0f;
			//	//matPtr->getTechnique(0)->getPass(0)->setDiffuse(R, G, B, A);
			//	matPtr->getTechnique(0)->getPass(0)->setAmbient(Ogre::ColourValue(R, G, B, A));
			//}
			//else
			//	matPtr = static_cast<Ogre::MaterialPtr>(MaterialManager::getSingleton().getByName(matName));

			WndCtx * pCtx = GetWndContext(m_hWnd);
			if (pCtx != NULL)
			{
				Ogre::SceneManager*  sceneMgr = (Ogre::SceneManager*)(pCtx->sceneMgr);
				Ogre::SceneNode * pNode = sceneMgr->getRootSceneNode();

				for (int i = 1; i < 6; ++i)
				{
					char buff[16];
					sprintf(buff, "DirectionLight%d", i);
					Ogre::String name = buff;

					if (pCtx->sceneMgr->hasLight(name))
					{
						Ogre::Light* pLight = pCtx->sceneMgr->getLight(name);

						if (earthquake)
						{
							pLight->setDiffuseColour(1.0f, 0.0f, 0.0f);
							pLight->setSpecularColour(1.0f, 0.0f, 0.0f);
						}
						else
						{
							if (i == 1)
							{
								pLight->setDiffuseColour(0.8f, 0.8f, 0.8f);
								pLight->setSpecularColour(0.3f, 0.3f, 0.3f);
							}
							else
							{
								pLight->setDiffuseColour(0.6f, 0.6f, 0.6f);
								pLight->setSpecularColour(0.3f, 0.3f, 0.3f);
							}
						}
					}
				}
			}
		}


	}


}



