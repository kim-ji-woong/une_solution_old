using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using IntegratedManagement4;
using UnE.Sensor;

namespace SDMS
{
    internal class ProxyMessenger : IProxyMessenser
    {
        public void RunSOPSimulator()
        {
            ArrayList arrDatas = MakeParam(InternalMessage.SdmsToSopSimulator.RUN_SOP_SIMULATOR);
            NetworkWebManager.Instance.ClientProviderInternal.SendData(TCP_ID.INTERNAL_MESSAGE, arrDatas);
        }

        public void IgnoreSOP(int nSensorHistoryID)
        {
            ArrayList arrDatas = MakeParam(InternalMessage.SdmsToSopSimulator.IGNORE_SOP, nSensorHistoryID);
            NetworkWebManager.Instance.ClientProviderInternal.SendData(TCP_ID.INTERNAL_MESSAGE, arrDatas);
        }

        public void CompleteLoading()
        {
            ArrayList arrDatas = MakeParam(InternalMessage.SdmsToSopSimulator.COMPLETE_LOADING);
            NetworkWebManager.Instance.ClientProviderInternal.SendData(TCP_ID.INTERNAL_MESSAGE, arrDatas);
        }

        public void ShowSOPSimulator()
        {
            ArrayList arrDatas = MakeParam(InternalMessage.SdmsToSopSimulator.SHOW_SOP_SIMULATOR);
            NetworkWebManager.Instance.ClientProviderInternal.SendData(TCP_ID.INTERNAL_MESSAGE, arrDatas);
        }

        public void HideSOPSimulator()
        {
            ArrayList arrDatas = MakeParam(InternalMessage.SdmsToSopSimulator.HIDE_SOP_SIMULATOR);
            NetworkWebManager.Instance.ClientProviderInternal.SendData(TCP_ID.INTERNAL_MESSAGE, arrDatas);
        }

        public void ShowSOPSimulatorIfInvisible()
        {
            ArrayList arrDatas = MakeParam(InternalMessage.SdmsToSopSimulator.SHOW_SOP_SIMULATOR_IF_INVISIBLE);
            NetworkWebManager.Instance.ClientProviderInternal.SendData(TCP_ID.INTERNAL_MESSAGE, arrDatas);
        }

        // Visible 상태이면 Hide(), Invisible 상태이면 Show() 시킨다.
        public void ShowHideSOPSimulator()
        {
            ArrayList arrDatas = MakeParam(InternalMessage.SdmsToSopSimulator.SHOW_HIDE_SOP_SIMULATOR);
            NetworkWebManager.Instance.ClientProviderInternal.SendData(TCP_ID.INTERNAL_MESSAGE, arrDatas);
        }

        // Visible 상태이면 Hide(), Invisible 상태이면 Show() 시킨다.
        public void ShowHideMissionStatus()
        {
            ArrayList arrDatas = MakeParam(InternalMessage.SdmsToSopSimulator.SHOW_HIDE_MISSION_STATUS);
            NetworkWebManager.Instance.ClientProviderInternal.SendData(TCP_ID.INTERNAL_MESSAGE, arrDatas);
        }

        // 화재신호 감지로 인한 SOP 로딩
        public void OpenSOP_Fire(int nZoneID, DateTime sopTime, int nSensorZoneID, int nSensorHistoryID)
        {
            // 이 작업은 SOPWebServer가 담당함
            /*ArrayList arrDatas = MakeParam(InternalMessage.SdmsToSopSimulator.OPEN_SOP_FIRE, nZoneID, sopTime.ToBinary(), nSensorZoneID, nSensorHistoryID);
            NetworkWebManager.Instance.ClientProviderInternal.SendData(TCP_ID.INTERNAL_MESSAGE, arrDatas);*/
        }

        // 누출신호 감지로 인한 SOP 로딩
        public void OpenSOP_PSM(int nEquipZoneID, DateTime sopTime, int nSensorZoneID, int nSensorHistoryID)
        {
            // 이 작업은 SOPWebServer가 담당함
            /*ArrayList arrDatas = MakeParam(InternalMessage.SdmsToSopSimulator.OPEN_SOP_PSM, nEquipZoneID, sopTime.ToBinary(), nSensorZoneID, nSensorHistoryID);
            NetworkWebManager.Instance.ClientProviderInternal.SendData(TCP_ID.INTERNAL_MESSAGE, arrDatas);*/
        }

        // 방범신호 감지로 인한 SOP 로딩
        public void OpenSOP_Security(int nEquipZoneID, DateTime sopTime, int nSensorZoneID, int nSensorHistoryID, int nSensorType)
        {
            // 이 작업은 SOPWebServer가 담당함
            /*ArrayList arrDatas = MakeParam(InternalMessage.SdmsToSopSimulator.OPEN_SOP_SECURITY, nEquipZoneID, sopTime.ToBinary(), nSensorZoneID, nSensorHistoryID, nSensorType);
            NetworkWebManager.Instance.ClientProviderInternal.SendData(TCP_ID.INTERNAL_MESSAGE, arrDatas);*/
        }

        public void OnAfterLoadingCCTV()
        {
            ArrayList arrDatas = MakeParam(InternalMessage.SdmsToSopSimulator.ON_AFTER_LOADING_CCTV);
            NetworkWebManager.Instance.ClientProviderInternal.SendData(TCP_ID.INTERNAL_MESSAGE, arrDatas);
        }

        // 센서 종료 신호
        public void SensorClose(int nSensorZoneID, int nSensorZoneHistoryID)
        {
            ArrayList arrDatas = MakeParam(InternalMessage.SdmsToSopSimulator.SENSOR_CLOSE, nSensorZoneID, nSensorZoneHistoryID);
            NetworkWebManager.Instance.ClientProviderInternal.SendData(TCP_ID.INTERNAL_MESSAGE, arrDatas);
        }

        // 센서 종료 신호
        public void SameSensorGroupRunning(int deActivateSensorHistoryID, int activeSensorHistoryID)
        {
            ArrayList arrDatas = MakeParam(InternalMessage.SdmsToSopSimulator.SAME_SENSORGROUP_RUNNING, deActivateSensorHistoryID, activeSensorHistoryID);
            NetworkWebManager.Instance.ClientProviderInternal.SendData(TCP_ID.INTERNAL_MESSAGE, arrDatas);
        }

        // Toggle CCTV Viewer
        public void EnableCCTV()
        {
            if (UnE.SOP.ProxySOP.Instance.ShowCCTVForm == true)
            {
                UnE.SOP.IDisasterContainer container = UnE.SOP.ProxySOP.Instance.SOPDisasterContainer;

                if (container != null)
                {
                    container.ShowCCTVForm(true);
                }
            }
            else
            {
                ArrayList arrDatas = MakeParam(InternalMessage.SdmsToSopSimulator.ENABLE_CCTV);
                NetworkWebManager.Instance.ClientProviderInternal.SendData(TCP_ID.INTERNAL_MESSAGE, arrDatas);
            }
        }

        public void ToggleSOPBulletin()
        {
            ArrayList arrDatas = MakeParam(InternalMessage.SdmsToSopSimulator.TOGGLE_SOP_BULLETIN);
            NetworkWebManager.Instance.ClientProviderInternal.SendData(TCP_ID.INTERNAL_MESSAGE, arrDatas);
        }

        public void AddLastHistoryDisasterPoistion(string strDisasterName, string strPositionName, string strBroadcastName, string strBuildingID, float fFloorIndex, int nActionStepHistoryID, int nIconID, int nPSMDistance, string strPSMMaterial, float x, float y, float z, int nZoneID)
        {
            ArrayList arrDatas = MakeParam(InternalMessage.SdmsToSopSimulator.ADD_LAST_HISTORY_DISASTER_POSITION, strDisasterName, strPositionName, strBroadcastName, strBuildingID, fFloorIndex, nActionStepHistoryID, nIconID, nPSMDistance, strPSMMaterial, x, y, z, nZoneID);
            NetworkWebManager.Instance.ClientProviderInternal.SendData(TCP_ID.INTERNAL_MESSAGE, arrDatas);
        }

        public void SetWorkFlowOptionPosition(string strPositionName)
        {
            ArrayList arrDatas = MakeParam(InternalMessage.SdmsToSopSimulator.SET_WORK_FLOW_OPTION_POSITION, strPositionName);
            NetworkWebManager.Instance.ClientProviderInternal.SendData(TCP_ID.INTERNAL_MESSAGE, arrDatas);
        }

        // 지진신호가 끝났는지 여부를 알려준다.
        public void Reply_EarthquakeEventIsFinished(bool isFinished)
        {
            ArrayList arrDatas = MakeParam(InternalMessage.SdmsToSopSimulator.EARTHQUAKE_EVENT_IS_FINISHED, isFinished);
            NetworkWebManager.Instance.ClientProviderInternal.SendData(TCP_ID.INTERNAL_MESSAGE, arrDatas);
        }

        public void SOPPositionName(string strPositionName)
        {
            ArrayList arrDatas = MakeParam(InternalMessage.SdmsToSopSimulator.SOP_POSITION_NAME, strPositionName);
            NetworkWebManager.Instance.ClientProviderInternal.SendData(TCP_ID.INTERNAL_MESSAGE, arrDatas);
        }

        public void SetViewProcessID(List<int> processIDs)
        {
            ArrayList arrDatas = new ArrayList();

            arrDatas.Add(InternalMessage.SDMS_2_MANAGER);
            arrDatas.Add(InternalMessage.SdmsToManager.CLEAR_N_ADD_PROCESS_IDS);
            arrDatas.Add(processIDs.Count);

            foreach (int nProcessID in processIDs)
            {
                arrDatas.Add(nProcessID);
            }

            NetworkWebManager.Instance.ClientProviderInternal.SendData(TCP_ID.INTERNAL_MESSAGE, arrDatas);
        }

        private ArrayList MakeParam(params object[] args)
        {
            ArrayList arrDatas = new ArrayList();
            arrDatas.Add(InternalMessage.SDMS_2_SOP_SIMULATOR);

            foreach (object arg in args)
            {
                arrDatas.Add(arg);
            }

            return arrDatas;
        }
    }
}
