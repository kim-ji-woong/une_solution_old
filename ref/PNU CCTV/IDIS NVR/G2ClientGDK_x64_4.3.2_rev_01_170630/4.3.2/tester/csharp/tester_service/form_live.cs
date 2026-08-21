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
    public partial class form_live : Form
    {
        private class liveinfo
        {
            public G2GUID streaming_service;
            public int adaptor_channel;
            public List<G2DEVICE_LEAF> camera_info_list;
            public List<G2DEVICE_LEAF> alarm_out_list;
            public List<G2DEVICE_ROOT> root_infos;
            public List<int> channelext_list;
            public Dictionary<int, int> channelext_panel_dic;
            public Dictionary<G2GUID, int> camera_guid_panel_dic;

            public liveinfo()
            {
                adaptor_channel = -1;
                channelext_list = new List<int>();
                alarm_out_list = new List<G2DEVICE_LEAF>();
                root_infos = new List<G2DEVICE_ROOT>();
                channelext_panel_dic = new Dictionary<int, int>();
                camera_guid_panel_dic = new Dictionary<G2GUID, int>();
            }
            
            public G2STRING_128 get_camera_name(G2GUID cam_guid)
            {
                G2STRING_128 res = "";
                for (int i = 0; i < this.camera_info_list.Count; ++i)
                {
                    if (this.camera_info_list[i]._guid == cam_guid)
                    {
                        res = this.camera_info_list[i]._name;
                        break;
                    }
                }
                return res;
            }

            public List<G2GUID> get_root_guids()
            {
                List<G2GUID> root_guids = new List<G2GUID>();

                for (int i = 0; i < this.camera_info_list.Count; ++i)
                {
                    if (root_guids.Contains(this.camera_info_list[i]._guid_root) == false)
                    {
                        root_guids.Add(this.camera_info_list[i]._guid_root);
                    }
                }

                return root_guids;
            }

            public G2DEVICE_ROOT get_root_info(G2GUID root)
            {
                for (int i = 0; i < this.root_infos.Count; ++i)
                {
                    if (this.root_infos[i]._guid == root)
                    {
                        return this.root_infos[i];
                    }
                }

                return new G2DEVICE_ROOT();
            }
        }

        public form_live(Dictionary<G2GUID, List<G2DEVICE_LEAF>> service_camera_dic, List<G2DEVICE_LEAF> alarm_outs, List<G2DEVICE_ROOT> root_infos,
                        g2admin admin, g2user user)
        {
            _liveinfo_list = new List<liveinfo>();
            _total_camera_count = 0;
            _admin_adaptor = admin;
            _user_adaptor = user;

            foreach (KeyValuePair<G2GUID, List<G2DEVICE_LEAF>> pair in service_camera_dic)
            {
                liveinfo info = new liveinfo();
                info.streaming_service = pair.Key;
                info.camera_info_list = pair.Value;

                List<G2GUID> roots = info.get_root_guids();
                for (int i = 0; i < alarm_outs.Count; ++i)
                {
                    if (roots.Contains(alarm_outs[i]._guid_root))
                    {
                        if (info.alarm_out_list.Contains(alarm_outs[i]) == false) info.alarm_out_list.Add(alarm_outs[i]);
                    }
                }

                for (int j = 0; j < root_infos.Count; ++j)
                {
                    if (roots.Contains(root_infos[j]._guid))
                    {
                        info.root_infos.Add(root_infos[j]);
                    }
                }

                _liveinfo_list.Add(info);
                _total_camera_count += info.camera_info_list.Count;
            }
            _panel_number = 0;

            this.SCREEN_PANE = new screen_pane_service();
            this.SCREEN_PANE.Name = "SCREEN_PANE";
            this.Controls.Add(this.SCREEN_PANE);

            InitializeComponent();

            this.SCREEN_PANE.Location = STC_SCREEN.Location;
            this.SCREEN_PANE.Size = STC_SCREEN.Size;

            init_control_ptz();

            init_connective_live();
            init_connective_screen();
        }

        protected override void OnShown(EventArgs e)
        {
            bool connect_res = connect_live();

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
            for (int i = 0; i < _liveinfo_list.Count; ++i)
            {
                if (_adaptor.is_disconnectable(_liveinfo_list[i].adaptor_channel))
                {
                    _adaptor.disconnect(_liveinfo_list[i].adaptor_channel);

                    while (_liveinfo_list[i].adaptor_channel >= 0)
                    {
                        Thread.Sleep(10);
                    }
                }
            }

            _adaptor.cleanup();
            _screen.cleanup();
        }
    }
}