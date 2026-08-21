using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using XtremeCommandBars;

namespace UBMLViewer
{
    public partial class PageBackstageHome : Form
    {
        private CommandBarsGlobalSettings CommandBarsGlobalSettings;
        private static PageBackstageHome m_home = null;
        public static PageBackstageHome Instance
        {
            get { return m_home; }
        }


        public AxXtremeDockingPane.AxDockingPane DockingPane
        {
            get { return m_axDockingPane; }
        }

        public System.Windows.Forms.Panel ContentPanel
        {
            get { return m_ContentPanel; }
        }

        public System.Windows.Forms.ToolStripContainer ViewToolStripContainer
        {
            get { return m_ToolStripContainer; }
        }

        public System.Windows.Forms.ToolStrip ToolStripOperator
        {
            get { return m_ToolStripOperator; }
        }

        public System.Windows.Forms.ToolStrip ToolStrip3DView
        {
            get { return m_ToolStrip3DView; }
        }

        public PageBackstageHome()
        {
            m_home = this;

            InitializeComponent();
        }


        private void CreateDockingPane()
        {
        }

        private void DockingPane_ResizeEvent(object sender, EventArgs e)
        {
            int left, top, right, bottom;
            DockingPane.GetClientRect(out left, out top, out right, out bottom);
            m_ToolStripContainer.SetBounds(left, top, right - left, bottom - top);
        }

        private void DockingPane_AttachPaneEvent(object sender, AxXtremeDockingPane._DDockingPaneEvents_AttachPaneEvent e)
        {

        }

        //public void OnChangeTheme(object sender, AxXtremeCommandBars._DCommandBarsEvents_ExecuteEvent e)
        //{
        //    switch (e.control.Id)
        //    {
        //        case ID.ID_OPTIONS_STYLEBLACK:
        //            {
        //                CommandBarsGlobalSettings = new XtremeCommandBars.CommandBarsGlobalSettings();
        //                CommandBarsGlobalSettings.ResourceImages.LoadFromFile(FormMain.StylesPath() + "Office2007.dll", "Office2007Black.ini");
        //                CommandBars.PaintManager.RefreshMetrics();
        //                CommandBars.RecalcLayout();
        //            }
        //            break;
        //        case ID.ID_OPTIONS_STYLEBLUE:
        //            {
        //                CommandBarsGlobalSettings = new XtremeCommandBars.CommandBarsGlobalSettings();
        //                CommandBarsGlobalSettings.ResourceImages.LoadFromFile("", "");                        
        //                CommandBars.PaintManager.RefreshMetrics();
        //                CommandBars.RecalcLayout();
        //            }
        //            break;
        //        case ID.ID_OPTIONS_STYLEAQUA:
        //            {
        //                CommandBarsGlobalSettings = new XtremeCommandBars.CommandBarsGlobalSettings();
        //                CommandBarsGlobalSettings.ResourceImages.LoadFromFile(FormMain.StylesPath() + "Office2007.dll", "Office2007Aqua.ini");
                       
        //                CommandBars.PaintManager.RefreshMetrics();
        //                CommandBars.RecalcLayout();
        //            }
        //            break;
        //        case ID.ID_OPTIONS_STYLESILVER:
        //            {
        //                CommandBarsGlobalSettings = new XtremeCommandBars.CommandBarsGlobalSettings();
        //                CommandBarsGlobalSettings.ResourceImages.LoadFromFile(FormMain.StylesPath() + "Office2007.dll", "Office2007Silver.ini");
                        
        //                CommandBars.PaintManager.RefreshMetrics();
        //                CommandBars.RecalcLayout();
        //            }
        //            break;
        //        case ID.ID_OPTIONS_STYLEOFFCIE2010BLUE:
        //            {
        //                CommandBarsGlobalSettings = new XtremeCommandBars.CommandBarsGlobalSettings();
        //                CommandBarsGlobalSettings.ResourceImages.LoadFromFile(FormMain.StylesPath() + "Office2010.dll", "Office2010Blue.ini");
                        
        //                CommandBars.PaintManager.RefreshMetrics();
        //                CommandBars.RecalcLayout();
        //            }
        //            break;
        //        case ID.ID_OPTIONS_STYLEOFFICE2010SILVER:
        //            {
        //                CommandBarsGlobalSettings = new XtremeCommandBars.CommandBarsGlobalSettings();
        //                CommandBarsGlobalSettings.ResourceImages.LoadFromFile(FormMain.StylesPath() + "Office2010.dll", "Office2010Silver.ini");
                        
        //                CommandBars.PaintManager.RefreshMetrics();
        //                CommandBars.RecalcLayout();
        //            }
        //            break;
        //        case ID.ID_OPTIONS_STYLEOFFCIE2010BLACK:
        //            {
        //                CommandBarsGlobalSettings = new XtremeCommandBars.CommandBarsGlobalSettings();
        //                CommandBarsGlobalSettings.ResourceImages.LoadFromFile(FormMain.StylesPath() + "Office2010.dll", "Office2010Black.ini");
        //                CommandBars.PaintManager.RefreshMetrics();
        //                CommandBars.RecalcLayout();
        //            }
        //            break;
        //        case ID.ID_OPTIONS_STYLESCENIC:
        //            {
        //                CommandBarsGlobalSettings = new XtremeCommandBars.CommandBarsGlobalSettings();
        //                CommandBarsGlobalSettings.ResourceImages.LoadFromFile(FormMain.StylesPath() + "Windows7.dll", "Windows7Blue.ini");                        
        //                CommandBars.PaintManager.RefreshMetrics();
        //                CommandBars.RecalcLayout();
        //            }
        //            break;
        //        default:
        //            break;
        //    };
        //}

        //private void m_axCommandBars_Execute(object sender, AxXtremeCommandBars._DCommandBarsEvents_ExecuteEvent e)
        //{

        //}

        //private void m_axCommandBars_UpdateEvent(object sender, AxXtremeCommandBars._DCommandBarsEvents_UpdateEvent e)
        //{

        //}

        //private void m_axCommandBars_ResizeEvent(object sender, EventArgs e)
        //{
   
        //}
    }
}
