using DBUtility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Text;
using KpxPipeMonitoring.Popups;

namespace KpxPipeMonitoring.ChildForms
{
    public partial class ChildTank : Form
    { 
        public bool bIsAlarm { get; set; }  
        public delegate void AlarmClearEventArgs();
        public event AlarmClearEventArgs alarmClearEventArgs;
         
        public int nTankID { get; set; }
        public List<int> nConnectWorkPipeIDs { get; set; }
        public bool bIsWork { get; set; } 

        public List<CommonFunction.AllAlarm> oldAlarmList = new List<CommonFunction.AllAlarm>();

        public ChildTank(int tankID)
        {
            this.DoubleBuffered = true;
            InitializeComponent();
             
            MainForm.Instance.SetDoubleBuffer(panel1, true);

            this.nTankID = tankID;
               
            pictureBox_alarmClear.Cursor = Cursors.Hand;
             
            pictureBox_BeginWork.Visible = true;
            pictureBox_EndWork.Visible = false;
            label_workTime.Visible = false;
            label_workTime.Location = new Point(150, 18);

            pictureBox_alarmPressure.Parent = pictureBox_tank;
            pictureBox_alarmPressure.Location = new Point(21, 34);
             
            label_pipeName.Text = "";

            LoadTankImage();
            this.BackColor = Color.FromArgb(248, 247, 249);
            MainForm.Instance.commonFunction.SettingButton(pictureBox_alarmClear, global::KpxPipeMonitoring.Properties.Resources.AlarmClear, global::KpxPipeMonitoring.Properties.Resources.AlarmClear, "알람해제");
            MainForm.Instance.commonFunction.SettingButton(pictureBox_rangeRefresh, global::KpxPipeMonitoring.Properties.Resources.RangeRefresh3_Normal, global::KpxPipeMonitoring.Properties.Resources.RangeRefresh3_Click, "정상범위 새로고침");
        } 
         
        public void SetAlarm(bool isAlarm, bool isChgAlarm, List<CommonFunction.AllAlarm> newAlarmList, bool isWork)
        {
            try
            {
                foreach (CommonFunction.TankInfo item in MainForm.Instance.tankInfo)
                {
                    if (this.nTankID == item.nTankID)
                    {
                        if (isWork)
                        {
                            label_tankName.Text = "TK-" + item.strTankName + item.strType;
                            label_pipeName.Text = string.Join("\r\n", item.strConnectPipeNames);
                            pictureBox_rangeRefresh.Visible = true;
                            label_workTime.Visible = true;
                        }
                        else
                        {
                            label_tankName.Text = "TK-" + item.strTankName + item.strType;
                            label_pipeName.Text = "";
                            pictureBox_rangeRefresh.Visible = false;
                            label_workTime.Visible = false;
                        }
                        break;
                    }
                }

                // 상황 종류
                // - 1. 작업o, 알람x 
                // - 2. 작업o, 알람o
                // - 3. 작업x, 알람o
                // - 4. 작업x, 알람x
                // 작업이 새로 시작되거나 종료된 경우, 알람이 생기거나 해제된 경우, 알람 내용이 변경된 경우
                if (isWork != this.bIsWork || isAlarm != this.bIsAlarm || isChgAlarm)
                {
                    List<int> nsumAlarmType = new List<int>();
                    foreach (CommonFunction.AllAlarm item in newAlarmList)
                    {
                        if (item.nTankID != this.nTankID) continue;
                        if (item.nAlarmHistoryID <= 0) continue;

                        if (!nsumAlarmType.Contains(item.nAlarmType))
                            nsumAlarmType.Add(item.nAlarmType);
                    }

                    if (isWork && !isAlarm)
                    {
                        panel1.BackgroundImage = GetTankImage("Tank_Work");
                        pictureBox_alarmClear.Visible = false;
                        pictureBox_alarmPressure.Visible = false;
                    }
                    else if ((isWork && isAlarm) || (!isWork && isAlarm))
                    {
                        if (isWork)
                        {
                            #region 황산
                            if (nsumAlarmType.Count == 1 && nsumAlarmType.Contains((int)AlarmType.황산누출))
                            {
                                // 황산
                                panel1.BackgroundImage = GetTankImage("Tank_Work_Liquid");
                                pictureBox_alarmPressure.Visible = false;
                            }
                            else if (nsumAlarmType.Count == 2 && (nsumAlarmType.Contains((int)AlarmType.탱크온도상승) || nsumAlarmType.Contains((int)AlarmType.탱크온도하강))
                                                              && nsumAlarmType.Contains((int)AlarmType.황산누출))
                            {
                                // 황산, 온도
                                panel1.BackgroundImage = GetTankImage("Tank_Work_LiquidTemp");
                                pictureBox_alarmPressure.Visible = false;
                            }
                            else if (nsumAlarmType.Count == 2 && nsumAlarmType.Contains((int)AlarmType.탱크최고레벨)
                                                              && nsumAlarmType.Contains((int)AlarmType.황산누출))
                            {
                                // 황산, 레벨
                                panel1.BackgroundImage = GetTankImage("Tank_Work_LiquidLevel");
                                pictureBox_alarmPressure.Visible = false;
                            }
                            else if (nsumAlarmType.Count == 2 && (nsumAlarmType.Contains((int)AlarmType.유량증가) || nsumAlarmType.Contains((int)AlarmType.유량감소))
                                                              && nsumAlarmType.Contains((int)AlarmType.황산누출))
                            {
                                // 황산, 유량
                                panel1.BackgroundImage = GetTankImage("Tank_Work_LiquidFlow");
                                pictureBox_alarmPressure.Visible = false;
                            }
                            else if (nsumAlarmType.Count == 2 && (nsumAlarmType.Contains((int)AlarmType.압력상승) || nsumAlarmType.Contains((int)AlarmType.압력하강))
                                                              && nsumAlarmType.Contains((int)AlarmType.황산누출))
                            {
                                // 황산, 압력
                                panel1.BackgroundImage = GetTankImage("Tank_Work_Liquid");
                                pictureBox_alarmPressure.Visible = true;
                            }
                            else if (nsumAlarmType.Count == 3 && (nsumAlarmType.Contains((int)AlarmType.탱크온도상승) || nsumAlarmType.Contains((int)AlarmType.탱크온도하강))
                                                              && nsumAlarmType.Contains((int)AlarmType.탱크최고레벨) && nsumAlarmType.Contains((int)AlarmType.황산누출))
                            {
                                // 황산, 온도, 레벨
                                panel1.BackgroundImage = GetTankImage("Tank_Work_LiquidLevelTemp");
                                pictureBox_alarmPressure.Visible = false;
                            }
                            else if (nsumAlarmType.Count == 3 && (nsumAlarmType.Contains((int)AlarmType.탱크온도상승) || nsumAlarmType.Contains((int)AlarmType.탱크온도하강))
                                                              && (nsumAlarmType.Contains((int)AlarmType.유량증가) || nsumAlarmType.Contains((int)AlarmType.유량감소))
                                                              && nsumAlarmType.Contains((int)AlarmType.황산누출))
                            {
                                // 황산, 온도, 유량
                                panel1.BackgroundImage = GetTankImage("Tank_Work_LiquidTempFlow");
                                pictureBox_alarmPressure.Visible = false;
                            }
                            else if (nsumAlarmType.Count == 3 && (nsumAlarmType.Contains((int)AlarmType.탱크온도상승) || nsumAlarmType.Contains((int)AlarmType.탱크온도하강))
                                                              && (nsumAlarmType.Contains((int)AlarmType.압력상승) || nsumAlarmType.Contains((int)AlarmType.압력하강))
                                                              && nsumAlarmType.Contains((int)AlarmType.황산누출))
                            {
                                // 황산, 온도, 압력
                                panel1.BackgroundImage = GetTankImage("Tank_Work_LiquidTemp");
                                pictureBox_alarmPressure.Visible = true;
                            }

                            else if (nsumAlarmType.Count == 3 && (nsumAlarmType.Contains((int)AlarmType.유량증가) || nsumAlarmType.Contains((int)AlarmType.유량감소))
                                                              && nsumAlarmType.Contains((int)AlarmType.탱크최고레벨) && nsumAlarmType.Contains((int)AlarmType.황산누출))
                            {
                                // 황산, 레벨, 유량
                                panel1.BackgroundImage = GetTankImage("Tank_Work_LiquidLevelFlow");
                                pictureBox_alarmPressure.Visible = false;
                            }
                            else if (nsumAlarmType.Count == 3 && (nsumAlarmType.Contains((int)AlarmType.탱크최고레벨))
                                                              && (nsumAlarmType.Contains((int)AlarmType.압력상승) || nsumAlarmType.Contains((int)AlarmType.압력하강))
                                                              && nsumAlarmType.Contains((int)AlarmType.황산누출))
                            {
                                // 황산, 레벨, 압력
                                panel1.BackgroundImage = GetTankImage("Tank_Work_LiquidLevel");
                                pictureBox_alarmPressure.Visible = true;
                            }
                            else if (nsumAlarmType.Count == 3 && (nsumAlarmType.Contains((int)AlarmType.유량증가) || nsumAlarmType.Contains((int)AlarmType.유량감소))
                                                              && (nsumAlarmType.Contains((int)AlarmType.압력상승) || nsumAlarmType.Contains((int)AlarmType.압력하강))
                                                              && nsumAlarmType.Contains((int)AlarmType.황산누출))
                            {
                                // 황산, 유량, 압력
                                panel1.BackgroundImage = GetTankImage("Tank_Work_LiquidFlow");
                                pictureBox_alarmPressure.Visible = true;
                            }
                            else if (nsumAlarmType.Count == 4 && (nsumAlarmType.Contains((int)AlarmType.유량증가) || nsumAlarmType.Contains((int)AlarmType.유량감소))
                                                              && (nsumAlarmType.Contains((int)AlarmType.탱크온도상승) || nsumAlarmType.Contains((int)AlarmType.탱크온도하강))
                                                              && nsumAlarmType.Contains((int)AlarmType.탱크최고레벨)
                                                              && nsumAlarmType.Contains((int)AlarmType.황산누출))
                            {
                                // 황산, 온도, 레벨, 유량
                                panel1.BackgroundImage = GetTankImage("Tank_Work_LiquidLevelTempFlow");
                                pictureBox_alarmPressure.Visible = false;
                            }
                            else if (nsumAlarmType.Count == 4 && (nsumAlarmType.Contains((int)AlarmType.압력상승) || nsumAlarmType.Contains((int)AlarmType.압력하강))
                                                              && (nsumAlarmType.Contains((int)AlarmType.탱크온도상승) || nsumAlarmType.Contains((int)AlarmType.탱크온도하강))
                                                              && nsumAlarmType.Contains((int)AlarmType.탱크최고레벨)
                                                              && nsumAlarmType.Contains((int)AlarmType.황산누출))
                            {
                                // 황산, 온도, 레벨, 압력
                                panel1.BackgroundImage = GetTankImage("Tank_Work_LiquidLevelTemp");
                                pictureBox_alarmPressure.Visible = true;
                            }
                            else if (nsumAlarmType.Count == 4 && (nsumAlarmType.Contains((int)AlarmType.압력상승) || nsumAlarmType.Contains((int)AlarmType.압력하강))
                                                              && (nsumAlarmType.Contains((int)AlarmType.탱크온도상승) || nsumAlarmType.Contains((int)AlarmType.탱크온도하강))
                                                              && (nsumAlarmType.Contains((int)AlarmType.유량증가) || nsumAlarmType.Contains((int)AlarmType.유량감소))
                                                              && nsumAlarmType.Contains((int)AlarmType.황산누출))
                            {
                                // 황산, 온도, 유량, 압력
                                panel1.BackgroundImage = GetTankImage("Tank_Work_LiquidTempFlow");
                                pictureBox_alarmPressure.Visible = true;
                            }
                            else if (nsumAlarmType.Count == 4 && nsumAlarmType.Contains((int)AlarmType.탱크최고레벨)
                                                              && (nsumAlarmType.Contains((int)AlarmType.압력상승) || nsumAlarmType.Contains((int)AlarmType.압력하강))
                                                              && (nsumAlarmType.Contains((int)AlarmType.유량증가) || nsumAlarmType.Contains((int)AlarmType.유량감소))
                                                              && nsumAlarmType.Contains((int)AlarmType.황산누출))
                            {
                                // 황산, 레벨, 유량, 압력
                                panel1.BackgroundImage = GetTankImage("Tank_Work_LiquidLevelFlow");
                                pictureBox_alarmPressure.Visible = true;
                            }
                            else if (nsumAlarmType.Count == 5 && nsumAlarmType.Contains((int)AlarmType.탱크최고레벨)
                                                              && (nsumAlarmType.Contains((int)AlarmType.탱크온도상승) || nsumAlarmType.Contains((int)AlarmType.탱크온도하강))
                                                              && (nsumAlarmType.Contains((int)AlarmType.압력상승) || nsumAlarmType.Contains((int)AlarmType.압력하강))
                                                              && (nsumAlarmType.Contains((int)AlarmType.유량증가) || nsumAlarmType.Contains((int)AlarmType.유량감소))
                                                              && nsumAlarmType.Contains((int)AlarmType.황산누출))
                            {
                                // 황산, 온도, 레벨, 유량, 압력
                                panel1.BackgroundImage = GetTankImage("Tank_Work_LiquidLevelTempFlow");
                                pictureBox_alarmPressure.Visible = true;
                            }
                            #endregion

                            else if (nsumAlarmType.Count == 1 && (nsumAlarmType.Contains((int)AlarmType.탱크온도상승) || nsumAlarmType.Contains((int)AlarmType.탱크온도하강)))
                            {
                                // 온도
                                panel1.BackgroundImage = GetTankImage("Tank_Work_Temp");
                                pictureBox_alarmPressure.Visible = false;
                            }
                            else if (nsumAlarmType.Count == 1 && nsumAlarmType.Contains((int)AlarmType.탱크최고레벨))
                            {
                                // 레벨
                                panel1.BackgroundImage = GetTankImage("Tank_Work_Level");
                                pictureBox_alarmPressure.Visible = false;
                            }
                            else if (nsumAlarmType.Count == 1 && (nsumAlarmType.Contains((int)AlarmType.유량증가) || nsumAlarmType.Contains((int)AlarmType.유량감소)))
                            {
                                // 유량
                                panel1.BackgroundImage = GetTankImage("Tank_Work_Flow");
                                pictureBox_alarmPressure.Visible = false;
                            }
                            else if (nsumAlarmType.Count == 1 && (nsumAlarmType.Contains((int)AlarmType.압력상승) || nsumAlarmType.Contains((int)AlarmType.압력하강)))
                            {
                                // 압력
                                panel1.BackgroundImage = GetTankImage("Tank_Work");
                                pictureBox_alarmPressure.Visible = true;
                            }
                            else if (nsumAlarmType.Count == 2 && (nsumAlarmType.Contains((int)AlarmType.탱크온도상승) || nsumAlarmType.Contains((int)AlarmType.탱크온도하강))
                                                          && nsumAlarmType.Contains((int)AlarmType.탱크최고레벨))
                            {
                                // 온도, 레벨
                                panel1.BackgroundImage = GetTankImage("Tank_Work_LevelTemp");
                                pictureBox_alarmPressure.Visible = false;
                            }
                            else if (nsumAlarmType.Count == 2 && (nsumAlarmType.Contains((int)AlarmType.탱크온도상승) || nsumAlarmType.Contains((int)AlarmType.탱크온도하강))
                                                         && (nsumAlarmType.Contains((int)AlarmType.유량증가) || nsumAlarmType.Contains((int)AlarmType.유량감소)))
                            {
                                // 온도, 유량
                                panel1.BackgroundImage = GetTankImage("Tank_Work_TempFlow");
                                pictureBox_alarmPressure.Visible = false;
                            }
                            else if (nsumAlarmType.Count == 2 && (nsumAlarmType.Contains((int)AlarmType.탱크온도상승) || nsumAlarmType.Contains((int)AlarmType.탱크온도하강))
                                                         && (nsumAlarmType.Contains((int)AlarmType.압력상승) || nsumAlarmType.Contains((int)AlarmType.압력하강)))
                            {
                                // 온도, 압력
                                panel1.BackgroundImage = GetTankImage("Tank_Work_Temp");
                                pictureBox_alarmPressure.Visible = true;
                            }
                            else if (nsumAlarmType.Count == 2 && (nsumAlarmType.Contains((int)AlarmType.유량증가) || nsumAlarmType.Contains((int)AlarmType.유량감소))
                                                          && nsumAlarmType.Contains((int)AlarmType.탱크최고레벨))
                            {
                                // 레벨, 유량
                                panel1.BackgroundImage = GetTankImage("Tank_Work_LevelFlow");
                                pictureBox_alarmPressure.Visible = false;
                            }
                            else if (nsumAlarmType.Count == 2 && (nsumAlarmType.Contains((int)AlarmType.탱크최고레벨))
                                                 && (nsumAlarmType.Contains((int)AlarmType.압력상승) || nsumAlarmType.Contains((int)AlarmType.압력하강)))
                            {
                                // 레벨, 압력
                                panel1.BackgroundImage = GetTankImage("Tank_Work_Level");
                                pictureBox_alarmPressure.Visible = true;
                            }
                            else if (nsumAlarmType.Count == 2 && (nsumAlarmType.Contains((int)AlarmType.유량증가) || nsumAlarmType.Contains((int)AlarmType.유량감소))
                                                 && (nsumAlarmType.Contains((int)AlarmType.압력상승) || nsumAlarmType.Contains((int)AlarmType.압력하강)))
                            {
                                // 유량, 압력
                                panel1.BackgroundImage = GetTankImage("Tank_Work_Flow");
                                pictureBox_alarmPressure.Visible = true;
                            }
                            else if (nsumAlarmType.Count == 3 && (nsumAlarmType.Contains((int)AlarmType.유량증가) || nsumAlarmType.Contains((int)AlarmType.유량감소))
                                                          && (nsumAlarmType.Contains((int)AlarmType.탱크온도상승) || nsumAlarmType.Contains((int)AlarmType.탱크온도하강))
                                                          && nsumAlarmType.Contains((int)AlarmType.탱크최고레벨))
                            {
                                // 온도, 레벨, 유량
                                panel1.BackgroundImage = GetTankImage("Tank_Work_LevelTempFlow");
                                pictureBox_alarmPressure.Visible = false;
                            }
                            else if (nsumAlarmType.Count == 3 && (nsumAlarmType.Contains((int)AlarmType.압력상승) || nsumAlarmType.Contains((int)AlarmType.압력하강))
                                                          && (nsumAlarmType.Contains((int)AlarmType.탱크온도상승) || nsumAlarmType.Contains((int)AlarmType.탱크온도하강))
                                                          && nsumAlarmType.Contains((int)AlarmType.탱크최고레벨))
                            {
                                // 온도, 레벨, 압력
                                panel1.BackgroundImage = GetTankImage("Tank_Work_LevelTemp");
                                pictureBox_alarmPressure.Visible = true;
                            }
                            else if (nsumAlarmType.Count == 3 && (nsumAlarmType.Contains((int)AlarmType.압력상승) || nsumAlarmType.Contains((int)AlarmType.압력하강))
                                                          && (nsumAlarmType.Contains((int)AlarmType.탱크온도상승) || nsumAlarmType.Contains((int)AlarmType.탱크온도하강))
                                                          && (nsumAlarmType.Contains((int)AlarmType.유량증가) || nsumAlarmType.Contains((int)AlarmType.유량감소)))
                            {
                                // 온도, 유량, 압력
                                panel1.BackgroundImage = GetTankImage("Tank_Work_TempFlow");
                                pictureBox_alarmPressure.Visible = true;
                            }
                            else if (nsumAlarmType.Count == 3 && nsumAlarmType.Contains((int)AlarmType.탱크최고레벨)
                                                          && (nsumAlarmType.Contains((int)AlarmType.압력상승) || nsumAlarmType.Contains((int)AlarmType.압력하강))
                                                          && (nsumAlarmType.Contains((int)AlarmType.유량증가) || nsumAlarmType.Contains((int)AlarmType.유량감소)))
                            {
                                // 레벨, 유량, 압력
                                panel1.BackgroundImage = GetTankImage("Tank_Work_LevelFlow");
                                pictureBox_alarmPressure.Visible = true;
                            }
                            else if (nsumAlarmType.Count == 4 && nsumAlarmType.Contains((int)AlarmType.탱크최고레벨)
                                                              && (nsumAlarmType.Contains((int)AlarmType.탱크온도상승) || nsumAlarmType.Contains((int)AlarmType.탱크온도하강))
                                                              && (nsumAlarmType.Contains((int)AlarmType.압력상승) || nsumAlarmType.Contains((int)AlarmType.압력하강))
                                                              && (nsumAlarmType.Contains((int)AlarmType.유량증가) || nsumAlarmType.Contains((int)AlarmType.유량감소)))
                            {
                                // 온도, 레벨, 유량, 압력
                                panel1.BackgroundImage = GetTankImage("Tank_Work_LevelTempFlow");
                                pictureBox_alarmPressure.Visible = true;
                            }
                        }
                        else
                        {
                            #region 황산
                            if (nsumAlarmType.Count == 1 && nsumAlarmType.Contains((int)AlarmType.황산누출))
                            {
                                // 황산       
                                panel1.BackgroundImage = GetTankImage("Tank_Default_Liquid");
                                pictureBox_alarmPressure.Visible = false;
                            }
                            if (nsumAlarmType.Count == 2 && (nsumAlarmType.Contains((int)AlarmType.탱크온도상승) || nsumAlarmType.Contains((int)AlarmType.탱크온도하강))
                                                                         && nsumAlarmType.Contains((int)AlarmType.황산누출))
                            {
                                // 황산, 온도                        
                                panel1.BackgroundImage = GetTankImage("Tank_Default_LiquidTemp");
                                pictureBox_alarmPressure.Visible = false;
                            }
                            else if (nsumAlarmType.Count == 2 && nsumAlarmType.Contains((int)AlarmType.탱크최고레벨)
                                                              && nsumAlarmType.Contains((int)AlarmType.황산누출))
                            {
                                // 황산, 레벨
                                panel1.BackgroundImage = GetTankImage("Tank_Default_LiquidLevel");
                                pictureBox_alarmPressure.Visible = false;
                            }
                            else if (nsumAlarmType.Count == 3 && (nsumAlarmType.Contains((int)AlarmType.탱크온도상승) || nsumAlarmType.Contains((int)AlarmType.탱크온도하강))
                                                          && nsumAlarmType.Contains((int)AlarmType.탱크최고레벨)
                                                          && nsumAlarmType.Contains((int)AlarmType.황산누출))
                            {
                                // 황산, 온도, 레벨
                                panel1.BackgroundImage = GetTankImage("Tank_Default_LiquidLevelTemp");
                                pictureBox_alarmPressure.Visible = false;
                            }
                            #endregion

                            if (nsumAlarmType.Count == 1 && (nsumAlarmType.Contains((int)AlarmType.탱크온도상승) || nsumAlarmType.Contains((int)AlarmType.탱크온도하강)))
                            {
                                // 온도                        
                                panel1.BackgroundImage = GetTankImage("Tank_Default_Temp");
                                pictureBox_alarmPressure.Visible = false;
                            }
                            else if (nsumAlarmType.Count == 1 && nsumAlarmType.Contains((int)AlarmType.탱크최고레벨))
                            {
                                // 레벨
                                panel1.BackgroundImage = GetTankImage("Tank_Default_Level");
                                pictureBox_alarmPressure.Visible = false;
                            }
                            else if (nsumAlarmType.Count == 2 && (nsumAlarmType.Contains((int)AlarmType.탱크온도상승) || nsumAlarmType.Contains((int)AlarmType.탱크온도하강))
                                                          && nsumAlarmType.Contains((int)AlarmType.탱크최고레벨))
                            {
                                // 온도, 레벨
                                panel1.BackgroundImage = GetTankImage("Tank_Default_LevelTemp");
                                pictureBox_alarmPressure.Visible = false;
                            }
                        }

                        pictureBox_alarmClear.Visible = true;
                        pictureBox_alarmClear.BringToFront();
                    }
                    else
                    {
                        panel1.BackgroundImage = GetTankImage("Tank_Default");
                        pictureBox_alarmClear.Visible = false;
                        pictureBox_alarmPressure.Visible = false;
                    }

                    if (isWork)
                    {
                        pictureBox_BeginWork.Visible = false;
                        pictureBox_EndWork.Visible = true;
                    }
                    else
                    {
                        pictureBox_BeginWork.Visible = true;
                        pictureBox_EndWork.Visible = false;
                        pictureBox_alarmPressure.Visible = false;
                    }
                     
                    oldAlarmList = newAlarmList;

                    if (!this.bIsWork && isWork)
                    {
                        ArrayList arrResult = MainForm.Instance.dbMgr.GetResultData("SELECT BeginTime FROM LastWorkHistory WHERE TankID=" + this.nTankID + " AND EndTime IS NULL ORDER BY BeginTime", 0);
                        if (arrResult != null && arrResult.Count > 0)
                        {
                            m_recentBeginTime = DBUtility.WebDBManager.GetDateTimeField(arrResult[0]);
                        }
                    }
                }
                this.bIsAlarm = isAlarm;
                this.bIsWork = isWork;
            }
            catch (Exception ex)
            {
                MainForm.Instance.SetSystemLog("[ERROR] SetAlarm(bool isAlarm, bool isChgAlarm, List<CommonFunction.AllAlarm> newAlarmList, bool isWork) / " + ex.Message);
            }
        }

        #region 알람 
        private void pictureBox_alarmClear_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;

            bool buttonStatus = false;
            string msg = "알람을 해제하시겠습니까?";

            ArrayList arrResult = MainForm.Instance.dbMgr.GetResultData("SELECT PropertyValue FROM Options WHERE PropertyName='ButtonStatus'", 0);
            if (arrResult != null)
            {
                int result = DBUtility.WebDBManager.GetIntField(arrResult[0].ToString(), 1);
                if (result == 0) buttonStatus = true;
            }

            //UnE.Utility.UMessageBox.FrameColor = Color.FromArgb(1, 4, 20);
            //if (buttonStatus)
            //    msg = "알람을 해제하시겠습니까?\r함체박스의 Push 버튼이 눌려져 있으므로 알람을 해제해도 경광등은 꺼지지 않습니다.\r경광등을 끄기 위해서는 함체박스의 Push버튼을 다시 눌러주시기 바랍니다.";
             
            //if (UnE.Utility.UMessageBox.Show(MainForm_Tank.Instance, "알람을 해제하시겠습니까?", "알람 해제", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No) return;
            AlarmClear ac = new Popups.AlarmClear(msg);
            ac.StartPosition = FormStartPosition.CenterParent;
            DialogResult dr = ac.ShowDialog();
            if (dr == DialogResult.OK)
                AlarmClear(ac.occurenceType, ac.comment);
        } 

        public void AlarmClear(int occurType, string comment)
        {
            int nCmdID = MainForm.Instance.commonFunction.GetMaxTableID("Command") + 1;
            int nCmdHistoryID = MainForm.Instance.commonFunction.GetMaxTableID("CommandHistory") + 1;

            foreach (CommonFunction.AllAlarm item in oldAlarmList)
            { 
                int commandType = -1;
                if (item.nPipeID > 0)
                    commandType = 0;
                else
                {
                    if (item.nAlarmType < 0)
                        commandType = 11; // 황산 누출
                    else
                        commandType = 2;
                }
                item.nAlarmOccurType = occurType;
                item.strAlarmComment = comment;

                StringBuilder sb = null; 

                if (commandType == 0)
                {
                    sb = new StringBuilder();
                    sb.Append("INSERT INTO Command (ID, CommandType, TimeStamp, PipeID, TankID, UserID) ");
                    sb.Append("VALUES(" + nCmdID + ", 0, now(), " + item.nPipeID + ", " + item.nTankID + ", " + MainForm.Instance.nUserID + ") ");
                    MainForm.Instance.dbMgr.GetResultData(sb.ToString(), 0);

                    sb = new StringBuilder();
                    sb.Append("INSERT INTO CommandHistory (ID, CommandType, CommandMakeTime, CommandExecuteTime, UserID, CmdID, PipeID, TankID, AlarmOccurType, alarmComment, AlarmHistoryID) ");
                    sb.AppendFormat("VALUES ({0}, 0, now(), NULL, {1}, {2}, {3}, {4}, {5}, '{6}', {7})", nCmdHistoryID, MainForm.Instance.nUserID, nCmdID, item.nPipeID, this.nTankID, item.nAlarmOccurType, item.strAlarmComment, item.nAlarmHistoryID);
                    MainForm.Instance.dbMgr.GetResultData(sb.ToString(), 0);
                }
                else if (commandType == 2)
                {
                    sb = new StringBuilder();
                    sb.Append("INSERT INTO Command (ID, CommandType, TimeStamp, TankID, UserID) ");
                    sb.Append("VALUES(" + nCmdID + ", 0, now(), " + nTankID + ", " + MainForm.Instance.nUserID + ") ");
                    MainForm.Instance.dbMgr.GetResultData(sb.ToString(), 0);

                    sb = new StringBuilder();
                    sb.Append("INSERT INTO CommandHistory (ID, CommandType, CommandMakeTime, CommandExecuteTime, UserID, CmdID, TankID, AlarmOccurType, alarmComment, AlarmHistoryID) ");
                    sb.AppendFormat("VALUES ({0}, 0, now(), NULL, {1}, {2}, {3}, {4}, '{5}', {6})", nCmdHistoryID, MainForm.Instance.nUserID, nCmdID, this.nTankID, item.nAlarmOccurType, item.strAlarmComment, item.nAlarmHistoryID);
                    MainForm.Instance.dbMgr.GetResultData(sb.ToString(), 0);
                }
                else if (commandType == 11)
                {
                    // 황산 누출 해제

                    // Buzzer OFF
                    sb = new StringBuilder();
                    sb.Append("INSERT INTO Command (ID, CommandType, TimeStamp, TankID, UserID, CommandValue) ");
                    sb.AppendFormat("VALUES ({0}, 11, now(), {1}, {2}, 1)", nCmdID, item.nTankID, MainForm.Instance.nUserID);
                    MainForm.Instance.dbMgr.GetResultData(sb.ToString(), 0);

                    sb = new StringBuilder();
                    sb.Append("INSERT INTO CommandHistory (ID, CommandType, CommandMakeTime, CommandExecuteTime, UserID, CmdID, TankID, CommandValue, AlarmOccurType, alarmComment, AlarmHistoryID) ");
                    sb.AppendFormat("VALUE ({0},11,now(),null,{1},{2},{3},1,{4},'{5}',{6}) ", nCmdHistoryID, MainForm.Instance.nUserID, nCmdID, item.nTankID, item.nAlarmOccurType, item.strAlarmComment, item.nAlarmHistoryID);
                    MainForm.Instance.dbMgr.GetResultData(sb.ToString(), 0);

                    nCmdID++;
                    nCmdHistoryID++;

                    //Reset
                    sb = new StringBuilder();
                    sb.Append("INSERT INTO Command (ID, CommandType, TimeStamp, TankID, UserID, CommandValue) ");
                    sb.AppendFormat("VALUES ({0}, 13, now(), {1}, {2}, 0) ", nCmdID, item.nTankID, MainForm.Instance.nUserID);
                    MainForm.Instance.dbMgr.GetResultData(sb.ToString(), 0);

                    sb = new StringBuilder();
                    sb.Append("INSERT INTO CommandHistory (ID, CommandType, CommandMakeTime, CommandExecuteTime, UserID, CmdID, TankID, CommandValue, AlarmHistoryID) ");
                    sb.AppendFormat("VALUE ({0},13,now(),null,{4},{1},{2},0,{3}) ", nCmdHistoryID, nCmdID, item.nTankID, item.nAlarmHistoryID, MainForm.Instance.nUserID);
                    MainForm.Instance.dbMgr.GetResultData(sb.ToString(), 0); 
                }

                nCmdID++;
                nCmdHistoryID++;
            }

            if (alarmClearEventArgs != null)
                alarmClearEventArgs();
        } 
        #endregion

        #region 데이터 바인딩 
        public void InitData(CommonFunction.TankInfo tank)
        {
            try
            {
                if (tank.strLiquidType == "황산")
                {
                    if (!tank.bIsLeakStatus && tank.bIsLeakMonitoring)
                    {
                        pictureBox_leakStatus.Image = GetTankImage("Wifi");
                        pictureBox_leakStatus.Visible = true;
                    }
                    else if ((tank.bIsLeakStatus && !tank.bIsLeakMonitoring) || tank.bIsLeakStatus && tank.bIsLeakMonitoring)
                    {
                        pictureBox_leakStatus.Image = GetTankImage("LeakAlarm");
                        pictureBox_leakStatus.Visible = true;
                    }
                    else if (!tank.bIsLeakMonitoring)
                    {
                        pictureBox_leakStatus.Image = GetTankImage("NoWifi");
                        pictureBox_leakStatus.Visible = true;
                    }
                    else
                    {
                        pictureBox_leakStatus.Visible = false;
                    }
                }

                label_tankName.Text = "TK-" + tank.strTankName + tank.strType;
                label_liquidType.Text = tank.strLiquidType;
                label_capacity.Text = "(" + String.Format("{0:##,##}", tank.nCapacity) + "㎘)";
                if (tank.nDensity == -999)
                    label_density.Text = "-";
                else label_density.Text = String.Format("{0:F2}", tank.nDensity);

                if (tank.nTemp == -999)
                    label_temp.Text = "-";
                else
                    label_temp.Text = String.Format("{0:F1}", tank.nTemp);

                if (tank.nMass == -999)
                    label_mass.Text = "-";
                else
                {
                    string strMass = String.Format("{0:##,##.#}", tank.nMass);
                    if (strMass.Substring(0, 1) == ".")
                        strMass = "0" + strMass;
                    label_mass.Text = strMass;
                }

                if (tank.nCurLevel == -999)
                    label_curLevel.Text = "-";
                else
                    label_curLevel.Text = String.Format("{0:F1}", tank.nCurLevel);

                if (tank.nFlow == -999)
                    label_flow.Text = "-";
                else
                    label_flow.Text = String.Format("{0:F1}", tank.nFlow);

                if (label_flow.Text.Length > 6)
                    label_flow.Text = String.Format("{0:F1}", tank.nFlow);

                if (tank.nHighLevel == -999)
                    label_highLevel.Text = "(m, - )";
                else
                    label_highLevel.Text = "(m, " + String.Format("{0:F1}", tank.nHighLevel) + ")";

                if (tank.nMinTemp == -999 || tank.nMaxTemp == -999)
                    label_tempRange.Text = "( - )";
                else
                    label_tempRange.Text = "(" + tank.nMinTemp + " ~ " + tank.nMaxTemp + ")";

                if (tank.nCurLevel == -999 || tank.nHighLevel == -999 || tank.nFlow == -999)
                {
                    pictureBox_tank.Image = GetTankImage("TankNormal0");
                    tankUpDownStatus = TankUpDownStatus.NONE;
                }
                else
                {
                    int nLevelPer = 0;
                    double dd = Math.Round((tank.nCurLevel / tank.nHighLevel) * 100);
                    double dd2 = dd % 5;
                    if (dd2 > 2.5)
                        nLevelPer = Convert.ToInt32(dd + (5 - dd2));
                    else
                        nLevelPer = Convert.ToInt32(dd - dd2);

                    if (nLevelPer > 0 && nLevelPer <= 100)
                    {
                        if (tank.nFlow > 10)
                        {
                            pictureBox_tank.Image = GetTankImage("TankUp" + nLevelPer);
                            tankUpDownStatus = TankUpDownStatus.UP;
                        }
                        else if (tank.nFlow < -10)
                        {
                            pictureBox_tank.Image = GetTankImage("TankDown" + nLevelPer);
                            tankUpDownStatus = TankUpDownStatus.DOWN;
                        }
                        else
                        {
                            pictureBox_tank.Image = GetTankImage("TankNormal" + nLevelPer);
                            tankUpDownStatus = TankUpDownStatus.NONE;
                        }
                    }
                    else if (nLevelPer > 100)
                    {
                        if (tank.nFlow > 10)
                        {
                            pictureBox_tank.Image = GetTankImage("TankUp100");
                            tankUpDownStatus = TankUpDownStatus.UP;
                        }
                        else if (tank.nFlow < -10)
                        {
                            pictureBox_tank.Image = GetTankImage("TankDown100");
                            tankUpDownStatus = TankUpDownStatus.DOWN;
                        }
                        else
                        {
                            pictureBox_tank.Image = GetTankImage("TankNormal100");
                            tankUpDownStatus = TankUpDownStatus.NONE;
                        }
                    }
                    else
                    {
                        pictureBox_tank.Image = GetTankImage("TankNormal0");
                        tankUpDownStatus = TankUpDownStatus.NONE;
                    }
                }

                if (tank.nStandardFlow != -9999 && tank.nStandardFlow != -999)
                {
                    foreach (CommonFunction.AlarmTankOptionInfo item in MainForm.Instance.alarmTankOptionInfo)
                    {
                        if (item.nTankID == this.nTankID)
                        {
                            double minStripLine = 0;
                            double maxStripLine = 0;
                            string strStable = "";
                            if (item.nTankStableType == 0) // 비율 사용
                            {
                                minStripLine = tank.nStandardFlow - Math.Abs((tank.nStandardFlow * item.nTankStableRatio) / 100);
                                maxStripLine = tank.nStandardFlow + Math.Abs((tank.nStandardFlow * item.nTankStableRatio) / 100);
                            }
                            else if (item.nTankStableType == 1) // 절대값 사용
                            {
                                minStripLine = tank.nStandardFlow - item.nTankStableAbsolute;
                                maxStripLine = tank.nStandardFlow + item.nTankStableAbsolute;
                            }

                            string firstStr = "(" + Math.Round(minStripLine, 1) + " ~ " + Math.Round(maxStripLine, 1) + ")";
                            string secondStr = item.nTankStableRatio + "%";

                            if (firstStr.Length <= 14)
                                label_flowRange.Text = firstStr + "\r\n" + secondStr;
                            else
                                label_flowRange.Text = firstStr + "," + secondStr;
                            //label_flowRange.Text = "(" + Math.Round(minStripLine, 1) + " ~ " + Math.Round(maxStripLine, 1) + ")" + "," + item.nTankStableRatio + "%"; 
                            break;
                        }
                    }
                }
                else
                    label_flowRange.Text = "-";
            }
            catch (Exception ex)
            {
                MainForm.Instance.SetSystemLog("[ERROR] InitData(CommonFunction.TankInfo tank) / " + ex.Message);
            }
        }
        
        #endregion  

        private DBUtility.VariousData<DateTime> m_recentBeginTime = null;
        public void SetWorkTime()
        {
            if (m_recentBeginTime == null)
                label_workTime.Text = "-";
            else
            {
                TimeSpan span = MainForm.Instance.SystemNow - m_recentBeginTime.Data;

                int nTotalSeconds = (int)span.TotalSeconds;
                int nHour = nTotalSeconds / 3600;
                int nMin = (nTotalSeconds - nHour * 3600) / 60;
                int nSec = nTotalSeconds - nHour * 3600 - nMin * 60;

                label_workTime.Text = string.Format("{0:00}:{1:00}:{2:00}", nHour, nMin, nSec);
            }
        }

        public enum TankUpDownStatus { NONE, UP, DOWN }
        public TankUpDownStatus tankUpDownStatus { get; set; }

        #region 작업 관련
        private void pictureBox_BeginWork_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;

            bool bDisconnected = false;
            foreach (CommonFunction.TankInfo item in  MainForm.Instance.tankInfo) 
            {
                if (item.nTankID == this.nTankID && item.bDisconnected)
                {
                    bDisconnected = true;
                    break;
                }
            }

            int nConnectWorkPipeID = -1;

            if (bDisconnected)
            {
                BeginWorkSelectPipe pop = new BeginWorkSelectPipe(nTankID, false);
                pop.StartPosition = FormStartPosition.CenterParent;
                if (pop.ShowDialog() != System.Windows.Forms.DialogResult.Yes) return;

                nConnectWorkPipeID = pop.nPipeID;                 
            }
            else
            { 
                BeginWorkSelectPipe pop = new BeginWorkSelectPipe(nTankID);
                pop.StartPosition = FormStartPosition.CenterParent;
                if (pop.ShowDialog() != System.Windows.Forms.DialogResult.Yes) return;

                nConnectWorkPipeID = pop.nPipeID;
            }
              
            //작업시작 
            int nCmdID = MainForm.Instance.commonFunction.GetMaxTableID("Command") + 1;
            int nCmdHistoryID = MainForm.Instance.commonFunction.GetMaxTableID("CommandHistory") + 1;

            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO Command (ID, CommandType, TimeStamp, PipeID, TankID, UserID) ");
            sb.Append("VALUES(" + nCmdID + ", 4, now(), " + nConnectWorkPipeID + ", " + this.nTankID + ", " + MainForm.Instance.nUserID + ") ");
            MainForm.Instance.dbMgr.GetResultData(sb.ToString(), 0);
             
            sb = new StringBuilder();
            sb.Append("INSERT INTO CommandHistory (ID, CommandType, CommandMakeTime, CommandExecuteTime, UserID, CmdID, PipeID, TankID) ");
            sb.Append("VALUES (" + nCmdHistoryID + ", 4, now(), NULL," + MainForm.Instance.nUserID + ", " + nCmdID + ", " + nConnectWorkPipeID + "," + this.nTankID + ") ");
            MainForm.Instance.dbMgr.GetResultData(sb.ToString(), 0); 
        }
        private void pictureBox_EndWork_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;

            UnE.Utility.UMessageBox.FrameColor = Color.FromArgb(1, 4, 20);
            if (UnE.Utility.UMessageBox.Show(MainForm_Tank.Instance, "작업을 종료하시겠습니까?", "작업 종료", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No) return;

            int nCmdID = MainForm.Instance.commonFunction.GetMaxTableID("Command") + 1;
            int nCmdHistoryID = MainForm.Instance.commonFunction.GetMaxTableID("CommandHistory") + 1;

            //List<int> ConnectWorkPipeIDs = commonFunction.ReturnConnectPipeIDs(this.nTankID);

            StringBuilder sb = null;
            foreach (int nConnectWorkPipeID in nConnectWorkPipeIDs)
            {
                sb = new StringBuilder();
                sb.Append("INSERT INTO Command (ID, CommandType, TimeStamp, PipeID, TankID, UserID) ");
                sb.Append("VALUES(" + nCmdID + ", 5, now(), " + nConnectWorkPipeID + ", " + this.nTankID + ", " + MainForm.Instance.nUserID + ") ");
                MainForm.Instance.dbMgr.GetResultData(sb.ToString(), 0);

                sb = new StringBuilder();
                sb.Append("INSERT INTO CommandHistory (ID, CommandType, CommandMakeTime, CommandExecuteTime, UserID, CmdID, PipeID, TankID) ");
                sb.Append("VALUES (" + nCmdHistoryID + ", 5, now(), NULL," + MainForm.Instance.nUserID + ", " + nCmdID + ", " + nConnectWorkPipeID + "," + this.nTankID + ") ");
                MainForm.Instance.dbMgr.GetResultData(sb.ToString(), 0);

                nCmdID++;
                nCmdHistoryID++;
            }
             
            oldAlarmList = new List<CommonFunction.AllAlarm>();
        }  
        #endregion 

        #region 정상범위 새로고침
        private void pictureBox_rangeRefresh_MouseClick(object sender, MouseEventArgs e)
        {
            if (this.nTankID < 0) return;

            if (UnE.Utility.UMessageBox.Show(MainForm_Tank.Instance, "현재값을 기준으로 압력과 유량의 정상범위를 새로 설정하시겠습니까?", "", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No) return;

            int nCmdID = MainForm.Instance.commonFunction.GetMaxTableID("Command") + 1;
            int nCmdHistoryID = MainForm.Instance.commonFunction.GetMaxTableID("CommandHistory") + 1;

            //List<int> ConnectWorkPipeIDs = commonFunction.ReturnConnectPipeIDs(this.nTankID);
            StringBuilder sb = null;

            foreach (int nConnectWorkPipeID in nConnectWorkPipeIDs)
            {
                // 배관없이 작업하는 탱크는 PipeID를 -1로 넘긴다 
                int pipeId = -1;
                if (nConnectWorkPipeID > 0) pipeId = nConnectWorkPipeID;

                sb = new StringBuilder();
                sb.Append("INSERT INTO Command (ID, CommandType, TimeStamp, PipeID, TankID, UserID) ");
                sb.Append("VALUES(" + nCmdID + ", 8, now(), " + pipeId + ", " + this.nTankID + ", " + MainForm.Instance.nUserID + ") ");
                MainForm.Instance.dbMgr.GetResultData(sb.ToString(), 0);

                sb = new StringBuilder();
                sb.Append("INSERT INTO CommandHistory (ID, CommandType, CommandMakeTime, CommandExecuteTime, UserID, CmdID, PipeID, TankID) ");
                sb.AppendFormat("VALUES ({0}, 8, now(), NULL, {1}, {2}, {3}, {4})", nCmdHistoryID, MainForm.Instance.nUserID, nCmdID, pipeId, this.nTankID);
                MainForm.Instance.dbMgr.GetResultData(sb.ToString(), 0);

                nCmdID++;
                nCmdHistoryID++;
            }
        } 
        #endregion

        private Dictionary<string, Image> dicTankLevelImage = new Dictionary<string, Image>();

        private Image GetTankImage(string imgName)
        {
            if (dicTankLevelImage.ContainsKey(imgName))
                return dicTankLevelImage[imgName];
            else
                return dicTankLevelImage["TankNormal0"];
        }
        private void LoadTankImage()
        {
            dicTankLevelImage.Add("TankNormal0", global::KpxPipeMonitoring.Properties.Resources.TankNormal0);
            dicTankLevelImage.Add("TankNormal5", global::KpxPipeMonitoring.Properties.Resources.TankNormal5);
            dicTankLevelImage.Add("TankNormal10", global::KpxPipeMonitoring.Properties.Resources.TankNormal10);
            dicTankLevelImage.Add("TankNormal15", global::KpxPipeMonitoring.Properties.Resources.TankNormal15);
            dicTankLevelImage.Add("TankNormal20", global::KpxPipeMonitoring.Properties.Resources.TankNormal20);
            dicTankLevelImage.Add("TankNormal25", global::KpxPipeMonitoring.Properties.Resources.TankNormal25);
            dicTankLevelImage.Add("TankNormal30", global::KpxPipeMonitoring.Properties.Resources.TankNormal30);
            dicTankLevelImage.Add("TankNormal35", global::KpxPipeMonitoring.Properties.Resources.TankNormal35);
            dicTankLevelImage.Add("TankNormal40", global::KpxPipeMonitoring.Properties.Resources.TankNormal40);
            dicTankLevelImage.Add("TankNormal45", global::KpxPipeMonitoring.Properties.Resources.TankNormal45);
            dicTankLevelImage.Add("TankNormal50", global::KpxPipeMonitoring.Properties.Resources.TankNormal50);
            dicTankLevelImage.Add("TankNormal55", global::KpxPipeMonitoring.Properties.Resources.TankNormal55);
            dicTankLevelImage.Add("TankNormal60", global::KpxPipeMonitoring.Properties.Resources.TankNormal60);
            dicTankLevelImage.Add("TankNormal65", global::KpxPipeMonitoring.Properties.Resources.TankNormal65);
            dicTankLevelImage.Add("TankNormal70", global::KpxPipeMonitoring.Properties.Resources.TankNormal70);
            dicTankLevelImage.Add("TankNormal75", global::KpxPipeMonitoring.Properties.Resources.TankNormal75);
            dicTankLevelImage.Add("TankNormal80", global::KpxPipeMonitoring.Properties.Resources.TankNormal80);
            dicTankLevelImage.Add("TankNormal85", global::KpxPipeMonitoring.Properties.Resources.TankNormal85);
            dicTankLevelImage.Add("TankNormal90", global::KpxPipeMonitoring.Properties.Resources.TankNormal90);
            dicTankLevelImage.Add("TankNormal95", global::KpxPipeMonitoring.Properties.Resources.TankNormal95);
            dicTankLevelImage.Add("TankNormal100", global::KpxPipeMonitoring.Properties.Resources.TankNormal100);
             
            dicTankLevelImage.Add("TankUp5", global::KpxPipeMonitoring.Properties.Resources.TankUp5);
            dicTankLevelImage.Add("TankUp10", global::KpxPipeMonitoring.Properties.Resources.TankUp10);
            dicTankLevelImage.Add("TankUp15", global::KpxPipeMonitoring.Properties.Resources.TankUp15);
            dicTankLevelImage.Add("TankUp20", global::KpxPipeMonitoring.Properties.Resources.TankUp20);
            dicTankLevelImage.Add("TankUp25", global::KpxPipeMonitoring.Properties.Resources.TankUp25);
            dicTankLevelImage.Add("TankUp30", global::KpxPipeMonitoring.Properties.Resources.TankUp30);
            dicTankLevelImage.Add("TankUp35", global::KpxPipeMonitoring.Properties.Resources.TankUp35);
            dicTankLevelImage.Add("TankUp40", global::KpxPipeMonitoring.Properties.Resources.TankUp40);
            dicTankLevelImage.Add("TankUp45", global::KpxPipeMonitoring.Properties.Resources.TankUp45);
            dicTankLevelImage.Add("TankUp50", global::KpxPipeMonitoring.Properties.Resources.TankUp50);
            dicTankLevelImage.Add("TankUp55", global::KpxPipeMonitoring.Properties.Resources.TankUp55);
            dicTankLevelImage.Add("TankUp60", global::KpxPipeMonitoring.Properties.Resources.TankUp60);
            dicTankLevelImage.Add("TankUp65", global::KpxPipeMonitoring.Properties.Resources.TankUp65);
            dicTankLevelImage.Add("TankUp70", global::KpxPipeMonitoring.Properties.Resources.TankUp70);
            dicTankLevelImage.Add("TankUp75", global::KpxPipeMonitoring.Properties.Resources.TankUp75);
            dicTankLevelImage.Add("TankUp80", global::KpxPipeMonitoring.Properties.Resources.TankUp80);
            dicTankLevelImage.Add("TankUp85", global::KpxPipeMonitoring.Properties.Resources.TankUp85);
            dicTankLevelImage.Add("TankUp90", global::KpxPipeMonitoring.Properties.Resources.TankUp90);
            dicTankLevelImage.Add("TankUp95", global::KpxPipeMonitoring.Properties.Resources.TankUp95);
            dicTankLevelImage.Add("TankUp100", global::KpxPipeMonitoring.Properties.Resources.TankUp100);

            dicTankLevelImage.Add("TankDown5", global::KpxPipeMonitoring.Properties.Resources.TankDown5);
            dicTankLevelImage.Add("TankDown10", global::KpxPipeMonitoring.Properties.Resources.TankDown10);
            dicTankLevelImage.Add("TankDown15", global::KpxPipeMonitoring.Properties.Resources.TankDown15);
            dicTankLevelImage.Add("TankDown20", global::KpxPipeMonitoring.Properties.Resources.TankDown20);
            dicTankLevelImage.Add("TankDown25", global::KpxPipeMonitoring.Properties.Resources.TankDown25);
            dicTankLevelImage.Add("TankDown30", global::KpxPipeMonitoring.Properties.Resources.TankDown30);
            dicTankLevelImage.Add("TankDown35", global::KpxPipeMonitoring.Properties.Resources.TankDown35);
            dicTankLevelImage.Add("TankDown40", global::KpxPipeMonitoring.Properties.Resources.TankDown40);
            dicTankLevelImage.Add("TankDown45", global::KpxPipeMonitoring.Properties.Resources.TankDown45);
            dicTankLevelImage.Add("TankDown50", global::KpxPipeMonitoring.Properties.Resources.TankDown50);
            dicTankLevelImage.Add("TankDown55", global::KpxPipeMonitoring.Properties.Resources.TankDown55);
            dicTankLevelImage.Add("TankDown60", global::KpxPipeMonitoring.Properties.Resources.TankDown60);
            dicTankLevelImage.Add("TankDown65", global::KpxPipeMonitoring.Properties.Resources.TankDown65);
            dicTankLevelImage.Add("TankDown70", global::KpxPipeMonitoring.Properties.Resources.TankDown70);
            dicTankLevelImage.Add("TankDown75", global::KpxPipeMonitoring.Properties.Resources.TankDown75);
            dicTankLevelImage.Add("TankDown80", global::KpxPipeMonitoring.Properties.Resources.TankDown80);
            dicTankLevelImage.Add("TankDown85", global::KpxPipeMonitoring.Properties.Resources.TankDown85);
            dicTankLevelImage.Add("TankDown90", global::KpxPipeMonitoring.Properties.Resources.TankDown90);
            dicTankLevelImage.Add("TankDown95", global::KpxPipeMonitoring.Properties.Resources.TankDown95);
            dicTankLevelImage.Add("TankDown100", global::KpxPipeMonitoring.Properties.Resources.TankDown100);

            dicTankLevelImage.Add("Tank_Default",global::KpxPipeMonitoring.Properties.Resources.Tank_Default);
            dicTankLevelImage.Add("Tank_Default_Flow",global::KpxPipeMonitoring.Properties.Resources.Tank_Default_Flow);
            dicTankLevelImage.Add("Tank_Default_Level",global::KpxPipeMonitoring.Properties.Resources.Tank_Default_Level);
            dicTankLevelImage.Add("Tank_Default_LevelFlow",global::KpxPipeMonitoring.Properties.Resources.Tank_Default_LevelFlow);
            dicTankLevelImage.Add("Tank_Default_LevelTemp",global::KpxPipeMonitoring.Properties.Resources.Tank_Default_LevelTemp);
            dicTankLevelImage.Add("Tank_Default_LevelTempFlow",global::KpxPipeMonitoring.Properties.Resources.Tank_Default_LevelTempFlow);
            dicTankLevelImage.Add("Tank_Default_Temp",global::KpxPipeMonitoring.Properties.Resources.Tank_Default_Temp);
            dicTankLevelImage.Add("Tank_Default_TempFlow",global::KpxPipeMonitoring.Properties.Resources.Tank_Default_TempFlow);
            dicTankLevelImage.Add("Tank_Default_Liquid",global::KpxPipeMonitoring.Properties.Resources.Tank_Default_Liquid);
            dicTankLevelImage.Add("Tank_Default_LiquidFlow",global::KpxPipeMonitoring.Properties.Resources.Tank_Default_LiquidFlow);
            dicTankLevelImage.Add("Tank_Default_LiquidTemp",global::KpxPipeMonitoring.Properties.Resources.Tank_Default_LiquidTemp);
            dicTankLevelImage.Add("Tank_Default_LiquidLevel",global::KpxPipeMonitoring.Properties.Resources.Tank_Default_LiquidLevel);
            dicTankLevelImage.Add("Tank_Default_LiquidLevelTemp",global::KpxPipeMonitoring.Properties.Resources.Tank_Default_LiquidLevelTemp);
            dicTankLevelImage.Add("Tank_Default_LiquidLevelTempFlow",global::KpxPipeMonitoring.Properties.Resources.Tank_Default_LiquidLevelTempFlow);
            dicTankLevelImage.Add("Tank_Default_LiquidLevelFlow",global::KpxPipeMonitoring.Properties.Resources.Tank_Default_LiquidLevelFlow);
            dicTankLevelImage.Add("Tank_Default_LiquidTempFlow",global::KpxPipeMonitoring.Properties.Resources.Tank_Default_LiquidTempFlow);
            
            dicTankLevelImage.Add("Tank_Work",global::KpxPipeMonitoring.Properties.Resources.Tank_Work);
            dicTankLevelImage.Add("Tank_Work_Flow",global::KpxPipeMonitoring.Properties.Resources.Tank_Work_Flow);
            dicTankLevelImage.Add("Tank_Work_Level",global::KpxPipeMonitoring.Properties.Resources.Tank_Work_Level);
            dicTankLevelImage.Add("Tank_Work_LevelFlow",global::KpxPipeMonitoring.Properties.Resources.Tank_Work_LevelFlow);
            dicTankLevelImage.Add("Tank_Work_LevelTemp",global::KpxPipeMonitoring.Properties.Resources.Tank_Work_LevelTemp);
            dicTankLevelImage.Add("Tank_Work_LevelTempFlow",global::KpxPipeMonitoring.Properties.Resources.Tank_Work_LevelTempFlow);
            dicTankLevelImage.Add("Tank_Work_Temp",global::KpxPipeMonitoring.Properties.Resources.Tank_Work_Temp);
            dicTankLevelImage.Add("Tank_Work_TempFlow",global::KpxPipeMonitoring.Properties.Resources.Tank_Work_TempFlow);
            dicTankLevelImage.Add("Tank_Work_Liquid",global::KpxPipeMonitoring.Properties.Resources.Tank_Work_Liquid);
            dicTankLevelImage.Add("Tank_Work_LiquidFlow",global::KpxPipeMonitoring.Properties.Resources.Tank_Work_LiquidFlow);
            dicTankLevelImage.Add("Tank_Work_LiquidTemp",global::KpxPipeMonitoring.Properties.Resources.Tank_Work_LiquidTemp);
            dicTankLevelImage.Add("Tank_Work_LiquidLevel",global::KpxPipeMonitoring.Properties.Resources.Tank_Work_LiquidLevel);
            dicTankLevelImage.Add("Tank_Work_LiquidLevelTemp",global::KpxPipeMonitoring.Properties.Resources.Tank_Work_LiquidLevelTemp);
            dicTankLevelImage.Add("Tank_Work_LiquidLevelTempFlow",global::KpxPipeMonitoring.Properties.Resources.Tank_Work_LiquidLevelTempFlow);
            dicTankLevelImage.Add("Tank_Work_LiquidLevelFlow",global::KpxPipeMonitoring.Properties.Resources.Tank_Work_LiquidLevelFlow);
            dicTankLevelImage.Add("Tank_Work_LiquidTempFlow",global::KpxPipeMonitoring.Properties.Resources.Tank_Work_LiquidTempFlow);

            dicTankLevelImage.Add("Wifi",global::KpxPipeMonitoring.Properties.Resources.Wifi);
            dicTankLevelImage.Add("NoWifi",global::KpxPipeMonitoring.Properties.Resources.NoWifi);
            dicTankLevelImage.Add("LeakAlarm",global::KpxPipeMonitoring.Properties.Resources.LeakAlarm);
        }
    }
}
