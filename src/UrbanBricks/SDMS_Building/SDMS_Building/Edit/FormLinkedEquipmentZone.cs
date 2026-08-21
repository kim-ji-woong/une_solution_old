using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UnE.Sensor;
using UnE.Spatial;

namespace SDMS_Building.Edit
{
    public partial class FormLinkedEquipmentZone : Form
    {
        private Dictionary<int, LinkedPOIs> m_dicLink = new Dictionary<int, LinkedPOIs>();
        private uFormEdit2 m_parent = null;
        public FormLinkedEquipmentZone(uFormEdit2 parent)
        {
            InitializeComponent();

            m_parent = parent;
            m_parent.SetEditSubType(EditSubType.EquipmentZone);            
            initComboBox();
        }

        private void DisplayPOILink(int equipmentZoneID)
        {
            dataGridView2.Rows.Clear();
            m_parent.HideLinkedPOI();

            ArrayList arrResult = FormMain.Instance.DBManager.GetResultData("Select c1.ID, c1.EquipmentZoneID, c2.CameraName From CCTVLink as c1, CCTV as c2 Where c1.ID=c2.ID And c1.EquipmentZoneID= " + equipmentZoneID);
            if (arrResult == null)
                return;

            int resultCount = arrResult.Count;
            for (int i = 0; i < resultCount; i += 3)
            {
                DBUtility2.VariousData<int> nID = DBUtility2.WebDBManager.GetIntField(arrResult[i].ToString());
                DBUtility2.VariousData<int> nEquipmentZoneID = DBUtility2.WebDBManager.GetIntField(arrResult[i + 1].ToString());
                string strName = DBUtility2.WebDBManager.GetStringField(arrResult[i + 2]);

                if (nID == null || nEquipmentZoneID == null)
                    continue;

                //LinkedPOIs linked = new LinkedPOIs();
                //linked.EquipmentZoneID = nEquipmentZoneID.Data;
                //linked.Name = strName;

                dataGridView2.Rows.Add(nID.Data, nEquipmentZoneID.Data, strName);
                m_parent.AddLinkedPOI(nID.Data);
            }

            foreach (KeyValuePair<int, LinkedPOIs> item in m_dicLink)
            {
                if (item.Value.EquipmentZoneID != equipmentZoneID)
                    continue;

                dataGridView2.Rows.Add(item.Key, item.Value.EquipmentZoneID, item.Value.Name);
            }
        }

        private void initComboBox()
        {
            cbBuilding.ValueMember = "ID";
            cbBuilding.DisplayMember = "Name";
            cbZone.ValueMember = "ID";
            cbZone.DisplayMember = "Name";

            foreach (KeyValuePair<int, Building> item in ZoneManager.Instance.DicBuildings)
            {
                if (item.Value.ID == 0)
                    continue;

                ComboBoxItem cbItem = new ComboBoxItem();
                cbItem.ID = item.Value.ID;
                cbItem.Name = item.Value.BuildingName;

                cbBuilding.Items.Add(cbItem);
            }

            cbBuilding.SelectedIndex = 0;
        }

        private void cbBuilding_SelectedIndexChanged(object sender, EventArgs e)
        {
            cbZone.Items.Clear();

            ComboBoxItem selectedBuilding = cbBuilding.SelectedItem as ComboBoxItem;
            if (selectedBuilding == null)
                return;

            foreach (KeyValuePair<int, Zone> item in ZoneManager.Instance.DicZones)
            {
                if (item.Value.Building == null)
                    continue;
                if (item.Value.Building.ID != selectedBuilding.ID)
                    continue;

                ComboBoxItem cbItem = new ComboBoxItem();
                cbItem.ID = item.Value.ID;
                cbItem.Name = item.Value.ZoneName;

                cbZone.Items.Add(cbItem);
            }

            if (cbZone.Items.Count > 0)
                cbZone.SelectedIndex = 0;
        }
        private void cbZone_SelectedIndexChanged(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();

            ComboBoxItem selectedZone = cbZone.SelectedItem as ComboBoxItem;
            if (selectedZone == null)
                return;

            foreach (KeyValuePair<int, EquipmentZone> item in ZoneManager.Instance.DicEquipZones)
            {
                if (item.Value.LinkedZone == null)
                    continue;

                if (item.Value.LinkedZone.ID != selectedZone.ID)
                    continue;
                                
                dataGridView1.Rows.Add(item.Value.ID, item.Value.ZoneName);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }
        private void FormLinkedEquipmentZone_FormClosed(object sender, FormClosedEventArgs e)
        {
            m_parent.SetEditSubType(EditSubType.None);
        }

        public void AddLinkEquipmentZone(CCTV cctv)
        {
            if (dataGridView1.SelectedRows == null || dataGridView1.SelectedRows.Count == 0)
                return;

            int equipmentZoneID = (int)dataGridView1.SelectedRows[0].Cells[0].Value;

            string strDeleteQuery = string.Format("Delete from CCTVLink Where ID={0}", cctv.ID);
            if (FormMain.Instance.DBManager.GetResultData(strDeleteQuery) == null)
            {
                MessageBox.Show("Error : " + FormMain.Instance.DBManager.LastErrorMessage);
                return;
            }

            string strInsertQuery = string.Format("Insert into CCTVLink (ID, EquipmentZoneID) Values({0},{1})", cctv.ID, equipmentZoneID);
            if (FormMain.Instance.DBManager.GetResultData(strInsertQuery) == null)
            {
                MessageBox.Show("Error : " + FormMain.Instance.DBManager.LastErrorMessage);
                return;
            }

            dataGridView2.Rows.Add(cctv.ID, equipmentZoneID, cctv.AccessKey);
            m_parent.AddLinkedPOI(cctv);

            //DisplayPOILink(equipmentZoneID);
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.SelectedRows == null || dataGridView1.SelectedRows.Count == 0)
                return;

            int equipmentZoneID = (int)dataGridView1.SelectedRows[0].Cells[0].Value;
            DisplayPOILink(equipmentZoneID);
        }

        private void dataGridView2_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (dataGridView2.SelectedRows == null || dataGridView2.SelectedRows.Count == 0)
                return;

            int nID = (int)dataGridView2.SelectedRows[0].Cells[0].Value;

            if (e.Button == MouseButtons.Right) // 삭제
            {
                string strDeleteQuery = string.Format("Delete from CCTVLink Where ID={0}", nID);
                if (FormMain.Instance.DBManager.GetResultData(strDeleteQuery) == null)
                {
                    MessageBox.Show("Error : " + FormMain.Instance.DBManager.LastErrorMessage);
                    return;
                }

                if (dataGridView1.SelectedRows == null || dataGridView1.SelectedRows.Count == 0)
                    return;

                dataGridView2.Rows.Remove(dataGridView2.SelectedRows[0]);
                m_parent.HideLinkedPOI(nID);
            }
        }
    }

    public class ComboBoxItem
    {
        private int m_nID = -1;
        private string m_strName = "";

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
    }

    public class LinkedPOIs
    {
        private int m_nEquipmentZoneID = -1;
        private string m_strName = "";

        public int EquipmentZoneID
        {
            get { return m_nEquipmentZoneID; }
            set { m_nEquipmentZoneID = value; }
        }

        public string Name
        {
            get { return m_strName; }
            set { m_strName = value; }
        }
    }
}
