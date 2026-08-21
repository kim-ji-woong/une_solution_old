using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DIDViewer
{
    public class DoubleBufferedPanel : Panel
    {

        public DoubleBufferedPanel()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);
            this.DoubleBuffered = true;
            this.UpdateStyles();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // DoubleBufferedPanel
            // 
            this.ControlRemoved += new System.Windows.Forms.ControlEventHandler(this.DoubleBufferedPanel_ControlRemoved);
            this.ResumeLayout(false);

        }

        private void DoubleBufferedPanel_ControlRemoved(object sender, ControlEventArgs e)
        {
            this.Controls.Clear();
        }
    }
}
