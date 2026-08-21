// frame_buffer.cpp : implementation file
//

#include "stdafx.h"
#include "frame_buffer.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

using namespace client;

//////////////////////////////////////////////////////////////////////////

frame_buffer::frame_buffer(MODE mode, TYPE type, int limit)
    : _mode(mode)
    , _type(type)
    , _limit(limit)
    , _capacity(_I16_MAX)
    , _channel(-1)
{

}

frame_buffer::~frame_buffer(void)
{

}

//////////////////////////////////////////////////////////////////////////
