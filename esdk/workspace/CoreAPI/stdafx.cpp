#include "stdafx.h"


#include <map>
//////////////////////////////////////////////////////////////////////////
// global object

UnE::Core::WndCtx	*   g_WndCtx;
UnE::Core::WndCtx	*   m_SubWndCtx;
Ogre::RenderSystem  *	m_renderSystem;
Ogre::Root		    *	m_Root;
std::map<HWND, UnE::Core::WndCtx*>		m_subWnds;

Ogre::AxisAlignedBox	g_AABB;


UnE::Core::WndCtx * GetWndContext(HWND hWnd)
{
	if( hWnd == NULL )
		return  NULL;

	if( g_WndCtx == NULL)
		return NULL;

	if( g_WndCtx->hWnd == hWnd)
		return g_WndCtx;
	
	if( m_SubWndCtx != NULL && m_SubWndCtx->hWnd == hWnd)
		return m_SubWndCtx;

	if(m_subWnds.size() == 0)
		return NULL;
	if( m_subWnds.find(hWnd) == m_subWnds.end())
		return NULL;

	UnE::Core::WndCtx * pWndCtx = m_subWnds[hWnd];	
	return pWndCtx;
}


void ClearAllWndCtx()
{
	if( g_WndCtx != NULL)
	{
		delete g_WndCtx;
		g_WndCtx = NULL;
	}

	if( m_SubWndCtx != NULL)
	{
		delete m_SubWndCtx;
		m_SubWndCtx = NULL;
	}

	std::map<HWND, UnE::Core::WndCtx*>::iterator iter;
	for( iter = m_subWnds.begin(); iter != m_subWnds.end(); iter++)
	{
		UnE::Core::WndCtx* pCtx = iter->second;
		if( pCtx != NULL)
		{
			delete pCtx;
		}
	}
	m_subWnds.clear();
}

void AddWndContext( UnE::Core::WndCtx* pCtx )
{
	if( pCtx == NULL)
		return;

	if( g_WndCtx == NULL)
	{
		g_WndCtx = pCtx;
		return;
	}
	if( m_SubWndCtx == NULL)
		m_SubWndCtx = pCtx;

	m_subWnds.insert(std::make_pair(pCtx->hWnd, pCtx));
}


	