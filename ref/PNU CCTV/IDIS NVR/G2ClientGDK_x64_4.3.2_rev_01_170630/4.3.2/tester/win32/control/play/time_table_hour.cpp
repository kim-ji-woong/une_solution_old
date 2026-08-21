// time_table_hour.cpp : implementation file
//

#include "stdafx.h"
#include "time_table_hour.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

using namespace client;

//////////////////////////////////////////////////////////////////////////

time_table_hour::time_table_hour(void)
{

}

time_table_hour::~time_table_hour(void)
{

}

//////////////////////////////////////////////////////////////////////////

void time_table_hour::set_data(search_data_ptr data, const std::set<int>& cameras)
{
    g2::scoped_criticalsection lock(_lock_data);

    const search::SEARCH_HOUR_INFO_DVR_LIST& info = data->get_hour_info_dvr();
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
            if (*itr >= 0 && (unsigned)*itr < info.size()) {
                datas.resize(24 * 60, *itr);
                const search::ELEMENT_HOUR& hour = info.at(*itr);

                for (int i = 0; i < 24; ++i) {
                    for (int j = 0; j < 60; ++j) {
                        int pos = i * 60 + j;
                        time_data& data = datas._data.at(pos);
                        data._pos = pos;
                        data._time = CTime(time.GetYear(), time.GetMonth(), time.GetDay(), i, j, 0);
                        data._segment = segment;
                        data._tick = tick;
                        data._rec_type = hour._hour[i];
                    }
                }
            }
        }
    }
}

void time_table_hour::update(search_data_ptr data, const std::set<int>& cameras)
{
    G2RETURN_IF_FAIL(GetSafeHwnd());

    set_data(data, cameras);
    _enable = true;

    int begin_time = 0, end_time = 0, time_count = 0;
    if (time_list().size() > 0) {
        begin_time = time_list().front()._time.GetHour() * 60;
        end_time = (23 - time_list().back()._time.GetHour()) * 60;
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

bool time_table_hour::is_valid_rec_type(int type)
{
    return (type == record_type_::EXIST ||
            type == record_type_::SEGMENT);
}

bool time_table_hour::get_rec_type_color(int type, COLORREF& color)
{
    bool retv = true;

    if (type == record_type_::EXIST) {
        color = RGB(50, 67, 89);
    }
    else if (type == record_type_::SEGMENT) {
        color = RGB(128, 128, 128);
    }
    else {
        retv = false;
    }

    return retv;
}
