using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DBUtility;
using System.Collections;

namespace ServerController
{
    public partial class FormMain : Form
    {
        private WebDBManager m_dbMgr = null;
        private int m_nSiteID = 1;

        private Dictionary<int, SDMSService> m_dicServices = new Dictionary<int, SDMSService>();

        public FormMain()
        {
            InitializeComponent();

            ReadSiteID();
            m_dbMgr = new WebDBManager(m_nSiteID);
        }

        private void ReadSiteID()
        {
            Utility ini = new Utility();

            string strSection = "Server Connection Info";
            string strSiteID = ini.getinivalue(strSection, "siteid");

            int.TryParse(strSiteID, out m_nSiteID);
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            LoadServiceList();
            timer1.Start();
        }

        private void LoadServiceList()
        {
            string strSQL = "Select ID, ServiceName, Status, Description from SDMSServiceList where SiteID = " + m_nSiteID.ToString();
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;
            int y = 0, space = 30;

            for (int i = 0; i < nResultCount - 3; i += 4)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                string strServiceName = WebDBManager.GetStringField(arrResult[i + 1]);
                VariousData<int> status = WebDBManager.GetIntField(arrResult[i + 2].ToString());
                string strDescription = WebDBManager.GetStringField(arrResult[i + 3]);

                if (id == null || strServiceName == null || status == null)
                    continue;

                SDMSService service = new SDMSService();
                service.ID = id.Data;
                service.ServiceName = strServiceName;
                service.Description = strDescription == null ? "" : strDescription;
                service.Status = (SDMSService.StatusType)status.Data;

                m_dicServices[service.ID] = service;

                Label labelServiceName = new Label();
                labelServiceName.Text = service.ToString();
                labelServiceName.Location = new Point(labelServiceNameSample.Location.X, labelServiceNameSample.Location.Y + y);
                labelServiceName.AutoSize = true;
                this.Controls.Add(labelServiceName);
                labelServiceName.Visible = true;

                Label labelServiceStatus = new Label();
                SetServiceStatus(labelServiceStatus, status.Data);
                labelServiceStatus.Location = new Point(labelServiceStatusSample.Location.X, labelServiceStatusSample.Location.Y + y);
                labelServiceStatus.AutoSize = true;
                this.Controls.Add(labelServiceStatus);
                labelServiceStatus.Visible = true;

                ComboBox cboStatus = new ComboBox();
                cboStatus.Size = cboStatusSample.Size;

                foreach (string strItem in cboStatusSample.Items)
                {
                    cboStatus.Items.Add(strItem);
                }

                cboStatus.Location = new Point(cboStatusSample.Location.X, cboStatusSample.Location.Y + y);
                cboStatus.DropDownStyle = cboStatusSample.DropDownStyle;
                this.Controls.Add(cboStatus);
                cboStatus.Visible = true;

                Button btnSend = new Button();
                btnSend.Size = btnSendSample.Size;
                btnSend.Location = new Point(btnSendSample.Location.X, btnSendSample.Location.Y + y);
                btnSend.Text = btnSendSample.Text;
                btnSend.Tag = service;
                btnSend.Click += btnSend_Click;
                this.Controls.Add(btnSend);
                btnSend.Visible = true;

                service.StatusControl = labelServiceStatus;
                service.StatusComboBox = cboStatus;
                service.SendButton = btnSend;

                y += space;
            }
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;

            SDMSService service = (SDMSService)btn.Tag;

            if (service == null)
                return;

            if (service.StatusComboBox.SelectedIndex < 0)
            {
                service.StatusComboBox.Focus();
                MessageBox.Show("변경할 서비스 상태를 먼저 선택하세요.");
                return;
            }

            service.TargetStatus = (SDMSService.StatusType)service.StatusComboBox.SelectedIndex;
            btn.Enabled = false;

            DateTime dtNow = DateTime.Now;
            string strTime = string.Format("{0}-{1:00}-{2:00} {3:00}:{4:00}:{5:00}", dtNow.Year, dtNow.Month, dtNow.Day, dtNow.Hour, dtNow.Minute, dtNow.Second);

            string isStop = service.TargetStatus == SDMSService.StatusType.STOP || service.TargetStatus == SDMSService.StatusType.Restart ? "1" : "NULL";
            string isStart = service.TargetStatus == SDMSService.StatusType.RUN || service.TargetStatus == SDMSService.StatusType.Restart ? "1" : "NULL";
            string strServiceName = service.ServiceName;

            string strsQL = "INSERT INTO SDMSCommand(ID, Command, TimeStamp, SearchPath, IsStop, IsStopService, StopName, IsUpdate, UpdateName, IsStart, IsStartService, StartName) ";
            strsQL += string.Format("values ((select isnull(max(id) + 1, 1) from sdmscommand), 3, '{0}', '', {1}, {1}, '{2}', 0, '', {3}, {3}, '{2}')",
                strTime, isStop, strServiceName, isStart);

            m_dbMgr.GetResultData(strsQL, 0);

            // 재실행은 먼저 중지후 실행을 해야하기 때문에 최종적으로 실행상태가 되기전 중지 과정을 거쳤는지 먼저 판단할 필요가 있다.
            // 따라서, 일정시간 동안은 상태검사를 하지 않다가 타겟 시간이 경과하면 그때 실행중 상태인지 검사하게 된다.
            if (service.TargetStatus == SDMSService.StatusType.Restart)
                service.TargetTime = DateTime.Now.AddSeconds(3);
        }

        private void SetServiceStatus(Label label, int nStatus)
        {
            if (nStatus == (int)SDMSService.StatusType.UNKNOWN)
            {
                label.Font = new Font(label.Font, FontStyle.Regular);
                label.ForeColor = Color.Black;
                label.Text = "알수없음";
            }
            else if (nStatus == (int)SDMSService.StatusType.STOP)
            {
                label.Font = new Font(label.Font, FontStyle.Bold);
                label.ForeColor = Color.Red;
                label.Text = "중지";
            }
            else if (nStatus == (int)SDMSService.StatusType.RUN)
            {
                label.Font = new Font(label.Font, FontStyle.Bold);
                label.ForeColor = Color.Green;
                label.Text = "실행중";
            }
        }

        private void OnTimer(object sender, EventArgs e)
        {
            ReadServiceStatus();
        }

        private void ReadServiceStatus()
        {
            string strSQL = "Select ID, Status from SDMSServiceList where SiteID = " + m_nSiteID.ToString();
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 1; i += 2)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                VariousData<int> status = WebDBManager.GetIntField(arrResult[i + 1].ToString());

                if (id == null || status == null)
                    continue;

                SDMSService service = null;

                if (m_dicServices.TryGetValue(id.Data, out service) == false)
                    continue;

                if ((int)service.Status != status.Data)
                {
                    SetServiceStatus(service.StatusControl, status.Data);
                    service.Status = (SDMSService.StatusType)status.Data;
                }

                if (service.TargetStatus != SDMSService.StatusType.UNKNOWN)
                {
                    if (service.TargetStatus == SDMSService.StatusType.Restart)
                    {
                        if (DateTime.Now > service.TargetTime && status.Data == (int)SDMSService.StatusType.RUN)
                        {
                            service.SendButton.Enabled = true;
                            service.TargetStatus = SDMSService.StatusType.UNKNOWN;
                        }
                    }
                    else if ((service.TargetStatus == SDMSService.StatusType.RUN && status.Data == (int)SDMSService.StatusType.RUN) ||
                        (service.TargetStatus == SDMSService.StatusType.STOP && status.Data == (int)SDMSService.StatusType.STOP))
                    {
                        service.SendButton.Enabled = true;
                        service.TargetStatus = SDMSService.StatusType.UNKNOWN;
                    }
                }
            }
        }
    }

    internal class SDMSService
    {
        public enum StatusType { UNKNOWN = -1, STOP = 0, RUN, Restart };

        private int m_nID = 0;
        private string m_strServiceName = "";
        private string m_strDescription = "";
        private StatusType m_status = StatusType.UNKNOWN;
        private StatusType m_statusTarget = StatusType.UNKNOWN;
        private DateTime m_targetTime = new DateTime();

        private Label m_labelStatus = null;
        private ComboBox m_cboStatus = null;
        private Button m_btnSend = null;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public string ServiceName
        {
            get { return m_strServiceName; }
            set { m_strServiceName = value; }
        }

        public string Description
        {
            get { return m_strDescription; }
            set { m_strDescription = value; }
        }

        public StatusType Status
        {
            get { return m_status; }
            set { m_status = value; }
        }

        public StatusType TargetStatus
        {
            get { return m_statusTarget; }
            set { m_statusTarget = value; }
        }

        public DateTime TargetTime
        {
            get { return m_targetTime; }
            set { m_targetTime = value; }
        }

        public Label StatusControl
        {
            get { return m_labelStatus; }
            set { m_labelStatus = value; }
        }

        public ComboBox StatusComboBox
        {
            get { return m_cboStatus; }
            set { m_cboStatus = value; }
        }

        public Button SendButton
        {
            get { return m_btnSend; }
            set { m_btnSend = value; }
        }

        public override string ToString()
        {
            if (m_strDescription.Length > 0)
                return m_strDescription + "(" + m_strServiceName + ")";

            return m_strServiceName;
        }
    }
}
