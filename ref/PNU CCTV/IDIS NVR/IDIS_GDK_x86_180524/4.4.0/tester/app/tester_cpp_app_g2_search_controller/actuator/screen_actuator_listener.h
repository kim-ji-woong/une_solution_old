// ScreenListener.h : header file
//

#ifndef _SCREEN_ACTUATOR_LISTENER_H_
#define _SCREEN_ACTUATOR_LISTENER_H_

#include "screen_common.h"
#include <vector>

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

namespace client {

//////////////////////////////////////////////////////////////////////////

class screen_actuator_listener
{
public:
    screen_actuator_listener(void)
    {

    }

    virtual ~screen_actuator_listener(void)
    {

    }

protected:
    virtual void on_screen_image_loaded(const G2FRAME& frame) = 0;
    virtual void on_screen_no_image_loaded(short channel, short camera) = 0;

private:
    friend class screen_actuator;
};

//////////////////////////////////////////////////////////////////////////

} // !_namespace_client

#endif // !_SCREEN_ACTUATOR_LISTENER_H_
