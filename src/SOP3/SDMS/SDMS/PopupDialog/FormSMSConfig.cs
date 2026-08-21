using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using Microsoft.Win32;
using System.Reflection;
using System.Security;
using System.Security.AccessControl;

namespace SDMS
{
    public partial class FormSMSConfig : Form
    {
        public enum MessageType { FACILITY_FAULT = 0, DETECT_FIRE };

        public FormSMSConfig()
        {
            InitializeComponent();
        }

		private bool m_bRealMode = true;
        private bool m_bFacilityFault = true;
        
        private void FormSMSConfig_Load(object sender, EventArgs e)
        {
            LoadDB();

			m_bRealMode = PreferenceManager.Instance.RealMode;
			checkBox1.Checked = !m_bRealMode;
            checkBox3.Checked = m_bSendSmsNightDuty;

			m_bRunSimulator = ReadRunSimulator();
			ckbRunSimulator.Checked = m_bRunSimulator;
        }

        private void LoadDB()
        {
            DBUtility.WebDBManager dbMgr = FormMain.Instance.DBManager;
            string strSQL = "Select id, MessageType, UseSMS from SDMSSMSConfig";

            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);
            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 2; i += 3)
            {
                int nID = DBUtility.WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                int nMessageType = DBUtility.WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
                bool useSMS = DBUtility.WebDBManager.GetIntField(arrResult[i + 2].ToString(), 0) == 0 ? false : true;

                if (nMessageType == (int)MessageType.FACILITY_FAULT)
                    checkBoxFacilityFault.Checked = useSMS;
                else if (nMessageType == (int)MessageType.DETECT_FIRE)
                    checkBoxDetectFire.Checked = useSMS;
            }

			
			string szSQL2 = "SELECT PropertyValue FROM OptionSDMS where PropertyName ='TranningMode'";
			ArrayList arResult2 = dbMgr.GetResultData(szSQL2, 0);
			if (arResult2 == null || arResult2.Count == 0)
			{
				m_bRealMode = true;
			}
			else
			{
				int nDur = DBUtility.WebDBManager.GetIntField(arResult2[0].ToString(),0);					
				if (nDur == 1)
					m_bRealMode = false;
				else
					m_bRealMode = true;				
			}
			PreferenceManager.Instance.RealMode = m_bRealMode;

             /*string szSQL3 = "SELECT PropertyValue FROM OptionSDMS where PropertyName='BroadcastOn'";
             ArrayList arResult3 = dbMgr.GetResultData(szSQL3, 0);
            if (arResult3 == null || arResult3.Count == 0)
            {
                m_bEnableBroadcast = false;
            }
            else
            {
                int nTemp = DBUtility.WebDBManager.GetIntField(arResult3[0].ToString(), -1);

                if (nTemp == 1)
                    m_bEnableBroadcast = true;
                else
                    m_bEnableBroadcast = false;
            }

            string szSQL4 = "SELECT PropertyValue FROM OptionSDMS where PropertyName='SendSMSonNightDuty'";
            ArrayList arResult4 = dbMgr.GetResultData(szSQL4, 0);
            if (arResult4 == null || arResult4.Count == 0)
            {
                m_bSendSmsNightDuty = false;
            }
            else
            {
                int nTemp = DBUtility.WebDBManager.GetIntField(arResult4[0].ToString(), -1);

                if (nTemp == 1)
                    m_bSendSmsNightDuty = true;
                else
                    m_bSendSmsNightDuty = false;
            }*/
		}

        private bool m_bSendSmsNightDuty = false;

        private void SaveDB()
        {
            bool useFacilityFault = checkBoxFacilityFault.Checked;
            bool useDetectFire = checkBoxDetectFire.Checked;
            
            Save(useFacilityFault, MessageType.FACILITY_FAULT);
            Save(useDetectFire, MessageType.DETECT_FIRE);

			string szSQL1 = string.Format("UPDATE OptionSDMS SET PropertyValue={0} WHERE PropertyName='TranningMode'", m_bRealMode == true ? 0 : 1);
			FormMain.Instance.DBManager.GetResultData(szSQL1, 0);

            /*string szSQL2 = string.Format("UPDATE OptionSDMS SET PropertyValue={0} WHERE PropertyName='BroadcastOn'", m_bEnableBroadcast == true ? 1 : 0);
            FormMain.Instance.DBManager.GetResultData(szSQL2, 0);

            string szSQL3 = string.Format("UPDATE OptionSDMS SET PropertyValue={0} WHERE PropertyName='SendSMSonNightDuty'", m_bSendSmsNightDuty == true ? 1 : 0);
            FormMain.Instance.DBManager.GetResultData(szSQL3, 0);*/

			WriteRunSimulator(m_bRunSimulator);
        }

        private void Save(bool useSMS, MessageType type)
        {
            DBUtility.WebDBManager dbMgr = FormMain.Instance.DBManager;
            string strSQL = string.Format("Update SDMSSMSConfig set UseSMS = {0} where MessageType = {1}", useSMS ? 1 : 0, (int)type);

            if (dbMgr.GetResultData(strSQL, 0) == null)
            {
                ArrayList arrResult = dbMgr.GetResultData("Select max(id) from SDMSSMSConfig", 0);
                if (arrResult == null)
                    return;

                int nID = arrResult.Count == 0 ? 1 : DBUtility.WebDBManager.GetIntField(arrResult[0].ToString(), 0) + 1;
                strSQL = string.Format("Insert into SDMSSMSConfig (ID, MessageType, UseSMS, Description) values ({0}, {1}, {2}, NULL)",
                    nID, (int)type, useSMS ? 1 : 0);

                dbMgr.GetResultData(strSQL, 0);
            }


			PreferenceManager.Instance.RealMode = m_bRealMode;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            SaveDB();

            DialogResult = System.Windows.Forms.DialogResult.OK;
			
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
            Close();
        }

        private void labelDetect_Click(object sender, EventArgs e)
        {
            checkBoxDetectFire.Checked = !checkBoxDetectFire.Checked;
        }

		private void checkBox1_CheckedChanged(object sender, EventArgs e)
		{
			if (checkBox1.Checked)
			{
				m_bRealMode = false;
			}
			else
			{
				m_bRealMode = true;
			}
		}

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox3.Checked)
            {
                m_bSendSmsNightDuty = true;
            }
            else
            {
                m_bSendSmsNightDuty = false;
            }
        }

		private bool m_bRunSimulator = true;
		private void ckbRunSimulator_CheckedChanged(object sender, EventArgs e)
		{
			bool bChecked = ckbRunSimulator.Checked;
			m_bRunSimulator = bChecked;
		}

		public static bool ReadRunSimulator()
		{
			bool bResult = false;
			try
			{
				
				RegistryKey rkey = Registry.CurrentUser.OpenSubKey(@"SDMS\Simulator");
				if (rkey == null)
				{
					return true;
				}
				else
				{
					int nRun = (int)rkey.GetValue("Run", 1);
					bResult = (nRun == 1 ? true : false);
				}
				if (rkey != null)
					rkey.Close();
			}
			catch (System.Exception)
			{				
			}
			return bResult;
		}

		public static void WriteRunSimulator(bool bCheck)
		{
			try
			{
				string szUserName = Environment.UserDomainName + "\\" + Environment.UserName;

				RegistrySecurity rs = new RegistrySecurity();

				rs.AddAccessRule(new RegistryAccessRule(szUserName,
					RegistryRights.ReadKey | RegistryRights.Delete | RegistryRights.WriteKey,
					InheritanceFlags.None,
					PropagationFlags.None,
					AccessControlType.Allow));

				rs.AddAccessRule(new RegistryAccessRule(szUserName,
					RegistryRights.ChangePermissions,
					InheritanceFlags.None,
					PropagationFlags.None,
					AccessControlType.Deny));

				RegistryKey rkey = Registry.CurrentUser.OpenSubKey(@"SDMS\Simulator", true);
				if (rkey == null)
				{
					try
					{
						rkey = Registry.CurrentUser.CreateSubKey(@"SDMS\Simulator", RegistryKeyPermissionCheck.ReadWriteSubTree, rs);
					}
					catch (Exception)
					{
					}
				}

				if (rkey != null)
				{
					int nValue = (bCheck == true ? 1 : 0);
					rkey.SetValue("Run", nValue);
					rkey.Close();
				}
			}
			catch (System.Exception)
			{				
			}			
		}

        
        private void checkBoxFacilityFault_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxFacilityFault.Checked)
            {
                m_bRealMode = false;
            }
            else
            {
                m_bRealMode = true;
            }
        }
    }
}
