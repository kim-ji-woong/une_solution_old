using System;
using System.Collections;
using UnE.Spatial;
using UnE.Sensor;
using DBUtility;

namespace SDMS
{
	public class EditSpringCooler : ChangedData, IDisposable
	{
		private SpringCooler m_Sensor = null;

		public EditSpringCooler(SpringCooler sensor)
		{
			m_Sensor = sensor;

			if (sensor.POI != null)
			{
				m_zone = sensor.POI.Zone;
				m_pos = new UnE.Geometry.Vertex3F(sensor.POI.X, sensor.POI.Y, sensor.POI.Z);
			}
		}

		private Zone m_zone = null;
		private UnE.Geometry.Vertex3F m_pos = null;
		private VariousData<string> m_description = null;

		private bool m_bDelete = false;

		public new bool IsDeleting
		{
			get { return m_bDelete; }
			set { m_bDelete = value; }
		}

		public int ID
		{
			get { return m_Sensor == null ? -1 : m_Sensor.ID; }
		}

        public void Dispose()
        {
            if (m_pos != null)
                m_pos.Dispose();
        }

		private bool Insert(DBUtility.WebDBManager dbMgr)
		{
			POI poi = m_Sensor.POI;
			if (poi == null)
				return false;

			Zone zone = poi.Zone;
			if (zone == null)
				return false;

			string strSQL = "Select max(id) from SpringCooler";
			ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

			if (arrResult == null)
				return false;

			int nID = -1;

			if (arrResult.Count == 0)
				nID = 1;
			else
				nID = DBUtility.WebDBManager.GetIntField(arrResult[0].ToString(), 0) + 1;

			strSQL = string.Format("Insert into SpringCooler (ID, Name, PositionName, X, Y, Z, ZoneID, IsIndoor, Description) values ({0}, '{1}', '{2}', {3}, {4}, {5}, {6}, {7}, NULL)",
                nID, "SpringCooler", zone.DisplayText, poi.X, poi.Y, poi.Z, zone.ID, poi.IsIndoor ? 1 : 0);

			if (dbMgr.GetResultData(strSQL, 0) != null)
			{
				m_Sensor.OrgSensorID = nID;
			}
			else
				return false;

			strSQL = "Select max(id) from SensorZone";
			arrResult = dbMgr.GetResultData(strSQL, 0);

			int nSensorZoneID = -1;
			if (arrResult == null)
				return false;

			nSensorZoneID = DBUtility.WebDBManager.GetIntField(arrResult[0].ToString(), 0) + 1;

			strSQL = string.Format("Insert into SensorZone (ID, Type, Connected, EquipZoneID, Data, Description, OrgSensorID, Zone) values ({0}, {1}, {2}, {3}, {4}, '{5}', {6}, {7})",
			   nSensorZoneID, (int)IFacility.FacilityType.COOLER_SENSOR, "NULL", m_Sensor.EquipZoneID, "NULL", m_Sensor.Description, nID, zone.ID);

			m_Sensor.OrgSensorID = nID;
			m_Sensor.ID = nSensorZoneID;

			if (dbMgr.GetResultData(strSQL, 0) != null)
			{
				poi.UpdateDBData();

				if (SensorManager.Instance.DicSpringCooler.ContainsKey(nID))
					SensorManager.Instance.DicSpringCooler.Remove(nID);
				SensorManager.Instance.DicSpringCooler.Add(nID, m_Sensor);

				if (!SensorManager.Instance.DicSensorZone.ContainsKey(m_Sensor.EquipZoneID))
					SensorManager.Instance.DicSensorZone.Add(m_Sensor.EquipZoneID, new EquipmentZoneObjectList());
				if (!SensorManager.Instance.DicSensorZone[m_Sensor.EquipZoneID].SensorList.Contains(m_Sensor))
					SensorManager.Instance.DicSensorZone[m_Sensor.EquipZoneID].SensorList.Add(m_Sensor);
				return true;
			}
			return false;
		}

		override public bool Update(DBUtility.WebDBManager dbMgr)
		{
			if (m_Sensor == null)
				return false;

			if (m_Sensor.ID < 0 && IsDeleting == false)
				return Insert(dbMgr);

			if (IsDeleting)
			{
				string strSQL = string.Format("Delete from SpringCooler where ID = {0}", m_Sensor.OrgSensorID);
				bool bResult1 = (dbMgr.GetResultData(strSQL, 0) != null);

				if (bResult1 == true)
				{
					strSQL = string.Format("delete From SensorReactionHistory where SensorHistoryID in ( select id from SensorZoneHistory where SensorID = {0} )", m_Sensor.ID);
					bResult1 = (dbMgr.GetResultData(strSQL, 0) != null);

					strSQL = string.Format("delete From SensorZoneHistory where SensorID = {0}", m_Sensor.ID);
					bResult1 = (dbMgr.GetResultData(strSQL, 0) != null);

					strSQL = string.Format("delete From SensorZone where ID = {0}", m_Sensor.ID);
					bResult1 = (dbMgr.GetResultData(strSQL, 0) != null);
				}
				SensorManager.Instance.DicSpringCooler.Remove(m_Sensor.ID);

				if (SensorManager.Instance.DicSensorZone.ContainsKey(m_Sensor.EquipZoneID))
				{
					SensorManager.Instance.DicSensorZone[m_Sensor.EquipZoneID].SensorList.Remove(m_Sensor);
				}

				SensorManager.Instance.DicAllSenor.Remove(m_Sensor.ID);

				return bResult1;
			}

			string strField = "", strValue = "";

			if (m_zone != null)
			{
                strValue = string.Format("PositionName = '{0}', ZoneID = {1}, IsIndoor = {2}", m_zone.DisplayText, m_zone.ID, m_Sensor.POI.IsIndoor ? 1 : 0);
				AddQueryString(ref strField, strValue);
			}

			//if (m_Sensor != null)
			//{
			//    strValue = string.Format("EquipZoneID = '{0}'", m_Sensor.EquipZoneID);
			//    AddQueryString(ref strField, strValue);
			//}

			if (m_pos != null)
			{
				strValue = string.Format("X = {0}, Y = {1}, Z = {2}", m_pos.x, m_pos.y, m_pos.z);
				AddQueryString(ref strField, strValue);
			}

			if (m_description != null)
			{
				strValue = string.Format("Description = '{0}'", m_description.Data);
				AddQueryString(ref strField, strValue);
			}

			if (strField.Length == 0)
				return false;

			string strSQL2 = string.Format("Update SpringCooler set {0} where id = {1}", strField, m_Sensor.OrgSensorID);

			if (dbMgr.GetResultData(strSQL2, 0) != null)
			{
				m_Sensor.POI.UpdateDBData();

				return true;
			}

			return false;
		}

		override public void AddToManager(IChangedDataManager mgr)
		{
			ArrayList arrDatas = mgr.GetDataList();
			Type type = typeof(EditSpringCooler);

			foreach (ChangedData data in arrDatas)
			{
				if (data.GetType() == type)
				{
					EditSpringCooler sensor = (EditSpringCooler)data;

					if (sensor.m_Sensor == this.m_Sensor)
					{
						sensor.m_bDelete = this.m_bDelete;

						if (sensor.ID < 0)
						{
							if (sensor.m_bDelete)
							{
								arrDatas.Remove(data);
								mgr.SomethingChanged(null);
								return;
							}
						}

						if (this.m_zone != null)
							sensor.m_zone = this.m_zone;
						if (this.m_pos != null)
							sensor.m_pos = this.m_pos;
						if (this.m_description != null)
							sensor.m_description = this.m_description;
						return;
					}
				}
			}

			// DB에 저장된 것과 동일한 상태인가?
			if (!IsOriginStatus())
				mgr.SomethingChanged(this);
		}

        public override bool IsOriginStatus()
		{
			if (IsDeleting)
				return false;

			if (m_Sensor == null)
				return false;

			if (this.m_zone != null)
			{
				if (this.m_zone != m_Sensor.POI.ZoneDB)
					return false;
			}

			if (this.m_pos != null)
			{
				if (this.m_pos.GetDistance(new UnE.Geometry.Vertex3F(m_Sensor.POI.XDB, m_Sensor.POI.YDB, m_Sensor.POI.ZDB)) >
					UnE.Geometry.Math.HALF_TOLERANCE())
					return false;
			}

			if (this.m_description != null)
			{
				return false;
			}

			return true;
		}
	}
}