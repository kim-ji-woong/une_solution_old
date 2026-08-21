using System;
using System.Collections;
using System.Windows.Forms;
using UnE.Spatial;
using UnE.Sensor;

namespace SDMS
{
	public partial class DockingFormCCTVProperties : Form, IFormControl
	{
		private POI m_poi = null;
		private string m_strDefaultPortNo = "";
		private string[] m_strPrevIP = new string[] { "", "", "", "" };
		private string m_strPrevPortNo = "";
		private bool m_ignoreChange = false;
		private string m_strPrevCameraName = "";

        private string m_szUserName = "";
        private string m_szPassword = "";
        private string m_szChannel = "";
        private string m_szStream = "";




		public DockingFormCCTVProperties()
		{
            this.DoubleBuffered = true;
			InitializeComponent();

			textBoxPortNo.Text = CCTV.DefaultPortNo.ToString();
			m_strDefaultPortNo = textBoxPortNo.Text;
			m_strPrevPortNo = textBoxPortNo.Text;
			m_strPrevCameraName = textBoxCameraName.Text;

            m_szUserName = UserName.Text;
            m_szPassword = Password.Text;
            m_szChannel = Channel.Text;
            m_szStream = Stream.Text;

			this.TopLevel = false;

			ShowLODDescription(false);

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

            if (poi == null || poi.Type != IFacility.FacilityType.CCTV)
			{
				labelCCTVID.Text = "";
				labelType.Text = "";
				textBoxManager.Text = "";
				textBoxPhoneNumber.Text = "";
				textBoxPortNo.Text = m_strDefaultPortNo;
				textBoxCameraName.Text = "";
				cboLOD.SelectedIndex = 1;

                CCTVTypes.SelectedIndex = 0;

				//byte[] arrIP = new byte[4] { 0, 0, 0, 0 };
				//ipAddressControl1.SetAddressBytes(arrIP);
				textBoxIP1.Text = textBoxIP2.Text = textBoxIP3.Text = textBoxIP4.Text = "";
			}
			else
			{
				m_ignoreChange = true;

				CCTV cctv = (CCTV)poi.Facility;

				string strPhoneNumber = "";
				string strCCTVManagerName = GetCCTVManagerName(cctv, ref strPhoneNumber);

				labelCCTVID.Text = "CCTV_" + cctv.ID.ToString();
				labelType.Text = "CCTV";
				textBoxManager.Text = strCCTVManagerName;
				textBoxPhoneNumber.Text = strPhoneNumber;
				textBoxPortNo.Text = cctv.PortNo.ToString();
				textBoxCameraName.Text = cctv.AccessKey;
				cboLOD.SelectedIndex = (int)cctv.LODType;

                m_szUserName = cctv.UserName;
                m_szPassword = cctv.Password;
                m_szChannel = cctv.Channel.ToString();
                m_szStream = cctv.Stream.ToString();

                CCTVTypes.SelectedIndex = cctv.CCTVType;

				//ipAddressControl1.SetAddressBytes(cctv.IPBytes);
				textBoxIP1.Text = ((int)cctv.IPBytes[0]).ToString();
				textBoxIP2.Text = ((int)cctv.IPBytes[1]).ToString();
				textBoxIP3.Text = ((int)cctv.IPBytes[2]).ToString();
				textBoxIP4.Text = ((int)cctv.IPBytes[3]).ToString();

				m_ignoreChange = false;
			}
		}

		// Return 값 : 첫번째 Manager 이름
		// strPhoneNumber : 첫번째 Manager 전화번호
		private string GetCCTVManagerName(CCTV cctv, ref string strPhoneNumber)
		{
			FacilityManagerGroup group = null;

			if (cctv.POI.Zone != null)
			{
				if (cctv.POI.Zone.Building != null)
                    group = FormMain.Instance.DataManager.GetBuildingFacilityManagerGroup(IFacility.FacilityType.CCTV, cctv.POI.Zone.Building, true);
				else
                    group = FormMain.Instance.DataManager.GetOutdoorFacilityManagerGroup(IFacility.FacilityType.CCTV, cctv.POI.Zone, true);
			}

			if (group == null || group.IsEmpty())
                group = FormMain.Instance.DataManager.GetEntireFacilityManagerGroup(IFacility.FacilityType.CCTV, true);

			return FormMain.Instance.DataManager.GetFacilityManagerName(group, ref strPhoneNumber);
		}

		// group의 첫번째 담당자를 리턴한다.
		private FacilityManager GetCCTVManager(FacilityManagerGroup group)
		{
			if (group == null)
				return null;

			if (group.RegularTeams != null && group.RegularTeams.Count > 0)
				return (FacilityManager)group.RegularTeams[0];

			if (group.CompanyMembers != null && group.CompanyMembers.Count > 0)
				return (FacilityManager)group.CompanyMembers[0];

			if (group.ExternalTeams != null && group.ExternalTeams.Count > 0)
				return (FacilityManager)group.ExternalTeams[0];

			if (group.ExternalCompanyMembers != null && group.ExternalTeams.Count > 0)
				return (FacilityManager)group.ExternalCompanyMembers[0];

			return null;
		}

		private void textBoxIP_TextChanged(object sender, EventArgs e)
		{
			if (m_ignoreChange)
				return;

			TextBox textBox = (TextBox)sender;
			int nIndex = -1;

			if (textBox == textBoxIP1)
				nIndex = 0;
			else if (textBox == textBoxIP2)
				nIndex = 1;
			else if (textBox == textBoxIP3)
				nIndex = 2;
			else if (textBox == textBoxIP4)
				nIndex = 3;
			else
				return;

			string strIP = textBox.Text;

			if (strIP == "")
			{
				m_strPrevIP[nIndex] = "";
				OnChangedIP(nIndex);
				return;
			}
			else
			{
				try
				{
					int nIP = int.Parse(strIP);

					if (nIP < 0 || nIP > 255)
					{
						textBox.Text = m_strPrevIP[nIndex];
						return;
					}
				}
				catch (Exception)
				{
					textBox.Text = m_strPrevIP[nIndex];
				}

				m_strPrevIP[nIndex] = textBox.Text;
				OnChangedIP(nIndex);
			}
		}

		private void OnChangedIP(int nIndex)
		{
			if (m_ignoreChange)
				return;

			if (this.m_poi != null && this.m_poi.Facility != null)
			{
				CCTV cctv = (CCTV)this.m_poi.Facility;
				EditCCTV editCCTV = new EditCCTV(cctv);

				string strIPAddr = textBoxIP1.Text + "." + textBoxIP2.Text + "." + textBoxIP3.Text + "." + textBoxIP4.Text;
				editCCTV.IPAddr = strIPAddr;
				editCCTV.AddToManager(FormMain.Instance.PageHome);

				byte[] arrIP = cctv.IPBytes;
				if (byte.TryParse(m_strPrevIP[nIndex], out arrIP[nIndex]))
					cctv.IPBytes = arrIP;
			}
		}

		private void OnChangedPort()
		{
			if (this.m_poi != null && this.m_poi.Facility != null)
			{
				CCTV cctv = (CCTV)this.m_poi.Facility;
				EditCCTV editCCTV = new EditCCTV(cctv);

				short nPort;
				if (short.TryParse(textBoxPortNo.Text, out nPort))
					editCCTV.Port = nPort;
				else
					editCCTV.Port = -1;

				editCCTV.AddToManager(FormMain.Instance.PageHome);
				cctv.PortNo = nPort;
			}
		}

		/*private void btnSave_Click(object sender, EventArgs e)
		{
			if (m_poi == null)
				return;

			if (m_poi.Facility == null || m_poi.Type != Facility.FacilityType.CCTV)
				return;

			string strIP = "";
			if (!ValidCheckIP(ref strIP))
				return;

			short nPort = -1;
			if (!ValidPort(ref nPort))
				return;

			CCTV cctv = (CCTV)m_poi.Facility;
			cctv.IPAddress = strIP;
			cctv.PortNo = nPort;

			if (cctv.ID < 0)
			{
				if (InsertCCTV(cctv) == false)
				{
					// remove poi
				}
			}
			else
				UpdateCCTV(cctv);
		}*/

        //private void UpdateCCTV(CCTV cctv)
        //{
        //    DBUtility.WebDBManager dbMgr = FormMain.Instance.DBManager;

        //    int nZoneID = -1;
        //    string strPositionName = "NULL";
        //    POI poi = cctv.POI;

        //    if (poi.Zone != null)
        //    {
        //        strPositionName = "'" + poi.Zone.BroadcastName + "'";
        //        nZoneID = poi.Zone.ID;
        //    }

        //    string strSQL = string.Format("Update CCTV set IPAddr = '{0}', Port = {1}, PositionName = {2}, X = {3}, Y = {4}, Z = {5}, ZoneID = {6} where ID = {7}",
        //        cctv.IPAddress, cctv.PortNo, strPositionName, poi.X, poi.Y, poi.Z, nZoneID, cctv.ID);

        //    dbMgr.GetResultData(strSQL, 0);
        //}

        //private bool InsertCCTV(CCTV cctv)
        //{
        //    DBUtility.WebDBManager dbMgr = FormMain.Instance.DBManager;

        //    string strSQL = "Select max(id) from CCTV";
        //    ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

        //    if (arrResult == null || arrResult.Count == 0)
        //        return false;

        //    int nMaxID = DBUtility.WebDBManager.GetIntField(arrResult[0].ToString(), 0);

        //    int nZoneID = -1;
        //    string strPositionName = "NULL";
        //    POI poi = cctv.POI;

        //    if (poi.Zone != null)
        //    {
        //        strPositionName = "'" + poi.Zone.BroadcastName + "'";
        //        nZoneID = poi.Zone.ID;
        //    }
        //    else
        //    {
        //        return false;
        //    }

        //    strSQL = string.Format("Insert into CCTV (ID, CameraName, IPAddr, Port, PositionName, X, Y, Z, ZoneID, IsIndoor, Description) values ({0}, 'BNC-3220HR-W', '{1}', {2}, {3}, {4}, {5}, {6}, {7},{8}, NULL)",
        //        ++nMaxID, cctv.IPAddress, cctv.PortNo, strPositionName, poi.X, poi.Y, poi.Z, nZoneID, (poi.Zone.IsOutdoor == true ? 0 : 1));

        //    if (dbMgr.GetResultData(strSQL, 0) == null)
        //        return false;

        //    cctv.ID = nMaxID;
        //    return true;
        //}

		private bool ValidPort(ref short nPort)
		{
			if (textBoxPortNo.Text.Length == 0)
				return false;

			try
			{
				nPort = short.Parse(textBoxPortNo.Text);

				if (nPort <= 64 || nPort > 32767)
					return false;
			}
			catch (Exception)
			{
				return false;
			}

			return true;
		}

		private bool ValidCheckIP(ref string strIP)
		{
			if (textBoxIP1.Text.Length == 0 || textBoxIP2.Text.Length == 0 ||
				textBoxIP3.Text.Length == 0 || textBoxIP4.Text.Length == 0)
				return false;

			try
			{
				int n1 = int.Parse(textBoxIP1.Text);
				int n2 = int.Parse(textBoxIP2.Text);
				int n3 = int.Parse(textBoxIP3.Text);
				int n4 = int.Parse(textBoxIP4.Text);

				if (n1 < 0 || n1 > 255)
					return false;
				if (n2 < 0 || n2 > 255)
					return false;
				if (n3 < 0 || n3 > 255)
					return false;
				if (n4 < 0 || n4 > 255)
					return false;

				strIP = string.Format("{0}.{1}.{2}.{3}", n1, n2, n3, n4);
			}
			catch (Exception)
			{
				return false;
			}

			return true;
		}

		private void textBoxPortNo_TextChanged(object sender, EventArgs e)
		{
			if (m_ignoreChange)
				return;

			if (m_strPrevPortNo == textBoxPortNo.Text)
				return;

			short nPortNo = 0;

			try
			{
				nPortNo = short.Parse(textBoxPortNo.Text);
			}
			catch (Exception)
			{
			}

			m_strPrevPortNo = textBoxPortNo.Text;
			OnChangedPort();
		}

		private void textBoxCameraName_TextChanged(object sender, EventArgs e)
		{
			if (m_ignoreChange)
				return;

			if (m_strPrevCameraName == textBoxCameraName.Text)
				return;

			m_strPrevCameraName = textBoxCameraName.Text;
			OnChangedCameraName();
		}

		private void OnChangedCameraName()
		{
			if (this.m_poi != null && this.m_poi.Facility != null)
			{
				CCTV cctv = (CCTV)this.m_poi.Facility;
				EditCCTV editCCTV = new EditCCTV(cctv);

				editCCTV.CameraName = textBoxCameraName.Text;

				editCCTV.AddToManager(FormMain.Instance.PageHome);
				cctv.AccessKey = textBoxCameraName.Text;
			}
		}

		private void ShowLODDescription(bool show)
		{
            //labelLOD1.Visible = show;
            //labelLOD2.Visible = show;
            //labelLOD3.Visible = show;
            //labelLOD4.Visible = show;
            //labelLOD5.Visible = show;
            //labelLOD6.Visible = show;
            //labelLOD7.Visible = show;
            //labelLOD8.Visible = show;
		}

		private void checkBoxLODDescription_CheckedChanged(object sender, EventArgs e)
		{
            //if (!labelLOD1.Visible)
            //{
            //    ShowLODDescription(true);
            //}
            //else
            //{
            //    ShowLODDescription(false);
            //}
		}

		private void cboLOD_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.m_poi != null && this.m_poi.Facility != null)
			{
				CCTV cctv = (CCTV)this.m_poi.Facility;
				EditCCTV editCCTV = new EditCCTV(cctv);

				editCCTV.LOD = cboLOD.SelectedIndex;

				editCCTV.AddToManager(FormMain.Instance.PageHome);
				cctv.LODType = (CCTV.LOD)(cboLOD.SelectedIndex);
			}
		}

		public void SetTitle(string szText)
		{
			this.lbTitle.Text = szText;
			this.Text = szText;
		}

        private void Password_TextChanged(object sender, EventArgs e)
        {
            if (m_ignoreChange)
                return;
            if (m_szPassword == Password.Text)
                return;
            m_szPassword = Password.Text;
            OnChangePassword();   
        }

        private void OnChangePassword()
        {
            if (this.m_poi != null && this.m_poi.Facility != null)
            {
                CCTV cctv = (CCTV)this.m_poi.Facility;
                EditCCTV editCCTV = new EditCCTV(cctv);
                editCCTV.Password = m_szPassword;
                editCCTV.AddToManager(FormMain.Instance.PageHome);
                cctv.Password = m_szPassword;
            }
        } 

        private void UserName_TextChanged(object sender, EventArgs e)
        {
            if (m_ignoreChange)
                return;
            if (m_szUserName == UserName.Text)
                return;
            m_szUserName = UserName.Text;
            OnChangeUserName();   
        }

        private void OnChangeUserName()
        {
            if (this.m_poi != null && this.m_poi.Facility != null)
            {
                CCTV cctv = (CCTV)this.m_poi.Facility;
                EditCCTV editCCTV = new EditCCTV(cctv);
                editCCTV.UserName = m_szUserName;
                editCCTV.AddToManager(FormMain.Instance.PageHome);
                cctv.UserName = m_szUserName;
            }
        }  

        private void CCTVTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.m_poi != null && this.m_poi.Facility != null)
            {
                CCTV cctv = (CCTV)this.m_poi.Facility;
                EditCCTV editCCTV = new EditCCTV(cctv);

                editCCTV.CCTVType = CCTVTypes.SelectedIndex;

                if (!m_ignoreChange)
                    editCCTV.AddToManager(FormMain.Instance.PageHome);

                cctv.CCTVType = CCTVTypes.SelectedIndex;
            }
        }

        private void Channel_TextChanged(object sender, EventArgs e)
        {
            if (m_ignoreChange)
                return;
            if (m_szChannel == Channel.Text)
                return;
            short nPortNo = 0;
            try
            {
                if( short.TryParse(Channel.Text, out nPortNo))
                {
                    m_szChannel = Channel.Text;
                    OnChangeChannel(nPortNo);
                }               
            }
            catch (Exception)
            {
            }            
        }

        private void OnChangeChannel(int nChannel)
        {
            if (this.m_poi != null && this.m_poi.Facility != null)
            {
                CCTV cctv = (CCTV)this.m_poi.Facility;
                EditCCTV editCCTV = new EditCCTV(cctv);

                editCCTV.Channel = nChannel;
                editCCTV.AddToManager(FormMain.Instance.PageHome);
                cctv.Channel = nChannel;
            }
        }

        private void Stream_TextChanged(object sender, EventArgs e)
        {
            if (m_ignoreChange)
                return;
            if (m_szStream == Stream.Text)
                return;
            short nPortNo = 0;
            try
            {
                if (short.TryParse(Stream.Text, out nPortNo))
                {
                    m_szStream = Stream.Text;
                    OnChangeStream(nPortNo);
                }
            }
            catch (Exception)
            {
            }
        }
        private void OnChangeStream(int nStream)
        {
            if (this.m_poi != null && this.m_poi.Facility != null)
            {
                CCTV cctv = (CCTV)this.m_poi.Facility;
                EditCCTV editCCTV = new EditCCTV(cctv);

                editCCTV.Stream = nStream;
                editCCTV.AddToManager(FormMain.Instance.PageHome);
                cctv.Stream = nStream;
            }
        }      
	}
}