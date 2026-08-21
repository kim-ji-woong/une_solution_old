#include "StdAfx.h"
#include "UDB.h"
#include "UBaseView.h"
#include "UPolygon.h"
#include "DynamicLines.h"

#include "Ogre.h"
#include "OgreEntity.h"

#include "Poly2Tri.h"
#include "OgreException.h"

#include "UEffectObject.h"

namespace UnE
{
	namespace Core
	{
		UPolygon::UPolygon(UBaseView* pView)
		: UCoreObject()
		{	
			m_fHeight = 0.0f;
			m_pView = pView;
			m_bFirstLoad = true;
			
			
			char buf[512];
			sprintf(buf, "%d#Polygons_%d", (int)(pView->GetHWnd()), this->mID);

			pInternal = NULL;
			mObjName = std::string(buf);
			std::string szNodeName = mObjName  + "_Node";
			mTypeName = "UPolygon";	
			
			WndCtx * pCtx = GetWndContext(pView->GetHWnd());
			if( pCtx != NULL)
			{
				DynamicLines *lines = new DynamicLines(Ogre::RenderOperation::OT_LINE_STRIP);
				lines->update();
				Ogre::SceneNode *linesNode = pCtx->sceneMgr->getRootSceneNode()->createChildSceneNode(szNodeName);
				linesNode->attachObject(lines);
				SetInternal(linesNode);

				sprintf(buf, "%d#Polygons_%d_Poly", (int)(pView->GetHWnd()), this->mID);
				Ogre::SceneNode * polygonNode = linesNode->createChildSceneNode(std::string(buf));
			}	

			UObjectManager * pObjManager = UDB::GetObjectManger(m_pView->GetHWnd());
			pObjManager->AddUObject(this);
		}

		UPolygon::~UPolygon(void)
		{
			m_vecPoints.clear();
		}

		void UPolygon::addPoint( const UnE::Math::Vector3 &p )
		{
			m_fHeight = p.y;
			
			m_vecPoints.push_back(p);
		}

		void UPolygon::addPoint( Real x, Real y, Real z )
		{
			m_fHeight = y;
			m_vecPoints.push_back(UnE::Math::Vector3(x, y, z));			
		}

		void UPolygon::setPoint( unsigned int index, const UnE::Math::Vector3 &value )
		{
			if( index >= m_vecPoints.size())
				return;

			std::vector<UnE::Math::Vector3>::iterator iter = m_vecPoints.begin();
			
			iter += index;
			if( iter != m_vecPoints.end())
			{
				m_fHeight = value.y;
				m_vecPoints.insert(iter, value);
			}			
		}

		UnE::Math::Vector3 UPolygon::getPoint( unsigned short index )
		{
			return m_vecPoints[index];
		}

		unsigned int UPolygon::getNumPoints( void ) const
		{
			return m_vecPoints.size();
		}

		void UPolygon::clear()
		{
			m_vecPoints.clear();
		}

		void UPolygon::update()
		{
			WndCtx * pCtx = GetWndContext(m_pView->GetHWnd());
			if( pCtx != NULL)
			{
				Ogre::SceneNode *lnode = (Ogre::SceneNode*)(pInternal);
				DynamicLines *lines = dynamic_cast<DynamicLines*>(lnode->getAttachedObject(0));
				
				if (lines->getNumPoints() != ( m_vecPoints.size() + 1))
				{
					lines->clear();
					for (int i = 0 ; i< m_vecPoints.size(); ++i)
					{
						Ogre::Vector3 vec = Ogre::Vector3(m_vecPoints[i].x, m_vecPoints[i].y, m_vecPoints[i].z);
						lines->addPoint(vec);
					}	
					Ogre::Vector3 vec = Ogre::Vector3(m_vecPoints[0].x, m_vecPoints[0].y, m_vecPoints[0].z);
					lines->addPoint(vec);
				}
				else
				{					
					for (int i = 0; i < m_vecPoints.size(); ++i)
					{
						Ogre::Vector3 vec = Ogre::Vector3(m_vecPoints[i].x, m_vecPoints[i].y, m_vecPoints[i].z);
						lines->setPoint(i, vec);
					}
					Ogre::Vector3 vec = Ogre::Vector3(m_vecPoints[0].x, m_vecPoints[0].y, m_vecPoints[0].z);
					lines->addPoint(vec);
				}
				lines->update();
				lnode->needUpdate();


				CreateNode();
			}
		}

		Ogre::Entity* CreatePolygonMesh(Ogre::SceneManager * pSceneMgr,  std::string szMeshName, std::vector<p2t::Triangle*>& triangles, float fheight)
		{
			if( szMeshName == "")
				szMeshName = std::string("Polygon");

			Ogre::String lManualObjectName = szMeshName + "_CubeWithAxes";
			Ogre::String lNameOfTheMesh = szMeshName + "_MeshCubeAndAxe";
			Ogre::String lNameOfLight = szMeshName + "_MainLight";
			Ogre::String lNameOfMaterial = "Polygon_Material";
	
			//////////////////////////////////////////////////////////
			// Set Cube Material
			Ogre::String lResourceGroup = "Popular";
			
			if(!Ogre::MaterialManager::getSingleton().resourceExists(lNameOfMaterial))
			{
				Ogre::MaterialPtr myPathMaterial = Ogre::MaterialManager::getSingleton().create(lNameOfMaterial,lResourceGroup); 
				myPathMaterial->setReceiveShadows(false); 
				//myPathMaterial->setDepthCheckEnabled(false);
				myPathMaterial->getTechnique(0)->setLightingEnabled(false); 
				myPathMaterial->getTechnique(0)->setDepthCheckEnabled(false); 
				myPathMaterial->getTechnique(0)->setSceneBlending(Ogre::SceneBlendType::SBT_TRANSPARENT_ALPHA);
				myPathMaterial->getTechnique(0)->getPass(0)->setDiffuse(Ogre::ColourValue(1.0f, 0.0f, 0.0f, 0.5f));
				myPathMaterial->getTechnique(0)->getPass(0)->setAmbient(0.9f,0.9f,0.9f);	

			}	
				

			Ogre::AxisAlignedBox mBoundingBox;
			float lSize = 0.4f;
			Ogre::ManualObject* lManualObject = NULL;
			{
				lManualObject = pSceneMgr->createManualObject(lManualObjectName);
				bool lDoIWantToUpdateItLater = false;
				lManualObject->setDynamic(lDoIWantToUpdateItLater);
				int nIdx = 0;
					
				lManualObject->begin(lNameOfMaterial, Ogre::RenderOperation::OT_TRIANGLE_LIST);
				{						
					int nIdx = 0;
					for(size_t i = 0; i < triangles.size(); i++)
					{
						p2t::Triangle * tria = triangles[i];				
						for(size_t j = 0; j < 3; j++)
						{
							p2t::Point * pt = tria->GetPoint(j);					

							lManualObject->position(pt->x, fheight, pt->y);
							lManualObject->colour(Ogre::ColourValue(1.0f, 0.0f,0.0f,0.5f));
								
							mBoundingBox.merge(Ogre::Vector3(pt->x, fheight, pt->y));
							nIdx++;
						}

						lManualObject->triangle(nIdx-1, nIdx-2,  nIdx -3);
					}
				}
				lManualObject->end();
			}

			Ogre::MeshPtr pMesh = lManualObject->convertToMesh(szMeshName);
			pMesh->_setBounds(mBoundingBox);
			pMesh->_setBoundingSphereRadius(mBoundingBox.getHalfSize().length());

			Ogre::MaterialPtr pMatBox = Ogre::MaterialManager::getSingleton().load(lNameOfMaterial,lResourceGroup );
				
			Ogre::Entity* lEntity = pSceneMgr->createEntity(szMeshName, pMesh);

			Ogre::MeshManager::getSingleton().remove(pMesh->getName());
			pSceneMgr->destroyManualObject(lManualObjectName);

			lEntity->setMaterial(pMatBox);
			RenderableContext entityContext;
			entityContext.ignoreViewDetail = false;
			lEntity->setUserAny(Ogre::Any(entityContext));
			lEntity->getSubEntity(0)->setUserAny(Ogre::Any(entityContext));	
					
			return lEntity;
			
		}
	
		void UPolygon::CreateNode()
		{
			
			std::vector<p2t::Point*> polyline;
			for (int i = 0 ; i< m_vecPoints.size(); ++i)
			{
				polyline.push_back(new p2t::Point(m_vecPoints[i].x, m_vecPoints[i].z));
			}

			p2t::CDT* cdt = new p2t::CDT(polyline);
			cdt->Triangulate();
			std::vector<p2t::Triangle*> triangles;
			triangles = cdt->GetTriangles();

			WndCtx * pCtx = GetWndContext(m_pView->GetHWnd());
			if( pCtx != NULL)
			{
				char buf[512];
				Ogre::SceneNode *lnode = (Ogre::SceneNode*)(pInternal);
				
				//sprintf(buf, "%d#Polygons_%d_Poly", (int)(m_pView->GetHWnd()), this->mID);
				sprintf(buf, "%d#Polygons_%d_Poly", (int)(m_pView->GetHWnd()), this->mID);
				Ogre::SceneNode * polygonNode = dynamic_cast<Ogre::SceneNode*>(lnode->getChild(std::string(buf)));
				

				try
				{
					if(m_bFirstLoad == false)
					{
						
						Ogre::MovableObject * mvo = polygonNode->detachObject(mObjName);
						if(mvo != NULL)
						{					
							OGRE_DELETE mvo;
						}	
					}	
					else
					{
						m_bFirstLoad = false;
					}
				}
				catch (Ogre::ItemIdentityException* e)
				{				
				}		


				if(triangles.size() > 0)
				{
					Ogre::Entity * pEntity = CreatePolygonMesh(pCtx->sceneMgr, mObjName, triangles, m_fHeight);
					polygonNode->attachObject(pEntity);
				}				
				polygonNode->needUpdate();
			}
			
		}



		void UPolygon::SetVisible( bool bShow )
		{
			Ogre::SceneNode *lnode = (Ogre::SceneNode*)(pInternal);
			lnode->setVisible(bShow, true);
		}

		//////////////////////////////////////////////////////////////////////////
		// USpaceVolume

		USpaceVolume::USpaceVolume( UBaseView* pView )
			: UCoreObject()
		{
			m_pEffect = NULL;

			m_R = 1.0f;
			m_G = 0.0f;
			m_B = 0.0f;

			m_pView = pView;
			m_bFirstLoad = true;
			m_fHeight = 10.0f;

			char buf[512];
			sprintf(buf, "%d#SpaceVolume_%d", (int)(pView->GetHWnd()), this->mID);

			pInternal = NULL;
			mObjName = std::string(buf);
			std::string szNodeName = mObjName  + "_Node";
			mTypeName = "USpaceVolume";	

			WndCtx * pCtx = GetWndContext(pView->GetHWnd());
			if( pCtx != NULL)
			{				
				Ogre::SceneNode *linesNode = pCtx->sceneMgr->getRootSceneNode()->createChildSceneNode(szNodeName);
				SetInternal(linesNode);	
			}	
			UObjectManager * pObjManager = UDB::GetObjectManger(m_pView->GetHWnd());
			pObjManager->AddUObject(this);
		}

		USpaceVolume::~USpaceVolume( void )
		{
		}
		
		Ogre::Entity* CreateVolumeMesh(std::vector<UnE::Math::Vector3> ptList, Ogre::SceneManager * pSceneMgr,  std::string szMeshName, std::vector<p2t::Triangle*>& triangles, float fY, float fHeight, Ogre::ColourValue color)
		{
			if( szMeshName == "")
				szMeshName = std::string("SpaceVolume");

			Ogre::String lManualObjectName = szMeshName + "_CubeWithAxes";
			Ogre::String lNameOfTheMesh = szMeshName + "_MeshCubeAndAxe";
			Ogre::String lNameOfLight = szMeshName + "_MainLight";
			Ogre::String lNameOfMaterial = "SpaceVolume_Material";

			//////////////////////////////////////////////////////////
			// Set Cube Material
			Ogre::String lResourceGroup = "Popular";

			if(!Ogre::MaterialManager::getSingleton().resourceExists(lNameOfMaterial))
			{
				Ogre::MaterialPtr myPathMaterial = Ogre::MaterialManager::getSingleton().create(lNameOfMaterial,lResourceGroup); 
				myPathMaterial->setReceiveShadows(false); 
				//myPathMaterial->setDepthCheckEnabled(false);
				myPathMaterial->getTechnique(0)->setLightingEnabled(false); 
				myPathMaterial->getTechnique(0)->setDepthCheckEnabled(false); 
				myPathMaterial->getTechnique(0)->setSceneBlending(Ogre::SceneBlendType::SBT_TRANSPARENT_ALPHA);
				myPathMaterial->getTechnique(0)->getPass(0)->setDiffuse(color);
				myPathMaterial->getTechnique(0)->getPass(0)->setAmbient(0.9f,0.9f,0.9f);
				myPathMaterial->getTechnique(0)->getPass(0)->setCullingMode(Ogre::CullingMode::CULL_NONE);
			}

			Ogre::AxisAlignedBox mBoundingBox;
			float lSize = 0.4f;
			Ogre::ManualObject* lManualObject = NULL;
			{
				lManualObject = pSceneMgr->createManualObject(lManualObjectName);
				bool lDoIWantToUpdateItLater = false;
				lManualObject->setDynamic(lDoIWantToUpdateItLater);
				int nIdx = 0;

				lManualObject->begin(lNameOfMaterial, Ogre::RenderOperation::OT_TRIANGLE_LIST);
				{						
					int nIdx = 0;
					for(size_t i = 0; i < triangles.size(); i++)
					{
						p2t::Triangle * tria = triangles[i];				
						for(size_t j = 0; j < 3; j++)
						{
							p2t::Point * pt = tria->GetPoint(j);					

							lManualObject->position(pt->x, fY, pt->y);
							lManualObject->colour(color);

							mBoundingBox.merge(Ogre::Vector3(pt->x, fY, pt->y));
							nIdx++;
						}
						lManualObject->triangle(nIdx-3, nIdx-2,  nIdx -1);						
					}

					for(size_t i = 0; i < triangles.size(); i++)
					{
						p2t::Triangle * tria = triangles[i];				
						for(size_t j = 0; j < 3; j++)
						{
							p2t::Point * pt = tria->GetPoint(j);					

							lManualObject->position(pt->x, fY + fHeight, pt->y);
							lManualObject->colour(color);

							mBoundingBox.merge(Ogre::Vector3(pt->x, fY + fHeight, pt->y));
							nIdx++;
						}
						lManualObject->triangle(nIdx-1, nIdx-2,  nIdx -3);						
					}

					for( int i = 0 ; i < ptList.size(); i++)
					{
						UnE::Math::Vector3& vec1 = ptList[i];

						if( i == (ptList.size() - 1))
						{
							UnE::Math::Vector3& vec2 = ptList[0];
							
							lManualObject->position(vec1.x, fY + fHeight, vec1.z);
							lManualObject->colour(color);
							nIdx++;
							lManualObject->position(vec2.x, fY + fHeight, vec2.z);
							lManualObject->colour(color);
							nIdx++;
							
							lManualObject->position(vec2.x, fY, vec2.z);
							lManualObject->colour(color);
							nIdx++;
							lManualObject->position(vec1.x, fY, vec1.z);
							lManualObject->colour(color);
							nIdx++;
							lManualObject->triangle(nIdx-4, nIdx-2,  nIdx - 1);
							lManualObject->triangle(nIdx-4, nIdx-3,  nIdx - 2);	
						}
						else
						{
							UnE::Math::Vector3& vec2 = ptList[i + 1];
							lManualObject->position(vec1.x, fY + fHeight, vec1.z);
							lManualObject->colour(color);
							nIdx++;
							lManualObject->position(vec2.x, fY + fHeight, vec2.z);
							lManualObject->colour(color);
							nIdx++;
							
							lManualObject->position(vec2.x, fY, vec2.z);
							lManualObject->colour(color);
							nIdx++;
							lManualObject->position(vec1.x, fY, vec1.z);
							lManualObject->colour(color);
							nIdx++;
							lManualObject->triangle(nIdx-4, nIdx-2,  nIdx -1);
							lManualObject->triangle(nIdx-4, nIdx-3,  nIdx - 2);	
						}
					}
				}
				lManualObject->end();
			}
	
			std::string strMeshName = szMeshName + "_Mesh";
			std::string strEntityName = szMeshName + "_Entity";
			Ogre::MeshPtr pMesh = lManualObject->convertToMesh(strMeshName);
		
			
			pMesh->_setBounds(mBoundingBox);
			pMesh->_setBoundingSphereRadius(mBoundingBox.getHalfSize().length());

			Ogre::MaterialPtr pMatBox = Ogre::MaterialManager::getSingleton().load(lNameOfMaterial,lResourceGroup );

			Ogre::Entity* lEntity = pSceneMgr->createEntity(strEntityName, pMesh);
		

			//Ogre::MeshManager::getSingleton().remove(pMesh->getName());
			pSceneMgr->destroyManualObject(lManualObjectName);

			lEntity->setMaterial(pMatBox);
			RenderableContext entityContext;
			entityContext.ignoreViewDetail = false;
			lEntity->setUserAny(Ogre::Any(entityContext));
			lEntity->getSubEntity(0)->setUserAny(Ogre::Any(entityContext));	

			return lEntity;

		}

		void USpaceVolume::CreateVolume( UPolygon* polygon, float yPos, float fHeight )
		{
			if( polygon!= NULL && m_pView != NULL)
			{
				WndCtx * pCtx = GetWndContext(m_pView->GetHWnd());
				if( pCtx != NULL && m_bFirstLoad == true)
				{
					std::vector<p2t::Point*> polyline;
					for (int i = 0 ; i< polygon->m_vecPoints.size() - 1; ++i)
					{
						polyline.push_back(new p2t::Point(polygon->m_vecPoints[i].x, polygon->m_vecPoints[i].z));
					}
					try
					{
						p2t::CDT* cdt = new p2t::CDT(polyline);
						cdt->Triangulate();
						std::vector<p2t::Triangle*> triangles;
						triangles = cdt->GetTriangles();

						if(triangles.size() > 0)
						{
							Ogre::SceneNode *polygonNode = (Ogre::SceneNode*)(pInternal);
							Ogre::Entity * pEntity = CreateVolumeMesh(polygon->m_vecPoints, pCtx->sceneMgr, mObjName, triangles, polygon->m_fHeight, fHeight, Ogre::ColourValue(m_R, m_G, m_B, 0.5f));
							polygonNode->attachObject(pEntity);

							//m_pEffect = new UEffectObject(m_pView, polygonNode, pEntity);
							//m_pEffect->SetHighlightColor(Ogre::ColourValue(1.0f,0.0f, 0.0f));					
							//m_pEffect->SetGlowSize(true);
							//polygonNode->setVisible(false, true);
						}
					}				
					catch (std::exception* e)
					{
					}
					
					

					m_bFirstLoad = false;
				}
			}			
		}

		void USpaceVolume::SetVisible( bool bShow )
		{
			if( m_pEffect != NULL)
			{
				if(m_pEffect->GetVisible() == true)
				{
					m_pEffect->SetVisible(bShow);
				}
			}
			Ogre::SceneNode *lnode = (Ogre::SceneNode*)(pInternal);
			lnode->setVisible(bShow, true);

			if( bShow == true && m_pEffect != NULL)
			{
				//m_pEffect->SelectComponent(CSO_NOTHING);
				m_pEffect->ShowObject(true);
				//m_pEffect->Caution(true);
			}
		}

		void USpaceVolume::SetColor(int r, int g, int b)
		{
			m_R = r;
			m_G = g;
			m_B = b;
		}

		
	}
}