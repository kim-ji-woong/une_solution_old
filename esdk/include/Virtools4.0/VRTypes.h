/*************************************************************************/
/*	File : VRTypes.h													 */
/*	Author :  Leïla AIT KACI											 */	
/*																		 */	
/*	Base Class for Virtools VRPack Managers								 */	
/*	Virtools SDK 														 */	 
/*	Copyright (c) Virtools 2004, All Rights Reserved.					 */	
/*************************************************************************/
#ifndef VRTypes_H

#define VRTypes_H "$Id:$"

#include "CKAll.h"
//#include "VRUtils.h"

//----------------------------------------------------------////
//		Class  List											////
//----------------------------------------------------------////
class VRConfig;
class VRContext;
class VRBaseManager;

/***************************************************************
Summary: A result of this type can be any of CKERR_xx or VRERR_xx defines values 

Remarks:
+VR_OK: Operation successful, same as CK_OK.
+VRERR_GENERICERROR: Generic VR Error when no precise description is available.
+VRERR_INVALIDCFGFILE: The VRPack.cfg Configuration file is invalid.
+VRERR_INVALIDHOST: The current host is not in the host list.
+VRERR_TOKENNOTFOUND: The requested token could not be found.
+VRERR_TOKENFORMATERROR: The requested token is wrongly formated.
+VRERR_TOKENWRONGTYPE: The requested token has the wrong type.
+VRERR_INVALIDVIEW: The given view index is invalid.
+VRERR_WARPTOOLARGE: Warping size is too large. A target texture can not be created.
+VRERR_ALREADYEXIST: The requested data already exist.
+VRERR_IMPOSSIBLE: This error should never be raised
+VRERR_NOVRCONTEXT: No VRContext is available (ie, we are not in the VRPlayer).
****************************************************************/
typedef	DWORD VRERROR;



#define AUTHOR_NAME						"Virtools"
#define AUTHOR_GUID						VIRTOOLS_GUID

#define VRDISTRIB_BEHAVIORS_GUID		CKGUID(0x6d0f3f29,0xc014d8)
#define VRCORE_BEHAVIORS_GUID		    CKGUID(0x67fc33ee,0x2ec64ede)
#define VRDISPLAY_BEHAVIORS_GUID		CKGUID(0x2e0202b,0x2e083275)

#define DISTRIB_CATEGORY				"VR/Cluster"
#define VR_COMMON						"VR/Core"
#define DISPLAY_CATEGORY		        "VR/Visualisation"

#define VRDISTRIB_MANAGER_GUID			CKGUID(0x593311eb,0x759a1966)
#define VRPN_MANAGER_GUID		        CKGUID(0x50da2d7b,0x59341bcc)
#define VRMAIN_MANAGER_GUID             CKGUID(0xFB7BBA92,0x14EEE4E3)
#define VRDISPLAY_MANAGER_GUID	        CKGUID(0x7006831,0x5b8a739f)
#define VRKERNEL_MANAGER_GUID           CKGUID(0x58480c87,0x4b787f3a)

#define VRDISPLAY_AUTHOR_NAME	"Virtools"
#define VRDISPLAY_AUTHOR_GUID	VIRTOOLS_GUID

#define TRCLEVEL_PLUGIN			2 // Level after which we display the plugins that were loaded.

/*******************************************************************************
Summary: Execution mode of the composition: Virtools Dev, VRPlayer...

Remarks:
	+ By default, a VRContext is in Virtools Dev Mode
See also: VRContext::GetExecutionMode, VRContext::SetExecutionMode
*******************************************************************************/
enum VR_EXECUTION_MODE
{
	VREXECUTION_DEV,					// Virtools Dev
	VREXECUTION_VRPLAYER,				// VRPlayer
	VREXECUTION_CUSTOM,					// Any other custom player
};




/*******************************************************************************
Summary: Interface for the VRDisplay Manager
*******************************************************************************/
class IVRDisplayManager {
protected:
    IVRDisplayManager( void ) {}
    virtual ~IVRDisplayManager( void ) = 0 {}

public:
    virtual
        void SetStereoParameters( const float& eyeSeparation,
                                  const float& focalLength ) = 0 ;
    virtual
        CKBOOL	IsStereoRenderingLeftEye() = 0;
};

/*******************************************************************************
Summary: Information on a view.

Remarks:
	+ This structure is filled according to the View_X_X token values
	of the configuration files.
See also: VRContext::GetViewCount, VRContext::GetView, 
*******************************************************************************/
struct VRView
{
	int				ID;					// View ID (index) on the host
	XString			CameraName;			// Camera used by the view
	XString			Mode;				// Camera mode : none, left, right, quad, custom...

	int				PositionX;			// X position in render window
	int				PositionY;			// Y position in render window
	int				Width;				// Width in render window
	int				Height;				// Height in render window	

	int				ScreenID;			// *Reserved for future use* - Screen ID
	void*			UserData;			// User data attached to the view
	XString			UserTokens;			// Additionnal user tokens

	DWORD			Flags;				// *Reserved for internal use* - Can be one or more of VR_VIEW_FLAGS
	CKTexture*		WarpTexture;		// Warping texture if available
};

/*******************************************************************************
Summary: Information on the warping of a view

Remarks:
	+ Width and Height need to be specified only when using a warping texture.
See also: VRContext::ActivateWarp
*******************************************************************************/
struct WarpInfo
{
	CKBOOL			UseWarpTexture;		// Should we use a render target
	int				Width;				// Width of the render target to use
	int				Height;				// Height of the render target to use
};


#endif
