using System;
using System.Collections.Generic;
using System.Text;

namespace History.BLL.Models.Request
{
    public class RequestData
    {
        private RequestUserHistories m_requestUserHistories = null;
        private RequestGetMinMaxIndex m_requestGetMinMaxIndex = null;
        private RequestSensorDetectHistories m_requestSensorDetectHistories = null;
        private RequestSOPHistories m_requestSOPHistories = null;
        private RequestSOPComponentHistories m_requestSOPComponentHistories = null;
        private RequestSensorDetectAnalysis m_requestSensorDetectAnalysis = null;
        private bool? m_requestDisasterCategories = null;
        private RequestUpdateAlarmMemo m_RequestUpdateAlarmMemo = null;
        public RequestUserHistories RequestUserHistories
        {
            get { return m_requestUserHistories; }
            set { m_requestUserHistories = value; }
        }

        public RequestGetMinMaxIndex RequestGetMinMaxIndex
        {
            get { return m_requestGetMinMaxIndex; }
            set { m_requestGetMinMaxIndex = value; }
        }
        public RequestSensorDetectHistories RequestSensorDetectHistories
        {
            get { return m_requestSensorDetectHistories; }
            set { m_requestSensorDetectHistories = value; }
        }

        public RequestSensorDetectAnalysis RequestSensorDetectAnalysis
        {
            get { return m_requestSensorDetectAnalysis; }
            set { m_requestSensorDetectAnalysis = value; }
        }

        public RequestSOPHistories RequestSOPHistories
        {
            get { return m_requestSOPHistories; }
            set { m_requestSOPHistories = value; }
        }
        public RequestSOPComponentHistories RequestSOPComponentHistories
        {
            get { return m_requestSOPComponentHistories; }
            set { m_requestSOPComponentHistories = value; }
        }
        public bool? RequestDisasterCategories
        {
            get { return m_requestDisasterCategories; }
            set { m_requestDisasterCategories = value; }
        }

        public RequestUpdateAlarmMemo RequestUpdateAlarmMemo
        {
            get { return m_RequestUpdateAlarmMemo; }
            set { m_RequestUpdateAlarmMemo = value; }
        }
    }

    public class RequestUserHistories
    {
        private string m_strBeginTime = "";
        private string m_strEndTime = "";

        public string BeginTime
        {
            get { return m_strBeginTime; }
            set { m_strBeginTime = value; }
        }

        public string EndTime
        {
            get { return m_strEndTime; }
            set { m_strEndTime = value; }
        }
    }

    public class RequestSensorDetectHistories
    {
        private string m_strBeginTime = "";
        private string m_strEndTime = "";
        private int m_nFacilityType = -1;
        private int m_nBuildingGroupID = -1;
        private int m_nBuildingID = -1;
        private int m_nZoneID = -1;

        private int m_nLastSensorZoneHistoryID = -1;
        private int m_nRowCount = 10; // 한 페이지에 보여줄 row 개수
        private bool m_bIsDesc = true; // 다음 페이지로 넘어갈 경우 작은값으로 조회, 이전페이지로 넘어갈 경우 큰값으로 조회


        public string BeginTime
        {
            get { return m_strBeginTime; }
            set { m_strBeginTime = value; }
        }

        public string EndTime
        {
            get { return m_strEndTime; }
            set { m_strEndTime = value; }
        }

        public int FacilityType
        {
            get { return m_nFacilityType; }
            set { m_nFacilityType = value; }
        }
        public int BuildingGroupID
        {
            get { return m_nBuildingGroupID; }
            set { m_nBuildingGroupID = value; }
        }
        public int BuildingID
        {
            get { return m_nBuildingID; }
            set { m_nBuildingID = value; }
        }
        public int ZoneID
        {
            get { return m_nZoneID; }
            set { m_nZoneID = value; }
        }

        public int LastSensorZoneHistoryID
        {
            get { return m_nLastSensorZoneHistoryID; }
            set { m_nLastSensorZoneHistoryID = value; }
        }

        public int RowCount
        {
            get { return m_nRowCount; }
            set { m_nRowCount = value; }
        }

        public bool IsDesc
        {
            get { return m_bIsDesc; }
            set { m_bIsDesc = value; }
        }
    }

    public class RequestGetMinMaxIndex
    {
        private string m_strBeginTime = "";
        private string m_strEndTime = "";
        private int m_nFacilityType = -1;
        private int m_nBuildingGroupID = -1;
        private int m_nBuildingID = -1;
        private int m_nZoneID = -1;

        public string BeginTime
        {
            get { return m_strBeginTime; }
            set { m_strBeginTime = value; }
        }

        public string EndTime
        {
            get { return m_strEndTime; }
            set { m_strEndTime = value; }
        }

        public int FacilityType
        {
            get { return m_nFacilityType; }
            set { m_nFacilityType = value; }
        }
        public int BuildingGroupID
        {
            get { return m_nBuildingGroupID; }
            set { m_nBuildingGroupID = value; }
        }
        public int BuildingID
        {
            get { return m_nBuildingID; }
            set { m_nBuildingID = value; }
        }
        public int ZoneID
        {
            get { return m_nZoneID; }
            set { m_nZoneID = value; }
        }
    }

    public class RequestSensorDetectAnalysis
    {
        private string m_strBeginTime = "";
        private string m_strEndTime = "";
        private int m_nFacilityType = -1;
        private int m_nBuildingGroupID = -1;
        private int m_nBuildingID = -1;
        private int m_nZoneID = -1;

        public string BeginTime
        {
            get { return m_strBeginTime; }
            set { m_strBeginTime = value; }
        }

        public string EndTime
        {
            get { return m_strEndTime; }
            set { m_strEndTime = value; }
        }
        public int FacilityType
        {
            get { return m_nFacilityType; }
            set { m_nFacilityType = value; }
        }
        public int BuildingGroupID
        {
            get { return m_nBuildingGroupID; }
            set { m_nBuildingGroupID = value; }
        }
        public int BuildingID
        {
            get { return m_nBuildingID; }
            set { m_nBuildingID = value; }
        }
        public int ZoneID
        {
            get { return m_nZoneID; }
            set { m_nZoneID = value; }
        }
    }

    public class RequestSOPHistories
    {
        private string m_strBeginTime = "";
        private string m_strEndTime = "";
        
        public string BeginTime
        {
            get { return m_strBeginTime; }
            set { m_strBeginTime = value; }
        }

        public string EndTime
        {
            get { return m_strEndTime; }
            set { m_strEndTime = value; }
        }
    }

    public class RequestSOPComponentHistories
    {
        private int m_nActionStepHistoryID = -1;
        public int ActionStepHistoryID
        {
            get { return m_nActionStepHistoryID; }
            set { m_nActionStepHistoryID = value; }
        }
    }

    public class RequestUpdateAlarmMemo
    {
        private int m_nSensorZoneHistoryID = -1;
        private string m_strMemo = "";

        public int SensorZoneHistoryID
        {
            get { return m_nSensorZoneHistoryID; }
            set { m_nSensorZoneHistoryID = value; }
        }

        public string Memo
        {
            get { return m_strMemo; }
            set { m_strMemo = value; }
        }
    }
}
