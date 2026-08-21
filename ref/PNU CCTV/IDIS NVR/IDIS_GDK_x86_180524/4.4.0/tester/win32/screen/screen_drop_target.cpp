// Implementation file
//

#include "stdAfx.h"
#include "screen_drop_target.h"
#include "../include/g2_guid.h"

#include <afxadv.h>

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

#pragma warning(disable : 4312)

using namespace client;
using namespace screen;

//////////////////////////////////////////////////////////////////////////

screen_drop_target::screen_drop_target(HWND target, drop_event_base* dropEvent)
    : CIDropTarget(target)
    , _dropEvent(dropEvent)
{
    assert(dropEvent != NULL);
}

screen_drop_target::~screen_drop_target(void)
{

}

//////////////////////////////////////////////////////////////////////////

bool screen_drop_target::OnDrop(FORMATETC* format, STGMEDIUM* medium, DWORD* effect, POINTL point, LONG_PTR userData)
{
    bool succeed = false;
    if (_dropEvent) {
        succeed = _dropEvent->OnDrop(format, medium, effect, point, userData);
        _secure.reset();
    }
    return succeed; // let base free the medium
}

bool screen_drop_target::OnDragEnter(FORMATETC* format, STGMEDIUM* medium, DWORD* effect, POINTL point)
{
    return (_dropEvent) ? _dropEvent->OnDragEnter(format, medium, effect, point, NULL) : false;
}

bool screen_drop_target::OnDragOver(POINTL point)
{
    return (_dropEvent) ? _dropEvent->OnDragOver(point) : false;
}

void screen_drop_target::OnDragLeave(void)
{
    if (_dropEvent) {
        _dropEvent->OnDragLeave();
        _secure.reset();
    }
}
