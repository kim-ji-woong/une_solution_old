// search_common.h : header file
//

#ifndef _SEARCH_COMMON_H_
#define _SEARCH_COMMON_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include <include/g2_define_play.h>
#include <vector>
#include <list>

namespace client {
    namespace search {

//////////////////////////////////////////////////////////////////////////

enum SEARCH_MODE {
    SEARCH_INVALID = -1,
    SEARCH_LOCAL = 0,           // dvr
    SEARCH_LOCAL_G2,            // dvr or nvr local search based on g2search
    SEARCH_FORCE_LIMIT
};

enum TIMELAPSE_MODE {
    TIME_MODE_UNDEFINED = -1,
    TIME_MODE_HOUR = 0,
    TIME_MODE_MINUTE,
    TIME_MODE_MINUTE_IDR,
    TIME_MODE_COUNT
};

struct SEARCH_MINUTE_INFO {
    CTime _time;
    int _segment;
    int _tick;
    int _rec_type;

    SEARCH_MINUTE_INFO(void)
        : _segment(0)
        , _tick(0)
        , _rec_type(0) {
    }

    SEARCH_MINUTE_INFO(const CTime& time, int segment, int tick, int rec_type)
        : _time(time)
        , _segment(segment)
        , _tick(tick)
        , _rec_type(rec_type) {
    }
};

struct ELEMENT_HOUR {
    char _hour[24];
};

struct ELEMENT_MINUTE {
    unsigned char _minute[24 * 60];
};

typedef std::vector<CTime> SEARCH_DATE_INFO_LIST;
typedef std::vector<ELEMENT_HOUR> SEARCH_HOUR_INFO_DVR_LIST;
typedef std::vector<ELEMENT_MINUTE> SEARCH_MINUTE_INFO_DVR_LIST;

typedef std::list<G2RECORD_TIME_INFO> RECORD_TIME_INFO_LIST;
typedef std::vector<SEARCH_MINUTE_INFO> SEARCH_MINUTE_INFO_LIST;

//////////////////////////////////////////////////////////////////////////

    } // !_namespace_search
} // !_namespace_client

#endif // !_SEARCH_COMMON_H_
