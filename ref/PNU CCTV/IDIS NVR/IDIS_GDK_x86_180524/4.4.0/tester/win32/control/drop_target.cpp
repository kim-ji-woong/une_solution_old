// Implementation file
//

#include "stdAfx.h"
#include "drop_target.h"
#include <include/g2_guid.h>

#include <afxadv.h>

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

#pragma warning(disable : 4312)

using namespace client;

//////////////////////////////////////////////////////////////////////////

drop_info::drop_info(void)
    : _dragFromId(0)
    , _dropcamera(invalid_::CAMERA_NUMBER)
    , _hostcamera(invalid_::CAMERA_NUMBER)
    , _countcamera(0)
    , _channelext(invalid_::CHANNEL_EXT)
    , _cameraLevel(0)
    , _connectMode(connect_mode_::UNDEFINED)
{

}

drop_info::drop_info(const drop_info& right)
{
    operator=(right);
}

drop_info::~drop_info(void)
{

}

//////////////////////////////////////////////////////////////////////////

drop_target::drop_target(HWND target, drop_event* eventer)
    : CIDropTarget(target)
    , _eventer(eventer)
{

}

//////////////////////////////////////////////////////////////////////////

bool drop_target::secureDropInfo(
                            drop_info& di,
                            FORMATETC* format,
                            STGMEDIUM* medium,
                            DWORD* effect,
                            const POINTL& point,
                            LONG_PTR objectID)
{
    if (format == NULL) return false;
    if (medium == NULL) return false;

    bool fcontinue = false;

    if (format->cfFormat == CF_TEXT &&
        medium->tymed == TYMED_HGLOBAL &&
        medium->hGlobal != NULL) {
        fcontinue = true;
    }

    if (fcontinue == false) {
        assert(!"failed to access shared memory");
        return false;
    }

    HGLOBAL hMemory = medium->hGlobal;

    CSharedFile memFile;
    memFile.SetHandle(hMemory, FALSE);
    CArchive ar(&memFile, CArchive::load);

    ///////////////////////////////////////////////////////

    ar >> di._dragFromId;

    if (di._dragFromId == drag_id_::DRAG_FROM_DEVICE_TREE) {
        ar >> di._sitename;
        ar >> di._hostcamera;
        di._channelext = di._hostcamera;;
    }
    else if (di._dragFromId == drag_id_::DRAG_FROM_INSTANT_EVENT) {
        /*
        ar >> di._strDeviceGUID;
        ar >> di._eventInfo._spot._segment;
        ar >> di._eventInfo._spot._time._time;
        ar >> di._eventInfo._spot._tick;
        ar >> di._eventInfo._refcameras;

        di._guidDevice = g2_guid_from_string(static_cast<LPCTSTR>(di._strDeviceGUID));
        */
    }
    else {
        assert(!"drag and drop about not registered type");
    }

    ar.Close();

    ///////////////////////////////////////////////////////

    di._point.x = point.x;
    di._point.y = point.y;

    /* not used
     *
    if (format->cfFormat == CF_HDROP &&
        medium.tymed == TYMED_HGLOBAL) {
        HDROP hDrop = (HDROP)GlobalLock(medium.hGlobal);
        if (hDrop != NULL) {
            TCHAR szFileName[MAX_PATH];
            unsigned int cFiles = DragQueryFile(hDrop, 0xFFFFFFFF, NULL, 0);
            for (unsigned int i = 0; i < cFiles; ++i) {
                ::DragQueryFile(hDrop, i, szFileName, sizeof(szFileName));
                // ...
            }
        }
        GlobalUnlock(medium.hGlobal);
    }*
     */

    return true;
}

bool drop_target::OnDrop(FORMATETC* format, STGMEDIUM* medium, DWORD* effect, POINTL point, LONG_PTR userData)
{
    drop_info di;
    if (secureDropInfo(di, format, medium, effect, point, userData)) {
        _eventer->OnDrop(di);
    }
    return true; // let base free the medium
}

bool drop_target::OnDragEnter(FORMATETC* format, STGMEDIUM* medium, DWORD* effect, POINTL point)
{
    return true;
}

bool drop_target::OnDragOver(POINTL point)
{
    return true;
}

void drop_target::OnDragLeave(void)
{

}

//////////////////////////////////////////////////////////////////////////

bool client::insert_screen_drop_property(HWND control, HWND receiver, unsigned int message)
{
    if (control == NULL || ::IsWindow(control) != TRUE) return false;
    if (receiver == NULL || ::IsWindow(receiver) != TRUE) return false;
    if (WM_USER < 0) return false;

    SetProp(control, _T("client::is_support_drop_camera_from_screen"), (HANDLE)(1));
    SetProp(control, _T("client::drop_receiver"), (HANDLE)(receiver));
    SetProp(control, _T("client::drop_message"), (HANDLE)(message));

    return true;
}

bool client::remove_screen_drop_property(HWND control)
{
    if (control == NULL || ::IsWindow(control) != TRUE) return false;

    RemoveProp(control, _T("client::is_support_drop_camera_from_screen"));
    RemoveProp(control, _T("client::drop_receiver"));
    RemoveProp(control, _T("client::drop_message"));

    return true;
}
