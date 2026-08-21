using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

using GDK;

namespace GDK_tester
{
    public partial class form_backup
    {
        private form_controller_backup _controller;

        public void init_control_controller()
        {
            Rectangle rc = new Rectangle(this.STC_CONTROLLER.Location, this.STC_CONTROLLER.Size);
            _controller = new form_controller_backup(this, rc, _backup_info, _service, _site);
            _controller.Visible = true;
            _controller.BringToFront();
            _controller.set_adaptor(_adaptor, _screen, _user_adaptor);
            _controller.set_event_list(LSV_EVENT);
            _controller.set_enable(false);
        }
    }
}
