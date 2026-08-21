using System.Windows.Forms;
using UnE.Spatial;
using UnE.Sensor;

namespace SDMS
{
	public partial class DockingFormProperties : Form
	{
		private IFormControl m_ctrlPrev = null;
		private DockingFormCCTVProperties m_frmCCTV = null;
		private DockingFormFireEquipmentProperties m_frmFireEquipment = null;
		private DockingFormSensorProperties m_frmSensor = null;

		public DockingFormProperties()
		{
            this.DoubleBuffered = true;

			InitializeComponent();

			m_frmCCTV = new DockingFormCCTVProperties();
			m_frmFireEquipment = new DockingFormFireEquipmentProperties();
			m_frmSensor = new DockingFormSensorProperties();

			this.Controls.Add(m_frmCCTV);
			this.Controls.Add(m_frmFireEquipment);
			this.Controls.Add(m_frmSensor);

			m_frmSensor.Dock = m_frmFireEquipment.Dock = m_frmCCTV.Dock = DockStyle.Fill;
			SetPOI(null);
		}

		public void SetPOI(POI poi)
		{
			IFormControl ctrlCurrent = null;

			if (poi != null)
			{
				if (poi.Type == IFacility.FacilityType.CCTV)
					ctrlCurrent = m_frmCCTV;
                else if (poi.Type == IFacility.FacilityType.FE ||
                    poi.Type == IFacility.FacilityType.HD ||
                    poi.Type == IFacility.FacilityType.FA)
					ctrlCurrent = m_frmFireEquipment;
                else if (poi.Type == IFacility.FacilityType.FIRE_SENSOR ||
                    poi.Type == IFacility.FacilityType.COOLER_SENSOR ||
                    poi.Type == IFacility.FacilityType.PRESSURE_SENSOR)
					ctrlCurrent = m_frmSensor;
			}

			if (m_ctrlPrev == null || m_ctrlPrev != ctrlCurrent)
			{
				if (m_ctrlPrev != null)
					m_ctrlPrev._Hide();

				m_ctrlPrev = ctrlCurrent;
			}

			if (ctrlCurrent != null)
			{
				ctrlCurrent._Update(poi);
				ctrlCurrent._Show();
			}
		}

		public void SetTitle(string szText)
		{
			m_frmCCTV.SetTitle(szText);
			m_frmFireEquipment.SetTitle(szText);
			m_frmSensor.SetTitle(szText);
		
			this.Text = szText;
		}

	}

	public interface IFormControl
	{
		void _Show();

		void _Hide();

		void _Update(POI poi);
	}
}