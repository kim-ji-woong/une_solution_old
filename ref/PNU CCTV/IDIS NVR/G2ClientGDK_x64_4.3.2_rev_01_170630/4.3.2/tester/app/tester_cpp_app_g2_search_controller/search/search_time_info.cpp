// search_data.cpp : implementation file
//

#include "stdafx.h"
#include "search_time_info.h"

using namespace client;

//////////////////////////////////////////////////////////////////////////

search_time_info::search_time_info(void)
{

}

search_time_info::~search_time_info(void)
{

}

//////////////////////////////////////////////////////////////////////////

void search_time_info::add_rectime_info(const G2RECORD_TIME_INFO& rti)
{
    search::RECORD_TIME_INFO_LIST::iterator itr;
    for (itr = _list.begin(); itr != _list.end(); ++itr) {
        const G2RECORD_TIME_INFO& info = *itr;
        if (g2_spot_compare(&rti._spot, &info._spot) < 0) {
            break;
        }
    }
    _list.insert(itr, rti);
}

bool search_time_info::get_time_info(int channelext, search::SEARCH_MINUTE_INFO_LIST& list)
{
    G2RETURN_VAL_IF_FAIL(client::valid_channel_ext(channelext), false);

    search::SEARCH_MINUTE_INFO_LIST().swap(list);

    int i = 0;
    size_t j = 0;
    CTime time;
    int segment;
    int tick;
    int rec_type;

    for (search::RECORD_TIME_INFO_LIST::const_iterator itr(_list.begin());
         itr != _list.end();
         ++itr) {
        time = CTime((time_t)itr->_spot._time._time);
        segment = itr->_spot._segment;
        tick = itr->_spot._tick;

        for (i = 0; i < itr->_time_size; ++i) {
            rec_type = 0;
            for (j = 0; j < itr->_channels._len; ++j) {
                const G2RECORD_TYPE_INFO::ELEMENT_TYPE& element = itr->_record_type[i]._elements[j];
                if (element._channelext == channelext) {
                    rec_type = element._rec_type;
                    break;
                }
            }
            list.push_back(search::SEARCH_MINUTE_INFO(time, segment, tick, rec_type));
            time += CTimeSpan(0, 0, 1, 0);
        }
    }

    return true;
}

G2SPOT search_time_info::get_spot_first(void) const
{
    if (_list.empty()) {
        return G2SPOT();
    }

    return (*_list.begin())._spot;
}

G2SPOT search_time_info::get_spot_last(void) const
{
    if (_list.empty()) {
        return G2SPOT();
    }

    return (*_list.rbegin())._spot;
}
