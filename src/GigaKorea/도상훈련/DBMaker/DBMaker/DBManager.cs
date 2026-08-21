using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FireSimulator;
using DBUtility;
using System.Collections;

namespace DBMaker
{
    public class DBManager
    {
        private static string m_strErrorMessage = "";

        public static string ErrorMessage
        {
            get { return m_strErrorMessage; }
        }

        public static bool SetData(Project project)
        {
            m_strErrorMessage = "";
            WebDBManager dbMgr = new WebDBManager(1);

            if (ClearData(dbMgr) == false)
                return false;

            int nBuildingID = MakeBuilding(dbMgr, project);

            if (nBuildingID < 0)
                return false;

            if (MakeZone(dbMgr, project, nBuildingID) == false)
                return false;

            return true;
        }

        private static bool MakeZone(WebDBManager dbMgr, Project project, int nBuildingID)
        {
            int nFloorIndex = 0;
            int nZoneIndex = 1;
            int nTotalCount = 0;

            foreach (Level level in project.Levels)
            {
                nTotalCount += level.Spaces.Count;
            }

            foreach (Level level in project.Levels)
            {
                foreach (Space space in level.Spaces)
                {
                    if (MakeZone(dbMgr, space, nBuildingID, nFloorIndex, nZoneIndex++) == false)
                        return false;
                    else
                    {
                        string strLog = string.Format("MakeZone {0} / {1}", nZoneIndex - 1, nTotalCount);
                        FormMain.Instance.ChangeStatus(strLog);
                        //System.Diagnostics.Trace.WriteLine(strLog);
                    }
                }

                nFloorIndex++;
            }

            return true;
        }

        private static bool MakeZone(WebDBManager dbMgr, Space space, int nBuildingID, int nFloorIndex, int nZoneID)
        {
            string strSQL = "Insert into Zone (ID, ZoneName, SiteID, BuildingID, FloorIndex, AddFloor, Boundary, DXFFileName, DXFAccessedTime, _3DFileName, _3DAccessedTime, TextCenter, BroadcastName, DxfTL, DxfBR, ImgTL, ImgBR, Azimuth, Scale, DisplayText) values ";
            strSQL += string.Format("({0}, '{1}', 1, {2}, {3}, NULL, NULL, NULL, NULL, NULL, NULL, NULL, '{4}', NULL, NULL, NULL, NULL, NULL, NULL, '{1}')", nZoneID, space.ID, nBuildingID, nFloorIndex, space.Name);

            if (dbMgr.GetResultData(strSQL, 0) == null)
            {
                m_strErrorMessage = dbMgr.LastErrorMessage;
                return false;
            }

            strSQL = "Insert into EquipmentZone (ID, ZoneName, Boundary, LinkedZoneIDList, Type, BroadcastName, TextCenter, Description, DisplayText, SiteID) values ";
            strSQL += string.Format("({0}, '{1}', '', '{0}', 0, '{1}', NULL, NULL, NULL, 1)", nZoneID, space.Name);

            if (dbMgr.GetResultData(strSQL, 0) == null)
            {
                m_strErrorMessage = dbMgr.LastErrorMessage;
                return false;
            }

            strSQL = "Insert into SensorZone (ID, Type, Connected, EquipZoneID, Data, Description, OrgSensorID, Zone) values ";
            strSQL += string.Format("({0}, 0, NULL, {0}, 1, NULL, NULL, NULL)", nZoneID);

            if (dbMgr.GetResultData(strSQL, 0) == null)
            {
                m_strErrorMessage = dbMgr.LastErrorMessage;
                return false;
            }

            return true;
        }

        private static int MakeBuilding(WebDBManager dbMgr, Project project)
        {
            string strSQL = string.Format("Insert into BuildingGroup (ID, GroupName, SiteID, TextCenter, DisplayText) values (1, '{0}', 1, NULL, NULL)", project.Name);

            if (dbMgr.GetResultData(strSQL, 0) == null)
            {
                m_strErrorMessage = dbMgr.LastErrorMessage;
                return -1;
            }

            strSQL = "Insert into Building (ID, BuildingID, BuildingCode, BuildingName, BuildingGroupID, MaxFloor, MinFloor, BroadCastingText, DisplayText) values ";
            strSQL += string.Format("(1, '1', '1', '{0}', 1, {1}, 0, NULL, NULL)", project.Name, project.Levels.Count);

            if (dbMgr.GetResultData(strSQL, 0) == null)
            {
                m_strErrorMessage = dbMgr.LastErrorMessage;
                return -1;
            }

            return 1;
        }

        private static bool ClearData(WebDBManager dbMgr)
        {
            if (ClearTable(dbMgr, "BuildingFacilityManager") == false)
                return false;

            if (ClearTable(dbMgr, "EquipZoneFacilityManager") == false)
                return false;

            if (ClearTable(dbMgr, "FacilityManager") == false)
                return false;

            if (ClearTable(dbMgr, "SensorZoneHistory") == false)
                return false;

            if (ClearTable(dbMgr, "SensorZone") == false)
                return false;

            if (ClearTable(dbMgr, "EquipmentZone") == false)
                return false;

            if (ClearTable(dbMgr, "Zone") == false)
                return false;

            if (ClearTable(dbMgr, "Building") == false)
                return false;

            if (ClearTable(dbMgr, "BuildingGroup") == false)
                return false;

            return true;
        }

        private static bool ClearTable(WebDBManager dbMgr, string strTableName)
        {
            string strSQL = "Delete from " + strTableName;

            if (dbMgr.GetResultData(strSQL, 0) == null)
            {
                m_strErrorMessage = dbMgr.LastErrorMessage;
                return false;
            }

            return true;
        }
    }
}
