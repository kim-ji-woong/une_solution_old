#include "StdAfx.h"
#include <OgreCommon.h>
#include <OgreException.h>
#include <OgreConfigFile.h>

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

#include <OgreSubMesh.h>
#include <OgreRay.h>

#include <OgreMaterialManager.h>
#include "FrameStastic.h"

using namespace Ogre;
CFrameRate::CFrameRate(Ogre::Root* pRoot, Ogre::SceneManager* pSceneManager)
{
	bFrame = false;
	mSceneMgr = pSceneManager;
	mRoot = pRoot;
	if( mRoot != NULL)
		mRoot->addFrameListener(this);
}

CFrameRate::~CFrameRate()
{
	if( mRoot != NULL )
		mRoot->removeFrameListener(this);
}

bool CFrameRate::frameStarted( const Ogre::FrameEvent& evt )
{
	bFrame = true;
	m_Evt1 = evt;
	return true;
}

bool CFrameRate::frameEnded( const FrameEvent& evt )
{
	bFrame = false;
	fps = 1.0 / evt.timeSinceLastFrame;
	m_Evt2 = evt;	
	return true;
}

float CFrameRate::getFrameRate()
{
	return fps;
}

LONG CFrameRate::getVertexCount()
{
	if( mSceneMgr != NULL)
	{
		return CountVertex(mSceneMgr->getRootSceneNode());		
	}
	return 0;
}


LONG CFrameRate::CountVertex( Ogre::SceneNode* _pNode )
{
	if( _pNode == NULL)
		return 0;
	
	Ogre::Node::ChildNodeIterator Itr = _pNode->getChildIterator();
	Ogre::SceneNode* pCurrent         = NULL;

	LONG vertexCount = 0;
	while ( Itr.hasMoreElements() )
	{
		pCurrent = dynamic_cast< Ogre::SceneNode* >( Itr.getNext() );
		if ( pCurrent != NULL )
			vertexCount += CountVertex( pCurrent );
	}

	if ( _pNode->numAttachedObjects() > 0 )
	{
		
		std::vector< Ogre::Entity* > Entities;
		uint CountOfObjects = _pNode->numAttachedObjects();		
		for ( uint i = 0; i < CountOfObjects; i++ )
		{
			Ogre::Entity* pCurrent = dynamic_cast< Ogre::Entity* >( _pNode->getAttachedObject(i) );
			if ( pCurrent != NULL )
				Entities.push_back( pCurrent );
		}

		std::vector< Ogre::Entity* >::iterator Itr = Entities.begin();
		std::vector< Ogre::Entity* >::iterator End = Entities.end();
		for ( ; Itr != End; Itr++ )
		{
			Ogre::Entity* _pEntity = *Itr;
			Ogre::Mesh*       pMesh             = _pEntity->getMesh().get();
			Ogre::VertexData* pSharedVertexData = pMesh->sharedVertexData;
			
			uint CountOfSubEntities = _pEntity->getNumSubEntities();
			for ( uint i = 0; i < CountOfSubEntities; i++ )
			{
				Ogre::SubEntity* _pEntitySub = _pEntity->getSubEntity(i);
				Ogre::SubMesh*   _pSubMesh   = _pEntitySub->getSubMesh();
				Ogre::VertexData* pVertexData = pSharedVertexData ? pSharedVertexData : _pSubMesh->vertexData;
				Ogre::IndexData*  pIndexData  = _pSubMesh->indexData;

				Ogre::VertexDeclaration* pVertexDeclaration = pVertexData->vertexDeclaration;
				uint CountOfVertexElements = pVertexDeclaration->getElementCount();
				for ( uint i = 0; i < CountOfVertexElements; i++ )
				{
					const Ogre::VertexElement* pVertexElement = pVertexDeclaration->getElement(i);
					if ( pVertexElement->getIndex() != 0 )
						continue;
					if ( pVertexElement->getSemantic() != Ogre::VES_POSITION )
						continue;

					vertexCount++;
				}
			}
		}			
	}
	return vertexCount;
}