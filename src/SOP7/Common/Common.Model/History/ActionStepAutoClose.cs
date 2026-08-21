using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Model.History
{
    public class ActionStepAutoClose
    {
        public enum Fields { ID, ActionStepHistoryID, ActionStepID, UseCloseNoInput, UseCloseSensorReset, UseCloseSensorResetWaitTime, InputWaitTime, SensorResetWaitTime, BeginTime, SensorZoneID, SensorZoneHistoryID, Description };

        private int m_nID = -1;
        private int m_nActionStepHistoryID = -1;
        private int? m_nActionStepID = null;
        // 입력이 없을때 SOP 자동 종료 사용여부
        private int? m_nUseCloseNoInput = null;
        // 센서 리셋 신호시 SOP자동 종료 사용여부
        private int? m_nUseCloseSensorReset = null;
        // 센서 신호시 몇분뒤 자동 종료 사용여부
        private int? m_nUseCloseSensorResetWaitTime = null;
        // 입력 대기시간 (초)
        private int? m_nInputWaitTime = null;
        // 센서 리셋 후 대기 시간 (초)
        private int? m_nSensorResetWaitTime = null;
        // SOP 시작 시간
        private DateTime? m_dtBegin = null;
        private int? m_nSensorZoneID = null;
        private int? m_nSensorZoneHistoryID = null;
        private string m_strDescription = null;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public int ActionStepHistoryID
        {
            get { return m_nActionStepHistoryID; }
            set { m_nActionStepHistoryID = value; }
        }

        public int? ActionStepID
        {
            get { return m_nActionStepID; }
            set { m_nActionStepID = value; }
        }

        /// <summary>
        /// 입력이 없을때 SOP 자동 종료 사용여부
        /// </summary>
        public int? UseCloseNoInput
        {
            get { return m_nUseCloseNoInput; }
            set { m_nUseCloseNoInput = value; }
        }

        /// <summary>
        /// 센서 리셋 신호시 SOP자동 종료 사용여부
        /// </summary>
        public int? UseCloseSensorReset
        {
            get { return m_nUseCloseSensorReset; }
            set { m_nUseCloseSensorReset = value; }
        }

        /// <summary>
        /// 센서 신호시 몇분뒤 자동 종료 사용여부
        /// </summary>
        public int? UseCloseSensorResetWaitTime
        {
            get { return m_nUseCloseSensorResetWaitTime; }
            set { m_nUseCloseSensorResetWaitTime = value; }
        }

        /// <summary>
        /// 입력 대기시간 (초)
        /// </summary>
        public int? InputWaitTime
        {
            get { return m_nInputWaitTime; }
            set { m_nInputWaitTime = value; }
        }

        /// <summary>
        /// 센서 리셋 후 대기 시간(초)
        /// </summary>
        public int? SensorResetWaitTime
        {
            get { return m_nSensorResetWaitTime; }
            set { m_nSensorResetWaitTime = value; }
        }

        /// <summary>
        /// SOP 시작 시간
        /// </summary>
        public DateTime? BeginTime
        {
            get { return m_dtBegin; }
            set { m_dtBegin = value; }
        }

        public int? SensorZoneID
        {
            get { return m_nSensorZoneID; }
            set { m_nSensorZoneID = value; }
        }

        public int? SensorZoneHistoryID
        {
            get { return m_nSensorZoneHistoryID; }
            set { m_nSensorZoneHistoryID = value; }
        }

        public string Description
        {
            get { return m_strDescription; }
            set { m_strDescription = value; }
        }

        public static string TableName
        {
            get { return "SopHistoryActionStepAutoClose"; }
        }

        public static string GetFieldName(Fields field, out bool isNullable)
        {
            if (field == Fields.ID ||
                field == Fields.ActionStepHistoryID)
                isNullable = false;
            else
                isNullable = true;

            return field.ToString();
        }
    }
}
