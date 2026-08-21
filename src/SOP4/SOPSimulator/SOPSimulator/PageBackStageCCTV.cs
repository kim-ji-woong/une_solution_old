using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SOPMonitoringSystem
{
    public partial class PageBackStageCCTV : Form
    {

        private static PopupTranslucentForm mTranslucentForm = new PopupTranslucentForm();
        private static int m_nTranslucentCommandID = -1;

        private SDMS.Form4CCTV m_frm4CCTV = null;

        public SDMS.Form4CCTV Frm4CCTV
        {
            get { return m_frm4CCTV; }
        }


        public PageBackStageCCTV()
        {
            InitializeComponent();
        }

        public static void ShowTranslucentForm(Form targetForm, int x, int y, int width, int height, int nCommandID)
        {
            if (targetForm == null)
                return;

            //FormSOP.Instance.SetDisableToolBar();

            if (mTranslucentForm == null || mTranslucentForm.IsDisposed)
                mTranslucentForm = new PopupTranslucentForm();

            m_nTranslucentCommandID = nCommandID;
            targetForm.ShowInTaskbar = false;
            if (mTranslucentForm.Visible == true)
            {
                mTranslucentForm.Detach();
            }

            targetForm.StartPosition = FormStartPosition.Manual;
            mTranslucentForm.AddContentForm(targetForm, x, y, mTranslucentForm.Size.Width, mTranslucentForm.Size.Height, FormSOP.Instance.GetPageCCTV());
            mTranslucentForm.Parent = FormSOP.Instance.GetPageCCTV();
            mTranslucentForm.ShowInTaskbar = false;
            mTranslucentForm.Show(FormSOP.Instance.GetPageCCTV());
        }

        public void ShowCCTV()
        {
            if( m_frm4CCTV != null)
            {
                try
                {
                    m_frm4CCTV.Dispose();
                }
                catch(Exception)
                {}
            }

            m_frm4CCTV = new SDMS.Form4CCTV(this, "SOP");
            m_frm4CCTV.SetOwner(FormSOP.Instance);
            m_frm4CCTV.SetDefaultCCTV();

            SDMS.CCTVList cvList = m_frm4CCTV.GetCCTVList(null);
            if (cvList != null)
            {
                ArrayList arrCCTVs = cvList.GetAllCCTV();
                if (arrCCTVs != null)
                {
                    m_frm4CCTV.SetCCTV(arrCCTVs, null);
                    SOPMonitoringSystem.PageBackStageCCTV.ShowTranslucentForm(m_frm4CCTV, 0, 0, Size.Width, Size.Height, ID.ID_SHOW_CCTV);
                }                
            }           
        }

        public void HideCCTV()
        {
            mTranslucentForm.CloseExternal();
            if (m_frm4CCTV != null)
            {
                m_frm4CCTV.Dispose();
                m_frm4CCTV = null;
            }
        }
    }
}
