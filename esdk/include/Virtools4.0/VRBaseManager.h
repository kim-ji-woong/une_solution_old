/*************************************************************************/
/*	File : VRBaseManager.h												 */
/*	Author :  Leïla AIT KACI											 */	
/*																		 */	
/*	Base Class for Virtools VRPack Managers								 */	
/*	Virtools SDK 														 */	 
/*	Copyright (c) Virtools 2004, All Rights Reserved.					 */	
/*************************************************************************/
#ifndef VRBaseManager_H

#define VRBaseManager_H "$Id:$"

#include "VRTypes.h"

class VRContext;

/*************************************************
Summary: Mask for VR Base Manager overridable functions.
Remarks:
	+ When implementing a VR manager the VRBaseManager::GetValidVRFunctionsMask 
	must be overriden to return a combination of these flag to indicate
	which VR methods are implemented.

See also: VRBaseManager::GetValidVRFunctionsMask
*************************************************/
typedef enum VRMANAGER_FUNCTIONS {
	VRMANAGER_FUNC_OnParse			= 0x00000001,	// Called after common parsing and before view creation
	VRMANAGER_FUNC_OnViewCreate		= 0x00000002,	// Called when a view is created by the VRPlayer
	VRMANAGER_FUNC_OnViewDelete		= 0x00000004,	// Called when a view is deleted by the VRPlayer
	VRMANAGER_FUNC_OnViewWarp		= 0x00000008,	// Called when a warped view should be rendered
} VRMANAGER_FUNCTIONS;



/*************************************************************************
Summary: Base Class for VR managers.
Remarks: 
	+ This class inherits from CKBaseManager.
	+ It provides additionnal virtual methods that can be overriden to add some
	processing at VR specific events.
	+ The instances of managers may be retrieved through the global function 
	CKContext::GetManagerByGuid()


See also: CKBaseManager
*************************************************************************/
class VRBaseManager : public CKBaseManager
{
public :	
	VRBaseManager(CKContext *iCtx, CKGUID guid,CKSTRING Name);	
	~VRBaseManager();

public :
	/*************************************************
	Summary: Called when the player starts and reads the 
	configuration files.In this method, you usually parse
	your own tokens with the parsing methods of the VRContext.
	Return Value:
		+ CK_OK if successful
		+ An error code otherwise
	Remarks:
		+ You can override this function to parse your tokens. Note that the
		following tokens are already available in the VRContext: 'LogLevel',
		'BaseDirectory', 'Hosts', 'VRResources', 'Resources', 'BitmapPath',
		'DataPath' and 'SoundPath'. You can then use the CKPathManager to
		locate data, texture or sound path if needed.
		+ You must override GetValidVRFunctionsMask and return a value including 
		VRMANAGER_FUNC_OnParse for this function to get called.
	
	See Also: VRContext::GetToken, VRContext::GetHiddenToken, CKPathManager, VRBaseManager::GetValidVRFunctionsMask
	*************************************************/	
	virtual CKERROR OnParse()	{ return CK_OK; }

	/*************************************************
	Summary: Called each time the VRPlayer creates a new view.
	The information on this view is provided  as an argument by
	the parsing of the View_X_X token (See the VR Pack’s 
	Documentation’s Viewport Setting topic).
	With this method, you usually read the additional view subtoken
	with the subtoken parsing methods of the VRContext and make any
	initialization needed by your plugin.
	Return Value:
		+ CK_OK if successful
		+ An error code otherwise
	Remarks:
		+ You can override this function to add specific processing at view creation.
		In particular, you can activate the warping on the view. You can also attach
		a user data to the view.
		+ You must override GetValidVRFunctionsMask and return a value including 
		VRMANAGER_FUNC_OnViewCreate for this function to get called.
	
	See Also: VRBaseManager::GetValidVRFunctionsMask, VRContext::ActivateWarp,
	VRContext::SetViewData, VRContext::GetSubToken	
	*************************************************/	
	virtual CKERROR OnViewCreate(VRView* iView)	{ return CK_OK; }

	/*************************************************
	Summary: Called each time the VRPlayer deletes a view.
	Here, you should delete any data you attached to a view.
	Return Value:
		+ CK_OK if successful
		+ An error code otherwise
	Remarks:
		+ You can override this function to add specific processing when a view is
		deleted. In particular, you should clear your user data if you added one
		to the current view.
		+ You must override GetValidVRFunctionsMask and return a value including 
		VRMANAGER_FUNC_OnViewDelete for this function to get called.
	
	See Also: VRContext::DeActivateWarp, VRContext::GetViewData, VRBaseManager::GetValidVRFunctionsMask
	*************************************************/	
	virtual CKERROR OnViewDelete(VRView* iView)	{ return CK_OK; }

	/*************************************************
	Summary: Called after each Virtools rendering for every warped view.
	Here, you should make your own custom rendering using either the 
	warping texture or the information on the render context back buffer.
	Return Value:
		+ CK_OK if successful
		+ An error code otherwise
	Remarks:
		+ You can override this function to implementing the view warping. Depending
		on the warping options, the Virtools rendering were done on the backbuffer or
		on a render texture. 
		+ You must override GetValidVRFunctionsMask and return a value including 
		VRMANAGER_FUNC_OnViewWarp for this function to get called.
	
	See Also: VRContext::GetViewData, VRContext::GetWarpTexture, VRBaseManager::GetValidVRFunctionsMask
	*************************************************/	
	virtual CKERROR OnViewWarp(CKRenderContext* rc,	VRView* iView)	{ return CK_OK; }

	/*************************************************
	Summary: Returns the priority of the specified VR manager function.
	Input Arguments:
		+ Function: A VRMANAGER_FUNCTIONS to get the priority of.
	Return Value: An integer giving priority for the specified function.
	Remarks:
		+ Override this function if you want to specify a priority for one or several functions of your manager.
		+ The default implementation returns a priority of 0 for all functions.
	Example:
	// To ensure that your OnParse() function will be one of the first to be called among all managers:
	int MyManager::GetVRFunctionPriority(VRMANAGER_FUNCTIONS Function)
	{
		if (Function==VRMANAGER_FUNC_OnParse) return 10000;	// High Priority
			return 0;
	}
	
	See Also: VRMANAGER_FUNCTIONS
	*************************************************/	
	virtual int	GetVRFunctionPriority(VRMANAGER_FUNCTIONS Function)	{ return 0; }	
	
	/*************************************************
	Summary: Returns list of VR functions implemented by the manager.
	Return Value: A combination of VRMANAGER_FUNCTIONS.
	Remarks:
		+ You must override this function to indicate which functions your VR manager implements.
	Example:
	// The attribute manager implements two VRBaseManager functions : OnParse and OnViewWarp,
	// so its GetValidFunctionsMask looks like this:
	virtual CKDWORD GetValidVRFunctionsMask()	
	{
		return  VRMANAGER_FUNC_OnParse		|
				VKMANAGER_FUNC_OnViewWarp	;
	}  
	
	See Also: VRMANAGER_FUNCTIONS
	*************************************************/	
	virtual CKDWORD	GetValidVRFunctionsMask() { return 0; }	

	/*************************************************
	Summary: Called at the end of the creation of a CKContext.
	Return Value:
		+ CK_OK if successful
		+ An error code otherwise
	Remarks:
		+ You can override this function if you need to add specific processing at initialization.
		If you do so, you *must* call the current implementation.
		+ If your manager is registered after the context has been created and if it implements the OnCKInit()
		function, this function will be called upon registration.
		+ The GetValidFunctionsMask implementation of the VRBaseManager return a valid mask for this method.
	
	See Also:Main Virtools Events, CKBaseManager::OnCKInit, VRBaseManager::GetValidFunctionsMask
	*************************************************/	
	virtual CKERROR OnCKInit();

	/*************************************************
	Summary: Called at deletion of a CKContext
	Return Value:
		+ CK_OK if successful
		+ An error code otherwise
	Remarks:
		+ You can override this function if you need to clean up things at the end of the session.
		If you do so, you *must* call the current implementation.
		+ This method is called at the beginning of the deletion of the CKContext, the manager 
		is also deleted a short time after.
		+ The GetValidFunctionsMask implementation of the VRBaseManager return a valid mask for this method.
	
	See Also:Main Virtools Events, CKBaseManager::OnCKEnd, VRBaseManager::GetValidFunctionsMask
	*************************************************/	
	virtual CKERROR OnCKEnd();

	/*************************************************
	Summary: Returns list of functions implemented by the manager.
	Return Value: A combination of CKMANAGER_FUNCTIONS.
	Remarks:
		+ You can override this function to indicates the additional callback methods that 
		are implemented by your manager from the CKBaseManager vitual methods. If you do so,
		you *must* call the current implementation as the VRBaseManager overrides OnCKInit
		and OnCKEnd.
	
	See Also: CKBaseManager::GetValidFunctionMask, CKMANAGER_FUNCTIONS
	*************************************************/	
	virtual CKDWORD	GetValidFunctionsMask()	
	{
		return	CKMANAGER_FUNC_OnCKInit |
				CKMANAGER_FUNC_OnCKEnd; 
	}

public:
	VRContext*	m_VRContext; // A pointer to the VRContext on which this manager is valid.
};

#endif
