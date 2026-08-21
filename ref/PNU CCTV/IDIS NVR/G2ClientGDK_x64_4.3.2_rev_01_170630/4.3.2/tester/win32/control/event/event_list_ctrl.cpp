// event_list_ctrl.cpp : implementation
//

#include "stdafx.h"
#include "event_list_ctrl.h"
#include "../include/g2_guid.h"

#include <afxadv.h>

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

using namespace client;

//////////////////////////////////////////////////////////////////////////

event_list_ctrl::event_list_ctrl(void)
    : _parent(NULL)
    , _dropTarget(NULL)
{

}

event_list_ctrl::~event_list_ctrl(void)
{
    unregister_drag_drop();
}

//////////////////////////////////////////////////////////////////////////

BEGIN_MESSAGE_MAP(event_list_ctrl, CListCtrl)
    ON_WM_CREATE()
    ON_WM_NCCALCSIZE()

    ON_NOTIFY_REFLECT(LVN_BEGINDRAG,  OnLvnBeginDrag)
    ON_NOTIFY_REFLECT(LVN_BEGINRDRAG, OnLvnBeginRDrag)
END_MESSAGE_MAP()

//////////////////////////////////////////////////////////////////////////

void event_list_ctrl::OnDrop(client::drop_info& di)
{

}

int event_list_ctrl::OnCreate(LPCREATESTRUCT lpCreateStruct)
{
    if (CListCtrl::OnCreate(lpCreateStruct) == -1) {
        return -1;
    }

    register_drag_drop();

    return 0;
}

void event_list_ctrl::OnNcCalcSize(BOOL bCalcValidRects, NCCALCSIZE_PARAMS* lpncsp)
{
    ShowScrollBar(SB_HORZ, FALSE);

    CListCtrl::OnNcCalcSize(bCalcValidRects, lpncsp);
}

void event_list_ctrl::OnLvnBeginDrag(NMHDR* pNMHDR, LRESULT* pResult)
{
    LPNMLISTVIEW pNMLV = reinterpret_cast<LPNMLISTVIEW>(pNMHDR);
    onLvnBeginDragImpl(pNMLV, pResult);
}

void event_list_ctrl::OnLvnBeginRDrag(NMHDR* pNMHDR, LRESULT* pResult)
{
    LPNMLISTVIEW pNMLV = reinterpret_cast<LPNMLISTVIEW>(pNMHDR);
    onLvnBeginDragImpl(pNMLV, pResult);
}

//////////////////////////////////////////////////////////////////////////

void event_list_ctrl::register_drag_drop(void)
{
    assert(!_dropTarget);

    _dropTarget = new drop_target(GetSafeHwnd(), this);

    if (SUCCEEDED(::RegisterDragDrop(GetSafeHwnd(), _dropTarget))) {
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
        assert(!"failed to create DragDrop object");
    }
}

void event_list_ctrl::unregister_drag_drop(void)
{
    HRESULT result = (_dropTarget) ? ::RevokeDragDrop(GetSafeHwnd()) : DRAGDROP_E_NOTREGISTERED;
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

void event_list_ctrl::onLvnBeginDragImpl(LPNMLISTVIEW pNMLV, LRESULT* pResult)
{
    if (pNMLV == NULL) {
        *pResult = 1L;
        return;
    }

    /////////////////////////////////////////////

    const int index = pNMLV->iItem;

    event_data* data = reinterpret_cast<event_data*>(GetItemData(index));
    if (data == NULL) {
        *pResult = 1L;
        return;
    }

    G2EVENT_INFO event = data->_info;

    /////////////////////////////////////////////

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

    G2STRING_46 strguid = { 0 };
    g2_guid_to_string(&event._source, &strguid);

    if (strguid._len < 1) {
        *pResult = 1L;
        return;
    }

    ar << drag_id_::DRAG_FROM_INSTANT_EVENT;
    ar << CString(strguid._string);
    ar << event._spot._segment;
    ar << event._spot._time._time;
    ar << event._spot._tick;
    ar << event._event_ras._cameras;

    ar.Close();

    /////////////////////////////////////////////

    medium.hGlobal = file.Detach();

    target->SetData(&fmtetc, &medium, TRUE);

    client::CDragSourceHelper dragHelper;
    dragHelper.init_from_window(GetSafeHwnd(), pNMLV->ptAction, target);

    DWORD dwEffect;
    HRESULT hr = ::DoDragDrop(target, source, DROPEFFECT_COPY, &dwEffect);

    source->Release();

    *pResult = 0L;
}
