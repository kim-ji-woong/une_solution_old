using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GDK;
using GDK_tester;

namespace UnESample2
{
    public partial class FormMain : screen_listener
    {
        public void init_connective_screen()
        {
            _screen = SCREEN_PANE;
            _screen.option_use_scaler_G2 = true;
            _screen.create(1, 1);
            _screen.set_format(1, false);
            _screen.set_pane_status(0, camera_pane.STATUS.NOT_CONNECTED, false);
            /*_screen.set_pane_status(1, camera_pane.STATUS.NOT_CONNECTED, false);
            _screen.set_pane_status(2, camera_pane.STATUS.NOT_CONNECTED, false);
            _screen.set_pane_status(3, camera_pane.STATUS.NOT_CONNECTED, false);*/
            _screen.run();
            _screen.update();
        }

        public void on_screen_changed_pane(int pane) { }
        public void on_screen_changed_format(screen_format.FORMAT format, screen_format.CHANGED mode) { }
        public void on_screen_image_disp(int pane, ref G2FRAME frame) { }
        public void on_screen_search_stopped(int channel, int pane, G2SPOT spot) { }
        public void on_screen_play_end_loaded(int channel) { }
    }
}
