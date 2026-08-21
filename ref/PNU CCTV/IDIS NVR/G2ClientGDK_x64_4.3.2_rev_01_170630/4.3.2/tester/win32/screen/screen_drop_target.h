// screen_drop_target.h : header file
//

#ifndef _SCREEN_DROP_TARGET_H_
#define _SCREEN_DROP_TARGET_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "control/drag_drop_impl.h"

namespace client {

//////////////////////////////////////////////////////////////////////////

#pragma pack(push, 4)

struct drop_info_param sealed {
    enum from_type_ {
        from_tree = 0,
        from_screen,
        from_event
    };

    enum level_type_ {
        level_undefined = 0,
        level_camera,
        level_host
    };

    from_type_ _from;
    FORMATETC* _format;
    STGMEDIUM* _medium;
    DWORD*     _effect;
    POINTL     _point;
    LONG_PTR   _userData;

    drop_info_param(void)
        : _from(from_screen)
        , _format(NULL)
        , _medium(NULL)
        , _effect(NULL)
        , _userData(NULL) {
    }

    /////////////////////////////////////////////

    struct secure_info {
        int         _count; // ex. camera count
        bool        _enableDrop;
        bool        _support;
        level_type_ _level;

        secure_info(void)
            : _count(0)
            , _enableDrop(true)
            , _support(true)
            , _level(level_undefined) {
        }

        void reset(void) {
            _count = 0;
            _enableDrop = true;
            _support = true;
            _level = level_undefined;
        }
    }
    _secured;
};

#pragma pack(pop)

//////////////////////////////////////////////////////////////////////////

namespace screen {

interface drop_event_base
{
    virtual bool OnDrop(FORMATETC* format,
                        STGMEDIUM* medium,
                        DWORD* effect,
                        POINTL point,
                        LONG_PTR userData) = 0;

    virtual bool OnDragEnter(FORMATETC* format,
                        STGMEDIUM* medium,
                        DWORD* effect,
                        POINTL point,
                        LONG_PTR userData) = 0;

    virtual bool OnDragOver(const POINTL& point) = 0;
    virtual bool OnDragLeave(void) = 0;
};

//////////////////////////////////////////////////////////////////////////

class screen_drop_target : public CIDropTarget
{
public:
    screen_drop_target(HWND target, drop_event_base* dropEvent);
    virtual ~screen_drop_target(void);

private:
    drop_event_base* _dropEvent;
    drop_info_param::secure_info _secure;

public:
    void set_secure_info(const drop_info_param::secure_info& secure);
    int drag_level(void) const;
    int camera_count(void) const;
    bool is_support(void) const;

protected:
    virtual bool OnDrop(FORMATETC* format, STGMEDIUM* medium, DWORD* effect, POINTL point, LONG_PTR userData);
    virtual bool OnDragEnter(FORMATETC* format, STGMEDIUM* medium, DWORD* effect, POINTL point);
    virtual bool OnDragOver(POINTL point);
    virtual void OnDragLeave(void);
};

//////////////////////////////////////////////////////////////////////////

inline void screen_drop_target::set_secure_info(const drop_info_param::secure_info& secure)
{
    _secure = secure;
}

inline int screen_drop_target::drag_level(void) const
{
    return _secure._level;
}

inline int screen_drop_target::camera_count(void) const
{
    return _secure._count;
}

inline bool screen_drop_target::is_support(void) const
{
    return _secure._support;
}

//////////////////////////////////////////////////////////////////////////

    } // !_namespace_screen
} // !_namespace_client

#endif // !_SCREEN_DROP_TARGET_H_
