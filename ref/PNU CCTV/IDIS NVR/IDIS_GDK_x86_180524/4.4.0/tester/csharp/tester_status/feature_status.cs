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
    public partial class feature_status : Form
    {
        public class DEFAULT
        {
            public static int COL_COUNT = 16;
            public static int COL_WIDTH = 36;
        }

        public enum EVENT_TYPE
        {
            MOTION = 0,
            OBJECT,
            VIDEO_BLIND,
            VIDEO_ANALYSIS,
            TRIP_ZONE,
            TAMPER,
            FACE_DETECT,
            INSTANT_RECORD,
            ALARM_IN,
            ALARM_OUT,
            ALARM_IN_NETWORK,
            AUDIO_IN,
            TEXT_IN,
            COUNT
        }

        public feature_status(Control parent, Rectangle rect)
        {
            InitializeComponent();

            this.TopLevel = false;
            this.Visible = true;
            this.Location = rect.Location;
            this.Size = rect.Size;
            this.AutoScroll = true;
            this.Parent = parent;

            this._status_cs = new object();
            this._count_col = DEFAULT.COL_COUNT;

            reset(true);
        }

        public void remove()
        {
            if (_controls != null)
            {
                for (int i = 0; i < _controls.Length; ++i)
                {
                    imp_control_remove(_controls[i]);
                }

                Array.Resize(ref _controls, 0);
            }
        }
        public void reset(bool prepare)
        {
            if (_reset) return;

            remove();

            _reset = true;
            _count_col = DEFAULT.COL_COUNT;

            if (prepare)
            {
                G2MONITORING_DEVICE_STATUS status = new G2MONITORING_DEVICE_STATUS();
                status._alarm_in_network = new byte[] { G2MONITORING_DEVICE_STATUS.NOT_VALUE };
                imp_control_prepare(ref status, DEFAULT.COL_COUNT);
            }
        }
        public void set_enable(bool enable)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke((MethodInvoker)delegate() { set_enable(enable); });
                return;
            }

            this.Enabled = enable;
        }
        public void set_product_info(G2_PRODUCT_INFO pi)
        {
            _pi = pi;
            _count_col = Math.Min((int)G2MONITORING_DEVICE_STATUS.CONST.NUM_UNITS_MAX, (int)
                         Math.Max(pi.device.count_camera,
                         Math.Max(pi.device.count_audio_in,
                         Math.Max(pi.device.count_alarm_in,
                         Math.Max(pi.device.count_alarm_out,
                         Math.Max(pi.device.count_alarm_in_network, pi.device.count_text_in))))));
        }
        public void set_status(ref G2MONITORING_DEVICE_STATUS status)
        {
            lock (_status_cs)
            {
                _status = status;
            }

            if (this.InvokeRequired)
            {
                this.BeginInvoke((MethodInvoker)delegate() { imp_status_update(); });
            }
            else
            {
                imp_status_update();
            }
        }

        protected void imp_control_prepare(ref G2MONITORING_DEVICE_STATUS status, int count_col)
        {
            int count_control = (count_col + DEFAULT.COL_COUNT - 1) / DEFAULT.COL_COUNT;

            _controls = new DataGridView[count_control];

            Point pos = new Point();

            for (int i = 0; i < _controls.Length; ++i)
            {
                DataGridView control = imp_control_create(pos);

                _controls[i] = control;

                int cols = ((i + 1) * DEFAULT.COL_COUNT > count_col) ? count_col % DEFAULT.COL_COUNT : DEFAULT.COL_COUNT;

                for (int j = 0; j < cols; ++j)
                {
                    int num = i * DEFAULT.COL_COUNT + j;
                    string header = num < _count_col ? Convert.ToString(num + 1) : "";
                    control.Columns.Add(header, header);
                    control.Columns[j].Width = (int)DEFAULT.COL_WIDTH;
                    control.Columns[j].SortMode = DataGridViewColumnSortMode.NotSortable;
                }

                for (int j = 0; j < (int)EVENT_TYPE.COUNT; ++j)
                {
                    EVENT_TYPE type = (EVENT_TYPE)j;
                    switch (type)
                    {
                        case EVENT_TYPE.MOTION: imp_control_prepare_row(control.Rows, type, status._motion); break;
                        case EVENT_TYPE.OBJECT: imp_control_prepare_row(control.Rows, type, status._object); break;
                        case EVENT_TYPE.VIDEO_BLIND: imp_control_prepare_row(control.Rows, type, status._video_blind); break;
                        case EVENT_TYPE.VIDEO_ANALYSIS: imp_control_prepare_row(control.Rows, type, status._video_analysis); break;
                        case EVENT_TYPE.TRIP_ZONE: imp_control_prepare_row(control.Rows, type, status._trip_zone); break;
                        case EVENT_TYPE.TAMPER: imp_control_prepare_row(control.Rows, type, status._tamper); break;
                        case EVENT_TYPE.FACE_DETECT: imp_control_prepare_row(control.Rows, type, status._face_detect); break;
                        case EVENT_TYPE.INSTANT_RECORD: imp_control_prepare_row(control.Rows, type, status._instant_record); break;
                        case EVENT_TYPE.ALARM_IN: imp_control_prepare_row(control.Rows, type, status._alarm_in); break;
                        case EVENT_TYPE.ALARM_OUT: imp_control_prepare_row(control.Rows, type, status._alarm_out); break;
                        case EVENT_TYPE.ALARM_IN_NETWORK: imp_control_prepare_row(control.Rows, type, status._alarm_in_network); break;
                        case EVENT_TYPE.AUDIO_IN: imp_control_prepare_row(control.Rows, type, status._audio_in); break;
                        case EVENT_TYPE.TEXT_IN: imp_control_prepare_row(control.Rows, type, status._text_in); break;
                    }
                }

                int height = control.ColumnHeadersHeight + 1;
                foreach (DataGridViewRow row in control.Rows)
                {
                    height += row.Height;
                }

                int width = control.RowHeadersWidth + 1;
                foreach (DataGridViewColumn col in control.Columns)
                {
                    width += col.Width;
                }

                control.Height = height;
                control.Width = width;
                control.CurrentCell = null;
                pos.Y += height + 5;

                _controls[i] = control;
            }

            if (this.VScroll)
            {
                pos.Y = 0;
                foreach (DataGridView gv in _controls)
                {
                    int width = gv.RowHeadersWidth + 1;
                    int height = gv.ColumnHeadersHeight + 1;
                    foreach (DataGridViewColumn col in gv.Columns)
                    {
                        col.Width = col.Width - 1;
                        width += col.Width;
                    }

                    foreach (DataGridViewRow row in gv.Rows)
                    {
                        row.Height = row.Height - 1;
                        height += row.Height;
                    }

                    gv.Location = pos;
                    gv.Width = width;
                    gv.Height = height;
                    pos.Y += height + 5;
                }
              
            }

            this.SuspendLayout();
            this.Controls.AddRange(_controls);
            this.ResumeLayout(false);
        }
        protected void imp_control_prepare_row(DataGridViewRowCollection collection, EVENT_TYPE type, byte[] state)
        {
            if (state == null ||   // for reset
                G2MONITORING_DEVICE_STATUS.is_empty_status(state) != true)
            {
                DataGridViewRow row = new DataGridViewRow();
                row.HeaderCell.Tag = type;
                row.HeaderCell.ValueType = typeof(string);
                row.HeaderCell.Value = imp_string_from_type(type);
                collection.Add(row);
            }
        }
        protected void imp_status_update()
        {
            if (_controls == null ||
                _reset)
            {
                _reset = false;

                remove();

                lock (_status_cs)
                {
                    imp_control_prepare(ref _status, Math.Max(_count_col, 8));
                }
            }

            lock (_status_cs)
            {
                int i_state = 0;
                for (int j = 0; j < _controls.Length; ++j)
                {
                    DataGridView control = _controls[j];
                    for (int i = 0; i < control.Rows.Count; ++i)
                    {
                        DataGridViewCellCollection cells = control.Rows[i].Cells;
                        switch ((EVENT_TYPE)control.Rows[i].HeaderCell.Tag)
                        {
                            case EVENT_TYPE.MOTION: imp_status_update_row(cells, _status._motion, i_state); break;
                            case EVENT_TYPE.OBJECT: imp_status_update_row(cells, _status._object, i_state); break;
                            case EVENT_TYPE.VIDEO_BLIND: imp_status_update_row(cells, _status._video_blind, i_state); break;
                            case EVENT_TYPE.VIDEO_ANALYSIS: imp_status_update_row(cells, _status._video_analysis, i_state); break;
                            case EVENT_TYPE.TRIP_ZONE: imp_status_update_row(cells, _status._trip_zone, i_state); break;
                            case EVENT_TYPE.TAMPER: imp_status_update_row(cells, _status._tamper, i_state); break;
                            case EVENT_TYPE.FACE_DETECT: imp_status_update_row(cells, _status._face_detect, i_state); break;
                            case EVENT_TYPE.INSTANT_RECORD: imp_status_update_row(cells, _status._instant_record, i_state); break;
                            case EVENT_TYPE.ALARM_IN: imp_status_update_row(cells, _status._alarm_in, i_state); break;
                            case EVENT_TYPE.ALARM_OUT: imp_status_update_row(cells, _status._alarm_out, i_state); break;
                            case EVENT_TYPE.ALARM_IN_NETWORK: imp_status_update_row(cells, _status._alarm_in_network, i_state); break;
                            case EVENT_TYPE.AUDIO_IN: imp_status_update_row(cells, _status._audio_in, i_state); break;
                            case EVENT_TYPE.TEXT_IN: imp_status_update_row(cells, _status._text_in, i_state); break;
                        }
                    }

                    i_state += control.Columns.Count;
                }
            }
        }
        protected void imp_status_update_row(DataGridViewCellCollection collection, byte[] state, int start_i)
        {
            for (int i = 0, j = start_i; i < collection.Count && j < state.Length; ++i, ++j)
            {
                DataGridViewCell cell = collection[i];
                cell.Value = imp_string_from_status(state[j]);
                cell.Style.ForeColor = (state[j] == (byte)G2MONITORING_DEVICE_STATUS.CHECK_EVENT.EVENT) ? Color.Red : Color.Empty;
            }
        }

        private DataGridView imp_control_create(Point loc)
        {
            DataGridView control = new DataGridView();
            ((ISupportInitialize)control).BeginInit();
            control.Location = loc;
            control.Parent = this;
            control.ReadOnly = true;
            control.MultiSelect = false;
            control.Visible = true;
            control.AllowUserToAddRows = false;
            control.AllowUserToDeleteRows = false;
            control.AllowUserToResizeRows = false;
            control.AllowUserToResizeColumns = false;
            control.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            control.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            control.BorderStyle = BorderStyle.None;
            control.GridColor = Color.LightGray;
            control.ScrollBars = ScrollBars.None;
            control.RowTemplate.Height = 22;
            control.RowHeadersWidth = 102;
            control.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            control.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            control.SelectionChanged += new EventHandler(on_dgv_selection_changed);
            ((ISupportInitialize)control).EndInit();
            return control;
        }
        private void imp_control_remove(DataGridView control)
        {
            if (control != null)
            {
                this.Controls.Remove(control);
                control.Dispose();
            }
        }

        private string imp_string_from_type(EVENT_TYPE type)
        {
            return (type == EVENT_TYPE.MOTION) ? "motion" :
                   (type == EVENT_TYPE.OBJECT) ? "object" :
                   (type == EVENT_TYPE.VIDEO_BLIND) ? "blind" :
                   (type == EVENT_TYPE.VIDEO_ANALYSIS) ? "analysis" :
                   (type == EVENT_TYPE.TRIP_ZONE) ? "tripzone" :
                   (type == EVENT_TYPE.TAMPER) ? "tamper" :
                   (type == EVENT_TYPE.FACE_DETECT) ? "face detect" :
                   (type == EVENT_TYPE.INSTANT_RECORD) ? "instant rec." :
                   (type == EVENT_TYPE.ALARM_IN) ? "sensor" :
                   (type == EVENT_TYPE.ALARM_OUT) ? "alarm-out" :
                   (type == EVENT_TYPE.ALARM_IN_NETWORK) ? "net sensor" :
                   (type == EVENT_TYPE.AUDIO_IN) ? "audio" :
                   (type == EVENT_TYPE.TEXT_IN) ? "text-in" : "";
        }
        private string imp_string_from_status(byte s)
        {
            return (s == (byte)G2MONITORING_DEVICE_STATUS.CHECK_EVENT.NO_EVENT) ? "--" :
                   (s == (byte)G2MONITORING_DEVICE_STATUS.CHECK_EVENT.EVENT) ? "on" : "";
        }

        private void on_dgv_selection_changed(object sender, EventArgs e)
        {
            DataGridView dgv = sender as DataGridView;
            dgv.CurrentCell = null;
        }

        private DataGridView[] _controls;
        private G2_PRODUCT_INFO _pi;
        private G2MONITORING_DEVICE_STATUS _status;
        private object _status_cs;
        private bool _reset;
        private int _count_col;
    }

    public partial class form_status
    {
        public void init_status()
        {
            Rectangle rc = new Rectangle(this.STC_STATUS.Location, this.STC_STATUS.Size);
            _status = new feature_status(this, rc);
            _status.Visible = true;
            _status.BringToFront();
        }
        public void feature_status_reset()
        {
            _status.reset(true);
        }
        public void feature_status_set_enable(bool enable)
        {
            _status.set_enable(enable);
        }
        public void feature_status_set_product_info(ref G2_PRODUCT_INFO pi)
        {
            _status.set_product_info(pi);
        }
        public void feature_status_set_status(int channel, ref G2MONITORING_DEVICE_STATUS status)
        {
            _status.set_status(ref status);
        }

        private feature_status _status;
    }
}
