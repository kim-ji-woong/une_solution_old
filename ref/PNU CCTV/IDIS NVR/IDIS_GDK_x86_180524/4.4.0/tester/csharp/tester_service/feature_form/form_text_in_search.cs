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
        public form_text_in_search(object options)
        {
            InitializeComponent();

            this._options = (G2SEARCH_G2_TEXT_IN_SEARCH_OPTIONS)options;
            this._source = new G2GUIDSET();

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

        public void set_device(List<G2DEVICE_LEAF> textins)
        {
            CHK_EXACT_WORD.Enabled = true;
            CHK_TRANSACTION_WISE.Enabled = true;

            imp_set_device(textins);
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

            scope = _options._scope;
            channelset = _options._channels;

            for (int i = 0; i < _options._condition_count; ++i)
            {
                imp_load_condition(i, _options._condition[i]);
            }

            CHK_CASE_SENSITIVE.Checked = _options._case_sensitive;
            CHK_EXACT_WORD.Checked = _options._match_whole_word;
            CHK_TRANSACTION_WISE.Checked = _options._transaction_wise;
            

            if (scope._begin._time.valid) DTP_FROM.Value = scope._begin._time;
            if (scope._end._time.valid) DTP_TO.Value = scope._end._time;

            CHK_FIRST.Checked = (scope._begin._time.valid != true);
            DTP_FROM.Enabled = CHK_FIRST.Checked != true;
            CHK_LAST.Checked = (scope._end._time.valid != true);
            DTP_TO.Enabled = CHK_LAST.Checked != true;
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

            foreach (TreeNode node in TRV_DEVICES.Nodes)
            {
                if (node.Checked)
                {
                    _source.Add(((G2DEVICE_LEAF)node.Tag)._guid);
                }
            }

            if (_source.empty())
            {
                MessageBox.Show(this, "There is no selected device.", "Text-In Search", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }

            _options._case_sensitive = CHK_CASE_SENSITIVE.Checked;
            _options._match_whole_word = CHK_EXACT_WORD.Checked;
            _options._transaction_wise = CHK_TRANSACTION_WISE.Checked;
            _options._condition = new G2SEARCH_G2_TEXT_IN_SEARCH_OPTIONS_CONDITION[8];

            int count = 0;
            for (int i = 0; i < 5; ++i)
            {
                G2SEARCH_G2_TEXT_IN_SEARCH_OPTIONS_CONDITION cond = new G2SEARCH_G2_TEXT_IN_SEARCH_OPTIONS_CONDITION();
                if (imp_save_condition(i, ref cond))
                {
                    _options._condition[count++] = cond;
                }
            }

            _options._condition_count = count;

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

        protected void imp_set_device(List<G2DEVICE_LEAF> textins)
        {
            TRV_DEVICES.Nodes.Clear();
            TRV_DEVICES.AfterCheck += new TreeViewEventHandler(on_tree_device_after_check);

            foreach (G2DEVICE_LEAF leaf in textins)
            {
                if (leaf._enable)
                {
                    TreeNode node = new TreeNode(leaf._name._string);
                    node.Tag = leaf;
                    TRV_DEVICES.Nodes.Add(node);
                }
            }
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

        private G2TIME _range_from;
        private G2TIME _range_to;
        private ComboBox[] OPT_X_CBX_TYPE;
        private TextBox[] OPT_X_EDT_NAME;
        private ComboBox[] OPT_X_CBX_COMP;
        private TextBox[] OPT_X_EDT_VALUE;
        private TextBox[] OPT_X_EDT_COLUMN;
        private TextBox[] OPT_X_EDT_LINE;
        public G2SEARCH_G2_TEXT_IN_SEARCH_OPTIONS _options;
        public G2GUIDSET _source;
    }
}
