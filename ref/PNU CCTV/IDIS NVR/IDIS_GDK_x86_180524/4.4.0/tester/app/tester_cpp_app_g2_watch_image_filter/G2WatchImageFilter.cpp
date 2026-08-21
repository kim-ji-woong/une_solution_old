// G2WatchImageFilter.cpp : Defines the entry point for the console application.
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
        OPTIONS(void) : ni(), cameras(), threads(0), renderer(NULL), surface(NULL)
                      , on_filter_median(true)
                      , on_filter_sharpen(true)
                      , on_filter_equalize(true)
                      , on_filter_high_boost(false)
                      , on_filter_edge_detect(false)
        {
            memset(&ni, 0, sizeof(ni));
            ni._address_type = G2NETWORK_INFO::ADDRESS_TYPE_IPV4;
        }

        std::string to_string_filters(void) const
        {
            std::string s;
            if (on_filter_median) {
                if (!s.empty()) s += "/";
                s += "MEDIAN";
            }
            if (on_filter_sharpen) {
                if (!s.empty()) s += "/";
                s += "SHARPEN";
            }
            if (on_filter_equalize) {
                if (!s.empty()) s += "/";
                s += "EQUALIZE";
            }
            if (on_filter_high_boost) {
                if (!s.empty()) s += "/";
                s += "HIGHBOOST";
            }
            if (on_filter_edge_detect) {
                if (!s.empty()) s += "/";
                s += "EDGE";
            }
            if (SDL_GetHint(SDL_HINT_RENDER_SCALE_QUALITY) == std::string("linear")) {
                if (!s.empty()) s += "/";
                s += "SCALE-LINEAR";
            }

            if (s.empty()) {
                s = "<none filters>";
            }
            return s;
        }

        G2NETWORK_INFO ni;
        std::set<int> cameras;
        int threads;
        SDL_Renderer* renderer;
        SDL_Texture* surface;
        bool on_filter_median;
        bool on_filter_sharpen;
        bool on_filter_equalize;
        bool on_filter_high_boost;
        bool on_filter_edge_detect;
    };

public:
    explicit adaptor_fixture(client::g2watch* adaptor)
        : _adaptor(adaptor)
        , _options()
        , _channel(-1)
        , _camera(0)
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
    int _camera;

private:
    bool _cond_entered;
    mutable boost::mutex _mutex;
    mutable boost::mutex _mutex_disp;
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
        int channel = _adaptor->connect_ras(options().ni);
        if (channel >= 0) {
            if (_cond_entered) {
                _cond_entered = false;
            }
            else {
                _cond.wait(lock);
                _cond_entered = false;
            }

            lock.unlock();

            for ( ; _adaptor->is_connecting(channel); ) {
                boost::this_thread::sleep(boost::posix_time::milliseconds(1));
            }

            lock.lock();

            res = _adaptor->is_connected(channel);
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

    void video_play(const G2FRAME& frame)
    {
        // if you want to use an external decoder, reference plain pointer.
        // plain video stream data is [frame._plain_ptr, frame._plain_ptr + frame._index._plain_size)

        G2SIZE res = { SDL_max(720, frame._width), SDL_max(576, frame._height) };

        size_t buf_len = res.cx * res.cy * 4;
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
        param._threads = options().threads;

        bool ret = _decoder.decompress(param);
        if (ret != false &&
            param._result._result == G2DECODER_VIDEO_RESULT::SUCCESS &&
            frame._extra._display) {
            boost::mutex::scoped_lock sl(_mutex_disp);
            SDL_Rect rect_texture = { 0 };
            SDL_QueryTexture(options().surface, NULL, NULL, &rect_texture.w, &rect_texture.h);

            if (rect_texture.w != param._result._width ||
                rect_texture.h != param._result._height) {
                SDL_RenderClear(options().renderer);
                SDL_DestroyTexture(options().surface);
                options().surface = SDL_CreateTexture(options().renderer, SDL_PIXELFORMAT_RGB24, SDL_TEXTUREACCESS_STATIC, param._result._width, param._result._height);
                SDL_RenderSetLogicalSize(options().renderer, param._result._width, param._result._height);
            }

            unsigned char* ptr = &_buf_decode[0];

            if (options().on_filter_equalize) {
                g2_img_filter_eqalize_l8(ptr, param._result._width, ptr, param._result._width, param._result._width, param._result._height);
            }
            if (options().on_filter_high_boost) {
                g2_img_filter_high_boost_l8(ptr, param._result._width, ptr, param._result._width, param._result._width, param._result._height, 1.1F);
            }
            if (options().on_filter_edge_detect) {
                g2_img_filter_edge_detect_l8(ptr, param._result._width, ptr, param._result._width, param._result._width, param._result._height);
            }

            g2_color_convert_yv12_to_rgb24(&_buf_filter[0], param._result._width * 3, ptr, param._result._width, param._result._height);
            ptr = &_buf_filter[0];

            if (options().on_filter_median) {
                g2_img_filter_median_rgb24(ptr, param._result._width * 3, ptr, param._result._width * 3, param._result._width, param._result._height);
            }
            if (options().on_filter_sharpen) {
                g2_img_filter_sharpen_rgb24(ptr, param._result._width * 3, ptr, param._result._width * 3, param._result._width, param._result._height);
            }

            SDL_UpdateTexture(options().surface, NULL, ptr, param._result._width * 3);
            SDL_RenderCopy(options().renderer, options().surface, NULL, NULL);
            SDL_RenderPresent(options().renderer);
        }
    }

    boost::mutex& get_mutex_disp(void) const { return _mutex_disp; }

    static bool set_turn(bool& var, int turn)
    {
        bool pre = var;
        var = (turn == 2) ? !pre : (turn != 0);
        return pre;
    }

    void print_help(void)
    {
        std::cout << " Q)  Filters an image using a box median filter" << std::endl;
        std::cout << " W)  Filters an image using sharpen convolution kernels" << std::endl;
        std::cout << " E)  Filters an image using histogram equalize" << std::endl;
        std::cout << " R)  Filters an image using high-boost" << std::endl;
        std::cout << " T)  Filters an image using edge detect roberts convolution kernels" << std::endl;
        std::cout << " Y)  Render scale quality change to Nearest <=> Linear" << std::endl;
        std::cout << "F1)  Show Help" << std::endl;
        std::cout << std::setfill('-') << std::setw(64);
        std::cout << "" << std::endl;
        std::cout << options().to_string_filters() << std::endl;
    }

public:
    virtual void on_g2watch_connected(G2HWATCH handle, int channel)
    {
        print_help();
        boost::unique_lock<boost::mutex> lock(_mutex);

        _channel = channel;
        _camera = *options().cameras.begin();
        _adaptor->set_camera_list(channel, options().cameras, false);
        _cond_entered = true;
        _cond.notify_all();
    }
    virtual void on_g2watch_disconnected(G2HWATCH handle, int channel, G2DISCONNECT_REASON::TYPE reason)
    {
        if (reason != G2DISCONNECT_REASON::LOGOUT) {
            G2STRING_128 s = { 0 };
            g2_get_string_disconnect_reason(reason, &s);
            if (s._len) {
                std::wcout << L"disconnected::reason(" << reason << L") " << s._string << std::endl;
            }
        }

        SDL_Event evt = { 0 };
        evt.type = SDL_QUIT;
        SDL_PushEvent(&evt);

        boost::unique_lock<boost::mutex> lock(_mutex);

        _cond_entered = true;
        _cond.notify_all();
    }
    virtual void on_g2watch_receive_frame_data(G2HWATCH handle, int channel, const G2FRAME& frame)
    {
        video_play(frame);
    }
    virtual void on_g2watch_receive_event(G2HWATCH handle, int channel, const G2EVENT_INFO& ei) {}
    virtual void on_g2watch_receive_device_status(G2HWATCH handle, int channel, const G2DEVICE_STATUS& status)
    {

    }
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

static void refresh_loop_wait_event(SDL_Event* e)
{
    SDL_PumpEvents();
    while (!SDL_PeepEvents(e, 1, SDL_GETEVENT, SDL_FIRSTEVENT, SDL_LASTEVENT)) {
        SDL_PumpEvents();
    }
}

int _tmain(int argc, _TCHAR* argv[])
{
    boost::program_options::options_description options_desc("app_g2_watch_image_filter");
    options_desc.add_options()
        ("help,h", "display current help message")
        ("address,a", boost::program_options::wvalue<std::wstring>(), "server(device) address ip:port")
        ("user", boost::program_options::wvalue<std::wstring>(), "user account id")
        ("password", boost::program_options::wvalue<std::wstring>(), "user account password")
        ("camera", boost::program_options::wvalue<int>(), "camera channel(or number) of device")
        ("threads", boost::program_options::wvalue<int>(), "decoding threads for frame-level multithreading")
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
    int threads = 0;
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
        std::cout << "sample: tester_cpp_app_g2_watch_image filter --address \"127.0.0.1:8016\" --user \"admin\" --password \"\" --camera 0" << std::endl;
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
    if (options.count("threads")) {
        threads = options["threads"].as<int>();
    }

    //////////////////////////////////////////////////////////////////////

    SDL_Init(SDL_INIT_VIDEO);
    SDL_SetHint(SDL_HINT_RENDER_SCALE_QUALITY, "linear");
    SDL_Window* window = SDL_CreateWindow("GDK G2WatchImageFilter", SDL_WINDOWPOS_UNDEFINED, SDL_WINDOWPOS_UNDEFINED, 960, 480, SDL_WINDOW_INPUT_FOCUS | SDL_WINDOW_SHOWN | SDL_WINDOW_RESIZABLE);
    SDL_Renderer* renderer = SDL_CreateRenderer(window, -1, SDL_RENDERER_ACCELERATED | SDL_RENDERER_PRESENTVSYNC);
    SDL_SetRenderDrawColor(renderer, 0, 0, 0, 255);
    SDL_RenderClear(renderer);
    SDL_RenderPresent(renderer);
    SDL_Texture* surface = SDL_CreateTexture(renderer, SDL_PIXELFORMAT_RGB24, SDL_TEXTUREACCESS_STATIC, 1920, 1080);
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
    fixture.options().cameras.insert(camera);
    fixture.options().threads = threads;
    fixture.options().renderer = renderer;
    fixture.options().surface = surface;

    if (fixture.connect()) {
        for ( ; ; ) {
            SDL_Event evt;
            refresh_loop_wait_event(&evt);

            bool reset_render = false;
            bool turns_filter = false;

            if (evt.window.event == SDL_WINDOWEVENT_RESIZED) {
                reset_render = true;
            }
            else if (evt.type == SDL_KEYDOWN) {
                size_t key = evt.key.keysym.sym;
            }
            else if (evt.type == SDL_KEYUP) {
                size_t key = evt.key.keysym.sym;
                if (key == SDLK_ESCAPE) {
                    break;
                }
                else if (key == SDLK_F1)
                {
                    fixture.print_help();
                }
                else if (key == SDLK_q) {
                    fixture.set_turn(fixture.options().on_filter_median, 2);
                    turns_filter = true;
                }
                else if (key == SDLK_w) {
                    fixture.set_turn(fixture.options().on_filter_sharpen, 2);
                    turns_filter = true;
                }
                else if (key == SDLK_e) {
                    fixture.set_turn(fixture.options().on_filter_equalize, 2);
                    turns_filter = true;
                }
                else if (key == SDLK_r) {
                    fixture.set_turn(fixture.options().on_filter_high_boost, 2);
                    turns_filter = true;
                }
                else if (key == SDLK_t) {
                    fixture.set_turn(fixture.options().on_filter_edge_detect, 2);
                    turns_filter = true;
                }
                else if (key == SDLK_y) {
                    turns_filter = true;
                    std::string s = SDL_GetHint(SDL_HINT_RENDER_SCALE_QUALITY);
                    if (s == "nearest") {
                        SDL_SetHint(SDL_HINT_RENDER_SCALE_QUALITY, "linear");
                    }
                    else {
                        SDL_SetHint(SDL_HINT_RENDER_SCALE_QUALITY, "nearest");
                    }

                    reset_render = true;
                }
                else {}

                if (turns_filter) {
                    std::cout << fixture.options().to_string_filters() << std::endl;
                }
            }
            else if (evt.type == SDL_QUIT) {
                break;
            }

            if (reset_render) {
                boost::mutex::scoped_lock sl(fixture.get_mutex_disp());
                SDL_DestroyTexture(fixture.options().surface);
                SDL_DestroyRenderer(fixture.options().renderer);
                fixture.options().renderer = SDL_CreateRenderer(window, -1, SDL_RENDERER_ACCELERATED | SDL_RENDERER_PRESENTVSYNC);
                SDL_RenderClear(renderer);
                fixture.options().surface = NULL;
            }
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
