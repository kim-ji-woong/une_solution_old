// time_table_minute.cpp : implementation file
//

#include "stdafx.h"
#include "time_table_minute.h"
#include "common/client_functional.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

using namespace client;

//////////////////////////////////////////////////////////////////////////

time_table_minute::time_table_minute(void)
{

}

time_table_minute::~time_table_minute(void)
{
    time_table_base::finalize();
}

//////////////////////////////////////////////////////////////////////////

void time_table_minute::set_data(search_data_ptr data, const std::set<int>& cameras)
{
    g2::scoped_criticalsection lock(_lock_data);

    const search_minute_info* info = data->get_minute_info();
    G2SPOT& spot = data->spot_selected();
    CTime time = g2_time_to_time32_t(&spot._time);

    reset_data_list(cameras.size());

    int index = 0;
    for (std::set<int>::const_iterator itr = cameras.begin();
        itr != cameras.end();
        ++itr, ++index) {
        int channelext = *itr;
        table_data& datas = data_list().at(index);

        search::SEARCH_MINUTE_INFO_LIST list;
        if (info && info->get_minute_info(channelext, list)) {
            datas.resize(24 * 60, channelext);
            
            for (unsigned int i = 0; i < list.size(); ++i) {
                const search::SEARCH_MINUTE_INFO& rti = list.at(i);
                if (client::is_same_date(time, rti._time)) {
                    int pos = rti._time.GetHour() * 60 + rti._time.GetMinute();

                    time_data& data = datas._data.at(pos);
                    data._pos = pos;
                    data._time = rti._time;
                    data._segment = rti._segment;
                    data._tick = rti._tick;
                    data._rec_type = rti._rec_type;
                }
            }
        }
    }
}

void time_table_minute::update(search_data_ptr data, const std::set<int>& cameras)
{
    G2RETURN_IF_FAIL(GetSafeHwnd());

    set_data(data, cameras);
    _enable = true;

    int height = (data_list().size() * default_::TIMETABLE_ROW_HEIGHT) + default_::TIMETABLE_HEAD_HEIGHT + default_::TIMETABLE_TIME_BOUNDARY_HEIGHT;
    int width = __max((int)time_list().size(), _rect.Width());

    initialize_surface(width, height);

    if (_selectPos == -1) {
        for (time_data_list::const_reverse_iterator itr = time_list().rbegin();
            itr != time_list().rend();
            ++itr) {
            if (is_valid_rec_type(itr->_rec_type)) {
                set_select_pos(itr->_pos, false);
                break;
            }
        }
    }
    else {
        int pos = find_pos_by_spot(data->spot_standard());
        if (pos > 0) {
            set_select_pos(pos, false);
        }
        else {
            set_select_pos(0, false);
        }
    }

    Invalidate();
}

//////////////////////////////////////////////////////////////////////////

bool time_table_minute::is_valid_rec_type(int type)
{
    return (type & G2FRAME::PANIC ||
            type & G2FRAME::PRE_EVENT ||
            type & G2FRAME::EVENT ||
            type & G2FRAME::TIME_LAPSE ||
            type & G2FRAME::IRREGULAR);
}

bool time_table_minute::get_rec_type_color(int type, COLORREF& color)
{
    bool retv = true;

    if (type & G2FRAME::PANIC) {
        color = RGB(41, 144, 229);
    }
    else if (type & G2FRAME::PRE_EVENT) {
        color = RGB(91, 41, 229);
    }
    else if (type & G2FRAME::EVENT) {
        color = RGB(203, 41, 229);
    }
    else if (type & G2FRAME::TIME_LAPSE) {
        color = RGB(50, 67, 89);
    }
    else if (type & G2FRAME::IRREGULAR) {
        color = RGB(222, 107, 4);
    }
    else {
        retv = false;
    }

    return retv;
}
