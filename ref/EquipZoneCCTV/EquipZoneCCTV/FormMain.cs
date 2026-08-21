using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DBUtility2;
using System.Collections;
using System.IO;

namespace EquipZoneCCTV
{
    public partial class FormMain : Form
    {
        private WebDBManager m_dbMgr = null;

        public FormMain()
        {
            InitializeComponent();

            int nSiteID = LoadSiteID();
            m_dbMgr = new WebDBManager(nSiteID);

            ReadScript();
            InitData();
        }

        private void ReadScript()
        {
            if (File.Exists("scriptComplete.txt"))
                return;

            StreamReader reader = new StreamReader("script.sql", Encoding.UTF8);

            while (reader.EndOfStream == false)
            {
                string strLine = reader.ReadLine().Trim();

                if (strLine.ToLower().StartsWith("update"))
                {
                    if (m_dbMgr.GetResultData(strLine) == null)
                    {
                        reader.Close();
                        return;
                    }
                }
            }

            reader.Close();

            StreamWriter writer = new StreamWriter("scriptComplete.txt", false, Encoding.UTF8);
            writer.Write("complete");
            writer.Close();
        }

        private int LoadSiteID()
        {
            Utility ini = new Utility();
            string strSiteID = ini.getinivalue("Server Connection Info", "siteid");
            int nSiteID = 1;

            if (strSiteID.Length > 0)
            {
                int.TryParse(strSiteID, out nSiteID);
            }

            return nSiteID;
        }
        
        private void InitData()
        {
            string strSQL = "Select ID, BuildingName from Building";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;
            string strBuildingIDs = "";

            Dictionary<int, Building> dicBuildings = new Dictionary<int, Building>();

            for (int i=0;i<nResultCount-1;i+=2)
            {
                VariousData<int> buildingID = WebDBManager.GetIntField(arrResult[i].ToString());
                string strBuildingName = WebDBManager.GetStringField(arrResult[i + 1]);

                if (buildingID == null || strBuildingName == null)
                    continue;

                Building building = new Building();
                building.ID = buildingID.Data;
                building.Name = strBuildingName;

                dicBuildings[building.ID] = building;

                if (strBuildingIDs.Length == 0)
                    strBuildingIDs = buildingID.Data.ToString();
                else
                    strBuildingIDs += ", " + buildingID.Data.ToString();

                cboBuildings.Items.Add(building);
            }

            if (strBuildingIDs.Length == 0)
                return;

            strSQL = "Select ID, ZoneName, BuildingID from Zone where BuildingID in (" + strBuildingIDs + ")";
            arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return;

            nResultCount = arrResult.Count;
            Dictionary<int, Zone> dicZones = new Dictionary<int, Zone>();

            for (int i=0;i<nResultCount-2;i+=3)
            {
                VariousData<int> zoneID = WebDBManager.GetIntField(arrResult[i].ToString());
                string strZoneName = WebDBManager.GetStringField(arrResult[i + 1]);
                VariousData<int> buildingID = WebDBManager.GetIntField(arrResult[i + 2].ToString());

                if (zoneID == null || strZoneName == null || buildingID == null)
                    continue;

                Building building;

                if (dicBuildings.TryGetValue(buildingID.Data, out building) == false)
                    continue;

                Zone zone = new Zone();
                zone.ID = zoneID.Data;
                zone.Name = strZoneName;

                dicZones[zone.ID] = zone;
                building.Zones.Add(zone);
            }

            strSQL = "Select ID, ZoneName, LinkedZoneIDList from EquipmentZone";
            arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return;

            nResultCount = arrResult.Count;
            Dictionary<int, EquipZoneCCTV> dicEquipZoneCCTVs = new Dictionary<int, EquipZoneCCTV>();

            for (int i=0;i<nResultCount-2;i+=3)
            {
                VariousData<int> equipZoneID = WebDBManager.GetIntField(arrResult[i].ToString());
                string strZoneName = WebDBManager.GetStringField(arrResult[i + 1]);
                string strLinkedZoneIDs = WebDBManager.GetStringField(arrResult[i + 2]);

                if (equipZoneID == null || strZoneName == null || strLinkedZoneIDs == null)
                    continue;

                strLinkedZoneIDs = strLinkedZoneIDs.Trim();

                if (strLinkedZoneIDs.Length == 0)
                    continue;

                string[] tokens = strLinkedZoneIDs.Split(',');

                foreach (string strToken in tokens)
                {
                    int nZoneID;
                    Zone zone;

                    if (int.TryParse(strToken.Trim(), out nZoneID) == false)
                        continue;

                    if (dicZones.TryGetValue(nZoneID, out zone) == false)
                        continue;

                    EquipZoneCCTV equipZoneCCTV = new EquipZoneCCTV();
                    equipZoneCCTV.EquipZoneID = equipZoneID.Data;
                    equipZoneCCTV.ZoneName = strZoneName;

                    zone.EquipZoneCCTVs.Add(equipZoneCCTV);
                    dicEquipZoneCCTVs[equipZoneID.Data] = equipZoneCCTV;
                }
            }

            strSQL = "Select CCTV.ID, EquipZoneID, CCTV1, CCTV2, CCTV3, CCTV4, CCTV5, CCTV6 ";
            strSQL += "from EquipZoneCCTV as CCTV, EquipmentZone as ez ";
            strSQL += "where CCTV.EquipZoneID = ez.ID";

            arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return;

            nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 7; i += 8)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                VariousData<int> equipZoneID = WebDBManager.GetIntField(arrResult[i + 1].ToString());
                VariousData<int> cctv1 = WebDBManager.GetIntField(arrResult[i + 2].ToString());
                VariousData<int> cctv2 = WebDBManager.GetIntField(arrResult[i + 3].ToString());
                VariousData<int> cctv3 = WebDBManager.GetIntField(arrResult[i + 4].ToString());
                VariousData<int> cctv4 = WebDBManager.GetIntField(arrResult[i + 5].ToString());
                VariousData<int> cctv5 = WebDBManager.GetIntField(arrResult[i + 6].ToString());
                VariousData<int> cctv6 = WebDBManager.GetIntField(arrResult[i + 7].ToString());

                if (id == null || equipZoneID == null)
                    continue;

                EquipZoneCCTV equipZoneCCTV;

                if (dicEquipZoneCCTVs.TryGetValue(equipZoneID.Data, out equipZoneCCTV) == false)
                    continue;

                equipZoneCCTV.ID = id.Data;
                equipZoneCCTV.CCTV1 = cctv1;
                equipZoneCCTV.CCTV2 = cctv2;
                equipZoneCCTV.CCTV3 = cctv3;
                equipZoneCCTV.CCTV4 = cctv4;
                equipZoneCCTV.CCTV5 = cctv5;
                equipZoneCCTV.CCTV6 = cctv6;
            }

            if (cboBuildings.Items.Count > 0)
                cboBuildings.SelectedIndex = 0;
        }

        private void cboBuildings_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboBuildings.SelectedIndex < 0)
                return;

            Building building = (Building)cboBuildings.Items[cboBuildings.SelectedIndex];

            cboZones.Items.Clear();
            
            foreach (Zone zone in building.Zones)
            {
                cboZones.Items.Add(zone);
            }

            if (cboZones.Items.Count > 0)
                cboZones.SelectedIndex = 0;
        }

        private void cboZones_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboZones.SelectedIndex < 0)
                return;

            Zone zone = (Zone)cboZones.Items[cboZones.SelectedIndex];
            gridEquipZoneCCTV.Rows.Clear();

            foreach (EquipZoneCCTV equipZoneCCTV in zone.EquipZoneCCTVs)
            {
                int nRowIndex = gridEquipZoneCCTV.Rows.Add();

                if (nRowIndex < 0)
                    break;

                DataGridViewRow row = gridEquipZoneCCTV.Rows[nRowIndex];
                row.Tag = equipZoneCCTV;

                row.Cells[0].Value = nRowIndex + 1;
                row.Cells[1].Value = equipZoneCCTV.EquipZoneID;
                row.Cells[2].Value = equipZoneCCTV.ZoneName;
                row.Cells[3].Value = equipZoneCCTV.CCTV1 == null ? "" : equipZoneCCTV.CCTV1.Data.ToString();
                row.Cells[4].Value = equipZoneCCTV.CCTV2 == null ? "" : equipZoneCCTV.CCTV2.Data.ToString();
                row.Cells[5].Value = equipZoneCCTV.CCTV3 == null ? "" : equipZoneCCTV.CCTV3.Data.ToString();
                row.Cells[6].Value = equipZoneCCTV.CCTV4 == null ? "" : equipZoneCCTV.CCTV4.Data.ToString();
                row.Cells[7].Value = equipZoneCCTV.CCTV5 == null ? "" : equipZoneCCTV.CCTV5.Data.ToString();
                row.Cells[8].Value = equipZoneCCTV.CCTV6 == null ? "" : equipZoneCCTV.CCTV6.Data.ToString();
            }
        }

        private void btnApplyDB_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in gridEquipZoneCCTV.Rows)
            {
                EquipZoneCCTV equipZoneCCTV = (EquipZoneCCTV)row.Tag;

                equipZoneCCTV.CCTV1 = ToInt(row.Cells[3]);
                equipZoneCCTV.CCTV2 = ToInt(row.Cells[4]);
                equipZoneCCTV.CCTV3 = ToInt(row.Cells[5]);
                equipZoneCCTV.CCTV4 = ToInt(row.Cells[6]);
                equipZoneCCTV.CCTV5 = ToInt(row.Cells[7]);
                equipZoneCCTV.CCTV6 = ToInt(row.Cells[8]);

                string strSQL = "Update EquipZoneCCTV set CCTV1 = " + (equipZoneCCTV.CCTV1 == null ? "NULL" : equipZoneCCTV.CCTV1.Data.ToString());
                strSQL += ", CCTV2 = " + (equipZoneCCTV.CCTV2 == null ? "NULL" : equipZoneCCTV.CCTV2.Data.ToString());
                strSQL += ", CCTV3 = " + (equipZoneCCTV.CCTV3 == null ? "NULL" : equipZoneCCTV.CCTV3.Data.ToString());
                strSQL += ", CCTV4 = " + (equipZoneCCTV.CCTV4 == null ? "NULL" : equipZoneCCTV.CCTV4.Data.ToString());
                strSQL += ", CCTV5 = " + (equipZoneCCTV.CCTV5 == null ? "NULL" : equipZoneCCTV.CCTV5.Data.ToString());
                strSQL += ", CCTV6 = " + (equipZoneCCTV.CCTV6 == null ? "NULL" : equipZoneCCTV.CCTV6.Data.ToString());
                strSQL += " where ID = " + equipZoneCCTV.ID.ToString();

                m_dbMgr.GetResultData(strSQL);
            }
        }

        private VariousData<int> ToInt(DataGridViewCell cell)
        {
            if (cell.Value == null)
                return null;

            string strValue = cell.Value.ToString().Trim();

            if (strValue.Length == 0)
                return null;

            int nID;

            if (int.TryParse(strValue, out nID))
            {
                return new VariousData<int>(nID);
            }

            return null;
        }

        private void btnCCTVURL_Click(object sender, EventArgs e)
        {
            FormCCTV frm = new FormCCTV(m_dbMgr);
            frm.ShowDialog();
        }

        private void btnDBBackup_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();

            dlg.Filter = "SQL Files|*.sql|All FIles|*.*";
            dlg.FilterIndex = 0;
            dlg.Title = "DB 백업";

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string strFilePath = dlg.FileName;
                SaveDB(strFilePath);
            }
        }

        private void SaveDB(string strPath)
        {
            StreamWriter writer = new StreamWriter(strPath, false, Encoding.UTF8);
            writer.WriteLine("USE " + m_dbMgr.DatabaseName);
            writer.WriteLine("GO");

            int nCount = 0;

            foreach (Building building in cboBuildings.Items)
            {
                foreach (Zone zone in building.Zones)
                {
                    foreach (EquipZoneCCTV cctv in zone.EquipZoneCCTVs)
                    {
                        string strSQL = "Update EquipZoneCCTV set CCTV1 = " + (cctv.CCTV1 == null ? "NULL" : cctv.CCTV1.Data.ToString());
                        strSQL += ", CCTV2 = " + (cctv.CCTV2 == null ? "NULL" : cctv.CCTV2.Data.ToString());
                        strSQL += ", CCTV3 = " + (cctv.CCTV3 == null ? "NULL" : cctv.CCTV3.Data.ToString());
                        strSQL += ", CCTV4 = " + (cctv.CCTV4 == null ? "NULL" : cctv.CCTV4.Data.ToString());
                        strSQL += ", CCTV5 = " + (cctv.CCTV5 == null ? "NULL" : cctv.CCTV5.Data.ToString());
                        strSQL += ", CCTV6 = " + (cctv.CCTV6 == null ? "NULL" : cctv.CCTV6.Data.ToString());
                        strSQL += " where ID = " + cctv.ID.ToString();

                        writer.WriteLine(strSQL);

                        if (++nCount == 30)
                        {
                            nCount = 0;
                            writer.WriteLine("GO");
                        }
                    }
                }
            }

            if (nCount != 0)
                writer.WriteLine("GO");

            writer.Close();
        }
    }

    class Building
    {
        private int m_nID = -1;
        private string m_strName = "";
        private List<Zone> m_zones = new List<Zone>();

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public string Name
        {
            get { return m_strName; }
            set { m_strName = value; }
        }

        public List<Zone> Zones
        {
            get { return m_zones; }
        }

        public override string ToString()
        {
            return Name;
        }
    }

    class Zone
    {
        private int m_nID = -1;
        private string m_strName = "";
        private List<EquipZoneCCTV> m_equipZoneCCTVs = new List<EquipZoneCCTV>();

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public string Name
        {
            get { return m_strName; }
            set { m_strName = value; }
        }

        public List<EquipZoneCCTV> EquipZoneCCTVs
        {
            get { return m_equipZoneCCTVs; }
        }

        public override string ToString()
        {
            return Name;
        }
    }

    class EquipZoneCCTV
    {
        private int m_nID = -1;
        private int m_nEquipZoneID = -1;
        private string m_strZoneName = "";
        private VariousData<int> m_cctv1 = null;
        private VariousData<int> m_cctv2 = null;
        private VariousData<int> m_cctv3 = null;
        private VariousData<int> m_cctv4 = null;
        private VariousData<int> m_cctv5 = null;
        private VariousData<int> m_cctv6 = null;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public int EquipZoneID
        {
            get { return m_nEquipZoneID; }
            set { m_nEquipZoneID = value; }
        }

        public string ZoneName
        {
            get { return m_strZoneName; }
            set { m_strZoneName = value; }
        }

        public VariousData<int> CCTV1
        {
            get { return m_cctv1; }
            set { m_cctv1 = value; }
        }

        public VariousData<int> CCTV2
        {
            get { return m_cctv2; }
            set { m_cctv2 = value; }
        }

        public VariousData<int> CCTV3
        {
            get { return m_cctv3; }
            set { m_cctv3 = value; }
        }

        public VariousData<int> CCTV4
        {
            get { return m_cctv4; }
            set { m_cctv4 = value; }
        }

        public VariousData<int> CCTV5
        {
            get { return m_cctv5; }
            set { m_cctv5 = value; }
        }

        public VariousData<int> CCTV6
        {
            get { return m_cctv6; }
            set { m_cctv6 = value; }
        }
    }
}
