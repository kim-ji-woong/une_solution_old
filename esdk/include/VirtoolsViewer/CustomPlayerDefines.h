/******************************************************************************
File : CustomPlayerDefines.h

Description: This file contains some defines/enums/types used by the project.

Virtools SDK
Copyright (c) Virtools 2005, All Rights Reserved.
******************************************************************************/

#if !defined(CUSTOMPLAYERDEFINES_H)
#define CUSTOMPLAYERDEFINES_H

#define INIT_ERROR						"Initialisation Error"
#define CANNOT_READ_CONFIG				"Cannot read configuration/command line.\nPlayer will quit!"
#define FAILED_TO_CREATE_WINDOWS		"Unable to create windows.\nPlayer will quit!"
#define UNABLE_TO_INIT_PLUGINS			"Unable to initialize plugins.\nPlayer will quit!"
#define UNABLE_TO_LOAD_RENDERENGINE		"Unable to load a RenderEngine.\nPlayer will quit!"
#define UNABLE_TO_INIT_CK				"Unable to initialize CK Engine.\nPlayer will quit!"
#define UNABLE_TO_INIT_MANAGERS			"Unable to initialize Managers.\nPlayer will quit!"
#define UNABLE_TO_INIT_DRIVER			"Unable to initialize display driver.\nPlayer will quit!"
#define UNABLE_TO_CREATE_RENDERCONTEXT	"Cannot initialize RenderContext.\nPlayer will quit!"
#define CANNOT_FIND_LEVEL				"Cannot find Level.\nPlayer will quit!"

#define MAINWINDOW_TITLE				"Virtools Custom Player"
#define MAINWINDOW_CLASSNAME			"CustomPlayer"
#define RENDERWINDOW_CLASSNAME			"CustomPlayerRender"
#define MISSINGUIDS_LOG					"CustomPlayerMissingGuids.log"



#define PUBLISHING_RIGHT_TITLE		"Note about Publishing Rights"
#define PUBLISHING_RIGHT_TEXT	\
	"IMPORTANT: The following warning should be removed by you (in the source\n"	\
	"code prior to compilation).\n\n"	\
	"This dialog box serves to remind you that publishing rights, a.k.a. runtime\n"	\
	"fees, are due when building any custom executable (like the player you just\n"	\
	"compiled). Contact info@virtools.com for more information.\n"


enum EConfigPlayer
{
	eAutoFullscreen	= 1,	// -auto_fullscreen (or -f) on command line
	eDisableKeys	= 2		// -disable_keys (or -d) on command line
};

#endif // CUSTOMPLAYERDEFINES_H