using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using libExternalUI;
using System.Windows.Forms;

namespace Sample.SampleLib
{
    public class SampleUIManager : IUIManager
    {
        private FormYoutube m_frmRed = null;
        private FormYoutube m_frmTwice = null;
        private Control m_parentCtrl = null;

        public SampleUIManager(Control parent)
        {
            m_frmRed = new FormYoutube("https://www.youtube.com/watch?v=YBnGBb1wg98");
            m_frmTwice = new FormYoutube("https://www.youtube.com/watch?v=rRzxEiBLQCA");
            m_parentCtrl = parent;
            Init();
        }

        private void Init()
        {
            int nParentWidth = m_parentCtrl.ClientSize.Width;
            int nParentHeight = m_parentCtrl.ClientSize.Height;

            int nFormWidth = nParentWidth / 3;
            int nFormHeight = nParentHeight / 3;

            m_frmRed.Location = new System.Drawing.Point(nParentWidth - nFormWidth, nParentHeight - nFormHeight);
            m_frmRed.Size = new System.Drawing.Size(nFormWidth, nFormHeight);
            m_parentCtrl.Controls.Add(m_frmRed);

            m_frmTwice.Location = new System.Drawing.Point(nParentWidth - nFormWidth, 0);
            m_frmTwice.Size = new System.Drawing.Size(nFormWidth, nFormHeight);
            m_parentCtrl.Controls.Add(m_frmTwice);
        }

        public void ShowControl(object arg)
        {
            if (arg != null && arg is int)
            {
                int option = (int)arg;

                if (option == 1)
                    m_frmRed.Show();
                else if (option == 2)
                    m_frmTwice.Show();
                else if (option == 3)
                {
                    m_frmRed.Show();
                    m_frmTwice.Show();
                }
            }
        }

        public void HideControl(object arg)
        {
            if (arg != null && arg is int)
            {
                int option = (int)arg;

                if (option == 1)
                    m_frmRed.Hide();
                else if (option == 2)
                    m_frmTwice.Hide();
                else if (option == 3)
                {
                    m_frmRed.Hide();
                    m_frmTwice.Hide();
                }
            }
        }

        public void OnResize()
        {
            int nParentWidth = m_parentCtrl.ClientSize.Width;
            int nParentHeight = m_parentCtrl.ClientSize.Height;

            m_frmRed.Location = new System.Drawing.Point(nParentWidth - m_frmRed.Size.Width, nParentHeight - m_frmRed.Size.Height);
            m_frmTwice.Location = new System.Drawing.Point(nParentWidth - m_frmTwice.Size.Width, 0);
        }
    }
}
