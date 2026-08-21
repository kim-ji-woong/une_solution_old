using System.Windows.Forms;
using UnE.Spatial;
using UnE.Sensor;


namespace SDMS
{
	public partial class DockingFormSensorProperties : Form, IFormControl
	{
		private POI m_poi = null;

		public DockingFormSensorProperties()
		{
            this.DoubleBuffered = true;
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
				ISensor equip = (ISensor)poi.Facility;
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
            IFacility.FacilityType type = equip.Facility.Type;

			if (equip.Facility != null)
			{
				if (equip.Facility.GetType() == typeof(ISensor))
				{
					EquipmentZone equipZone = ZoneManager.Instance.CheckEquipmentZone(equip.Zone, equip.X, equip.Y);
					if (equipZone != null)
                        group = FormMain.Instance.DataManager.GetEquipZoneFacilityManagerGroup(IFacility.FacilityType.CCTV, equipZone, true);
				}
			}

			if (group == null)
			{
				if (equip.Zone == null)
				{
					group = FormMain.Instance.DataManager.GetEntireFacilityManagerGroup(type, true);
				}
				else if (equip.Zone.Building == null)
				{
					group = FormMain.Instance.DataManager.GetOutdoorFacilityManagerGroup(type, equip.Zone, true);
					if (group == null || group.IsEmpty())
						group = FormMain.Instance.DataManager.GetEntireFacilityManagerGroup(type, true);
				}
				else
				{
					group = FormMain.Instance.DataManager.GetBuildingFacilityManagerGroup(type, equip.Zone.Building, true);
					if (group == null || group.IsEmpty())
						group = FormMain.Instance.DataManager.GetEntireFacilityManagerGroup(type, true);
				}
			}

			return FormMain.Instance.DataManager.GetFacilityManagerName(group, ref strPhoneNumber);
		}

		public void SetTitle(string szText)
		{
			this.lbTitle.Text = szText;
			this.Text = szText;
		}
	}
}