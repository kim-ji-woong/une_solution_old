using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBUtility2;
using System.Windows.Forms;

namespace UnE.SOP
{
    public interface ISupervisor
    {
        void TouchSOP(int nActionStepHistory);
        // 새로운 SOP 시작
        void AddSOP(int nActionStepHistoryID, int nSensorZoneID, int nSensorZoneHistoryID);
        void RemoveSOP(int nActionStepHistoryID);
        void SensorClose(int nSensorZoneID, int nSensorZoneHistoryID);
        //먼저 돌고 있는 sop에 같은 sensor group이 있을 때 해당 로직처리를 위한 dictionary
        //1개 이상의 sensorgroup이 있을 경우를 위한 처리. 
        void RegisterSameSensorGroupRunning(int sensorHistoryID, int activeSensorHistoryID);
        void Start(WebDBManager dbMgr, ISOPScenarioManager scenarioManager, Control invokeCtrl, ISOPOwner sopOwner);
        void Stop();
        // 이미 strSOPFullPath에 해당하는 SOP가 실행중이다.
        // 이 상태에서 새로운 알람 신호가 들어왔는데, 위험단계를 바꿔가며 또다른 SOP를 로딩해야 하는지를 확인한다.
        // strSOPPath : 마지막 ActionStep을 제외한 [대분류/중분류/소분류] 3단계로만 되어있다.
        // Return 값 : strSOPPath가 바뀌었는가?
        bool CheckOpenSOP(List<UnE.SOP.Workstate.SOPScenario> currentScenarios, ref string strSOPPath, int nSensorZoneID, int nSensorZoneHistoryID, int nSensorType);
        // strSOPPath : 마지막 ActionStep을 제외한 [대분류/중분류/소분류] 3단계로만 되어있다.
        // 실제 SOP를 불러오기 위해서는 마지막 ActionStepName이 필요한데, nSensorType에 따라 적당한 ActionStepName을 추천해준다.
        string GetActionStepName(string strSOPPath, int nSensorType);
        // strSOPPath : 마지막 ActionStep을 제외한 [대분류/중분류/소분류] 3단계로만 되어있다.
        // 실제 SOP를 불러오기 위해서는 마지막 ActionStepName이 필요한데, nAlarmDepth에 맞는 ActionStepName을 리턴한다.
        // 만일, 알람 단계에 해당하는 ActionStep이 존재하지 않을경우 그보다 하위 단계의 ActionStep을 리턴한다.
        // 그마저도 없을 경우 한단계씩 상위 단계의 ActionStep을 찾아 리턴한다.
        string GetActionStepNameFromAlarmDepth(string strSOPPath, int nAlarmDepth);
        void SortDisasterActionSteps(DisasterInfo disaster);
    }

    public interface ISOPScenarioManager
    {
        UnE.SOP.Workstate.SOPScenario GetSOPScenario(int nActionStepHistory);
    }

    public interface ISOPOwner
    {
        UnE.SOP.Workstate.SOPScenario GetSOPScenario(int nActionStepHistoryID);
        void StopWorkflow(DateTime dtStop, bool noDBWrite, int nActionStepID, bool isRealMode);
        bool HasSOPControl(int nActionStepHistoryID);
        // 이미 실행중인 SOP(nActionStepHistoryID)와 같은 Disaster 안에서 위기경보 단계만 다른(nActionStepIndex) SOP를 로딩한다.
        bool LoadSOP(int nSensorType, int nEquipZoneID, DateTime timeStamp, int nSensorZoneID, int nSensorZoneHistoryID, int nActionStepHistoryID, int nActionStepIndex, string strSensorValue, bool runSOP);
        bool GetEquipmentZoneInfo(int nEquipZoneID, out string strEquipZoneName, out int nZoneID, out int nFloorIndex, out int nBuildingID);
        bool GetZoneInfo(int nZoneID, out string strZoneName, out int nFloorIndex, out int nBuildingID);
    }
}
