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
    public partial class form_ptz_menu : Form
    {
        public class event_args : EventArgs
        {
            public G2LIVE_PTZ_COMMAND.COMMAND command;
            public int camera;
            public int i;
        }
        public class internal_data
        {
            public static int i_scan = 1;
            public static int i_tour = 1;
            public static int i_pattern = 1;
        }

        public form_ptz_menu()
        {
            InitializeComponent();

            this.BTN_AUTO_PAN.Tag = G2LIVE_PTZ_COMMAND.COMMAND.PTZ_AUTO_PAN_ON;
            this.BTN_SCAN.Tag = this.BTN_SCAN_ID.Tag = G2LIVE_PTZ_COMMAND.COMMAND.PTZ_SCAN_ON;
            this.BTN_TOUR.Tag = this.BTN_TOUR_ID.Tag = G2LIVE_PTZ_COMMAND.COMMAND.PTZ_TOUR_ON;
            this.BTN_PATTERN.Tag = this.BTN_PATTERN_ID.Tag = G2LIVE_PTZ_COMMAND.COMMAND.PTZ_PATTERN_ON;
            this.BTN_OSD_MENU.Tag = G2LIVE_PTZ_COMMAND.COMMAND.PTZ_MENU_ON;
            this.BTN_LIGHT.Tag = G2LIVE_PTZ_COMMAND.COMMAND.PTZ_LIGHT_TURNON;
            this.BTN_PUMP.Tag = G2LIVE_PTZ_COMMAND.COMMAND.PTZ_PUMP_TURNON;
            this.BTN_WIPER.Tag = G2LIVE_PTZ_COMMAND.COMMAND.PTZ_WIPER_TURNON;
            this.BTN_POWER.Tag = G2LIVE_PTZ_COMMAND.COMMAND.PTZ_POWER_TURNON;
            this.BTN_AUX.Tag = G2LIVE_PTZ_COMMAND.COMMAND.PTZ_AUX_TURNON;
            this.camera = -1;
        }
        public void set_features(ref G2LIVE_PTZ_MENU features)
        {
            this.features = features;

            CHK_CONTROL.Enabled = this.features.is_function(G2LIVE_PTZ_MENU.FUNCTION.CTRL);
            CHK_CONTROL.Checked = (this.features._active_CTRL);
            BTN_SPEED.Enabled = this.features.is_function(G2LIVE_PTZ_MENU.FUNCTION.SPEED);
            BTN_AUTO_PAN.Enabled = this.features.is_function(G2LIVE_PTZ_MENU.FUNCTION.AUTOPAN);
            BTN_TOUR.Enabled =
            BTN_TOUR_ID.Enabled = this.features.is_function(G2LIVE_PTZ_MENU.FUNCTION.TOUR);
            BTN_PATTERN.Enabled =
            BTN_PATTERN_ID.Enabled = this.features.is_function(G2LIVE_PTZ_MENU.FUNCTION.PATTERN);
            BTN_SCAN.Enabled =
            BTN_SCAN_ID.Enabled = this.features.is_function(G2LIVE_PTZ_MENU.FUNCTION.SCAN);
            BTN_OSD_MENU.Enabled = this.features.is_function(G2LIVE_PTZ_MENU.FUNCTION.OSDMENU);
            BTN_LIGHT.Enabled = this.features.is_function(G2LIVE_PTZ_MENU.FUNCTION.LIGHT);
            BTN_PUMP.Enabled = this.features.is_function(G2LIVE_PTZ_MENU.FUNCTION.PUMP);
            BTN_WIPER.Enabled = this.features.is_function(G2LIVE_PTZ_MENU.FUNCTION.WIPER);
            BTN_POWER.Enabled = this.features.is_function(G2LIVE_PTZ_MENU.FUNCTION.POWER);
            BTN_AUX.Enabled = this.features.is_function(G2LIVE_PTZ_MENU.FUNCTION.AUX);
            BTN_MOVE_TO_ORIGIN.Enabled = this.features.is_function(G2LIVE_PTZ_MENU.FUNCTION.ORIGIN);
            BTN_ENTER.Enabled = this.features.is_function(G2LIVE_PTZ_MENU.FUNCTION.ENTER);
            BTN_ESC.Enabled = this.features.is_function(G2LIVE_PTZ_MENU.FUNCTION.ESC);
        }

        private void on_btn_required_ID(object sender, EventArgs e)
        {
            ContextMenu pop = new ContextMenu();
            Control control = sender as Control;

            if (sender == BTN_SPEED)
            {
                for (int i = 1; i < 17; ++i)
                {
                    MenuItem mi = new MenuItem(i.ToString(), new EventHandler(on_menu_speed));
                    pop.MenuItems.Add(mi);
                }
            }
            else
            {
                int select = (G2LIVE_PTZ_COMMAND.COMMAND)control.Tag == G2LIVE_PTZ_COMMAND.COMMAND.PTZ_SCAN_ON ? internal_data.i_scan :
                             (G2LIVE_PTZ_COMMAND.COMMAND)control.Tag == G2LIVE_PTZ_COMMAND.COMMAND.PTZ_TOUR_ON ? internal_data.i_tour :
                             (G2LIVE_PTZ_COMMAND.COMMAND)control.Tag == G2LIVE_PTZ_COMMAND.COMMAND.PTZ_PATTERN_ON ? internal_data.i_pattern : -1;

                for (int i = 1; i < 17; ++i)
                {
                    MenuItem mi = new MenuItem(i.ToString(), new EventHandler(on_menu_id));
                    mi.Tag = control.Tag;
                    if (select == i)
                    {
                        mi.Checked = true;
                    }
                    pop.MenuItems.Add(mi);
                }
            }

            Point point = new Point(control.Right, control.Top);
            GroupBox parent = control.Parent as GroupBox;
            if (parent != null)
            {
                point.Offset(parent.Location);
            }

            pop.Show(this, point);
        }
        private void on_btn_rquired_on_off(object sender, EventArgs e)
        {
            Control control = sender as Control;
            ContextMenu pop = new ContextMenu();
            MenuItem[] mis = new MenuItem[2] { new MenuItem("On", new EventHandler(on_menu_on)),
                                               new MenuItem("Off", new EventHandler(on_menu_off)) };
            foreach (MenuItem mi in mis)
            {
                mi.Tag = control.Tag;
            }
            pop.MenuItems.AddRange(mis);

            if (sender == BTN_PATTERN)
            {
                if (this.features.is_function(G2LIVE_PTZ_MENU.FUNCTION.REC_PATTERN))
                {
                    MenuItem mi = pop.MenuItems.Add("Record");
                    mi.MenuItems.Add("Start", new EventHandler(on_menu_pattern_rec_start));
                    mi.MenuItems.Add("Stop", new EventHandler(on_menu_pattern_rec_stop));
                }
            }
            else if (sender == BTN_OSD_MENU)
            {
                if (this.features.is_function(G2LIVE_PTZ_MENU.FUNCTION.OSDMENU_OFF_NONE))
                {
                    pop.MenuItems[1].Enabled = false;
                }
            }

            Point point = new Point(control.Location.X, control.Bottom);
            GroupBox parent = control.Parent as GroupBox;
            if (parent != null)
            {
                point.Offset(parent.Location);
            }

            pop.Show(this, point);
        }
        private void on_btn_move_to_origin(object sender, EventArgs e)
        {
            event_args args = new event_args();
            args.command = G2LIVE_PTZ_COMMAND.COMMAND.PTZ_MOVETO_ORIGIN;
            args.camera = camera;
            if (handler != null)
            {
                handler(this, args);
            }
        }
        private void on_chk_set_control(object sender, EventArgs e)
        {
            CheckBox c = sender as CheckBox;
            event_args args = new event_args();
            args.command = G2LIVE_PTZ_COMMAND.COMMAND.PTZ_SETCTRL_ON;
            args.camera = camera;
            args.i = (c.Checked != false) ? 1 : 0;
            if (handler != null)
            {
                handler(this, args);
            }
        }
        private void on_btn_control(object sender, EventArgs e)
        {
            event_args args = new event_args();
            args.camera = camera;
            if (sender == BTN_ENTER) args.command = G2LIVE_PTZ_COMMAND.COMMAND.PTZ_MENU_ENTER;
            else if (sender == BTN_ESC) args.command = G2LIVE_PTZ_COMMAND.COMMAND.PTZ_MENU_ESC;
            else return;

            if (handler != null)
            {
                handler(this, args);
            }
        }
        private void on_menu_speed(object sender, EventArgs e)
        {
            MenuItem mi = sender as MenuItem;
            event_args args = new event_args();
            args.command = G2LIVE_PTZ_COMMAND.COMMAND.PTZ_SET_SPEED;
            args.camera = camera;
            args.i = mi.Index;
            if (handler != null)
            {
                handler(this, args);
            }
        }
        private void on_menu_id(object sender, EventArgs e)
        {
            MenuItem mi = sender as MenuItem;
            int i = mi.Index + 1;

            switch ((G2LIVE_PTZ_COMMAND.COMMAND)mi.Tag)
            {
                case G2LIVE_PTZ_COMMAND.COMMAND.PTZ_SCAN_ON: internal_data.i_scan = i; break;
                case G2LIVE_PTZ_COMMAND.COMMAND.PTZ_TOUR_ON: internal_data.i_tour = i; break;
                case G2LIVE_PTZ_COMMAND.COMMAND.PTZ_PATTERN_ON: internal_data.i_pattern = i; break;
            }
        }
        private void on_menu_on(object sender, EventArgs e)
        {
            MenuItem mi = sender as MenuItem;

            event_args args = new event_args();
            args.camera = camera;
            args.command = (G2LIVE_PTZ_COMMAND.COMMAND)mi.Tag;

            switch (args.command)
            {
                case G2LIVE_PTZ_COMMAND.COMMAND.PTZ_SCAN_ON: args.i = internal_data.i_scan; break;
                case G2LIVE_PTZ_COMMAND.COMMAND.PTZ_TOUR_ON: args.i = internal_data.i_tour; break;
                case G2LIVE_PTZ_COMMAND.COMMAND.PTZ_PATTERN_ON: args.i = internal_data.i_pattern; break;
            }

            if (handler != null)
            {
                handler(this, args);
            }
        }
        private void on_menu_off(object sender, EventArgs e)
        {
            MenuItem mi = sender as MenuItem;

            event_args args = new event_args();
            args.camera = camera;

            switch ((G2LIVE_PTZ_COMMAND.COMMAND)mi.Tag)
            {
                case G2LIVE_PTZ_COMMAND.COMMAND.PTZ_MENU_ON: args.command = G2LIVE_PTZ_COMMAND.COMMAND.PTZ_MENU_OFF; break;
                case G2LIVE_PTZ_COMMAND.COMMAND.PTZ_SCAN_ON: args.command = G2LIVE_PTZ_COMMAND.COMMAND.PTZ_SCAN_OFF; args.i = internal_data.i_scan; break;
                case G2LIVE_PTZ_COMMAND.COMMAND.PTZ_TOUR_ON: args.command = G2LIVE_PTZ_COMMAND.COMMAND.PTZ_TOUR_OFF; args.i = internal_data.i_tour; break;
                case G2LIVE_PTZ_COMMAND.COMMAND.PTZ_PATTERN_ON: args.command = G2LIVE_PTZ_COMMAND.COMMAND.PTZ_PATTERN_OFF; args.i = internal_data.i_pattern; break;
            }

            if (handler != null)
            {
                handler(this, args);
            }
        }
        private void on_menu_pattern_rec_start(object sender, EventArgs e)
        {
            event_args args = new event_args();
            args.command = G2LIVE_PTZ_COMMAND.COMMAND.PTZ_PATTERN_RECORD_ON;
            args.camera = camera;
            args.i = internal_data.i_pattern;

            if (handler != null)
            {
                handler(this, args);
            }
        }
        private void on_menu_pattern_rec_stop(object sender, EventArgs e)
        {
            event_args args = new event_args();
            args.command = G2LIVE_PTZ_COMMAND.COMMAND.PTZ_PATTERN_RECORD_OFF;
            args.camera = camera;
            args.i = internal_data.i_pattern;

            if (handler != null)
            {
                handler(this, args);
            }
        }

        public G2LIVE_PTZ_MENU features;
        public int camera;
        public event EventHandler handler;
    }
}
