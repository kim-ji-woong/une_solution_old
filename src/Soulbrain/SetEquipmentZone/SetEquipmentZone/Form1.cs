using dnsDBUtil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SetEquipmentZone
{
    public partial class Form1 : Form
    {
        private dnsDBUtil.WebDBManager m_dbManager = null;
        public Form1()
        {
            InitializeComponent();

            tabControl1.TabPages.Remove(tabPage1);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            for (int i = 0; i < dataGridViewOrgSensor.Columns.Count; i++)
            {
                dataGridViewOrgSensor.Columns[i].ReadOnly = true;
            }
            for (int i = 0; i < dataGridViewNewSensor.Columns.Count; i++)
            {
                dataGridViewNewSensor.Columns[i].ReadOnly = true;
            }

            m_dbManager = new dnsDBUtil.WebDBManager();
            m_dbManager.DatabaseName = "WSOP_10";
            m_dbManager.DatabaseType = WebDBManager.DBType.sqlserver;
            m_dbManager.WebServerURL = "http://192.168.254.201";

            DisplayNewFireSensor();

            DisplayAllFireSensor();
            DisplayZoneList();
            DisplayEquipmentZoneList();

            DisplayAllFireSensor2();
        }

        #region Tab1
        private void DisplayNewFireSensor()
        {
            dataGridViewNewSensor.Rows.Clear();

            StringBuilder sb = new StringBuilder();
            sb.Append("Select ID, Name, PositionName, X, Y, Z, ZoneID, EquipZoneID, TagNo, OrgTagNo, SensorSubType ");
            sb.Append("  From SdmsSensorFire ");

            ArrayList arrResult = m_dbManager.GetResultData(sb.ToString());
            if (arrResult == null)
                return;

            int resultCount = arrResult.Count;
            for (int i = 0; i < resultCount; i += 11)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                string strName = WebDBManager.GetStringField(arrResult[i + 1], "");
                string strPositionName = WebDBManager.GetStringField(arrResult[i + 2], "");

                string fX = WebDBManager.GetStringField(arrResult[i + 3].ToString(), "");
                string fY = WebDBManager.GetStringField(arrResult[i + 4].ToString(), "");
                string fZ = WebDBManager.GetStringField(arrResult[i + 5].ToString(), "");

                string nZoneID = WebDBManager.GetStringField(arrResult[i + 6].ToString(), "");
                string nEquipZoneID = WebDBManager.GetStringField(arrResult[i + 7].ToString(), "");
                string nTagNo = WebDBManager.GetStringField(arrResult[i + 8].ToString(), "");
                string nOrgTagNo = WebDBManager.GetStringField(arrResult[i + 9].ToString(), "");
                string nSensorSubType = WebDBManager.GetStringField(arrResult[i + 10].ToString(), "");

                bool visible = true;
                if (checkBox1.Checked && nOrgTagNo.Length > 0)
                    visible = false;

                if (visible)
                    dataGridViewNewSensor.Rows.Add(nID, strName, strPositionName, nTagNo, nOrgTagNo, nZoneID, nEquipZoneID, fX, fY, fZ, nSensorSubType);
            }
        }

        private void DisplayOrgFireSensor(string filter)
        {
            dataGridViewOrgSensor.Rows.Clear();

            string searchFilter = filter;
            StringBuilder sb = new StringBuilder();
            sb.Append("Select f.ID, f.Name, f.PositionName, TagNo, ");
            sb.Append("       f.ZoneID, (Select ZoneName From SdmsSpatialZone as z Where z.ID = f.ZoneID) as ZoneName, ");
            sb.Append("       EquipZoneID, (Select ZoneName From SdmsSpatialEquipmentZone as ez Where ez.ID = sz.EquipZoneID) as EquipZoneName, ");
            sb.Append("       X,Y,Z, SensorSubType ");
            sb.Append("from SdmsSensorFire as f, SdmsSensorZone as sz, SdmsSensorTagInfo as tag ");
            sb.Append("where f.ID = sz.OrgSensorID ");
            sb.Append(" and sz.ID = tag.SensorZoneID ");
            sb.Append(" and SensorType = 0");
            if (bTagNo)
            {
                searchFilter = filter.Substring(6);
                sb.AppendFormat(" and tagNo like '%{0}'", searchFilter);
            }
            else
                sb.AppendFormat(" and f.Name like '%{0}%'", searchFilter);
            sb.Append(" order by f.Name");

            ArrayList arrResult = m_dbManager.GetResultData(sb.ToString());
            if (arrResult == null)
                return;

            int resultCount = arrResult.Count;
            for (int i = 0; i < resultCount; i += 12)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                string strName = WebDBManager.GetStringField(arrResult[i + 1]);
                string strPositionName = WebDBManager.GetStringField(arrResult[i + 2]);
                string nTagNo = WebDBManager.GetStringField(arrResult[i + 3].ToString());

                string nZoneID = WebDBManager.GetStringField(arrResult[i + 4].ToString());
                string strZoneName = WebDBManager.GetStringField(arrResult[i + 5].ToString());
                string nEquipZoneID = WebDBManager.GetStringField(arrResult[i + 6].ToString());
                string strEquipZoneName = WebDBManager.GetStringField(arrResult[i + 7].ToString());

                string fX = WebDBManager.GetStringField(arrResult[i + 8].ToString());
                string fY = WebDBManager.GetStringField(arrResult[i + 9].ToString());
                string fZ = WebDBManager.GetStringField(arrResult[i + 10].ToString());

                string nSensorSubType = WebDBManager.GetStringField(arrResult[i + 11].ToString());

                dataGridViewOrgSensor.Rows.Add(nID, strName, strPositionName, nTagNo, nZoneID, strZoneName, nEquipZoneID, strEquipZoneName, fX, fY, fZ, nSensorSubType);
            }
        }

        private void dataGridViewNewSensor_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (dataGridViewNewSensor.SelectedRows == null || dataGridViewNewSensor.SelectedRows.Count == 0)
                return;

            string selectedTagNo = dataGridViewNewSensor.SelectedRows[0].Cells[3].Value.ToString();
            string selectedName = dataGridViewNewSensor.SelectedRows[0].Cells[1].Value.ToString();

            if (bTagNo)
                DisplayOrgFireSensor(selectedTagNo);
            else
                DisplayOrgFireSensor(selectedName);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (!checkBox1.Checked)
            {
                DisplayNewFireSensor();
                dataGridViewOrgSensor.Rows.Clear();
            }
            else
            {
                foreach (DataGridViewRow row in dataGridViewNewSensor.Rows)
                {
                    if (row.Cells["colOrgTagNo"].Value.ToString().Length > 0)
                    {
                        row.Visible = false;
                    }
                }
            }
        }

        private void dataGridViewOrgSensor_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (dataGridViewNewSensor.SelectedRows == null || dataGridViewNewSensor.SelectedRows.Count == 0)
                return;

            if (dataGridViewOrgSensor.SelectedRows == null || dataGridViewOrgSensor.SelectedRows.Count == 0)
                return;

            DataGridViewRow newSensorRow = dataGridViewNewSensor.SelectedRows[0];
            string nNewSensorID = newSensorRow.Cells[0].Value.ToString();

            DataGridViewRow row = dataGridViewOrgSensor.SelectedRows[0];

            string strTagNo = row.Cells[3].Value.ToString();
            string strZoneID = row.Cells[4].Value.ToString();
            string strEquipZoneID = row.Cells[6].Value.ToString();
            string strX = (row.Cells[8].Value == null) ? "NULL" : row.Cells[8].Value.ToString();
            string strY = (row.Cells[9].Value == null) ? "NULL" : row.Cells[9].Value.ToString();
            string strZ = (row.Cells[10].Value == null) ? "NULL" : row.Cells[10].Value.ToString();
            string strSensorSubType = (row.Cells[11].Value == null) ? "NULL" : row.Cells[11].Value.ToString();

            StringBuilder sb = new StringBuilder();
            sb.Append("Update SdmsSensorFire2 Set ");
            sb.AppendFormat("       X = {0}, Y = {1}, Z = {2} ", strX, strY, strZ);
            sb.AppendFormat("     , ZoneID = {0}, EquipZoneID = {1} ", strZoneID, strEquipZoneID);
            sb.AppendFormat("     , OrgTagNo = {0}, SensorSubType = {1} ", strTagNo, strSensorSubType);
            sb.AppendFormat("  Where ID = {0}", nNewSensorID);

            if (m_dbManager.GetResultData(sb.ToString()) != null)
            {
                newSensorRow.Cells[4].Value = strTagNo;
                newSensorRow.Cells[5].Value = strZoneID;
                newSensorRow.Cells[6].Value = strEquipZoneID;

                newSensorRow.Cells[7].Value = strX;
                newSensorRow.Cells[8].Value = strY;
                newSensorRow.Cells[9].Value = strZ;
                newSensorRow.Cells[10].Value = strSensorSubType;
            }
        }

        private bool bTagNo = true;
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            bTagNo = true;
        }
        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            bTagNo = false;
        }
        #endregion

        #region Tab2
        private void button1_Click(object sender, EventArgs e)
        {
            DisplayAllFireSensor();
        }

        private void DisplayAllFireSensor()
        {
            dgvAllFireSensor.Rows.Clear();

            StringBuilder sb = new StringBuilder();
            sb.Append("Select ID, Name, PositionName, '', ZoneID, (Select ZoneName From SdmsSpatialZone Where ID=ZoneID) as ZoneName, X, Y, Z, '' ");
            sb.Append("  From SdmsSensorFire where zoneid=193 order by ID");

            ArrayList arrResult = m_dbManager.GetResultData(sb.ToString());
            if (arrResult == null)
                return;

            int resultCount = arrResult.Count;
            for (int i = 0; i < resultCount; i += 10)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                string strName = WebDBManager.GetStringField(arrResult[i + 1], "");
                string strPositionName = WebDBManager.GetStringField(arrResult[i + 2], "");

                string nTagNo = WebDBManager.GetStringField(arrResult[i + 3].ToString(), "");
                string nZoneID = WebDBManager.GetStringField(arrResult[i + 4].ToString(), "");
                string strZoneName = WebDBManager.GetStringField(arrResult[i + 5].ToString(), "");

                string fX = WebDBManager.GetStringField(arrResult[i + 6].ToString(), "");
                string fY = WebDBManager.GetStringField(arrResult[i + 7].ToString(), "");
                string fZ = WebDBManager.GetStringField(arrResult[i + 8].ToString(), "");

                string strEquipZoneID = WebDBManager.GetStringField(arrResult[i + 9].ToString(), "");

                dgvAllFireSensor.Rows.Add(nID, strName, strPositionName, nTagNo, nZoneID, strZoneName, fX, fY, fZ, strEquipZoneID);
            }
        }

        private List<EquipmentZone> m_equipmentZoneList = new List<EquipmentZone>();

        private void DisplayZoneList()
        {
            dgvZone.Rows.Clear();

            StringBuilder sb = new StringBuilder();
            sb.Append("Select ID, (select DisplayText from SdmsSpatialBuilding where id=buildingid) as buildingName, ZoneName, DisplayText ");
            sb.Append("  From SdmsSpatialZone order by buildingid, displayText ");

            ArrayList arrResult = m_dbManager.GetResultData(sb.ToString());
            if (arrResult == null)
                return;

            int resultCount = arrResult.Count;
            for (int i = 0; i < resultCount; i += 4)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                string strBuildingName = WebDBManager.GetStringField(arrResult[i + 1], "");
                string strZoneName = WebDBManager.GetStringField(arrResult[i + 2], "");
                string strDisplayText = WebDBManager.GetStringField(arrResult[i + 3], "");

                dgvZone.Rows.Add(nID, strBuildingName, strZoneName, strDisplayText);
            }
        }

        private void DisplayEquipmentZoneList()
        {
            dgvEquipmentZoneList.Rows.Clear();

            StringBuilder sb = new StringBuilder();
            sb.Append("Select ID, ZoneName, DisplayText, TextCenter, LinkedZoneIDList ");
            sb.Append("  From SdmsSpatialEquipmentZone ");

            ArrayList arrResult = m_dbManager.GetResultData(sb.ToString());
            if (arrResult == null)
                return;

            int resultCount = arrResult.Count;
            for (int i = 0; i < resultCount; i += 5)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                string strName = WebDBManager.GetStringField(arrResult[i + 1], "");
                string strDisplayText = WebDBManager.GetStringField(arrResult[i + 2], "");

                string strTextCenter = WebDBManager.GetStringField(arrResult[i + 3].ToString(), "");
                string strLinkedZoneIDList = WebDBManager.GetStringField(arrResult[i + 4].ToString(), "");

                if (strLinkedZoneIDList.Length == 0)
                    continue;

                EquipmentZone ez = new EquipmentZone();
                ez.ID = nID;
                ez.ZoneName = strName;
                ez.DisplayText = strDisplayText;

                float x = 0.0f;
                float y = 0.0f;
                float z = 0.0f;
                ez.TextCenter = strTextCenter;
                if (strTextCenter.Length > 0)
                {
                    string[] centers = strTextCenter.Split(',');
                    if (centers.Length == 3)
                    {
                        float.TryParse(centers[0], out x);
                        float.TryParse(centers[1], out y);
                        float.TryParse(centers[2], out z);

                        ez.X = x;
                        ez.Y = y;
                        ez.Z = z;
                    }
                }

                int zoneID = -1;

                if (strLinkedZoneIDList.Contains(','))
                {
                    string[] zoneids = strLinkedZoneIDList.Split(',');
                    foreach (string str in zoneids)
                    {
                        if (!int.TryParse(str, out zoneID))
                        {
                            MessageBox.Show("error check zone id " + str);
                            return;
                        }
                        ez.ZoneIDList.Add(zoneID);
                    }
                }
                else
                {
                    if (!int.TryParse(strLinkedZoneIDList, out zoneID))
                    {
                        MessageBox.Show("error check zone id " + strLinkedZoneIDList);
                        return;
                    }
                    ez.ZoneIDList.Add(zoneID);
                }

                m_equipmentZoneList.Add(ez);
            }
        }

        private void dgvAllFireSensor_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (dgvAllFireSensor.SelectedRows == null || dgvAllFireSensor.SelectedRows.Count == 0)
                return;

            int nZoneID = Convert.ToInt32(dgvAllFireSensor.SelectedRows[0].Cells[4].Value);
        }

        private void dgvZone_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (dgvAllFireSensor.SelectedRows == null || dgvAllFireSensor.SelectedRows.Count == 0)
                return;

            if (dgvZone.SelectedRows == null || dgvZone.SelectedRows.Count == 0)
                return;

            //int sensorID = Convert.ToInt32(dgvAllFireSensor.SelectedRows[0].Cells[0].Value);
            //int zoneID = Convert.ToInt32(dgvZone.SelectedRows[0].Cells[0].Value);

            //string strQuery = string.Format("Update SdmsSensorFire Set ZoneID={0} Where ID = {1}", zoneID, sensorID);
            //m_dbManager.GetResultData(strQuery);

            //dgvAllFireSensor.SelectedRows[0].Cells[4].Value = zoneID.ToString();

            int nZoneID = Convert.ToInt32(dgvZone.SelectedRows[0].Cells[0].Value);

            dgvEquipmentZoneList.Rows.Clear();
            foreach (EquipmentZone ez in m_equipmentZoneList)
            {
                if (!ez.ZoneIDList.Contains(nZoneID))
                    continue;

                int rowIndex = dgvEquipmentZoneList.Rows.Add(ez.ID, ez.ZoneName, ez.DisplayText, ez.TextCenter, string.Join(",", ez.ZoneIDList));
                dgvEquipmentZoneList.Rows[rowIndex].Tag = ez;
            }
        }

        private void dgvEquipmentZoneList_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (dgvAllFireSensor.SelectedRows == null || dgvAllFireSensor.SelectedRows.Count == 0)
                return;

            if (dgvEquipmentZoneList.SelectedRows == null || dgvEquipmentZoneList.SelectedRows.Count == 0)
                return;

            int sensorID = Convert.ToInt32(dgvAllFireSensor.SelectedRows[0].Cells[0].Value);

            if (dgvEquipmentZoneList.SelectedRows[0].Tag == null)
                return;

            if ((dgvEquipmentZoneList.SelectedRows[0].Tag is EquipmentZone) == false)
                return;

            int nZoneID = Convert.ToInt32(dgvZone.SelectedRows[0].Cells[0].Value);
            EquipmentZone ez = dgvEquipmentZoneList.SelectedRows[0].Tag as EquipmentZone;

            string strQuery2 = string.Format("Update SdmsSensorFire Set X={0},Y={1},Z={2} Where ID = {3}", ez.X, ez.Y, ez.Z, sensorID);
            m_dbManager.GetResultData(strQuery2);

            string strQuery = string.Format("Update SdmsSensorZone Set EquipZoneID={0} Where SensorType=0 And OrgSensorID = {1}", ez.ID, sensorID);
            m_dbManager.GetResultData(strQuery);

            //dgvAllFireSensor.SelectedRows[0].Cells[4].Value = nZoneID.ToString();
            //dgvAllFireSensor.SelectedRows[0].Cells[6].Value = ez.X.ToString();
            //dgvAllFireSensor.SelectedRows[0].Cells[7].Value = ez.Y.ToString();
            //dgvAllFireSensor.SelectedRows[0].Cells[8].Value = ez.Z.ToString();
            dgvAllFireSensor.SelectedRows[0].Cells[9].Value = ez.ID.ToString();

        }
        #endregion

        #region Tab3
        private void button2_Click(object sender, EventArgs e)
        {
            DisplayAllFireSensor2();
        }
        private void DisplayAllFireSensor2()
        {
            dgvAllFireSensor2.Rows.Clear();

            StringBuilder sb = new StringBuilder();
            sb.Append("Select ID, Name, PositionName, TagNo, ZoneID, (Select ZoneName From SdmsSpatialZone Where ID=ZoneID) as ZoneName, X, Y, Z, EquipZoneID");
            sb.Append("  From SdmsSensorFire order by ID ");

            ArrayList arrResult = m_dbManager.GetResultData(sb.ToString());
            if (arrResult == null)
                return;

            int resultCount = arrResult.Count;
            for (int i = 0; i < resultCount; i += 10)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                string strName = WebDBManager.GetStringField(arrResult[i + 1], "");
                string strPositionName = WebDBManager.GetStringField(arrResult[i + 2], "");

                string nTagNo = WebDBManager.GetStringField(arrResult[i + 3].ToString(), "");
                string nZoneID = WebDBManager.GetStringField(arrResult[i + 4].ToString(), "");
                string strZoneName = WebDBManager.GetStringField(arrResult[i + 5].ToString(), "");

                string fX = WebDBManager.GetStringField(arrResult[i + 6].ToString(), "");
                string fY = WebDBManager.GetStringField(arrResult[i + 7].ToString(), "");
                string fZ = WebDBManager.GetStringField(arrResult[i + 8].ToString(), "");

                string strEquipZoneID = WebDBManager.GetStringField(arrResult[i + 9].ToString(), "");

                dgvAllFireSensor2.Rows.Add(nID, strName, strPositionName, nTagNo, nZoneID, strZoneName, fX, fY, fZ, strEquipZoneID);
            }
        }

        private void dgvAllFireSensor2_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (dgvAllFireSensor2.SelectedRows == null || dgvAllFireSensor2.SelectedRows.Count == 0)
                return;

            int nZoneID = Convert.ToInt32(dgvAllFireSensor2.SelectedRows[0].Cells[4].Value);

            dgvEquipmentZoneList2.Rows.Clear();
            foreach (EquipmentZone ez in m_equipmentZoneList)
            {
                if (!ez.ZoneIDList.Contains(nZoneID))
                    continue;

                int rowIndex = dgvEquipmentZoneList2.Rows.Add(ez.ID, ez.ZoneName, ez.DisplayText, ez.TextCenter, string.Join(",", ez.ZoneIDList));
                dgvEquipmentZoneList2.Rows[rowIndex].Tag = ez;
            }
        }

        private void dgvEquipmentZoneList2_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (dgvAllFireSensor2.SelectedRows == null || dgvAllFireSensor2.SelectedRows.Count == 0)
                return;

            if (dgvEquipmentZoneList2.SelectedRows == null || dgvEquipmentZoneList2.SelectedRows.Count == 0)
                return;

            int sensorID = Convert.ToInt32(dgvAllFireSensor2.SelectedRows[0].Cells[0].Value);

            if (dgvEquipmentZoneList2.SelectedRows[0].Tag == null)
                return;

            if ((dgvEquipmentZoneList2.SelectedRows[0].Tag is EquipmentZone) == false)
                return;

            EquipmentZone ez = dgvEquipmentZoneList2.SelectedRows[0].Tag as EquipmentZone;

            string strQuery = string.Format("Update SdmsSensorFire Set X={1}, Y={2}, Z={3} Where ID = {0}", sensorID, ez.X, ez.Y, ez.Z);
            m_dbManager.GetResultData(strQuery);

            dgvAllFireSensor2.SelectedRows[0].Cells[6].Value = ez.X.ToString();
            dgvAllFireSensor2.SelectedRows[0].Cells[7].Value = ez.Y.ToString();
            dgvAllFireSensor2.SelectedRows[0].Cells[8].Value = ez.Z.ToString();
            //dgvAllFireSensor2.SelectedRows[0].Cells[9].Value = ez.ID.ToString();
        }

        #endregion
    }

    public class EquipmentZone
    {
        private int m_nID = -1;
        private string m_strZoneName = "";
        private string m_strDisplayText = "";
        private string m_strTextCenter = "";
        private float m_fX = 0.0f;
        private float m_fY = 0.0f;
        private float m_fZ = 0.0f;
        private List<int> m_zoneIDList = new List<int>();

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public string ZoneName
        {
            get { return m_strZoneName; }
            set { m_strZoneName = value; }
        }

        public string DisplayText
        {
            get { return m_strDisplayText; }
            set { m_strDisplayText = value; }
        }

        public string TextCenter
        {
            get { return m_strTextCenter; }
            set { m_strTextCenter = value; }
        }

        public float X
        {
            get { return m_fX; }
            set { m_fX = value; }
        }

        public float Y
        {
            get { return m_fY; }
            set { m_fY = value; }
        }

        public float Z
        {
            get { return m_fZ; }
            set { m_fZ = value; }
        }

        public List<int> ZoneIDList
        {
            get { return m_zoneIDList; }
            set { m_zoneIDList = value; }
        }
    }
}
