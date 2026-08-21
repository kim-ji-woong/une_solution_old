using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace GDK
{
    public struct G2LANGUAGE
    {
        public enum ID
        {
            UNKNOWN = -1,
            ENGLISH = 0,            // English (U.S.)
            GERMAN,                 // German (Standard)
            FRENCH,                 // French (Standard)
            ITALIAN,                // Italian (Standard)
            SPANISH,                // Spanish (Spain, International Sort)
            KOREAN,                 // Korean
            JAPANESE,               // Japanese
            CHINESE_TRADITIONAL,    // Chinese (Taiwan)
            CHINESE_SIMPLIFIED,     // Chinese (RPC)
            DANISH,                 // Danish
            DUTCH,                  // Dutch (Netherlands)
            POLISH,                 // Polish
            SWEDISH,                // Swedish
            PORTUGUESE,             // Portuguese
            CZECH,                  // Czech
            RUSSIAN,                // Russian
            HUNGARIAN,              // Hungarian
            UNITEDKINGDOM,          // English (U.K.)
            FINNISH,                // Finnish (Finland)
            TURKISH,                // Turkish (Turkey)
            CROATIAN,               // Croatian
            COUNT
        }
    }

    public struct G2DATE_TIME_FORMAT
    {
        public enum DATE_TYPE : int
        {
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
        }
        public enum TIME_TYPE : int
        {
            TIME_UNDEFINED = -1,
            H24_MM_SS = 0,
            H12_MM_SS_AP,
            AP_H12_MM_SS
        }
    }

    public struct G2MAIN_VERBOSE
    {
        public enum LEVEL
        {
            QUIET = -1,
            PANIC = 0,
            FATAL,
            ERRORS,
            ASSERTS,
            WARNING,
            INFO,
            VERBOSE,
            DEBUGS
        }
    }
}
