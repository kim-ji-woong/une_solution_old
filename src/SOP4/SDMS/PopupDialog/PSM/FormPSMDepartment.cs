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

namespace SDMS.PopupDialog
{
    public partial class FormPSMDepartment : Form
    {

        private DBUtility.WebDBManager m_dbMgr = null;
        private UnE.PSM.PSMSensor m_Sensor = null;
        public UnE.PSM.PSMSensor Sensor
        {
            get { return m_Sensor; }
            set 
            {
                m_Sensor = value;
                InitData(m_Sensor);
            }
        }
        
        public FormPSMDepartment(UnE.PSM.PSMSensor sensor)
        {
            m_dbMgr = FormMain.Instance.DBManager;

            m_Sensor = sensor;

            InitializeComponent();

            InitData(m_Sensor);
        }

        private string szDepartment = "";
        private string szPhone = "";
        private void InitData(UnE.PSM.PSMSensor sensor)
        {
            int nSensorID = sensor.ID;
            if (nSensorID <= 0)
                return;

            szDepartment = sensor.Department;
            szPhone = sensor.PhoneNumber;

            txtSensorName.Text = m_Sensor.Name;

            List<UnE.PSM.PSMTank> tankList = m_Sensor.LinkedTankList;
            if (tankList == null || tankList.Count == 0)
                txtLocation.Text = "";
            else
            {
                txtLocation.Text = tankList[0].LocationName;
            }

            txtDepartment.Text = szDepartment;
            txtPhone.Text = szPhone;
        }

        

        private void btnSave_Click(object sender, EventArgs e)
        {
            if( IsChangedData())
            {
                SaveData(m_Sensor);
            }
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private bool IsChangedData()
        {
            if(txtDepartment.Text != szDepartment)
            {
                return true;
            }

            if (txtPhone.Text != szPhone)
                return true;

            return false;
        }

        private void SaveData(UnE.PSM.PSMSensor sensor)
        {
            int nSensor = sensor.ID;
            string szDepart = txtDepartment.Text;
            string szPhone = txtPhone.Text;

            sensor.Department = szDepart;
            sensor.PhoneNumber = szPhone;

            string szSQL = string.Format("UPDATE PSMSensor SET Department = '{0}', DepartmentPhoneNumber = '{1}' WHERE ID = {2}", szDepart, szPhone, nSensor);
            m_dbMgr.GetResultData(szSQL, 0);
        }
    }
}
