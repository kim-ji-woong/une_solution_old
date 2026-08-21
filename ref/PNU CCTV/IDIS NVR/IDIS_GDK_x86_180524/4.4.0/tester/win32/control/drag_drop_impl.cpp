// drag_drop_impl.cpp : implementation file
//

#include "stdafx.h"
#include "drag_drop_impl.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

using namespace client;

//////////////////////////////////////////////////////////////////////////

STDMETHODIMP CEnumFormatEtc::QueryInterface(REFIID refiid, void FAR* FAR* ppv)
{
    *ppv = NULL;
    if (IID_IUnknown == refiid ||
        IID_IEnumFORMATETC == refiid) {
        *ppv = this;
    }

    HRESULT hrval = E_NOINTERFACE;
    if (*ppv != NULL) {
        ((LPUNKNOWN)*ppv)->AddRef();
        hrval = S_OK;
    }
    return hrval;
}

STDMETHODIMP_(ULONG) CEnumFormatEtc::AddRef(void)
{
    return ++_refcount;
}

STDMETHODIMP_(ULONG) CEnumFormatEtc::Release(void)
{
    const long temp = --_refcount;
    assert(temp >= 0);
    if (temp == 0) {
        delete this;
    }
    return temp;
}

STDMETHODIMP CEnumFormatEtc::Next(ULONG celt, LPFORMATETC lpFormatEtc, ULONG FAR *pceltFetched)
{
    if (pceltFetched != NULL) {
        *pceltFetched = 0;
    }

    ULONG cReturn = celt;
    if (celt <= 0 || _icursor >= (int)_formatetc.size() ||
        lpFormatEtc == NULL) {
        return S_FALSE;
    }

    if (pceltFetched == NULL &&
        celt != 1) {
        // pceltFetched can be NULL only for 1 item request
        return S_FALSE;
    }

    while (_icursor < (int)_formatetc.size() &&
           cReturn > 0) {
        *lpFormatEtc++ = _formatetc[_icursor++];
        --cReturn;
    }

    if (pceltFetched != NULL) {
        *pceltFetched = celt - cReturn;
    }

    return (cReturn == 0) ? S_OK : S_FALSE;
}

STDMETHODIMP CEnumFormatEtc::Skip(ULONG celt)
{
    if ((_icursor + int(celt)) >= (int)_formatetc.size()) {
        return S_FALSE;
    }

    _icursor += celt;

    return S_OK;
}

STDMETHODIMP CEnumFormatEtc::Reset(void)
{
    _icursor = 0;

    return S_OK;
}

STDMETHODIMP CEnumFormatEtc::Clone(IEnumFORMATETC FAR* FAR* ppCloneEnumFormatEtc)
{
    if (ppCloneEnumFormatEtc == NULL) {
        return E_POINTER;
    }

    CEnumFormatEtc* cloner = new CEnumFormatEtc(_formatetc);
    if (cloner == NULL) {
        return E_OUTOFMEMORY;
    }

    cloner->AddRef();
    cloner->_icursor = _icursor;
    *ppCloneEnumFormatEtc = cloner;

    return S_OK;
}

//////////////////////////////////////////////////////////////////////////

STDMETHODIMP CIDropSource::QueryInterface(
                        /* [in] */          REFIID riid,
                        /* [iid_is][out] */ void __RPC_FAR* __RPC_FAR* ppvObject)
{
    *ppvObject = NULL;
    if (IID_IUnknown == riid ||
        IID_IDropSource == riid) {
        *ppvObject = this;
    }

    HRESULT hrval = E_NOINTERFACE;
    if (*ppvObject != NULL) {
        ((LPUNKNOWN)*ppvObject)->AddRef();
        hrval = S_OK;
    }
    return hrval;
}

STDMETHODIMP_(ULONG) CIDropSource::AddRef(void)
{
    return ++_refcount;
}

STDMETHODIMP_(ULONG) CIDropSource::Release(void)
{
    long temp = --_refcount;
    assert(temp >= 0);
    if (temp == 0) {
        delete this;
    }
    return temp;
}

STDMETHODIMP CIDropSource::QueryContinueDrag(
                        /* [in] */  BOOL  escapePressed,
                        /* [in] */  DWORD grfKeyState)
{
    if(escapePressed) {
        return DRAGDROP_S_CANCEL;
    }

    HRESULT hrval = S_OK;
    if (!(grfKeyState & (MK_LBUTTON | MK_RBUTTON))) {
        _dropped = true;
        hrval = DRAGDROP_S_DROP;
    }
    return hrval;
}

STDMETHODIMP CIDropSource::GiveFeedback(
                        /* [in] */  DWORD effect)
{
    return DRAGDROP_S_USEDEFAULTCURSORS;
}

//////////////////////////////////////////////////////////////////////////

STDMETHODIMP CIDataObject::QueryInterface(
                        /* [in] */          REFIID riid,
                        /* [iid_is][out] */ void __RPC_FAR* __RPC_FAR* ppvObject)
{
    *ppvObject = NULL;
    if (IID_IUnknown == riid ||
        IID_IDataObject == riid) {
        *ppvObject=this;
    }

    HRESULT hrval = E_NOINTERFACE;
    if (NULL != *ppvObject) {
        ((LPUNKNOWN)*ppvObject)->AddRef();
        hrval = S_OK;
    }
    return hrval;
}

STDMETHODIMP_(ULONG) CIDataObject::AddRef(void)
{
    return ++_refcount;
}

STDMETHODIMP_(ULONG) CIDataObject::Release(void)
{
    long temp = --_refcount;
    if (temp == 0) {
        delete this;
    }
    return temp;
}

//////////////////////////////////////////////////////////////////////////

STDMETHODIMP CIDataObject::GetData(
                        /* [unique][in] */  FORMATETC __RPC_FAR* pformatetcIn,
                        /* [out] */         STGMEDIUM __RPC_FAR* pmedium)
{
    if (pformatetcIn == NULL ||
        pmedium == NULL) {
        return E_INVALIDARG;
    }

    pmedium->hGlobal = NULL;

    assert(_stgMedium.size() == _formatetc.size());

    HRESULT hrval = DV_E_FORMATETC;
    for (size_t i = 0, size = _formatetc.size(); i < size; ++i) {
        if (pformatetcIn->tymed & _formatetc[i]->tymed &&
            pformatetcIn->dwAspect == _formatetc[i]->dwAspect &&
            pformatetcIn->cfFormat == _formatetc[i]->cfFormat) {
            copy_medium(pmedium, _stgMedium[i], _formatetc[i]);
            hrval = S_OK;
            break;
        }
    }
    return hrval;
}

STDMETHODIMP CIDataObject::GetDataHere(
                        /* [unique][in] */  FORMATETC __RPC_FAR* pformatetc,
                        /* [out][in] */     STGMEDIUM __RPC_FAR* pmedium)
{
    return E_NOTIMPL;
}

STDMETHODIMP CIDataObject::QueryGetData(
                        /* [unique][in] */  FORMATETC __RPC_FAR* pformatetc)
{
    if (pformatetc == NULL) {
        return E_INVALIDARG;
    }

    // support others if needed DVASPECT_THUMBNAIL, DVASPECT_ICON, DVASPECT_DOCPRINT
    if (!(DVASPECT_CONTENT & pformatetc->dwAspect)) {
        return DV_E_DVASPECT;
    }

    HRESULT hrval = DV_E_TYMED;
    for (size_t i = 0, size = _formatetc.size(); i < size; ++i) {
        if (pformatetc->tymed & _formatetc[i]->tymed) {
            if(pformatetc->cfFormat == _formatetc[i]->cfFormat) {
                hrval = S_OK;
                break;
            }
            else {
                hrval = DV_E_CLIPFORMAT;
            }
        }
        else {
            hrval = DV_E_TYMED;
        }
    }
    return hrval;
}

STDMETHODIMP CIDataObject::GetCanonicalFormatEtc(
                        /* [unique][in] */  FORMATETC __RPC_FAR* pformatectIn,
                        /* [out] */         FORMATETC __RPC_FAR* pformatetcOut)
{
    if (pformatetcOut == NULL) {
        return E_INVALIDARG;
    }

    //...

    return DATA_S_SAMEFORMATETC;
}

STDMETHODIMP CIDataObject::SetData(
                        /* [unique][in] */  FORMATETC __RPC_FAR* pformatetc,
                        /* [unique][in] */  STGMEDIUM __RPC_FAR* pmedium,
                        /* [in] */          BOOL fRelease)
{
    if (pformatetc == NULL ||
        pmedium == NULL) {
        return E_INVALIDARG;
    }

    assert(pformatetc->tymed == pmedium->tymed);

    FORMATETC* pfmtETC = new FORMATETC;
    STGMEDIUM* pStgMed = new STGMEDIUM;

    if (pfmtETC == NULL ||
        pStgMed == NULL) {
        return E_OUTOFMEMORY;
    }

    ZeroMemory(pfmtETC, sizeof(FORMATETC));
    ZeroMemory(pStgMed, sizeof(STGMEDIUM));

    *pfmtETC = *pformatetc;
    _formatetc.push_back(pfmtETC);

    if (fRelease) {
        *pStgMed = *pmedium;
    }
    else {
        copy_medium(pStgMed, pmedium, pformatetc);
    }

    _stgMedium.push_back(pStgMed);

    return S_OK;
}

void CIDataObject::copy_medium(
                        STGMEDIUM* pMedDest,
                        STGMEDIUM* pMedSrc,
                        FORMATETC* pFmtSrc)
{
    switch (pMedSrc->tymed) {
        case TYMED_HGLOBAL:
            pMedDest->hGlobal = (HGLOBAL)OleDuplicateData(pMedSrc->hGlobal,pFmtSrc->cfFormat, NULL);
            break;
        case TYMED_GDI:
            pMedDest->hBitmap = (HBITMAP)OleDuplicateData(pMedSrc->hBitmap,pFmtSrc->cfFormat, NULL);
            break;
        case TYMED_MFPICT:
            pMedDest->hMetaFilePict = (HMETAFILEPICT)OleDuplicateData(pMedSrc->hMetaFilePict,pFmtSrc->cfFormat, NULL);
            break;
        case TYMED_ENHMF:
            pMedDest->hEnhMetaFile = (HENHMETAFILE)OleDuplicateData(pMedSrc->hEnhMetaFile,pFmtSrc->cfFormat, NULL);
            break;
        case TYMED_FILE:
            pMedSrc->lpszFileName = (LPOLESTR)OleDuplicateData(pMedSrc->lpszFileName,pFmtSrc->cfFormat, NULL);
            break;
        case TYMED_ISTREAM:
            pMedDest->pstm = pMedSrc->pstm;
            pMedSrc->pstm->AddRef();
            break;
        case TYMED_ISTORAGE:
            pMedDest->pstg = pMedSrc->pstg;
            pMedSrc->pstg->AddRef();
            break;
        case TYMED_NULL:
        default:
            break;
    }

    pMedDest->tymed = pMedSrc->tymed;
    pMedDest->pUnkForRelease = NULL;

    if (pMedSrc->pUnkForRelease != NULL) {
        pMedDest->pUnkForRelease = pMedSrc->pUnkForRelease;
        pMedSrc->pUnkForRelease->AddRef();
    }
}

STDMETHODIMP CIDataObject::EnumFormatEtc(
                        /* [in] */  DWORD direction,
                        /* [out] */ IEnumFORMATETC __RPC_FAR* __RPC_FAR* ppenumFormatEtc)
{
    if(ppenumFormatEtc == NULL) {
        return E_POINTER;
    }

    *ppenumFormatEtc = NULL;

    switch (direction) {
        case DATADIR_GET:
            *ppenumFormatEtc = new CEnumFormatEtc(_formatetc);
            if (*ppenumFormatEtc == NULL) {
                return E_OUTOFMEMORY;
            }
            (*ppenumFormatEtc)->AddRef();
            break;

        case DATADIR_SET:
        default:
            return E_NOTIMPL;
            break;
    }

    return S_OK;
}

STDMETHODIMP CIDataObject::DAdvise(
                        /* [in] */          FORMATETC __RPC_FAR* pformatetc,
                        /* [in] */          DWORD advf,
                        /* [unique][in] */  IAdviseSink __RPC_FAR* pAdvSink,
                        /* [out] */         DWORD __RPC_FAR* pdwConnection)
{
    return OLE_E_ADVISENOTSUPPORTED;
}

STDMETHODIMP CIDataObject::DUnadvise(
                        /* [in] */  DWORD connection)
{
    return E_NOTIMPL;
}

HRESULT STDMETHODCALLTYPE CIDataObject::EnumDAdvise(
                        /* [out] */ IEnumSTATDATA __RPC_FAR* __RPC_FAR* ppenumAdvise)
{
    return OLE_E_ADVISENOTSUPPORTED;
}

//////////////////////////////////////////////////////////////////////////

void CIDropTarget::append_support_format(const FORMATETC& format)
{
    _formatetc.push_back(format);
}

bool CIDropTarget::query_drop(
                        DWORD grfKeyState,
                        LPDWORD effect)
{
    DWORD dwOKEffects = *effect;

    if (!_allowDrop) {
        *effect = DROPEFFECT_NONE;
        return false;
    }

    // CTRL+SHIFT  -- DROPEFFECT_LINK
    // CTRL        -- DROPEFFECT_COPY
    // SHIFT       -- DROPEFFECT_MOVE
    // no modifier -- DROPEFFECT_MOVE or whatever is allowed by src

    *effect = (grfKeyState & MK_CONTROL) ?
             ((grfKeyState & MK_SHIFT) ? DROPEFFECT_LINK : DROPEFFECT_COPY) :
             ((grfKeyState & MK_SHIFT) ? DROPEFFECT_MOVE : 0 );

    if (*effect == 0) {
        // No modifier keys used by user while dragging.
        if (DROPEFFECT_COPY & dwOKEffects) {
            *effect = DROPEFFECT_COPY;
        }
        else if (DROPEFFECT_MOVE & dwOKEffects) {
            *effect = DROPEFFECT_MOVE;
        }
        else if (DROPEFFECT_LINK & dwOKEffects) {
            *effect = DROPEFFECT_LINK;
        }
        else  {
            *effect = DROPEFFECT_NONE;
        }
    }
    else {
        // Check if the drag source application allows the drop effect desired by user.
        // The drag source specifies this in DoDragDrop
        if (!(*effect & dwOKEffects)) {
            *effect = DROPEFFECT_NONE;
        }
    }

    return (DROPEFFECT_NONE == *effect) ? false : true;
}


HRESULT STDMETHODCALLTYPE CIDropTarget::QueryInterface(
                        /* [in] */          REFIID riid,
                        /* [iid_is][out] */ void __RPC_FAR* __RPC_FAR* ppvObject)
{
    *ppvObject = NULL;
    if (IID_IUnknown == riid ||
        IID_IDropTarget == riid) {
        *ppvObject = this;
    }

    HRESULT hrval = E_NOINTERFACE;
    if (*ppvObject != NULL) {
        ((LPUNKNOWN)*ppvObject)->AddRef();
        hrval = S_OK;
    }
    return hrval;
}

ULONG STDMETHODCALLTYPE CIDropTarget::AddRef(void)
{
    return ++_refcount;
}

ULONG STDMETHODCALLTYPE CIDropTarget::Release(void)
{
    long temp = --_refcount;
    assert(temp >= 0);
    if (temp == 0) {
        delete this;
    }
    return temp;
}

HRESULT STDMETHODCALLTYPE CIDropTarget::DragEnter(
                        /* [unique][in] */  IDataObject __RPC_FAR* pDataObj,
                        /* [in] */          DWORD  grfKeyState,
                        /* [in] */          POINTL pt,
                        /* [out][in] */     DWORD __RPC_FAR* effect)
{
    if (pDataObj == NULL) {
        return E_INVALIDARG;
    }

    if (_dropTargetHelper) {
        _dropTargetHelper->DragEnter(_target, pDataObj, (LPPOINT)&pt, *effect);
    }

    _supportFormat = NULL;

    for (size_t i = 0, size = _formatetc.size(); i < size; ++i) {
        if (_allowDrop = (pDataObj->QueryGetData(&_formatetc[i]) == S_OK)) {
            _supportFormat = &_formatetc[i];
            break;
        }
    }

    bool continueEnter = true;
    if (query_drop(grfKeyState, effect))
    if (_allowDrop &&
        _supportFormat != NULL) {
        STGMEDIUM medium;
        if (pDataObj->GetData(_supportFormat, &medium) == S_OK) {
            continueEnter = OnDragEnter(_supportFormat, &medium, effect, pt);
        }
    }
    return continueEnter ? S_OK : E_ACCESSDENIED;
}

HRESULT STDMETHODCALLTYPE CIDropTarget::DragOver(
                        /* [in] */      DWORD  grfKeyState,
                        /* [in] */      POINTL pt,
                        /* [out][in] */ DWORD __RPC_FAR* effect)
{
    if (_dropTargetHelper) {
        _dropTargetHelper->DragOver((LPPOINT)&pt, *effect);
    }

    bool continueEnter = true;
    if (query_drop(grfKeyState, effect))
    if (_allowDrop) {
        continueEnter = OnDragOver(pt);
    }
    return continueEnter ? S_OK : E_ACCESSDENIED;
}

HRESULT STDMETHODCALLTYPE CIDropTarget::DragLeave(void)
{
    if (_dropTargetHelper) {
        _dropTargetHelper->DragLeave();
    }

    _allowDrop = false;
    _supportFormat = NULL;

    OnDragLeave();

    return S_OK;
}

HRESULT STDMETHODCALLTYPE CIDropTarget::Drop(
                        /* [unique][in] */  IDataObject __RPC_FAR* pDataObj,
                        /* [in] */          DWORD  grfKeyState,
                        /* [in] */          POINTL pt,
                        /* [out][in] */     DWORD __RPC_FAR* effect)
{
    if (pDataObj == NULL) {
        return E_INVALIDARG;
    }

    if (_dropTargetHelper) {
        _dropTargetHelper->Drop(pDataObj, (LPPOINT)&pt, *effect);
    }

    if (query_drop(grfKeyState, effect)) {
        if (_allowDrop && _supportFormat != NULL) {
            STGMEDIUM medium;
            if (pDataObj->GetData(_supportFormat, &medium) == S_OK) {
                if (OnDrop(_supportFormat, &medium, effect, pt, -1L)) {
                    // does derive class wants us to free medium?
                    ReleaseStgMedium(&medium);
                }
            }
        }
    }

    _allowDrop = false;
    *effect = DROPEFFECT_NONE;
    _supportFormat = NULL;

    return S_OK;
}

//////////////////////////////////////////////////////////////////////////

HRESULT CDragSourceHelper::init_from_bitmap(
                        HBITMAP hBitmap,
                        const POINT& pt,
                        const RECT&  rc,
                        IDataObject* object,
                        COLORREF colorKey /*= ::GetSysColor(COLOR_WINDOW)*/
                        )
{
    if (_sourceHelper == NULL) {
        return E_FAIL;
    }

    SHDRAGIMAGE di;
    BITMAP      bm;
    ::GetObject(hBitmap, sizeof(bm), &bm);
    di.sizeDragImage.cx = bm.bmWidth;
    di.sizeDragImage.cy = bm.bmHeight;
    di.hbmpDragImage = hBitmap;
    di.crColorKey = colorKey;
    di.ptOffset.x = pt.x - rc.left;
    di.ptOffset.y = pt.y - rc.top;

    return _sourceHelper->InitializeFromBitmap(&di, object);
}

HRESULT CDragSourceHelper::init_from_bitmap(
                        SHDRAGIMAGE di,
                        IDataObject* object)
{
    return _sourceHelper->InitializeFromBitmap(&di, object);
}

HRESULT CDragSourceHelper::init_from_window(
                        HWND hwnd,
                        POINT& pt,
                        IDataObject* object)
{
    return (_sourceHelper != NULL) ?
            _sourceHelper->InitializeFromWindow(hwnd, &pt, object) : E_FAIL;
}
