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

        // Key : SOP 이름
        private static Dictionary<string, Bitmap> m_dicSOPBitmaps = new Dictionary<string, Bitmap>();

        private PopupTranslucentForm mTranslucentForm = new PopupTranslucentForm();


        private int m_nSiteID = 1;

        // Form Move 를 위한 Panel Top 클릭 체크
        private bool m_bLeftMouseDown = false;
        // Form Move 를 위한 Panel Top 클릭 점
        private Point m_ptMove;

        private Font m_fontSaveCloseButton = new System.Drawing.Font("나눔바른고딕", 15F, System.Drawing.FontStyle.Bold);
        private Font m_fontButton = new System.Drawing.Font("나눔바른고딕", 12F, System.Drawing.FontStyle.Regular);

        private List<int> m_sopIDs = null;

        public PopupQuickButtonSetup()
        {
            InitializeComponent();
            SetRibbonButtonFont();

            Init();
        }

        public static void SetSOPBitmap(string strSOPName, int nSOPID, Bitmap bmp)
        {
            m_dicSOPBitmaps[strSOPName + "_" + nSOPID.ToString()] = bmp;
        }


        private void Init()
        {
            if (m_dicSOPBitmaps.Count == 0)
            {
                InitEvent();
                InitControl();
                InitQuickSOPInfo();
            }
            else
            {
                panelFire.Visible = panelPollution.Visible = panelEarthquake.Visible = panelTyphoon.Visible = panelHeavySnow.Visible = panelSecurity.Visible = panelSubmergence.Visible = panelTerror.Visible = false;

                List<int> sopIDs;
                List<Panel> panels = MakePanels(out sopIDs);
                InitEvent(panels);
                InitQuickSOPInfo(sopIDs);
                m_sopIDs = sopIDs;
            }
        }

        private List<Panel> MakePanels(out List<int> sopIDs)
        {
            int space = 127;
            Panel panelPrev = null;

            List<Panel> panels = new List<Panel>();
            sopIDs = new List<int>();

            foreach (KeyValuePair<string, Bitmap> pair in m_dicSOPBitmaps)
            {
                int nIndex = pair.Key.LastIndexOf('_');

                if (nIndex < 0)
                    continue;

                string strSOPName = pair.Key.Substring(0, nIndex);
                string strSOPID = pair.Key.Substring(nIndex + 1);

                int nSOPID;

                if (int.TryParse(strSOPID, out nSOPID) == false)
                    continue;

                if (sopIDs.Contains(nSOPID) == false)
                    sopIDs.Add(nSOPID);
                
                Panel panelSOP = new Panel();
                panelSOP.Size = panelFire.Size;

                int y = panelPrev == null ? panelFire.Location.Y : panelPrev.Location.Y + space;

                Label labelNormal = MakeLabel(strSOPName, true);
                Label labelEmergency = MakeLabel(strSOPName, false);

                UnE.GUI.RibbonButton btnNormalSelect = MakeButton(nSOPID, true, true);
                UnE.GUI.RibbonButton btnNormalDelete = MakeButton(nSOPID, true, false);
                UnE.GUI.RibbonButton btnEmergencySelect = MakeButton(nSOPID, false, true);
                UnE.GUI.RibbonButton btnEmergencyDelete = MakeButton(nSOPID, false, false);
                PictureBox pb = MakePictureBox(pair.Value);
                TextBox textBoxNormal = MakeTextBox(nSOPID, true);
                TextBox textBoxEmergency = MakeTextBox(nSOPID, false);

                panelSOP.BackColor = panelFire.BackColor;
                panelSOP.Controls.Add(btnEmergencyDelete);
                panelSOP.Controls.Add(btnEmergencySelect);
                panelSOP.Controls.Add(btnNormalDelete);
                panelSOP.Controls.Add(btnNormalSelect);
                panelSOP.Controls.Add(labelEmergency);
                panelSOP.Controls.Add(labelNormal);
                panelSOP.Controls.Add(pb);
                panelSOP.Controls.Add(textBoxNormal);
                panelSOP.Controls.Add(textBoxEmergency);
                panelSOP.Location = new System.Drawing.Point(panelFire.Location.X, y);

                this.paneBody.Controls.Add(panelSOP);
                panelSOP.Show();

                panelPrev = panelSOP;
                panels.Add(panelSOP);
            }

            return panels;
        }

        private TextBox MakeTextBox(int nSOPID, bool normal)
        {
            TextBox textBox = new TextBox();

            textBox.Anchor = txtFire.Anchor;
            textBox.BorderStyle = txtFire.BorderStyle;
            textBox.Font = txtFire.Font;

            if (normal)
                textBox.Location = txtFire.Location;
            else
                textBox.Location = txtFireEmergency.Location;

            textBox.ReadOnly = txtFire.ReadOnly;
            textBox.Size = txtFire.Size;

            if (normal)
                m_dicSOPTextBox[nSOPID] = textBox;
            else
                m_dicSOPEmergencyTextBox[nSOPID] = textBox;

            return textBox;
        }

        private PictureBox MakePictureBox(Bitmap bmp)
        {
            PictureBox pb = new PictureBox();

            pb.BackColor = pictureBoxFire.BackColor;
            pb.BackgroundImage = bmp;
            pb.BackgroundImageLayout = pictureBoxFire.BackgroundImageLayout;
            pb.Location = pictureBoxFire.Location;
            pb.Size = pictureBoxFire.Size;

            return pb;
        }

        private UnE.GUI.RibbonButton MakeButton(int nSOPID, bool normal, bool select)
        {
            UnE.GUI.RibbonButton btn = new UnE.GUI.RibbonButton();

            btn.BackColor = System.Drawing.Color.Transparent;
            btn.CheckButton = false;
            btn.CheckedBkgndImage = null;
            btn.CheckedImage = null;
            btn.CheckedMouseOver = null;
            btn.ClickedBackgroundImage = null;
            btn.CustomImageRect = btnFire.CustomImageRect;
            btn.DisabledBkgndImage = null;
            btn.DisabledImage = null;
            btn.FlatStyle = btnFire.FlatStyle;
            btn.ForeColor = btnFire.ForeColor;
            btn.ForeColorChecked = btnFire.ForeColorChecked;
            btn.ID = -1;
            btn.IsChecked = btnFire.IsChecked;
            btn.MouseOverBkgndImage = null;
            btn.Owner = null;
            btn.UseCustomImageRect = btnFire.UseCustomImageRect;
            btn.UseTextLocation = btnFire.UseTextLocation;
            btn.UseVisualStyleBackColor = false;
            btn.Tag = nSOPID;

            if (select)
            {
                btn.ClickedImage = btnFire.ClickedImage;
                btn.ForeColor = btnFire.ForeColor;
                btn.ForeColorCheckedMouseOver = btnFire.ForeColorCheckedMouseOver;
                btn.ForeColorDisabled = btnFire.ForeColorDisabled;
                btn.ForeColorMouseOver = btnFire.ForeColorMouseOver;
                btn.ForeColorsByTypeUse = btnFire.ForeColorsByTypeUse;
                btn.InitButtonWidth = btnFire.InitButtonWidth;

                if (normal)
                {
                    btn.Location = btnFire.Location;
                    btn.Click += btnSelect_Click;
                }
                else
                {
                    btn.Location = btnFireEmergency.Location;
                    btn.Click += btnSelectEmergency_Click;
                }

                btn.MouseOverImage = btnFire.MouseOverImage;
                btn.NormalImage = btnFire.NormalImage;
                btn.Size = btnFire.Size;
                btn.Text = btnFire.Text;
                btn.TextLocation = btnFire.TextLocation;
                btn.TextPos = btnFire.TextPos;
                btn.ToolTipText = btnFire.ToolTipText;
            }
            else
            {
                btn.ClickedImage = btnInitFire.ClickedImage;
                btn.ForeColor = btnInitFire.ForeColor;
                btn.ForeColorCheckedMouseOver = btnInitFire.ForeColorCheckedMouseOver;
                btn.ForeColorDisabled = btnInitFire.ForeColorDisabled;
                btn.ForeColorMouseOver = btnInitFire.ForeColorMouseOver;
                btn.ForeColorsByTypeUse = btnInitFire.ForeColorsByTypeUse;
                btn.InitButtonWidth = btnInitFire.InitButtonWidth;

                if (normal)
                {
                    btn.Location = btnInitFire.Location;
                    btn.Click += btnInit_Click;
                }
                else
                {
                    btn.Location = btnInitFireEmergency.Location;
                    btn.Click += btnInitEmergency_Click;
                }

                btn.MouseOverImage = btnInitFire.MouseOverImage;
                btn.NormalImage = btnInitFire.NormalImage;
                btn.Size = btnInitFire.Size;
                btn.Text = btnInitFire.Text;
                btn.TextLocation = btnInitFire.TextLocation;
                btn.TextPos = btnInitFire.TextPos;
                btn.ToolTipText = btnInitFire.ToolTipText;
            }

            return btn;
        }

        private Label MakeLabel(string strSOPName, bool normal)
        {
            Label label = new Label();

            label.AutoSize = labelFireNormal.AutoSize;
            label.Font = labelFireNormal.Font;

            if (normal)
            {
                label.Location = labelFireNormal.Location;
                label.Text = strSOPName + " - 평일";
            }
            else
            {
                label.Location = labelFireEmergency.Location;
                label.Text = strSOPName + " - 야간 및 휴일";
            }

            return label;
        }

        private void InitEvent(List<Panel> panels)
        {
            // Load Event
            Load += PopupQuickButtonSetup_Load;
            FormClosing += PopupQuickButtonSetup_FormClosing;

            // Save Button Event
            btnSave.Click += btnSave_Click;
            btnClose.Click += btnClose_Click;
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

        private void InitQuickSOPInfo(List<int> sopIDs) 
        {
            m_dicOriginQuickSOPs = FormSOP.Instance.GetPageHome().QuickSOPs;

            foreach (int nSOPID in sopIDs)
            {
                m_dicCloneQuickSOPs.Add(nSOPID, m_dicOriginQuickSOPs[nSOPID].Clone());
            }
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

        private void SetRibbonButtonFont()
        {
            btnFire.Font = m_fontButton;
            btnInitFire.Font = m_fontButton;
            btnFireEmergency.Font = m_fontButton;
            btnInitFireEmergency.Font = m_fontButton;
            btnPollution.Font = m_fontButton;
            btnInitPollution.Font = m_fontButton;
            btnPollutionEmergency.Font = m_fontButton;
            btnInitPollutionEmergency.Font = m_fontButton;
            btnEarthquake.Font = m_fontButton;

            btnInitEarthquake.Font = m_fontButton;
            btnEarthquakeEmergency.Font = m_fontButton;
            btnInitEarthquakeEmergency.Font = m_fontButton;
            btnTyphoon.Font = m_fontButton;
            btnInitTyphoon.Font = m_fontButton;
            btnTyphoonEmergency.Font = m_fontButton;
            btnInitTyphoonEmergency.Font = m_fontButton;
            btnSnowFall.Font = m_fontButton;
            btnInitSnowFall.Font = m_fontButton;
            btnSnowFallEmergency.Font = m_fontButton;
            btnInitSnowFallEmergency.Font = m_fontButton;

            btnSecurity.Font = m_fontButton;
            btnInitSecurity.Font = m_fontButton;
            btnSecurityEmergency.Font = m_fontButton;
            btnInitSecurityEmergency.Font = m_fontButton;
            btnSubMergence.Font = m_fontButton;
            btnInitSubMergence.Font = m_fontButton;
            btnSubMergenceEmergency.Font = m_fontButton;
            btnInitSubMergenceEmergency.Font = m_fontButton;
            btnTerror.Font = m_fontButton;
            btnInitTerror.Font = m_fontButton;
            btnTerrorEmergency.Font = m_fontButton;
            btnInitTerrorEmergency.Font = m_fontButton;

            btnSave.Font = m_fontSaveCloseButton;
            btnClose.Font = m_fontSaveCloseButton;

        }

        private void SaveData()
        {
            if (m_sopIDs == null)
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
            else
            {
                foreach (int nSOPID in m_sopIDs)
                {
                    SaveData(nSOPID);
                }
            }
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

            //ShowTranslucentForm(form, 400, -30, form.Width, form.Size.Height, ID.ID_SHOW_QUICK_MENU);
            form.ShowDialog(this);
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

            if (m_sopIDs != null)
            {
                TextBox textBoxNormal, textBoxEmergency;

                foreach (int nSOPID in m_sopIDs)
                {
                    if (m_dicSOPTextBox.TryGetValue(nSOPID, out textBoxNormal) &&
                        m_dicSOPEmergencyTextBox.TryGetValue(nSOPID, out textBoxEmergency))
                    {
                        textBoxNormal.Text = m_dicCloneQuickSOPs[nSOPID].SOPNormalPath;
                        textBoxEmergency.Text = m_dicCloneQuickSOPs[nSOPID].SOPEmergencyPath;
                    }
                }
            }
            else
            {
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

            //(Owner as PopupTranslucentForm).CloseExternal();
            this.Close();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            //(Owner as PopupTranslucentForm).CloseExternal();
            this.Close();
        }

        private void btnSave_Click_1(object sender, EventArgs e)
        {

        }

        private void plTitle_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                m_bLeftMouseDown = true;
                m_ptMove = plTitle.PointToScreen(new Point(e.X, e.Y));
            }
        }

        private void plTitle_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                m_bLeftMouseDown = false;
        }

        private void plTitle_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (m_bLeftMouseDown == true)
                {
                    Point ptCur = this.Location;

                    Point pt = this.PointToScreen(new Point(e.X, e.Y));
                    int dx = pt.X - m_ptMove.X;
                    int dy = pt.Y - m_ptMove.Y;
                    if (!(dx == 0 && dy == 0))
                    {

                        this.Location = new Point(ptCur.X + dx, ptCur.Y + dy);
                        m_ptMove.X += dx;
                        m_ptMove.Y += dy;
                    }
                }
            }
        }

        private void lbTitle_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                m_bLeftMouseDown = true;
                m_ptMove = plTitle.PointToScreen(new Point(e.X, e.Y));
            }
        }

        private void lbTitle_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                m_bLeftMouseDown = false;
        }

        private void lbTitle_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (m_bLeftMouseDown == true)
                {
                    Point ptCur = this.Location;

                    Point pt = this.PointToScreen(new Point(e.X, e.Y));
                    int dx = pt.X - m_ptMove.X;
                    int dy = pt.Y - m_ptMove.Y;
                    if (!(dx == 0 && dy == 0))
                    {

                        this.Location = new Point(ptCur.X + dx, ptCur.Y + dy);
                        m_ptMove.X += dx;
                        m_ptMove.Y += dy;
                    }
                }
            }
        }

        private void pictureBox6_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                m_bLeftMouseDown = true;
                m_ptMove = plTitle.PointToScreen(new Point(e.X, e.Y));
            }
        }

        private void pictureBox6_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                m_bLeftMouseDown = false;
        }

        private void pictureBox6_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (m_bLeftMouseDown == true)
                {
                    Point ptCur = this.Location;

                    Point pt = this.PointToScreen(new Point(e.X, e.Y));
                    int dx = pt.X - m_ptMove.X;
                    int dy = pt.Y - m_ptMove.Y;
                    if (!(dx == 0 && dy == 0))
                    {

                        this.Location = new Point(ptCur.X + dx, ptCur.Y + dy);
                        m_ptMove.X += dx;
                        m_ptMove.Y += dy;
                    }
                }
            }
        }

        private void btnCancle_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
