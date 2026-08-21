using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SOPMonitoringSystem
{
    public partial class FormStatus : Form
    {
        [System.Runtime.InteropServices.DllImport("User32.dll", EntryPoint = "SetParent", ExactSpelling = false)]
        static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndParent);

        public FormStatus(FormMain frmParent)
        {
            InitializeComponent();
            SetParent(this.Handle, frmParent.Handle);
        }

        public void RealMode(bool isCheck)
        {
            if (isCheck) //실제모드
            {
                pictureBox1.Image = global::SOPMonitoringSystem.Properties.Resources.mode_Real;
            }
            else
            {
                pictureBox1.Image = global::SOPMonitoringSystem.Properties.Resources.mode_Training;
            }
        }

        public void StatusBoard(Sections.WorkFlowState state)
        {
            switch(state)
            {
                case Sections.WorkFlowState.STANDBY:
                case Sections.WorkFlowState.WAIT:
                    pictureBox2.Image = global::SOPMonitoringSystem.Properties.Resources.run_wait;
                    break;
                case Sections.WorkFlowState.RUN:
                    pictureBox2.Image = global::SOPMonitoringSystem.Properties.Resources.run_play;
                    break;
                case Sections.WorkFlowState.STOP:
                    pictureBox2.Image = global::SOPMonitoringSystem.Properties.Resources.run_stop;
                    break;
                case Sections.WorkFlowState.DONE:
                    pictureBox2.Image = global::SOPMonitoringSystem.Properties.Resources.run_complete;
                    break;
            }
        }
    }
}
