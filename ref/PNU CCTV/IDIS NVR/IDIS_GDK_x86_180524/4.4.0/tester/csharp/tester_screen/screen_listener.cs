using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using GDK;

namespace GDK_tester
{
    public interface screen_listener
    {
        void on_screen_changed_pane(int pane);
        void on_screen_changed_format(screen_format.FORMAT format, screen_format.CHANGED mode);
        void on_screen_image_disp(int pane, ref G2FRAME frame);
        void on_screen_search_stopped(int channel, int pane, G2SPOT spot);
        void on_screen_play_end_loaded(int channel);
        void on_mouse_click(camera_pane cp, MouseEventArgs e);
        void on_mouse_doubleclick(camera_pane cp, MouseEventArgs e);
    }
}
