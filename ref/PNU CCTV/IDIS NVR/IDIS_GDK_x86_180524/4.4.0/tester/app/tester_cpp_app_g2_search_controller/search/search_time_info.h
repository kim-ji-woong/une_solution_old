// search_time_info.h : header file
//

#ifndef _SEARCH_TIME_INFO_H_
#define _SEARCH_TIME_INFO_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "search_common.h"

#include <include/g2_define_play.h>

namespace client {

//////////////////////////////////////////////////////////////////////////

class search_time_info
{
public:
    search_time_info(void);
    ~search_time_info(void);

private:
    search::RECORD_TIME_INFO_LIST _list;

public:
    void clear(void) { search::RECORD_TIME_INFO_LIST().swap(_list); }
    bool empty(void) const { return _list.empty(); }
    void swap(search_time_info& right) { _list.swap(right._list); }

    void add_rectime_info(const G2RECORD_TIME_INFO& rti);
    bool get_time_info(int channelext, search::SEARCH_MINUTE_INFO_LIST& list);

    G2SPOT get_spot_first(void) const;
    G2SPOT get_spot_last(void) const;
};

//////////////////////////////////////////////////////////////////////////

} // !_namespace_client

#endif // !_SEARCH_TIME_INFO_H_
