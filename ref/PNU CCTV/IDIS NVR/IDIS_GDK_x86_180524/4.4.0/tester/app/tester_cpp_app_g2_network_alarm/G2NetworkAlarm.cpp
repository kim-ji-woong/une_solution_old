// G2NetworkAlarm.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"

#include <include/g2_foundation.h>
#include <include/g2_main.h>
#include <sampler/cpp/g2client_watch.h>
#include <sampler/cpp/g2client_watch_listener.h>
#include <algorithm>
#include <iostream>
#include <boost/thread/condition_variable.hpp>
#include <boost/program_options.hpp>

//////////////////////////////////////////////////////////////////////////

class adaptor_fixture : public client::g2watch_listener
{
public:
    struct OPTIONS {
        OPTIONS(void)
            : ni()
            , alarm()
            , timeout(10)
            , retry(0)
        {
            memset(&ni, 0, sizeof(ni));
            memset(&alarm, 0, sizeof(alarm));
            ni._address_type = G2NETWORK_INFO::ADDRESS_TYPE_IPV4;
        }

        G2NETWORK_INFO ni;
        G2LIVE_NETWORK_ALARM_INFO alarm;
        int timeout;
        int retry;
    };

public:
    explicit adaptor_fixture(client::g2watch* adaptor)
        : _adaptor(adaptor)
        , _options()
        , _res()
        , _continue(true)
        , _cond_entered(false)
    {
        memset(&_res, 0, sizeof(_res));
        _res._result = G2LIVE_NETWORK_ALARM_RESULT::FAIL_UNKNOWN;
        _res._event._type = G2EVENT::TYPE_UNKNOWN;
    }

public:
    const OPTIONS& options(void) const { return _options; }
          OPTIONS& options(void) { return _options; }
    G2LIVE_NETWORK_ALARM_RESULT& result(void) { return _res; }
    bool is_continue(void) const { return _continue; }

private:
    client::g2watch* _adaptor;
    OPTIONS _options;
    G2LIVE_NETWORK_ALARM_RESULT _res;
    bool _continue;
    bool _cond_entered;
    boost::mutex _mutex;
    boost::condition_variable_any _cond;

public:
    bool operation(void)
    {
        boost::unique_lock<boost::mutex> lock(_mutex);

        int channel = _adaptor->connect_ras_event(options().ni);
        if (channel >= 0) {
            if (_cond_entered) {
                _cond_entered = false;
            }
            else {
                _cond.timed_wait(lock, boost::posix_time::seconds(options().timeout));
                _cond_entered = false;
            }

            if (_adaptor->is_disconnectable(channel)) {
                _adaptor->disconnect(channel);
            }

            lock.unlock();

            for ( ; _adaptor->is_disconnecting(channel); ) {
                boost::this_thread::sleep(boost::posix_time::milliseconds(1));
            }

            lock.lock();
        }

        return (result()._result == G2LIVE_NETWORK_ALARM_RESULT::RESULT_OK ||
                result()._result == G2LIVE_NETWORK_ALARM_RESULT::FAIL_NO_OPERATION ||
                result()._result == G2LIVE_NETWORK_ALARM_RESULT::FAIL_DEVICE_INACTIVE);
    }

    static void print(const G2LIVE_NETWORK_ALARM_RESULT& res)
    {
        wprintf(L"\tG2LIVE_NETWORK_ALARM_RESULT\n");
        wprintf(L"\tseq: %d\n", res._seq_number);
        wprintf(L"\tresult: %s\n", res._result == G2LIVE_NETWORK_ALARM_RESULT::RESULT_OK ? L"RESULT_OK" :
                                   res._result == G2LIVE_NETWORK_ALARM_RESULT::FAIL_NO_OPERATION ? L"NO_OPERATION" :
                                   res._result == G2LIVE_NETWORK_ALARM_RESULT::FAIL_DEVICE_INACTIVE ? L"DEVICE_INACTIVE" : L"FAIL");
        G2STRING_64 s = { 0 };
        g2_get_string_event_type_g2(res._event._type, &s);
        wprintf(L"\tevent: %s\n", s._string);
        wprintf(L"\tlevel: %s\n", res._event._level == G2EVENT::LEVEL_EMERGENCY ? L"EMERGENCY" :
                                  res._event._level == G2EVENT::LEVEL_ERROR ? L"ERROR" : L"NORMAL");
        if (res._event._network_alarm._version[1] != 0 ||
            res._event._network_alarm._version[0] != 0) {
            wprintf(L"\tversion: %d.%d\n", res._event._network_alarm._version[1], res._event._network_alarm._version[0]);
        }
        if (res._event._network_alarm._event) {
            wprintf(L"\tuser_defined: %d\n", res._event._network_alarm._event);
        }
        if (g2_time_is_valid(&res._event._network_alarm._time)) {
            g2_main_date_time_formatter_get_string_date_time(&res._event._network_alarm._time, &s);
            wprintf(L"\ttime: %s\n", s._string);
        }
        if (res._event._network_alarm._msec) {
            wprintf(L"\tmsec: %lld\n", res._event._network_alarm._msec);
        }
        wprintf(L"\tdata: %s\n", res._event._data._string);
    }

public:
    virtual void on_g2watch_connected(G2HWATCH handle, int channel)
    {
        if (_adaptor->is_support(channel, G2LIVE_SUPPORT::NETWORK_ALARM_G2)) {
            _adaptor->send_network_alarm_info(channel, &options().alarm);
        }
        else {
            _adaptor->disconnect(channel);
            _continue = false;
            std::cout << "adaptor::network alarm not supported" << std::endl;
        }
    }
    virtual void on_g2watch_disconnected(G2HWATCH handle, int channel, G2DISCONNECT_REASON::TYPE reason)
    {
        boost::unique_lock<boost::mutex> lock(_mutex);

        if (reason == G2DISCONNECT_REASON::INVALID_VERSION ||
            reason == G2DISCONNECT_REASON::LOGIN_FAIL ||
            reason == G2DISCONNECT_REASON::NO_AUTHORITY ||
            reason == G2DISCONNECT_REASON::NOT_SUPPORT_PRODUCT) {
            _continue = false;
        }

        if (reason != G2DISCONNECT_REASON::LOGOUT) {
            G2STRING_128 s = { 0 };
            g2_get_string_disconnect_reason(reason, &s);
            if (s._len) {
                wprintf(L"disconnected::reason::(%d) %s\n", reason, s._string);
            }
        }

        _cond_entered = true;
        _cond.notify_all();
    }
    virtual void on_g2watch_receive_frame_data(G2HWATCH handle, int channel, const G2FRAME& frame) {}
    virtual void on_g2watch_receive_event(G2HWATCH handle, int channel, const G2EVENT_INFO& ei) {}
    virtual void on_g2watch_receive_device_status(G2HWATCH handle, int channel, const G2DEVICE_STATUS& status) {}
    virtual void on_g2watch_receive_ptz_preset(G2HWATCH handle, int channel, int camera, const G2LIVE_PTZ_PRESET& preset) {}
    virtual void on_g2watch_receive_ptz_menu(G2HWATCH handle, int channel, int camera, const G2LIVE_PTZ_MENU& menu) {}
    virtual void on_g2watch_receive_camera_title_idr(G2HWATCH handle, int channel, int camera, const std::wstring& title) {}
    virtual void on_g2watch_receive_text_in(G2HWATCH handle, int channel, const G2TEXT_IN& data) {}
    virtual void on_g2watch_receive_network_camera_information(G2HWATCH handle, int channel) {}
    virtual void on_g2watch_receive_audio_out_not_available(G2HWATCH handle, int channel) {}
    virtual void on_g2watch_receive_command_result_control_color_status(G2HWATCH handle, int channel, int camera, const G2LIVE_COMMAND_CONTROL_COLOR& control, const G2LIVE_COMMAND_CONTROL_COLOR_RANGE& range) {}
    virtual void on_g2watch_receive_command_result_control_color(G2HWATCH handle, int channel, int camera, const G2LIVE_COMMAND_CONTROL_COLOR& control, G2LIVE_COMMAND_RESULT::TYPE result) {}
    virtual void on_g2watch_receive_command_result_control_ptz_status(G2HWATCH handle, int channel, int camera, const G2LIVE_COMMAND_CONTROL_PTZ& control, const G2LIVE_COMMAND_CONTROL_PTZ_RANGE& range) {}
    virtual void on_g2watch_receive_command_result_control_ptz(G2HWATCH handle, int channel, int camera, G2LIVE_COMMAND_RESULT::TYPE result) {}
    virtual void on_g2watch_receive_network_alarm_result(G2HWATCH handle, int channel, const G2LIVE_NETWORK_ALARM_RESULT& result)
    {
        _res = result;
        _adaptor->disconnect(channel);
    }
    virtual void on_g2watch_receive_elevator_status_info_response(G2HWATCH handle, int channel, unsigned int seq_number) {}
    virtual void on_g2watch_receive_instant_recording_start(G2HWATCH handle, int channel, const G2PARAM_INSTANT_RECORDING_CHANNEL_STATUS& status) {}
    virtual void on_g2watch_receive_instant_recording_stop(G2HWATCH handle, int channel, G2INSTANT_RECORDING_RESULT::TYPE result) {}
    virtual void on_g2watch_receive_instant_recording_status(G2HWATCH handle, int channel, const G2PARAM_INSTANT_RECORDING_CHANNEL_STATUS& status) {}
    virtual void on_g2watch_audio_streaming_started(G2HWATCH handle, int channel, int camera) {}
    virtual void on_g2watch_audio_streaming_stopped(G2HWATCH handle, int channel, int camera) {}
    virtual void on_g2watch_audio_capturing_started(G2HWATCH handle, int channel, int camera) {}
    virtual void on_g2watch_audio_capturing_stopped(G2HWATCH handle, int channel, int camera) {}
    virtual void on_g2watch_probe_session_profile(G2HWATCH handle, int channel, const G2PROBE_SESSION_PROFILE& probe) {}
};

//////////////////////////////////////////////////////////////////////////

int _tmain(int argc, _TCHAR* argv[])
{
    boost::program_options::options_description options_desc("app_g2_network_alarm");
    options_desc.add_options()
        ("help,h", "display current help message")
        ("address,a", boost::program_options::wvalue<std::wstring>(), "server(device) address ip:port")
        ("seq", boost::program_options::wvalue<int>(), "sequence number")
        ("id", boost::program_options::wvalue<int>(), "network alarm device number or id")
        ("event", boost::program_options::wvalue<int>(), "user defined event type")
        ("on", "network alarm on event")
        ("off", "network alarm off event")
        ("timeout", boost::program_options::wvalue<int>(), "timeout(seconds) per operation, default is 10 seconds")
        ("retry", boost::program_options::wvalue<int>(), "retry count")
        ("level", boost::program_options::wvalue<int>(), "event level\n0:NORMAL, 1:EMERGENCY, 2:ERROR")
        ("data", boost::program_options::wvalue<std::wstring>(), "user defined data")
        ;
    boost::program_options::variables_map options;
    try {
        boost::program_options::store(boost::program_options::wcommand_line_parser(__argc, __wargv).options(options_desc).run(), options);
        boost::program_options::notify(options);
    }
    catch (const std::exception& e) {
        std::cout << L"program_options::error::" << e.what() << std::endl;
        options.clear();
    }
    catch (...) {
        options.clear();
    }

    G2NETWORK_INFO ni = { 0 };
    G2LIVE_NETWORK_ALARM_INFO nai = { 0 };
    nai._version[1] = 0;
    nai._version[0] = 1;
    nai._on = true;
    int retry = 5;
    int timeout = 10;
    bool need_help = false;

    if (options.empty()) need_help = true;
    if (options.count("address")) {
        std::wstring a = options["address"].as<std::wstring>();
        size_t pos = a.find(L':');
        if (pos == std::wstring::npos) {
            need_help = true;
        }
        else {
            ni._address_type = G2NETWORK_INFO::ADDRESS_TYPE_IPV4;
            swprintf(ni._address, sizeof(ni._address) / sizeof(ni._address[0]), L"%s", a.substr(0, pos).c_str());
            if (pos < a.length()) {
                try {
                    ni._port[G2NETWORK_INFO::WATCH_PORT] = boost::lexical_cast<unsigned short>(a.substr(pos + 1));
                }
                catch (boost::bad_lexical_cast&) {}
                catch (...) {}
            }

            if (ni._port[G2NETWORK_INFO::WATCH_PORT] == 0) {
                need_help = true;
            }
        }
    }
    else {
        need_help = true;
    }

    if (options.count("help") || need_help) {
        std::cout << std::endl;
        std::cout << options_desc << std::endl;
        std::cout << "sample: tester_cpp_app_g2_network_alarm --address \"127.0.0.1:8016\" --on --data \"this is test data\"" << std::endl;
        return 0;
    }

    if (options.count("seq")) {
        nai._seq_number = options["seq"].as<int>();
    }
    if (options.count("id")) {
        nai._id = options["id"].as<int>();
    }
    if (options.count("event")) {
        nai._event = options["event"].as<int>();
    }
    if (options.count("on")) {
        nai._on = true;
    }
    else if (options.count("off")) {
        nai._on = false;
    }
    if (options.count("timeout")) {
        timeout = options["timeout"].as<int>();
    }
    if (options.count("retry")) {
        retry = options["retry"].as<int>();
    }
    if (options.count("level")) {
        nai._level = options["level"].as<int>();
    }
    if (options.count("data")) {
        std::wstring a = options["data"].as<std::wstring>();
        swprintf(nai._data._string, 128, L"%s", a.c_str());
        nai._data._len = std::min<unsigned int>(127, (unsigned int)a.length());
    }

    //////////////////////////////////////////////////////////////////////////

    g2_main_app_initialize(G2LANGUAGE_ID::ENGLISH);
    g2_main_verbose_set_level(G2MAIN_VERBOSE::ERRORS);

    client::g2watch adaptor;
    adaptor_fixture fixture(&adaptor);
    adaptor.startup(2, NULL);
    adaptor.set_listener(&fixture);

    g2_time_get_current(&nai._time);
    nai._msec = GetTickCount();
    fixture.options().alarm = nai;
    fixture.options().ni = ni;
    fixture.options().timeout = timeout;
    fixture.options().retry = retry;

    do {
        if (fixture.operation()) {
            break;
        }
    }
    while (--retry > 0 && fixture.is_continue());

    fixture.print(fixture.result());
    adaptor.cleanup();

    g2_main_app_finalize();

	return 0;
}
