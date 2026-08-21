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
    public partial class PopupMissionText : Form
    {
        protected static PopupMissionText instance = null;
        public static PopupMissionText Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new PopupMissionText();
                }
                return instance;
            }
        }

        public PopupMissionText()
        {
           // this.Parent = FormMain.Instance;
            InitializeComponent();
        }


        public void SetText(string szText, string szTextTarget, string szTextMedium)
        {
            this.Activate();
            this.TopMost = true;
            this.BringToFront();


            if (!FormMain.Instance.ShowMissionText)
                return;

            if (Visible == false)
            {
                this.TopMost = true;
                this.Show();
            }

            if (this.WindowState != FormWindowState.Normal)
            {
                this.WindowState = FormWindowState.Normal;
                this.Activate();
            }           

            textBox1.Text = szText;
			textBox2.Text = szTextTarget;
            textBoxMedium.Text = szTextMedium;
        }

        private void PopupMissionText_FormClosing(object sender, FormClosingEventArgs e)
        {
            instance = null;
        }
    }
}
