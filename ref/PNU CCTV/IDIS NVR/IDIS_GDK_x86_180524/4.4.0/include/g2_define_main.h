// g2_define_main.h : header file
//

#ifndef _G2_DEFINE_MAIN_H_
#define _G2_DEFINE_MAIN_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "g2_define.h"

#ifdef __cplusplus
extern "C" {
#endif

//////////////////////////////////////////////////////////////////////////

struct G2LANGUAGE_ID {
    enum ID {
        UNKNOWN = -1,
        ENGLISH = 0,         // English (U.S.)
        GERMAN,              // German (Standard)
        FRENCH,              // French (Standard)
        ITALIAN,             // Italian (Standard)
        SPANISH,             // Spanish (Spain, International Sort)
        KOREAN,              // Korean
        JAPANESE,            // Japanese
        CHINESE_TRADITIONAL, // Chinese (Taiwan)
        CHINESE_SIMPLIFIED,  // Chinese (RPC)
        DANISH,              // Danish
        DUTCH,               // Dutch (Netherlands)
        POLISH,              // Polish
        SWEDISH,             // Swedish
        PORTUGUESE,          // Portuguese
        CZECH,               // Czech
        RUSSIAN,             // Russian
        HUNGARIAN,           // Hungarian
        UNITEDKINGDOM,       // English (U.K.)
        FINNISH,             // Finnish (Finland)
        TURKISH,             // Turkish (Turkey)
        CROATIAN,            // Croatian

        ID_COUNT,
    };
};

struct G2DATE_TIME_FORMAT {
    enum DATE_TYPE {
        DATE_UNDEFINED = -1,
        MM_DD_YYYY_HYPHEN = 0,
        DD_MM_YYYY_HYPHEN,
        YYYY_MM_DD_HYPHEN,
        MM_DD_YYYY_SLASH,
        DD_MM_YYYY_SLASH,
        YYYY_MM_DD_SLASH,
        MM_DD_YYYY_DOT,
        DD_MM_YYYY_DOT,
        YYYY_MM_DD_DOT
    };
    enum TIME_TYPE {
        TIME_UNDEFINED = -1,
        H24_MM_SS = 0,
        H12_MM_SS_AP,
        AP_H12_MM_SS
    };
};

struct G2MAIN_VERBOSE {
    enum LEVEL {
        QUIET = -1,
        PANIC = 0,
        FATAL,
        ERRORS,
        ASSERTS,
        WARNING,
        INFO,
        VERBOSE,
        DEBUGS
    };
};

typedef void (G2CALLBACK* G2VERBOSE_CALLBACK)(G2MAIN_VERBOSE::LEVEL level, const wchar_t* string);

//////////////////////////////////////////////////////////////////////////

#ifdef __cplusplus
}
#endif

#endif // !_G2_DEFINE_MAIN_H_
