using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SOPBulletin
{
    public partial class DockingProgress2 : Form
    {
        public DockingProgress2()
        {
            InitializeComponent();
            DrawProgressBar(0);
        }

        public void DrawProgressBar(int nPercent)
        {
            progressBar.Value = nPercent;

            labelCurrent.Text = nPercent.ToString() + "%";
            labelCurrent.Location = new Point(labelMin.Location.X + progressBar.Width * nPercent / 100, labelCurrent.Location.Y);
        }

        public void ClearProgress()
        {
            DrawProgressBar(0);
        }

        public void UpdateProgress(ActionStepHistory actionStepHistory)
        {
            if (actionStepHistory == null)
            {
                DrawProgressBar(0);
                return;
            }

            DrawProgressBar(actionStepHistory.CurrentSectionNumberPercentage);
        }

        private void DockingProgress2_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                FormMain2.Instance.ShowContextMenu((Control)sender, e.X, e.Y);
            }
        }
    }
}
