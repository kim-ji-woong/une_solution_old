using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SDMS
{
    public partial class DockingFormSensorProperties : Form, IFormControl
    {
        private POI m_poi = null;

		public DockingFormSensorProperties()
        {
            InitializeComponent();

            this.TopLevel = false;
            _Update(null);
        }

        public void _Show()
        {
            this.Show();
        }

        public void _Hide()
        {
            this.Hide();
        }

        public void _Update(POI poi)
        {
            m_poi = poi;

            if (poi == null)
            {
                labelType.Text = "";
                labelEquipNo.Text = "";
                labelManagerName.Text = "";
                labelPhoneNumber.Text = "";
            }
            else
            {
				SensorZone equip = (SensorZone)poi.Facility;
                string strPhoneNumber = "";

                labelType.Text = equip.TypeString;
                labelEquipNo.Text = equip.TypeString + "_" + equip.ID.ToString();

				labelManagerName.Text = GetManager(poi, ref strPhoneNumber);
				labelPhoneNumber.Text = strPhoneNumber;
            }
        }

		private string GetManager(POI equip, ref string strPhoneNumber)
        {
            FacilityManagerGroup group = null;
			Facility.FacilityType type = equip.Facility.Type;

			if (equip.Facility != null)
			{
				if (equip.Facility.GetType() == typeof(SensorZone))
				{
					EquipmentZone equipZone = ZoneManager.Instance.CheckEquipmentZone(equip.Zone, equip.X, equip.Y);
					if (equipZone != null)
						group = FormMain.Instance.DataManager.GetEquipZoneFacilityManagerGroup(Facility.FacilityType.CCTV, equipZone);
				}
			}

			if (group == null)
			{

				if (equip.Zone == null)
				{
					group = FormMain.Instance.DataManager.GetEntireFacilityManagerGroup(type);
				}
				else if (equip.Zone.Building == null)
				{
					group = FormMain.Instance.DataManager.GetOutdoorFacilityManagerGroup(type, equip.Zone);
					if (group == null || group.IsEmpty())
						group = FormMain.Instance.DataManager.GetEntireFacilityManagerGroup(type);
				}
				else
				{
					group = FormMain.Instance.DataManager.GetBuildingFacilityManagerGroup(type, equip.Zone.Building);
					if (group == null || group.IsEmpty())
						group = FormMain.Instance.DataManager.GetEntireFacilityManagerGroup(type);
				}
			}

            return FormMain.Instance.DataManager.GetFacilityManagerName(group, ref strPhoneNumber);
        }
    }
}
