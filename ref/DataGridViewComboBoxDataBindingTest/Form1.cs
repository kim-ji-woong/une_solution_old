using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SDMS
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            ZoneManager.Instance.LoadBuildingData();            
            ZoneManager.Instance.LoadZones();
            ZoneManager.Instance.LoadEquipmentZone();

            dataGridView1.DataSource = null;
            Dictionary<int, UnE.Spatial.Zone> dicZone = ZoneManager.Instance.DicZones;
           
            foreach(UnE.Spatial.Zone zone in dicZone.Values)
            {
                bindingSourceZone.Add(zone);
            }


            DataGridViewComboBoxColumn col = (DataGridViewComboBoxColumn)dataGridView1.Columns[3];
            //col.DataSource = Enum.GetValues(typeof(UnE.Spatial.EquipmentZone.EquipZoneType));
           // col.ValueType = typeof(UnE.Spatial.EquipmentZone.EquipZoneType);
           // col.DataPropertyName = "UnE.Spatial.EquipmentZone.EquipZoneType";

         
            col.DataSource = new UnE.Spatial.EquipmentZone.EquipZoneType[] 
                        {   UnE.Spatial.EquipmentZone.EquipZoneType.NOTUSED,
                            UnE.Spatial.EquipmentZone.EquipZoneType.SENSOR_TYPE,
                            UnE.Spatial.EquipmentZone.EquipZoneType.FA_TYPE,
                            UnE.Spatial.EquipmentZone.EquipZoneType.PSM_TYPE,
                            UnE.Spatial.EquipmentZone.EquipZoneType.UNKOWN }
                .Select(value => new { Display = value.ToString(), Value = (UnE.Spatial.EquipmentZone.EquipZoneType)value })
                .ToList();
            col.ValueType = typeof(UnE.Spatial.EquipmentZone.EquipZoneType);
            col.ValueMember = "Value";
            col.DisplayMember = "Display";

            Dictionary<int, UnE.Spatial.EquipmentZone> dicEquipZone = ZoneManager.Instance.DicEquipZones;

            foreach (UnE.Spatial.EquipmentZone zone in dicEquipZone.Values)
            {
                bindingSourceEquipZone.Add(zone);
            }

            

            dataGridView1.DataSource = bindingSourceEquipZone;
        }

        private void dataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            System.Diagnostics.Trace.WriteLine(e.Exception);
        }

        private void dataGridView1_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {

        }
    }
}
