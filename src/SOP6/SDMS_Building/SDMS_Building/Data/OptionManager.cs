using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBUtility2;
using UnE.Spatial;
using UnE.Geometry;

namespace SDMS_Building.Data
{
    public class OptionManager
    {
        public void ReadUsageStatus()
        {
            string[] types = new string[] { "'UseFire'", "'UsePSM'", "'UseIntrusion'", "'UseEarthquake'", "'UseFirewall'", "'UseDoor'", "'UseBlackout'", "'UseStrongWind'", "'UseTerror'", "'UseSubmergency'" };

            // 사용하는 센서 종류 읽어오기
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT PropertyName, PropertyValue ");
            sb.Append("  FROM OptionSDMS ");
            sb.AppendFormat("WHERE PropertyName IN ({0})", string.Join(", ", types));
            sb.AppendFormat("  AND SiteID = {0}", UnE.SOP.ProxySOP.Instance.SiteID);

            string strSQL = sb.ToString();
            ArrayList arrResult = FormMain.Instance.DBManager.GetResultData(strSQL);

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 1; i += 2)
            {
                string strSensorName = WebDBManager.GetStringField(arrResult[i]);
                VariousData<int> useFlag = WebDBManager.GetIntField(arrResult[i + 1].ToString());

                if (strSensorName == "UsePSM")
                {
                    if (useFlag.Data == 1)
                        UnE.SOP.ProxySOP.Instance.UsePSM = true;
                    else
                        UnE.SOP.ProxySOP.Instance.UsePSM = false;
                }

                if (strSensorName == "UseIntrusion")
                {
                    if (useFlag.Data == 1)
                        UnE.SOP.ProxySOP.Instance.UseIntrusion = true;
                    else
                        UnE.SOP.ProxySOP.Instance.UseIntrusion = false;
                }

                if (strSensorName == "UseEarthquake")
                {
                    if (useFlag.Data == 1)
                        UnE.SOP.ProxySOP.Instance.UseEarthquake = true;
                    else
                        UnE.SOP.ProxySOP.Instance.UseEarthquake = false;
                }

                if (strSensorName == "UseFirewall")
                {
                    if (useFlag.Data == 1)
                        UnE.SOP.ProxySOP.Instance.UseFirewall = true;
                    else
                        UnE.SOP.ProxySOP.Instance.UseFirewall = false;
                }

                if (strSensorName == "UseDoor")
                {
                    if (useFlag.Data == 1)
                        UnE.SOP.ProxySOP.Instance.UseDoor = true;
                    else
                        UnE.SOP.ProxySOP.Instance.UseDoor = false;
                }

                if (strSensorName == "UseBlackout")
                {
                    if (useFlag.Data == 1)
                        UnE.SOP.ProxySOP.Instance.UseBlackout = true;
                    else
                        UnE.SOP.ProxySOP.Instance.UseBlackout = false;
                }

                if (strSensorName == "UseStrongWind")
                {
                    if (useFlag.Data == 1)
                        UnE.SOP.ProxySOP.Instance.UseStrongWind = true;
                    else
                        UnE.SOP.ProxySOP.Instance.UseStrongWind = false;
                }

                if (strSensorName == "UseTerror")
                {
                    if (useFlag.Data == 1)
                        UnE.SOP.ProxySOP.Instance.UseTerror = true;
                    else
                        UnE.SOP.ProxySOP.Instance.UseTerror = false;
                }

                if (strSensorName == "UseSubmergency")
                {
                    if (useFlag.Data == 1)
                        UnE.SOP.ProxySOP.Instance.UseSubmergency = true;
                    else
                        UnE.SOP.ProxySOP.Instance.UseSubmergency = false;
                }                
            }

            /*
            UnE.SOP.ProxySOP.Instance.UsePSM = true;
            UnE.SOP.ProxySOP.Instance.UseIntrusion = true;
            UnE.SOP.ProxySOP.Instance.UseEarthquake = true;
            UnE.SOP.ProxySOP.Instance.UseFirewall = true;
            UnE.SOP.ProxySOP.Instance.UseDoor = true;
            UnE.SOP.ProxySOP.Instance.UseBlackout = true;
            UnE.SOP.ProxySOP.Instance.UseStrongWind = true;
            UnE.SOP.ProxySOP.Instance.UseTerror = true;
            */
        }

        public bool UseFacilityManagerType(WebDBManager dbMgr)
        {
            string strSQL = "Select PropertyValue from OptionSDMS where PropertyName = 'UseFacilityManagerType' and SiteID = " + UnE.SOP.ProxySOP.Instance.SiteID.ToString();
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count == 0)
                return false;

            string strValue = WebDBManager.GetStringField(arrResult[0]);

            if (strValue == null)
                return false;

            strValue = strValue.Trim();

            if (strValue == "1" || string.Compare(strValue, "true", true) == 0)
            {
                return true;
            }

            return false;
        }

        #region EquipZoneVolume
        private bool m_useEquipZoneVolume = false;
        public bool UseEquipZoneVolume
        {
            get { return m_useEquipZoneVolume; }
        }
        // Key : EquipZoneID
        // Value : VolumeName
        private Dictionary<int, string> m_dicEquiZoneVolume = new Dictionary<int, string>();
        public Dictionary<int, string> DicEquiZoneVolume
        {
            get { return m_dicEquiZoneVolume; }
        }

        // Key : EquipZoneID
        // Value : Text Center
        private Dictionary<int, UnE.Geometry.Vertex3F> m_dicEquipZoneTextCenter = new Dictionary<int, UnE.Geometry.Vertex3F>();
        public Dictionary<int, string> EquipZoneTextCenter
        {
            get { return m_dicEquiZoneVolume; }
        }

        // Key : Zone ID
        // Value : SceneName
        private Dictionary<int, string> m_dicZoneScene = new Dictionary<int, string>();
        public Dictionary<int, string> DicZoneScene
        {
            get { return m_dicZoneScene; }
        }

        // Key : BuildingGroup ID
        // Value : Scene Name
        private Dictionary<int, string> m_dicBuildingGroupScene = new Dictionary<int, string>();
        public Dictionary<int, string> DicBuildingGroupScene
        {
            get { return m_dicBuildingGroupScene; }
        }
        #endregion

        public void ReadEquipZoneVolumeOption()
        {
            string strSQL = "Select PropertyValue from OptionSDMS where PropertyName = 'UseEquipZoneVolume' and SiteID = " + UnE.SOP.ProxySOP.Instance.SiteID.ToString();

            ArrayList arrResult = FormMain.Instance.DBManager.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count == 0)
                return;

            string strPropertyValue = WebDBManager.GetStringField(arrResult[0]);

            if (strPropertyValue == null)
                return;

            if (strPropertyValue == "1" || string.Compare("true", strPropertyValue, true) == 0)
            {
                m_useEquipZoneVolume = true;
                ReadEquipZoneVolume();
                ReadZoneScene();
                ReadBuildingGroupScene();
            }
        }

        private void ReadEquipZoneVolume()
        {
            string strSQL = "Select ez.ID, ezv.VolumeName, ez.TextCenter from EquipmentZone as ez, EquipZoneVolume as ezv where ez.ID = ezv.EquipZoneID";
            ArrayList arrResult = FormMain.Instance.DBManager.GetResultData(strSQL);

            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 2; i += 3)
            {
                VariousData<int> equipZoneID = WebDBManager.GetIntField(arrResult[i].ToString());
                string strVolumeName = WebDBManager.GetStringField(arrResult[i + 1]);
                string strTextCenter = WebDBManager.GetStringField(arrResult[i + 2]);

                if (equipZoneID == null || strVolumeName == null)
                    continue;

                m_dicEquiZoneVolume[equipZoneID.Data] = strVolumeName;

                UnE.Geometry.Vertex3F vCenter = ReadVertex3F(strTextCenter);

                if (vCenter != null)
                    m_dicEquipZoneTextCenter[equipZoneID.Data] = vCenter;
            }
        }

        private UnE.Geometry.Vertex3F ReadVertex3F(string strVertex)
        {
            if (strVertex == null)
                return null;

            string[] tokens = strVertex.Split(',');

            if (tokens.Count() != 3)
                return null;

            float x, y, z;

            if (float.TryParse(tokens[0].Trim(), out x) == false || float.TryParse(tokens[1].Trim(), out y) == false || float.TryParse(tokens[2].Trim(), out z) == false)
                return null;

            UnE.Geometry.Vertex3F vertex = new UnE.Geometry.Vertex3F(x, y, z);
            return vertex;
        }

        private void ReadZoneScene()
        {
            string strSQL = "Select Zone.ID, zs.SceneName from Zone, ZoneScene as zs where Zone.ID = zs.ZoneID";
            ArrayList arrResult = FormMain.Instance.DBManager.GetResultData(strSQL);

            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 1; i += 2)
            {
                VariousData<int> zoneID = WebDBManager.GetIntField(arrResult[i].ToString());
                string strSceneName = WebDBManager.GetStringField(arrResult[i + 1]);

                if (zoneID == null || strSceneName == null)
                    continue;

                m_dicZoneScene[zoneID.Data] = strSceneName;
            }
        }

        private void ReadBuildingGroupScene()
        {
            string strSQL = "Select bg.ID, bgs.SceneName from BuildingGroup as bg, BuildingGroupScene as bgs where bg.ID = bgs.BuildingGroupID";
            ArrayList arrResult = FormMain.Instance.DBManager.GetResultData(strSQL);

            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 1; i += 2)
            {
                VariousData<int> buildingGroupID = WebDBManager.GetIntField(arrResult[i].ToString());
                string strSceneName = WebDBManager.GetStringField(arrResult[i + 1]);

                if (buildingGroupID == null || strSceneName == null)
                    continue;

                m_dicBuildingGroupScene[buildingGroupID.Data] = strSceneName;
            }
        }

        public void AddEquipZoneText(UnE.Util.Unity.Panel4Unity panel)
        {
            string strSceneName = "";
            List<string> cmdList = new List<string>();

            foreach (KeyValuePair<int, Vertex3F> pair in m_dicEquipZoneTextCenter)
            {
                EquipmentZone equipZone = ZoneManager.Instance.GetEquipZone(pair.Key);

                if (equipZone != null)
                {
                    if (equipZone.LinkedZoneList.Count == 0)
                        continue;

                    Zone zone = (Zone)equipZone.LinkedZoneList[0];

                    if (m_dicZoneScene.TryGetValue(zone.ID, out strSceneName) == false)
                        continue;

                    string strCmd = string.Format("{0}_{1},{2},{3},{4}", strSceneName, equipZone.DisplayText, pair.Value.x, pair.Value.y, pair.Value.z);
                    cmdList.Add(strCmd);
                    //panel.AddGroupName(strSceneName + "_" + equipZone.DisplayText, pair.Value.x, pair.Value.y, pair.Value.z);
                }
            }

            string strFilePath = System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\AddEquipZoneText.txt";
            panel.AddGroupNameFile(strFilePath, cmdList);
        }

        public void SetPoiLod()
        {
            UnE.View.Content.IFormContent frmContent = FormMain.Instance.ContentManager.ContentForm;

            if (frmContent == null)
                return;

            string strUseTag = "UsePoiLod", strValueTag = "PoiLodValue";
            string strSQL = "Select PropertyName, PropertyValue from OptionSDMS where PropertyName = '" + strUseTag + "' or PropertyName = '" + strValueTag + "'";
            ArrayList arrResult = FormMain.Instance.DBManager.GetResultData(strSQL);

            if (arrResult == null)
                return;

            float fMin, fMax, fDistance;
            int nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount-1;i+=2)
            {
                string strName = WebDBManager.GetStringField(arrResult[i]);
                string strValue = WebDBManager.GetStringField(arrResult[i + 1]);

                if (strName == null || strValue == null)
                    continue;

                if (string.Compare(strName, strUseTag, true) == 0)
                {
                    string[] values = strValue.Split(',');

                    foreach (string strPOIType in values)
                    {
                        frmContent.SetPoiLod(strPOIType.Trim(), true);
                    }
                }
                else if (string.Compare(strName, strValueTag, true) == 0)
                {
                    string[] tokens = strValue.Split(';');

                    foreach (string strToken in tokens)
                    {
                        string[] values = strToken.Split(',');

                        if (values.Count() != 3)
                            continue;

                        if (float.TryParse(values[0].Trim(), out fMin) &&
                            float.TryParse(values[1].Trim(), out fMax) &&
                            float.TryParse(values[2].Trim(), out fDistance))
                        {
                            frmContent.AddPoiLodValue(fMin, fMax, fDistance);
                        }
                    }
                }
            }
        }
    }
}
