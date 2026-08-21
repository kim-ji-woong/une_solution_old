// time_table_minute_idr.cpp : implementation file
//

#include "stdafx.h"
#include "time_table_minute_idr.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

using namespace client;

//////////////////////////////////////////////////////////////////////////

time_table_minute_idr::time_table_minute_idr(void)
{

}

time_table_minute_idr::~time_table_minute_idr(void)
{
    time_table_base::finalize();
}

//////////////////////////////////////////////////////////////////////////

void time_table_minute_idr::set_data(search_data_ptr data, const std::set<int>& cameras)
{
    g2::scoped_criticalsection lock(_lock_data);

    const search::SEARCH_MINUTE_INFO_DVR_LIST& info = data->get_minute_info_idr();
    reset_data_list(cameras.size());

    const G2SPOT& spot = data->spot_selected();
    CTime time = g2_time_to_time32_t(&spot._time);
    int segment = spot._segment;
    int tick = spot._tick;
    
    if (info.empty() != true) {
        int index = 0;
        for (std::set<int>::const_iterator itr = cameras.begin();
            itr != cameras.end();
            ++itr, ++index) {
            table_data& datas = data_list().at(index);
            if (*itr >=0 && (unsigned)*itr < info.size()) {
                datas.resize(_default::MIN_COUNT, *itr);
                const search::ELEMENT_MINUTE& minute = info.at(*itr);

                for (int i = 0; i < _default::MIN_COUNT; ++i) {
                    time_data& data = datas._data.at(i);
                    data._pos = i;
                    data._time = CTime(time.GetYear(), time.GetMonth(), time.GetDay(), i/60, i%60, 0);
                    data._segment = segment;
                    data._tick = tick;
                    data._rec_type = minute._minute[i];
                }
            }
        }
    }
}

void time_table_minute_idr::update(search_data_ptr data, const std::set<int>& cameras)
{
    G2RETURN_IF_FAIL(GetSafeHwnd());

    set_data(data, cameras);
    _enable = true;

    int begin_time = 0, end_time = 0, time_count = 0;
    if (time_list().size() > 0) {
        //begin_time = time_list().front()._time.GetHour() * 60;
        //end_time = (23 - time_list().back()._time.GetHour()) * 60;
        time_count = time_list().size();
    }

    int height = (data_list().size() * default_::TIMETABLE_ROW_HEIGHT) + default_::TIMETABLE_HEAD_HEIGHT + default_::TIMETABLE_TIME_BOUNDARY_HEIGHT;
    int width = __max(begin_time + time_count + end_time, _rect.Width());

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

bool time_table_minute_idr::is_valid_rec_type(int type)
{
    return (type & G2FRAME::PANIC ||
            type & G2FRAME::PRE_EVENT ||
            type & G2FRAME::EVENT ||
            type & G2FRAME::TIME_LAPSE ||
            type & G2FRAME::IRREGULAR);
}

bool time_table_minute_idr::get_rec_type_color(int type, COLORREF& color)
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
