// screen_common.cpp : implementation file
//

#include "stdafx.h"
#include "screen_common.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

using namespace client;

namespace client {
    namespace screen {

//////////////////////////////////////////////////////////////////////////

const RECT margin_::CAMERA_RECT  = { 2, 2, 2, 2 };

const SIZE osd_size_::SCALE_BASE = { 480, 270 };
const SIZE osd_size_::TEXT_IN    = { 512, 480 };
const SIZE osd_size_::TITLE      = { 512, 48  };
const SIZE osd_size_::DATETIME   = { 320, 48  };

//////////////////////////////////////////////////////////////////////////

    } // !_namespace_screen
} // !_namespace_client
