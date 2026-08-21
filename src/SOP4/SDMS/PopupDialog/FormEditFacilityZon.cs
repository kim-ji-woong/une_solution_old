using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using UnE.Spatial;
using UnE.Sensor;


namespace SDMS
{
	public partial class FormEditFacilityZone : Form
	{
        private EquipmentZoneObjectList mCurrentLIst = null;
        public EquipmentZoneObjectList EquipmentZoneObjectList
        {
            get { return mCurrentLIst; }
            set
            { 
                mCurrentLIst = value;
                if (mCurrentLIst != null)
                {
                    AddGridData(mCurrentLIst);
                }
                else
                {
                    gridSensorList.ClearSelection();
                    gridSensorList.Rows.Clear();
                }
            }
        }

        public ArrayList GetSensorList()
        {
            ArrayList arSensorList = new ArrayList();
            foreach (DataGridViewRow row in gridSensorList.Rows)
            {
                arSensorList.Add(row.Tag);
            }
            return arSensorList;
        }

        private void AddGridData(EquipmentZoneObjectList list)
        {
            gridSensorList.ClearSelection();
            gridSensorList.Rows.Clear();

            int nCount = 1;

            foreach (ISensor iSensor in list.SensorList)
            {
                if (iSensor.GetType().IsAssignableFrom(typeof(FireSensor)))
                {
                    FireSensor sensor = (FireSensor)iSensor;
                    DataGridViewRow row = new DataGridViewRow();

                    DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
                    cell.Value = nCount.ToString();
                    row.Cells.Add(cell);

                    DataGridViewTextBoxCell cell2 = new DataGridViewTextBoxCell();
                    cell2.Value = sensor.TypeString;
                    row.Cells.Add(cell2);

                    DataGridViewTextBoxCell cell4 = new DataGridViewTextBoxCell();
                    cell4.Value = sensor.SensorName;
                    row.Cells.Add(cell4);

                    DataGridViewTextBoxCell cell3 = new DataGridViewTextBoxCell();
                    cell3.Value = sensor.PositionName;
                    row.Cells.Add(cell3);

                    row.Tag = sensor;
                    gridSensorList.Rows.Add(row);

                    nCount++;
                }
            }
        }

        private void AddGridExtraData(ArrayList list)
        {
            gridExtraList.ClearSelection();
            gridExtraList.Rows.Clear();

            int nCount = 1;
            foreach (ISensor iSensor in list)
            {
                if (iSensor.GetType().IsAssignableFrom(typeof(FireSensor)))
                {
                    FireSensor sensor = (FireSensor)iSensor;
                    DataGridViewRow row = new DataGridViewRow();
                    DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
                    cell.Value = nCount.ToString();
                    row.Cells.Add(cell);

                    DataGridViewTextBoxCell cell2 = new DataGridViewTextBoxCell();
                    cell2.Value = sensor.TypeString;
                    row.Cells.Add(cell2);

                    DataGridViewTextBoxCell cell4 = new DataGridViewTextBoxCell();
                    cell4.Value = sensor.SensorName;
                    row.Cells.Add(cell4);

                    DataGridViewTextBoxCell cell3 = new DataGridViewTextBoxCell();
                    cell3.Value = sensor.PositionName;
                    row.Cells.Add(cell3);

                    row.Tag = sensor;
                    gridExtraList.Rows.Add(row);

                    nCount++;
                }
            }
        }

        public FormEditFacilityZone()
		{
			InitializeComponent();

            this.DoubleBuffered = true;
            FormMain.SetDoubleBuffer(gridExtraList, true);
            FormMain.SetDoubleBuffer(gridSensorList, true);
            
            ArrayList arExtraSensor = SensorManager.Instance.GetExtranSensorList();
            List<ISensor> arEditedList = SensorManager.Instance.EditSensorList;
            arExtraSensor.AddRange(arEditedList);
            AddGridExtraData(arExtraSensor);
		}

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            ArrayList arSensorList = new ArrayList();
            DataGridViewSelectedRowCollection arList = gridSensorList.SelectedRows;

            if( arList != null)
            {
                foreach(DataGridViewRow row in arList)
                {
                    arSensorList.Add(row.Tag);
                    gridSensorList.Rows.Remove(row);
                }
            }

            int nCount = gridExtraList.Rows.Count + 1;
            foreach (ISensor iSensor in arSensorList)
            {
                if( iSensor.GetType().IsAssignableFrom(typeof(FireSensor)))
                {
                    FireSensor sensor = (FireSensor)iSensor;

                    DataGridViewRow row = new DataGridViewRow();

                    DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
                    cell.Value = nCount.ToString();
                    row.Cells.Add(cell);

                    DataGridViewTextBoxCell cell2 = new DataGridViewTextBoxCell();
                    cell2.Value = sensor.TypeString;
                    row.Cells.Add(cell2);
                
                    DataGridViewTextBoxCell cell4 = new DataGridViewTextBoxCell();
                    cell4.Value = sensor.SensorName;
                    row.Cells.Add(cell4);

                    DataGridViewTextBoxCell cell3 = new DataGridViewTextBoxCell();
                    cell3.Value = sensor.PositionName;
                    row.Cells.Add(cell3);

                    row.Tag = sensor;
                    gridExtraList.Rows.Add(row);

                     nCount++;
                }
            }

            ReOrderGrid(gridSensorList);
            ReOrderGrid(gridExtraList);
        }

        private void ReOrderGrid(DataGridView grid)
        {
            int count = 1;
            foreach (DataGridViewRow row in grid.Rows)
            {
                DataGridViewTextBoxCell cell = (DataGridViewTextBoxCell)row.Cells[0];
                cell.Value = count.ToString();
                count++;
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            ArrayList arSensorList = new ArrayList();
            DataGridViewSelectedRowCollection arList = gridExtraList.SelectedRows;

            if (arList != null)
            {
                foreach (DataGridViewRow row in arList)
                {
                    arSensorList.Add(row.Tag);
                    gridExtraList.Rows.Remove(row);
                }
            }          

            int nCount = gridSensorList.Rows.Count + 1;            
            foreach (ISensor iSensor in arSensorList)
            {
                if (iSensor.GetType().IsAssignableFrom(typeof(FireSensor)))
                {
                    FireSensor sensor = (FireSensor)iSensor;

                    DataGridViewRow row = new DataGridViewRow();

                    DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
                    cell.Value = nCount.ToString();
                    row.Cells.Add(cell);

                    DataGridViewTextBoxCell cell2 = new DataGridViewTextBoxCell();
                    cell2.Value = sensor.TypeString;
                    row.Cells.Add(cell2);

                    DataGridViewTextBoxCell cell4 = new DataGridViewTextBoxCell();
                    cell4.Value = sensor.SensorName;
                    row.Cells.Add(cell4);

                    DataGridViewTextBoxCell cell3 = new DataGridViewTextBoxCell();
                    cell3.Value = sensor.PositionName;
                    row.Cells.Add(cell3);

                    row.Tag = sensor;
                    gridSensorList.Rows.Add(row);

                    nCount++;
                }
            }
            ReOrderGrid(gridSensorList);
            ReOrderGrid(gridExtraList);
        }		
	}
}