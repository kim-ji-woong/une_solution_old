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
        //void RunTrainingModeSOPSimulator(int nSensorID, int nEquipZoneID);
        void IgnoreSOP(int nSensorHistoryID);
        void CompleteLoading();

        void ShowSOPSimulator();
        void HideSOPSimulator();
        bool IsVisibleSOPSimulator();

        void ShowMissionStatus();
        void HideMissionStatus();
        bool IsVisibleMissionStatus();

        void EnableCCTV();

        // 화재신호 감지로 인한 SOP 로딩
        void OpenSOP_Fire(int nZoneID, DateTime sopTime, int nSensorID, int nSensorHistoryID);
        // 누출신호 감지로 인한 SOP 로딩
        void OpenSOP_PSM(int nEquipZoneID, DateTime sopTime, int nSensorID, int nSensorHistoryID);
        // 방범신호 감지로 인한 SOP 로딩
        void OpenSOP_Security(int nEquipZoneID, DateTime sopTime, int nSensorID, int nSensorHistoryID, ISensor targetSensor);

        void OnAfterLoadingCCTV();

        bool OnlySDMS();

        // 센서 종료 신호
        void SensorClose(int nSensorID, int nSensorZoneHistoryID);
    }
}
