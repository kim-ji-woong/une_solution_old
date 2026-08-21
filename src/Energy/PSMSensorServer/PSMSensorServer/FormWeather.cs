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
using DBUtility;

namespace PSMSensorServer
{
    public partial class FormWeather : Form
    {
        WebDBManager m_dbMgr = null;
        public FormWeather(WebDBManager dbMgr)
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
            ReadWeatherInfo(m_dbMgr);
        }

        private string szAwsID = "A1000";
        private void ReadWeatherInfo(WebDBManager dbMgr)
        {
            string szTemp = "SELECT Aws_date, Aws_wind, Aws_wspd, Aws_temp FROM r_aws_dat where Aws_ID = '{0}' order by Aws_date DESC limit 1";
            string szSQL = string.Format(szTemp, szAwsID);
            
            ArrayList arResult = dbMgr.GetResultData(szSQL, 0);
            DateTime dtNow = DateTime.Now;
            bool bSet = false;
            if( arResult != null && arResult.Count > 0)
            {
                for(int i = 0 ; i < arResult.Count -3 ; i+= 4)
                {
                    string szDate = WebDBManager.GetStringField(arResult[i]);
                    DateTime dtDate = DateTime.MinValue;
                    bool bParse = false;
                    try
                    {
                        dtDate = DateTime.ParseExact(szDate, "yyyyMMddHHmmss", null);
                        bParse = true;
                    }
                    catch(Exception)
                    {

                    }
                    
                    //VariousData<DateTime> dt = WebDBManager.GetDateTimeField(arResult[i]);
                    float fWind = WebDBManager.GetFloatField(arResult[i + 1].ToString(), -9999);
                    float fWSpeed = WebDBManager.GetFloatField(arResult[i + 2].ToString(), -9999);
                    float fTemp = WebDBManager.GetFloatField(arResult[i + 3].ToString(), -9999);

                    if (bParse == true)
                    {
                        TimeSpan span = dtNow - dtDate;
                        if (span.TotalSeconds < 120)
                        {
                            label5.Text = fWind.ToString();
                            label6.Text = fWSpeed.ToString();
                            label4.Text = fTemp.ToString();

                            bSet = true;
                        }
                        else
                        {
                            label4.Text = "데이터가 오래됨:" + (int)span.TotalMinutes + "분";
                            label5.Text = "데이터가 오래됨:" + (int)span.TotalMinutes + "분";
                            label6.Text = "데이터가 오래됨:" + (int)span.TotalMinutes + "분";

                            bSet = true;
                        }
                    }
                    
                   
                }
            }

            if( bSet== false)
            {
                label4.Text = "-";
                label5.Text = "-";
                label6.Text = "-";
            }

        }
    }
}
