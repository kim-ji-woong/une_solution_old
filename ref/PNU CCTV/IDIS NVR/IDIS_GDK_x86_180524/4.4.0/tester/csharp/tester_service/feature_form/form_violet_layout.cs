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
    public partial class form_violet_layout : Form
    {
        const int MAX_MONITOR_PER_VIDEO_WALL_AGENT = 6;

        public form_violet_layout(List<G2LAYOUT> list)
        {
            InitializeComponent();

            _list = list;
            g2guid.g2_guid_make_invalid(ref _layout);
            _mon = 0;

            EDT_VIOLET_LAYOUT_MON.Text = "1";

            foreach (G2LAYOUT l in list)
            {
                CBX_VIOLET_LAYOUT_LAYOUT.Items.Add(l._name._string);
            }
            CBX_VIOLET_LAYOUT_LAYOUT.SelectedIndex = 0;

            BTN_OK.Click += new EventHandler(on_btn_ok);
        }

        protected void on_btn_ok(object sender, EventArgs e)
        {
            int mon = 0;
            if (!int.TryParse(EDT_VIOLET_LAYOUT_MON.Text, out mon) || 0 >= mon || MAX_MONITOR_PER_VIDEO_WALL_AGENT < mon)
            {
                MessageBox.Show(string.Format("invalid monitor number: {0}", EDT_VIOLET_LAYOUT_MON.Text));
                return;
            }
            _mon = mon;

            int sel = CBX_VIOLET_LAYOUT_LAYOUT.SelectedIndex;
            if (sel >= _list.Count)
            {
                MessageBox.Show(string.Format("invalid layout index: {0}", CBX_VIOLET_LAYOUT_LAYOUT.SelectedIndex));
                return;
            }
            _layout = _list[sel]._guid;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        public G2GUID get_selected_layout() { return _layout; } 
        public int get_selected_monitor()  { return _mon; }

        private List<G2LAYOUT> _list;
        G2GUID _layout;
        int _mon;
    }
}