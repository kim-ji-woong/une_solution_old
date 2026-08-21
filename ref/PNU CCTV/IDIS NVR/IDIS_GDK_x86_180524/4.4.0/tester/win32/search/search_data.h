// search_data.h : header file
//

#ifndef _SEARCH_DATA_H_
#define _SEARCH_DATA_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "search_common.h"
#include "search_date_info.h"

#include <boost/shared_ptr.hpp>
#include <boost/noncopyable.hpp>

namespace client {
    class search_minute_info;

//////////////////////////////////////////////////////////////////////////

class search_data : private boost::noncopyable
{
public:
    search_data(short mode, short channel = client::invalid_::CHANNEL);
    virtual ~search_data(void);

protected:
    short _mode;
    short _channel;
    short _segmentId;

    G2SPOT _spotStandard;
    G2SPOT _spotSelected;
    G2SPOT _spotGoto;

    bool _reserveReload;

    g2::critical_section _syncData;

protected:
    // local data
    search::SEARCH_DATE_INFO_LIST _dateInfoDvr;
    search::SEARCH_HOUR_INFO_DVR_LIST _hourInfoDvr;
    search::SEARCH_MINUTE_INFO_DVR_LIST _minuteInfoIdr;

    // local G2 data
    search_date_info _dateInfo;
    search_minute_info* _minuteInfo;

public:
    void new_minute_info(void);

public:
    void clear_info_date(void);
    void clear_info_hour(void);
    void clear_info_minute(void);
    void clear_info_minute_idr(void);

    void cleanup(void);

public:
    void set_mode(short mode) { _mode = mode; }
    void set_channel(short channel) { _channel = channel; }
    void set_segment_id(short segmentId) { _segmentId = segmentId; }
    void set_spot_standard(const G2SPOT& spot) { _spotStandard = spot; }
    void set_spot_selected(const G2SPOT& spot) { _spotSelected = spot; }
    void set_spot_goto(const G2SPOT& spot) { _spotGoto = spot; }
    void set_reserve_reload(bool reload) { _reserveReload = reload; }

    void set_date_info(const G2RECORD_TIME_INFO& rti);
    void set_minute_info(const G2RECORD_TIME_INFO& rti);
    void set_date_info_dvr(const std::vector<G2TIME>& dates);
    void set_hour_info_dvr(const bool hour[][24], int segcount);
    void set_minute_info_idr(const unsigned char minute[][24 * 60]);

    void prepare_reload(void);

    short mode(void) const { return _mode; }
    short channel(void) const { return _channel; }
    short segment_id(void) const { return _segmentId; }
    G2SPOT spot_standard(void) const { return _spotStandard; }
    G2SPOT spot_selected(void) const { return _spotSelected; }
    G2SPOT spot_goto(void) const { return _spotGoto; }

    bool is_reserve_reload(void) const { return _reserveReload; }
    bool is_mode_search(void) const { return _mode == search::SEARCH_LOCAL; }

    bool get_date_info(int channelext, search::SEARCH_DATE_INFO_LIST& list) const;
    bool get_minute_info_idr(int channelext, search::ELEMENT_MINUTE& info) const;
    bool get_date_info_dvr(search::SEARCH_DATE_INFO_LIST& list) const;
    bool get_hour_info_dvr(search::SEARCH_HOUR_INFO_DVR_LIST& list) const;
    bool get_hour_info_dvr(search::ELEMENT_HOUR& element, int segmentId) const;

    const search::SEARCH_HOUR_INFO_DVR_LIST& get_hour_info_dvr(void) const;
    const search::SEARCH_MINUTE_INFO_DVR_LIST& get_minute_info_idr(void) const;
    const search_minute_info* get_minute_info(void) const;

    G2SPOT spot_minute_first(void);
    G2SPOT spot_minute_last(void);

    int count_segment(void) const;

    // for atomic ///////////////////////////////

    bool try_lock(void) const { return _syncData.trylock(); }
    void lock(void)     const { _syncData.lock();   }
    void unlock(void)   const { _syncData.unlock(); }
    void enter(void)    const { _syncData.enter();  }
    void leave(void)    const { _syncData.leave();  }
};

typedef boost::shared_ptr<search_data> search_data_ptr;

//////////////////////////////////////////////////////////////////////////

} // !_namespace_client

#endif // !_SEARCH_DATA_H_
