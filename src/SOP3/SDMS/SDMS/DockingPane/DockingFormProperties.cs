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
    public partial class DockingFormProperties : Form
    {
        private IFormControl m_ctrlPrev = null;
        private DockingFormCCTVProperties m_frmCCTV = null;
        private DockingFormFireEquipmentProperties m_frmFireEquipment = null;
		private DockingFormSensorProperties m_frmSensor = null;

        public DockingFormProperties()
        {
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
				if (poi.Type == Facility.FacilityType.CCTV)
					ctrlCurrent = m_frmCCTV;
				else if (poi.Type == Facility.FacilityType.FE ||
					poi.Type == Facility.FacilityType.HD ||
					poi.Type == Facility.FacilityType.FA)
					ctrlCurrent = m_frmFireEquipment;

				else if (poi.Type == Facility.FacilityType.FIRE_SENSOR ||
					poi.Type == Facility.FacilityType.COOLER_SENSOR ||
					poi.Type == Facility.FacilityType.PRESSURE_SENSOR)
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
    }

    public interface IFormControl
    {
        void _Show();
        void _Hide();
        void _Update(POI poi);
    }
}
