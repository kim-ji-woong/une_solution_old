// drop_target.h : header file
//

#ifndef _CLIENT_DROP_TARGET_H_
#define _CLIENT_DROP_TARGET_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "drag_drop_impl.h"

#include <sampler/cpp/g2client_guid.h>

namespace client {

//////////////////////////////////////////////////////////////////////////

class drop_info
{
public:
    drop_info(void);
    drop_info(const drop_info& right);
    ~drop_info(void);

public:
    int     _dragFromId;

    CPoint  _point;
    int     _dropcamera;
    int     _hostcamera;
    int     _countcamera;
    int     _channelext;
    bool    _cameraLevel;
    int     _connectMode;

    struct event_info_t {
        int       _numberSeq;
        int       _type;
        int       _id;
        time_t    _time;
        int       _msec;
        unsigned __int64 _refcameras;
        int       _level;
        G2STRING_256 _label;
        G2SPOT    _spot;
    }
    _eventInfo;

public:
    CString         _sitename;
    G2NETWORK_INFO  _network;
};

//////////////////////////////////////////////////////////////////////////

interface drop_event
{
    virtual void OnDrop(drop_info& di) = 0;
};

//////////////////////////////////////////////////////////////////////////

class drop_target : public CIDropTarget
{
public:
    drop_target(HWND target, drop_event* eventer);

private:
    drop_event* _eventer;

public:
    static bool secureDropInfo(drop_info& di,
                               FORMATETC* format,
                               STGMEDIUM* medium,
                               DWORD* effect,
                               const POINTL& point,
                               LONG_PTR userData);

protected:
    virtual bool OnDrop(FORMATETC* format, STGMEDIUM* medium, DWORD* effect, POINTL point, LONG_PTR userData);
    virtual bool OnDragEnter(FORMATETC* format, STGMEDIUM* medium, DWORD* effect, POINTL point);
    virtual bool OnDragOver(POINTL point);
    virtual void OnDragLeave(void);
};

//////////////////////////////////////////////////////////////////////////

bool insert_screen_drop_property(HWND control, HWND receiver, unsigned int message);
bool remove_screen_drop_property(HWND control);

//////////////////////////////////////////////////////////////////////////

typedef drop_target drop_target;

} // !_namespace_client

#endif // !_CLIENT_DROP_TARGET_H_
