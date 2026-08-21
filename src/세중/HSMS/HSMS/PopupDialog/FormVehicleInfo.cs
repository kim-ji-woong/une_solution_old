using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HSMS.PopupDialog
{
    public partial class FormVehicleInfo : Form
    {
        private DataCar m_worker = null;

        public DataCar Worker
        {
            get { return m_worker; }
            set { SetWorker(value); }
        }

        public FormVehicleInfo()
        {
            InitializeComponent();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void SetWorker(DataCar worker)
        {
            if (m_worker == worker)
                return;

            m_worker = worker;

            if (m_worker == null)
            {
                labelWorkerName.Text = "";
                textBoxVehicleCode.Text = "";
                textBoxVehicleName.Text = "";
                textBoxMadeBy.Text = "";
                textBoxTeam.Text = "";
                textBoxSensorID.Text = "";
                textBoxNumber.Text = "";
                textBoxCode.Text = "";
                textBoxUsage.Text = "";
            }
            else
            {
                labelWorkerName.Text = m_worker.Name;
                textBoxVehicleCode.Text = m_worker.Code;

                textBoxVehicleName.Text = m_worker.Name;

                textBoxMadeBy.Text = m_worker.MakerCompany;                
                textBoxTeam.Text = m_worker.TeamCode;               

                textBoxSensorID.Text = m_worker.Sensor;
                if (m_worker.CarType != null)
                    textBoxNumber.Text = m_worker.CarType.Code;
                else
                    textBoxNumber.Text = "";
                if (m_worker.CarStandard != null)
                    textBoxCode.Text = m_worker.CarStandard.Name;
                else
                    textBoxCode.Text = "";
                textBoxUsage.Text = m_worker.Use;
            }

            this.Refresh();
        }
    }
}
