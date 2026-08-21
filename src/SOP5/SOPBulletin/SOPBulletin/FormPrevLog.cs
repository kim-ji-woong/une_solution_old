using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;
using DBUtility;

namespace SOPBulletin
{
    public partial class FormPrevLog : Form
    {
        public enum Option { LAST_10 = 0, LAST_100, LAST_24H, LAST_3DAY, LAST_7DAY, LAST_2WEEK, LAST_1MONTH, LAST_3MONTH, LAST_6MONTH, LAST_1YEAR };

        private ActionStepHistory m_actionStepHistory = null;

        public ActionStepHistory SelectedActionStepHistory
        {
            get { return m_actionStepHistory; }
        }
        
        public FormPrevLog()
        {
            InitializeComponent();
        }

        private void FormPrevLog_Load(object sender, EventArgs e)
        {
            cboOptions.SelectedIndex = 0;
        }

        private void cboOptions_SelectedIndexChanged(object sender, EventArgs e)
        {
            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            if (cboOptions.SelectedIndex < 0)
                return;

            Option opt = (Option)cboOptions.SelectedIndex;

            if (opt == Option.LAST_10)
                LoadLastData(10);
            else if (opt == Option.LAST_100)
                LoadLastData(100);
            else
                LoadLastTime(opt);
        }

        private DateTime GetLastTime(Option opt)
        {
            if (opt == Option.LAST_24H)
                return DateTime.Now.AddHours(-24.0);
            else if (opt == Option.LAST_3DAY)
                return DateTime.Now.AddDays(-3.0);
            else if (opt == Option.LAST_7DAY)
                return DateTime.Now.AddDays(-7.0);
            else if (opt == Option.LAST_2WEEK)
                return DateTime.Now.AddDays(-14.0);
            else if (opt == Option.LAST_1MONTH)
                return DateTime.Now.AddMonths(-1);
            else if (opt == Option.LAST_3MONTH)
                return DateTime.Now.AddMonths(-3);
            else if (opt == Option.LAST_6MONTH)
                return DateTime.Now.AddMonths(-6);
            //else if (opt == Option.LAST_1YEAR)
                return DateTime.Now.AddYears(-1);
        }

        private Dictionary<int, Data_SOPGenUser> GetSOPGenUserInfo()
        {
            string strSQL = "Select ID, MemberID, UserID, NickName from SOPGenUser where SiteID = " + FormMain2.Instance.SiteID.ToString();
            ArrayList arrResult = FormMain2.Instance.DBManager.GetResultData(strSQL, 0);

            if (arrResult == null)
                return null;

            Dictionary<int, Data_SOPGenUser> dicGenUsers = new Dictionary<int, Data_SOPGenUser>();
            Dictionary<int, Data_SOPGenUser> dicGenUsers2 = new Dictionary<int, Data_SOPGenUser>();

            int nResultCount = arrResult.Count;
            string strIDs = "";

            for (int i=0;i<nResultCount-3;i+=4)
            {
                VariousData<int> nID = WebDBManager.GetIntField(arrResult[i].ToString());
                VariousData<int> nMemberID = WebDBManager.GetIntField(arrResult[i + 1].ToString());
                string strUserID = WebDBManager.GetStringField(arrResult[i + 2]);
                string strNickName = WebDBManager.GetStringField(arrResult[i + 3]);

                if (nID == null || strUserID == null)
                    continue;

                Data_SOPGenUser user = new Data_SOPGenUser();

                user.ID = nID.Data;
                user.UserID = strUserID;
                user.NickName = strNickName;

                if (strNickName == null && nMemberID != null)
                {
                    if (strIDs.Length == 0)
                        strIDs = nMemberID.Data.ToString();
                    else
                        strIDs += ", " + nMemberID.Data.ToString();

                    user.MemberID = nMemberID.Data;
                    dicGenUsers2[user.MemberID] = user;
                }

                dicGenUsers[user.ID] = user;
            }

            if (strIDs.Length == 0)
                return dicGenUsers;

            strSQL = "Select ID, MemberName from CompanyMember where ID in (" + strIDs + ")";
            arrResult = FormMain2.Instance.DBManager.GetResultData(strSQL, 0);

            for (int i=0;i<nResultCount-1;i+=2)
            {
                VariousData<int> nID = WebDBManager.GetIntField(arrResult[i].ToString());
                string strMemberName = WebDBManager.GetStringField(arrResult[i + 1]);

                if (nID == null || strMemberName == null)
                    continue;

                Data_SOPGenUser user;

                if (dicGenUsers2.TryGetValue(nID.Data, out user))
                {
                    user.UserName = strMemberName;
                }
            }

            dicGenUsers2.Clear();
            return dicGenUsers;
        }

        private void LoadData(string strSQL, Dictionary<int, Data_SOPGenUser> dicGenUsers, int nLimit = 0)
        {
            ArrayList arrResult = nLimit > 0 ? FormMain2.Instance.DBManager.GetResultData(strSQL, 0, nLimit) : FormMain2.Instance.DBManager.GetResultData(strSQL, 0);

            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 13; i += 14)
            {
                VariousData<int> nID = WebDBManager.GetIntField(arrResult[i].ToString());
                VariousData<int> nActionStepID = WebDBManager.GetIntField(arrResult[i + 1].ToString());
                VariousData<int> nRealMode = WebDBManager.GetIntField(arrResult[i + 2].ToString());
                VariousData<DateTime> beginTime = WebDBManager.GetDateTimeField(arrResult[i + 3]);
                VariousData<DateTime> detectTime = WebDBManager.GetDateTimeField(arrResult[i + 4]);
                string strPosition = WebDBManager.GetStringField(arrResult[i + 5]);
                VariousData<int> nAccessedUserID = WebDBManager.GetIntField(arrResult[i + 6].ToString());
                VariousData<int> nNormal = WebDBManager.GetIntField(arrResult[i + 7].ToString());
                VariousData<DateTime> endTime = WebDBManager.GetDateTimeField(arrResult[i + 8]);
                VariousData<DateTime> cancelTime = WebDBManager.GetDateTimeField(arrResult[i + 9]);
                string strCategoryName = WebDBManager.GetStringField(arrResult[i + 10]);
                string strSubCategoryName = WebDBManager.GetStringField(arrResult[i + 11]);
                string strDisasterName = WebDBManager.GetStringField(arrResult[i + 12]);
                string strActionStepName = WebDBManager.GetStringField(arrResult[i + 13]);

                if (nID == null || nActionStepID == null || nRealMode == null || beginTime == null || nNormal == null || nAccessedUserID == null ||
                    strCategoryName == null || strSubCategoryName == null || strDisasterName == null || strActionStepName == null)
                    continue;

                if (endTime == null && cancelTime == null)
                    continue;

                Data_SOPGenUser user;

                if (!dicGenUsers.TryGetValue(nAccessedUserID.Data, out user))
                    continue;

                string strCommanderName = "";

                if (user.NickName != null && user.NickName != "")
                    strCommanderName = user.NickName;
                else if (user.UserName != null && user.UserName != "")
                    strCommanderName = user.UserName;

                if (strPosition == null)
                    strPosition = "";

                ActionStepHistory actionStepHistory = new ActionStepHistory();

                actionStepHistory.ActionStepHistoryID = nID.Data;
                actionStepHistory.ActionStepID = nActionStepID.Data;
                actionStepHistory.ActionStepPath = strCategoryName + "/" + strSubCategoryName + "/" + strDisasterName + "/" + strActionStepName;
                actionStepHistory.BeginTime = new TimeInfo(beginTime.Data);

                if (endTime != null)
                    actionStepHistory.EndTime = new TimeInfo(endTime.Data);
                else
                    actionStepHistory.CancelTime = new TimeInfo(cancelTime.Data);

                actionStepHistory.DetectTime = detectTime == null ? null : new TimeInfo(detectTime.Data);
                actionStepHistory.CommanderName = strCommanderName;
                actionStepHistory.Position = strPosition;
                actionStepHistory.RealMode = nRealMode.Data == 1;
                actionStepHistory.IsNormal = nNormal.Data == 1;

                DataGridViewRow row = DockingRealTime.MakeNewRow(dataGridView1);

                row.Cells[0].Value = row.Index + 1;

                if (detectTime == null)
                    row.Cells[1].Value = "";
                else
                {
                    DateTime time = detectTime.Data;
                    row.Cells[1].Value = string.Format("{0}년 {1}월 {2}일 {3}시 {4}분 {5}초", time.Year, time.Month, time.Day, time.Hour, time.Minute, time.Second);
                }

                row.Cells[2].Value = strPosition;
                row.Cells[3].Value = actionStepHistory;
                row.Tag = actionStepHistory;
            }
        }

        private void LoadLastData(int num)
        {
            dataGridView1.Rows.Clear();

            Dictionary<int, Data_SOPGenUser> dicGenUsers = GetSOPGenUserInfo();

            if (dicGenUsers == null)
                return;

            string strSQL = "Select ash.ID, ash.ActionStepID, ash.RealMode, ash.BeginTime, ash.DetectTime, ash.Position, ash.LastAccessedUserID, v.isNormal, ash.EndTime, ash.CancelTime, dc.CategoryName, sdc.SubCategoryName, d.DisasterName, _as.StepName ";
            strSQL += "from ActionStepHistory as ash, ActionStep as _as, Disaster as d, Version as v, DisasterCategory as dc, SubDisasterCategory as sdc ";
            strSQL += "where ash.ActionStepID = _as.ID and _as.DisasterID = d.ID and d.VersionID = v.ID and d.SubDisasterID = sdc.ID and sdc.DisasterID = dc.ID and (ash.EndTime is not NULL or ash.CancelTime is not NULL) order by ash.BeginTime desc";

            LoadData(strSQL, dicGenUsers, num);
        }

        private void LoadLastTime(Option opt)
        {
            dataGridView1.Rows.Clear();

            Dictionary<int, Data_SOPGenUser> dicGenUsers = GetSOPGenUserInfo();

            if (dicGenUsers == null)
                return;

            DateTime time = GetLastTime(opt);
            string strTime = string.Format("'{0}-{1:00}-{2:00} {3:00}:{4:00}:{5:00}'", time.Year, time.Month, time.Day, time.Hour, time.Minute, time.Second);

            string strSQL = "Select ash.ID, ash.ActionStepID, ash.RealMode, ash.BeginTime, ash.DetectTime, ash.Position, ash.LastAccessedUserID, v.isNormal, ash.EndTime, ash.CancelTime, dc.CategoryName, sdc.SubCategoryName, d.DisasterName, _as.StepName ";
            strSQL += "from ActionStepHistory as ash, ActionStep as _as, Disaster as d, Version as v, DisasterCategory as dc, SubDisasterCategory as sdc ";
            strSQL += "where ash.ActionStepID = _as.ID and _as.DisasterID = d.ID and d.VersionID = v.ID and d.SubDisasterID = sdc.ID and sdc.DisasterID = dc.ID and (ash.EndTime is not NULL or ash.CancelTime is not NULL) and detectTime >= " + strTime + " order by ash.BeginTime desc";

            LoadData(strSQL, dicGenUsers);
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedCells.Count == 0)
            {
                MessageBox.Show("SOP 로그를 선택해 주세요");
                return;
            }

            DataGridViewRow row = dataGridView1.SelectedCells[0].OwningRow;

            if (row.Tag == null || (row.Tag is ActionStepHistory) == false)
                return;

            ActionStepHistory actionStepHistory = (ActionStepHistory)row.Tag;
            m_actionStepHistory = actionStepHistory;

            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void dataGridView1_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            btnSelect_Click(null, null);
        }
    }
}
