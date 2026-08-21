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

namespace JubixSensorSimulator
{
    public partial class FormMain : Form
    {
        private class AlarmInfo
        {
            private string m_strAlarm1 = "";
            private string m_strAlarm2 = "";
            private string m_strAlarm3 = "";

            // 1단계 알람 임계치
            public string Alarm1
            {
                get { return m_strAlarm1; }
                set { m_strAlarm1 = value; }
            }

            // 2단계 알람 임계치
            public string Alarm2
            {
                get { return m_strAlarm2; }
                set { m_strAlarm2 = value; }
            }

            // 3단계 알람 임계치
            public string Alarm3
            {
                get { return m_strAlarm3; }
                set { m_strAlarm3 = value; }
            }
        }

        private WebDBManager m_dbJubix = null;
        private WebDBManager m_dbSOP = null;
        private bool m_systemInput = false;

        private const int ID_Index = 0;
        private const int SSID_Index = 1;
        private const int LOCATION_Index = 2;
        private const int DENSITY_Index = 3;
        private const int STATE_Index = 4;

        public FormMain()
        {
            InitializeComponent();
            InitDB();
        }

        private void InitDB()
        {
            Utility ini = new Utility();

            string strSection = "Jubix Connection Info";
            string strServerIP = ini.getinivalue(strSection, "server_ip");
            string strServerPort = ini.getinivalue(strSection, "server_port");
            string strServerDB = ini.getinivalue(strSection, "server_db");

            int nSiteID;
            string strSiteID = ini.getinivalue("Server Connection Info", "siteid");

            if (int.TryParse(strSiteID, out nSiteID) == false)
                return;

            m_dbJubix = new WebDBManager(nSiteID);

            m_dbJubix.DatabaseHost = strServerIP;
            m_dbJubix.WebServerURL = "http://127.0.0.1:8080/JUBIX";
            m_dbJubix.DatabaseType = WebDBManager.DBType.mysql;
            m_dbJubix.DatabaseName = strServerDB;
            m_dbJubix.DatabasePort = strServerPort;

            m_dbSOP = new WebDBManager(nSiteID);
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            LoadPSMSensors();
        }

        private void LoadPSMSensors()
        {
            string strSQL = "SELECT ID, SensorName FROM SensorTagInfo";
            ArrayList arrResult = m_dbSOP.GetResultData(strSQL, 0);

            if (arrResult == null)
                return;

            Dictionary<string, int> dicSensorTags = new Dictionary<string, int>();
            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 1;i+=2 )
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                string strSensorName = WebDBManager.GetStringField(arrResult[i + 1]);

                if (id == null || strSensorName == null)
                    continue;

                dicSensorTags[strSensorName] = id.Data;
            }

            // ss_Stat가 '00'이면 센서상태 정상. '01'이면 비가동
            strSQL = "SELECT ss_ID, ss_Pst_Nm, ss_Cur_Value, ss_Cur_Stat, ss_Alrm_1St, ss_Alrm_2nd, ss_Alrm_3rd FROM c_ss_info where ss_Stat = '00'";
            arrResult = m_dbJubix.GetResultData(strSQL, 0);

            if (arrResult == null)
                return;

            int nID;
            nResultCount = arrResult.Count;
            m_systemInput = true;

            for (int i=0;i<nResultCount-6;i+=7)
            {
                string strSSID = WebDBManager.GetStringField(arrResult[i]);
                string strLocation = WebDBManager.GetStringField(arrResult[i + 1]);
                string strCurrentValue = WebDBManager.GetStringField(arrResult[i + 2]);
                string strCurrentState = WebDBManager.GetStringField(arrResult[i + 3]);
                string strAlarm1 = WebDBManager.GetStringField(arrResult[i + 4], "");
                string strAlarm2 = WebDBManager.GetStringField(arrResult[i + 5], "");
                string strAlarm3 = WebDBManager.GetStringField(arrResult[i + 6], "");

                if (strSSID == null || strLocation == null)
                    continue;

                if (dicSensorTags.TryGetValue(strSSID, out nID) == false)
                    continue;

                int nRowIndex = gridSensor.Rows.Add();

                if (nRowIndex < 0)
                    continue;

                DataGridViewRow row = gridSensor.Rows[nRowIndex];

                row.Cells[ID_Index].Value = nID;
                row.Cells[SSID_Index].Value = strSSID;
                row.Cells[LOCATION_Index].Value = strLocation;
                row.Cells[DENSITY_Index].Value = strCurrentValue;
                row.Cells[STATE_Index].Value = GetSensorState(strCurrentState);
                row.Cells[STATE_Index].Tag = row.Cells[STATE_Index].Value;

                row.Cells[DENSITY_Index].Tag = -1.0;

                if (strCurrentValue != null)
                {
                    double density;

                    if (double.TryParse(strCurrentValue, out density))
                    {
                        row.Cells[DENSITY_Index].Tag = density;
                    }
                }

                AlarmInfo alarm = new AlarmInfo();
                alarm.Alarm1 = strAlarm1;
                alarm.Alarm2 = strAlarm2;
                alarm.Alarm3 = strAlarm3;

                row.Tag = alarm;
            }

            gridSensor.Sort(gridSensor.Columns[ID_Index], ListSortDirection.Ascending);
            m_systemInput = false;

            gridSensor_SelectionChanged(null, null);
        }

        private string GetSensorState(string strState)
        {
            if (strState == "00")
                return "정상";
            else if (strState == "01")
                return "Alarm1";
            else if (strState == "02")
                return "Alarm2";
            else if (strState == "03")
                return "Alarm3";
            else if (strState == "10")
                return "알람";
            else if (strState == "11")
                return "알람요청";
            else if (strState == "12")
                return "중지요청";
            else if (strState == "20")
                return "CCTV컷";
            else if (strState == "21")
                return "CCTV컷 요청";
            else if (strState == "99")
                return "실패";

            return "알수없음";
        }

        private string GetSensorStateTag(string strState)
        {
            if (strState == "정상")
                return "00";
            else if (strState == "Alarm1")
                return "01";
            else if (strState == "Alarm2")
                return "02";
            else if (strState == "Alarm3")
                return "03";
            else if (strState == "알람")
                return "10";
            else if (strState == "알람요청")
                return "01";
            else if (strState == "중지요청")
                return "12";
            else if (strState == "CCTV컷")
                return "20";
            else if (strState == "CCTV컷 요청")
                return "21";
            else if (strState == "실패")
                return "99";

            return "99";
        }

        private void gridSensor_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (m_systemInput)
                return;

            if (e.RowIndex < 0)
                return;

            if (e.ColumnIndex == STATE_Index)
            {
                DataGridViewCell cell = gridSensor.Rows[e.RowIndex].Cells[e.ColumnIndex];

                if (cell.Tag == null)
                    return;

                string strValue = cell.Value.ToString();

                if (strValue.EndsWith("요청") == false)
                {
                    MessageBox.Show(strValue + "는 선택할 수 없습니다.");
                    m_systemInput = true;
                    cell.Value = cell.Tag;
                    m_systemInput = false;
                    return;
                }

                if (cell.Tag != cell.Value)
                    gridSensor.Rows[e.RowIndex].Cells[0].Style.BackColor = Color.Red;
                else
                {
                    if (IsSameDensity(e.RowIndex))
                        gridSensor.Rows[e.RowIndex].Cells[0].Style.BackColor = gridSensor.Rows[e.RowIndex].Cells[1].Style.BackColor;
                    else
                        gridSensor.Rows[e.RowIndex].Cells[0].Style.BackColor = Color.Red;
                }

                CheckChange(STATE_Index);
            }
            else if (e.ColumnIndex == DENSITY_Index)
            {
                DataGridViewCell cell = gridSensor.Rows[e.RowIndex].Cells[e.ColumnIndex];

                if (cell.Tag == null)
                    return;

                VariousData<double> density = new VariousData<double>();
                bool sameDensity = IsSameDensity(e.RowIndex, density);

                if (density.Data == 1111111.1111)
                {
                    MessageBox.Show("숫자만 입력 가능합니다.");
                    m_systemInput = true;
                    cell.Value = cell.Tag;
                    m_systemInput = false;
                    return;
                }

                if (density.Data < 0.0)
                {
                    MessageBox.Show("0 또는 그 이상의 숫자만 입력 가능합니다.");
                    m_systemInput = true;
                    cell.Value = cell.Tag;
                    m_systemInput = false;
                    return;
                }

                if (sameDensity == false)
                    gridSensor.Rows[e.RowIndex].Cells[0].Style.BackColor = Color.Red;
                else
                {
                    if (gridSensor.Rows[e.RowIndex].Cells[STATE_Index].Tag == gridSensor.Rows[e.RowIndex].Cells[STATE_Index].Value)
                        gridSensor.Rows[e.RowIndex].Cells[0].Style.BackColor = gridSensor.Rows[e.RowIndex].Cells[1].Style.BackColor;
                    else
                        gridSensor.Rows[e.RowIndex].Cells[0].Style.BackColor = Color.Red;
                }

                CheckChange(DENSITY_Index);
            }
        }

        private bool IsSameDensity(int nRowIndex, VariousData<double> density = null)
        {
            DataGridViewCell cell = gridSensor.Rows[nRowIndex].Cells[DENSITY_Index];

            if (cell.Tag == null)
                return true;

            double density2;
            string strValue = cell.Value.ToString();

            if (double.TryParse(strValue, out density2))
            {
                if (density != null)
                    density.Data = density2;

                if ((double)cell.Tag == density2)
                    return true;
                else
                    return false;
            }
            else
            {
                if (density != null)
                    density.Data = 1111111.1111;
            }

            return true;
        }

        private void CheckChange(int nColumnIndex)
        {
            foreach (DataGridViewRow row in gridSensor.Rows)
            {
                if (nColumnIndex == STATE_Index)
                {
                    if (row.Cells[STATE_Index].Value != row.Cells[STATE_Index].Tag)
                    {
                        btnApply.Enabled = true;
                        return;
                    }
                }
                else if (nColumnIndex == DENSITY_Index)
                {
                    double density;

                    if (row.Cells[DENSITY_Index].Tag != null && row.Cells[DENSITY_Index].Value != null)
                    {
                        if (double.TryParse(row.Cells[DENSITY_Index].Value.ToString(), out density))
                        {
                            if ((double)row.Cells[DENSITY_Index].Tag != density)
                            {
                                btnApply.Enabled = true;
                                return;
                            }
                        }
                    }
                }
            }

            btnApply.Enabled = false;
        }

        private void gridSensor_SelectionChanged(object sender, EventArgs e)
        {
            if (m_systemInput)
                return;

            if (gridSensor.SelectedCells.Count == 0)
            {
                labelAlarm1.Text = labelAlarm2.Text = labelAlarm3.Text = "";
            }
            else
            {
                AlarmInfo alarm = (AlarmInfo)gridSensor.Rows[gridSensor.SelectedCells[0].RowIndex].Tag;

                labelAlarm1.Text = alarm.Alarm1;
                labelAlarm2.Text = alarm.Alarm2;
                labelAlarm3.Text = alarm.Alarm3;
            }
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            btnApply.Enabled = false;

            bool success = true;

            foreach (DataGridViewRow row in gridSensor.Rows)
            {
                if (row.Cells[0].Style.BackColor == Color.Red)
                {
                    string strSQL = "Update c_ss_info set ss_Cur_Stat = '" + GetSensorStateTag(row.Cells[STATE_Index].Value.ToString()) + "', ss_Cur_Value = ";

                    if (row.Cells[DENSITY_Index].Value == null)
                        strSQL += "NULL";
                    else
                        strSQL += "' " + row.Cells[DENSITY_Index].Value.ToString() + "'";

                    strSQL += " where ss_ID = '" + row.Cells[SSID_Index].Value.ToString() + "'";

                    if (m_dbJubix.GetResultData(strSQL, 0) != null)
                    {
                        row.Cells[STATE_Index].Tag = row.Cells[STATE_Index].Value;

                        if (row.Cells[DENSITY_Index].Value == null)
                            row.Cells[DENSITY_Index].Tag = -1.0;
                        else
                        {
                            double density;

                            if (double.TryParse(row.Cells[DENSITY_Index].Value.ToString(), out density))
                                row.Cells[DENSITY_Index].Tag = density;
                            else
                                row.Cells[DENSITY_Index].Tag = -1.0;
                        }

                        row.Cells[0].Style.BackColor = row.Cells[1].Style.BackColor;
                    }
                    else
                        success = false;
                }
            }

            if (success)
                btnApply.Enabled = false;
            else
                btnApply.Enabled = true;
        }
    }
}
