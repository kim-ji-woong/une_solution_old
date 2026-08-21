using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DBUtility;

namespace SDMS
{
    public partial class FormSensorDetectPolicy : Form
    {
        private int m_nDetectPolicy = 0;
        public int DetectPolicy
        {
            get { return m_nDetectPolicy; }
            set { m_nDetectPolicy = value; }
        }
        private int m_nDuration = 0;
        public int Duration
        {
            get { return m_nDuration; }
            set { m_nDuration = value; }
        }
		private WebDBManager webDBManager = null;
        public FormSensorDetectPolicy()
        {
			webDBManager = FormMain.Instance.DBManager;

            InitializeComponent();

            mCmbTimeMin.Visible = false;
            mCmbTimeDay.Visible = false;
            mCmbTimeDay.Location = mCmbTimeMin.Location;
            mCmbTimeHour.Visible = false;
            mCmbTimeHour.Location = mCmbTimeMin.Location;

			LoadData();

			InitState();			
        }

		private void InitState()
		{
			ComboBox current = null;
			mCmbTimeMin.Visible = false;
			mCmbTimeDay.Visible = false;
			mCmbTimeHour.Visible = false;
			switch (m_nDetectPolicy)
			{
				case 0:
					break;
				case 1:
					
					mCmbTimeMin.Visible = true;
					current = mCmbTimeMin;
					break;
				case 2:
					mCmbTimeHour.Visible = true;
					current = mCmbTimeHour;
					break;
				case 3:
					mCmbTimeDay.Visible = true;
					current = mCmbTimeDay;
					break;
				case 4:
					break;
				default:
					break;
			}
			mCmbDetectPolicy.SelectedIndex = m_nDetectPolicy;

			if (m_nDuration > 0 && current != null)
			{
				int nIdx = -1;
				switch (m_nDuration)
				{
					case 300:
						nIdx = 0;
						break;
					case 900:
						nIdx = 1;
						break;
					case 1800:
						nIdx = 2;
						break;
					case 2700:
						nIdx = 3;
						break;
					case 3600:
						nIdx = 0;
						break;
					case 10800:
						nIdx = 1;
						break;
					case 18000:
						nIdx = 2;
						break;
					case 28800:
						nIdx = 3;
						break;
					case 86400:
						nIdx = 0;
						break;
					case 172800:
						nIdx = 1;
						break;
					case 259200:
						nIdx = 2;
						break;
					case 604800:
						nIdx = 3;
						break;
				}
				if (nIdx != -1)
					current.SelectedIndex = nIdx;
			}	
		}

		private void SaveData()
		{
			string szSQL1 = string.Format("UPDATE OptionSDMS SET PropertyValue={0} WHERE PropertyName='AbnormalSensorDetectPolicy'", m_nDetectPolicy);
			webDBManager.GetResultData(szSQL1, 0);	

			string szSQL2 = string.Format("UPDATE OptionSDMS SET PropertyValue={0} WHERE PropertyName='IgnoreDurate'", m_nDuration);
			webDBManager.GetResultData(szSQL2, 0);
	
		}
		private void LoadData()
		{
			string szSQL = "SELECT PropertyValue FROM OptionSDMS where PropertyName='AbnormalSensorDetectPolicy'";
			ArrayList arResult = webDBManager.GetResultData(szSQL, 0);
			if (arResult == null || arResult.Count == 0)
			{
				m_nDetectPolicy = 0;
			}
			else
			{
				m_nDetectPolicy = WebDBManager.GetIntField(arResult[0].ToString(), -1);
			}
			

			string szSQL2 = "SELECT PropertyValue FROM OptionSDMS where PropertyName='IgnoreDurate'";
			ArrayList arResult2 = webDBManager.GetResultData(szSQL2, 0);
			if (arResult2 == null || arResult2.Count == 0)
			{
				m_nDuration = 0;
			}
			else
			{
				m_nDuration = WebDBManager.GetIntField(arResult2[0].ToString(), -1);
			}
		}		

        private void CmbDetectPolicySelectedIndexChanged(object sender, EventArgs e)
        {

            int nIdx = mCmbDetectPolicy.SelectedIndex;
            if (nIdx != -1)
            {
                mCmbTimeMin.Visible = false;
                mCmbTimeDay.Visible = false;
                mCmbTimeHour.Visible = false;
                switch(nIdx)
                {
                    case 0:
                        break;
                    case 1:
                        mCmbTimeMin.Visible = true;
                        break;
                    case 2:
                        mCmbTimeHour.Visible = true;                     
                        break;
                    case 3:
                        mCmbTimeDay.Visible = true;   
                        break;
                    case 4:
                        break;
                    default:
                        break;
                }
				m_nDetectPolicy = nIdx;
            }
        }

        private void CmbTimeMin_SelectedIndexChanged(object sender, EventArgs e)
        {
            int nIdx = mCmbTimeMin.SelectedIndex;
            if (nIdx != -1)
            {
                switch (nIdx)
                {
                    case 0:
                        m_nDuration = 300;//5
                        break;
                    case 1:
                        m_nDuration = 900;//15
                        break;
                    case 2:
                        m_nDuration = 1800;//30
                        break;
                    case 3:
                        m_nDuration = 2700;//45
                        break;
                    default:
                        m_nDuration = 0;
                        break;
                }
            }
        }

        private void CmbTimeHour_SelectedIndexChanged(object sender, EventArgs e)
        {
            int nIdx = mCmbTimeHour.SelectedIndex;
            if (nIdx != -1)
            {
                switch (nIdx)
                {
                    case 0:
                        m_nDuration = 3600;
                        break;
                    case 1:
                        m_nDuration = 10800;
                        break;
                    case 2:
                        m_nDuration = 18000;
                        break;
                    case 3:
                        m_nDuration = 28800;
                        break;
                    default:
                        m_nDuration = 0;
                        break;
                }
            }
        }

        private void CmbTimeDay_SelectedIndexChanged(object sender, EventArgs e)
        {
            int nIdx = mCmbTimeDay.SelectedIndex;
            if (nIdx != -1)
            {
                switch (nIdx)
                {
                    case 0:
                        m_nDuration = 86400;
                        break;
                    case 1:
                        m_nDuration = 172800;
                        break;
                    case 2:
                        m_nDuration = 259200;
                        break;
                    case 3:
                        m_nDuration = 604800;
                        break;
                    default:
                        m_nDuration = 0;
                        break;
                }
            }
        }

		private void button1_Click(object sender, EventArgs e)
		{
			SaveData();
			this.Close();
		}

		private void mBtnCancel_Click(object sender, EventArgs e)
		{
			this.Close();
		}
    }
}
