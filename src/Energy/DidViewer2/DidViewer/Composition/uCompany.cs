using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;

namespace DidViewer.Composition
{
    public partial class uCompany : UserControl
    {
        public uCompany(ArrayList datas)
        {
            InitializeComponent();

            this.SetStyle(System.Windows.Forms.ControlStyles.UserPaint, true);
            this.SetStyle(System.Windows.Forms.ControlStyles.OptimizedDoubleBuffer | System.Windows.Forms.ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(System.Windows.Forms.ControlStyles.EnableNotifyMessage, true);

            foreach (Control ctrl in this.Controls)
            {
                if (ctrl is PictureBox)
                    FormMain.Instance.SetDoubleBuffer(ctrl as PictureBox, true);
                else if (ctrl is Label)
                    FormMain.Instance.SetDoubleBuffer(ctrl as Label, true);
            }


            lblName.Text = (datas == null || datas.Count <= 0) ? "" : datas[0].ToString();
            lblWorkProcess.Text = (datas == null || datas.Count <= 1) ? "" : datas[1].ToString();
            lblWorkZone.Text = (datas == null || datas.Count <= 2) ? "" : datas[2].ToString();
            lblStayMembers.Text = (datas == null || datas.Count <= 3) ? "" : datas[3].ToString();
        }
    }
}
