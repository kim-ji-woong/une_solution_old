using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace SimulationScenarioMaker
{
    public partial class FormMain : Form
    {
        private DataManager m_dataMgr = null;
        private XMLManager m_xmlMgr = null;
        private SensorEvents m_currentEvents = null;

        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            m_dataMgr = new DataManager();
            m_xmlMgr = new XMLManager();

            if (m_xmlMgr.ReadXML(m_xmlMgr.FilePath, m_dataMgr))
                UpdateData();
            else
                radioSensorType_CheckedChanged(radioWorker, null);
        }

        private void UpdateData()
        {
            int nRunningTimeMinute = m_dataMgr.RunningTime / 60;
            int nRunningTimeSecond = m_dataMgr.RunningTime % 60;

            textBoxRunningMinute.Text = nRunningTimeMinute.ToString();
            textBoxRunningSecond.Text = nRunningTimeSecond.ToString();

            if (m_dataMgr.RepeatCount <= 0)
                cboRepeat.SelectedIndex = 0;
            else if (m_dataMgr.RepeatCount >= cboRepeat.Items.Count)
                cboRepeat.SelectedIndex = cboRepeat.Items.Count - 1;
            else
                cboRepeat.SelectedIndex = m_dataMgr.RepeatCount;

            textBoxEventMinute.Text = "";
            textBoxEventSecond.Text = "";
            textBoxPeriodSecond.Text = "";
            textBoxX.Text = textBoxY.Text = "";
            textBoxCoords.Text = "";
            cboSensorIDs.Items.Clear();

            DataManager.SensorType type = DataManager.SensorType.WORKER;

            if (radioWorker.Checked)
                type = DataManager.SensorType.WORKER;
            else if (radioVehicle.Checked)
                type = DataManager.SensorType.VEHICLE;
            else if (radioEquipment.Checked)
                type = DataManager.SensorType.EQUIPMENT;
            else
                return;

            ChangeSensorType(type);
        }

        private void ChangeSensorType(DataManager.SensorType type)
        {
            cboSensorIDs.Items.Clear();

            ArrayList arrSensorEvents = m_dataMgr.GetSensorEvents(type);

            if (arrSensorEvents != null)
            {
                foreach (SensorEvents events in arrSensorEvents)
                {
                    cboSensorIDs.Items.Add(events);
                }
            }

            if (cboSensorIDs.Items.Count > 0)
                cboSensorIDs.SelectedIndex = 0;
            else
            {
                textBoxCoords.Text = "";
                m_currentEvents = null;
            }
        }

        private bool ApplyEvents()
        {
            SensorEvents events = m_currentEvents;

            if (events == null)
                return true;

            events.Events.Clear();

            int nBeginIndex = 0;
            int min, max, second;
            double x, y;

            while (nBeginIndex < textBoxCoords.Text.Length)
            {
                int nIndex1 = textBoxCoords.Text.IndexOf('\r', nBeginIndex);
                int nIndex2 = textBoxCoords.Text.IndexOf('\n', nBeginIndex);

                if (nIndex1 < 0 && nIndex2 < 0)
                    min = max = textBoxCoords.Text.Length;
                else if (nIndex1 < 0)
                    min = max = nIndex2;
                else if (nIndex2 < 0)
                    min = max = nIndex1;
                else
                {
                    if (nIndex1 < nIndex2)
                    {
                        min = nIndex1;
                        max = nIndex2;
                    }
                    else
                    {
                        min = nIndex2;
                        max = nIndex1;
                    }
                }

                string strLine = textBoxCoords.Text.Substring(nBeginIndex, min - nBeginIndex);

                if (!GetEventData(strLine, out second, out x, out y))
                    return false;

                SensorEvents.SensorEvent sensorEvent = new SensorEvents.SensorEvent();

                sensorEvent.EventTime = second;
                sensorEvent.X = x;
                sensorEvent.Y = y;

                events.Events.Add(sensorEvent);

                nBeginIndex = max + 1;
            }

            return true;
        }

        private void cboSensorIDs_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboSensorIDs.SelectedIndex < 0)
                return;

            if (!ApplyEvents())
            {
                if (MessageBox.Show("TextBox에 잘못된 텍스트가 포함되어 있습니다.\r\n해당 센서의 모든 데이터가 삭제될 수 있습니다.\r\n계속 진행하시겠습니까?",
                    "경고", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No)
                    return;
            }

            SensorEvents events = (SensorEvents)cboSensorIDs.Items[cboSensorIDs.SelectedIndex];

            string strCoords = "";
            int nEventTime = 0, nPrevEventTime = 0;
            int nPeriodTime = 1;

            foreach (SensorEvents.SensorEvent data in events.Events)
            {
                string strLine = MakeLineString(data.EventTime, data.X, data.Y);

                if (strCoords.Length == 0)
                    strCoords = strLine;
                else
                    strCoords += "\r\n" + strLine;

                nPeriodTime = data.EventTime - nPrevEventTime;
                nEventTime = data.EventTime + nPeriodTime;
                nPrevEventTime = data.EventTime;
            }

            int min = nEventTime / 60;
            int sec = nEventTime % 60;

            if (nPeriodTime <= 0)
                nPeriodTime = 1;

            textBoxEventMinute.Text = min.ToString();
            textBoxEventSecond.Text = sec.ToString();
            textBoxPeriodSecond.Text = nPeriodTime.ToString();
            textBoxX.Text = textBoxY.Text = "";
            textBoxCoords.Text = strCoords;

            m_currentEvents = events;
        }

        private string MakeLineString(int nEventTime, double x, double y)
        {
            return string.Format("{0}초,{1:F3},{2:F3}", nEventTime, x, y);
        }

        private void AddLineString(string strLine)
        {
            if (textBoxCoords.Text.Length == 0)
                textBoxCoords.Text = strLine;
            else
                textBoxCoords.Text += "\r\n" + strLine;
        }

        private void radioSensorType_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton radioType = (RadioButton)sender;

            if (!radioType.Checked)
                return;

            if (!ApplyEvents())
            {
                if (MessageBox.Show("TextBox에 잘못된 텍스트가 포함되어 있습니다.\r\n해당 센서의 모든 데이터가 삭제될 수 있습니다.\r\n계속 진행하시겠습니까?",
                    "경고", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No)
                    return;
            }

            if (radioType == radioWorker)
                ChangeSensorType(DataManager.SensorType.WORKER);
            else if (radioType == radioVehicle)
                ChangeSensorType(DataManager.SensorType.VEHICLE);
            else if (radioType == radioEquipment)
                ChangeSensorType(DataManager.SensorType.EQUIPMENT);
        }

        private SensorEvents.SensorEvent GetLastEvent()
        {
            if (textBoxCoords.Text.Length == 0)
                return null;

            string strLine = "";
            int nPrevIndex = textBoxCoords.Text.Length;
            int min, max, sec;
            double x, y;

            while (nPrevIndex > 0)
            {
                int nIndex1 = textBoxCoords.Text.LastIndexOf('\r', nPrevIndex - 1);
                int nIndex2 = textBoxCoords.Text.LastIndexOf('\n', nPrevIndex - 1);

                if (nIndex1 < 0 && nIndex2 < 0)
                    min = max = -1;
                else if (nIndex1 < 0)
                    min = max = nIndex2;
                else if (nIndex2 < 0)
                    min = max = nIndex1;
                else
                {
                    if (nIndex1 < nIndex2)
                    {
                        min = nIndex1;
                        max = nIndex2;
                    }
                    else
                    {
                        min = nIndex2;
                        max = nIndex1;
                    }
                }

                strLine = textBoxCoords.Text.Substring(max + 1, nPrevIndex - max - 1);

                if (GetEventData(strLine, out sec, out x, out y))
                {
                    SensorEvents.SensorEvent sensorEvent = new SensorEvents.SensorEvent();

                    sensorEvent.EventTime = sec;
                    sensorEvent.X = x;
                    sensorEvent.Y = y;

                    return sensorEvent;
                }

                nPrevIndex = min;
            }

            return null;
        }

        private bool GetEventData(string strLine, out int second, out double x, out double y)
        {
            string[] arrDatas = strLine.Split(',');

            if (arrDatas.Count() == 3)
            {
                int nIndex = arrDatas[0].IndexOf('초');

                if (nIndex >= 0)
                {
                    string strSeconds = arrDatas[0].Substring(0, nIndex);

                    if (int.TryParse(strSeconds, out second) && double.TryParse(arrDatas[1], out x) && double.TryParse(arrDatas[2], out y))
                        return true;
                }
            }

            second = 0;
            x = y = 0.0;
            return false;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (textBoxX.Text.Length == 0)
            {
                MessageBox.Show("X 값을 입력하세요");
                return;
            }

            if (textBoxY.Text.Length == 0)
            {
                MessageBox.Show("Y 값을 입력하세요");
                return;
            }

            if (textBoxPeriodSecond.Text.Length == 0)
            {
                MessageBox.Show("주기를 입력하세요");
                return;
            }

            if (textBoxEventMinute.Text.Length == 0 && textBoxEventSecond.Text.Length == 0)
            {
                MessageBox.Show("이벤트 시간을 입력하세요.");
                return;
            }

            int nPeriodSeconds, min = 0, sec = 0;
            double x, y;

            if (textBoxEventMinute.Text.Length > 0)
            {
                if (!int.TryParse(textBoxEventMinute.Text, out min))
                {
                    MessageBox.Show("이벤트 시간은 0 또는 0 보다 큰 정수 형태의 값이어야 합니다.");
                    return;
                }
                else if (min < 0)
                {
                    MessageBox.Show("이벤트 시간은 0 또는 0 보다 큰 정수 형태의 값이어야 합니다.");
                    return;
                }
            }

            if (textBoxEventSecond.Text.Length > 0)
            {
                if (!int.TryParse(textBoxEventSecond.Text, out sec))
                {
                    MessageBox.Show("이벤트 시간은 0 또는 0 보다 큰 정수 형태의 값이어야 합니다.");
                    return;
                }
                else if (sec < 0)
                {
                    MessageBox.Show("이벤트 시간은 0 또는 0 보다 큰 정수 형태의 값이어야 합니다.");
                    return;
                }
            }

            if (!int.TryParse(textBoxPeriodSecond.Text, out nPeriodSeconds))
            {
                MessageBox.Show("주기는 0보다 큰 정수 형태의 값이어야 합니다.");
                return;
            }
            else if (nPeriodSeconds < 1)
            {
                MessageBox.Show("주기는 0보다 큰 정수 형태의 값이어야 합니다.");
                return;
            }

            if (!double.TryParse(textBoxX.Text, out x))
            {
                MessageBox.Show("X 값은 실수 형태의 값이어야 합니다.");
                return;
            }

            if (!double.TryParse(textBoxY.Text, out y))
            {
                MessageBox.Show("Y 값은 실수 형태의 값이어야 합니다.");
                return;
            }

            int nEventTime = min * 60 + sec;
            SensorEvents.SensorEvent sensorEvent = GetLastEvent();

            if (sensorEvent == null)
            {
                AddLineString(MakeLineString(nEventTime, x, y));
            }
            else
            {
                if (sensorEvent.EventTime == nEventTime)
                {
                    MessageBox.Show("이전 이벤트 시간과 동일합니다.\r\n이벤트 시간을 다시 확인하세요.");
                    return;
                }
                else if (sensorEvent.EventTime > nEventTime)
                {
                    MessageBox.Show("이전 이벤트 시간보다 현재의 이벤트 시간이 더 오래되었습니다.\r\n이벤트 시간을 다시 확인하세요.");
                    return;
                }

                int nTimeSpan = nEventTime - sensorEvent.EventTime;
                int nCount = nTimeSpan / nPeriodSeconds;

                UnE.Geometry.Vertex2D vBegin = new UnE.Geometry.Vertex2D(sensorEvent.X, sensorEvent.Y);
                UnE.Geometry.Vertex2D vEnd = new UnE.Geometry.Vertex2D(x, y);
                double distance = vBegin.GetDistance(vEnd);

                for (int i = 1; i <= nCount; i++)
                {
                    int nAddTime = nPeriodSeconds * i;
                    int time = sensorEvent.EventTime + nAddTime;
                    UnE.Geometry.Vertex2D vertex = UnE.Geometry.Math.GetLinearVertex(vBegin, vEnd, distance * nAddTime / nTimeSpan);

                    AddLineString(MakeLineString(time, vertex.x, vertex.y));
                }

                if (nTimeSpan % nPeriodSeconds > 0)
                {
                    AddLineString(MakeLineString(nEventTime, x, y));
                }
            }
        }

        private bool GetRunningTime(out int min, out int sec)
        {
            min = sec = -1;

            if (textBoxRunningMinute.Text.Length == 0 && textBoxRunningSecond.Text.Length == 0)
            {
                MessageBox.Show("Running Time을 입력해야 합니다.");
                return false;
            }
            else if (textBoxRunningMinute.Text.Length == 0)
            {
                min = 0;

                if (!int.TryParse(textBoxRunningSecond.Text, out sec))
                {
                    MessageBox.Show("Running Time에 정수형태의 숫자가 아닌 값이 들어있습니다.");
                    return false;
                }
            }
            else if (textBoxRunningSecond.Text.Length == 0)
            {
                sec = 0;

                if (!int.TryParse(textBoxRunningMinute.Text, out min))
                {
                    MessageBox.Show("Running Time에 정수형태의 숫자가 아닌 값이 들어있습니다.");
                    return false;
                }
            }
            else
            {
                if (!int.TryParse(textBoxRunningSecond.Text, out sec) ||
                    !int.TryParse(textBoxRunningMinute.Text, out min))
                {
                    MessageBox.Show("Running Time에 정수형태의 숫자가 아닌 값이 들어있습니다.");
                    return false;
                }
            }

            if (min < 0 || sec < 0)
            {
                MessageBox.Show("Running Time은 0보다 큰 정수만 입력 가능합니다.");
                return false;
            }

            return true;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            int min, sec;

            if (!GetRunningTime(out min, out sec))
                return;

            if (!ApplyEvents())
            {
                if (MessageBox.Show("TextBox에 잘못된 텍스트가 포함되어 있습니다.\r\n해당 센서의 모든 데이터가 삭제될 수 있습니다.\r\n계속 진행하시겠습니까?",
                    "경고", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No)
                    return;
            }

            m_dataMgr.CalcByEvent();
            m_dataMgr.RepeatCount = cboRepeat.SelectedIndex == 0 ? -1 : cboRepeat.SelectedIndex;
            m_dataMgr.RunningTime = min * 60 + sec;

            if (m_xmlMgr.SaveXML(m_xmlMgr.FilePath, m_dataMgr))
                MessageBox.Show("데이터 파일이 저장되었습니다.");
            else
                MessageBox.Show("데이터 파일 저장에 실패하였습니다.");
        }
    }
}

namespace HSMS
{
    public class SensorWorker
    {
    }

    public class SensorVehicle
    {
    }
}