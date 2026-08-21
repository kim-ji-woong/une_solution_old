// g2_define_temple.h : header file
//

#ifndef _G2_DEFINE_TEMPLE_H_
#define _G2_DEFINE_TEMPLE_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "g2_define.h"

#ifdef __cplusplus
extern "C" {
#endif

//////////////////////////////////////////////////////////////////////////
    
struct G2PUMP_MESSAGE {
    enum FLAGS {
        G2_PM_NOREMOVE = 0,
        G2_PM_REMOVE = 1,
        G2_PM_NOYIELD = 2,
        G2_PM_QS_INPUT = 67567616,
        G2_PM_QS_POSTMESSAGE = 9961472,
        G2_PM_QS_PAINT = 2097152,
        G2_PM_QS_SENDMESSAGE = 4194304
    };
};

//////////////////////////////////////////////////////////////////////////

#ifdef __cplusplus
}
#endif

#endif // !_G2_DEFINE_TEMPLE_H_
