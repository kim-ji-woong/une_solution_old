// drag_drop_impl.h : header file
//

#ifndef _CONTROL_DRAGDROP_IMPL_H_
#define _CONTROL_DRAGDROP_IMPL_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include <shlobj.h>
#include <vector>

namespace client {

//////////////////////////////////////////////////////////////////////////

class CEnumFormatEtc : public IEnumFORMATETC
{
public:
    CEnumFormatEtc(const std::vector<FORMATETC>& formatEtc)
        : _refcount(0)
        , _formatetc(formatEtc)
        , _icursor(0)
    {

    }
    CEnumFormatEtc(const std::vector<FORMATETC*>& formatEtc)
        : _refcount(0)
        , _icursor(0)
    {
        for (size_t i = 0, size = formatEtc.size(); i < size; ++i) {
            _formatetc.push_back(*formatEtc[i]);
        }
    }

    virtual ~CEnumFormatEtc(void)
    {
        assert(_refcount == 0);
    }

private:
    long    _refcount;
    long    _icursor;
    std::vector<FORMATETC>  _formatetc;

public:
    STDMETHOD(QueryInterface)(REFIID, void FAR* FAR*);
    STDMETHOD_(ULONG, AddRef)(void);
    STDMETHOD_(ULONG, Release)(void);

public:
    STDMETHOD(Next)(ULONG, LPFORMATETC, ULONG FAR*);
    STDMETHOD(Skip)(ULONG);
    STDMETHOD(Reset)(void);
    STDMETHOD(Clone)(IEnumFORMATETC FAR* FAR*);
};

//////////////////////////////////////////////////////////////////////////

class CIDropSource : public IDropSource
{
public:
    CIDropSource(void)
        : _refcount(0)
        , _dropped(false)
    {

    }

    virtual ~CIDropSource(void)
    {
        assert(_refcount == 0);
    }

private:
    long    _refcount;
    bool    _dropped;

public:
    virtual HRESULT STDMETHODCALLTYPE QueryInterface(
                        /* [in] */          REFIID riid,
                        /* [iid_is][out] */ void __RPC_FAR* __RPC_FAR* ppvObject);
    virtual ULONG STDMETHODCALLTYPE AddRef(void);
    virtual ULONG STDMETHODCALLTYPE Release(void);

public:
    virtual HRESULT STDMETHODCALLTYPE QueryContinueDrag(
                        /* [in] */          BOOL  escapePressed,
                        /* [in] */          DWORD grfKeyState);

    virtual HRESULT STDMETHODCALLTYPE GiveFeedback(
                        /* [in] */          DWORD effect);
};

//////////////////////////////////////////////////////////////////////////

class CIDataObject : public IDataObject
{
public:
    CIDataObject(CIDropSource* source)
        : _refcount(0)
        , _dropSource(source)
    {

    }

    virtual ~CIDataObject(void)
    {
        size_t i, size;
        for (i = 0, size = _stgMedium.size(); i < size; ++i) {
            ReleaseStgMedium(_stgMedium[i]);
            if (_stgMedium[i]) {
                delete _stgMedium[i];
                _stgMedium[i] = NULL;
            }
        }
        for (i = 0, size = _formatetc.size(); i < size; ++i) {
            if (_formatetc[i]) {
                delete _formatetc[i];
                _formatetc[i] = NULL;
            }
        }

        assert(_refcount == 0);
    }

private:
    long                    _refcount;
    CIDropSource*           _dropSource;
    std::vector<FORMATETC*> _formatetc;
    std::vector<STGMEDIUM*> _stgMedium;

public:
    void copy_medium(STGMEDIUM* pMedDest, STGMEDIUM* pMedSrc, FORMATETC* pFmtSrc);

public:
    virtual HRESULT STDMETHODCALLTYPE QueryInterface(
                        /* [in] */          REFIID riid,
                        /* [iid_is][out] */ void __RPC_FAR* __RPC_FAR* ppvObject);
    virtual ULONG STDMETHODCALLTYPE AddRef(void);
    virtual ULONG STDMETHODCALLTYPE Release(void);

public:
    virtual /* [local] */ HRESULT STDMETHODCALLTYPE GetData(
                        /* [unique][in] */  FORMATETC __RPC_FAR* pformatetcIn,
                        /* [out] */         STGMEDIUM __RPC_FAR* pmedium);

    virtual /* [local] */ HRESULT STDMETHODCALLTYPE GetDataHere(
                        /* [unique][in] */  FORMATETC __RPC_FAR* pformatetc,
                        /* [out][in] */     STGMEDIUM __RPC_FAR* pmedium);

    virtual HRESULT STDMETHODCALLTYPE QueryGetData(
                        /* [unique][in] */  FORMATETC __RPC_FAR* pformatetc);

    virtual HRESULT STDMETHODCALLTYPE GetCanonicalFormatEtc(
                        /* [unique][in] */  FORMATETC __RPC_FAR* pformatectIn,
                        /* [out] */         FORMATETC __RPC_FAR* pformatetcOut);

    virtual /* [local] */ HRESULT STDMETHODCALLTYPE SetData(
                        /* [unique][in] */  FORMATETC __RPC_FAR* pformatetc,
                        /* [unique][in] */  STGMEDIUM __RPC_FAR* pmedium,
                        /* [in] */          BOOL fRelease);

    virtual HRESULT STDMETHODCALLTYPE EnumFormatEtc(
                        /* [in] */          DWORD dwDirection,
                        /* [out] */         IEnumFORMATETC __RPC_FAR* __RPC_FAR* ppenumFormatEtc);

    virtual HRESULT STDMETHODCALLTYPE DAdvise(
                        /* [in] */          FORMATETC __RPC_FAR* pformatetc,
                        /* [in] */          DWORD advf,
                        /* [unique][in] */  IAdviseSink __RPC_FAR* pAdvSink,
                        /* [out] */         DWORD __RPC_FAR* pdwConnection);

    virtual HRESULT STDMETHODCALLTYPE DUnadvise(
                        /* [in] */          DWORD connection);

    virtual HRESULT STDMETHODCALLTYPE EnumDAdvise(
                        /* [out] */         IEnumSTATDATA __RPC_FAR* __RPC_FAR* ppenumAdvise);
};

//////////////////////////////////////////////////////////////////////////

class CIDropTarget : public IDropTarget
{
public:
    CIDropTarget(HWND target)
        : _target(target)
        , _refcount(0)
        , _allowDrop(false)
        , _dropTargetHelper(NULL)
        , _supportFormat(NULL)
    {
        assert(_target != NULL && ::IsWindow(_target));

        if (FAILED(CoCreateInstance(
                        CLSID_DragDropHelper,
                        NULL,
                        CLSCTX_INPROC_SERVER,
                        IID_IDropTargetHelper,
                        (LPVOID*)&_dropTargetHelper))) {
            _dropTargetHelper = NULL;
        }
    }

    virtual ~CIDropTarget(void)
    {
        if (_dropTargetHelper != NULL) {
            _dropTargetHelper->Release();
            _dropTargetHelper = NULL;
        }
    }

protected:
    HWND                    _target;
    long                    _refcount;
    bool                    _allowDrop;
    IDropTargetHelper*      _dropTargetHelper;
    FORMATETC*              _supportFormat;
    std::vector<FORMATETC>  _formatetc;

public:
    void append_support_format(const FORMATETC& format);
    bool query_drop(DWORD grfKeyState, LPDWORD effect);

    // return values: true - release the medium. false - don't release the medium
    virtual bool OnDrop(FORMATETC* format, STGMEDIUM* medium, DWORD* effect, POINTL point, LONG_PTR userData) = 0;
    virtual bool OnDragEnter(FORMATETC* format, STGMEDIUM* medium, DWORD* effect, POINTL pt) = 0;
    virtual bool OnDragOver(POINTL pt) = 0;
    virtual void OnDragLeave(void) = 0;

public:
    virtual HRESULT STDMETHODCALLTYPE QueryInterface(
                        /* [in] */          REFIID riid,
                        /* [iid_is][out] */ void __RPC_FAR* __RPC_FAR* ppvObject);
    virtual ULONG STDMETHODCALLTYPE AddRef(void);
    virtual ULONG STDMETHODCALLTYPE Release(void);

public:
    virtual HRESULT STDMETHODCALLTYPE DragEnter(
                        /* [unique][in] */  IDataObject __RPC_FAR* pDataObj,
                        /* [in] */          DWORD  grfKeyState,
                        /* [in] */          POINTL pt,
                        /* [out][in] */     DWORD __RPC_FAR* effect);
    virtual HRESULT STDMETHODCALLTYPE DragOver(
                        /* [in] */          DWORD  grfKeyState,
                        /* [in] */          POINTL pt,
                        /* [out][in] */     DWORD __RPC_FAR* effect);
    virtual HRESULT STDMETHODCALLTYPE DragLeave(
                                            void);
    virtual HRESULT STDMETHODCALLTYPE Drop(
                        /* [unique][in] */  IDataObject __RPC_FAR* pDataObj,
                        /* [in] */          DWORD  grfKeyState,
                        /* [in] */          POINTL pt,
                        /* [out][in] */     DWORD __RPC_FAR* effect);
};

//////////////////////////////////////////////////////////////////////////

class CDragSourceHelper
{
public:
    CDragSourceHelper(void)
        : _sourceHelper(NULL)
    {
        if (FAILED(CoCreateInstance(
                        CLSID_DragDropHelper,
                        NULL,
                        CLSCTX_INPROC_SERVER,
                        IID_IDragSourceHelper,
                        (void**)&_sourceHelper))) {
            _sourceHelper = NULL;
        }
    }

    virtual ~CDragSourceHelper(void)
    {
        if (_sourceHelper != NULL) {
            _sourceHelper->Release();
            _sourceHelper = NULL;
        }
    }

private:
    IDragSourceHelper* _sourceHelper;

public:
    HRESULT init_from_bitmap(
                        HBITMAP hBitmap,
                        const POINT& pt,    // cursor position in client coordinate of the window
                        const RECT&  rc,    // selected item's bounding rect
                        IDataObject* object,
                        COLORREF colorKey = ::GetSysColor(COLOR_WINDOW) // color of the window used for transparent effect.
                        );

    HRESULT init_from_bitmap(
                        SHDRAGIMAGE di,
                        IDataObject* object);

    HRESULT init_from_window(
                        HWND hwnd,
                        POINT& pt,
                        IDataObject* object);
};

//////////////////////////////////////////////////////////////////////////

} // !_namespace_client

#endif // !_CONTROL_DRAGDROP_IMPL_H_
