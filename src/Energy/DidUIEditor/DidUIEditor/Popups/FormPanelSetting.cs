using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DidUIEditor.Popups
{
    public partial class FormPanelSetting : Form
    {
        public Size SetSize
        {
            get { return new Size((int)numWidth.Value, (int)numHeight.Value); }
        }

        public Point SetLocation
        {
            get { return new Point((int)numX.Value, (int)numY.Value); }
        }

        private Size orgSize = new Size();
        private Point orgPT = new Point();

        public FormPanelSetting(Size size, Point pt)
        {
            InitializeComponent();

            orgSize = size;
            orgPT = pt;

            numWidth.Value = orgSize.Width;
            numHeight.Value = orgSize.Height;
            numX.Value = orgPT.X;
            numY.Value = orgPT.Y;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {   
            if (orgSize != new Size((int)numWidth.Value, (int)numHeight.Value) ||
                orgPT != new Point((int)numX.Value, (int)numY.Value))
            {                  
                this.DialogResult = DialogResult.Yes;
            }

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.No;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.No;
        }
    }
}
