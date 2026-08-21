using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using Sections;

namespace UnE.SOP.Sections
{
    public enum TabPageState 
    {
        USE = 1,
        NOUSE = 2
    }

    public partial class SectionTabPage : TabPage
    {
        public int height = 0;
        private ITabPageSpecialWorker m_tabPageSpecialWorker = null;
                
        private static TabControl m_ParentTab = null;
        public static System.Windows.Forms.TabControl ParentTab
        {
            get { return m_ParentTab; }
            set { m_ParentTab = value; }
        }

        public ITabPageSpecialWorker SpecialWorker
        {
            get { return m_tabPageSpecialWorker; }
            set { m_tabPageSpecialWorker = value; }
        }
        
        public SectionTabPage(TabControl tabControl)
            : base()
        {
            if (m_ParentTab == null)
                m_ParentTab = tabControl;

            InitializeComponent();

        }


        // Tab없애기
        //private string m_szText = "";
        //public override string Text 
        //{
        //    get { return m_szText; }
        //    set
        //    {
        //        base.Text = "";
        //        m_szText = value;
        //    } 
        //}


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
        public TabPageState State
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
            set
            {
                m_nActionStepHistoryID = value;                
            }
        }

        private bool bUseWaterMark = false;
        public bool UseWaterMark
        {
            get { return bUseWaterMark; }
            set {
                bUseWaterMark = value;
                WatermarkImage();
            }
        }

        private int m_nSensorZoneHistoryID = -1;
        public int SensorZoneHistoryID
        {
            get { return m_nSensorZoneHistoryID; }
            set { m_nSensorZoneHistoryID = value; }
        }

        private int m_nSensorID = -1;
        public int SensorID
        {
            get { return m_nSensorID; }
            set { m_nSensorID = value; }
        }
        

        #region 이 TabPage에 나타나는 SOP에 사용된 팀 리스트
        // 사용자 정의조직 ID, 연결된 정의 조직 Data
        private Dictionary<int, Data_UserDefinedTeam> m_dicUserDefinedTeam = new Dictionary<int, Data_UserDefinedTeam>();
        // 외부조직 ID, 연결된 외부조직 Data
        private Dictionary<int, Data_ExternalTeam> m_dicExternalTeam = new Dictionary<int, Data_ExternalTeam>();
        // 평일 비상조직 ID, 연결된 연결된 평일 비상조직 Data
        private Dictionary<int, Data_NormalTeam> m_dicTemporaryNormalTeam = new Dictionary<int, Data_NormalTeam>();
        // 야간 및 휴일 비상조직 ID, 연결된 연결된 야간 및 휴일 비상조직 Data
        private Dictionary<int, Data_EmergencyTeam> m_dicTemporaryEmergencyTeam = new Dictionary<int, Data_EmergencyTeam>();
        // 정규조직 ID, 연결된 정규조직 Data
        private Dictionary<int, Data_RegularTeam> m_dicRegularTeam = new Dictionary<int, Data_RegularTeam>();
        private Dictionary<int, Data_ControlRoom> m_dicControlRoom = new Dictionary<int, Data_ControlRoom>();
        #endregion

        public List<Data_UserDefinedTeam> GetUsingUserDefineTeams()
        {
            List<Data_UserDefinedTeam> arResult = new List<Data_UserDefinedTeam>();
            arResult.AddRange(m_dicUserDefinedTeam.Values);
            return arResult;
        }

        public List<Data_ExternalTeam> GetUsingExternalTeams()
        {
            List<Data_ExternalTeam> arResult = new List<Data_ExternalTeam>();
            arResult.AddRange(m_dicExternalTeam.Values);
            return arResult;
        }

        public List<Data_NormalTeam> GetUsingTemporaryNormalTeams()
        {
            List<Data_NormalTeam> arResult = new List<Data_NormalTeam>();
            arResult.AddRange(m_dicTemporaryNormalTeam.Values);
            return arResult;
        }

        public List<Data_EmergencyTeam> GetUsingTemporaryEmergencyTeams()
        {
            List<Data_EmergencyTeam> arResult = new List<Data_EmergencyTeam>();
            arResult.AddRange(m_dicTemporaryEmergencyTeam.Values);
            return arResult;
        }

        public List<Data_RegularTeam> GetUsingRegularTeams()
        {
            List<Data_RegularTeam> arResult = new List<Data_RegularTeam>();
            arResult.AddRange(m_dicRegularTeam.Values);
            return arResult;
        } 

        public void AddExternalTeam(Data_ExternalTeam team)
        {
            m_dicExternalTeam[team.ID] = team;
            /*if (!m_dicExternalTeam.ContainsKey(team.ID))
            {
                m_dicExternalTeam.Add(team.ID, team);
            }*/
        }

        public void AddUserDefinedTeam(Data_UserDefinedTeam team)
        {
            m_dicUserDefinedTeam[team.ID] = team;
            /*if (!m_dicUserDefinedTeam.ContainsKey(team.ID))
            {
                m_dicUserDefinedTeam.Add(team.ID, team);
            }*/
        }

        public void AddTemporaryNormalTeam(Data_NormalTeam team)
        {
            m_dicTemporaryNormalTeam[team.ID] = team;
            /*if (!m_dicTemporaryNormalTeam.ContainsKey(team.ID))
            {
                m_dicTemporaryNormalTeam.Add(team.ID, team);
            }*/
        }

        public void AddTemporaryEmergencyTeam(Data_EmergencyTeam team)
        {
            m_dicTemporaryEmergencyTeam[team.ID] = team;
            /*if (!m_dicTemporaryEmergencyTeam.ContainsKey(team.ID))
            {
                m_dicTemporaryEmergencyTeam.Add(team.ID, team);
            }*/
        }

        public void AddRegularTeam(Data_RegularTeam team)
        {
            m_dicRegularTeam[team.ID] = team;
            /*if (!m_dicRegularTeam.ContainsKey(team.ID))
            {
                m_dicRegularTeam.Add(team.ID, team);
            }*/
        }
        public void AddControlRoom(Data_ControlRoom team)
        {
            m_dicControlRoom[team.ID] = team; 
        }

        /*public void AddUserDefinedTeams(ArrayList teams)
        {
            m_dicUserDefinedTeam.Clear();
            foreach (Data_UserDefinedTeam team in teams)
            {
                AddUserDefinedTeam(team);
            }
        }*/

        public Data_EmergencyTeam GetTemporaryEmergencyTeamMember(int nTeamID)
        {
            if (!m_dicTemporaryEmergencyTeam.ContainsKey(nTeamID))
                return null;

            return m_dicTemporaryEmergencyTeam[nTeamID];
        }

        public Data_NormalTeam GetTemporaryNormalTeamMember(int nTeamID)
        {
            if (!m_dicTemporaryNormalTeam.ContainsKey(nTeamID))
                return null;

            return m_dicTemporaryNormalTeam[nTeamID];
        }

        public Data_RegularTeam GetRegularTeamMember(int nTeamID)
        {
            if (!m_dicRegularTeam.ContainsKey(nTeamID))
            {
                if (UnE.SOP.ProxySOP.Instance.SOPDataContainer == null)
                    return null;

                SOPManager mgr = (SOPManager)UnE.SOP.ProxySOP.Instance.SOPDataContainer;
                return mgr.GetRegularTeam(nTeamID);
            }

            return m_dicRegularTeam[nTeamID];
        }
        public Data_ControlRoom GetControlRoomMember(int nTeamID)
        {
            if (!m_dicControlRoom.ContainsKey(nTeamID))
                return null;

            return m_dicControlRoom[nTeamID];
        }

        public Data_ExternalTeam GetExternalTeamMember(int nTeamID)
        {
            if (!m_dicExternalTeam.ContainsKey(nTeamID))
                return null;

            return m_dicExternalTeam[nTeamID];
        }

        public Data_UserDefinedTeam GetUserDefinedTeamMember(int nUserDefinedTeamID)
        {
            if (!m_dicUserDefinedTeam.ContainsKey(nUserDefinedTeamID))
                return null;

            return m_dicUserDefinedTeam[nUserDefinedTeamID];
        }       

        private Panel panelComponentContents = new Panel();
        public Panel PanelComponentContents
        {
            get { return panelComponentContents; }
        }

        private Panel panelPreviewComponentContents = null;
        public Panel PanelPreviewComponentContents
        {
            get { return panelPreviewComponentContents; }
            set { panelPreviewComponentContents = value; }
        }

        private string m_strLinkedZoneName = "";
        public string LinkedZoneName
        {
            get { return m_strLinkedZoneName; }
            set { m_strLinkedZoneName = value; }
        }

        private int m_nLinkedZoneID = -1;
        public int LinkedZoneID
        {
            get { return m_nLinkedZoneID; }
            set { m_nLinkedZoneID = value; }
        }

        private DateTime m_dtLinkedTime = new DateTime();
        public DateTime LinkedTime
        {
            get { return m_dtLinkedTime; }
            set { m_dtLinkedTime = value; }
        }

        public void WatermarkImage()
        {
            if (bVirtualMode && UseWaterMark)
            {
                Bitmap bitmap = new Bitmap(global::libSOP.Properties.Resources.BackgroundLog);
                foreach (Control contorl in Controls)
                {
                    if (typeof(PanelSection).IsAssignableFrom(contorl.GetType()))                  
                    {
                        PanelSection panel = (PanelSection)contorl;
                        panel.BackgroundImage = bitmap;
                        panel.BackgroundImageLayout = ImageLayout.None;
                    }                        
                }
            }
            else
            {
                Bitmap bitmap = new Bitmap(global::libSOP.Properties.Resources.BackgroundNon);
                foreach (Control contorl in Controls)
                {
                    if (typeof(PanelSection).IsAssignableFrom(contorl.GetType()))
                    {
                        PanelSection panel = (PanelSection)contorl;
                        panel.BackgroundImage = bitmap;
                        panel.BackgroundImageLayout = ImageLayout.None;
                    }                        
                }                    
            }
        }


        public override void Refresh()
        {
            base.Refresh();

            foreach (Control contorl in Controls)
            {
                if (typeof(PanelSection).IsAssignableFrom(contorl.GetType()))               
                {
                    PanelSection panel = (PanelSection)contorl;
                    panel.Refresh();
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
            TabControl tabControl = m_ParentTab;
                
            if( tabControl != null)
            {
                TabPage tabPage1 = this;
                Size sz = tabPage1.Size;
                int nCount = tabPage1.Controls.Count;
                int nVisibleCount = 0;
                foreach (PanelSection panel in tabPage1.Controls)
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
                    foreach (PanelSection panel in tabPage1.Controls)
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

        public List<PanelSection> GetPanelSections()
        {
            List<PanelSection> arPanels = new List<PanelSection>();
            foreach (PanelSection panel in Controls)
            {
                arPanels.Add(panel);
            }
            return arPanels;
        }
    }

    public interface ITabPageSpecialWorker
    {
        void Work(object arg);
    }
}
