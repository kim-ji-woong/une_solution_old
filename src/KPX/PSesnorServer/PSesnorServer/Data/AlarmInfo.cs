using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSensorServer
{
    public enum AlarmType
    {
        온도상승 = 1,
        온도하강 = 2,
        최고레벨 = 4,
        
        탱크유량증가 = 8,
        탱크유량감소 = 16,
        
        압력상승 = 256,
        압력하강 = 512,
        
        탱크배관유량증가 = 1024,
        탱크배관유량감소 = 2048
    }
      
    public class AlarmInfo
    {
        int m_nAlarmHistoryID = -1;
        int m_nAlarmType = 0;

        int m_nTankID = -1;
        int m_nPipeID = -1;

        private DateTime m_dtBegin;
        private DateTime m_dtEnd;
        
        public int AlarmHistoryID
        {
            get { return m_nAlarmHistoryID; }
            set { m_nAlarmHistoryID = value; }
        }    
        
        public int TankID 
        {
            get { return m_nTankID; }
            set { m_nTankID = value; }
        }

        public int PipeID 
        {
            get { return m_nPipeID;  }
            set { m_nPipeID = value; }
        }

        public DateTime BeginTime 
        {
            get { return m_dtBegin; }
            set { m_dtBegin = value; }
        }

        public DateTime EndTime
        {
            get { return m_dtEnd; }
            set { m_dtEnd = value; }
        }

        public int AlarmType 
        {
            get { return m_nAlarmType; }
            set { m_nAlarmType = value; }
        }

        private float m_fStandardValue;
        public float StandardValue
        {
            get { return  m_fStandardValue; }
            set { m_fStandardValue = value; }
        }

        private float m_fStandardRange;
        public float StandardRange 
        {
            get { return m_fStandardRange; }
            set { m_fStandardRange = value; }
        }

        private float m_fRealValue;
        public float RealValue 
        {
            get { return m_fRealValue; }
            set { m_fRealValue = value; }
        }

        private int m_nOccurrenceType = 0;
        public int OccurrenceType
        {
            get { return m_nOccurrenceType; }
            set { m_nOccurrenceType = value; }
        }

        private string m_strComment = "작업종료로 인한 알람 해제";
        public string Comment
        {
            get { return m_strComment; }
            set { m_strComment = value; }
        }
    }


    public class PipeAlarmOption
    {
        private int m_nID = -1;
        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        private int m_nPipeID = -1;
        public int PipeID
        {
            get { return m_nPipeID; }
            set { m_nPipeID = value; }
        }

        private int m_nLinkedPipeID = -1;
        public int LinkedPipeID
        {
            get { return m_nLinkedPipeID; }
            set { m_nLinkedPipeID = value; }
        }

        // 배관 압력 안정범위 비율(%)
        private float m_fPipeStableRatio;
        public float PipeStableRatio
        {
            get { return m_fPipeStableRatio; }
            set { m_fPipeStableRatio = value; }
        }

        // 배관 압력 안정범위 절대값 (kg/cm3)
        private float m_fPipeStableAbsolute;
        public float PipeStableAbsolute
        {
            get { return m_fPipeStableAbsolute; }
            set { m_fPipeStableAbsolute = value; }
        }

        // 배관 안정 범위 타입
        private int m_nPipeStableType;
        public int PipeStableType
        {
            get { return m_nPipeStableType; }
            set { m_nPipeStableType = value; }
        }

        // 배관 압력 안정범위 유지시간(m)
        private int m_nPipeStableCTime;
        public int PipeStableCTime
        {
            get { return m_nPipeStableCTime; }
            set { m_nPipeStableCTime = value; }
        }

        // 배관 압력 안정범위 유지시간 사용여부
        private int m_nPipeStableCTimeUse;
        public int PipeStableCTimeUse
        {
            get { return m_nPipeStableCTimeUse; }
            set { m_nPipeStableCTimeUse = value; }
        }       
    }

    public class AlarmOption
    {
        private int m_nID = -1;
        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        private int m_nTankID = -1;
        public int TankID
        {
            get { return m_nTankID; }
            set { m_nTankID = value; }
        }

        private int m_nLinkedPipeID = -1;
        public int LinkedPipeID
        {
            get { return m_nLinkedPipeID; }
            set { m_nLinkedPipeID = value; }
        }

        // 배관 압력 안정범위 비율(%)
        private float m_fPipeStableRatio;
        public float PipeStableRatio
        {
            get { return m_fPipeStableRatio; }
            set { m_fPipeStableRatio = value; }
        }

        // 배관 압력 안정범위 절대값 (kg/cm3)
        private float m_fPipeStableAbsolute;
        public float PipeStableAbsolute
        {
            get { return m_fPipeStableAbsolute; }
            set { m_fPipeStableAbsolute = value; }
        }

        // 배관 안정 범위 타입
        private int m_nPipeStableType;
        public int PipeStableType
        {
            get { return m_nPipeStableType; }
            set { m_nPipeStableType = value; }
        }

        // 배관 압력 안정범위 유지시간(m)
        private int m_nPipeStableCTime;
        public int PipeStableCTime
        {
            get { return m_nPipeStableCTime; }
            set { m_nPipeStableCTime = value; }
        }

        // 배관 압력 안정범위 유지시간 사용여부
        private int m_nPipeStableCTimeUse;
        public int PipeStableCTimeUse
        {
            get { return m_nPipeStableCTimeUse; }
            set { m_nPipeStableCTimeUse = value; }
        }

        // 탱크 압력 안정범위 비율(%)
        private float m_fTankStableRatio;
        public float TankStableRatio
        {
            get { return m_fTankStableRatio; }
            set { m_fTankStableRatio = value; }
        }

        // 탱크 압력 안정범위 절대값 (kg/cm3)
        private float m_fTankStableAbsolute;
        public float TankStableAbsolute
        {
            get { return m_fTankStableAbsolute; }
            set { m_fTankStableAbsolute = value; }
        }

        // 탱크 안정 범위 타입
        private int m_nTankStableType;
        public int TankStableType
        {
            get { return m_nTankStableType; }
            set { m_nTankStableType = value; }
        }
        
        // 탱크 압력 안정범위 유지시간(m)
        private int m_nTankStableCTime;
        public int TankStableCTime
        {
            get { return m_nTankStableCTime; }
            set { m_nTankStableCTime = value; }
        }

        // 탱크 압력 안정범위 유지시간 사용여부
        private int m_nTankStableCTimeUse;
        public int TankStableCTimeUse
        {
            get { return m_nTankStableCTimeUse; }
            set { m_nTankStableCTimeUse = value; }
        }

        // 알람 발생후 무시시간
        private int m_nAlarmInterval;
        public int AlarmInterval
        {
            get { return m_nAlarmInterval; }
            set { m_nAlarmInterval = value; }
        }

        // 안정범위 유지시간 사용여부
        private int m_nAlarmIntervalUse;
        public int AlarmIntervalUse
        {
            get { return m_nAlarmIntervalUse; }
            set { m_nAlarmIntervalUse = value; }
        }
        
        // 안정 범위 작업시작후 기준(분) - 알람무시구간
        private int m_nStableBeginWorkM;
        public int StableBeginWorkM
        {
            get { return m_nStableBeginWorkM; }
            set { m_nStableBeginWorkM = value; }
        }

        // Pipe별 옵션 저장
        private void SavePipeOption()
        {
            throw new NotImplementedException();
        }

        // Tank별 옵션 저장
        private void SaveTankOption()
        {
            throw new NotImplementedException();
        }

        // AlarmOption 저장
        private void SaveAlarmOption2()
        {
            throw new NotImplementedException();
        }

        public void SaveAlarmOption()
        {
            SavePipeOption();
            SaveTankOption();

            SaveAlarmOption2();
        }
    }

    public class AlarmIgnore
    {
        private int m_nID = -1;
        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        private int m_nTargetType;
        public int TargetType
        {
            get { return m_nTargetType; }
            set { m_nTargetType = value; }
        }

        private int m_nTargetID;
        public int TargetID
        {
            get { return m_nTargetID; }
            set { m_nTargetID = value; }
        }

        private DateTime m_dtBegin;
        public DateTime Begin
        {
            get { return m_dtBegin; }
            set { m_dtBegin = value; }
        }

        private DateTime m_dtEnd;
        public DateTime End
        {
            get { return m_dtEnd; }
            set { m_dtEnd = value; }
        }

        private int m_nIgnreTime;
        public int IgnreTime
        {
            get { return m_nIgnreTime; }
            set { m_nIgnreTime = value; }
        }

        private int m_nBeginUser;
        public int BeginUser
        {
            get { return m_nBeginUser; }
            set { m_nBeginUser = value; }
        }

        private int m_nEndUser;
        public int EndUser
        {
            get { return m_nEndUser; }
            set { m_nEndUser = value; }
        }

        private int m_nWorkHistoryID;
        public int WorkHistoryID
        {
            get { return m_nWorkHistoryID; }
            set { m_nWorkHistoryID = value; }
        }

    }
}
