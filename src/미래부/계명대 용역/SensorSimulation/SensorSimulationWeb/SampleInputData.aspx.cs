using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SensorSimulationWeb
{
    public partial class SampleInputData : System.Web.UI.Page
    {
        private class PageData
        {
            private string m_strSensorPH = null;
            private string m_strSensorDO = null;
            private string m_strSensorORP = null;
            private string m_strSensorConductivity = null;
            private string m_strSensorDepth = null;
            private string m_strSensorTemp = null;
            private string m_strSensorNO3N = null;
            private string m_strSensorNH4 = null;
            private string m_strSensorTN = null;
            private string m_strSensorPO4 = null;
            private string m_strSensorTP = null;
            private string m_strSensorTurbidity = null;
            private string m_strSensorChlorophyll = null;
            private string m_strStationPH = null;
            private string m_strStationDO = null;
            private string m_strStationTN = null;
            private string m_strStationTP = null;
            private string m_strStationTOC = null;
            private string m_strStationTemp = null;
            private string m_strStationEC = null;
            private string m_strStationChlorophyllA = null;
            private string m_strStationNH3N = null;
            private string m_strStationNO3N = null;
            private string m_strStationPO4P = null;

            public string SensorPH
            {
                get { return m_strSensorPH; }
                set { m_strSensorPH = value; }
            }

            public string SensorDO
            {
                get { return m_strSensorDO; }
                set { m_strSensorDO = value; }
            }

            public string SensorORP
            {
                get { return m_strSensorORP; }
                set { m_strSensorORP = value; }
            }

            public string SensorConductivity
            {
                get { return m_strSensorConductivity; }
                set { m_strSensorConductivity = value; }
            }

            public string SensorDepth
            {
                get { return m_strSensorDepth; }
                set { m_strSensorDepth = value; }
            }

            public string SensorTemp
            {
                get { return m_strSensorTemp; }
                set { m_strSensorTemp = value; }
            }

            public string SensorNO3N
            {
                get { return m_strSensorNO3N; }
                set { m_strSensorNO3N = value; }
            }

            public string SensorNH4
            {
                get { return m_strSensorNH4; }
                set { m_strSensorNH4 = value; }
            }

            public string SensorTN
            {
                get { return m_strSensorTN; }
                set { m_strSensorTN = value; }
            }

            public string SensorPO4
            {
                get { return m_strSensorPO4; }
                set { m_strSensorPO4 = value; }
            }

            public string SensorTP
            {
                get { return m_strSensorTP; }
                set { m_strSensorTP = value; }
            }

            public string SensorTurbidity
            {
                get { return m_strSensorTurbidity; }
                set { m_strSensorTurbidity = value; }
            }

            public string SensorChlorophyll
            {
                get { return m_strSensorChlorophyll; }
                set { m_strSensorChlorophyll = value; }
            }

            public string StationPH
            {
                get { return m_strStationPH; }
                set { m_strStationPH = value; }
            }

            public string StationDO
            {
                get { return m_strStationDO; }
                set { m_strStationDO = value; }
            }

            public string StationTN
            {
                get { return m_strStationTN; }
                set { m_strStationTN = value; }
            }

            public string StationTP
            {
                get { return m_strStationTP; }
                set { m_strStationTP = value; }
            }

            public string StationTOC
            {
                get { return m_strStationTOC; }
                set { m_strStationTOC = value; }
            }

            public string StationTemp
            {
                get { return m_strStationTemp; }
                set { m_strStationTemp = value; }
            }

            public string StationEC
            {
                get { return m_strStationEC; }
                set { m_strStationEC = value; }
            }

            public string StationChlorophyllA
            {
                get { return m_strStationChlorophyllA; }
                set { m_strStationChlorophyllA = value; }
            }

            public string StationNH3N
            {
                get { return m_strStationNH3N; }
                set { m_strStationNH3N = value; }
            }

            public string StationNO3N
            {
                get { return m_strStationNO3N; }
                set { m_strStationNO3N = value; }
            }

            public string StationPO4P
            {
                get { return m_strStationPO4P; }
                set { m_strStationPO4P = value; }
            }

            // Key : Page ID
            private static Dictionary<int, PageData> m_dicDatas = new Dictionary<int, PageData>();

            public static void SetData(PageData data, int nPageID)
            {
                m_dicDatas.Clear();
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

        public const string PageTag = "SampleInputPageID";
        public const string PreviousPageTag = "PreviousSampleInputPageID";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Page.IsPostBack == false)
            {
                int sampleInputID = GetApplicationData(PageTag);
                Application[PreviousPageTag] = sampleInputID;

                PageData data = PageData.GetData(sampleInputID);

                if (data != null)
                {
                    SetData(textBoxSensorPH, data.SensorPH);
                    SetData(textBoxSensorDO, data.SensorDO);
                    SetData(textBoxSensorORP, data.SensorORP);
                    SetData(textBoxSensorConductivity, data.SensorConductivity);
                    SetData(textBoxSensorDepth, data.SensorDepth);
                    SetData(textBoxSensorTemp, data.SensorTemp);
                    SetData(textBoxSensorNO3N, data.SensorNO3N);
                    SetData(textBoxSensorNH4, data.SensorNH4);
                    SetData(textBoxSensorTN, data.SensorTN);
                    SetData(textBoxSensorPO4, data.SensorPO4);
                    SetData(textBoxSensorTP, data.SensorTP);
                    SetData(textBoxSensorTurbidity, data.SensorTurbidity);
                    SetData(textBoxSensorChlorophyll, data.SensorChlorophyll);
                    SetData(textBoxStationPH, data.StationPH);
                    SetData(textBoxStationDO, data.StationDO);
                    SetData(textBoxStationTN, data.StationTN);
                    SetData(textBoxStationTP, data.StationTP);
                    SetData(textBoxStationTOC, data.StationTOC);
                    SetData(textBoxStationTemp, data.StationTemp);
                    SetData(textBoxStationEC, data.StationEC);
                    SetData(textBoxStationChlorophyllA, data.StationChlorophyllA);
                    SetData(textBoxStationNH3N, data.StationNH3N);
                    SetData(textBoxStationNO3N, data.StationNO3N);
                    SetData(textBoxStationPO4P, data.StationPO4P);
                }
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

        protected void btnSave_Click(object sender, EventArgs e)
        {
            string strParams = "";
            PageData pageData = new PageData();

            AddParams(ref strParams, textBoxSensorPH, "sensor_PH", pageData);
            AddParams(ref strParams, textBoxSensorDO, "sensor_DO", pageData);
            AddParams(ref strParams, textBoxSensorORP, "sensor_ORP", pageData);
            AddParams(ref strParams, textBoxSensorConductivity, "sensor_conductivity", pageData);
            AddParams(ref strParams, textBoxSensorDepth, "sensor_depth", pageData);
            AddParams(ref strParams, textBoxSensorTemp, "sensor_temp", pageData);
            AddParams(ref strParams, textBoxSensorNO3N, "sensor_NO3N", pageData);
            AddParams(ref strParams, textBoxSensorNH4, "sensor_NH4", pageData);
            AddParams(ref strParams, textBoxSensorTN, "sensor_TN", pageData);
            AddParams(ref strParams, textBoxSensorPO4, "sensor_PO4", pageData);
            AddParams(ref strParams, textBoxSensorTP, "sensor_TP", pageData);
            AddParams(ref strParams, textBoxSensorTurbidity, "sensor_Turbidity", pageData);
            AddParams(ref strParams, textBoxSensorChlorophyll, "sensor_Chlorophyll", pageData);
            AddParams(ref strParams, textBoxStationPH, "station_PH", pageData);
            AddParams(ref strParams, textBoxStationDO, "station_DO", pageData);
            AddParams(ref strParams, textBoxStationTN, "station_TN", pageData);
            AddParams(ref strParams, textBoxStationTP, "station_TP", pageData);
            AddParams(ref strParams, textBoxStationTOC, "station_TOC", pageData);
            AddParams(ref strParams, textBoxStationTemp, "station_TEMP", pageData);
            AddParams(ref strParams, textBoxStationEC, "station_EC", pageData);
            AddParams(ref strParams, textBoxStationChlorophyllA, "station_Chlorophyll_a", pageData);
            AddParams(ref strParams, textBoxStationNH3N, "station_NH3N", pageData);
            AddParams(ref strParams, textBoxStationNO3N, "station_NO3N", pageData);
            AddParams(ref strParams, textBoxStationPO4P, "station_PO4P", pageData);

            if (strParams.Length == 0)
            {
                MessageBox("데이터 값을 입력하세요.");
                return;
            }

            using (SensorSimulationWeb.Service.SensorSimulator service = new SensorSimulationWeb.Service.SensorSimulator())
            {
                service.SendParameter(strParams);

                int nCurrentPageID = this.GetHashCode();

                PageData.SetData(pageData, nCurrentPageID);
                PageData.RemoveData(GetApplicationData(PreviousPageTag));
                Application[PageTag] = nCurrentPageID;

                Response.Redirect(@"./Sample.aspx");
            }
        }

        private void MessageBox(string sMessage)
        {
            string msg = "<script language=\"javascript\">";
            msg += "alert('" + sMessage + "');";
            msg += "</script>";
            Response.Write(msg);
        }

        private void AddParams(ref string strParams, TextBox textBox, string strVariableName, PageData pageData)
        {
            string str = textBox.Text.Trim();

            if (str.Length == 0)
                return;

            double data;

            if (double.TryParse(str, out data))
            {
                string strParam = strVariableName + "=" + str;

                if (strParams.Length == 0)
                    strParams = strParam;
                else
                    strParams += ";" + strParam;

                if (strVariableName == "sensor_PH")
                    pageData.SensorPH = str;
                else if (strVariableName == "sensor_DO")
                    pageData.SensorDO = str;
                else if (strVariableName == "sensor_ORP")
                    pageData.SensorORP = str;
                else if (strVariableName == "sensor_conductivity")
                    pageData.SensorConductivity = str;
                else if (strVariableName == "sensor_depth")
                    pageData.SensorDepth = str;
                else if (strVariableName == "sensor_temp")
                    pageData.SensorTemp = str;
                else if (strVariableName == "sensor_NO3N")
                    pageData.SensorNO3N = str;
                else if (strVariableName == "sensor_NH4")
                    pageData.SensorNH4 = str;
                else if (strVariableName == "sensor_TN")
                    pageData.SensorTN = str;
                else if (strVariableName == "sensor_PO4")
                    pageData.SensorPO4 = str;
                else if (strVariableName == "sensor_TP")
                    pageData.SensorTP = str;
                else if (strVariableName == "sensor_Turbidity")
                    pageData.SensorTurbidity = str;
                else if (strVariableName == "sensor_Chlorophyll")
                    pageData.SensorChlorophyll = str;
                else if (strVariableName == "station_PH")
                    pageData.StationPH = str;
                else if (strVariableName == "station_DO")
                    pageData.StationDO = str;
                else if (strVariableName == "station_TN")
                    pageData.StationTN = str;
                else if (strVariableName == "station_TP")
                    pageData.StationTP = str;
                else if (strVariableName == "station_TOC")
                    pageData.StationTOC = str;
                else if (strVariableName == "station_TEMP")
                    pageData.StationTemp = str;
                else if (strVariableName == "station_EC")
                    pageData.StationEC = str;
                else if (strVariableName == "station_Chlorophyll_a")
                    pageData.StationChlorophyllA = str;
                else if (strVariableName == "station_NH3N")
                    pageData.StationNH3N = str;
                else if (strVariableName == "station_NO3N")
                    pageData.StationNO3N = str;
                else if (strVariableName == "station_PO4P")
                    pageData.StationPO4P = str;
            }
        }

        private void SetData(TextBox textBox, string str)
        {
            if (str != null)
                textBox.Text = str;
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect(@"./Sample.aspx");
        }
    }
}