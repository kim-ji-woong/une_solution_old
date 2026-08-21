// search_data_manager.cpp : implementation file
//

#include "stdafx.h"
#include "search_data_manager.h"

using namespace client;

//////////////////////////////////////////////////////////////////////////

search_data_manager::search_data_manager(void)
{

}

search_data_manager::~search_data_manager(void)
{
    cleanup();
}

//////////////////////////////////////////////////////////////////////////

search_data_manager& search_data_manager::get(void)
{
    static search_data_manager __object__;
    return __object__;
}

//////////////////////////////////////////////////////////////////////////

search_data_manager::SearchDataMap& search_data_manager::map_data(short mode)
{
    assert(valid_data_mode(mode) && "invalid search data mode");

    return (mode == search::SEARCH_LOCAL_G2 ) ? _dataSearch_g2 : 
                                                _dataSearch;
}

g2::critical_section& search_data_manager::map_spin(short mode)
{
    assert(valid_data_mode(mode) && "invalid search data mode");

    return (mode == search::SEARCH_LOCAL_G2 ) ? _spinSearch_g2 : 
                                                _spinSearch;
}

void search_data_manager::initialize(short mode, int connections)
{
    assert(connections > 0);
    connections = __max(connections, 1);

    if (mode == search::SEARCH_LOCAL) {
        g2::scoped_criticalsection lockSearch(_spinSearch);
        SearchDataMap(connections).swap(_dataSearch);
    }
    else if (mode == search::SEARCH_LOCAL_G2) {
        g2::scoped_criticalsection lockSearch_g2(_spinSearch_g2);
        SearchDataMap(connections).swap(_dataSearch_g2);
    }
}

void search_data_manager::cleanup(short mode)
{
    if (mode == search::SEARCH_LOCAL) {
        g2::scoped_criticalsection lockSearch(_spinSearch);
        SearchDataMap().swap(_dataSearch);
    }
    else if (mode == search::SEARCH_LOCAL_G2) {
        g2::scoped_criticalsection lockSearch_g2(_spinSearch_g2);
        SearchDataMap().swap(_dataSearch_g2);
    }
}

void search_data_manager::cleanup(void)
{
    g2::scoped_criticalsection lockSearch(_spinSearch);
    g2::scoped_criticalsection lockSearch_g2(_spinSearch_g2);

    _dataSearch.clear();
    _dataSearch_g2.clear();
}

//////////////////////////////////////////////////////////////////////////

search_data_ptr search_data_manager::new_search_data(short mode, int channel)
{
    return (valid_channel(mode, channel)) ? search_data_ptr(new search_data(mode, channel)): search_data_ptr();
}

bool search_data_manager::find_search_data(short mode, int channel, search_data_ptr& data)
{
    G2RETURN_VAL_IF_FAIL(valid_channel(mode, channel), false);

    g2::scoped_criticalsection lock(map_spin(mode));

    search_data_ptr& temp = map_data(mode)[channel];
    if (temp) data = temp;
    return (temp.get() != NULL);
}

bool search_data_manager::find_search_data_search(int channel, search_data_ptr& data)
{
    return find_search_data(search::SEARCH_LOCAL, channel, data);
}

bool search_data_manager::find_search_data_search_g2(int channel, search_data_ptr& data)
{
    return find_search_data(search::SEARCH_LOCAL_G2, channel, data);
}

search_data_ptr search_data_manager::find_search_data(short mode, int channel)
{
    G2RETURN_VAL_IF_FAIL(valid_channel(mode, channel), search_data_ptr());

    g2::scoped_criticalsection lock(map_spin(mode));

    return map_data(mode)[channel];
}

search_data_ptr search_data_manager::find_search_data_search(int channel)
{
    return find_search_data(search::SEARCH_LOCAL, channel);
}

search_data_ptr search_data_manager::find_search_data_search_g2(int channel)
{
    return find_search_data(search::SEARCH_LOCAL_G2, channel);
}

bool search_data_manager::append_search_data(const search_data_ptr& data)
{
    G2RETURN_VAL_IF_ASSERT(valid_channel(data), false);

    g2::scoped_criticalsection lock(map_spin(data->mode()));

    map_data(data->mode())[data->channel()] = data;

    return true;
}

bool search_data_manager::remove_search_data(short mode, int channel)
{
    G2RETURN_VAL_IF_FAIL(valid_channel(mode, channel), false);

    g2::scoped_criticalsection lock(map_spin(mode));

    map_data(mode)[channel].reset();

    return true;
}
