#include "StdAfx.h"
#include "UBaseDriver.h"

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
// CORE API
#include "UBaseView.h"
#include "UMaterial.h"
#include "UDB.h"
#include "UAnimation.h"


extern		Ogre::Root							*	m_Root;
extern		Ogre::RenderSystem					*	m_renderSystem;

namespace UnE
{
	namespace Core
	{
		UnE::Core::UBaseDriver UBaseDriver::s_cInstance;
		

		UBaseDriver::UBaseDriver(void)
		{		
			m_nScrWidth = 1280;
			m_nScrHeight = 1024;
			m_nPixFormat = 32;

			m_renderSystem  = NULL;
			m_Root = NULL;
			g_WndCtx = NULL;
			m_SubWndCtx = NULL;

			m_bInitClient = false;
			m_bInitRegistry = false;
			m_bInitDriver = false;
			m_ChildView.clear();
		}


		UBaseDriver::~UBaseDriver(void)
		{
			if( m_bInitDriver == true)
			{
				DisposeDriver();
				ClearDriver();
			}
		}

		bool UBaseDriver::InitDriver(std::string szEngineWorkDir, std::string szAppName)
		{
			if( m_bInitClient == false)
			{
				OGRE_EXCEPT(Ogre::Exception::ERR_INVALID_STATE,
					"Cannot Engine init - no instance handle , set your istance handle.", "UBaseView::InitEngine");
			}

			if( m_bInitRegistry == false)
			{
				OGRE_EXCEPT(Ogre::Exception::ERR_INVALID_STATE,
					"Cannot Engine init - no main registry , set your registry handle.", "UBaseView::InitEngine");
			}

			// get work directory

			//GetWorkDir(m_szEngineWorkDir);
			m_szEngineWorkDir = szEngineWorkDir;
			m_szAppName = szAppName;
			Ogre::String szPath(m_szEngineWorkDir);
			//szPath = szEngineWorkDir;

#ifdef _DEBUG
			Ogre::String mResourcesCfg = szPath + "resources.cfg";
			Ogre::String mPluginsCfg = szPath + "plugins_d.cfg";
#else
			Ogre::String mResourcesCfg = szPath+ "resources.cfg";
			Ogre::String mPluginsCfg = szPath+ "plugins.cfg";
#endif

			//// check cfg files
			//if( GetFileAttributesA(mPluginsCfg.c_str()) == INVALID_FILE_ATTRIBUTES )
			//{
			//	OGRE_EXCEPT(Ogre::Exception::ERR_INVALID_STATE,
			//		(std::string("Cannot Engine init - check plugin.cfg ") + std::string(mPluginsCfg.c_str())).c_str()
			//		, "UBaseView::InitEngine");
			//}

			//if( GetFileAttributesA(mResourcesCfg.c_str()) == INVALID_FILE_ATTRIBUTES)
			//{
			//	OGRE_EXCEPT(Ogre::Exception::ERR_INVALID_STATE,
			//		(std::string("Cannot Engine init - check resources.cfg ") + std::string(mResourcesCfg.c_str())).c_str()
			//		,"UBaseView::InitEngine");
			//}

			Ogre::String cfgFileName = szEngineWorkDir + szAppName + ".cfg";
			Ogre::String logFineName = szEngineWorkDir + szAppName + ".log";
			// construct Ogre::Root
			m_Root = new Ogre::Root("", cfgFileName, logFineName );
			
#ifdef DEBUG
#ifdef WIN64
			Ogre::String pluginList[] =
			{
				"RenderSystem_Direct3D912x64D",
				"RenderSystem_GL12x64D",
				"Plugin_ParticleFX12x64D",
				"Plugin_BSPSceneManager12x64D",
				"Plugin_CgProgramManager12x64D",
				"Plugin_PCZSceneManager12x64D",
				"Plugin_OctreeZone12x64D",
				"Plugin_OctreeSceneManager12x64D" // 8
			};
#else
#ifdef _MSC_VER >= 1700
			Ogre::String pluginList[] = 
			{
				"RenderSystem_Direct3D912Win32D",
				"RenderSystem_GL12Win32D",
				"Plugin_ParticleFX12Win32D",
				"Plugin_BSPSceneManager12Win32D",
				"Plugin_CgProgramManager12Win32D",
				"Plugin_PCZSceneManager12Win32D",
				"Plugin_OctreeZone12Win32D",
				"Plugin_OctreeSceneManager12Win32D" // 8
			};
#else
			Ogre::String pluginList[] = 
			{
				"RenderSystem_Direct3D910Win32D",
				"RenderSystem_GL10Win32D",
				"Plugin_ParticleFX10Win32D",
				"Plugin_BSPSceneManager10Win32D",
				"Plugin_CgProgramManager10Win32D",
				"Plugin_PCZSceneManager10Win32D",
				"Plugin_OctreeZone10Win32D",
				"Plugin_OctreeSceneManager10Win32D" // 8
			};
#endif
#endif

#else
			Ogre::String pluginList[] =
			{
				"RenderSystem_Direct3D9",
				"RenderSystem_GL",
				"Plugin_ParticleFX",
				"Plugin_BSPSceneManager",
				"Plugin_CgProgramManager",
				"Plugin_PCZSceneManager",
				"Plugin_OctreeZone",
				"Plugin_OctreeSceneManager" // 8
			};
#endif

			for( int i = 0 ; i < 8 ; i++)
			{
				Ogre::String plugInName = szEngineWorkDir + pluginList[i];
				m_Root->loadPlugin(plugInName);
			}			
			
			// setup resources
			// Load resource paths from config file
			//Ogre::ConfigFile cf;			
			//cf.load(mResourcesCfg);

			//// Go through all sections & settings in the file
			//Ogre::ConfigFile::SectionIterator seci = cf.getSectionIterator();

			//Ogre::String secName, typeName, archName;
			//while (seci.hasMoreElements())
			//{
			//	secName = seci.peekNextKey();
			//	Ogre::ConfigFile::SettingsMultiMap *settings = seci.getNext();
			//	Ogre::ConfigFile::SettingsMultiMap::iterator i;
			//	for (i = settings->begin(); i != settings->end(); ++i)
			//	{
			//		typeName = i->first;
			//		archName = i->second;
			//		Ogre::ResourceGroupManager::getSingleton().addResourceLocation(archName, typeName, secName);
			//	}
			//}	

			// Essential
			std::string archName = m_szEngineWorkDir + "\\Media\\packs\\SdkTrays.zip";
			Ogre::ResourceGroupManager::getSingleton().addResourceLocation(archName, "Zip", "Essential");
			archName = m_szEngineWorkDir + "\\Media\\packs\\profiler.zip";
			Ogre::ResourceGroupManager::getSingleton().addResourceLocation(archName, "Zip", "Essential");
			archName = m_szEngineWorkDir + "\\Media\\thumbnails";
			Ogre::ResourceGroupManager::getSingleton().addResourceLocation(archName, "FileSystem", "Essential");
			
			// Popular
			archName = m_szEngineWorkDir + "\\Media\\fonts";
			Ogre::ResourceGroupManager::getSingleton().addResourceLocation(archName, "FileSystem", "Popular");
			archName = m_szEngineWorkDir + "\\Media\\materials";
			Ogre::ResourceGroupManager::getSingleton().addResourceLocation(archName, "FileSystem", "Popular");
			archName = m_szEngineWorkDir + "\\Media\\materials\\programs";
			Ogre::ResourceGroupManager::getSingleton().addResourceLocation(archName, "FileSystem", "Popular");
			archName = m_szEngineWorkDir + "\\Media\\materials\\textures";
			Ogre::ResourceGroupManager::getSingleton().addResourceLocation(archName, "FileSystem", "Popular");
			archName = m_szEngineWorkDir + "\\Media\\models";
			Ogre::ResourceGroupManager::getSingleton().addResourceLocation(archName, "FileSystem", "Popular");
			archName = m_szEngineWorkDir + "\\Media\\particle";
			Ogre::ResourceGroupManager::getSingleton().addResourceLocation(archName, "FileSystem", "Popular");
			archName = m_szEngineWorkDir + "\\Media\\DeferredShadingMedia";
			Ogre::ResourceGroupManager::getSingleton().addResourceLocation(archName, "FileSystem", "Popular");
			archName = m_szEngineWorkDir + "\\Media\\PCZAppMedia";
			Ogre::ResourceGroupManager::getSingleton().addResourceLocation(archName, "FileSystem", "Popular");
			archName = m_szEngineWorkDir + "\\Media\\packs\\cubemap.zip";
			Ogre::ResourceGroupManager::getSingleton().addResourceLocation(archName, "Zip", "Popular");
			archName = m_szEngineWorkDir + "\\Media\\packs\\skybox.zip";
			Ogre::ResourceGroupManager::getSingleton().addResourceLocation(archName, "Zip", "Popular");
			archName = m_szEngineWorkDir + "\\Media\\packs\\cubemapsJS.zip";
			Ogre::ResourceGroupManager::getSingleton().addResourceLocation(archName, "Zip", "Popular");
			
#ifndef HSMS

			archName = m_szEngineWorkDir + "\\Media\\Component\\TreeA";
			Ogre::ResourceGroupManager::getSingleton().addResourceLocation(archName, "FileSystem", "Popular");
			archName = m_szEngineWorkDir + "\\Media\\Component\\TreeB";
			Ogre::ResourceGroupManager::getSingleton().addResourceLocation(archName, "FileSystem", "Popular");
			archName = m_szEngineWorkDir + "\\Media\\Component\\WindMill";
			Ogre::ResourceGroupManager::getSingleton().addResourceLocation(archName, "FileSystem", "Popular");

			archName = m_szEngineWorkDir + "\\Media\\Component\\Sphere";
			Ogre::ResourceGroupManager::getSingleton().addResourceLocation(archName, "FileSystem", "Popular");

#endif

			// General
			archName = m_szEngineWorkDir + "\\Media";
			Ogre::ResourceGroupManager::getSingleton().addResourceLocation(archName, "FileSystem", "General");

			CreateRenderSystem(m_nRenderer);

			m_Root->initialise(false, "Main Render Window");

			m_bInitDriver = true;
			return true;
		}

		void UBaseDriver::SetClient( HINSTANCE& gInstance )
		{
			m_bInitClient = true;
			m_hInstance = gInstance;
		}

		void UBaseDriver::SetRegistry( HKEY& hRoot )
		{
			m_bInitRegistry = true;
			m_hRootKey = hRoot;
		}

		void UBaseDriver::SetRenderer( URenderer nType )
		{
			m_nRenderer = nType;
		}

		void UBaseDriver::ChangeRenderer( URenderer nType )
		{
			m_nRenderer = nType;
			SaveRendererType(m_nRenderer);

			if( m_bInitDriver == false)
			{
				return;
			}

			if( m_Root != NULL)
			{
				UMaterialManager::Instance().Clear();
								
				DisposeDriver();		
				InitDriver(m_szEngineWorkDir, m_szAppName);

				OnChangeRenderer();
				
			}
		}

		void UBaseDriver::DisposeDriver()
		{
			m_bInitDriver = false;
			if( m_Root != NULL)
			{				
				NotityDispose();
				Ogre::MeshManager::getSingleton().unloadAll();
				Ogre::MaterialManager::getSingleton().unloadAll();							

				UnE::Core::UAnimationManager * pAniManager = UnE::Core::UDB::GetUDB()->GetAnimationManager();
				delete pAniManager;
				UnE::Core::UDB::GetUDB()->SetAnimationManager(NULL);			


				// delete ogre root
				delete m_Root;
				m_Root = NULL;	
			}

			ClearAllWndCtx();
		}

		void UBaseDriver::ClearDriver()
		{
			m_ChildView.clear();
		}

		void UBaseDriver::NotityDispose()
		{
			std::list<UBaseView*>::iterator iter = m_ChildView.begin();
			for(  ;  iter != m_ChildView.end(); iter++)
			{
				UBaseView * pView = *iter;
				if( pView != NULL)
				{
					pView->StopRendering();
					pView->Dispose();
					pView->ResumeRendering();						
				}
			}
		}

		void UBaseDriver::OnChangeRenderer()
		{
			if( m_ChildView.size() == 0)
			{
				return;
			}

			std::list<UBaseView*>::iterator iter = m_ChildView.begin();
			for(  ;  iter != m_ChildView.end(); iter++)
			{
				UBaseView * pView = *iter;
				if( pView != NULL)
				{
					pView->StopRendering();
					pView->OnChangeRenderer();
					pView->ResumeRendering();		
				}
			}			
		}

		void UBaseDriver::Add( UnE::Core::UBaseView * pView )
		{
			if( pView != NULL)
			{
				m_ChildView.push_back(pView);
			}
		}

		void UBaseDriver::Remove( UnE::Core::UBaseView * pView )
		{
			if( pView != NULL)
				m_ChildView.remove(pView);
		}

		void UBaseDriver::SaveRendererType( URenderer eRendererType )
		{
			int nType = TClamp((int)eRendererType, 0, 2);
			WriteProfileData(GetRegistry(), "RendererType", nType);
		}

		int UBaseDriver::GetRendererType()
		{
			int nType = TClamp((int)ReadProfileData(GetRegistry(), "RendererType", (int)0), 0, 2);
			return nType;
		}

		void UBaseDriver::GetWorkDir( std::string& strAppPath )
		{
			const size_t bufsz = MAX_PATH * 2;
			char szPath[bufsz] = {0,};
			BOOL bDirOk = FALSE;
			CStringA m_strAppDirectory;
			// get engine dll location
			if ( ::GetModuleFileNameA(NULL, szPath, bufsz) != 0 )
			{
				if ( GetLastError() != ERROR_INSUFFICIENT_BUFFER )
				{
					CString strPath(szPath);
					// add seperator
					m_strAppDirectory = strPath.Left(strPath.ReverseFind(_T('\\')) + 1);
					// save path
					strAppPath = std::string(m_strAppDirectory.GetBuffer());
					bDirOk = TRUE;
				}
			}
			if ( !bDirOk )
			{
				::memset(szPath, 0, sizeof(CHAR) * bufsz);
				// get current dir
				if ( GetCurrentDirectoryA(bufsz, szPath) != 0 )
				{
					m_strAppDirectory.SetString(szPath);
					// add seperator
					m_strAppDirectory += _T('\\');
					// save path
					strAppPath = std::string(m_strAppDirectory.GetBuffer());
				}
			}
		}

		void UBaseDriver::CreateRenderSystem( URenderer nType /*= eRS_OPENGL*/ )
		{
			char szColorMode[512];

			if( nType == eRS_OPENGL )
			{
				sprintf_s(szColorMode, "%dx%d", m_nScrWidth, m_nScrHeight);
				// OpenGL RenderSystem
				m_renderSystem = m_Root->getRenderSystemByName("OpenGL Rendering Subsystem");		
				m_renderSystem->setConfigOption("Fixed Pipeline Enabled", "No");
				m_renderSystem->setConfigOption("Full Screen", "No");
				m_renderSystem->setConfigOption("Video Mode", Ogre::String(szColorMode));
				m_renderSystem->setConfigOption("RTT Preferred Mode", "FBO");
				m_renderSystem->setConfigOption("FSAA", "8");
				m_renderSystem->setConfigOption("Colour Depth", "16");
				m_renderSystem->setConfigOption("VSync Interval", "1");
				m_renderSystem->setConfigOption("VSync", "No");
				m_Root->setRenderSystem(m_renderSystem);
			}
			else if( nType == eRS_DIRECT11)
			{
				// [Direct3D11 Rendering Subsystem]
				m_renderSystem = m_Root->getRenderSystemByName("Direct3D11 Rendering Subsystem");
				m_renderSystem->setConfigOption("Video Mode", Ogre::String(szColorMode));
				m_renderSystem->setConfigOption("Driver type", "Hardware");
				m_renderSystem->setConfigOption("FSAA", "8");
				m_renderSystem->setConfigOption("Floating-point mode", "Fastest");
				m_renderSystem->setConfigOption("VSync Interval", "1");
				m_renderSystem->setConfigOption("VSync", "No");

				m_Root->setRenderSystem(m_renderSystem);
			}
			else if( nType == eRS_DIRECT9 )
			{
				sprintf_s(szColorMode, "%dx%d @ %d colors", m_nScrWidth, m_nScrHeight, m_nPixFormat);
				// [Direct3D9 Rendering Subsystem]
				m_renderSystem = m_Root->getRenderSystemByName("Direct3D9 Rendering Subsystem");
				m_renderSystem->setConfigOption("Fixed Pipeline Enabled", "Yes");
				m_renderSystem->setConfigOption("FSAA", "16");
				m_renderSystem->setConfigOption("Use Multihead", "True");
				//m_renderSystem->setConfigOption("Floating-point mode", "Fastest");
				m_renderSystem->setConfigOption("VSync Interval", "1");
				m_renderSystem->setConfigOption("VSync", "Yes");
				m_renderSystem->setConfigOption("Video Mode", Ogre::String(szColorMode));
				//m_renderSystem->setConfigOption("Allow NVPerfHUD", "Yes");
				//m_renderSystem->setConfigOption("Colour Depth", "32");
				m_Root->setRenderSystem(m_renderSystem);
			}
			Ogre::ConfigOptionMap mapOp = m_renderSystem->getConfigOptions();
		}

		void UBaseDriver::SetDisplayMode( int nWidth, int nHeightm, int nPixelFormat )
		{
			m_nPixFormat = nPixelFormat;
			m_nScrWidth = nWidth;
			m_nScrHeight = nHeightm;
		}

		void UBaseDriver::RenderAllView()
		{
			std::list<UBaseView*>::iterator iter = m_ChildView.begin();
			for(  ;  iter != m_ChildView.end(); iter++)
			{
				UBaseView * pView = *iter;
				if( pView != NULL)
				{					
					pView->RenderScene();
		
				}
			}
		}

	}
}
