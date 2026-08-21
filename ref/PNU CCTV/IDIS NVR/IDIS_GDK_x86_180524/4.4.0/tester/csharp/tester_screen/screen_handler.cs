using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using GDK;

namespace GDK_tester
{
    public partial class screen_pane
    {
        private void on_paint(object sender, PaintEventArgs e)
        {
            present(e.Graphics);
        }
        private void on_mouse_click(object sender, MouseEventArgs e)
        {
            Point pt = e.Location;
            int pane = 0;
            camera_pane selectedCamera = null;

            lock (this)
            {
                int i = 0;
                foreach (camera_pane cp in _panes)
                {
                    if (cp._visible &&
                        cp.pt_in_pane(pt))
                    {
                        pane = i;
                        selectedCamera = cp;
                        break;
                    }
                    i++;
                }
            }

            //set_pane_select(pane);

            if (_listener != null)
            {
                _listener.on_mouse_click(selectedCamera, e);
            }
        }
        private void on_mouse_double_click(object sender, MouseEventArgs e)
        {
            Point pt = e.Location;
            int pane = 0;
            camera_pane selectedCamera = null;

            lock (this)
            {
                int i = 0;
                foreach (camera_pane cp in _panes)
                {
                    if (cp._visible &&
                        cp.pt_in_pane(pt))
                    {
                        pane = i;
                        selectedCamera = cp;
                        break;
                    }
                    i++;
                }
            }

            /*set_pane_select(pane, false);

            if (_format.is_format1x1())
            {
                set_format(_format_pre, true);
            }
            else
            {
                if (valid_pane(pane))
                {
                    set_format(screen_format.FORMAT.LAYOUT1X1, true);
                }
                else
                {
                    update();
                }
            }*/

            if (_listener != null)
            {
                _listener.on_mouse_doubleclick(selectedCamera, e);
            }
        }
    }
}
