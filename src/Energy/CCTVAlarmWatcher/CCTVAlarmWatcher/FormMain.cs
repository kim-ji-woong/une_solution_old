using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AxednetpluginocxLib;
using System.IO;

namespace CCTVAlarmWatcher
{
    public partial class FormMain : Form
    {
        private const int EDNETP_EVENT_STATE = 0;
        private const int EDNETP_EVENT_ATTR = 1;
        private const int EDNETP_EVENT_LOGIN = 2;
        private const int EDNETP_EVENT_CONNECT = 3;
        private const int EDNETP_EVENT_SHUTDOWN = 4;
        private const int EDNETP_EVENT_HOST_EVENT = 5;
        private const int EDNETP_EVENT_CHANNEL_STATE = 9;

        private List<AxEDNetPluginOCX> m_ocxList = new List<AxEDNetPluginOCX>();
        private Dictionary<AxEDNetPluginOCX, NVR> m_dicOCXNVR = new Dictionary<AxEDNetPluginOCX, NVR>();

        private string m_strTargetFolder = "";
        private string m_strSearchPattern = "";
        private string m_strTargetFile = "";
        private string m_strCurrentFile = "";
        private DateTime m_dtCurrent;

        private UTF8Encoding m_encoding = new UTF8Encoding(true);

        public FormMain(List<NVR> datas, string strTargetFile)
        {
            InitializeComponent(datas);

            m_strTargetFile = strTargetFile;
            SetSearchPattern();

            DateTime dtNow = DateTime.Now;
            RemoveAlarmFile(dtNow.Year, dtNow.Month, dtNow.Day);

            foreach (KeyValuePair<AxEDNetPluginOCX, NVR> pair in m_dicOCXNVR)
            {
                NVR nvr = pair.Value;

                if (nvr.ID.Length == 0 || nvr.Password.Length == 0)
                    continue;

                string strLoginData = string.Format("{0}:{1}:{2}:{3}", nvr.Host, nvr.Port, nvr.ID, nvr.Password);
                pair.Key.sendEvent(EDNETP_EVENT_LOGIN, strLoginData);
                pair.Key.sendEvent(EDNETP_EVENT_CONNECT, "");
            }
        }

        private void SetSearchPattern()
        {
            string strFileName = "";
            int nIndex = m_strTargetFile.LastIndexOf('\\');

            if (nIndex > 0)
            {
                m_strTargetFolder = m_strTargetFile.Substring(0, nIndex);
                strFileName = m_strTargetFile.Substring(nIndex + 1);
            }
            else
            {
                m_strTargetFile = ".";
                strFileName = m_strTargetFile;
            }

            nIndex = strFileName.LastIndexOf('.');

            if (nIndex > 0)
            {
                m_strSearchPattern = string.Format("{0}*.{1}", strFileName.Substring(0, nIndex), strFileName.Substring(nIndex + 1));
            }
            else
            {
                m_strSearchPattern = strFileName + "*";
            }
        }

        private void InitializeComponent(List<NVR> datas)
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));

            foreach (NVR data in datas)
            {
                AxEDNetPluginOCX axEDNetPluginOCX = new AxEDNetPluginOCX();
                ((System.ComponentModel.ISupportInitialize)(axEDNetPluginOCX)).BeginInit();
                m_ocxList.Add(axEDNetPluginOCX);

                m_dicOCXNVR[axEDNetPluginOCX] = data;
            }

            this.cboNVR = new System.Windows.Forms.ComboBox();
            this.btnMakeAlarm = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();

            this.SuspendLayout();

            int nIndex = 1;

            foreach (AxEDNetPluginOCX axEDNetPluginOCX in m_ocxList)
            {
                // 
                // axEDNetPluginOCX
                // 
                axEDNetPluginOCX.Enabled = true;
                axEDNetPluginOCX.Location = new System.Drawing.Point(0, 0);
                axEDNetPluginOCX.Name = "axEDNetPluginOCX" + (nIndex++).ToString();
                axEDNetPluginOCX.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject(axEDNetPluginOCX.Name + ".OcxState")));
                axEDNetPluginOCX.Size = new System.Drawing.Size(192, 192);
                axEDNetPluginOCX.TabIndex = nIndex - 2;
                axEDNetPluginOCX.onEvent += new AxednetpluginocxLib._IEDNetPluginOCXEvents_onEventEventHandler(this.axEDNetPluginOCX_onEvent);
            }

            // 
            // cboNVR
            // 
            this.cboNVR.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboNVR.FormattingEnabled = true;
            this.cboNVR.Items.AddRange(new object[] {
            "4:1",
            "4:2",
            "4:5",
            "4:4"});
            this.cboNVR.Location = new System.Drawing.Point(607, 342);
            this.cboNVR.Name = "cboNVR";
            this.cboNVR.Size = new System.Drawing.Size(75, 20);
            this.cboNVR.TabIndex = 0;
            // 
            // btnMakeAlarm
            // 
            this.btnMakeAlarm.Location = new System.Drawing.Point(607, 368);
            this.btnMakeAlarm.Name = "btnMakeAlarm";
            this.btnMakeAlarm.Size = new System.Drawing.Size(75, 23);
            this.btnMakeAlarm.TabIndex = 1;
            this.btnMakeAlarm.Text = "알람 발생";
            this.btnMakeAlarm.UseVisualStyleBackColor = true;
            this.btnMakeAlarm.Click += new System.EventHandler(this.btnMakeAlarm_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.Location = new System.Drawing.Point(605, 318);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(135, 16);
            this.label1.TabIndex = 2;
            this.label1.Text = "Test Alarm 발생";

            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);

            foreach (AxEDNetPluginOCX axEDNetPluginOCX in m_ocxList)
            {
                this.Controls.Add(axEDNetPluginOCX);
            }

            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnMakeAlarm);
            this.Controls.Add(this.cboNVR);

            this.Name = "FormMain";
            this.ShowInTaskbar = false;
            this.Text = "Hidden OCX Form";

            foreach (AxEDNetPluginOCX axEDNetPluginOCX in m_ocxList)
            {
                ((System.ComponentModel.ISupportInitialize)(axEDNetPluginOCX)).EndInit();
            }

            this.ResumeLayout(false);
        }

        private void axEDNetPluginOCX_onEvent(object sender, AxednetpluginocxLib._IEDNetPluginOCXEvents_onEventEvent e)
        {
            System.Diagnostics.Trace.WriteLine(string.Format("onEvent({0}) : {1}", e.@event, e.data));

            if (e.@event == EDNETP_EVENT_HOST_EVENT)
            {
                NVR nvr;

                if (m_dicOCXNVR.TryGetValue((AxEDNetPluginOCX)sender, out nvr))
                {
                    CCTV cctv = nvr.FindCCTV(e.data.Trim());

                    if (cctv != null)
                    {
                        WriteAlarm(cctv);
                    }
                }
            }
        }

        private void WriteAlarm(CCTV cctv)
        {
            DateTime dtNow = DateTime.Now;

            if (m_strCurrentFile.Length == 0 || IsSameDay(dtNow, m_dtCurrent) == false)
            {
                SetCurrentFile(dtNow);

                // 하루가 지난 파일을 삭제한다.
                // 바로 전일의 로그는 남겨둔다.
                RemoveAlarmFile(m_dtCurrent.Year, m_dtCurrent.Month, m_dtCurrent.Day);
            }
            
            using (FileStream fs = File.Open(m_strCurrentFile, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite))
            {
                // 기존 파일에 계속 붙여넣기 한다.
                fs.Seek(0, SeekOrigin.End);

                byte[] bytes = GetAlarmBytes(cctv, dtNow);
                fs.Write(bytes, 0, bytes.Length);
            }
        }

        private void RemoveAlarmFile(int year, int month, int day)
        {
            DateTime dtCurrent = new DateTime(year, month, day), dtFile;
            string[] files = Directory.GetFiles(m_strTargetFolder, m_strSearchPattern);

            foreach (string strFile in files)
            {
                if (GetFileDate(strFile, out dtFile))
                {
                    TimeSpan span = dtCurrent - dtFile;

                    // 하루가 지났으면 삭제한다.
                    if (span.TotalDays > 1.0)
                        File.Delete(strFile);
                }
            }
        }

        private bool GetFileDate(string strFilePath, out DateTime dtDate)
        {
            dtDate = new DateTime();
            int nIndex = strFilePath.LastIndexOf('.');

            string strDate = "";

            if (nIndex > 0)
            {
                if (nIndex < 8)
                    return false;
                else
                    strDate = strFilePath.Substring(nIndex - 8, 8);
            }
            else
            {
                int len = strFilePath.Length;

                if (len < 8)
                    return false;
                else
                    strDate = strFilePath.Substring(len - 8, 8);
            }

            string strYear = strDate.Substring(0, 4);
            string strMonth = strDate.Substring(4, 2);
            string strDay = strDate.Substring(6, 2);

            int year, month, day;

            if (int.TryParse(strYear, out year) == false || int.TryParse(strMonth, out month) == false || int.TryParse(strDay, out day) == false)
                return false;

            dtDate = new DateTime(year, month, day);
            return true;
        }

        private byte[] GetAlarmBytes(CCTV cctv, DateTime timeStamp)
        {
            string strLine = string.Format("{0} 0 1 {1}\r\n", GetDateTimeAlarmString(timeStamp), cctv.ID);
            return m_encoding.GetBytes(strLine);
        }

        private void SetCurrentFile(DateTime dtNow)
        {
            int nIndex1 = m_strTargetFile.LastIndexOf('\\');
            int nIndex2 = m_strTargetFile.LastIndexOf('.');

            if (nIndex2 < 0 || nIndex2 < nIndex1)
                m_strCurrentFile = m_strTargetFile + GetDateTimeFileString(dtNow);
            else
            {
                string str1 = m_strTargetFile.Substring(0, nIndex2);
                string str2 = m_strTargetFile.Substring(nIndex2);

                m_strCurrentFile = str1 + GetDateTimeFileString(dtNow) + str2;
            }

            m_dtCurrent = dtNow;
        }

        private bool IsSameDay(DateTime dt1, DateTime dt2)
        {
            return dt1.Year == dt2.Year && dt1.Month == dt2.Month && dt1.Day == dt2.Day;
        }

        private string GetDateTimeAlarmString(DateTime timeStamp)
        {
            return string.Format("{0}-{1:00}-{2:00}_{3:00}:{4:00}:{5:00}", timeStamp.Year, timeStamp.Month, timeStamp.Day, timeStamp.Hour, timeStamp.Minute, timeStamp.Second);
        }

        private string GetDateTimeFileString(DateTime timeStamp)
        {
            return string.Format("_{0}{1:00}{2:00}", timeStamp.Year, timeStamp.Month, timeStamp.Day);
        }

        private void btnMakeAlarm_Click(object sender, EventArgs e)
        {
            if (cboNVR.SelectedIndex < 0)
                return;

            _IEDNetPluginOCXEvents_onEventEvent ev = new _IEDNetPluginOCXEvents_onEventEvent(EDNETP_EVENT_HOST_EVENT, cboNVR.Text);
            axEDNetPluginOCX_onEvent(m_ocxList[0], ev);
        }
    }

    public class NVR
    {
        private string m_strHost = "";
        private int m_nPort = 0;
        private string m_strID = "";
        private string m_strPW = "";

        private List<CCTV> m_cctvs = new List<CCTV>();
        private Dictionary<string, CCTV> m_dicFireEventCCTVs = new Dictionary<string, CCTV>();

        public string Host
        {
            get { return m_strHost; }
            set { m_strHost = value; }
        }

        public int Port
        {
            get { return m_nPort; }
            set { m_nPort = value; }
        }

        public string ID
        {
            get { return m_strID; }
            set { m_strID = value; }
        }

        public string Password
        {
            get { return m_strPW; }
            set { m_strPW = value; }
        }

        public void AddCCTV(CCTV cctv)
        {
            m_cctvs.Add(cctv);
        }

        public void SetFireEventCCTV(string strEvent, CCTV cctv)
        {
            m_dicFireEventCCTVs[strEvent] = cctv;
        }

        public CCTV FindCCTV(string strEvent)
        {
            CCTV cctv;

            if (m_dicFireEventCCTVs.TryGetValue(strEvent, out cctv))
                return cctv;

            return null;
        }
    }

    public class CCTV
    {
        private int m_nID = 0;
        private string m_strCameraName = "";
        private int m_nChannel = 0;
        private bool m_isFire = false;
        private string m_strFireEventData = "";

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public string CameraName
        {
            get { return m_strCameraName; }
            set { m_strCameraName = value; }
        }

        public int Channel
        {
            get { return m_nChannel; }
            set { m_nChannel = value; }
        }

        public bool IsFire
        {
            get { return m_isFire; }
            set { m_isFire = value; }
        }

        public string FireEventData
        {
            get { return m_strFireEventData; }
            set { m_strFireEventData = value; }
        }
    }
}
