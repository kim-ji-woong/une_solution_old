using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace HSMS
{
    public partial class PageBackstageHome : Form, IChangedDataManager, ITranslucentFormParent
    {
        private ArrayList m_arrChangedData = new ArrayList();
        public ArrayList GetDataList()
        {
            return m_arrChangedData;
        }

        public PageBackstageHome()
        {
            m_home = this;

            InitializeComponent();

            
            //Create3DView();
            
        }

        private static PageBackstageHome m_home = null;
        public static PageBackstageHome Instance
        {
            get { return m_home; }
        }

        private FormReportPage m_ReportForm = null;
        public FormReportPage FrmReport
        {
            get { return m_ReportForm; }
        }      

        private static FormTranslucentForm mTranslucentForm = new FormTranslucentForm();
        public static FormTranslucentForm TranslucentForm
        {
            get { return mTranslucentForm; }
            set { mTranslucentForm = value; }
        }

        public static void ShowTranslucentForm(Form targetForm, int x, int y, int width, int height, int nCommandID)
        {
            if (targetForm == null)
                return;

            if (mTranslucentForm == null || mTranslucentForm.IsDisposed)
                mTranslucentForm = new FormTranslucentForm();

            targetForm.ShowInTaskbar = false;
            if (mTranslucentForm.Visible == true)
            {
                mTranslucentForm.Detach();
            }

            targetForm.StartPosition = FormStartPosition.Manual;
            mTranslucentForm.AddContentForm(targetForm, x, y, targetForm.Size.Width, targetForm.Size.Height);
            mTranslucentForm.Parent = m_home;
            mTranslucentForm.ShowInTaskbar = false;
            mTranslucentForm.Show(m_home);
        }

        public static DialogResult ShowTranslucentSubForm(Form form)
        {
            if (form == null)
                return DialogResult.Cancel;
            if (mTranslucentForm.Visible == true)
            {
                FormStartPosition pos = form.StartPosition;
                form.StartPosition = FormStartPosition.Manual;
                form.ShowInTaskbar = false;
                DialogResult result = mTranslucentForm.AddSubModalForm(form);
                form.StartPosition = pos;
                return result;
            }
            return DialogResult.Cancel;
        }

        private FormContent m_3DView = null;
	    public FormContent ContentView
	    {
		    get { return m_3DView; }
		    set { m_3DView = value; }
	    }

        public Panel ContentPane
        {
            get { return m_ContentPanel; }
            
        }

        public void OnCommandExcute(int nID)
        {
            FormMain frmMain = FormMain.Instance;

            if (nID > ID.ID_VIEW_BEGIN && nID < ID.ID_VIEW_END)
            {
                OnViewCommandExcute(nID);
                return;
            }

            switch (nID)
            {
                case ID.ID_ADMIN_WORKER:
                    {
                        FormWorker frm = new FormWorker();
                        ShowTranslucentForm(frm, 10, -30, frm.Size.Width, frm.Size.Height, nID);
                    }
                    break;
                case ID.ID_ADMIN_VEHICLE:
                    {
                        FormVehicle frm = new FormVehicle();
                        ShowTranslucentForm(frm, 10, -30, frm.Size.Width, frm.Size.Height, nID);
                    }
                    break;
                case ID.ID_ADMIN_DANGERFACILITY:
                    {
                        FormDangerFacility frm = new FormDangerFacility();
                        ShowTranslucentForm(frm, 10, -30, frm.Size.Width, frm.Size.Height, nID);
                    }
                    break;
                case ID.ID_ADMIN_DANGERZONE:
                    {
                        FormDangerZone frm = new FormDangerZone();
                        ShowTranslucentForm(frm, 10, -30, frm.Size.Width, frm.Size.Height, nID);
                    }
                    break;
                case ID.ID_ADMIN_SAVE:
                    {
                        SaveChangedData();
                    }
                    break;
                case ID.ID_ADMIN_DELETE:
                    {

                    }
                    break;
                case ID.ID_ADMIN_MANAGER:
                    {
                        FormManager frm = new FormManager();
                        ShowTranslucentForm(frm, 10, -30, frm.Size.Width, frm.Size.Height, nID);
                    }
                    break;
                case ID.ID_ADMIN_MESSAGE:
                    {
                        FormMessage frm = new FormMessage();
                        ShowTranslucentForm(frm, 10, -30, frm.Size.Width, frm.Size.Height, nID);
                    }
                    break;
                case ID.ID_ADMIN_ALARMDISTANCE:
                    {
                        FormAlarmDistance frm = new FormAlarmDistance();
                        ShowTranslucentForm(frm, 10, -30, frm.Size.Width, frm.Size.Height, nID);

                    }
                    break;
                case ID.ID_ADMIN_DETECT:
                    {
                        FormDetect frm = new FormDetect();
                        ShowTranslucentForm(frm, 10, -30, frm.Size.Width, frm.Size.Height, nID);

                    }
                    break;
                case ID.ID_ADMIN_LIST:
                    {
                        FormList frm = new FormList();
                        ShowTranslucentForm(frm, 10, -30, frm.Size.Width, frm.Size.Height, nID);

                    }
                    break;
                case ID.ID_ADMIN_OPTION:
                    {
                        FormOption frm = new FormOption();
                        ShowTranslucentForm(frm, 10, -30, frm.Size.Width, frm.Size.Height, nID);
                    }
                    break;
                case ID.ID_ADMIN_HISTORY:
                    {

                    }
                    break;
            }
        }

        private void OnViewCommandExcute(int nID)
        {
            if (m_3DView == null)
                return;

            switch (nID)
            {
                case ID.ID_VIEW_HOME:
                    m_3DView.HomeView();
                    break;
                case ID.ID_VIEW_SAVEHOME:
                    m_3DView.SaveHomeView();
                    break;
                case ID.ID_VIEW_PICK:
                    m_3DView.SetPickMode();
                    break;
                case ID.ID_VIEW_PAN:
                    m_3DView.SetPanMode();
                    break;
                case ID.ID_VIEW_ORBIT:
                    m_3DView.SetOrbitMode();
                    break;
                case ID.ID_VIEW_ZOOMIN:
                    m_3DView.ZoomIn();
                    break;
                case ID.ID_VIEW_ZOOMOUT:
                    m_3DView.ZoomOut();
                    break;
                case ID.ID_VIEW_SCREENSHOT:
                    m_3DView.SaveToImage();
                    break;
                case ID.ID_VIEW_TOPVIEW:
                    m_3DView.TopView();
                    break;
            }
        }

        private void PostChangedData()
        {

            m_arrChangedData.Clear();

        }

        public void SomethingChanged(ChangedData data)
        {
            if (data != null)
                m_arrChangedData.Add(data);

            UnE.GUI.RibbonButton btn = FormMain.Instance.GetButton(ID.ID_ADMIN_SAVE);
            btn.Enabled = m_arrChangedData.Count > 0;
            if (btn.Enabled == true)
                btn.IsChecked = true;
            else
                btn.IsChecked = false;   
        }

        public void RemoveData(ChangedData data)
        {
            m_arrChangedData.Remove(data);

            UnE.GUI.RibbonButton btn = FormMain.Instance.GetButton(ID.ID_ADMIN_SAVE);
            btn.Enabled = m_arrChangedData.Count > 0;
            if (btn.Enabled == true)
                btn.IsChecked = true;
            else
                btn.IsChecked = false;

        }

        public void SaveChangedData()
        {           
            DBConn conn = new DBConn("HSMS");
            foreach (ChangedData data in m_arrChangedData)
            {
                data.Update(conn);
            }

            PostChangedData();

            UnE.GUI.RibbonButton btn = FormMain.Instance.GetButton(ID.ID_ADMIN_SAVE); 
            btn.Enabled = false;

            btn.IsChecked = false;     
        }

        private void timer1_Tick(object sender, EventArgs e)
        {

        }

        public void OnCloseTranslucentForm()
        {
            FormMain.Instance.ResetCheckRibbonButtonAdmin();

            FormFrame.Instance.BringToFront();
            FormFrame.Instance.Activate();
            FormMain.Instance.Focus();
        }     

        public void CreateReportForm()
        {
            m_ReportForm = new FormReportPage(FormMain.Instance.DataReport);

            m_ReportForm.TopLevel = false;
            m_ReportForm.Parent = m_ContentPanel;
            m_ReportForm.Dock = DockStyle.Fill;
            //m_ReportForm.Visible = true;

            m_ContentPanel.Controls.Add(m_ReportForm);
        } 

        private void PageBackstageHome_Load(object sender, EventArgs e)
        {

        }

        private void PageBackstageHome_Resize(object sender, EventArgs e)
        {
            int left, top, right, bottom;
            Rectangle rect = this.ClientRectangle;
            left = rect.Left;
            top = rect.Top;
            right = rect.Right;
            bottom = rect.Bottom;

            m_ContentPanel.SetBounds(left, top, right - left, bottom - top);
            if( this.m_3DView != null)
            {
                m_3DView.Update3DView();
            }
        }


    }

}
