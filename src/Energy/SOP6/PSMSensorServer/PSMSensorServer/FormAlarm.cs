using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DBUtility2;
using DBUtility;

namespace PSMSensorServer
{
    public partial class FormAlarm : Form
    {
        LocalDBManager m_dbMgr = null;
        public FormAlarm(LocalDBManager dbMgr)
        {
            m_dbMgr = dbMgr;
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FormWeather_Load(object sender, EventArgs e)
        {
            ReadAlarmInfo();
        }

        private string MakeSimpleDateTimeString(DateTime time)
		{
			return string.Format("{0}{1:00}{2:00}{3:00}{4:00}{5:00}", time.Year, time.Month, time.Day, time.Hour, time.Minute, time.Second);
		}

        private string MakeSimpleYearDateTimeString(DateTime time)
        {
            return string.Format("{0}{1:00}{2:00}", time.Year, time.Month, time.Day);
        }

        private int GetMaxID(string strTableName, string szIDField)
        {
            string szTime = MakeSimpleYearDateTimeString(DateTime.Now);
            string strSQL = "select MAX(" + szIDField + ") from " + strTableName + " WHERE ss_Ctl_Day = '" + szTime + "'";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count == 0)
                return 0;

            return WebDBManager.GetIntField(arrResult[0].ToString(), 0);
        }

        private void SetAlarmOnOff(int nType, bool bOnOff)
        {
            DateTime dt = DateTime.Now;
            string szDate = MakeSimpleDateTimeString(dt);
            string szDate2 = MakeSimpleYearDateTimeString(dt);
            InsertCtrInfo(nType, bOnOff, szDate2, szDate);
            UpdateSensorInfo(nType, bOnOff, szDate);
        }

        private void UpdateSensorInfo(int nType, bool bOnOff, string szDate)
        {
            string szUpdateTime = WebDBManager.MakeDateTimeString(DateTime.Now);
            string szCtrlID = ((nType == 1) ? szAlarmID1 : szAlarmID2);
            string szTemp = "UPDATE c_ss_info SET ss_Cur_Stat='{0}', ss_Cur_Date='{1}', ss_Cur_Value='{2}', Mdfy_User='ETMGR', Mdfy_Dttm='{3}' WHERE ss_ID = '{4}'";
            string szSQL = string.Format(szTemp, (bOnOff == true ? "11" : "12"), szDate, "0", szUpdateTime, szCtrlID);

            m_dbMgr.GetResultData(szSQL, 0); 
        }

        private void InsertCtrInfo(int nType, bool bOnOff, string szDate, string szCreateDate)
        {
            string szTemp = "INSERT INTO r_ss_ctl_dat (ss_Ctl_Day,Ss_Ctl_Seq,Ss_Ctl_ID,Ss_ID,Ss_Ctl_Stat, " +
                            " Ss_Ctl_Type,Ss_Ctl_Value,Ss_Ctl_Mgr,Ss_Ctl_RecDate,Crte_User,Mdfy_User) " +
                            " VALUES ( '{0}','{1}','{2}','','01','{3}','0','ETMGR','{4}','ETMGR','ETMGR')";
            int nMaxID = GetMaxID("r_ss_ctl_dat", "Ss_ctl_seq") + 1;
            string szCtrlID = ((nType == 1) ? szAlarmID1 : szAlarmID2);
            string szType = (bOnOff == true ? "00":"01");

            string szSQL = string.Format(szTemp, szDate, nMaxID, szCtrlID, szType, szCreateDate);
            m_dbMgr.GetResultData(szSQL, 0);            
        }

        private string szAlarmID1 = "W1000";
        private string szAlarmID2 = "W2000";
        private bool bAlarm1 = false;
        private bool bAlarm2 = false;

        private void ReadAlarmInfo()
        {
            string szTemp = "SELECT ss_Cur_Stat, ss_Cur_Date FROM c_ss_info WHERE ss_ID = '{0}'";
            string szSQL = string.Format(szTemp, szAlarmID1);
            ArrayList arResult = m_dbMgr.GetResultData(szSQL, 0);
            if (arResult != null && arResult.Count > 0)
            {
                for (int i = 0; i < arResult.Count - 1; i += 2)
                {
                    string value = LocalDBManager.GetStringField(arResult[0]);
                    string strDate = arResult[1].ToString();
                    try
                    {
                        // DateTime dtDate = DateTime.ParseExact(strDate, "yyyyMMddhhmmss", null);
                        //if (CheckTime(dtDate))
                        {
                            if (value == "10")
                            {
                                label4.Text = "알람";
                                bAlarm1 = true;
                            }
                            else if (value == "11")
                            {
                                label4.Text = "알람요청";
                                bAlarm1 = true;
                            }
                            else if (value == "12")
                            {
                                label4.Text = "중지요청";
                                bAlarm1 = true;
                            }
                            else if (value == "99")
                            {
                                label4.Text = "연결안됨";
                                bAlarm1 = true;
                            }

                            else if (value == "03")
                            {
                                label4.Text = "제어실패";
                                bAlarm1 = true;
                            }
                            else
                            {
                                label5.Text = "정상";
                                bAlarm1 = false;
                            }
                        }
                    }
                    catch (Exception )
                    {
                    }
                }
            }

            string szSQL2 = string.Format(szTemp, szAlarmID2);
            ArrayList arResult2 = m_dbMgr.GetResultData(szSQL2, 0);
            if (arResult2 != null && arResult2.Count > 0)
            {
                for (int i = 0; i < arResult2.Count - 1; i += 2)
                {
                    string value = LocalDBManager.GetStringField(arResult2[0]);

                    string strDate = arResult2[1].ToString();
                    try
                    {
                        if (value == "10")
                        {
                            label5.Text = "알람";
                            bAlarm2 = true;
                        }

                        else if (value == "03")
                        {
                            label5.Text = "제어실패";
                            bAlarm2 = true;
                        }
                        else if (value == "11")
                        {
                            label5.Text = "알람요청";
                            bAlarm2 = true;
                        }
                        else if (value == "12")
                        {
                            label5.Text = "중지요청";
                            bAlarm2 = true;
                        }
                        else if (value == "99")
                        {
                            label5.Text = "연결안됨";
                            bAlarm2 = true;
                        }
                        else
                        {
                            label5.Text = "정상";
                            bAlarm2 = false;
                        }                      
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }

        private int mValidateTime = 12000000;//second
        private bool CheckTime(DateTime dt)
        {
            DateTime dtNow = DateTime.Now;
            if (dt != null)
            {
                TimeSpan span = dtNow - dt;
                if (span.TotalSeconds < mValidateTime)
                {
                    return true;
                }
            }
            return false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SetAlarmOnOff(1, true);

        }

        private void button3_Click(object sender, EventArgs e)
        {
            SetAlarmOnOff(2, true);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            SetAlarmOnOff(1, false);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            SetAlarmOnOff(2, false);
        }

    }
}
