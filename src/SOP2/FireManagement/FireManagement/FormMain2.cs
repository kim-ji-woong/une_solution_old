using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using UnE.GUI;
using FireManagement.Docking;

namespace FireManagement
{
    // 일반모드, 편집모드, 설비이력, 설비점검
    public enum Mode { GENERAL = 0, EDIT, EQUIP_HISTORY, CHECK_EQUIP };

    public partial class FormMain2 : Form, ITextPictureBoxOwner, IRibbonButtonOwner
    {
        //private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;

        private RibbonButton m_btnBeforeSelected = null;
        private RibbonButton m_btnCurrentSelected = null;

        // Button별 ID
        private Dictionary<Button, int> m_dicButtonIDs = new Dictionary<Button, int>();
        private Dictionary<int, Button> m_dicIDButtons = new Dictionary<int, Button>();
        private Dictionary<Button, bool> m_dicButtonChecked = new Dictionary<Button, bool>();

        private Dictionary<FireEquipment.EquipmentType, DXFViewer.Layer> m_dicEquipmentLayer = new Dictionary<FireEquipment.EquipmentType, DXFViewer.Layer>();
        private IOManager m_ioMgr = null;
        private DataFileManager m_fileMgr = null;
        private DBUtility.WebDBManager m_dbMgr = null;

        private int m_nSOPGenUserID = 1;

        private Ubists.RFIDReader m_rfidReader = new Ubists.RFIDReader();


        private PageBackstageUpdate m_pageUpdate;
        private PageBackstageSave2 m_pageSave;

        public PageBackstageSave2 PageSave
        {
            get { return m_pageSave; }
            set { m_pageSave = value; }
        }
        private PageBackstageClose m_pageExit;

        private FormCheckEquip3 m_frmCheckEquip = null;
        private FormEquipHistory m_frmEquipHistory = null;
        private DockingEquipHistory m_frmEquipHistoryList = null;

        public DockingEquipHistory FrmEquipHistoryList
        {
            get { return m_frmEquipHistoryList; }
            set { m_frmEquipHistoryList = value; }
        }

        public DBUtility.WebDBManager DBManager
        {
            get { return m_dbMgr; }
        }


        private bool m_isPCMode = true;
        //private FormPanel m_frmPanel = null;

        private int nTypePictureBoxTab = 0;
        public int TypePictureBoxTab
        {
            get { return nTypePictureBoxTab; }
            set { nTypePictureBoxTab = value; }
        }

        private FireManagement.Docking.FormPanel2 m_frmPanel2 = null;
        public System.Windows.Forms.Panel PanelMain
        {
            get { return panelMain; }
            set { panelMain = value; }
        }

        public System.Windows.Forms.Panel PanelTop
        {
            get { return panelTop; }
            set { panelTop = value; }
        }
        public System.Windows.Forms.Panel PanelLeft
        {
            get { return panelLeft; }
            set { panelLeft = value; }
        }

        public bool IsPCMode
        {
            get { return m_isPCMode; }
        }

        public FireManagement.IOManager IOManager
        {
            get { return m_ioMgr; }
        }

        public bool TagInputMode
        {
            get { return m_tagInputMode; }
        }

        public int BluetoothComport
        {
            get { return m_nBluetoothComport; }
        }

        public DataFileManager FileManager
        {
            get { return m_fileMgr; }
        }
        private Zone m_zoneCurrent = null;
        public Zone CurrentZone
        {
            get { return m_zoneCurrent; }
            set { m_zoneCurrent = value; }
        }

        private DXFManager m_dxfManager = new DXFManager();
        public FireManagement.DXFManager DXFManager
        {
            get { return m_dxfManager; }
        }

        public ArrayList CurrentEquipments
        {
            get { return m_dxfManager.Equipments; }
        }

        public Dictionary<FireEquipment, FireEquipmentHistory> CurrentEquipHisotry
        {
            get { return m_dxfManager.EquipmentHistory; }
        }

        // 길이 단위 변환을 위한 Flag
        public float UnitFlag
        {
            get { return GetUnitFlag(DXFViewer.UnitOfLength.METER); }
        }

        public DXFViewer.DXFControl DXFControl
        {
            get { return m_frmPanel2.DXFControl; }
        }

        public FormPanel2 ViewControl
        {
            get { return m_frmPanel2; }
        }

        public Ubists.RFIDReader RFIDReader
        {
            get { return m_rfidReader; }
        }

        public FormCheckEquip3 EquipmentChecker
        {
            get { return m_frmCheckEquip; }
        }

        public FormEquipHistory EquipmentHistoryViewer
        {
            set { m_frmEquipHistory = value; }
            get { return m_frmEquipHistory; }
        }

        public int SOPGenUserID
        {
            get { return m_nSOPGenUserID; }
        }

        public string StatusText
        {
            set { lblStatusText.Text = value; }
        }

        public bool NeedScreenInput()
        {
            return m_frmPanel2.FrmAddEquip.IsWorking;
        }

        public void ScreenInput(double x, double y)
        {
            if (m_frmPanel2.FrmAddEquip.IsWorking)
                m_frmPanel2.FrmAddEquip.ScreenInput(x, y);
        }

        public void DeleteEquipment(FireEquipment equip)
        {
            if (equip == null)
                return;

            m_frmPanel2.LeftBar.DeleteEquipment(equip);
            m_frmPanel2.FrmEquipHistory.DeleteEquipment(equip);
            IOManager.DeleteEquipment(equip);
            DXFManager.DeleteEquipment(equip);
        }

        public UnE.GUI.TextPictureBox PictureBoxCheckEquip
        {
            get { return pictureBoxCheckEquip; }
            set { pictureBoxCheckEquip = value; }
        }
        public UnE.GUI.TextPictureBox PictureBoxHistory
        {
            get { return pictureBoxHistory; }
            set { pictureBoxHistory = value; }
        }
        public UnE.GUI.TextPictureBox PictureBoxEditMode
        {
            get { return pictureBoxEditMode; }
            set { pictureBoxEditMode = value; }
        }
        public UnE.GUI.TextPictureBox PictureBoxNormalMode
        {
            get { return pictureBoxNormalMode; }
            set { pictureBoxNormalMode = value; }
        }


        public UnE.GUI.TextPictureBox PictureBoxMgr
        {
            get { return pictureBoxMgr; }
            set { pictureBoxMgr = value; }
        }

        public UnE.GUI.TextPictureBox PictureBoxFire
        {
            get { return pictureBoxFire; }
            set { pictureBoxFire = value; }
        }

        public void SetEquipmentLayer(FireEquipment.EquipmentType type, DXFViewer.Layer layer)
        {
            m_dicEquipmentLayer[type] = layer;
        }

        public System.Windows.Forms.Label ZoneNameText
        {
            get { return lblZoneName; }
            set { lblZoneName = value; }
        }

        public Form MainFrame
        {
            get { return FormFrame.Instance; }
            //get { return this; }
        }

        public System.Windows.Forms.Panel PanelTitle
        {
            get { return panelTitle; }
            set { panelTitle = value; }
        }

        private FireManagement.Docking.FormFileLoad m_FormFileLoad = null;
        public FireManagement.Docking.FormFileLoad FormFileLoad
        {
            get { return m_FormFileLoad; }
            set { m_FormFileLoad = value; }
        }

        public System.Windows.Forms.Panel PanelBottom
        {
            get { return panelBottom; }
            set { panelBottom = value; }
        }

        // 도면내 설비들을 하나씩 Click해 나가면서 설비 ID를 입력해야 하는 모드인가?
        private bool m_tagInputMode = false;
        private int m_nBluetoothComport = 0;
        

        static public FormMain2 Instance = null;
        public FormMain2()
        {
            // 저장기간이 지난 로그 삭제
            LogManager mgr = LogManager.Instance;

            InitializeComponent();
            

            Instance = this;
            InitMode();
           
            m_ioMgr = new IOManager();
            m_fileMgr = new DataFileManager();
            m_dbMgr = new WebDBManager();

            m_frmCheckEquip = new FormCheckEquip3();
            m_frmEquipHistoryList = new DockingEquipHistory();

            LoadDB();

            m_btnBeforeSelected = btnLoad;
            m_btnCurrentSelected = btnLoad;
           
            //ShowSplash();
        }

        private void FormMain2_Load(object sender, EventArgs e)
        {
            if (this.TopLevel == false)
            {
                Form form = this.ParentForm;
                if (form != null)
                {
                    form.FormClosing += FormMain2_FormClosing;
                }
            }

            InitTab();
            InitPanels();
            InitButtons();

            StatusText = "";
            lblZoneName.Visible = false;
            lblMenuName.Text = "파일 > 불러오기";

            MainFrame.WindowState = FormWindowState.Maximized;
        }

        private void InitMode()
        {
            Utility m_ini = new Utility();
            string strPath = Application.StartupPath + "\\FMConfig.ini";

            string strValue = m_ini.getinivalue("General", "isPCMode", strPath);
            m_isPCMode = strValue == "1" ? true : false;

            strValue = m_ini.getinivalue("General", "TagInputMode", strPath);
            m_tagInputMode = strValue == "1" ? true : false;

            strValue = m_ini.getinivalue("General", "BluetoothComport", strPath);
            int.TryParse(strValue, out m_nBluetoothComport);

            int nFakeMode;
            strValue = m_ini.getinivalue("General", "FakeRFIDReader", strPath);

            if (int.TryParse(strValue, out nFakeMode))
                m_rfidReader.FakeMode = nFakeMode == 1 ? true : false;

            int nEquipZoneTextVisible;
            strValue = m_ini.getinivalue("EquipZoneText", "Visible", strPath);

            if (int.TryParse(strValue, out nEquipZoneTextVisible))
                DXFManager.ShowEquipZoneText = nEquipZoneTextVisible == 1 ? true : false;

            int nEditEquipZoneTextPosition;
            strValue = m_ini.getinivalue("EquipZoneText", "PosEdit", strPath);

            if (int.TryParse(strValue, out nEditEquipZoneTextPosition))
                FormPanel2.EquipZoneTextEditMode = nEditEquipZoneTextPosition == 1 ? true : false;
        }


        private void LoadDB()
        {
            if (IsPCMode)
                m_ioMgr.LoadDB();
            else
            {
                bool isPCMode = !m_isPCMode;
                m_fileMgr.FirstRead = true;
                m_fileMgr.ImportData(System.Windows.Forms.Application.StartupPath + "\\" + IOManager.TabletDataFile, ref isPCMode);
                m_fileMgr.FirstRead = false;
            }
        }

        private void InitPanels()
        {
            panelMain.Location = new Point(panelLeft.Right, panelTop.Location.Y + panelTop.Size.Height);
            panelMain.Size = new Size(this.Size.Width, this.Size.Height - panelMain.Location.Y);

            m_FormFileLoad.Location = new Point(0, 0);
            m_FormFileLoad.Dock = DockStyle.Fill;
            m_FormFileLoad.TopLevel = false;
            m_FormFileLoad.Parent = this;
            panelMain.Controls.Add(m_FormFileLoad);
            m_FormFileLoad.Show();

            m_pageUpdate.Location = new Point(0, 0);
            m_pageUpdate.Dock = DockStyle.Fill;
            m_pageUpdate.TopLevel = false;
            m_pageUpdate.Parent = this;
            panelMain.Controls.Add(m_pageUpdate);
            m_pageUpdate.Show();

            m_pageSave.Location = new Point(0, 0);
            m_pageSave.Dock = DockStyle.Fill;
            m_pageSave.TopLevel = false;
            m_pageSave.Parent = this;
            panelMain.Controls.Add(m_pageSave);
            m_pageSave.Show();

            m_pageExit.Location = new Point(0, 0);
            m_pageExit.Dock = DockStyle.Fill;
            m_pageExit.TopLevel = false;
            m_pageExit.Parent = this;
            panelMain.Controls.Add(m_pageExit);
            m_pageExit.Show();

            m_frmPanel2.Location = new Point(0, 0);
            m_frmPanel2.Dock = DockStyle.Fill;
            m_frmPanel2.TopLevel = false;
            m_frmPanel2.Parent = this;
            panelMain.Controls.Add(m_frmPanel2);
            m_frmPanel2.Show();
        }

        private void InitTab()
        {
            m_FormFileLoad = new FireManagement.Docking.FormFileLoad(m_ioMgr);
            m_frmPanel2 = new FormPanel2();
            m_pageUpdate = new PageBackstageUpdate();
            m_pageSave = new PageBackstageSave2();
            m_pageExit = new PageBackstageClose();

            if (!IsPCMode)
            {
                m_FormFileLoad.SetDocumentInfo(m_fileMgr.Header);
            }

            pictureBoxNormalMode.Location = new Point(FormMain2.Instance.Width - 570 , panelinTop.Location.Y);
            pictureBoxEditMode.Location = new Point(pictureBoxNormalMode.Location.X + 130, pictureBoxNormalMode.Location.Y);
            pictureBoxHistory.Location = pictureBoxNormalMode.Location;
            PictureBoxCheckEquip.Location = PictureBoxEditMode.Location;

            pictureBoxNormalMode.Visible = false;
            pictureBoxEditMode.Visible = false;
            pictureBoxHistory.Visible = false;
            pictureBoxCheckEquip.Visible = false;

            pictureBoxFile.SetPictureBoxOwner(this);
            pictureBoxFire.SetPictureBoxOwner(this);
            pictureBoxMgr.SetPictureBoxOwner(this);
            pictureBoxNormalMode.SetPictureBoxOwner(this);
            pictureBoxEditMode.SetPictureBoxOwner(this);
            pictureBoxHistory.SetPictureBoxOwner(this);
            pictureBoxCheckEquip.SetPictureBoxOwner(this);

            SelectFileTab();
        }

        private void InitRibbonButton(RibbonButton btn, int nID, Image imgNormal, Image imgChecked, Image imgDisabled, Image imgMouseOverBkgnd, Image imgCheckedBkgnd, Image imgDisabledBkgnd)
        {
            btn.NormalImage = imgNormal;
            btn.CheckedImage = imgChecked;
            btn.DisabledImage = imgDisabled;
            btn.MouseOverBkgndImage = imgMouseOverBkgnd;
            btn.CheckedBkgndImage = imgCheckedBkgnd;
            btn.DisabledBkgndImage = imgDisabledBkgnd;
            btn.Owner = this;

            SetButtonID(btn, nID);
        }

        public void SetButtonID(Button btn, int nID, string strTooltipText = "")
        {
            m_dicButtonIDs[btn] = nID;
            m_dicIDButtons[nID] = btn;
            m_dicButtonChecked[btn] = false;

            if (strTooltipText.Length > 0)
            {
                ToolTip tooltip = new ToolTip();
                tooltip.SetToolTip(btn, strTooltipText);
            }
        }

        public int GetButtonID(Button btn)
        {
            if (m_dicButtonIDs.ContainsKey(btn))
                return m_dicButtonIDs[btn];

            return -1;
        }

        private void InitButtons()
        {
            InitRibbonButton(btnLoad, ID.ID_FILE_LOAD, global::FireManagement.Properties.Resources.Load_Icon, null, null, global::FireManagement.Properties.Resources.LeftBar_Click_Area, global::FireManagement.Properties.Resources.LeftBar_Click_Area, null);
            InitRibbonButton(btnUpdate, ID.ID_FILE_UPDATE, global::FireManagement.Properties.Resources.Update_Icon, null, null, global::FireManagement.Properties.Resources.LeftBar_Click_Area, global::FireManagement.Properties.Resources.LeftBar_Click_Area, null);
            InitRibbonButton(btnSave, ID.ID_FILE_SAVE, global::FireManagement.Properties.Resources.Save_icon, null, null, global::FireManagement.Properties.Resources.LeftBar_Click_Area, global::FireManagement.Properties.Resources.LeftBar_Click_Area, null);
            InitRibbonButton(btnClose, ID.ID_FILE_EXIT, global::FireManagement.Properties.Resources.Close_icon, null, null, global::FireManagement.Properties.Resources.LeftBar_Click_Area, global::FireManagement.Properties.Resources.LeftBar_Click_Area, null);
        }

        public void TextPictureBox_MouseDown(TextPictureBox pictureBox, MouseEventArgs e)
        {
            if (e != null)
            {
                if (e.Button != System.Windows.Forms.MouseButtons.Left)
                    return;
            }

            if (pictureBox == pictureBoxFile)
            {
                SelectFileTab();
            }
            else if (pictureBox == pictureBoxFire)
            {
                SelectFireManagerTab(1);
            }
            else if (pictureBox == pictureBoxMgr)
            {
                SelectFireManagerTab(2);
            }
            else if (pictureBox == pictureBoxNormalMode)
            {
                pictureBoxNormalMode.BackgroundImage = global::FireManagement.Properties.Resources.MgrTab_click;
                pictureBoxEditMode.BackgroundImage = global::FireManagement.Properties.Resources.MgrTab_normal_;
                pictureBoxHistory.BackgroundImage = global::FireManagement.Properties.Resources.MgrTab_normal_;
                pictureBoxCheckEquip.BackgroundImage = global::FireManagement.Properties.Resources.MgrTab_normal_;
                m_frmPanel2.ChangeDocking(Mode.GENERAL);
            }
            else if (pictureBox == pictureBoxEditMode)
            {
                //m_frmPanel2.FrmAddEquip.Show();
                
                pictureBoxNormalMode.BackgroundImage = global::FireManagement.Properties.Resources.MgrTab_normal_;
                pictureBoxEditMode.BackgroundImage = global::FireManagement.Properties.Resources.MgrTab_click;
                pictureBoxHistory.BackgroundImage = global::FireManagement.Properties.Resources.MgrTab_normal_;
                pictureBoxCheckEquip.BackgroundImage = global::FireManagement.Properties.Resources.MgrTab_normal_;
                m_frmPanel2.ChangeDocking(Mode.EDIT);
            }
            else if (pictureBox == pictureBoxHistory)
            {
                pictureBoxNormalMode.BackgroundImage = global::FireManagement.Properties.Resources.MgrTab_normal_;
                pictureBoxEditMode.BackgroundImage = global::FireManagement.Properties.Resources.MgrTab_normal_;
                pictureBoxHistory.BackgroundImage = global::FireManagement.Properties.Resources.MgrTab_click;
                pictureBoxCheckEquip.BackgroundImage = global::FireManagement.Properties.Resources.MgrTab_normal_;
                m_frmPanel2.ChangeDocking(Mode.EQUIP_HISTORY);
            }
            else if (pictureBox == pictureBoxCheckEquip)
            {
                pictureBoxNormalMode.BackgroundImage = global::FireManagement.Properties.Resources.MgrTab_normal_;
                pictureBoxEditMode.BackgroundImage = global::FireManagement.Properties.Resources.MgrTab_normal_;
                pictureBoxHistory.BackgroundImage = global::FireManagement.Properties.Resources.MgrTab_normal_;
                pictureBoxCheckEquip.BackgroundImage = global::FireManagement.Properties.Resources.MgrTab_click;
                m_frmPanel2.ChangeDocking(Mode.CHECK_EQUIP);
            }

        }

        public void SelectFileTab()
        {
            pictureBoxFile.BackgroundImage = global::FireManagement.Properties.Resources.Top_3Btn_Click;
            pictureBoxFire.BackgroundImage = global::FireManagement.Properties.Resources.LeftTop_3_AreaBG;
            pictureBoxMgr.BackgroundImage = global::FireManagement.Properties.Resources.LeftTop_3_AreaBG;
            //panelinTop.BackgroundImage = global::FireManagement.Properties.Resources.Top_Titlebar;
            panelinTop.BackColor = Color.Transparent;

            m_frmPanel2.Visible = false;
            panelLeft.Visible = true;

            SelectedPage(2);

            lblMenuName.Visible = true;
            lblZoneName.Visible = false;

            panelMain.Location = new Point(panelLeft.Right, panelTop.Size.Height);
            panelMain.Size = new Size(this.Size.Width, this.Size.Height - panelTop.Size.Height);

            nTypePictureBoxTab = 0;
            m_frmPanel2.ChangedTab();
        }

        public void SelectFireManagerTab(int nTabType)
        {
            if (nTabType == 1)
            {
                pictureBoxFile.BackgroundImage = global::FireManagement.Properties.Resources.LeftTop_3_AreaBG;
                pictureBoxFile.BackgroundImageLayout = ImageLayout.Stretch;
                pictureBoxFire.BackgroundImage = global::FireManagement.Properties.Resources.Top_3Btn_Click;
                pictureBoxFire.BackgroundImageLayout = ImageLayout.Stretch;
                pictureBoxMgr.BackgroundImage = global::FireManagement.Properties.Resources.LeftTop_3_AreaBG;
               // panelinTop.BackgroundImage = global::FireManagement.Properties.Resources.File_Open_Top_graybar;
                panelinTop.BackColor = Color.DimGray;

                //탭을 체인지하면 일반모드를 기본적으로 선택하게끔한다.
                pictureBoxNormalMode.BackgroundImage = global::FireManagement.Properties.Resources.MgrTab_click;
                pictureBoxEditMode.BackgroundImage = global::FireManagement.Properties.Resources.MgrTab_normal_;
                pictureBoxHistory.BackgroundImage = global::FireManagement.Properties.Resources.MgrTab_normal_;
                pictureBoxCheckEquip.BackgroundImage = global::FireManagement.Properties.Resources.MgrTab_normal_;
                m_frmPanel2.ChangeDocking(Mode.GENERAL);
                                
                nTypePictureBoxTab = 1;
                m_frmPanel2.ChangedTab();
            }
            else if(nTabType == 2)
            {
                pictureBoxFile.BackgroundImage = global::FireManagement.Properties.Resources.LeftTop_3_AreaBG;
                pictureBoxFire.BackgroundImage = global::FireManagement.Properties.Resources.LeftTop_3_AreaBG;
                pictureBoxMgr.BackgroundImage = global::FireManagement.Properties.Resources.Top_3Btn_Click;
               // panelinTop.BackgroundImage = global::FireManagement.Properties.Resources.File_Open_Top_graybar;
                panelinTop.BackColor = Color.DimGray;

                //탭을 체인지하면 이력확인을 기본적으로 선택하게끔한다.
                pictureBoxNormalMode.BackgroundImage = global::FireManagement.Properties.Resources.MgrTab_normal_;
                pictureBoxEditMode.BackgroundImage = global::FireManagement.Properties.Resources.MgrTab_normal_;
                pictureBoxHistory.BackgroundImage = global::FireManagement.Properties.Resources.MgrTab_click;
                pictureBoxCheckEquip.BackgroundImage = global::FireManagement.Properties.Resources.MgrTab_normal_;
                m_frmPanel2.ChangeDocking(Mode.EQUIP_HISTORY);

                nTypePictureBoxTab = 2;
                m_frmPanel2.ChangedTab();
            }

            //panelTop.Size = new Size(panelTop.Size.Width, m_nPanelTopInitHeight);

            m_FormFileLoad.Visible = false;
            m_frmPanel2.Visible = true;
            panelLeft.Visible = false;
            m_pageUpdate.Visible = false;
            m_pageSave.Visible = false;
            m_pageExit.Visible = false;

            lblMenuName.Visible = false;
            lblZoneName.Visible = true;

            panelMain.Location = new Point(0, panelTop.Size.Height);
            panelMain.Size = new Size(this.Size.Width, this.Size.Height - panelTop.Size.Height);
        }


        public void OnRibbonButtonMouseDown(object sender, MouseEventArgs e)
        {

        }

        public void OnRibbonButtonMouseUp(object sender, MouseEventArgs e)
        {
            RibbonButton btn = (RibbonButton)sender;
            int nButtonID = GetButtonID(btn);

            btn.IsChecked = true;
            switch (nButtonID)
            {
                case ID.ID_FILE_LOAD:
                    m_FormFileLoad.Visible = true;
                    m_pageUpdate.Visible = false;
                    m_pageSave.Visible = false;
                    m_pageExit.Visible = false;

                    m_btnBeforeSelected = m_btnCurrentSelected;
                    m_btnCurrentSelected = btnLoad;

                    btnRefresh(btnUpdate, btnSave, btnClose);

                    lblMenuName.Text = "파일 > 불러오기";
                    break;
                case ID.ID_FILE_UPDATE :
                    m_FormFileLoad.Visible = false;
                    m_pageUpdate.Visible = true;
                    m_pageSave.Visible = false;
                    m_pageExit.Visible = false;

                    m_btnBeforeSelected = m_btnCurrentSelected;
                    m_btnCurrentSelected = btnUpdate;

                    btnRefresh(btnLoad, btnSave, btnClose);

                    lblMenuName.Text = "파일 > 업데이트";
                    break;
                case ID.ID_FILE_SAVE :
                    m_FormFileLoad.Visible = false;
                    m_pageUpdate.Visible = false;
                    m_pageSave.Visible = true;
                    m_pageExit.Visible = false;

                    m_btnBeforeSelected = m_btnCurrentSelected;
                    m_btnCurrentSelected = btnSave;

                    btnRefresh(btnUpdate, btnLoad, btnClose);

                    lblMenuName.Text = "파일 > 저장";
                    break;
                case ID.ID_FILE_EXIT :
                    m_FormFileLoad.Visible = false;
                    m_pageUpdate.Visible = false;
                    m_pageSave.Visible = false;
                    m_pageExit.Visible = true;

                    m_btnBeforeSelected = m_btnCurrentSelected;
                    m_btnCurrentSelected = btnClose;

                    btnRefresh(btnUpdate, btnSave, btnLoad);

                    lblMenuName.Text = "파일 > 닫기";
                    break;
            }
        }

        private void btnRefresh(RibbonButton btn1, RibbonButton btn2, RibbonButton btn3)
        {
            if (btn1.IsChecked == true)
            {
                btn1.IsChecked = false;
                //btn1.Refresh();
            }
            else if (btn2.IsChecked == true)
            {
                btn2.IsChecked = false;
               // btn2.Refresh();
            }
            else if (btn3.IsChecked == true)
            {
                btn3.IsChecked = false;
              //  btn3.Refresh();
            }

            btnUpdate.Refresh();
            btnLoad.Refresh();
            btnSave.Refresh();
            btnClose.Refresh();
        }

        public void SelectedPage(int nType)
        {
            RibbonButton rbbtn = null;

            //전에있던 페이지
            if (nType == 1)
            {
                rbbtn = m_btnBeforeSelected;
            }
            //현재 선택되어있는 페이지
            else if(nType == 2)
            {
                rbbtn = m_btnCurrentSelected;
            }

            if (rbbtn == btnLoad)
            {
                m_FormFileLoad.Visible = true;

                m_btnBeforeSelected = m_btnCurrentSelected;
                m_btnCurrentSelected = btnLoad;

                btnRefresh(btnUpdate, btnSave, btnClose);

                btnLoad.IsChecked = true;
                lblMenuName.Text = "파일 > 불러오기";
            }
            else if (rbbtn == btnUpdate)
            {
                m_pageUpdate.Visible = true;

                m_btnBeforeSelected = m_btnCurrentSelected;
                m_btnCurrentSelected = btnUpdate;

                btnRefresh(btnLoad, btnSave, btnClose);

                btnUpdate.IsChecked = true;
                lblMenuName.Text = "파일 > 업데이트";
            }
            else if (rbbtn == btnSave)
            {
                m_pageSave.Visible = true;

                m_btnBeforeSelected = m_btnCurrentSelected;
                m_btnCurrentSelected = btnSave;

                btnRefresh(btnUpdate, btnLoad, btnClose);

                btnSave.IsChecked = true;
                lblMenuName.Text = "파일 > 저장";
            }

            if (nType == 2)
            {
                if (rbbtn == btnClose)
                {
                    m_pageExit.Visible = true;

                    m_btnBeforeSelected = m_btnCurrentSelected;
                    m_btnCurrentSelected = btnClose;

                    btnRefresh(btnUpdate, btnLoad, btnSave);

                    btnClose.IsChecked = true;
                    lblMenuName.Text = "파일 > 닫기";
                }
            }
        }


        // DXFViewer의 단위계를 unitTrg으로 변환하기 위한 flag 값을 리턴한다.
        public float GetUnitFlag(DXFViewer.UnitOfLength unitTrg)
        {
            if (DXFControl == null)
                return 1.0f;

            DXFViewer.UnitOfLength unitSrc = DXFControl.UnitOfLength;

            if (unitSrc == DXFViewer.UnitOfLength.INCH)
            {
                if (unitTrg == DXFViewer.UnitOfLength.INCH)
                    return 1.0f;
                else if (unitTrg == DXFViewer.UnitOfLength.FEET)
                    return 1.0f / 12;
                else if (unitTrg == DXFViewer.UnitOfLength.MILLIMETER)
                    return 25.4f;
                else if (unitTrg == DXFViewer.UnitOfLength.CENTIMETER)
                    return 2.54f;
                else if (unitTrg == DXFViewer.UnitOfLength.METER)
                    return 0.0254f;
            }
            else if (unitSrc == DXFViewer.UnitOfLength.FEET)
            {
                if (unitTrg == DXFViewer.UnitOfLength.INCH)
                    return 12.0f;
                else if (unitTrg == DXFViewer.UnitOfLength.FEET)
                    return 1.0f;
                else if (unitTrg == DXFViewer.UnitOfLength.MILLIMETER)
                    return 304.8f;
                else if (unitTrg == DXFViewer.UnitOfLength.CENTIMETER)
                    return 30.48f;
                else if (unitTrg == DXFViewer.UnitOfLength.METER)
                    return 0.3048f;
            }
            else if (unitSrc == DXFViewer.UnitOfLength.MILLIMETER)
            {
                if (unitTrg == DXFViewer.UnitOfLength.INCH)
                    return 1.0f / 25.4f;
                else if (unitTrg == DXFViewer.UnitOfLength.FEET)
                    return 1.0f / 25.4f / 12f;
                else if (unitTrg == DXFViewer.UnitOfLength.MILLIMETER)
                    return 1.0f;
                else if (unitTrg == DXFViewer.UnitOfLength.CENTIMETER)
                    return 0.1f;
                else if (unitTrg == DXFViewer.UnitOfLength.METER)
                    return 0.001f;
            }
            else if (unitSrc == DXFViewer.UnitOfLength.CENTIMETER)
            {
                if (unitTrg == DXFViewer.UnitOfLength.INCH)
                    return 1.0f / 2.54f;
                else if (unitTrg == DXFViewer.UnitOfLength.FEET)
                    return 1.0f / 2.54f / 12;
                else if (unitTrg == DXFViewer.UnitOfLength.MILLIMETER)
                    return 10;
                else if (unitTrg == DXFViewer.UnitOfLength.CENTIMETER)
                    return 1.0f;
                else if (unitTrg == DXFViewer.UnitOfLength.METER)
                    return 0.01f;
            }
            else if (unitSrc == DXFViewer.UnitOfLength.METER)
            {
                if (unitTrg == DXFViewer.UnitOfLength.INCH)
                    return 1.0f / 0.0254f;
                else if (unitTrg == DXFViewer.UnitOfLength.FEET)
                    return 1.0f / 0.0254f / 12;
                else if (unitTrg == DXFViewer.UnitOfLength.MILLIMETER)
                    return 1000.0f;
                else if (unitTrg == DXFViewer.UnitOfLength.CENTIMETER)
                    return 100.0f;
                else if (unitTrg == DXFViewer.UnitOfLength.METER)
                    return 1.0f;
            }

            return 1.0f;
        }

        public DXFViewer.Layer GetEquipmentLayer(FireEquipment.EquipmentType type)
        {
            if (m_dicEquipmentLayer.ContainsKey(type))
                return m_dicEquipmentLayer[type];

            return null;
        }

        public FireEquipment.EquipmentType GetEquipmentLayerType(DXFViewer.Layer layer)
        {
            foreach (KeyValuePair<FireEquipment.EquipmentType, DXFViewer.Layer> pair in m_dicEquipmentLayer)
            {
                if (pair.Value == layer)
                    return pair.Key;
            }

            return FireEquipment.EquipmentType.UNKNOWN;
        }

        public void ClearEquipmentLayer()
        {
            m_dicEquipmentLayer.Clear();
        }

        public void SetEquipmentLayerOnOff()
        {
            EquipmentLayerOn(FireEquipment.EquipmentType.FE, m_frmPanel2.BtnFireExtinguisher.IsChecked, false);
            EquipmentLayerOn(FireEquipment.EquipmentType.HD, m_frmPanel2.BtnFirePlug.IsChecked, false);
            EquipmentLayerOn(FireEquipment.EquipmentType.FA, m_frmPanel2.BtnFireAlarm.IsChecked, false);

            ViewControl.LeftBar.Rearrange(m_frmPanel2.BtnFireExtinguisher.IsChecked, m_frmPanel2.BtnFirePlug.IsChecked, m_frmPanel2.BtnFireAlarm.IsChecked);
        }
        public void EquipmentLayerOn(FireEquipment.EquipmentType type, bool turnOn, bool refresh = true)
        {
            if (m_dicEquipmentLayer.ContainsKey(type))
            {
                DXFViewer.Layer layer = m_dicEquipmentLayer[type];
                layer.Hidden = !turnOn;

                if (refresh)
                    ViewControl.Refresh();
            }
        }

        public void GroupShowHide(FireEquipment.EquipmentType type, bool trunOn, bool refresh = true)
        {
            if (m_dicEquipmentLayer.ContainsKey(type))
            {
                DXFViewer.Layer layer = m_dicEquipmentLayer[type];

                //layer.VisibleGroup = trunOn;

                if (refresh)
                    ViewControl.Refresh();

            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnMax_Click(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                this.WindowState = FormWindowState.Maximized;
                btnMax.BackgroundImage = global::FireManagement.Properties.Resources.NormalWindow_Normal;
            }
            else if (this.WindowState == FormWindowState.Maximized)
            {
                this.WindowState = FormWindowState.Normal;
                btnMax.BackgroundImage = global::FireManagement.Properties.Resources.MaxWindow_Normal;
            }
        }

        private void btnMin_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void FormMain2_FormClosing(object sender, FormClosingEventArgs e)
        {
            // 현재 열려있는 Zone에서 변경된 데이터가 있는지 확인한다.
            Zone zoneCurrent = this.CurrentZone;

            if (zoneCurrent != null)
                IOManager.CompareZoneEquipmentsToDB(zoneCurrent);

            if (IOManager.ChangedZones.Count > 0)
            {
                if (MessageBox.Show("시스템에 저장되지 않은 변경된 데이터가 존재합니다.\r\n지금 저장하지 않으면 프로그램 종료시 변경된 모든 데이터는 초기화 됩니다.\r\n변경된 데이터를 저장하시겠습니까?",
                    "경고", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                {
                    if (IsPCMode)
                        m_pageSave.SaveToDB(false);
                    else
                        m_pageSave.SaveToFile(false);
                }
            }

           // FormMain2.Instance.RFIDReader.Owner = null;
            RFIDReader.FinishReading(true);
        }

        private int nFormWidth = 600;

        private void FormMain2_Resize(object sender, EventArgs e)
        {
            if (nTypePictureBoxTab == 0)
            {
                panelMain.Location = new Point(panelLeft.Right, panelTop.Location.Y + panelTop.Size.Height);
                panelMain.Size = new Size(this.Size.Width, this.Size.Height - panelMain.Location.Y);

            }
            else
            {
                panelMain.Location = new Point(0, panelTop.Location.Y + panelTop.Size.Height);
                panelMain.Size = new Size(this.Size.Width, this.Size.Height - panelTop.Size.Height);
            }
            panelBottom.Location = new Point(panelBottom.Location.X, this.panelLeft.Bottom);

           
            //if (FormFrame.Instance.WindowState == FormWindowState.Normal)
            //{
            //    FormFrame.Instance.TitlePosition = (this.Width / 2) - (FormFrame.Instance.TitleTextWidth / 2);
            //}

            //else if (FormFrame.Instance.WindowState == FormWindowState.Maximized)
            //{
            //    FormFrame.Instance.TitlePosition = (this.Width / 2) - (FormFrame.Instance.TitleTextWidth / 2);
            //}
            FormFrame.Instance.TitlePosition = (this.Width / 2) - (FormFrame.Instance.TitleTextWidth / 2);
            FormFrame.Instance.ResizeFrame();


            if (m_frmPanel2 != null)
            {
                m_frmPanel2.ResizeControls();

                pictureBoxNormalMode.Location = new Point(FormMain2.Instance.Width - 570, panelinTop.Location.Y);
                pictureBoxEditMode.Location = new Point(pictureBoxNormalMode.Location.X + 130, pictureBoxNormalMode.Location.Y);
                pictureBoxHistory.Location = pictureBoxNormalMode.Location;
                PictureBoxCheckEquip.Location = PictureBoxEditMode.Location;

                m_frmPanel2.LeftBar.ReSizeControl();
                m_frmPanel2.FrmEquipHistory.ReSizeControl();
            }
        }
    }
}
