// connective_host_list.h : header file
//

#ifndef _CONNECTIVE_HOST_LIST_H_
#define _CONNECTIVE_HOST_LIST_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include <list>
#include <algorithm>
#include <boost/noncopyable.hpp>
#include <boost/unordered_map.hpp>
#include <boost/mem_fn.hpp>
#include <boost/bind.hpp>
#include <boost/shared_ptr.hpp>

#include <include/g2_define.h>

namespace client {

//////////////////////////////////////////////////////////////////////////

class connective_host_info
{
public:
    connective_host_info(void);
    connective_host_info(const connective_host_info& right);
    connective_host_info(const std::wstring& siteName,
                         const G2NETWORK_INFO& network,
                         int mode,
                         short channel = client::invalid_::CHANNEL,
                         short hostId = client::invalid_::HOST_ID);

    virtual ~connective_host_info(void);

protected:
    short  _hostId;
    short  _channel;
    int    _mode;

    std::wstring _siteName;
    G2NETWORK_INFO _network;

public:
    void initialize(void);

    std::wstring site_name(void) const { return _siteName; }
    std::wstring address(void) const { return _network._address; }
    std::wstring user_name(void) const { return _network._user_id; }
    std::wstring password(void) const { return _network._password; }

    int port(void) const;
    int port(G2NETWORK_INFO::PORT_TYPE type) const;

    const G2NETWORK_INFO& network(void) const { return _network; }

    int address_type(void) const { return _network._address_type; }

    short host_id(void) const { return _hostId; }
    short channel(void) const { return _channel; }
    int mode(void) const { return _mode; }

    bool is_channel_mode(short channel, int mode) const;

    ///////////////////////////////////////////////////////

    void set_host_id(short hostId) { _hostId = hostId; }
    void set_channel(short channel) { _channel = channel; }
    void set_site_name(const std::wstring& name) { _siteName = name; }
    void set_network(const G2NETWORK_INFO& network) { _network = network; }
    void set_mode(int mode) { _mode = mode; }

    ///////////////////////////////////////////////////////

    void set(const std::wstring& siteName,
             const G2NETWORK_INFO& network,
             int mode = client::connect_mode_::UNDEFINED,
             short channel = client::invalid_::CHANNEL,
             short hostId = client::invalid_::HOST_ID);

public:
    connective_host_info& operator=(const connective_host_info& info);
};

typedef boost::shared_ptr<connective_host_info> connective_host_info_ptr;

/////////////////////////////////////////////////////////////////////////

class connective_host_list : private boost::noncopyable
{
public:
    connective_host_list(void);
    ~connective_host_list(void);

public:
    typedef std::list<connective_host_info_ptr>                     container_type;
    typedef boost::unordered_map<int, connective_host_info_ptr>     ment_type;
    typedef container_type::iterator                                iterator;
    typedef container_type::const_iterator                          const_iterator;
    typedef container_type::const_reverse_iterator                  const_reverse_iterator;
    typedef container_type::value_type                              value_type;

protected:
    container_type          _list;
    ment_type               _ment;
    g2::critical_section    _lock_exec;

public:
    const_iterator begin(void)          const { return _list.begin();  }
    const_iterator end(void)            const { return _list.end();    }
    const_reverse_iterator rbegin(void) const { return _list.rbegin(); }
    const_reverse_iterator rend(void)   const { return _list.rend();   }

public:
    int append_host(const connective_host_info& info);
    void remove_host(int id);

    void clear(void);
    int count(void) const;
    int count_mode(int mode) const;

    void set_host_channel(int id, int channel);

    int find_free_host_id(void) const;
    int find_host_id(const connective_host_info& info) const;
    int find_channel_from_host_id(int id) const;
    int find_host_id_from_channel(int channel, int mode) const;
    int find_mode_from_host_id(int id) const;
    const std::vector<int> find_channels_from_connect_mode(client::connect_mode_::MODE mode);

    std::wstring site_name_from_host_id(int id) const;
    std::wstring address_from_host_id(int id) const;

    // finder ///////////////////////////////////

    connective_host_info_ptr get_host_info(int id) const;
    connective_host_info_ptr get_host_info(int channel, int mode) const;

    // secure other data structure //////////////

    template<typename OutputIterator> inline    // connect_host_info
    OutputIterator secure_raw_data(OutputIterator out) const;
    template<typename OutputIterator> inline    // connective_host_info_ptr
    OutputIterator secure_data(OutputIterator out) const;
    template<typename OutputIterator, typename Predicate> inline    // connective_host_info_ptr
    OutputIterator secure_data_if(OutputIterator out, Predicate pred) const;
    template<typename OutputIterator> inline    // host_id
    OutputIterator secure_host_id(OutputIterator out) const;

protected:
    struct finder_host_info : public std::unary_function<value_type, bool> {
        finder_host_info(const connective_host_info& info)
            : _info(info) {}
        const connective_host_info& _info;
        bool operator()(const value_type& value) const {
            bool result = false;
            if (_info.mode() == value->mode() &&
                _info.port() == value->port() &&
                _info.address() == value->address() &&
                _info.user_name() == value->user_name()) {
                result = true;
            }
            return result;
        }
    };
};

//////////////////////////////////////////////////////////////////////////

} // !_namespace_client

#include "connective_host_list.inl"

#endif // !_CONNECTIVE_HOST_LIST_H_
