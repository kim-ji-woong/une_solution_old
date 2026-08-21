using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using GDK;

namespace GDK_tester
{
    public interface controller
    {
        void set_enable(bool enable);
        void set_select_date(DateTime time);
        void request_record_time(G2SPOT spot);
        void on_screen_changed_pane(int pane);
        void on_screen_changed_format(screen_format.FORMAT format, screen_format.CHANGED mode);
        void on_screen_image_disp(ref G2FRAME frame);
        void on_screen_play_end_loaded(int channel);
        void on_time_table_move_to_spot(G2SPOT spot);
    }

    public partial class form_search : screen_listener
    {
        public void init_connective_controller()
        {
            init_connective_controller_g1();
            init_connective_controller_g2();
        }
        public void init_connective_controller_g1()
        {
            Rectangle rc = new Rectangle(this.STC_CONTROLLER.Location, this.STC_CONTROLLER.Size);
            _controller_g1 = new controller_search_g1(this, rc);
            _controller_g1.Visible = false;
            _controller_g1.BringToFront();
            _controller_g1.set_adaptor(_adaptor_g1, _screen);
            _controller_g1.set_event_list(LSV_EVENT);
            _controller_g1.set_enable(false);
        }
        public void init_connective_controller_g2()
        {
            Rectangle rc = new Rectangle(this.STC_CONTROLLER.Location, this.STC_CONTROLLER.Size);
            _controller_g2 = new controller_search_g2(this, rc);
            _controller_g1.Visible = true;
            _controller_g2.BringToFront();
            _controller_g2.set_adaptor(_adaptor_g2, _screen);
            _controller_g2.set_event_list(LSV_EVENT);
            _controller_g2.set_enable(false);
            _controller = _controller_g2;
        }

        private controller _controller;
        private controller_search_g2 _controller_g2;
        private controller_search_g1 _controller_g1;
    }
}
