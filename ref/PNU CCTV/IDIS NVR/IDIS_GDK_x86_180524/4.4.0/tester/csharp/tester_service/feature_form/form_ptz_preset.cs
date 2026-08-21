using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using GDK;

namespace GDK_tester
{
    public partial class form_ptz_preset : Form
    {
        public form_ptz_preset()
        {
            InitializeComponent();
        }

        public void set_preset(ref G2LIVE_PTZ_PRESET preset)
        {
            data = preset;

            if (preset.is_G2)
            {
                int i = 0;
                foreach (G2STRING_32 s in preset._g2._preset)
                {
                    i++;
                    if (i > preset._g2._count)
                    {
                        break;
                    }

                    ListViewItem lvi = new ListViewItem(i.ToString());
                    lvi.SubItems.Add(s);
                    LST_PRESET.Items.Add(lvi);
                }

                if (command == G2LIVE_PTZ_COMMAND.COMMAND.PTZ_SET_PRESET)
                {
                    EDT_TITLE.MaxLength = (int)G2LIVE_PTZ_PRESET.CONST.MAX_LEN_PRESET_G2 - 1;
                }
            }
            else
            {
                int i = 0;
                foreach (G2LIVE_PTZ_PRESET.PRESET p in preset._g1._preset)
                {
                    i++;
                    if (i > preset._g1._count)
                    {
                        break;
                    }

                    ListViewItem lvi = new ListViewItem(i.ToString());
                    lvi.SubItems.Add(p.title);
                    LST_PRESET.Items.Add(lvi);
                }

                if (command == G2LIVE_PTZ_COMMAND.COMMAND.PTZ_SET_PRESET)
                {
                    EDT_TITLE.MaxLength = (int)G2LIVE_PTZ_PRESET.CONST.MAX_LEN_PRESET - 1;
                }
            }

            if (LST_PRESET.Items.Count > 0)
            {
                LST_PRESET.Items[0].Selected = true;
                BTN_OK.Enabled = true;
                selected = 0;
            }
            else
            {
                BTN_OK.Enabled = false;
                selected = -1;
            }
        }

        private void on_lst_select_changed(object sender, EventArgs e)
        {
            foreach (int i in LST_PRESET.SelectedIndices)
            {
                selected = i;
                break;
            }
        }
        private void on_lst_double_click(object sender, EventArgs e)
        {
            if (handler_move != null)
            {
                if (command == G2LIVE_PTZ_COMMAND.COMMAND.PTZ_MOVE_PRESET)
                {
                    int selected = -1;
                    foreach (int i in LST_PRESET.SelectedIndices)
                    {
                        selected = i;
                        break;
                    }

                    if (selected >= 0)
                    {
                        this.selected = selected;
                        handler_move(this, e);
                    }
                }
            }
        }
        private void on_btn_ok(object sender, EventArgs e)
        {
            if (command == G2LIVE_PTZ_COMMAND.COMMAND.PTZ_SET_PRESET)
            {
                string message = "Do you want to apply to preset " + (selected + 1) + "?";
                if (MessageBox.Show(this, message, "PTZ preset", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                {
                    return;
                }

                string title = EDT_TITLE.Text.Trim();
                if (data.is_G2)
                {
                    data._g2._preset[selected] = title;
                }
                else
                {
                    data._g1._preset[selected].title = title;
                }

                data._select = selected;
            }

            this.Close();
        }

        public G2GUID camera;
        public int selected;
        public G2LIVE_PTZ_PRESET data;
        public G2LIVE_PTZ_COMMAND.COMMAND command_;
        public G2LIVE_PTZ_COMMAND.COMMAND command
        {
            get
            {
                return command_;
            }
            set
            {
                command_ = value;
                STC_TITLE.Visible =
                EDT_TITLE.Visible = (command_ == G2LIVE_PTZ_COMMAND.COMMAND.PTZ_SET_PRESET);
            }
        }
        public event EventHandler handler_move;
    }
}
