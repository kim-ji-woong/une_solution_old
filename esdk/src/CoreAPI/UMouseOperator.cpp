#include "StdAfx.h"

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
#include <OgrePlaneBoundedVolume.h>

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
// UMath API
#include "UVector3.h"
#include "UQuaternion.h"
#include "URay.h"
#include "UPlane.h"
//////////////////////////////////////////////////////////////////////////
// CORE API
#include "UDB.h"
#include "UBaseModel.h"
#include "UScene.h"
#include "UObject.h"
#include "UEntity.h"
#include "UCamera.h"
#include "UMouseOperator.h"
#include "UBaseView.h"
#include "IconPOI.h"

//////////////////////////////////////////////////////////////////////////
using namespace UnE::Math;


//////////////////////////////////////////////////////////////////////////

namespace UnE
{
	namespace Core
	{
		typedef std::map<int, UnE::Core::UIconPOI*> UIconPOIList;
		extern UIconPOIList gIconPOIList;


		Ogre::SceneNode* gSphereScene = NULL;

		//-----------------------------------------------------------------------
		static Ogre::SceneNode * gNode = NULL;
		//-----------------------------------------------------------------------
		static const Ogre::Vector3& _GetWorldPosition(Ogre::Node& rNode)
		{
			return rNode._getDerivedPosition();
		}
		//-----------------------------------------------------------------------
		static const Ogre::Quaternion& _GetWorldOrientation(Ogre::Node& rNode)
		{
			return rNode._getDerivedOrientation();
		}

		
		//-----------------------------------------------------------------------
		MouseOperator::MouseOperator( HWND	hWnd )
		{
			m_hWnd = hWnd;
			m_pTargetView = NULL;

			mCamera = m_pTargetView->CreateCamera();

			m_bUseTrackBall = false;
			mOrbitRadius = 1.0f;
			m_pSelectedObject = NULL;
			m_pSelectNode = NULL;

		}
		//-----------------------------------------------------------------------
		MouseOperator::MouseOperator()
		{
			m_pTargetView = NULL;
			m_hWnd = NULL;
			m_bUseTrackBall = false;
			mCamera = NULL;
			mOrbitRadius = 1.0f; 
			m_pSelectedObject = NULL;
			m_pSelectNode = NULL;

		}

		//-----------------------------------------------------------------------
		MouseOperator::~MouseOperator( void )
		{
			//m_pTargetView->DeleteCamera(m_hWnd);
			m_pTargetView = NULL;
			m_hWnd = NULL;
			m_bUseTrackBall = false; 
			mCamera = NULL;
		}

		//-----------------------------------------------------------------------
		BOOL MouseOperator::OnLButtonDown(UINT nFlags, CPoint point)
		{
			if( m_hWnd == INVALID_HANDLE_VALUE || m_pTargetView == NULL)
				return FALSE;

			SetCapture(m_hWnd);

			if (gSphereScene == NULL)
			{
				CreateSphere();
			}
			
			Orbit(point);

			m_bMouseOrbitMode = TRUE;

			return FALSE;
		}
		//-----------------------------------------------------------------------
		BOOL MouseOperator::OnLButtonUp(UINT nFlags, CPoint point)
		{
			if( m_hWnd == NULL || m_pTargetView == NULL)
				return FALSE;

			m_bMouseOrbitMode = FALSE;

			ReleaseCapture();	

			if (gSphereScene != NULL)
			{
				gSphereScene->setVisible(false);
			}

			WndCtx * pCTX = GetWndContext(m_hWnd);
			pCTX->camera->setAutoTracking(false);
			return FALSE;
		}

		//-----------------------------------------------------------------------
		BOOL MouseOperator::OnMButtonDown(UINT nFlags, CPoint point)
		{
			if( m_hWnd == NULL || m_pTargetView == NULL)
				return FALSE;

			m_bMousePanMode = TRUE;

			SetCapture(m_hWnd);

			Pan(point);	

			return FALSE;
		}
		//-----------------------------------------------------------------------
		BOOL MouseOperator::OnMButtonUp(UINT nFlags, CPoint point)
		{
			if( m_hWnd == NULL || m_pTargetView == NULL)
				return FALSE;

			m_bMousePanMode = FALSE;

			ReleaseCapture();

			WndCtx * pCTX = GetWndContext(m_hWnd);
			pCTX->camera->setAutoTracking(false);
			return FALSE;
		}

		//-----------------------------------------------------------------------
		BOOL MouseOperator::OnMouseMove(UINT nFlags, CPoint point)
		{
			if( m_hWnd == NULL || m_pTargetView == NULL)
				return FALSE;

			if(m_bMouseOrbitMode == TRUE && (MK_LBUTTON & nFlags))
			{
				Orbit(point, true);
				m_pTargetView->RenderOneFrame();
			}
			else if( m_bMousePanMode == TRUE && (MK_MBUTTON & nFlags))
			{
				Pan(point, true);
				//UBaseView::Instance().RenderOneFrame(m_hWnd);
				m_pTargetView->RenderOneFrame();

			}
			return TRUE;
		}

		//-----------------------------------------------------------------------
		BOOL MouseOperator::OnMouseWheel(UINT nFlags, short zDelta, CPoint pt)
		{
			if( m_hWnd == NULL || m_pTargetView == NULL)
				return FALSE;

			if( zDelta != 0 )
			{
				Zoom(zDelta);
				m_pTargetView->RenderScene();
			}

			return TRUE;
		}
		//-----------------------------------------------------------------------
		BOOL MouseOperator::OnRButtonUp( UINT nFlags, CPoint point )
		{
			if( m_hWnd == NULL || m_pTargetView == NULL)
				return FALSE;

			WndCtx * pCTX = GetWndContext(m_hWnd);
			pCTX->camera->setAutoTracking(false);
			return FALSE;
		}
		//-----------------------------------------------------------------------
		void MouseOperator::SetHWnd( HWND val )
		{
			if( m_hWnd == NULL)
			{
				m_hWnd = val;
				if(mCamera == NULL)
				{
					mCamera = m_pTargetView->CreateCamera();					
				}
			}

		}
		//-----------------------------------------------------------------------
		HWND MouseOperator::GetHWnd() const
		{
			return m_hWnd;
		}
		//-----------------------------------------------------------------------
		void MouseOperator::SetOrbitCenter( UnE::Math::Vector3 vCenter )
		{
			mOrbitCenter = vCenter;
		}
		//-----------------------------------------------------------------------
		void MouseOperator::SetOrbitRadius( Real fLength )
		{
			mOrbitRadius = fLength;
		}
		//-----------------------------------------------------------------------
		void MouseOperator::Zoom(int zDelta)
		{
			float distance = (float)((-zDelta) /8.0f);
			Ogre::Vector3 transvector(0.f, 0.f, distance);
			WndCtx * pCTX = GetWndContext(m_hWnd);
			Ogre::Vector3 vOrgPos = pCTX->camera->getPosition();
			pCTX->camera->moveRelative(transvector);
			Ogre::Vector3 vPos = pCTX->camera->getPosition();
			Ogre::Vector3 vDir = pCTX->camera->getDirection();
			if( vPos.y < 10.0f )
				vPos.y = 10.0f;
			if( vDir.y < 5.0f)
			{
				int i = 0;
				i++;
			}
			/*pCTX->rootNode->needUpdate();
			Ogre::SceneNode::ChildNodeIterator it = pCTX->rootNode->getChildIterator();
			while (it.hasMoreElements())
			{
				Ogre::SceneNode* node = (Ogre::SceneNode*)it.getNext();
				if (node != pCTX->rootNode)
				{
					node->showBoundingBox(true);
					
					if (node->_getWorldAABB().intersects())
					{
						pCTX->camera->setPosition(vOrgPos);
						return;
					}

				}				
			}*/
			pCTX->camera->setPosition(vPos);
		}


		//-----------------------------------------------------------------------
		void MouseOperator::Pan( CPoint point, bool bMove )
		{
			if(bMove)
			{
				if( point != m_PtPrev)
				{
					Vector3 temp(0,0,0);
					Vector3 temp1(0,0,0);
					WndCtx * pCTX = GetWndContext(m_hWnd);
					Ogre::Ray tRay = pCTX->camera->getCameraToViewportRay(
						float(point.x) / (pCTX->viewport->getActualWidth() - 1),
						float(point.y) / (pCTX->viewport->getActualHeight() - 1));
					
					Vector3 vDir (tRay.getDirection().x , tRay.getDirection().y, tRay.getDirection().z);
					Vector3 vOrg (tRay.getOrigin().x , tRay.getOrigin().y, tRay.getOrigin().z);
					
					Ray MouseRay(vOrg, vDir);

					std::pair<bool, Real> result = MouseRay.intersects(mSelectPanPlane);
					if ( result.first && result.second < pCTX->camera->getPosition().length() * 2)
					{
						temp = MouseRay.getPoint(result.second);
						temp1=  (mSelectPanPt-temp);
						Vector3 campos = mCamera->getPosition();
						campos = campos + temp1;

						if(campos.y < 1.0f )
							campos.y = 1.0f;
						mCamera->setPosition(campos);
					}
				}
			}
			else
			{
				Vector3 vec = Vector3(0, 0, 0);
				Pick(point, vec, 1);
				SelectPanPlane(point, vec);		
			}
			m_PtPrev = point;
		}
		//-----------------------------------------------------------------------
		void MouseOperator::SelectPanPlane(CPoint point, Vector3& vec)
		{
			WndCtx * pCTX = GetWndContext(m_hWnd);
			Camera* tmpCam = mCamera;
			if(tmpCam->getProjectionType() == PT_ORTHOGRAPHIC && tmpCam->getRealDirection().y < 0.0001 && vec == Vector3::ZERO)
			{
				Ray MouseRay = tmpCam->getCameraToViewportRay(
					float(point.x) / (pCTX->viewport->getActualWidth() - 1),
					float(point.y) / (pCTX->viewport->getActualHeight() - 1));

				Plane tmpPlane(-tmpCam->getDirection(), tmpCam->getPosition() + tmpCam->getDirection() * tmpCam->getNearClipDistance() * 2);
				std::pair<bool, Real> result = MouseRay.intersects(tmpPlane);
				if ( result.first)
				{
					vec = MouseRay.getPoint(result.second);
				}
				else
				{
					vec = Vector3(0, 0, 0);
				}
			}
			SetOrbitCenter(vec);
			SetOrbitRadius((mCamera->getPosition() - mOrbitCenter).length());
			Vector3 PlaneNormal;
			PlaneNormal = mCamera->getDirection() * -1;
			mSelectPanPlane.redefine( PlaneNormal,vec);
			mSelectPanPt = vec;
		}
		//-----------------------------------------------------------------------			
		void MouseOperator::Orbit( float fAngleX, float fAngleY )
		{

			WndCtx * pCTX = GetWndContext(m_hWnd);

			if( gNode != NULL)
				pCTX->camera->setAutoTracking(true, gNode);

			Ogre::Radian yaw = Ogre::Radian(Ogre::Degree(fAngleX));
			Ogre::Radian pitch = Ogre::Radian(Ogre::Degree(fAngleY));
			Ogre::Radian roll = Ogre::Radian(Ogre::Degree(0));
			
			Ogre::Quaternion rot1;
			rot1.FromAngleAxis( yaw, Ogre::Vector3::UNIT_Y);
			pCTX->camera->yaw(yaw);
			
			Ogre::Quaternion rot2;
			rot2.FromAngleAxis(pitch, pCTX->camera->getRight());
			pCTX->camera->pitch(pitch);
	
			Ogre::Quaternion q1 = (rot1 * rot2 );
			Ogre::Vector3 center(mOrbitCenter.x, mOrbitCenter.y, mOrbitCenter.z);			
			Ogre::Vector3 MposToCam = q1 * (pCTX->camera->getPosition() - center);
			
			pCTX->camera->setPosition(MposToCam + center);
			Ogre::Vector3 vPos = pCTX->camera->getPosition();
			if( vPos.y < 1.0f )
				vPos.y = 1.0f;
			pCTX->camera->setPosition(vPos);
			pCTX->camera->setAutoTracking(false);			
		}

		//-----------------------------------------------------------------------
		void MouseOperator::Orbit( CPoint pt, bool bMove /*= false*/ )
		{
			if( m_pTargetView == NULL || mCamera == NULL)
				return;

			if(bMove)
			{
				CPoint PtDiff = pt - m_PtPrev;

				if( m_PtPrev == pt )
					return;
				//Orbit(m_PtPrev, pt);

				float pitchAngle = (-0.5f * PtDiff.y);
				float yawAngle = (-0.5f *   PtDiff.x);

				Orbit(yawAngle, pitchAngle);
			} 
			else
			{
				Vector3 vec = Vector3(0, 0, 0);		
				Pick(pt, vec, 1);		
				SetOrbitCenter(vec);

				if (gSphereScene != NULL)
				{
					gSphereScene->setPosition(Ogre::Vector3(vec.x, vec.y, vec.z));
					gSphereScene->setVisible(true);
				}

				SetOrbitRadius((mCamera->getPosition() - mOrbitCenter).length());		
			}
			m_PtPrev = pt;
		}

		//-----------------------------------------------------------------------
		BOOL MouseOperator::OnRButtonDown(UINT nFlags, CPoint point)
		{
			if( m_hWnd == NULL || m_pTargetView == NULL)
				return FALSE;

			return FALSE;
		}
		//-----------------------------------------------------------------------
		void MouseOperator::Reset()
		{
			mOrbitCenter = Vector3(0,0,0);
			mSelectedEntity.clear();
		}

		//-----------------------------------------------------------------------
		static void GetMeshInformation(const Ogre::MeshPtr mesh,
			size_t &vertex_count,
			Ogre::Vector3* &vertices,
			size_t &index_count,
			unsigned long* &indices,
			const Ogre::Vector3 &position,
			const Ogre::Quaternion &orient,
			const Ogre::Vector3 &scale)
		{

			bool added_shared = false;
			size_t current_offset = 0;
			size_t shared_offset = 0;
			size_t next_offset = 0;
			size_t index_offset = 0;

			vertex_count = index_count = 0;

			// Calculate how many vertices and indices we're going to need
			for (unsigned short i = 0; i < mesh->getNumSubMeshes(); ++i)
			{
				Ogre::SubMesh* submesh = mesh->getSubMesh( i );

				// We only need to add the shared vertices once
				if(submesh->useSharedVertices)
				{
					if( !added_shared )
					{
						vertex_count += mesh->sharedVertexData->vertexCount;
						added_shared = true;
					}
				}
				else
				{
					vertex_count += submesh->vertexData->vertexCount;
				}

				// Add the indices
				index_count += submesh->indexData->indexCount;
			}


			// Allocate space for the vertices and indices
			vertices = new Ogre::Vector3[vertex_count];
			indices = new unsigned long[index_count];

			added_shared = false;

			// Run through the submeshes again, adding the data into the arrays
			for ( unsigned short i = 0; i < mesh->getNumSubMeshes(); ++i)
			{
				Ogre::SubMesh* submesh = mesh->getSubMesh(i);

				Ogre::VertexData* vertex_data = submesh->useSharedVertices ? mesh->sharedVertexData : submesh->vertexData;

				if((!submesh->useSharedVertices)||(submesh->useSharedVertices && !added_shared))
				{
					if(submesh->useSharedVertices)
					{
						added_shared = true;
						shared_offset = current_offset;
					}

					const Ogre::VertexElement* posElem =
						vertex_data->vertexDeclaration->findElementBySemantic(Ogre::VES_POSITION);

					Ogre::HardwareVertexBufferSharedPtr vbuf =
						vertex_data->vertexBufferBinding->getBuffer(posElem->getSource());

					unsigned char* vertex =
						static_cast<unsigned char*>(vbuf->lock(Ogre::HardwareBuffer::HBL_READ_ONLY));

					// There is _no_ baseVertexPointerToElement() which takes an Ogre::Real or a double
					//  as second argument. So make it float, to avoid trouble when Ogre::Real will
					//  be comiled/typedefed as double:
					//      Ogre::Real* pReal;
					float* pReal;

					for( size_t j = 0; j < vertex_data->vertexCount; ++j, vertex += vbuf->getVertexSize())
					{
						posElem->baseVertexPointerToElement(vertex, &pReal);

						Ogre::Vector3 pt(pReal[0], pReal[1], pReal[2]);

						vertices[current_offset + j] = (orient * (pt * scale)) + position;
					}

					vbuf->unlock();
					next_offset += vertex_data->vertexCount;
				}


				Ogre::IndexData* index_data = submesh->indexData;
				size_t numTris = index_data->indexCount / 3;
				Ogre::HardwareIndexBufferSharedPtr ibuf = index_data->indexBuffer;

				bool use32bitindexes = (ibuf->getType() == Ogre::HardwareIndexBuffer::IT_32BIT);

				unsigned long*  pLong = static_cast<unsigned long*>(ibuf->lock(Ogre::HardwareBuffer::HBL_READ_ONLY));
				unsigned short* pShort = reinterpret_cast<unsigned short*>(pLong);


				size_t offset = (submesh->useSharedVertices)? shared_offset : current_offset;

				if ( use32bitindexes )
				{
					for ( size_t k = 0; k < numTris*3; ++k)
					{
						indices[index_offset++] = pLong[k] + static_cast<unsigned long>(offset);
					}
				}
				else
				{
					for ( size_t k = 0; k < numTris*3; ++k)
					{
						indices[index_offset++] = static_cast<unsigned long>(pShort[k]) +
							static_cast<unsigned long>(offset);
					}
				}

				ibuf->unlock();
				current_offset = next_offset;
			}

			if (index_count != index_offset)
				index_count = index_offset;
		}
		
		//-----------------------------------------------------------------------
		static BOOL PickQuery( HWND m_hWnd, float xpos, float ypos, OUT Ogre::RaySceneQueryResult& queryResult, 
			uint32 queryMask,
			bool bCenterPick, 
			Ogre::Ray& mPickingRay, 
			Ogre::Vector3 mPickPoint,
			Real& mPickDistance)
		{

			WndCtx * pCTX = GetWndContext(m_hWnd);
			Ogre::Camera* tCamera = pCTX->camera;
			if(bCenterPick)
			{
				mPickingRay = tCamera->getCameraToViewportRay(0.5, 0.5);
			}
			else
			{
				mPickingRay = tCamera->getCameraToViewportRay(
					xpos / (tCamera->getViewport()->getActualWidth() - 1),
					ypos / (tCamera->getViewport()->getActualHeight() - 1));
			}

			Ogre::RaySceneQuery* mRaySceneQuery = pCTX->raySceneQuery;
			mRaySceneQuery->setQueryMask(queryMask);
			mRaySceneQuery->setSortByDistance(true, 0);
			mRaySceneQuery->setRay(mPickingRay);

			queryResult.clear();	
			Ogre::RaySceneQueryResult tempResult = mRaySceneQuery->execute();
			if ( tempResult.size() <= 0 )
				return FALSE;

			Ogre::Real closest_distance = -1.0f;
			Ogre::Vector3 closest_result;
			tempResult = mRaySceneQuery->getLastResults();

			size_t resultidx;

			for (size_t qr_idx = 0; qr_idx < tempResult.size(); qr_idx++)
			{
				// stop checking if we have found a raycast hit that is closer
				// than all remaining entities
				if ((closest_distance >= 0.0f) && (closest_distance < tempResult[qr_idx].distance))
				{
					break;
				}

				// only check this result if its a hit against an entity
				if ((tempResult[qr_idx].movable != NULL) &&
					(tempResult[qr_idx].movable->getMovableType().compare("Entity") == 0) &&
					tempResult[qr_idx].movable->getVisible())
				{
					unsigned int n = tempResult[qr_idx].movable->getQueryFlags();
					// get the entity to check
					Ogre::Entity *pentity = static_cast<Ogre::Entity*>(tempResult[qr_idx].movable);           

					// mesh data to retrieve         
					size_t vertex_count;
					size_t index_count;
					Ogre::Vector3 *vertices;
					unsigned long *indices;

					// get the mesh information
					GetMeshInformation(pentity->getMesh(), vertex_count, vertices, index_count, indices,             
						//pentity->getParentNode()->getWorldPosition(),
						_GetWorldPosition(*pentity->getParentNode()),
						//pentity->getParentNode()->getWorldOrientation(),
						_GetWorldOrientation(*pentity->getParentNode()),
						pentity->getParentNode()->_getDerivedScale());

					// test for hitting individual triangles on the mesh
					bool new_closest_found = false;
					for (int i = 0; i < static_cast<int>(index_count); i += 3)
					{
						int i1 = indices[i];
						int i2 = indices[i+1];
						int i3 = indices[i+2];
						if( i1 >= vertex_count || i2 >= vertex_count || i3 >= vertex_count)
						{
							TRACE0("Index Error!!\n");
							continue;
						}
						if(vertices[indices[i+0]]==vertices[indices[i+1]])
							continue;
						if(vertices[indices[i+1]]==vertices[indices[i+2]])
							continue;
						if(vertices[indices[i+2]]==vertices[indices[i+0]])
							continue;

						// check for a hit against this triangle
						std::pair<bool, Ogre::Real> hit = Ogre::Math::intersects(mPickingRay, vertices[indices[i]],
							vertices[indices[i+1]], vertices[indices[i+2]], true, false);

						// if it was a hit check if its the closest
						if (hit.first)
						{
							Ogre::Vector3 tempNormal=Ogre::Math::calculateBasicFaceNormal  ( vertices[indices[i]],vertices[indices[i+1]], vertices[indices[i+2]]);
							Ogre::Vector3 tempPoint=mPickingRay.getPoint(hit.second);
							//TRACE("---  %f\n",hit.second);
							if (Ogre::Math::pointInTri3D  ( tempPoint, vertices[indices[i]],vertices[indices[i+1]], vertices[indices[i+2]], tempNormal ))
							{
								if ((closest_distance < 0.0f) ||
									(hit.second < closest_distance))
								{
									mPickPoint = mPickingRay.getPoint(hit.second);
									mPickDistance = hit.second;
									// this is the closest so far, save it off
									closest_distance = hit.second;
									new_closest_found = true;
								}
							}
						}
					}
					// free the verticies and indicies memory
					delete[] vertices;
					delete[] indices;

					// if we found a new closest raycast for this object, update the
					// closest_result before moving on to the next object.
					if (new_closest_found)
					{
						closest_result = mPickingRay.getPoint(closest_distance);               
						resultidx = qr_idx;
					}
				}       
			}
			
			// return the result
			if (closest_distance >= 0.0f)
			{
				// raycast success
				//result = closest_result;
				tempResult[resultidx].distance = closest_distance;
				queryResult.push_back(tempResult[resultidx]);
				return (TRUE);
			}
			else
			{
				// raycast failed
				return (FALSE);
			} 
		}

		
		BOOL MouseOperator::Pick(CPoint point, UnE::Math::Vector3& vResult,unsigned int querymask)
		{			
			Ogre::Vector3 vec;
			WndCtx * pCTX = GetWndContext(m_hWnd);
			Ogre::Camera* tCamera = pCTX->camera;
			Ogre::Ray MouseRay = tCamera->getCameraToViewportRay(
				float(point.x) / (tCamera->getViewport()->getActualWidth() - 1),
				float(point.y) / (tCamera->getViewport()->getActualHeight() - 1));

			Ogre::RaySceneQueryResult result;
			Ogre::Ray pickRay;
			Ogre::Vector3 pickPoint;
			float dPickDistance;
			if ( PickQuery(m_hWnd, (float)point.x, (float)point.y, result, querymask , false, pickRay, pickPoint, dPickDistance) == TRUE )
			{
				for ( Ogre::RaySceneQueryResult::iterator i = result.begin() ; i != result.end() ; ++i )
				{
					//i->movable->
					vec = pickRay.getPoint(i->distance);
					gNode = i->movable->getParentSceneNode();


					vResult.x = vec.x;
					vResult.y = vec.y;
					vResult.z = vec.z;
					return TRUE;
				}
			}		
			else
			{
				std::pair<bool, Real> result = MouseRay.intersects(Ogre::Plane(Ogre::Vector3::UNIT_Y, 0));
				if ( result.first && result.second < tCamera->getPosition().length() * 2)
				{
					vec = MouseRay.getPoint(result.second);
				}
				else
				{
					vec = Ogre::Vector3(0, 0, 0);
				}
				vResult.x = vec.x;
				vResult.y = vec.y;
				vResult.z = vec.z;
				gNode = NULL;
				//m_pSelectedObject = NULL;
			}
			return FALSE;
		}

		void MouseOperator::SavePoint( UINT nFlag, CPoint pt )
		{
			mSaveFlag = nFlag;
			mSavePt = pt;
		}

		BOOL MouseOperator::OnSelectNode( UINT nFlags, CPoint point )
		{
			Ogre::Vector3 vec;
			WndCtx * pCTX = GetWndContext(m_hWnd);

			Ogre::Camera* tCamera = pCTX->camera;
			Ogre::Ray MouseRay = tCamera->getCameraToViewportRay(
				float(point.x) / (tCamera->getViewport()->getActualWidth() - 1),
				float(point.y) / (tCamera->getViewport()->getActualHeight() - 1));

			Ogre::RaySceneQueryResult result;
			Ogre::Ray pickRay;
			Ogre::Vector3 pickPoint;
			float dPickDistance;
			uint32 querymask = 1;
			Ogre::ColourValue mHighlihgtColor(1.0f, 0.0f, 0.0f);
			if ( PickQuery(m_hWnd, (float)point.x, (float)point.y, result, querymask , false, pickRay, pickPoint, dPickDistance) == TRUE )
			{
				for ( Ogre::RaySceneQueryResult::iterator i = result.begin() ; i != result.end() ; ++i )
				{
					vec = pickRay.getPoint(i->distance);
					Ogre::Entity * pEntity = (Ogre::Entity*)i->movable;			
					Ogre::SceneNode * pNode = pEntity->getParentSceneNode();
					UnE::Core::USceneNodeManager * uscenMan = UnE::Core::UDB::GetBaseModel((int)m_hWnd)->GetSecneManager();
					m_pSelectNode = uscenMan->FindSceneNode(pNode->getName());
					return TRUE;
				}
			}	
			m_pSelectNode = NULL;
			return FALSE;
		}

		BOOL MouseOperator::OnSelect( UINT nFlags, CPoint point )
		{
			Ogre::Vector3 vec;
			WndCtx * pCTX = GetWndContext(m_hWnd);

			Ogre::Camera* tCamera = pCTX->camera;
			Ogre::Ray MouseRay = tCamera->getCameraToViewportRay(
				float(point.x) / (tCamera->getViewport()->getActualWidth() - 1),
				float(point.y) / (tCamera->getViewport()->getActualHeight() - 1));

			Ogre::RaySceneQueryResult result;
			Ogre::Ray pickRay;
			Ogre::Vector3 pickPoint;
			float dPickDistance;
			uint32 querymask = 1;
			Ogre::ColourValue mHighlihgtColor(1.0f, 0.0f, 0.0f);
			if ( PickQuery(m_hWnd, (float)point.x, (float)point.y, result, querymask , false, pickRay, pickPoint, dPickDistance) == TRUE )
			{
				for ( Ogre::RaySceneQueryResult::iterator i = result.begin() ; i != result.end() ; ++i )
				{
					vec = pickRay.getPoint(i->distance);
					Ogre::Entity * pEntity = (Ogre::Entity*)i->movable;			
					
					RenderableContext entityContext;
					entityContext.selected = true;
					entityContext.ignoreViewDetail = false;
					pEntity->setUserAny(Ogre::Any(entityContext));	
					pEntity->getSubEntity(0)->setUserAny(Ogre::Any(entityContext));	

					mSelectedEntity.push_back(i->movable);
					
					std::string szName = std::string(pEntity->getName().c_str());
					//Ogre::SceneNode * pNode = pEntity->getParentSceneNode();
					//UnE::Core::USceneNodeManager * uscenMan = UnE::Core::UDB::GetBaseModel((int)m_hWnd)->GetSecneManager();
					//m_pSelectNode = uscenMan->FindSceneNode(pNode->getName());
					UObjectManager * pManager = pCTX->objectManager;
					m_pSelectedObject = (UEntity*)pManager->GetUObject(szName); 
					
					return TRUE;
				}
			}	
			m_pSelectedObject = NULL;
			//m_pSelectNode = NULL;
			return FALSE;
		}

		void MouseOperator::ClearSelect()
		{
			int nSize = mSelectedEntity.size();
			for ( int i = 0; i < nSize ; i++ )
			{
				Ogre::Entity * pEntity = (Ogre::Entity*)mSelectedEntity[i];		
				if(pEntity != NULL)					
				{
					RenderableContext entityContext;
					entityContext.selected = false;
					entityContext.ignoreViewDetail = false;
					pEntity->setUserAny(Ogre::Any(entityContext));	
					if(pEntity->getSubEntity(0) != NULL)
						pEntity->getSubEntity(0)->setUserAny(Ogre::Any(entityContext));	
				}						
			}
			mSelectedEntity.clear();

			
		}

		void MouseOperator::RemoveTexture()
		{
			int nSize = mSelectedEntity.size();
			for ( int i = 0; i < nSize ; i++ )
			{
				Ogre::Entity * pEntity = (Ogre::Entity*)mSelectedEntity[i];		
				Ogre::MaterialPtr pMat = pEntity->getSubEntity(0)->getMaterial();
				pMat->getTechnique(0)->getPass(0)->removeAllTextureUnitStates();
			}
		}

		void MouseOperator::SetTexture( std::string& szPath )
		{
			int nSize = mSelectedEntity.size();
			for ( int i = 0; i < nSize ; i++ )
			{
				Ogre::Entity * pEntity = (Ogre::Entity*)mSelectedEntity[i];		
				Ogre::MaterialPtr pMat = pEntity->getSubEntity(0)->getMaterial();
				pMat->getTechnique(0)->getPass(0)->removeAllTextureUnitStates();
				
				char drive[MAX_PATH];
				char path[MAX_PATH];
				char fileName[MAX_PATH];
				char ext[MAX_PATH];
				_splitpath(szPath.c_str(), drive, path, fileName, ext);

				Ogre::ResourceGroupManager::getSingleton().addResourceLocation(szPath, "FileSystem", "Popular");
				pMat->getTechnique(0)->getPass(0)->createTextureUnitState(szPath);	
			}
		}

		BOOL MouseOperator::OnDelete( UINT nFlags, CPoint point )
		{
			if( m_pSelectedObject != NULL)
			{
				UDB * pDB = UDB::GetUDB();
				pDB->GetAnimationManager()->RemoveAnimationState(m_pSelectedObject);

				std::string szName = m_pSelectedObject->GetName();
				
				WndCtx * pCTX = GetWndContext(m_hWnd);
				if( pCTX->sceneMgr->hasSceneNode(szName))
					pCTX->sceneMgr->destroySceneNode(szName);
				
			}
			return TRUE;
		}

		UOpType MouseOperator::GetType()
		{
			return eOp_Mouse;
		}

		UnE::Math::Vector3 MouseOperator::GetLastPoistion()
		{			
			UnE::Math::Vector3 vPos;
			Pick(mSavePt, vPos, 1);			
			return vPos;				
		}
		
		void FindOptimunView(HWND hWnd, std::string szObjectName, unsigned int queryMask, bool bTopview, double topViewAngle, double topViewDist)
		{
			WndCtx * pCtx = GetWndContext(hWnd);
			Ogre::SceneNode * pNode = (Ogre::SceneNode *)pCtx->sceneMgr->getRootSceneNode()->getChild(szObjectName);
			if( pCtx != NULL)
			{
				Ogre::SceneManager * mpSceneManager = pCtx->sceneMgr;
				Ogre::Camera * mpPerspectiveCam = pCtx->camera;

				Ogre::RaySceneQuery* pSceneQuery = mpSceneManager->createRayQuery(Ogre::Ray());
				pSceneQuery->setQueryMask(queryMask);
				Ogre::MovableObject * pMovable = pNode->getAttachedObject(0);
				Ogre::AxisAlignedBox box = pMovable->getBoundingBox();
				box.transformAffine(pMovable->getParentNode()->_getFullTransform());
				float scale = mpSceneManager->getRootSceneNode()->getScale().x;
				Ogre::Vector3 vCenter = box.getCenter();// / scale;
				float len = box.getHalfSize().length() * 3 + mpPerspectiveCam->getNearClipDistance();// / scale;

				if(bTopview)
				{
					Ogre::Quaternion q1, q2;
					q1.FromAngleAxis(Ogre::Radian(Ogre::Degree(float(-topViewAngle))), Ogre::Vector3::UNIT_Y);
					q2.FromAngleAxis(Ogre::Radian(Ogre::Degree(-90)), Ogre::Vector3::UNIT_X);

					float dist = (float)(topViewDist * scale);
					if(topViewDist == 0)
						dist = len;

					mpPerspectiveCam->setOrientation(q1 * q2);
					mpPerspectiveCam->setPosition(vCenter + Ogre::Vector3::UNIT_Y * dist);
					return;
				}

				Ogre::Vector3 pos[4];
				bool bRes[4];
				float dist[4];
				pos[0] = Ogre::Vector3(0, 1, 1);
				pos[1] = Ogre::Vector3(1, 1, 0);
				pos[2] = Ogre::Vector3(0, 1, -1);
				pos[3] = Ogre::Vector3(-1, 1, 0);
				for(int i = 0; i < 4; i++)
				{
					bRes[i] = false;
				}
				for(int i = 0; i < 4; i++)
				{
					pos[i] = (pos[i].normalisedCopy() * len) + vCenter;
					Ogre::Ray ray;
					ray.setOrigin(vCenter);
					Ogre::Vector3 dir = pos[i] - vCenter;
					dir.y = 0;
					ray.setDirection(dir.normalisedCopy());
					pSceneQuery->setRay(ray);
					pSceneQuery->setSortByDistance(true, 0);
					Ogre::RaySceneQueryResult queryResult = pSceneQuery->execute();
					if(queryResult.size() > 0)
					{
						for(size_t s = 0; s < queryResult.size(); s++)
						{
							if(queryResult.at(s).movable != pMovable && queryResult.at(s).distance <= Ogre::Math::Cos(Ogre::Math::HALF_PI * 0.5f) * len &&
								queryResult.at(s).movable->getVisible() && (queryResult.at(s).movable->getMovableType().compare("Entity") == 0))
							{
								bRes[i] = true;
								dist[i] = queryResult.at(s).distance;
								break;
							}
						}
					}
				}
				mpSceneManager->destroyQuery(pSceneQuery);
				Ogre::Vector3 vResPos;
				if(!bRes[0] && !bRes[1] && !bRes[2] && !bRes[3])
				{
					vResPos = pos[0];
				}
				else if(bRes[0] && bRes[1] && bRes[2] && bRes[3])
				{
					int sel = 0;
					float selDist;
					for(int i = 0; i < 4; i++)
					{
						if(i == 0)
						{
							sel = 0;
							selDist = dist[i];
						}
						else
						{
							if(selDist < dist[i])
							{
								selDist = dist[i];
								sel = i;
							}
						}
					}
					vResPos = pos[sel];
				}
				else if(bRes[0])
				{
					if(!bRes[2])      vResPos = pos[2];
					else if(!bRes[1]) vResPos = pos[1];
					else			  vResPos = pos[3];
				}
				else if(bRes[1])
				{
					if(!bRes[3])      vResPos = pos[3];
					else if(!bRes[2]) vResPos = pos[2];
					else			  vResPos = pos[0];
				}
				else if(bRes[2])
				{
					if(!bRes[0])      vResPos = pos[0];
					else if(!bRes[1]) vResPos = pos[1];
					else			  vResPos = pos[3];
				}
				else if(bRes[3])
				{
					if(!bRes[1])	  vResPos = pos[1];
					else if(!bRes[2]) vResPos = pos[2];
					else			  vResPos = pos[0];
				}
				mpPerspectiveCam->setPosition(vResPos);
				mpPerspectiveCam->setDirection(vCenter - vResPos);
			}
		}

		bool FindOptimunPoint(HWND hWnd, std::string szObjectName, unsigned int queryMask, Ogre::Vector3& vPos)
		{
			WndCtx * pCtx = GetWndContext(hWnd);
			Ogre::SceneNode * pNode = (Ogre::SceneNode *)pCtx->sceneMgr->getRootSceneNode()->getChild(szObjectName);
			if (pCtx != NULL)
			{
				Ogre::SceneManager * mpSceneManager = pCtx->sceneMgr;
				Ogre::Camera * mpPerspectiveCam = pCtx->camera;

				Ogre::RaySceneQuery* pSceneQuery = mpSceneManager->createRayQuery(Ogre::Ray());
				pSceneQuery->setQueryMask(queryMask);
				Ogre::MovableObject * pMovable = pNode->getAttachedObject(0);
				Ogre::AxisAlignedBox box = pMovable->getBoundingBox();
				box.transformAffine(pMovable->getParentNode()->_getFullTransform());
				float scale = mpSceneManager->getRootSceneNode()->getScale().x;
				Ogre::Vector3 vCenter = box.getCenter();// / scale;
				vPos = vCenter;
				return true;
			}
			return false;
		}

		void MouseOperator::SetZoomObject( std::string szObjectName )
		{
			WndCtx * pCtx = GetWndContext(m_hWnd);
			if( pCtx != NULL)
			{

				Ogre::SceneNode * pNode = NULL;
				try
				{
					pNode = (Ogre::SceneNode *)pCtx->sceneMgr->getRootSceneNode()->getChild(szObjectName);
				}
				catch (Ogre::Exception& e)
				{
				}

				
				if( pNode != NULL)
				{
					FindOptimunView(m_hWnd, szObjectName, 1, false, 30.0, 30);					
				}	
			}					
		}

		bool MouseOperator::GetObjectPoint(std::string szObjectName, UnE::Math::Vector3& vPos)
		{
			WndCtx * pCtx = GetWndContext(m_hWnd);
			if (pCtx != NULL)
			{

				Ogre::SceneNode * pNode = NULL;
				try
				{
					pNode = (Ogre::SceneNode *)pCtx->sceneMgr->getRootSceneNode()->getChild(szObjectName);
				}
				catch (Ogre::Exception& e)
				{
				}


				if (pNode != NULL)
				{
					Ogre::Vector3 v(0, 0, 0);
					if (FindOptimunPoint(m_hWnd, szObjectName, 1, v))
					{
						vPos = UnE::Math::Vector3(v.x, v.y, v.z);
						return true;
					}
				}
			}
			return false;
		}

		BOOL MouseOperator::Get3DPosition( CPoint pt, UnE::Math::Vector3& vec )
		{
			return Pick(pt, vec, 1);
		}

		void MouseOperator::TargetZoom( UnE::Math::Vector3 target, float dist )
		{
			WndCtx * pCtx = GetWndContext(m_hWnd);
			if( pCtx != NULL)
			{
				Ogre::Vector3 mZoomTarget = Ogre::Vector3(target.x, target.y, target.z);
				Ogre::Vector3 vDir = mZoomTarget - pCtx->camera->getPosition();
				Ogre::Plane nearplan = pCtx->camera->getFrustumPlane(0);
				float len = nearplan.getDistance(mZoomTarget);
				if(len < dist)
				{
					return;
				}
				//vDir.normalise();
				float length = vDir.length();
				if( length < 0.1f)
				{
					return;
				}
				float ratio = (length - dist) / length;
				vDir *= ratio;
				Ogre::Vector3 vPos = pCtx->camera->getPosition();
				Ogre::Vector3 vResPos = vPos + vDir;
				pCtx->camera->setPosition(vResPos);
				pCtx->camera->setDirection(vDir);
				//pCtx->camera->moveRelative(vDir * ratio);
			}
		}

		BOOL MouseOperator::CheckInFrustum(float x, float y, float z)
		{
			WndCtx * pCtx = GetWndContext(m_hWnd);
			if( pCtx != NULL)
			{
				Ogre::Vector3 m3DPosition(x, y, z);

				if( pCtx->camera != NULL)
				{
					Ogre::PlaneBoundedVolume volume;
					int width = pCtx->viewport->getActualWidth();
					int height = pCtx->viewport->getActualHeight();
					pCtx->camera->getCameraToViewportBoxVolume(0,0, width, height, &volume);					

					Ogre::Sphere sp(m3DPosition, 1.0f);
					if( volume.intersects(sp) == true)
						return TRUE;					
				}				
			}		
			return FALSE;
		}

		BOOL MouseOperator::Get2DPosition( UnE::Math::Vector3 vec, CPoint& pt )
		{
			pt.x = 0;
			pt.y = 0;

			WndCtx * pCtx = GetWndContext(m_hWnd);
			if( pCtx != NULL)
			{
				Ogre::Vector3 m3DPosition(vec.x, vec.y, vec.z);
				Ogre::Vector3 rootScale = pCtx->sceneMgr->getRootSceneNode()->getScale();
				Ogre::Vector3 m2DPosition = pCtx->camera->getProjectionMatrix() * (pCtx->camera->getViewMatrix() * (m3DPosition * rootScale));
				pt.x = ((m2DPosition.x / 2) + 0.5f) * pCtx->camera->getViewport()->getActualWidth();
				pt.y = (1 - ((m2DPosition.y / 2) + 0.5f)) * pCtx->camera->getViewport()->getActualHeight();
				return TRUE;
			}		
			return FALSE;
		}		

		int MouseOperator::OnSelectPOI( int x, int y )
		{			
			WndCtx * pCtx = GetWndContext(m_hWnd);
			if( pCtx != NULL)
			{
				Ogre::Vector3 vCamPos = pCtx->camera->getPosition();
				double min_length = DBL_MAX;
				UnE::Core::UIconPOI * pTargetIcon = NULL;
				int targetId = -1;
				UIconPOIList::reverse_iterator it;
				for(it = gIconPOIList.rbegin(); it != gIconPOIList.rend(); it++)
				{
					UnE::Core::UIconPOI * pIcon = it->second;					
					std::pair<bool, float> rpair = pIcon->Pick(x, y);
					if(rpair.first == true)
					{
						Ogre::Vector3 vPos = pIcon->Get3DPosition();
						double distance = (vCamPos - vPos).length();
						if( distance < min_length)
						{
							min_length = distance;
							pTargetIcon = pIcon;
							targetId = it->first;
						}						
					}
				}
				return targetId;
			}
			return -1;			
		}

		void MouseOperator::CreateSphere()
		{
#ifndef HSMS
			WndCtx * pWndCtx = GetWndContext(m_hWnd);
			std::string names1[] = { "Popular", "sphere", "Sphere001.mesh" };

			int nCookie = UDB::GetNextCookie();

			gSphereScene = pWndCtx->sceneMgr->getRootSceneNode()->createChildSceneNode();
			Ogre::Entity* mEntity = pWndCtx->sceneMgr->createEntity("Sphere001", Ogre::SceneManager::PT_SPHERE);

			Ogre::String lNameOfMaterial = "Mouse_sphere_Material";
			Ogre::String lResourceGroup = "Popular";
			if (!Ogre::MaterialManager::getSingleton().resourceExists(lNameOfMaterial))
			{
				Ogre::MaterialPtr myPathMaterial = Ogre::MaterialManager::getSingleton().create(lNameOfMaterial, lResourceGroup);

				myPathMaterial->setReceiveShadows(false);
				myPathMaterial->getTechnique(0)->setLightingEnabled(true);
				myPathMaterial->getTechnique(0)->getPass(0)->setDiffuse(1.0f, 0.1f, 0.1f, 1.0f);
				myPathMaterial->getTechnique(0)->getPass(0)->setAmbient(Ogre::ColourValue(1.0f, 0.2f, 0.2f, 0.1f));
				//myPathMaterial->getTechnique(0)->getPass(0)->setDepthWriteEnabled(false);
				myPathMaterial->getTechnique(0)->getPass(0)->setSceneBlending(Ogre::SceneBlendType::SBT_TRANSPARENT_ALPHA);
				Ogre::TextureUnitState * pUnit = myPathMaterial->getTechnique(0)->getPass(0)->createTextureUnitState("3d03.png");
			}

			mEntity->setMaterialName(lNameOfMaterial, "Popular");
			gSphereScene->attachObject(mEntity);
			gSphereScene->scale(0.3f, 0.3f, 0.3f);
			gSphereScene->setVisible(false);
#endif
		}
	}
}

//////////////////////////////////////////////////////////////////////////
