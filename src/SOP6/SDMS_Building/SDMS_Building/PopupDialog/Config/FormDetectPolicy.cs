using DBUtility2;
using SDMS_Building.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using UnE.GUI;

namespace SDMS_Building.PopupDialog.Config
{
    public partial class FormDetectPolicy : Form
    {
		private UEWpfControl.WpfComboBox m_cbDetectPolicy = null;
		private UEWpfControl.WpfComboBox m_cbTimeMin = null;
		private UEWpfControl.WpfComboBox m_cbTimeDay = null;
		private UEWpfControl.WpfComboBox m_cbTimeHour = null;
		private UEWpfControl.WpfComboBox m_cbSignal = null;


		private int m_nDetectPolicy = 0;
		private int m_nSiteID = 1;
		private int m_nDuration = 0;

		private WebDBManager webDBManager = null;

		private bool m_bReciveFireSignal = true;
		private bool m_bRecivePSMSignal = true;
		private bool m_bReciveEarthquakeSignal = true;
		private bool m_bReciveBlackoutSignal = true;
		private bool m_bReciveDoorSignal = true;
		private bool m_bReciveStrongWindSignal = true;
		private bool m_bReciveFirewallSignal = true;

        private bool m_bGoOutside = true;

		public FormDetectPolicy()
        {
			m_nSiteID = UnE.SOP.ProxySOP.Instance.SiteID;
			webDBManager = FormMain.Instance.DBManager;

			InitializeComponent();

            this.DoubleBuffered = true;

			m_cbDetectPolicy = new UEWpfControl.WpfComboBox();
			eleDetectPolicy.Child = m_cbDetectPolicy;
			m_cbDetectPolicy.customComboBox.SelectionChanged += DetectPolicyComboBox_SelectionChanged;
			m_cbDetectPolicy.SetSize(eleDetectPolicy.Width, eleDetectPolicy.Height);

			m_cbTimeMin = new UEWpfControl.WpfComboBox();
			m_cbTimeMin.customComboBox.SelectionChanged += TimeMinComboBox_SelectionChanged;
			m_cbTimeMin.SetSize(eleTime.Width, eleTime.Height);

			m_cbTimeDay = new UEWpfControl.WpfComboBox();
			m_cbTimeDay.customComboBox.SelectionChanged += TimeDayComboBox_SelectionChanged;
			m_cbTimeDay.SetSize(eleTime.Width, eleTime.Height);

			m_cbTimeHour = new UEWpfControl.WpfComboBox();
			m_cbTimeHour.customComboBox.SelectionChanged += TimeHourComboBox_SelectionChanged;
			m_cbTimeHour.SetSize(eleTime.Width, eleTime.Height);

			eleTime.Visible = false;

			m_cbSignal = new UEWpfControl.WpfComboBox();
			eleSignal.Child = m_cbSignal;
			m_cbSignal.customComboBox.SelectionChanged += SignalComboBox_SelectionChanged;
			m_cbSignal.SetSize(eleSignal.Width, eleSignal.Height);

			LoadConfig();

			InitDetectPolicyComboBox();
			InitTimeComboBox();
			InitSignalComboBox();

			LoadData();
			InitState();

            m_bGoOutside = FormMain.Instance.bGoOutside;
            SetBtnSignal(btnGoOutside, m_bGoOutside);
        }

		public void Save()
		{
			SaveData();
			SaveConfig();
            SetGoOutside();
        }

		private void SaveData()
		{
			string szSQL1 = string.Format("UPDATE OptionSDMS SET PropertyValue={0} WHERE PropertyName='AbnormalSensorDetectPolicy' and SiteID = {1}", m_nDetectPolicy, m_nSiteID);
			webDBManager.GetResultData(szSQL1);

			
			string szSQL2 = string.Format("UPDATE OptionSDMS SET PropertyValue={0} WHERE PropertyName='IgnoreDurate' and SiteID = {1}", m_nDuration, m_nSiteID);
			webDBManager.GetResultData(szSQL2);
		}

		private void LoadData()
		{
			string szSQL = "SELECT PropertyValue FROM OptionSDMS where PropertyName='AbnormalSensorDetectPolicy' and SiteID = " + m_nSiteID.ToString();
			ArrayList arResult = webDBManager.GetResultData(szSQL);
			if (arResult == null || arResult.Count == 0)
			{
				m_nDetectPolicy = 0;
			}
			else
			{
				m_nDetectPolicy = WebDBManager.GetIntField(arResult[0].ToString(), -1);
			}

			string szSQL2 = "SELECT PropertyValue FROM OptionSDMS where PropertyName='IgnoreDurate' and SiteID = " + m_nSiteID.ToString();
			ArrayList arResult2 = webDBManager.GetResultData(szSQL2);
			if (arResult2 == null || arResult2.Count == 0)
			{
				m_nDuration = 0;
			}
			else
			{
				m_nDuration = WebDBManager.GetIntField(arResult2[0].ToString(), -1);
			}

		}

		private void LoadConfig()
		{
			m_bReciveFireSignal = PreferenceManager.Instance.ReciveFireSignal;
			m_bRecivePSMSignal = PreferenceManager.Instance.RecivePSMSignal;
			m_bReciveEarthquakeSignal = PreferenceManager.Instance.ReciveEarthquakeSignal;
			m_bReciveBlackoutSignal = PreferenceManager.Instance.ReciveBlackoutSignal;
			m_bReciveDoorSignal = PreferenceManager.Instance.ReciveSecuritySignal;
			m_bReciveStrongWindSignal = PreferenceManager.Instance.ReciveStrongWindSignal;
			m_bReciveFirewallSignal = PreferenceManager.Instance.ReciveFirewallSignal;
		}

		private void SaveConfig()
		{
			PreferenceManager.Instance.ReciveFireSignal = m_bReciveFireSignal;
			PreferenceManager.Instance.RecivePSMSignal = m_bRecivePSMSignal;
			PreferenceManager.Instance.ReciveSecuritySignal = m_bReciveDoorSignal;
			PreferenceManager.Instance.ReciveEarthquakeSignal = m_bReciveEarthquakeSignal;
			PreferenceManager.Instance.ReciveBlackoutSignal = m_bReciveBlackoutSignal;
			PreferenceManager.Instance.ReciveStrongWindSignal = m_bReciveStrongWindSignal;
			PreferenceManager.Instance.ReciveFirewallSignal = m_bReciveFirewallSignal;
		}

		private void InitDetectPolicyComboBox()
		{
			/*
			m_cbBuilding.customComboBox.DisplayMemberPath = "BuildingName";
			foreach (KeyValuePair<int, Building> item in UnE.Spatial.ZoneManager.Instance.DicBuildings)
			{
				m_cbBuilding.customComboBox.Items.Add(item.Value);
			}

			if (m_cbBuilding.customComboBox.Items.Count > 0)
				m_cbBuilding.customComboBox.SelectedIndex = 0;
			*/

			m_cbDetectPolicy.customComboBox.Items.Add("모든 탐지 값을 표시");
			m_cbDetectPolicy.customComboBox.Items.Add("몇 분 동안 표시하지 않습니다");
			m_cbDetectPolicy.customComboBox.Items.Add("몇 시간 동안 표시하지 않습니다");
			m_cbDetectPolicy.customComboBox.Items.Add("몇 일 동안 표시하지 않습니다");
			m_cbDetectPolicy.customComboBox.Items.Add("완전히 표시하지 않습니다");

			if (m_cbDetectPolicy.customComboBox.Items.Count > 0)
				m_cbDetectPolicy.customComboBox.SelectedIndex = 0;
		}

		private void InitTimeComboBox()
		{
			m_cbTimeMin.customComboBox.Items.Add("5분");
			m_cbTimeMin.customComboBox.Items.Add("15분");
			m_cbTimeMin.customComboBox.Items.Add("30분");
			m_cbTimeMin.customComboBox.Items.Add("45분");

			m_cbTimeDay.customComboBox.Items.Add("1일");
			m_cbTimeDay.customComboBox.Items.Add("2일");
			m_cbTimeDay.customComboBox.Items.Add("3일");
			m_cbTimeDay.customComboBox.Items.Add("5일");
			m_cbTimeDay.customComboBox.Items.Add("7일");
			m_cbTimeDay.customComboBox.Items.Add("10일");
			m_cbTimeDay.customComboBox.Items.Add("15일");
			m_cbTimeDay.customComboBox.Items.Add("30일");

			m_cbTimeHour.customComboBox.Items.Add("1시간");
			m_cbTimeHour.customComboBox.Items.Add("2시간");
			m_cbTimeHour.customComboBox.Items.Add("3시간");
			m_cbTimeHour.customComboBox.Items.Add("4시간");
			m_cbTimeHour.customComboBox.Items.Add("5시간");
			m_cbTimeHour.customComboBox.Items.Add("6시간");
			m_cbTimeHour.customComboBox.Items.Add("8시간");
			m_cbTimeHour.customComboBox.Items.Add("10시간");
			m_cbTimeHour.customComboBox.Items.Add("12시간");
		}

		private void InitSignalComboBox()
		{
			m_cbSignal.customComboBox.Items.Add(Data.CommonString.POI_Fire_Kor);

			if (UnE.SOP.ProxySOP.Instance.UsePSM)
			{
				m_cbSignal.customComboBox.Items.Add(Data.CommonString.POI_Gas_Kor);
			}

			//if (UnE.SOP.ProxySOP.Instance.UseDoor)
			//{
			//	m_cbSignal.customComboBox.Items.Add("출입문");
			//}

			if (UnE.SOP.ProxySOP.Instance.UseEarthquake)
			{
				m_cbSignal.customComboBox.Items.Add(Data.CommonString.POI_Earthquake_Kor);
			}

			//if (UnE.SOP.ProxySOP.Instance.UseFirewall)
			//{
			//	m_cbSignal.customComboBox.Items.Add("방화벽");
			//}

			if (UnE.SOP.ProxySOP.Instance.UseBlackout)
			{
				m_cbSignal.customComboBox.Items.Add(Data.CommonString.POI_Blackout_Kor);
			}

			if (UnE.SOP.ProxySOP.Instance.UseStrongWind)
			{
				m_cbSignal.customComboBox.Items.Add(Data.CommonString.POI_StrongWind_Kor);
			}

			m_cbSignal.customComboBox.SelectedIndex = 0;
		}


		private void InitState()
		{
			UEWpfControl.WpfComboBox current = null;

			switch (m_nDetectPolicy)
			{
				case 0:
					break;

				case 1:
					//mCmbTimeMin.Visible = true;
					current = m_cbTimeMin;
					break;

				case 2:
					//mCmbTimeHour.Visible = true;
					current = m_cbTimeHour;
					break;

				case 3:
					//mCmbTimeDay.Visible = true;
					current = m_cbTimeDay;
					break;

				case 4:
					break;

				default:
					break;
			}

			if (current != null)
			{
				eleTime.Child = current;
				eleTime.Visible = true;
			}

			int nDuration = m_nDuration;
			m_cbDetectPolicy.customComboBox.SelectedIndex = m_nDetectPolicy;

			if (m_nDuration > 0 && current != null)
			{
				InitComboBox(nDuration, current);
			}
		}

		private void InitComboBox(int nDuration, UEWpfControl.WpfComboBox cbo)
		{
			int nTime = 0;

			if (cbo == m_cbTimeMin)
				nTime = 60;
			else if (cbo == m_cbTimeHour)
				nTime = 3600;
			else if (cbo == m_cbTimeDay)
				nTime = 86400;
			else
				return;

			int nItemCount = cbo.customComboBox.Items.Count;

			for (int i = 0; i < nItemCount; i++)
			{
				string str = cbo.customComboBox.Items[i].ToString();
				int nTime2 = GetTimeString(str.Trim());

				if (nTime2 == nDuration / nTime)
				{
					cbo.customComboBox.SelectedIndex = i;
					break;
				}
			}
		}

		private void DetectPolicyComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
		{
			int nIdx = m_cbDetectPolicy.customComboBox.SelectedIndex;

			if (nIdx != -1)
			{
				eleTime.Visible = false;
				UEWpfControl.WpfComboBox selected = null;

				switch (nIdx)
				{
					case 0:
						break;

					case 1:
						selected = m_cbTimeMin;
						break;

					case 2:
						selected = m_cbTimeHour;
						break;

					case 3:
						selected = m_cbTimeDay;
						break;

					case 4:
						break;

					default:
						break;
				}

				m_nDetectPolicy = nIdx;

				if (selected != null)
				{
					eleTime.Child = selected;
					eleTime.Visible = true; 

					if (selected.customComboBox.SelectedIndex < 0 && selected.customComboBox.Items.Count > 0)
						selected.customComboBox.SelectedIndex = 0;
				}
			}
		}

		private void TimeMinComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
		{
			int nTime = GetTimeString((string)m_cbTimeMin.customComboBox.SelectedItem);

			if (nTime >= 0)
				m_nDuration = nTime * 60;
		}

		private void TimeDayComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
		{
			int nTime = GetTimeString((string)m_cbTimeDay.customComboBox.SelectedItem);

			if (nTime >= 0)
				m_nDuration = nTime * 86400;
		}

		private void TimeHourComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
		{
			int nTime = GetTimeString((string)m_cbTimeHour.customComboBox.SelectedItem);

			if (nTime >= 0)
				m_nDuration = nTime * 3600;
		}

		private void SignalComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
		{
			string strValue = (string)m_cbSignal.customComboBox.SelectedItem;

			if (strValue == Data.CommonString.POI_Fire_Kor)
			{
				SetBtnSignal(btnSignal, m_bReciveFireSignal);
			}
			else if (strValue == Data.CommonString.POI_Gas_Kor)
			{
				SetBtnSignal(btnSignal, m_bRecivePSMSignal);
			}
			else if (strValue == Data.CommonString.POI_Door_Kor)
			{
				SetBtnSignal(btnSignal, m_bReciveDoorSignal);
			}
			else if (strValue == Data.CommonString.POI_Earthquake_Kor)
			{
				SetBtnSignal(btnSignal, m_bReciveEarthquakeSignal);
			}
			else if (strValue == Data.CommonString.POI_FireWall_Kor)
			{
				SetBtnSignal(btnSignal, m_bReciveFirewallSignal);
			}
			else if (strValue == Data.CommonString.POI_Blackout_Kor)
			{
				SetBtnSignal(btnSignal, m_bReciveBlackoutSignal);
			}
			else if (strValue == Data.CommonString.POI_StrongWind_Kor)
			{
				SetBtnSignal(btnSignal, m_bReciveStrongWindSignal);
			}
		}

		private void SetBtnSignal(ImageButton btn, bool status)
		{
			if (status == true)
                btn.ImageNormal = global::SDMS_Building.Properties.Resources.check_Checked;
			else
                btn.ImageNormal = global::SDMS_Building.Properties.Resources.check_UnChecked;

			btn.Refresh();
		}

		private int GetTimeString(string strTime)
		{
			int len = strTime.Length;

			int num = -1;

			for (int i = 0; i < len; i++)
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

		private void btnSignal_Click(object sender, EventArgs e)
		{
			string strValue = (string)m_cbSignal.customComboBox.SelectedItem;

			if (strValue == Data.CommonString.POI_Fire_Kor)
			{
				if (m_bReciveFireSignal == true)
					m_bReciveFireSignal = false;
				else
					m_bReciveFireSignal = true;

				SetBtnSignal(btnSignal, m_bReciveFireSignal);
			}
			else if (strValue == Data.CommonString.POI_Gas_Kor)
			{
				if (m_bRecivePSMSignal == true)
					m_bRecivePSMSignal = false;
				else
					m_bRecivePSMSignal = true;

				SetBtnSignal(btnSignal, m_bRecivePSMSignal);
			}
			else if (strValue == Data.CommonString.POI_Door_Kor)
			{
				if (m_bReciveDoorSignal == true)
					m_bReciveDoorSignal = false;
				else
					m_bReciveDoorSignal = true;

				SetBtnSignal(btnSignal, m_bReciveDoorSignal);
			}
			else if (strValue == Data.CommonString.POI_Earthquake_Kor)
			{
				if (m_bReciveEarthquakeSignal == true)
					m_bReciveEarthquakeSignal = false;
				else
					m_bReciveEarthquakeSignal = true;

				SetBtnSignal(btnSignal, m_bReciveEarthquakeSignal);
			}
			else if (strValue == Data.CommonString.POI_FireWall_Kor)
			{
				if (m_bReciveFirewallSignal == true)
					m_bReciveFirewallSignal = false;
				else
					m_bReciveFirewallSignal = true;

				SetBtnSignal(btnSignal, m_bReciveFirewallSignal);
			}
			else if (strValue == Data.CommonString.POI_Blackout_Kor)
			{
				if (m_bReciveBlackoutSignal == true)
					m_bReciveBlackoutSignal = false;
				else
					m_bReciveBlackoutSignal = true;

				SetBtnSignal(btnSignal, m_bReciveBlackoutSignal);
			}
			else if (strValue == Data.CommonString.POI_StrongWind_Kor)
			{
				if (m_bReciveStrongWindSignal == true)
					m_bReciveStrongWindSignal = false;
				else
					m_bReciveStrongWindSignal = true;

				SetBtnSignal(btnSignal, m_bReciveStrongWindSignal);
			}
		}

        private void SetGoOutside()
        {
            DBUtility2.Utility util = new DBUtility2.Utility();
            util.setinivalue("SDMS", "go_outside", m_bGoOutside ? "1" : "0");

            FormMain.Instance.bGoOutside = m_bGoOutside;
        }
        
        private void btnGoOutside_Click(object sender, EventArgs e)
        {
            m_bGoOutside = !m_bGoOutside;

            SetBtnSignal(btnGoOutside, m_bGoOutside);
        }
    }
}
