using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using IntegratedManagement4;
using SDMS;

namespace SOPMonitoringSystem
{
    internal class ProxyMessenger
    {
        private static ProxyMessenger m_instance = null;

        public static ProxyMessenger Instance
        {
            get
            {
                if (m_instance == null)
                    m_instance = new ProxyMessenger();

                return m_instance;
            }
        }

        private bool m_earthquakeEventIsFinished = false;

        public bool EarthquakeEventIsFinished
        {
            get { return m_earthquakeEventIsFinished; }
            set { m_earthquakeEventIsFinished = value; }
        }

        private ProxyMessenger()
        {
        }

        public void SetCheckPosition(string strDisasterName, string strPositionName, string strBroadcastName, string strBuildingID, float fFloorIndex, int nActionStepHistoryID, int nIconID, int nPSMDistance, string strPSMMaterial, float x, float y, float z, int nZoneID, bool isChecked)
        {
            ArrayList arrDatas = MakeParam(InternalMessage.SopSimulatorToSdms.SET_CHECK_POSITION, strDisasterName, strPositionName, strBroadcastName, strBuildingID, fFloorIndex, nActionStepHistoryID, nIconID, nPSMDistance, strPSMMaterial, x, y, z, nZoneID, isChecked);
            NetworkWebManager.Instance.ClientProviderInternal.SendInternalData(arrDatas);
        }

        public void SetLastPosition(string strDisasterName, string strPositionName, string strBroadcastName, string strBuildingID, float fFloorIndex, int nActionStepHistoryID, int nIconID, int nPSMDistance, string strPSMMaterial, float x, float y, float z, int nZoneID)
        {
            ArrayList arrDatas = MakeParam(InternalMessage.SopSimulatorToSdms.SET_LAST_POSITION, strDisasterName, strPositionName, strBroadcastName, strBuildingID, fFloorIndex, nActionStepHistoryID, nIconID, strPSMMaterial, x, y, z, nZoneID);
            NetworkWebManager.Instance.ClientProviderInternal.SendInternalData(arrDatas);
        }

        public void RemoveDisasterPos()
        {
            ArrayList arrDatas = MakeParam(InternalMessage.SopSimulatorToSdms.REMOVE_DISASTER_POS);
            NetworkWebManager.Instance.ClientProviderInternal.SendInternalData(arrDatas);
        }

        public void NullLastPosition()
        {
            ArrayList arrDatas = MakeParam(InternalMessage.SopSimulatorToSdms.NULL_LAST_POSITION);
            NetworkWebManager.Instance.ClientProviderInternal.SendInternalData(arrDatas);
        }

        public void Update3DView()
        {
            ArrayList arrDatas = MakeParam(InternalMessage.SopSimulatorToSdms.UPDATE_3D_VIEW);
            NetworkWebManager.Instance.ClientProviderInternal.SendInternalData(arrDatas);
        }

        public void ToggleMinimumWindow()
        {
            ArrayList arrDatas = MakeParam(InternalMessage.SopSimulatorToSdms.TOGGLE_MINIMUM_WINDOW);
            NetworkWebManager.Instance.ClientProviderInternal.SendInternalData(arrDatas);
        }

        public void EarthquakeEvent(int nIntensity, float fMagnitude, string strPosition, bool isRealMode)
        {
            ArrayList arrDatas = MakeParam(InternalMessage.SopSimulatorToSdms.EARTHQUAKE_EVENT, nIntensity, fMagnitude, strPosition, isRealMode);
            NetworkWebManager.Instance.ClientProviderInternal.SendInternalData(arrDatas);
        }

        // 지진신호가 끝났는지 묻고 그 응답을 기다린다.
        public void Ask_EarthquakeEventIsFinished()
        {
            ArrayList arrDatas = MakeParam(InternalMessage.SopSimulatorToSdms.EARTHQUAKE_EVENT_IS_FINISHED);
            NetworkWebManager.Instance.ClientProviderInternal.SendInternalData(arrDatas);
        }

        public void OnCheckPositionEnd(bool bResult)
        {
            ArrayList arrDatas = MakeParam(InternalMessage.SopSimulatorToSdms.ON_CHECK_POSITION_END, bResult);
            NetworkWebManager.Instance.ClientProviderInternal.SendInternalData(arrDatas);
        }

        public void ShowBuildingCollapse(string szBuildingID, string szDisplayName)
        {
            ArrayList arrDatas = MakeParam(InternalMessage.SopSimulatorToSdms.SHOW_BUILDING_COLLAPSE, szBuildingID, szDisplayName);
            NetworkWebManager.Instance.ClientProviderInternal.SendInternalData(arrDatas);
        }

        public void CloseBuildingCollapse(string szBuildingID)
        {
            ArrayList arrDatas = MakeParam(InternalMessage.SopSimulatorToSdms.CLOSE_BUILDING_COLLAPSE, szBuildingID);
            NetworkWebManager.Instance.ClientProviderInternal.SendInternalData(arrDatas);
        }

        public void ToggleCCTV()
        {
            ArrayList arrDatas = MakeParam(InternalMessage.SopSimulatorToSdms.TOGGLE_CCTV);
            NetworkWebManager.Instance.ClientProviderInternal.SendInternalData(arrDatas);
        }

        public void OpenSopOnSensorDetect(bool loadSOP)
        {
            ArrayList arrDatas = MakeParam(InternalMessage.SopSimulatorToSdms.OPEN_SOP_ON_SENSOR_DETECT, loadSOP);
            NetworkWebManager.Instance.ClientProviderInternal.SendInternalData(arrDatas);
        }

        private ArrayList MakeParam(params object[] args)
        {
            ArrayList arrDatas = new ArrayList();
            arrDatas.Add(InternalMessage.SOP_SIMULATOR_2_SDMS);

            foreach (object arg in args)
            {
                arrDatas.Add(arg);
            }

            return arrDatas;
        }
    }
}
