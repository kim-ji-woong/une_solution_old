// time_table_minute.h : header file
//

#ifndef _TIME_TABLE_MINUTE_H_
#define _TIME_TABLE_MINUTE_H_

#include "time_table_base.h"

namespace client {

//////////////////////////////////////////////////////////////////////////

class time_table_minute : public time_table_base
{
public:
    time_table_minute(void);
    virtual ~time_table_minute(void);

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

#endif // !_TIME_TABLE_MINUTE_H_
