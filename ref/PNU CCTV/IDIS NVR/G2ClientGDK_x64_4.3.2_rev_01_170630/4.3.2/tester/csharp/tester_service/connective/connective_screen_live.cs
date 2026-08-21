using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

using GDK;

namespace GDK_tester
{
    public partial class form_live : screen_listener
    {
        private screen_pane_service _screen;
        private bool _transfer_all;

        public int CUR_PAN
        {
            get { return _screen.selected_pane(); }
        }

        public void init_connective_screen()
        {
            _screen = SCREEN_PANE;
            _screen.option_use_scaler_G2 = true;
            int camera_count;
            if (_total_camera_count == 1) camera_count = 1;
            else if (_total_camera_count < 10) camera_count = _total_camera_count < 5 ? 4 : 9;
            else if (_total_camera_count < 17) camera_count = _total_camera_count < 13 ? 12 : 16;
            else if (_total_camera_count < 26) camera_count = _total_camera_count < 21 ? 20 : 25;
            else if (_total_camera_count < 37) camera_count = _total_camera_count < 34 ? 33 : 36;
            else if (_total_camera_count < 50) camera_count = 49;
            else camera_count = 64;
            _screen.create(2, camera_count == 1 ? 4 : camera_count);
            _screen.set_listener(this);
            _screen.set_pane_select(0, false);
            _screen.set_format(camera_count, true);

            int[] panes = new int[_total_camera_count];
            for (int i = 0; i < _total_camera_count; ++i)
            {
                panes[i] = i;
            }
            _screen.set_pane_mode(panes, camera_pane.MODE.LIVE);
            _transfer_all = false;
        }

        // record dot ////////////////////////////////////////////////////////////
        public bool set_is_rec(G2GUID camera, bool on_recording)
        {
            for (int i = 0; i < _liveinfo_list.Count; ++i)
            {
                if (_liveinfo_list[i].camera_guid_panel_dic.ContainsKey(camera))
                {
                    int panel = _liveinfo_list[i].camera_guid_panel_dic[camera];
                    ((camera_pane_service)(_screen.get_pane(panel))).IS_REC = on_recording;
                    return true;
                }
            }

            return false;
        }
        //////////////////////////////////////////////////////////////////////////

        #region screen_listener 멤버

        public void on_screen_changed_format(screen_format.FORMAT format, screen_format.CHANGED mode)
        {
            if (!_transfer_all)
            {
                if (format == screen_format.FORMAT.LAYOUT1X1)
                {
                    int channel_ext = _screen.selected_pane_channelext();
                    int panel_num = _screen.selected_pane();
                    for (int i = 0; i < _liveinfo_list.Count; ++i)
                    {
                        g2channel_set channels = new g2channel_set();
                        if (_liveinfo_list[i].channelext_list.Contains(channel_ext) && _liveinfo_list[i].channelext_panel_dic[channel_ext] == panel_num)
                        {
                            channels.insert(channel_ext);
                        }
                        _adaptor.set_camera_list(_liveinfo_list[i].adaptor_channel, channels, true);
                        _adaptor.set_camera_list_interest(_liveinfo_list[i].adaptor_channel, channels, true);
                    }
                }
                else
                {
                    for (int i = 0; i < _liveinfo_list.Count; ++i)
                    {
                        g2channel_set channels = new g2channel_set();
                        channels.insert(_liveinfo_list[i].channelext_list.ToArray());
                        _adaptor.set_camera_list(_liveinfo_list[i].adaptor_channel, channels, true);
                        _adaptor.set_camera_list_interest(_liveinfo_list[i].adaptor_channel, channels, true);
                    }
                }
            }
        }

        public void on_screen_changed_pane(int pane)
        {
            if (_screen.contains_pane(pane))
            {
                camera_pane cp = _screen.get_pane(pane);

                for (int i = 0; i < _liveinfo_list.Count; ++i)
                {
                    if (_liveinfo_list[i].channelext_list.Contains(cp._channelext) && _liveinfo_list[i].channelext_panel_dic.ContainsKey(cp._channelext) &&
                        _liveinfo_list[i].channelext_panel_dic[cp._channelext] == pane)
                    {
                        int index = _liveinfo_list[i].channelext_list.IndexOf(cp._channelext);
                        G2GUID camera_guid = _liveinfo_list[i].camera_info_list[index]._guid;
                        if (!_transfer_all)
                        {
                            bool multi = _adaptor.is_enable_multi_stream(_liveinfo_list[i].adaptor_channel, ref camera_guid);
                            int stream_count = _adaptor.get_stream_count(_liveinfo_list[i].adaptor_channel, ref camera_guid);
                            bool transfer = (stream_count > 1 && _total_camera_count == 1);
                            feature_control_multi_stream(multi, transfer);
                        }
                        else
                        {
                            feature_control_multi_stream(false, true);
                        }
                        if (_user_adaptor.is_authority(G2USER_AUTHORITY.TYPE.AUTHORITY_LIVE_IMAGE_PROCESSING))
                        {
                            feature_control_color_set_enable(true);
                        }
                        else
                        {
                            feature_control_color_set_enable(false);
                        }
                        if (_user_adaptor.is_authority(G2USER_AUTHORITY.TYPE.AUTHORITY_PTZ))
                        {
                            G2LIVE_CAMERA_STATUS status;
                            _adaptor.get_camera_status(_liveinfo_list[i].adaptor_channel, ref camera_guid, out status);
                            bool ptz_enable = status.ptz > G2LIVE_CAMERA_STATUS.PTZ.INACTIVE;
                            feature_control_PTZ_set_enable(ptz_enable, status.ptz_function);
                        }
                        else
                        {
                            feature_control_PTZ_set_enable(false, 0);
                        }                        
                        bool audio_enable = _adaptor.is_support(_liveinfo_list[i].adaptor_channel, ref camera_guid, G2LIVE_SUPPORT.QUERY.AUDIO_STREAM_IN_WATCH_PORT);
                        feature_control_audio_set_enable(audio_enable);
                        feature_control_instant_rec(true, camera_guid);
                        
                        break;
                    }
                }
            }
            else
            {
                feature_control_multi_stream(false, false);
                feature_control_color_set_enable(false);
                feature_control_PTZ_set_enable(false, 0);
                feature_control_audio_set_enable(false);
                feature_control_instant_rec(false, G2GUID.INVALID_GUID);
            }
        }

        public void on_screen_image_disp(int pane, ref G2FRAME frame)
        {
            Debug.WriteLine("callback [on_screen_image_disp] is not implemented.");
        }

        public void on_screen_play_end_loaded(int channel)
        {
            Debug.WriteLine("callback [on_screen_play_end_loaded] is not implemented.");
        }

        public void on_screen_search_stopped(int channel, int pane, G2SPOT spot)
        {
            Debug.WriteLine("callback [on_screen_search_stopped] is not implemented.");
        }

        #endregion
    }
}
