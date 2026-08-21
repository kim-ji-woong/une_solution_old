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
    public partial class form_text_in_search : Form
    {
        public form_text_in_search(object options, bool camera_base)
        {
            InitializeComponent();

            if (options is G2SEARCH_G2_TEXT_IN_SEARCH_OPTIONS)
            {
                this._options_G2 = (G2SEARCH_G2_TEXT_IN_SEARCH_OPTIONS)options;
            }
            else if (options is G2TEXT_IN_QUERY_CONDITION)
            {
                this._options_G1 = (G2TEXT_IN_QUERY_CONDITION)options;
            }

            this._options_pre = options;
            this._camera_base = camera_base;
            this.OPT_1_CBX_TYPE.Tag = 1;
            this.OPT_2_CBX_TYPE.Tag = 2;
            this.OPT_3_CBX_TYPE.Tag = 3;
            this.OPT_4_CBX_TYPE.Tag = 4;
            this.OPT_X_CBX_TYPE = new ComboBox[] { null, OPT_1_CBX_TYPE, OPT_2_CBX_TYPE, OPT_3_CBX_TYPE, OPT_4_CBX_TYPE };
            this.OPT_X_EDT_NAME = new TextBox[] { OPT_0_EDT_NAME, OPT_1_EDT_NAME, OPT_2_EDT_NAME, OPT_3_EDT_NAME, OPT_4_EDT_NAME };
            this.OPT_X_CBX_COMP = new ComboBox[] { OPT_0_CBX_COMP, OPT_1_CBX_COMP, OPT_2_CBX_COMP, OPT_3_CBX_COMP, OPT_4_CBX_COMP };
            this.OPT_X_EDT_VALUE = new TextBox[] { OPT_0_EDT_VALUE, OPT_1_EDT_VALUE, OPT_2_EDT_VALUE, OPT_3_EDT_VALUE, OPT_4_EDT_VALUE };
            this.OPT_X_EDT_COLUMN = new TextBox[] { OPT_0_EDT_COLUMN, OPT_1_EDT_COLUMN, OPT_2_EDT_COLUMN, OPT_3_EDT_COLUMN, OPT_4_EDT_COLUMN };
            this.OPT_X_EDT_LINE = new TextBox[] { OPT_0_EDT_LINE, OPT_1_EDT_LINE, OPT_2_EDT_LINE, OPT_3_EDT_LINE, OPT_4_EDT_LINE };

            for (int i = 1; i < 5; ++i)
            {
                OPT_X_EDT_NAME[i].Enabled = OPT_X_CBX_COMP[i].Enabled = OPT_X_EDT_VALUE[i].Enabled = OPT_X_EDT_COLUMN[i].Enabled = OPT_X_EDT_LINE[i].Enabled = false;
            }

            foreach (ComboBox c in OPT_X_CBX_COMP)
            {
                if (c != null) c.SelectedIndex = 0;
            }
            foreach (ComboBox c in OPT_X_CBX_TYPE)
            {
                if (c != null) c.SelectedIndex = 0;
            }
        }

        public void set_product_info(ref G2_PRODUCT_INFO pi)
        {
            _pi = pi;
            _count_camera = (_pi.remote_search.base_channel > 0 &&
                             _pi.remote_search.stream_count > 0) ? pi.remote_search.base_channel * _pi.remote_search.stream_count :
                             _pi.device.count_camera;
            _count_text_in = _pi.device.count_text_in;

            if (_pi.remote_search.rec_info_type == (uint)G2_PRODUCT_INFO_CAPS.REMOTE_SEARCH.REC_INFO_TYPE.IDR_MINUTE)
            {
                DTP_FROM.CustomFormat = " HH:mm:ss";
                DTP_FROM.ShowUpDown = true;
                DTP_TO.CustomFormat = " HH:mm:ss";
                DTP_TO.ShowUpDown = true;
            }

            CHK_EXACT_WORD.Enabled = _pi.text_in_search.match_whole_word;
            CHK_TRANSACTION_WISE.Enabled = _pi.text_in_search.transaction_wise;

            imp_set_device();
        }
        public void set_time_range(G2TIME from, G2TIME to)
        {
            _range_from = from;
            _range_to = to;
        }
        public void load()
        {
            imp_load();

            if (CHK_FIRST.Checked &&
                CHK_LAST.Checked)
            {
                if (_range_from.valid &&
                    _range_to.valid)
                {
                    DTP_FROM.Value = _range_from;
                    DTP_TO.Value = _range_to;
                }
            }
        }

        protected void imp_load()
        {
            G2SCOPE scope = new G2SCOPE();
            g2channel_set channelset = new g2channel_set();

            if (_options_pre is G2SEARCH_G2_TEXT_IN_SEARCH_OPTIONS)
            {
                scope = _options_G2._scope;
                channelset = _options_G2._channels;

                for (int i = 0; i < _options_G2._condition_count; ++i)
                {
                    imp_load_condition(i, _options_G2._condition[i]);
                }

                CHK_CASE_SENSITIVE.Checked = _options_G2._case_sensitive;
                CHK_EXACT_WORD.Checked = _options_G2._match_whole_word;
                CHK_TRANSACTION_WISE.Checked = _options_G2._transaction_wise;
            }
            else if (_options_pre is G2TEXT_IN_QUERY_CONDITION)
            {
                scope._begin._time = _options_G1._begin;
                scope._end._time = _options_G1._end;
                channelset.from(_options_G1._channels);

                for (int i = 0; i < _options_G1._item_count; ++i)
                {
                    imp_load_condition(i, _options_G1._item[i]);
                }

                CHK_CASE_SENSITIVE.Checked = _options_G1._case_sensitive;
                CHK_EXACT_WORD.Checked = _options_G1._match_whole_word;
                CHK_TRANSACTION_WISE.Checked = _options_G1._transaction_wise;
            }

            if (scope._begin._time.valid) DTP_FROM.Value = scope._begin._time;
            if (scope._end._time.valid) DTP_TO.Value = scope._end._time;

            CHK_FIRST.Checked = (scope._begin._time.valid != true);
            DTP_FROM.Enabled = CHK_FIRST.Checked != true;
            CHK_LAST.Checked = (scope._end._time.valid != true);
            DTP_TO.Enabled = CHK_LAST.Checked != true;

            bool check_all = true;
            bool empty = true;
            foreach (TreeNode node in TRV_DEVICES.Nodes[0].Nodes)
            {
                node.Checked = channelset.contains((int)node.Tag);
                if (node.Checked != true)
                {
                    check_all = false;
                }
                else
                {
                    empty = false;
                }
            }
            TRV_DEVICES.Nodes[0].Checked = check_all;

            if (check_all != true)
            {
                if (empty)
                {
                    foreach (TreeNode node in TRV_DEVICES.Nodes[0].Nodes)
                    {
                        node.Checked = true;
                    }
                    TRV_DEVICES.Nodes[0].Checked = true;
                }
            }
        }
        protected void imp_load_condition(int i, G2SEARCH_G2_TEXT_IN_SEARCH_OPTIONS_CONDITION cond)
        {
            if (OPT_X_CBX_TYPE[i] != null)
            {
                OPT_X_CBX_TYPE[i].SelectedIndex = cond._combi_and ? 1 : 2;
            }

            OPT_X_EDT_NAME[i].Text = cond._exp._name;
            OPT_X_CBX_COMP[i].SelectedIndex = cond._exp._op;
            OPT_X_EDT_VALUE[i].Text = cond._exp._value;
            OPT_X_EDT_COLUMN[i].Text = cond._exp._column.ToString();
            OPT_X_EDT_LINE[i].Text = cond._exp._line.ToString();
        }
        protected void imp_load_condition(int i, G2TEXT_IN_QUERY_CONDITION.ITEM_TYPE cond)
        {
            string name = cond.name;
            string value = cond.value;

            if (OPT_X_CBX_TYPE[i] != null)
            {
                OPT_X_CBX_TYPE[i].SelectedIndex = cond._condition;
            }

            OPT_X_EDT_NAME[i].Text = name;
            OPT_X_CBX_COMP[i].SelectedIndex = cond._comparator;
            OPT_X_EDT_VALUE[i].Text = value;
            OPT_X_EDT_COLUMN[i].Text = cond._column.ToString();
            OPT_X_EDT_LINE[i].Text = cond._line.ToString();
        }

        protected bool imp_save()
        {
            G2SCOPE scope;
            if (imp_get_time_range(out scope) != true)
            {
                MessageBox.Show(this, "Invalid recording range", "Text-In Search", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }

            g2channel_set channelset = new g2channel_set();
            foreach (TreeNode node in TRV_DEVICES.Nodes[0].Nodes)
            {
                if (node.Checked)
                {
                    channelset.insert((int)node.Tag);
                }
            }

            if (channelset.empty())
            {
                MessageBox.Show(this, "There is no selected device.", "Text-In Search", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }

            if (_options_pre is G2SEARCH_G2_TEXT_IN_SEARCH_OPTIONS)
            {
                _options_G2._channels = channelset;
                _options_G2._case_sensitive = CHK_CASE_SENSITIVE.Checked;
                _options_G2._match_whole_word = CHK_EXACT_WORD.Checked;
                _options_G2._transaction_wise = CHK_TRANSACTION_WISE.Checked;
                _options_G2._condition = new G2SEARCH_G2_TEXT_IN_SEARCH_OPTIONS_CONDITION[8];

                int count = 0;
                for (int i = 0; i < 5; ++i)
                {
                    G2SEARCH_G2_TEXT_IN_SEARCH_OPTIONS_CONDITION cond = new G2SEARCH_G2_TEXT_IN_SEARCH_OPTIONS_CONDITION();
                    if (imp_save_condition(i, ref cond))
                    {
                        _options_G2._condition[count++] = cond;
                    }
                }

                _options_G2._condition_count = count;
            }
            else if (_options_pre is G2TEXT_IN_QUERY_CONDITION)
            {
                _options_G1._channels = channelset.to_uint32();
                _options_G1._case_sensitive = CHK_CASE_SENSITIVE.Checked;
                _options_G1._match_whole_word = CHK_EXACT_WORD.Checked;
                _options_G1._transaction_wise = CHK_TRANSACTION_WISE.Checked;
                _options_G1._item = new G2TEXT_IN_QUERY_CONDITION.ITEM_TYPE[5];

                int count = 0;
                for (int i = 0; i < 5; ++i)
                {
                    G2TEXT_IN_QUERY_CONDITION.ITEM_TYPE cond = new G2TEXT_IN_QUERY_CONDITION.ITEM_TYPE();
                    if (imp_save_condition(i, ref cond))
                    {
                        _options_G1._item[count++] = cond;
                    }
                }

                _options_G1._item_count = count;
            }
            return true;
        }
        protected bool imp_save_condition(int i, ref G2SEARCH_G2_TEXT_IN_SEARCH_OPTIONS_CONDITION cond)
        {
            if (i > 0)
            {
                int type = OPT_X_CBX_TYPE[i].SelectedIndex;
                if (type == 0)
                {
                    return false;
                }
                cond._combi_and = (type == 1);
            }

            cond._exp._name = OPT_X_EDT_NAME[i].Text;
            cond._exp._op = OPT_X_CBX_COMP[i].SelectedIndex;
            cond._exp._value = OPT_X_EDT_VALUE[i].Text;
            cond._exp._column = imp_int32_from_string(OPT_X_EDT_COLUMN[i].Text);
            cond._exp._line = imp_int32_from_string(OPT_X_EDT_LINE[i].Text);
            return cond.valid;
        }
        protected bool imp_save_condition(int i, ref G2TEXT_IN_QUERY_CONDITION.ITEM_TYPE cond)
        {
            if (i > 0)
            {
                cond._condition = OPT_X_CBX_TYPE[i].SelectedIndex;
                if (cond._condition == (int)G2TEXT_IN_QUERY_CONDITION.CONDITION.COND_NONE)
                {
                    return false;
                }
            }

            cond.name = OPT_X_EDT_NAME[i].Text;
            cond.value = OPT_X_EDT_VALUE[i].Text;
            cond._comparator = OPT_X_CBX_COMP[i].SelectedIndex;
            cond._column = imp_int32_from_string(OPT_X_EDT_COLUMN[i].Text);
            cond._line = imp_int32_from_string(OPT_X_EDT_LINE[i].Text);
            return true;
        }

        protected void imp_set_device()
        {
            TRV_DEVICES.Nodes.Clear();
            TRV_DEVICES.AfterCheck += new TreeViewEventHandler(on_tree_device_after_check);

            if (_camera_base)
            {
                if (_count_camera > 0)
                {
                    TRV_DEVICES.Nodes.Add(imp_build_node("Camera", _count_camera));
                }
            }
            else
            {
                if (_count_text_in > 0)
                {
                    TRV_DEVICES.Nodes.Add(imp_build_node("Text-In", _count_text_in));
                }
            }

            foreach (TreeNode node in TRV_DEVICES.Nodes)
            {
                node.ExpandAll();
            }
        }
        protected TreeNode imp_build_node(string name, int count)
        {
            TreeNode node = new TreeNode(name);
            for (int i = 0; i < count; ++i)
            {
                node.Nodes.Add(name + ((int)(i + 1)).ToString()).Tag = i;
            }
            return node;
        }
        protected int imp_int32_from_string(string s)
        {
            if (s.Length == 0)
            {
                return 0;
            }
            else
            {
                int i = 0;
                try
                {
                    i = Convert.ToInt32(s);
                }
                catch { }
                return i;
            }
        }

        protected bool imp_get_time_range(out G2SCOPE scope)
        {
            scope = new G2SCOPE();

            if (CHK_FIRST.Checked) scope._begin._time.reset();
            else scope._begin._time = DTP_FROM.Value;
            if (CHK_LAST.Checked) scope._end._time.reset();
            else scope._end._time = DTP_TO.Value;

            return (scope._begin._time.valid && scope._end._time.valid &&
                   (scope._begin._time >= scope._end._time)) ? false : true;
        }

        private void on_opt_type_changed(object sender, EventArgs e)
        {
            ComboBox c = sender as ComboBox;
            bool enable = c.SelectedIndex > 0;
            int tag = (int)c.Tag;
            OPT_X_EDT_NAME[tag].Enabled =
            OPT_X_CBX_COMP[tag].Enabled =
            OPT_X_EDT_VALUE[tag].Enabled =
            OPT_X_EDT_COLUMN[tag].Enabled =
            OPT_X_EDT_LINE[tag].Enabled = enable;
        }
        private void on_chk_first(object sender, EventArgs e)
        {
            bool enable = (CHK_FIRST.Checked != true);
            DTP_FROM.Enabled = enable;
        }
        private void on_chk_last(object sender, EventArgs e)
        {
            bool enable = (CHK_LAST.Checked != true);
            DTP_TO.Enabled = enable;
        }
        private void on_dtp_mouse_wheel(object sender, MouseEventArgs e)
        {
            SendKeys.Send(e.Delta > 0 ? "{UP}" : "{DOWN}");
        }
        private void on_tree_device_after_check(object sender, TreeViewEventArgs e)
        {
            if (e.Action != TreeViewAction.Unknown)
            {
                if (e.Node.Nodes.Count > 0)
                {
                    foreach (TreeNode node in e.Node.Nodes)
                    {
                        node.Checked = e.Node.Checked;
                    }
                }
            }
        }
        private void on_btn_OK(object sender, EventArgs e)
        {
            if (imp_save())
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private object _options_pre;
        private bool _camera_base;
        private G2_PRODUCT_INFO _pi;
        private G2TIME _range_from;
        private G2TIME _range_to;
        private int _count_camera;
        private int _count_text_in;
        private ComboBox[] OPT_X_CBX_TYPE;
        private TextBox[] OPT_X_EDT_NAME;
        private ComboBox[] OPT_X_CBX_COMP;
        private TextBox[] OPT_X_EDT_VALUE;
        private TextBox[] OPT_X_EDT_COLUMN;
        private TextBox[] OPT_X_EDT_LINE;
        public G2SEARCH_G2_TEXT_IN_SEARCH_OPTIONS _options_G2;
        public G2TEXT_IN_QUERY_CONDITION _options_G1;
    }
}
