// g2_define_burner.h : header file
//

#ifndef _G2_DEFINE_BURNER_H_
#define _G2_DEFINE_BURNER_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "g2_define.h"

#ifdef __cplusplus
extern "C" {
#endif

//////////////////////////////////////////////////////////////////////////

struct G2BURNER_CALLBACK {
    enum {
        IDLECALLBACK = 0,
        USERDIALOG,
        PROGRESSCALLBACK,
        ABORTEDCALLBACK,
        ADDLOGLINE,
        SETPHASECALLBACK,
        SETMAJORPHASECALLBACK,
        PROGRESSCALLBACKERASE,

        CALLBACK_COUNT
    };
};

struct G2BURNER_RESULT {
    enum TYPE {
        OK                                 =  0,
        FAILED                             =  3,
        BURN_OK                            =  0,
        BURN_UNKNOWN_CD_FORMAT             =  1,
        BURN_INVALID_DRIVE                 =  2,
        BURN_FAILED                        =  3,
        BURN_FUNCTION_NOT_ALLOWED          =  4,
        BURN_DRIVE_NOT_ALLOWED             =  5,
        BURN_USER_ABORT                    =  6,
        BURN_BAD_MESSAGE_FILE              =  7,
        DEVICE_GENERIC_ERROR               = -1,
        DEVICE_DRIVE_IN_USE                = -2,
        DEVICE_DRIVE_NOT_READY             = -3,
        DEVICE_NO_DRIVE                    = -4,
        DEVICE_DISC_NOT_PRESENT            = -5,
        DEVICE_DISC_NOT_PRESENT_TRAY_OPEN  = -6,
        DEVICE_DISC_NOT_PRESENT_TRAY_CLOSED= -7,
        DEVICE_DISC_NOT_ENOUGHT_FREE_SPACE = -8,
    };
};

struct G2BURNER_MEDIA_TYPE {
    enum TYPE {
        BURN_TYPE_UNKOWN	= 0,
        BURN_TYPE_CDROM	= 0x1,
        BURN_TYPE_CDR	= 0x2,
        BURN_TYPE_CDRW	= 0x3,
        BURN_TYPE_DVDROM	= 0x4,
        BURN_TYPE_DVDRAM	= 0x5,
        BURN_TYPE_DVDPLUSR	= 0x6,
        BURN_TYPE_DVDPLUSRW	= 0x7,
        BURN_TYPE_DVDPLUSR_DUALLAYER	= 0x8,
        BURN_TYPE_DVDDASHR	= 0x9,
        BURN_TYPE_DVDDASHRW	= 0xa,
        BURN_TYPE_DVDDASHR_DUALLAYER	= 0xb,
        BURN_TYPE_DISK	= 0xc,
        BURN_TYPE_DVDPLUSRW_DUALLAYER	= 0xd,
        BURN_TYPE_HDDVDROM	= 0xe,
        BURN_TYPE_HDDVDR	= 0xf,
        BURN_TYPE_HDDVDRAM	= 0x10,
        BURN_TYPE_BDROM	= 0x11,
        BURN_TYPE_BDR	= 0x12,
        BURN_TYPE_BDRE	= 0x13,
        BURN_TYPE_MAX	= 0x13
    };
};

struct G2BURNER_MAJOR_PHASE {
    enum TYPE {
        UNSPECIFIED                   = -1,
        START_CACHE                   = 24,
        DONE_CACHE                    = 25,
        FAIL_CACHE                    = 26,
        ABORT_CACHE                   = 27,
        START_TEST                    = 28,
        DONE_TEST                     = 29,
        FAIL_TEST                     = 30,
        ABORT_TEST                    = 31,
        START_SIMULATE                = 32,
        DONE_SIMULATE                 = 33,
        FAIL_SIMULATE                 = 34,
        ABORT_SIMULATE                = 35,
        START_WRITE                   = 36,
        DONE_WRITE                    = 37,
        FAIL_WRITE                    = 38,
        ABORT_WRITE                   = 39,
        START_SIMULATE_NOSPD          = 61,
        DONE_SIMULATE_NOSPD           = 62,
        FAIL_SIMULATE_NOSPD           = 63,
        ABORT_SIMULATE_NOSPD          = 64,
        START_WRITE_NOSPD             = 65,
        DONE_WRITE_NOSPD              = 66,
        FAIL_WRITE_NOSPD              = 67,
        ABORT_WRITE_NOSPD             = 68,
        PREPARE_ITEMS                 = 73,
        VERIFY_COMPILATION            = 78,
        VERIFY_ABORTED                = 79,
        VERIFY_END_OK                 = 80,
        VERIFY_END_FAIL               = 81,
        ENCODE_VIDEO                  = 82,
        SEAMLESSLINK_ACTIVATED        = 87,
        BUP_ACTIVATED                 = 90,
        START_FORMATTING              = 98,
        CONTINUE_FORMATTING           = 99,
        FORMATTING_SUCCESSFUL         = 100,
        FORMATTING_FAILED             = 101,
        PREPARE_CD                    = 105,
        DONE_PREPARE_CD               = 106,
        FAIL_PREPARE_CD               = 107,
        ABORT_PREPARE_CD              = 108,
        DVDVIDEO_DETECTED             = 111,
        DVDVIDEO_REALLOC_STARTED      = 112,
        DVDVIDEO_REALLOC_COMPLETED    = 113,
        DVDVIDEO_REALLOC_NOTNEEDED    = 114,
        DVDVIDEO_REALLOC_FAILED       = 115,
        DRM_CHECK_FAILURE             = 169,
        START_WRITE_DATAFILES         = 171,
        DONE_WRITE_DATAFILES          = 172,
        FAIL_WRITE_DATAFILES          = 173,
        WARN_WRITE_DATAFILES          = 174,
        START_BAO_FINALIZE            = 175,
        FAIL_BAO_FINALIZE             = 176,
        DONE_BAO_FINALIZE             = 177,
        FAIL_BAO_PREPARE              = 178,
        FAIL_BAO_WRITEFILE            = 179,
        BURN_LAYER_1                  = 180,
        BURN_LAYER_2                  = 181,
    };
};

//////////////////////////////////////////////////////////////////////////

#ifdef __cplusplus
}
#endif

#endif // !_G2_DEFINE_BURNER_H_
