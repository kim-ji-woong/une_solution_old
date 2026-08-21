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
    : _hour_count(0)
    , _hour_boundary(0)
{

}

time_table_minute::~time_table_minute(void)
{
    time_table_base::finalize();
}

//////////////////////////////////////////////////////////////////////////

void time_table_minute::set_data(search_data_ptr data, const std::set<int>& cameras)
{
    g2::scoped_criticalsection lock(_lock);

    const search_minute_info* minutes = data->get_minute_info();

    hour_map().swap(_hour_map);
    minute_map().swap(_min_map);

    for (std::set<int>::const_iterator itr = cameras.begin();
        itr != cameras.end();
        ++itr) {
        int channelext = *itr;
        search::SEARCH_MINUTE_INFO_LIST list;

        if (minutes && minutes->get_minute_info(channelext, list)) {
            int pre_segment = -1;
            hour_list* h_ptr = NULL;
            
            for (unsigned int i = 0; i < list.size(); i += 60) {
                const CTime& time = list.at(i)._time;
                int segment = list.at(i)._segment;

                if (h_ptr == NULL || pre_segment != segment) {
                    pre_segment = segment;

                    hour_map::iterator hours = _hour_map.find(segment);
                    if (hours == _hour_map.end()) {
                        hours = _hour_map.insert(std::make_pair(segment, hour_list())).first;
                    }
                    h_ptr = &hours->second;
                }
                h_ptr->insert(time);
            }
        }
    }

    for (std::set<int>::const_iterator itr = cameras.begin();
        itr != cameras.end();
        ++itr) {
        int channelext = *itr;
        search::SEARCH_MINUTE_INFO_LIST list;

        if (minutes && minutes->get_minute_info(channelext, list)) {
            time_data data((int)list.size() / 60);
            int index = 0;
            for (unsigned int i = 0; i < list.size(); i += 60, ++index) {
                const CTime& time = list.at(i)._time;
                int segment = list.at(i)._segment;
                int hour_index = 0;
                bool is_find = find_hour_index_by_segment_time(segment, time, hour_index);

                data._hours.at(index)._time = time;
                data._hours.at(index)._segment = segment;
                data._hours.at(index)._index = is_find ? hour_index : -1;
            }
            data._minutes = list;
            data._channelext = channelext;

            _min_map.insert(std::make_pair(channelext, data));
        }
    }
}

void time_table_minute::update(search_data_ptr data, const std::set<int>& cameras)
{
    G2RETURN_IF_FAIL(GetSafeHwnd());

    set_data(data, cameras);

    _enable = true;

    int height = ((int)_min_map.size() * default_::TIMETABLE_ROW_HEIGHT) + default_::TIMETABLE_HEAD_HEIGHT + default_::TIMETABLE_TIME_BOUNDARY_HEIGHT;
    height = __max(_rect.Height(), height);

    _hour_count = 0;
    _hour_boundary = 0;

    for (hour_map::const_iterator itr = _hour_map.begin();
        itr != _hour_map.end();
        ++itr) {
        _hour_count += (int)itr->second.size();
        hour_map::const_iterator temp = itr;
        if (++temp == _hour_map.end()) {
            const CTime& time = *itr->second.rbegin();
            _hour_boundary = 23 - time.GetHour();
        }
    }

    int width = (_hour_count + _hour_boundary) * 60;
    width = __max(width, _rect.Width());
    
    initialize_surface(width, height);

    if (_selectPos == -1) {
        time_data& mins = _min_map.rbegin()->second;
        int index = 0;

        std::vector<search::SEARCH_MINUTE_INFO>::reverse_iterator ritr = mins._minutes.rbegin();
        while (ritr != mins._minutes.rend() && ritr->_rec_type == 0) {
            ++ritr;
            ++index;
        }
        set_select_pos((_hour_count * 60) - index, false);
    }
    else {
        int pos;
        if (find_pos_by_spot(data->spot_standard(), pos)) {
            set_select_pos(pos, false);
        }
        else {
            set_select_pos(0, false);
        }
    }

    Invalidate();
}

//////////////////////////////////////////////////////////////////////////

bool time_table_minute::find_hour_index_by_segment_time(int segment, const CTime& time, int& index)
{
    bool is_find = false;
    int hour_index = 0;

    for (hour_map::iterator jtr = _hour_map.begin();
        jtr != _hour_map.end();
        ++jtr) {
        hour_list& hours = jtr->second;
        if (jtr->first == segment) {
            for (hour_list::iterator ktr = hours.begin();
                ktr != hours.end();
                ++ktr, ++hour_index) {
                if (*ktr == time) { 
                    is_find = true; 
                    break;
                }
            }
            if (is_find) break;
        }
        else {
            hour_index += (int)hours.size();
        }
    }

    if (is_find) {
        index = hour_index;
    }

    return is_find;
}

bool time_table_minute::find_pos_by_spot(const G2SPOT& spot, int& pos)
{
    CTime time = g2_time_to_time32_t(&spot._time);
    CTime hour(time.GetYear(), time.GetMonth(), time.GetDay(), time.GetHour(), 0, 0);
    int hour_index = 0;

    if (find_hour_index_by_segment_time(spot._segment, hour, hour_index)) {
        pos = hour_index * 60 + time.GetMinute();
        return true;
    }
    return false;
}

//////////////////////////////////////////////////////////////////////////

void time_table_minute::screen_image_drew(const G2SPOT& spot) 
{
    int pos;
    if (find_pos_by_spot(spot, pos)) {
        set_select_pos(pos, false);
    }
}

//////////////////////////////////////////////////////////////////////////

void time_table_minute::draw_time(CDC *pDC)
{
    if (pDC == NULL ||
        pDC->GetSafeHdc() == NULL) {
            return;
    }
    
    int oldMode = pDC->SetBkMode(TRANSPARENT);
    CFont* oldFont = pDC->SelectObject(&_font);
    
    int pos = 0, hour = 0;
    CString string;
    
    for (hour_map::const_iterator itr = _hour_map.begin();
        itr != _hour_map.end();
        ++itr) {
        CTime pre_date;
        for (hour_list::const_iterator jtr = itr->second.begin();
            jtr != itr->second.end();
            ++jtr, pos += 60) {
            const CTime& time = *jtr;
            CTime date(time.GetYear(), time.GetMonth(), time.GetDay(), 0, 0, 0);
            hour = time.GetHour();

            if (pre_date != date) {
                CRect rect(_rectHead);
                rect.left += pos;
                rect.right += pos;

                pre_date = date;
                string = time.Format(_T("%Y-%m-%d"));
                pDC->DrawText(string, string.GetLength(), rect, DT_SINGLELINE | DT_LEFT | DT_TOP);
            }

            string.Format(L"%02d", hour);
            pDC->DrawText(string, string.GetLength(), CRect(pos, (default_::TIMETABLE_HEAD_HEIGHT / 2), pos + 12, default_::TIMETABLE_HEAD_HEIGHT), 0);
            pDC->FillSolidRect(pos, default_::TIMETABLE_HEAD_HEIGHT + (default_::TIMETABLE_TIME_BOUNDARY_HEIGHT / 2), 1, (default_::TIMETABLE_TIME_BOUNDARY_HEIGHT / 2), RGB(90, 90, 90));
        }
    }

    for (int i = hour + 1; i <= 23; ++i, pos += 60) {
        string.Format(L"%02d", i);
        pDC->DrawText(string, string.GetLength(), CRect(pos, (default_::TIMETABLE_HEAD_HEIGHT / 2), pos + 12, default_::TIMETABLE_HEAD_HEIGHT), 0);
        pDC->FillSolidRect(pos, default_::TIMETABLE_HEAD_HEIGHT + (default_::TIMETABLE_TIME_BOUNDARY_HEIGHT / 2), 1, (default_::TIMETABLE_TIME_BOUNDARY_HEIGHT / 2), RGB(90, 90, 90));
    }

    pDC->SelectObject(oldFont);
    pDC->SetBkMode(oldMode);
}

void time_table_minute::draw_time_bar(CDC* pDC)
{
    if (pDC == NULL ||
        pDC->GetSafeHdc() == NULL) {
        return;
    }

    g2::scoped_criticalsection lock(_lock_data);

    int x = 0, y = 0, cx = 0;
    unsigned int type = 0;
    unsigned int prevType = 0;

    x = 0;
    y = time_table_base::default_::TIMETABLE_HEAD_HEIGHT + time_table_base::default_::TIMETABLE_TIME_BOUNDARY_HEIGHT;

    int tableW = _surface_size.cx;

    if (_min_map.size() <= 0) return;
    int row = 0;
    
    COLORREF color[2] = { RGB(200, 200, 200), RGB(170, 170, 170) };
    
    for (minute_map::const_iterator itr = _min_map.begin();
        itr != _min_map.end();
        ++itr, ++row) {
        const time_data& data = itr->second;
        const std::vector<search::SEARCH_MINUTE_INFO>& mins = data._minutes;

        pDC->FillSolidRect(0, y, tableW, time_table_base::default_::TIMETABLE_ROW_HEIGHT, color[row % 2]);

        for (unsigned int i = 0; i < data._hours.size(); ++i) {
            int offset = data._hours.at(i)._index;
            if (offset < 0) continue;

            for (unsigned int j = 0; j < 60; ++j) {
                unsigned int x = (offset * 60) + j;
                unsigned int index = (i * 60) + j;
                if (index >= mins.size()) break;

                const search::SEARCH_MINUTE_INFO& min_info = mins.at(index);
                COLORREF color;
                if (color_rec_time_info(min_info._rec_type, color)) {
                    pDC->FillSolidRect(x, y + 5, 1, time_table_base::default_::TIMETABLE_BAR_HEIGHT, color);
                }
            }
        }

        y += time_table_base::default_::TIMETABLE_ROW_HEIGHT;
    }
}

bool time_table_minute::set_select_pos(int pos, bool request /*= true*/)
{
    bool retv =  time_table_base::set_select_pos(pos, request);

    if (retv && request) {
        unsigned int index = pos / 60;
        unsigned int minute = pos % 60;
        CTime time;
        int segment = -1;

        for (hour_map::iterator itr = _hour_map.begin();
            itr != _hour_map.end();
            ++itr) {
            hour_list& list = itr->second;
            if (list.size() > index && index >= 0) {
                hour_list::iterator jtr = list.begin();
                while (index--) ++jtr;
                
                segment = itr->first;
                time = CTime(jtr->GetYear(), jtr->GetMonth(), jtr->GetDay(), jtr->GetHour(), minute, 0);
                break;
            }
            else {
                index -= (unsigned int)list.size();
            }
        }

        if (segment > 0) {
            G2SPOT spot;
            spot._segment = segment;
            g2_time_from_time32_t(&spot._time, (unsigned long)time.GetTime());

            if (g2_spot_is_valid(&spot)) {
                if (_listener) _listener->on_changed_select_time_table(spot);
            }
        }
    }

    return retv;
}

bool time_table_minute::color_rec_time_info(int type, COLORREF& color)
{
    if (type & G2FRAME::PANIC) {
        color = RGB(218, 26, 75);
    }
    else if (type & G2FRAME::PRE_EVENT) {
        color = RGB(255, 174, 1);
    }
    else if (type & G2FRAME::EVENT) {
        color = RGB(91, 41, 229);
    }
    else if (type & G2FRAME::TIME_LAPSE) {
        color = RGB(41, 144, 229);
    }
    else if (type & G2FRAME::IRREGULAR) {
        color = RGB(5, 184, 13);
    }
    else {
        return false;
    }

    return true;
}