using DBUtility2;
using SDMS_Building.Network;

namespace SDMS_Building.Data
{
	public class PreferenceManager
	{
		private static PreferenceManager m_instance = null;

		public static PreferenceManager Instance
		{
			get
			{
				if (m_instance == null)
				{
					m_instance = new PreferenceManager();
				}
				return m_instance;
			}
		}

		private PreferenceManager()
		{
            // 센서 신호 수신 여부 로드
            LoadSignalState();
		}

		private bool m_bRealMode = false;
		public bool RealMode
		{
			get { return m_bRealMode; }

			set { m_bRealMode = value; }
		}
        
        // 화재 신호를 수신할것인지 여부
        private bool m_bReciveFireSignal = true;
        public bool ReciveFireSignal
        {
            get { return m_bReciveFireSignal; }
            set 
            {
                m_bReciveFireSignal = value;

                SaveSignalState();
            }
        }

        private bool m_bRecivePSMSignal = true;
        public bool RecivePSMSignal
        {
            get { return m_bRecivePSMSignal; }
            set 
            {
                m_bRecivePSMSignal = value;
                SaveSignalState();
            }
        }

        private bool m_bReciveSecuritySignal = true;
        public bool ReciveSecuritySignal
        {
            get { return m_bReciveSecuritySignal; }
            set
            {
                m_bReciveSecuritySignal = value;
                SaveSignalState();
            }
        }

        // 지진 신호를 수신할것인지 여부
        private bool m_bReciveEarthquakeSignal = true;
        public bool ReciveEarthquakeSignal
        {
            get { return m_bReciveEarthquakeSignal; }
            set
            {
                m_bReciveEarthquakeSignal = value;
                SaveSignalState();
            }
        }

        // 정전 신호를 수신할것인지 여부
        private bool m_bReciveBlackoutSignal = true;
        public bool ReciveBlackoutSignal
        {
            get { return m_bReciveBlackoutSignal; }
            set
            {
                m_bReciveBlackoutSignal = value;
                SaveSignalState();
            }
        }

        // 강풍 신호를 수신할것인지 여부
        private bool m_bReciveStrongWindSignal = true;
        public bool ReciveStrongWindSignal
        {
            get { return m_bReciveStrongWindSignal; }
            set
            {
                m_bReciveStrongWindSignal = value;
                SaveSignalState();
            }
        }

        // 방화벽 신호를 수신할것인지 여부
        private bool m_bReciveFirewallSignal = true;
        public bool ReciveFirewallSignal
        {
            get { return m_bReciveFirewallSignal; }
            set
            {
                m_bReciveFirewallSignal = value;
                SaveSignalState();
            }
        }

        private void SaveSignalState()
        {
            int nSiteID = UnE.SOP.ProxySOP.Instance.SiteID;
            
            bool bReciveFireSignal = true;
            bool bRecivePSMSignal = true;
            bool bReciveSecuritySignal = true;
            bool bReciveEarthquakeSignal = true;
            bool bReciveBlackoutSignal = true;
            bool bReciveStrongWindSignal = true;
            bool bReciveFirewallSignal = true;

            string szValue1 = RegUtil.ReadRegValue("SDMS", "ReciveFireSignal", nSiteID);
            if (szValue1.ToLower() == "false")
            {
                bReciveFireSignal = false;
            }

            string szValue2 = RegUtil.ReadRegValue("SDMS", "RecivePSMSignal", nSiteID);
            if (szValue2.ToLower() == "false")
            {
                bRecivePSMSignal = false;
            }

            string szValue3 = RegUtil.ReadRegValue("SDMS", "ReciveSecuritySignal", nSiteID);
            if (szValue3.ToLower() == "false")
            {
                bReciveSecuritySignal = false;
            }

            string szValue4 = RegUtil.ReadRegValue("SDMS", "ReciveEarthquakeSignal", nSiteID);
            if (szValue4.ToLower() == "false")
            {
                bReciveEarthquakeSignal = false;
            }

            string szValue5 = RegUtil.ReadRegValue("SDMS", "ReciveBlackoutSignal", nSiteID);
            if (szValue5.ToLower() == "false")
            {
                bReciveBlackoutSignal = false;
            }

            string szValue6 = RegUtil.ReadRegValue("SDMS", "ReciveStrongWindSignal", nSiteID);
            if (szValue6.ToLower() == "false")
            {
                bReciveStrongWindSignal = false;
            }

            string szValue7 = RegUtil.ReadRegValue("SDMS", "ReciveFirewallSignal", nSiteID);
            if (szValue7.ToLower() == "false")
            {
                bReciveFirewallSignal = false;
            }

            bool bRequestReactionLogList = false;
            bool bRemoveFireSignalList = false;
            bool bRemovePSMSignalList = false;
            bool bRemoveSecuritySignalList = false;
            bool bRemoveEarthquakeSignalList = false;
            bool bRemoveBlackoutSignalList = false;
            bool bRemoveStrongWindSignalList = false;
            bool bRemoveFirewallSignalList = false;

            if (bReciveFireSignal == false && m_bReciveFireSignal == true)
            {
                bRequestReactionLogList = true;
            }
            if (bRecivePSMSignal == false && m_bRecivePSMSignal == true)
            {
                bRequestReactionLogList = true;
            }
            if (bReciveSecuritySignal == false && m_bReciveSecuritySignal == true)
            {
                bRequestReactionLogList = true;
            }
            if (bReciveEarthquakeSignal == false && m_bReciveEarthquakeSignal == true)
            {
                bRequestReactionLogList = true;
            }
            if (bReciveBlackoutSignal == false && m_bReciveBlackoutSignal == true)
            {
                bRequestReactionLogList = true;
            }
            if (bReciveStrongWindSignal == false && m_bReciveStrongWindSignal == true)
            {
                bRequestReactionLogList = true;
            }
            if (bReciveFirewallSignal == false && m_bReciveFirewallSignal == true)
            {
                bRequestReactionLogList = true;
            }

            if (bReciveFireSignal == true && m_bReciveFireSignal == false)
            {
                bRemoveFireSignalList = true;
            }
            if (bRecivePSMSignal == true && m_bRecivePSMSignal == false)
            {
                bRemovePSMSignalList = true;
            }
            if (bReciveSecuritySignal == true && m_bReciveSecuritySignal == false)
            {
                bRemoveSecuritySignalList = true;
            }
            if (bReciveEarthquakeSignal == true && m_bReciveEarthquakeSignal == false)
            {
                bRemoveEarthquakeSignalList = true;
            }
            if (bReciveBlackoutSignal == true && m_bReciveBlackoutSignal == false)
            {
                bRemoveBlackoutSignalList = true;
            }
            if (bReciveStrongWindSignal == true && m_bReciveStrongWindSignal == false)
            {
                bRemoveStrongWindSignalList = true;
            }
            if (bReciveFirewallSignal == true && m_bReciveFirewallSignal == false)
            {
                bRemoveFirewallSignalList = true;
            }

            RegUtil.WriteRegValue("SDMS", "ReciveFireSignal", m_bReciveFireSignal.ToString(), nSiteID);
            RegUtil.WriteRegValue("SDMS", "RecivePSMSignal", m_bRecivePSMSignal.ToString(), nSiteID);
            RegUtil.WriteRegValue("SDMS", "ReciveSecuritySignal", m_bReciveSecuritySignal.ToString(), nSiteID);
            RegUtil.WriteRegValue("SDMS", "ReciveEarthquakeSignal", m_bReciveEarthquakeSignal.ToString(), nSiteID);
            RegUtil.WriteRegValue("SDMS", "ReciveBlackoutSignal", m_bReciveBlackoutSignal.ToString(), nSiteID);
            RegUtil.WriteRegValue("SDMS", "ReciveStrongWindSignal", m_bReciveStrongWindSignal.ToString(), nSiteID);
            RegUtil.WriteRegValue("SDMS", "ReciveFirewallSignal", m_bReciveFirewallSignal.ToString(), nSiteID);

            // TODO: SaveSignalState()
            if (bRequestReactionLogList == true)
            {
                //NetworkWebManager.Instance.SendRequestReactionLogList();
            }

            if (bRemoveFireSignalList == true)
            {
                //MainForm.Instance.RemoveAllFireSensorDetect();
            }

            if (bRemovePSMSignalList == true)
            {
                //MainForm.Instance.RemoveAllPSMSensorDetect();
            }

            if (bRemoveSecuritySignalList == true)
            {
                //MainForm.Instance.RemoveAllSecuritySensorDetect();
            }
        }

        public void LoadSignalState()
        {
            int nSiteID = UnE.SOP.ProxySOP.Instance.SiteID;
            string szValue1 = RegUtil.ReadRegValue("SDMS", "ReciveFireSignal", nSiteID);
            if( szValue1.ToLower() == "false")
            {
                m_bReciveFireSignal = false;
            }

            string szValue2 = RegUtil.ReadRegValue("SDMS", "RecivePSMSignal", nSiteID);
            if (szValue2.ToLower() == "false")
            {
                m_bRecivePSMSignal = false;
            }

            string szValue3 = RegUtil.ReadRegValue("SDMS", "ReciveSecuritySignal", nSiteID);
            if (szValue3.ToLower() == "false")
            {
                m_bReciveSecuritySignal = false;
            }

            string szValue4 = RegUtil.ReadRegValue("SDMS", "ReciveEarthquakeSignal", nSiteID);
            if (szValue4.ToLower() == "false")
            {
                m_bReciveEarthquakeSignal = false;
            }

            string szValue5 = RegUtil.ReadRegValue("SDMS", "ReciveBlackoutSignal", nSiteID);
            if (szValue5.ToLower() == "false")
            {
                m_bReciveBlackoutSignal = false;
            }

            string szValue6 = RegUtil.ReadRegValue("SDMS", "ReciveStrongWindSignal", nSiteID);
            if (szValue6.ToLower() == "false")
            {
                m_bReciveStrongWindSignal = false;
            }

            string szValue7 = RegUtil.ReadRegValue("SDMS", "ReciveFirewallSignal", nSiteID);
            if (szValue7.ToLower() == "false")
            {
                m_bReciveFirewallSignal = false;
            }
        }



        public void SaveCCTVState()
        {            
        }

        public void SaveViewState()
        {
        }

        public void SaveToolbarState()
        {
        }
	}



    public class CCTVState
    {

    }

    public class ViewState
    {

    }

    public class ToolBarState
    {

    }
}