using System.Windows.Forms;
using UnE.Spatial;
using UnE.Sensor;


namespace SDMS
{
	public partial class DockingFormFireEquipmentProperties : Form, IFormControl
	{
		private POI m_poi = null;

		public DockingFormFireEquipmentProperties()
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
				textBoxStatus.Text = "";
				textBoxLastCheckedTime.Text = "";
				textBoxCheckersOpinion.Text = "";
			}
			else
			{
				//m_ignoreChange = true;

				FireEquipment equip = (FireEquipment)poi.Facility;
				string strPhoneNumber = "";

				labelType.Text = equip.TypeString;
				labelEquipNo.Text = equip.TypeString + "_" + equip.EquipID;
				labelManagerName.Text = GetManager(equip, ref strPhoneNumber);
				labelPhoneNumber.Text = strPhoneNumber;
				textBoxStatus.Text = equip.StatusString;

				if (equip.Status != FireEquipment.EquipmentStatus.UNKNOWN)
				{
					textBoxLastCheckedTime.Text = string.Format("{0} {1:00}:{2:00}:{3:00}",
						equip.LastCheckedTime.ToShortDateString(),
						equip.LastCheckedTime.Hour,
						equip.LastCheckedTime.Minute,
						equip.LastCheckedTime.Second);
				}
				else
					textBoxLastCheckedTime.Text = "";

				textBoxCheckersOpinion.Text = equip.CheckersOpinion;
			}
		}

		private string GetManager(FireEquipment equip, ref string strPhoneNumber)
		{
			FacilityManagerGroup group = null;

			if (equip.POI != null && equip.POI.Facility != null)
			{
                if (equip.POI.Facility.GetType() == typeof(ISensor))
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
                    group = FormMain.Instance.DataManager.GetEntireFacilityManagerGroup(IFacility.FacilityType.FE, true);
				}
				else if (equip.Zone.Building == null)
				{
                    group = FormMain.Instance.DataManager.GetOutdoorFacilityManagerGroup(IFacility.FacilityType.FE, equip.Zone, true);
					if (group == null || group.IsEmpty())
                        group = FormMain.Instance.DataManager.GetEntireFacilityManagerGroup(IFacility.FacilityType.FE, true);
				}
				else
				{
                    group = FormMain.Instance.DataManager.GetBuildingFacilityManagerGroup(IFacility.FacilityType.FE, equip.Zone.Building, true);
					if (group == null || group.IsEmpty())
                        group = FormMain.Instance.DataManager.GetEntireFacilityManagerGroup(IFacility.FacilityType.FE, true);
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