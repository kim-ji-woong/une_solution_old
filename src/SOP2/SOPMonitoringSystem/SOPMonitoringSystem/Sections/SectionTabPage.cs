using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using SOPMonitoringSystem;

namespace Sections
{
    public enum TabPageState 
    {
        USE = 1,
        NOUSE = 2
    }

    public partial class SectionTabPage : TabPage
    {
        public int height = 0;

        public SectionTabPage()
            : base()
        {
            InitializeComponent();
        }

        private bool bVirtualMode = false;
        public bool VirtualMode
        {
            get { return bVirtualMode; }
            set { 
                bVirtualMode = value;
                WatermarkImage();
            }
        }

        private bool bNewCreate = true;
        public bool CreateNew
        {
            get { return bNewCreate; }
            set { bNewCreate = value; }
        }

        private TabPageState mState = TabPageState.NOUSE;
        public Sections.TabPageState State
        {
            get { return mState; }
            set { mState = value; }
        }
        private int nActionStepID = 0;
        public int ActionStepID
        {
            get { return nActionStepID; }
            set { nActionStepID = value; }
        }

        private int m_nActionStepHistoryID = 0;
        public int ActionStepHistoryID
        {
            get { return m_nActionStepHistoryID; }
            set { m_nActionStepHistoryID = value; }
        }

        public bool bUseWaterMark = false;
        public bool UseWaterMark
        {
            get { return bUseWaterMark; }
            set {
                bUseWaterMark = value;
                WatermarkImage();
            }
        }
                
        public void WatermarkImage()
        {
            if (bVirtualMode && UseWaterMark)
            {
                Bitmap bitmap = new Bitmap(global::SOPMonitoringSystem.Properties.Resources.BackgroundLog);
                foreach (Control contorl in Controls)
                {
                    if( contorl.GetType() == typeof(PanelSectionEx))
                    {
                        Sections.PanelSectionEx panel = (PanelSectionEx)contorl;
                        panel.BackgroundImage = bitmap;
                        panel.BackgroundImageLayout = ImageLayout.None;
                    }
                        
                }
            }
            else
            {
                Bitmap bitmap = new Bitmap(global::SOPMonitoringSystem.Properties.Resources.BackgroundNon);
                foreach (Control contorl in Controls)
                {
                    if( contorl.GetType() == typeof(PanelSectionEx))
                    {
                        Sections.PanelSectionEx panel = (PanelSectionEx)contorl;
                        panel.BackgroundImage = bitmap;
                        panel.BackgroundImageLayout = ImageLayout.None;
                    }
                        
                }                    
            }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // SectionTabPage
            // 
            this.SizeChanged += new System.EventHandler(this.SectionTabPage_SizeChanged);           
            this.ResumeLayout(false);
        }

	    public new System.Drawing.Size Size
	    {
		    get { return base.Size; }
		    set { base.Size = value; }
	    }

        public void ReSizePanel()
        {
            if (SOPMonitoringSystem.FormMain.Instance == null)
                return;

            if (SOPMonitoringSystem.FormMain.Instance.GetPageHome() == null)
                return;

            TabControl tabControl = SOPMonitoringSystem.FormMain.Instance.GetPageHome().TabControls; 
                
            if( tabControl != null)
            {
                TabPage tabPage1 = this;
                Size sz = tabPage1.Size;
                int nCount = tabPage1.Controls.Count;
                int nVisibleCount = 0;
                foreach (Sections.PanelSectionEx panel in tabPage1.Controls)
                {
                    if (panel.Visible == true)
                        nVisibleCount++;
                    else
                    {
                        panel.Size = new System.Drawing.Size(sz.Width, sz.Height);
                        panel.Location = new System.Drawing.Point(0, 0);
                    }
                }

                if (nCount > 0 && nVisibleCount > 0)
                {
                    sz.Width = tabPage1.Width / nVisibleCount;
                    sz.Height = tabPage1.Size.Height;
                    Point pt = new Point(0, 0);
                    foreach (Sections.PanelSectionEx panel in tabPage1.Controls)
                    {
                        if (panel.Visible == true)
                        {
                            panel.Size = new System.Drawing.Size(sz.Width, sz.Height);
                            panel.Location = new System.Drawing.Point(pt.X, 0);
                            pt.X += sz.Width;
                        }
                    }
                }
            }
        }

        private void SectionTabPage_SizeChanged(object sender, EventArgs e)
        {
            ReSizePanel();
        }
    }     
}
