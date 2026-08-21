// search_data.cpp : implementation file
//

#include "stdafx.h"
#include "search_data.h"
#include "search_minute_info.h"

using namespace client;

//////////////////////////////////////////////////////////////////////////

search_data::search_data(short mode, short channel /*= client::invalid_::CHANNEL*/)
    : _mode(mode)
    , _channel(channel)
    , _segmentId(0)
    , _reserveReload(false)
    , _minuteInfo(NULL)
{
    g2_spot_make_invalid(&_spotStandard);
    g2_spot_make_invalid(&_spotSelected);
    g2_spot_make_invalid(&_spotGoto);
}

search_data::~search_data(void)
{
    cleanup();
    SAFE_DELETE(_minuteInfo);
}

//////////////////////////////////////////////////////////////////////////

void search_data::new_minute_info(void)
{
    g2::scoped_criticalsection lock(_syncData);

    SAFE_DELETE(_minuteInfo);
    _minuteInfo = new search_minute_info();
}

void search_data::clear_info_date(void)
{
    g2::scoped_criticalsection lock(_syncData);

    _dateInfo.clear();
    search::SEARCH_DATE_INFO_LIST().swap(_dateInfoDvr);
}

void search_data::clear_info_hour(void)
{
    g2::scoped_criticalsection lock(_syncData);

    search::SEARCH_HOUR_INFO_DVR_LIST().swap(_hourInfoDvr);
}

void search_data::clear_info_minute(void)
{
    G2RETURN_IF_FAIL(_minuteInfo != NULL);

    g2::scoped_criticalsection lock(_syncData);

    _minuteInfo->clear();
}

void search_data::clear_info_minute_idr(void)
{
    g2::scoped_criticalsection lock(_syncData);

    search::SEARCH_MINUTE_INFO_DVR_LIST().swap(_minuteInfoIdr);
}

void search_data::cleanup(void)
{
    g2::scoped_criticalsection lock(_syncData);

    clear_info_minute_idr();
    clear_info_minute();
    clear_info_hour();
    clear_info_date();
}

void search_data::set_date_info(const G2RECORD_TIME_INFO& rti)
{
    g2::scoped_criticalsection lock(_syncData);

    _dateInfo.add_rectime_info(rti);
}

void search_data::set_minute_info(const G2RECORD_TIME_INFO& rti)
{
    g2::scoped_criticalsection lock(_syncData);

    if (_minuteInfo == NULL) {
        new_minute_info();
    }

    _minuteInfo->add_rectime_info(rti);
}

void search_data::set_date_info_dvr(const std::vector<G2TIME>& dates)
{
    g2::scoped_criticalsection lock(_syncData);

    for (std::vector<G2TIME>::const_iterator itr(dates.begin());
        itr != dates.end();
        ++itr) {
        const G2TIME& time = *itr;
        _dateInfoDvr.push_back(CTime(time._time));
    }
}

void search_data::set_hour_info_dvr(const bool hour[][24], int segcount)
{
    g2::scoped_criticalsection lock(_syncData);

    set_segment_id(0);
    _hourInfoDvr.resize(segcount);

    for (int i = 0; i < segcount; ++i) {
        memcpy(&_hourInfoDvr[i]._hour, hour[i], 24);
    }
}

void search_data::set_minute_info_idr(const unsigned char minute[][24 * 60])
{
    g2::scoped_criticalsection lock(_syncData);

    if (_minuteInfoIdr.empty()) {
        _minuteInfoIdr.resize(32);
    }

    memcpy(&_minuteInfoIdr[0], minute, 32 * sizeof(search::ELEMENT_MINUTE));
}

void search_data::prepare_reload(void)
{
    g2::scoped_criticalsection lock(_syncData);

    clear_info_date();
    clear_info_minute();

    g2_spot_make_invalid(&_spotStandard);
    g2_spot_make_invalid(&_spotSelected);
    g2_spot_make_invalid(&_spotGoto);
}

bool search_data::get_date_info(int channelext, search::SEARCH_DATE_INFO_LIST& list) const
{
    g2::scoped_criticalsection lock(_syncData);

    return _dateInfo.get_date_info(channelext, list);
}

bool search_data::get_minute_info_idr(int channelext, search::ELEMENT_MINUTE& info) const
{
    g2::scoped_criticalsection lock(_syncData);

    bool result = false;
    if (channelext >= 0 &&
        channelext < static_cast<int>(_minuteInfoIdr.size())) {
        info = _minuteInfoIdr[channelext];
        result = true;
    }
    return result;
}

bool search_data::get_date_info_dvr(search::SEARCH_DATE_INFO_LIST& list) const 
{
    g2::scoped_criticalsection lock(_syncData);

    list = _dateInfoDvr;
    return (list.empty() != true);
}

bool search_data::get_hour_info_dvr(search::SEARCH_HOUR_INFO_DVR_LIST& list) const
{
    g2::scoped_criticalsection lock(_syncData);

    list = _hourInfoDvr;
    return (list.empty() != true);
}

bool search_data::get_hour_info_dvr(search::ELEMENT_HOUR& element, int segmentId) const
{
    g2::scoped_criticalsection lock(_syncData);

    bool result = false;
    if (segmentId >= 0 &&
        segmentId < static_cast<int>(_hourInfoDvr.size())) {
        element = _hourInfoDvr[segmentId];
        result = true;
    }
    return result;
}

const search::SEARCH_HOUR_INFO_DVR_LIST& search_data::get_hour_info_dvr(void) const
{
    return _hourInfoDvr;
}

const search::SEARCH_MINUTE_INFO_DVR_LIST& search_data::get_minute_info_idr(void) const
{
    return _minuteInfoIdr;
}

const search_minute_info* search_data::get_minute_info(void) const
{
    g2::scoped_criticalsection lock(_syncData);

    return _minuteInfo;
}

G2SPOT search_data::spot_minute_first(void)
{
    g2::scoped_criticalsection lock(_syncData);

    return _minuteInfo->get_spot_first();
}

G2SPOT search_data::spot_minute_last(void)
{
    g2::scoped_criticalsection lock(_syncData);

    return _minuteInfo->get_spot_last();
}

int search_data::count_segment(void) const
{
    g2::scoped_criticalsection lock(_syncData);

    return static_cast<int>(_hourInfoDvr.size());
}
