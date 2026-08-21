using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnE.Spatial;
using UnE.Sensor;
using UnE.Util.Unity;

namespace SDMS
{
    public interface IProxyMessenser
    {
        void RunSOPSimulator();
        void IgnoreSOP(int nSensorHistoryID);
        void CompleteLoading();

        void ShowSOPSimulator();
        void HideSOPSimulator();
        void ShowSOPSimulatorIfInvisible();
        // Visible 상태이면 Hide(), Invisible 상태이면 Show() 시킨다.
        void ShowHideSOPSimulator();

        // Visible 상태이면 Hide(), Invisible 상태이면 Show() 시킨다.
        void ShowHideMissionStatus();
        
        // 화재신호 감지로 인한 SOP 로딩
        void OpenSOP_Fire(int nZoneID, DateTime sopTime, int nSensorZoneID, int nSensorHistoryID);
        // 누출신호 감지로 인한 SOP 로딩
        void OpenSOP_PSM(int nEquipZoneID, DateTime sopTime, int nSensorZoneID, int nSensorHistoryID);
        // 방범신호 감지로 인한 SOP 로딩
        void OpenSOP_Security(int nEquipZoneID, DateTime sopTime, int nSensorZoneID, int nSensorHistoryID, int nSensorType);

        void OnAfterLoadingCCTV();

        // 센서 종료 신호
        void SensorClose(int nSensorZoneID, int nSensorZoneHistoryID);

        // Toggle CCTV Viewer
        void EnableCCTV();

        void ToggleSOPBulletin();

        void AddLastHistoryDisasterPoistion(string strDisasterName, string strPositionName, string strBroadcastName, string strBuildingID, float fFloorIndex, int nActionStepHistoryID, int nIconID, int nPSMDistance, string strPSMMaterial, float x, float y, float z, int nZoneID);
        void SetWorkFlowOptionPosition(string strPositionName);

        // 지진신호가 끝났는지 여부를 알려준다.
        void Reply_EarthquakeEventIsFinished(bool isFinished);

        //같은 Equipzone에 들어있는 센서 그룹에 대해 SDMS에 먼저 들어온 신호와 나중에 들어온 신호가 있을 경우 SOP에 등록하기 위한 메서드
        //SOP에서 CheckSensorClose에서 thread로 상황 해제 체크하므로 해제는 별도 처리 하지 않음. 
        void SameSensorGroupRunning(int sleepProcessSensorHistoryID, int activeProcessSensorHistoryID);
        void SOPPositionName(string strPositionName);

        // 통합관리자에게 보내는 메시지
        void SetViewProcessID(List<int> processIDs);
    }
}
