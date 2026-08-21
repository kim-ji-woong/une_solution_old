/*************************************************************************/
/*	File : VRContext.h													 */
/*	Author :  Leïla AIT KACI											 */	
/*																		 */	
/*	Base Class for Virtools VRPack Managers								 */	
/*	Virtools SDK 														 */	 
/*	Copyright (c) Virtools 2004, All Rights Reserved.					 */	
/*************************************************************************/
#ifndef VRContext_H

#define VRContext_H "$Id:$"

#include "VRBaseManager.h"

#if defined(FLEXLM)
#include "VFlexLMUtils.h"
#endif

#include <list>
#include <vector>
#include <string>

// Forward declaration

class CKPluginManager;
class FileLogger;




/*************************************************************************
{filename:VRContext}
Summary: Main VR Interface Context
Remarks: 
	+ The VRContext object gives access to all VR informations for Virtools VR Pack
	  applications.
	+ Use VRCreateContext to create a new VR context and VRCloseContext to close it.
	+ Use VRGetContext to get the VRContext attached to a give CKContext.
	+ For now, the VRContext is not available in Virtools Dev Mode, it is only
	available in the VRPlayer mode.

See also: CKContext, VRCreateContext, VRCloseContext, VRGetContext.
*************************************************************************/
class VRContext
{
public :                                 
    /***************************************************************
    Summary: Get the VRDisplayManager
    Return Value:
        + A pointer to the VRDisplayManager        
    ****************************************************************/
    IVRDisplayManager* GetVRDisplayManager( void );
    
    /***************************************************************
    Summary: check the licenses
    Return Value:
        + true: all licenses requested by the license configuration
                are present
        + false: at least one license is missing
    ****************************************************************/       
    bool CheckLicenses( void );

    /***************************************************************
    Summary: Open the config file.
    Return Value:
        + VR_OK: success
        + VRERR_INVALIDCFGFILE: couldn't open the file
    ****************************************************************/
    VRERROR InitParse( void );
    VRERROR InitParse( XString& fileName );

    /***************************************************************
    Summary: Close the config file.    
    ****************************************************************/
    void CloseParse();

	/***************************************************************
	Summary: Get the main configuration file name.
	Return Value:
		+ Absolute path to the VRPack.cfg configuration file of the
		application or "" if not found.
	
    See also: SetCfgFilename
	****************************************************************/
	XString&	GetCfgFilename();

    /***************************************************************
    Summary: Set the main configuration file name.
    Input Argument:
    + Absolute path to the VRPack.cfg configuration file of the
    application
    
    See also: GetCfgFilename
    ****************************************************************/
    void	SetCfgFilename( XString& );

	/***************************************************************
	Summary: Get a token value.
	Input Arguments:
		+ iToken: Token unique name
		+ iDefault: Default value to return when the token was not found
	Output Arguments:
		+ oValue: Token's read value
	Return Value:
		+ VR_OK if successfully found
		+ VRERR_INVALIDCFGFILE when the configuration file is invalid
		+ VRERR_TOKENNOTFOUND when the token was not found
	Remarks: 
		+ This method is case insensitive regarding the iToken value.
		+ When an error occurs, the returned oValue is set to iDefault.
		+ The token values is displayed on the output window (Event Log
		or Console Window).
	
	See also: GetHiddenToken
	****************************************************************/
	CKERROR GetToken(const XString& iToken, int *oValue, int iDefault=0);

	CKERROR GetToken(const XString& iToken, float *oValue, float iDefault=0.0f);

	CKERROR GetToken(const XString& iToken, XString *oValue, XString iDefault="-");

	/***************************************************************
	Summary: Get a token value without displaying it
	Input Arguments:
		+ iToken: Token unique name
		+ iDefault: Default value to return when the token was not found.
	Output Arguments:
		+ oValue: Token's readed value
	Return Value:
		+ VR_OK if succesfully found
		+ VRERR_INVALIDCFGFILE when the configuration file is invalid
		+ VRERR_TOKENNOTFOUND when the token was not found
	Remarks: 
		+ This method is case insensitive regarding the iToken value.
		+ When an error occurs, the returned oValue is set to iDefault.
		+ The token values is not displayed on the output window 
		(Event Log or Console Window).
	
	See also: GetToken
	****************************************************************/
	CKERROR GetHiddenToken(const XString& iToken, int *oValue, int iDefault=0);

	CKERROR GetHiddenToken(const XString& iToken, float *oValue, float iDefault=0.0f);

	CKERROR GetHiddenToken(const XString& iToken, XString *oValue, XString iDefault="-");

    /***************************************************************
    Summary: Get a token value represented by an array of 3 ints
    Input Arguments:
    + iToken: Token unique name
    + iDefault: Default value to return when the token was not found
    Output Arguments:
    + oValue: Token's read value
    Return Value:
    + VR_OK if successfully found
    + VRERR_INVALIDCFGFILE when the configuration file is invalid
    + VRERR_TOKENNOTFOUND when the token was not found
    Remarks: 
    + This method is case insensitive regarding the iToken value.
    + When an error occurs, the returned oValue is set to iDefault.
    + The token values is displayed on the output window (Event Log
    or Console Window).
        
    ****************************************************************/
    CKERROR GetInt3(const XString& iToken, int* oValue, const int* iDefault);

    /***************************************************************
    Summary: Get a token value represented by an array of 3 ints
    Input Arguments:
    + iToken: Token unique name    
    Output Arguments:
    + oValue: Token's read value
    Return Value:
    + VR_OK if successfully found
    + VRERR_INVALIDCFGFILE when the configuration file is invalid
    + VRERR_TOKENNOTFOUND when the token was not found
    Remarks:    
    + This method is case insensitive regarding the iToken value.
    + When an error occurs, the returned oValue is *not* modified.
    + The token values is displayed on the output window (Event Log
    or Console Window).
        
    ****************************************************************/
    CKERROR GetInt3(const XString& iToken, int* oValue);


    /***************************************************************
    Summary: Get a token value represented by an array of 3 floats
    Input Arguments:
    + iToken: Token unique name
    + iDefault: Default value to return when the token was not found
    Output Arguments:
    + oValue: Token's read value
    Return Value:
    + VR_OK if successfully found
    + VRERR_INVALIDCFGFILE when the configuration file is invalid
    + VRERR_TOKENNOTFOUND when the token was not found
    Remarks: 
    + This method is case insensitive regarding the iToken value.
    + When an error occurs, the returned oValue is set to iDefault.
    + The token values is displayed on the output window (Event Log
    or Console Window).
        
    ****************************************************************/
    CKERROR GetFloat3(const XString& iToken, float* oValue, const float* iDefault);

    /***************************************************************
    Summary: Get a token value represented by an array of 3 floats
    Input Arguments:
    + iToken: Token unique name    
    Output Arguments:
    + oValue: Token's read value
    Return Value:
    + VR_OK if successfully found
    + VRERR_INVALIDCFGFILE when the configuration file is invalid
    + VRERR_TOKENNOTFOUND when the token was not found
    Remarks:    
    + This method is case insensitive regarding the iToken value.
    + When an error occurs, the returned oValue is *not* modified.
    + The token values is displayed on the output window (Event Log
    or Console Window).
        
    ****************************************************************/
    CKERROR GetFloat3(const XString& iToken, float* oValue);


    /***************************************************************
    Summary: Get a token value represented by an array of 4 floats
    Input Arguments:
    + iToken: Token unique name
    + iDefault: Default value to return when the token was not found
    Output Arguments:
    + oValue: Token's read value
    Return Value:
    + VR_OK if successfully found
    + VRERR_INVALIDCFGFILE when the configuration file is invalid
    + VRERR_TOKENNOTFOUND when the token was not found
    Remarks: 
    + This method is case insensitive regarding the iToken value.
    + When an error occurs, the returned oValue is set to iDefault.
    + The token values is displayed on the output window (Event Log
    or Console Window).
        
    ****************************************************************/
    CKERROR GetFloat4(const XString& iToken, float* oValue, const float* iDefault);

    /***************************************************************
    Summary: Get a token value represented by an array of 4 floats
    Input Arguments:
    + iToken: Token unique name    
    Output Arguments:
    + oValue: Token's read value
    Return Value:
    + VR_OK if successfully found
    + VRERR_INVALIDCFGFILE when the configuration file is invalid
    + VRERR_TOKENNOTFOUND when the token was not found
    Remarks:    
    + This method is case insensitive regarding the iToken value.
    + When an error occurs, the returned oValue is *not* modified.
    + The token values is displayed on the output window (Event Log
    or Console Window).
        
    ****************************************************************/
    CKERROR GetFloat4(const XString& iToken, float* oValue);


    /***************************************************************
    Summary: Get a token value represented by XString
    Input Arguments:
    + iToken: Token unique name
    + iDefault: Default value to return when the token was not found
    Output Arguments:
    + oValue: Token's read value
    Return Value:
    + VR_OK if successfully found
    + VRERR_INVALIDCFGFILE when the configuration file is invalid
    + VRERR_TOKENNOTFOUND when the token was not found
    Remarks: 
    + This method is case insensitive regarding the iToken value.
    + When an error occurs, the returned oValue is set to iDefault.
    + The token values is displayed on the output window (Event Log
    or Console Window).
        
    ****************************************************************/
    CKERROR GetString(const XString& iToken, char* oValue, const char* iDefault);

    /***************************************************************
    Summary: Get a token value represented by a XString
    Input Arguments:
    + iToken: Token unique name    
    Output Arguments:
    + oValue: Token's read value
    Return Value:
    + VR_OK if successfully found
    + VRERR_INVALIDCFGFILE when the configuration file is invalid
    + VRERR_TOKENNOTFOUND when the token was not found
    Remarks:    
    + This method is case insensitive regarding the iToken value.
    + When an error occurs, the returned oValue is *not* modified.
    + The token values is displayed on the output window (Event Log
    or Console Window).
        
    ****************************************************************/
    CKERROR GetString(const XString& iToken, char* oValue);

	/***************************************************************
	Summary: Get a subtoken value from the given string.
	Input Arguments:
		+ iInput: String to search
		+ iToken: Subtoken unique name
		+ iDefault: Subtoken's default value
	Output Arguments:
		+ oValue: Subtoken's readed value
	Return Value:
		+ VR_OK if succesfully found
		+ VRERR_TOKENNOTFOUND when the token was not found
		+ VRERR_TOKENFORMATERROR when the token is wrongly formatted
		+ VRERR_TOKENWRONGTYPE when the token was found but is not of integer type
	Remarks: 
		+ This method is case insensitive regarding the iToken value.
		+ A subtoken must be formated as (SubTokenName=SubTokenValue) when the
		value doesn't contain any space character or as (SubTokenName="SubToken Value") 
		when the value contains or not a space character.
		+ You will usually use this method to parse the user token on the 'View' 
		definition given by VRView::UserTokens.
		+ When an error occurs, the returned oValue is set to iDefault.
	
	See also: VRView::UserTokens
	****************************************************************/
	CKERROR GetSubToken(const XString& iInput, const XString& iToken, int *oValue, int iDefault=0);
	
	CKERROR GetSubToken(const XString& iInput, const XString& iToken, float *oValue, float iDefault=0.0f);

	CKERROR GetSubToken(const XString& iInput, const XString& iToken, XString *oValue, XString iDefault="-");

	/***************************************************************
	Summary: Get the current LogLevel as it is define in the
	application's configuration file (token 'LogLevel').
	Return Value:
		+ LogLevel value
	
	****************************************************************/
	int GetLogLevel() const;

	/***************************************************************
	Summary: Get the current BaseDirectory as it is define in the 
	application's configuration files (token 'BaseDirectory').
	Return Value:
		+ BaseDirectory value
	Remarks:
		+ The BaseDirectory is always an absolute path
	
	****************************************************************/
	XString GetBaseDirectory();

	/***************************************************************
	Summary: Get the current list of hosts as it is define in the
		application's configuration file (token 'Hosts').
	Return Value:
		+ Hosts list
	Remarks:
		+ Hosts are separated by a space in the host list string.
	
	****************************************************************/
	XString GetHostList();

	/***************************************************************
	Summary: Get the ID of the current host (i.e. its index in the 
	host list) as it is define in the application's configuration file.
	Return Value:
		+ The ID of the current host.
	Remarks:
		+ When the current host appears 2 times in the host list,
		its ID depends on the current PipeID value:
		- If it is 0, the ID is the first appearance of the host in the list,
		- If it is 1, the ID is the second appearance of the host in the list,
		- ...
	
	****************************************************************/
	int GetHostID();

	/***************************************************************
	Summary: Get the number of hosts that appears in the host list
	as it is define in the application's configuration file.
	Return Value:
		+ Number of host on the cluster
	
	****************************************************************/
	int GetHostCount();	

	/***************************************************************
	Summary: Get the current VR execution mode (Virtools Dev, VRPlayer...)
	Return Value:
		+ Current execution mode
	
	See also: SetExecutionMode
	****************************************************************/
	VR_EXECUTION_MODE GetExecutionMode();

	/***************************************************************
	Summary: Display a message on the output window (Event Log or Console Window)
	Input Arguments:
		+ iLevel: The message will be displayed only if iLevel is bigger than the 
		current LogLevel.
		+ iFormat...: Same format as the printf function. Please review you C++ documentation 
		for more information.
	
	See also: GetLogLevel
	****************************************************************/
	void	OutputToLog(int iLevel, const char* iFormat,...) const;

	/***************************************************************
	Summary: Get the number of views on the current host
	Return Value:
		+ View count on the current host
	Remarks:
		+ The views are defined by the View_x_x tokens of the 
		configuration files.
		+ The list order is given by the declaration order of the
		configuration files.
	
	See also: VRContext::GetView, VRContext::GetViewData
	****************************************************************/
	int GetViewCount();

	/***************************************************************
	Summary: Get a view of the current host
	Input Arguments: 
		+ iIndex: Index of the view (View ID)
	Return Value:
		+ NULL if the view was not found, a pointer on the view
		information otherwise.
	Remarks:
		+ The views are defined by the View_x_x tokens of the 
		configuration files.
		+ The list order is given by the declaration order of the
		configuration files.
	
	See also: VRContext::GetViewCount
	****************************************************************/
	VRView* GetView(int iIndex);

	/***************************************************************
	Summary: Get the user data attached to a view
	Input Arguments: 
		+ iView : View where to get the data from
	Return Value: 
		+ The view data or NULL if none was set
	
	See also: VRContext::GetViewCount, VRContext::GetView, VRContext::SetViewData
	****************************************************************/
	void* GetViewData(VRView* iView);

	/***************************************************************
	Summary: Attach a user data to a view
	Input Arguments: 
		+ iView: View where to get the data from
		+ iData: data to set
	Return Value: 
		+ The previous data if one was set
	
	See also: VRContext::GetViewCount, VRContext::GetView, VRContext::GetViewData
	****************************************************************/
	void* SetViewData(VRView* iView, void* data);

	/***************************************************************
	Summary: Activate the VRPlayer warping mode on the given view.
	Input Arguments:
		+ iView: View to warp
		+ iInfo: Warping info
	Return Value:
		+ VR_OK if successfully
		+ VRERR_WARPTOOLARGE when the warping size is too large and a warping texture 
		could not be created for it.
	Remarks: 
		+ When activating texture warping, this method create a new warping texture
		and set it in iInfo::WarpTexture. This texture is created as a private and
		not for deleted object. All the objects you create for your warping mechanism
		should be created with these flags as well:
		CKObject::ModifyObjectFlags(CK_OBJECT_NOTTOBEDELETED|CK_OBJECT_PRIVATE, 0);
		+ The VRManager::OnViewWarp callback will be called for each warped view. 
		You should implement the final warped rendering in this callback.
		+ You will usually call this method from VRBaseManager::OnViewCreate.
	
	See also: VRContext::GetViewCount, VRContext::GetView, 
	VRContext::DeActivateWarp, VRContext::GetWarpTexture,
	VRBaseManager::OnViewWarp, VRBaseManager::OnViewCreate
	****************************************************************/
	CKERROR ActivateWarp(VRView* iView, WarpInfo* iInfo);

	/***************************************************************
	Summary: Deactivate the VRPlayer warping mode on the given view.
	Input Arguments:
		+ iView: View to unwarp
	Remarks: 
		+ If a warping texture was used, it is destroyed. So, make sure
		to remove any reference to this texture when unwarping a view.
		+ You will usually call this method from VRBaseManager::OnViewDelete.
	
	See also: VRContext::GetViewCount, VRContext::GetView, VRContext::ActivateWarp,
	VRContext::GetWarpTexture, VRBaseManager::OnViewDelete
	****************************************************************/
	void DeActivateWarp(VRView* iView);	

	/***************************************************************
	Summary: Get the warping texture used for the given view.
	Input Arguments:
		+ iView: Warped view
	Return Value:
		+ NULL if the view is not warped or not using a warping texture,
		the warping texture otherwise
	
	See also: VRContext::GetViewCount, VRContext::GetView,
	VRContext::ActivateWarp, VRView
	****************************************************************/
	CKTexture* GetWarpTexture(VRView* iView);

    /***************************************************************
    Summary: Tells the VRPlayer to exit at the end of this frame.
    ****************************************************************/
    void FlagExit();    
        
    /***************************************************************
    Summary: Register a new VR Manager.
    Input Argument:
        + iManager: the manager to register
    Return Value:
        + CKERR_ALREADYPRESENT: if manager is already registered
        + VR_OK: if manager was successfully registered
    ****************************************************************/
    CKERROR RegisterNewManager(VRBaseManager* iManager);

/* CK_PRIVATE_VERSION_VIRTOOLS */
};

#endif
