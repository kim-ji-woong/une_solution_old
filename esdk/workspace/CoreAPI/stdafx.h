
#pragma once

#include "targetver.h"

#define WIN32_LEAN_AND_MEAN            
#define _ATL_CSTRING_EXPLICIT_CONSTRUCTORS    

#ifndef VC_EXTRALEAN
#define VC_EXTRALEAN            
#endif

#include <afx.h>
#include <afxwin.h>         
#include <afxext.h>

#ifndef _AFX_NO_OLE_SUPPORT
#include <afxdtctl.h>          
#endif

#ifndef _AFX_NO_AFXCMN_SUPPORT
#include <afxcmn.h>           
#endif

#ifndef NOMINMAX
#	define NOMINMAX
#	ifdef max
#		undef max
#	endif
#	ifdef min
#		undef min
#	endif
#endif

//#include <windows.h>

#define OGRE_DEBUG_MEMORY_MANAGER 1
#define OGRE_CONFIG_THREADS 3

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
#include <OgreAxisAlignedBox.h>
#include <OgreSubMesh.h>
#include <OgreRay.h>
#include <OgreMaterialManager.h>


//////////////////////////////////////////////////////////////////////////
namespace UnE
{
	namespace Core
	{   
		class UPOIManager;
		class UObjectManager;
		class UMovableTextOverlayManager;
		struct WndCtx
		{
			Ogre::RenderWindow	* renderWnd;
			Ogre::SceneManager	* sceneMgr;
			Ogre::SceneNode		* rootNode;
			Ogre::Ray			* ray;
			Ogre::RaySceneQuery * raySceneQuery;
			Ogre::Camera		* camera;
			Ogre::Viewport		* viewport;	
			Ogre::Quaternion	* orientaion;
			Ogre::AxisAlignedBox  aabb;
			HWND				  hWnd;
			bool				  bSubWindow;
			UPOIManager			* poiManger;
			UObjectManager		* objectManager;
			UMovableTextOverlayManager  * mvTextManager;
		};
	}
}


extern		Ogre::Root							*	m_Root;
extern		UnE::Core::WndCtx					*   g_WndCtx;
extern		UnE::Core::WndCtx					*   m_SubWndCtx;
extern		std::map<HWND, UnE::Core::WndCtx*>		m_subWnds;
extern		Ogre::RenderSystem					*	m_renderSystem;


extern		UnE::Core::WndCtx * GetWndContext(HWND hWnd);
extern      void				AddWndContext(UnE::Core::WndCtx* pCtx);
extern		void				ClearAllWndCtx();

//////////////////////////////////////////////////////////////////////////
// template function
template<class T>
static void TSwap( T& value1, T& value2 )
{
	T temp = value1;
	value1 = value2;
	value2 = temp;
}

template<class T>
static T TClamp(T value, T lower, T upper)
{
	if( upper < lower )
	{
		TSwap(upper, lower);
	}
	if( value > upper)
		return upper;
	if( value < lower)
		return lower;
	return value;
}

template<class T>
BOOL WriteProfileData(HKEY hSectionKey, LPCSTR entry, T value)
{ 
	if(NULL==hSectionKey)
		return FALSE; 

	// Write the double precision value to the 'entry' key 
	LONG lResult = ::RegSetValueExA(hSectionKey, entry, NULL, REG_BINARY, (LPBYTE)&value, sizeof(value));

	// Return success/failure     
	return (lResult == ERROR_SUCCESS) ? TRUE : FALSE; 
} 

template<class T>
T ReadProfileData(HKEY hSectionKey, LPCSTR entry, T defaultValue)
{
	// Get double value from 'entry' under section key 
	T regValue = defaultValue;  
	DWORD dwType = REG_BINARY; 
	DWORD dwCount = sizeof(regValue); 
	LONG lResult = ::RegQueryValueExA(hSectionKey,(LPCSTR)entry,NULL,&dwType,(LPBYTE)&regValue,&dwCount);

	// Return value read from registry 
	if(ERROR_SUCCESS==lResult) 
	{ 
		ASSERT(REG_BINARY==dwType); 
		ASSERT(sizeof(regValue)==dwCount); 
		return regValue; 
	} 
	// Or return default value 
	else 
	{ 
		return defaultValue; 
	} 
}
