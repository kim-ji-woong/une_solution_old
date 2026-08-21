using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using libExternalUI;

namespace Sample
{
    public partial class FormMain : Form
    {
        private IUIManager m_uiManager = null;
        public FormMain()
        {
            InitializeComponent();
            m_uiManager = SampleLib.SampleFactory.GetUIManager(this);
        }

        private void btnShow_Click(object sender, EventArgs e)
        {
            if (m_uiManager != null)
            {
                if (sender == btnShowRed)
                    m_uiManager.ShowControl(1);
                else if (sender == btnShowTwice)
                    m_uiManager.ShowControl(2);
                else if (sender == btnShowBoth)
                    m_uiManager.ShowControl(3);
            }
        }

        private void btnHide_Click(object sender, EventArgs e)
        {
            if (m_uiManager != null)
            {
                if (sender == btnShowRed)
                    m_uiManager.HideControl(1);
                else if (sender == btnShowTwice)
                    m_uiManager.HideControl(2);
                else if (sender == btnShowBoth)
                    m_uiManager.HideControl(3);
            }
        }

        private void FormMain_Resize(object sender, EventArgs e)
        {
            if (m_uiManager != null)
                m_uiManager.OnResize();
        }
    }
}
