using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using DBUtility;
using SDMS.Help;
using UnE.GUI;

namespace SDMS
{
	public partial class FormSensorDetectPolicy : PopupFormBase
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
        
        private int m_nSiteID = 1;

        private ManualManager m_manualManager = null;

		public FormSensorDetectPolicy()
		{
            m_nSiteID = UnE.SOP.ProxySOP.Instance.SiteID;

			webDBManager = FormMain.Instance.DBManager;
            this.DoubleBuffered = true;

			InitializeComponent();
             
			mCmbTimeMin.Visible = false;
			mCmbTimeDay.Visible = false;
			//mCmbTimeDay.Location = mCmbTimeMin.Location;
			mCmbTimeHour.Visible = false;
			//mCmbTimeHour.Location = mCmbTimeMin.Location;

			LoadData();
            LoadConfig();
			InitState();

            this.UseFrmMove = false;
            InitCtrlSize(this);
            SetChildCtrlResize(this, this.Width, this.Height); 

            if (UnE.SOP.ProxySOP.Instance.UsePSM == false)
            { 
                btnPSMSignalOn.Enabled = false;
                btnPSMSignalOn.ImageNormal = imgCheckBoxUnChecked;
                btnPSMSignalOn.ImageMouseOver = imgCheckBoxUnChecked;
                btnPSMSignalOn.ImageClicked = imgCheckBoxUnChecked;

                PreferenceManager.Instance.RecivePSMSignal = false;

                if (UnE.SOP.ProxySOP.Instance.UseIntrusion)
                {
                    btnSecuritySignalOn.Visible = lblSecuritySignalOn.Visible = true;
                    btnSecuritySignalOn.Location = btnPSMSignalOn.Location;
                    lblSecuritySignalOn.Location = lblPSMSignalOn.Location;
                    btnPSMSignalOn.Visible = lblPSMSignalOn.Visible = false;
                }
            }

            if (!UnE.SOP.ProxySOP.Instance.UseIntrusion)
                PreferenceManager.Instance.ReciveSecuritySignal = false;
             
            if ((UnE.SOP.ProxySOP.Instance.SiteID == 100))
            {
                lbl_CfgSignalName.Visible = false;
                label4.Visible = false;
                btnFireSignalOn.Visible = false;
                lblFireSignalOn.Visible = false;
                btnPSMSignalOn.Visible = false;
                lblPSMSignalOn.Visible = false;
                label5.Visible = false;
                label6.Visible = false;
            }

            m_manualManager = new ManualManager(this);
            SetManualID();
		}

        private void InitComboBox(int nDuration, ComboBox cbo)
        {
            int nTime = 0;

            if (cbo == mCmbTimeMin)
                nTime = 60;
            else if (cbo == mCmbTimeHour)
                nTime = 3600;
            else if (cbo == mCmbTimeDay)
                nTime = 86400;
            else
                return;

            int nItemCount = cbo.Items.Count;

            for (int i=0;i<nItemCount;i++)
            {
                string str = cbo.Items[i].ToString();
                int nTime2 = GetTimeString(str.Trim());

                if (nTime2 == nDuration / nTime)
                {
                    cbo.SelectedIndex = i;
                    break;
                }
            }
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

            int nDuration = m_nDuration;
			mCmbDetectPolicy.SelectedIndex = m_nDetectPolicy;

			if (m_nDuration > 0 && current != null)
			{
                InitComboBox(nDuration, current);
				/*int nIdx = -1;
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
					current.SelectedIndex = nIdx;*/
			}

            if(m_bReciveFireSignal == true)
            { 
                btnFireSignalOn.ImageNormal = imgCheckBoxChecked;
                btnFireSignalOn.ImageMouseOver = imgCheckBoxChecked;
                btnFireSignalOn.ImageClicked = imgCheckBoxChecked; 
            } 
            else
            {
                btnFireSignalOn.ImageNormal = imgCheckBoxUnChecked;
                btnFireSignalOn.ImageMouseOver = imgCheckBoxUnChecked;
                btnFireSignalOn.ImageClicked = imgCheckBoxUnChecked; 
            }

            if (m_bRecivePSMSignal == true)
            {
                btnPSMSignalOn.ImageNormal = imgCheckBoxChecked;
                btnPSMSignalOn.ImageMouseOver = imgCheckBoxChecked;
                btnPSMSignalOn.ImageClicked = imgCheckBoxChecked;
            }
            else
            {
                btnPSMSignalOn.ImageNormal = imgCheckBoxUnChecked;
                btnPSMSignalOn.ImageMouseOver = imgCheckBoxUnChecked;
                btnPSMSignalOn.ImageClicked = imgCheckBoxUnChecked;
            }

            if (m_bReciveSecuritySignal == true)
            {
                btnSecuritySignalOn.ImageNormal = imgCheckBoxChecked;
                btnSecuritySignalOn.ImageMouseOver = imgCheckBoxChecked;
                btnSecuritySignalOn.ImageClicked = imgCheckBoxChecked;
            }
            else
            {
                btnSecuritySignalOn.ImageNormal = imgCheckBoxUnChecked;
                btnSecuritySignalOn.ImageMouseOver = imgCheckBoxUnChecked;
                btnSecuritySignalOn.ImageClicked = imgCheckBoxUnChecked;
            }
		}

		private void SaveData()
		{
			string szSQL1 = string.Format("UPDATE OptionSDMS SET PropertyValue={0} WHERE PropertyName='AbnormalSensorDetectPolicy' and SiteID = {1}", m_nDetectPolicy, m_nSiteID);
			webDBManager.GetResultData(szSQL1, 0);

			string szSQL2 = string.Format("UPDATE OptionSDMS SET PropertyValue={0} WHERE PropertyName='IgnoreDurate' and SiteID = {1}", m_nDuration, m_nSiteID);
			webDBManager.GetResultData(szSQL2, 0);
		}

		private void LoadData()
		{
            string szSQL = "SELECT PropertyValue FROM OptionSDMS where PropertyName='AbnormalSensorDetectPolicy' and SiteID = " + m_nSiteID.ToString();
			ArrayList arResult = webDBManager.GetResultData(szSQL, 0);
			if (arResult == null || arResult.Count == 0)
			{
				m_nDetectPolicy = 0;
			}
			else
			{
				m_nDetectPolicy = WebDBManager.GetIntField(arResult[0].ToString(), -1);
			}

            string szSQL2 = "SELECT PropertyValue FROM OptionSDMS where PropertyName='IgnoreDurate' and SiteID = " + m_nSiteID.ToString();
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
                ComboBox selected = null;

				switch (nIdx)
				{
					case 0:
						break;

					case 1:
                        selected = mCmbTimeMin;
						break;

					case 2:
                        selected = mCmbTimeHour;
						break;

					case 3:
                        selected = mCmbTimeDay;
						break;

					case 4:
						break;

					default:
						break;
				}

				m_nDetectPolicy = nIdx;

                if (selected != null)
                {
                    selected.Visible = true;

                    if (selected.SelectedIndex < 0 && selected.Items.Count > 0)
                        selected.SelectedIndex = 0;
                }
			}
		}

        private int GetTimeString(string strTime)
        {
            int len = strTime.Length;

            int num = -1;

            for (int i=0;i<len;i++)
            {
                char ch = strTime[i];

                if (ch < '0' || ch > '9')
                {
                    string strNumber = strTime.Substring(0, i);
                    int.TryParse(strNumber, out num);
                    break;
                }
            }

            return num;
        }

		private void CmbTimeMin_SelectedIndexChanged(object sender, EventArgs e)
		{
            int nTime = GetTimeString(mCmbTimeMin.Text.Trim());

            if (nTime >= 0)
                m_nDuration = nTime * 60;

			/*int nIdx = mCmbTimeMin.SelectedIndex;
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
			}*/
		}

		private void CmbTimeHour_SelectedIndexChanged(object sender, EventArgs e)
		{
            int nTime = GetTimeString(mCmbTimeHour.Text.Trim());

            if (nTime >= 0)
                m_nDuration = nTime * 3600;

			/*int nIdx = mCmbTimeHour.SelectedIndex;
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
			}*/
		}

		private void CmbTimeDay_SelectedIndexChanged(object sender, EventArgs e)
		{
            int nTime = GetTimeString(mCmbTimeDay.Text.Trim());

            if (nTime >= 0)
                m_nDuration = nTime * 86400;

			/*int nIdx = mCmbTimeDay.SelectedIndex;
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
			}*/
		} 

        private bool m_bReciveFireSignal = true;
        private bool m_bRecivePSMSignal = true;
        private bool m_bReciveSecuritySignal = true;

        private Image imgCheckBoxUnChecked = global::SDMS.Properties.Resources.CheckBox_Default;
        private Image imgCheckBoxChecked = global::SDMS.Properties.Resources.CheckBox_Click;          

        private void btnFireSignalOn_Click(object sender, EventArgs e)
        {
            if (m_bReciveFireSignal)
            {
                btnFireSignalOn.ImageNormal = imgCheckBoxUnChecked;
                btnFireSignalOn.ImageMouseOver = imgCheckBoxUnChecked;
                btnFireSignalOn.ImageClicked = imgCheckBoxUnChecked;

                m_bReciveFireSignal = false;
            } 
            else
            {
                btnFireSignalOn.ImageNormal = imgCheckBoxChecked;
                btnFireSignalOn.ImageMouseOver = imgCheckBoxChecked;
                btnFireSignalOn.ImageClicked = imgCheckBoxChecked;

                m_bReciveFireSignal = true;
            } 
        }

        private void btnPSMSignalOn_Click(object sender, EventArgs e)
        {
            if (m_bRecivePSMSignal)
            {
                btnPSMSignalOn.ImageNormal = imgCheckBoxUnChecked;
                btnPSMSignalOn.ImageMouseOver = imgCheckBoxUnChecked;
                btnPSMSignalOn.ImageClicked = imgCheckBoxUnChecked;

                m_bRecivePSMSignal = false;
            }
            else
            {
                btnPSMSignalOn.ImageNormal = imgCheckBoxChecked;
                btnPSMSignalOn.ImageMouseOver = imgCheckBoxChecked;
                btnPSMSignalOn.ImageClicked = imgCheckBoxChecked;

                m_bRecivePSMSignal = true;
            } 
        }

        private void btnSecuritySignalOn_Click(object sender, EventArgs e)
        {
            if (m_bReciveSecuritySignal)
            {
                btnSecuritySignalOn.ImageNormal = imgCheckBoxUnChecked;
                btnSecuritySignalOn.ImageMouseOver = imgCheckBoxUnChecked;
                btnSecuritySignalOn.ImageClicked = imgCheckBoxUnChecked;

                m_bReciveSecuritySignal = false;
            }
            else
            {
                btnSecuritySignalOn.ImageNormal = imgCheckBoxChecked;
                btnSecuritySignalOn.ImageMouseOver = imgCheckBoxChecked;
                btnSecuritySignalOn.ImageClicked = imgCheckBoxChecked;

                m_bReciveSecuritySignal = true;
            }
        } 

        private void LoadConfig()
        {
            m_bReciveFireSignal = PreferenceManager.Instance.ReciveFireSignal;
            m_bRecivePSMSignal = PreferenceManager.Instance.RecivePSMSignal;
            m_bReciveSecuritySignal = PreferenceManager.Instance.ReciveSecuritySignal;
        }

        private void SaveConfig()
        {
            PreferenceManager.Instance.ReciveFireSignal = m_bReciveFireSignal;
            PreferenceManager.Instance.RecivePSMSignal = m_bRecivePSMSignal;
            PreferenceManager.Instance.ReciveSecuritySignal = m_bReciveSecuritySignal;
        }

        private void mBtnSave_Click(object sender, EventArgs e)
        {
            SaveData();

            SaveConfig();

            this.Close();
        }

        private void SetManualID()
        {
            m_manualManager.Handle = this.Handle;

            m_manualManager.Clear();

            m_manualManager.SetID(this, "SDMS_Manage_SensorDetection");
            m_manualManager.SetID(lbl_DetectSensorName, "SDMS_Manage_SensorDetection_Malfunction");
            m_manualManager.SetID(lbl_CfgSignalName, "SDMS_Manage_SensorDetection_Deny_SensorDetection"); 
            m_manualManager.ProcessEvent();
        }
	}
}