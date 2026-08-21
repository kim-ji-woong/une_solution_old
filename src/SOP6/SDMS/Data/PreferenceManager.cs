using DBUtility2;

namespace SDMS
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

        private void SaveSignalState()
        {
            int nSiteID = UnE.SOP.ProxySOP.Instance.SiteID;
            
            bool bReciveFireSignal = true;
            bool bRecivePSMSignal = true;
            bool bReciveSecuritySignal = true;
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
            
            bool bRequestReactionLogList = false;
            bool bRemoveFireSignalList = false;
            bool bRemovePSMSignalList = false;
            bool bRemoveSecuritySignalList = false;

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
          
            RegUtil.WriteRegValue("SDMS", "ReciveFireSignal", m_bReciveFireSignal.ToString(), nSiteID);
            RegUtil.WriteRegValue("SDMS", "RecivePSMSignal", m_bRecivePSMSignal.ToString(), nSiteID);
            RegUtil.WriteRegValue("SDMS", "ReciveSecuritySignal", m_bReciveSecuritySignal.ToString(), nSiteID);

            if (bRequestReactionLogList == true)
            {
                NetworkWebManager.Instance.SendRequestReactionLogList();
            }

            if( bRemoveFireSignalList == true)
            {
                FormMain.Instance.RemoveAllFireSensorDetect();
            }

            if( bRemovePSMSignalList == true)
            {
                FormMain.Instance.RemoveAllPSMSensorDetect();
            }

            if (bRemoveSecuritySignalList == true)
            {
                FormMain.Instance.RemoveAllSecuritySensorDetect();
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