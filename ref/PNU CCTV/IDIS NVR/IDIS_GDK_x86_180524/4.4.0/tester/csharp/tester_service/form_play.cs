using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using GDK;

namespace GDK_tester
{
    public partial class form_play : Form
    {
        public class GUID_search
        {
            G2GUID _guid;

            public GUID_search(G2GUID guid)
            {
                _guid = guid;
            }

            public bool is_same(G2GUID guid)
            {
                return guid == _guid;
            }
        }

        public delegate void on_rec_failover_start(G2GUID failover_service, play_info info);
        public delegate bool check_exist_failover_service();

        public class play_info
        {
            public List<G2GUID> camera_list;
            public List<G2DEVICE_LEAF> camera_infos;
            public List<G2DEVICE_ROOT> root_infos;
            public List<G2DEVICE_LEAF> textin_infos;
            public List<int> channelexts;
            public List<int> panel_numbers;

            public play_info(List<G2GUID> cameras, List<G2DEVICE_LEAF> cinfos,
                        List<G2DEVICE_ROOT> rinfos, List<G2DEVICE_LEAF> tinfos)
            {
                camera_list = cameras;
                camera_infos = cinfos;
                root_infos = rinfos;
                textin_infos = new List<G2DEVICE_LEAF>(cameras.Count);
                for (int i = 0; i < cameras.Count; ++i)
                {
                    G2DEVICE_LEAF tinfo = new G2DEVICE_LEAF();
                    for (int j = 0; j < tinfos.Count; ++j)
                    {
                        if (tinfos[j]._guid_root == camera_infos[i]._guid_root)
                        {
                            tinfo = tinfos[j];
                            break;
                        }
                    }

                    textin_infos.Add(tinfo);
                }
                channelexts = new List<int>(cameras.Count);
                for (int i = 0; i < cameras.Count; ++i)
                {
                    channelexts.Add(-1);
                }
                panel_numbers = new List<int>(cameras.Count);
                for (int i = 0; i < cameras.Count; ++i)
                {
                    panel_numbers.Add(-1);
                }
            }

            public int index_from_camera(G2GUID camera)
            {
                GUID_search gs = new GUID_search(camera);
                int index = camera_list.FindIndex(gs.is_same);
                return index;
            }

            public int number_root_based(G2GUID camera)
            {
                int number = -1;
                for (int i = 0; i < camera_infos.Count; ++i)
                {
                    if (camera_infos[i]._guid == camera)
                    {
                        number = (int)camera_infos[i]._number_root_based;
                        break;
                    }
                }
                return number;
            }

            public int index_from_channelext(int channelext)
            {
                int index = -1;
                for (int i = 0; i < channelexts.Count; ++i)
                {
                    if (channelexts[i] == channelext)
                    {
                        index = i;
                        break;
                    }
                }

                return index;
            }

            public int channelext_from_panel(int panel)
            {
                int index = -1;
                for (int i = 0; i < panel_numbers.Count; ++i)
                {
                    if (panel_numbers[i] == panel)
                    {
                        index = i;
                        break;
                    }
                }

                return index == -1 ? -1 : channelexts[index];
            }

            public int panel_from_channelext(int channelext)
            {
                int index = -1;
                for (int i = 0; i < channelexts.Count; ++i)
                {
                    if (channelexts[i] == channelext)
                    {
                        index = i;
                        break;
                    }
                }

                return index == -1 ? -1 : panel_numbers[index];
            }

            public string camera_name(G2GUID camera)
            {
                GUID_search gs = new GUID_search(camera);
                int index = camera_list.FindIndex(gs.is_same);
                return index == -1 ? "" : camera_infos[index]._name._string;
            }

            public string root_name(G2GUID camera)
            {
                GUID_search gs = new GUID_search(camera);
                int index = camera_list.FindIndex(gs.is_same);
                return index == -1 ? "" : root_infos[index]._name._string;
            }
        }

        public form_play(G2GUID service, List<G2GUID> cameras, List<G2DEVICE_LEAF> camera_infos,
                        List<G2DEVICE_ROOT> root_infos, List<G2DEVICE_LEAF> textin_infos, g2user user)
        {
            _service = service;
            _user_adaptor = user;
            _play_info = new play_info(cameras, camera_infos, root_infos, textin_infos);
            _total_camera_count = cameras.Count;
            _panel_num = 0;
            _channel = -1;

            this.SCREEN_PANE = new screen_pane();
            this.SCREEN_PANE.Name = "SCREEN_PANE";
            this.Controls.Add(this.SCREEN_PANE);

            InitializeComponent();

            this.SCREEN_PANE.Location = STC_SCREEN.Location;
            this.SCREEN_PANE.Size = STC_SCREEN.Size;
            this.LSV_EVENT.Location = STC_TIME_TABLE.Location;
            this.LSV_EVENT.Size = STC_TIME_TABLE.Size;

            init_connective_play();
            init_connective_screen();
            init_connective_time_table();

            init_control_controller();
        }

        protected override void OnShown(EventArgs e)
        {
            bool connect_res = connect_play();

            if (connect_res)
            {
                base.OnShown(e);
                _screen.message().disp(STRING.NIS_CONNECTING, "connecting...", 0, true, 1000);
            }
            else
            {

            }
        }

        private void on_form_closing(object sender, FormClosingEventArgs e)
        {
            if (_adaptor.is_disconnectable(_channel))
            {
                _adaptor.disconnect(_channel);

                while (_channel >= 0)
                {
                    Thread.Sleep(10);
                }
            }

            _adaptor.cleanup();
            _screen.cleanup();
        }

        private void on_event_list_selected_changed(object sender, EventArgs e)
        {
            if (LSV_EVENT.SelectedItems.Count != 0)
            {
                object tag = LSV_EVENT.SelectedItems[0].Tag;

                _controller.on_event_list_selected();
            }
        }

        public void check_clip_auth()
        {
            _controller.imp_set_channel_update();
        }

        public void failover_btn_invisible()
        {
            _controller.failover_btn_invisible();
        }

        public void set_func(on_rec_failover_start func, check_exist_failover_service func2)
        {
            _controller.set_func(func, func2);
        }
    }
}