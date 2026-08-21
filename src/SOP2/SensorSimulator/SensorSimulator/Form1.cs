using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace SensorSimulator
{
    public partial class Form1 : Form
    {
        Utilities.WebDBManager m_dbMgr = new Utilities.WebDBManager();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ArrayList arrCheckedIDs = GetCheckedEquipmentIDList();

            string strSQL = "Select FireEquipment.ID, EquipID, EquipType, ZoneName from FireEquipment, Zone where FireEquipment.ZoneID = Zone.ID and EquipType > 1";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 3; i += 4)
            {
                int nID = m_dbMgr.GetIntField(arrResult[i].ToString(), -1);
                string strEquipID = m_dbMgr.GetStringField(arrResult[i + 1], "");
                int nEquipType = m_dbMgr.GetIntField(arrResult[i + 2].ToString(), -1);
                string strZoneName = m_dbMgr.GetStringField(arrResult[i + 3], "");

                DataGridViewRow row = new DataGridViewRow();
                row.Tag = nID;

                DataGridViewCheckBoxCell checkCell = new DataGridViewCheckBoxCell();
                checkCell.Value = arrCheckedIDs.Contains(nID) ? true : false;
                row.Cells.Add(checkCell);

                DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
                cell.Value = strEquipID;
                cell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                row.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                if (nEquipType == 2) cell.Value = "소화전";
                else if (nEquipType == 3) cell.Value = "발신기";
                else cell.Value = "";
                cell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                row.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Value = " " + strZoneName;
                row.Cells.Add(cell);

                dataGridView1.Rows.Add(row);
            }
        }

        // 이미 신호가 들어와 있는 설비 ID들을 DB로부터 읽어온다.
        private ArrayList GetCheckedEquipmentIDList()
        {
            ArrayList arrIDs = new ArrayList();

            string strSQL = "Select EquipmentID from FireEquipmentSignal";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return arrIDs;

            int nResultCount = arrResult.Count;

            foreach (object obj in arrResult)
            {
                int nID = m_dbMgr.GetIntField(obj.ToString(), -1);
                if (nID > 0)
                    arrIDs.Add(nID);
            }

            return arrIDs;
        }

        private void btnSendSignal_Click(object sender, EventArgs e)
        {
            string strIDs = "";
            ArrayList arrID = new ArrayList();

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                bool isChecked = (bool)row.Cells[0].Value;

                if (isChecked)
                {
                    if (strIDs.Length == 0)
                        strIDs = row.Tag.ToString();
                    else
                        strIDs += ", " + row.Tag.ToString();

                    arrID.Add(row.Tag);
                }
            }

            if (strIDs.Length == 0)
            {
                string strSQL = "delete from FireEquipmentSignal";
                m_dbMgr.GetResultData(strSQL, 0);
            }
            else
            {
                string strSQL = string.Format("delete from FireEquipmentSignal where EquipmentID not in ({0})", strIDs);
                if (m_dbMgr.GetResultData(strSQL, 0) == null)
                    return;

                strSQL = "select EquipmentID from FireEquipmentSignal";
                ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

                if (arrResult == null)
                    return;

                foreach (object data in arrResult)
                {
                    int nEuipID = m_dbMgr.GetIntField(data.ToString(), -1);

                    int nIndex = arrID.IndexOf(nEuipID);

                    if (nIndex >= 0)
                        arrID.RemoveAt(nIndex);
                }

                foreach (int nEquipID in arrID)
                {
                    strSQL = string.Format("Insert into FireEquipmentSignal (EquipmentID) values ({0})", nEquipID);
                    if (m_dbMgr.GetResultData(strSQL, 0) == null)
                        return;
                }
            }
        }
    }
}
