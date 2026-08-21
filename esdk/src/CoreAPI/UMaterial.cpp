#include "StdAfx.h"

//////////////////////////////////////////////////////////////////////////
// System header
#include <list>
#include <map>
#include <string>
#include <vector>

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

#include <OgreFont.h>
#include <OgreFontManager.h>

#include <OgreOverlay.h>
#include <OgreException.h>

//////////////////////////////////////////////////////////////////////////

#include "UMaterial.h"



namespace UnE
{
	namespace Core
	{
		//-----------------------------------------------------------------------
		static std::string UMATERIAL_GENERAL_RESOURCE_GROUP		= Ogre::ResourceGroupManager::DEFAULT_RESOURCE_GROUP_NAME;
		static std::string UMATERIAL_WIREFRAME_RESOURCE_GROUP	= "WireFrame";
		static std::string UMATERIAL_HIDDEN_RESOURCE_GROUP		= "HiddenLine";
		static std::string UMATERIAL_POINT_RESOURCE_GROUP		= "Point";
		static std::string UMATERIAL_SELECT_RESOURCE_GROUP		= "Selected";
		//-----------------------------------------------------------------------
		class MaterialManagerInternal 
		{
		public:
			
			std::vector<std::string> mUseMaterial;

			
			
		public:
			MaterialManagerInternal();
			~MaterialManagerInternal();
			
			void AddMaterialName(std::string szMatName)
			{
				mUseMaterial.push_back(szMatName);
			}
			void IsUsingName(std::string szMatName)
			{
				
			}
			
		};
		
		//-----------------------------------------------------------------------
		MaterialManagerInternal::MaterialManagerInternal()
		{
		}
		//-----------------------------------------------------------------------
		MaterialManagerInternal::~MaterialManagerInternal()
		{
		}
		//-----------------------------------------------------------------------
		MaterialManagerInternal gInternalManager;		
		UMaterialManager UMaterialManager::s_cInstance;

		//-----------------------------------------------------------------------
		UMaterialManager::UMaterialManager(void)
		{
			m_bInit = false;
			m_bResLoadComplete = false;
		}

		//-----------------------------------------------------------------------
		UMaterialManager::~UMaterialManager(void)
		{
		}

		//-----------------------------------------------------------------------
		bool UMaterialManager::CreateMaterial( std::string szMatName )
		{
			if( m_bInit == false)
			{
				m_bInit = true;
				Ogre::ResourceGroupManager::getSingleton().createResourceGroup(UMATERIAL_WIREFRAME_RESOURCE_GROUP);
				Ogre::ResourceGroupManager::getSingleton().createResourceGroup(UMATERIAL_HIDDEN_RESOURCE_GROUP);
				Ogre::ResourceGroupManager::getSingleton().createResourceGroup(UMATERIAL_POINT_RESOURCE_GROUP);
				Ogre::ResourceGroupManager::getSingleton().createResourceGroup(UMATERIAL_SELECT_RESOURCE_GROUP);
				Ogre::ResourceGroupManager::getSingleton().initialiseResourceGroup(UMATERIAL_WIREFRAME_RESOURCE_GROUP);
				Ogre::ResourceGroupManager::getSingleton().initialiseResourceGroup(UMATERIAL_HIDDEN_RESOURCE_GROUP);
				Ogre::ResourceGroupManager::getSingleton().initialiseResourceGroup(UMATERIAL_POINT_RESOURCE_GROUP);
				Ogre::ResourceGroupManager::getSingleton().initialiseResourceGroup(UMATERIAL_SELECT_RESOURCE_GROUP);
			}

			Ogre::String lNameOfMaterial = szMatName;
			if( Ogre::MaterialManager::getSingleton().resourceExists(lNameOfMaterial) == true)
				return false;

			Ogre::MaterialPtr ptrMatNormal	= Ogre::MaterialManager::getSingleton().create(lNameOfMaterial, UMATERIAL_GENERAL_RESOURCE_GROUP);
			if(! ptrMatNormal.isNull())
			{
				// make noraml material
				// DELETE DEFAULT PASS
				ptrMatNormal->getTechnique(0)->removeAllPasses();

				// Create One Pass
				ptrMatNormal->getTechnique(0)->createPass();
				// no shadow
				ptrMatNormal->setReceiveShadows(false); 
				// Color Mode enable
				//ptrMatNormal->getTechnique(0)->setLightingEnabled(false); 
				//ptrMatNormal->getTechnique(0)->setDepthCheckEnabled(true);
				//ptrMatNormal->getTechnique(0)->setDepthWriteEnabled(false);
				//ptrMatNormal->getTechnique(0)->setCullingMode(Ogre::CULL_NONE);
			}
			return true;
			
			Ogre::MaterialPtr ptrMatWire = Ogre::MaterialManager::getSingleton().create(lNameOfMaterial, UMATERIAL_WIREFRAME_RESOURCE_GROUP);
			if(! ptrMatWire.isNull())
			{

				// make wireframe material
				ptrMatWire->getTechnique(0)->removeAllPasses();

				ptrMatWire->getTechnique(0)->createPass();
				ptrMatWire->getTechnique(0)->getPass(0)->setAmbient(Ogre::ColourValue(0.2f, 0.2f, 0.2f, 1.0));
				ptrMatWire->getTechnique(0)->getPass(0)->setDiffuse(Ogre::ColourValue(0.2f, 0.2f, 0.2f, 1.0));
				ptrMatWire->getTechnique(0)->getPass(0)->setLightingEnabled(false);
				ptrMatWire->getTechnique(0)->getPass(0)->setDepthCheckEnabled(true);
				ptrMatWire->getTechnique(0)->getPass(0)->setDepthWriteEnabled(true);
				ptrMatWire->getTechnique(0)->getPass(0)->setCullingMode(Ogre::CULL_NONE);
				ptrMatWire->getTechnique(0)->getPass(0)->setPolygonMode(Ogre::PM_WIREFRAME);
				ptrMatWire->getTechnique(0)->getPass(0)->setDepthFunction(Ogre::CMPF_LESS_EQUAL);
				ptrMatWire->getTechnique(0)->getPass(0)->setDepthBias(10);

				ptrMatWire->getTechnique(0)->createPass();
				ptrMatWire->getTechnique(0)->getPass(1)->setAmbient(Ogre::ColourValue(0.2f, 0.2f, 0.2f, 1.0));
				ptrMatWire->getTechnique(0)->getPass(1)->setDiffuse(Ogre::ColourValue(0.2f, 0.2f, 0.2f, 1.0));
				ptrMatWire->getTechnique(0)->getPass(1)->setLightingEnabled(true);
				ptrMatWire->getTechnique(0)->getPass(1)->setDepthCheckEnabled(true);
				ptrMatWire->getTechnique(0)->getPass(1)->setDepthWriteEnabled(true);
				ptrMatWire->getTechnique(0)->getPass(1)->setCullingMode(Ogre::CULL_NONE);
				ptrMatWire->getTechnique(0)->getPass(1)->setPolygonMode(Ogre::PM_POINTS);
				ptrMatWire->getTechnique(0)->getPass(1)->setDepthFunction(Ogre::CMPF_LESS_EQUAL);
				ptrMatWire->getTechnique(0)->getPass(1)->setDepthBias(11);
				ptrMatWire->getTechnique(0)->getPass(1)->setPointSize(5.0f);
				
			}

			Ogre::MaterialPtr ptrMatHidden	= Ogre::MaterialManager::getSingleton().create(lNameOfMaterial, UMATERIAL_HIDDEN_RESOURCE_GROUP);
			if(! ptrMatHidden.isNull())
			{
				// make hidden material
				ptrMatHidden->getTechnique(0)->removeAllPasses();

				ptrMatHidden->getTechnique(0)->createPass();
				ptrMatHidden->getTechnique(0)->getPass(0)->setAmbient(Ogre::ColourValue(0.2f, 0.2f, 0.2f, 0.0));
				ptrMatHidden->getTechnique(0)->getPass(0)->setDiffuse(Ogre::ColourValue(0.2f, 0.2f, 0.2f, 0.0));
				ptrMatHidden->getTechnique(0)->getPass(0)->setLightingEnabled(true);
				ptrMatHidden->getTechnique(0)->getPass(0)->setDepthCheckEnabled(true);
				ptrMatHidden->getTechnique(0)->getPass(0)->setDepthWriteEnabled(true);
				ptrMatHidden->getTechnique(0)->getPass(0)->setCullingMode(Ogre::CULL_NONE);
				ptrMatHidden->getTechnique(0)->getPass(0)->setPolygonMode(Ogre::PM_SOLID);
				ptrMatHidden->getTechnique(0)->getPass(0)->setDepthFunction(Ogre::CMPF_LESS_EQUAL);
				ptrMatHidden->getTechnique(0)->getPass(0)->setDepthBias(10);

				ptrMatHidden->getTechnique(0)->createPass();
				ptrMatHidden->getTechnique(0)->getPass(1)->setAmbient(Ogre::ColourValue(0.2f, 0.2f, 0.2f, 1.0));
				ptrMatHidden->getTechnique(0)->getPass(1)->setDiffuse(Ogre::ColourValue(0.2f, 0.2f, 0.2f, 1.0));
				ptrMatHidden->getTechnique(0)->getPass(1)->setLightingEnabled(false);
				ptrMatHidden->getTechnique(0)->getPass(1)->setDepthCheckEnabled(true);
				ptrMatHidden->getTechnique(0)->getPass(1)->setDepthWriteEnabled(true);
				ptrMatHidden->getTechnique(0)->getPass(1)->setCullingMode(Ogre::CULL_NONE);
				ptrMatHidden->getTechnique(0)->getPass(1)->setPolygonMode(Ogre::PM_WIREFRAME);
				ptrMatHidden->getTechnique(0)->getPass(1)->setDepthFunction(Ogre::CMPF_LESS_EQUAL);
				ptrMatHidden->getTechnique(0)->getPass(1)->setDepthBias(11);
			}

			Ogre::MaterialPtr ptrMatSelected	= Ogre::MaterialManager::getSingleton().create(lNameOfMaterial, UMATERIAL_SELECT_RESOURCE_GROUP);
			if(! ptrMatSelected.isNull())
			{
				// make selected material
				ptrMatSelected->getTechnique(0)->removeAllPasses();				
				ptrMatSelected->getTechnique(0)->createPass();
				ptrMatSelected->getTechnique(0)->getPass(0)->setLightingEnabled(true);
				ptrMatSelected->getTechnique(0)->getPass(0)->setDiffuse(Ogre::ColourValue(1.0f, 0.0f, 0.0f, 1.0f));
				ptrMatSelected->getTechnique(0)->getPass(0)->setAmbient(Ogre::ColourValue(1.0f, 0.0f, 0.0f, 1.0f));
			}

			Ogre::MaterialPtr ptrMatPoints	= Ogre::MaterialManager::getSingleton().create(lNameOfMaterial, UMATERIAL_POINT_RESOURCE_GROUP);
			if(! ptrMatPoints.isNull())
			{
				// make points material
			}	

			//gInternalManager.AddMaterialName(szMatName);
			return true;

		}
		//-----------------------------------------------------------------------
		bool UMaterialManager::RemoveMaterial( std::string szMatName )
		{
			Ogre::String lNameOfMaterial = szMatName;
			if( Ogre::MaterialManager::getSingleton().resourceExists(lNameOfMaterial) == false)
				return false;
			 
			 Ogre::ResourcePtr ptrMat	= Ogre::MaterialManager::getSingleton().getByName(lNameOfMaterial, UMATERIAL_GENERAL_RESOURCE_GROUP);

			 if( ! ptrMat.isNull())
			 {
				 Ogre::MaterialManager::getSingleton().remove(ptrMat);
			 }
			 ptrMat	= Ogre::MaterialManager::getSingleton().getByName(lNameOfMaterial, UMATERIAL_WIREFRAME_RESOURCE_GROUP);
			 if( ! ptrMat.isNull())
			 {
				 Ogre::MaterialManager::getSingleton().remove(ptrMat);
			 }
			 ptrMat	= Ogre::MaterialManager::getSingleton().getByName(lNameOfMaterial, UMATERIAL_HIDDEN_RESOURCE_GROUP);
			 if( ! ptrMat.isNull())
			 {
				 Ogre::MaterialManager::getSingleton().remove(ptrMat);
			 }
			 ptrMat	= Ogre::MaterialManager::getSingleton().getByName(lNameOfMaterial, UMATERIAL_POINT_RESOURCE_GROUP);
			 if( ! ptrMat.isNull())
			 {
				 Ogre::MaterialManager::getSingleton().remove(ptrMat);
			 }
			 ptrMat	= Ogre::MaterialManager::getSingleton().getByName(lNameOfMaterial, UMATERIAL_SELECT_RESOURCE_GROUP);
			 if( ! ptrMat.isNull())
			 {
				 Ogre::MaterialManager::getSingleton().remove(ptrMat);
			 }
			 
			 
			 
			 return true;
		}
		//-----------------------------------------------------------------------
		bool UMaterialManager::Clear()
		{
			m_bInit = false;
			m_bResLoadComplete = false;
			return true;
		}

		void UMaterialManager::LoadDefultResource()
		{
			if(m_bResLoadComplete == false)
			{

				try
				{
					// initialise all resource groups
					Ogre::ResourceGroupManager::getSingleton().initialiseAllResourceGroups();
					//Ogre::ResourceGroupManager::getSingleton().initialiseResourceGroup(ResourceGroupManager::DEFAULT_RESOURCE_GROUP_NAME);

					//if( m_WndCtx != NULL)
					//	m_WndCtx->sceneMgr->setSkyBox(true, "Examples/CloudyNoonSkyBox", 5000);

					Ogre::ResourcePtr pFont = Ogre::FontManager::getSingleton().getByName("AritaSB", "Popular");
					if (pFont.isNull())
						throw Ogre::Exception(Ogre::Exception::ERR_ITEM_NOT_FOUND, "Could not find font ", "UBaseView::AddTextPOI");

					if(!pFont->isLoaded())
						pFont->load();
				}

				catch (Ogre::Exception* e)
				{
				}

				m_bResLoadComplete = true;
			}
		}
		//-----------------------------------------------------------------------
	}
}
