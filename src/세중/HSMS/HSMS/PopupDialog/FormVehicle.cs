using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Data.SqlClient;

namespace HSMS
{
    public partial class FormVehicle : Form
    {
        private Dictionary<string, DataCar> m_dicGridData = new Dictionary<string, DataCar>();
        private ArrayList m_arrTempDataGrid = null;
        private ArrayList m_arrDBData = null;
        private DataManager m_DataMgr= null;

        public FormVehicle()
        {
            InitializeComponent();

            m_DataMgr = FormMain.Instance.DataMgr;

            //DB에 저장되어있는 데이터
            m_arrDBData = FormMain.Instance.DataMgr.GetCars();
        }

        private void FormVehicle_Load(object sender, EventArgs e)
        {
            SetGridView();
            SetVehicleTreeNode();
            LoadDataGridView();
        }

        private void SetGridView()
        {
            for (int i = 0; i < gridMember.Columns.Count; i++)
            {
                gridMember.Columns[i].SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            }

            for (int i = 0; i < gridManager.Columns.Count; i++)
            {
                gridManager.Columns[i].SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            }

            gridMember.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            gridMember.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            gridManager.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            gridManager.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        private void LoadDataGridView()
        {
            gridManager.Rows.Clear();

            int nCount = 0;
            m_arrTempDataGrid = FormMain.Instance.DataMgr.GetCars();
            foreach (DataCar car in m_arrTempDataGrid) 
            {
                nCount++;
                DataGridViewRow row = new DataGridViewRow();


                DataGridViewTextBoxCell cell0 = new DataGridViewTextBoxCell();
                cell0.Value = nCount;
                row.Cells.Add(cell0);

                DataGridViewTextBoxCell cell1 = new DataGridViewTextBoxCell();
                cell1.Value = car.Name;
                row.Cells.Add(cell1);

                string strStandard = "";
                if (car.CarStandard != null)
                    strStandard = car.CarStandard.Name;

                string strType = "";
                if (car.CarType != null)
                    strType = car.CarType.Name;

                DataGridViewTextBoxCell cell2 = new DataGridViewTextBoxCell();
                cell2.Value = strStandard;
                row.Cells.Add(cell2);

                DataGridViewTextBoxCell cell3 = new DataGridViewTextBoxCell();
                cell3.Value = strType;
                row.Cells.Add(cell3);

                DataGridViewTextBoxCell cell4 = new DataGridViewTextBoxCell();
                cell4.Value = car.Length;
                row.Cells.Add(cell4);

                DataGridViewTextBoxCell cell5 = new DataGridViewTextBoxCell();
                cell5.Value = car.Width;
                row.Cells.Add(cell5);

                DataGridViewTextBoxCell cell6 = new DataGridViewTextBoxCell();
                cell6.Value = car.Height;
                row.Cells.Add(cell6);

                DataGridViewTextBoxCell cell7 = new DataGridViewTextBoxCell();
                cell7.Value = car.MakerCompany;
                row.Cells.Add(cell7);

                DataGridViewTextBoxCell cell8 = new DataGridViewTextBoxCell();
                cell8.Value = car.Use;
                row.Cells.Add(cell8);

                DataGridViewTextBoxCell cell9 = new DataGridViewTextBoxCell();
                cell9.Value = car.DriverName;
                row.Cells.Add(cell9);

                DataGridViewTextBoxCell cell10 = new DataGridViewTextBoxCell();
                cell10.Value = car.Sensor;
                row.Cells.Add(cell10);

                row.Tag = car;

                gridManager.Rows.Add(row);

                m_dicGridData[car.Code] = car;
            }
        }

        private void SetVehicleTreeNode()
        {
            treeViewTeam.Nodes.Clear();
            TreeNode CarTypeNode = null;
            Dictionary<string, DataCarType> Types = ERPManager.Instance.DicCarTypes;
            foreach (KeyValuePair<string, DataCarType> pair in Types)
            {
                DataCarType carType = pair.Value;
                CarTypeNode = new TreeNode(carType.Name);
                CarTypeNode.Tag = carType;
                treeViewTeam.Nodes.Add(CarTypeNode);

                Dictionary<string, ArrayList> dicCars = new Dictionary<string, ArrayList>();
                foreach (DataCarStandard Carstandard in carType.CarStandards)
                {
                    if (Carstandard.Cars.Count == 0)
                        continue;

                    ArrayList arCars = null;
                    if (!dicCars.ContainsKey(Carstandard.Name))
                    {
                        arCars = new ArrayList();
                        dicCars[Carstandard.Name] = arCars;
                    }
                    else
                    {
                        arCars = (ArrayList)dicCars[Carstandard.Name];

                    }
                    arCars.AddRange(Carstandard.Cars);
                }

                foreach (KeyValuePair<string, ArrayList> p in dicCars)
                {
                    string szType = p.Key;
                    ArrayList arCars = p.Value;

                    if (szType == "")
                        continue;

                    TreeNode standardNode = new TreeNode(szType);
                    standardNode.Tag = arCars;
                    CarTypeNode.Nodes.Add(standardNode);

                }

            }

            Dictionary<string, DataCar> cars = ERPManager.Instance.DicCompanyCars;
            bool m_bFirst = true;
            TreeNode UnteamNode = null;
            foreach (KeyValuePair<string, DataCar> pair in cars)
            {
                DataCar car = pair.Value;
                if (car.CarStandard == null)
                {
                    if (m_bFirst == true)
                    {
                        m_bFirst = false;
                        UnteamNode = new TreeNode("Unknown Type");
                        CarTypeNode.Nodes.Add(UnteamNode);
                        break;
                    }
                }
            }
            treeViewTeam.ExpandAll();
        }

        private void treeViewTeam_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Action == TreeViewAction.Unknown)
                return;

            TreeNode node = e.Node;
            ArrayList arCar = null;

            if (node.Text == "Unknown Team")
            {
                arCar = new ArrayList();
                Dictionary<string, DataCar> cars = ERPManager.Instance.DicCompanyCars;
                foreach (KeyValuePair<string, DataCar> pair in cars)
                {
                    DataCar car = pair.Value;
                    if (car.CarStandard == null)
                    {
                        arCar.Add(car);
                    }
                }
            }
            else
            {
                object obj = node.Tag;
                if (obj.GetType() == typeof(DataCarType))
                    return;
                arCar = (ArrayList)obj;
            }

            Dictionary<string, DataCar> dicDataCar = ERPManager.Instance.DicCompanyCars;

            gridMember.Rows.Clear();
            foreach (DataCar w in arCar)
            {
                DataCar car = w;

                DataGridViewRow row = new DataGridViewRow();

                DataGridViewTextBoxCell cell1 = new DataGridViewTextBoxCell();
                cell1.Value = car.Name;
                row.Cells.Add(cell1);

                string strStandard = "";
                if (car.CarStandard != null)
                    strStandard = car.CarStandard.Name;

                string strType = "";
                if (car.CarType != null)
                    strType = car.CarType.Name;

                DataGridViewTextBoxCell cell2 = new DataGridViewTextBoxCell();
                cell2.Value = strStandard;
                row.Cells.Add(cell2);

                DataGridViewTextBoxCell cell3 = new DataGridViewTextBoxCell();
                cell3.Value = strType;
                row.Cells.Add(cell3);

                DataGridViewTextBoxCell cell4 = new DataGridViewTextBoxCell();
                cell4.Value = car.Length;
                row.Cells.Add(cell4);

                DataGridViewTextBoxCell cell5 = new DataGridViewTextBoxCell();
                cell5.Value = car.Width;
                row.Cells.Add(cell5);

                DataGridViewTextBoxCell cell6 = new DataGridViewTextBoxCell();
                cell6.Value = car.Height;
                row.Cells.Add(cell6);

                DataGridViewTextBoxCell cell7 = new DataGridViewTextBoxCell();
                cell7.Value = car.Number;
                row.Cells.Add(cell7);

                DataGridViewTextBoxCell cell8 = new DataGridViewTextBoxCell();
                cell8.Value = car.Sensor;
                row.Cells.Add(cell8);

                row.Tag = w;

                gridMember.Rows.Add(row);
            } 
        }

        private void treeViewTeam_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            if (e.Action == TreeViewAction.Unknown)
                return;
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            DataGridViewSelectedRowCollection arRows = gridManager.SelectedRows;
            if (arRows != null && arRows.Count > 0)
            {
                for (int i = 0; i < arRows.Count; i++)
                {
                    DataGridViewRow row = arRows[i];
                    gridManager.Rows.Remove(row);

                    DataCar car = (DataCar)row.Tag;
                    m_dicGridData.Remove(car.Code);
                }
            }

            int nCount = 0;
            for (int i = 0; i < gridManager.Rows.Count; i++)
            {
                nCount++;
                gridManager.Rows[i].Cells[0].Value = nCount;
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            int nRowCount = gridManager.Rows.Count;

            int nCount = 0;
            if (gridManager.Rows.Count == 0)
                nCount = 0;
            else
                nCount = (int)gridManager.Rows[nRowCount - 1].Cells[0].Value;

            nCount++;

            DataGridViewSelectedRowCollection arRows = gridMember.SelectedRows;
            if (arRows != null && arRows.Count > 0)
            {
                for (int i = 0; i < arRows.Count; i++)
                {
                    bool isChecked = true;
                    DataGridViewRow row = arRows[i];
                    DataCar car = (DataCar)row.Tag;
                    DataGridViewRow row2 = new DataGridViewRow();
                    row2.Tag = car;

                    //중복검사
                    if (m_dicGridData.ContainsKey(car.Code))
                    {
                        DataCar data = m_dicGridData[car.Code];

                        if (data == car)
                            isChecked = false;
                    }

                    //센서아이디가 없는 데이터는 추가X
                    if (car.Sensor.Trim() == "")
                    {
                        MessageBox.Show("센서아이디가 없는 데이터는 추가할 수 없습니다.");
                        isChecked = false;
                        continue;
                    }

                    //중복되면 추가안함
                    if (isChecked == false)
                    {
                        MessageBox.Show("중복된 데이터입니다..");
                        continue;
                    }

                    m_dicGridData[car.Code] = car;

                    DataGridViewTextBoxCell cell0 = new DataGridViewTextBoxCell();
                    cell0.Value = nCount;
                    row2.Cells.Add(cell0);


                    DataGridViewTextBoxCell cell1 = new DataGridViewTextBoxCell();
                    cell1.Value = car.Name;
                    row2.Cells.Add(cell1);

                    string strStandard = "";
                    if (car.CarStandard != null)
                        strStandard = car.CarStandard.Name;

                    string strType = "";
                    if (car.CarType != null)
                        strType = car.CarType.Name;

                    DataGridViewTextBoxCell cell2 = new DataGridViewTextBoxCell();
                    cell2.Value = strStandard;
                    row2.Cells.Add(cell2);

                    DataGridViewTextBoxCell cell3 = new DataGridViewTextBoxCell();
                    cell3.Value = strType;
                    row2.Cells.Add(cell3);

                    DataGridViewTextBoxCell cell4 = new DataGridViewTextBoxCell();
                    cell4.Value = car.Length;
                    row2.Cells.Add(cell4);

                    DataGridViewTextBoxCell cell5 = new DataGridViewTextBoxCell();
                    cell5.Value = car.Width;
                    row2.Cells.Add(cell5);

                    DataGridViewTextBoxCell cell6 = new DataGridViewTextBoxCell();
                    cell6.Value = car.Height;
                    row2.Cells.Add(cell6);

                    DataGridViewTextBoxCell cell7 = new DataGridViewTextBoxCell();
                    cell7.Value = car.MakerCompany;
                    row2.Cells.Add(cell7);

                    DataGridViewTextBoxCell cell8 = new DataGridViewTextBoxCell();
                    cell8.Value = car.Use;
                    row2.Cells.Add(cell8);

                    DataGridViewTextBoxCell cell9 = new DataGridViewTextBoxCell();
                    cell9.Value = car.DriverName;
                    row2.Cells.Add(cell9);

                    DataGridViewTextBoxCell cell10 = new DataGridViewTextBoxCell();
                    cell10.Value = car.Sensor;
                    row2.Cells.Add(cell10);

                    gridManager.Rows.Add(row2);
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }
        
        //적용만. 저장버튼을 누르기 전까진 DB에 저장 안됨
        private void btnOK_Click(object sender, EventArgs e)
        {
            m_arrTempDataGrid.Clear();

            DataGridViewRowCollection arRows = gridManager.Rows;
            if (arRows != null && arRows.Count > 0)
            {
                for (int i = 0; i < arRows.Count; i++)
                {
                    DataGridViewRow row = arRows[i];
                    DataCar car = (DataCar)row.Tag;
                    m_arrTempDataGrid.Add(car);
                }
            }

            UpdateChangeData(m_arrDBData, m_arrTempDataGrid);

            m_arrTempDataGrid.Clear();

            DialogResult = DialogResult.OK;
            this.Close();
        }

        private DataCar FindDataCar(int nID, ArrayList arrCars)
        {
            foreach (DataCar car in arrCars)
            {
                if (car.ID == nID)
                    return car;
            }
            return null;
        }

        private ArrayList m_arrEditCars = new ArrayList();

        private void UpdateChangeData(ArrayList arrOrigin, ArrayList arrCurrent)
        {
            foreach (DataCar car in arrCurrent)
            {
                if(car.ID < 0)
                {
                    EditCar editCar = new EditCar();
                    editCar.Car = car;
                    editCar.Code = car.Code;
                    editCar.SQLType = ChangedData.INSERT;

                    m_arrEditCars.Add(editCar);
                }
            }

            foreach (DataCar car in arrOrigin)
            {
                if (FindDataCar(car.ID, arrCurrent) == null)
                {
                    EditCar editCar = new EditCar();
                    editCar.Car = car;
                    editCar.SQLType = ChangedData.DELETE;

                    m_arrEditCars.Add(editCar);
                }
            }
            UpdateDB(m_arrEditCars);
        }

        private void UpdateDB(ArrayList arr)
        {
            ArrayList arrDeletes = new ArrayList();

            foreach (EditCar editcar in arr)
            {
                //데이터 넣었다가 뺀거면 값을 바꿀 필요가 없음
                if (editcar.ID < 0)
                {
                    if (editcar.SQLType == ChangedData.DELETE)
                    {
                        arrDeletes.Add(editcar);
                    }
                }
            }

            foreach (EditCar editCar in arrDeletes)
            {
                arr.Remove(editCar);
            }

            foreach (EditCar editcar in arr)
            {
                editcar.Update(null);
            }
        }
    }
}
