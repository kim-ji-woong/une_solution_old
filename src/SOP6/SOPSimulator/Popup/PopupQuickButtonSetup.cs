using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SOPMonitoringSystem.Popup
{
    public partial class PopupQuickButtonSetup : Form
    {
        private Dictionary<int, PageBackstageSOP.QuickSOPButton> m_dicOriginQuickSOPs = null;
        private Dictionary<int, PageBackstageSOP.QuickSOPButton> m_dicCloneQuickSOPs = new Dictionary<int, PageBackstageSOP.QuickSOPButton>();
        private Dictionary<int, TextBox> m_dicSOPTextBox = new Dictionary<int, TextBox>();
        private Dictionary<int, TextBox> m_dicSOPEmergencyTextBox = new Dictionary<int, TextBox>();

        private PopupTranslucentForm mTranslucentForm = new PopupTranslucentForm();


        private int m_nSiteID = 1;


        public PopupQuickButtonSetup()
        {
            InitializeComponent();

            Init();
        }


        private void Init()
        {
            InitEvent();
            InitControl();
            InitQuickSOPInfo();
        }

        private void InitEvent()
        {
            // Load Event
            Load += PopupQuickButtonSetup_Load;
            FormClosing += PopupQuickButtonSetup_FormClosing;

            // Select Button Event
            btnFire.Click += btnSelect_Click;
            btnPollution.Click += btnSelect_Click;
            btnEarthquake.Click += btnSelect_Click;
            btnTyphoon.Click += btnSelect_Click;
            btnSnowFall.Click += btnSelect_Click;
            btnSecurity.Click += btnSelect_Click;
            btnSubMergence.Click += btnSelect_Click;
            btnTerror.Click += btnSelect_Click;

            btnFireEmergency.Click += btnSelectEmergency_Click;
            btnPollutionEmergency.Click += btnSelectEmergency_Click;
            btnEarthquakeEmergency.Click += btnSelectEmergency_Click;
            btnTyphoonEmergency.Click += btnSelectEmergency_Click;
            btnSnowFallEmergency.Click += btnSelectEmergency_Click;
            btnSecurityEmergency.Click += btnSelectEmergency_Click;
            btnSubMergenceEmergency.Click += btnSelectEmergency_Click;
            btnTerrorEmergency.Click += btnSelectEmergency_Click;

            // Init Button Event
            btnInitFire.Click += btnInit_Click;
            btnInitPollution.Click += btnInit_Click;
            btnInitEarthquake.Click += btnInit_Click;
            btnInitTyphoon.Click += btnInit_Click;
            btnInitSnowFall.Click += btnInit_Click;
            btnInitSecurity.Click += btnInit_Click;
            btnInitSubMergence.Click += btnInit_Click;
            btnInitTerror.Click += btnInit_Click;

            btnInitFireEmergency.Click += btnInitEmergency_Click;
            btnInitPollutionEmergency.Click += btnInitEmergency_Click;
            btnInitEarthquakeEmergency.Click += btnInitEmergency_Click;
            btnInitTyphoonEmergency.Click += btnInitEmergency_Click;
            btnInitSnowFallEmergency.Click += btnInitEmergency_Click;
            btnInitSecurityEmergency.Click += btnInitEmergency_Click;
            btnInitSubMergenceEmergency.Click += btnInitEmergency_Click;
            btnInitTerrorEmergency.Click += btnInitEmergency_Click;

            // Save Button Event
            btnSave.Click += btnSave_Click;
            btnClose.Click += btnClose_Click;
        }

        private void InitControl()
        {
            btnFire.Tag = ID.ID_SOP_FIRE;
            btnPollution.Tag = ID.ID_SOP_POLLUTION;
            btnEarthquake.Tag = ID.ID_SOP_EARTHQUAKE;
            btnTyphoon.Tag = ID.ID_SOP_TYPHOON;
            btnSnowFall.Tag = ID.ID_SOP_HEAVY_SNOW;
            btnSecurity.Tag = ID.ID_SOP_SECURITY;
            btnSubMergence.Tag = ID.ID_SOP_SUBMERGENCE;
            btnTerror.Tag = ID.ID_SOP_TERROR;

            btnFireEmergency.Tag = ID.ID_SOP_FIRE;
            btnPollutionEmergency.Tag = ID.ID_SOP_POLLUTION;
            btnEarthquakeEmergency.Tag = ID.ID_SOP_EARTHQUAKE;
            btnTyphoonEmergency.Tag = ID.ID_SOP_TYPHOON;
            btnSnowFallEmergency.Tag = ID.ID_SOP_HEAVY_SNOW;
            btnSecurityEmergency.Tag = ID.ID_SOP_SECURITY;
            btnSubMergenceEmergency.Tag = ID.ID_SOP_SUBMERGENCE;
            btnTerrorEmergency.Tag = ID.ID_SOP_TERROR;

            btnInitFire.Tag = ID.ID_SOP_FIRE;
            btnInitPollution.Tag = ID.ID_SOP_POLLUTION;
            btnInitEarthquake.Tag = ID.ID_SOP_EARTHQUAKE;
            btnInitTyphoon.Tag = ID.ID_SOP_TYPHOON;
            btnInitSnowFall.Tag = ID.ID_SOP_HEAVY_SNOW;
            btnInitSecurity.Tag = ID.ID_SOP_SECURITY;
            btnInitSubMergence.Tag = ID.ID_SOP_SUBMERGENCE;
            btnInitTerror.Tag = ID.ID_SOP_TERROR;

            btnInitFireEmergency.Tag = ID.ID_SOP_FIRE;
            btnInitPollutionEmergency.Tag = ID.ID_SOP_POLLUTION;
            btnInitEarthquakeEmergency.Tag = ID.ID_SOP_EARTHQUAKE;
            btnInitTyphoonEmergency.Tag = ID.ID_SOP_TYPHOON;
            btnInitSnowFallEmergency.Tag = ID.ID_SOP_HEAVY_SNOW;
            btnInitSecurityEmergency.Tag = ID.ID_SOP_SECURITY;
            btnInitSubMergenceEmergency.Tag = ID.ID_SOP_SUBMERGENCE;
            btnInitTerrorEmergency.Tag = ID.ID_SOP_TERROR;

            m_dicSOPTextBox[ID.ID_SOP_FIRE] = txtFire;
            m_dicSOPTextBox[ID.ID_SOP_POLLUTION] = txtPollution;
            m_dicSOPTextBox[ID.ID_SOP_EARTHQUAKE] = txtEarthquake;
            m_dicSOPTextBox[ID.ID_SOP_TYPHOON] = txtTyphoon;
            m_dicSOPTextBox[ID.ID_SOP_HEAVY_SNOW] = txtSnowFall;
            m_dicSOPTextBox[ID.ID_SOP_SECURITY] = txtSecurity;
            m_dicSOPTextBox[ID.ID_SOP_SUBMERGENCE] = txtSubMergence;
            m_dicSOPTextBox[ID.ID_SOP_TERROR] = txtTerror;

            m_dicSOPEmergencyTextBox[ID.ID_SOP_FIRE] = txtFireEmergency;
            m_dicSOPEmergencyTextBox[ID.ID_SOP_POLLUTION] = txtPollutionEmergency;
            m_dicSOPEmergencyTextBox[ID.ID_SOP_EARTHQUAKE] = txtEarthquakeEmergency;
            m_dicSOPEmergencyTextBox[ID.ID_SOP_TYPHOON] = txtTyphoonEmergency;
            m_dicSOPEmergencyTextBox[ID.ID_SOP_HEAVY_SNOW] = txtSnowFallEmergency;
            m_dicSOPEmergencyTextBox[ID.ID_SOP_SECURITY] = txtSecurityEmergency;
            m_dicSOPEmergencyTextBox[ID.ID_SOP_SUBMERGENCE] = txtSubMergenceEmergency;
            m_dicSOPEmergencyTextBox[ID.ID_SOP_TERROR] = txtTerrorEmergency;
        }

        private void InitQuickSOPInfo()
        {
            m_dicOriginQuickSOPs = FormSOP.Instance.GetPageHome().QuickSOPs;

            m_dicCloneQuickSOPs.Add(ID.ID_SOP_FIRE, m_dicOriginQuickSOPs[ID.ID_SOP_FIRE].Clone());
            m_dicCloneQuickSOPs.Add(ID.ID_SOP_POLLUTION, m_dicOriginQuickSOPs[ID.ID_SOP_POLLUTION].Clone());
            m_dicCloneQuickSOPs.Add(ID.ID_SOP_EARTHQUAKE, m_dicOriginQuickSOPs[ID.ID_SOP_EARTHQUAKE].Clone());
            m_dicCloneQuickSOPs.Add(ID.ID_SOP_TYPHOON, m_dicOriginQuickSOPs[ID.ID_SOP_TYPHOON].Clone());
            m_dicCloneQuickSOPs.Add(ID.ID_SOP_HEAVY_SNOW, m_dicOriginQuickSOPs[ID.ID_SOP_HEAVY_SNOW].Clone());
            m_dicCloneQuickSOPs.Add(ID.ID_SOP_GENERAL_DISASTER, m_dicOriginQuickSOPs[ID.ID_SOP_GENERAL_DISASTER].Clone());
            m_dicCloneQuickSOPs.Add(ID.ID_SOP_SUBMERGENCE, m_dicOriginQuickSOPs[ID.ID_SOP_SUBMERGENCE].Clone());
            m_dicCloneQuickSOPs.Add(ID.ID_SOP_TERROR, m_dicOriginQuickSOPs[ID.ID_SOP_TERROR].Clone());
            m_dicCloneQuickSOPs.Add(ID.ID_SOP_SECURITY, m_dicOriginQuickSOPs[ID.ID_SOP_SECURITY].Clone());
        }


        private void SaveData()
        {
            SaveData(ID.ID_SOP_FIRE);
            SaveData(ID.ID_SOP_POLLUTION);
            SaveData(ID.ID_SOP_EARTHQUAKE);
            SaveData(ID.ID_SOP_TYPHOON);
            SaveData(ID.ID_SOP_HEAVY_SNOW);
            SaveData(ID.ID_SOP_GENERAL_DISASTER);
            SaveData(ID.ID_SOP_SUBMERGENCE);
            SaveData(ID.ID_SOP_TERROR);
            SaveData(ID.ID_SOP_SECURITY);
        }

        private void SaveData(int nID)
        {
            WebDBManager dbMgr = FormSOP.Instance.DBManager;

            string strIFNull = dbMgr.DatabaseType == WebDBManager.DBType.sqlserver ? "ISNULL" : "IFNULL";

            string strSelect = "SELECT ID FROM OptionQuickButton WHERE ButtonID = {0} AND SiteID = {1} AND IsNormal = {2}";
            string strInsert = "INSERT INTO OptionQuickButton ( ID, ButtonID, IsNormal, DisasterName, ActionStepName, SiteID ) SELECT " + strIFNull + "(MAX(ID), 0) + 1 , {0}, {1}, '{2}', '{3}', {4} FROM OptionQuickButton";
            string strUpdate = "UPDATE OptionQuickButton SET DisasterName = '{0}', ActionStepName = '{1}' WHERE ID = {2}";

            int nOptionID = -1;

            string strSQL = String.Format(strSelect, nID, m_nSiteID, 1);

            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult != null)
            {
                if (arrResult.Count > 0)
                {
                    nOptionID = WebDBManager.GetIntField(arrResult[0].ToString(), -1);
                }
            }

            // 신규
            if (nOptionID == -1)
            {
                strSQL = String.Format(strInsert,
                    nID,
                    1,
                    m_dicCloneQuickSOPs[nID].SOPNormal,
                    m_dicCloneQuickSOPs[nID].SOPActionStepNameNormal,
                    m_nSiteID);
            }
            // 수정
            else
            {
                strSQL = String.Format(strUpdate,
                    m_dicCloneQuickSOPs[nID].SOPNormal,
                    m_dicCloneQuickSOPs[nID].SOPActionStepNameNormal,
                    nOptionID);
            }

            if (dbMgr.GetResultData(strSQL) == null)
                return;


            nOptionID = -1;

            strSQL = String.Format(strSelect, nID, m_nSiteID, 0);

            arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult != null)
            {
                if (arrResult.Count > 0)
                {
                    nOptionID = WebDBManager.GetIntField(arrResult[0].ToString(), -1);
                }
            }

            // 신규
            if (nOptionID == -1)
            {
                strSQL = String.Format(strInsert,
                    nID,
                    0,
                    m_dicCloneQuickSOPs[nID].SOPEmergency,
                    m_dicCloneQuickSOPs[nID].SOPActionStepNameEmergency,
                    m_nSiteID);
            }
            // 수정
            else
            {
                strSQL = String.Format(strUpdate,
                    m_dicCloneQuickSOPs[nID].SOPEmergency,
                    m_dicCloneQuickSOPs[nID].SOPActionStepNameEmergency,
                    nOptionID);
            }

            if (dbMgr.GetResultData(strSQL) == null)
                return;


            m_dicOriginQuickSOPs[nID].SOPNormal = m_dicCloneQuickSOPs[nID].SOPNormal;
            m_dicOriginQuickSOPs[nID].SOPEmergency = m_dicCloneQuickSOPs[nID].SOPEmergency;
            m_dicOriginQuickSOPs[nID].SOPActionStepNameNormal = m_dicCloneQuickSOPs[nID].SOPActionStepNameNormal;
            m_dicOriginQuickSOPs[nID].SOPActionStepNameEmergency = m_dicCloneQuickSOPs[nID].SOPActionStepNameEmergency;
        }


        private void RevertSOPInfo(int nID, bool isNormal)
        {
            if (isNormal)
            {
                m_dicCloneQuickSOPs[nID].SOPNormal = string.Empty;
                m_dicCloneQuickSOPs[nID].SOPActionStepNameNormal = string.Empty;
                m_dicSOPTextBox[nID].Text = string.Empty;
            }
            else
            {
                m_dicCloneQuickSOPs[nID].SOPEmergency = string.Empty;
                m_dicCloneQuickSOPs[nID].SOPActionStepNameEmergency = string.Empty;
                m_dicSOPEmergencyTextBox[nID].Text = string.Empty;
            }
        }

        private void SelectSOPInfo(int nID, bool isNormal)
        {
            PopupSelectSOP form = new PopupSelectSOP();
            form.IsNormal = isNormal;
            form.DisasterTypeID = nID;
            form.QuickSOP = m_dicCloneQuickSOPs[nID];
            form.SelectButtonClickEvent += (s, e) => { ChangeSOPInfo(nID); };

            ShowTranslucentForm(form, 400, -30, form.Width, form.Size.Height, ID.ID_SHOW_QUICK_MENU);
        }

        private void ChangeSOPInfo(int nID)
        {
            m_dicSOPTextBox[nID].Text = m_dicCloneQuickSOPs[nID].SOPNormalPath;
            m_dicSOPEmergencyTextBox[nID].Text = m_dicCloneQuickSOPs[nID].SOPEmergencyPath;
        }

        public void ShowTranslucentForm(Form targetForm, int x, int y, int width, int height, int nCommandID)
        {
            if (targetForm == null)
                return;

            if (mTranslucentForm == null || mTranslucentForm.IsDisposed)
                mTranslucentForm = new PopupTranslucentForm();

            targetForm.ShowInTaskbar = false;
            if (mTranslucentForm.Visible == true)
            {
                mTranslucentForm.Detach();
            }

            targetForm.StartPosition = FormStartPosition.Manual;
            mTranslucentForm.AddContentForm(targetForm, x, y, targetForm.Size.Width, targetForm.Size.Height, this);
            mTranslucentForm.Parent = this;
            mTranslucentForm.ShowInTaskbar = false;
            mTranslucentForm.Show(this);
        }

        public void CloseTranslucentForm()
        {
            if (mTranslucentForm == null || mTranslucentForm.IsDisposed)
                return;

            mTranslucentForm.CloseExternal();
        }


        private void PopupQuickButtonSetup_Load(object sender, EventArgs e)
        {
            m_nSiteID = UnE.SOP.ProxySOP.Instance.SiteID;

            txtFire.Text = m_dicCloneQuickSOPs[ID.ID_SOP_FIRE].SOPNormalPath;
            txtPollution.Text = m_dicCloneQuickSOPs[ID.ID_SOP_POLLUTION].SOPNormalPath;
            txtEarthquake.Text = m_dicCloneQuickSOPs[ID.ID_SOP_EARTHQUAKE].SOPNormalPath;
            txtTyphoon.Text = m_dicCloneQuickSOPs[ID.ID_SOP_TYPHOON].SOPNormalPath;
            txtSnowFall.Text = m_dicCloneQuickSOPs[ID.ID_SOP_HEAVY_SNOW].SOPNormalPath;
            txtSecurity.Text = m_dicCloneQuickSOPs[ID.ID_SOP_SECURITY].SOPNormalPath;
            txtSubMergence.Text = m_dicCloneQuickSOPs[ID.ID_SOP_SUBMERGENCE].SOPNormalPath;
            txtTerror.Text = m_dicCloneQuickSOPs[ID.ID_SOP_TERROR].SOPNormalPath;

            txtFireEmergency.Text = m_dicCloneQuickSOPs[ID.ID_SOP_FIRE].SOPEmergencyPath;
            txtPollutionEmergency.Text = m_dicCloneQuickSOPs[ID.ID_SOP_POLLUTION].SOPEmergencyPath;
            txtEarthquakeEmergency.Text = m_dicCloneQuickSOPs[ID.ID_SOP_EARTHQUAKE].SOPEmergencyPath;
            txtTyphoonEmergency.Text = m_dicCloneQuickSOPs[ID.ID_SOP_TYPHOON].SOPEmergencyPath;
            txtSnowFallEmergency.Text = m_dicCloneQuickSOPs[ID.ID_SOP_HEAVY_SNOW].SOPEmergencyPath;
            txtSecurityEmergency.Text = m_dicCloneQuickSOPs[ID.ID_SOP_SECURITY].SOPEmergencyPath;
            txtSubMergenceEmergency.Text = m_dicCloneQuickSOPs[ID.ID_SOP_SUBMERGENCE].SOPEmergencyPath;
            txtTerrorEmergency.Text = m_dicCloneQuickSOPs[ID.ID_SOP_TERROR].SOPEmergencyPath;
        }

        private void PopupQuickButtonSetup_FormClosing(object sender, FormClosingEventArgs e)
        {
            CloseTranslucentForm();
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            Button btn = (sender as Button);

            SelectSOPInfo(Convert.ToInt32(btn.Tag), true);
        }

        private void btnSelectEmergency_Click(object sender, EventArgs e)
        {
            Button btn = (sender as Button);

            SelectSOPInfo(Convert.ToInt32(btn.Tag), false);
        }

        private void btnInit_Click(object sender, EventArgs e)
        {
            Button btn = (sender as Button);

            RevertSOPInfo(Convert.ToInt32(btn.Tag), true);
        }

        private void btnInitEmergency_Click(object sender, EventArgs e)
        {
            Button btn = (sender as Button);

            RevertSOPInfo(Convert.ToInt32(btn.Tag), false);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveData();

            (Owner as PopupTranslucentForm).CloseExternal();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            (Owner as PopupTranslucentForm).CloseExternal();
        }

        private void btnSave_Click_1(object sender, EventArgs e)
        {

        }
    }
}
