using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Data.OleDb;
using XtremePropertyGrid;
using XtremeDockingPane;

namespace section
{
    public partial class FormMain : Form
    {
        public AxXtremeCommandBars.AxCommandBars CommandBars;

        private PageHome m_PageHome = null;

        public System.Windows.Forms.ContextMenuStrip MainContextMenuStrip
        {
            get { return mainContextMenuStrip; }
            set { mainContextMenuStrip = value; }
        }

        public FormMain()
        {
            Application.EnableVisualStyles();
            m_PageHome = new PageHome(this);


            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer, true); //더블버퍼링, 화면깜빡임 없애줌

            InitializeComponent();

            CommandBars = axCommandBars1;

       
            int left, top, right, bottom;
            CommandBars.GetClientRect(out left, out top, out right, out bottom);
            m_MainPanel.SetBounds(left, top, right - left, bottom - top);
           
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            LoadIcons();

            CreateRibbonBar();

            m_PageHome.Location = new Point(0, 0);
            m_PageHome.Dock = DockStyle.Fill;
            m_PageHome.TopLevel = false;
            m_PageHome.Parent = this;
            m_MainPanel.Controls.Add(m_PageHome);
            m_PageHome.Show();
        }

        private void CreateRibbonBar()
        {
            XtremeCommandBars.RibbonTab TabWrite = null;
            XtremeCommandBars.RibbonGroup GroupMenu = null;
            XtremeCommandBars.RibbonBar RibbonBar = null;

            XtremeCommandBars.CommandBarControl contorl = null;

            RibbonBar = CommandBars.AddRibbonBar("The Ribbon");
            RibbonBar.EnableDocking(XtremeCommandBars.XTPToolBarFlags.xtpFlagStretched);

            TabWrite = RibbonBar.InsertTab(0, "&Menu");
            TabWrite.Id = ID.ID_TAB_WRITE;

            GroupMenu = TabWrite.Groups.AddGroup("&DB", ID.ID_GROUP_CLIPBOARD);

            contorl = GroupMenu.Add(XtremeCommandBars.XTPControlType.xtpControlButton, ID.ID_BUTTON_SAVE, "Save", false, false);
            contorl.Height = 64;
            contorl = GroupMenu.Add(XtremeCommandBars.XTPControlType.xtpControlButton, ID.ID_BUTTON_LOAD, "Load", false, false);
            contorl.Height = 64;
            contorl = GroupMenu.Add(XtremeCommandBars.XTPControlType.xtpControlButton, ID.ID_BUTTON_EXIT, "Exit", false, false);
            contorl.Height = 64;


            RibbonBar.ShowQuickAccess = false;
            RibbonBar.ShowCaptionAlways = false;
        }

      

        private void axCommandBars1_Execute(object sender, AxXtremeCommandBars._DCommandBarsEvents_ExecuteEvent e)
        {
            switch (e.control.Id)
            {
                case ID.ID_BUTTON_SAVE:
                    Menu_Save();
                    break;

                case ID.ID_BUTTON_LOAD:
                    Menu_Load();
                    break;
                case ID.ID_BUTTON_EXIT:
                    Menu_Exit();
                    break;
                default:
                    break;
            }
        }

        private void axCommandBars1_ResizeEvent(object sender, EventArgs e)
        {

            int left, top, right, bottom;

            axCommandBars1.GetClientRect(out left, out top, out right, out bottom);
            m_MainPanel.SetBounds(left, top, right - left, bottom - top);

        }

        private void axCommandBars1_UpdateEvent(object sender, AxXtremeCommandBars._DCommandBarsEvents_UpdateEvent e)
        {

        }

        //경로
        public string ResourcePath()
        {
            string strExePath = System.IO.Path.GetDirectoryName(Application.ExecutablePath);

            if (System.IO.Directory.Exists(strExePath + "\\..\\res"))
                return strExePath + "\\..\\res\\";

            if (System.IO.Directory.Exists(strExePath + "\\..\\..\\res"))
                return strExePath + "\\..\\..\\res\\";

            if (System.IO.Directory.Exists(strExePath + "\\..\\..\\..\\res"))
                return strExePath + "\\..\\..\\..\\res\\";

            return strExePath + "\\res\\";
        }

        //아이콘
        private void LoadIcons()
        {
            CommandBars.Options.UseSharedImageList = false;
            CommandBars.Icons.LoadBitmap(ResourcePath() + "BackstageIcons.png", new int[] { ID.ID_BUTTON_SAVE, 2, ID.ID_BUTTON_LOAD, 2, 2, ID.ID_BUTTON_EXIT }, XtremeCommandBars.XTPImageState.xtpImageNormal);
        }

        private void Add_Click(object sender, EventArgs e)
        {
            m_PageHome.Add_Click(sender, e);
        }

        ////종료
        private void Menu_Exit()
        {
            this.Close();
        }

        //저장
        private void Menu_Save()
        {
            m_PageHome.SaveToDB();
        }

        //불러오기
        private void Menu_Load()
        {
            m_PageHome.LoadFromDB();
        }    

    }
}
