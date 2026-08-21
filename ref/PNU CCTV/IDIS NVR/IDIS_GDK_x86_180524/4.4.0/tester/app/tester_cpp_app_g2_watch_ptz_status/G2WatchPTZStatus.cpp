// G2WatchPTZStatus.cpp : Defines the entry point for the console application.
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
        OPTIONS(void) : ni(), cameras(), threads(0), renderer(NULL), surface(NULL) {
            memset(&ni, 0, sizeof(ni));
            ni._address_type = G2NETWORK_INFO::ADDRESS_TYPE_IPV4;
        }

        G2NETWORK_INFO ni;
        std::set<int> cameras;
        int threads;
        SDL_Renderer* renderer;
        SDL_Texture* surface;
    };

public:
    explicit adaptor_fixture(client::g2watch* adaptor)
        : _adaptor(adaptor)
        , _options()
        , _channel(-1)
        , _camera(0)
        , _speed(7)
        , _relative(7)
        , _ptz(G2DEVICE_STATUS::UNKNOWN)
        , _pressed(false)
        , _move_type(G2LIVE_COMMAND_CONTROL_PTZ::MT_ABSOLUTE)
        , _status()
        , _cond_entered(false)
    {
        _decoder.startup(2);
        memset(&_status, 0, sizeof(_status));
    }

    static const int RELATIVE_SPEED_STEP = 10;

public:
    const OPTIONS& options(void) const { return _options; }
          OPTIONS& options(void) { return _options; }

    void cleanup(void) { _decoder.cleanup(); }

    bool is_relative(void) const { return _move_type == G2LIVE_COMMAND_CONTROL_PTZ::MT_VELOCITY; }

protected:
    client::g2watch* _adaptor;
    OPTIONS _options;
    int _channel;
    int _camera;
    int _speed;
    int _relative;
    int _ptz;
    int _move_type;
    bool _pressed;
    G2LIVE_COMMAND_CONTROL_PTZ_STATUS _status;

private:
    bool _cond_entered;
    boost::mutex _mutex;
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

    void set_speed(int value)
    {
        int channel = _channel;
        int camera = _camera;

        if (value >= 0) {
            if (_adaptor->is_connected(channel)) {
                if (is_relative()) {
                    _relative = value;
                    std::cout << "PTZ_RELATIVE_SPEED:" << value << std::endl;
                }
                else {
                    G2LIVE_PTZ_COMMAND ptz;
                    ptz._command = G2LIVE_PTZ_COMMAND::PTZ_SET_SPEED;
                    ptz._argument = value;

                    if (_adaptor->set_ptz_command(channel, camera, ptz)) {
                        _speed = value;
                        std::cout << "PTZ_SET_SPEED:" << value << std::endl;
                    }
                }
            }
        }
    }

    void change_move_type(void)
    {
        int channel = _channel;
        int camera = _camera;
        bool support = false;
        
        G2_PRODUCT_INFO pi;
        _adaptor->get_product_info(channel, pi);
        if (pi.ip_camera.is) {
            unsigned short* begin = pi.remote_watch.command_types;
            unsigned short* end = begin + pi.remote_watch.command_types_len;
            unsigned short* iter = std::find(begin, end, G2_PRODUCT_INFO_CAPS::REMOTE_WATCH::COMMAND_CONTROL_PTZ_RELATIVE);        
            support = (iter != end);
        }
        if (!support) {
            unsigned int fn = _adaptor->get_status_ptz_function(channel, camera);
            if (!(fn & G2DEVICE_STATUS::PTZ_FUNC_RELATIVE_MOVE)) return;
        }

        if (is_relative()) {
            _move_type = G2LIVE_COMMAND_CONTROL_PTZ::MT_VELOCITY;
        }
        else {
            _move_type = G2LIVE_COMMAND_CONTROL_PTZ::MT_ABSOLUTE;
        }

        std::string str = is_relative() ? "RELATIVE" : "ABSOLUTE";
        int value = is_relative() ? _relative : _speed;
        std::cout << "Move Type: " << str << " (" << value << ")" << std::endl;

        request_status();
    }

    bool request_status(void)
    {
        int channel = _channel;
        if (_adaptor->is_connected(channel)) {
            return (is_relative()) ? _adaptor->send_command_control_ptz_status_relative(channel, _camera) :
                                     _adaptor->send_command_control_ptz_status(channel, _camera);
        }
        return false;
    }

    bool request_command_control(size_t key)
    {
        if (_status._control._move_type != _move_type) return false;

        G2LIVE_COMMAND_CONTROL_PTZ_STATUS status = _status;
        status._control._types = 0;
        status._control._move_type = _move_type;

        if (key == SDLK_RIGHT ||
            key == SDLK_LEFT) {
            status._control._types |= G2LIVE_COMMAND_CONTROL_PTZ::PAN;

            float sign = (key == SDLK_RIGHT) ? +1.0f : -1.0f;
            if (is_relative()) {
                status._control._pan = sign * ((_relative) ? (((float)_relative / RELATIVE_SPEED_STEP) * status._range._max_pan) : 0);
            }
            else {
                status._control._pan += sign * 10.0f;                
            }            
            if (status._control._pan < status._range._min_pan) status._control._pan = status._range._min_pan;
            if (status._control._pan > status._range._max_pan) status._control._pan = status._range._max_pan;
        }
        else if (key == SDLK_DOWN ||
                 key == SDLK_UP) {
            status._control._types |= G2LIVE_COMMAND_CONTROL_PTZ::TILT;

            float sign = (key == SDLK_UP) ? +1.0f : -1.0f;
            if (is_relative()) {
                status._control._tilt = sign * ((_relative) ? (((float)_relative / RELATIVE_SPEED_STEP) * status._range._max_tilt) : 0);
            }
            else {
                status._control._tilt += sign * 10.0f;                
            }            
            if (status._control._tilt < status._range._min_tilt) status._control._tilt = status._range._min_tilt;
            if (status._control._tilt > status._range._max_tilt) status._control._tilt = status._range._max_tilt;
        }
        else if (key == SDLK_COMMA ||
                 key == SDLK_PERIOD) {
            status._control._types |= G2LIVE_COMMAND_CONTROL_PTZ::ZOOM;

            float sign = (key == SDLK_PERIOD) ? +1.0f : -1.0f;
            if (is_relative()) {
                status._control._zoom = sign * ((_relative) ? (((float)_relative / RELATIVE_SPEED_STEP) * status._range._max_zoom) : 0);
            }
            else {
                float step = (status._range._max_zoom - status._range._min_zoom) / 10.0F;
                status._control._zoom += sign * step;                
            }
            if (status._control._zoom < status._range._min_zoom) status._control._zoom = status._range._min_zoom;
            if (status._control._zoom > status._range._max_zoom) status._control._zoom = status._range._max_zoom;
        }

        int channel = _channel;
        if (_adaptor->is_connected(channel)) {
            _status = status;
            print_status(status._control, status._range);
            return _adaptor->send_command_control_ptz(channel, _camera, &status._control);
        }
        return false;
    }

    void control_key_down(size_t key)
    {
        if (_pressed == true) return;

        bool pressed = true;
        int command = G2LIVE_PTZ_COMMAND::PTZ_STOP_MOVE;

        switch (key) {
            case SDLK_KP_1: command = G2LIVE_PTZ_COMMAND::PTZ_MOVE_SW; break;
            case SDLK_KP_2: command = G2LIVE_PTZ_COMMAND::PTZ_MOVE_S; break;
            case SDLK_KP_3: command = G2LIVE_PTZ_COMMAND::PTZ_MOVE_SE; break;
            case SDLK_KP_4: command = G2LIVE_PTZ_COMMAND::PTZ_MOVE_W; break;
            case SDLK_KP_6: command = G2LIVE_PTZ_COMMAND::PTZ_MOVE_E; break;
            case SDLK_KP_7: command = G2LIVE_PTZ_COMMAND::PTZ_MOVE_NW; break;
            case SDLK_KP_8: command = G2LIVE_PTZ_COMMAND::PTZ_MOVE_N; break;
            case SDLK_KP_9: command = G2LIVE_PTZ_COMMAND::PTZ_MOVE_NE; break;
            case SDLK_KP_PLUS: command = G2LIVE_PTZ_COMMAND::PTZ_ZOOM_IN; break;
            case SDLK_KP_MINUS: command = G2LIVE_PTZ_COMMAND::PTZ_ZOOM_OUT; break;
            case SDLK_KP_DIVIDE: command = G2LIVE_PTZ_COMMAND::PTZ_FOCUS_FAR; break;
            case SDLK_KP_MULTIPLY: command = G2LIVE_PTZ_COMMAND::PTZ_FOCUS_NEAR; break;
            case SDLK_KP_0: command = G2LIVE_PTZ_COMMAND::PTZ_IRIS_OPEN; break;
            case SDLK_KP_PERIOD: command = G2LIVE_PTZ_COMMAND::PTZ_IRIS_CLOSE; break;
            default:
                pressed = false;
                break;
        }

        if (pressed) {
            int channel = _channel;
            int camera = _camera;
            int status = _adaptor->get_status_ptz(channel, camera);
            unsigned int fn = _adaptor->get_status_ptz_function(channel, camera);

            if (status == G2DEVICE_STATUS::INACTIVE) {
                return;
            }
            else if (command == G2LIVE_PTZ_COMMAND::PTZ_ZOOM_IN ||
                     command == G2LIVE_PTZ_COMMAND::PTZ_ZOOM_OUT) {
                if (!(fn & G2DEVICE_STATUS::PTZ_FUNC_ZOOM)) {
                    return;
                }
            }
            else if (command == G2LIVE_PTZ_COMMAND::PTZ_MOVE_SW ||
                     command == G2LIVE_PTZ_COMMAND::PTZ_MOVE_S  ||
                     command == G2LIVE_PTZ_COMMAND::PTZ_MOVE_SE ||
                     command == G2LIVE_PTZ_COMMAND::PTZ_MOVE_W  ||
                     command == G2LIVE_PTZ_COMMAND::PTZ_MOVE_E  ||
                     command == G2LIVE_PTZ_COMMAND::PTZ_MOVE_NW ||
                     command == G2LIVE_PTZ_COMMAND::PTZ_MOVE_N  ||
                     command == G2LIVE_PTZ_COMMAND::PTZ_MOVE_NE) {
                if (!(fn & G2DEVICE_STATUS::PTZ_FUNC_PAN_TILT)) {
                    return;
                }
            }
            else if (command == G2LIVE_PTZ_COMMAND::PTZ_FOCUS_FAR ||
                     command == G2LIVE_PTZ_COMMAND::PTZ_FOCUS_NEAR) {
                if (!(fn & G2DEVICE_STATUS::PTZ_FUNC_FOCUS)) {
                    return;
                }
            }
            else if (command == G2LIVE_PTZ_COMMAND::PTZ_IRIS_CLOSE ||
                     command == G2LIVE_PTZ_COMMAND::PTZ_IRIS_OPEN) {
                if (!(fn & G2DEVICE_STATUS::PTZ_FUNC_IRIS)) {
                    return;
                }
            }

            G2LIVE_PTZ_COMMAND ptz = { 0 };
            ptz._command = command;
            ptz._argument = 1;

            if (_adaptor->is_connected(channel)) {
                _adaptor->set_ptz_command(channel, camera, ptz);
                _pressed = true;
            }
        }
    }

    void control_key_up(size_t key)
    {
        if (_pressed == false) return;

        bool pressed = true;
        int command = G2LIVE_PTZ_COMMAND::PTZ_STOP_MOVE;
        int channel = _channel;
        int camera = _camera;

        switch (key) {
            case SDLK_KP_1:
            case SDLK_KP_2:
            case SDLK_KP_3:
            case SDLK_KP_4:
            case SDLK_KP_6:
            case SDLK_KP_7:
            case SDLK_KP_8:
            case SDLK_KP_9: command = G2LIVE_PTZ_COMMAND::PTZ_STOP_MOVE; break;
            case SDLK_KP_PLUS:
            case SDLK_KP_MINUS: command = G2LIVE_PTZ_COMMAND::PTZ_STOP_ZOOM; break;
            case SDLK_KP_DIVIDE:
            case SDLK_KP_MULTIPLY: command = G2LIVE_PTZ_COMMAND::PTZ_STOP_FOCUS; break;
            case SDLK_KP_0:
            case SDLK_KP_PERIOD: command = G2LIVE_PTZ_COMMAND::PTZ_STOP_IRIS; break;
            default:
                pressed = false;
                break;
        }

        if (pressed) {
            G2LIVE_PTZ_COMMAND ptz = { 0 };
            ptz._command = command;
            ptz._argument = 0;

            if (_adaptor->is_connected(channel)) {
                _adaptor->set_ptz_command(channel, _camera, ptz);
                _pressed = false;
            }
        }
    }

    void control_speed(size_t key)
    {
        int value = is_relative() ? _relative : _speed;
        int max = is_relative() ? RELATIVE_SPEED_STEP : 15;
        int speed = (key == SDLK_PAGEDOWN) ? SDL_max(  0, value - 1) :
                    (key == SDLK_PAGEUP  ) ? SDL_min(max, value + 1) : -1;

        if (value == speed) return;
        else { value = speed; }

        set_speed(speed);
    }

    void control_focus_onepush()
    {
        int channel = _channel;
        int camera = _camera;
        if (_adaptor->is_connected(channel)) {
            unsigned int fn = _adaptor->get_status_ptz_function(channel, camera);

            if (!!(fn & G2DEVICE_STATUS::PTZ_FUNC_FOCUS_ONEPUSH)) {
                G2LIVE_PTZ_COMMAND ptz;
                ptz._command = G2LIVE_PTZ_COMMAND::PTZ_FOCUS_ONEPUSH;
                ptz._argument = 0;

                if (_adaptor->set_ptz_command(channel, camera, ptz)) {
                    std::cout << "PTZ_FOCUS_ONEPUSH" << std::endl;
                }
            }
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
        param._threads = options().threads;

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

    static void print_help(void)
    {
        std::cout << "KEY_PAD(7)    PTZ_MOVE_NW" << std::endl;
        std::cout << "KEY_PAD(8)    PTZ_MOVE_N" << std::endl;
        std::cout << "KEY_PAD(9)    PTZ_MOVE_NE" << std::endl;
        std::cout << "KEY_PAD(4)    PTZ_MOVE_W" << std::endl;
        std::cout << "KEY_PAD(5)    PTZ_FOCUS_ONEPUSH" << std::endl;
        std::cout << "KEY_PAD(6)    PTZ_MOVE_E" << std::endl;
        std::cout << "KEY_PAD(1)    PTZ_MOVE_SW" << std::endl;
        std::cout << "KEY_PAD(2)    PTZ_MOVE_S" << std::endl;
        std::cout << "KEY_PAD(3)    PTZ_MOVE_SE" << std::endl;
        std::cout << "KEY_PAD(-)    PTZ_ZOOM_OUT" << std::endl;
        std::cout << "KEY_PAD(+)    PTZ_ZOOM_IN" << std::endl;
        std::cout << "KEY_PAD(/)    PTZ_FOCUS_FAR" << std::endl;
        std::cout << "KEY_PAD(*)    PTZ_FOCUS_NEAR" << std::endl;
        std::cout << "KEY_PAD(0)    PTZ_IRIS_OPEN" << std::endl;
        std::cout << "KEY_PAD(.)    PTZ_IRIS_CLOSE" << std::endl;
        std::cout << "Tab           Change Move Type" << std::endl;
        std::cout << "End           Set Speed to Zero(0)" << std::endl;
        std::cout << "Space Bar     Request Current PTZ Status" << std::endl;
        std::cout << "Page Up(DOWN) PTZ_SET_SPEED" << std::endl;
        std::cout << "F1            Show Help" << std::endl;
        std::cout << std::setfill('-') << std::setw(37);
        std::cout << "" << std::endl;
    }

    static void print_status(const G2LIVE_COMMAND_CONTROL_PTZ& control, const G2LIVE_COMMAND_CONTROL_PTZ_RANGE& range)
    {
        if (control._types & G2LIVE_COMMAND_CONTROL_PTZ::PAN  ) std::cout << "P:" << std::fixed << std::setprecision(2) << control._pan   << " " << "[" << std::setprecision(1) << range._min_pan   << ":" << range._max_pan   << "] ";
        if (control._types & G2LIVE_COMMAND_CONTROL_PTZ::TILT ) std::cout << "T:" << std::fixed << std::setprecision(2) << control._tilt  << " " << "[" << std::setprecision(1) << range._min_tilt  << ":" << range._max_tilt  << "] ";
        if (control._types & G2LIVE_COMMAND_CONTROL_PTZ::ZOOM ) std::cout << "Z:" << std::fixed << std::setprecision(2) << control._zoom  << " " << "[" << std::setprecision(1) << range._min_zoom  << ":" << range._max_zoom  << "] ";
        if (control._types & G2LIVE_COMMAND_CONTROL_PTZ::FOCUS) std::cout << "F:" << std::fixed << std::setprecision(2) << control._focus << " " << "[" << std::setprecision(1) << range._min_focus << ":" << range._max_focus << "] ";
        if (control._types & G2LIVE_COMMAND_CONTROL_PTZ::IRIS ) std::cout << "I:" << std::fixed << std::setprecision(2) << control._iris  << " " << "[" << std::setprecision(1) << range._min_iris  << ":" << range._max_iris  << "] ";
        std::cout << std::endl;
    }

public:
    virtual void on_g2watch_connected(G2HWATCH handle, int channel)
    {
        print_help();
        boost::unique_lock<boost::mutex> lock(_mutex);

        _channel = channel;
        _camera = *options().cameras.begin();
        _adaptor->send_command_control_ptz_status(channel, _camera);
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
        G2DEVICE_STATUS_PTZ_ADVANCED_INFO pai = { 0 };
        if (_adaptor->get_status_ptz_advanced_info(channel, _camera, pai)) {
            if (pai._speed >= 0) {
                _speed = pai._speed;
            }
        }

        int ptz = _adaptor->get_status_ptz(channel, _camera);
        if (_ptz != ptz) {
            _ptz =  ptz;
            std::cout << "* PTZ Features ";
            if (ptz == G2DEVICE_STATUS::INACTIVE) {
                std::cout << "NONE" << std::endl;
            }
            else {
                unsigned int features = _adaptor->get_status_ptz_function(channel, _camera);
                if (features) {
                    std::cout << std::endl;
                    int i = 1;
                    if (features & G2DEVICE_STATUS::PTZ_FUNC_PAN_TILT     ) std::cout << i++ << ") " << "Pan and Tilt"      << std::endl;
                    if (features & G2DEVICE_STATUS::PTZ_FUNC_ZOOM         ) std::cout << i++ << ") " << "Zoom Control"      << std::endl;
                    if (features & G2DEVICE_STATUS::PTZ_FUNC_FOCUS        ) std::cout << i++ << ") " << "Focus Control"     << std::endl;
                    if (features & G2DEVICE_STATUS::PTZ_FUNC_IRIS         ) std::cout << i++ << ") " << "Iris Control"      << std::endl;
                    if (features & G2DEVICE_STATUS::PTZ_FUNC_PRESET_SETUP ) std::cout << i++ << ") " << "Preset Save"       << std::endl;
                    if (features & G2DEVICE_STATUS::PTZ_FUNC_PRESET_MOVE  ) std::cout << i++ << ") " << "Preset MoveTo"     << std::endl;
                    if (features & G2DEVICE_STATUS::PTZ_FUNC_OSD_MENU     ) std::cout << i++ << ") " << "OSD Menu"          << std::endl;
                    if (features & G2DEVICE_STATUS::PTZ_FUNC_ADVANCED     ) std::cout << i++ << ") " << "Advanced Menu"     << std::endl;
                    if (features & G2DEVICE_STATUS::PTZ_FUNC_FOCUS_ONEPUSH) std::cout << i++ << ") " << "OnePush Focus(AF)" << std::endl;
                    if (features & G2DEVICE_STATUS::PTZ_FUNC_RELATIVE_MOVE) std::cout << i++ << ") " << "PTZ Relative Move" << std::endl;
                    std::cout << std::endl;
                }
            }
        }
    }
    virtual void on_g2watch_receive_ptz_preset(G2HWATCH handle, int channel, int camera, const G2LIVE_PTZ_PRESET& preset) {}
    virtual void on_g2watch_receive_ptz_menu(G2HWATCH handle, int channel, int camera, const G2LIVE_PTZ_MENU& menu) {}
    virtual void on_g2watch_receive_camera_title_idr(G2HWATCH handle, int channel, int camera, const std::wstring& title) {}
    virtual void on_g2watch_receive_text_in(G2HWATCH handle, int channel, const G2TEXT_IN& data) {}
    virtual void on_g2watch_receive_network_camera_information(G2HWATCH handle, int channel) {}
    virtual void on_g2watch_receive_audio_out_not_available(G2HWATCH handle, int channel) {}
    virtual void on_g2watch_receive_command_result_control_color_status(G2HWATCH handle, int channel, int camera, const G2LIVE_COMMAND_CONTROL_COLOR& control, const G2LIVE_COMMAND_CONTROL_COLOR_RANGE& range) {}
    virtual void on_g2watch_receive_command_result_control_color(G2HWATCH handle, int channel, int camera, const G2LIVE_COMMAND_CONTROL_COLOR& control, G2LIVE_COMMAND_RESULT::TYPE result) {}
    virtual void on_g2watch_receive_command_result_control_ptz_status(G2HWATCH handle, int channel, int camera, const G2LIVE_COMMAND_CONTROL_PTZ& control, const G2LIVE_COMMAND_CONTROL_PTZ_RANGE& range)
    {
        _status._camera = camera;
        _status._control = control;
        _status._range = range;

        print_status(control, range);
    }
    virtual void on_g2watch_receive_command_result_control_ptz(G2HWATCH handle, int channel, int camera, G2LIVE_COMMAND_RESULT::TYPE result)
    {
        if (result == G2LIVE_COMMAND_RESULT::SUCCESS) {
            std::cout << "G2LIVE_COMMAND_RESULT::SUCCESS" << std::endl;
        }
        else {
            std::cout << "G2LIVE_COMMAND_RESULT::FAIL" << std::endl;
        }
    }
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
    boost::program_options::options_description options_desc("app_g2_watch_ptz_status");
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
        std::cout << "sample: tester_cpp_app_g2_watch_ptz_status --address \"127.0.0.1:8016\" --user \"admin\" --password \"\" --camera 0" << std::endl;
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
    SDL_Window* window = SDL_CreateWindow("GDK G2WatchPTZStatus", SDL_WINDOWPOS_UNDEFINED, SDL_WINDOWPOS_UNDEFINED, 960, 480, SDL_WINDOW_INPUT_FOCUS | SDL_WINDOW_SHOWN);
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
    fixture.options().cameras.insert(camera);
    fixture.options().threads = threads;
    fixture.options().renderer = renderer;
    fixture.options().surface = surface;

    if (fixture.connect()) {
        for ( ; ; ) {
            SDL_Event evt;
            refresh_loop_wait_event(&evt);

            if (evt.type == SDL_KEYDOWN) {
                size_t key = evt.key.keysym.sym;
                fixture.control_key_down(key);
            }
            else if (evt.type == SDL_KEYUP) {
                size_t key = evt.key.keysym.sym;
                if (key == SDLK_ESCAPE) {
                    break;
                }
                else if (key == SDLK_SPACE) {
                    fixture.request_status();
                }
                else if (key == SDLK_RIGHT ||
                         key == SDLK_LEFT ||
                         key == SDLK_DOWN ||
                         key == SDLK_UP ||
                         key == SDLK_COMMA ||
                         key == SDLK_PERIOD) {
                    fixture.request_command_control(key);
                }
                else if (key == SDLK_KP_5) {
                    fixture.control_focus_onepush();
                }
                else if (key == SDLK_PAGEDOWN ||
                         key == SDLK_PAGEUP) {
                    fixture.control_speed(key);
                }
                else if (key == SDLK_TAB) {
                    fixture.change_move_type();
                }
                else if (key == SDLK_END) {
                    fixture.set_speed(0);
                }
                else if (key == SDLK_F1)
                {
                    fixture.print_help();
                }
                else {
                    fixture.control_key_up(key);
                }
            }
            else if (evt.type == SDL_QUIT) {
                break;
            }
        }
    }

    fixture.disconnect();
    fixture.cleanup();
    adaptor.cleanup();

    g2_main_app_finalize();

    //////////////////////////////////////////////////////////////////////

    SDL_DestroyTexture(surface);
    SDL_DestroyRenderer(renderer);
    SDL_DestroyWindow(window);
    SDL_Quit();

    return 0;
}
