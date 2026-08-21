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
    public partial class form_backup : Form
    {
        public form_backup(G2GUID service, form_play.play_info backup_info, g2user user, G2GUID site)
        {
            _service = service;
            _user_adaptor = user;
            _backup_info = backup_info;
            _total_camera_count = backup_info.camera_list.Count;
            _channel = -1;
            _site = site;

            this.SCREEN_PANE = new screen_pane();
            this.SCREEN_PANE.Name = "SCREEN_PANE";
            this.Controls.Add(this.SCREEN_PANE);

            InitializeComponent();

            this.SCREEN_PANE.Location = STC_SCREEN.Location;
            this.SCREEN_PANE.Size = STC_SCREEN.Size;
            this.LSV_EVENT.Location = STC_TIME_TABLE.Location;
            this.LSV_EVENT.Size = STC_TIME_TABLE.Size;

            init_connective_backup();
            init_connective_screen();
            init_connective_time_table();
            init_control_controller();
        }

        protected override void OnShown(EventArgs e)
        {
            bool connect_result = connect_backup();

            if (connect_result)
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
    }
}