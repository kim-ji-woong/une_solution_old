/*************************************************************************/
/*	File : VRGlobals.h													 */
/*	Author :  Leïla AIT KACI											 */	
/*																		 */	
/*	Base Class for Virtools VRPack Managers								 */	
/*	Virtools SDK 														 */	 
/*	Copyright (c) Virtools 2004, All Rights Reserved.					 */	
/*************************************************************************/
#ifndef VRGlobals_H

#define VRGlobals_H "$Id:$"

#include "CKAll.h"
#include "VRAll.h"

/***************************************************************
Summary: Create a new VRContext
Output Arguments:
	+ oVRContext: created VRContext
Input Arguments:
	+ iCKContext: CKContext to which the VRContext is attached. This argument
	may be NULL to allow getting token values prior to the CKContext creation.
	In this case, a CKContext is automatically attached when creating the 
	VRMainManager.
Return Value:
	+ VR_OK if success
	+ VR_ALREADYEXIST if a VRContext already exist for the given CKContext
Remarks:
	+ The created context must be closed with VRCloseContext.

See also: VRGetContext, VRCloseContext, VRContext.
****************************************************************/
VRERROR VRCreateContext(VRContext** oVRContext, CKContext* iCKContext);

/***************************************************************
Summary: Get the VRContext attached to the given CKContext
Output Arguments:
	+ oVRContext: The VRContext found, a NULL pointer otherwise.
Input Arguments:
	+ iCKContext: CKContext to which the VRContext is attached.
Return Value: VR_OK

See also: VRCreateContext, VRCloseContext, VRContext.
****************************************************************/
VRERROR VRGetContext(VRContext** oVRContext, CKContext* iCKContext);
VRContext* VRGetContext( CKContext* iCKContext ); 

/***************************************************************
Summary: Close a VRContext created with VRCreateContext.
Input Arguments:
	+ iVRContext: context to close.
Return Value: VR_OK
Remarks:
	+ The VRContext should be close at the end of the application.
	+ Closing the VRContext will call OnViewDelete on all the VRManager
	to allow the VRBaseManager to delete user datas.

See also: VRCreateContext, VRGetContext, VRContext.
****************************************************************/
VRERROR VRCloseContext(VRContext* iVRContext);

/*******************************************
Summary: Convert a VRERROR code to a descriptive string.
Input Arguments:
	+ iErr: An error code which description string should be returned.
Return Value:
	A string describing the error.
Remarks:
	+ When the error is not a VRERROR, the string returned is "Unknown VR Error"

See Also: VRERROR
********************************************/
CKSTRING VRErrorToString(VRERROR iErr);




#endif
