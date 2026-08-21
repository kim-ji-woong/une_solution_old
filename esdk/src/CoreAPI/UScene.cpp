#include "StdAfx.h"
#include "UScene.h"
#include "UMath.h"
#include "UVector3.h"
#include "UAxisAlignedBox.h"

namespace UnE
{
	namespace Core
	{
		//-------------------------------------------------------------------------
		UNode::UNode(void)
		{
			mParent = NULL;
			mTag = NULL;
		}
		//-------------------------------------------------------------------------
		UNode::~UNode(void)
		{
			mParent = NULL;
		}
		//-------------------------------------------------------------------------
		unsigned int UNode::GetNumChilds()
		{
			return (unsigned int)mChilds.size();
		}
		//-------------------------------------------------------------------------
		UNode* UNode::GetParent()
		{
			return mParent;
		}

		UNode* UNode::ChildAt( int idx )
		{
			int count = 0;
			std::list<UNode*>::iterator iter = mChilds.begin();
			for( ; iter != mChilds.end(); iter++)
			{				
				UNode * pNode = *iter;
				if( count == idx)
					return pNode;
				count++;
			}
			return NULL;
		}

		void UNode::SetTag( void * val )
		{
			mTag = val;
		}


		//////////////////////////////////////////////////////////////////////////
		// USceneNode Implementation
			
		USceneNode::USceneNode()
			: UNode()
		{
			mParent = NULL;
			mSceneName = "";
			mbIncludeScene = false;			
			mbShowBound = false;
			mbVisible = true;
		}
		//-------------------------------------------------------------------------
		USceneNode::~USceneNode()
		{
			std::list<UNode*>::iterator iter = mChilds.begin();
			for( ; iter != mChilds.end(); iter++)
			{
				USceneNode * pNode = (USceneNode*)*iter;
				if( pNode != NULL)
					delete pNode;
			}
			mChilds.clear();
			
			/*if( mParent != NULL)
			{
				USceneNode * pNode = (USceneNode*)mParent;
				pNode->RemoveChild(mSceneName);
			}*/
		}
		//-------------------------------------------------------------------------
		USceneNode* USceneNode::CreateChild( std::string& szName )
		{
			USceneNode * pNode = new USceneNode();
			pNode->mSceneName = szName;
			pNode->mParent = this;
			this->mbIncludeScene = true;
			mChilds.push_back(pNode);
		
			return pNode;
		}
		//-------------------------------------------------------------------------
		USceneNode* USceneNode::GetParentScene()
		{
			return (USceneNode*)mParent;
		}

		USceneNode* USceneNode::AddChild( USceneNode* pNode )
		{
			pNode->mParent = this;			
			mChilds.push_back(pNode);
			return pNode;
		}

		USceneNode* USceneNode::RemoveChild( std::string& szName )
		{
			std::list<UNode*>::iterator iter = mChilds.begin();
			for( ; iter != mChilds.end(); iter++)
			{
				USceneNode * pNode = (USceneNode*)*iter;
				if( pNode->mSceneName == szName)
				{
					mChilds.remove(pNode);
					pNode->mParent = NULL;
					return pNode;
				}
			}
			return NULL;
		}

		void USceneNode::ShowBoundingBox(bool bshow)
		{
			Ogre::SceneNode * pNode = (Ogre::SceneNode *)mTag;
			if( pNode != NULL)
				pNode->showBoundingBox(bshow);			
		}

		void USceneNode::SetVisible( bool bVisible )
		{
			mbVisible = bVisible;
			
			if( mParent == NULL)
			{
				std::list<UNode*>::iterator iter = mChilds.begin();
				for( ; iter != mChilds.end(); iter++)
				{
					USceneNode * pUNode = (USceneNode*)*iter;
					Ogre::SceneNode * pNode = (Ogre::SceneNode *)pUNode->mTag;
					pNode->setVisible(mbVisible);
				}
			}
			
			Ogre::SceneNode * pNode = (Ogre::SceneNode *)mTag;
			if( pNode != NULL)
				pNode->setVisible(mbVisible);	
			
			
		}

		USceneNode* USceneNode::GetChildNode( std::string& szName )
		{
			std::list<UNode*>::iterator iter = mChilds.begin();
			for( ; iter != mChilds.end(); iter++)
			{
				USceneNode * pNode = (USceneNode*)*iter;
				if( pNode->mSceneName == szName)
				{					
					return pNode;
				}
				USceneNode * pChildNode = pNode->GetChildNode(szName);
				if( pChildNode != NULL)
					return pChildNode;
			}
			return NULL;
		}

		void USceneNode::GetPosition( UnE::Math::Vector3& pos )
		{
			Ogre::SceneNode * pNode = (Ogre::SceneNode *)mTag;
			if( pNode != NULL)
			{
				//Ogre::Vector3 vCenter = pNode->_getDerivedPosition();
				Ogre::AxisAlignedBox aabox = pNode->_getWorldAABB();
				Ogre::Vector3 vCenter = pNode->_getWorldAABB().getCenter();
				pos.x = vCenter.x;
				pos.y = vCenter.y;
				pos.z = vCenter.z;
					 
			}
		}

		void USceneNode::SetPosition( UnE::Math::Vector3& pos )
		{
			Ogre::SceneNode * pNode = (Ogre::SceneNode *)mTag;
			if( pNode != NULL)
			{
				//pNode->setPosition(Ogre::Vector3(pos.x, pos.y, pos.z));
				UnE::Math::Vector3 orgPos;
				GetPosition(orgPos);
				UnE::Math::Vector3 res = pos - orgPos;
				pNode->translate(res.x, res.y, res.z, Ogre::Node::TS_WORLD);
				Ogre::Vector3 vec(pos.x, pos.y, pos.z);				
				pNode->_update(true, false);
			}
		}

		void USceneNode::GetBoundBox( UnE::Math::AxisAlignedBox& aabb )
		{
			Ogre::SceneNode * pNode = (Ogre::SceneNode *)mTag;
			if( pNode != NULL)
			{
				Ogre::AxisAlignedBox waabb = pNode->_getWorldAABB();
				aabb.setExtents(waabb.getMinimum().x, waabb.getMinimum().y, waabb.getMinimum().z,
								waabb.getMaximum().x, waabb.getMaximum().y, waabb.getMaximum().z);
			}
		}

		void USceneNode::UpdateBound()
		{
			Ogre::SceneNode * pNode = (Ogre::SceneNode *)mTag;
			if( pNode != NULL)
			{
				pNode->needUpdate();
			}
		}
		
		void USceneNode::SetHeading( UnE::Math::Vector3& dir )
		{
			Ogre::SceneNode * pNode = (Ogre::SceneNode *)mTag;
			if( pNode != NULL)
			{
				UnE::Math::Vector3 orgPos;
				GetPosition(orgPos);

				UnE::Math::Vector3 vPos = UnE::Math::Vector3::ZERO;
				SetPosition(vPos);
			    

				Ogre::Vector3 vDir = Ogre::Vector3(dir.x, dir.y, dir.z);
				vDir.y = 0;
				vDir.normalise();
				float angle = Ogre::Math::ACos((Ogre::Vector3::UNIT_Z).dotProduct(vDir)).valueDegrees();
				if(vDir.x > 0)
				{
					angle *= -1;
				}

				angle -= 270;

				if(angle > 360)
				{
					angle = angle - (int(angle/360) * 360);
				}
				else if(angle < 0)
				{
					angle = 360 + (angle - (int(angle/360) * 360));
				}
				
				Ogre::Quaternion quat(Ogre::Radian(Ogre::Degree(angle)), -Ogre::Vector3::UNIT_Y); // heading+180을 하면 기준은 Z축 하지 않으면 -Z축
				pNode->setOrientation(quat);				
				
				char buf[1096];
			    SetPosition(orgPos);
				sprintf(buf, "NODE : %f, %f, %f",orgPos.x, orgPos.y, orgPos.z );
				Ogre::LogManager::getSingleton().logMessage(Ogre::String(buf));

			}
		}

		void USceneNode::Translate( float x, float y, float z )
		{
			Ogre::SceneNode * pNode = (Ogre::SceneNode *)mTag;
			if( pNode != NULL)
			{
				pNode->translate(x, y, z,  Ogre::Node::TS_WORLD);
				pNode->_update(true, false);
			}
		}

		void USceneNode::Move( float x, float y, float z , bool updateOrientation)
		{
			Ogre::SceneNode * pNode = (Ogre::SceneNode *)mTag;
			if ( updateOrientation )
			{				
				Ogre::Vector3 prevPos = pNode->getPosition();
				Ogre::Real fy = prevPos.z - z;
				Ogre::Real fx = prevPos.x - x;

				if ( fabs(fy) != 0 && fabs(fx) != 0 )
				{
					Ogre::Radian r = Ogre::Math::ATan2(fy, fx);
					r += Ogre::Radian(Ogre::Degree(90));

					SetRotation(-(r.valueRadians()));
				}
			}

			pNode->translate(Ogre::Vector3(x,y,z));
			
		}
		void USceneNode::SetRotation( float r )
		{
			Ogre::SceneNode * pNode = (Ogre::SceneNode *)mTag;
			if( pNode != NULL)
			{
				Ogre::Quaternion q(Ogre::Radian(r) + Ogre::Radian(Ogre::Math::PI), Ogre::Vector3::UNIT_Y);
				pNode->setOrientation(q);
			}
		}

		void USceneNode::GetDirection( UnE::Math::Vector3& vDir )
		{
			Ogre::SceneNode * pNode = (Ogre::SceneNode *)mTag;
			if( pNode != NULL)
			{
				Ogre::Quaternion direction;
				Ogre::Vector3 vRet;
				direction = pNode->getOrientation();
				direction.ToAxes(&vRet);
				vDir = UnE::Math::Vector3(vRet.x, vRet.y, vRet.z);
			}
		}

		void USceneNode::Dettach()
		{
			/*Ogre::SceneNode * pNode = (Ogre::SceneNode *)mTag;
			if( pNode != NULL)
			{
				pNode->detachAllObjects();
				pNode->removeAndDestroyAllChildren();				
			}	*/		
		}

		//-------------------------------------------------------------------------	

		

		//////////////////////////////////////////////////////////////////////////
		// USceneNodeManager Implementation
		USceneNodeManager::USceneNodeManager()
		{
			mRoot = new USceneNode();
		}
		//-------------------------------------------------------------------------
		USceneNodeManager::~USceneNodeManager()
		{
			if( mRoot != NULL)
				delete mRoot;
			mRoot = NULL;
		}
		//-------------------------------------------------------------------------
		USceneNode* USceneNodeManager::GetRootSceneNode()
		{
			return mRoot;
		}

		USceneNode* USceneNodeManager::FindSceneNode( std::string szName )
		{
			if (mRoot == NULL)
				return NULL;
			return mRoot->GetChildNode(szName);
		}

		//-------------------------------------------------------------------------
	}
}
