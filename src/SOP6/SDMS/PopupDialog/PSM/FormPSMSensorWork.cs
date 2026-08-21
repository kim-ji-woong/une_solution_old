using DBUtility2;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SDMS.Help;

namespace SDMS.PopupDialog
{
    public partial class FormPSMSensorWork : PopupFormBase
    {
        public delegate List<UnE.PSM.PSMSensor> BeginSaveAllSensorWorkEventHandler();
        public event BeginSaveAllSensorWorkEventHandler BeginSaveAllSensorWorkEvent;

        private WebDBManager m_dbMgr = null;

        private UnE.PSM.PSMSensor m_sensor = null;
        private int m_nSensorNo = -1;
        private string m_strLocationName = "";
        private string m_strMaterialName = "";

        public const string LocalOffTag = "PSMSensorLocalOff";
        public const string LocalOffSection = "AlarmOption";

        public UnE.PSM.PSMSensor Sensor
        {
            get { return m_sensor; }
            set { m_sensor = value; }
        }

        public int SensorNo
        {
            get { return m_nSensorNo; }
            set { m_nSensorNo = value; }
        }

        public string LocationName
        {
            get { return m_strLocationName; }
            set { m_strLocationName = value; }
        }

        public string MaterialName
        {
            get { return m_strMaterialName; }
            set { m_strMaterialName = value; }
        }

        private ManualManager m_manualManager = null;

        public FormPSMSensorWork(UnE.PSM.PSMSensor sensor, int nSensorNo, string strLocationName, string strMaterialName)
        {
            InitializeComponent();

            m_dbMgr = FormMain.Instance.DBManager;

            m_sensor = sensor;
            m_nSensorNo = nSensorNo;
            m_strLocationName = strLocationName;
            m_strMaterialName = strMaterialName;

            InitEvent();

            InitCtrlSize(this);

            lblTitle.MouseDown += PopupFormBase_MouseDown;
            lblTitle.MouseMove += PopupFormBase_MouseMove;
            lblTitle.MouseUp += PopupFormBase_MouseUp;

            m_manualManager = new ManualManager(this);
            SetManualID();
        }

        private void InitEvent()
        {
            this.Load += FormPSMSensorWork_Load;

            this.rdoON.Click += (s, e) => { ChangeState(UnE.PSM.PSMSensor.Status.On); };
            this.rdoOFF.Click += (s, e) => { ChangeState(UnE.PSM.PSMSensor.Status.Off); };
            this.rdoWork.Click += (s, e) => { ChangeState(UnE.PSM.PSMSensor.Status.Off4Work); };

            this.btnSave.Click += btnSave_Click;
            this.btnCancel.Click += btnCancel_Click;
        }

        private void LoadSensorState()
        {
            // 데이터 로드..
            if (m_sensor != null)
            {
                if (m_sensor.SensorStatus == UnE.PSM.PSMSensor.Status.On)
                {
                    rdoON.Checked = true;
                    ChangeState(UnE.PSM.PSMSensor.Status.On);
                }
                else if (m_sensor.SensorStatus == UnE.PSM.PSMSensor.Status.Off)
                {
                    rdoOFF.Checked = true;
                    ChangeState(UnE.PSM.PSMSensor.Status.Off);
                }
                else if (m_sensor.SensorStatus == UnE.PSM.PSMSensor.Status.Off4Work)
                {
                    rdoWork.Checked = true;

                    if (m_sensor.BeginWorkTime != null && m_sensor.EndWorkTime != null)
                    {
                        InitTime(m_sensor.BeginWorkTime, m_sensor.EndWorkTime);
                        ChangeState(UnE.PSM.PSMSensor.Status.Off4Work);
                        return;
                    }
                }
                else if (m_sensor.SensorStatus == UnE.PSM.PSMSensor.Status.LocalOff)
                {
                    rdoLocalOff.Checked = true;
                    ChangeState(UnE.PSM.PSMSensor.Status.LocalOff);
                }
                else
                {
                    rdoUnvisible.Checked = true;
                    ChangeState(UnE.PSM.PSMSensor.Status.Unknown);
                }
            }

            InitTime(null, null);
        }

        private void SaveStateForAllSensor()
        {
            // 원본 센서 객체 저장
            UnE.PSM.PSMSensor orgSensor = m_sensor;

            List<UnE.PSM.PSMSensor> liSensors = new List<UnE.PSM.PSMSensor>();

            if (BeginSaveAllSensorWorkEvent != null)
            {
                liSensors = BeginSaveAllSensorWorkEvent();
            }

            foreach (UnE.PSM.PSMSensor sensor in liSensors)
            {
                m_sensor = sensor;
                SaveState();
            }

            m_sensor = orgSensor;

            chkAll.Checked = false;

        }

        private void SaveState()
        {
            // 데이터 저장..
            if (m_sensor == null)
                return;

            //RemoveLocalOff();
            VariousData<DateTime> beginDate = null, beginTime = null;
            VariousData<DateTime> endDate = null, endTime = null;
            UnE.PSM.PSMSensor.Status status = UnE.PSM.PSMSensor.Status.On;

            if (rdoON.Checked)
                status = UnE.PSM.PSMSensor.Status.On;
            else if (rdoOFF.Checked)
                status = UnE.PSM.PSMSensor.Status.Off;
            else if (rdoWork.Checked)
            {
                status = UnE.PSM.PSMSensor.Status.Off4Work;

                beginDate = new VariousData<DateTime>(dtFromDate.Value);
                beginTime = new VariousData<DateTime>(dtFromTime.Value);
                endDate = new VariousData<DateTime>(dtToDate.Value);
                endTime = new VariousData<DateTime>(dtToTime.Value);
            }
            else if (rdoLocalOff.Checked)
                status = UnE.PSM.PSMSensor.Status.LocalOff;
            else
                return;

            SaveSensorStatus(Sensor, status, beginDate, beginTime, endDate, endTime);

            /*UnE.PSM.PSMSensor.Status status = UnE.PSM.PSMSensor.Status.On;
            long beginWorkTime = 0, endWorkTime = 0;

            if (rdoON.Checked)
            {
                status = UnE.PSM.PSMSensor.Status.On;

                if (Sensor.SensorStatus == UnE.PSM.PSMSensor.Status.On)
                    return;
            }
            else if (rdoOFF.Checked)
            {
                status = UnE.PSM.PSMSensor.Status.Off;

                if (Sensor.SensorStatus == UnE.PSM.PSMSensor.Status.Off)
                    return;
            }
            else if (rdoWork.Checked)
            {
                status = UnE.PSM.PSMSensor.Status.Off4Work;

                DateTime beginDate = dtFromDate.Value;
                DateTime beginTime = dtFromTime.Value;
                DateTime endDate = dtToDate.Value;
                DateTime endTime = dtToTime.Value;

                DateTime dtBegin = new DateTime(beginDate.Year, beginDate.Month, beginDate.Day, beginTime.Hour, beginTime.Minute, beginTime.Second);
                DateTime dtEnd = new DateTime(endDate.Year, endDate.Month, endDate.Day, endTime.Hour, endTime.Minute, endTime.Second);

                beginWorkTime = dtBegin.ToBinary();
                endWorkTime = dtEnd.ToBinary();

                if (Sensor.SensorStatus == UnE.PSM.PSMSensor.Status.Off4Work &&
                    Sensor.BeginWorkTime != null && Sensor.EndWorkTime != null &&
                    Sensor.BeginWorkTime.Data == dtBegin && Sensor.EndWorkTime.Data == dtEnd)
                    return;
            }
            else if (rdoLocalOff.Checked)
            {
                status = UnE.PSM.PSMSensor.Status.LocalOff;

                if (Sensor.SensorStatus == UnE.PSM.PSMSensor.Status.LocalOff)
                    return;
            }
            else
                return;

            // LocalOff는 Local 속성이므로 서버에게 알리지 않는다.
            if (status == UnE.PSM.PSMSensor.Status.LocalOff)
            {
                SaveLocalOff(Sensor);
            }
            else
            {
                if (Sensor.SensorStatus == UnE.PSM.PSMSensor.Status.LocalOff)
                    RemoveLocalOff(Sensor);

                if (IsChanged(Sensor, status, beginWorkTime, endWorkTime))
                    NetworkManager.Instance.SendPSMSensorStatus(Sensor.ID, (byte)status, beginWorkTime, endWorkTime);
            }*/
        }

        public static void SaveSensorStatus(UnE.PSM.PSMSensor sensor, UnE.PSM.PSMSensor.Status status, VariousData<DateTime> beginDate = null, VariousData<DateTime> beginTime = null, VariousData<DateTime> endDate = null, VariousData<DateTime> endTime = null)
        {
            if (sensor == null || sensor.SensorStatus == status)
                return;

            long beginWorkTime = 0, endWorkTime = 0;

            if (status == UnE.PSM.PSMSensor.Status.Off4Work)
            {
                DateTime dtBegin = new DateTime(beginDate.Data.Year, beginDate.Data.Month, beginDate.Data.Day, beginTime.Data.Hour, beginTime.Data.Minute, beginTime.Data.Second);
                DateTime dtEnd = new DateTime(endDate.Data.Year, endDate.Data.Month, endDate.Data.Day, endTime.Data.Hour, endTime.Data.Minute, endTime.Data.Second);

                beginWorkTime = dtBegin.ToBinary();
                endWorkTime = dtEnd.ToBinary();

                if (sensor.SensorStatus == UnE.PSM.PSMSensor.Status.Off4Work &&
                    sensor.BeginWorkTime != null && sensor.EndWorkTime != null &&
                    sensor.BeginWorkTime.Data == dtBegin && sensor.EndWorkTime.Data == dtEnd)
                    return;
            }

            if (status == UnE.PSM.PSMSensor.Status.LocalOff)
                SaveLocalOff(sensor);
            else
            {
                if (sensor.SensorStatus == UnE.PSM.PSMSensor.Status.LocalOff)
                    RemoveLocalOff(sensor);

                if (IsChanged(sensor, status, beginWorkTime, endWorkTime))
                    NetworkWebManager.Instance.SendPSMSensorStatus(sensor.ID, (byte)status, beginWorkTime, endWorkTime);
            }
        }

        private static bool IsChanged(UnE.PSM.PSMSensor sensor, UnE.PSM.PSMSensor.Status status, long beginWorkTime, long endWorkTime)
        {
            string strSQL = "Select Status, BeginTime, EndTime from PSMSensorSchedule where SensorID = " + sensor.ID.ToString();
            System.Collections.ArrayList arrResult = FormMain.Instance.DBManager.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count != 3)
                return false;

            int nStatus = WebDBManager.GetIntField(arrResult[0].ToString(), -1);
            VariousData<DateTime> dtBegin = WebDBManager.GetDateTimeField(arrResult[1]);
            VariousData<DateTime> dtEnd = WebDBManager.GetDateTimeField(arrResult[2]);

            if (nStatus == (int)status)
            {
                if (status == UnE.PSM.PSMSensor.Status.Off4Work)
                {
                    if (dtBegin == null || dtEnd == null)
                        return true;

                    long begin = dtBegin.Data.ToBinary();
                    long end = dtEnd.Data.ToBinary();

                    if (begin == beginWorkTime && end == endWorkTime)
                        return false;
                    else
                        return true;
                }

                return false;
            }

            return true;
        }

        public static List<int> ReadLocalOffSensorIDList()
        {
            List<int> sensorIDList = new List<int>();
            string strLocalOffList = FormMain.Instance.DBManager.LoadIni(LocalOffTag, LocalOffSection).Trim();

            if (strLocalOffList.Length == 0)
                return sensorIDList;

            string[] sensorList = strLocalOffList.Split(',');

            foreach (string strSensorID in sensorList)
            {
                int nSensorID;

                if (int.TryParse(strSensorID.Trim(), out nSensorID))
                {
                    sensorIDList.Add(nSensorID);
                }
            }

            return sensorIDList;
        }

        private static void RemoveLocalOff(UnE.PSM.PSMSensor sensor)
        {
            string strLocalOffList = FormMain.Instance.DBManager.LoadIni(LocalOffTag, LocalOffSection).Trim();

            if (strLocalOffList.Length == 0)
                return;

            string strFinalList = "";
            string[] sensorList = strLocalOffList.Split(',');

            foreach (string strSensorID in sensorList)
            {
                int nSensorID;

                if (int.TryParse(strSensorID.Trim(), out nSensorID))
                {
                    if (sensor.ID == nSensorID)
                    {
                        // LocalOff 속성을 없앴으니 DB의 값을 읽어온다.
                        LoadSensorStatusData(sensor);
                        continue;
                    }

                    if (strFinalList.Length == 0)
                        strFinalList = nSensorID.ToString();
                    else
                        strFinalList += "," + nSensorID.ToString();
                }
            }

            FormMain.Instance.DBManager.SaveIni(LocalOffTag, strFinalList, LocalOffSection);

            NetworkWebManager.Instance.SendRequestReactionLogList();
        }

        private static void SaveLocalOff(UnE.PSM.PSMSensor sensor)
        {
            string strLocalOffList = FormMain.Instance.DBManager.LoadIni(LocalOffTag, LocalOffSection).Trim();

            bool find = false;
            string strFinalList = "";

            if (strLocalOffList.Length > 0)
            {
                string[] sensorList = strLocalOffList.Split(',');

                foreach (string strSensorID in sensorList)
                {
                    int nSensorID;

                    if (int.TryParse(strSensorID.Trim(), out nSensorID))
                    {
                        if (sensor.ID == nSensorID)
                            find = true;

                        if (strFinalList.Length == 0)
                            strFinalList = nSensorID.ToString();
                        else
                            strFinalList += "," + nSensorID.ToString();
                    }
                }
            }

            if (find == false)
            {
                if (strFinalList.Length == 0)
                    strFinalList = sensor.ID.ToString();
                else
                    strFinalList += "," + sensor.ID.ToString();
            }

            sensor.SensorStatus = UnE.PSM.PSMSensor.Status.LocalOff;
            FormMain.Instance.DBManager.SaveIni(LocalOffTag, strFinalList, LocalOffSection);

            List<UnE.Sensor.ISensor> pSensor = SensorManager.Instance.GetPSMSensorZone(sensor.ID);
            if (pSensor != null)
            {
                foreach (UnE.Sensor.ISensor _sensor in pSensor)
                {
                    FormMain.Instance.RemoveSensorDetect(_sensor.ID);
                }
            }

        }

        private static void LoadSensorStatusData(UnE.PSM.PSMSensor sensor)
        {
            string strSQL = "Select Status, BeginTime, EndTime from PSMSensorSchedule where SensorID = " + sensor.ID.ToString();
            System.Collections.ArrayList arrResult = FormMain.Instance.DBManager.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count != 3)
                return;

            int nStatus = WebDBManager.GetIntField(arrResult[0].ToString(), -1);
            VariousData<DateTime> dtBegin = WebDBManager.GetDateTimeField(arrResult[1]);
            VariousData<DateTime> dtEnd = WebDBManager.GetDateTimeField(arrResult[2]);

            sensor.SensorStatus = UnE.PSM.PSMSensor.ToStatus(nStatus);
            sensor.BeginWorkTime = dtBegin;
            sensor.EndWorkTime = dtEnd;
        }

        private void ChangeState(UnE.PSM.PSMSensor.Status state)
        {
            switch (state)
            {
                case UnE.PSM.PSMSensor.Status.On:
                    /*rdoON.Checked = true;
                    rdoOFF.Checked =
                    rdoWork.Checked = false;*/

                    //this.Size = new Size(196, this.Size.Height);
                    break;

                case UnE.PSM.PSMSensor.Status.Off:
                case UnE.PSM.PSMSensor.Status.LocalOff:
                    /*rdoOFF.Checked = true;
                    rdoON.Checked =
                    rdoWork.Checked = false;*/

                    //this.Size = new Size(196, this.Size.Height);
                    break;

                case UnE.PSM.PSMSensor.Status.Off4Work:
                    /*rdoWork.Checked = true;
                    rdoOFF.Checked =
                    rdoON.Checked = false;*/

                    //this.Size = new Size(600, this.Size.Height);
                    break;

            }

            lblWork.Visible =
            lblSymbol.Visible =
            dtFromDate.Visible =
            dtFromTime.Visible =
            dtToDate.Visible =
            dtToTime.Visible = rdoWork.Checked;

        }

        private bool CheckValidate(out string strError)
        {
            strError = string.Empty;

            if (rdoWork.Checked == true)
            {
                DateTime dtFrom = dtFromDate.Value.Date + dtFromTime.Value.TimeOfDay;
                DateTime dtTo = dtToDate.Value.Date + dtToTime.Value.TimeOfDay;

                if (dtFrom > dtTo)
                {
                    strError = String.Format("작업시작일시가 작업종료일시보다 빠릅니다.{0}작업기간를 다시 설정하세요.", Environment.NewLine);
                    return false;
                }

            }

            return true;
        }


        private void FormPSMSensorWork_Load(object sender, EventArgs e)
        { 
            LoadData();
        }

        private void InitTime(VariousData<DateTime> beginWorkTime, VariousData<DateTime> endWorkTime)
        {
            if (beginWorkTime == null)
            {
                dtFromDate.Value = DateTime.Now;
                dtFromTime.Value = dtFromDate.Value;
            }
            else
            {
                dtFromDate.Value = beginWorkTime.Data;
                dtFromTime.Value = dtFromDate.Value;
            }

            if (endWorkTime == null)
            {
                dtToDate.Value = dtFromDate.Value.AddDays(1.0);
                dtToTime.Value = dtToDate.Value;
            }
            else
            {
                dtToDate.Value = endWorkTime.Data;
                dtToTime.Value = dtToDate.Value;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (chkAll.Checked == true)
            {
                string strUpdateState = (rdoON.Checked == true) ? "On" : "Off";

                if (MessageBox.Show(String.Format("모든 유해물질 감지센서에 대해서\r\n'{0}' 상태로 일괄 적용하시겠습니까?", strUpdateState), "위험물질 감지센서 설정", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    return;
            }

            string strError = string.Empty;

            // 유효성 검사
            if (CheckValidate(out strError) == false)
            {
                MessageBox.Show(strError, "PSM Sensor State", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 적용(저장)
            if (chkAll.Checked == true)
                SaveStateForAllSensor();
            else
                SaveState();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            // 닫기(취소)
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        public void LoadData()
        {
            string strTitle = string.Format("{0} - {1}, {2}", m_nSensorNo, m_strLocationName, m_strMaterialName);
            this.Text = strTitle;
            LoadSensorState();
        }

        private void SetManualID()
        {
            m_manualManager.Handle = this.Handle;

            m_manualManager.Clear();
            m_manualManager.SetID(lblTitle, "PSMList_SensorOnOff"); 
            m_manualManager.ProcessEvent();
        } 
    }
}