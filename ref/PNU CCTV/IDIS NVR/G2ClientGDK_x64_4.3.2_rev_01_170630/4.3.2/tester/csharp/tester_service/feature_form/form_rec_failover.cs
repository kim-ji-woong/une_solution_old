using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;

using GDK;

namespace GDK_tester
{
    partial class form_rec_failover : Form
    {
        public form_rec_failover(List<string[]> datas, bool enable)
        {
            InitializeComponent();

            foreach (string[] s in datas)
            {
                DGV_REC_FAILOVER.Rows.Add(s);
            }
            DGV_REC_FAILOVER.ClearSelection();

            if (enable)
            {
                BTN_SWITCH_FAILOVER.Enabled = true;                
            }
            else
            {
                BTN_SWITCH_FAILOVER.Text = "service offline";
            }
        }
    }
}
