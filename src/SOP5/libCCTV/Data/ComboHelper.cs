using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UnE.CCTV
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
                // FloorIndex가 10000일 경우 특정 층이 아닌 건물 전체를 가르킨다.
                if (floor.FloorIndex >= 10000)
                    continue;

                cmb.Items.Add(floor);
            }
        }
    }
}
