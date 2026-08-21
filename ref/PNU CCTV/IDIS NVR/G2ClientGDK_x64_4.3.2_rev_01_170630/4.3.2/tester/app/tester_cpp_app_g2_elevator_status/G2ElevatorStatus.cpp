// G2ElevatorStatus.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"

#include <include/g2_foundation.h>
#include <include/g2_main.h>
#include <sampler/cpp/g2client_watch.h>
#include <sampler/cpp/g2client_watch_listener.h>
#include <algorithm>
#include <boost/thread/condition_variable.hpp>
#include <boost/program_options.hpp>

//////////////////////////////////////////////////////////////////////////

class adaptor_fixture : public client::g2watch_listener
{
public:
    struct OPTIONS {
        OPTIONS(void)
            : ni()
            , status()
            , timeout(10)
            , retry(0)
        {
            memset(&ni, 0, sizeof(ni));
            memset(&status, 0, sizeof(status));
            ni._address_type = G2NETWORK_INFO::ADDRESS_TYPE_IPV4;
        }

        G2NETWORK_INFO ni;
        G2LIVE_ELEVATOR_STATUS_INFO status;
        int timeout;
        int retry;
    };

public:
    explicit adaptor_fixture(client::g2watch* adaptor)
        : _adaptor(adaptor)
        , _options()
        , _seq_number(-1)
        , _continue(true)
        , _cond_entered(false)
    {
    }

public:
    const OPTIONS& options(void) const { return _options; }
          OPTIONS& options(void) { return _options; }
    bool is_continue(void) const { return _continue; }

private:
    client::g2watch* _adaptor;
    OPTIONS _options;
    unsigned int _seq_number;
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

        return (_seq_number == _options.status._seq_number);
    }

    void print(void)
    {
        wprintf(L"\tG2LIVE_ELEVATOR_STATUS_INFO\n");
        wprintf(L"\tseq: %u, received(%u)\n", _options.status._seq_number, _seq_number);
    }

public:
    virtual void on_g2watch_connected(G2HWATCH handle, int channel)
    {
        if (_adaptor->is_support(channel, G2LIVE_SUPPORT::SI_ELEVATOR_STATUS_INFO)) {
            _adaptor->send_elevator_status_info(channel, &options().status);
        }
        else {
            _adaptor->disconnect(channel);
            _continue = false;
            std::cout << "adaptor::elevator status info not supported" << std::endl;
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
    virtual void on_g2watch_receive_network_alarm_result(G2HWATCH handle, int channel, const G2LIVE_NETWORK_ALARM_RESULT& result) {}
    virtual void on_g2watch_receive_elevator_status_info_response(G2HWATCH handle, int channel, unsigned int seq_number)
    {
        if (_options.status._seq_number == seq_number) {
            _seq_number = seq_number;
            _adaptor->disconnect(channel);
        }
    }
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
    boost::program_options::options_description options_desc("app_g2_elevator_status");
    options_desc.add_options()
        ("help,h", "display current help message")
        ("address,a", boost::program_options::wvalue<std::wstring>(), "server(device) address ip:port")
        ("seq", boost::program_options::wvalue<unsigned int>(), "sequence number")
        ("id", boost::program_options::wvalue<unsigned short>(), "device number or id")
        ("floor", boost::program_options::wvalue<float>(), "floor floating-point value")
        ("door", boost::program_options::wvalue<unsigned short>(), "door status\n0:UNKNOWN, 1:CLOSING, 2:CLOSE, 3:OPENING, 4:OPEN")
        ("direction", boost::program_options::wvalue<unsigned short>(), "direction\n0:UNKNOWN, 1:DOWN, 2:STOP, 3:UP")
        ("mode", boost::program_options::wvalue<unsigned short>(), "mode\n0:UNKNOWN, 1:MANUAL, 2:AUTO")
        ("data", boost::program_options::wvalue<std::wstring>(), "user defined data")
        ("timeout", boost::program_options::wvalue<int>(), "timeout(seconds) per operation, default is 10 seconds")
        ("retry", boost::program_options::wvalue<int>(), "retry count")
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
    G2LIVE_ELEVATOR_STATUS_INFO esi = { 0 };
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
        std::cout << "sample: tester_cpp_app_g2_elevator_status --address \"127.0.0.1:8016\" --seq 0 --id 0 --floor 1.5 --door 1 --direction 1 --mode 2 --data \"this is test data\"" << std::endl;
        return 0;
    }

    if (options.count("seq")) {
        esi._seq_number = options["seq"].as<unsigned int>();
    }
    if (options.count("id")) {
        esi._status._id = options["id"].as<unsigned short>();
    }
    if (options.count("floor")) {
        esi._status._floor = options["floor"].as<float>();
    }
    if (options.count("door")) {
        esi._status._door_status = static_cast<unsigned char>(options["door"].as<unsigned short>());
    }
    if (options.count("direction")) {
        esi._status._direction = static_cast<unsigned char>(options["direction"].as<unsigned short>());
    }
    if (options.count("mode")) {
        esi._status._mode = static_cast<unsigned char>(options["mode"].as<unsigned short>());
    }
    if (options.count("timeout")) {
        timeout = options["timeout"].as<int>();
    }
    if (options.count("retry")) {
        retry = options["retry"].as<int>();
    }
    if (options.count("data")) {
        std::wstring a = options["data"].as<std::wstring>();
        swprintf(esi._status._additional._string, 128, L"%s", a.c_str());
        esi._status._additional._len = std::min<unsigned int>(127, (unsigned int)a.length());
    }

    //////////////////////////////////////////////////////////////////////////

    g2_main_app_initialize(G2LANGUAGE_ID::ENGLISH);
    g2_main_verbose_set_level(G2MAIN_VERBOSE::ERRORS);

    client::g2watch adaptor;
    adaptor_fixture fixture(&adaptor);
    adaptor.startup(2, NULL);
    adaptor.set_listener(&fixture);

    fixture.options().status = esi;
    fixture.options().ni = ni;
    fixture.options().timeout = timeout;
    fixture.options().retry = retry;

    do {
        if (fixture.operation()) {
            break;
        }
    }
    while (--retry > 0 && fixture.is_continue());

    fixture.print();
    adaptor.cleanup();

    g2_main_app_finalize();

	return 0;
}
