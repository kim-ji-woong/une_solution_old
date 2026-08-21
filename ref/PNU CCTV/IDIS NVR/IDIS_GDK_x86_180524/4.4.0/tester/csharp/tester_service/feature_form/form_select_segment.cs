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
    public partial class form_select_segment : Form
    {
        public form_select_segment()
        {
            InitializeComponent();
        }

        public void set_data(G2SCOPE[] scopes)
        {
            uint i = 1;
            uint id_pre = uint.MaxValue;
            foreach (G2SCOPE scope in scopes)
            {
                if (id_pre == uint.MaxValue ||
                    id_pre != scope._begin._segment)
                {
                    id_pre = scope._begin._segment;

                    string time = string.Format("({0}) {1} ~ ({2}) {3}",
                                                scope._begin._segment, scope._begin._time.to_string_date_time(),
                                                scope._end._segment, scope._end._time.to_string_date_time());
                    ListViewItem liv = new ListViewItem(i.ToString());
                    liv.SubItems.Add(time);
                    liv.Tag = scope.Clone();
                    LST_SEGMENT.Items.Add(liv);
                    i = i + 1;
                }
            }
            if (LST_SEGMENT.Items.Count > 0)
            {
                LST_SEGMENT.Items[0].Selected = true;
            }
        }
        public void set_data(G2SPOT[] spots)
        {
            uint i = 1;
            foreach (G2SPOT spot in spots)
            {
                G2SPOT val = (G2SPOT)spot.Clone();
                string time = string.Format("({0}) {1}", spot._segment, spot._time.to_string_date_time());
                ListViewItem liv = new ListViewItem(i.ToString());
                liv.SubItems.Add(time);
                liv.Tag = new G2SCOPE(val);
                LST_SEGMENT.Items.Add(liv);
                i = i + 1;
            }
        }
        public void set_data_clip(G2SCOPE[] scopes)
        {
            uint i = 1;
            foreach (G2SCOPE scope in scopes)
            {
                string time = string.Format("({0}) {1} ~ ({2}) {3}",
                                            scope._begin._segment, scope._begin._time.to_string_date_time(),
                                            scope._end._segment, scope._end._time.to_string_date_time());
                ListViewItem liv = new ListViewItem(i.ToString());
                liv.SubItems.Add(time);
                liv.Tag = scope.Clone();
                LST_SEGMENT.Items.Add(liv);
                i = i + 1;
            }
            if (LST_SEGMENT.Items.Count > 0)
            {
                LST_SEGMENT.Items[0].Selected = true;
            }
        }

        private void on_lst_selected_index_changed(object sender, EventArgs e)
        {
            if (LST_SEGMENT.SelectedItems.Count != 0)
            {
                scope_selected = (G2SCOPE)LST_SEGMENT.SelectedItems[0].Tag;
                BTN_OK.Enabled = true;
            }
            else
            {
                BTN_OK.Enabled = false;
            }
        }

        public G2SCOPE scope_selected;
    }
}
