/*************************************************************************/
/*	File : VRError.h													 */
/*	Author :  Leïla AIT KACI											 */	
/*																		 */	
/*	Base Class for Virtools VRPack Managers								 */	
/*	Virtools SDK 														 */	 
/*	Copyright (c) Virtools 2004, All Rights Reserved.					 */	
/*************************************************************************/
#ifndef VRError_H

#define VRError_H "$Id:$"

// Operation successful, same as CK_OK
// 

#define VR_OK								0

// Generic Error when nothing fits
// 

#define VRERR_GENERICERROR					-300

// Invalid VRPack.cfg Configuration file
// 

#define VRERR_INVALIDCFGFILE				-301

// Invalid host : not in host list
// 

#define VRERR_INVALIDHOST					-302

// Token could not be found
// 

#define VRERR_TOKENNOTFOUND					-303

// Token value format is wrong
// 

#define VRERR_TOKENFORMATERROR				-304

// Token value format is wrong
// 

#define VRERR_TOKENWRONGTYPE				-305

// Invalid view index for the current host
// 

#define VRERR_INVALIDVIEW					-306

// Warping size too large. Can't create target texture
// 

#define VRERR_WARPTOOLARGE					-307

// The requested data already existed
// 

#define VRERR_ALREADYEXIST					-308

// This error should never be raised
// 

#define VRERR_IMPOSSIBLE					-309

// No VRContext available.
// 

#define VRERR_NOVRCONTEXT					-310

#endif