// device_tree.h : header file
//

#ifndef _CONTROL_DEVICE_TREE_H_
#define _CONTROL_DEVICE_TREE_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "drop_target.h"

namespace client {
    class CIDropTarget;

//////////////////////////////////////////////////////////////////////////

class device_tree : public CTreeCtrl
                  , protected drop_event
{
public:
    device_tree(void);
    virtual ~device_tree(void);

public:
    typedef struct _item_param {
        int channel;
        int type;
    }TreeItemData, *LPTREEITEMDATA;

protected:
    enum ITEM_TYPE {
        ITEM_TYPE_ALL_DEVICE = 0,
        ITEM_TYPE_DEVICE_DVR,
        ITEM_TYPE_DEVICE_IPCAMERA,
        ITEM_TYPE_DEVICE_CAMERA,
        ITEM_TYPE_DEVICE_ALARM_OUT,

        ITEM_TYPE_COUNT
    };

protected:
    HTREEITEM   _root;
    int         _icon_index[ITEM_TYPE_COUNT];

protected:
    CImageList*     _dragImageList;
    CIDropTarget*   _dropTarget;

protected:
    CImageList* create_drag_image(HTREEITEM item);
    void begin_drag_impl(LPNMTREEVIEW pNMTV);
    void register_drag_drop(void);
    void unregister_drag_drop(void);

    //////////////////////////////////////////////////////////////////////////

protected:
    void set_system_img_list(void);
    int get_system_icon_index(ITEM_TYPE type);

    //////////////////////////////////////////////////////////////////////////

protected:
    HTREEITEM find_item(HTREEITEM item, const CString& key);
    BOOL remove_item(HTREEITEM item);
    BOOL remove_item_children(HTREEITEM item);
    BOOL set_item_data(HTREEITEM item, TreeItemData data);
    LPTREEITEMDATA get_item_data(HTREEITEM item);
    
public:
    HWND safe_hwnd(void) const { return GetSafeHwnd(); }
    
    void clear(void);
    bool append_device(const CString& device, bool isDvr = true);
    bool append_device_child(const CString& device, const CString& child, int channel, bool isCamera = true);
    bool remove_device_child(const CString& device, int channel);
    bool modify_device(const CString& before, const CString& after);
    bool remove_device(const CString& device);
    bool remove_device_children(const CString& device);
    CString selected_device(void);
    bool get_child_text(const CString& device, int channel, CString& text);
    bool has_children(const CString& device);

    //////////////////////////////////////////////////////////////////////////

protected:
    virtual void OnDrop(client::drop_info& di) {}

    DECLARE_MESSAGE_MAP()

    afx_msg int  OnCreate(LPCREATESTRUCT lpCreateStruct);
    afx_msg void OnDestroy();
    afx_msg void OnMouseMove(unsigned int nFlags, CPoint point);
    afx_msg void OnLButtonDown(unsigned int nFlags, CPoint point);
    afx_msg void OnLButtonUp(unsigned int nFlags, CPoint point);
    afx_msg void OnTvnBegindrag(NMHDR *pNMHDR, LRESULT *pResult);
};

//////////////////////////////////////////////////////////////////////////

} // !_namespace_client

#endif // !_CONTROL_DEVICE_TREE_H_
