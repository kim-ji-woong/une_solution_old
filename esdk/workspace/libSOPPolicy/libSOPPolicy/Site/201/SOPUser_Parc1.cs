using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBUtility2;
using System.Collections;

namespace libSOPPolicy
{
    using Common;

    class SOPUser_Parc1 : BaseSOPUser
    {
        // 일반유저, 건물관리자, 총괄관리자
        public enum UserType { Unknown = 0, NormalUser, BuildingAdmin, Master };

        private UserType m_userType = UserType.Unknown;
        private int m_nBuildingID = -1;

        public SOPUser_Parc1(WebDBManager dbMgr, int nSOPGenUserID)
        {
            SetUserType(dbMgr, nSOPGenUserID);
        }

        public SOPUser_Parc1(DirectDBManager dbMgr, int nSOPGenUserID)
        {
            SetUserType(dbMgr, nSOPGenUserID);
        }

        public UserType GetUserType()
        {
            return m_userType;
        }

        private void SetUserType(WebDBManager dbMgr, int nSOPGenUserID)
        {
            _SetUserType(dbMgr, nSOPGenUserID, dbMgr.SiteID);
        }

        private void SetUserType(DirectDBManager dbMgr, int nSOPGenUserID)
        {
            if (dbMgr == null)
                return;

            dbMgr = dbMgr.Clone();

            if (dbMgr.Connect() == false)
                return;

            _SetUserType(dbMgr, nSOPGenUserID, dbMgr.SiteID);
            dbMgr.Close();
        }

        private void _SetUserType(object dbMgr, int nSOPGenUserID, int nSiteID)
        {
            ID = nSOPGenUserID;

            if (dbMgr == null)
                return;

            this.SiteID = nSiteID;

            string strSQL = "Select su.ID, su.UserID, su.NickName, su.UserLevel, sl.LevelName, sb.BuildingID from SOPGenUser as su, SOPGenLevel as sl, SOPGenUserBuilding as sb where su.UserLevel = sl.ID and su.ID = sb.UserID and su.ID = " + nSOPGenUserID.ToString();
            ArrayList arrResult = DBManager.GetResultData(strSQL, dbMgr);

            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 5; i += 6)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                string strUserID = WebDBManager.GetStringField(arrResult[i + 1]);
                string strUserNickName = WebDBManager.GetStringField(arrResult[i + 2]);
                VariousData<int> userLevel = WebDBManager.GetIntField(arrResult[i + 3].ToString());
                string strLevelName = WebDBManager.GetStringField(arrResult[i + 4]);
                VariousData<int> buildingID = WebDBManager.GetIntField(arrResult[i + 5].ToString());

                if (id == null || strUserID == null || userLevel == null || strLevelName == null || strUserNickName == null)
                    continue;

                UserID = strUserID;
                NickName = strUserNickName;

                if (strLevelName.Contains("관리자"))
                {
                    if (buildingID == null)
                    {
                        m_userType = UserType.Master;
                        m_nBuildingID = -1;
                    }
                    else
                    {
                        m_userType = UserType.BuildingAdmin;
                        m_nBuildingID = buildingID.Data;
                    }
                }
                else
                {
                    // 일반 관리요원일 경우 반드시 해당 건물정보가 있어야 한다.
                    if (buildingID == null)
                        continue;

                    m_userType = UserType.NormalUser;
                    m_nBuildingID = buildingID.Data;
                }

                break;
            }
        }

        // nSensorZoneID 신호가 탐지되었을때 이에 연결된 SOP를 이 SOPUser 계정으로 제어가 가능한지 여부를 알려준다.
        public override bool AbletoAccess(int nSensorZoneID, int nSiteID, WebDBManager dbMgr)
        {
            return _AbletoAccess(nSensorZoneID, nSiteID, dbMgr);
        }

        // nSensorZoneID 신호가 탐지되었을때 이에 연결된 SOP를 이 SOPUser 계정으로 제어가 가능한지 여부를 알려준다.
        public override bool AbletoAccess(int nSensorZoneID, int nSiteID, DirectDBManager dbMgr)
        {
            dbMgr = dbMgr.Clone();

            if (dbMgr.Connect() == false)
                return false;

            bool result = _AbletoAccess(nSensorZoneID, nSiteID, dbMgr);
            dbMgr.Close();
            return result;
        }

        // nSensorZoneID 신호가 탐지되었을때 이에 연결된 SOP를 이 SOPUser 계정으로 제어가 가능한지 여부를 알려준다.
        private bool _AbletoAccess(int nSensorZoneID, int nSiteID, object dbMgr)
        {
            if (m_userType == UserType.Unknown || m_userType == UserType.Master || nSensorZoneID < 0)
                return true;

            string strSQL = "Select EquipZoneID, Zone, ez.LinkedZoneIDList from SensorZone as sz, EquipmentZone as ez where sz.EquipZoneID = ez.ID and sz.ID = " + nSensorZoneID.ToString();
            ArrayList arrResult = DBManager.GetResultData(strSQL, dbMgr);

            if (arrResult == null || arrResult.Count < 3)
                return false;

            VariousData<int> equipZoneID = WebDBManager.GetIntField(arrResult[0].ToString());
            VariousData<int> zoneID = WebDBManager.GetIntField(arrResult[1].ToString());
            string strLinkedZoneIDs = WebDBManager.GetStringField(arrResult[2]);

            int nBuildingID = -1;

            if (zoneID != null)
                nBuildingID = GetBuildingID(zoneID.Data, dbMgr);
            else
            {
                if (strLinkedZoneIDs != null && strLinkedZoneIDs.Length > 0)
                {
                    string[] tokens = strLinkedZoneIDs.Split(',');

                    int nZoneID;

                    if (int.TryParse(tokens[0].Trim(), out nZoneID))
                    {
                        nBuildingID = GetBuildingID(nZoneID, dbMgr);
                    }
                }
            }

            if (nBuildingID < 0)
            {
                // 공통 SOP
                return true;
            }
            else
            {
                if (m_nBuildingID == nBuildingID)
                    return true;
            }

            return false;
        }

        private int GetBuildingID(int nZoneID, object dbMgr)
        {
            string strSQL = "Select BuildingID from Zone where ID = " + nZoneID.ToString();
            ArrayList arrResult = DBManager.GetResultData(strSQL, dbMgr);

            if (arrResult == null || arrResult.Count == 0)
                return -1;

            VariousData<int> buildingID = WebDBManager.GetIntField(arrResult[0].ToString());

            if (buildingID == null)
                return -1;

            return buildingID.Data;
        }

        // strSOPFullPath 해당하는 SOP를 이 SOPUser 계정으로 제어가 가능한지 여부를 알려준다.
        public override bool AbletoAccess(string strSOPFullPath, int nSiteID, WebDBManager dbMgr)
        {
            return _AbletoAccess(strSOPFullPath, nSiteID, dbMgr);
        }

        // strSOPFullPath 해당하는 SOP를 이 SOPUser 계정으로 제어가 가능한지 여부를 알려준다.
        public override bool AbletoAccess(string strSOPFullPath, int nSiteID, DirectDBManager dbMgr)
        {
            dbMgr = dbMgr.Clone();

            if (dbMgr.Connect() == false)
                return false;

            bool result = _AbletoAccess(strSOPFullPath, nSiteID, dbMgr);
            dbMgr.Close();
            return result;
        }

        // strSOPFullPath 해당하는 SOP를 이 SOPUser 계정으로 제어가 가능한지 여부를 알려준다.
        private bool _AbletoAccess(string strSOPFullPath, int nSiteID, object dbMgr)
        {
            if (m_userType == UserType.Unknown || m_userType == UserType.Master)
                return true;

            SOPLoader loader = new SOPLoader();
            int nDisasterID;

            if (loader.GetLinkedSOP(strSOPFullPath, dbMgr, out nDisasterID) == false)
                return false;

            int nBuildingID = loader.GetDisasterBuilding(nDisasterID, dbMgr);

            if (nBuildingID < 0)
            {
                // 공통 SOP
                return true;
            }
            else
            {
                if (m_nBuildingID == nBuildingID)
                    return true;
            }

            return false;
        }

        public override bool AbleToEditTools()
        {
            return m_userType >= UserType.BuildingAdmin;
        }

        // Site별로 계정등급은 다르게 설정되어 있을수 있다.
        public override int GetUserGrade()
        {
            return (int)m_userType;
        }
    }
}
