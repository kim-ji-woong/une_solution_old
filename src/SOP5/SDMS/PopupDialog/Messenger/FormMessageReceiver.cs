using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using DBUtility;
using System.Collections;
using System.Reflection;
using UnE.GUI;

namespace SDMS.PopupDialog
{
    public partial class FormMessageReceiver : PopupFormBase
    {
        public class Message
        {
            private int m_nID = -1;
            // 제목
            private string m_strTitle = "";
            // 본문
            private string m_strMessage = "";
            private string m_strRtf = null;
            private int m_nSOPGenUserID = -1;
            private string m_strSenderName = null;
            private DateTime m_dtReceiveTime = new DateTime();

            public int ID
            {
                get { return m_nID; }
                set { m_nID = value; }
            }

            // 제목
            public string Title
            {
                get { return m_strTitle; }
                set { m_strTitle = value; }
            }

            // 본문
            public string Text
            {
                get { return m_strMessage; }
                set { m_strMessage = value; }
            }

            public string RTF
            {
                get { return m_strRtf; }
                set { m_strRtf = value; }
            }

            public int SOPGenUserID
            {
                get { return m_nSOPGenUserID; }
                set { m_nSOPGenUserID = value; }
            }

            public string SenderName
            {
                get { return m_strSenderName; }
                set { m_strSenderName = value; }
            }

            public DateTime Time
            {
                get { return m_dtReceiveTime; }
                set { m_dtReceiveTime = value; }
            }
        }

        private static string m_strDelIniFileName = "", m_strLastIniFileName = "";
        private static int m_nLastReadID = -1;
        // 읽고난 후 삭제한 ID들
        private static List<int> m_deletedIDs = new List<int>();

        private Message m_msgCurrent = null;
        private int m_nBigSpace = 0;
        private int m_nSmallSpace = 77;

        private bool m_closeForm = false;

        public bool CloseForm
        {
            get { return m_closeForm; }
            set
            {
                m_closeForm = value;

                if (value)
                    this.Close();
            }
        }

        public static int LastReadID
        {
            get { return m_nLastReadID; }
        }

        public Message CurrentMessage
        {
            get { return m_msgCurrent; }
        }

        public FormMessageReceiver()
        {
            this.DoubleBuffered = true;

            InitializeComponent();
             
            InitColumns(gridUnread);
            InitSize();

            Type dgvType1 = gridUnread.GetType();
            PropertyInfo pi1 = dgvType1.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            pi1.SetValue(gridUnread, true, null);

            InitCtrlSize(this);
            FormMain.Instance.CustomizeGridView(gridUnread);
        }

        private void InitSize()
        {
            int nTextBoxBottom = rtbBody.Location.Y + rtbBody.Size.Height;
            int nFormHeight = this.Size.Height;
            m_nBigSpace = nFormHeight - nTextBoxBottom;

            HideGrid();
        }

        private void InitColumns(DataGridView grid)
        {
            foreach (DataGridViewColumn column in grid.Columns)
            {
                if (column != colNo && column != colDelete)
                    column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
        }

        private static void CheckIniFile()
        {
            if (m_strDelIniFileName.Length == 0 || m_strLastIniFileName.Length == 0)
            {
                // 굳이 ApplicationData\SOP 폴더에 파일을 저장하는 것은 프로그램을 재설치 하였을때 마지막에 읽었던
                // 메시지 정보를 기억할 수 있도록 하기 위해서다.
                string strFolderName = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\SOP";

                if (!System.IO.Directory.Exists(strFolderName))
                    System.IO.Directory.CreateDirectory(strFolderName);

                string strHead = "http://", strTail = "/SOP", strAdd = "";
                int index1 = FormMain.Instance.DBManager.WebServerURL.IndexOf(strHead);
                int index2 = FormMain.Instance.DBManager.WebServerURL.IndexOf(strTail);

                if (index1 >= 0 && index2 > index1)
                    strAdd = FormMain.Instance.DBManager.WebServerURL.Substring(index1 + strHead.Length, index2 - (index1 + strHead.Length));
                else if (index1 >= 0)
                    strAdd = FormMain.Instance.DBManager.WebServerURL.Substring(index1 + strHead.Length);
                else if (index2 > 0)
                    strAdd = FormMain.Instance.DBManager.WebServerURL.Substring(0, index2);
                else if (index1 < 0 && index2 <= 0)
                    strAdd = FormMain.Instance.DBManager.WebServerURL.Trim();

                // 아래의 문자들은 폴더명에 사용할 수 없다.
                strAdd = strAdd.Replace('\\', '_');
                strAdd = strAdd.Replace('/', '_');
                strAdd = strAdd.Replace(':', '_');
                strAdd = strAdd.Replace('*', '_');
                strAdd = strAdd.Replace('?', '_');
                strAdd = strAdd.Replace('\"', '_');
                strAdd = strAdd.Replace('<', '_');
                strAdd = strAdd.Replace('>', '_');
                strAdd = strAdd.Replace('|', '_');

                if (strAdd.Length > 0)
                {
                    // 서버 URL을 사용하여 폴더 추가
                    strFolderName += "\\" + strAdd;

                    if (!System.IO.Directory.Exists(strFolderName))
                        System.IO.Directory.CreateDirectory(strFolderName);

                    // Site ID를 사용하여 폴더 추가
                    strFolderName += "\\" + UnE.SOP.ProxySOP.Instance.SiteID.ToString();

                    if (!System.IO.Directory.Exists(strFolderName))
                        System.IO.Directory.CreateDirectory(strFolderName);
                }

                m_strDelIniFileName = strFolderName + "\\DeletedMessage.ini";
                m_strLastIniFileName = strFolderName + "\\LastReadMessage.ini";
            }
        }

        private static void ReadLastID()
        {
            CheckIniFile();

            if (File.Exists(m_strDelIniFileName))
            {
                StreamReader reader = new StreamReader(m_strDelIniFileName, Encoding.UTF8);
                string strLine = reader.ReadToEnd();
                reader.Close();

                int nID;
                string[] tokens = strLine.Split(',');

                foreach (string strID in tokens)
                {
                    if (int.TryParse(strID.Trim(), out nID))
                    {
                        m_deletedIDs.Add(nID);
                    }
                }
            }

            if (File.Exists(m_strLastIniFileName))
            {
                StreamReader reader = new StreamReader(m_strLastIniFileName, Encoding.UTF8);
                string strLine = reader.ReadToEnd();
                reader.Close();

                int nID;
                
                if (int.TryParse(strLine.Trim(), out nID))
                {
                    m_nLastReadID = nID;
                }
            }
        }

        private static void AddDeletedID(int nID)
        {
            CheckIniFile();

            StreamWriter writer = new StreamWriter(m_strDelIniFileName, true, Encoding.UTF8);

            if (m_deletedIDs.Count > 0)
                writer.Write("," + nID.ToString());
            else
                writer.Write(nID);
            
            writer.Close();
        }

        private void WriteLastID(int nID)
        {
            CheckIniFile();

            StreamWriter writer = new StreamWriter(m_strLastIniFileName, false, Encoding.UTF8);
            writer.Write(nID);
            writer.Close();

            m_nLastReadID = nID;
        }

        public void AddMessage(int nID, DateTime dtReceiveTime, string strTitle, string strMessage, string strRTF, int nSOPGenUserID, string strSenderName)
        {
            if (IsExist(nID))
                return;

            if (nID > m_nLastReadID)
            {
                m_nLastReadID = nID;
                WriteLastID(nID);
            }

            if (strSenderName == null || strSenderName.Trim().Length == 0)
                strSenderName = GetSenderName(nID);

            Message message = new Message();

            message.ID = nID;
            message.Title = strTitle;
            message.Text = strMessage;
            message.RTF = strRTF;
            message.SOPGenUserID = nSOPGenUserID;
            message.SenderName = strSenderName;
            message.Time = dtReceiveTime;

            // Message가 하나라도 있을때는 Grid를 보이도록 한다.
            AddRow(message);
            // Message가 하나밖에 없을때는 Grid를 안보이게 한다.
            /*if (m_msgCurrent != null)
            {
                if (gridUnread.Rows.Count == 0)
                    AddRow(m_msgCurrent);

                AddRow(message);
            }
            else
                SetMessage(message);*/
        }

        private string GetSenderName(int nID)
        {
            string strSQL = string.Format("Select NickName from SOPGenUser where ID = {0} and SiteID = {1}",
                nID, UnE.SOP.ProxySOP.Instance.SiteID);
            ArrayList arrResult = FormMain.Instance.DBManager.GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count == 0)
                return null;

            string strNickName = WebDBManager.GetStringField(arrResult[0]);
            return strNickName;
        }

        private void AddRow(Message message)
        {
            DataGridViewRow row = new DataGridViewRow();

            DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
            cell.Value = gridUnread.Rows.Count + 1;
            row.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Value = message.Title == null || message.Title.Length == 0 ? message.Text : message.Title;
            row.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Value = GetTimeString(message.Time);
            row.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Value = message.SenderName;
            row.Cells.Add(cell);

            DataGridViewButtonCell cellButton = new DataGridViewButtonCell();
            cellButton.Value = "삭제";
            row.Cells.Add(cellButton);

            gridUnread.Rows.Add(row);
            row.Tag = message;

            if (gridUnread.Rows.Count == 1)
                ShowGrid();
        }

        private string GetTimeString(DateTime time)
        {
            string strTime = string.Format("{0}-{1:00}-{2:00} {3:00}:{4:00}:{5:00}",
                    time.Year, time.Month, time.Day, time.Hour, time.Minute, time.Second);

            return strTime;
        }

        private bool IsExist(int nID)
        {
            if (m_msgCurrent != null && m_msgCurrent.ID == nID)
                return true;

            foreach (DataGridViewRow row in gridUnread.Rows)
            {
                Message message = (Message)row.Tag;

                if (message.ID == nID)
                    return true;
            }

            // 이미 읽은 메시지인가?
            /*if (m_nLastReadID >= nID)
                return true;*/

            /*foreach (DataGridViewRow row in gridUnread.Rows)
            {
                Message message = (Message)row.Tag;

                if (message.ID == nID)
                    return true;
            }*/

            return false;
        }

        private void SetTitle(string strTitle)
        {
            //int nBottom = rtbBody.Location.Y + rtbBody.Size.Height;
            //rtbBody.Anchor = AnchorStyles.Left | AnchorStyles.Right;

            //if (strTitle == null || strTitle.Length == 0)
            //{
            //    labelTitle.Visible = false;
            //    rtbBody.Location = new Point(rtbBody.Location.X, textBoxTitle.Location.Y + 5);
            //}
            //else
            //{
            //    rtbBody.Location = new Point(rtbBody.Location.X, textBoxTitle.Location.Y + textBoxTitle.Size.Height + 4);
            //    textBoxTitle.Text = strTitle;
            //    labelTitle.Visible = textBoxTitle.Visible = true;
            //}

            //rtbBody.Size = new System.Drawing.Size(rtbBody.Size.Width, nBottom - rtbBody.Location.Y);
            //rtbBody.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
        }

        private void SetMessage(Message msg)
        {
            if (msg == null)
            {
                //SetTitle(null);
                rtbBody.Text = "";
            }
            else
            {
                //SetTitle(msg.Title);

                try
                {
                    if (msg.RTF != null)
                        rtbBody.Rtf = msg.RTF;
                    else
                        rtbBody.Text = msg.Text;
                }
                catch (Exception e)
                {
                    System.Diagnostics.Trace.WriteLine(e.Message);
                }
            }

            if (msg == null)
            {
                labelSender.Text = "작성자 : ";
                labelReceiveTime.Text = "작성시간 : ";
                labelTitle.Text = "제목 : ";
            }
            else
            {
                labelSender.Text = "작성자    : " + msg.SenderName;
                labelReceiveTime.Text = "작성시간 : " + GetTimeString(msg.Time);
                labelTitle.Text = "제목 : " + msg.Title;
            }

            m_msgCurrent = msg;
        }

        private void HideGrid()
        {
            rtbBody.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            
            labelUnread.Visible = false;
            gridUnread.Visible = false;
            //this.Size = new Size(this.Size.Width, rtbBody.Location.Y + rtbBody.Size.Height + m_nSmallSpace);

            rtbBody.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;

            labelNoMessage.Location = new Point((int)(this.Width * 0.5 - labelNoMessage.Width * 0.5) , (int)(this.Height * 0.5 - labelNoMessage.Height * 0.5));
        }

        private void ShowGrid()
        {
            rtbBody.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            labelUnread.Visible = true;
            gridUnread.Visible = true;
            //this.Size = new Size(this.Size.Width, rtbBody.Location.Y + rtbBody.Size.Height + m_nBigSpace);

            rtbBody.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;

            labelNoMessage.Location = new Point((this.ClientRectangle.Width - labelNoMessage.Size.Width) / 2, (this.ClientRectangle.Height - labelNoMessage.Size.Height) / 2);
        }

        public static void ReadNewMessage(ref FormMessageReceiver frm, WebDBManager dbMgr, int nMessageID)
        {
            // 이미 읽은 메시지인가?
            if (m_nLastReadID >= nMessageID)
                return;

            string strSQL = "Select ID, SendTime, Title, Text, RichTextFormat, SOPGenUserID, SenderName from SDMSMessage ";
            strSQL += string.Format("where SiteID = {0} and MessageType = {1} and ID > {2}",
                UnE.SOP.ProxySOP.Instance.SiteID, FormMessageSender.SDMS_PUBLIC_MESSAGE_TYPE, m_nLastReadID);

            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return;

            if (frm == null || frm.IsDisposed)
                frm = new FormMessageReceiver();

            bool added = false;
            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 6; i += 7)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                VariousData<DateTime> time = WebDBManager.GetDateTimeField(arrResult[i + 1].ToString());
                string strTitle = WebDBManager.GetStringField(arrResult[i + 2]);
                string strText = WebDBManager.GetStringField(arrResult[i + 3]);
                string strRtf = WebDBManager.GetStringField(arrResult[i + 4]);
                VariousData<int> userID = WebDBManager.GetIntField(arrResult[i + 5].ToString());
                string strSenderName = WebDBManager.GetStringField(arrResult[i + 6]);

                if (id == null || time == null || strText == null || userID == null)
                    continue;

                if (strTitle == null)
                    strTitle = "";

                frm.AddMessage(id.Data, time.Data, strTitle, strText, strRtf, userID.Data, strSenderName);
                added = true;
            }

            if (added)
            {
                frm.SelectLastRow();

                if (frm.Visible)
                    frm.Focus();
                else
                    frm.Show(FormMain.Instance);
            }
        }

        private static string GetIDs(List<int> ids)
        {
            string strIDs = "";

            foreach (int id in ids)
            {
                if (strIDs.Length == 0)
                    strIDs = id.ToString();
                else
                    strIDs += "," + id.ToString();
            }

            return strIDs;
        }

        public static void ReadNewMessage(ref FormMessageReceiver frm, WebDBManager dbMgr)
        {
            if (m_nLastReadID < 0)
                ReadLastID();

            int nLastID = m_nLastReadID;

            string strSQL = "Select ID, SendTime, Title, Text, RichTextFormat, SOPGenUserID, SenderName from SDMSMessage ";
            strSQL += string.Format("where SiteID = {0} and MessageType = {1}",
                UnE.SOP.ProxySOP.Instance.SiteID, FormMessageSender.SDMS_PUBLIC_MESSAGE_TYPE);
            /*string strSQL = "Select ID, SendTime, Title, Text, RichTextFormat, SOPGenUserID, SenderName from SDMSMessage ";
            strSQL += string.Format("where SiteID = {0} and MessageType = {1} and ID > {2}",
                UnE.SOP.ProxySOP.Instance.SiteID, FormMessageSender.SDMS_PUBLIC_MESSAGE_TYPE, m_nLastReadID);*/

            if (m_deletedIDs.Count > 0)
                strSQL += string.Format(" and ID not in ({0})", GetIDs(m_deletedIDs));

            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return;

            if (frm == null || frm.IsDisposed)
                frm = new FormMessageReceiver();

            bool added = false, newMessage = false;
            int nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount-6;i+=7)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                VariousData<DateTime> time = WebDBManager.GetDateTimeField(arrResult[i + 1].ToString());
                string strTitle = WebDBManager.GetStringField(arrResult[i + 2]);
                string strText = WebDBManager.GetStringField(arrResult[i + 3]);
                string strRtf = WebDBManager.GetStringField(arrResult[i + 4]);
                VariousData<int> userID = WebDBManager.GetIntField(arrResult[i + 5].ToString());
                string strSenderName = WebDBManager.GetStringField(arrResult[i + 6]);

                if (id == null || time == null || strText == null || userID == null)
                    continue;

                if (strTitle == null)
                    strTitle = "";

                frm.AddMessage(id.Data, time.Data, strTitle, strText, strRtf, userID.Data, strSenderName);
                added = true;

                if (id.Data > nLastID)
                    newMessage = true;
            }

            if (added)
            {
                frm.SelectLastRow();

                if (newMessage)
                {
                    if (frm.Visible)
                        frm.Focus();
                    else
                        frm.Show(FormMain.Instance);
                }
            }
        }

        public void ReadNewMessages(List<Message> messages)
        {
            foreach (Message message in messages)
            {
                this.AddMessage(message.ID, message.Time, message.Title, message.Text, message.RTF, message.SOPGenUserID, message.SenderName);
            }

            if (messages.Count > 0)
            {
                this.SelectLastRow();

                if (this.Visible)
                    this.Focus();
                else
                    this.Show(FormMain.Instance);
            }
        }

        private void FormMessageReceiver_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!m_closeForm)
            {
                e.Cancel = true;
                btnOK_Click(null, null);
                return;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.Hide();
            //gridUnread.Rows.Clear();
            //HideGrid();
            //m_msgCurrent = null;
        }

        private void gridUnread_SelectionChanged(object sender, EventArgs e)
        {
            if (gridUnread.SelectedCells.Count == 0)
                return;

            DataGridViewRow row = gridUnread.SelectedCells[0].OwningRow;
            Message message = (Message)row.Tag;
            SetMessage(message);
        }

        public void SelectLastRow()
        {
            if (gridUnread.Rows.Count > 0)
            {
                gridUnread.Rows[gridUnread.Rows.Count - 1].Cells[0].Selected = true;
            }
        }

        private void gridUnread_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex == colDelete.Index)
            {
                DeleteRow(gridUnread.Rows[e.RowIndex]);
            }
        }

        private void DeleteRow(DataGridViewRow row)
        {
            if (row == null)
                return;

            if (MessageBox.Show(this, "선택한 메시지를 삭제할까요?\r\n한번 삭제된 메시지는 다시 확인할 수 없습니다.", "확인", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No)
                return;

            Message message = (Message)row.Tag;

            if (message == null)
                return;

            int nDeletedRowIndex = row.Index;

            m_deletedIDs.Add(message.ID);
            AddDeletedID(message.ID);
            gridUnread.Rows.Remove(row);

            int nRowCount = gridUnread.Rows.Count;

            for (int i=nDeletedRowIndex;i<nRowCount;i++)
            {
                DataGridViewRow _row = gridUnread.Rows[i];
                _row.Cells[0].Value = i + 1;
            }

            if (nRowCount == 0)
            {
                m_msgCurrent = null;
                HideGrid();
                CheckCurrentMessage();
            }
        }

        private void CheckCurrentMessage()
        {
            if (m_msgCurrent == null)
            {
                labelNoMessage.Visible = true;
                labelSender.Visible = labelReceiveTime.Visible = labelTitle.Visible = rtbBody.Visible = false;
                labelUnread.Visible = gridUnread.Visible = false;
            }
            else
            {
                labelNoMessage.Visible = false;
                labelSender.Visible = labelReceiveTime.Visible = rtbBody.Visible = true;
                //labelTitle.Visible = m_msgCurrent.Title != null && m_msgCurrent.Title.Length > 0;

                if (gridUnread.Rows.Count == 0)
                    labelUnread.Visible = gridUnread.Visible = false;
                else
                    labelUnread.Visible = gridUnread.Visible = true;
            }
        }

        public new void Show(IWin32Window owner)
        {
            CheckCurrentMessage();
            base.Show(owner);
        }

        public new bool Focus()
        {
            CheckCurrentMessage();
            return base.Focus();
        } 

        // 특정 시간이 지난 SDMSMessage는 서버에서 삭제한다.
        // 그럼에도 불구하고 m_strDelIniFileName의 데이터는 계속 남아있게 되는데, 이 데이터가 늘어날수록
        // SDMSMessage Query시 DB에 부하가 늘어난다.
        // 따라서, m_deletedIDs에 저장된 값들 가운데 DB에서 삭제된 것들은 지우고 m_strDelIniFileName의 내용도 갱신시킨다.
        public static void CheckDeletedIDs(WebDBManager dbMgr)
        {
            if (m_deletedIDs.Count == 0)
                return;

            string strSQL = "Select ID from SDMSMessage where SiteID = " + UnE.SOP.ProxySOP.Instance.SiteID.ToString();
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return;

            List<int> ids = new List<int>(m_deletedIDs);
            int nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount;i++)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());

                if (id == null)
                    continue;

                ids.Remove(id.Data);
            }

            if (ids.Count > 0)
            {
                foreach (int nID in ids)
                {
                    m_deletedIDs.Remove(nID);
                }

                string strIDs = GetIDs(m_deletedIDs);

                CheckIniFile();

                try
                {
                    StreamWriter writer = new StreamWriter(m_strDelIniFileName, false, Encoding.UTF8);
                    writer.Write(strIDs);
                    writer.Close();
                }
                catch (Exception)
                {
                }
            }
        }
         
        private Image imgDelete = SDMS.Properties.Resources.MessageReceiver_Delete_Default;

        private void gridUnread_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            if (e.ColumnIndex == 4)
            {
                e.Paint(e.CellBounds, DataGridViewPaintParts.All);

                e.Graphics.DrawImage(imgDelete, new Rectangle(e.CellBounds.X, e.CellBounds.Y, e.CellBounds.Width, e.CellBounds.Height));
                e.Handled = true;
            }
        }  
    }
}
