using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace SDMS
{
    public partial class FormBroadcastConfig : Form
    {
        // 화재탐지(0), 화재신고(1)
        public enum SituationType { DETECT_FIRE = 0, REPORT_FIRE };

        public FormBroadcastConfig()
        {
            InitializeComponent();
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
            richTextBox.Text = "";

            DBUtility.WebDBManager dbMgr = FormMain.Instance.DBManager;

            #region 화재탐지
            string strSQL = "select id, UseBroadcast, Message, UseSiren, RepeatCount from SDMSBroadcastConfig where SituationType = " + ((int)SituationType.DETECT_FIRE).ToString();

            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);
            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 4; i += 5)
            {
                int nID = DBUtility.WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                bool useBroadcast = DBUtility.WebDBManager.GetIntField(arrResult[i + 1].ToString(), 0) == 0 ? false : true;
                string strMessage = DBUtility.WebDBManager.GetStringField(arrResult[i + 2], "");
                bool useSiren = DBUtility.WebDBManager.GetIntField(arrResult[i + 3].ToString(), 0) == 0 ? false : true;
                int nRepeatCount = DBUtility.WebDBManager.GetIntField(arrResult[i + 4].ToString(), 0);

                checkBoxUseBroadcast.Checked = useBroadcast;
                checkBoxUseSiren.Checked = useSiren;

                if (nRepeatCount == 1)
                    radioRepeatOnce.Checked = true;
                else if (nRepeatCount == 2)
                    radioRepeatTwice.Checked = true;
                else
                    radioNoRepeat.Checked = true;

                richTextBox.Text = strMessage;

                //break;
            }
            #endregion

            #region 화재신고
            strSQL = "select UseBroadcast from SDMSBroadcastConfig where SituationType = " + ((int)SituationType.REPORT_FIRE).ToString();

            arrResult = dbMgr.GetResultData(strSQL, 0);
            if (arrResult == null || arrResult.Count == 0)
            {
                checkBoxUseBroadcast2.Checked = true;
                return;
            }

            nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount; i++ )
            {
                bool useBroadcast = DBUtility.WebDBManager.GetIntField(arrResult[i].ToString(), 0) == 0 ? false : true;
                checkBoxUseBroadcast2.Checked = useBroadcast;
            }
            #endregion
        }

        private void SaveDB()
        {
            string strMessage = richTextBox.Text;
            bool useSiren = checkBoxUseSiren.Checked;
            int nRepeatCount = 0;

            if (radioRepeatOnce.Checked)
                nRepeatCount = 1;
            else if (radioRepeatTwice.Checked)
                nRepeatCount = 2;

            DBUtility.WebDBManager dbMgr = FormMain.Instance.DBManager;

            #region 화재탐지
            string strSQL = string.Format("Update SDMSBroadcastConfig set UseBroadcast = {0}, Message = '{1}', UseSiren = {2}, RepeatCount = {3} where SituationType = {4}",
                checkBoxUseBroadcast.Checked ? 1 : 0, strMessage, useSiren ? 1 : 0, nRepeatCount, (int)SituationType.DETECT_FIRE);

            if (dbMgr.GetResultData(strSQL, 0) == null)
            {
                ArrayList arrResult = dbMgr.GetResultData("Select max(id) from SDMSBroadcastConfig", 0);
                if (arrResult == null)
                    return;

                int nID = arrResult.Count == 0 ? 1 : DBUtility.WebDBManager.GetIntField(arrResult[0].ToString(), 0) + 1;
                strSQL = string.Format("Insert into SDMSBroadcastConfig (ID, SituationType, UseBroadcast, Message, UseSiren, RepeatCount, Description) values ({0}, {1}, {2}, '{3}', {4}, {5}, NULL)",
                    nID, (int)SituationType.DETECT_FIRE, checkBoxUseBroadcast.Checked ? 1 : 0, strMessage, useSiren ? 1 : 0, nRepeatCount);

                dbMgr.GetResultData(strSQL, 0);
            }
            #endregion

            #region 화재신고
            strSQL = string.Format("Update SDMSBroadcastConfig set UseBroadcast = {0}, Message = '{1}', UseSiren = {2}, RepeatCount = {3} where SituationType = {4}",
                checkBoxUseBroadcast2.Checked ? 1 : 0, strMessage, useSiren ? 1 : 0, nRepeatCount, (int)SituationType.REPORT_FIRE);

            if (dbMgr.GetResultData(strSQL, 0) == null)
            {
                ArrayList arrResult = dbMgr.GetResultData("Select max(id) from SDMSBroadcastConfig", 0);
                if (arrResult == null)
                    return;

                int nID = arrResult.Count == 0 ? 1 : DBUtility.WebDBManager.GetIntField(arrResult[0].ToString(), 0) + 1;
                strSQL = string.Format("Insert into SDMSBroadcastConfig (ID, SituationType, UseBroadcast, Message, UseSiren, RepeatCount, Description) values ({0}, {1}, {2}, '{3}', {4}, {5}, NULL)",
                    nID, (int)SituationType.REPORT_FIRE, checkBoxUseBroadcast2.Checked ? 1 : 0, strMessage, useSiren ? 1 : 0, nRepeatCount);

                dbMgr.GetResultData(strSQL, 0);
            }
            #endregion
        }
    }
}
