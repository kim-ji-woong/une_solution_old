using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegratedManagement4
{
    public class InternalMessage
    {
        public const byte SOP_SIMULATOR_2_SDMS = 1;
        public const byte SDMS_2_SOP_SIMULATOR = 2;
        public const byte SDMS_2_MANAGER = 3;

        public class SdmsToSopSimulator
        {
            #region FunctionCall
            // void RunSOPSimulator()
            public const short RUN_SOP_SIMULATOR = 1;
            // void IgnoreSOP(int nSensorHistoryID)
            public const short IGNORE_SOP = 2;
            // void CompleteLoading()
            public const short COMPLETE_LOADING = 3;
            // void ShowSOPSimulator()
            public const short SHOW_SOP_SIMULATOR = 4;
            // void HideSOPSimulator()
            public const short HIDE_SOP_SIMULATOR = 5;
            // void ShowSOPSimulatorIfInvisible()
            public const short SHOW_SOP_SIMULATOR_IF_INVISIBLE = 6;
            // Visible 상태이면 Hide(), Invisible 상태이면 Show() 시킨다.
            // void ShowHideSOPSimulator()
            public const short SHOW_HIDE_SOP_SIMULATOR = 7;
            // Visible 상태이면 Hide(), Invisible 상태이면 Show() 시킨다.
            // void ShoHidewMissionStatus()
            public const short SHOW_HIDE_MISSION_STATUS = 8;
            // 화재신호 감지로 인한 SOP 로딩
            // void OpenSOP_Fire(int nZoneID, DateTime sopTime, int nSensorZoneID, int nSensorHistoryID)
            public const short OPEN_SOP_FIRE = 9;
            // 누출신호 감지로 인한 SOP 로딩
            // void OpenSOP_PSM(int nEquipZoneID, DateTime sopTime, int nSensorZoneID, int nSensorHistoryID)
            public const short OPEN_SOP_PSM = 10;
            // 방범신호 감지로 인한 SOP 로딩
            // void OpenSOP_Security(int nEquipZoneID, DateTime sopTime, int nSensorZoneID, int nSensorHistoryID, int nSensorType)
            public const short OPEN_SOP_SECURITY = 11;
            // void OnAfterLoadingCCTV()
            public const short ON_AFTER_LOADING_CCTV = 12;
            // 센서 종료 신호
            // void SensorClose(int nSensorZoneID)
            public const short SENSOR_CLOSE = 13;
            // Toggle CCTV Viewer
            // void EnableCCTV()
            public const short ENABLE_CCTV = 14;
            // void ToggleSOPBulletin()
            public const short TOGGLE_SOP_BULLETIN = 15;
            // void AddLastHistoryDisasterPoistion(string strDisasterName, string strPositionName, string strBroadcastName, string strBuildingID, float fFloorIndex, int nActionStepHistoryID, int nIconID, int nPSMDistance, string strPSMMaterial, float x, float y, float z, int nZoneID)
            public const short ADD_LAST_HISTORY_DISASTER_POSITION = 16;
            // void SetWorkFlowOptionPosition(string strPositionName)
            public const short SET_WORK_FLOW_OPTION_POSITION = 17;

            public const short SAME_SENSORGROUP_RUNNING = 18;
            // SOP 시작시 재난발생위치
            public const short SOP_POSITION_NAME = 19;

            #endregion FunctionCall

            #region ReplyFunction
            // 지진신호가 끝났는지 여부를 알려준다.
            // void Reply_EarthquakeEventIsFinished(bool isFinished)
            public const short EARTHQUAKE_EVENT_IS_FINISHED = 1000;
            #endregion
        }

        public class SopSimulatorToSdms
        {
            #region FunctionCall
            // void SetCheckPosition(string strDisasterName, string strPositionName, string strBroadcastName, string strBuildingID, float fFloorIndex, int nActionStepHistoryID, int nIconID, int nPSMDistance, string strPSMMaterial, float x, float y, float z, int nZoneID, bool isChecked)
            public const short SET_CHECK_POSITION = 1;
            // void SetLastPosition(string strDisasterName, string strPositionName, string strBroadcastName, string strBuildingID, float fFloorIndex, int nActionStepHistoryID, int nIconID, int nPSMDistance, string strPSMMaterial, float x, float y, float z, int nZoneID)
            public const short SET_LAST_POSITION = 2;
            // void RemoveDisasterPos()
            public const short REMOVE_DISASTER_POS = 3;
            // void NullLastPosition()
            public const short NULL_LAST_POSITION = 4;
            // void Update3DView()
            public const short UPDATE_3D_VIEW = 5;
            // void ToggleMinimumWindow()
            public const short TOGGLE_MINIMUM_WINDOW = 6;
            // void EarthquakeEvent(int nIntensity, float fMagnitude, string strPosition, bool isRealMode)
            public const short EARTHQUAKE_EVENT = 7;
            // 지진신호가 끝났는지 묻고 그 응답을 기다린다.
            // void Ask_EarthquakeEventIsFinished()
            public const short EARTHQUAKE_EVENT_IS_FINISHED = 8;
            // void OnCheckPositionEnd(bool bResult)
            public const short ON_CHECK_POSITION_END = 9;
            // void ShowBuildingCollapse(string szBuildingID, string szDisplayName)
            public const short SHOW_BUILDING_COLLAPSE = 10;
            // void CloseBuildingCollapse(string szBuildingID)
            public const short CLOSE_BUILDING_COLLAPSE = 11;
            // void ToggleCCTV()
            public const short TOGGLE_CCTV = 12;
            // void ShowWindow()
            public const short SHOW_WINDOW = 13;
            // void OpenSopOnSensorDetect()
            public const short OPEN_SOP_ON_SENSOR_DETECT = 14;
            #endregion
        }

        // SDMS에서 통합관리자에게
        public class SdmsToManager
        {
            #region FunctionCall
            // void AddProcessIDs(List<int> processIDs)
            public const short ADD_PROCESS_IDS = 1;
            // void RemoveProcessIDs(List<int> processIDs)
            public const short REMOVE_PROCESS_IDS = 2;
            // void ClearProcessIDs()
            public const short CLEAR_PROCESS_IDS = 3;
            // void ClearNAddProcessIDs()
            public const short CLEAR_N_ADD_PROCESS_IDS = 4;

            #endregion
        }

        public static int GetInternalServerPort(DBUtility2.WebDBManager dbMgr, int nSiteID)
        {
            int nDefaultPort = 5000;

            string strSQL = "Select PropertyValue from OptionSDMS where PropertyName = 'InternalMessagePort' and SiteID = " + nSiteID;
            System.Collections.ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count == 0)
                return nDefaultPort;

            int nPort = DBUtility2.WebDBManager.GetIntField(arrResult[0].ToString(), nDefaultPort);
            return nPort;
        }
    }
}
