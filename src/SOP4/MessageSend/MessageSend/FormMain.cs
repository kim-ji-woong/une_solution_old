using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Collections.Generic;
using DBUtility;

namespace MessageSend
{
    public partial class FormMain : Form
    {
        private string MemberID;
        public WebDBManager m_dbMgr = null;
        private Image m_imgRemove = null;
        private ArrayList m_arrActionID = null;

        private DataGridViewComboBoxCell m_SelectSOP = null;

        private int m_nSiteID = 1;
        private int m_nMemberID = -1;

        public FormMain(string[] args)
        {
            InitializeComponent();

            MemberID = args[0];
            if (!int.TryParse(MemberID, out m_nMemberID))
                m_nMemberID = -1;

            string szSiteID = args[1];
            if (!int.TryParse(szSiteID, out m_nSiteID))
                m_nSiteID = 1;

            InitForm();
        }

        public void InitForm()
        {
          
            m_arrActionID = new ArrayList();

            AddRemoveImage();

            AddSendGridView();

            m_dbMgr = new WebDBManager(LoadSiteID());

            InitGrid();

            ReceiveGridView.Sort(Time, ListSortDirection.Descending);

            for (int i = 0; i < ReceiveGridView.Rows.Count; i++)
            {
                ReceiveGridView[0, i].Value = i + 1;
            }
        }

        public int LoadSiteID()
        {
            DBUtility.Utility ini = new DBUtility.Utility();
            string strSiteID = ini.getinivalue("Server Connection Info", "siteid");
            //string strSiteID = m_dbMgr.LoadIni("siteid", "Server Connection Info");

            int nSiteID = 1;

            if (strSiteID.Length > 0)
            {
                int.TryParse(strSiteID, out nSiteID);
            }

            return nSiteID;
        }

        private void InitDB()
        {
            //string strSQL = "select SendTime,  Message, ActionStepID from Message where MemberID = " + MemberID;

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT msg.SendTime, msg.Message, ash.RealMode, dc.CategoryName, sdc.SubCategoryName, dis.DisasterName, step.StepName FROM Message as msg ");
            sb.AppendFormat(" INNER JOIN ActionStepHistory as ash ON ash.ID = msg.ActionStepHistoryID AND msg.MemberID = {0} ", MemberID);
            sb.Append(" INNER JOIN ActionStep as step ON step.ID = ash.ActionStepID ");
            sb.Append(" INNER JOIN Disaster as dis ON step.DisasterID = dis.ID ");
            sb.Append(" INNER JOIN SubDisasterCategory as sdc ON dis.SubDisasterID = sdc.ID ");
            sb.AppendFormat(" INNER JOIN DisasterCategory as dc ON dc.ID = sdc.DisasterID AND dc.SiteID = {0}", m_nSiteID);
            sb.Append(" ORDER BY msg.SendTime ASC");

            string strSQL = sb.ToString();

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);
            if (arrResult == null) return;

            for (int i = 0; i < arrResult.Count - 6; i = i + 7)
            {
                string szSendTime = arrResult[i].ToString();
                string szMsg = arrResult[i + 1].ToString();

                int nReal = WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
                bool bRealMode = (nReal == 1 ? true : false);

                string szCategory = WebDBManager.GetStringField(arrResult[i + 3].ToString(), "");
                string szSubCategory = WebDBManager.GetStringField(arrResult[i + 4].ToString(), "");
                string szDisasterName = WebDBManager.GetStringField(arrResult[i + 5].ToString(), "");
                string szStepName = WebDBManager.GetStringField(arrResult[i + 6].ToString(), "");

                string szReal = bRealMode ? "실제/" : "훈련/";
                string Disaster = szReal + szCategory + "/" + szSubCategory + "/" + szDisasterName + "/" + szStepName;

                AddReceiveGridView(0, szSendTime, Disaster, arrResult[i + 1].ToString());
            }
        }

        public void InitGrid()
        {
            try
            {
                StreamReader sr = new StreamReader(MemberID + ".csv", Encoding.GetEncoding("euc-kr"));
                string s = "";
                while (!sr.EndOfStream)
                {
                    s = sr.ReadLine();                   
                    string[] strDataArray = s.Split(new char[] { '\t' });
                    AddReceiveGridView(int.Parse(strDataArray[0]), strDataArray[1], strDataArray[2], strDataArray[3]); 
                }
                sr.Close();
            }
            catch
            {
                InitDB();
                saveCSV();
            }
        }

        private void AddRemoveImage()
        {
            Bitmap bmpRemove = new Bitmap(global::MessageSend.Properties.Resources.report_remove);

            ImageList imgListRemove = new ImageList();
            imgListRemove.ImageSize = new Size(16, 16);
            imgListRemove.Images.AddStrip(bmpRemove);

            m_imgRemove = imgListRemove.Images[0];

            ReceiveGridView.Sort(ID, ListSortDirection.Descending);
        }
        
        private void AddSendGridView()
        {
            DataGridViewRow gridRow = new DataGridViewRow();
            DataGridViewComboBoxCell cell = new DataGridViewComboBoxCell();
            m_SelectSOP = cell;
            cell.Value = "";
            gridRow.Cells.Add(cell);

            DataGridViewTextBoxCell cell2 = new DataGridViewTextBoxCell();
            cell2.Value = "";
            gridRow.Cells.Add(cell2);

            SendGridView.Rows.Add(gridRow); 
        }

        private void AddReceiveGridView(int Id, string Time, string Disa, string Act)
        {
            DataGridViewRow gridRow = new DataGridViewRow();
            DataGridViewCell cell = new DataGridViewTextBoxCell();
            cell.Value = Id.ToString();
            gridRow.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Value = Time;
            gridRow.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Value = Disa;
            gridRow.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Value = Act;
            gridRow.Cells.Add(cell);

            cell = new DataGridViewImageCell();
            cell.Value = m_imgRemove;
            gridRow.Cells.Add(cell);

            ReceiveGridView.Rows.Insert(0, gridRow);
        }

        private void ReceiveGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 4)
            {
                ReceiveGridView.Rows.RemoveAt(e.RowIndex);
            }

            for (int i = 0; i < ReceiveGridView.Rows.Count; i++)
            {
                ReceiveGridView[0, i].Value = i+1;
            }

            saveCSV();
        }
  
        private void btnSend_Click(object sender, EventArgs e)
        {
            DateTime dtNow = DateTime.Now;

            if (SendGridView[1, 0].Value.ToString() == "")
            {
                MessageBox.Show("상황 혹은 조치내용을 입력하여 주십시오.", "알림");
                return;
            }
            else if (SendGridView[0, 0].Value.ToString() == "")
            {
                MessageBox.Show("SOP 재난명을 설정하여 주십시오.");
                return;
            }

            //if (MessageBox.Show("최종발송 하시겠습니까 ?", "최종 확인", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                 // id, Time, Disa, Solu, Act
                string disaster = SendGridView[0, 0].Value.ToString();
                string szMessage = SendGridView[1, 0].Value.ToString();

                int nIdx = m_SelectSOP.Items.IndexOf(disaster);
                if (nIdx < 0)
                    return;

                AddReceiveGridView(0, dtNow.ToString("yyyy-MM-dd HH:mm:ss"),disaster , szMessage);

                ActionStepInfo step = (ActionStepInfo)m_arrActionID[nIdx];
                int nActionStepID = step.ActionStepID;
                int nActionStepHistoryID = step.ActionStepHistoryID;
                
                int a  = AddMessage(dtNow.ToString("yyyy-MM-dd HH:mm:ss"), 
                                    szMessage,
                                    m_nMemberID, 
                                    nActionStepID, 
                                    nActionStepHistoryID);
                
            }

            for (int i = 0; i < ReceiveGridView.Rows.Count; i++)
            {
                ReceiveGridView[0, i].Value = i + 1;
            }

            saveCSV();
        }

        private void btnDelete_Click(object sender, EventArgs e) // 전체삭제
        {
            if (MessageBox.Show("삭제하시겠습니까 ?", "최종 확인", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                ReceiveGridView.Rows.Clear();
            }
            saveCSV();
        }

        public int AddMessage(string SendTime, string message, int MemID, int ActionStepID, int ActionStepHistoryID) // DB에 메세지 전송
        {
            //string strSQL = "INSERT INTO Message(SendTime, Message, MemberID, ActionStepID, ActionStepHistoryID)"
            //+ " VALUES ('" + SendTime + "', '" + message + "', " + MemID + ", " + ActionStepID + ", " + ActionStepHistoryID + ");"; // ID, SendTime, State, Message, MemberID, ActionStepID

            string szText = "INSERT INTO Message(SendTime, Message, MemberID, ActionStepID, ActionStepHistoryID) VALUES ('{0}','{1}',{2},{3},{4})";
            string strSQL = string.Format(szText, SendTime, message, MemID, ActionStepID, ActionStepHistoryID);
            return m_dbMgr.GetResultData(strSQL, 1) != null ? 0 : 4;
        }
        
        public string MakePath(ActionStepInfo info)
        {
            if (info == null)
                return "";
            string szReal = info.RealMode ? "실제/" : "훈련/";
            return szReal + info.Categroy + "/" + info.SubCategroy + "/" + info.DisasterName + "/" + info.StepName;
        }
             
        void GetRunningActionStep()
        {            
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT ash.ID, ash.ActionStepID, ash.RealMode, dis.ID,  dis.VersionID, step.StepName, dis.DisasterName, sdc.SubCategoryName, dc.CategoryName, dis.SubDisasterID, v.isRegular, v.isNormal FROM ActionStepHistory as ash ");
            sb.AppendFormat(" INNER JOIN ActionStep as step ON step.ID = ash.ActionStepID and ash.EndTime is null and CancelTime is null");// and ash.RealMode = {0}", bRealMode ? 1 : 0);
            sb.Append(" INNER JOIN Disaster as dis ON step.DisasterID = dis.ID  ");
            sb.Append(" INNER JOIN Version as v ON dis.VersionID = v.ID  ");
            sb.Append(" INNER JOIN SubDisasterCategory as sdc ON dis.SubDisasterID = sdc.ID ");
            sb.AppendFormat(" INNER JOIN DisasterCategory as dc ON dc.ID = sdc.DisasterID AND dc.SiteID = {0} ", m_nSiteID);
            sb.Append(" INNER JOIN ( SELECT ActionStepID , max(BeginTime)  as maxTime, max(ID) as maxID FROM ActionStepHistory GROUP BY ActionStepID ) ash2 ");
            sb.Append("     ON ash2.ActionStepID = ash.ActionStepID and ash.BeginTime = ash2.maxTime and ash.ID = ash2.maxID ");
            sb.Append(" ORDER BY ash.ID DESC, dis.VersionID DESC");

            string szSQL = sb.ToString();
            ArrayList arrResult = m_dbMgr.GetResultData(szSQL, 0);
            if (arrResult == null || arrResult.Count == 0)
                return;

            for (int i = 0; i < arrResult.Count - 11; i = i + 12)
            {
                int nActionStepHistory = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                int nActionStepID = WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
                int nReal = WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
                int nDisasterID = WebDBManager.GetIntField(arrResult[i + 3].ToString(), -1);
                int nVersion = WebDBManager.GetIntField(arrResult[i + 4].ToString(), -1);

                string szStepName = WebDBManager.GetStringField(arrResult[i + 5].ToString(), "");
                string szDisasterName = WebDBManager.GetStringField(arrResult[i + 6].ToString(), "");
                string szSubCategory = WebDBManager.GetStringField(arrResult[i + 7].ToString(), "");
                string szCategory = WebDBManager.GetStringField(arrResult[i + 8].ToString(), "");
                int nSubDisasterID = WebDBManager.GetIntField(arrResult[i + 9].ToString(), -1);
                bool isRegular = WebDBManager.GetIntField(arrResult[i + 10].ToString(), 0) == 0 ? false : true;
                bool isNormal = WebDBManager.GetIntField(arrResult[i + 11].ToString(), 0) == 0 ? false : true;

                int nMaxVersionID = GetMaxVersionID(szDisasterName, nSubDisasterID, isRegular, isNormal);

                // 가장 최신 버전의 SOP가 아닐경우 로딩하지 않는다.
                if (nVersion != nMaxVersionID)
                    continue;
                                
                ActionStepInfo data = new ActionStepInfo();
                data.ActionStepHistoryID = nActionStepHistory;
                data.ActionStepID = nActionStepID;
                data.DisasterID = nDisasterID;
                data.RealMode = (nReal == 1 ? true : false);
                data.Version = nVersion;

                data.StepName = szStepName;
                data.DisasterName = szDisasterName;
                data.SubCategroy = szSubCategory;
                data.Categroy = szCategory;

                m_arrActionID.Add(data);
            }
        }

        private int GetMaxVersionID(string strDisasterName, int nSubDisasterID, bool isRegular, bool isNormal)
        {
            string strSQL = string.Format("select max(v.ID) from Disaster as d, Version as v where DisasterName = '{0}' and SubDisasterID = {1} and v.ID = d.VersionID and v.isRegular = {2} and v.isNormal = {3}",
                strDisasterName, nSubDisasterID, isRegular ? 1 : 0, isNormal ? 1 : 0);
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count == 0)
                return -1;

            int nMaxID = WebDBManager.GetIntField(arrResult[0].ToString(), -1);
            return nMaxID;
        }
     
        private void btnGetSop_Click(object sender, EventArgs e) // 재난명 받아오기 버튼
        {
            m_arrActionID.Clear();
            
            m_SelectSOP.Items.Clear();
            m_SelectSOP.Value = "";

            GetRunningActionStep();
            //GetRunningActionStep(true);
            //GetRunningActionStep(false);

            foreach (ActionStepInfo step in m_arrActionID)
            {                       
                string szPath = MakePath(step);
                m_SelectSOP.Items.Add(szPath);
            }

            if (m_arrActionID.Count > 0)
            {
                m_SelectSOP.Value = m_SelectSOP.Items[0];
            }
        }

        private void saveCSV()
        {
            StreamWriter sw = new StreamWriter(MemberID+".csv", false, Encoding.Unicode);

            for (int i = 0; i < ReceiveGridView.RowCount; i++)
            {
                sw.WriteLine(ReceiveGridView[0, i].Value + "\t" + ReceiveGridView[1, i].Value + "\t" + ReceiveGridView[2, i].Value + "\t" + ReceiveGridView[3, i].Value);
            }
            sw.Close();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            btnGetSop_Click(null, null);
        }

        private void SendGridView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
                return;

            if (SendGridView.Rows.Count == 0)
                return;

            if (SendGridView.Rows[0].Cells[1].Selected)
            {
                string strValue = (string)SendGridView.Rows[0].Cells[1].Value;
                if (strValue != "")
                    btnSend_Click(null, null);
            }
        }

        private void FormMain_SizeChanged(object sender, EventArgs e)
        {
            Point pt1 = ReceiveGridView.Location;
            Point pt2 = lbMsg.Location;

            int dH = (pt2.Y - pt1.Y) - 5;
            if (dH < 150) // 최소사이즈 (150)
                dH = 150;
            ReceiveGridView.Height = dH;
        }
    }

    public class ActionStepInfo
    {
        private int m_nActionStepID;
        private int m_nActionStepHistoryID;
        private bool m_bRealMode;   
        private int m_nDisasterID;
	    public int DisasterID
	    {
		    get { return m_nDisasterID; }
		    set { m_nDisasterID = value; }
	    }
        public bool RealMode
        {
            get { return m_bRealMode; }
            set { m_bRealMode = value; }
        }
        public int ActionStepID
        {
            get { return m_nActionStepID; }
            set { m_nActionStepID = value; }
        }
        public int ActionStepHistoryID
        {
            get { return m_nActionStepHistoryID; }
            set { m_nActionStepHistoryID = value; }
        }

        private string m_strDisasterName;
        public string DisasterName
        {
            get { return m_strDisasterName; }
            set { m_strDisasterName = value; }
        }

        private int m_nVersion;
        public int Version
        {
            get { return m_nVersion; }
            set { m_nVersion = value; }
        }
       
        private string m_szSubCategroy = "";
        public string SubCategroy
        {
            get { return m_szSubCategroy; }
            set { m_szSubCategroy = value; }
        }

        private string m_szCategroy = "";
        public string Categroy
        {
            get { return m_szCategroy; }
            set { m_szCategroy = value; }
        }

        private string m_szStepName = "";
        public string StepName
        {
            get { return m_szStepName; }
            set { m_szStepName = value; }
        }
    }
}
