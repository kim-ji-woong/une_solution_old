using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnE.Spatial;
using UnE.Sensor;
using System.Collections;

namespace libSensorProcess
{
    /// <summary>
    /// 함수이름이 Invoke로 끝나면 함수구현시 구현부를 Invoke로 감싸도록 한다.
    /// </summary>
    public interface IProcessOwner
    {
        bool UsePopupSensorOn { get; }
        bool OpenSOPOnDetectSensor { get; }
        void OpenSOP(EquipmentZone equipZone, DateTime sopTime, ProcessIF process);
        void AddSensorDectectInvoke(ProcessIF process, bool bAddSelect, bool bCallSelect);
        void ShowEvacCircleInvoke(int nLevel);
        // 새로운 센서신호가 탐지되었음을 알린다.
        void ShowSensorAlarmInvoke(ProcessIF process, ReactionType notifyType);
        // 기존에 탐지된 센서신호 가운데 특정 센서신호를 현재 화면에 나타나도록 한다.
        void SelectProcessInvoke(ProcessIF process, bool showDetectSensorTooltipCCTV, ArrayList arrCCTVs, int nSensorZoneID);
        // 탐지된 센서신호를 실제 재난상황으로 판단한다.
        // Return 값 : CCTV List
        ArrayList ConfirmDisasterInvoke(ProcessIF process, bool showDetectSensorTooltipCCTV, int nSensorZoneID, ReactionType notifyType, int nAlarmLevel);
        void EndNotifyProcessInvoke(ReactionLog log);
        void SetPSMDetectModeInvoke(ReactionLog log);
        void SetNormalModeInvoke(ReactionLog log);
        void SetFireDetectModeInvoke(ReactionLog log);
        void SetSecurityDetectModeInvoke(ReactionLog log);
        void SetEarthquakeDetectModeInvoke(ReactionLog log);
        void NotifyProcessInvoke(ReactionLog log);
        //void BeginNotifyProcessInvoke(ReactionLog log);
        void RunSOPInvoke(ReactionLog log);
        void RunNCancelSOPInvoke(ReactionLog log);
        void FinishSOPInvoke(ReactionLog log);
        void IgnoreSOPInvoke(ReactionLog log);
        void AddLogMessageInvoke(ReactionLog log);
    }
}
