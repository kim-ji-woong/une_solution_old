// date_info.h : header file
//

#ifndef _SEARCH_HOUR_INFO_H_
#define _SEARCH_HOUR_INFO_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "search_common.h"
#include <include/g2_define_play.h>

namespace client {

//////////////////////////////////////////////////////////////////////////

class search_hour_info
{
public:
    search_hour_info(void);
    ~search_hour_info(void);

private:
    unsigned char _hourInfo[24];

public:
    unsigned char* hour_info(void) { return _hourInfo; }
    void set_hour_info(unsigned char* info) { memcpy(_hourInfo, info, sizeof(unsigned char) * 24); }
    bool valid_index(unsigned char index)   { return (index >= 0 && index < 24); }
    unsigned char operator[](unsigned char index);
};

inline unsigned char hour_search_info::operator [](unsigned char index)
{
    assert(valid_index(index));
    return valid_index(index) ? _hourInfo[index] : UCHAR_MAX;
}

//////////////////////////////////////////////////////////////////////////

} // !_namespace_client

#endif // !_SEARCH_HOUR_INFO_H_
