// connective_host_list.inl : inline file
//

#ifndef _CONNECTIVE_HOST_MAP_INL_
#define _CONNECTIVE_HOST_MAP_INL_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include <common/client_functional.h>
#include <common/client_validate.h>

namespace client {

//////////////////////////////////////////////////////////////////////////

inline connective_host_info::connective_host_info(void)
    : _hostId(client::invalid_::HOST_ID)
    , _channel(client::invalid_::CHANNEL)
    , _mode(client::connect_mode_::UNDEFINED)
{

}

inline connective_host_info::connective_host_info(const connective_host_info& right)
{
    operator=(right);
}

inline connective_host_info::connective_host_info(
    const std::wstring& siteName,
    const G2NETWORK_INFO& network,
    int mode,
    short channel /*= client::invalid_::CHANNEL*/,
    short hostId /*= client::invalid_::HOST_ID*/)
{
    set(siteName,
        network,
        mode,
        channel,
        hostId);
}

inline connective_host_info::~connective_host_info(void)
{

}

//////////////////////////////////////////////////////////////////////////

inline void connective_host_info::initialize(void)
{
    _hostId  = invalid_::HOST_ID;
    _channel = invalid_::CHANNEL;
    _mode    = connect_mode_::UNDEFINED;
}

inline int connective_host_info::port(void) const
{
    return _network._port[client::port_type_from_connect_mode(_mode)];
}

inline int connective_host_info::port(G2NETWORK_INFO::PORT_TYPE type) const
{
    return _network._port[type];
}

inline bool connective_host_info::is_channel_mode(short channel, int mode) const
{
    return (_channel == channel &&
            _mode == mode);
}

inline void connective_host_info::set(
    const std::wstring& siteName,
    const G2NETWORK_INFO& network,
    int mode        /*= ScreenView::NONE*/,
    short channel   /*= client::invalid_::CHANNEL*/,
    short hostId    /*= client::invalid_::HOST_ID*/)
{
    set_site_name(siteName);
    set_network(network);
    set_mode(mode);
    set_channel(channel);
    set_host_id(hostId);
}

//////////////////////////////////////////////////////////////////////////

inline connective_host_info& connective_host_info::operator=(const connective_host_info& right)
{
    if (this == &right) return (*this);

    set(right.site_name(),
        right.network(),
        right.mode(),
        right.channel(),
        right.host_id());

    return (*this);
}

//////////////////////////////////////////////////////////////////////////

inline connective_host_list::connective_host_list(void)
{

}

inline connective_host_list::~connective_host_list(void)
{
    clear();
}

//////////////////////////////////////////////////////////////////////////

inline int connective_host_list::append_host(const connective_host_info& info)
{
    g2::scoped_criticalsection lock(_lock_exec);
    int id = client::invalid_::HOST_ID;
    if (static_cast<int>(_list.size()) < client::MAX_CONNECTIVE_CHANNEL) {
        id = info.host_id();
        _list.push_back(connective_host_info_ptr(new connective_host_info(info)));
        _ment[id] = _list.back();
    }
    assert(_list.size() == _ment.size());
    return id;
}

inline void connective_host_list::remove_host(int id)
{
    g2::scoped_criticalsection lock(_lock_exec);
    iterator itr(std::find_if(_list.begin(), _list.end(),
                 boost::bind(std::equal_to<int>(),
                 boost::bind(&connective_host_info::host_id, _1), id)));
    if (itr != _list.end()) {
        _list.erase(itr);
        _ment.erase(id);
    }
    assert(_list.size() == _ment.size());
}

inline void connective_host_list::clear(void)
{
    g2::scoped_criticalsection lock(_lock_exec);

    container_type().swap(_list);
    ment_type().swap(_ment);
}

inline int connective_host_list::count(void) const
{
    g2::scoped_criticalsection lock(_lock_exec);
    return static_cast<int>(_list.size());
}

inline int connective_host_list::count_mode(int mode) const
{
    g2::scoped_criticalsection lock(_lock_exec);
    return static_cast<int>(std::count_if(_list.begin(), _list.end(),
                            boost::bind(std::equal_to<int>(),
                            boost::bind(&connective_host_info::mode, _1), mode)));
}

inline void connective_host_list::set_host_channel(int id, int channel)
{
    if (connective_host_info_ptr info = get_host_info(id)) {
        info->set_channel(channel);
    }
}

inline int connective_host_list::find_free_host_id(void) const
{
    g2::scoped_criticalsection lock(_lock_exec);
    int id = client::invalid_::HOST_ID;
    for (int i = 0; i < client::MAX_CONNECTIVE_CHANNEL; ++i) {
        if (std::find_if(_list.begin(), _list.end(),
            boost::bind(std::equal_to<int>(),
            boost::bind(&connective_host_info::host_id, _1), i)) == _list.end()) {
            id = i;
            break;
        }
    }
    return id;
}

inline int connective_host_list::find_host_id(const connective_host_info& info) const
{
    g2::scoped_criticalsection lock(_lock_exec);
    const_iterator itr(std::find_if(_list.begin(), _list.end(),
                       finder_host_info(info)));
    return (itr != _list.end() &&
           (*itr)) ? (*itr)->host_id() : client::invalid_::HOST_ID;
}

inline int connective_host_list::find_channel_from_host_id(int id) const
{
    G2RETURN_VAL_IF_FAIL(client::valid_host_id(id), client::invalid_::CHANNEL);
    const connective_host_info_ptr info = get_host_info(id);
    return (info) ? info->channel() : client::invalid_::CHANNEL;
}

inline int connective_host_list::find_host_id_from_channel(int channel, int mode) const
{
    G2RETURN_VAL_IF_FAIL(client::valid_channel(channel) &&
                         client::valid_net_mode(mode), client::invalid_::HOST_ID);

    g2::scoped_criticalsection lock(_lock_exec);
    const_iterator itr(std::find_if(_list.begin(), _list.end(),
                       boost::bind(&connective_host_info::is_channel_mode, _1, channel, mode)));
    return (itr != _list.end() &&
           (*itr)) ? (*itr)->host_id() : client::invalid_::HOST_ID;
}

inline int connective_host_list::find_mode_from_host_id(int id) const
{
    G2RETURN_VAL_IF_FAIL(client::valid_host_id(id), client::connect_mode_::UNDEFINED);
    const connective_host_info_ptr info = get_host_info(id);
    return (info) ? info->mode() : client::connect_mode_::UNDEFINED;
}

inline const std::vector<int> connective_host_list::find_channels_from_connect_mode(client::connect_mode_::MODE mode)
{
    std::vector<int> bunch;

    for (container_type::const_iterator itr(_list.begin());
         itr != _list.end();
         ++itr) {
        connective_host_info_ptr info = *itr;
        if (info->mode() == mode) {
            bunch.push_back(info->channel());
        }
    }

    return bunch;
}

inline std::wstring connective_host_list::site_name_from_host_id(int id) const
{
    const connective_host_info_ptr info = get_host_info(id);
    return (info) ? info->site_name() : L"";
}

inline std::wstring connective_host_list::address_from_host_id(int id) const
{
    const connective_host_info_ptr info = get_host_info(id);
    return (info) ? info->address() : L"";
}

//////////////////////////////////////////////////////////////////////////

inline connective_host_info_ptr connective_host_list::get_host_info(int id) const
{
    g2::scoped_criticalsection lock(_lock_exec);
    ment_type::const_iterator itr(_ment.find(id));
    return (itr != _ment.end()) ?
            connective_host_info_ptr(itr->second) : connective_host_info_ptr();
}

inline connective_host_info_ptr connective_host_list::get_host_info(int channel, int mode) const
{
    g2::scoped_criticalsection lock(_lock_exec);
    const_iterator itr(std::find_if(_list.begin(), _list.end(),
                       boost::bind(&connective_host_info::is_channel_mode, _1, channel, mode)));
    return (itr != _list.end()) ?
            connective_host_info_ptr(*itr) : connective_host_info_ptr();
}

//////////////////////////////////////////////////////////////////////////

template<typename OutputIterator> inline
OutputIterator connective_host_list::secure_raw_data(OutputIterator out) const
{
    g2::scoped_criticalsection lock(_lock_exec);
    for (const_iterator itr(_list.begin());
         itr != _list.end();
         ++itr) {
        if (*itr) *out++ = **itr;
    }
    return out;
}

template<typename OutputIterator> inline
OutputIterator connective_host_list::secure_data(OutputIterator out) const
{
    g2::scoped_criticalsection lock(_lock_exec);
    return std::copy(_list.begin(), _list.end(), out);
}

template<typename OutputIterator, typename Predicate> inline
OutputIterator connective_host_list::secure_data_if(OutputIterator out, Predicate pred) const
{
    g2::scoped_criticalsection lock(_lock_exec);

    template<typename InputIterator, typename OuputIterator, typename Predicate> inline
    OuputIterator copy_if(InputIterator first, InputIterator last, OuputIterator out, Predicate pred)
    {
        for ( ; first != last; ++first) {
            if (pred(*first)) *out++ = *first;
        }
        return out;
    }

    return copy_if(_list.begin(), _list.end(), out, pred);
}

template<typename OutputIterator> inline
OutputIterator connective_host_list::secure_host_id(OutputIterator out) const
{
    g2::scoped_criticalsection lock(_lock_exec);
    return std::transform(_list.begin(), _list.end(),
                          out,
                          boost::mem_fn(&connective_host_info::host_id));
}

//////////////////////////////////////////////////////////////////////////

} // !_namespace_client

#endif // !#ifndef _CONNECTIVE_HOST_MAP_INL_
