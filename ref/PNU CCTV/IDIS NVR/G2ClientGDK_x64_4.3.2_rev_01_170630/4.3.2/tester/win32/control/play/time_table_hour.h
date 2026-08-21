// time_table_hour.h : header file
//

#ifndef _TIME_TABLE_HOUR_H_
#define _TIME_TABLE_HOUR_H_

#include "time_table_base.h"

namespace client {

//////////////////////////////////////////////////////////////////////////

class time_table_hour : public time_table_base
{
public:
    time_table_hour(void);
    virtual ~time_table_hour(void);

protected:
    struct record_type_ {
        enum TYPE {
            NONE = 0,
            EXIST,
            SEGMENT
        };
    };

protected:
    virtual void set_data(search_data_ptr data, const std::set<int>& cameras);
    
public:
    virtual void update(search_data_ptr data, const std::set<int>& cameras);

protected:
    virtual bool is_valid_rec_type(int type);
    virtual bool get_rec_type_color(int type, COLORREF& color);
};

//////////////////////////////////////////////////////////////////////////

} // !_namespace_client

#endif // !_TIME_TABLE_HOUR_H_
