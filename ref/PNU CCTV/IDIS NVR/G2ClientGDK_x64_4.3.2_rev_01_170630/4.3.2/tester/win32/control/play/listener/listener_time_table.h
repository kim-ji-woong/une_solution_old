// listener_time_table.h : header file
//

#ifndef _LISTENER_TIME_TABLE_H_
#define _LISTENER_TIME_TABLE_H_

#include <include/g2_define.h>

namespace client {

//////////////////////////////////////////////////////////////////////////

class listener_time_table
{
public:
    virtual void on_changed_select_time_table(const G2SPOT& spot) = 0;
};

//////////////////////////////////////////////////////////////////////////

} // !_namespace_client

#endif // !_LISTENER_TIME_TABLE_H_