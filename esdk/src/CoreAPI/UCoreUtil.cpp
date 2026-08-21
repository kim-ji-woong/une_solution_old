#include "StdAfx.h"
#include "UCoreUtil.h"
#include <Ogre.h>


#define EPSILON 0.0001
using namespace Ogre;

namespace UnE
{
	namespace Core
	{
		Radian UCoreUtil::ToRadian(float angle, bool bDegree)
		{
			Ogre::Radian _angle;
			if(bDegree)
			{
				_angle = Ogre::Radian(Ogre::Degree(angle));
			}
			else
			{
				_angle = Ogre::Radian(angle);
			}

			return _angle;
		}

		void UCoreUtil::MakeBoundingBox(ManualObject* pMo, const AxisAlignedBox& box, const ColourValue& boxColor, bool bNoDepth)
		{
			const Vector3* allcorners = box.getAllCorners();

			String mtrName;
			if(bNoDepth) mtrName = "NoShadowNoLightNoDepth";
			else mtrName = "NoShadowNoLight";
			pMo->begin(mtrName, RenderOperation::OT_LINE_LIST);

			for(int i = 0; i < 8; i++)
			{
				pMo->position(allcorners[i]);
				pMo->colour(boxColor);
			}

			pMo->position((allcorners[1] - allcorners[0]) * 0.2f + allcorners[0]);
			pMo->colour(boxColor);
			pMo->position((allcorners[3] - allcorners[0]) * 0.2f + allcorners[0]);
			pMo->colour(boxColor);
			pMo->position((allcorners[6] - allcorners[0]) * 0.2f + allcorners[0]);
			pMo->colour(boxColor);

			pMo->position((allcorners[0] - allcorners[1]) * 0.2f + allcorners[1]);
			pMo->colour(boxColor);
			pMo->position((allcorners[2] - allcorners[1]) * 0.2f + allcorners[1]);
			pMo->colour(boxColor);
			pMo->position((allcorners[5] - allcorners[1]) * 0.2f + allcorners[1]);
			pMo->colour(boxColor);

			pMo->position((allcorners[1] - allcorners[2]) * 0.2f + allcorners[2]);
			pMo->colour(boxColor);
			pMo->position((allcorners[3] - allcorners[2]) * 0.2f + allcorners[2]);
			pMo->colour(boxColor);
			pMo->position((allcorners[4] - allcorners[2]) * 0.2f + allcorners[2]);
			pMo->colour(boxColor);

			pMo->position((allcorners[0] - allcorners[3]) * 0.2f + allcorners[3]);
			pMo->colour(boxColor);
			pMo->position((allcorners[2] - allcorners[3]) * 0.2f + allcorners[3]);
			pMo->colour(boxColor);
			pMo->position((allcorners[7] - allcorners[3]) * 0.2f + allcorners[3]);
			pMo->colour(boxColor);

			pMo->position((allcorners[2] - allcorners[4]) * 0.2f + allcorners[4]);
			pMo->colour(boxColor);
			pMo->position((allcorners[5] - allcorners[4]) * 0.2f + allcorners[4]);
			pMo->colour(boxColor);
			pMo->position((allcorners[7] - allcorners[4]) * 0.2f + allcorners[4]);
			pMo->colour(boxColor);

			pMo->position((allcorners[1] - allcorners[5]) * 0.2f + allcorners[5]);
			pMo->colour(boxColor);
			pMo->position((allcorners[4] - allcorners[5]) * 0.2f + allcorners[5]);
			pMo->colour(boxColor);
			pMo->position((allcorners[6] - allcorners[5]) * 0.2f + allcorners[5]);
			pMo->colour(boxColor);

			pMo->position((allcorners[0] - allcorners[6]) * 0.2f + allcorners[6]);
			pMo->colour(boxColor);
			pMo->position((allcorners[5] - allcorners[6]) * 0.2f + allcorners[6]);
			pMo->colour(boxColor);
			pMo->position((allcorners[7] - allcorners[6]) * 0.2f + allcorners[6]);
			pMo->colour(boxColor);

			pMo->position((allcorners[3] - allcorners[7]) * 0.2f + allcorners[7]);
			pMo->colour(boxColor);
			pMo->position((allcorners[4] - allcorners[7]) * 0.2f + allcorners[7]);
			pMo->colour(boxColor);
			pMo->position((allcorners[6] - allcorners[7]) * 0.2f + allcorners[7]);
			pMo->colour(boxColor);

			for(int i = 0; i < 8; i++)
			{
				for(int j = 0; j < 3; j++)
				{
					pMo->index(i);
					pMo->index(8 + i*3 + j);
				}
			}

			pMo->end();
			pMo->setRenderQueueGroup(RENDER_QUEUE_SKIES_LATE - 1);
		}

		std::pair<bool, float> UCoreUtil::UtilPickEntity(const Ray& pickRay, const Entity* pEntity)
		{
			AxisAlignedBox tBox = pEntity->getBoundingBox();
			const Vector3* tConstBoxConers = tBox.getAllCorners(); 

			Vector3 tBoxConers[8];
			for(int i = 0; i < 8; i++)
			{
				tBoxConers[i] = tConstBoxConers[i];
				tBoxConers[i] *= pEntity->getParentNode()->_getDerivedScale();
				//tBoxConers[i] = pEntity->getParentNode()->getWorldOrientation() * tBoxConers[i];
				tBoxConers[i] = _GetWorldOrientation(*pEntity->getParentNode()) * tBoxConers[i];
				//tBoxConers[i] += pEntity->getParentNode()->getWorldPosition();
				tBoxConers[i] += _GetWorldPosition(*pEntity->getParentNode());
			}

			PlaneBoundedVolume tVol;
			tVol.planes.push_back(Plane(tBoxConers[0], tBoxConers[3], tBoxConers[2]));
			tVol.planes.push_back(Plane(tBoxConers[0], tBoxConers[1], tBoxConers[5]));
			tVol.planes.push_back(Plane(tBoxConers[4], tBoxConers[2], tBoxConers[3]));
			tVol.planes.push_back(Plane(tBoxConers[3], tBoxConers[0], tBoxConers[6]));
			tVol.planes.push_back(Plane(tBoxConers[6], tBoxConers[5], tBoxConers[4]));
			tVol.planes.push_back(Plane(tBoxConers[1], tBoxConers[2], tBoxConers[4]));

			return UtilIntersect(tVol, pickRay);
		}

		std::pair<bool, float> UCoreUtil::UtilPickEntityEx(const Ray& pickRay, const Entity* pEntity)
		{
			if(!pEntity->getVisible())
			{
				std::pair<bool, float> res;
				res.first = false;
				return res;
			}
			Real closest_distance = -1.0f;
			Vector3 closest_result;

			size_t vertex_count;
			size_t index_count;
			Vector3 *vertices;
			unsigned long *indices;

			GetMeshInformationEx(pEntity, vertex_count, vertices, index_count, indices);

			bool new_closest_found = false;
			for (int i = 0; i < static_cast<int>(index_count); i += 3)
			{
				std::pair<bool, Real> hit = Math::intersects(pickRay, vertices[indices[i]],
					vertices[indices[i+1]], vertices[indices[i+2]], true, false);

				if (hit.first)
				{
					if ((closest_distance < 0.0f) || (hit.second < closest_distance))
					{
						closest_distance = hit.second;
						new_closest_found = true;
					}
				}
			}

			OGRE_DELETE_ARRAY(vertices);
			OGRE_DELETE_ARRAY(indices);

			if (new_closest_found)
			{
				closest_result = pickRay.getPoint(closest_distance);               
			}

			std::pair<bool, float> res;
			res.first = false;
			if(closest_distance >= 0.0f)
			{
				res.first = true;
				res.second = closest_distance; 
			}

			return res;
		}

		void UCoreUtil::GetMeshInformationEx(const Entity* pEntity, size_t &vertex_count, Vector3* &vertices, size_t &index_count, unsigned long* &indices, bool bUseScale, bool bOnlyParentScale)
		{
			bool added_shared = false;
			size_t current_offset = 0;
			size_t shared_offset = 0;
			size_t next_offset = 0;
			size_t index_offset = 0;

			Node* pNode = pEntity->getParentNode();
			//Vector3 position = pNode->getWorldPosition();
			Vector3 position = _GetWorldPosition(*pNode);
			//Quaternion orient = pNode->getWorldOrientation();
			Quaternion orient = _GetWorldOrientation(*pNode);
			Vector3 scale = pEntity->getParentNode()->_getDerivedScale();

			vertex_count = index_count = 0;

			MeshPtr pMesh = pEntity->getMesh();

			for ( ushort i = 0; i < pMesh->getNumSubMeshes(); i++)
			{
				Ogre::SubMesh* pSubmesh = pMesh->getSubMesh(i);

				if(pSubmesh->useSharedVertices)
				{
					if( !added_shared )
					{
						vertex_count += pMesh->sharedVertexData->vertexCount;
						added_shared = true;
					}
				}
				else
				{
					vertex_count += pSubmesh->vertexData->vertexCount;
				}
				index_count += pSubmesh->indexData->indexCount;
			}

			vertices = new Ogre::Vector3[vertex_count];
			indices = new unsigned long[index_count];
			memset(indices, 0, sizeof(unsigned long)* index_count);
			memset(vertices, 0, sizeof(Ogre::Vector3)* vertex_count);

			added_shared = false;
			for(unsigned short i = 0; i < pEntity->getNumSubEntities(); ++i)
			{
				SubEntity* pSubEntity = pEntity->getSubEntity(i);
				SubMesh* pSubmesh = pSubEntity->getSubMesh();

				VertexData* vertex_data = pSubEntity->getVertexDataForBinding();
				if((!pSubmesh->useSharedVertices)||(pSubmesh->useSharedVertices && !added_shared))
				{
					if(pSubmesh->useSharedVertices)
					{
						added_shared = true;
						shared_offset = current_offset;
					}

					const VertexElement* posElem =
						vertex_data->vertexDeclaration->findElementBySemantic(VES_POSITION);

					HardwareVertexBufferSharedPtr vbuf =
						vertex_data->vertexBufferBinding->getBuffer(posElem->getSource());

					unsigned char* vertex =
						static_cast<unsigned char*>(vbuf->lock(HardwareBuffer::HBL_READ_ONLY));

					float* pReal;

					for( size_t j = 0; j < vertex_data->vertexCount; ++j, vertex += vbuf->getVertexSize())
					{
						posElem->baseVertexPointerToElement(vertex, &pReal);

						Ogre::Vector3 pt(pReal[0], pReal[1], pReal[2]);

						if(bUseScale)
						{
							if(bOnlyParentScale)
							{
								scale = pNode->getScale();
								position = pNode->getPosition();
							}
							vertices[current_offset + j] = (orient * (pt * scale)) + position;
						}
						else
							vertices[current_offset + j] = (orient * pt) + position;
					}

					vbuf->unlock();
					next_offset += vertex_data->vertexCount;
				}

				Ogre::IndexData* index_data = pSubmesh->indexData;
				size_t idxCount = index_data->indexCount;
				size_t idxStart = index_data->indexStart;
				Ogre::HardwareIndexBufferSharedPtr ibuf = index_data->indexBuffer;

				bool use32bitindexes = (ibuf->getType() == Ogre::HardwareIndexBuffer::IT_32BIT);

				unsigned long*  pLong = static_cast<unsigned long*>(ibuf->lock(Ogre::HardwareBuffer::HBL_READ_ONLY));
				unsigned short* pShort = reinterpret_cast<unsigned short*>(pLong);

				size_t offset = (pSubmesh->useSharedVertices)? shared_offset : current_offset;

				if ( use32bitindexes )
				{
					for ( size_t k = 0; k < idxCount; ++k)
					{
						indices[index_offset++] = pLong[k + idxStart] + static_cast<unsigned long>(offset);
					}
				}
				else
				{
					for ( size_t k = 0; k < idxCount; ++k)
					{
						indices[index_offset++] = static_cast<unsigned long>(pShort[k + idxStart]) + static_cast<unsigned long>(offset);
					}
				}

				ibuf->unlock();
				current_offset = next_offset;
			}
		}

		std::pair<bool, float> UCoreUtil::UtilIntersect(const PlaneBoundedVolume& vol, const Ray& ray)
		{
			std::pair<bool, float> res;
			Vector3 vCross;
			bool bFirst = true;

			PlaneList::const_iterator it;
			for(it = vol.planes.begin(); it != vol.planes.end(); it++)
			{
				std::pair<bool, float> temp = ray.intersects(*it);
				if(temp.first)
				{			
					vCross = ray.getPoint(temp.second + 0.01f);
					if(UtilIntersect(vol, vCross))
					{
						if(bFirst)
						{
							res = temp;
							bFirst = false;
							continue;
						}
						if(res.second > temp.second)
							res = temp;
					}
				}
			}

			return res;
		}

		bool UCoreUtil::UtilIntersect(const PlaneBoundedVolume& vol, const Vector3& pos)
		{
			PlaneList::const_iterator it;
			float fDist;

			for(it = vol.planes.begin(); it != vol.planes.end(); it++)
			{
				fDist = (*it).getDistance(pos);
				if(fDist < 0)
				{
					return false;
				}
			}

			return true;
		}

		bool UCoreUtil::UtilIntersect(const PlaneBoundedVolume& vol, const Vector3& pos, float radius)
		{
			PlaneList::const_iterator it;
			float fDist;

			for(it = vol.planes.begin(); it != vol.planes.end(); it++)
			{
				fDist = (*it).getDistance(pos);
				if(fDist < -radius)
				{
					return false;
				}
			}

			return true;
		}

		bool UCoreUtil::UtilIntersectLine(const Ray& ray, const Vector3& p1, const Vector3& p2, float snapSens)
		{
			Vector3 vDir = (ray.getOrigin() - p1).normalisedCopy();
			Vector3 vLine = (p2 - p1).normalisedCopy();
			Vector3 vCross = vDir.crossProduct(vLine).normalisedCopy() * snapSens;

			return UtilIntersectPolygon(ray, p1 + vCross, p2 + vCross, p2 - vCross, p1 - vCross);
		}

		bool UCoreUtil::UtilIntersectPolygon(const Ray& ray, const Vector3& p1, const Vector3& p2, const Vector3& p3, const Vector3& p4)
		{
			Plane polygon = Plane(p1,p2,p3);
			std::pair<bool, Real> res = ray.intersects(polygon);
			if(!res.first)
			{
				return false;
			}

			Vector3 pos = ray.getPoint(res.second);

			PlaneBoundedVolume camVol;
			camVol.planes.push_back(Plane(p1+polygon.normal,p2,p1));
			camVol.planes.push_back(Plane(p2+polygon.normal, p3, p2));
			camVol.planes.push_back(Plane(p3+polygon.normal, p4, p3));
			camVol.planes.push_back(Plane(p4+polygon.normal, p1, p4));

			return UtilIntersect(camVol, pos);
		}

		DisplayString UCoreUtil::StringToDisplayString(String str)
		{
			LPCSTR ansiStr = str.c_str();
			LPWSTR szUniStr;  
			// 유니코드로 변환 전 return 되는 길이얻기 
			int nLen = MultiByteToWideChar(CP_ACP, 0, ansiStr, -1, NULL, NULL);
			nLen = nLen * sizeof(WCHAR);  
			szUniStr = (LPWSTR)malloc(nLen+1);
			// 메모리를 할당한다. 
			memset(szUniStr,0,nLen+1);  
			// 이제 변환을 수행한다. 
			MultiByteToWideChar(CP_ACP, 0, ansiStr, -1, szUniStr, nLen);     
			DisplayString result(szUniStr);

			free(szUniStr);
			return result;
		}

		float UCoreUtil::ReCalAngle(float angle)
		{
			float res = angle;
			if(angle < 0)
			{
				res -= 360 * int(res / 360);
				res += 360;
			}
			else if(angle > 360)
			{
				res -= 360 * int(res / 360);
			}
			return res;
		}

		void UCoreUtil::UtilSearchPlaneIntersectObject(std::vector<MovableObject*>& outObjs, SceneNode* pSceneNode, 
			std::vector<String> containNames, std::vector<String> exceptNames, const Plane& plane, bool bCompairPlane)
		{
			if(!pSceneNode) return;

			Ogre::SceneNode::ChildNodeIterator it = pSceneNode->getChildIterator();
			Ogre::SceneNode* pTmpSceneNode = NULL;
			while(it.hasMoreElements())
			{
				pTmpSceneNode = static_cast<Ogre::SceneNode*>(it.peekNextValue());
				String name = pTmpSceneNode->getName();

				if(!exceptNames.empty())
				{
					bool bExcept = false;
					for(size_t i = 0; i < exceptNames.size(); i++)
					{
						if(name.find(exceptNames.at(i)) != String::npos)
						{
							bExcept = true;
							break;
						}
					}
					if(bExcept)
					{
						it.moveNext();
						continue;
					}
				}

				if(!containNames.empty())
				{
					bool bCompared = false;
					for(size_t i = 0; i < containNames.size(); i++)
					{
						if(name.find(containNames.at(i)) != String::npos)
						{
							bCompared = true;
							break;
						}
					}
					if(!bCompared)
					{
						it.moveNext();
						continue;
					}
				}

				if(pTmpSceneNode->numChildren() != 0)
				{
					UtilSearchPlaneIntersectObject(outObjs, pTmpSceneNode, containNames, exceptNames, plane, bCompairPlane);
				}

				unsigned short childNum = pTmpSceneNode->numAttachedObjects();
				for(unsigned short i = 0; i < childNum; i++)
				{
					Ogre::MovableObject* pTmpMovable = pTmpSceneNode->getAttachedObject(i);
					if(bCompairPlane)
					{
						AxisAlignedBox box = pTmpMovable->getBoundingBox();
						box.transformAffine(pTmpMovable->getParentNode()->_getFullTransform());
						if(Math::intersects(plane, box) && pTmpMovable->getVisible())
						{
							outObjs.push_back(pTmpMovable);
						}
					}
					else
					{
						if(pTmpMovable->getVisible())
						{
							outObjs.push_back(pTmpMovable);
						}
					}
				}
				it.moveNext();
			}
		}

		Plane CreatePlane(Entity* pEntity, bool& bNoIdx)
		{
			size_t vertex_count;
			size_t index_count;
			Vector3 *vertices;
			unsigned long *indices;
			bNoIdx = false;

			UCoreUtil::GetMeshInformationEx(pEntity, vertex_count, vertices, index_count, indices);

			Vector3 vPolygons[3];
			if(index_count > 2)
			{
				vPolygons[0] = vertices[indices[0]];
				vPolygons[1] = vertices[indices[1]];
				vPolygons[2] = vertices[indices[2]];
			}
			else
			{
				vPolygons[0] = vertices[0];
				vPolygons[1] = vertices[1];
				vPolygons[2] = vertices[2];
				bNoIdx = true;
			}

			Plane plane(vPolygons[0], vPolygons[1], vPolygons[2]);

			OGRE_DELETE_ARRAY(vertices);
			OGRE_DELETE_ARRAY(indices);

			return plane;
		}

		bool EqualVector3Ez(const Vector3& p1, const Vector3& p2)
		{
			return (Math::Abs(p1.x - p2.x) < EPSILON * 10 && Math::Abs(p1.y - p2.y) < EPSILON * 10 && Math::Abs(p1.z - p2.z) < EPSILON * 10);
		}

		bool EqualPlane(Plane p1, Plane p2, bool bNotUseNormal)
		{
			bool bEqualNormal;
			if(bNotUseNormal)
			{
				bEqualNormal = p1.normal.normalisedCopy().dotProduct(p2.normal.normalisedCopy()) > 1 - EPSILON * 10;
			}
			else
			{
				bEqualNormal = Math::Abs(p1.normal.normalisedCopy().dotProduct(p2.normal.normalisedCopy())) > 1 - EPSILON * 10;
			}
			if(bEqualNormal)
			{
				if(bNotUseNormal)
				{
					if(Math::Abs(Math::Abs(p1.d) - Math::Abs(p2.d)) < EPSILON * 10)
					{
						return true;
					}
				}
				else
				{
					if(Math::Abs(p1.d - p2.d) < EPSILON * 10)
					{
						return true;
					}
				}
			}
			return false;
		}

		void SearchEqualPlane(std::vector<Ogre::MovableObject*>& outObjs, Ogre::SceneNode* pSceneNode, const Entity* pEntity, const Plane& plane, bool bNotUseNormal)
		{
			Ogre::SceneNode::ChildNodeIterator it = pSceneNode->getChildIterator();
			Ogre::SceneNode* pTmpSceneNode = NULL;
			while(it.hasMoreElements())
			{
				pTmpSceneNode = static_cast<Ogre::SceneNode*>(it.peekNextValue());

				if(pTmpSceneNode->numChildren() != 0)
				{
					SearchEqualPlane(outObjs, pTmpSceneNode, pEntity, plane, bNotUseNormal);
				}

				unsigned short childNum = pTmpSceneNode->numAttachedObjects();
				for(unsigned short i = 0; i < childNum; i++)
				{
					Ogre::MovableObject* pTmpMovable = pTmpSceneNode->getAttachedObject(i);
					Entity* pEntity2 = NULL;
					if(pTmpMovable->getMovableType().compare("Entity") == 0)
					{
						pEntity2 = static_cast<Entity*>(pTmpMovable);
					}
					else
					{
						continue;
					}
					if(pEntity == pEntity2)
					{
						continue;
					}
					bool bNoIdx;
					bool bNotUseNormalRes = false;
					Plane plane2 = CreatePlane(pEntity2, bNoIdx);
					if(bNotUseNormal || bNoIdx)
					{
						bNotUseNormalRes = true;
					}
					if(EqualPlane(plane, plane2, bNotUseNormalRes))
					{
						outObjs.push_back(pEntity2);
					}
				}
				it.moveNext();
			}
		}

		bool AddAdjWall(std::vector<Ogre::MovableObject*>& outObjs, std::vector<Ogre::MovableObject*>& inObjs)
		{
			bool bFindEqual = false;
			for(size_t i = 0; i < outObjs.size(); i++)
			{
				Entity* pEntity = static_cast<Entity*>(outObjs.at(i));
				size_t vertex_count;
				size_t index_count;
				Vector3 *vertices;
				unsigned long *indices;
				UCoreUtil::GetMeshInformationEx(pEntity, vertex_count, vertices, index_count, indices);

				std::vector<Ogre::MovableObject*>::iterator it;
				for(it = inObjs.begin(); it != inObjs.end(); it++)
				{
					MovableObject* pCurrObj = (*it);
					Entity* pEntity2 = static_cast<Entity*>(pCurrObj);
					size_t vertex_count2;
					size_t index_count2;
					Vector3 *vertices2;
					unsigned long *indices2;
					UCoreUtil::GetMeshInformationEx(pEntity2, vertex_count2, vertices2, index_count2, indices2);

					for(size_t j = 0; j < vertex_count; j++)
					{
						for(size_t k = 0; k < vertex_count2; k++)
						{
							if(EqualVector3Ez(vertices[j], vertices2[k]))
							{
								bFindEqual = true;
								outObjs.push_back(pCurrObj);
								inObjs.erase(it);
								break;
							}
						}
						if(bFindEqual) break;
					}

					OGRE_DELETE_ARRAY(vertices2);
					OGRE_DELETE_ARRAY(indices2);
					if(bFindEqual) break;
				}

				OGRE_DELETE_ARRAY(vertices);
				OGRE_DELETE_ARRAY(indices);
				if(bFindEqual) break;
			}

			return bFindEqual;
		}

		void UCoreUtil::UtilSearchAdjWall(std::vector<Ogre::MovableObject*>& outObjs, Ogre::SceneNode* pSceneNode, Ogre::MovableObject* pObj)
		{
			if(!pSceneNode) return;
			Entity* pEntity = NULL;
			if(pObj->getMovableType().compare("Entity") == 0)
			{
				pEntity = static_cast<Entity*>(pObj);
			}
			else
			{
				return;
			}
			outObjs.push_back(pObj);

			bool bNoIdx;
			Plane firstPlane = CreatePlane(pEntity, bNoIdx);

			std::vector<Ogre::MovableObject*> tempObjs;
			SearchEqualPlane(tempObjs, pSceneNode, pEntity, firstPlane, bNoIdx);

			while(1)
			{
				if(!AddAdjWall(outObjs, tempObjs))
				{
					break;
				}
			}

			// 		if(outObjs.size() > 1)
			// 		{
			// 			outObjs.erase(outObjs.begin());
			// 			outObjs.push_back(pObj);
			// 		}
		}

		void SearchEqualPlane2(std::vector<Ogre::MovableObject*>& outObjs, Ogre::SceneNode* pSceneNode, const Entity* pEntity, const String strID, const Plane& plane, bool bNotUseNormal)
		{
			Ogre::SceneNode::ChildNodeIterator it = pSceneNode->getChildIterator();
			Ogre::SceneNode* pTmpSceneNode = NULL;
			while(it.hasMoreElements())
			{
				pTmpSceneNode = static_cast<Ogre::SceneNode*>(it.peekNextValue());

				if(pTmpSceneNode->numChildren() != 0)
				{
					SearchEqualPlane2(outObjs, pTmpSceneNode, pEntity, strID, plane, bNotUseNormal);
				}

				unsigned short childNum = pTmpSceneNode->numAttachedObjects();
				for(unsigned short i = 0; i < childNum; i++)
				{
					Ogre::MovableObject* pTmpMovable = pTmpSceneNode->getAttachedObject(i);
					String gideLineID = pTmpMovable->getName();
					size_t tokenIdx = gideLineID.find('_', 2);
					gideLineID = gideLineID.substr(2, tokenIdx - 2);
					if(gideLineID != strID)
						continue;

					Entity* pEntity2 = NULL;
					if(pTmpMovable->getMovableType().compare("Entity") == 0)
					{
						pEntity2 = static_cast<Entity*>(pTmpMovable);
					}
					else
					{
						continue;
					}
					if(pEntity == pEntity2)
					{
						continue;
					}
					bool bNoIdx;
					bool bNotUseNormalRes = false;
					Plane plane2 = CreatePlane(pEntity2, bNoIdx);
					if(bNotUseNormal || bNoIdx)
					{
						bNotUseNormalRes = true;
					}
					if(EqualPlane(plane, plane2, bNotUseNormalRes))
					{
						outObjs.push_back(pEntity2);
					}
				}
				it.moveNext();
			}
		}

		void UCoreUtil::UtilSearchAdjWall2(std::vector<Ogre::MovableObject*>& outObjs, Ogre::MovableObject* pObj)
		{
			Ogre::SceneNode* pWallNode = pObj->getParentSceneNode()->getParentSceneNode();
			Entity* pEntity = NULL;
			if(pObj->getMovableType().compare("Entity") == 0)
			{
				pEntity = static_cast<Entity*>(pObj);
			}
			else
			{
				return;
			}
			outObjs.push_back(pObj);

			String gideLineID = pObj->getName();
			size_t tokenIdx = gideLineID.find('_', 2);
			gideLineID = gideLineID.substr(2, tokenIdx - 2);

			bool bNoIdx;
			Plane firstPlane = CreatePlane(pEntity, bNoIdx);
			SearchEqualPlane2(outObjs, pWallNode, pEntity, gideLineID, firstPlane, bNoIdx);
		}

		void RegularVector3(Vector3& pt)
		{
			if((pt.x < 0.0f && pt.x > -EPSILON) || (pt.x > 0.0f && pt.x < EPSILON))
			{
				pt.x = 0;
			}
			if((pt.y < 0.0f && pt.y > -EPSILON) || (pt.y > 0.0f && pt.y < EPSILON))
			{
				pt.y = 0;
			}
			if((pt.z < 0.0f && pt.z > -EPSILON) || (pt.z > 0.0f && pt.z < EPSILON))
			{
				pt.z = 0;
			}
		}

		bool UCoreUtil::EqualVector3(const Vector3& p1, const Vector3& p2)
		{
			return (Math::Abs(p1.x - p2.x) < EPSILON && Math::Abs(p1.y - p2.y) < EPSILON && Math::Abs(p1.z - p2.z) < EPSILON);
		}

		void UCoreUtil::UtilGetCrossLineToPlane(std::vector<std::pair<Vector3, Vector3>>& outLines, const std::vector<MovableObject*>& objs, const Plane& plane, Vector3 rootNodeScale)
		{
			for(size_t s = 0; s < objs.size(); s++)
			{
				if(objs.at(s)->getMovableType().compare("Entity") == 0)
				{
					size_t vertex_count = 0;
					size_t index_count = 0;
					Vector3 *vertices;
					unsigned long *indices;
					Entity* pTmpEntity = static_cast<Entity*>(objs.at(s));
					GetMeshInformationEx(pTmpEntity, vertex_count, vertices, index_count, indices);

					for (int i = 0; i < static_cast<int>(index_count); i += 3)
					{
						Vector3 vPolygons[3];
						vPolygons[0] = vertices[indices[i]];
						vPolygons[1] = vertices[indices[i+1]];
						vPolygons[2] = vertices[indices[i+2]];
						bool bFirst = true;
						bool bSecond = false;
						std::pair<Vector3, Vector3> line;
						for(int j = 0; j < 3; j++)
						{
							Vector3 vOri = vPolygons[j];
							unsigned long tmpIdx = j + 1;
							if(j + 1 == 3) tmpIdx = 0;
							Vector3 vDir = vPolygons[tmpIdx] - vOri;
							float rayDist = vDir.length();
							vDir.normalise();
							Ray ray(vOri, vDir);
							std::pair<bool, Real> hit = Math::intersects(ray, plane);
							if(hit.first && hit.second <= rayDist)
							{
								if(bFirst)
								{
									line.first = ray.getPoint(hit.second) / rootNodeScale;
									RegularVector3(line.first);
									bFirst = false;
								}
								else
								{
									line.second = ray.getPoint(hit.second) / rootNodeScale;
									RegularVector3(line.second);
									bSecond = true;
								}
							}
						}
						if(!bFirst && bSecond)
						{
							bool bAdj = false;
							if(i != 0 && index_count <= 6)
							{
								Vector3 a1 = outLines.back().first;
								Vector3 a2 = outLines.back().second;
								Vector3 b1 = line.first;
								Vector3 b2 = line.second;
								if(EqualVector3(a1, b1))
								{
									outLines.back().first = b2;
									bAdj = true;
								}
								else if(EqualVector3(a1, b2))
								{
									outLines.back().first = b1;
									bAdj = true;
								}
								else if(EqualVector3(a2, b1))
								{
									outLines.back().second = b2;
									bAdj = true;
								}
								else if(EqualVector3(a2, b2))
								{
									outLines.back().second = b1;
									bAdj = true;
								}
							}
							if(!bAdj)
							{
								bool beq = false;
								std::vector<std::pair<Vector3, Vector3>>::iterator it;
								for(it = outLines.begin(); it != outLines.end(); it++)
								{
									if(UtilEqualLine(it->first, it->second, line.first, line.second))
									{
										outLines.erase(it);
										beq = true;
										break;
									}
								}
								if(!beq)
									outLines.push_back(line);
							}
						}
					}

					OGRE_DELETE_ARRAY(vertices);
					OGRE_DELETE_ARRAY(indices);
				}
			}
		}

		void UCoreUtil::UtilGetCrossLineToPlane(std::vector<std::pair<Vector3, Vector3>>& outLines, SceneNode* pSceneNode, 
			std::vector<String> containNames, std::vector<String> exceptNames, const Plane& plane, Vector3 rootNodeScale)
		{
			std::vector<Ogre::MovableObject*> res;
			UtilSearchPlaneIntersectObject(res, pSceneNode, containNames, exceptNames, plane);
			UCoreUtil::UtilGetCrossLineToPlane(outLines, res, plane, rootNodeScale);
		}

		void InsertPointToPolyline(std::vector<Vector3>& polyline, AxisAlignedBox& aab, Vector3& vMin, Vector3 addPoint)
		{
			size_t size = polyline.size();
			if(size < 2)
			{
				polyline.push_back(addPoint);
				aab.merge(addPoint);
				if(aab.getMinimum().x == addPoint.x || aab.getMinimum().z == addPoint.z)
				{
					vMin = addPoint;
				}
				return;
			}

			Vector3 p1 = polyline.at(size - 2);
			Vector3 p2 = polyline.at(size - 1);
			Vector3 dir1 = (p2 - p1).normalisedCopy();
			Vector3 dir2 = (addPoint - p2).normalisedCopy();
			if(dir1.dotProduct(dir2) > 1 - EPSILON)
			{
				polyline.pop_back();
			}
			polyline.push_back(addPoint);
			aab.merge(addPoint);
			if((aab.getMinimum().x == addPoint.x && addPoint.z <= vMin.z) || (aab.getMinimum().z == addPoint.z && addPoint.x <= vMin.x))
			{
				vMin = addPoint;
			}
		}

		void InsertFrontPointToPolyline(std::vector<Vector3>& polyline, AxisAlignedBox& aab, Vector3& vMin, Vector3 addPoint)
		{
			size_t size = polyline.size();
			if(size < 2)
			{
				polyline.insert(polyline.begin(), addPoint);
				aab.merge(addPoint);
				if(aab.getMinimum().x == addPoint.x || aab.getMinimum().z == addPoint.z)
				{
					vMin = addPoint;
				}
				return;
			}

			Vector3 p1 = polyline.at(1);
			Vector3 p2 = polyline.at(0);
			Vector3 dir1 = (p2 - p1).normalisedCopy();
			Vector3 dir2 = (addPoint - p2).normalisedCopy();
			if(dir1.dotProduct(dir2) > 1 - EPSILON)
			{
				polyline.erase(polyline.begin());
			}
			polyline.insert(polyline.begin(), addPoint);
			aab.merge(addPoint);
			if((aab.getMinimum().x == addPoint.x && addPoint.z <= vMin.z) || (aab.getMinimum().z == addPoint.z && addPoint.x <= vMin.x))
			{
				vMin = addPoint;
			}
		}

		void UCoreUtil::UtilLinesToPolygons(std::vector<std::pair<std::vector<Vector3>, AxisAlignedBox>>& outPolygons, std::vector<Ogre::Vector3>& outMins, 
			std::vector<std::pair<Vector3, Vector3>>& lines, bool bOnlyClosePolyline)
		{
			if(lines.size() < 2) return;

			std::vector<Vector3> polyline;
			AxisAlignedBox aab;
			Vector3 vMin;
			std::pair<Vector3, Vector3> line = lines.at(0);
			lines.erase(lines.begin());

			std::vector<std::pair<Vector3, Vector3>>::iterator it;
			bool bFind = false;
			for(it = lines.begin(); it != lines.end(); it++)
			{
				if(EqualVector3(line.first, it->first))
				{
					InsertPointToPolyline(polyline, aab, vMin, line.second);
					InsertPointToPolyline(polyline, aab, vMin, it->first);
					InsertPointToPolyline(polyline, aab, vMin, it->second);
					bFind = true;
				}
				else if(EqualVector3(line.first, it->second))
				{
					InsertPointToPolyline(polyline, aab, vMin, line.second);
					InsertPointToPolyline(polyline, aab, vMin, it->second);
					InsertPointToPolyline(polyline, aab, vMin, it->first);
					bFind = true;
				}
				else if(EqualVector3(line.second, it->first))
				{
					InsertPointToPolyline(polyline, aab, vMin, line.first);
					InsertPointToPolyline(polyline, aab, vMin, it->first);
					InsertPointToPolyline(polyline, aab, vMin, it->second);
					bFind = true;
				}
				else if(EqualVector3(line.second, it->second))
				{
					InsertPointToPolyline(polyline, aab, vMin, line.first);
					InsertPointToPolyline(polyline, aab, vMin, it->second);
					InsertPointToPolyline(polyline, aab, vMin, it->first);
					bFind = true;
				}
				if(bFind)
				{
					lines.erase(it);
					break;
				}
			}
			if(bFind)
			{
				if(FindNextLine(polyline, aab, vMin, lines))
				{
					polyline.pop_back();
					Vector3 vPrev = polyline.back();
					Vector3 vCurr = polyline.front();
					Vector3 vNext = polyline.at(1);
					Vector3 dir1 = (vCurr - vPrev).normalisedCopy();
					Vector3 dir2 = (vNext - vCurr).normalisedCopy();
					if(dir1.dotProduct(dir2) > 1 - EPSILON)
					{
						polyline.erase(polyline.begin());
					}

					std::pair<std::vector<Vector3>, AxisAlignedBox> polygon;
					polygon.first = polyline;
					polygon.second = aab;
					outPolygons.push_back(polygon);
					outMins.push_back(vMin);
				}
				else if(!bOnlyClosePolyline)
				{
					Vector3 vPrev = polyline.back();
					Vector3 vCurr = polyline.front();
					Vector3 vNext = polyline.at(1);
					Vector3 dir1 = (vCurr - vPrev).normalisedCopy();
					Vector3 dir2 = (vNext - vCurr).normalisedCopy();
					if(dir1.dotProduct(dir2) > 1 - EPSILON)
					{
						polyline.erase(polyline.begin());
					}
					std::pair<std::vector<Vector3>, AxisAlignedBox> polygon;
					polygon.first = polyline;
					polygon.second = aab;
					outPolygons.push_back(polygon);
					outMins.push_back(vMin);
				}
			}
			UtilLinesToPolygons(outPolygons, outMins, lines, bOnlyClosePolyline);
		}

		bool UCoreUtil::FindNextLine(std::vector<Vector3>& polyline, AxisAlignedBox& aab, Vector3& vMin, std::vector<std::pair<Vector3, Vector3>>& lines)
		{
			std::vector<std::pair<Vector3, Vector3>>::iterator it;
			bool bFind = false;
			for(it = lines.begin(); it != lines.end(); it++)
			{
				if(EqualVector3(polyline.back(), it->first))
				{
					InsertPointToPolyline(polyline, aab, vMin, it->second);
					bFind = true;
				}
				else if(EqualVector3(polyline.back(), it->second))
				{
					InsertPointToPolyline(polyline, aab, vMin, it->first);
					bFind = true;
				}
				else if(EqualVector3(polyline.front(), it->first))
				{
					InsertFrontPointToPolyline(polyline, aab, vMin, it->second);
					bFind = true;
				}
				else if(EqualVector3(polyline.front(), it->second))
				{
					InsertFrontPointToPolyline(polyline, aab, vMin, it->first);
					bFind = true;
				}
				if(bFind)
				{
					lines.erase(it);
					break;
				}
			}
			if(bFind)
			{
				if(EqualVector3(polyline.front(), polyline.back()))
				{
					return true;
				}
				else
				{
					return FindNextLine(polyline, aab, vMin, lines);
				}
			}

			return false;
		}

		long GetNextActive(long x, long vertexCount, const bool *active)
		{
			for(long t = 0; t <= vertexCount*30; t++)
			{
				if(++x == vertexCount) x = 0;
				if(active[x]) return (x);
			}

			return -1;
		}

		long UCoreUtil::UtilTriangulatePolygon(const std::vector<Ogre::Vector3>* points, const Ogre::Vector3& normal, TringleIndex* triangle)
		{
			long vertexCount = long(points->size());
			bool *active = new bool[vertexCount];

			long triangleCount = 0;
			long start = 0;
			long p1 = 0;
			long p2 = 1;
			long m1 = vertexCount - 1;
			long m2 = vertexCount - 2;
			const float epsilon2 = 0.001f;

			bool lastPositive = false;

			for(long a = 0; a < vertexCount; a++)
				active[a] = true;

			for(;;)
			{
				if(p2 == m2) // 3개의 점만으로 이루어짐
				{
					triangle->index[0] = m1;
					triangle->index[1] = p1;
					triangle->index[2] = p2;
					triangleCount++;
					break;
				}

				const Vector3& vp1 = points->at(p1);
				const Vector3& vp2 = points->at(p2);
				const Vector3& vm1 = points->at(m1);
				const Vector3& vm2 = points->at(m2);
				bool positive = false;
				bool negative = false;

				Vector3 n1 = normal.crossProduct((vm1 - vp2).normalisedCopy());
				if(n1.dotProduct(vp1 - vp2) > epsilon2) // 예각이면.. (p1->p2->m1 이 반 시계방향)
				{
					positive = true;
					Vector3 n2 = (normal.crossProduct((vp1 - vm1).normalisedCopy()));
					Vector3 n3 = (normal.crossProduct((vp2 - vp1).normalisedCopy()));

					// 삼각형 안에 다른 점이 포함되는지 검사
					for(long a = 0; a < vertexCount; a++) 
					{
						if((active[a]) && (a != p1) && (a != p2) && (a != m1))
						{
							const Vector3& v = points->at(a);
							// 하나의 정점에 대해서라도 3가지다 예각이면(p1 , p2, m1 이 이루는 삼각형 안에 v가 포함됨)
							if( (n1.dotProduct((v - vp2).normalisedCopy()) > -epsilon2) &&
								(n2.dotProduct((v - vm1).normalisedCopy()) > -epsilon2) &&
								(n3.dotProduct((v - vp1).normalisedCopy()) > -epsilon2)) 
							{
								positive = false;
								break;
							}
						}
					}
				}

				n1 = normal.crossProduct((vm2 - vm1).normalisedCopy());
				if(n1.dotProduct(vm1 - vp1) > epsilon2) // p1 -> m1 -> m2 가 반시계방향
				{
					negative = true;
					Vector3 n2 = (normal.crossProduct((vm1 - vm2).normalisedCopy()));
					Vector3 n3 = (normal.crossProduct((vp1 - vm1).normalisedCopy()));

					// 삼각형 안에 다른 점이 포함되는지 검사
					for(long a = 0; a < vertexCount; a++)
					{
						if((active[a]) && (a != m1) && (a != m2) && (a != p1))
						{
							const Vector3& v = points->at(a);
							// 하나의 정점에 대해서라도 3가지다 예각이면(p1 , m1, m2 이 이루는 삼각형 안에 v가 포함됨)
							if( (n1.dotProduct((v - vp1).normalisedCopy()) > -epsilon2) &&
								(n2.dotProduct((v - vm2).normalisedCopy()) > -epsilon2) &&
								(n3.dotProduct((v - vm1).normalisedCopy()) > -epsilon2))
							{
								negative = false;
								break;
							}
						}
					}
				}

				if( positive && negative ) // (p1, p2, m1) 과 (p1, m1, m2) 가 둘다 가능 하면(두 삼각형이 서로 교차한다)
				{
					double pd = (vp2 - vm1).normalisedCopy().dotProduct((vm2 - vm1).normalisedCopy());
					double md = (vm2 - vp1).normalisedCopy().dotProduct((vp2 - vp1).normalisedCopy());

					if(fabs(pd - md) < epsilon2) // 두각이 거의 일치할 때 두 삼각형 중 하나를 제거한다.
					{
						if(lastPositive) positive = false; // 이전에 negative 삼각형을 제거 했으면 positive 삼각형을 제거 하고 아니면 negatriv 삼각형을 제거한다.
						else negative = false;
					}
					else
					{
						if(pd < md) negative = false; //각이 큰 삼각형을 제거한다.
						else positive = false;
					}
				}

				if(positive)
				{
					active[p1] = false; // 세점의 중심에 있는 점을 제거한다.
					triangle->index[0] = m1;
					triangle->index[1] = p1;
					triangle->index[2] = p2;
					triangleCount++;
					triangle++;

					p1 = GetNextActive(p1, vertexCount, active);
					p2 = GetNextActive(p2, vertexCount, active);
					lastPositive = true;
					start = -1;
				}
				else if(negative)
				{
					active[m1] = false;
					triangle->index[0] = m2;
					triangle->index[1] = m1;
					triangle->index[2] = p1;
					triangleCount++;
					triangle++;

					m1 = GetNextActive(m1, vertexCount, active);
					m2 = GetNextActive(m2, vertexCount, active);
					lastPositive = false;
					start = -1;
				}
				else
				{
					if(start == -1) start = p2;
					else if(p2 == start) break;

					m2 = m1;
					m1 = p1;
					p1 = p2;
					p2 = GetNextActive(p2, vertexCount, active);
				}
			}

			delete [] active;
			return triangleCount;
		}

		long UCoreUtil::UtilSimpleTriangulatePolygon(const std::vector<Vector3>* points, TringleIndex* triangle)
		{
			long triangleCount = long(points->size()) - 2;
			for(long i = 0; i < triangleCount; i++)
			{
				triangle[i].index[0] = 0;
				triangle[i].index[1] = i + 1;
				triangle[i].index[2] = i + 2;
			}
			return triangleCount;
		}

		void UCoreUtil::UtilAddPassWireDraw(Ogre::SceneNode* pRootNode, ColourValue wireColor, bool bAddWire)
		{
			if(!pRootNode) return;

			Ogre::SceneNode::ChildNodeIterator it = pRootNode->getChildIterator();
			Ogre::SceneNode* pTmpSceneNode = NULL;
			while(it.hasMoreElements())
			{
				pTmpSceneNode = static_cast<Ogre::SceneNode*>(it.peekNextValue());
				String name = pTmpSceneNode->getName();

				if(pTmpSceneNode->numChildren() != 0)
				{
					UtilAddPassWireDraw(pTmpSceneNode, wireColor, bAddWire);
				}

				unsigned short childNum = pTmpSceneNode->numAttachedObjects();
				for(unsigned short i = 0; i < childNum; i++)
				{
					Ogre::MovableObject* pTmpMovable = pTmpSceneNode->getAttachedObject(i);
					if(pTmpMovable->getMovableType().compare("Entity") == 0)
					{
						Entity* pTmpEntity = static_cast<Entity*>(pTmpMovable);
						unsigned int subNum = pTmpEntity->getNumSubEntities();
						for(unsigned int j = 0; j < subNum; j++)
						{
							MaterialPtr mtr = pTmpEntity->getSubEntity(j)->getMaterial();
							if(bAddWire)
							{
								bool bWire = false;
								unsigned short passNum = mtr->getTechnique(0)->getNumPasses();
								for(unsigned short k = 0; k < passNum; k++)
								{
									if(mtr->getTechnique(0)->getPass(k)->getPolygonMode() == PM_WIREFRAME)
									{
										bWire = true;
										break;
									}
								}
								if(!bWire)
								{
									Pass* pass = mtr->getTechnique(0)->createPass();
									pass->setDiffuse(wireColor);
									pass->setAmbient(wireColor);
									pass->setPolygonMode(PM_WIREFRAME);
								}
							}
							else
							{
								unsigned short passNum = mtr->getTechnique(0)->getNumPasses();
								for(unsigned short k = 0; k < passNum; k++)
								{
									if(mtr->getTechnique(0)->getPass(k)->getPolygonMode() == PM_WIREFRAME)
									{
										mtr->getTechnique(0)->removePass(k);
										break;
									}
								}
							}
						}
					}
				}
				it.moveNext();
			}
		}

		void UCoreUtil::UtilOnlyColorDraw(Ogre::SceneNode* pRootNode, ColourValue color, bool bOnlyColor, std::map<Ogre::String, std::pair<Ogre::ColourValue, Ogre::ColourValue>>& colorMap)
		{
			if(!pRootNode) return;

			Ogre::SceneNode::ChildNodeIterator it = pRootNode->getChildIterator();
			Ogre::SceneNode* pTmpSceneNode = NULL;
			while(it.hasMoreElements())
			{
				pTmpSceneNode = static_cast<Ogre::SceneNode*>(it.peekNextValue());
				String name = pTmpSceneNode->getName();

				if(pTmpSceneNode->numChildren() != 0)
				{
					UtilOnlyColorDraw(pTmpSceneNode, color, bOnlyColor, colorMap);
				}

				unsigned short childNum = pTmpSceneNode->numAttachedObjects();
				for(unsigned short i = 0; i < childNum; i++)
				{
					Ogre::MovableObject* pTmpMovable = pTmpSceneNode->getAttachedObject(i);
					if(pTmpMovable->getMovableType().compare("Entity") == 0)
					{
						Entity* pTmpEntity = static_cast<Entity*>(pTmpMovable);
						unsigned int subNum = pTmpEntity->getNumSubEntities();
						for(unsigned int j = 0; j < subNum; j++)
						{
							MaterialPtr mtr = pTmpEntity->getSubEntity(j)->getMaterial();
							Pass* pPass = mtr->getTechnique(0)->getPass(0);
							if(bOnlyColor)
							{
								unsigned short textureNum = pPass->getNumTextureUnitStates();
								if(textureNum != 0)
								{
									pPass->getTextureUnitState(0)->setColourOperationEx(LBX_SOURCE1, LBS_CURRENT, LBS_CURRENT);
									if(colorMap.find(mtr->getName()) == colorMap.end())
									{
										std::pair<ColourValue, ColourValue> colors;
										colors.first = pPass->getDiffuse();
										colors.second = pPass->getAmbient();
										colorMap[mtr->getName()] = colors;
									}
									pPass->setDiffuse(color);
									pPass->setAmbient(color);
								}
							}
							else
							{
								unsigned short textureNum = pPass->getNumTextureUnitStates();
								if(textureNum != 0)
								{
									pPass->getTextureUnitState(0)->setColourOperationEx(LBX_MODULATE);
									if(colorMap.find(mtr->getName()) != colorMap.end())
									{
										std::pair<ColourValue, ColourValue> colors = colorMap[mtr->getName()];
										pPass->setDiffuse(colors.first);
										pPass->setAmbient(colors.second);
										colorMap.erase(mtr->getName());
									}
								}
							}
						}
					}
				}
				it.moveNext();
			}
		}

		void UCoreUtil::UtilColorChange(Ogre::SceneNode* pRootNode, Ogre::ColourValue color, bool bColorChange, std::map<Ogre::String, std::vector<Ogre::MaterialPtr>>& materialMap)
		{
			if(!pRootNode) return;

			Ogre::SceneNode::ChildNodeIterator it = pRootNode->getChildIterator();
			Ogre::SceneNode* pTmpSceneNode = NULL;
			while(it.hasMoreElements())
			{
				pTmpSceneNode = static_cast<Ogre::SceneNode*>(it.peekNextValue());
				String name = pTmpSceneNode->getName();

				if(pTmpSceneNode->numChildren() != 0)
				{
					UtilColorChange(pTmpSceneNode, color, bColorChange, materialMap);
				}

				unsigned short childNum = pTmpSceneNode->numAttachedObjects();
				for(unsigned short i = 0; i < childNum; i++)
				{
					Ogre::MovableObject* pTmpMovable = pTmpSceneNode->getAttachedObject(i);
					if(pTmpMovable->getMovableType().compare("Entity") == 0)
					{
						Entity* pTmpEntity = static_cast<Entity*>(pTmpMovable);
						String entityName = pTmpEntity->getName();
						unsigned int subNum = pTmpEntity->getNumSubEntities();
						if(bColorChange)
						{
							std::vector<MaterialPtr> mtrvector;
							for(unsigned int j = 0; j < subNum; j++)
							{
								String newMtrName = entityName + StringConverter::toString(subNum) + "CopyMtr";
								MaterialPtr originMtr = pTmpEntity->getSubEntity(j)->getMaterial();
								mtrvector.push_back(originMtr);
								MaterialPtr mtr = originMtr->clone(newMtrName);
								Pass* pPass = mtr->getTechnique(0)->getPass(0);
								pPass->setDiffuse(color);
								pPass->setAmbient(color);
								pTmpEntity->getSubEntity(j)->setMaterial(mtr);
							}
							materialMap[entityName] = mtrvector;
						}
						else
						{
							if(materialMap.find(entityName) != materialMap.end())
							{
								std::vector<MaterialPtr> mtrvector = materialMap[entityName];
								for(unsigned int j = 0; j < subNum; j++)
								{
									MaterialPtr prevMtr = pTmpEntity->getSubEntity(j)->getMaterial();
									pTmpEntity->getSubEntity(j)->setMaterial(mtrvector.at(j));
									MaterialManager::getSingleton().remove(prevMtr->getHandle());
								}
								materialMap.erase(entityName);
							}
						}
					}
				}
				it.moveNext();
			}
		}

		void UCoreUtil::UtilAlphaDraw(Ogre::SceneNode* pRootNode, float alpha, bool bAlpha)
		{
			if(!pRootNode) return;

			Ogre::SceneNode::ChildNodeIterator it = pRootNode->getChildIterator();
			Ogre::SceneNode* pTmpSceneNode = NULL;
			while(it.hasMoreElements())
			{
				pTmpSceneNode = static_cast<Ogre::SceneNode*>(it.peekNextValue());
				String name = pTmpSceneNode->getName();

				if(pTmpSceneNode->numChildren() != 0)
				{
					UtilAlphaDraw(pTmpSceneNode, alpha, bAlpha);
				}

				unsigned short childNum = pTmpSceneNode->numAttachedObjects();
				for(unsigned short i = 0; i < childNum; i++)
				{
					Ogre::MovableObject* pTmpMovable = pTmpSceneNode->getAttachedObject(i);
					if(pTmpMovable->getMovableType().compare("Entity") == 0)
					{
						Entity* pTmpEntity = static_cast<Entity*>(pTmpMovable);
						unsigned int subNum = pTmpEntity->getNumSubEntities();
						for(unsigned int j = 0; j < subNum; j++)
						{
							MaterialPtr mtr = pTmpEntity->getSubEntity(j)->getMaterial();
							if(mtr->getName().find("Glass") == String::npos)
							{
								if(bAlpha)
								{
									mtr->getTechnique(0)->getPass(0)->setSceneBlending(SBT_TRANSPARENT_ALPHA);
									mtr->getTechnique(0)->getPass(0)->setDepthWriteEnabled(false);
									ColourValue diffuse = mtr->getTechnique(0)->getPass(0)->getDiffuse();
									diffuse.a = alpha;
									mtr->getTechnique(0)->getPass(0)->setDiffuse(diffuse);
								}
								else
								{
									mtr->getTechnique(0)->getPass(0)->setSceneBlending(SBT_REPLACE);
									mtr->getTechnique(0)->getPass(0)->setDepthWriteEnabled(true);
									ColourValue diffuse = mtr->getTechnique(0)->getPass(0)->getDiffuse();
									diffuse.a = 1;
									mtr->getTechnique(0)->getPass(0)->setDiffuse(diffuse);
								}
							}
						}
					}
				}
				it.moveNext();
			}
		}

		AxisAlignedBox MakeAAB(const std::vector<Vector3>& points, const bool *active)
		{
			size_t vertexCount = points.size();
			AxisAlignedBox aab;
			for(size_t i = 0; i < vertexCount; i++)
			{
				if(active[i])
					aab.merge(points.at(i));
			}
			return aab;
		}

		long GetPrevIndex(long currIdx, long vertexCount, const bool *active)
		{
			for(long t = 0; t <= vertexCount*30; t++)
			{
				if(--currIdx < 0) currIdx = vertexCount - 1;
				if(active[currIdx]) return (currIdx);
			}

			return -1;
		}

		long GetNextIndex(long currIdx, long vertexCount, const bool *active)
		{
			for(long t = 0; t <= vertexCount*30; t++)
			{
				if(++currIdx == vertexCount) currIdx = 0;
				if(active[currIdx]) return (currIdx);
			}

			return -1;
		}

		bool UtilIsPointInTriangle(const std::vector<Vector3>& points, Vector3 vP1, Vector3 vP2, Vector3 vP3, Vector3 normal, size_t& InIdx)
		{
			for(size_t i = 0; i < points.size(); i++)
			{
				Vector3 vCurr = points.at(i);
				if(vCurr != vP1 && vCurr != vP2 && vCurr != vP3)
				{
					if(Math::pointInTri3D(vCurr, vP1, vP2, vP3, normal))
					{
						InIdx = i;
						return true;
					}
				}
			}

			return false;
		}

		bool MakeTringle(const std::vector<Vector3>& points, const Vector3& vMin, const Vector3& vMax, const Vector3& normal, long currIdx, bool *active, 
			long& triangleCount, TringleIndex* triangle, int dir = 0)
		{
			long vertexCount = (long)points.size();
			if(active[currIdx])
			{
				Vector3 vCurr = points.at(currIdx);
				long prevIdx = GetPrevIndex(currIdx, vertexCount, active);
				long nextIdx = GetNextIndex(currIdx, vertexCount, active);
				Vector3 vPrev = points.at(prevIdx);
				Vector3 vNext = points.at(nextIdx);
				Vector3 dir1 = (vCurr - vPrev).normalisedCopy();
				Vector3 dir2 = (vNext - vCurr).normalisedCopy();
				if(dir1.dotProduct(dir2) > 1 - EPSILON)
				{
					return false;
				}
				if(vMin.x == vCurr.x || vMin.y == vCurr.y || vMin.z == vCurr.z || vMax.x == vCurr.x || vMax.y == vCurr.y || vMax.z == vCurr.z)
				{
					size_t InIdx = 0;
					if(!UtilIsPointInTriangle(points, vPrev, vCurr, vNext, normal, InIdx))
					{
						active[currIdx] = false;
						triangle[triangleCount].index[0] = prevIdx;
						triangle[triangleCount].index[1] = currIdx;
						triangle[triangleCount].index[2] = nextIdx;
						triangleCount++;
						return true;
					}
					else
					{
						long pprevIdx = GetPrevIndex(prevIdx, vertexCount, active);
						long nnextIdx = GetNextIndex(nextIdx, vertexCount, active);
						if(InIdx == pprevIdx && (dir == 0 || dir == 1))
						{
							return MakeTringle(points, vMin, vMax, normal, prevIdx, active, triangleCount, triangle, 1);
						}
						else if(InIdx == nnextIdx && (dir == 0 || dir == 2))
						{
							return MakeTringle(points, vMin, vMax, normal, nnextIdx, active, triangleCount, triangle, 2);
						}
					}
				}
			}

			return false;
		}

		void MakePolygon(const std::vector<Vector3>& points, const Vector3& normal, TringleIndex* triangle, bool *active, long& triangleCount)
		{
			long vertexCount = (long)points.size();
			AxisAlignedBox aab = MakeAAB(points, active);

			Vector3 vMin = aab.getMinimum();
			Vector3 vMax = aab.getMaximum();

			for(long i = 0; i < vertexCount; i++)
			{
				if(MakeTringle(points, vMin, vMax, normal, i, active, triangleCount, triangle))
				{
					break;
				}
			}
		}

		long UCoreUtil::UtilTriangulatePolygonEx(const std::vector<Vector3>& points, const Vector3& normal, TringleIndex* triangle)
		{
			long triangleCount = 0;
			long vertexCount = (long)points.size();

			if(vertexCount == 3)
			{
				triangle->index[0] = 0;
				triangle->index[1] = 1;
				triangle->index[2] = 2;
				return 1;
			}
			else if(vertexCount < 3)
			{
				return 0;
			}

			bool *active = new bool[vertexCount];
			for(long a = 0; a < vertexCount; a++)
				active[a] = true;

			bool bContinue = true;
			int count = 0;
			while(bContinue)
			{
				MakePolygon(points, normal, triangle, active, triangleCount);
				bContinue = false;
				int activecount = 0;
				for(int i = 0; i < vertexCount; i++)
				{
					if(active[i])
					{
						activecount++;
					}
				}
				if(activecount > 2)
				{
					bContinue = true;
				}
				count++;
				if(count > vertexCount * 30)
				{
					break;
				}
			}

			return triangleCount;
		}

		bool UCoreUtil::UtilEqualLine(Ogre::Vector3 a1, Ogre::Vector3 a2, Ogre::Vector3 b1, Ogre::Vector3 b2)
		{
			return ((EqualVector3Ez(a1, b1) && EqualVector3Ez(a2, b2)) || (EqualVector3Ez(a1, b2) && EqualVector3Ez(a2, b1)));
		}
	}// namespace Core
}// namespace UnE