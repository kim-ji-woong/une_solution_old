using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using GDK;

namespace GDK_tester
{
    public partial class form_search : time_table_listener
    {
        public void init_connective_time_table()
        {
            _table_minute = new time_table_minute(this);
            _table_minute.init(new Rectangle(this.STC_TIME_TABLE.Location, this.STC_TIME_TABLE.Size), true);
            _table_minute.set_listener(this);
            _table_hour = new time_table_hour(this);
            _table_hour.init(new Rectangle(this.STC_TIME_TABLE.Location, this.STC_TIME_TABLE.Size), true);
            _table_hour.set_listener(this);
            _table_hour.Visible = false;
            _table_IDR = new time_table_IDR(this);
            _table_IDR.init(new Rectangle(this.STC_TIME_TABLE.Location, this.STC_TIME_TABLE.Size), true);
            _table_IDR.set_listener(this);
            _table_IDR.Visible = false;
            _table = _table_minute;
            _table_set = new time_table_base[] { _table_minute, _table_hour, _table_IDR };
        }

        public void load_time_table(int channel)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke((MethodInvoker)delegate() { on_post_load_time_table(channel); });
                return;
            }

            if (_adaptor is g2search_g2)
            {
                imp_load_time_table_g2(channel);
            }
            else
            {
                imp_load_time_table_g1(channel);
            }
        }

        public void on_time_table_move_to_spot(G2SPOT spot)
        {
            _controller.on_time_table_move_to_spot(spot);
        }
        public G2SEARCH_PLAYBACK.COMMAND on_time_table_get_current_command()
        {
            return _adaptor_g1.get_current_command(_channel);
        }

        private void on_post_load_time_table(int channel)
        {
            load_time_table(channel);
        }

        private time_table_base _table;
        private time_table_base[] _table_set;
        private time_table_minute _table_minute;
        private time_table_hour _table_hour;
        private time_table_IDR _table_IDR;
    }
}
