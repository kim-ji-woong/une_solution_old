using System.Collections.Generic;
using dnsDBUtil;

namespace SDMS.DAL
{
    using IDAL;
    using SDMS.Model.Broadcast;
    using SDMS.Model.History;
    using SDMS.Model.Sensor;
    using SDMS.Model.Spatial;
    using SDMS.Model.Alarm;
    using SDMS.Model.CCTV;
    using SDMS.Model.Facility;

    public class DeleteManager : QueryManager, IDelete
    {
        private DataManager m_dataManager = null;
        //private WebDBManager m_dbManager = null;

        public DeleteManager(DataManager dataManager)
        {
            m_dataManager = dataManager;
            m_dbManager = m_dataManager.GetDBManager() as WebDBManager;
        }

        public bool DeleteZone(int id, out string strErrorMessage)
        {
            string strSQL = string.Format("Delete from {0} where ID = {1}", Zone.TableName, id);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            strErrorMessage = null;
            return true;
        }

        public bool DeleteZone(Dictionary<Zone.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            strErrorMessage = null;
            string strCondition = "";

            if (SetCondition<Zone.Fields>(ref strCondition, dicConditions, Zone.GetFieldName, Zone.TableName, ref strErrorMessage) == false)
                return false;

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " and " + strAdditionalConditions;
                else
                    strCondition = strAdditionalConditions;
            }

            string strSQL = string.Format("Delete from {0}", Zone.TableName);

            if (strCondition.Length > 0)
                strSQL += " where " + strCondition;

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            strErrorMessage = null;
            return true;
        }

        public bool DeleteEquipmentZone(int id, out string strErrorMessage)
        {
            string strSQL = string.Format("Delete from {0} where ID = {1}", EquipmentZone.TableName, id);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            strErrorMessage = null;
            return true;
        }

        public bool DeleteEquipmentZone(Dictionary<EquipmentZone.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            strErrorMessage = null;
            string strCondition = "";

            if (SetCondition<EquipmentZone.Fields>(ref strCondition, dicConditions, EquipmentZone.GetFieldName, EquipmentZone.TableName, ref strErrorMessage) == false)
                return false;

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " and " + strAdditionalConditions;
                else
                    strCondition = strAdditionalConditions;
            }

            string strSQL = string.Format("Delete from {0}", EquipmentZone.TableName);

            if (strCondition.Length > 0)
                strSQL += " where " + strCondition;

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            strErrorMessage = null;
            return true;
        }

        public bool DeleteBuildingGroup(int id, out string strErrorMessage)
        {
            string strSQL = string.Format("Delete from {0} where ID = {1}", BuildingGroup.TableName, id);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            strErrorMessage = null;
            return true;
        }

        public bool DeleteBuildingGroup(Dictionary<BuildingGroup.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            strErrorMessage = null;
            string strCondition = "";

            if (SetCondition<BuildingGroup.Fields>(ref strCondition, dicConditions, BuildingGroup.GetFieldName, BuildingGroup.TableName, ref strErrorMessage) == false)
                return false;

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " and " + strAdditionalConditions;
                else
                    strCondition = strAdditionalConditions;
            }

            string strSQL = string.Format("Delete from {0}", BuildingGroup.TableName);

            if (strCondition.Length > 0)
                strSQL += " where " + strCondition;

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            strErrorMessage = null;
            return true;
        }

        public bool DeleteBuilding(int id, out string strErrorMessage)
        {
            string strSQL = string.Format("Delete from {0} where ID = {1}", Building.TableName, id);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            strErrorMessage = null;
            return true;
        }

        public bool DeleteBuilding(Dictionary<Building.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            strErrorMessage = null;
            string strCondition = "";

            if (SetCondition<Building.Fields>(ref strCondition, dicConditions, Building.GetFieldName, Building.TableName, ref strErrorMessage) == false)
                return false;

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " and " + strAdditionalConditions;
                else
                    strCondition = strAdditionalConditions;
            }

            string strSQL = string.Format("Delete from {0}", Building.TableName);

            if (strCondition.Length > 0)
                strSQL += " where " + strCondition;

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            strErrorMessage = null;
            return true;
        }

        public bool DeleteSensorZone(int id, out string strErrorMessage)
        {
            string strSQL = string.Format("Delete from {0} where ID = {1}", SensorZone.TableName, id);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            strErrorMessage = null;
            return true;
        }

        public bool DeleteSensorZone(Dictionary<SensorZone.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            strErrorMessage = null;
            string strCondition = "";

            if (SetCondition<SensorZone.Fields>(ref strCondition, dicConditions, SensorZone.GetFieldName, SensorZone.TableName, ref strErrorMessage) == false)
                return false;

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " and " + strAdditionalConditions;
                else
                    strCondition = strAdditionalConditions;
            }

            string strSQL = string.Format("Delete from {0}", SensorZone.TableName);

            if (strCondition.Length > 0)
                strSQL += " where " + strCondition;

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            strErrorMessage = null;
            return true;
        }

        public bool DeletePSMSensor(int id, out string strErrorMessage)
        {
            string strSQL = string.Format("Delete from {0} where ID = {1}", PSM.TableName, id);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            strErrorMessage = null;
            return true;
        }

        public bool DeletePSMSensor(Dictionary<PSM.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            strErrorMessage = null;
            string strCondition = "";

            if (SetCondition<PSM.Fields>(ref strCondition, dicConditions, PSM.GetFieldName, PSM.TableName, ref strErrorMessage) == false)
                return false;

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " and " + strAdditionalConditions;
                else
                    strCondition = strAdditionalConditions;
            }

            string strSQL = string.Format("Delete from {0}", PSM.TableName);

            if (strCondition.Length > 0)
                strSQL += " where " + strCondition;

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            strErrorMessage = null;
            return true;
        }

        public bool DeletePSMMaterial(int id, out string strErrorMessage)
        {
            string strSQL = string.Format("Delete from {0} where ID = {1}", Material.TableName, id);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            strErrorMessage = null;
            return true;
        }

        public bool DeletePSMMaterial(Dictionary<Material.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            strErrorMessage = null;
            string strCondition = "";

            if (SetCondition<Material.Fields>(ref strCondition, dicConditions, Material.GetFieldName, Material.TableName, ref strErrorMessage) == false)
                return false;

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " and " + strAdditionalConditions;
                else
                    strCondition = strAdditionalConditions;
            }

            string strSQL = string.Format("Delete from {0}", Material.TableName);

            if (strCondition.Length > 0)
                strSQL += " where " + strCondition;

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            strErrorMessage = null;
            return true;
        }

        public bool DeleteFireSensor(int id, out string strErrorMessage)
        {
            string strSQL = string.Format("Delete from {0} where ID = {1}", Fire.TableName, id);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            strErrorMessage = null;
            return true;
        }

        public bool DeleteFireSensor(Dictionary<Fire.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            strErrorMessage = null;
            string strCondition = "";

            if (SetCondition<Fire.Fields>(ref strCondition, dicConditions, Fire.GetFieldName, Fire.TableName, ref strErrorMessage) == false)
                return false;

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " and " + strAdditionalConditions;
                else
                    strCondition = strAdditionalConditions;
            }

            string strSQL = string.Format("Delete from {0}", Fire.TableName);

            if (strCondition.Length > 0)
                strSQL += " where " + strCondition;

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            strErrorMessage = null;
            return true;
        }

        public bool DeleteFacilityType(int id, out string strErrorMessage)
        {
            string strSQL = string.Format("Delete from {0} where ID = {1}", FacilityType.TableName, id);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            strErrorMessage = null;
            return true;
        }

        public bool DeleteFacilityType(Dictionary<FacilityType.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            strErrorMessage = null;
            string strCondition = "";

            if (SetCondition<FacilityType.Fields>(ref strCondition, dicConditions, FacilityType.GetFieldName, FacilityType.TableName, ref strErrorMessage) == false)
                return false;

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " and " + strAdditionalConditions;
                else
                    strCondition = strAdditionalConditions;
            }

            string strSQL = string.Format("Delete from {0}", FacilityType.TableName);

            if (strCondition.Length > 0)
                strSQL += " where " + strCondition;

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            strErrorMessage = null;
            return true;
        }

        public bool DeleteSensorZoneHistory(int id, out string strErrorMessage)
        {
            string strSQL = string.Format("Delete from {0} where ID = {1}", SensorZoneHistory.TableName, id);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            strErrorMessage = null;
            return true;
        }

        public bool DeleteSensorZoneHistory(Dictionary<SensorZoneHistory.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            strErrorMessage = null;
            string strCondition = "";

            if (SetCondition<SensorZoneHistory.Fields>(ref strCondition, dicConditions, SensorZoneHistory.GetFieldName, SensorZoneHistory.TableName, ref strErrorMessage) == false)
                return false;

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " and " + strAdditionalConditions;
                else
                    strCondition = strAdditionalConditions;
            }

            string strSQL = string.Format("Delete from {0}", SensorZoneHistory.TableName);

            if (strCondition.Length > 0)
                strSQL += " where " + strCondition;

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            strErrorMessage = null;
            return true;
        }

        public bool DeleteSensorReactionHistory(int id, out string strErrorMessage)
        {
            string strSQL = string.Format("Delete from {0} where ID = {1}", SensorReactionHistory.TableName, id);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            strErrorMessage = null;
            return true;
        }

        public bool DeleteSensorReactionHistory(Dictionary<SensorReactionHistory.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            strErrorMessage = null;
            string strCondition = "";

            if (SetCondition<SensorReactionHistory.Fields>(ref strCondition, dicConditions, SensorReactionHistory.GetFieldName, SensorReactionHistory.TableName, ref strErrorMessage) == false)
                return false;

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " and " + strAdditionalConditions;
                else
                    strCondition = strAdditionalConditions;
            }

            string strSQL = string.Format("Delete from {0}", SensorReactionHistory.TableName);

            if (strCondition.Length > 0)
                strSQL += " where " + strCondition;

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            strErrorMessage = null;
            return true;
        }

        public bool DeleteSensorReactionHistoryDescription(int id, out string strErrorMessage)
        {
            string strSQL = string.Format("Delete from {0} where ID = {1}", SensorReactionHistoryDescription.TableName, id);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            strErrorMessage = null;
            return true;
        }

        public bool DeleteSensorReactionHistoryDescription(Dictionary<SensorReactionHistoryDescription.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            strErrorMessage = null;
            string strCondition = "";

            if (SetCondition<SensorReactionHistoryDescription.Fields>(ref strCondition, dicConditions, SensorReactionHistoryDescription.GetFieldName, SensorReactionHistoryDescription.TableName, ref strErrorMessage) == false)
                return false;

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " and " + strAdditionalConditions;
                else
                    strCondition = strAdditionalConditions;
            }

            string strSQL = string.Format("Delete from {0}", SensorReactionHistoryDescription.TableName);

            if (strCondition.Length > 0)
                strSQL += " where " + strCondition;

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            strErrorMessage = null;
            return true;
        }

        public bool DeleteSensorReactionHistoryDescriptionText(int id, out string strErrorMessage)
        {
            string strSQL = string.Format("Delete from {0} where ID = {1}", SensorReactionHistoryDescriptionText.TableName, id);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            strErrorMessage = null;
            return true;
        }

        public bool DeleteSensorReactionHistoryDescriptionText(Dictionary<SensorReactionHistoryDescriptionText.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            strErrorMessage = null;
            string strCondition = "";

            if (SetCondition<SensorReactionHistoryDescriptionText.Fields>(ref strCondition, dicConditions, SensorReactionHistoryDescriptionText.GetFieldName, SensorReactionHistoryDescriptionText.TableName, ref strErrorMessage) == false)
                return false;

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " and " + strAdditionalConditions;
                else
                    strCondition = strAdditionalConditions;
            }

            string strSQL = string.Format("Delete from {0}", SensorReactionHistoryDescriptionText.TableName);

            if (strCondition.Length > 0)
                strSQL += " where " + strCondition;

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            strErrorMessage = null;
            return true;
        }

        public bool DeleteBroadcast(int id, out string strErrorMessage)
        {
            string strSQL = string.Format("Delete from {0} where ID = {1}", Broadcast.TableName, id);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            strErrorMessage = null;
            return true;
        }

        public bool DeleteBroadcast(Dictionary<Broadcast.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            strErrorMessage = null;
            string strCondition = "";

            if (SetCondition<Broadcast.Fields>(ref strCondition, dicConditions, Broadcast.GetFieldName, Broadcast.TableName, ref strErrorMessage) == false)
                return false;

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " and " + strAdditionalConditions;
                else
                    strCondition = strAdditionalConditions;
            }

            string strSQL = string.Format("Delete from {0}", Broadcast.TableName);

            if (strCondition.Length > 0)
                strSQL += " where " + strCondition;

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            strErrorMessage = null;
            return true;
        }

        public bool DeleteBroadcastHistory(int id, out string strErrorMessage)
        {
            string strSQL = string.Format("Delete from {0} where ID = {1}", Model.Broadcast.History.TableName, id);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            strErrorMessage = null;
            return true;
        }

        public bool DeleteBroadcastHistory(Dictionary<Model.Broadcast.History.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            strErrorMessage = null;
            string strCondition = "";

            if (SetCondition<Model.Broadcast.History.Fields>(ref strCondition, dicConditions, Model.Broadcast.History.GetFieldName, Model.Broadcast.History.TableName, ref strErrorMessage) == false)
                return false;

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " and " + strAdditionalConditions;
                else
                    strCondition = strAdditionalConditions;
            }

            string strSQL = string.Format("Delete from {0}", Model.Broadcast.History.TableName);

            if (strCondition.Length > 0)
                strSQL += " where " + strCondition;

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            strErrorMessage = null;
            return true;
        }

        public bool DeleteBroadcastState(int id, out string strErrorMessage)
        {
            string strSQL = string.Format("Delete from {0} where ID = {1}", Model.Broadcast.State.TableName, id);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            strErrorMessage = null;
            return true;
        }

        public bool DeleteBroadcastState(Dictionary<Model.Broadcast.State.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            strErrorMessage = null;
            string strCondition = "";

            if (SetCondition<Model.Broadcast.State.Fields>(ref strCondition, dicConditions, Model.Broadcast.State.GetFieldName, Model.Broadcast.State.TableName, ref strErrorMessage) == false)
                return false;

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " and " + strAdditionalConditions;
                else
                    strCondition = strAdditionalConditions;
            }

            string strSQL = string.Format("Delete from {0}", Model.Broadcast.State.TableName);

            if (strCondition.Length > 0)
                strSQL += " where " + strCondition;

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            strErrorMessage = null;
            return true;
        }

        public bool DeleteSMSHistory(int id, out string strErrorMessage)
        {
            string strSQL = string.Format("Delete from {0} where ID = {1}", SMSHistory.TableName, id);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            strErrorMessage = null;
            return true;
        }

        public bool DeleteSMSHistory(Dictionary<SMSHistory.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            strErrorMessage = null;
            string strCondition = "";

            if (SetCondition<SMSHistory.Fields>(ref strCondition, dicConditions, SMSHistory.GetFieldName, SMSHistory.TableName, ref strErrorMessage) == false)
                return false;

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " and " + strAdditionalConditions;
                else
                    strCondition = strAdditionalConditions;
            }

            string strSQL = string.Format("Delete from {0}", SMSHistory.TableName);

            if (strCondition.Length > 0)
                strSQL += " where " + strCondition;

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            strErrorMessage = null;
            return true;
        }

        public bool DeleteBroadcastConfig(int id, out string strErrorMessage)
        {
            string strSQL = string.Format("Delete from {0} where ID = {1}", Model.Config.Broadcast.TableName, id);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            strErrorMessage = null;
            return true;
        }

        public bool DeleteBroadcastConfig(Dictionary<Model.Config.Broadcast.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            strErrorMessage = null;
            string strCondition = "";

            if (SetCondition<Model.Config.Broadcast.Fields>(ref strCondition, dicConditions, Model.Config.Broadcast.GetFieldName, Model.Config.Broadcast.TableName, ref strErrorMessage) == false)
                return false;

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " and " + strAdditionalConditions;
                else
                    strCondition = strAdditionalConditions;
            }

            string strSQL = string.Format("Delete from {0}", Model.Config.Broadcast.TableName);

            if (strCondition.Length > 0)
                strSQL += " where " + strCondition;

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            strErrorMessage = null;
            return true;
        }

        public bool DeleteSMSConfig(int id, out string strErrorMessage)
        {
            string strSQL = string.Format("Delete from {0} where ID = {1}", Model.Config.SMS.TableName, id);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            strErrorMessage = null;
            return true;
        }

        public bool DeleteSMSConfig(Dictionary<Model.Config.SMS.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            strErrorMessage = null;
            string strCondition = "";

            if (SetCondition<Model.Config.SMS.Fields>(ref strCondition, dicConditions, Model.Config.SMS.GetFieldName, Model.Config.SMS.TableName, ref strErrorMessage) == false)
                return false;

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " and " + strAdditionalConditions;
                else
                    strCondition = strAdditionalConditions;
            }

            string strSQL = string.Format("Delete from {0}", Model.Config.SMS.TableName);

            if (strCondition.Length > 0)
                strSQL += " where " + strCondition;

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            strErrorMessage = null;
            return true;
        }

        public bool DeleteCurrentAlarm(Dictionary<CurrentAlarm.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            strErrorMessage = null;
            string strCondition = "";

            if (SetCondition<CurrentAlarm.Fields>(ref strCondition, dicConditions, CurrentAlarm.GetFieldName, CurrentAlarm.TableName, ref strErrorMessage) == false)
                return false;

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " and " + strAdditionalConditions;
                else
                    strCondition = strAdditionalConditions;
            }

            string strSQL = string.Format("Delete from {0}", CurrentAlarm.TableName);

            if (strCondition.Length > 0)
                strSQL += " where " + strCondition;

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            strErrorMessage = null;
            return true;
        }

        public bool DeleteFacilityManager(int id, out string strErrorMessage)
        {
            string strSQL = string.Format("Delete from {0} where ID = {1}", FacilityManager.TableName, id);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            strErrorMessage = null;
            return true;
        }

        public bool DeleteFacilityManager(Dictionary<FacilityManager.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            strErrorMessage = null;
            string strCondition = "";

            if (SetCondition<FacilityManager.Fields>(ref strCondition, dicConditions, FacilityManager.GetFieldName, FacilityManager.TableName, ref strErrorMessage) == false)
                return false;

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " and " + strAdditionalConditions;
                else
                    strCondition = strAdditionalConditions;
            }

            string strSQL = string.Format("Delete from {0}", FacilityManager.TableName);

            if (strCondition.Length > 0)
                strSQL += " where " + strCondition;

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            strErrorMessage = null;
            return true;
        }

        public bool DeleteBuildingFacilityManager(int id, out string strErrorMessage)
        {
            string strSQL = string.Format("Delete from {0} where ID = {1}", BuildingFacilityManager.TableName, id);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            strErrorMessage = null;
            return true;
        }

        public bool DeleteBuildingFacilityManager(Dictionary<BuildingFacilityManager.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            strErrorMessage = null;
            string strCondition = "";

            if (SetCondition<BuildingFacilityManager.Fields>(ref strCondition, dicConditions, BuildingFacilityManager.GetFieldName, BuildingFacilityManager.TableName, ref strErrorMessage) == false)
                return false;

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " and " + strAdditionalConditions;
                else
                    strCondition = strAdditionalConditions;
            }

            string strSQL = string.Format("Delete from {0}", BuildingFacilityManager.TableName);

            if (strCondition.Length > 0)
                strSQL += " where " + strCondition;

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            strErrorMessage = null;
            return true;
        }

        public bool DeleteEquipZoneFacilityManager(int id, out string strErrorMessage)
        {
            string strSQL = string.Format("Delete from {0} where ID = {1}", EquipZoneFacilityManager.TableName, id);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            strErrorMessage = null;
            return true;
        }

        public bool DeleteEquipZoneFacilityManager(Dictionary<EquipZoneFacilityManager.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            strErrorMessage = null;
            string strCondition = "";

            if (SetCondition<EquipZoneFacilityManager.Fields>(ref strCondition, dicConditions, EquipZoneFacilityManager.GetFieldName, EquipZoneFacilityManager.TableName, ref strErrorMessage) == false)
                return false;

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " and " + strAdditionalConditions;
                else
                    strCondition = strAdditionalConditions;
            }

            string strSQL = string.Format("Delete from {0}", EquipZoneFacilityManager.TableName);

            if (strCondition.Length > 0)
                strSQL += " where " + strCondition;

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            strErrorMessage = null;
            return true;
        }

        public bool DeleteCCTV(int id, out string strErrorMessage)
        {
            string strSQL = string.Format("Delete from {0} where ID = {1}", CCTV.TableName, id);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            strErrorMessage = null;
            return true;
        }

        public bool DeleteCCTV(Dictionary<CCTV.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            strErrorMessage = null;
            string strCondition = "";

            if (SetCondition<CCTV.Fields>(ref strCondition, dicConditions, CCTV.GetFieldName, CCTV.TableName, ref strErrorMessage) == false)
                return false;

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " and " + strAdditionalConditions;
                else
                    strCondition = strAdditionalConditions;
            }

            string strSQL = string.Format("Delete from {0}", CCTV.TableName);

            if (strCondition.Length > 0)
                strSQL += " where " + strCondition;

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            strErrorMessage = null;
            return true;
        }

        public bool DeleteEquipZoneCCTV(int id, out string strErrorMessage)
        {
            string strSQL = string.Format("Delete from {0} where ID = {1}", EquipZoneCCTV.TableName, id);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            strErrorMessage = null;
            return true;
        }

        public bool DeleteEquipZoneCCTV(Dictionary<EquipZoneCCTV.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            strErrorMessage = null;
            string strCondition = "";

            if (SetCondition<EquipZoneCCTV.Fields>(ref strCondition, dicConditions, EquipZoneCCTV.GetFieldName, EquipZoneCCTV.TableName, ref strErrorMessage) == false)
                return false;

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " and " + strAdditionalConditions;
                else
                    strCondition = strAdditionalConditions;
            }

            string strSQL = string.Format("Delete from {0}", EquipZoneCCTV.TableName);

            if (strCondition.Length > 0)
                strSQL += " where " + strCondition;

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            strErrorMessage = null;
            return true;
        }

        public bool DeleteGltfModel(int id, out string strErrorMessage)
        {
            string strSQL = string.Format("Delete from {0} where ID = {1}", Model.GLTF.Model.TableName, id);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            strErrorMessage = null;
            return true;
        }

        public bool DeleteGltfModel(Dictionary<Model.GLTF.Model.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            strErrorMessage = null;
            string strCondition = "";

            if (SetCondition<Model.GLTF.Model.Fields>(ref strCondition, dicConditions, Model.GLTF.Model.GetFieldName, Model.GLTF.Model.TableName, ref strErrorMessage) == false)
                return false;

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " and " + strAdditionalConditions;
                else
                    strCondition = strAdditionalConditions;
            }

            string strSQL = string.Format("Delete from {0}", Model.GLTF.Model.TableName);

            if (strCondition.Length > 0)
                strSQL += " where " + strCondition;

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            strErrorMessage = null;
            return true;
        }

        public bool DeleteGltfModelData(int id, out string strErrorMessage)
        {
            string strSQL = string.Format("Delete from {0} where ID = {1}", Model.GLTF.ModelData.TableName, id);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            strErrorMessage = null;
            return true;
        }

        public bool DeleteGltfModelData(Dictionary<Model.GLTF.ModelData.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            strErrorMessage = null;
            string strCondition = "";

            if (SetCondition<Model.GLTF.ModelData.Fields>(ref strCondition, dicConditions, Model.GLTF.ModelData.GetFieldName, Model.GLTF.ModelData.TableName, ref strErrorMessage) == false)
                return false;

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " and " + strAdditionalConditions;
                else
                    strCondition = strAdditionalConditions;
            }

            string strSQL = string.Format("Delete from {0}", Model.GLTF.ModelData.TableName);

            if (strCondition.Length > 0)
                strSQL += " where " + strCondition;

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            strErrorMessage = null;
            return true;
        }

        public bool DeleteGltfModelOrthoData(int id, out string strErrorMessage)
        {
            string strSQL = string.Format("Delete from {0} where ID = {1}", Model.GLTF.ModelOrthoData.TableName, id);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            strErrorMessage = null;
            return true;
        }

        public bool DeleteGltfModelOrthoData(Dictionary<Model.GLTF.ModelOrthoData.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            strErrorMessage = null;
            string strCondition = "";

            if (SetCondition<Model.GLTF.ModelOrthoData.Fields>(ref strCondition, dicConditions, Model.GLTF.ModelOrthoData.GetFieldName, Model.GLTF.ModelOrthoData.TableName, ref strErrorMessage) == false)
                return false;

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " and " + strAdditionalConditions;
                else
                    strCondition = strAdditionalConditions;
            }

            string strSQL = string.Format("Delete from {0}", Model.GLTF.ModelOrthoData.TableName);

            if (strCondition.Length > 0)
                strSQL += " where " + strCondition;

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            strErrorMessage = null;
            return true;
        }

        public bool DeleteOptionEtcSensor(int sensorType, out string strErrorMessage)
        {
            bool isNullable;
            string strSQL = string.Format("Delete from {0} where {1} = {2}",
                Model.Sensor.Option.Etc.TableName,
                Model.Sensor.Option.Etc.GetFieldName(Model.Sensor.Option.Etc.Fields.SensorType, out isNullable),
                sensorType);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            strErrorMessage = null;
            return true;
        }

        public bool DeleteEtcSensor(Dictionary<Model.Sensor.Option.Etc.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            strErrorMessage = null;
            string strCondition = "";

            if (SetCondition<Model.Sensor.Option.Etc.Fields>(ref strCondition, dicConditions, Model.Sensor.Option.Etc.GetFieldName, Model.Sensor.Option.Etc.TableName, ref strErrorMessage) == false)
                return false;

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " and " + strAdditionalConditions;
                else
                    strCondition = strAdditionalConditions;
            }

            string strSQL = string.Format("Delete from {0}", Model.Sensor.Option.Etc.TableName);

            if (strCondition.Length > 0)
                strSQL += " where " + strCondition;

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            strErrorMessage = null;
            return true;
        }

        public bool DeleteOptionEtcSensorData(int sensorType, int alarmDepth, out string strErrorMessage)
        {
            bool isNullable;
            string strSQL = string.Format("Delete from {0} where {1} = {2} and {3} = {4}",
                Model.Sensor.Option.EtcData.TableName,
                Model.Sensor.Option.EtcData.GetFieldName(Model.Sensor.Option.EtcData.Fields.SensorType, out isNullable),
                sensorType,
                Model.Sensor.Option.EtcData.GetFieldName(Model.Sensor.Option.EtcData.Fields.AlarmDepth, out isNullable),
                alarmDepth);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            strErrorMessage = null;
            return true;
        }

        public bool DeleteEtcSensorData(Dictionary<Model.Sensor.Option.EtcData.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            strErrorMessage = null;
            string strCondition = "";

            if (SetCondition<Model.Sensor.Option.EtcData.Fields>(ref strCondition, dicConditions, Model.Sensor.Option.EtcData.GetFieldName, Model.Sensor.Option.EtcData.TableName, ref strErrorMessage) == false)
                return false;

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " and " + strAdditionalConditions;
                else
                    strCondition = strAdditionalConditions;
            }

            string strSQL = string.Format("Delete from {0}", Model.Sensor.Option.EtcData.TableName);

            if (strCondition.Length > 0)
                strSQL += " where " + strCondition;

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            strErrorMessage = null;
            return true;
        }

        public bool DeleteFacilityInfo(int id, out string strErrorMessage)
        {
            bool isNullable;
            string strSQL = string.Format("Delete from {0} where {1} = {2}",
                Info.TableName,
                Info.GetFieldName(Info.Fields.ID, out isNullable),
                id);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            strErrorMessage = null;
            return true;
        }

        public bool DeleteFacilityInfo(Dictionary<Info.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            strErrorMessage = null;
            string strCondition = "";

            if (SetCondition<Info.Fields>(ref strCondition, dicConditions, Info.GetFieldName, Info.TableName, ref strErrorMessage) == false)
                return false;

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " and " + strAdditionalConditions;
                else
                    strCondition = strAdditionalConditions;
            }

            string strSQL = string.Format("Delete from {0}", Info.TableName);

            if (strCondition.Length > 0)
                strSQL += " where " + strCondition;

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            strErrorMessage = null;
            return true;
        }

        public bool DeleteFacilityInfoData(int nFacilityInfoID, int nOrderIndex, out string strErrorMessage)
        {
            bool isNullable;
            string strSQL = string.Format("Delete from {0} where {1} = {2} and {3} = {4}",
                InfoData.TableName,
                InfoData.GetFieldName(InfoData.Fields.FacilityInfoID, out isNullable),
                nFacilityInfoID,
                InfoData.GetFieldName(InfoData.Fields.OrderIndex, out isNullable),
                nOrderIndex);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            strErrorMessage = null;
            return true;
        }

        public bool DeleteFacilityInfoData(Dictionary<InfoData.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            strErrorMessage = null;
            string strCondition = "";

            if (SetCondition<InfoData.Fields>(ref strCondition, dicConditions, InfoData.GetFieldName, InfoData.TableName, ref strErrorMessage) == false)
                return false;

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " and " + strAdditionalConditions;
                else
                    strCondition = strAdditionalConditions;
            }

            string strSQL = string.Format("Delete from {0}", InfoData.TableName);

            if (strCondition.Length > 0)
                strSQL += " where " + strCondition;

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            strErrorMessage = null;
            return true;
        }

        public bool DeleteBuildingData(int nBuildingID, int nOrderIndex, out string strErrorMessage)
        {
            bool isNullable;
            string strSQL = string.Format("Delete from {0} where {1} = {2} and {3} = {4}",
                BuildingData.TableName,
                BuildingData.GetFieldName(BuildingData.Fields.BuildingID, out isNullable),
                nBuildingID,
                BuildingData.GetFieldName(BuildingData.Fields.OrderIndex, out isNullable),
                nOrderIndex);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            strErrorMessage = null;
            return true;
        }

        public bool DeleteBuildingData(Dictionary<BuildingData.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            strErrorMessage = null;
            string strCondition = "";

            if (SetCondition<BuildingData.Fields>(ref strCondition, dicConditions, BuildingData.GetFieldName, BuildingData.TableName, ref strErrorMessage) == false)
                return false;

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " and " + strAdditionalConditions;
                else
                    strCondition = strAdditionalConditions;
            }

            string strSQL = string.Format("Delete from {0}", BuildingData.TableName);

            if (strCondition.Length > 0)
                strSQL += " where " + strCondition;

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            strErrorMessage = null;
            return true;
        }

        public bool DeleteBuildingGroupData(int nBuildingGroupID, int nOrderIndex, out string strErrorMessage)
        {
            bool isNullable;
            string strSQL = string.Format("Delete from {0} where {1} = {2} and {3} = {4}",
                BuildingGroupData.TableName,
                BuildingGroupData.GetFieldName(BuildingGroupData.Fields.BuildingGroupID, out isNullable),
                nBuildingGroupID,
                BuildingGroupData.GetFieldName(BuildingGroupData.Fields.OrderIndex, out isNullable),
                nOrderIndex);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            strErrorMessage = null;
            return true;
        }

        public bool DeleteBuildingGroupData(Dictionary<BuildingGroupData.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            strErrorMessage = null;
            string strCondition = "";

            if (SetCondition<BuildingGroupData.Fields>(ref strCondition, dicConditions, BuildingGroupData.GetFieldName, BuildingGroupData.TableName, ref strErrorMessage) == false)
                return false;

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " and " + strAdditionalConditions;
                else
                    strCondition = strAdditionalConditions;
            }

            string strSQL = string.Format("Delete from {0}", BuildingGroupData.TableName);

            if (strCondition.Length > 0)
                strSQL += " where " + strCondition;

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            strErrorMessage = null;
            return true;
        }

        public bool DeleteFakeWall(int id, out string strErrorMessage)
        {
            bool isNullable;
            string strSQL = string.Format("Delete from {0} where {1} = {2}",
                FakeWall.TableName,
                FakeWall.GetFieldName(FakeWall.Fields.ID, out isNullable),
                id);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            strErrorMessage = null;
            return true;
        }

        public bool DeleteFakeWall(Dictionary<FakeWall.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            strErrorMessage = null;
            string strCondition = "";

            if (SetCondition<FakeWall.Fields>(ref strCondition, dicConditions, FakeWall.GetFieldName, FakeWall.TableName, ref strErrorMessage) == false)
                return false;

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " and " + strAdditionalConditions;
                else
                    strCondition = strAdditionalConditions;
            }

            string strSQL = string.Format("Delete from {0}", FakeWall.TableName);

            if (strCondition.Length > 0)
                strSQL += " where " + strCondition;

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            strErrorMessage = null;
            return true;
        }

        public bool DeleteSpreadMessage(int id, out string strErrorMessage)
        {
            string strSQL = string.Format("Delete from {0} where ID = {1}", Model.Config.SpreadMessage.TableName, id);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            strErrorMessage = null;
            return true;
        }

        public bool DeleteSpreadMessage(Dictionary<Model.Config.SpreadMessage.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            strErrorMessage = null;
            string strCondition = "";

            if (SetCondition<Model.Config.SpreadMessage.Fields>(ref strCondition, dicConditions, Model.Config.SpreadMessage.GetFieldName, Model.Config.SpreadMessage.TableName, ref strErrorMessage) == false)
                return false;

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " and " + strAdditionalConditions;
                else
                    strCondition = strAdditionalConditions;
            }

            string strSQL = string.Format("Delete from {0}", Model.Config.SpreadMessage.TableName);

            if (strCondition.Length > 0)
                strSQL += " where " + strCondition;

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            strErrorMessage = null;
            return true;
        }

        public bool DeleteZoneData(int zoneID, out string strErrorMessage)
        {
            bool isNullable;
            string strSQL = string.Format("Delete from {0} where ID = {1}", ZoneData.TableName, ZoneData.GetFieldName(ZoneData.Fields.ZoneID, out isNullable), zoneID);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            strErrorMessage = null;
            return true;
        }

        public bool DeleteZoneData(Dictionary<ZoneData.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            strErrorMessage = null;
            string strCondition = "";

            if (SetCondition<ZoneData.Fields>(ref strCondition, dicConditions, ZoneData.GetFieldName, ZoneData.TableName, ref strErrorMessage) == false)
                return false;

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " and " + strAdditionalConditions;
                else
                    strCondition = strAdditionalConditions;
            }

            string strSQL = string.Format("Delete from {0}", ZoneData.TableName);

            if (strCondition.Length > 0)
                strSQL += " where " + strCondition;

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            strErrorMessage = null;
            return true;
        }
    }
}
