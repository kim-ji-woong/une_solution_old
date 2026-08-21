using KpxPipeMonitoring.ChildForms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Collections;
using DBUtility; 

namespace KpxPipeMonitoring
{
    public enum AlarmType
    {
        황산누출 = -1,
        탱크온도상승 = 1,
        탱크온도하강 = 2,
        탱크최고레벨 = 4,
        //탱크유량증가 = 8,
        //탱크유량감소 = 16,

        압력상승 = 256,
        압력하강 = 512,
        유량증가 = 1024,
        유량감소 = 2048
    }

    public class CommonFunction
    {   
        public void SettingGridView(DataGridView gridView, string columnsName, string headerText, Color colHeaderBackground, int columnsWidth = 0, int ColumnHeadersHeight = 40)
        {
            gridView.Columns.Add(columnsName, headerText);
            if (columnsWidth != 0)
            {
                gridView.Columns[columnsName].Width = columnsWidth;
                gridView.Columns[columnsName].MinimumWidth = columnsWidth; 
            }
            gridView.Columns[columnsName].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            gridView.Columns[columnsName].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            gridView.RowHeadersVisible = false;
            gridView.AllowUserToAddRows = false; 
            gridView.ReadOnly = true;
            gridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            gridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; 
            gridView.BackgroundColor = Color.White;
            gridView.ColumnHeadersDefaultCellStyle.BackColor = colHeaderBackground;
            gridView.EnableHeadersVisualStyles = false;
            gridView.Columns[columnsName].SortMode = DataGridViewColumnSortMode.NotSortable; 
            gridView.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            gridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            gridView.RowTemplate.Height = gridView.ColumnHeadersHeight = ColumnHeadersHeight;
            gridView.MultiSelect = false; 
        }

        ToolTip tooltip = new ToolTip();
        public void SettingButton(PictureBox btn, Image normalImg, Image clickImg, string tooltipText = "")
        {
            btn.Image = normalImg;
           
            btn.MouseClick += (s, e) =>
                {
                    if (e.Button == MouseButtons.Right) return;
                };
            btn.MouseEnter += (s, e) =>
                {                    
                    btn.Image = clickImg;
                };
            btn.MouseLeave += (s, e) =>
                {
                    btn.Image = normalImg;
                };

            if (tooltipText.Length > 0)
            {
                btn.MouseHover += (s, e) =>
                    {
                        tooltip.SetToolTip(btn, tooltipText);
                    }; 
            } 
        } 

        public int GetChartPointCount(DateTime beforeDate, DateTime afterDate)
        { 
            TimeSpan ts = afterDate - beforeDate;
            double totalMinutes = ts.TotalMinutes;

            int displayConditionCnt = 0;
            if (totalMinutes <= 1) return 1;
            else if (totalMinutes <= 30) displayConditionCnt = 30;
            else if (totalMinutes <= 60) displayConditionCnt = 60;
            else if (totalMinutes <= 180) displayConditionCnt = 180;
            else if (totalMinutes <= 300) displayConditionCnt = 300; 
            else displayConditionCnt = Convert.ToInt32(totalMinutes / 2);

            return displayConditionCnt / 10;
        }

        public DateTimeIntervalType GetIntervalType(DateTime beforeDate, DateTime afterDate)
        {
            DateTimeIntervalType intervalType = DateTimeIntervalType.Auto;
            intervalType = DateTimeIntervalType.Auto;

            TimeSpan ts = afterDate - beforeDate;            
            double totalSeconds = ts.TotalSeconds;
            if (totalSeconds <= 120) // 2분 이하 
                intervalType = DateTimeIntervalType.Seconds;
            else if (totalSeconds <= 3600) // 60분 이하 
                intervalType = DateTimeIntervalType.Minutes;
            else if (totalSeconds <= 86400) // 하루 이하 
                intervalType = DateTimeIntervalType.Minutes;
            else if (totalSeconds <= 259200) // 3일
                intervalType = DateTimeIntervalType.Hours;
            //else if (totalSeconds <= 604800) // 일주일
            //    intervalType = DateTimeIntervalType.Hours;
            //else if (totalSeconds <= 1209600) // 이주일
            //    intervalType = DateTimeIntervalType.Hours;
            else
                intervalType = DateTimeIntervalType.Days;

            return intervalType;
        }

        public double GetInterval(DateTimeIntervalType intervalType, DateTime beforeDate, DateTime afterDate)
        {
            double interval = 1;
            TimeSpan ts = afterDate - beforeDate;            
            double totalSeconds = ts.TotalSeconds;

            if (intervalType == DateTimeIntervalType.Seconds)
            {
                if (totalSeconds <= 10)
                    interval = 1;
                else if (totalSeconds <= 30)
                    interval = 5;
                else
                    interval = 10;
            }
            else if (intervalType == DateTimeIntervalType.Minutes)
            {
                if (totalSeconds <= 900)
                    interval = 1;
                else if (totalSeconds <= 1800)
                    interval = 2.5; 
                else if (totalSeconds <= 3600)
                    interval = 5;
                else if (totalSeconds <= 10800)
                    interval = 15;
                else if (totalSeconds <= 21600)
                    interval = 30;
                else if (totalSeconds <= 43200)
                    interval = 45;
                else if (totalSeconds <= 86400)
                    interval = 60;
            }                 
            else if (intervalType == DateTimeIntervalType.Hours)
                interval = 3;
            else if (intervalType == DateTimeIntervalType.Days)
                interval = 1;

            return interval;
        }

        public System.Windows.Forms.DataVisualization.Charting.StripLine GetStripLine(double intervalOffset, StringAlignment textLineAlignment, string textType)
        {
            System.Windows.Forms.DataVisualization.Charting.StripLine stripLine = new System.Windows.Forms.DataVisualization.Charting.StripLine();            
            stripLine.BorderDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dash;
            stripLine.BorderWidth = 1;
            stripLine.TextAlignment = StringAlignment.Far;
            stripLine.TextLineAlignment = textLineAlignment;
            stripLine.IntervalOffset = intervalOffset;
            stripLine.Text = textType + " " + String.Format("{0:F1}", intervalOffset);
            stripLine.BorderColor = Color.FromArgb(147, 188, 228);
            stripLine.ForeColor = Color.FromArgb(147, 188, 228);

            return stripLine;
        }
        public System.Windows.Forms.DataVisualization.Charting.StripLine GetStripLineFlow(double intervalOffset, StringAlignment textLineAlignment, string textType)
        {
            System.Windows.Forms.DataVisualization.Charting.StripLine stripLine = new System.Windows.Forms.DataVisualization.Charting.StripLine();
            
            stripLine.BorderDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dash;
            stripLine.BorderWidth = 1;
            stripLine.TextAlignment = StringAlignment.Far;
            stripLine.TextLineAlignment = textLineAlignment;
            stripLine.IntervalOffset = intervalOffset;
            stripLine.Text = textType + " " + String.Format("{0:F1}", intervalOffset);
            stripLine.BorderColor = Color.FromArgb(248, 149, 34);
            stripLine.ForeColor = Color.FromArgb(248, 149, 34);

            return stripLine;
        }

        public class PipeInfo
        {
            public int nPipeID { get; set; }
            public string strPipeName { get; set; } 
            public string strPipeType { get; set; }
            public double nStandardPressure { get; set; }
            public double nStandardFlow { get; set; }
            public int nConnectTankID { get; set; } // 작업중일때 연결된 탱크 ID 
            public double nPressure { get; set; }

            public PipeInfo(int pipeId, string pipeName, string pipeType, double standardPressure, double standardFlow, int connectTankId, double pressure)
            {
                this.nPipeID = pipeId;
                this.strPipeName = pipeName; 
                this.strPipeType = pipeType;
                this.nStandardPressure = standardPressure;
                this.nStandardFlow = standardFlow;
                this.nConnectTankID = connectTankId;
                this.nPressure = pressure;
            }
        } 

        public class ChartField 
        {
            public int nPipeID { get; set; }
            public int nTankID { get; set; }
            public DateTime dtTimeStamp { get; set; } 
            public double dPressure { get; set; }
            public double dFlow { get; set; }

            public ChartField(int pipeId, int tankId, DateTime timeStamp, double pressure, double flow = 0)
            {
                this.nPipeID = pipeId;
                this.nTankID = tankId; 
                this.dtTimeStamp = timeStamp; 
                this.dPressure = pressure;
                this.dFlow = flow;
            } 
        } 

        public class WorkListField
        {
            public int nPipeID { get; set; }
            public int nTankID { get; set; }
            public string strPipeName { get; set; }
            public string strTankName { get; set; }
            public double dBeginTime { get; set; }
            public double dEndTime { get; set; } 

            public WorkListField(int pipeId, int tankId, string pipeName, string tankName, double beginTime, double endTime)
            {
                this.nPipeID = pipeId;
                this.nTankID = tankId;
                this.strPipeName = pipeName;
                this.strTankName = tankName;
                this.dBeginTime = beginTime;
                this.dEndTime = endTime;
            }
        }

        public class TankInfo
        {
            public int nTankID { get; set; }
            public string strTankName { get; set; }
            public string strLiquidType { get; set; }
            public double nDensity { get; set; }
            public double nTemp { get; set; }
            public double nMass { get; set; }
            public double nCurLevel { get; set; }
            public double nFlow { get; set; }
            public string strType { get; set; }
            public double nHighLevel { get; set; }
            public double nCapacity { get; set; }
            public double nMinTemp { get; set; }
            public double nMaxTemp { get; set; }
            public double nOrgHighLevel { get; set; }
            public double nOrgMinTemp { get; set; }
            public double nOrgMaxTemp { get; set; }
            public bool bIsWork { get; set; }
            public List<int> nConnectPipeIDs { get; set; } //bIsWork가 false면 -1
            public List<string> strConnectPipeNames { get; set; } //bIsWork가 false면 null
            public bool bDisconnected { get; set; } //배관과 연결 가능한 탱크인지
            public double nLeakLevel { get; set; } // 누수 레벨
            public int nLeakTime { get; set; } // 누수 시간
            public double nStandardFlow { get; set; }
            public bool bIsLeakStatus { get; set; }
            public bool bIsLeakMonitoring { get; set; }

            public TankInfo(int tankId, string tankName, string liquidType, double density, double temp, double mass, double curLevel, double flow, string type
                , double highLevel, double capacity, double minTemp, double maxTemp, double orgHighLevel, double orgMinTemp, double orgMaxTemp, bool isWork
                , List<int> connectPipeID, List<string> connectPipeName, bool disconnected, double leakLevel, int leakTime, double standardFlow
                , bool isLeakStatus, bool isLeakMonitoring)
            {
                this.nTankID = tankId;
                this.strTankName = tankName;
                this.strLiquidType = liquidType;
                this.nDensity = density;
                this.nTemp = temp;
                this.nMass = mass;
                this.nCurLevel = curLevel;
                this.nFlow = flow;
                this.strType = type;
                this.nHighLevel = highLevel;
                this.nCapacity = capacity;
                this.nMinTemp = minTemp;
                this.nMaxTemp = maxTemp;
                this.nOrgHighLevel = orgHighLevel;
                this.nOrgMinTemp = orgMinTemp;
                this.nOrgMaxTemp = orgMaxTemp;
                this.bIsWork = isWork;
                this.nConnectPipeIDs = connectPipeID;
                this.strConnectPipeNames = connectPipeName;
                this.bDisconnected = disconnected;
                this.nLeakLevel = leakLevel;
                this.nLeakTime = leakTime;
                this.nStandardFlow = standardFlow;
                this.bIsLeakStatus = isLeakStatus;
                this.bIsLeakMonitoring = isLeakMonitoring;
            }
        } 

        public class PipeAlarm
        {
            public bool IsAlarm { get; set; }
            public int nStatus { get; set; }
            public int nAlarmHistoryID { get; set; } 


            public PipeAlarm(bool isAlarm, int status, int alarmHistoryID)
            {
                this.IsAlarm = isAlarm;
                this.nStatus = status;
                this.nAlarmHistoryID = alarmHistoryID; 
            }
        }

        public class TankAlarm
        {
            public bool IsAlarm { get; set; }
            public int nLevelStatus { get; set; }
            public int nLevelAlarmHistoryID { get; set; }
            public int nTempStatus { get; set; }
            public int nTempAlarmHistoryID { get; set; }


            public TankAlarm(bool isAlarm, int levelStatus, int levelAlarmHistoryID, int tempStatus, int tempAlarmHistoryID)
            {
                this.IsAlarm = isAlarm;
                this.nLevelStatus = levelStatus;
                this.nTempStatus = tempStatus;
                this.nLevelAlarmHistoryID = levelAlarmHistoryID; 
                this.nTempAlarmHistoryID = tempAlarmHistoryID; 
            }
        }

        public class AllAlarm
        { 
            public int nTankID { get; set; }
            public int nPipeID { get; set; }
            public int nAlarmHistoryID { get; set; }
            public DateTime dtBeginTime { get; set; }
            public int nAlarmType { get; set; }
            public string strAlarmDescription { get; set; }
            public string strAlarmTerminator { get; set; }
            public double nStandardValue { get; set; }
            public double nStandardRange { get; set; }
            public double nRealValue { get; set; }
            public int nAlarmOccurType { get; set; }
            public string strAlarmComment { get; set; }

            public AllAlarm(int tankId, int pipeId, int alarmHistoryId, DateTime beginTime, int alarmType, string alarmDescription, string alarmTerminator, double standardValue, double standardRange, double realValue, int alarmOccurType, string alarmComment)
            {
                this.nTankID = tankId;
                this.nPipeID = pipeId;
                this.nAlarmHistoryID = alarmHistoryId;
                this.dtBeginTime = beginTime;
                this.nAlarmType = alarmType;
                this.strAlarmDescription = alarmDescription;
                this.strAlarmTerminator = alarmTerminator;
                this.nStandardValue = standardValue;
                this.nStandardRange = standardRange;
                this.nRealValue = realValue;
                this.nAlarmOccurType = alarmOccurType;
                this.strAlarmComment = alarmComment;
            }
        }

        public class AlarmTankOptionInfo
        {
            public int nTankID { get; set; }
            public string strTankName { get; set; }

            public int nStableBeginWorkM { get; set; }
            public int nAlarmInterval { get; set; }
            public int nAlarmIntervalUse { get; set; }
             
            public double nTankStableRatio { get; set; }
            public double nTankStableAbsolute { get; set; }
            public int nTankStableType { get; set; } 
            public int nTankStableCTime { get; set; }
            public int nTankStableCTimeUse { get; set; }

            public AlarmTankOptionInfo(int tankId, string tankName, int stableBeingWorkM, int alarmInterval, int alarmIntervalUse 
                , double tankStableRatio, double tankStableAbsolute, int tankStableType, int tankStableCTime, int tankStableCTimeUse)
            {
                this.nTankID = tankId;
                this.strTankName = tankName;

                this.nStableBeginWorkM = stableBeingWorkM;
                this.nAlarmInterval = alarmInterval;
                this.nAlarmIntervalUse = alarmIntervalUse; 
                 
                this.nTankStableRatio = tankStableRatio;
                this.nTankStableAbsolute = tankStableAbsolute;
                this.nTankStableType = tankStableType; 
                this.nTankStableCTime = tankStableCTime;
                this.nTankStableCTimeUse = tankStableCTimeUse; 
            }
        }

        public class AlarmPipeOptionInfo
        {
            public int nPipeID { get; set; }
            public string strPipeName { get; set; }
             
            public double nPipeStableRatio { get; set; }
            public double nPipeStableAbsolute { get; set; }
            public int nPipeStableType { get; set; }
            public int nPipeStableCTime { get; set; }
            public int nPipeStableCTimeUse { get; set; }
             
            public AlarmPipeOptionInfo(int pipeId, string pipeName
                , double pipeStableRatio, double pipeStableAbsolute, int pipeStableType, int pipeStableCTime, int pipeStableCTimeUse)
            {
                this.nPipeID = pipeId;
                this.strPipeName = pipeName;
                 
                this.nPipeStableRatio = pipeStableRatio;
                this.nPipeStableAbsolute = pipeStableAbsolute;
                this.nPipeStableType = pipeStableType;
                this.nPipeStableCTime = pipeStableCTime;
                this.nPipeStableCTimeUse = pipeStableCTimeUse; 
            }
        } 
         
        /// <summary>
        /// 숫자 Text가 소숫점 이하를 표현하고 있을 경우, 마지막이 0으로 끝나게 되면 0을 없앤다. 
        /// </summary> 
        public string removeTailZero(string number)
        {
            if (number.Contains(".") == false)
                return number;

            while (number.EndsWith("0"))
            {
                number = number.Substring(0, number.Length - 1);
            }

            if (number.EndsWith("."))
                number = number.Substring(0, number.Length - 1);

            return number;
        }

        public double removeDigit(string number, int digit)
        {
            if (!number.Contains("."))
                return Convert.ToDouble(number);

            int pointIndex = number.IndexOf(".") + 1;
            return Convert.ToDouble(number.Substring(0, pointIndex + digit));
        }

        public int GetMaxTableID(string strTableName)
        {
            string strSQL = "Select max(ID) from " + strTableName;
            ArrayList arrResult = MainForm.Instance.dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count == 0)
                return 0;

            VariousData<int> id = WebDBManager.GetIntField(arrResult[0].ToString());
            return id == null ? 0 : id.Data;
        }

        public DateTime GetDateTimeNow()
        {
            ArrayList arrList = MainForm.Instance.dbMgr.GetResultData("select now()", 0);
            if (arrList == null || arrList.Count == 0) 
                return DateTime.Now;

            DateTime dbTime = DateTime.Now;
            dbTime = Convert.ToDateTime(arrList[0].ToString()); 

            return dbTime;
        }

        /// <summary>
        /// 모든 탱크에 연결된 배관ID를 가져옴
        /// </summary>
        /// <param name="?"></param>
        /// <returns></returns>
        public Dictionary<int, List<int>> ReturnConnectPipeIDs()
        {
            Dictionary<int, List<int>> connectPipeIDs = new Dictionary<int, List<int>>();

            string strSQL = "SELECT TankId, ifnull(PipeID, ifnull(AnotherLink, -1)) as PipeID FROM LastWorkHistory WHERE EndTime IS NULL ORDER BY BeginTime";
            ArrayList arrResult = MainForm.Instance.dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count == 0)
            {  
                return connectPipeIDs;
            }

            for (int i = 0; i < arrResult.Count; i += 2)
            {
                int nTankID = DBUtility.WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                int nPipeID = DBUtility.WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);

                if (connectPipeIDs.ContainsKey(nTankID))
                    connectPipeIDs[nTankID].Add(nPipeID);
                else
                {
                    connectPipeIDs.Add(nTankID, new List<int>());
                    connectPipeIDs[nTankID].Add(nPipeID);
                }
            }
            return connectPipeIDs;
        }

        /// <summary>
        /// 특정 탱크에 연결된 배관ID를 가져옴
        /// </summary>
        /// <param name="tankId">TankID</param>
        /// <returns>PipeIds</returns>
        public List<int> ReturnConnectPipeIDs(int tankId)
        {
            List<int> connectPipeIDs = new List<int>();

            string strSQL = "SELECT ifnull(PipeID, ifnull(AnotherLink, -1)) as PipeID FROM LastWorkHistory WHERE EndTime IS NULL AND TankID = " + tankId + " ORDER BY BeginTime";
            ArrayList arrResult = MainForm.Instance.dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count == 0)
            {
                connectPipeIDs.Add(-1);
                return connectPipeIDs;
            }

            for (int i = 0; i < arrResult.Count; i++)
            {
                int nPipeID = DBUtility.WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                connectPipeIDs.Add(nPipeID);
            } 
            return connectPipeIDs;
        }

        /// <summary>
        /// 배관에 연결된 탱크ID를 가져옴
        /// </summary>
        /// <param name="tankId">PipeID</param>
        /// <returns>TankIds</returns>
        public int ReturnConnectTankIDs(int pipeId)
        {
            int connectTankID = -1;

            string strSQL = "SELECT TankID FROM LastWorkHistory WHERE EndTime IS NULL AND PipeID = " + pipeId + " ORDER BY BeginTime";
            ArrayList arrResult = MainForm.Instance.dbMgr.GetResultData(strSQL, 0);
            if (arrResult == null) return -1; 

            for (int i = 0; i < arrResult.Count; i++)
            {
                connectTankID = DBUtility.WebDBManager.GetIntField(arrResult[i].ToString(), -1); 
            }

            return connectTankID;
        }
    }
}
