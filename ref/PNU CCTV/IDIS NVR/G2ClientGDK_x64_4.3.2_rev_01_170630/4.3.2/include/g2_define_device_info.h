// g2_define_device_info.h : header file
//

#ifndef _G2_DEFINE_DEVICE_INFO_H_
#define _G2_DEFINE_DEVICE_INFO_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "g2_define.h"
#include "g2_define_product.h"

#ifdef __cplusplus
extern "C" {
#endif

//////////////////////////////////////////////////////////////////////////

struct G2DEVICE_INFO_CALLBACK{
    enum TYPE {
        on_receive_product_info = 0,
        on_failed = 1,
        on_canceled = 2,

        CALLBACK_COUNT = 3
    };
};

//////////////////////////////////////////////////////////////////////////

#ifdef __cplusplus
}
#endif

#endif // !_G2_DEFINE_DEVICE_INFO_H_
