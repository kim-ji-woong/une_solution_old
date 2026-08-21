// search_data_manager.h : header file
//

#ifndef _SEARCH_DATA_MANAGER_H_
#define _SEARCH_DATA_MANAGER_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "search_common.h"
#include "search_data.h"

#include <vector>
#include <boost/shared_ptr.hpp>

namespace client {

//////////////////////////////////////////////////////////////////////////

class search_data_manager : private boost::noncopyable
{
protected:
    search_data_manager(void);

public:
    ~search_data_manager(void);

    static search_data_manager& get(void);

private:
    typedef std::vector<search_data_ptr> SearchDataMap;

    SearchDataMap _dataSearch;
    SearchDataMap _dataSearch_g2;

    g2::critical_section _spinSearch;
    g2::critical_section _spinSearch_g2;

private:
    SearchDataMap& map_data(short mode);
    g2::critical_section& map_spin(short mode);

public:
    void initialize(short mode, int connections);
    void cleanup(short mode);
    void cleanup(void);

public:
    search_data_ptr find_search_data(short mode, int channel);
    bool find_search_data(short mode, int channel, search_data_ptr& data);

public:
    search_data_ptr new_search_data(short mode, int channel);
    search_data_ptr find_search_data_search(int channel);
    search_data_ptr find_search_data_search_g2(int channel);

    bool find_search_data_search(int channel, search_data_ptr& data);
    bool find_search_data_search_g2(int channel, search_data_ptr& data);

    bool append_search_data(const search_data_ptr& data);
    bool remove_search_data(short mode, int channel);
    
    bool valid_data_mode(short mode) const;
    bool valid_channel(short mode, short channel) const;
    bool valid_channel(const search_data_ptr& data) const;
};

//////////////////////////////////////////////////////////////////////////

inline bool search_data_manager::valid_data_mode(short mode) const
{
    return (mode > search::SEARCH_INVALID && mode < search::SEARCH_FORCE_LIMIT) ? true : false;
}

inline bool search_data_manager::valid_channel(short mode, short channel) const
{
    return (valid_data_mode(mode) &&
            channel >= 0 &&
            channel < ((mode == client::search::SEARCH_LOCAL_G2 ) ? static_cast<short>(_dataSearch_g2.size()) :
                                                                    static_cast<short>(_dataSearch.size())));
}

inline bool search_data_manager::valid_channel(const search_data_ptr& data) const
{
    return (data.get() != NULL &&
            valid_channel(data->mode(), data->channel()));
}

//////////////////////////////////////////////////////////////////////////

} // !_namespace_client

#endif // !_SEARCH_DATA_MANAGER_H_
