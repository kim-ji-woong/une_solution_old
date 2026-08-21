// clip_copy_dlg.cpp : implementation file
//

#include "stdafx.h"
#include "MainFrm.h"
#include "G2ClientView.h"
#include "clip_copy_dlg.h"
#include "clip_copy_password_dlg.h"
#include "select_scope_dlg.h"
#include <sampler/cpp/g2client_search.h>
#include <sampler/cpp/g2client_search_g2.h>

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

using namespace client;

#define PROCEED_CANCELED_SELFPLAYER(channel)    \
{                                               \
    if (_clipcopyStatus == CANCELED) {       \
        TRACE("$$ SaveVideoPlay:: already sending canceled message\n"); \
        return;                                 \
    }                                           \
    if (_clipcopyStatus == STANBY) {            \
        PostMessage(um_clip_copy_::UM_STOP_SAVE_VIDEO); \
        return;                                 \
    }                                           \
}

enum { IDD = IDD_PLAY_CLIPCOPY };

enum TIMER_ID {
    TIMER_ID_CLIPCOPY_MEASURE_SIZE = 1000,
    TIMER_ID_CLIPCOPY_ON_CANCEL
};

//////////////////////////////////////////////////////////////////////////

clip_copy_dlg::clip_copy_dlg(short hostId, CWnd* parent /*= NULL*/)
    : CDialog(IDD, parent)
    , _parent(parent)
    , _centerBase(parent)
    , _channel(client::invalid_::CHANNEL)
    , _channelext(client::invalid_::CHANNEL_EXT)
    , _hostId(hostId)
    , _initialized(false)
    , _started(false)
    , _usePassword(false)
    , _progress(-1)
    , _clipcopyStatus(STANBY)
    , _mode(CONNECT_SEARCH)
{
    initialize_search();
}

clip_copy_dlg::clip_copy_dlg(short hostId, client::g2search_g2* search_g2, CWnd* parent /*= NULL*/)
    : CDialog(IDD, parent)
    , _parent(parent)
    , _centerBase(parent)
    , _channel(client::invalid_::CHANNEL)
    , _channelext(client::invalid_::CHANNEL_EXT)
    , _hostId(hostId)
    , _initialized(false)
    , _started(false)
    , _usePassword(false)
    , _progress(-1)
    , _clipcopyStatus(STANBY)
    , _mode(CONNECT_SEARCH_G2)
{
    _search_g2 = search_g2;

    initialize_search();
}

clip_copy_dlg::~clip_copy_dlg(void)
{
    finalize_search();
}

//////////////////////////////////////////////////////////////////////////


void clip_copy_dlg::DoDataExchange(CDataExchange* pDX)
{
    CDialog::DoDataExchange(pDX);

    DDX_Control(pDX, IDC_CHK_FIRST,         _chkFirst);
    DDX_Control(pDX, IDC_CHK_LAST,          _chkLast);
    DDX_Control(pDX, IDC_CHK_SAVE_PW,       _chkPassword);

    DDX_Control(pDX, IDC_DTP_FROM_DATE,     _dtcFromDate);
    DDX_Control(pDX, IDC_DTP_FROM_TIME,     _dtcFromTime);
    DDX_Control(pDX, IDC_DTP_TO_DATE,       _dtcToDate);
    DDX_Control(pDX, IDC_DTP_TO_TIME,       _dtcToTime);
    DDX_Control(pDX, IDC_PROGRESS_BAR,      _progressBar);
    DDX_Control(pDX, IDC_BTN_START_STOP,    _btnStart);
    DDX_Control(pDX, IDCANCEL,              _btnCancel);
}


//////////////////////////////////////////////////////////////////////////

BEGIN_MESSAGE_MAP(clip_copy_dlg, CDialog)
    ON_WM_CREATE()
    ON_WM_DESTROY()
    ON_WM_TIMER()

    ON_COMMAND(IDC_BTN_START_STOP,                  on_btn_start_stop)
    ON_COMMAND(IDC_CHK_FIRST,                       on_chk_first)
    ON_COMMAND(IDC_CHK_LAST,                        on_chk_last)

    ON_MESSAGE(um_clip_copy_::UM_DISCONNECTED,      on_post_disconnected)
    ON_MESSAGE(um_clip_copy_::UM_START_SAVE_VIDEO,  on_post_start_save_video)
    ON_MESSAGE(um_clip_copy_::UM_STOP_SAVE_VIDEO,   on_post_stop_save_video)
    ON_MESSAGE(um_clip_copy_::UM_CANCELED_CLIPCOPY, on_post_canceled_clipcopy)
    ON_MESSAGE(um_clip_copy_::UM_COMPLETE_CLIPCOPY, on_post_complete_clipcopy)
    ON_MESSAGE(um_clip_copy_::UM_UPDATE_CLIPCOPY,   on_post_update_clipcopy)
END_MESSAGE_MAP()

//////////////////////////////////////////////////////////////////////////

BOOL clip_copy_dlg::OnInitDialog()
{
    CDialog::OnInitDialog();

    ///////////////////////////////////////////////////////

    CWnd* parent = (_centerBase != NULL && _centerBase->GetSafeHwnd()) ? _centerBase : _parent;
    CenterWindow(parent);

    _chkFirst.SetCheck(BST_CHECKED);
    _chkLast.SetCheck(BST_CHECKED);

    on_chk_first();
    on_chk_last();

    _progressBar.SetRange(0, 100);

    if (_mode == CONNECT_SEARCH && _search) {
        enable_controls(_search->is_connected(_channel));
    }

    return TRUE;
}

int clip_copy_dlg::OnCreate(LPCREATESTRUCT lpcs)
{
    if (CDialog::OnCreate(lpcs) == -1) {
        return -1;
    }

    return 0;
}

void clip_copy_dlg::OnDestroy()
{
    CDialog::OnDestroy();
}

void clip_copy_dlg::OnCancel()
{
    CDialog::OnCancel();
}

void clip_copy_dlg::OnTimer(UINT_PTR nIDEvent)
{
    CDialog::OnTimer(nIDEvent);

    switch (nIDEvent) {
        case TIMER_ID_CLIPCOPY_MEASURE_SIZE:
            KillTimer(nIDEvent);
            if (_mode == CONNECT_SEARCH_G2 && _search_g2) _search_g2-> request_clipcopy_size(_channel);
            else if (_search.get()) _search->request_clipcopy_size(_channel);
            break;
        case TIMER_ID_CLIPCOPY_ON_CANCEL:
            KillTimer(nIDEvent);
            PostMessage(um_clip_copy_::UM_CANCELED_CLIPCOPY);
            break;
    }
}

void clip_copy_dlg::initialize_search(void)
{
    if (client::runner* runner = runner_ptr()) {
        client::screen_actuator& actuator = runner->actuator_ref();
        const short scrcamera = actuator.selected_camera();
        _channelext = client::find_host_camera_from_camera(scrcamera);

        client::connective_host_list* hostList = runner_ptr()->get_host_list();
        connective_host_info_ptr hi = hostList->get_host_info(_hostId);
        G2RETURN_IF_FAIL(hi != NULL);

        if (_mode == CONNECT_SEARCH_G2 && _search_g2) {
            _channel = hi->channel();
            _search_g2->set_invoke_saver(_channel, this);
        }
        else {
            _search.reset(new client::g2search);
            _search->set_listener(this);
            _search->startup(2);
            _channel = _search->connect_ras(&hi->network());
        }
    }

    if (client::valid_channel(_channel)) {
        _initialized = true;
    }
    else {
        finalize_search();
    }

    return;
}

void clip_copy_dlg::finalize_search(void)
{
    if (_mode == CONNECT_SEARCH_G2 && _search_g2) {
        _search_g2->set_revoke_saver(_channel);
        _search_g2 = NULL;
    }
    else if (_search) {
        //_search->set_revoke_saver(_channel);
        _search->disconnect(_channel);
        _search->cleanup();
        _search.reset();
    }
}

//////////////////////////////////////////////////////////////////////////

void clip_copy_dlg::on_g2search_connected(G2HSEARCH handle, int channel)
{
    G2RETURN_IF_FAIL(channel == _channel);
    G2RETURN_IF_FAIL(client::valid_channel(_channel));

    enable_controls(true);

    //_search->set_invoke_saver(channel, this);
}

void clip_copy_dlg::on_g2search_disconnected(G2HSEARCH handle, int channel, G2DISCONNECT_REASON::TYPE reason)
{
    _search->set_revoke_saver(channel);

    if (reason != G2DISCONNECT_REASON::LOGOUT) {
        G2STRING_128 string = { 0 };
        g2_get_string_disconnect_reason(reason, &string);

        CString message;
        message.Format(_T("Disconnect : %s"), string._string);
        MessageBox(message, NULL, MB_OK);
    }

    PostMessage(um_clip_copy_::UM_DISCONNECTED);
}

void clip_copy_dlg::on_g2search_saver_disconnected(G2HSEARCH handle, int channel, unsigned int reason)
{
    G2RETURN_IF_FAIL(channel == _channel);
}

void clip_copy_dlg::on_g2search_saver_receive_clipcopy_scope(G2HSEARCH handle, int channel, const std::vector<G2SCOPE>& scopes)
{
    G2RETURN_IF_ASSERT(channel == _channel);

    PROCEED_CANCELED_SELFPLAYER(channel)

    int selected = 0;

    if (scopes.empty()) {        
        MessageBox(L"No search data.", NULL, MB_OK);
        PostMessage(um_clip_copy_::UM_STOP_SAVE_VIDEO);
        return;
    }
    else if (scopes.size() == 1) {
        selected = 0;
    }
    else if (scopes.size() > 1) {
        select_scope_dlg dlg(this);
        dlg.set_scope_list(scopes);
        if (dlg.DoModal() == IDOK) {
            if (_search->is_connected(_channel) &&
                selected == dlg.get_selected_index()) {
                    // nothing
            }
            else {
                PostMessage(um_clip_copy_::UM_STOP_SAVE_VIDEO);
                return;
            }
        }
    }

    G2SCOPE scope = scopes[selected];
    unsigned int size = get_dir_free_size(_dirPath, true);

    _search->request_clipcopy_measure_size(_channel, scope, size, false);
}

void clip_copy_dlg::on_g2search_saver_receive_clipcopy_cancel(G2HSEARCH handle, int channel)
{
    KillTimer(TIMER_ID_CLIPCOPY_ON_CANCEL);
    PostMessage(um_clip_copy_::UM_CANCELED_CLIPCOPY);
}

void clip_copy_dlg::on_g2search_saver_receive_clipcopy_measure_size(G2HSEARCH handle, int channel)
{
    G2RETURN_IF_FAIL(channel == _channel);
    PROCEED_CANCELED_SELFPLAYER(channel)

    _search->request_clipcopy_size(channel);
}

void clip_copy_dlg::on_g2search_saver_receive_clipcopy_size(G2HSEARCH handle, int channel, signed char status, unsigned __int64 size, const G2TIME& begin, const G2TIME& end)
{
    TRACE("$$ SaveVideoPlay:: check size channel: %d, status: %d, size: %u\n", channel, status, size);

    PROCEED_CANCELED_SELFPLAYER(channel)

    if (status == G2CLIPCOPY_STATUS::NOT_ENOUGH_SPACE) {
        MessageBox(L"There is not sufficient space at storage media.", NULL, MB_OK);
        PostMessage(um_clip_copy_::UM_STOP_SAVE_VIDEO);
        return;
    }
    else if (status == G2CLIPCOPY_STATUS::NOT_COMPLETED) {
        SetTimer(TIMER_ID_CLIPCOPY_MEASURE_SIZE, 500, NULL);
    }
    else if (status == G2CLIPCOPY_STATUS::PARTIAL_COPIABLE) {
        TCHAR systemPath[_MAX_PATH] = { 0 };
        ::GetSystemDirectory(systemPath, _MAX_PATH);

        TCHAR drive[_MAX_DRIVE];
        TCHAR dir[_MAX_DIR];
        TCHAR fname[_MAX_FNAME];
        TCHAR ext[_MAX_EXT];
        _tsplitpath_s(systemPath, drive, dir, fname, ext);

        CString message;
        message.Format(((get_dir_free_size(_dirPath, true) >= client::max_::REQUIRED_SPACE_4) ?
                            _T("Do you want to copy to %s\\?\n(File size was limited to 4GB.)") :
                            _T("Not Enough Space in %s\\.\nDo you want to continue using the available space?")),
                       drive);

        if (MessageBox(message, NULL, MB_ICONQUESTION | MB_YESNO) == IDNO) {
            PostMessage(um_clip_copy_::UM_STOP_SAVE_VIDEO);
            return;
        }
    }

    G2RETURN_IF_FAIL(_search->is_connected(_channel));

    _file.SetLength(size);

    if (_usePassword) {
        G2STRING_32 password;
        int size = min(_password.GetLength() * 2, sizeof(password._string));
        memcpy(&password._string, _password.GetBuffer(), size);
        password._len = _password.GetLength();

        _search->request_clipcopy_password(_channel, _usePassword, password);
    }
    else {
        _search->request_clipcopy(_channel, true);
    }
}

void clip_copy_dlg::on_g2search_saver_receive_clipcopy_data(G2HSEARCH handle, int channel, bool completed, int progress, int count, int length, const unsigned char* data)
{
    PROCEED_CANCELED_SELFPLAYER(channel)

    if (_progress != progress) {
        _progress = progress;
        PostMessage(um_clip_copy_::UM_UPDATE_CLIPCOPY, _progress);
    }

    try {
        int index = 0, offset, size;
        for (int i = 0; i < count; ++i) {
            offset = *((int*)(data + index));
            size   = *((int*)(data + index + sizeof(int)));
            _file.Seek(offset, CFile::begin);
            _file.Write(data + index + sizeof(int) + sizeof(int), size);
            index += size + sizeof(int) + sizeof(int);
        }

        TRACE("$$ SaveVideoPlay:: receive data channel: %d, offset: %I64d, size: %u, progress %d\n", channel, offset, size, progress);
    }
    catch (CFileException* e) {
        _search->request_clipcopy_cancel(channel);

        CString error;
        e->GetErrorMessage(error.GetBuffer(1024), 1024);
        e->Delete();
        error.ReleaseBuffer();
        MessageBox(error, NULL, MB_OK | MB_ICONERROR);
    }

    if (completed) {
        if (progress < 100) {
            MessageBox( L"There is no data in the selected interval.", NULL, MB_OK);
            PostMessage(um_clip_copy_::UM_CANCELED_CLIPCOPY);
            return;
        }

        PostMessage(um_clip_copy_::UM_COMPLETE_CLIPCOPY);
    }
    else {
        _search->request_clipcopy(channel, false);
    }
}

void clip_copy_dlg::on_g2search_saver_receive_clipcopy_password(G2HSEARCH handle, int channel)
{
    G2RETURN_IF_ASSERT(channel == _channel);
    PROCEED_CANCELED_SELFPLAYER(channel)

    _search->request_clipcopy(channel, true);
}

void clip_copy_dlg::on_g2search_saver_receive_clipcopy_enable_channels(G2HSEARCH handle, int channel, unsigned int cameras)
{
    G2RETURN_IF_ASSERT(channel == _channel);
}

//////////////////////////////////////////////////////////////////////////

void clip_copy_dlg::on_g2search_g2_saver_disconnected(G2HSEARCH_G2 handle, int channel, G2DISCONNECT_REASON::TYPE reason)
{
    if (reason != G2DISCONNECT_REASON::LOGOUT) {
        G2STRING_128 string = { 0 };
        g2_get_string_disconnect_reason(reason, &string);

        CString message;
        message.Format(_T("Disconnect : %s"), string._string);
        MessageBox(message, NULL, MB_OK);
    }

    PostMessage(um_clip_copy_::UM_DISCONNECTED, WPARAM(channel), LPARAM(reason));
}

void clip_copy_dlg::on_g2search_g2_saver_receive_scope_list(G2HSEARCH_G2 handle, int channel, const std::vector<G2SCOPE>& scopes, G2SEARCH_G2_SCOPE_TYPE::TYPE type)
{
    G2RETURN_IF_ASSERT(channel == _channel);

    PROCEED_CANCELED_SELFPLAYER(channel)

    int selected = 0;

    if (scopes.empty()) {
        MessageBox(L"No search data.", NULL, MB_OK);
        PostMessage(um_clip_copy_::UM_CANCELED_CLIPCOPY);
        return;
    }
    else if (scopes.size() == 1) {
        selected = 0;
    }
    else if (scopes.size() > 1) {
        select_scope_dlg dlg(this);
        dlg.set_scope_list(scopes);
        if (dlg.DoModal() == IDOK) {
            if (_search_g2->is_connected(_channel) &&
                selected == dlg.get_selected_index()) {
                // nothing
            }
            else {
                PostMessage(um_clip_copy_::UM_STOP_SAVE_VIDEO);
                return;
            }
        }
    }

    G2SCOPE scope = scopes[selected];
    std::set<int> channels;
    channels.insert(_channelext);
    unsigned int size = get_dir_free_size(_dirPath, true);
    unsigned __int64 total = __min(size, 64ui64 * 1024ui64 * 1024ui64 * 1024ui64 * 1024ui64);
    unsigned int slice_size = UINT_MAX;

    std::vector<int> ordered_set(channels.begin(),  channels.end());

    _search_g2->request_clipcopy_measure_size(_channel, channels, scope, total, ordered_set, true, slice_size, false);
    _search_g2->request_clipcopy_info(_channel, channels);
}

void clip_copy_dlg::on_g2search_g2_saver_receive_clipcopy_size(G2HSEARCH_G2 handle, int channel, G2CLIPCOPY_STATUS::TYPE status, const G2CLIPCOPY_SIZE_INFO& info, unsigned int progress)
{
    G2RETURN_IF_ASSERT(channel == _channel);

    PROCEED_CANCELED_SELFPLAYER(channel)

    TRACE("$$ SaveVideoPlay:: check size channel: %d, status: %d, size: %u\n", channel, status, info._info[0]._size);

    if (status == G2CLIPCOPY_STATUS::NOT_ENOUGH_SPACE) {        
        MessageBox(L"There is not sufficient space at storage media.", NULL, MB_OK);
        PostMessage(um_clip_copy_::UM_STOP_SAVE_VIDEO);
        return;
    }
    else if (status == G2CLIPCOPY_STATUS::NOT_COMPLETED) {
        SetTimer(TIMER_ID_CLIPCOPY_MEASURE_SIZE, 500, NULL);
    }
    else if (status == G2CLIPCOPY_STATUS::PARTIAL_COPIABLE) {
        TCHAR systemPath[_MAX_PATH] = { 0 };
        ::GetSystemDirectory(systemPath, _MAX_PATH);

        TCHAR drive[_MAX_DRIVE];
        TCHAR dir[_MAX_DIR];
        TCHAR fname[_MAX_FNAME];
        TCHAR ext[_MAX_EXT];
        _tsplitpath_s(systemPath, drive, dir, fname, ext);

        CString message;
        message.Format(((get_dir_free_size(_dirPath, true) >= client::max_::REQUIRED_SPACE_4) ?
                            _T("Do you want to copy to %s\\?\n(File size was limited to 4GB.)") :
                            _T("Not Enough Space in %s\\.\nDo you want to continue using the available space?")),
                        drive);

        if (MessageBox(message, NULL, MB_ICONQUESTION | MB_YESNO) == IDNO) {
            PostMessage(um_clip_copy_::UM_STOP_SAVE_VIDEO);
            return;
        }
    }

    G2RETURN_IF_FAIL(_search_g2->is_connected(_channel));

    _file.SetLength(info._info[0]._size);

    if (_usePassword) {
        _search_g2->request_clipcopy_password(_channel, _password.GetBuffer(0));
    }
    else {
        _search_g2->request_clipcopy_data(_channel);
    }
}

void clip_copy_dlg::on_g2search_g2_saver_receive_clipcopy_data(G2HSEARCH_G2 handle, int channel, unsigned __int64 offset, unsigned int size, const unsigned char* data, unsigned int progress)
{
    G2RETURN_IF_ASSERT(channel == _channel);

    PROCEED_CANCELED_SELFPLAYER(channel)

    TRACE("$$ SaveVideoPlay:: receive data channel: %d, offset: %I64d, size: %u, progress %d\n", channel, offset, size, progress);
    
    if (_progress != progress) {
        _progress = progress;
        PostMessage(um_clip_copy_::UM_UPDATE_CLIPCOPY, _progress);
    }

    try {
        _file.Seek(offset, CFile::begin);
        _file.Write(data, size);
    }
    catch (CFileException* e) {
        _search_g2->request_clipcopy_cancel(channel);

        CString error;
        e->GetErrorMessage(error.GetBuffer(1024), 1024);
        e->Delete();
        error.ReleaseBuffer();
        MessageBox(error, NULL, MB_OK | MB_ICONERROR);
    }
}

void clip_copy_dlg::on_g2search_g2_saver_receive_clipcopy_set_password(G2HSEARCH_G2 handle, int channel, unsigned int result)
{
    G2RETURN_IF_ASSERT(channel == _channel);
    PROCEED_CANCELED_SELFPLAYER(channel)

    _search_g2->request_clipcopy_data(channel);
}

void clip_copy_dlg::on_g2search_g2_saver_receive_clipcopy_canceled(G2HSEARCH_G2 handle, int channel, G2CLIPCOPY_ERROR::TYPE error)
{
    G2RETURN_IF_ASSERT(channel == _channel);

    KillTimer(TIMER_ID_CLIPCOPY_ON_CANCEL);
    PostMessage(um_clip_copy_::UM_CANCELED_CLIPCOPY);
}

void clip_copy_dlg::on_g2search_g2_saver_receive_clipcopy_job_started(G2HSEARCH_G2 handle, int channel, G2CLIPCOPY_JOB::TYPE job, unsigned int num, unsigned int total)
{
    G2RETURN_IF_ASSERT(channel == _channel);

    TRACE("$$ SaveVideoPlay:: jop started [%d]...\n", job);
    if (job == G2CLIPCOPY_JOB::MEASURE_SIZE) {
        _search_g2->request_clipcopy_size(channel);
    }
}

void clip_copy_dlg::on_g2search_g2_saver_receive_clipcopy_job_finished(G2HSEARCH_G2 handle, int channel, G2CLIPCOPY_JOB::TYPE job, unsigned int num, unsigned int total)
{
    G2RETURN_IF_ASSERT(channel == _channel);

    TRACE("$$ SaveVideoPlay:: jop finished [%d]...\n", job);
    if (job == G2CLIPCOPY_JOB::COPY_PLAYER) {
        PostMessage(um_clip_copy_::UM_COMPLETE_CLIPCOPY);
    }
}

//////////////////////////////////////////////////////////////////////////

void clip_copy_dlg::on_btn_start_stop(void)
{
    if (_started) {
        PostMessage(um_clip_copy_::UM_STOP_SAVE_VIDEO);
        return;
    }

    if (make_time_condition(_begin, _end) != true) {
        return;
    }

    if (CButton* chkPassword = (CButton*)GetDlgItem(IDC_CHK_SAVE_PW)) {
        _usePassword = (chkPassword->GetCheck() == BST_CHECKED);
    }

    if (_usePassword) {
        clip_copy_password_dlg dlg;
        if (dlg.DoModal() != IDOK) {
            MessageBox(L"Password required.", NULL, MB_OK);
            return;
        }
        _password = dlg.get_password();
    }

    // open file dlg
    _filePath = open_file_path();
    _dirPath = _filePath.Mid(0, _filePath.ReverseFind(_T('\\')));

    if (_filePath.IsEmpty()) {
        return;
    }

    enable_controls(false);
    _clipcopyStatus = CLIPCOPY;

    PostMessage(um_clip_copy_::UM_START_SAVE_VIDEO);
}


void clip_copy_dlg::on_chk_first(void)
{
    BOOL enable = (_chkFirst.GetCheck() == BST_CHECKED) ? FALSE : TRUE;
    _dtcFromDate.EnableWindow(enable);
    _dtcFromTime.EnableWindow(enable);
}

void clip_copy_dlg::on_chk_last(void)
{
    BOOL enable = (_chkLast.GetCheck() == BST_CHECKED) ? FALSE : TRUE;
    _dtcToDate.EnableWindow(enable);
    _dtcToTime.EnableWindow(enable);
}

//////////////////////////////////////////////////////////////////////////

bool clip_copy_dlg::make_time_condition(G2TIME& begin, G2TIME& end)
{
    g2_time_make_invalid(&begin);
    g2_time_make_invalid(&end);

    bool result = true;

    CTime fromDate, fromTime, toDate, toTime;
    _dtcFromDate.GetTime(fromDate);
    _dtcFromTime.GetTime(fromTime);
    _dtcToDate.GetTime(toDate);
    _dtcToTime.GetTime(toTime);

    CTime from(fromDate.GetYear(), fromDate.GetMonth(), fromDate.GetDay(), fromTime.GetHour(), fromTime.GetMinute(), fromTime.GetSecond());
    CTime to(toDate.GetYear(), toDate.GetMonth(), toDate.GetDay(), toTime.GetHour(), toTime.GetMinute(), toTime.GetSecond());

    if (_chkFirst.GetCheck() == BST_UNCHECKED && _chkLast.GetCheck() == BST_UNCHECKED) {
        if (from >= to) {
            MessageBox(L"The end time must be later than the start time.", NULL, MB_OK);
            result = false;
        }
    }

    if (_chkFirst.GetCheck() != BST_CHECKED) {
        g2_time_from_time32_t(&begin, (unsigned long)from.GetTime());
    }
    if (_chkLast.GetCheck() != BST_CHECKED) {
        g2_time_from_time32_t(&end, (unsigned long)to.GetTime());
    }

    return result;
}

bool clip_copy_dlg::create_video_file(const CString& path)
{
    if (is_exit_file(path)) {
        try {
            CFile::Remove(path);
        }
        catch (CFileException* e) {
            TRACE(_T("%d\n"), ::GetLastError());
            e->Delete();
            return false;
        }
    }

    if (_file.m_hFile != CFile::hFileNull) {
        _file.Flush();
        _file.Close();
    }

    bool result = (_file.Open(path, CFile::modeWrite | CFile::modeCreate | CFile::shareExclusive) == TRUE);

    if (result != true) {
        MessageBox(L"Cannot create file.", NULL, MB_OK);
    }

    return result;
}

bool clip_copy_dlg::remove_video_file(void)
{
    if (_file.m_hFile == CFile::hFileNull) {
        return true;
    }

    CString filepath = _file.GetFilePath();
    _file.Flush();
    _file.Close();

    try {
        CFile::Remove(filepath);
    }
    catch (CFileException* e) {
        CString cause;
        e->m_strFileName = filepath;
        e->GetErrorMessage(cause.GetBuffer(1024), 1024);
        e->Delete();
        cause.ReleaseBuffer();
        TRACE(_T("SaveVideo:: %s\n"), cause);
    }
    return true;
}

bool clip_copy_dlg::is_exit_file(const CString& path)
{
    CFileFind finder;
    return (finder.FindFile(path) == TRUE);
}

CString clip_copy_dlg::open_file_path(void)
{
    CString path;

    CTime time = CTime::GetCurrentTime();
    CString fileName;
    fileName.Format(_T("clip_%s.exe"), time.Format(_T("%Y-%m-%d_%H-%M-%S")).operator LPCTSTR());

    TCHAR cur_dir[_MAX_PATH];
    GetCurrentDirectory(_MAX_PATH, cur_dir);

    CFileDialog dlg(FALSE,
                    _T("exe"),
                    fileName,
                    OFN_DONTADDTORECENT | OFN_ENABLESIZING | OFN_OVERWRITEPROMPT | OFN_EXPLORER | OFN_FILEMUSTEXIST | OFN_PATHMUSTEXIST,
                    _T("Self-Player file(*.exe)|*.exe||"),
                    this);
    dlg.m_ofn.Flags |= OFN_NOCHANGEDIR;

    if (dlg.DoModal() == IDOK) {
        path = dlg.GetPathName();
    }

    return path;
}

unsigned int clip_copy_dlg::get_dir_free_size(const CString& path, bool expandable /*= false*/)
{
    const unsigned int SYSTEM_MARGINAL_SPACE = 100 * 1024 * 1024;

    TCHAR systemPath[_MAX_PATH] = { 0 };
    ::GetSystemDirectory(systemPath, _MAX_PATH);

    TCHAR drive[_MAX_DRIVE];
    TCHAR dir[_MAX_DIR];
    TCHAR fname[_MAX_FNAME];
    TCHAR ext[_MAX_EXT];
    _tsplitpath_s(systemPath, drive, dir, fname, ext);

    unsigned __int64 available = 0x0ui64;

    ULARGE_INTEGER ulFreeBytesAvailable;
    ULARGE_INTEGER ulTotalnumberOfBytes;
    ULARGE_INTEGER ulTotalNumberOfFreeBytes;

    if (::GetDiskFreeSpaceEx(path, &ulFreeBytesAvailable, &ulTotalnumberOfBytes, &ulTotalNumberOfFreeBytes) == TRUE) {
        available = ulFreeBytesAvailable.QuadPart;
        if (path.Find(drive) == 0) {
            available = (available < SYSTEM_MARGINAL_SPACE) ? 0 : available - SYSTEM_MARGINAL_SPACE;
        }
    }

    unsigned int bound = _UI32_MAX / 2;
    if (expandable) {
        TCHAR name[_MAX_FNAME] = { 0 };
        TCHAR system[16] = { 0 };
        unsigned long serial = 0;
        unsigned long componentLengthMax = 0;
        unsigned long flags = 0;

        CString root(drive);
        root += _T("\\");

        bool success = (::GetVolumeInformation(
                                root,
                                name,
                                _MAX_FNAME,
                                &serial,
                                &componentLengthMax,
                                &flags,
                                system,
                                64) == TRUE);

        if (success) {
            _tcsupr_s(system, 16);
            CString temp = system;

            if (temp == _T("NTFS") ||
                temp == _T("FAT32") ||
                temp == _T("FAT") ||
                temp == _T("CDFS")) {
                bound = client::max_::REQUIRED_SPACE_4;
            }
        }
    }
    ULARGE_INTEGER temp; temp.QuadPart = available;
    unsigned int dirfree = (temp.HighPart > 0 || temp.LowPart > bound) ? bound : temp.LowPart;

    return dirfree;
}

void clip_copy_dlg::enable_controls(bool enable /*= true*/)
{
    BOOL bEnable = enable ? TRUE : FALSE;

    _chkFirst.EnableWindow(bEnable);
    _chkLast.EnableWindow(bEnable);
    _dtcFromDate.EnableWindow(bEnable);
    _dtcFromTime.EnableWindow(bEnable);
    _dtcToDate.EnableWindow(bEnable);
    _dtcToTime.EnableWindow(bEnable);
    _btnCancel.EnableWindow(bEnable);
    _chkPassword.EnableWindow(bEnable);
    
    _progressBar.SetPos(0);

    if (enable) {
        _started = false;
        _btnStart.SetWindowTextW(L"Start");
    }
    else {
        _started = true;
        _btnStart.SetWindowText(L"Stop");
    }
}

//////////////////////////////////////////////////////////////////////////

LRESULT clip_copy_dlg::on_post_disconnected(WPARAM wParam, LPARAM lParam)
{
    remove_video_file();
    EndDialog(IDCANCEL);

    return 0L;
}

LRESULT clip_copy_dlg::on_post_start_save_video(WPARAM wParam, LPARAM lParam)
{
    _started = create_video_file(_filePath);
    
    if (_started) {
        if (_mode == CONNECT_SEARCH_G2 && _search_g2) {
            std::set<int> channels;
            channels.insert(_channelext);
            _search_g2->request_scope_list(_channel, _begin, _end, channels, G2PLAY_SCOPE_TYPE::CLIP_COPY);
        }
        else if (_search) {
            unsigned int cameras = 0x1 << _channelext;
            _search->request_clipcopy_scope(_channel, _begin, _end, cameras);
        }
        else {
            _started = false;
            MessageBox(L"Invalid search connection.", NULL, MB_OK);
        }
    }
    else {
        MessageBox(L"Failed create video file.", NULL, MB_OK);
    }

    if (_started == false) {
        PostMessage(um_clip_copy_::UM_CANCELED_CLIPCOPY);
        return 1L;
    }

    return 0L;
}

LRESULT clip_copy_dlg::on_post_stop_save_video(WPARAM wParam, LPARAM lParam)
{
    KillTimer(TIMER_ID_CLIPCOPY_ON_CANCEL);

    _clipcopyStatus = CANCELED;

    if (_mode == CONNECT_SEARCH_G2 && _search_g2)
        _search_g2->request_clipcopy_cancel(_channel);
    else if (_search)
        _search->request_clipcopy_cancel(_channel);

    SetTimer(TIMER_ID_CLIPCOPY_ON_CANCEL, 500, NULL);

    return 0L;
}

LRESULT clip_copy_dlg::on_post_canceled_clipcopy(WPARAM wParam, LPARAM lParam)
{
    if (_clipcopyStatus != STANBY) {
        remove_video_file();
        enable_controls(true);
        _clipcopyStatus = STANBY;
    }

    return 0L;
}

LRESULT clip_copy_dlg::on_post_complete_clipcopy(WPARAM wParam, LPARAM lParam)
{
    if (_file.m_hFile != CFile::hFileNull) {
        _file.Close();
    }
    _progressBar.SetPos(100);
    MessageBox(L"Saving completed.", NULL, MB_OK);

    enable_controls(true);
    _clipcopyStatus = STANBY;

    return 0L;
}

LRESULT clip_copy_dlg::on_post_update_clipcopy(WPARAM wParam, LPARAM lParam)
{
    int progress = static_cast<int>(wParam);

    _progressBar.SetPos(progress);

    return 0L;
}
