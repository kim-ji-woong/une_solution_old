using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

using Sections;
using UnE.SOP.Tree;
using UnE.SOP.History;
using UnE.SOP.Log;
using UnE.SOP.Data;
using UnE.SOP.Sections;
using UnE.SOP.Workstate;

namespace UnE
{
    namespace SOP
    {
        
        
        public class ProxySOP
        {
            private static ProxySOP m_Instance = new ProxySOP();
            public static UnE.SOP.ProxySOP Instance
            {
                get { return m_Instance; }               
            }

            /// <summary>
            /// CCTV를 별도의 LibCCTV를 이용하는지 여부 , 2016-10-17이후 항상 True임. 내부 CCTV는 사용하지 않음
            /// </summary>
            private bool m_bShowCCTVForm = true;
            public bool ShowCCTVForm
            {
                get { return m_bShowCCTVForm; }
                set { m_bShowCCTVForm = value; }
            }

            private int m_nSimulatorMonitor = 1;
            public int SimulatorMonitor
            {
                get { return m_nSimulatorMonitor; }
                set { m_nSimulatorMonitor = value; }
            }

            private int m_nSDMSMonitor = 2;
            public int SDMSMonitor
            {
                get { return m_nSDMSMonitor; }
                set { m_nSDMSMonitor = value; }
            }

            private int m_nMissionListMonitor = 3;
            public int MissionListMonitor
            {
                get { return m_nMissionListMonitor; }
                set { m_nMissionListMonitor = value; }
            }

            private int m_nCCTVMontior = 3;
            public int CCTVMontior
            {
                get { return m_nCCTVMontior; }
                set { m_nCCTVMontior = value; }
            }                        

            private System.Windows.Forms.Form m_InvokeForm = null;
            public System.Windows.Forms.Form InvokeForm
            {
                get { return m_InvokeForm; }
                set { m_InvokeForm = value; }
            }

            private DBUtility.WebDBManager m_dbMgr = null;
            public DBUtility.WebDBManager DBManager
            {
                get { return m_dbMgr; }
                set { m_dbMgr = value; }
            }

            private ProxySOP()
            {
            }

            public bool IsOK()
            {
                if (m_dbMgr == null)
                    return false;

                if (m_HistoryContainer == null)
                    return false;

                if (m_SOPDataContainer == null)
                    return false;

                if (m_WorkflowContainer == null)
                    return false;

                if (m_SOPTreeContainer == null)
                    return false;

                if (m_SOPLogContainer == null)
                    return false;

                if (m_InvokeForm == null)
                    return false;

                if (m_PageContainer == null)
                    return false;

                //if (m_SOPDisasterContainer == null)
                //    return false;

                return true;
            }

            private ISOPPageContainer m_PageContainer = null;
            public ISOPPageContainer PageContainer
            {
                get { return m_PageContainer; }
                set { m_PageContainer = value; }
            }

            private ISOPHistoryContainer m_HistoryContainer = null;
            public UnE.SOP.History.ISOPHistoryContainer HistoryContainer
            {
                get { return m_HistoryContainer; }
                set { m_HistoryContainer = value; }
            }
     
            private ISOPDataContainer m_SOPDataContainer = null;
	        public UnE.SOP.Data.ISOPDataContainer SOPDataContainer
	        {
		        get { return m_SOPDataContainer; }
		        set { m_SOPDataContainer = value; }
	        }

            private IWorkflowContainer m_WorkflowContainer = null;
            public UnE.SOP.Workstate.IWorkflowContainer WorkflowContainer
            {
                get { return m_WorkflowContainer; }
                set { m_WorkflowContainer = value; }
            }
            
            private ISOPTreeContainer m_SOPTreeContainer = null;
            public UnE.SOP.Tree.ISOPTreeContainer SOPTreeContainer
            {
                get { return m_SOPTreeContainer; }
                set { m_SOPTreeContainer = value; }
            }
            
            private ISOPLogContainer m_SOPLogContainer = null;
            public UnE.SOP.Log.ISOPLogContainer SOPLogContainer
            {
                get { return m_SOPLogContainer; }
                set { m_SOPLogContainer = value; }
            }

            private ISOPContainer m_SOPContainer = null;
            public ISOPContainer SOPContainer
            {
                get { return m_SOPContainer; }
                set { m_SOPContainer = value; }
            }
            
            private IDisasterContainer m_SOPDisasterContainer = null;
            public UnE.SOP.IDisasterContainer SOPDisasterContainer
            {
                get { return m_SOPDisasterContainer; }
                set { m_SOPDisasterContainer = value; }
            }
            
            /// <summary>
            /// 로그인된 사용자 SOPGenUserID
            /// </summary>
            private int m_nSOPGenUserID = -1;
            public int SOPGenUserID
            {
                get { return m_nSOPGenUserID; }
                set { m_nSOPGenUserID = value; }
            }

            /// <summary>
            /// 로그인된 사용자 CompanyMemberName
            /// </summary>
            private string m_nSOPUserName = "";
            public string SOPUserName
            {
                get { return m_nSOPUserName; }
                set { m_nSOPUserName = value; }
            }

            /// <summary>
            /// 로그인된 사용자 SOPGenUserLevel
            /// </summary>
            private int m_nSOPUserLevel = -1;
            public int SOPUserLevel
            {
                get { return m_nSOPUserLevel; }
                set { m_nSOPUserLevel = value; }
            }

            /// <summary>
            /// 실제모드(true), 훈련모드(false) 
            /// </summary>
            private bool m_bRealMode = false;
            public bool RealMode
            {
                get { return m_bRealMode; }
                set { m_bRealMode = value; }
            }

            /// <summary>
            /// 평일(true), 야간휴일 모드(false)
            /// </summary>
            private bool m_bNormalMode = false;
            public bool NormalMode
            {
                get { return m_bNormalMode; }
                set { m_bNormalMode = value; }
            }

            /// <summary>
            /// 등록모드(true), 미등록모드(false) 
            /// </summary>
            private bool m_bRegisterMode = false;
            public bool RegisterMode
            {
                get { return m_bRegisterMode; }
                set { m_bRegisterMode = value; }
            }

            /// <summary>
            /// 훈련모드 워터마크 이미지 사용여부
            /// </summary>
            private bool m_bUseWaterMark = true;
            public bool UseWaterMark
            {
                get { return m_bUseWaterMark; }
                set { m_bUseWaterMark = value; }
            }

            /// <summary>
            /// 연습용 모드인지 여부
            /// </summary>
            private bool m_isSimulationMode = false;
            public bool SimulationMode
            {
                get { return m_isSimulationMode; }
                set { m_isSimulationMode = value; }
            }

            /// <summary>
            /// 지정 Site ID , 1이면 삼천포, 2이면 영흥 이후 추가
            /// </summary>
            private int m_nSiteID = 1;
            public int SiteID
            {
                get { return m_nSiteID; }
                set { m_nSiteID = value; }
            }

            /// <summary>
            /// 발전소 이름
            /// </summary>
            private string m_strSiteName = "";
            public string SiteName
            {
                get { return m_strSiteName; }
                set { m_strSiteName = value; }
            }

            /// <summary>
            /// 화재탐지시 실행중인 SOP가 없는경우 연결된 SOP 자동 열기
            /// </summary>
            private bool bOpenSOPOnFireDetect = true;
            public bool OpenSOPOnFireDetect
            {
                get { return bOpenSOPOnFireDetect; }
                set { bOpenSOPOnFireDetect = value; }
            }

            /// <summary>
            /// SOP에서 문자 전송시 확인 여부
            /// </summary>
            private bool bConfirmSendSMS = false;
            public bool ConfirmSendSMS
            {
                get { return bConfirmSendSMS; }
                set { bConfirmSendSMS = value; }
            }

            /// <summary>
            /// SOP에서 문자 전송 확인에서 yes to all 선택 여부
            /// </summary>
            private bool bConfirmSMSAll = false;
            public bool ConfirmSMSAll
            {
                get { return bConfirmSMSAll; }
                set { bConfirmSMSAll = value; }
            }

            /// <summary>
            /// SDMS에서 PSM관련 기능 사용 여부
            /// </summary>
            private bool bUsePSM = true;
            public bool UsePSM
            {
                get { return bUsePSM; }
                set { bUsePSM = value; }
            }

            /// <summary>
            /// SDMS에서 Intrusion관련 기능 사용 여부
            /// </summary>
            private bool bUseIntrusion = true;
            public bool UseIntrusion
            {
                get { return bUseIntrusion; }
                set { bUseIntrusion = value; }
            }

            /// <summary>
            /// SDMS에서 지진관련 기능 사용 여부
            /// </summary>
            private bool bUseEarthquake = false;
            public bool UseEarthquake
            {
                get { return bUseEarthquake; }
                set { bUseEarthquake = value; }
            }

            /// <summary>
            /// SDMS에서 실내뷰 사용 여부
            /// </summary>
            private bool bUse2D = true;
            public bool Use2D
            {
                get { return bUse2D; }
                set { bUse2D = value; }
            }





            //public bool UseCloseSOPWaitInputTime { get; set; }

            //public int CloseSOPWaitInputTime { get; set; }

            //public bool UseCloseSOPSensorReset { get; set; }

            //public bool UseCloseSOPSensorResetWaitTime { get; set; }

            //public int CloseSOPSensorResetWaitTime { get; set; }


            private Dictionary<string, SOPCloseOption> optionSOPAutoCloseSet = new Dictionary<string, SOPCloseOption>();
            public Dictionary<string, SOPCloseOption> OptionSOPAutoCloseSet
            {
                get { return optionSOPAutoCloseSet; }
                set { optionSOPAutoCloseSet = value; }
            }


            //public bool UseCloseSOPWaitInputTime { get; set; }

            //public int CloseSOPWaitInputTime { get; set; }

            //public bool UseCloseSOPSensorReset { get; set; }

            //public bool UseCloseSOPSensorResetWaitTime { get; set; }

            //public int CloseSOPSensorResetWaitTime { get; set; }
        }

        
    }
}
