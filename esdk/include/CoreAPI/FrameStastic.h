#pragma once

#include <Ogre.h>
#include <OgreSceneManager.h>


class CFrameRate  : public Ogre::FrameListener
{
public:
	CFrameRate(Ogre::Root* mRoot, Ogre::SceneManager* pSceneManager);
	virtual ~CFrameRate();
	virtual bool frameStarted(const Ogre::FrameEvent& evt);
	virtual bool frameEnded(const Ogre::FrameEvent& evt);

	float getFrameRate();
	LONG  getVertexCount();
protected:
	LONG CountVertex( Ogre::SceneNode* _pNode );

	Ogre::FrameEvent m_Evt1;
	Ogre::FrameEvent m_Evt2;
	bool bFrame;

	Ogre::SceneManager*		mSceneMgr;	
	Ogre::Root*				mRoot;
	float fps;
};