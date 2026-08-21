using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBUtility2;
using System.Collections;
using UnE.Sensor;
using UnE.Spatial;

namespace libSOPPolicy
{
    using Common;
    using UnE.Earthquake;
    using UnE.SOP;

    public class BaseSOPUser
    {
        private int m_nID = -1;
        private int m_nSiteID = -1;
        private string m_strUserID = "";
        private string m_strUserNickName = "";

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public int SiteID
        {
            get { return m_nSiteID; }
            set { m_nSiteID = value; }
        }

        public string UserID
        {
            get { return m_strUserID; }
            set { m_strUserID = value; }
        }

        public string NickName
        {
            get { return m_strUserNickName; }
            set { m_strUserNickName = value; }
        }

        public BaseSOPUser()
        {
        }

        // nSensorZoneID 신호가 탐지되었을때 이에 연결된 SOP를 이 SOPUser 계정으로 제어가 가능한지 여부를 알려준다.
        public virtual bool AbletoAccess(int nSensorZoneID, int nSiteID, WebDBManager dbMgr)
        {
            return true;
        }

        // nSensorZoneID 신호가 탐지되었을때 이에 연결된 SOP를 이 SOPUser 계정으로 제어가 가능한지 여부를 알려준다.
        public virtual bool AbletoAccess(int nSensorZoneID, int nSiteID, DirectDBManager dbMgr)
        {
            return true;
        }

        // strSOPFullPath 해당하는 SOP를 이 SOPUser 계정으로 제어가 가능한지 여부를 알려준다.
        public virtual bool AbletoAccess(string strSOPFullPath, int nSiteID, WebDBManager dbMgr)
        {
            return true;
        }

        // strSOPFullPath 해당하는 SOP를 이 SOPUser 계정으로 제어가 가능한지 여부를 알려준다.
        public virtual bool AbletoAccess(string strSOPFullPath, int nSiteID, DirectDBManager dbMgr)
        {
            return true;
        }

        // 탐지된 nSensorZoneID 신호가 실제 재난상황으로 전파되었다.
        // 이에 연결된 SOP를 이 SOPUser 계정으로 제어가 가능한지 여부를 알려준다.
        // 이 SOPUser 계정에 권한이 없으면 -2가 리턴된다.
        // 적당한 SOP가 존재하지 않으면 -1이 리턴된다.
        // 권한도 있고 SOP도 있으면 해당 Disaster ID가 리턴된다.
        public virtual int GetReportDisasterID(int nSensorZoneID, int nSiteID, WebDBManager dbMgr)
        {
            // 일반적인 경우 재난신고에 대하여는 별도의 SOP를 동작시키지 않는다.
            // 탐지시 동작시킨 SOP로 재난신고에도 대응한다.
            return -1;
        }

        // 탐지된 nSensorZoneID 신호가 실제 재난상황으로 전파되었다.
        // 이에 연결된 SOP를 이 SOPUser 계정으로 제어가 가능한지 여부를 알려준다.
        // 이 SOPUser 계정에 권한이 없으면 -2가 리턴된다.
        // 적당한 SOP가 존재하지 않으면 -1이 리턴된다.
        // 권한도 있고 SOP도 있으면 해당 Disaster ID가 리턴된다.
        public virtual int GetReportDisasterID(int nSensorZoneID, int nSiteID, DirectDBManager dbMgr)
        {
            // 일반적인 경우 재난신고에 대하여는 별도의 SOP를 동작시키지 않는다.
            // 탐지시 동작시킨 SOP로 재난신고에도 대응한다.
            return -1;
        }

        public virtual bool AbleToEditTools()
        {
            return true;
        }

        // Site별로 계정등급은 다르게 설정되어 있을수 있다.
        public virtual int GetUserGrade()
        {
            return 1;
        }

        public virtual string GetLinkedSOPFullPath(int nSensorZoneID, IFacility.FacilityType sensorType, object param, DirectDBManager dbMgr, ISensorZoneManager sensorZoneMgr, int manualReportZoneID, int alarmDepth)
        {
            string strDefault = "";
            dbMgr = dbMgr.Clone();

            if (dbMgr.Connect() == false)
                return strDefault;

            string strSOPFullPath = strDefault;

            if (IFacility.IsFireSensorType(sensorType))
                strSOPFullPath = GetFireLinkedSOPFullPath(nSensorZoneID, dbMgr, sensorZoneMgr, strDefault, manualReportZoneID);
            else if (IFacility.IsPSMSensorType(sensorType))
                strSOPFullPath = GetPSMLinkedSOPFullPath(nSensorZoneID, dbMgr, sensorZoneMgr, strDefault, manualReportZoneID);
            else if (IFacility.IsSecurityType(sensorType))
                strSOPFullPath = GetSecurityLinkedSOPFullPath(nSensorZoneID, (int)sensorType, dbMgr, sensorZoneMgr, strDefault);
            else if (IFacility.IsEarthquakeSensorType(sensorType))
                strSOPFullPath = GetEarthquakeLinkedSOPFullPath(nSensorZoneID, param, dbMgr, strDefault);
            else if (IFacility.IsETCSensorType(sensorType))
                strSOPFullPath = GetEtcLinkedSOPFullPath(nSensorZoneID, (int)sensorType, dbMgr, sensorZoneMgr, strDefault, manualReportZoneID);

            ISupervisor supervisor = SupervisorFactory.MakeInstance(dbMgr);

            if (supervisor != null)
            {
                string strActionStepName = "";

                // 수동신고
                if (nSensorZoneID >= 1000000 && alarmDepth >= 0)
                {
                    strActionStepName = supervisor.GetActionStepNameFromAlarmDepth(strSOPFullPath, alarmDepth);
                }
                else
                    strActionStepName = supervisor.GetActionStepName(strSOPFullPath, (int)sensorType);

                if (strActionStepName != null && strActionStepName.Length > 0)
                    strSOPFullPath += "/" + strActionStepName;
            }

            dbMgr.Close();
            return strSOPFullPath;
        }

        protected virtual string GetEarthquakeLinkedSOPFullPath(int nSensorZoneID, object param, DirectDBManager dbMgr, string strDefault)
        {
            if (param == null || (param is EarthquakeOption) == false)
                return strDefault;

            EarthquakeOption option = (EarthquakeOption)param;
            return option.LinkedSOP;
        }

        protected virtual string GetEtcLinkedSOPFullPath(int nSensorZoneID, int nSensorType, DirectDBManager dbMgr, ISensorZoneManager sensorZoneMgr, string strDefault, int manualReportZoneID)
        {
            int nEquipZoneID, nZoneID, nBuildingID;

            // 수동신고
            if (nSensorZoneID >= 1000000)
            {
                if (manualReportZoneID == -1)
                    return "";

                Zone zone = sensorZoneMgr.GetZone(manualReportZoneID);

                nZoneID = zone.ID;
                nBuildingID = zone.Building.ID;
            }
            else
            { 
                if (GetSensorZoneInfo(nSensorZoneID, dbMgr, sensorZoneMgr, out nEquipZoneID, out nZoneID, out nBuildingID) == false)
                    return strDefault;
            }

            // 1. LinkedZone에 맞는 SOP가 있으면 먼저 선택한다.
            // 2. LinkedBuilding에 맟는 SOP가 있으면 그 다음 우선순위로 선택한다.
            // 3. 둘다 없을 경우 SensorType에 맞는 SOP를 선택한다.
            string strSQL = string.Format("Select SOPName, LinkedBuildingID, LinkedZoneID from ETCSensorSOPLink where Type = {0} and SiteID = {1}", nSensorType, dbMgr.SiteID);
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return strDefault;

            int nResultCount = arrResult.Count;

            // Zone ID가 null이 아닌 값들
            ArrayList arrZoneIDs = new ArrayList();
            // Building ID가 null이 아닌 값들
            ArrayList arrBuildingIDs = new ArrayList();
            // 나머지 들
            ArrayList arrOthers = new ArrayList();

            for (int i = 0; i < nResultCount - 2; i += 3)
            {
                string strSOPName = WebDBManager.GetStringField(arrResult[i]);
                string strBuildingIDs = WebDBManager.GetStringField(arrResult[i + 1]);
                string strZoneIDs = WebDBManager.GetStringField(arrResult[i + 2]);

                if (strSOPName == null)
                    continue;

                if (strZoneIDs != null)
                {
                    arrZoneIDs.Add(strSOPName);
                    arrZoneIDs.Add(strZoneIDs);
                }

                if (strBuildingIDs != null)
                {
                    arrBuildingIDs.Add(strSOPName);
                    arrBuildingIDs.Add(strBuildingIDs);
                }

                if (strBuildingIDs == null && strZoneIDs == null)
                {
                    arrOthers.Add(strSOPName);
                }

                /*if (strZoneIDs != null && nZoneID >= 0)
                {
                    List<int> zoneIDs = GetIDs(strZoneIDs);

                    if (zoneIDs != null && zoneIDs.Contains(nZoneID))
                    {
                        return strSOPName;
                    }
                }

                if (strBuildingIDs != null && nBuildingID >= 0)
                {
                    List<int> buildingIDs = GetIDs(strBuildingIDs);

                    if (buildingIDs != null && buildingIDs.Contains(nBuildingID))
                    {
                        return strSOPName;
                    }
                }

                if (strBuildingIDs == null && strZoneIDs == null)
                    return strSOPName;*/
            }

            int nZoneSOPCount = arrZoneIDs.Count;

            for (int i = 0; i < nZoneSOPCount - 1; i += 2)
            {
                string strSOPName = (string)arrZoneIDs[i];
                string strLinkedZoneID = (string)arrZoneIDs[i + 1];

                if (nZoneID >= 0)
                {
                    List<int> zoneIDs = GetIDs(strLinkedZoneID);

                    if (zoneIDs != null)
                    {
                        if (zoneIDs.Contains(nZoneID))
                            return strSOPName;
                    }
                }
            }

            int nBuildingSOPCount = arrBuildingIDs.Count;

            for (int i = 0; i < nBuildingSOPCount - 1; i += 2)
            {
                string strSOPName = (string)arrBuildingIDs[i];
                string strLinkedBuildingID = (string)arrBuildingIDs[i + 1];

                if (nBuildingID >= 0)
                {
                    List<int> buildingIDs = GetIDs(strLinkedBuildingID);

                    if (buildingIDs != null)
                    {
                        if (buildingIDs.Contains(nBuildingID))
                            return strSOPName;
                    }
                }
            }

            foreach (string strSOPName in arrOthers)
            {
                return strSOPName;
            }

            return strDefault;
        }

        protected virtual string GetSecurityLinkedSOPFullPath(int nSensorZoneID, int nSensorType, DirectDBManager dbMgr, ISensorZoneManager sensorZoneMgr, string strDefault)
        {
            int nEquipZoneID, nZoneID, nBuildingID;

            if (GetSensorZoneInfo(nSensorZoneID, dbMgr, sensorZoneMgr, out nEquipZoneID, out nZoneID, out nBuildingID) == false)
                return strDefault;

            int nSecurityTypeID = GetSecurityTypeTableID(dbMgr, nSensorType);

            if (nSecurityTypeID < 0)
                return strDefault;

            string strSQL = "select li.SOPName, link.LinkedBuildingID, link.LinkedZoneID ";
            strSQL += "from SecuritySensorSOPLink as link, SecuritySensorSOPList as li ";
            strSQL += string.Format("where link.SOPID = li.ID and link.SecurityTypeID = {0} and link.SiteID = {1}", nSecurityTypeID, dbMgr.SiteID);

            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return strDefault;

            int nResultCount = arrResult.Count;

            // Zone ID가 null이 아닌 값들
            ArrayList arrZoneIDs = new ArrayList();
            // Building ID가 null이 아닌 값들
            ArrayList arrBuildingIDs = new ArrayList();
            // 나머지 들
            ArrayList arrOthers = new ArrayList();

            for (int i = 0; i < nResultCount - 2; i += 3)
            {
                string strSOPName = WebDBManager.GetStringField(arrResult[i]);
                string strLinkedBuildingID = WebDBManager.GetStringField(arrResult[i + 1]);
                string strLinkedZoneID = WebDBManager.GetStringField(arrResult[i + 2]);

                if (strSOPName == null)
                    continue;

                if (strLinkedZoneID != null)
                {
                    arrZoneIDs.Add(strSOPName);
                    arrZoneIDs.Add(strLinkedZoneID);
                }

                if (strLinkedBuildingID != null)
                {
                    arrBuildingIDs.Add(strSOPName);
                    arrBuildingIDs.Add(strLinkedBuildingID);
                }

                if (strLinkedBuildingID == null && strLinkedZoneID == null)
                {
                    arrOthers.Add(strSOPName);
                }

                /*if (strLinkedZoneID != null)
                {
                    List<int> zoneIDs = GetIDs(strLinkedZoneID);

                    if (zoneIDs != null && nZoneID >= 0)
                    {
                        if (zoneIDs.Contains(nZoneID))
                            return strSOPName;
                    }
                }

                if (strLinkedBuildingID != null && nBuildingID >= 0)
                {
                    List<int> buildingIDs = GetIDs(strLinkedBuildingID);

                    if (buildingIDs != null)
                    {
                        if (buildingIDs.Contains(nBuildingID))
                            return strSOPName;
                    }
                }

                if (strLinkedBuildingID == null && strLinkedZoneID == null)
                    return strSOPName;*/
            }

            int nZoneSOPCount = arrZoneIDs.Count;

            for (int i = 0; i < nZoneSOPCount - 1; i += 2)
            {
                string strSOPName = (string)arrZoneIDs[i];
                string strLinkedZoneID = (string)arrZoneIDs[i + 1];

                if (nZoneID >= 0)
                {
                    List<int> zoneIDs = GetIDs(strLinkedZoneID);

                    if (zoneIDs != null)
                    {
                        if (zoneIDs.Contains(nZoneID))
                            return strSOPName;
                    }
                }
            }

            int nBuildingSOPCount = arrBuildingIDs.Count;

            for (int i = 0; i < nBuildingSOPCount - 1; i += 2)
            {
                string strSOPName = (string)arrBuildingIDs[i];
                string strLinkedBuildingID = (string)arrBuildingIDs[i + 1];

                if (nBuildingID >= 0)
                {
                    List<int> buildingIDs = GetIDs(strLinkedBuildingID);

                    if (buildingIDs != null)
                    {
                        if (buildingIDs.Contains(nBuildingID))
                            return strSOPName;
                    }
                }
            }

            foreach (string strSOPName in arrOthers)
            {
                return strSOPName;
            }

            return strDefault;
        }

        private int GetSecurityTypeTableID(DirectDBManager dbMgr, int nSensorType)
        {
            string strSQL = "Select ID, FacilityTypeIDs from securitytypetable";
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return -1;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 1; i += 2)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                string strTypeIDs = WebDBManager.GetStringField(arrResult[i + 1]);

                if (id == null || strTypeIDs == null)
                    continue;

                string[] tokens = strTypeIDs.Split(',');

                foreach (string strToken in tokens)
                {
                    int nID;

                    if (int.TryParse(strToken.Trim(), out nID))
                    {
                        if (nID == nSensorType)
                            return id.Data;
                    }
                }
            }

            return -1;
        }

        protected virtual string GetPSMLinkedSOPFullPath(int nSensorZoneID, DirectDBManager dbMgr, ISensorZoneManager sensorZoneMgr, string strDefault, int manualReportZoneID)
        {
            // 수동신고 - 누출사고 수동신고는 ETC에서 찾는다
            if (nSensorZoneID >= 1000000)
            {
                return GetEtcLinkedSOPFullPath(nSensorZoneID, (int)IFacility.FacilityType.PSM_SENSOR, dbMgr, sensorZoneMgr, strDefault, manualReportZoneID);
            }

            int nEquipZoneID, nZoneID, nBuildingID;

            if (GetSensorZoneInfo(nSensorZoneID, dbMgr, sensorZoneMgr, out nEquipZoneID, out nZoneID, out nBuildingID) == false)
                return strDefault;

            if (nEquipZoneID < 0)
                return strDefault;

            string strSQL = "select ID from PSMTank where EquipZoneID = " + nEquipZoneID.ToString();
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return null;

            int nResultCount = arrResult.Count;
            Dictionary<int, int> dicTankIDs = new Dictionary<int, int>();

            for (int i = 0; i < nResultCount; i++)
            {
                VariousData<int> tankID = WebDBManager.GetIntField(arrResult[i].ToString());

                if (tankID != null)
                    dicTankIDs[tankID.Data] = tankID.Data;
            }

            if (dicTankIDs.Count == 0)
                return strDefault;

            strSQL = "Select SOPName, LinkedTankID from PSMSensorSOPLink where SiteID = " + dbMgr.SiteID.ToString();
            arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return strDefault;

            nResultCount = arrResult.Count;

            // Tank ID가 null이 아닌 값들
            ArrayList arrTankIDs = new ArrayList();
            // 나머지 들
            ArrayList arrOthers = new ArrayList();

            for (int i = 0; i < nResultCount - 1; i += 2)
            {
                string strSOPName = WebDBManager.GetStringField(arrResult[i]);
                string strLinkedTankID = WebDBManager.GetStringField(arrResult[i + 1]);

                if (strSOPName == null)
                    continue;

                if (strLinkedTankID != null)
                {
                    arrTankIDs.Add(strSOPName);
                    arrTankIDs.Add(strLinkedTankID);
                }
                else
                {
                    arrOthers.Add(strSOPName);
                }

                /*if (strLinkedTankID == null)
                {
                    return strSOPName;
                }

                List<int> tankIDs = GetIDs(strLinkedTankID);

                foreach (int nTankID in tankIDs)
                {
                    if (dicTankIDs.ContainsKey(nTankID))
                        return strSOPName;
                }*/
            }

            int nTankCount = arrTankIDs.Count;

            for (int i=0;i<nTankCount-1;i+=2)
            {
                string strSOPName = (string)arrTankIDs[i];
                string strLinkedTankID = (string)arrTankIDs[i + 1];

                List<int> tankIDs = GetIDs(strLinkedTankID);

                foreach (int nTankID in tankIDs)
                {
                    if (dicTankIDs.ContainsKey(nTankID))
                        return strSOPName;
                }
            }

            foreach (string strSOPName in arrOthers)
            {
                return strSOPName;
            }

            return strDefault;
        }

        protected virtual string GetFireLinkedSOPFullPath(int nSensorZoneID, DirectDBManager dbMgr, ISensorZoneManager sensorZoneMgr, string strDefault, int manualReportZoneID)
        {
            int nEquipZoneID, nZoneID, nBuildingID;

            // 수동신고
            if (nSensorZoneID >= 1000000)
            {
                if (manualReportZoneID == -1)
                    return "";

                Zone zone = sensorZoneMgr.GetZone(manualReportZoneID);

                nZoneID = zone.ID;
                nBuildingID = zone.Building.ID;
            }
            else
            {
                if (GetSensorZoneInfo(nSensorZoneID, dbMgr, sensorZoneMgr, out nEquipZoneID, out nZoneID, out nBuildingID) == false)
                    return strDefault;
            }

            string strSQL = "Select SOPName, LinkedBuildingID, LinkedZoneID from FireSensorSOPLink where SiteID = " + dbMgr.SiteID.ToString();
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return strDefault;

            int nResultCount = arrResult.Count;

            // Zone ID가 null이 아닌 값들
            ArrayList arrZoneIDs = new ArrayList();
            // Building ID가 null이 아닌 값들
            ArrayList arrBuildingIDs = new ArrayList();
            // 나머지 들
            ArrayList arrOthers = new ArrayList();

            for (int i = 0; i < nResultCount - 2; i += 3)
            {
                string strSOPName = WebDBManager.GetStringField(arrResult[i]);
                string strLinkedBuildingID = WebDBManager.GetStringField(arrResult[i + 1]);
                string strLinkedZoneID = WebDBManager.GetStringField(arrResult[i + 2]);

                if (strSOPName == null)
                    continue;

                if (strLinkedZoneID != null)
                {
                    arrZoneIDs.Add(strSOPName);
                    arrZoneIDs.Add(strLinkedZoneID);
                }

                if (strLinkedBuildingID != null)
                {
                    arrBuildingIDs.Add(strSOPName);
                    arrBuildingIDs.Add(strLinkedBuildingID);
                }

                if (strLinkedBuildingID == null && strLinkedZoneID == null)
                {
                    arrOthers.Add(strSOPName);
                }

                /*if (strLinkedZoneID != null && nZoneID >= 0)
                {
                    List<int> zoneIDs = GetIDs(strLinkedZoneID);

                    if (zoneIDs != null)
                    {
                        if (zoneIDs.Contains(nZoneID))
                            return strSOPName;
                    }
                }

                if (strLinkedBuildingID != null && nBuildingID >= 0)
                {
                    List<int> buildingIDs = GetIDs(strLinkedBuildingID);

                    if (buildingIDs != null)
                    {
                        if (buildingIDs.Contains(nBuildingID))
                            return strSOPName;
                    }
                }

                if (strLinkedBuildingID == null && strLinkedZoneID == null)
                    return strSOPName;*/
            }

            int nZoneSOPCount = arrZoneIDs.Count;

            for (int i=0;i<nZoneSOPCount-1;i+=2)
            {
                string strSOPName = (string)arrZoneIDs[i];
                string strLinkedZoneID = (string)arrZoneIDs[i + 1];

                if (nZoneID >= 0)
                {
                    List<int> zoneIDs = GetIDs(strLinkedZoneID);

                    if (zoneIDs != null)
                    {
                        if (zoneIDs.Contains(nZoneID))
                            return strSOPName;
                    }
                }
            }

            int nBuildingSOPCount = arrBuildingIDs.Count;

            for (int i = 0; i < nBuildingSOPCount - 1; i += 2)
            {
                string strSOPName = (string)arrBuildingIDs[i];
                string strLinkedBuildingID = (string)arrBuildingIDs[i + 1];

                if (nBuildingID >= 0)
                {
                    List<int> buildingIDs = GetIDs(strLinkedBuildingID);

                    if (buildingIDs != null)
                    {
                        if (buildingIDs.Contains(nBuildingID))
                            return strSOPName;
                    }
                }
            }

            foreach (string strSOPName in arrOthers)
            {
                return strSOPName;
            }

            return strDefault;
        }

        protected List<int> GetIDs(string strIDs)
        {
            string[] tokens = strIDs.Split(',');
            List<int> ids = new List<int>();
            int nID;

            foreach (string strToken in tokens)
            {
                if (GetIDInternal(ids, strToken.Trim()) == false)
                    return null;
                /*if (int.TryParse(strToken.Trim(), out nID) == false)
                    return null;
                else
                    ids.Add(nID);*/
            }

            return ids;
        }

        private bool GetIDInternal(List<int> ids, string strIDs)
        {
            string[] tokens = strIDs.Split('-');

            if (tokens.Count() == 1)
            {
                int nID;

                if (int.TryParse(tokens[0].Trim(), out nID))
                    ids.Add(nID);
                else
                    return false;
            }
            else if (tokens.Count() == 2)
            {
                int nBeginID, nEndID;

                if (int.TryParse(tokens[0].Trim(), out nBeginID) && int.TryParse(tokens[1].Trim(), out nEndID))
                {
                    for (int i = nBeginID; i <= nEndID; i++)
                    {
                        ids.Add(i);
                    }
                }
                else
                    return false;
            }

            return true;
        }

        protected bool GetSensorZoneInfo(int nSensorZoneID, DirectDBManager dbMgr, ISensorZoneManager sensorZoneMgr, out int nEquipZoneID, out int nZoneID, out int nBuildingID)
        {
            nEquipZoneID = nZoneID = nBuildingID = -1;

            string strSQL = "Select EquipZoneID, Zone from SensorZone where ID = " + nSensorZoneID.ToString();
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count < 2)
                return false;

            VariousData<int> equipZoneID = WebDBManager.GetIntField(arrResult[0].ToString());
            VariousData<int> zoneID = WebDBManager.GetIntField(arrResult[1].ToString());

            if (equipZoneID == null)
                return false;

            EquipmentZone equipZone = sensorZoneMgr.GetEquipmentZone(equipZoneID.Data);

            if (equipZone == null)
                return false;
            else
                nEquipZoneID = equipZone.ID;

            if (zoneID == null)
                return true;

            Zone zone = sensorZoneMgr.GetZone(zoneID.Data);

            if (zone == null)
                return false;
            else
                nZoneID = zone.ID;

            if (zone.Building != null)
                nBuildingID = zone.Building.ID;

            return true;
        }
    }
}
