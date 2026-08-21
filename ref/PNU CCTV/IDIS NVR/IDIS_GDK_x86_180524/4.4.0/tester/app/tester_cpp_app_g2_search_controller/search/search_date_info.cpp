// search_date_info.cpp : implementation file
//

#include "stdafx.h"
#include "search_date_info.h"

#include <set>

using namespace client;

//////////////////////////////////////////////////////////////////////////

search_date_info::search_date_info(void)
{

}

search_date_info::~search_date_info(void)
{
    if (empty() == false) clear();
}

//////////////////////////////////////////////////////////////////////////

void search_date_info::add_rectime_info(const G2RECORD_TIME_INFO& rti)
{
    _list.push_back(rti);
}

bool search_date_info::get_date_info(int channelext, search::SEARCH_DATE_INFO_LIST& dates) const
{
    search::SEARCH_DATE_INFO_LIST().swap(dates);
    if (empty()) return false;

    int i = 0;
    size_t j = 0;

    std::set<CTime> buffer;
    CTime time;
    const CTimeSpan span(1, 0, 0, 0);

    // request about tango channel (channelext)
    if (client::valid_channel_ext(channelext)) {
        for (search::RECORD_TIME_INFO_LIST::const_iterator itr(_list.begin());
             itr != _list.end();
             ++itr) {
            time = CTime(g2_time_to_time32_t(&itr->_spot._time));

            for (i = 1; i < itr->_time_size; ++i) {
                for (j = 0; j < itr->_channels._len; ++j) {
                    const G2RECORD_TYPE_INFO::ELEMENT_TYPE& element = itr->_record_type[i]._elements[j];
                    if (element._channelext == channelext &&
                        element._rec_type != 0) {
                        buffer.insert(time);
                    }
                }
                time += span;
            }
        }
    }
    else {
        for (search::RECORD_TIME_INFO_LIST::const_iterator itr(_list.begin());
             itr != _list.end();
             ++itr) {
            time = CTime(g2_time_to_time32_t(&itr->_spot._time));

            for (i = 1; i < itr->_time_size; ++i) {
                for (j = 0; j < itr->_channels._len; ++j) {
                    const G2RECORD_TYPE_INFO::ELEMENT_TYPE& element = itr->_record_type[i]._elements[j];
                    if (element._rec_type != 0) {
                        buffer.insert(time);
                    }
                }
                time += span;
            }
        }
    }

    dates.insert(dates.end(), buffer.begin(), buffer.end());

    return (dates.empty() != true);
}
