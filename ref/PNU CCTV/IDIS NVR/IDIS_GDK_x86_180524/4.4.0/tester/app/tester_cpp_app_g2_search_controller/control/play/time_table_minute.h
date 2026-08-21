// time_table_minute.h : header file
//

#ifndef _TIME_TABLE_MINUTE_H_
#define _TIME_TABLE_MINUTE_H_

#include "time_table_base.h"
#include "search/search_common.h"
#include "search/search_data.h"
#include "search/search_minute_info.h"

#include <map>

namespace client {

//////////////////////////////////////////////////////////////////////////

class time_table_minute : public time_table_base
{
public:
    time_table_minute(void);
    virtual ~time_table_minute(void);

    class time_data {
        struct hour_data {
            hour_data(void) 
                : _index(-1)
                , _segment(-1) {
            }
            hour_data(int hour, int i, int segment) 
                : _index(i)
                , _segment(segment) {
            }
            
            int _index;
            CTime _time;
            int _segment;
        };

    public:
        time_data(void) {
            _channelext = -1;
        }

        time_data(int hours) {
            reset(hours);
        }

        void reset(int hours) {
            _channelext = -1;
            std::vector<hour_data>(hours).swap(_hours);
            std::vector<search::SEARCH_MINUTE_INFO>(hours * 60).swap(_minutes);
        }

    public:
        int _channelext;
        std::vector<hour_data> _hours;
        std::vector<search::SEARCH_MINUTE_INFO> _minutes;
    };

    typedef std::set<CTime> hour_list;
    typedef std::map<int, hour_list> hour_map;
    typedef std::map<int, time_data> minute_map;

    hour_map _hour_map;
    minute_map _min_map;

    int _hour_count;
    int _hour_boundary;

    g2::critical_section _lock;

public:
    void set_data(search_data_ptr data, const std::set<int>& cameras);
    void update(search_data_ptr data, const std::set<int>& cameras);

    virtual void screen_image_drew(const G2SPOT& spot);

protected:
    bool find_hour_index_by_segment_time(int segment, const CTime& time, int& index);
    bool find_pos_by_spot(const G2SPOT& spot, int& pos);

    virtual void draw_time(CDC* pDC);
    virtual void draw_time_bar(CDC* pDC);
    virtual bool set_select_pos(int pos, bool request = true);
    virtual bool color_rec_time_info(int type, COLORREF& color);
};

//////////////////////////////////////////////////////////////////////////

} // !_namespace_client

#endif // !_TIME_TABLE_MINUTE_H_
