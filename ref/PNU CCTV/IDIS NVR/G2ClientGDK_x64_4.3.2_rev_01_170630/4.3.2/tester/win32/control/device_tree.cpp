// device_tree.cpp : implementation
//

#include "stdafx.h"
#include "G2Client.h"

#include "device_tree.h"
#include "drop_target.h"

#include <include/g2_main.h>

#include <shlwapi.h>
#include <afxadv.h>
#include <algorithm>

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

using namespace client;

//////////////////////////////////////////////////////////////////////////

device_tree::device_tree(void)
    : _dragImageList(NULL)
    , _dropTarget(NULL)
{
    for (int i = 0; i < ITEM_TYPE_COUNT; ++i) {
        _icon_index[i] = get_system_icon_index(ITEM_TYPE(i));
    }
}

device_tree::~device_tree(void)
{
    if (_dragImageList) {
        _dragImageList->DeleteImageList();
    }
    delete _dragImageList;
}

//////////////////////////////////////////////////////////////////////////

BEGIN_MESSAGE_MAP(device_tree, CTreeCtrl)
    ON_WM_CREATE()
    ON_WM_DESTROY()
    ON_WM_MOUSEMOVE()
    ON_WM_LBUTTONDOWN()
    ON_WM_LBUTTONUP()
    ON_NOTIFY_REFLECT(TVN_BEGINDRAG, OnTvnBegindrag)
END_MESSAGE_MAP()

//////////////////////////////////////////////////////////////////////////

int device_tree::OnCreate(LPCREATESTRUCT lpCreateStruct)
{
    if (CTreeCtrl::OnCreate(lpCreateStruct) == -1) {
        return -1;
    }

    ModifyStyle(0, TVS_NOHSCROLL | TVS_SHOWSELALWAYS | TVS_HASBUTTONS | TVS_LINESATROOT | TVS_HASLINES);

    /////////////////////////////////////////////

    register_drag_drop();
    set_system_img_list();

    _root = InsertItem(L"All Devices", _icon_index[ITEM_TYPE_ALL_DEVICE], _icon_index[ITEM_TYPE_ALL_DEVICE], TVI_ROOT, TVI_FIRST);

    return 0;
}

void device_tree::OnDestroy()
{
    if (_root) { remove_item(_root); _root = NULL; }
    unregister_drag_drop();

    CTreeCtrl::OnDestroy();
}

void device_tree::OnMouseMove(unsigned int flags, CPoint point)
{
    CTreeCtrl::OnMouseMove(flags, point);
}

void device_tree::OnLButtonDown(unsigned int flags, CPoint point)
{
    CPoint pt = point;
    ClientToScreen(&pt);
    ScreenToClient(&pt);

    HTREEITEM item = HitTest(pt);
    if (item) {
        SetFocus();
        SelectItem(item);
    }

    CTreeCtrl::OnLButtonDown(flags, point);
}

void device_tree::OnLButtonUp(unsigned int flags, CPoint point)
{
    CTreeCtrl::OnLButtonUp(flags, point);
}

void device_tree::OnTvnBegindrag(NMHDR *pNMHDR, LRESULT *pResult)
{
    LPNMTREEVIEW pNMTV = reinterpret_cast<LPNMTREEVIEW>(pNMHDR);

    if (pNMTV == NULL) {
        *pResult = 1L;
        return;
    }

    const HTREEITEM item = pNMTV->itemNew.hItem;
    if (item != _root) begin_drag_impl(pNMTV);

    *pResult = 0;
}

//////////////////////////////////////////////////////////////////////////

void device_tree::set_system_img_list(void)
{
    CImageList list;
    SHFILEINFO sfi = { 0 };
    unsigned int flags = SHGFI_ICON | SHGFI_SYSICONINDEX | SHGFI_OPENICON | SHGFI_PIDL | SHGFI_SMALLICON;
    list.Attach((HIMAGELIST)SHGetFileInfo(L"", NULL, &sfi, sizeof(sfi), flags));
    SetImageList(&list, TVSIL_NORMAL);
    list.Detach();
}

int device_tree::get_system_icon_index(ITEM_TYPE type)
{
    LPMALLOC pointer_malloc;
    SHGetMalloc(&pointer_malloc);
    SHFILEINFO file_info = { 0 };
    LPITEMIDLIST pointer_item_id_list;

    int typeID = (type == ITEM_TYPE_ALL_DEVICE)       ? CSIDL_COMPUTERSNEARME :
                 (type == ITEM_TYPE_DEVICE_DVR)       ? CSIDL_DRIVES :
                 (type == ITEM_TYPE_DEVICE_IPCAMERA)  ? CSIDL_DRIVES :
                 (type == ITEM_TYPE_DEVICE_CAMERA)    ? CSIDL_DRIVES :
                 (type == ITEM_TYPE_DEVICE_ALARM_OUT) ? CSIDL_MYMUSIC : 
                                                   CSIDL_PERSONAL ;

    SHGetSpecialFolderLocation(NULL, typeID, &pointer_item_id_list);
    SHGetFileInfo((TCHAR*)pointer_item_id_list, 0, &file_info, sizeof(file_info), SHGFI_DISPLAYNAME | SHGFI_SYSICONINDEX | SHGFI_PIDL);
    pointer_malloc->Free(pointer_item_id_list);
    pointer_malloc->Release();

    return file_info.iIcon;
}

//////////////////////////////////////////////////////////////////////////

HTREEITEM device_tree::find_item(HTREEITEM item, const CString& key)
{
    HTREEITEM nextItem = item ? GetChildItem(item) : NULL;

    while (nextItem) {
        if (key.Compare(GetItemText(nextItem)) == 0) break;
        nextItem = GetNextItem(nextItem, TVGN_NEXT);
    }

    return nextItem;
}

BOOL device_tree::remove_item(HTREEITEM item)
{
    if (item) {
        CString text = GetItemText(item);

        LPTREEITEMDATA lpData = (LPTREEITEMDATA)GetItemData(item);
        SAFE_DELETE(lpData);
        if (ItemHasChildren(item)) remove_item_children(item);
        DeleteItem(item);
        Invalidate();
        return true;
    }
    return false;
}

BOOL device_tree::remove_item_children(HTREEITEM item)
{
    CString t = GetItemText(item);

    if (item) {
        HTREEITEM nextItem = GetChildItem(item);
        while (nextItem) {
            CString text = GetItemText(nextItem);

            LPTREEITEMDATA lpData = (LPTREEITEMDATA)GetItemData(nextItem);
            SAFE_DELETE(lpData); 
            if (ItemHasChildren(nextItem)) remove_item_children(nextItem);
            DeleteItem(nextItem);
            nextItem = GetChildItem(item);
        }
        Invalidate();
        return true;
    }
    return false;
}

BOOL device_tree::set_item_data(HTREEITEM item, TreeItemData data)
{
    LPTREEITEMDATA lpData = (LPTREEITEMDATA)GetItemData(item);
    SAFE_DELETE(lpData);

    lpData = new TreeItemData;
    *lpData = data;

    return SetItemData(item, (DWORD_PTR)lpData);
}

device_tree::LPTREEITEMDATA device_tree::get_item_data(HTREEITEM item)
{
    return (LPTREEITEMDATA)GetItemData(item);
}

//////////////////////////////////////////////////////////////////////////

void device_tree::clear(void)
{
    remove_item_children(_root);
    Invalidate();
}

//////////////////////////////////////////////////////////////////////////

bool device_tree::append_device(const CString& device, bool isDvr)
{
    if (find_item(_root, device)) return false;

    int type = isDvr ? ITEM_TYPE_DEVICE_DVR : ITEM_TYPE_DEVICE_IPCAMERA;
    int icon = _icon_index[type];
    HTREEITEM item = InsertItem(device, icon, icon, _root, TVI_SORT);
    TreeItemData data   = {0,};
    data.channel        = -1;
    data.type           = type;
    set_item_data(item, data);
    Expand(_root, TVE_EXPAND);
    Invalidate();
    return true;
}

bool device_tree::append_device_child(const CString& device, const CString& child, int channel, bool isCamera)
{
    int type = isCamera ? ITEM_TYPE_DEVICE_CAMERA : ITEM_TYPE_DEVICE_ALARM_OUT;
    int icon = _icon_index[type];
    bool findItem = false;
    HTREEITEM parent = find_item(_root, device);
    if (parent) {
        HTREEITEM nextItem = GetChildItem(parent);

        while (nextItem) {
            LPTREEITEMDATA lpNextItemData = get_item_data(nextItem);
            if (lpNextItemData && lpNextItemData->channel == channel) {
                if (child.Compare(GetItemText(nextItem)) == 0) return false;
                SetItemText(nextItem, child);
                findItem = true;
                break;
            }
            else if (lpNextItemData && lpNextItemData->channel > channel) {
                if (nextItem == GetChildItem(parent)) nextItem = TVI_FIRST;
                else nextItem = GetNextItem(nextItem, TVGN_PREVIOUS);
                break;
            }
            nextItem = GetNextItem(nextItem, TVGN_NEXT);
        }

        if (findItem == false) {
            HTREEITEM item = InsertItem(child, icon, icon, parent, nextItem ? nextItem : TVI_LAST);
            TreeItemData data = {0,};
            data.channel    = channel;
            data.type       = type; 
            set_item_data(item, data);
        }
        
        Invalidate();
        return true;
    }
    return false;
}

bool device_tree::get_child_text(const CString& device, int channel, CString& text)
{
    HTREEITEM parent = find_item(_root, device);
    HTREEITEM nextItem = GetChildItem(parent);
    int index = 0;
    
    while (nextItem && index < channel) {
        nextItem = GetNextItem(nextItem, TVGN_NEXT);
        if (nextItem == NULL) break;
        ++index;
    }

    if (nextItem) {
        text = GetItemText(nextItem);
        return true;
    }
    return false;
}

bool device_tree::remove_device_child(const CString& device, int channel)
{
    HTREEITEM parent = find_item(_root, device);
    if (parent) {
        HTREEITEM nextItem = GetChildItem(parent);

        while (nextItem) {
            LPTREEITEMDATA lpNextItemData = get_item_data(nextItem);
            if (lpNextItemData && lpNextItemData->channel == channel) {
                break;
            }
            nextItem = GetNextItem(nextItem, TVGN_NEXT);
        }
        if (nextItem) {
            LPTREEITEMDATA lpData = (LPTREEITEMDATA)GetItemData(nextItem);
            SAFE_DELETE(lpData);
            DeleteItem(nextItem);
            Invalidate();
            return true;
        }
    }
    return false;
}

bool device_tree::modify_device(const CString& before, const CString& after)
{
    return remove_device(before) && append_device(after);
}

bool device_tree::remove_device(const CString& device)
{
    return remove_item(find_item(_root, device)) ? true : false;
}

bool device_tree::remove_device_children(const CString& device)
{
    return remove_item_children(find_item(_root, device)) ? true : false;
}

CString device_tree::selected_device()
{
    HTREEITEM item = GetSelectedItem();
    if (item) {
        LPTREEITEMDATA lpData = get_item_data(item);
        if (lpData && (lpData->type == ITEM_TYPE_DEVICE_DVR || lpData->type == ITEM_TYPE_DEVICE_IPCAMERA)) {
            return GetItemText(item);
        }
    }
    return CString();
}

bool device_tree::has_children(const CString& device)
{
    return ItemHasChildren(find_item(_root, device)) ? true : false;
}

//////////////////////////////////////////////////////////////////////////

CImageList* device_tree::create_drag_image(HTREEITEM item)
{
    CImageList* icon = CreateDragImage(item);

    CClientDC dc(this);
    CDC memDC;
    CBitmap bitmap;
    CRect rect;
    CImageList* pDragImageList = NULL;

    GetItemRect(item, &rect, TRUE);
    rect.right += 25;
    rect.bottom += 4;

    bool proceed = true;
    if (memDC.CreateCompatibleDC(&dc) != TRUE) {
        proceed = false;
    }
    if (bitmap.CreateCompatibleBitmap(&dc, rect.Width(), rect.Height()) != TRUE) {
        proceed = false;
    }

    if (proceed) {
        CBitmap* oldBitmap = memDC.SelectObject(&bitmap);
        memDC.FillSolidRect(0, 0, rect.Width(), rect.Height(), RGB(0, 148, 255));

        CFont* pFont = GetFont();
        LOGFONT lf;
        pFont->GetLogFont(&lf);
        lf.lfQuality = DEFAULT_QUALITY; //NONANTIALIASED_QUALITY;
        CFont newFont;
        newFont.CreateFontIndirect(&lf);
        CFont* oldFont = memDC.SelectObject(&newFont);

        icon->Draw(&memDC, 0, CPoint(4, 2), ILD_TRANSPARENT);

        CString string = GetItemText(item);
        memDC.DrawText(string, -1, CRect(25, 2, rect.right, rect.bottom), DT_LEFT | DT_SINGLELINE);
        memDC.SelectObject(oldFont);
        memDC.SelectObject(oldBitmap);

        pDragImageList = new CImageList;
        pDragImageList->Create(rect.Width(), rect.Height(), ILC_COLOR32, 0, 1);
        pDragImageList->Add(&bitmap, RGB(255, 0, 0));
        bitmap.DeleteObject();
    }

    if (icon) {
        icon->DeleteImageList();
    }
    delete icon;

    return pDragImageList;
}

void device_tree::begin_drag_impl(LPNMTREEVIEW pNMTV)
{
    client::CIDropSource* source = new client::CIDropSource;
    client::CIDataObject* target = new client::CIDataObject(source);

    source->AddRef();

    FORMATETC fmtetc = { 0 };
    fmtetc.cfFormat  = CF_TEXT;
    fmtetc.dwAspect  = DVASPECT_CONTENT;
    fmtetc.lindex    = -1;
    fmtetc.tymed     = TYMED_HGLOBAL;

    STGMEDIUM medium = { 0 };
    medium.tymed = TYMED_HGLOBAL;

    /////////////////////////////////////////////

    CSharedFile file(GMEM_ZEROINIT | GMEM_DDESHARE | GMEM_MOVEABLE);
    CArchive ar(&file, CArchive::store);


    HTREEITEM item = pNMTV->itemNew.hItem;
    HTREEITEM parent = NULL;
    LPTREEITEMDATA lpData = get_item_data(item);
    if (lpData && (lpData->type == ITEM_TYPE_DEVICE_CAMERA || lpData->type == ITEM_TYPE_DEVICE_ALARM_OUT)) {
        parent = GetParentItem(item);
    }

    CString device = parent ? GetItemText(parent) : GetItemText(item);
    int channel = parent ? lpData->channel : -1;

    ar << drag_id_::DRAG_FROM_DEVICE_TREE;
    ar << device;
    ar << channel;

    ar.Close();

    /////////////////////////////////////////////

    medium.hGlobal = file.Detach();

    target->SetData(&fmtetc, &medium, TRUE);

    client::CDragSourceHelper dragHelper;
    dragHelper.init_from_window(safe_hwnd(), pNMTV->ptDrag, target);

    DWORD dwEffect;
    HRESULT hr = ::DoDragDrop(target, source, DROPEFFECT_COPY, &dwEffect);

    source->Release();
}

void device_tree::register_drag_drop(void)
{
    assert(!_dropTarget);

    _dropTarget = new drop_target(safe_hwnd(), this);

    if (SUCCEEDED(::RegisterDragDrop(safe_hwnd(), _dropTarget))) {
        FORMATETC fmtETC = { 0 };
        fmtETC.cfFormat = CF_TEXT;
        fmtETC.dwAspect = DVASPECT_CONTENT;
        fmtETC.lindex   = -1;
        fmtETC.tymed    = TYMED_HGLOBAL;
        _dropTarget->append_support_format(fmtETC);
        fmtETC.cfFormat = CF_HDROP;
        _dropTarget->append_support_format(fmtETC);
    }
    else {
        SAFE_DELETE(_dropTarget);
        assert("failed to create DragDrop object");
    }
}

void device_tree::unregister_drag_drop(void)
{
    HRESULT result = (_dropTarget) ? ::RevokeDragDrop(safe_hwnd()) : DRAGDROP_E_NOTREGISTERED;
    switch (result) {
        case S_OK:
            // registration as a target window was revoked successfully
            _dropTarget = NULL;
            break;
        case DRAGDROP_E_INVALIDHWND:
            // invalid handle returned in the hwnd parameter
        case DRAGDROP_E_NOTREGISTERED:
            // an attempt was made to revoke a drop target that has not been registered
        default:
            SAFE_DELETE(_dropTarget);
            break;
    }
}

//////////////////////////////////////////////////////////////////////////

