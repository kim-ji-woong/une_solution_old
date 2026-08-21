using SAP.Middleware.Connector;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using TeamEditor.Model.Sop.Team;


namespace ERPReadServer
{
    public partial class FormMain : Form
    {
        private XMLManager m_xmlManager = new XMLManager();
        private TeamEditor.DAL.DataManager m_teamDataManager = null;
        private SOPManager.DAL.DataManager m_sopDataManager = null;
        DataManager m_dataManager = null;

        private string DestinationConfigName = null;
        private string MemberFunctionName = null;
        private string MemberParamName = null;
        private string MemberParamValue = null;
        private string MemberTableName = null;
        private string TeamFunctionName = null;
        private string TeamParamName = null;
        private string TeamParamValue = null;
        private string TeamTableName = null;

        private System.Timers.Timer m_timer = null;
        private bool m_bTimerChk = false;                           // 이미 타이머 실행 유무 체크
        private DateTime m_dtLast = new DateTime();

        public FormMain()
        {
            InitializeComponent();
            
            // WSOP DB 매니저
            InitDBSet();

            // ERP 기본 설정
            initERPConfig();

            // 1분에 한번씩 동작
            m_timer = new System.Timers.Timer();
            m_timer.Interval = 1000 * 60;       // 1분(1초 * 60) = 1분
            m_timer.Elapsed += new ElapsedEventHandler(timerReload_Elapsed);
            timerReload_Elapsed(null, null);

            m_timer.Start();
        }

        private void timerReload_Elapsed(object sender, ElapsedEventArgs e)
        {
            string strErrorMessage = "";
            string strMemberXMLPath = "(RFC) 사원정보.XML";
            string strTeamXMLPath = "(RFC) 부서정보.XML";

            // 지난 로그 삭제
            DateTime dtNow = DateTime.Now;
            if ((dtNow - m_dtLast).TotalDays >= 1)
            {
                Logger.Instance.RemoveOldLogs();
                m_dtLast = DateTime.Now;
            } 
            else
            {
                // 하루에 최초 한번 동작
                return;
            }

            // 타이머 실행 유무 체크
            if (m_bTimerChk == true)
                return;

            m_bTimerChk = true;                 // 타이머 실행 중 체크

            DataTable dtTeam = null;
            DataTable dtMember = null;

            
            if (LoadERPTable(out dtTeam, out dtMember) == false)
            {
                m_bTimerChk = false;
                return;
            }
            
            // TODO: 임시 테스트 XML 데이터 읽어오기
            /*
            dtTeam = m_xmlManager.ReadTeamXML(strTeamXMLPath, out strErrorMessage);
            dtMember = m_xmlManager.ReadMemberXML(strMemberXMLPath, out strErrorMessage);
            */

            // ERP 데이터로 DB 업데이트
            if (m_dataManager.ReflashERPData(dtTeam, dtMember, out strErrorMessage) == false)
            {
                Logger.Instance.Write("[ERROR] ReflashERPData is fail : " + strErrorMessage);
            }

            m_bTimerChk = false;
        }

        private void initERPConfig()
        {
            // 사원정보 연동 설정
            DestinationConfigName = ConfigurationManager.AppSettings.Get("DestinationConfigName");
            if (DestinationConfigName == null || DestinationConfigName.Length == 0)
                DestinationConfigName = "DEST_SAP_DEV";

            MemberFunctionName = ConfigurationManager.AppSettings.Get("MemberFunctionName");
            if (MemberFunctionName == null || MemberFunctionName.Length == 0)
                MemberFunctionName = "ZHR_INTER_M017";

            MemberParamName = ConfigurationManager.AppSettings.Get("MemberParamName");
            if (MemberParamName == null || MemberParamName.Length == 0)
                MemberParamName = "I_BUKRS";

            MemberParamValue = ConfigurationManager.AppSettings.Get("MemberParamValue");
            if (MemberParamValue == null || MemberParamValue.Length == 0)
                MemberParamValue = "1100";

            MemberTableName = ConfigurationManager.AppSettings.Get("MemberTableName");
            if (MemberTableName == null || MemberTableName.Length == 0)
                MemberTableName = "IT_ZHR01";


            // 부서정보 연동 설정
            TeamFunctionName = ConfigurationManager.AppSettings.Get("TeamFunctionName");
            if (TeamFunctionName == null || TeamFunctionName.Length == 0)
                TeamFunctionName = "ZHR_INTER_M012";

            TeamParamName = ConfigurationManager.AppSettings.Get("TeamParamName");
            if (TeamParamName == null || TeamParamName.Length == 0)
                TeamParamName = "I_BUKRS";

            TeamParamValue = ConfigurationManager.AppSettings.Get("TeamParamValue");
            if (TeamParamValue == null || TeamParamValue.Length == 0)
                TeamParamValue = "1100";

            TeamTableName = ConfigurationManager.AppSettings.Get("TeamTableName");
            if (TeamTableName == null || TeamTableName.Length == 0)
                TeamTableName = "T_ORG";

            IDestinationConfiguration destinationConfig = null;
            bool destinationIsInialised = false;

            if (!destinationIsInialised)
            {
                destinationConfig = new SAPDestinationConfig();
                destinationConfig.GetParameters(DestinationConfigName);

                if (RfcDestinationManager.TryGetDestination(DestinationConfigName) == null)
                {
                    RfcDestinationManager.RegisterDestinationConfiguration(destinationConfig);
                    destinationIsInialised = true;
                }
            }
        }

        private void InitDBSet()
        {
            string strSiteID = ConfigurationManager.AppSettings.Get("SITE_ID");
            if (strSiteID == null || strSiteID.Length == 0)
                strSiteID = "10";

            string strDBName = ConfigurationManager.AppSettings.Get("DB_NAME");
            if (strDBName == null || strDBName.Length == 0)
                strDBName = "WSOP_10";

            string strDBType = ConfigurationManager.AppSettings.Get("DB_TYPE");
            if (strDBType == null || strDBType.Length == 0)
                strDBType = "0";

            string strWebServerURL = ConfigurationManager.AppSettings.Get("WebServerURL");
            if (strWebServerURL == null || strWebServerURL.Length == 0)
                strWebServerURL = "http://127.0.0.1:808";

            int nSiteID, nDBType;
            int.TryParse(strSiteID.Trim(), out nSiteID);
            int.TryParse(strDBType.Trim(), out nDBType);

            m_teamDataManager = new TeamEditor.DAL.DataManager(strDBName, nDBType, nSiteID, strWebServerURL);
            m_sopDataManager = new SOPManager.DAL.DataManager(strDBName, nDBType, nSiteID, strWebServerURL);
            m_dataManager = new DataManager(m_teamDataManager, m_sopDataManager);
        }

        private bool LoadERPTable(out DataTable dtTeam, out DataTable dtMember)
        {
            SAPConnectorInterface sapConnectorInterface = new SAPConnectorInterface();
            dtTeam = null;
            dtMember = null;

            if (sapConnectorInterface.TestConnection(DestinationConfigName) == false)
                return false;

            dtTeam = sapConnectorInterface.RetrieveCustomers(DestinationConfigName, TeamFunctionName, TeamTableName, TeamParamName, TeamParamValue);
            if (dtTeam == null)
                return false;

            dtMember = sapConnectorInterface.RetrieveCustomers(DestinationConfigName, MemberFunctionName, MemberTableName, MemberParamName, MemberParamValue);
            if (dtTeam == null)
                return false;

            return true;
        }
    }
}
