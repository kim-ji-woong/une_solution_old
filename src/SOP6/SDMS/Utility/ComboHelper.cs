using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using UnE.Spatial;
using UnE.Sensor;
using UnE.Util.Unity;

namespace SDMS
{
	public class ComboHelper
	{
		public static void InitBuildingGroupComboBox(ComboBox cmb)
		{
			foreach (KeyValuePair<int, BuildingGroup> pair in ZoneManager.Instance.DicBuildingGroup)
			{
				cmb.Items.Add(pair.Value);
			}
		}

		public static void InitBuildingComboBox(ComboBox cmb, BuildingGroup group)
		{
			cmb.Items.Clear();
			if (group.GroupID > 0)
			{
				ArrayList arrBuildings = group.BuildingList;
				if (arrBuildings == null)
					return;
				foreach (Building building in arrBuildings)
				{
					ArrayList arrFloors = building.FloorList;

					if (arrFloors != null && arrFloors.Count > 0)
					{
						// Zone이 하나도 없는 빌딩, 즉 도면이 하나도 없는 빌딩은 콤보박스에 보여주지 않는다.
						cmb.Items.Add(building);
					}
				}
			}
			else
			{
				foreach (KeyValuePair<int, Zone> pair in ZoneManager.Instance.DicOutdoorZones)
				{
					cmb.Items.Add(pair.Value);
				}
			}
		}

		public static void InitFloorComboBox(ComboBox cmb, Building building)
		{
			cmb.Items.Clear();
			ArrayList arrZones = ZoneManager.Instance.GetZoneList(building.ID);
			ArrayList arrFloor = new ArrayList();
			foreach (Zone zone in arrZones)
			{
				Floor floor = new Floor(zone.FloorIndex + zone.AddFloor);
				floor.Zone = zone;
				arrFloor.Add(floor);
			}
			arrFloor.Sort();
			foreach (Floor floor in arrFloor)
			{
				cmb.Items.Add(floor);
			}
		}
	}

	public class ComboItemDate : Object
	{
		private int nID = -1;
		private int nZoneHistoryID = -1;
		private int nReactionType = 0;
		private int nSensorType = -1; //센서타입이 0이면 수동신고, 1이면 자탐센서

		public int ReactionType
		{
			get { return nReactionType; }
			set { nReactionType = value; }
		}

		public int ZoneHistoryID
		{
			get { return nZoneHistoryID; }
			set { nZoneHistoryID = value; }
		}

		public int ID
		{
			get { return nID; }
			set { nID = value; }
		}

		private DateTime date;

		public DateTime Date
		{
			get { return date; }
			set { date = value; }
		}

		public int SensorType
		{
			get { return nSensorType; }
			set { nSensorType = value; }
		}

		public override string ToString()
		{
			string strReactionType = "";
			if (nReactionType == 22)
				strReactionType = "화재 발생";
			else if (nReactionType == 21)
				strReactionType = "오작동 처리";
			else if (nReactionType == 23)
				strReactionType = "화재탐지 후 상황해제";

			if (nSensorType == 0)
				return date.ToString() + "    [ 수동 신고 ] " + strReactionType;
			else
				return date.ToString() + "    [ 자탐 ] " + strReactionType;
		}
	}
}