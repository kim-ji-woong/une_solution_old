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

namespace KpxPipeMonitoring.ChildForms
{
    public partial class ChildDetailTank : Form
    {
        public delegate void ThisFormClose();
        public event ThisFormClose thisFormClose;
         
        private int nTankID { get; set; }         

        public ChildDetailTank(int tankID)
        {
            this.DoubleBuffered = true;
            InitializeComponent();

            this.Opacity = 0.8;

            this.KeyPreview = true;
            this.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Escape)
                    this.thisFormClose(); 
            };
             
            this.nTankID = tankID;

            MainForm.Instance.SetDoubleBuffer(panel1, true); 
             
            //System.Reflection.Assembly myAssembly = this.GetType().Assembly;
            //System.Resources.ResourceManager res = new System.Resources.ResourceManager("KpxPipeMonitoring.Properties.Resources", myAssembly);
            //panel1.BackgroundImage = (System.Drawing.Image)res.GetObject("TankDetail_" + this.nTankID);

            InitData();
            SettingStatus();
        } 

        #region 데이터 바인딩
        private void InitData()
        {
            if (this.nTankID < 1 || MainForm.Instance.dbMgr == null) return;

            string strQuery = "SELECT Name, LiquidType, Density, Temperature, Mass, Level, Flow, Type, HighLevel, Capacity, MinTemp, MaxTemp FROM Tank WHERE ID = " + this.nTankID;

            ArrayList arrResult = MainForm.Instance.dbMgr.GetResultData(strQuery, 0);
            if (arrResult == null) return;

            System.Reflection.Assembly myAssembly = this.GetType().Assembly;
            System.Resources.ResourceManager res = new System.Resources.ResourceManager("KpxPipeMonitoring.Properties.Resources", myAssembly);

            for (int i = 0; i < arrResult.Count; i += 12)
            {
                string strTankName = WebDBManager.GetStringField(arrResult[i]);
                string liquidType = WebDBManager.GetStringField(arrResult[i + 1]);
                if (liquidType == "N-BUTANOL") liquidType = "BUTANOL";
                else if (liquidType == "메틸렌클로라이드") liquidType = "MC";
                label_liquidType.Text = liquidType;
                double nDensity = (arrResult[i + 2].ToString() == "null") ? -999 : Convert.ToDouble(arrResult[i + 2]);
                double nTemp = (arrResult[i + 3].ToString() == "null") ? -999 : Convert.ToDouble(arrResult[i + 3]);
                double nMass = (arrResult[i + 4].ToString() == "null") ? -999 : Convert.ToDouble(arrResult[i + 4]);
                double nCurLevel = (arrResult[i + 5].ToString() == "null") ? -999 : Convert.ToDouble(arrResult[i + 5]);
                double nFlow = (arrResult[i + 6].ToString() == "null") ? -999 : Convert.ToDouble(arrResult[i + 6]); 
                double nHighLevel = (arrResult[i + 8].ToString() == "null") ? -999 : Convert.ToDouble(arrResult[i + 8]);
                label_capacity.Text = "(" + String.Format("{0:##,##}", WebDBManager.GetIntField(arrResult[i + 9].ToString(), 0)) + "㎘)";
                double nMinTemp = (arrResult[i + 10].ToString() == "null") ? -999 : Convert.ToDouble(arrResult[i + 10]);
                double nMaxTemp = (arrResult[i + 11].ToString() == "null") ? -999 : Convert.ToDouble(arrResult[i + 11]);

                label_tankName.Text = "TK-" + strTankName + WebDBManager.GetStringField(arrResult[i + 7]);
                if (nDensity == -999) 
                    label_density.Text = "-"; 
                else 
                    label_density.Text = String.Format("{0:F2}", nDensity);

                if (nTemp == -999) 
                    label_temp.Text = "-"; 
                else 
                    label_temp.Text = String.Format("{0:F1}", nTemp);

                if (nMass == -999)
                    label_mass.Text = "-";
                else
                {
                    string strMass = String.Format("{0:##,##.#}", nMass);
                    if (strMass.Substring(0, 1) == ".")
                        strMass = "0" + strMass;
                    label_mass.Text = strMass;
                }
                //else 
                //    label_mass.Text = String.Format("{0:##,##.#}", nMass);

                if (nCurLevel == -999) 
                    label_curLevel.Text = "-"; 
                else label_curLevel.Text = String.Format("{0:F1}", nCurLevel);

                if (nFlow == -999) 
                    label_flow.Text = "-"; 
                else label_flow.Text = String.Format("{0:F1}", nFlow);

                if (nHighLevel == -999) 
                    label_highLevel.Text = "(m, - )"; 
                else
                    label_highLevel.Text = "(m, " + String.Format("{0:F1}", nHighLevel) + ")";

                if (nMinTemp == -999 || nMaxTemp == -999) 
                    label_tempRange.Text = "( - )"; 
                else 
                    label_tempRange.Text = "(" + nMinTemp + " ~ " + nMaxTemp + ")";

                if (nCurLevel == -999 || nHighLevel == -999 || nFlow == -999)
                {
                    pictureBox_tank.Image = (System.Drawing.Image)res.GetObject("TankDetailNormal0");
                    continue;
                }

                int nLevelPer = 0;
                double dd = Math.Round((nCurLevel / nHighLevel) * 100);
                double dd2 = dd % 5;
                if (dd2 > 2.5)
                    nLevelPer = Convert.ToInt32(dd + (5 - dd2));
                else
                    nLevelPer = Convert.ToInt32(dd - dd2);
                  
                if (nLevelPer > 0 && nLevelPer <= 100)
                {
                    if (nFlow > 10) 
                        pictureBox_tank.Image = (System.Drawing.Image)res.GetObject("TankDetailUp" + nLevelPer); 
                    else if (nFlow < -10)
                        pictureBox_tank.Image = (System.Drawing.Image)res.GetObject("TankDetailDown" + nLevelPer); 
                    else
                        pictureBox_tank.Image = (System.Drawing.Image)res.GetObject("TankDetailNormal" + nLevelPer); 
                }
                else if (nLevelPer > 100)
                {
                    if (nFlow > 10)
                        pictureBox_tank.Image = (System.Drawing.Image)res.GetObject("TankDetailUp100"); 
                    else if (nFlow < -10)
                        pictureBox_tank.Image = (System.Drawing.Image)res.GetObject("TankDetailDown100"); 
                    else
                        pictureBox_tank.Image = (System.Drawing.Image)res.GetObject("TankDetailNormal100"); 
                }
                else
                    pictureBox_tank.Image = (System.Drawing.Image)res.GetObject("TankDetailNormal0");
            } 
        } 
        #endregion 

        #region 알람 상태
        private List<CommonFunction.AllAlarm> oldAlarmList = new List<CommonFunction.AllAlarm>();
        private bool isAlarm = false;

        Image Tank_Default = global::KpxPipeMonitoring.Properties.Resources.TankDetail_Default;
        Image Tank_Default_Flow = global::KpxPipeMonitoring.Properties.Resources.TankDetail_Default_Flow;
        Image Tank_Default_Level = global::KpxPipeMonitoring.Properties.Resources.TankDetail_Default_Level;
        Image Tank_Default_LevelFlow = global::KpxPipeMonitoring.Properties.Resources.TankDetail_Default_LevelFlow;
        Image Tank_Default_LevelTemp = global::KpxPipeMonitoring.Properties.Resources.TankDetail_Default_LevelTemp;
        Image Tank_Default_LevelTempFlow = global::KpxPipeMonitoring.Properties.Resources.TankDetail_Default_LevelTempFlow;
        Image Tank_Default_Temp = global::KpxPipeMonitoring.Properties.Resources.TankDetail_Default_Temp;
        Image Tank_Default_TempFlow = global::KpxPipeMonitoring.Properties.Resources.TankDetail_Default_TempFlow;

        public void SettingStatus()
        {
            List<CommonFunction.AllAlarm> newAlarmInfo = MainForm.Instance.newAlarmInfo.Where(p => p.nTankID == this.nTankID).ToList();

            bool isAlarm = false; // 알람이 있는지 
            bool isChgAlarm = false; // 알람이 변경됐는지 (해제, 신규)

            if (newAlarmInfo.Count > 0)
                isAlarm = true;

            if (oldAlarmList.Count != newAlarmInfo.Count)
                isChgAlarm = true;

            foreach (CommonFunction.AllAlarm newInfo in newAlarmInfo)
            {
                int cnt = oldAlarmList.Where(p => p.nAlarmHistoryID == newInfo.nAlarmHistoryID).Count();
                if (cnt == 0)
                    isChgAlarm = true;
            }

            // 작업이 새로 시작되거나 종료된 경우, 알람이 생기거나 해제된 경우, 알람 내용이 변경된 경우
            if (isAlarm != this.isAlarm || isChgAlarm)
            {
                List<int> nsumAlarmType = new List<int>();
                foreach (CommonFunction.AllAlarm item in newAlarmInfo)
                {
                    if (item.nTankID != this.nTankID) continue;
                    if (item.nAlarmHistoryID <= 0) continue;

                    nsumAlarmType.Add(item.nAlarmType);
                }

                if (isAlarm)
                {
                    if (nsumAlarmType.Count == 1 && (nsumAlarmType.Contains((int)AlarmType.탱크온도상승) || nsumAlarmType.Contains((int)AlarmType.탱크온도하강)))
                    {
                        // 온도                        
                        panel1.BackgroundImage = Tank_Default_Temp;
                        //pictureBox_alarmPressure.Visible = false;
                    }
                    else if (nsumAlarmType.Count == 1 && nsumAlarmType.Contains((int)AlarmType.탱크최고레벨))
                    {
                        // 레벨
                        panel1.BackgroundImage = Tank_Default_Level;
                        //pictureBox_alarmPressure.Visible = false;
                    }
                    else if (nsumAlarmType.Count == 2 && (nsumAlarmType.Contains((int)AlarmType.탱크온도상승) || nsumAlarmType.Contains((int)AlarmType.탱크온도하강))
                                                  && nsumAlarmType.Contains((int)AlarmType.탱크최고레벨))
                    {
                        // 온도, 레벨
                        panel1.BackgroundImage = Tank_Default_LevelTemp;
                        //pictureBox_alarmPressure.Visible = false;
                    }
                }
                else
                {
                    panel1.BackgroundImage = Tank_Default; 
                }

                oldAlarmList = newAlarmInfo;

                this.isAlarm = isAlarm;
            }
        }
        #endregion

        private void pictureBox_close_MouseEnter(object sender, EventArgs e)
        {
            pictureBox_close.Image = KpxPipeMonitoring.Properties.Resources.Close_MouseOver;
        }

        private void pictureBox_close_MouseLeave(object sender, EventArgs e)
        {
            pictureBox_close.Image = KpxPipeMonitoring.Properties.Resources.Close;
        }
    }    
}
