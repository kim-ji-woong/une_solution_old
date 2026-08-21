using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace HSMS
{
    public partial class FormList : Form
    {
        public FormList()
        {
            InitializeComponent();
        }

        private ArrayList m_arWorkers = new ArrayList();
        private ArrayList m_arCars = new ArrayList();
        private ArrayList m_arEquips = new ArrayList();

        private void LoadData()
        {
            DataManager dataMgr = FormMain.Instance.DataMgr;

            int nCars = dataMgr.GetCarCount();
            for (int i = 0; i < nCars; i++)
            {
                m_arCars.Add(dataMgr.GetCar(i));
            }

            int nEquip = dataMgr.GetEquipCount();
            for (int i = 0; i < nEquip; i++)
            {
                DataEquip equip = dataMgr.GetEquip(i);
                if (equip.Sensor == null || equip.Sensor == "")
                    continue;
                m_arEquips.Add(equip);
            }

            int nWorkers = dataMgr.GetWorkerCount();
            for (int i = 0; i < nWorkers; i++)
            { 
                m_arWorkers.Add(dataMgr.GetWorker(i));
            }
        }

        private void CreateGrid()
        {
            for (int i = 0; i < gridWorker.Columns.Count; i++)
            {
                gridWorker.Columns[i].SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            }

            for (int i = 0; i < gridCars.Columns.Count; i++)
            {
                gridCars.Columns[i].SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            }

            for (int i = 0; i < gridEquip.Columns.Count; i++)
            {
                gridEquip.Columns[i].SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            }

            Rectangle rect = gridEquip.Bounds;
            gridCars.SetBounds(rect.X, rect.Y, rect.Width, rect.Height);
            gridCars.Visible = false;

            gridWorker.SetBounds(rect.X, rect.Y, rect.Width, rect.Height);
            gridWorker.Visible = false;
            gridEquip.Visible = true;  
        }

        private void SetupGridData()
        {        
            int nCount = 1;
            foreach(DataWorker worker in m_arWorkers)
            {
                DataGridViewRow row = new DataGridViewRow();
                row.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                
                // 번호
                DataGridViewTextBoxCell cellNo = new DataGridViewTextBoxCell();
                cellNo.Value = nCount++;
                row.Cells.Add(cellNo);

                // 이름
                DataGridViewTextBoxCell cellName = new DataGridViewTextBoxCell();
                cellName.Value = worker.Name;
                row.Cells.Add(cellName);

                // SensorID
                DataGridViewTextBoxCell cellID = new DataGridViewTextBoxCell();
                cellID.Value = worker.Sensor;
                row.Cells.Add(cellID);

                // 센서 사용여부
                DataGridViewTextBoxCell cellDetect = new DataGridViewTextBoxCell();
                cellDetect.Value = worker.SensorDetect == true ? "사용" : "미사용";
                row.Cells.Add(cellDetect);

                DataGridViewTextBoxCell cellEtc = new DataGridViewTextBoxCell();
                cellEtc.Value = "";
                row.Cells.Add(cellEtc);

                gridWorker.Rows.Add(row);
            }

            nCount = 1;
            foreach(DataCar car in m_arCars)
            {
                DataGridViewRow row = new DataGridViewRow();
                row.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                // 번호
                DataGridViewTextBoxCell cellNo = new DataGridViewTextBoxCell();
                cellNo.Value = nCount++;
                row.Cells.Add(cellNo);

                // 종류
                DataGridViewTextBoxCell cellCode = new DataGridViewTextBoxCell();
                cellCode.Value = car.Type;
                row.Cells.Add(cellCode);

                // 이름
                DataGridViewTextBoxCell cellName = new DataGridViewTextBoxCell();
                cellName.Value = car.Name;
                row.Cells.Add(cellName);

                // SensorID
                DataGridViewTextBoxCell cellID = new DataGridViewTextBoxCell();
                cellID.Value = car.Sensor;
                row.Cells.Add(cellID);

                // 센서 사용여부
                DataGridViewTextBoxCell cellDetect = new DataGridViewTextBoxCell();
                cellDetect.Value = car.SensorDetect == true ? "사용" : "미사용";
                row.Cells.Add(cellDetect);

                // 비고
                DataGridViewTextBoxCell cellEtc = new DataGridViewTextBoxCell();
                cellEtc.Value = "";
                row.Cells.Add(cellEtc);

                gridCars.Rows.Add(row);

            }
            nCount = 1;
            foreach(DataEquip equip in m_arEquips)
            {
                DataGridViewRow row = new DataGridViewRow();
                row.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                // 번호
                DataGridViewTextBoxCell cellNo = new DataGridViewTextBoxCell();
                cellNo.Value = nCount++;
                row.Cells.Add(cellNo);

                // 종류
                DataGridViewTextBoxCell cellCode = new DataGridViewTextBoxCell();
                cellCode.Value = equip.TypeName;
                row.Cells.Add(cellCode);

                // 이름
                DataGridViewTextBoxCell cellName = new DataGridViewTextBoxCell();
                cellName.Value = equip.Name;
                row.Cells.Add(cellName);

                // SensorID
                DataGridViewTextBoxCell cellID = new DataGridViewTextBoxCell();
                cellID.Value = equip.Sensor;
                row.Cells.Add(cellID);

                // 센서 사용여부
                DataGridViewTextBoxCell cellDetect = new DataGridViewTextBoxCell();
                cellDetect.Value = equip.SensorDetect == true ? "사용" : "미사용";
                row.Cells.Add(cellDetect);

                // 비고
                DataGridViewTextBoxCell cellEtc = new DataGridViewTextBoxCell();
                cellEtc.Value = "";
                row.Cells.Add(cellEtc);

                gridEquip.Rows.Add(row);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FormList_Load(object sender, EventArgs e)
        {
            LoadData();

            CreateGrid();

            SetupGridData();

            cmbType.SelectedIndex = 0;
        }

        private void FormList_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void cmbType_SelectedIndexChanged(object sender, EventArgs e)
        {
            int nSelect = cmbType.SelectedIndex;
            if (nSelect == -1)
                return;

            if (nSelect == 0)
            {
                gridWorker.Visible = true;
                gridCars.Visible = false;
                gridEquip.Visible = false;
            }
            else if (nSelect == 1)
            {                
                gridCars.Visible = true;
                gridWorker.Visible = false;
                gridEquip.Visible = false;
            }
            else if (nSelect == 2)
            {
                gridEquip.Visible = true;
                gridCars.Visible = false;
                gridWorker.Visible = false;               
            }

        }
    }
}
