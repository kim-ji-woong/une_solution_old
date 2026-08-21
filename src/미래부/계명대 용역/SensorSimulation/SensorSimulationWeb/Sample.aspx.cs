using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SensorSimulationWeb
{
	public partial class Sample : System.Web.UI.Page
	{
        private class PageData
        {
            private string m_strTitle = null;
            private string m_strValue = null;
            private bool m_isMonitoring = true;

            public string Title
            {
                get { return m_strTitle; }
                set { m_strTitle = value; }
            }

            public string Value
            {
                get { return m_strValue; }
                set { m_strValue = value; }
            }

            public bool IsMonitoring
            {
                get { return m_isMonitoring; }
                set { m_isMonitoring = value; }
            }

            // Key : Page ID
            private static Dictionary<int, PageData> m_dicDatas = new Dictionary<int, PageData>();

            public static void SetData(string strTitle, string strValue, bool isMonitoring, int nPageID)
            {
                PageData data = new PageData();

                data.Title = strTitle;
                data.Value = strValue;
                data.IsMonitoring = isMonitoring;

                m_dicDatas[nPageID] = data;
            }

            public static PageData GetData(int nPageID)
            {
                PageData data;

                if (m_dicDatas.TryGetValue(nPageID, out data))
                    return data;

                return null;
            }

            public static void RemoveData(int nPageID)
            {
                m_dicDatas.Remove(nPageID);
            }
        }

        public const string PageTag = "SamplePageID";
        public const string PreviousPageTag = "PreviousSamplePageID";

        private bool m_loadScenarioList = false;
        
        protected void Page_PreLoad(object sender, EventArgs e)
        {
            if (!this.Page.IsPostBack)
            {
                int sampleID = GetApplicationData(PageTag);
                Application[PreviousPageTag] = sampleID;
                
                m_loadScenarioList = false;
                PageData data = PageData.GetData(sampleID);

                if (data != null)
                {
                    if (data.Title != null)
                        labelResultTitle.Text = data.Title;

                    if (data.Value != null)
                        labelResult.Text = data.Value;

                    radioMode.SelectedIndex = data.IsMonitoring ? 0 : 1;
                }

                if (m_loadScenarioList == false)
                    LoadScenarioList();
            }
        }

        private int GetApplicationData(string strTag)
        {
            object obj = Application[strTag];

            if (obj == null)
                return 0;

            int data = (int)obj;
            return data;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.Page.IsPostBack)
            {
            }
            else
            {
                //m_szSelectedSenairo = listBoxSenario.SelectedValue;

                //m_szLocation = cmbLocation.SelectedValue;
                //int nHeartRate = 0;
                //if (int.TryParse(txtHeartRate.Text, out nHeartRate))
                //{
                //    m_nHeartRate = nHeartRate;
                //}
                //m_szAcc = cmbAcc.SelectedValue;
                //m_szAlcohol = cmbAlcohol.SelectedValue;
                //m_szScream = cmbScream.SelectedValue;
                //m_szImpact = cmbImpact.SelectedValue;

                //m_bUseLocation = chkLocation.Checked;
                //m_bUseHeartRate = chkHeartRate.Checked;
                //m_bUseAcc = chkAcc.Checked;
                //m_bUseAlcohol = chkAlcohol.Checked;
                //m_bUseScream = chkScream.Checked;
                //m_bUseImpact = chkImpact.Checked;
            }

        }

        private void LoadScenarioList()
        {
            m_loadScenarioList = true;

            using (SensorSimulationWeb.Service.SensorSimulator service = new SensorSimulationWeb.Service.SensorSimulator())
            {
                listBoxScenario.Items.Clear();
                string[] scenarios = service.ScenarioList(radioMode.SelectedIndex == 0);

                if (scenarios == null)
                    return;

                foreach (string strScenarioName in scenarios)
                {
                    listBoxScenario.Items.Add(strScenarioName);
                }
            }
        }

        protected void btnAddData_Click(object sender, EventArgs e)
        {
            int nCurrentPageID = this.GetHashCode();

            PageData.SetData(labelResultTitle.Text, labelResult.Text, radioMode.SelectedIndex == 0, nCurrentPageID);
            PageData.RemoveData(GetApplicationData(PreviousPageTag));
            Application[PageTag] = nCurrentPageID;

            Response.Redirect(@"./SampleInputData.aspx");
        }

        private void MessageBox(string sMessage)
        {
            string msg = "<script language=\"javascript\">";
            msg += "alert('" + sMessage + "');";
            msg += "</script>";
            Response.Write(msg);
        }

        protected void btnRun_Click(object sender, EventArgs e)
        {
            string strScenario = listBoxScenario.SelectedValue;
            
            if (strScenario.Length == 0)
            {
                labelSelectedScenario.Text = "선택된 시나리오 : " + strScenario;
                MessageBox("시나리오를 선택하세요");
                return;
            }

            labelSelectedScenario.Text = "선택된 시나리오 : " + strScenario;

            using (SensorSimulationWeb.Service.SensorSimulator service = new SensorSimulationWeb.Service.SensorSimulator())
            {
                string[] results = null;

                if (radioMode.SelectedIndex == 0)
                {
                    labelResultTitle.Text = "수질 현황";
                    results = service.RunMonitor2(strScenario);
                }
                else
                {
                    labelResultTitle.Text = "녹조 발생 예보";
                    results = service.RunPredict2(strScenario);
                }

                if (results == null || results.Count() != 4 || results[0] != "OK")
                    labelResult.Text = "실패";
                else
                {
                    int nResult;

                    if (!int.TryParse(results[2], out nResult))
                        labelResult.Text = results[2];
                    else
                    {
                        string strResult = GetResult(nResult, results[3]);
                        labelResult.Text = strResult;
                    }
                }
            }
        }

        private string GetResult(int nData, string strLine)
        {
            string strData = nData.ToString();
            string[] tokens = strLine.Split(' ');

            foreach (string strToken in tokens)
            {
                string[] datas = strToken.Split('=');

                if (datas.Count() != 2)
                    continue;

                if (strData == datas[1].Trim())
                    return datas[0].Trim();
            }

            return "";
        }
        
        protected void btnSearch_Click(object sender, EventArgs e)
        {
        }

        protected void listBoxScenario_SelectedIndexChanged(object sender, EventArgs e)
        {
            string item = listBoxScenario.SelectedValue;
            if (item != null)
            {
                labelSelectedScenario.Text = "선택된 시나리오 : " + item;
                //m_szSelectedSenairo = item;
            }
        }

        protected void radioMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadScenarioList();
        }
	}
}