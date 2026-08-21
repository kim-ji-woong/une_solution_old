// device_info_manager.h : header file
//

#ifndef _DEVICE_INFO_MANAGER_H_
#define _DEVICE_INFO_MANAGER_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "device_info_repository.h"
#include <boost/shared_ptr.hpp>

namespace client {
    class device_tree;

//////////////////////////////////////////////////////////////////////////

class device_info_manager
{
public:
    device_info_manager(void);
    virtual ~device_info_manager(void);



public:
    typedef device_info_repository::valueType       valueType;
    typedef device_info_repository::keyType         keyType;
    typedef device_info_repository::containerType   containerType;
    typedef device_info_repository::iterator        iterator;

private:
    device_info_repository                  _repo;
    boost::shared_ptr<client::device_tree>  _tree;

private:
    void import_from_file(void);
    void export_to_file(void);

public:
    void set_device_tree(boost::shared_ptr<client::device_tree> tree) { _tree = tree; update(); }

public:
    bool add(const valueType& data, bool isDvr = true);
    bool add_child(const keyType& site, const CString& child, int channel, bool isCamera = true);
    bool remove_child(const keyType& site, int channel);
    bool remove_children(const keyType& site);
    bool modify_child(const keyType& site, const CString& child, int channel, bool isCamera = true);
    bool remove_selected(void);
    bool modify(const keyType& key, const valueType& data);
    bool get_info(const keyType& key, valueType& data);
    bool get_child_info(const keyType& key, int channel, CString& text);

    CString selected_site(void);
    bool has_children(const keyType& key);
    void update(void);
    void clear(void);
};

//////////////////////////////////////////////////////////////////////////

} // !_namespace_client

#endif // !_DEVICE_INFO_MANAGER_H_