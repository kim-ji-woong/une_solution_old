// device_info_repository.h : header file
//

#ifndef _DEVICE_INFO_REPOSITORY_H_
#define _DEVICE_INFO_REPOSITORY_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include <set>

namespace client {

//////////////////////////////////////////////////////////////////////////

struct device_info
{
    device_info(CString site            = _T(""),
                CString address         = _T(""),
                CString id              = _T(""),
                CString password        = _T(""),
                unsigned int adminPort  = 8200, 
                unsigned int watchPort  = 8016, 
                unsigned int searchPort = 10019, 
                unsigned int audioPort  = 8116)
        : _site(site)
        , _address(address)
        , _id(id)
        , _password(password)
        , _adminPort(adminPort)
        , _watchPort(watchPort)
        , _searchPort(searchPort)
        , _audioPort(audioPort)
    {
    }

    bool operator < (const device_info& rhs) const {
        return _site < rhs._site;
    }

    bool operator == (const CString& key) const {
        return _site == key;
    }

    CString         _site;
    CString         _address;
    CString         _id;
    CString         _password;
    unsigned int    _adminPort;
    unsigned int    _watchPort;
    unsigned int    _searchPort;
    unsigned int    _audioPort;
};

template <class T, class Key>
class set_manager
{
public:
    set_manager(void) {}
    virtual ~set_manager(void) { clear(); }

public:
    typedef T                                   valueType;
    typedef Key                                 keyType;
    typedef std::set<T>                         containerType;
    typedef typename containerType::iterator    iterator;

private:
    containerType container;

public:
    bool add(const T& data) {
        std::pair<iterator, bool> retv = container.insert(data);
        return retv.second;
    }

    bool remove(const Key& key) {
        iterator itr = container.find(key);
        bool retv = itr != container.end();
        if (retv) { container.erase(itr); }
        return retv;
    }

    bool modify(const Key& key, const T& data) {
        return remove(key) && add(data);
    }

    const containerType& container_ref(void)    { return container; }
    const containerType* container_ptr(void)    { return &container; }

    iterator find(const Key& key)               { return container.find(key); }
    iterator begin(void)                        { return container.begin(); }
    iterator end(void)                          { return container.end(); }
    void clear(void)                            { containerType().swap(container); }
    unsigned int size(void)                     { return container.size(); }
};

//////////////////////////////////////////////////////////////////////////

} // !_namespace_client

namespace client {
    typedef set_manager<device_info, CString> device_info_repository;
}

#endif // !_DEVICE_INFO_REPOSITORY_H_