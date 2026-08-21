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
    public partial class form_ptz_command : Form
    {
        public form_ptz_command()
        {
            InitializeComponent();

            this.channel = -1;
            this.camera = -1;
            this.CHK_PAN.Tag = this.EDT_PAN;
            this.CHK_TILT.Tag = this.EDT_TILT;
            this.CHK_ZOOM.Tag = this.EDT_ZOOM;
            this.CHK_FOCUS.Tag = this.EDT_FOCUS;
            this.CHK_IRIS.Tag = this.EDT_IRIS;
            this.EDT_PAN.Tag = this.EDT_PAN_RANGE;
            this.EDT_TILT.Tag = this.EDT_TILT_RANGE;
            this.EDT_ZOOM.Tag = this.EDT_ZOOM_RANGE;
            this.EDT_FOCUS.Tag = this.EDT_FOCUS_RANGE;
            this.EDT_IRIS.Tag = this.EDT_IRIS_RANGE;

            this.SELECTOR = new CheckBox[] {
                this.CHK_PAN,
                this.CHK_TILT,
                this.CHK_ZOOM,
                this.CHK_FOCUS,
                this.CHK_IRIS
            };
        }

        public void set_status(ref G2LIVE_COMMAND_CONTROL_PTZ control, ref G2LIVE_COMMAND_CONTROL_PTZ_RANGE range)
        {
            this.data = control;
            this.range = range;
            this.BTN_SET.Enabled = false;

            imp_load(G2LIVE_COMMAND_CONTROL_PTZ.TYPE.PAN, CHK_PAN, data._pan, range._min_pan, range._max_pan);
            imp_load(G2LIVE_COMMAND_CONTROL_PTZ.TYPE.TILT, CHK_TILT, data._tilt, range._min_tilt, range._max_tilt);
            imp_load(G2LIVE_COMMAND_CONTROL_PTZ.TYPE.ZOOM, CHK_ZOOM, data._zoom, range._min_zoom, range._max_zoom);
            imp_load(G2LIVE_COMMAND_CONTROL_PTZ.TYPE.FOCUS, CHK_FOCUS, data._focus, range._min_focus, range._max_focus);
            imp_load(G2LIVE_COMMAND_CONTROL_PTZ.TYPE.IRIS, CHK_IRIS, data._iris, range._min_iris, range._max_iris);
        }

        private void imp_load(G2LIVE_COMMAND_CONTROL_PTZ.TYPE type, CheckBox control, float val, float min, float max)
        {
            TextBox EDT = control.Tag as TextBox;
            TextBox EDT_RANGE = EDT.Tag as TextBox;

            if (this.data.contains(type))
            {
                EDT.Enabled = control.Checked = (form_ptz_command.selected & type) != 0;
                EDT.Text = val.ToString("0.0");
                EDT_RANGE.Text = string.Format("[{0}:{1}]", min.ToString("0.0"), max.ToString("0.0"));
            }
            else
            {
                control.Enabled = EDT.Enabled = EDT_RANGE.Enabled = false;
            }
        }
        private bool imp_save(G2LIVE_COMMAND_CONTROL_PTZ.TYPE type, CheckBox control, float min, float max, ref float res, ref G2LIVE_COMMAND_CONTROL_PTZ.TYPE selected)
        {
            if (control.Checked)
            {
                TextBox EDT = control.Tag as TextBox;
                bool succeed = false;
                try
                {
                    float val = float.Parse(EDT.Text);
                    float val_pre = val;
                    if (val < min) val = min;
                    if (val > max) val = max;
                    if (val != val_pre)
                    {
                        MessageBox.Show(this, "Value does not fall within the expected range.", "PTZ command control", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        selected |= type;
                        res = val;
                        succeed = true;
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show(this, "Input string was not in a correct format", "PTZ command control", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                if (succeed != true)
                {
                    EDT.SelectAll();
                    EDT.Focus();
                }
                return succeed;
            }
            return true;
        }

        private void on_chk_changed(object sender, EventArgs e)
        {
            CheckBox c = sender as CheckBox;
            TextBox edit = c.Tag as TextBox;
            if (edit != null)
            {
                edit.Enabled = c.Checked;
            }

            bool at_least = false;
            foreach (CheckBox sel in SELECTOR)
            {
                if (sel.Checked)
                {
                    at_least = true;
                    break;
                }
            }

            BTN_SET.Enabled = at_least;
        }
        private void on_btn_set(object sender, EventArgs e)
        {
            G2LIVE_COMMAND_CONTROL_PTZ.TYPE selected = 0;
            bool res = true;
            if (res) res = imp_save(G2LIVE_COMMAND_CONTROL_PTZ.TYPE.PAN, CHK_PAN, range._min_pan, range._max_pan, ref this.data._pan, ref selected);
            if (res) res = imp_save(G2LIVE_COMMAND_CONTROL_PTZ.TYPE.TILT, CHK_TILT, range._min_tilt, range._max_tilt, ref this.data._tilt, ref selected);
            if (res) res = imp_save(G2LIVE_COMMAND_CONTROL_PTZ.TYPE.ZOOM, CHK_ZOOM, range._min_zoom, range._max_zoom, ref this.data._zoom, ref selected);
            if (res) res = imp_save(G2LIVE_COMMAND_CONTROL_PTZ.TYPE.FOCUS, CHK_FOCUS, range._min_focus, range._max_focus, ref this.data._focus, ref selected);
            if (res) res = imp_save(G2LIVE_COMMAND_CONTROL_PTZ.TYPE.IRIS, CHK_IRIS, range._min_iris, range._max_iris, ref this.data._iris, ref selected);
            if (res != true)
            {
                return;
            }

            if (selected == 0)
            {
                MessageBox.Show(this, "There is no selected item.", "PTZ command control", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            this.data.types = selected;
            form_ptz_command.selected = selected;

            if (handler_set != null)
            {
                handler_set(this, e);
            }
            else
            {
                this.DialogResult = DialogResult.OK;
            }
        }

        public int channel;
        public int camera;
        public G2LIVE_COMMAND_CONTROL_PTZ data;
        public G2LIVE_COMMAND_CONTROL_PTZ_RANGE range;
        public static G2LIVE_COMMAND_CONTROL_PTZ.TYPE selected = G2LIVE_COMMAND_CONTROL_PTZ.TYPE.ALL;
        private CheckBox[] SELECTOR;
        public event EventHandler handler_set;
    }
}
