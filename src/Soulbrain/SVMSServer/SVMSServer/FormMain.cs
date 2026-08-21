using System;
using System.Collections.Generic;
using dnsData.Sensor;
using System.Windows.Forms;
using System.Drawing;
using SDMS.Model.CCTV;

namespace SVMSServer
{
    public partial class FormMain : Form, ISVMSEventOwner
    {
        private SVMSEventReceiver m_svmsEventReceiver = null;
        private CCTVManager m_cctvManager = null;
        private AlarmManager m_alarmManager = null;
        private DateTime? m_dtLastUpdate = null;

        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
#if DB_TEST
            FormDBTest frm = new FormDBTest();
            frm.ShowDialog();
#else
            List<SVMSEventReceiver> svmsEventReceivers = SVMSEventReceiver.MakeInstances(this);
            //m_svmsEventReceiver = new SVMSEventReceiver(this);/*SetEquipZoneCCTV(m_svmsEventReceiver.DataManager);*/

            if (svmsEventReceivers != null && svmsEventReceivers.Count > 0)
            {
                m_svmsEventReceiver = svmsEventReceivers[0];
                FormLogin frmLogin = new FormLogin(m_svmsEventReceiver.SvmsServerIP, m_svmsEventReceiver.SvmsPort, m_svmsEventReceiver.ID, m_svmsEventReceiver.Password);

                if (frmLogin.ShowDialog() == DialogResult.OK)
                {
                    m_cctvManager = new CCTVManager(m_svmsEventReceiver.DataManager, m_svmsEventReceiver.CommonDataManager);
                    m_svmsEventReceiver.ConnectServer(frmLogin.ServerIP, frmLogin.ServerPort, frmLogin.UserID, frmLogin.Password);
                    m_alarmManager = new AlarmManager(m_svmsEventReceiver.DataManager, m_svmsEventReceiver.CommonDataManager);
                }

                timer1.Start();
            }
#endif
        }

        /*private void SetEquipZoneCCTV(SDMS.DAL.DataManager dataManager)
        {
            bool isNullable;
            string strConditions = string.Format("{0} >= {1}", SDMS.Model.Sensor.SensorZone.GetFieldName(SDMS.Model.Sensor.SensorZone.Fields.SensorType, out isNullable), 900);

            string strErrorMessage;
            List<SDMS.Model.Sensor.SensorZone> sensorZones = dataManager.GetSelectManager().SelectSensorZones(null, strConditions, out strErrorMessage);

            if (sensorZones == null)
                return;

            Dictionary<int, int> dicEquipZoneCCTVs = new Dictionary<int, int>();

            foreach (SDMS.Model.Sensor.SensorZone sensorZone in sensorZones)
            {
                dicEquipZoneCCTVs[sensorZone.EquipZoneID] = sensorZone.OrgSensorID;
            }

            int cctvID;
            List<EquipZoneCCTV> equipZoneCCTVs = dataManager.GetSelectManager().SelectEquipZoneCCTVs(null, null, out strErrorMessage);

            foreach (EquipZoneCCTV equipZoneCCTV in equipZoneCCTVs)
            {
                if (dicEquipZoneCCTVs.TryGetValue(equipZoneCCTV.EquipZoneID, out cctvID))
                {
                    if (equipZoneCCTV.CCTV1 == cctvID)
                        continue;
                    if (equipZoneCCTV.CCTV2 == cctvID)
                        continue;
                    if (equipZoneCCTV.CCTV3 == cctvID)
                        continue;
                    if (equipZoneCCTV.CCTV4 == cctvID)
                        continue;

                    equipZoneCCTV.CCTV1 = cctvID;
                    
                    if (dataManager.GetUpdateManager().UpdateEquipZoneCCTV(equipZoneCCTV, out strErrorMessage) == false)
                    {
                        System.Diagnostics.Trace.WriteLine("Update Faile : " + strErrorMessage);
                    }
                }
            }
        }*/

        private void AddLog(string strLog)
        {
            string strPrev = textBoxStatus.Text.Trim();

            if (strPrev.Length == 0)
                textBoxStatus.Text = strLog;
            else
                textBoxStatus.Text = strPrev + "\r\n" + strLog;
        }

        public void OnConnection(bool isSuccess)
        {
            if (isSuccess)
                AddLog("SVMS 접속 성공");
            else
                AddLog("SVMS 접속 실패");
        }

        public void OnClientType(string strClientGUID)
        {

        }

        public void OnLogin(bool isSuccess)
        {
            if (isSuccess)
                AddLog("SVMS 로그인 성공");
            else
                AddLog("SVMS 로그인 실패");
        }

        public void OnDisconnect()
        {
            AddLog("SVMS 접속 끊어짐");
        }

        public void OnMessage(DateTime eventTime, string uniqueKey, Facility.FacilityType sensorType, string strMessage)
        {
            DateTime dtNow = DateTime.Now;
            string strTime = string.Format("{0}-{1:00}-{2:00} {3:00}:{4:00}:{5:00} ", dtNow.Year, dtNow.Month, dtNow.Day, dtNow.Hour, dtNow.Minute, dtNow.Second);

            if (sensorType != Facility.FacilityType.NONE)
            {
                m_cctvManager.SendEvent(eventTime, uniqueKey, sensorType);
            }

            this.Invoke((MethodInvoker)delegate
            {
                string strText = textBoxStatus.Text.Trim();

                if (strText.Length == 0)
                    strText = strTime + strMessage;
                else
                    strText += "\r\n" + strTime + strMessage;

                textBoxStatus.Text = strText;
            });
        }

        public void OnModifiedCamera(CCTV cctv)
        {
            if (m_cctvManager != null)
            {
                if (m_cctvManager.UpdateCCTV(cctv))
                {
                    m_dtLastUpdate = DateTime.Now;
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (m_svmsEventReceiver != null)
            {
                bool isConnected = m_svmsEventReceiver.IsConnectSVMS;

                if (isConnected == true)
                {
                    labelSVMSStatus.Text = "SVMS 접속중";
                    labelSVMSStatus.ForeColor = Color.Green;
                }
                else
                {
                    labelSVMSStatus.Text = "SVMS 접속안됨";
                    labelSVMSStatus.ForeColor = Color.Red;
                }

                if (m_dtLastUpdate != null && PassUpdateTime((DateTime)m_dtLastUpdate, 1.0))
                {
                    // 마지막 업데이트 한 이후로 1분이 경과했으면 CCTV Server를 새로 시작시킨다.
                    m_dtLastUpdate = null;
                    m_cctvManager.RestartProcess();
                }

                m_alarmManager.CheckAutoClose();
                Logger.Instance.RemoveOldLogs();

                /*if (eventReciver.Client == null)
                {
                    lbSOPServer.Text = "접속안됨";
                    lbSOPServer.ForeColor = Color.Red;
                }
                else
                {
                    if (eventReciver.Client.IsConnected)
                    {
                        lbSOPServer.Text = "접속중";
                        lbSOPServer.ForeColor = Color.Green;
                    }
                    else
                    {
                        lbSOPServer.Text = "접속안됨";
                        lbSOPServer.ForeColor = Color.Red;
                    }
                }*/
            }
        }

        private bool PassUpdateTime(DateTime dtPrev, double seconds)
        {
            TimeSpan span = DateTime.Now - dtPrev;
            return span.TotalSeconds >= seconds;
        }

        private void btnUpdateCCTV_Click(object sender, EventArgs e)
        {
            // svms로부터 받아야 한다.
            ICollection<CCTV> svmsCCTVs = m_svmsEventReceiver.GetCCTVList();
            //List<CCTV> svmsCCTVs = new List<CCTV>();
            m_cctvManager.Update(svmsCCTVs);
        }

        public void OnAddCCTV(CCTV cctv)
        {
        }

        private void btnSendSVMSEvent_Click(object sender, EventArgs e)
        {
            string strID = textBoxCCTVID.Text.Trim();
            int nID;

            if (int.TryParse(strID, out nID) == false)
            {
                MessageBox.Show("CCTV ID를 입력하세요.");
                return;
            }

            m_cctvManager.SendTestEvent(DateTime.Now, nID, Facility.FacilityType.Collapse_S1);
        }
    }
}
