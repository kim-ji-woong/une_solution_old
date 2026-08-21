// G2ClientView.cpp : implementation of the CG2ClientView class
//

#include "stdafx.h"
#include "G2Client.h"
#include "G2ClientDoc.h"
#include "G2ClientView.h"
#include "MainFrm.h"
#include "control/message_box.h"
#include "search/search_data_manager.h"
#include "panel/controller/controller_watch.h"
#include "panel/controller/controller_search.h"
#include "panel/controller/controller_search_g2.h"

#include <include/g2_guid.h>
#include <include/g2_main.h>
#include <sampler/cpp/g2client_watch.h>
#include <sampler/cpp/g2client_search.h>
#include <sampler/cpp/g2client_search_g2.h>
#include <sampler/cpp/g2client_dualaudio.h>
#include <sampler/cpp/g2client_status.h>
#include <sampler/cpp/g2client_user.h>
#include <boost/bind.hpp>

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

using namespace client;

//////////////////////////////////////////////////////////////////////////

enum CONST_DATA {
    ALIVE_CHECK_INTERVAL = 1000 * 30
};

//////////////////////////////////////////////////////////////////////////

CG2ClientView::CG2ClientView(void)
    : _dropTarget(NULL)
    , _message(NULL)
    , _viewType(client::view_::VIEW_SCREEN)
    , _selSpkCamera(invalid_::CAMERA_NUMBER)
    , _selMicCamera(invalid_::CAMERA_NUMBER)
{
    __self_pointer__ = this;

    _watch  = new client::g2watch;
    _search = new client::g2search;
    _search_g2 = new client::g2search_g2;
    _audio  = new client::g2dualaudio;
    _status = new client::g2status;

    g2_main_app_initialize(G2LANGUAGE_ID::ENGLISH);
}

CG2ClientView::~CG2ClientView(void)
{
    SAFE_DELETE(_watch);
    SAFE_DELETE(_search);
    SAFE_DELETE(_search_g2);
    SAFE_DELETE(_audio);
    SAFE_DELETE(_status);

    g2_main_app_finalize();

    __self_pointer__ = NULL;
}

//////////////////////////////////////////////////////////////////////////

client::runner* client::runner::__self_pointer__ = NULL;

//////////////////////////////////////////////////////////////////////////

IMPLEMENT_DYNCREATE(CG2ClientView, CView)

BEGIN_MESSAGE_MAP(CG2ClientView, CView)
    ON_WM_CREATE()
    ON_WM_ERASEBKGND()
    ON_WM_SIZE()
    ON_WM_TIMER()

    ON_COMMAND(ID_FILE_PRINT,               &CView::OnFilePrint)
    ON_COMMAND(ID_FILE_PRINT_DIRECT,        &CView::OnFilePrint)
    ON_COMMAND(ID_FILE_PRINT_PREVIEW,       &CView::OnFilePrintPreview)

    ON_COMMAND(ID_SCREEN_CONNECT_WATCH,     on_drop_watch)
    ON_COMMAND(ID_SCREEN_CONNECT_SEARCH,    on_drop_search)
    ON_COMMAND(ID_CAMERA_INSTANT_RECORD,    on_screen_camera_instant_record)
    ON_COMMAND(ID_CAMERA_LISTEN,            on_screen_camera_listen)
    ON_COMMAND(ID_CAMERA_TALK,              on_screen_camera_talk)

    ON_COMMAND_RANGE(ID_CAMERA_REMOVE_CAMERA, ID_CAMERA_REMOVE_ALL,             on_screen_camera_disconnect)
    ON_COMMAND_RANGE(ID_CAMERA_RATIO_FIT_TO_SCREEN, ID_CAMERA_RATIO_ORIGINAL,   on_screen_camera_ratio)

    ON_MESSAGE(um_runner_::UM_DROP_FROM_SCREEN,                                 on_drop_from_screen)
    ON_MESSAGE(um_runner_::UM_DROP_INFO_SECURE,                                 on_drop_info_secure)
    ON_MESSAGE(um_runner_::UM_CONNECTED_SEARCH,                                 on_post_connected_search)
    ON_MESSAGE(um_runner_::UM_CONNECTED_SEARCH_G2,                              on_post_connected_search_g2)

    ON_MESSAGE(um_runner_::UM_DISCONNECTED_SEARCH,                              on_post_disconnected_search)
    ON_MESSAGE(um_runner_::UM_DISCONNECTED_SEARCH_G2,                           on_post_disconnected_search_g2)

    ON_MESSAGE(um_runner_::UM_RECEIVE_RECORDED_DATE_SEARCH,                     on_post_receive_recorded_date_search)

    ON_MESSAGE(um_runner_::UM_SCREEN_NO_IMAGE_LOADED_SEARCH,                    on_post_screen_no_image_loaded_search)
    ON_MESSAGE(um_runner_::UM_SCREEN_NO_IMAGE_LOADED_SEARCH_G2,                 on_post_screen_no_image_loaded_search_g2)

    ON_MESSAGE(um_runner_::UM_RECTIME_LOADED_SEARCH_G2,                         on_post_rectime_info_load_end_search_g2)
    ON_MESSAGE(um_runner_::UM_NO_RECORDED_DATA_SEARCH_G2,                       on_post_no_recorded_data_search_g2)
    ON_MESSAGE(um_runner_::UM_RECEIVE_COMMAND_BEGIN_SEARCH_G2,                  on_post_receive_command_begin_search_g2)
END_MESSAGE_MAP()

//////////////////////////////////////////////////////////////////////////

BOOL CG2ClientView::PreCreateWindow(CREATESTRUCT& cs)
{
	return CView::PreCreateWindow(cs);
}

void CG2ClientView::OnDraw(CDC* /*pDC*/)
{
	CG2ClientDoc* pDoc = GetDocument();
	ASSERT_VALID(pDoc);
    if (!pDoc) {
		return;
    }
}

BOOL CG2ClientView::OnPreparePrinting(CPrintInfo* pInfo)
{
	return DoPreparePrinting(pInfo);
}

void CG2ClientView::OnBeginPrinting(CDC* /*pDC*/, CPrintInfo* /*pInfo*/)
{

}

void CG2ClientView::OnEndPrinting(CDC* /*pDC*/, CPrintInfo* /*pInfo*/)
{

}

#ifdef _DEBUG
void CG2ClientView::AssertValid() const
{
    CView::AssertValid();
}

void CG2ClientView::Dump(CDumpContext& dc) const
{
    CView::Dump(dc);
}

CG2ClientDoc* CG2ClientView::GetDocument(void) const // non-debug version is inline
{
    ASSERT(m_pDocument->IsKindOf(RUNTIME_CLASS(CG2ClientDoc)));
    return (CG2ClientDoc*)m_pDocument;
}
#endif //_DEBUG

//////////////////////////////////////////////////////////////////////////

int CG2ClientView::OnCreate(LPCREATESTRUCT lpcs)
{
    CView::OnCreate(lpcs);

    client::foundation_ptr()->PostMessage(foundation::UM_POST_INITIALIZE);

    return 0;
}

BOOL CG2ClientView::OnEraseBkgnd(CDC* pDC)
{
    return TRUE;
}

void CG2ClientView::OnSize(unsigned int nType, int cx, int cy)
{
    CView::OnSize(nType, cx, cy);

    if (::IsWindow(_splitter.GetSafeHwnd())) {
        _splitter.SetWindowPos(NULL, 0, 0, cx, cy, SWP_NOMOVE | SWP_NOZORDER | SWP_NOREDRAW);
    }
}

void CG2ClientView::OnTimer(UINT_PTR nIDEvent)
{
    if (nIDEvent >= TIMER_ID_RECONNECT) {
        KillTimer(nIDEvent);
        reconnect_impl((short)(nIDEvent - TIMER_ID_RECONNECT));
    }

    CView::OnTimer(nIDEvent);
}

//////////////////////////////////////////////////////////////////////////

g2watch& CG2ClientView::watch_ref(void) const { return *_watch; }
g2search& CG2ClientView::search_ref(void) const { return *_search; }
g2search_g2& CG2ClientView::search_g2_ref(void) const { return *_search_g2; }
g2dualaudio& CG2ClientView::audio_ref(void) const { return *_audio; }
g2status& CG2ClientView::status_ref(void) const { return *_status; }

client::screen_actuator& CG2ClientView::actuator_ref(void) { return _screenPannel.actuator_ref(); }

client::message_box* CG2ClientView::message_box_ptr(void) { return (_message != NULL && ::IsWindow(_message->GetSafeHwnd())) ? _message : NULL; }
client::controller_watch* CG2ClientView::controller_live_ptr(void) { return _controllerPannel.controller_live_ptr(); }
client::controller_search* CG2ClientView::controller_search_ptr(void) { return _controllerPannel.controller_search_ptr(); }
client::controller_search_g2* CG2ClientView::controller_search_g2_ptr(void) { return _controllerPannel.controller_search_g2_ptr(); }

std::vector<client::controller_base*> CG2ClientView::controller_bunch(void) { return _controllerPannel.controller_bunch(); }

//////////////////////////////////////////////////////////////////////////

void CG2ClientView::initialize(void)
{
    ctor_network();
    register_drag_drop();

    _cameraMap.create(client::MAX_SCREEN_CAMERA_COUNT);

    /////////////////////////////////////////////

    if (_splitter.CreateStatic(this, 2, 1) != TRUE) {
        return;
    }

    _screenPannel.create(&_splitter, _splitter.IdFromRowCol(0, 0));
    _controllerPannel.create(&_splitter, _splitter.IdFromRowCol(1, 0));

    _splitter.create(0, 0, &_screenPannel, CSize(0, 480));
    _splitter.create(1, 0, &_controllerPannel, CSize(0, 110));

    CRect rect;
    GetClientRect(rect);
    _splitter.SetWindowPos(NULL, 0, 0, rect.Width(), rect.Height(), SWP_NOMOVE | SWP_NOZORDER);

    _screenPannel.ShowWindow(SW_SHOW);
    _controllerPannel.ShowWindow(SW_SHOW);

    /////////////////////////////////////////////

    screen_actuator& actuator = _screenPannel.actuator_ref();
    actuator.set_drag_drop(this, um_runner_::UM_DROP_FROM_SCREEN, um_runner_::UM_DROP_INFO_SECURE);
    actuator.set_listener(this);
    actuator.show();
    actuator.set_active_window();
    actuator.set_focus();
    actuator.run();

    /////////////////////////////////////////////

    _message = new message_box();
    _message->create(actuator.screen_ptr());
}

void CG2ClientView::finalize(void)
{
    dtor_network();
    unregister_drag_drop();

    disconnect_all();

    /////////////////////////////////////////////

    if (_message != NULL &&
        ::IsWindow(_message->GetSafeHwnd())) {
        _message->DestroyWindow();
    }
    SAFE_DELETE(_message);
}

void CG2ClientView::ctor_network(void)
{
    _watch->startup(client::MAX_CONNECTIVE_CHANNEL, safe_hwnd());
    _watch->set_listener(this);
    _search->startup(client::MAX_CONNECTIVE_CHANNEL);
    _search->set_listener(this);
    _search_g2->startup(client::MAX_CONNECTIVE_CHANNEL);
    _search_g2->set_listener(this);
    _audio->startup(client::MAX_CONNECTIVE_CHANNEL, safe_hwnd());
    _audio->set_listener(this);
    _status->startup(client::MAX_CONNECTIVE_CHANNEL, client::MAX_CONNECTIVE_CHANNEL);
    _status->set_listener(this);

    search_data_manager::get().initialize(search::SEARCH_LOCAL,     client::MAX_CONNECTIVE_CHANNEL);
    search_data_manager::get().initialize(search::SEARCH_LOCAL_G2,  client::MAX_CONNECTIVE_CHANNEL);

    /////////////////////////////////////////////

    _hostList.clear();
    _disconnectByMe.clear();
    _endofplayHost.clear();
}

void CG2ClientView::dtor_network(void)
{
    for (connective_host_list::const_iterator citr = _hostList.begin();
        citr != _hostList.end();
        ++citr) {
        connective_host_info_ptr info = *citr;

        switch (info->mode()) {
        case client::connect_mode_::SEARCH:
            if (_search->is_connected(info->channel()))
                _search->disconnect(info->channel());
            break;
        case client::connect_mode_::SEARCH_G2:
            if (_search_g2->is_connected(info->channel()))
                _search_g2->disconnect(info->channel());
            break;
        case client::connect_mode_::WATCH:
            if (_watch->is_connected(info->channel()))
                _watch->disconnect(info->channel());
            break;
        }
    }

    _watch->cleanup();
    _search->cleanup();
    _search_g2->cleanup();
    _audio->cleanup();

    search_data_manager::get().cleanup();

    /////////////////////////////////////////////

    _hostList.clear();
    _disconnectByMe.clear();
}

void CG2ClientView::cleanup(void)
{
    screen_actuator& actuator = actuator_ref();
    actuator.reset_screen(true);

    _hostList.clear();
    _cameraMap.cleanup();
}

void CG2ClientView::show_screen_message(const std::wstring& string, bool autohide /*= true*/, int elapse /*= 5000*/)
{
    G2RETURN_IF_FAIL(_message != NULL &&
                     ::IsWindow(_message->GetSafeHwnd()));

    _message->hide();
    _message->set_string(string);
    _message->show(false, autohide, elapse);
}

void CG2ClientView::show_screen_message(const std::wstring& string, const std::wstring& message, bool autohide /*= true*/, int elapse /*= 5000*/)
{
    G2RETURN_IF_FAIL(_message != NULL &&
                     ::IsWindow(_message->GetSafeHwnd()));

    _message->hide();
    _message->set_string(string);
    _message->set_message(message);
    _message->show(true, autohide, elapse);
}

void CG2ClientView::hide_screen_message(void)
{
    G2RETURN_IF_FAIL(_message != NULL &&
                     ::IsWindow(_message->GetSafeHwnd()));

    _message->hide();
}

//////////////////////////////////////////////////////////////////////////

void CG2ClientView::OnDrop(client::drop_info& di)
{
    if (client::app::get().is_initialized() != true) return;

    /*
    if (g2_guid_is_device(&di._guidDevice)) {
        if (client::g2admin::get().is_device_enable(di._guidDevice) != true) {
            MessageBox(_T("Status of selected device is disabled."), NULL, MB_OK);
            return;
        }
    }
    */

    _dropInfo = di;

    CPoint ptcurrent = di._point;
    screen_actuator& actuator = actuator_ref();
    screen_view& screen = actuator.screen_ref();

    int scrcamera = invalid_::CAMERA_NUMBER;
    if (ptcurrent == CPoint(_I32_MIN, _I32_MAX)) {
        scrcamera = _dropInfo._dropcamera;
    }
    else {
        screen.ScreenToClient(&ptcurrent);
        scrcamera = screen.find_camera_by_point(ptcurrent);
    }

    if (client::valid_camera(scrcamera) != true) {
        return;
        //scrcamera = 0;
    }
    _dropInfo._dropcamera = scrcamera;

    G2RETURN_IF_FAIL(_dropInfo._dropcamera != invalid_::CAMERA_NUMBER);

    actuator.set_camera(scrcamera);
    actuator.update_screen();

    /////////////////////////////////////////////

    CMenu menu, *popup = NULL;
    if (menu.LoadMenu(IDR_MENU_CAMERA_CONTEXT) &&
       (popup = menu.GetSubMenu(screen_menu_::CONTEXT_CONNECTIVE))) {
        // success loaded
    }
    G2RETURN_IF_FAIL(popup != NULL);

    popup->TrackPopupMenu(TPM_LEFTALIGN | TPM_NOANIMATION, di._point.x, di._point.y, this);
}

void CG2ClientView::register_drag_drop(void)
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
        assert(!"failed to create DragDrop object");
    }
}

void CG2ClientView::unregister_drag_drop(void)
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

void CG2ClientView::release_host_id(short hostId, bool redraw /*= true*/, bool connect /*= true*/)
{
    G2RETURN_IF_FAIL(client::valid_host_id(hostId));

    set_disconnect_by_me(hostId, false);

    reset_screen_view(hostId, redraw, connect);

    screen_actuator& actuator = actuator_ref();
    actuator.set_ignore_frame(true);
    actuator.remove_buffer(hostId);
    actuator.set_ignore_frame(false);

    _cameraMap.remove_by_host_id(hostId);
    _hostList.remove_host(hostId);
}

//////////////////////////////////////////////////////////////////////////

void CG2ClientView::on_drop_watch(void)
{
    client::drop_info info = _dropInfo; // value copy

    connect_watch(info);
}

void CG2ClientView::on_drop_search(void)
{
    client::drop_info info = _dropInfo; // value copy

    connect_search(info);
}

void CG2ClientView::on_screen_camera_instant_record(void)
{
    const screen_actuator& actuator = actuator_ref();
    const short selcamera = actuator.selected_camera();
    //const client::g2guid& guid = _cameraMap.get_guid_camera(selcamera);

    /*
    if (g2_guid_is_valid(guid.c_ptr())) {
        bool activate = client::g2admin::get().is_activate_instant_record(guid.c_ref());
        client::g2admin::get().request_recording_instant(guid.c_ref(), !activate);
    }
    */
}

void CG2ClientView::on_screen_camera_disconnect(UINT nID)
{
    const screen_actuator& actuator = actuator_ref();
    const short selcamera = actuator.selected_camera();

    if (client::valid_camera(selcamera)) {
        switch (nID) {
            case ID_CAMERA_REMOVE_CAMERA:
                disconnect_each_camera(selcamera);
                break;
            case ID_CAMERA_REMOVE_DEVICE:
                disconnect_each_device(selcamera);
                break;
            case ID_CAMERA_REMOVE_ALL:
                disconnect_all();
                break;
        }
    }
}

void CG2ClientView::on_screen_camera_ratio(UINT nID)
{
    screen_actuator& actuator = actuator_ref();
    const short selcamera = actuator.selected_camera();

    if (client::valid_camera(selcamera)) {
        screen::img_ratio_::FACTOR factor = screen::img_ratio_::CONSTANT;
        switch (nID) {
            case ID_CAMERA_RATIO_FIT_TO_SCREEN:
                factor = screen::img_ratio_::NORMAL;
                break;
            case ID_CAMERA_RATIO_ORIGINAL:
                factor = screen::img_ratio_::CONSTANT;
                break;
            default:
                assert(!"undefined ratio type");
                break;
        }

        actuator.set_camera_ratio(selcamera, factor);
    }
}

void CG2ClientView::on_screen_camera_listen(void)
{
    screen_actuator& actuator = actuator_ref();
    const short selcamera = actuator.selected_camera();
    const int hostId = client::find_host_id_from_camera(selcamera);

    bool activate = (is_activate_dual_audio_speaker(selcamera) != true);

    on_post_connected_dual_audio_in(selcamera, activate);
}

void CG2ClientView::on_screen_camera_talk(void)
{
    screen_actuator& actuator = actuator_ref();
    const short selcamera = actuator.selected_camera();

    bool activate = (is_activate_dual_audio_microphone(selcamera) != true);
    
    on_post_connected_dual_audio_out(selcamera, activate);
}

LRESULT CG2ClientView::on_drop_from_screen(WPARAM wParam, LPARAM lParam)
{
    drop_info_param* param = reinterpret_cast<drop_info_param*>(lParam);
    assert(param != NULL);

    if (_dropTarget) {
        _dropTarget->OnDrop(param->_format, param->_medium, param->_effect, param->_point, param->_userData);
    }
    return 0L;
}

LRESULT CG2ClientView::on_drop_info_secure(WPARAM wParam, LPARAM lParam)
{
    return 0L;
}

//////////////////////////////////////////////////////////////////////////

client::runner* client::runner_ptr(bool securing /*= false*/)
{
    return (securing && g_mainframe) ?
            g_mainframe->get_runner() : client::runner::__self_pointer__;
}

//////////////////////////////////////////////////////////////////////////

void CG2ClientView::on_screen_no_image_loaded(short hostId)
{
    unsigned int messageId = _UI32_MAX;
    switch (client::find_mode_from_host_id(hostId)) {
        case client::connect_mode_::PLAY:
            messageId = um_runner_::UM_SCREEN_NO_IMAGE_LOADED_PLAY;
            break;
        case client::connect_mode_::SEARCH:
            messageId = um_runner_::UM_SCREEN_NO_IMAGE_LOADED_SEARCH;
            break;
        case client::connect_mode_::SEARCH_G2:
            messageId = um_runner_::UM_SCREEN_NO_IMAGE_LOADED_SEARCH_G2;
            break;
        default:
            assert(!"unknown connect mode");
            break;
    }

    if (messageId != _UI32_MAX) {
        PostMessage(messageId, WPARAM(hostId));
    }
}

void CG2ClientView::on_screen_camera_changed(short camera)
{
    G2RETURN_IF_FAIL(client::valid_camera(camera));

    _controllerPannel.PostMessage(um_controller_::UM_SCREEN_CAMERA_CHANGED, WPARAM(camera));

    foundation_ptr()->PostMessage(foundation::UM_POST_SCREEN_SELECTED, WPARAM(camera));
}

void CG2ClientView::on_screen_layout_changed(int layout, int changed /*= client::screen::layout_change_::CHANGE*/)
{
    std::vector<client::controller_base*> bunch = controller_bunch();
    std::for_each(bunch.begin(), bunch.end(), boost::bind(&client::controller_base::on_screen_layout_changed, _1, layout, changed));

    std::set<int> host_id;
    for (int i = 0; i < _cameraMap.size(); ++i) {
        host_id.insert(_cameraMap.get_host_id(i));
    }
    if (host_id.empty() != true) {
        std::for_each(host_id.begin(), host_id.end(),
                      boost::bind(&client::runner::set_camera_list, this, _1, true, false));
    }

    screen_actuator& actuator = actuator_ref();
    const short selcamera = actuator.selected_camera();
    _controllerPannel.PostMessage(um_controller_::UM_UPDATE_TIME_TABLE, WPARAM(selcamera));
}

void CG2ClientView::on_screen_image_drew(short camera, const G2SPOT& spot)
{
   const int connectMode = _cameraMap.get_connect_mode(camera);

   client::controller_base* controller = NULL;
   if (connectMode == connect_mode_::SEARCH) {
       controller = controller_search_ptr();
   }
   else if (connectMode == connect_mode_::SEARCH_G2) {
       controller = controller_search_g2_ptr();
   }

   if (controller != NULL) {
       controller->on_screen_image_drew(camera, spot);
   }
}

void CG2ClientView::on_screen_lbutton_down(unsigned int flags, const CPoint& point)
{

}

void CG2ClientView::on_screen_lbutton_up(unsigned int flags, const CPoint& point)
{

}

void CG2ClientView::on_screen_lbutton_dblclk(unsigned int flags, const CPoint& point)
{

}

void CG2ClientView::on_screen_rbutton_down(unsigned int flags, const CPoint& point)
{

}

void CG2ClientView::on_screen_rbutton_up(unsigned int flags, const CPoint& point)
{
    CMenu menu, *popup = NULL;
    if (menu.LoadMenu(IDR_MENU_CAMERA_CONTEXT) &&
       (popup = menu.GetSubMenu(screen_menu_::CONTEXT_CAMERA))) {
        // success loaded
    }
    G2RETURN_IF_FAIL(popup != NULL);

    const screen_actuator& actuator = actuator_ref();
    const short selcamera = actuator.selected_camera();
    const short hostId = _cameraMap.get_host_id(selcamera);
    //const client::g2guid& guid = _cameraMap.get_guid_camera(selcamera);
    const int mode = _cameraMap.get_connect_mode(selcamera);

    switch (actuator.camera_ratio(selcamera)) {
        case screen::img_ratio_::NORMAL:
            popup->CheckMenuItem(ID_CAMERA_RATIO_FIT_TO_SCREEN, MF_BYCOMMAND | MF_CHECKED);
            break;
        case screen::img_ratio_::CONSTANT:
            popup->CheckMenuItem(ID_CAMERA_RATIO_ORIGINAL, MF_BYCOMMAND | MF_CHECKED);
            break;
    }

    if (/*g2_user_is_login() != true ||*/
        valid_host_id(hostId) != true) {
        popup->EnableMenuItem(ID_CAMERA_REMOVE_CAMERA, MF_BYCOMMAND | MF_GRAYED | MF_DISABLED);
        popup->EnableMenuItem(ID_CAMERA_REMOVE_DEVICE, MF_BYCOMMAND | MF_GRAYED | MF_DISABLED);
        popup->EnableMenuItem(ID_CAMERA_REMOVE_ALL, MF_BYCOMMAND | MF_GRAYED | MF_DISABLED);
        popup->EnableMenuItem(ID_CAMERA_LISTEN, MF_BYCOMMAND | MF_GRAYED | MF_DISABLED);
        popup->EnableMenuItem(ID_CAMERA_TALK, MF_BYCOMMAND | MF_GRAYED | MF_DISABLED);
        popup->EnableMenuItem(ID_CAMERA_INSTANT_RECORD, MF_BYCOMMAND | MF_GRAYED | MF_DISABLED);
    }
    else {
        if (mode == connect_mode_::LIVE/* && g2user::is_authority(G2USER_AUTHORITY::AUTHORITY_INSTANT_RECORDING)*/) {
            popup->EnableMenuItem(ID_CAMERA_INSTANT_RECORD, MF_BYCOMMAND | MF_ENABLED);

            /*
            if (client::g2admin::get().is_activate_instant_record(guid.c_ref())) {
                popup->CheckMenuItem(ID_CAMERA_INSTANT_RECORD, MF_BYCOMMAND | MF_CHECKED);
            }
            */
        }
        else {
            popup->EnableMenuItem(ID_CAMERA_INSTANT_RECORD, MF_BYCOMMAND | MF_GRAYED | MF_DISABLED);
        }

        if (is_activate_dual_audio_speaker(selcamera)) {
            popup->CheckMenuItem(ID_CAMERA_LISTEN, MF_BYCOMMAND | MF_CHECKED);
        }
        
        if (is_activate_dual_audio_microphone(selcamera)) {
            popup->CheckMenuItem(ID_CAMERA_TALK, MF_BYCOMMAND | MF_CHECKED);
        }
    }

    CPoint pt = point;
    ClientToScreen(&pt);
    popup->TrackPopupMenu(TPM_LEFTALIGN | TPM_NOANIMATION, pt.x, pt.y, this);
}

void CG2ClientView::on_screen_mbutton_down(unsigned int flags, const CPoint& point)
{

}

void CG2ClientView::on_screen_mbutton_up(unsigned int flags, const CPoint& point)
{

}

void CG2ClientView::on_screen_mouse_move(unsigned int flags, const CPoint& point)
{

}

void CG2ClientView::on_screen_mouse_wheel(unsigned int flags, short zdelta, const CPoint& point)
{

}

bool CG2ClientView::on_screen_set_cursor(CWnd* pWnd, unsigned int hittest, unsigned int message, int camera, const CRect& rect)
{
    return false;
}

void CG2ClientView::on_screen_key_down(unsigned int nChar, unsigned int nRepCnt, unsigned int nFlags)
{

}

void CG2ClientView::on_screen_key_up(unsigned int nChar, unsigned int nRepCnt, unsigned int nFlags)
{

}

void CG2ClientView::on_screen_set_foucs(CWnd* pOldWnd)
{

}

void CG2ClientView::on_screen_resized(unsigned int type, int cx, int cy)
{

}

int CG2ClientView::set_play_speed(short hostId, int command, int clientSpeed /*= client::search::speed_::PLAY_SPEED_UNDEFINED*/)
{
    G2RETURN_VAL_IF_FAIL(client::valid_host_id(hostId), 0);

    screen_actuator& actuator = actuator_ref();

    int speed = _I32_MIN;

    switch (command) {
        case G2PLAYER::NONE:         speed =   0; break;
        case G2PLAYER::PLAY_SLOW:    speed =  +5; break;
        case G2PLAYER::PLAY_NORMAL:  speed = +10; break;
        case G2PLAYER::PLAY_FAST:    speed = +20; break;
        case G2PLAYER::PLAY_FASTER:  speed = +40; break;
        case G2PLAYER::PLAY_FASTEST: speed = +80; break;
        case G2PLAYER::BACK_SLOW:    speed =  -5; break;
        case G2PLAYER::BACK_NORMAL:  speed = -10; break;
        case G2PLAYER::BACK_FAST:    speed = -20; break;
        case G2PLAYER::BACK_FASTER:  speed = -40; break;
        case G2PLAYER::BACK_FASTEST: speed = -80; break;
        default:
            assert(!"not defined speed command");
            break;
    }

    if (command != G2PLAYER::NONE &&
        clientSpeed != client::search::speed_::PLAY_SPEED_UNDEFINED) {
        clientSpeed = client::revise_speed(command, clientSpeed);
        switch (clientSpeed) {
            case search::speed_::BACK_FASTEST_MORE: speed = -120; break;
            case search::speed_::BACK_FASTEST:      speed = -100; break;
            case search::speed_::BACK_FASTER_MORE:  speed =  -80; break;
            case search::speed_::BACK_FASTER:       speed =  -60; break;
            case search::speed_::BACK_FAST_MORE:    speed =  -50; break;
            case search::speed_::BACK_FAST:         speed =  -40; break;
            case search::speed_::BACK_NORMAL_TRIPLE:speed =  -30; break;
            case search::speed_::BACK_NORMAL_TWICE: speed =  -20; break;
            case search::speed_::BACK_NORMAL_HFAST: speed =  -15; break;
            case search::speed_::BACK_NORMAL:       speed =  -10; break;
            case search::speed_::BACK_SLOW:         speed =   -5; break;
            case search::speed_::PLAY_STOP:         speed =    0; break;
            case search::speed_::PLAY_SLOW:         speed =   +5; break;
            case search::speed_::PLAY_NORMAL:       speed =  +10; break;
            case search::speed_::PLAY_NORMAL_HFAST: speed =  +15; break;
            case search::speed_::PLAY_NORMAL_TWICE: speed =  +20; break;
            case search::speed_::PLAY_NORMAL_TRIPLE:speed =  +30; break;
            case search::speed_::PLAY_FAST:         speed =  +40; break;
            case search::speed_::PLAY_FAST_MORE:    speed =  +50; break;
            case search::speed_::PLAY_FASTER:       speed =  +60; break;
            case search::speed_::PLAY_FASTER_MORE:  speed =  +80; break;
            case search::speed_::PLAY_FASTEST:      speed = +100; break;
            case search::speed_::PLAY_FASTEST_MORE: speed = +120; break;

            case search::speed_::PLAY_NORMAL_INFINITE:
            case search::speed_::BACK_NORMAL_INFINITE:
            case search::speed_::PLAY_FASTEST_INFINITE:
            case search::speed_::BACK_FASTEST_INFINITE: speed = _I32_MAX; break;
            default:
                assert(!"not defined speed for client play");
                break;
        }
    }

    if (speed != _I32_MIN) {
        bool ignoreSound = false;
        actuator.set_search_play_speed(hostId, speed, ignoreSound);
    }
    return speed;
}

void CG2ClientView::set_view_type(int type)
{
    /*
    if (_viewType != type) {
        _viewType = type;
        _screenPannel.change_view(type);
        _controllerPannel.change_controller(type);
    }
    */
}

