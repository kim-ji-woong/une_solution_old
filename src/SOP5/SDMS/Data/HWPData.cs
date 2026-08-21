using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDMS.Data
{
    public enum ReportMode
    {
        DetectFireAnalyze = 0,
        DetectFire = 1,
        ProcessFire = 2,
        ActionFire = 3,
        SMSFire = 4,
        DetectPSMAnalyze = 5,
        DetectPSM = 6,
        ProcessPSM = 7,
        ActionPSM = 8,
        SMSPSM = 9,
        //침입
        DetectIntrusionAnalyze = 10,
        DetectIntrusion = 11,
        ProcessIntrusion = 12,
        ActionIntrusion = 13,
        SMSIntrusion = 14,
        DisasterPrevention = 15 // 방재장비
    }

    public interface IParetoPage
    {
        //이미지 캡쳐
        void ControllCapture();
        void FileWriter();
        void SetHwpData();
        string GetHWPFileName();
        ReportMode GetReportMode();
    }
}
