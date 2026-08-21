using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using DBUtility2;
using SDMS.Help;
using UnE.GUI;


namespace SDMS
{
    public partial class FormBroadcastConfig : PopupFormBase
	{
		// 화재탐지(0), 화재신고(1), 누출탐지(2), 누출신고(3)
		public enum SituationType { DETECT_FIRE = 0, REPORT_FIRE, SPILL_LEVEL1, SPILL_LEVEL2};

        private int m_nSiteID = 1;
        private SOPManager.PopupSpecialMessage m_frmSpecialMessage = null;

        private ManualManager m_manualManager = null;

		public FormBroadcastConfig()
		{
            m_nSiteID = UnE.SOP.ProxySOP.Instance.SiteID;
            this.DoubleBuffered = true;

			InitializeComponent();

            this.UseFrmMove = false;
            InitCtrlSize(this);
            SetChildCtrlResize(this, this.Width, this.Height);
            SetChildCtrlLocation();

            m_manualManager = new ManualManager(this);
            SetManualID();
		}

		public ArrayList GetBroadcastConfigOption()
		{
			LoadDB();

			ArrayList arr = new ArrayList();
			arr.Add(checkBoxUseBroadcastFireDetect.Checked);
			arr.Add(checkBoxUseBroadcastFireReport.Checked);
			arr.Add(checkBoxUseSiren.Checked);

			//반복횟수
			if (radioNoRepeat.Checked)
				arr.Add(1);
			else if (radioRepeatOnce.Checked)
				arr.Add(2);
			else if (radioRepeatTwice.Checked)
				arr.Add(3);

			arr.Add(richTextBoxFireDetect.Text);
			arr.Add(richTextBoxFireReport.Text);

			return arr;
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

		private void FormBroadcastConfig_Load(object sender, EventArgs e)
		{
			LoadDB();
		}

		private void LoadDB()
		{
			richTextBoxFireDetect.Text = "";
			richTextBoxFireReport.Text = "";
			radioNoRepeat.Checked = true;
			radioRepeatOnce.Checked = radioRepeatTwice.Checked = false;

			//WebDBManager dbMgr = FormMain.Instance.DBManager;

			#region 화재탐지

            LoadDB(SituationType.DETECT_FIRE, checkBoxUseBroadcastFireDetect, richTextBoxFireDetect);
            //string strSQL = "select id, UseBroadcast, Message, UseSiren, RepeatCount from SDMSBroadcastConfig where SituationType = " + ((int)SituationType.DETECT_FIRE).ToString();
            /*string szText = "select id, UseBroadcast, Message, UseSiren, RepeatCount from SDMSBroadcastConfig where SituationType = {0} and SiteID = {1}";
            string strSQL = string.Format(szText, ((int)SituationType.DETECT_FIRE), m_nSiteID);

			ArrayList arrResult = dbMgr.GetResultData(strSQL);
			if (arrResult == null)
				return;

			int nResultCount = arrResult.Count;

			for (int i = 0; i < nResultCount - 4; i += 5)
			{
				int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
				bool useBroadcast = WebDBManager.GetIntField(arrResult[i + 1].ToString(), 0) == 0 ? false : true;
				string strMessage = WebDBManager.GetStringField(arrResult[i + 2], "");
				bool useSiren = WebDBManager.GetIntField(arrResult[i + 3].ToString(), 0) == 0 ? false : true;
				int nRepeatCount = WebDBManager.GetIntField(arrResult[i + 4].ToString(), 0);

				checkBoxUseBroadcastFireDetect.Checked = useBroadcast;
				checkBoxUseSiren.Checked = useSiren;

				if (nRepeatCount == 1)
					radioRepeatOnce.Checked = true;
				else if (nRepeatCount == 2)
					radioRepeatTwice.Checked = true;
				else
					radioNoRepeat.Checked = true;

				richTextBoxFireDetect.Text = strMessage;

				//break;
			}*/

			#endregion 화재탐지

			#region 화재신고
            LoadDB(SituationType.REPORT_FIRE, checkBoxUseBroadcastFireReport, richTextBoxFireReport);
			//strSQL = "select UseBroadcast, Message from SDMSBroadcastConfig where SituationType = " + ((int)SituationType.REPORT_FIRE).ToString();
            /*szText = "select UseBroadcast, Message from SDMSBroadcastConfig where SituationType = {0} and SiteID = {1}";
            strSQL = string.Format(szText, ((int)SituationType.REPORT_FIRE), m_nSiteID);

			arrResult = dbMgr.GetResultData(strSQL);
			if (arrResult == null || arrResult.Count == 0)
			{
				checkBoxUseBroadcastFireReport.Checked = true;
				return;
			}

			nResultCount = arrResult.Count;

			for (int i = 0; i < nResultCount - 1; i += 2)
			{
				bool useBroadcast = WebDBManager.GetIntField(arrResult[i].ToString(), 0) == 0 ? false : true;
				checkBoxUseBroadcastFireReport.Checked = useBroadcast;

				string strMessage = WebDBManager.GetStringField(arrResult[i + 1], "");
				richTextBoxFireReport.Text = strMessage;
			}*/

			#endregion 화재신고


            #region 누출탐지
            LoadDB(SituationType.SPILL_LEVEL1, checkBoxUseBroadcastPSMDetect, richTextBoxPSMDetect);
            /*szText = "select UseBroadcast, Message from SDMSBroadcastConfig where SituationType = {0} and SiteID = {1}";
            strSQL = string.Format(szText, ((int)SituationType.SPILL_LEVEL1), m_nSiteID);

            arrResult = dbMgr.GetResultData(strSQL);
            if (arrResult == null || arrResult.Count == 0)
            {
                checkBoxUseBroadcastPSMDetect.Checked = false;
                //rbNone1.Checked = true;
                return;
            }

            nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 1; i += 2)
            {
                int nBroadcast = WebDBManager.GetIntField(arrResult[i].ToString(), 0);

                if (nBroadcast == 0)
                {
                    checkBoxUseBroadcastPSMDetect.Checked = false;
                }
                else if (nBroadcast == 1)
                {
                    checkBoxUseBroadcastPSMDetect.Checked = true;
                }
                string strMessage = WebDBManager.GetStringField(arrResult[i + 1], "");
                richTextBoxPSMDetect.Text = strMessage;
            }*/

            #endregion 누출탐지

            #region 누출신고
            LoadDB(SituationType.SPILL_LEVEL2, checkBoxUseBroadcastPSMReport, richTextBoxPSMReport);
            /*szText = "select UseBroadcast, Message from SDMSBroadcastConfig where SituationType = {0} and SiteID = {1}";
            strSQL = string.Format(szText, ((int)SituationType.SPILL_LEVEL2), m_nSiteID);

            arrResult = dbMgr.GetResultData(strSQL);
            if (arrResult == null || arrResult.Count == 0)
            {
                checkBoxUseBroadcastPSMReport.Checked = false;
                return;
            }

            nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 1; i += 2)
            {
                int nBroadcast = WebDBManager.GetIntField(arrResult[i].ToString(), 0);

                if (nBroadcast == 0)
                {
                    checkBoxUseBroadcastPSMReport.Checked = false;
                }
                else if (nBroadcast == 1)
                {
                    checkBoxUseBroadcastPSMReport.Checked = true;
                }          

                string strMessage = WebDBManager.GetStringField(arrResult[i + 1], "");
                richTextBoxPSMReport.Text = strMessage;
            }*/

            #endregion 누출신고
        }

        private bool LoadDB(SituationType type, CheckBox checkBoxUseBroadcast, RichTextBox textBox)
        {
            if (textBox.Enabled == false)
                return false;

            checkBoxUseSiren.Checked = false;
            textBox.Text = "";

            string szText = "select id, UseBroadcast, Message, UseSiren, RepeatCount from SDMSBroadcastConfig where SituationType = {0} and SiteID = {1}";
            string strSQL = string.Format(szText, (int)type, m_nSiteID);

            ArrayList arrResult = FormMain.Instance.DBManager.GetResultData(strSQL);
            if (arrResult == null)
                return false;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 4; i += 5)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                bool useBroadcast = WebDBManager.GetIntField(arrResult[i + 1].ToString(), 0) == 0 ? false : true;
                string strMessage = WebDBManager.GetStringField(arrResult[i + 2], "");
                bool useSiren = WebDBManager.GetIntField(arrResult[i + 3].ToString(), 0) == 0 ? false : true;
                int nRepeatCount = WebDBManager.GetIntField(arrResult[i + 4].ToString(), 0);

                checkBoxUseBroadcast.Checked = useBroadcast;
                checkBoxUseSiren.Checked = useSiren;

                if (nRepeatCount == 1)
                    radioRepeatOnce.Checked = true;
                else if (nRepeatCount == 2)
                    radioRepeatTwice.Checked = true;
                else
                    radioNoRepeat.Checked = true;

                textBox.Text = strMessage;
            }

            return true;
        }

        private bool SaveDB(string strMessage, bool useBroadcast, bool useSiren, int nRepeatCount, SituationType type, ref int nMaxID)
        {
            string strSQL = "Select ID from SDMSBroadcastConfig where SituationType = " + ((int)type).ToString();
            ArrayList arrResult = FormMain.Instance.DBManager.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count == 0)
            {
                if (nMaxID < 0)
                    nMaxID = GetMaxID();

                strSQL = string.Format("Insert into SDMSBroadcastConfig (ID, SituationType, UseBroadcast, Message, UseSiren, RepeatCount, Description, SiteID) values ({0}, {1}, {2}, '{3}', {4}, {5}, NULL, {6})",
                    nMaxID + 1, (int)type, useBroadcast ? 1 : 0, strMessage, useSiren ? 1 : 0, nRepeatCount, m_nSiteID);

                if (FormMain.Instance.DBManager.GetResultData(strSQL) != null)
                {
                    nMaxID++;
                    return true;
                }
            }
            else
            {
                strSQL = string.Format("Update SDMSBroadcastConfig set UseBroadcast = {0}, Message = '{1}', UseSiren = {2}, RepeatCount = {3} where SituationType = {4} and SiteID = {5}",
                    useBroadcast ? 1 : 0, strMessage, useSiren ? 1 : 0, nRepeatCount, (int)type, m_nSiteID);

                if (FormMain.Instance.DBManager.GetResultData(strSQL) != null)
                    return true;
            }

            return false;
        }

        private int GetMaxID()
        {
            string strSQL = "Select max(ID) from SDMSBroadcastConfig";
            ArrayList arrResult = FormMain.Instance.DBManager.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count == 0)
                return 0;

            VariousData<int> maxID = WebDBManager.GetIntField(arrResult[0].ToString());
            return maxID == null ? 0 : maxID.Data;
        }

		private void SaveDB()
		{
			string strMessage = richTextBoxFireDetect.Text;
			string strMessage2 = richTextBoxFireReport.Text;
            string strMessage3 = richTextBoxPSMDetect.Text;
            string strMessage4 = richTextBoxPSMReport.Text;

			bool useSiren = checkBoxUseSiren.Checked;
			int nRepeatCount = 0;

			if (radioRepeatOnce.Checked)
				nRepeatCount = 1;
			else if (radioRepeatTwice.Checked)
				nRepeatCount = 2;

			WebDBManager dbMgr = FormMain.Instance.DBManager;
            int nMaxID = -1;

			#region 화재탐지

            SaveDB(strMessage, checkBoxUseBroadcastFireDetect.Checked, useSiren, nRepeatCount, SituationType.DETECT_FIRE, ref nMaxID);
			/*string strSQL = string.Format("Update SDMSBroadcastConfig set UseBroadcast = {0}, Message = '{1}', UseSiren = {2}, RepeatCount = {3} where SituationType = {4} and SiteID = {5}",
				checkBoxUseBroadcastFireDetect.Checked ? 1 : 0, strMessage, useSiren ? 1 : 0, nRepeatCount, (int)SituationType.DETECT_FIRE, m_nSiteID);

			if (dbMgr.GetResultData(strSQL) == null)
			{
				ArrayList arrResult = dbMgr.GetResultData("Select max(id) from SDMSBroadcastConfig");
				if (arrResult == null)
					return;

				int nID = arrResult.Count == 0 ? 1 : WebDBManager.GetIntField(arrResult[0].ToString(), 0) + 1;
				strSQL = string.Format("Insert into SDMSBroadcastConfig (ID, SituationType, UseBroadcast, Message, UseSiren, RepeatCount, Description, SiteID) values ({0}, {1}, {2}, '{3}', {4}, {5}, NULL, {6})",
                    nID, (int)SituationType.DETECT_FIRE, checkBoxUseBroadcastFireDetect.Checked ? 1 : 0, strMessage, useSiren ? 1 : 0, nRepeatCount, m_nSiteID);

				dbMgr.GetResultData(strSQL);
			}*/

			#endregion 화재탐지

			#region 화재신고

            SaveDB(strMessage2, checkBoxUseBroadcastFireReport.Checked, useSiren, nRepeatCount, SituationType.REPORT_FIRE, ref nMaxID);
			/*strSQL = string.Format("Update SDMSBroadcastConfig set UseBroadcast = {0}, Message = '{1}', UseSiren = {2}, RepeatCount = {3} where SituationType = {4} and SiteID = {5}",
				checkBoxUseBroadcastFireReport.Checked ? 1 : 0, strMessage2, useSiren ? 1 : 0, nRepeatCount, (int)SituationType.REPORT_FIRE, m_nSiteID);

			if (dbMgr.GetResultData(strSQL) == null)
			{
				ArrayList arrResult = dbMgr.GetResultData("Select max(id) from SDMSBroadcastConfig");
				if (arrResult == null)
					return;

				int nID = arrResult.Count == 0 ? 1 : WebDBManager.GetIntField(arrResult[0].ToString(), 0) + 1;
				strSQL = string.Format("Insert into SDMSBroadcastConfig (ID, SituationType, UseBroadcast, Message, UseSiren, RepeatCount, Description, SiteID) values ({0}, {1}, {2}, '{3}', {4}, {5}, NULL, {6})",
					nID, (int)SituationType.REPORT_FIRE, checkBoxUseBroadcastFireReport.Checked ? 1 : 0, strMessage2, useSiren ? 1 : 0, nRepeatCount, m_nSiteID);

				dbMgr.GetResultData(strSQL);
			}*/

			#endregion 화재신고

            #region 누출탐지
            if (richTextBoxPSMDetect.Enabled == true)
                SaveDB(strMessage3, checkBoxUseBroadcastPSMDetect.Checked, useSiren, nRepeatCount, SituationType.SPILL_LEVEL1, ref nMaxID);
            /*int nBroadcast = 0;

            if (checkBoxUseBroadcastPSMDetect.Checked == true)
                nBroadcast = 1;

            strSQL = string.Format("Update SDMSBroadcastConfig set UseBroadcast = {0}, Message = '{1}', UseSiren = {2}, RepeatCount = {3} where SituationType = {4} and SiteID = {5}",
                nBroadcast, strMessage3, useSiren ? 1 : 0, nRepeatCount, (int)SituationType.SPILL_LEVEL1, m_nSiteID);

            if (dbMgr.GetResultData(strSQL) == null)
            {
                ArrayList arrResult = dbMgr.GetResultData("Select max(id) from SDMSBroadcastConfig");
                if (arrResult == null)
                    return;

                int nID = arrResult.Count == 0 ? 1 : WebDBManager.GetIntField(arrResult[0].ToString(), 0) + 1;
                strSQL = string.Format("Insert into SDMSBroadcastConfig (ID, SituationType, UseBroadcast, Message, UseSiren, RepeatCount, Description, SiteID) values ({0}, {1}, {2}, '{3}', {4}, {5}, NULL, {6})",
                    nID, (int)SituationType.SPILL_LEVEL1, nBroadcast, strMessage3, useSiren ? 1 : 0, nRepeatCount, m_nSiteID);

                dbMgr.GetResultData(strSQL);
            }*/

            #endregion 누출탐지

            #region 누출신고
            if (richTextBoxPSMReport.Enabled == true)
                SaveDB(strMessage4, checkBoxUseBroadcastPSMReport.Checked, useSiren, nRepeatCount, SituationType.SPILL_LEVEL2, ref nMaxID);
            /*nBroadcast = 0;
            if (checkBoxUseBroadcastPSMReport.Checked == true)
                nBroadcast = 1;

            strSQL = string.Format("Update SDMSBroadcastConfig set UseBroadcast = {0}, Message = '{1}', UseSiren = {2}, RepeatCount = {3} where SituationType = {4} and SiteID = {5}",
                nBroadcast, strMessage4, useSiren ? 1 : 0, nRepeatCount, (int)SituationType.SPILL_LEVEL2, m_nSiteID);

            if (dbMgr.GetResultData(strSQL) == null)
            {
                ArrayList arrResult = dbMgr.GetResultData("Select max(id) from SDMSBroadcastConfig");
                if (arrResult == null)
                    return;

                int nID = arrResult.Count == 0 ? 1 : WebDBManager.GetIntField(arrResult[0].ToString(), 0) + 1;
                strSQL = string.Format("Insert into SDMSBroadcastConfig (ID, SituationType, UseBroadcast, Message, UseSiren, RepeatCount, Description, SiteID) values ({0}, {1}, {2}, '{3}', {4}, {5}, NULL, {6})",
                    nID, (int)SituationType.SPILL_LEVEL2, nBroadcast, strMessage4, useSiren ? 1 : 0, nRepeatCount, m_nSiteID);

                dbMgr.GetResultData(strSQL);
            }*/

            #endregion 누출신고
        }

        private void checkBoxUseSiren_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private UnE.GUI.DialogFormFrameRibbon m_frmForMessage = null;

        private void btnSpecialMessage_Click(object sender, EventArgs e)
        {
            if (((System.Windows.Forms.MouseEventArgs)(e)) != null && ((System.Windows.Forms.MouseEventArgs)(e)).Button != System.Windows.Forms.MouseButtons.Left)
                return;

            if (m_manualManager.IsHelpMode)
                return;

            this.Invoke(new MethodInvoker(delegate()
            {
                if (m_frmForMessage == null || m_frmForMessage.IsDisposed == true)
                { 
                    m_frmSpecialMessage = new SOPManager.PopupSpecialMessage(); 
                    m_frmSpecialMessage.BtnOkNormalImg = global::SDMS.Properties.Resources.__COMMON_Ok;
                    m_frmSpecialMessage.BtnOkClickedImg = global::SDMS.Properties.Resources.__COMMON_OkClick;
                    m_frmSpecialMessage.BtnOkMouseOverImg = global::SDMS.Properties.Resources.__COMMON_OkClick;
                    m_frmSpecialMessage.TopMost = true;

                    foreach (Control ctrl in m_frmSpecialMessage.Controls)
                    {
                        if (ctrl is Panel)
                        {
                            Panel panel = (Panel)ctrl;

                            foreach (Control ctrl2 in panel.Controls)
                            {
                                if (ctrl2 is ComboBox)
                                {
                                    ComboBox cbo = (ComboBox)ctrl2;

                                    // [기후정보] 없애기
                                    if (cbo.Items.Count >= 5)
                                        cbo.Items.RemoveAt(4);

                                    // [SOP 모드] 없애기
                                    if (cbo.Items.Count >= 3)
                                        cbo.Items.RemoveAt(2);

                                    break;
                                }
                            }
                        }
                    }

                    m_frmForMessage = new UnE.GUI.DialogFormFrameRibbon(m_frmSpecialMessage);
                    m_frmForMessage.TitleBarBackColor = Color.FromArgb(0xf7, 0xa9, 0x2b); // 주황색
                    m_frmForMessage.StartPosition = FormStartPosition.WindowsDefaultLocation;
                }

                if (m_frmForMessage.Visible == false)
                    m_frmForMessage.Show(this);
            }));             
        }
         
        public void SetChildCtrlLocation()
        { 
            if (UnE.SOP.ProxySOP.Instance.UsePSM == false)
            {
                label6.Enabled = false;
                checkBoxUseBroadcastPSMDetect.Enabled = false;
                checkBoxUseBroadcastPSMReport.Enabled = false;
                richTextBoxPSMDetect.Enabled = false;
                richTextBoxPSMReport.Enabled = false;

                if (UnE.SOP.ProxySOP.Instance.UseIntrusion)
                {
                    label6.Visible = false;
                    checkBoxUseBroadcastPSMDetect.Visible = false;
                    checkBoxUseBroadcastPSMReport.Visible = false;

                    labelDetectPSM.Visible = richTextBoxPSMDetect.Visible = labelReportPSM.Visible = richTextBoxPSMReport.Visible = false;

                    label7.Visible = true;
                    label7.Location = label6.Location;
                    labelDetectSecurity.Location = labelDetectPSM.Location;
                    richTextBoxSecurityDetect.Location = richTextBoxPSMDetect.Location;
                    labelReportSecurity.Location = labelReportPSM.Location;
                    richTextBoxSecurityReport.Location = richTextBoxPSMReport.Location;

                    checkBoxUseBroadcastSecurityDetect.Location = checkBoxUseBroadcastPSMDetect.Location;
                    checkBoxUseBroadcastSecurityReport.Location = checkBoxUseBroadcastPSMReport.Location;
                    checkBoxUseBroadcastSecurityDetect.Visible = true;
                    checkBoxUseBroadcastSecurityReport.Visible = true;

                    labelDetectSecurity.Visible = richTextBoxSecurityDetect.Visible = labelReportSecurity.Visible = richTextBoxSecurityReport.Visible = true;
                }
            }
        }

        private void SetManualID()
        {
            m_manualManager.Handle = this.Handle;

            m_manualManager.Clear();

            m_manualManager.SetID(this, "SDMS_Manage_Broadcast");
            m_manualManager.SetID(label1, "Broadcast_Option_Fire");
            m_manualManager.SetID(label6, "Broadcast_Option_PSM");
            if (UnE.SOP.ProxySOP.Instance.UseIntrusion)
            {
                m_manualManager.SetID(label7, "Broadcast_Option_Security");
            }

            m_manualManager.SetID(label8, "Broadcast_Option_Play");
            m_manualManager.SetID(btnSpecialMessage, "Broadcast_Option_EditMessage_Special");

            m_manualManager.SetID(label4, "Broadcast_Option_EditMessage");
            m_manualManager.SetID(label5, "Broadcast_Option_EditMessage");
            m_manualManager.SetID(labelDetectPSM, "Broadcast_Option_EditMessage");
            m_manualManager.SetID(labelDetectSecurity, "Broadcast_Option_EditMessage");
            m_manualManager.SetID(labelReportPSM, "Broadcast_Option_EditMessage");
            m_manualManager.SetID(labelReportSecurity, "Broadcast_Option_EditMessage");
            
            m_manualManager.ProcessEvent(); 
        } 
	}
}