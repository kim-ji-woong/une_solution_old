using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Collections.Generic;

namespace MessageSend
{
    public partial class FormMain : Form
    {
        string MemberID;
        public WebDBManager m_dbMgr = null;
        private Image m_imgRemove = null;
        private ArrayList arrActionStepID = null;
        
        public System.Collections.ArrayList ActionStepIDs
        {
            get { return arrActionStepID; }
            set { arrActionStepID = value; }
        }

        private ArrayList m_arrActionID = null;

        public FormMain(string[] args)
        {
            InitializeComponent();

            MemberID = args[0];
            InitForm();
        }

        public void InitForm()
        {
            arrActionStepID = new ArrayList();
            m_arrActionID = new ArrayList();

            AddRemoveImage();

            Add_SendGridView();

            m_dbMgr = new WebDBManager(this);

            InitGrid();

            ReceiveGridView.Sort(Time, ListSortDirection.Descending);

            for (int i = 0; i < ReceiveGridView.Rows.Count; i++)
            {
                ReceiveGridView[0, i].Value = i + 1;
            }
        }

        private void InitDB()
        {
            string strSQL = "select SendTime, State, Message, ActionStepID from Message where MemberID = " + MemberID;
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);
            if (arrResult == null) return;

            for (int i = 0; i < arrResult.Count - 3; i = i + 4)
            {
                string Disaster = SearchDisa(arrResult[i + 3].ToString());
                Add_ReceiveGridView(0, arrResult[i].ToString(), Disaster, arrResult[i + 1].ToString());
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
                    //listBox1.Items.Add(s);
                    string[] strDataArray = s.Split(new char[] { '\t' });
                    //MessageBox.Show(strDataArray[0] + strDataArray[1]);

                    Add_ReceiveGridView(int.Parse(strDataArray[0]), strDataArray[1], strDataArray[2], strDataArray[3]);
                    // s에는 컴마(,)로 구분되어 있으므로 s.Split로 잘라서 처리하면 됩니다.
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

        private void Add_SendGridView()
        {
            DataGridViewRow gridRow = new DataGridViewRow();
            DataGridViewCell cell = new DataGridViewComboBoxCell();
            cell.Value = "";
            gridRow.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Value = "";
            gridRow.Cells.Add(cell);

            SendGridView.Rows.Add(gridRow);

            Send_Disa.Items.Clear();
        }

        private void Add_ReceiveGridView(int Id, string Time, string Disa, string Act)
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

                int nIdx = Send_Disa.Items.IndexOf(disaster);
                if (nIdx < 0)
                    return;

                Add_ReceiveGridView(0, dtNow.ToString("yyyy-MM-dd HH:mm:ss"),disaster , szMessage);

                ActionStep step = (ActionStep)m_arrActionID[nIdx];
                int nActionStepID = step.ActionStepID;
                int nActionStepHistoryID = step.ActionStepHistoryID;
                
                int a  = ADD_DB(dtNow.ToString("yyyy-MM-dd HH:mm:ss"), 
                                    szMessage, 
                                    int.Parse(MemberID), 
                                    nActionStepID, 
                                    nActionStepHistoryID);
                
                /*if( a == 0 )
                    MessageBox.Show("메세지 전송 완료");*/
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

        public string SearchDisa(string actionStepID)
        {
            string strSQL = "select DisasterID, StepName from ActionStep where ID = " + actionStepID;
            ArrayList arrActionStep = m_dbMgr.GetResultData(strSQL, 0);

            strSQL = "select ID, DisasterName, SubDisasterID from Disaster where ID = " + arrActionStep[0];
            ArrayList arrDisaster = m_dbMgr.GetResultData(strSQL, 0);

            strSQL = "select ID, SubCategoryName, DisasterID from SubDisastercategory where ID = " + arrDisaster[2];
            ArrayList arrSubDisastercategory = m_dbMgr.GetResultData(strSQL, 0);

            strSQL = "select ID, CategoryName from DisasterCategory where ID = " + arrSubDisastercategory[2];
            ArrayList arrDisastercategory = m_dbMgr.GetResultData(strSQL, 0);

            return arrDisastercategory[1].ToString() + "/" + arrSubDisastercategory[1].ToString() + "/" + arrDisaster[1].ToString() + "/" + arrActionStep[1].ToString();
        }

        public int ADD_DB(string SendTime, string message, int MemID, int ActionStepID, int ActionStepHistoryID) // DB에 메세지 전송
        {
            string strSQL = "SELECT ID FROM Message order by 1 desc"; // GenUser의 ID 값
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);
            int GenUser_Count = 1;
            if (arrResult.Count != 0)
                GenUser_Count = int.Parse(arrResult[0].ToString()) + 1;

            strSQL = "INSERT INTO Message(SendTime, Message, MemberID, ActionStepID, ActionStepHistoryID)"
            + " VALUES ('" + SendTime + "', '" + message + "', " + MemID + ", " + ActionStepID + ", " + ActionStepHistoryID + ");"; // ID, SendTime, State, Message, MemberID, ActionStepID

            return m_dbMgr.GetResultData(strSQL, 1) != null ? 0 : 4;
        }       
       
     
        void GetRunningActionStep(bool bRealMode)
        {
            string strSQL = string.Format("select ActionStepID from ActionStepHistory where CancelTime is null and EndTime is null and RealMode = {0}",
                bRealMode ? 1 : 0);
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count == 0)
                return;

            strSQL = "select id, ActionStepID from ActionStepHistory where EndTime is null and CancelTime is null and id in (";

            bool isFirst = true;
            for (int i = 0; i < arrResult.Count; i++)
            {
                int nActionID = m_dbMgr.GetIntField(arrResult[i].ToString(), 0);
                string strSubSQL = string.Format("(select max(id) from ActionStepHistory where BeginTime = (select max(BeginTime) from ActionStepHistory where ActionStepID = '{0}' and RealMode = '{1}'))",
                    nActionID, bRealMode ? 1 : 0);
                if (isFirst)
                    isFirst = false;
                else
                    strSubSQL = ", " + strSubSQL;
                strSQL += strSubSQL;
            }

            if (isFirst)
                return;
            strSQL += ")";

            ArrayList arrResult2 = m_dbMgr.GetResultData(strSQL, 0);
            if (arrResult2 == null || arrResult2.Count == 0)
                return;
            
            ArrayList tempActionStep = new ArrayList();
            for (int n = 0; n < arrResult2.Count - 1; n += 2)
            {
                ActionStep data = new ActionStep();
                data.ActionStepHistoryID = m_dbMgr.GetIntField(arrResult2[n].ToString(), 0);

                int nActionStepID = m_dbMgr.GetIntField(arrResult2[n + 1].ToString(), 0);
                data.RealMode = bRealMode;
                if (nActionStepID > 0)
                {
                    data.ActionStepID = nActionStepID;
                    tempActionStep.Add(data);
                    //tempActionStep.Add(nActionStepID);
                }                
            }

            //////////////////////////////////////////////////////////// ActionStepHistory에서 ActionStepID를 이용하여 DisasterID 가져오기
            strSQL = "select DisasterID from ActionStep where id in ( ";     

            isFirst = true;

            for(int n = 0; n < tempActionStep.Count ; n++ )
            {
                ActionStep step = (ActionStep)tempActionStep[n];
                string strSubSQL = step.ActionStepID.ToString();
                if (isFirst)
                    isFirst = false;
                else
                    strSubSQL = ", " + strSubSQL;
                strSQL += strSubSQL;

            }
            strSQL += " )";
            arrResult = m_dbMgr.GetResultData(strSQL, 1);

            //////////////////////////////////////////////////////////// ActionStep에서 DisasterID를 이용하여  가져오기
            strSQL = "select ID, DisasterName, SubDisasterID, VersionID from Disaster where id in ( ";


            isFirst = true;
            for(int n = 0; n < tempActionStep.Count ; n++ )
            {
                ActionStep step = (ActionStep)tempActionStep[n];
                step.DisasterID =  m_dbMgr.GetIntField(arrResult[n].ToString(), 0);
                string strSubSQL = m_dbMgr.GetStringField(arrResult[n].ToString(), "");
                if (isFirst)
                    isFirst = false;
                else
                    strSubSQL = ", " + strSubSQL;
                strSQL += strSubSQL;
            }
            strSQL += " ) order by VersionID desc";
            arrResult = m_dbMgr.GetResultData(strSQL, 0);

            //////////////////////////////////////////////////////////// 중복된 Disaster 제거
            ArrayList arrDisa = new ArrayList();
            for (int i = 0; i < arrResult.Count - 3; i += 4)
            {
                if (!arrDisa.Contains(arrResult[i+1]))
                {
                    arrDisa.Add(arrResult[i]);
                    arrDisa.Add(arrResult[i + 1]);
                    arrDisa.Add(arrResult[i + 2]);
                    arrDisa.Add(arrResult[i + 3]);
                }
            }

            ArrayList arrDisInfo = new ArrayList();
            for (int i = 0; i < arrDisa.Count - 3; i += 4)
            {   
                DisasterInfo info = new DisasterInfo();
                info.DisasterID = m_dbMgr.GetIntField(arrResult[i].ToString(), 0);
                info.DisasterName = m_dbMgr.GetStringField(arrResult[i].ToString(), "");
                info.SubDisasterID = m_dbMgr.GetIntField(arrResult[i].ToString(), 0);
                info.SubDisasterID = m_dbMgr.GetIntField(arrResult[i].ToString(), 0);
                arrDisInfo.Add(info);
            }
            
            
            for( int j = 0 ; j < arrDisInfo.Count ; j++)
            {   
                DisasterInfo info = (DisasterInfo)arrDisInfo[j];
                for(int i = 0 ; i < tempActionStep.Count; i++)
                {
                    ActionStep step = (ActionStep)tempActionStep[i];
                    if( info.DisasterID == step.DisasterID)
                    {
                        m_arrActionID.Add(step);
                        break;                    
                    }
                }
            } 
        }
     
        private void btnGetSop_Click(object sender, EventArgs e) // 재난명 받아오기 버튼
        {
            m_arrActionID.Clear();
            arrActionStepID.Clear();
            Send_Disa.Items.Clear();

            GetRunningActionStep(true);
            GetRunningActionStep(false);


            for (int i = 0; i < m_arrActionID.Count; i++)
            {
                ActionStep step = (ActionStep)m_arrActionID[i];
                string szReal = step.RealMode ? "실제/" : "훈련/";
                string a = szReal + SearchDisa(step.ActionStepID.ToString());
                Send_Disa.Items.Add(a);
            }

            if (m_arrActionID.Count > 0)
            {
                DataGridViewComboBoxCell cell = (DataGridViewComboBoxCell)SendGridView.Rows[0].Cells[0];
                cell.Value = cell.Items[0];
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
    }

    public class DisasterInfo
    {
        private int m_nDisasterID;
	    public int DisasterID
	    {
		    get { return m_nDisasterID; }
		    set { m_nDisasterID = value; }
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
        private int m_nSubDisasterID;
	    public int SubDisasterID
	    {
		    get { return m_nSubDisasterID; }
		    set { m_nSubDisasterID = value; }
	    }
    }

    public class ActionStep
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
    }
}
