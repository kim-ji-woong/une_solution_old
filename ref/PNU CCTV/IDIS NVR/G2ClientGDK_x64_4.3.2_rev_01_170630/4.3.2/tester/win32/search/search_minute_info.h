// search_minute_info.h : header file
//

#ifndef _SEARCH_MINUTE_INFO_H_
#define _SEARCH_MINUTE_INFO_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "search_common.h"

#include <include/g2_define_play.h>

namespace client {

//////////////////////////////////////////////////////////////////////////

class search_minute_info
{
public:
    search_minute_info(void);
    ~search_minute_info(void);

private:
    std::list<G2RECORD_TIME_INFO> _list;

public:
    void clear(void) { _list.clear(); }
    bool empty(void) const { return _list.empty(); }
    void swap(search_minute_info& right) { _list.swap(right._list); }

    void add_rectime_info(const G2RECORD_TIME_INFO& rti);
    bool get_minute_info(int channelext, search::SEARCH_MINUTE_INFO_LIST& list) const;
    bool get_minute_info_dvr(short camera, search::SEARCH_MINUTE_INFO_LIST& list) const;

    G2SPOT get_spot_first(void) const;
    G2SPOT get_spot_last(void) const;
};

//////////////////////////////////////////////////////////////////////////

} // !_namespace_client

#endif // !_SEARCH_MINUTE_INFO_H_
