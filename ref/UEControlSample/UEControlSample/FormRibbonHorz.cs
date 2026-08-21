using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace UEControlSample
{
    public partial class FormRibbonHorz : Form, UnE.GUI.IRibbonButtonOwner
    {
        public FormRibbonHorz()
        {
            InitializeComponent();
            this.TopLevel = false;
        }

        private void FormRibbonHorz_Load(object sender, System.EventArgs e)
        {
            ribbonButton1.Owner = this;
            ribbonButton2.Owner = this;
            ribbonButton3.Owner = this;
        }

        public void OnRibbonButtonMouseDown(object sender, MouseEventArgs e)
        {
        }

        public void OnRibbonButtonMouseUp(object sender, MouseEventArgs e)
        {
            if (sender != ribbonButton1 && ribbonButton1.IsChecked)
                ribbonButton1.IsChecked = false;

            if (sender != ribbonButton2 && ribbonButton2.IsChecked)
                ribbonButton2.IsChecked = false;

            if (sender != ribbonButton3 && ribbonButton3.IsChecked)
                ribbonButton3.IsChecked = false;

            ((UnE.GUI.RibbonButton)sender).IsChecked = true;

            Refresh();
        }
    }
}
