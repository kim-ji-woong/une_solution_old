/******************************************************************************
File : CustomPlayer.cpp

Description: This file contains the CCustomPlayer class which
is the "interface" with the Virtools SDK. All other files are Windows/MFC
specific code.

Virtools SDK
Copyright (c) Virtools 2005, All Rights Reserved.
******************************************************************************/

#include "stdafx.h"
#include "CustomPlayer.h"
#include "CustomPlayerDefines.h"
#include "CK3DEntity.h"

#if defined(CUSTOM_PLAYER_STATIC)
	// include file used for the static configuration
	#include "CustomPlayerStaticLibs.h"
	#include "CustomPlayerRegisterDlls.h"
#endif

extern CCustomPlayer*	thePlayer;


////////////////////////////////////////////////////////////////////////////////
//
// CCustomPlayer: PUBLIC STATIC METHODS
//
////////////////////////////////////////////////////////////////////////////////

CCustomPlayer&
CCustomPlayer::Instance()
{
	if (thePlayer==0) {
		thePlayer = new CCustomPlayer();
	}
	return *thePlayer;
}

////////////////////////////////////////////////////////////////////////////////
//
// CCustomPlayer: PUBLIC METHODS
//
////////////////////////////////////////////////////////////////////////////////

CCustomPlayer::~CCustomPlayer()
{
	// here we stop/release/clear the Virtools Engine

	try {
		if (!m_CKContext) {
			return;
		}

		// clear the CK context
		m_CKContext->Reset();
		m_CKContext->ClearAll();

		// destroy the render engine
		if (m_RenderManager && m_RenderContext) {
			m_RenderManager->DestroyRenderContext(m_RenderContext);
		}
		m_RenderContext = NULL;

		// close the ck context
		//CKCloseContext(m_CKContext);

		m_CKContext = NULL;

		// shutdown all
		CKShutdown();
	} catch(...) {
	}
}

BOOL CCustomPlayer::ChangeResolution()
{
	if (!m_RenderContext)
		return FALSE;
	
	if (m_RenderContext->IsFullScreen()) {
		// we are in fullscreen mode
		if (_GetFullScreenResolution()) {
			// the resolution has changed

			// pause the player
			m_CKContext->Pause();
			// and stop fullscreen
			m_RenderContext->StopFullScreen();
			
			// return to fullscreen with the new resolution
			m_RenderContext->GoFullScreen(m_FullscreenWidth,m_FullscreenHeight,m_FullscreenBpp,m_Driver);
			
			VxDriverDesc* Check_API_Desc = m_RenderManager->GetRenderDriverDescription(m_Driver);
			//we have to resize the mainwin to allow correct picking (only in DX)
			if (Check_API_Desc->Caps2D.Family==CKRST_DIRECTX && m_RenderContext->IsFullScreen()) {
				//store current size
				GetWindowRect(m_MainWindow,&m_MainWindowRect);
				
				//Resize the window
				::SetWindowPos(m_MainWindow,HWND_TOPMOST,0,0,m_FullscreenWidth,m_FullscreenHeight,NULL);
				
				//Prevent the window from beeing resized
				LONG st = GetWindowLong(m_MainWindow,GWL_STYLE);
				st&=~WS_THICKFRAME;
				st&=~WS_SIZEBOX;
				SetWindowLong(m_MainWindow,GWL_STYLE,st);
			}	
			// everything is ok to restart the player
			m_CKContext->Play();
		}
	} else {
		// we are in windowed mode	
		if (_GetWindowedResolution()) {
			// the resolution has changed

			//allow the window to be resized
			LONG st = GetWindowLong(m_MainWindow,GWL_STYLE);
			st|=WS_THICKFRAME;
			st&=~WS_SIZEBOX;
			SetWindowLong(m_MainWindow,GWL_STYLE,st);
				
			//reposition the window
			m_MainWindowRect.left = (GetSystemMetrics(SM_CXSCREEN)-m_WindowedWidth)/2;
			m_MainWindowRect.right = m_WindowedWidth+m_MainWindowRect.left;
			m_MainWindowRect.top = (GetSystemMetrics(SM_CYSCREEN)-m_WindowedHeight)/2;
			m_MainWindowRect.bottom = m_WindowedHeight+m_MainWindowRect.top;
			BOOL ret = AdjustWindowRect(&m_MainWindowRect,WS_OVERLAPPEDWINDOW & ~(WS_SYSMENU|WS_SIZEBOX|WS_MAXIMIZEBOX|WS_MINIMIZEBOX|WS_SIZEBOX),FALSE);
			::SetWindowPos(m_MainWindow,0,m_MainWindowRect.left,m_MainWindowRect.top,m_MainWindowRect.right - m_MainWindowRect.left,m_MainWindowRect.bottom - m_MainWindowRect.top,NULL);
			
			// and set the position of the render window in the main window
			::SetWindowPos(m_RenderWindow,NULL,0,0,m_WindowedWidth,m_WindowedHeight,SWP_NOMOVE|SWP_NOZORDER);
			m_RenderContext->Resize(0,0,m_WindowedWidth,m_WindowedHeight);
		}
	}

	return TRUE;
}

BOOL CCustomPlayer::ClipMouse(BOOL iEnable)
{
	// manage the mouse clipping

	if(iEnable==FALSE) {
		// disable the clipping
		return ClipCursor(NULL);
	}
	
	if (!m_RenderContext) {
		return FALSE;
	}

	// retrieve the render window rectangle
	VxRect r;
	m_RenderContext->GetWindowRect(r,TRUE);

	RECT rect;
	rect.top = (LONG)r.top;
	rect.left = (LONG)r.left;
	rect.bottom = (LONG)r.bottom;
	rect.right = (LONG)r.right;
	
	// to clip the mouse in it.
	return ClipCursor(&rect);
}

BOOL CCustomPlayer::InitPlayer(HWND iMainWindow, HWND iRenderWindow, int iConfig, const void* iData,int iDataSize)
{
	// keep a reference on the main/render window
	m_MainWindow = iMainWindow;
	m_RenderWindow = iRenderWindow;

	// start the Virtools Engine
	CKStartUp();

	// retrieve the plugin manager ...
	CKPluginManager* pluginManager = CKGetPluginManager();

	//  ... to intialize plugins ...
	if (!_InitPlugins(*pluginManager)) {
		MessageBox(NULL, _T(UNABLE_TO_INIT_PLUGINS), _T(INIT_ERROR),MB_OK|MB_ICONERROR);
		return FALSE;
	}

	// ... and the render engine.
	int renderEngine = _InitRenderEngines(*pluginManager);
	if (renderEngine==-1) {
		MessageBox(NULL, _T(UNABLE_TO_LOAD_RENDERENGINE), _T(INIT_ERROR),MB_OK|MB_ICONERROR);
		return FALSE;
	}

	// now create the CK context
	CKERROR res = CKCreateContext(&m_CKContext,m_MainWindow,0);
	if (res!=CK_OK) {
		MessageBox(NULL, _T(UNABLE_TO_INIT_CK), _T(INIT_ERROR),MB_OK|MB_ICONERROR);
		return FALSE;
	}

	// retrieve the main managers which will be used by the player
	m_MessageManager	= m_CKContext->GetMessageManager();
	m_RenderManager		= m_CKContext->GetRenderManager();
	m_TimeManager		= m_CKContext->GetTimeManager();
	m_AttributeManager	= m_CKContext->GetAttributeManager();
	m_InputManager		= (CKInputManager*)m_CKContext->GetManagerByGuid(INPUT_MANAGER_GUID);
	if (!m_MessageManager || !m_RenderManager || !m_TimeManager || !m_AttributeManager || !m_InputManager) {
		MessageBox(NULL, _T(UNABLE_TO_INIT_MANAGERS), _T(INIT_ERROR),MB_OK|MB_ICONERROR);
		return FALSE;
	}

	// initialize the display driver using the player configuration (resolution, rasterizer, ...)
	if (!_InitDriver()) {		
		MessageBox(NULL, _T(UNABLE_TO_INIT_DRIVER), _T(INIT_ERROR),MB_OK|MB_ICONERROR);
		return FALSE;
	}

	// create the render context
	if (iConfig&eAutoFullscreen) {
		// in fullscreen we specify the rendering size using a rectangle (CKRECT)
		CKRECT rect;
		rect.left = 0;
		rect.top = 0;
		rect.right = m_FullscreenWidth;
		rect.bottom = m_FullscreenHeight;

		// create the render context
		m_RenderContext = m_RenderManager->CreateRenderContext(m_RenderWindow,m_Driver,&rect,TRUE,m_FullscreenBpp);

		// set the position of the render window
		::SetWindowPos(m_RenderWindow,NULL,0,0,m_FullscreenWidth,m_FullscreenHeight,SWP_NOMOVE|SWP_NOZORDER);

		// resize the render context
		if (m_RenderContext) {
			m_RenderContext->Resize(0,0,m_FullscreenWidth,m_FullscreenHeight);
		}
	} else {
		// set the position of the render window
		::SetWindowPos(m_RenderWindow,NULL,0,0,m_WindowedWidth,m_WindowedHeight,SWP_NOMOVE|SWP_NOZORDER);

		// create the render context
		m_RenderContext = m_RenderManager->CreateRenderContext(m_RenderWindow,m_Driver,0,FALSE);
		
		// resize the render context
		if (m_RenderContext) {
			m_RenderContext->Resize(0,0,m_WindowedWidth,m_WindowedHeight);
		}
	}

	if (!m_RenderContext) {
		MessageBox(NULL, _T(UNABLE_TO_CREATE_RENDERCONTEXT), _T(INIT_ERROR),MB_OK|MB_ICONERROR);
		return FALSE;
	}
	
	// now load the compostion
	if (iDataSize) { // if iDataSize is not null it means the composition is already in memory
		if (_Load(iData,iDataSize)!=CK_OK) {
			MessageBox(NULL, _T("Unable to load composition from memory."), _T("Initialisation Error"),MB_OK|MB_ICONERROR);
			return FALSE;
		}
	} else if (_Load((const char*)iData)!=CK_OK) { // else we load it from a file (iData contains the filename)
		MessageBox(NULL, _T("Unable to load composition from file."), _T("Initialisation Error"),MB_OK|MB_ICONERROR);
		return FALSE;
	}

	// cleare the render view
	m_RenderContext->Clear();
	m_RenderContext->BackToFront();
	m_RenderContext->Clear();

	// finialize the loading
	if (!_FinishLoad()) {
		return FALSE;
	}

	return TRUE;
}

void CCustomPlayer::OnActivateApp(BOOL iActivate)
{
	// if
	// - the application is being activated (iActivate == TRUE)
	// - the render context is in fullscreen
	// - and the player is not already switching fullscreen (m_EatDisplayChange == FALSE)
	if (iActivate==TRUE && m_RenderContext && m_RenderContext->IsFullScreen() && !m_EatDisplayChange) {
		// we switch fullscreen because the application is deactivated
		//SwitchFullscreen();
		_FullScreen();
	}
}

void CCustomPlayer::PauseInput(bool pause)
{
	m_InputManager->Pause(pause ? TRUE : FALSE);
}

void CCustomPlayer::OnFocusChange(BOOL iFocus)
{
	// here we manage the focus change

	if (!m_CKContext) {
		return;
	}

	///////////////////////
	// First, check minimize/restore
	///////////////////////

	if ( (m_State==ePlaying) && IsIconic(m_MainWindow) ) { // we must pause process
		// the application is minimized
		m_State = eMinimized;
		// so we pause the player
		m_CKContext->Pause();
		return;
	}
	
	if ( (m_State==eMinimized) && !IsIconic(m_MainWindow) ) { // we must restart process
		// the application is no longer minimized
		m_State = ePlaying;
		// so we restart the player
		m_CKContext->Play();
		return;
	}

	///////////////////////
	// then check focus lost behavior
	///////////////////////
	
	CKDWORD pauseMode = m_CKContext->GetFocusLostBehavior();

	if(m_State==ePlaying || m_State==eFocusLost) {
		switch (pauseMode)
		{
		// if the composition is configured to pause
		// the input manager when the focus is lost
		// note: for a stand alone player
		// CK_FOCUSLOST_PAUSEINPUT_MAIN and CK_FOCUSLOST_PAUSEINPUT_PLAYER
		// is the same thing. there is a difference only for the webplayer
		case CK_FOCUSLOST_PAUSEINPUT_MAIN:
		case CK_FOCUSLOST_PAUSEINPUT_PLAYER:
		//case CK_FOCUSLOST_PAUSEINPUT_WHENOUT:
			// we pause/unpause it depending on the focus
			m_InputManager->Pause(!iFocus);
			break;
			
		// if the composition is configured to pause
		// the player (pause all) when the focus is lost
		case CK_FOCUSLOST_PAUSEALL:
			if (!iFocus) {
				// focus lost
				m_State = eFocusLost;
				// pause the player
				m_CKContext->Pause();	
			} else {
				// else
				m_State = ePlaying;
				// play
				m_CKContext->Play();
			}
			break;
		}
	}
}

void CCustomPlayer::OnMouseClick(int iMessage)
{
	// here we manage the mouse click

	if (!m_RenderContext) {
		return;
	}

	// retrieve the cursor position
	POINT pt;
	GetCursorPos(&pt);
	ScreenToClient(m_RenderWindow,&pt);
	
	// convert the windows message to the virtools message
	int msg = (iMessage==WM_LBUTTONDOWN)?m_MsgClick:m_MsgDoubleClick;
			
	// retrieve information about object "under" the mouse
	VxIntersectionDesc desc;
	CKObject* obj = m_RenderContext->Pick(pt.x,pt.y,&desc);
	if(obj && CKIsChildClassOf(obj,CKCID_BEOBJECT)) {
		// send a message to the beobject
		m_MessageManager->SendMessageSingle(msg,(CKBeObject*)obj);
	}

	if (desc.Sprite) {
		// send a message to the sprite
		m_MessageManager->SendMessageSingle(msg,(CKBeObject *)desc.Sprite);
	}
}

void CCustomPlayer::InitLayerList()
{
	char* str[] = {"a1_off", "a1_on", "a2_off", "a2_on", "a3_on", "a3_off", "lakj", "Level", "kkk"};
	int nArrCount = sizeof(str) / sizeof(char*);

	for (int i=0;i<nArrCount;i++)
	{

	}
}

int CCustomPlayer::GetMessageList(std::vector<CString>& rVecMessage)
{
	int nMsgCount = m_MessageManager->GetMessageTypeCount();
	CKSTRING strMsgName;

	for (int i=0;i<nMsgCount;i++)
	{
		strMsgName = m_MessageManager->GetMessageTypeName(i);
		rVecMessage.push_back(strMsgName);
	}

	return (int)rVecMessage.size();
}

bool CCustomPlayer::SendMessage(char* strObjectName, char* strMessageName)
{
	CK3dObject* pObject = (CK3dObject*)m_CKContext->GetObjectByName(strObjectName);
	if (pObject == 0) return false;

	CString str = strObjectName;
	str += ", ";
	str += strMessageName;
	str += "\n";
	TRACE(str);

	CKMessageType nMsgType = m_MessageManager->AddMessageType(strMessageName);
	CKMessage* pMessage = m_MessageManager->SendMessageSingle(nMsgType, (CKBeObject *)pObject);

	if (pMessage)
	{
		CKParameter* param = pMessage->GetParameter(0);
		CKSTRING str = param->GetName();
		printf("s;klj");
	}

	return pMessage == 0 ? false : true;
}

//void CCustomPlayer::A2_OFF()
//{	
//	//const XObjectArray& rArray = m_RenderContext->Compute3dRootObjects();
//	//m_RenderContext->GetRo
//	//int nArraySize = rArray.Size();
//
//	//CKContext context;
//	//CKObject* pObject;
//
//	//for (int i=0;i<nArraySize;i++)
//	//{
//	//	CK_ID id = rArray.GetObjectID(i);
//	//	pObject = m_RenderContext->GetCKObject(id);
//	//	/*pObject = rArray.GetObject(&context, i);
//	//	if (pObject == 0) continue;*/
//
//	//	CKSTRING strObjectName = pObject->GetName();
//
//	//	if (!stricmp(strObjectName, "Model"))
//	//	{
//	//		CK3dEntity* pEntity = (CK3dEntity*)pObject;
//	//		int nChildCount = pEntity->GetChildrenCount();
//
//	//		for (int j=0;j<nChildCount;j++)
//	//		{
//	//			CK3dEntity* pChild = pEntity->GetChild(j);
//	//			strObjectName = pChild->GetName();
//
//	//			if (!stricmp(strObjectName, "a2"))
//	//			{
//	//				m_MessageManager->SendMessageSingle(1,(CKBeObject *)pChild);
//	//			}
//	//		}
//
//	//		return;
//	//	}
//	//}
//}

void CCustomPlayer::OnPaint()
{
	if (!m_RenderContext || m_RenderContext->IsFullScreen()) {
		return;
	}
	// in windowed mode call render when WM_PAINT
	m_RenderContext->Render();
}

LRESULT CCustomPlayer::OnSysKeyDownMainWindow(int iConfig, int iKey)
{
	// Manage system key (ALT + KEY)
	// system keys can be disable using eDisableKeys

	switch(iKey)
	{
	case VK_RETURN:
		if (!(iConfig&eDisableKeys)) {
			// ALT + ENTER -> SwitchFullscreen
			SwitchFullscreen();
		}
		break;

	case VK_F4:
		if (!(iConfig&eDisableKeys)) {
			// ALT + F4 -> Quit the application
			PostQuitMessage(0);
			return 1;
		}
		break;
	}
	return 0;
}

void CCustomPlayer::Pause()
{
	// mark the play as paused
	m_State = ePlaused;
	// pause
	m_CKContext->Pause();
}

void CCustomPlayer::Play()
{
	// mark the player as playing
	m_State = ePlaying;
	// play
	m_CKContext->Play();
}

BOOL
CCustomPlayer::Process(int iConfig)
{
	// just to be sure we check the player is ready and playing
	if (!m_CKContext || !m_CKContext->IsPlaying()) {
		Sleep(10);
		return TRUE;
	}

	float beforeRender	= 0;
	float beforeProcess	= 0;
	// retrieve the time to wait in milliseconds to perfom a rendering or a process loop
	// so that the time manager limits are respected.
	m_TimeManager->GetTimeToWaitForLimits(beforeRender,beforeProcess);

	if (beforeProcess<=0) {
		// ok to process
		m_TimeManager->ResetChronos(FALSE,TRUE);
		m_CKContext->Process();
	}

	if (beforeRender<=0) {
		// ok to render
		m_TimeManager->ResetChronos(TRUE,FALSE);
		m_RenderContext->Render();
	}

	// now we check if the composition has not change the state of a "control" attribute

	// quit has been set
	if (m_Level->HasAttribute(m_QuitAttType)) {
		// we remove it (so it is not "here" the next frame)
		m_Level->RemoveAttribute(m_QuitAttType);
		// and return FALSE to quit
		return FALSE;
	}

	// switch fullscreen has been set
	if (m_Level->HasAttribute(m_SwitchFullscreenAttType)) {
		// we remove it (so it is not "here" the next frame)
		m_Level->RemoveAttribute(m_SwitchFullscreenAttType);
		// and switch fullscreen (ie goes from fullscreen to windowed
		// or to windowed from fullscreen)
		SwitchFullscreen();
	}
		
	// switch resolution has been set
	if (m_Level->HasAttribute(m_SwitchResolutionAttType)) {
		// we remove it (so it is not "here" the next frame)
		m_Level->RemoveAttribute(m_SwitchResolutionAttType);
		// and we change the resolution
		ChangeResolution();
	}

	// switch resolution has been set
	if (m_Level->HasAttribute(m_SwitchMouseClippingAttType)) {
		// we remove it (so it is not "here" the next frame)
		m_Level->RemoveAttribute(m_SwitchMouseClippingAttType);
		m_MouseClipped = !m_MouseClipped;
		ClipMouse(m_MouseClipped);
	}

	return TRUE;
}

void
CCustomPlayer::Reset()
{
	// mark the player as playing
	m_State = ePlaying;
	// reset
	m_CKContext->Reset();
	// and play
	m_CKContext->Play();
}

BOOL
CCustomPlayer::SwitchFullscreen()
{
	if (!m_RenderContext || !m_FullscreenEnabled) {
		return FALSE;
	}

	// mark eat display change to true
	// so we cannot switch the display during this function.
	m_EatDisplayChange = TRUE;

	if (m_RenderContext->IsFullScreen()) {
		// siwtch to windowed mode
		_Windowed();

		/*// retrieve the windowed resolution from the attributes
		_GetWindowedResolution();
		// we pause the player
		m_CKContext->Pause();
		// stop the fullscreen
		m_RenderContext->StopFullScreen();
			
		//restore the main window size (only in DirectX rasterizer->m_MainWindowRect.bottom not modified)
		if (m_MainWindowRect.bottom!=0 && !m_RenderContext->IsFullScreen()) {
			//allow the window to be resized
			LONG st = GetWindowLong(m_MainWindow,GWL_STYLE);
			st|=WS_THICKFRAME;
			st&=~WS_SIZEBOX;
			SetWindowLong(m_MainWindow,GWL_STYLE,st);			
		}
				
		//reposition the window
		m_MainWindowRect.left = (GetSystemMetrics(SM_CXSCREEN)-m_WindowedWidth)/2;
		m_MainWindowRect.right = m_WindowedWidth+m_MainWindowRect.left;
		m_MainWindowRect.top = (GetSystemMetrics(SM_CYSCREEN)-m_WindowedHeight)/2;
		m_MainWindowRect.bottom = m_WindowedHeight+m_MainWindowRect.top;
		BOOL ret = AdjustWindowRect(&m_MainWindowRect,WS_OVERLAPPEDWINDOW & ~(WS_SYSMENU|WS_SIZEBOX|WS_MAXIMIZEBOX|WS_MINIMIZEBOX|WS_SIZEBOX),FALSE);
		::SetWindowPos(m_MainWindow,HWND_NOTOPMOST,m_MainWindowRect.left,m_MainWindowRect.top,m_MainWindowRect.right - m_MainWindowRect.left,m_MainWindowRect.bottom - m_MainWindowRect.top,NULL);

		// now we can show the main widnwos
		ShowWindow(m_MainWindow,SW_SHOW);
		SetFocus(m_MainWindow);

		// and set the position of the render window in the main window
		::SetWindowPos(m_RenderWindow,NULL,0,0,m_WindowedWidth,m_WindowedHeight,SWP_NOMOVE|SWP_NOZORDER);

		// and give the focus to the render window
		SetFocus(m_RenderWindow);

		// everything is ok to restart the player
		m_CKContext->Play();*/
	} else {
		// switch to fullscreen mode
		_FullScreen();

		/*// retrieve the fullscreen resolution from the attributes
		_GetFullScreenResolution();
		// we pause the player
		m_CKContext->Pause();
		// and go fullscreen
		m_RenderContext->GoFullScreen(m_FullscreenWidth,m_FullscreenHeight,m_FullscreenBpp,m_Driver);

		// everything is ok to restart the player
		m_CKContext->Play();
			
		VxDriverDesc* Check_API_Desc = m_RenderManager->GetRenderDriverDescription(m_Driver);
		//we have to resize the mainwin to allow correct picking (only in DX)
		if (Check_API_Desc->Caps2D.Family==CKRST_DIRECTX && m_RenderContext->IsFullScreen()) {
			//store current size
			GetWindowRect(m_MainWindow,&m_MainWindowRect);
				
			//Resize the window
			::SetWindowPos(m_MainWindow,HWND_TOPMOST,0,0,m_FullscreenWidth,m_FullscreenHeight,NULL);
				
			//Prevent the window from beeing resized
			LONG st = GetWindowLong(m_MainWindow,GWL_STYLE);
			st&=~WS_THICKFRAME;
			st&=~WS_SIZEBOX;
			SetWindowLong(m_MainWindow,GWL_STYLE,st);
		}*/
	} 

	// mark eat display change to false
	// as we have finished
	m_EatDisplayChange = FALSE;

	ClipMouse(m_MouseClipped);

	return TRUE;
}

//////////////////////////////////////////////////////////////////////
//
// PRIVATE
//
//////////////////////////////////////////////////////////////////////


CCustomPlayer::CCustomPlayer()
	: m_State(eInitial), m_MainWindow(0),m_RenderWindow(0),
	m_CKContext(0),m_RenderContext(0),
	m_MessageManager(0),m_RenderManager(0),m_TimeManager(0),
	m_AttributeManager(0),m_InputManager(0),
	m_Level(0),m_QuitAttType(-1),m_SwitchResolutionAttType(-1),m_SwitchMouseClippingAttType(-1),
	m_WindowedResolutionAttType(-1),m_FullscreenResolutionAttType(-1),m_FullscreenBppAttType(-1),
	m_MsgClick(0),m_MsgDoubleClick(0),
	m_RasterizerFamily(CKRST_DIRECTX),m_RasterizerFlags(CKRST_SPECIFICCAPS_HARDWARE|CKRST_SPECIFICCAPS_DX9),
	m_WindowedWidth(640),m_WindowedHeight(480),
	m_MinWindowedWidth(400),m_MinWindowedHeight(300),
	m_FullscreenWidth(640),m_FullscreenHeight(480),m_FullscreenBpp(32),
	m_Driver(-1),m_FullscreenEnabled(FALSE),
	m_EatDisplayChange(FALSE),m_MouseClipped(FALSE)
{
}



BOOL CCustomPlayer::_CheckDriver(VxDriverDesc* iDesc, int iFlags)
{
	// check the rasterizer family
	if (iFlags & eFamily) { 
		if (iDesc->Caps2D.Family!=m_RasterizerFamily) {
			return FALSE;
		}
	}

	// test directx version
	if ( (iDesc->Caps2D.Family==CKRST_DIRECTX) && (iFlags & eDirectXVersion) ) {
		if ((iDesc->Caps3D.CKRasterizerSpecificCaps&0x00000f00UL)!=(m_RasterizerFlags&0x00000f00UL)) {
			return FALSE;
		}
	}

	// test hardware/software
	if (iFlags & eSoftware) {
		if (((int)iDesc->Caps3D.CKRasterizerSpecificCaps&CKRST_SPECIFICCAPS_SOFTWARE)!=(m_RasterizerFlags&CKRST_SPECIFICCAPS_SOFTWARE)) {
			return FALSE;
		}	
	}

	return TRUE;
}

BOOL CCustomPlayer::_CheckFullscreenDisplayMode(BOOL iDoBest)
{
	VxDriverDesc* desc = m_RenderManager->GetRenderDriverDescription(m_Driver);

	// try to find the correct fullscreen display mode
	for (VxDisplayMode* displayMode=desc->DisplayModes.Begin();displayMode!=desc->DisplayModes.End();displayMode++) {
		if (displayMode->Width==m_FullscreenWidth	&&
			displayMode->Height==m_FullscreenHeight	&&
			displayMode->Bpp==m_FullscreenBpp) {
			m_FullscreenEnabled = TRUE;
			return TRUE;
		}
	}

	if (!iDoBest) {
		return FALSE;
	}

	// we did not find a display mode
	// try 640x480x32
	m_FullscreenWidth = 640;
	m_FullscreenHeight = 480;
	m_FullscreenBpp = 32;

	for (VxDisplayMode* displayMode=desc->DisplayModes.Begin();displayMode!=desc->DisplayModes.End();displayMode++) {
		if (displayMode->Width==m_FullscreenWidth	&&
			displayMode->Height==m_FullscreenHeight	&&
			displayMode->Bpp==m_FullscreenBpp) {
			m_FullscreenEnabled = TRUE;
			return TRUE;
		}
	}
	
	// we did not find a display mode
	// try 640x480x16
	m_FullscreenBpp = 16;

	for (VxDisplayMode* displayMode=desc->DisplayModes.Begin();displayMode!=desc->DisplayModes.End();displayMode++) {
		if (displayMode->Width==m_FullscreenWidth	&&
			displayMode->Height==m_FullscreenHeight	&&
			displayMode->Bpp==m_FullscreenBpp) {
			m_FullscreenEnabled = TRUE;
			return TRUE;
		}
	}

	m_FullscreenEnabled = FALSE;
	return FALSE;
}

BOOL CCustomPlayer::_FinishLoad()
{
	// retrieve the level
	m_Level = m_CKContext->GetCurrentLevel();
	if (!m_Level) {
		MessageBox(NULL, _T(CANNOT_FIND_LEVEL), _T(INIT_ERROR),MB_OK|MB_ICONERROR);
		return FALSE;
	}

	// Add the render context to the level
	m_Level->AddRenderContext(m_RenderContext,TRUE);

	// Take the first camera we found and attach the viewpoint to it.
	// (in case it is not set by the composition later)
	const XObjectPointerArray cams = m_CKContext->GetObjectListByType(CKCID_CAMERA,TRUE);
	if(cams.Size()) {
		m_RenderContext->AttachViewpointToCamera((CKCamera*)cams[0]);
	}

	// Hide curves ?
	int curveCount = m_CKContext->GetObjectsCountByClassID(CKCID_CURVE);
	CK_ID* curve_ids = m_CKContext->GetObjectsListByClassID(CKCID_CURVE);
	for (int i=0;i<curveCount;++i) {
		CKMesh* mesh = ((CKCurve*)m_CKContext->GetObject(curve_ids[i]))->GetCurrentMesh();
		if(mesh)
			mesh->Show(CKHIDE);
	}

	// retrieve custom player attributes
	// from an exemple about using this attributes see sample.cmo which is delivered with this player sample
	// simply set the "Quit" attribute to quit the application
	m_QuitAttType = m_AttributeManager->GetAttributeTypeByName("Quit");
	// simply set the "Switch Fullscreen" attribute to make the player switch between fullscreen and windowed mode
	m_SwitchFullscreenAttType = m_AttributeManager->GetAttributeTypeByName("Switch Fullscreen");
	// simply set the "Switch Resolution" attribute to make the player change its resolution according
	// to the "Windowed Resolution" or "Fullscreen Resolution" (depending on the current mode)
	m_SwitchResolutionAttType = m_AttributeManager->GetAttributeTypeByName("Switch Resolution");
	// simply set the "Switch Mouse Clipping" attribute to make the player clip/unclip the mouse to the render window
	m_SwitchMouseClippingAttType = m_AttributeManager->GetAttributeTypeByName("Switch Mouse Clipping");
	// the windowed resolution
	m_WindowedResolutionAttType = m_AttributeManager->GetAttributeTypeByName("Windowed Resolution");
	// the fullscreen resolution
	m_FullscreenResolutionAttType = m_AttributeManager->GetAttributeTypeByName("Fullscreen Resolution");
	// the fullscreen bpp
	m_FullscreenBppAttType = m_AttributeManager->GetAttributeTypeByName("Fullscreen Bpp");

	// remove attributes (quit,switch fullscreen and switch resolution) if present
	if (m_Level->HasAttribute(m_QuitAttType)) {
		m_Level->RemoveAttribute(m_QuitAttType);
	}
	if (m_Level->HasAttribute(m_SwitchFullscreenAttType)) {
		m_Level->RemoveAttribute(m_SwitchFullscreenAttType);
	}
	if (m_Level->HasAttribute(m_SwitchResolutionAttType)) {
		m_Level->RemoveAttribute(m_SwitchResolutionAttType);
	}
	if (m_Level->HasAttribute(m_SwitchMouseClippingAttType)) {
		m_Level->RemoveAttribute(m_SwitchMouseClippingAttType);
	}

	// set the attributes so it match the current player configuration
	_SetResolutions();

	// set a fake last cmo loaded
	// we build a filename using the exe full filename
	// remplacing exe by vmo.
	{
		TCHAR path[MAX_PATH];
		if (::GetModuleFileName(0,path,MAX_PATH)) {
			TCHAR drive[MAX_PATH];
			TCHAR dir[MAX_PATH];
			TCHAR filename[MAX_PATH];
			TCHAR ext[MAX_PATH];
			_tsplitpath(path,drive,dir,filename,ext);
			_tmakepath(path,drive,dir,filename,_T("vmo"));
			m_CKContext->SetLastCmoLoaded(path);

		}
	}

	// we launch the default scene
	m_Level->LaunchScene(NULL);

	// ReRegister OnClick Message in case it changed
	m_MsgClick = m_MessageManager->AddMessageType("OnClick");
	m_MsgDoubleClick = m_MessageManager->AddMessageType("OnDblClick");

	// render the first frame
	m_RenderContext->Render();

	return TRUE;
}

BOOL CCustomPlayer::_GetFullScreenResolution()
{
	// retrieve the fullscreen resolution from attribute

	// retrieve the attribute (resolution width and height)
	CKParameterOut* paramRes = m_Level->GetAttributeParameter(m_FullscreenResolutionAttType);
	if (!paramRes) {
		return FALSE;
	}

	// save old values
	int oldWidth = m_FullscreenWidth;
	int oldHeight = m_FullscreenHeight;
	int oldBpp = m_FullscreenBpp;

	// retrieve the attribute (bpp)
	CKParameterOut* paramBpp = m_Level->GetAttributeParameter(m_FullscreenBppAttType);
	if (paramBpp) {
		paramBpp->GetValue(&m_FullscreenBpp);
	}

	Vx2DVector res(m_FullscreenWidth,m_FullscreenHeight);
	paramRes->GetValue(&res);
	m_FullscreenWidth = (int)res.x;
	m_FullscreenHeight = (int)res.y;

	// check the resolution is compatible with the fullscreen mode
	if (!_CheckFullscreenDisplayMode(FALSE)) {
		// else ...
		m_FullscreenWidth = oldWidth;
		m_FullscreenHeight = oldHeight;
		m_FullscreenBpp = oldBpp;
		// ... reset attributes with old values
		_SetResolutions();
	}

	// returns TRUE if at least one value has changed
	return (m_FullscreenWidth!=oldWidth || m_FullscreenHeight!=oldHeight || m_FullscreenBpp!=oldBpp);
}

BOOL CCustomPlayer::_GetWindowedResolution()
{	
	// retrieve the windowed resolution from attribute

	// retrieve the attribute (resolution width and height)
	CKParameterOut* pout = m_Level->GetAttributeParameter(m_WindowedResolutionAttType);
	if (!pout) {
		return FALSE;
	}

	// save old values
	int oldWidth = m_WindowedWidth;
	int oldHeight = m_WindowedHeight;

	Vx2DVector res(m_WindowedWidth,m_WindowedHeight);
	pout->GetValue(&res);
	m_WindowedWidth = (int)res.x;
	m_WindowedHeight = (int)res.y;

	// returns TRUE if at least one value has changed
	return (m_WindowedWidth!=oldWidth || m_WindowedHeight!=oldHeight);
}

BOOL CCustomPlayer::_Windowed()
{
	// retrieve the windowed resolution from the attributes
	_GetWindowedResolution();
	// we pause the player
	m_CKContext->Pause();
	// stop the fullscreen
	m_RenderContext->StopFullScreen();

	m_RenderContext->Resize(0,0,m_WindowedWidth,m_WindowedHeight);

	//restore the main window size (only in DirectX rasterizer->m_MainWindowRect.bottom not modified)
	if (m_MainWindowRect.bottom!=0 && !m_RenderContext->IsFullScreen()) {
		//allow the window to be resized
		LONG st = GetWindowLong(m_MainWindow,GWL_STYLE);
		st|=WS_THICKFRAME;
		st&=~WS_SIZEBOX;
		SetWindowLong(m_MainWindow,GWL_STYLE,st);			
	}

	//reposition the window
	m_MainWindowRect.left = (GetSystemMetrics(SM_CXSCREEN)-m_WindowedWidth)/2;
	m_MainWindowRect.right = m_WindowedWidth+m_MainWindowRect.left;
	m_MainWindowRect.top = (GetSystemMetrics(SM_CYSCREEN)-m_WindowedHeight)/2;
	m_MainWindowRect.bottom = m_WindowedHeight+m_MainWindowRect.top;
	BOOL ret = AdjustWindowRect(&m_MainWindowRect,WS_OVERLAPPEDWINDOW & ~(WS_SYSMENU|WS_SIZEBOX|WS_MAXIMIZEBOX|WS_MINIMIZEBOX|WS_SIZEBOX),FALSE);
	::SetWindowPos(m_MainWindow,HWND_NOTOPMOST,m_MainWindowRect.left,m_MainWindowRect.top,m_MainWindowRect.right - m_MainWindowRect.left,m_MainWindowRect.bottom - m_MainWindowRect.top,NULL);

	// now we can show the main widnwos
	ShowWindow(m_MainWindow,SW_SHOW);
	SetFocus(m_MainWindow);

	// and set the position of the render window in the main window
	::SetWindowPos(m_RenderWindow,NULL,0,0,m_WindowedWidth,m_WindowedHeight,SWP_NOMOVE|SWP_NOZORDER);

	// and give the focus to the render window
	SetFocus(m_RenderWindow);

	// everything is ok to restart the player
	m_CKContext->Play();

	return TRUE;
}

BOOL CCustomPlayer::_FullScreen()
{
	// retrieve the fullscreen resolution from the attributes
	_GetFullScreenResolution();
	// we pause the player
	m_CKContext->Pause();
	// and go fullscreen

	ShowWindow(m_MainWindow,SW_HIDE);
	//ShowWindow(m_RenderWindow,SW_SHOW);

	m_RenderContext->StopFullScreen();
	m_RenderContext->GoFullScreen(m_FullscreenWidth,m_FullscreenHeight,m_FullscreenBpp,m_Driver);

	// everything is ok to restart the player
	m_CKContext->Play();

	VxDriverDesc* Check_API_Desc = m_RenderManager->GetRenderDriverDescription(m_Driver);
	//we have to resize the mainwin to allow correct picking (only in DX)
	if (Check_API_Desc->Caps2D.Family==CKRST_DIRECTX && m_RenderContext->IsFullScreen()) {
		//store current size
		GetWindowRect(m_MainWindow,&m_MainWindowRect);

		//Resize the window
		::SetWindowPos(m_MainWindow,HWND_TOPMOST,0,0,m_FullscreenWidth,m_FullscreenHeight,NULL);

		//Prevent the window from beeing resized
		LONG st = GetWindowLong(m_MainWindow,GWL_STYLE);
		st&=~WS_THICKFRAME;
		st&=~WS_SIZEBOX;
		SetWindowLong(m_MainWindow,GWL_STYLE,st);
	}
	return TRUE;
}

BOOL CCustomPlayer::_InitDriver()
{
	int count = m_RenderManager->GetRenderDriverCount();
	int i = 0;
	
	// first, we try to get exactly what is required
	int checkFlags = eFamily | eDirectXVersion | eSoftware;
	for (i=0;i<count;++i) {
		VxDriverDesc* desc = m_RenderManager->GetRenderDriverDescription(i);
		if (!desc) {
			continue;
		}

		if (_CheckDriver(desc,checkFlags)) {
			m_Driver = i;
			_CheckFullscreenDisplayMode(TRUE);
			return TRUE;
		}
	}

	if (m_RasterizerFamily == CKRST_OPENGL) {
		return FALSE;
	}

	// if we did not find a driver,
	// we only check family and software flags
	checkFlags &= ~eDirectXVersion;
	for (i=0;i<count;++i) {
		VxDriverDesc* desc = m_RenderManager->GetRenderDriverDescription(i);
		if (!desc) {
			continue;
		}

		if (_CheckDriver(desc,checkFlags)) {
			m_Driver = i;
			_CheckFullscreenDisplayMode(TRUE);
			return TRUE;
		}
	}

	return FALSE;
}

BOOL CCustomPlayer::_InitPlugins(CKPluginManager& iPluginManager)
{
#if !defined(CUSTOM_PLAYER_STATIC)
	// if the player is not static

	char szPath[_MAX_PATH];
	char PluginPath[_MAX_PATH];
	char RenderPath[_MAX_PATH];
	char BehaviorPath[_MAX_PATH];
	char ManagerPath[_MAX_PATH];

	VxGetModuleFileName(NULL,szPath,_MAX_PATH);
	CKPathSplitter ps(szPath);
	sprintf(PluginPath,"%s%s%s",ps.GetDrive(),ps.GetDir(),"Plugins");
	sprintf(RenderPath,"%s%s%s",ps.GetDrive(),ps.GetDir(),"RenderEngines");
	sprintf(ManagerPath,"%s%s%s",ps.GetDrive(),ps.GetDir(),"Managers");
	sprintf(BehaviorPath,"%s%s%s",ps.GetDrive(),ps.GetDir(),"BuildingBlocks");

	// we initialize plugins by parsing directories
	iPluginManager.ParsePlugins(RenderPath);
	iPluginManager.ParsePlugins(ManagerPath);
	iPluginManager.ParsePlugins(BehaviorPath);
	iPluginManager.ParsePlugins(PluginPath);
#else
	// else if the player is static
	// we initialize plugins by manually register them
	// for an exemple look at CustomPlayerRegisterDlls.h
	CustomPlayerRegisterRenderEngine(iPluginManager);
	CustomPlayerRegisterReaders(iPluginManager);
	CustomPlayerRegisterManagers(iPluginManager);
	CustomPlayerRegisterBehaviors(iPluginManager);
#endif

	return TRUE;
}

int CCustomPlayer::_InitRenderEngines(CKPluginManager& iPluginManager)
{
	// here we look for the render engine (ck2_3d)
	int count = iPluginManager.GetPluginCount(CKPLUGIN_RENDERENGINE_DLL);
	for (int i=0;i<count;i++) {
		CKPluginEntry* desc = iPluginManager.GetPluginInfo(CKPLUGIN_RENDERENGINE_DLL,i); 
		CKPluginDll*   dll  = iPluginManager.GetPluginDllInfo(desc->m_PluginDllIndex); 
#if !defined(CUSTOM_PLAYER_STATIC)
		XDWORD pos = dll->m_DllFileName.RFind(DIRECTORY_SEP_CHAR);
		if (pos==XString::NOTFOUND)
			continue;
		XString str = dll->m_DllFileName.Substring(pos+1);
		if (strnicmp(str.CStr(),"ck2_3d",strlen("ck2_3d"))==0)
			return i;
#else
		if (dll->m_DllFileName.ICompare("ck2_3d")==0)
			return i;
#endif
	}

	return -1;
}

CKERROR CCustomPlayer::_Load(const char* str) 
{
	// the composition from a file

	// validate the filename
	if(!str || !(*str) || strlen(str)<=0) {
		return FALSE;
	}

	// here we decompose the loading from a file to manage the missing guids case

	// create a ckfile
	CKFile *f = m_CKContext->CreateCKFile();	
	XString resolvedfile = str;	
	// resolve the filename using the pathmanager
	{
		CKPathManager *pm = m_CKContext->GetPathManager();	
		if(pm) {
			pm->ResolveFileName(resolvedfile,0);
		}
	}
	
	// open the file
	DWORD res = CKERR_INVALIDFILE;
	res = f->OpenFile(resolvedfile.Str(),(CK_LOAD_FLAGS) (CK_LOAD_DEFAULT | CK_LOAD_CHECKDEPENDENCIES));
	if (res!=CK_OK) {
		// something failed
		if (res==CKERR_PLUGINSMISSING) {
			// log the missing guids
			_MissingGuids(f,resolvedfile.CStr());
		}
		//m_CKContext->DeleteCKFile(f);
		//return res;
	}

	CKPathSplitter ps(resolvedfile.Str());
	CKPathMaker mp(ps.GetDrive(),ps.GetDir(),NULL,NULL);
	if(strcmp(mp.GetFileName(),CKGetStartPath())){
		CKPathManager *pm = m_CKContext->GetPathManager();
		XString XStr = XString(mp.GetFileName());
		pm->AddPath(BITMAP_PATH_IDX,XStr);
		XStr = XString(mp.GetFileName());
		pm->AddPath(DATA_PATH_IDX,XStr);
		XStr = XString(mp.GetFileName());
		pm->AddPath(SOUND_PATH_IDX,XStr);			
	}

	CKObjectArray *array = CreateCKObjectArray();	
	res = f->LoadFileData(array);	
	if(res != CK_OK) {
		m_CKContext->DeleteCKFile(f);
		DeleteCKObjectArray(array);
		return res;
	}

	m_CKContext->DeleteCKFile(f);

	DeleteCKObjectArray(array);	
	return CK_OK;
}

CKERROR CCustomPlayer::_Load(const void* iMemoryBuffer,int iBufferSize)
{
	CKFile *f = m_CKContext->CreateCKFile();	
	
	DWORD res = CKERR_INVALIDFILE;
	res = f->OpenMemory((void*)iMemoryBuffer,iBufferSize,(CK_LOAD_FLAGS) (CK_LOAD_DEFAULT | CK_LOAD_CHECKDEPENDENCIES));
	if (res!=CK_OK) {
		if (res==CKERR_PLUGINSMISSING) {
			_MissingGuids(f,0);
		}
		//m_CKContext->DeleteCKFile(f);
		//return res;
	}

	CKObjectArray *array = CreateCKObjectArray();	
	res = f->LoadFileData(array);	
	if(res != CK_OK) {
		m_CKContext->DeleteCKFile(f);
		return res;
	}

	m_CKContext->DeleteCKFile(f);

	DeleteCKObjectArray(array);	
	return CK_OK;
}

void CCustomPlayer::_MissingGuids(CKFile* iFile, const char* iResolvedFile)
{
	// here we manage the error CKERR_PLUGINSMISSING when loading a composition failed

	// create missing guids log filename
	TCHAR fp[_MAX_PATH];
	{
		GetTempPath(_MAX_PATH,fp);
		char drive[_MAX_DRIVE];
		char dir[_MAX_DIR];
		char fname[_MAX_FNAME];
		char ext[_MAX_EXT];
				
		_splitpath(fp,drive,dir,fname,ext);
		_makepath(fp,drive,dir,MISSINGUIDS_LOG,NULL);
	}

	// retrieve the list of missing plugins/guids
	XClassArray<CKFilePluginDependencies> *p = iFile->GetMissingPlugins();
	CKFilePluginDependencies*it = p->Begin();	

	FILE *logf = NULL;	
	char str[64];
	for(CKFilePluginDependencies* it=p->Begin();it!=p->End();it++) {
		int count = (*it).m_Guids.Size();
		for(int i=0;i<count;i++) {
			if (!((*it).ValidGuids[i])) {
				if(!logf) {
					logf = fopen(fp,_T("wt"));
					if (!logf) {
						return;
					}
					if (iResolvedFile) {
						fprintf(logf,"File Name : %s\nMissing GUIDS:\n",iResolvedFile);
					}
				}

				sprintf(str,"%x,%x\n",(*it).m_Guids[i].d1,(*it).m_Guids[i].d2);
				fprintf(logf,"%s",str);
			}
		}
	}
			
	fclose(logf);
}

void CCustomPlayer::_SetResolutions()
{
	// retrieve the windowed attribute
	CKParameterOut* pout = m_Level->GetAttributeParameter(m_WindowedResolutionAttType);
	if (pout) {
		Vx2DVector res(m_WindowedWidth,m_WindowedHeight);
		// set it
		pout->SetValue(&res);
	}
	
	// retrieve the fullscreen attribute
	pout = m_Level->GetAttributeParameter(m_FullscreenResolutionAttType);
	if (pout) {
		Vx2DVector res(m_FullscreenWidth,m_FullscreenHeight);
		// set it
		pout->SetValue(&res);
	}

	// retrieve the fullscreen bpp attribute
	pout = m_Level->GetAttributeParameter(m_FullscreenBppAttType);
	if (pout) {
		// set it
		pout->SetValue(&m_FullscreenBpp);
	}
}

CKERROR TestCallBack(const CKBehaviorContext& behcontext)
{
	CKBehavior* beh = behcontext.Behavior;
	if (beh == 0) return CKBR_OK;

	CString str;
	str.Format("Msg : %d", behcontext.CallbackMessage);
	TRACE(str);

	/*switch( behcontext.CallbackMessage )
	{
		case CKM_BEHAVIORLOAD:
		{
    		if( beh->GetVersion()==0x00020000 ){
				beh->SetFunction(GetDeltaTime);
      		} else {	
				beh->SetFunction(GetDeltaTime_old);
      		}
		}
	}*/

	return CKBR_OK; 
}

int Test(const CKBehaviorContext& rContext)
{
	return CKBR_OK;
}

CKERROR  CKContextCallBack(CKUICallbackStruct& param, void *data)
{
	TRACE(param.ConsoleString);
	return CKBR_OK;
}

void CCustomPlayer::TestCallback()
{
	CKBOOL is = m_CKContext->IsInInterfaceMode();
	this->m_CKContext->SetInterfaceMode(1, CKContextCallBack);
	is = m_CKContext->IsInInterfaceMode();
	return;

	CKBehaviorPrototype* proto = ::CreateCKBehaviorPrototype("TestCallBack");
	if (proto == 0) return;

	proto->DeclareOutput("Out0");
	proto->DeclareOutput("Error");

	proto->DeclareOutParameter("OutP0", CKPGUID_STRING);

	proto->SetBehaviorCallbackFct(TestCallBack, CKCB_BEHAVIORATTACH|CKCB_BEHAVIORDETACH|CKCB_BEHAVIORDELETE|CKCB_BEHAVIOREDITED|CKCB_BEHAVIORSETTINGSEDITED|CKCB_BEHAVIORLOAD|CKCB_BEHAVIORPRESAVE|CKCB_BEHAVIORPOSTSAVE|CKCB_BEHAVIORRESUME|CKCB_BEHAVIORPAUSE|CKCB_BEHAVIORRESET|CKCB_BEHAVIORRESET|CKCB_BEHAVIORDEACTIVATESCRIPT|CKCB_BEHAVIORACTIVATESCRIPT|CKCB_BEHAVIORREADSTATE, NULL);
proto->SetFunction(Test);

	/*pProtoType->SetBehaviorCallbackFct(TestCallBack, CKCB_BEHAVIORALL);
	CKBEHAVIORCALLBACKFCT func = pProtoType->GetBehaviorCallbackFct();
	return;*/
}
