using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SOPMonitoringSystem
{
    public partial class SectionTabControl : TabControl
    {
        public SectionTabControl()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // SectionTabControl
            // 
            this.Selected += new System.Windows.Forms.TabControlEventHandler(this.SectionTabControl_Selected);
            this.SizeChanged += new System.EventHandler(this.SectionTabControl_SizeChanged);
            this.ResumeLayout(false);

        }

        private void SectionTabControl_Selected(object sender, TabControlEventArgs e)
        {
            if (this.SelectedTab != null)
            {
                Sections.SectionTabPage page = (Sections.SectionTabPage)this.SelectedTab;
                if( page != null)
                    page.ReSizePanel();
            }
        }

        private void SectionTabControl_SizeChanged(object sender, EventArgs e)
        {
            foreach (Sections.SectionTabPage page in Controls)
            {
                page.ReSizePanel();
            }           
           
        }
    }
}
