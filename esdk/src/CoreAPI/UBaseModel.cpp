#include "StdAfx.h"

//////////////////////////////////////////////////////////////////////////
// Core API
#include "UDB.h"
#include "UBaseDriver.h"
#include "UBaseView.h"
#include "UBaseModel.h"
#include "UScene.h"
#include "UAssimpLoader.h"

#include  "UDotSceneLoader.h"


using namespace UnE::Core;
using namespace UnE::Math;

//////////////////////////////////////////////////////////////////////////

namespace UnE
{
	namespace Core
	{
		//-----------------------------------------------------------------------
		UBaseModel * pBaseMode = NULL;
		//-----------------------------------------------------------------------
		UBaseModel* GetActiveModel()
		{
			return pBaseMode;
		}
		//-----------------------------------------------------------------------
		void SetActiveModel( UBaseModel* db )
		{
			pBaseMode = db;
		}

		//////////////////////////////////////////////////////////////////////////
		// UModel Implementaion
		UModel::UModel(void)
		{
			
		}

		//-----------------------------------------------------------------------
		UModel::~UModel(void)
		{
			
		}

		//////////////////////////////////////////////////////////////////////////
		// UBaseModel Implementation
		UBaseModel::UBaseModel( HWND hWnd )
		{
			m_pAssimpLoader = new UAssimpLoader();
			m_pView = NULL;
			m_hWnd = hWnd;
			SetActiveModel(this);

			m_pSecneManager = NULL;
		}		
		//-----------------------------------------------------------------------
		UBaseModel::~UBaseModel()
		{
			m_pView = NULL;

			UBaseModel * pBaseModel = GetActiveModel();
			if( pBaseModel == this)
			{
				SetActiveModel(NULL);
			}

			if(m_pAssimpLoader)
			{
				delete m_pAssimpLoader;
				m_pAssimpLoader = NULL;
			}

			if( m_pSecneManager != NULL)
			{
				delete m_pSecneManager;
				m_pSecneManager = NULL;
			}
		}	
		//-----------------------------------------------------------------------
		UModelInfo * UBaseModel::GetHModelInfo()
		{
			return m_pModelInfo;
		}
		//-----------------------------------------------------------------------

		void UBaseModel::SetBaseView( UBaseView * pView )
		{
			m_pView = pView;
		}
		//-----------------------------------------------------------------------
		void UBaseModel::Init()
		{

		}
		//-----------------------------------------------------------------------
		void UBaseModel::Flush()
		{

		}
		//-----------------------------------------------------------------------
		std::string& UBaseModel::GetModelName()
		{
			return mModelName;				
		}
		//-----------------------------------------------------------------------
		bool UBaseModel::ComputeData( int data_cycles /*= 30*/ )
		{
			return false;
		}
		//-----------------------------------------------------------------------
		UShellVertexData * UBaseModel::GetShellVertexData( void )
		{
			return m_pShellVertexData;
		}
		//-----------------------------------------------------------------------
		int UBaseModel::GetShellVertexDataCount( void )
		{
			return m_ShellVertexDataCount;
		}
		//-----------------------------------------------------------------------
		int UBaseModel::GetDataCycles( void )
		{
			return m_DataCycles;
		}
		//-----------------------------------------------------------------------
		UEventManager* UBaseModel::GetEventManager()
		{			
			return m_pEventManager;
		}
		//-----------------------------------------------------------------------
		void UBaseModel::Update( bool forceUpdate /*= false*/ )
		{
	
		}

		void UBaseModel::CreateSceneManager()
		{
			if (m_pSecneManager != NULL)
			{
				delete m_pSecneManager;
			}

			m_pSecneManager = new USceneNodeManager();
		}

		UnE::Core::UFileInputResult UBaseModel::ReadDAE(const std::string& szFileName)
		{
			if (m_pAssimpLoader == NULL)
				return eIF_RESULT_FAIL;
			Ogre::LogManager::getSingleton().logMessage("mesh converted successfully!");

			CreateSceneManager();


			std::string filename = szFileName;

			if (GetFileAttributesA(szFileName.c_str()) == INVALID_FILE_ATTRIBUTES)
			{
				return eIF_RESULT_BAD_FILENAME;
			}

			Ogre::String extension, basename, path;
			Ogre::StringUtil::splitFullFilename(szFileName, basename, extension, path);
			Ogre::String outName = basename + "_" + extension + ".mesh";
			Ogre::ResourceGroupManager::getSingleton().addResourceLocation(path, "FileSystem", "General");

			// CLEAR ENTITY
			WndCtx * pCTX = GetWndContext(m_hWnd);
			//m_pView = UDB::GetBaseView((int)m_hWnd);
			m_pView->ClearViewData();

			Ogre::MeshManager::getSingleton().unloadUnreferencedResources(false);
			Ogre::MaterialManager::getSingleton().unloadUnreferencedResources(false);
			m_pView->CreateBackgroundPane();

			m_pAssimpLoader->logPath = UBaseDriver::Instance().GetEngineWorkDir();

			int res = m_pAssimpLoader->convert(m_hWnd, szFileName);
			if (res)
			{
				try
				{
					m_pSecneManager->GetRootSceneNode()->SetTag(pCTX->sceneMgr->getRootSceneNode());
					Ogre::Vector3 vCamPos = pCTX->camera->getDerivedPosition();
					Ogre::Light *newLight = pCTX->sceneMgr->createLight();


					Ogre::Vector3 posCenter = pCTX->aabb.getCenter();
					posCenter.y = -10000.0f;
					newLight->setPosition(posCenter);
					posCenter.y = 0.0f;
					//newLight->setDirection( posCenter );
					newLight->setDiffuseColour(0.8f, 0.8f, 0.8f);
					//newLight->setSpecularColour( 0.3f, 0.3f, 0.3f );
					newLight->setType(Ogre::Light::LT_POINT);

					SetFileLoadComplete(true);

					m_pView->SetHomeView();
					SetFirstFitComplete(true);

					return eIF_RESULT_OK;
				}
				catch (Ogre::Exception* e)
				{
				}
				return eIF_RESULT_NOT_HANDLED;
			}


			return eIF_RESULT_FAIL;
		}

#ifndef HSMS
		//-----------------------------------------------------------------------
		UnE::Core::UFileInputResult UBaseModel::ReadScene( const std::string& szFileName )
		{				
			if( m_pAssimpLoader == NULL)
				return eIF_RESULT_FAIL;
			Ogre::LogManager::getSingleton().logMessage("mesh converted successfully!");	
			
			if( m_pSecneManager != NULL)
			{
				delete m_pSecneManager;
			}

			m_pSecneManager = new USceneNodeManager();		
			

			std::string filename =  szFileName;

			if( GetFileAttributesA(szFileName.c_str()) == INVALID_FILE_ATTRIBUTES)
			{
				return eIF_RESULT_BAD_FILENAME;
			}
						
			
			// CLEAR ENTITY
			WndCtx * pCTX = GetWndContext(m_hWnd);
			//m_pView = UDB::GetBaseView((int)m_hWnd);
			m_pView->ClearViewData();

			//Ogre::MeshManager::getSingleton().unloadUnreferencedResources(false);
			//Ogre::MaterialManager::getSingleton().unloadUnreferencedResources(false);
			m_pView->CreateBackgroundPane();
			
			m_pAssimpLoader->logPath = UBaseDriver::Instance().GetEngineWorkDir();
			
			Ogre::String extension, basename, path;
			Ogre::StringUtil::splitFullFilename(szFileName, basename, extension, path);
			Ogre::String outName = basename + "_" + extension + ".mesh";

			try
			{
				Ogre::ResourceGroupManager::getSingleton().createResourceGroup("DotSceneModel");
				Ogre::ResourceGroupManager::getSingleton().addResourceLocation(path, "FileSystem", "DotSceneModel");
				//Ogre::ResourceGroupManager::getSingleton().addResourceLocation(path + "material", "FileSystem", "DotSceneModel");
				//Ogre::ResourceGroupManager::getSingleton().addResourceLocation(path + "mesh", "FileSystem", "DotSceneModel");
				//Ogre::ResourceGroupManager::getSingleton().addResourceLocation(path + "bitmap", "FileSystem", "DotSceneModel");

				Ogre::ResourceGroupManager::getSingleton().addResourceLocation(path + basename + ".zip", "Zip", "DotSceneModel", true);
				Ogre::ResourceGroupManager::getSingleton().initialiseResourceGroup("DotSceneModel");
			}
			catch (Ogre::Exception* e)
			{
			}
			try
			{
				Ogre::ResourceGroupManager::getSingleton().loadResourceGroup("DotSceneModel");
			}
			catch (Ogre::Exception* e)
			{
			}
			//int res = m_pAssimpLoader->convert(m_hWnd, szFileName);
			int res = 1;
			Ogre::UDotSceneLoader * loader = new Ogre::UDotSceneLoader(m_hWnd);
			loader->parseDotScene(szFileName, "DotSceneModel", pCTX->sceneMgr, pCTX->rootNode, "");
			
			if(res)
			{						
				try
				{
					
					pCTX->sceneMgr->setShadowTechnique(Ogre::ShadowTechnique::SHADOWTYPE_STENCIL_ADDITIVE);
					pCTX->sceneMgr->setShadowColour(Ogre::ColourValue(0.8f, 0.8f, 0.8f));
					//pCTX->rootNode->showBoundingBox(true);
					//pCTX->rootNode->scale(0.001f, 0.001f, 0.001f);
					pCTX->rootNode->needUpdate();

					Ogre::Vector3 posCenter = pCTX->aabb.getCenter();
					posCenter.x += 5000.0f;
					posCenter.y = 5000.0f;
					posCenter.z = -500.0f;

					m_pSecneManager->GetRootSceneNode()->SetTag(pCTX->sceneMgr->getRootSceneNode());
					Ogre::Vector3 vCamPos = pCTX->camera->getDerivedPosition();
					
					Ogre::Light *newLight = pCTX->sceneMgr->createLight("DirectionLight1");	
					newLight->setPosition( posCenter );
					//newLight->setCastShadows(true);
										
					Ogre::Vector3 posCenter2 = pCTX->aabb.getCenter();
					posCenter2.y = 0.0f;					
					Ogre::Vector3 vDir = posCenter2 - posCenter;

					newLight->setDiffuseColour( 0.8f, 0.8f, 0.8f );
					newLight->setSpecularColour(0.3f, 0.3f, 0.3f );						
					newLight->setType(Ogre::Light::LT_DIRECTIONAL);
					newLight->setDirection(vDir);	
					newLight->setCastShadows(false);

					Ogre::Light *newLight2 = pCTX->sceneMgr->createLight("DirectionLight2");

					posCenter.x = posCenter2.x / 2.0f ;
					posCenter.y = 5000.0f;
					posCenter.z -= 5000.0f;

					Ogre::Vector3 vDir2 = posCenter2 - posCenter;
					newLight2->setPosition(posCenter);					
					newLight2->setDiffuseColour(0.6f, 0.6f, 0.6f);
					newLight2->setSpecularColour( 0.3f, 0.3f, 0.3f );
					newLight2->setType(Ogre::Light::LT_DIRECTIONAL);
					newLight2->setDirection(vDir2);
					newLight2->setCastShadows(false);


					Ogre::Light *newLight3 = pCTX->sceneMgr->createLight("DirectionLight3");
					posCenter.x = -9000.0f;
					posCenter.y = 5000.0f;
					posCenter.z = posCenter2.z / 2.0f;
					Ogre::Vector3 vDir3 = posCenter2 - posCenter;
					newLight3->setPosition(posCenter);
					newLight3->setDiffuseColour(0.6f, 0.6f, 0.6f);
					newLight3->setSpecularColour(0.3f, 0.3f, 0.3f);
					newLight3->setType(Ogre::Light::LT_DIRECTIONAL);
					newLight3->setDirection(vDir3);
					newLight3->setCastShadows(false);

					Ogre::Light *newLight4 = pCTX->sceneMgr->createLight("DirectionLight4");
					posCenter.x = posCenter2.x / 2.0f;
					posCenter.y = 5000.0f;
					posCenter.z += 5000.0f;
					Ogre::Vector3 vDir4 = posCenter2 - posCenter;
					newLight4->setPosition(posCenter);
					newLight4->setDiffuseColour(0.6f, 0.6f, 0.6f);
					newLight4->setSpecularColour(0.3f, 0.3f, 0.3f);				
					newLight4->setType(Ogre::Light::LT_DIRECTIONAL);
					newLight4->setDirection(vDir4);
					newLight4->setCastShadows(false);


					Ogre::Light *newLight5 = pCTX->sceneMgr->createLight("DirectionLight5");
					posCenter.x = posCenter2.x / 2.0f;
					posCenter.y = -5000.0f;
					posCenter.z += 5000.0f;
					Ogre::Vector3 vDir5 = posCenter2 - posCenter;
					newLight5->setPosition(posCenter);
					newLight5->setDiffuseColour(0.6f, 0.6f, 0.6f);
					newLight5->setSpecularColour(0.3f, 0.3f, 0.3f);
					newLight5->setType(Ogre::Light::LT_DIRECTIONAL);
					newLight5->setDirection(vDir5);
					newLight5->setCastShadows(false);


					SetFileLoadComplete(true);

					m_pView->SetHomeView();
					SetFirstFitComplete(true);

					return eIF_RESULT_OK;	
				}			
				catch (Ogre::Exception* e)
				{
				}
				return eIF_RESULT_NOT_HANDLED;
			}
				
						
			return eIF_RESULT_FAIL;
		}
#else
		UnE::Core::UFileInputResult UBaseModel::ReadScene(const std::string& szFileName)
		{
			return eIF_RESULT_NOT_HANDLED;
		}
#endif
		//-----------------------------------------------------------------------
		UnE::Core::UFileOutputResult UBaseModel::Write( const char * szFileName )
		{
			return eOF_RESULT_OK;
		}
		//-----------------------------------------------------------------------
	}
}
