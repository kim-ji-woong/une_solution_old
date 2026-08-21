// G2WatchImageOneTime.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"

#include <include/g2_foundation.h>
#include <include/g2_main.h>
#include <sampler/cpp/g2client_decoder.h>
#include <sampler/cpp/g2client_watch.h>
#include <sampler/cpp/g2client_watch_listener.h>
#include <SDL/include/SDL.h>
#include <boost/thread/condition_variable.hpp>
#include <boost/program_options.hpp>
#include <algorithm>
#include <iterator>
#include <iomanip>
#include <iostream>
#include <string>

#pragma comment(lib, "SDL2main.lib")
#pragma comment(lib, "SDL2.lib")

//////////////////////////////////////////////////////////////////////////

class adaptor_fixture : public client::g2watch_listener
{
public:
    struct OPTIONS {
        OPTIONS(void) : ni(), streams(), renderer(NULL), surface(NULL)
        {
            memset(&ni, 0, sizeof(ni));
            ni._address_type = G2NETWORK_INFO::ADDRESS_TYPE_IPV4;
        }

        G2NETWORK_INFO ni;
        std::set<std::pair<int, int> > streams;
        SDL_Renderer* renderer;
        SDL_Texture* surface;
    };

public:
    explicit adaptor_fixture(client::g2watch* adaptor)
        : _adaptor(adaptor)
        , _options()
        , _channel(-1)
        , _cond_entered(false)
    {
        _decoder.startup(2);
    }

public:
    const OPTIONS& options(void) const { return _options; }
          OPTIONS& options(void) { return _options; }

    void cleanup(void) { _decoder.cleanup(); }

protected:
    client::g2watch* _adaptor;
    OPTIONS _options;
    int _channel;

private:
    bool _cond_entered;
    mutable boost::mutex _mutex;
    boost::condition_variable_any _cond;

private:
    client::g2decoder _decoder;
    std::vector<unsigned char> _buf_decode;
    std::vector<unsigned char> _buf_filter;

public:
    bool connect(void)
    {
        boost::unique_lock<boost::mutex> lock(_mutex);
        bool res = false;
        if (_channel >= 0 && _adaptor->is_connected(_channel)) {
            return true;
        }
        _channel = _adaptor->connect_ras(options().ni);
        if (_channel >= 0) {
            if (_cond_entered) {
                _cond_entered = false;
            }
            else {
                _cond.wait(lock);
                _cond_entered = false;
            }

            lock.unlock();

            for ( ; _adaptor->is_connecting(_channel); ) {
                boost::this_thread::sleep(boost::posix_time::milliseconds(1));
            }

            lock.lock();

            res = _adaptor->is_connected(_channel);
        }
        if (!res) {
            _channel = -1;
            std::cout << "connect failed.." << std::endl;
        }
        return res;
    }

    void disconnect(void)
    {
        int channel = _channel;
        if (_adaptor->is_disconnectable(channel)) {
            boost::unique_lock<boost::mutex> lock(_mutex);

            _adaptor->disconnect(channel);

            if (_cond_entered) {
                _cond_entered = false;
            }
            else {
                _cond.wait(lock);
                _cond_entered = false;
            }

            lock.unlock();

            for ( ; _adaptor->is_disconnecting(channel); ) {
                boost::this_thread::sleep(boost::posix_time::milliseconds(1));
            }

            lock.lock();
        }
    }

    void change_stream(int stream_id = -1)
    {
        if (_adaptor->is_connected(_channel)) {
            std::set<std::pair<int, int> >& streams = options().streams;
            if (stream_id >= 0) {
                for (std::set<std::pair<int, int> >::iterator itr = streams.begin();
                    itr != streams.end();
                    ++itr) {
                    std::pair<int, int>& stream = *itr;
                    stream.second = stream_id;
                }
            }
            std::cout << "set_camera_stream_set.." << std::endl;
            _adaptor->set_camera_stream_set(_channel, streams, streams);
        }
    }

    void video_play(const G2FRAME& frame)
    {
        // if you want to use an external decoder, reference plain pointer.
        // plain video stream data is [frame._plain_ptr, frame._plain_ptr + frame._index._plain_size)

        G2SIZE res = { SDL_max(720, frame._width), SDL_max(576, frame._height) };

        size_t buf_len = res.cx * res.cy * 2;
        if (_buf_decode.size() < buf_len) {
            _buf_decode.resize(buf_len);
            _buf_filter.resize(buf_len);
        }

        G2DECODER_VIDEO_PARAM param = { 0 };
        param._frame = &frame;
        param._format = G2DECODER_VIDEO_PIX_FORMAT::YV12;
        param._buf_decompress = &_buf_decode[0];
        param._buf_filter = &_buf_filter[0];
        param._buf_decompress_len = static_cast<unsigned int>(_buf_decode.size());
        param._buf_filter_len = static_cast<unsigned int>(_buf_filter.size());
        param._decoder_id = 0;
        param._threads = 0;

        bool ret = _decoder.decompress(param);
        if (ret != false &&
            param._result._result == G2DECODER_VIDEO_RESULT::SUCCESS &&
            frame._extra._display) {
            SDL_Rect rect_texture = { 0 };
            SDL_QueryTexture(options().surface, NULL, NULL, &rect_texture.w, &rect_texture.h);

            if (rect_texture.w != param._result._width ||
                rect_texture.h != param._result._height) {
                SDL_RenderClear(options().renderer);
                SDL_DestroyTexture(options().surface);
                options().surface = SDL_CreateTexture(options().renderer, SDL_PIXELFORMAT_YV12, SDL_TEXTUREACCESS_STATIC, param._result._width, param._result._height);
                SDL_RenderSetLogicalSize(options().renderer, param._result._width, param._result._height);
            }

            unsigned char* ptr = &_buf_decode[0];
            unsigned char* YV12[3] = { 0 };
            YV12[0] = ptr;
            YV12[1] = ptr + param._result._width * param._result._height;
            YV12[2] = YV12[1] + (param._result._width * param._result._height >> 2);

            SDL_UpdateYUVTexture(options().surface, NULL, YV12[0], param._result._width, YV12[2], param._result._width >> 1, YV12[1], param._result._width >> 1);
            SDL_RenderCopy(options().renderer, options().surface, NULL, NULL);
            SDL_RenderPresent(options().renderer);
        }
    }

    void print_help(void)
    {
        std::cout << "F1    Show Help" << std::endl;
        std::cout << "Esc   Exit Program" << std::endl;
        std::cout << "C     Connect" << std::endl;
        std::cout << "D     Disconnect" << std::endl;
        std::cout << "1     Stream 1" << std::endl;
        std::cout << "2     Stream 2" << std::endl;
        std::cout << "3     Stream 3" << std::endl;
        std::cout << "4     Stream 4" << std::endl;
        std::cout << std::setfill('-') << std::setw(64);
        std::cout << "" << std::endl;
    }

public:
    virtual void on_g2watch_connected(G2HWATCH handle, int channel)
    {
        boost::unique_lock<boost::mutex> lock(_mutex);

        _adaptor->set_camera_stream_set(channel, options().streams, options().streams);
        _cond_entered = true;
        _cond.notify_all();

        std::wcout << L"connected(" << channel << L") " << std::endl;
    }
    virtual void on_g2watch_disconnected(G2HWATCH handle, int channel, G2DISCONNECT_REASON::TYPE reason)
    {
        boost::unique_lock<boost::mutex> lock(_mutex);

        G2STRING_128 s = { 0 };
        g2_get_string_disconnect_reason(reason, &s);
        if (s._len) {
            std::wcout << L"disconnected(" << channel << L")::reason(" << reason << L") " << s._string << std::endl;
        }

        _cond_entered = true;
        _cond.notify_all();
        _channel = -1;

        if (reason != G2DISCONNECT_REASON::LOGOUT &&
            reason != G2DISCONNECT_REASON::LOGIN_FAIL) {
            SDL_Event evt = { 0 };
            evt.type = SDL_KEYUP;
            evt.key.keysym.sym = SDLK_c;
            SDL_PushEvent(&evt);
        }
    }
    virtual void on_g2watch_receive_frame_data(G2HWATCH handle, int channel, const G2FRAME& frame)
    {
        std::wcout << L"receive frame data - "
                   << L"channel: " << channel
                   << L", camera: " << frame._index._channel
                   << L", stream: " << frame._index._stream_id
                   << L", res: " << frame._width << L" x " << frame._height
                   << std::endl;
        video_play(frame);
    }
    virtual void on_g2watch_receive_event(G2HWATCH handle, int channel, const G2EVENT_INFO& ei)
    {
        G2STRING_64 evt;
        g2_get_string_event_type(ei._level1, ei._level2, &evt);
        std::wcout << L"on receive event: " << evt._string << std::endl;
    }
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

static bool refresh_loop_wait_event(SDL_Event* e, int interval)
{
    bool ret = false;
    DWORD begin = ::GetTickCount();
    int limit = (interval > 0 ? interval : 10) * 1000;
    do {
        SDL_PumpEvents();
        if (SDL_PeepEvents(e, 1, SDL_GETEVENT, SDL_FIRSTEVENT, SDL_LASTEVENT)) {
            ret = true;
        }
        else {
            boost::this_thread::sleep(boost::posix_time::milliseconds(1));
        }
    }
    while (!ret && ((::GetTickCount() - begin) < limit));
    return ret;
}

int _tmain(int argc, _TCHAR* argv[])
{
    boost::program_options::options_description options_desc("app_g2_watch_image_onetime");
    options_desc.add_options()
        ("help,h", "display current help message")
        ("address,a", boost::program_options::wvalue<std::wstring>(), "server(device) address ip:port")
        ("user", boost::program_options::wvalue<std::wstring>(), "user account id")
        ("password", boost::program_options::wvalue<std::wstring>(), "user account password")
        ("camera", boost::program_options::wvalue<int>(), "camera channel(or number) of device")
        ("stream", boost::program_options::wvalue<int>(), "stream number of camera")
        ("interval", boost::program_options::wvalue<int>(), "get image interval(sec), default 10 sec")
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
    int camera = 0;
    int stream = 0;
    int interval = 0;
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
        std::cout << "sample: tester_cpp_app_g2_watch_image filter --address \"127.0.0.1:8016\" --user \"admin\" --password \"\" --camera 0 --stream 0" << std::endl;
        return 0;
    }

    if (options.count("user")) {
        std::wstring a = options["user"].as<std::wstring>();
        swprintf(ni._user_id, sizeof(ni._user_id) / sizeof(ni._user_id[0]), L"%s", a.c_str());
    }
    if (options.count("password")) {
        std::wstring a = options["password"].as<std::wstring>();
        swprintf(ni._password, sizeof(ni._password) / sizeof(ni._password[0]), L"%s", a.c_str());
    }
    if (options.count("camera")) {
        camera = options["camera"].as<int>();
    }
    if (options.count("stream")) {
        stream = options["stream"].as<int>();
    }
    if (options.count("interval")) {
        interval = options["interval"].as<int>();
    }
    
    //////////////////////////////////////////////////////////////////////

    SDL_Init(SDL_INIT_VIDEO);
    SDL_SetHint(SDL_HINT_RENDER_SCALE_QUALITY, "linear");
    SDL_Window* window = SDL_CreateWindow("GDK G2WatchImageOneTime", SDL_WINDOWPOS_UNDEFINED, SDL_WINDOWPOS_UNDEFINED, 960, 480, SDL_WINDOW_INPUT_FOCUS | SDL_WINDOW_SHOWN | SDL_WINDOW_RESIZABLE);
    SDL_Renderer* renderer = SDL_CreateRenderer(window, -1, SDL_RENDERER_ACCELERATED | SDL_RENDERER_PRESENTVSYNC);
    SDL_SetRenderDrawColor(renderer, 0, 0, 0, 255);
    SDL_RenderClear(renderer);
    SDL_RenderPresent(renderer);
    SDL_Texture* surface = SDL_CreateTexture(renderer, SDL_PIXELFORMAT_YV12, SDL_TEXTUREACCESS_STATIC, 1920, 1080);
    SDL_EventState(SDL_SYSWMEVENT, SDL_IGNORE);
    SDL_EventState(SDL_USEREVENT, SDL_IGNORE);

    //////////////////////////////////////////////////////////////////////

    g2_main_app_initialize(G2LANGUAGE_ID::ENGLISH);
    g2_main_verbose_set_level(G2MAIN_VERBOSE::ERRORS);

    client::g2watch adaptor;
    adaptor_fixture fixture(&adaptor);
    adaptor.startup(2, NULL);
    adaptor.set_listener(&fixture);

    fixture.options().ni = ni;
    fixture.options().streams.insert(std::make_pair(camera, stream));
    fixture.options().renderer = renderer;
    fixture.options().surface = surface;
    fixture.print_help();
    fixture.connect();

    for ( ; ; ) {
        SDL_Event evt;
        if (refresh_loop_wait_event(&evt, interval)) {
            if (evt.type == SDL_KEYUP) {
                size_t key = evt.key.keysym.sym;
                if (key == SDLK_ESCAPE) {
                    break;
                }
                else if (key == SDLK_F1) {
                    fixture.print_help();
                }
                else if (key == SDLK_1) {
                    fixture.change_stream(0);
                }
                else if (key == SDLK_2) {
                    fixture.change_stream(1);
                }
                else if (key == SDLK_3) {
                    fixture.change_stream(2);
                }
                else if (key == SDLK_4) {
                    fixture.change_stream(3);
                }
                else if (key == SDLK_d) {
                    fixture.disconnect();
                }
                else if (key == SDLK_c) {
                    fixture.connect();
                }
            }
            else if (evt.type == SDL_QUIT) {
                break;
            }
        }
        else {
            fixture.change_stream();
        }
    }

    fixture.disconnect();
    fixture.cleanup();
    adaptor.cleanup();

    g2_main_app_finalize();

    //////////////////////////////////////////////////////////////////////

    SDL_DestroyTexture(fixture.options().surface);
    SDL_DestroyRenderer(fixture.options().renderer);
    SDL_DestroyWindow(window);
    SDL_Quit();

    return 0;
}
