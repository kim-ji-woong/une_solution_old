using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.IO;

namespace SensorSimulation
{
    /// <summary>
    /// Service1의 요약 설명입니다.
    /// </summary>
    [WebService(Namespace = "http://unes.iptime.org:30005/SensorSimulation")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // ASP.NET AJAX를 사용하여 스크립트에서 이 웹 서비스를 호출하려면 다음 줄의 주석 처리를 제거합니다. 
    // [System.Web.Script.Services.ScriptService]
    public class SensorSimulator : System.Web.Services.WebService
    {
        // struct와 같이 null이 허용되지 않는 데이터를 위한 Wrapper 클래스
        private class VariousData<DataType>
        {
            private DataType data;

            public DataType Data
            {
                get { return data; }
                set { data = value; }
            }

            public VariousData()
            {
            }

            public VariousData(DataType data)
            {
                this.data = data;
            }
        }

        private VariousData<double> m_sensorPH = null;
        private VariousData<double> m_sensorDO = null;
        private VariousData<double> m_sensorORP = null;
        private VariousData<double> m_sensorConductivity = null;
        private VariousData<double> m_sensorDepth = null;
        private VariousData<double> m_sensorTemp = null;
        private VariousData<double> m_sensorNO3N = null;
        private VariousData<double> m_sensorNH4 = null;
        private VariousData<double> m_sensorTN = null;
        private VariousData<double> m_sensorPO4 = null;
        private VariousData<double> m_sensorTP = null;
        private VariousData<double> m_sensorTurbidity = null;
        private VariousData<double> m_sensorChlorophyll = null;
        private VariousData<double> m_stationPH = null;
        private VariousData<double> m_stationDO = null;
        private VariousData<double> m_stationTN = null;
        private VariousData<double> m_stationTP = null;
        private VariousData<double> m_stationTOC = null;
        private VariousData<double> m_stationTemp = null;
        private VariousData<double> m_stationEC = null;
        private VariousData<double> m_stationChlorophyllA = null;
        private VariousData<double> m_stationNH3N = null;
        private VariousData<double> m_stationNO3N = null;
        private VariousData<double> m_stationPO4P = null;

        [WebMethod(MessageName = "ScenarioList", Description = "실행가능한 시나리오 목록을 알려줍니다.")]
        public string[] ScenarioList(bool isMonitoring)
        {
            string strFolder = isMonitoring ? "모니터링" : "예측모델링";
            string szPath = AppDomain.CurrentDomain.BaseDirectory + "App_Data\\" + strFolder;
            IEnumerable<string> files = Directory.EnumerateFiles(szPath, "*.xml");

            int nCount = files.Count<string>();
            if (nCount == 0)
                return null;

            string[] szResult = new string[nCount];
            int i = 0;
            
            foreach (string path in files)
            {
                FileInfo info = new FileInfo(path);
                string szName = (info.Name).Replace(info.Extension, "");

                szResult[i] = szName;
                i++;
            }

            return szResult;
        }

        [WebMethod(MessageName = "RunMonitor", Description = "입력된 조건으로 시나리오를 실행하여 모니터링 결과를 리턴해줍니다.")]
        public string[] RunMonitor(string scenarioName,
            double sensorPH, double sensorDO,
            double sensorORP, double sensorConductivity,
            double sensorDepth, double sensorTemp,
            double sensorNO3N, double sensorNH4,
            double sensorTN, double sensorPO4,
            double sensorTP, double sensorTurbidity,
            double sensorChlorophyll, double stationPH,
            double stationDO, double stationTN,
            double stationTP, double stationTOC,
            double stationTemp, double stationEC,
            double stationChlorophyll_a, double stationNH3N,
            double stationNO3N, double stationPO4P)
        {
            string[] results = RunScript(scenarioName, sensorPH, sensorDO, sensorORP, sensorConductivity, sensorDepth, sensorTemp, sensorNO3N, sensorNH4, sensorTN, sensorPO4, sensorTP, sensorTurbidity, sensorChlorophyll,
                stationPH, stationDO, stationTN, stationTP, stationTOC, stationTemp, stationEC, stationChlorophyll_a, stationNH3N, stationNO3N, stationPO4P, "moni_LEVEL", "3", "수질등급", "모니터링");

            return results;
        }

        [WebMethod(MessageName = "RunMonitor2", Description = "SendParameter를 통하여 전달된 인자를 사용하여 시나리오를 실행하여 모니터링 결과를 리턴해줍니다.")]
        public string[] RunMonitor2(string scenarioName)
        {
            string[] results = RunScript(scenarioName, m_sensorPH, m_sensorDO, m_sensorORP, m_sensorConductivity, m_sensorDepth, m_sensorTemp, m_sensorNO3N, m_sensorNH4, m_sensorTN, m_sensorPO4, m_sensorTP, m_sensorTurbidity, m_sensorChlorophyll,
                m_stationPH, m_stationDO, m_stationTN, m_stationTP, m_stationTOC, m_stationTemp, m_stationEC, m_stationChlorophyllA, m_stationNH3N, m_stationNO3N, m_stationPO4P, "moni_LEVEL", "3", "수질등급", "모니터링");

            return results;
        }

        [WebMethod(MessageName = "RunPredict", Description = "입력된 조건으로 시나리오를 실행하여 예측 모델링 결과를 리턴해줍니다.")]
        public string[] RunPredict(string scenarioName,
            double sensorPH, double sensorDO,
            double sensorORP, double sensorConductivity,
            double sensorDepth, double sensorTemp,
            double sensorNO3N, double sensorNH4,
            double sensorTN, double sensorPO4,
            double sensorTP, double sensorTurbidity,
            double sensorChlorophyll, double stationPH,
            double stationDO, double stationTN,
            double stationTP, double stationTOC,
            double stationTemp, double stationEC,
            double stationChlorophyll_a, double stationNH3N,
            double stationNO3N, double stationPO4P)
        {
            string[] results = RunScript(scenarioName, sensorPH, sensorDO, sensorORP, sensorConductivity, sensorDepth, sensorTemp, sensorNO3N, sensorNH4, sensorTN, sensorPO4, sensorTP, sensorTurbidity, sensorChlorophyll,
                stationPH, stationDO, stationTN, stationTP, stationTOC, stationTemp, stationEC, stationChlorophyll_a, stationNH3N, stationNO3N, stationPO4P, "pred_LEVEL", "5", "녹조발생 레벨", "예측 모델링");

            return results;
        }

        [WebMethod(MessageName = "RunPredict2", Description = "SendParameter를 통하여 전달된 인자를 사용하여 시나리오를 실행하여 예측 모델링 결과를 리턴해줍니다.")]
        public string[] RunPredict2(string scenarioName)
        {
            string[] results = RunScript(scenarioName, m_sensorPH, m_sensorDO, m_sensorORP, m_sensorConductivity, m_sensorDepth, m_sensorTemp, m_sensorNO3N, m_sensorNH4, m_sensorTN, m_sensorPO4, m_sensorTP, m_sensorTurbidity, m_sensorChlorophyll,
                m_stationPH, m_stationDO, m_stationTN, m_stationTP, m_stationTOC, m_stationTemp, m_stationEC, m_stationChlorophyllA, m_stationNH3N, m_stationNO3N, m_stationPO4P, "pred_LEVEL", "5", "녹조발생 레벨", "예측 모델링");

            return results;
        }


        [WebMethod(MessageName = "SendParameter", Description = "Parameter들을 하나의 문자열에 담아 미리 전송합니다.")]
        public void SendParameter(string strParams)
        {
            string[] tokens = strParams.Split(';');

            foreach (string strToken in tokens)
            {
                string[] datas = strToken.Split('=');

                if (datas.Count() != 2)
                    continue;

                datas[0] = datas[0].Trim();
                datas[1] = datas[1].Trim();

                if (string.Compare(datas[0], "sensor_PH", true) == 0)
                    SetParameter(ref m_sensorPH, datas[1]);
                else if (string.Compare(datas[0], "sensor_DO", true) == 0)
                    SetParameter(ref m_sensorDO, datas[1]);
                else if (string.Compare(datas[0], "sensor_ORP", true) == 0)
                    SetParameter(ref m_sensorORP, datas[1]);
                else if (string.Compare(datas[0], "sensor_conductivity", true) == 0)
                    SetParameter(ref m_sensorConductivity, datas[1]);
                else if (string.Compare(datas[0], "sensor_depth", true) == 0)
                    SetParameter(ref m_sensorDepth, datas[1]);
                else if (string.Compare(datas[0], "sensor_temp", true) == 0)
                    SetParameter(ref m_sensorTemp, datas[1]);
                else if (string.Compare(datas[0], "sensor_NO3N", true) == 0)
                    SetParameter(ref m_sensorNO3N, datas[1]);
                else if (string.Compare(datas[0], "sensor_NH4", true) == 0)
                    SetParameter(ref m_sensorNH4, datas[1]);
                else if (string.Compare(datas[0], "sensor_TN", true) == 0)
                    SetParameter(ref m_sensorTN, datas[1]);
                else if (string.Compare(datas[0], "sensor_PO4", true) == 0)
                    SetParameter(ref m_sensorPO4, datas[1]);
                else if (string.Compare(datas[0], "sensor_TP", true) == 0)
                    SetParameter(ref m_sensorTP, datas[1]);
                else if (string.Compare(datas[0], "sensor_Turbidity", true) == 0)
                    SetParameter(ref m_sensorTurbidity, datas[1]);
                else if (string.Compare(datas[0], "sensor_Chlorophyll", true) == 0)
                    SetParameter(ref m_sensorChlorophyll, datas[1]);
                else if (string.Compare(datas[0], "station_PH", true) == 0)
                    SetParameter(ref m_stationPH, datas[1]);
                else if (string.Compare(datas[0], "station_DO", true) == 0)
                    SetParameter(ref m_stationDO, datas[1]);
                else if (string.Compare(datas[0], "station_TN", true) == 0)
                    SetParameter(ref m_stationTN, datas[1]);
                else if (string.Compare(datas[0], "station_TP", true) == 0)
                    SetParameter(ref m_stationTP, datas[1]);
                else if (string.Compare(datas[0], "station_TOC", true) == 0)
                    SetParameter(ref m_stationTOC, datas[1]);
                else if (string.Compare(datas[0], "station_TEMP", true) == 0)
                    SetParameter(ref m_stationTemp, datas[1]);
                else if (string.Compare(datas[0], "station_EC", true) == 0)
                    SetParameter(ref m_stationEC, datas[1]);
                else if (string.Compare(datas[0], "station_Chlorophyll_a", true) == 0)
                    SetParameter(ref m_stationChlorophyllA, datas[1]);
                else if (string.Compare(datas[0], "station_NH3N", true) == 0)
                    SetParameter(ref m_stationNH3N, datas[1]);
                else if (string.Compare(datas[0], "station_NO3N", true) == 0)
                    SetParameter(ref m_stationNO3N, datas[1]);
                else if (string.Compare(datas[0], "station_PO4P", true) == 0)
                    SetParameter(ref m_stationPO4P, datas[1]);
            }
        }

        private string[] RunScript(string scenarioName,
            double sensorPH, double sensorDO,
            double sensorORP, double sensorConductivity,
            double sensorDepth, double sensorTemp,
            double sensorNO3N, double sensorNH4,
            double sensorTN, double sensorPO4,
            double sensorTP, double sensorTurbidity,
            double sensorChlorophyll, double stationPH,
            double stationDO, double stationTN,
            double stationTP, double stationTOC,
            double stationTemp, double stationEC,
            double stationChlorophyll_a, double stationNH3N,
            double stationNO3N, double stationPO4P, string strVariable, string strInitValue, string strTag, string strDisasterType)
        {
            return RunScript(scenarioName,
                new VariousData<double>(sensorPH), new VariousData<double>(sensorDO),
                new VariousData<double>(sensorORP), new VariousData<double>(sensorConductivity),
                new VariousData<double>(sensorDepth), new VariousData<double>(sensorTemp),
                new VariousData<double>(sensorNO3N), new VariousData<double>(sensorNH4),
                new VariousData<double>(sensorTN), new VariousData<double>(sensorPO4),
                new VariousData<double>(sensorTP), new VariousData<double>(sensorTurbidity),
                new VariousData<double>(sensorChlorophyll), new VariousData<double>(stationPH),
                new VariousData<double>(stationDO), new VariousData<double>(stationTN),
                new VariousData<double>(stationTP), new VariousData<double>(stationTOC),
                new VariousData<double>(stationTemp), new VariousData<double>(stationEC),
                new VariousData<double>(stationChlorophyll_a), new VariousData<double>(stationNH3N),
                new VariousData<double>(stationNO3N), new VariousData<double>(stationPO4P), strVariable, strInitValue, strTag, strDisasterType);
        }

        private string[] RunScript(string scenarioName,
            VariousData<double> sensorPH, VariousData<double> sensorDO,
            VariousData<double> sensorORP, VariousData<double> sensorConductivity,
            VariousData<double> sensorDepth, VariousData<double> sensorTemp,
            VariousData<double> sensorNO3N, VariousData<double> sensorNH4,
            VariousData<double> sensorTN, VariousData<double> sensorPO4,
            VariousData<double> sensorTP, VariousData<double> sensorTurbidity,
            VariousData<double> sensorChlorophyll, VariousData<double> stationPH,
            VariousData<double> stationDO, VariousData<double> stationTN,
            VariousData<double> stationTP, VariousData<double> stationTOC,
            VariousData<double> stationTemp, VariousData<double> stationEC,
            VariousData<double> stationChlorophyll_a, VariousData<double> stationNH3N,
            VariousData<double> stationNO3N, VariousData<double> stationPO4P, string strVariable, string strInitValue, string strTag, string strDisasterType)
        {
            string strFolder = strDisasterType == "모니터링" ? "모니터링" : "예측모델링";
            string szPath = AppDomain.CurrentDomain.BaseDirectory;
            string szFilePath = szPath + "App_Data\\" + strFolder + "\\" + scenarioName + ".xml";

            string[] varResult = new string[4];
            string szInputValue = "INPUT={";
            varResult[0] = "OK";
            int nInputCount = 0;

            if (System.IO.File.Exists(szFilePath))
            {
                XMLManager mgr = new XMLManager();
                ScenarioManager smgr = new ScenarioManager();
                mgr.Load(szFilePath, smgr);

                AddVariable(ref nInputCount, ref szInputValue, smgr, sensorPH, "sensor_PH");
                AddVariable(ref nInputCount, ref szInputValue, smgr, sensorDO, "sensor_DO");
                AddVariable(ref nInputCount, ref szInputValue, smgr, sensorORP, "sensor_ORP");
                AddVariable(ref nInputCount, ref szInputValue, smgr, sensorConductivity, "sensor_conductivity");
                AddVariable(ref nInputCount, ref szInputValue, smgr, sensorDepth, "sensor_depth");
                AddVariable(ref nInputCount, ref szInputValue, smgr, sensorTemp, "sensor_temp");
                AddVariable(ref nInputCount, ref szInputValue, smgr, sensorNO3N, "sensor_NO3N");
                AddVariable(ref nInputCount, ref szInputValue, smgr, sensorNH4, "sensor_NH4");
                AddVariable(ref nInputCount, ref szInputValue, smgr, sensorTN, "sensor_TN");
                AddVariable(ref nInputCount, ref szInputValue, smgr, sensorPO4, "sensor_PO4");
                AddVariable(ref nInputCount, ref szInputValue, smgr, sensorTP, "sensor_TP");
                AddVariable(ref nInputCount, ref szInputValue, smgr, sensorTurbidity, "sensor_Turbidity");
                AddVariable(ref nInputCount, ref szInputValue, smgr, sensorChlorophyll, "sensor_Chlorophyll");
                AddVariable(ref nInputCount, ref szInputValue, smgr, stationPH, "station_PH");
                AddVariable(ref nInputCount, ref szInputValue, smgr, stationDO, "station_DO");
                AddVariable(ref nInputCount, ref szInputValue, smgr, stationTN, "station_TN");
                AddVariable(ref nInputCount, ref szInputValue, smgr, stationTP, "station_TP");
                AddVariable(ref nInputCount, ref szInputValue, smgr, stationTOC, "station_TOC");
                AddVariable(ref nInputCount, ref szInputValue, smgr, stationTemp, "station_TEMP");
                AddVariable(ref nInputCount, ref szInputValue, smgr, stationEC, "station_EC");
                AddVariable(ref nInputCount, ref szInputValue, smgr, stationChlorophyll_a, "station_Chlorophyll_a");
                AddVariable(ref nInputCount, ref szInputValue, smgr, stationNH3N, "station_NH3N");
                AddVariable(ref nInputCount, ref szInputValue, smgr, stationNO3N, "station_NO3N");
                AddVariable(ref nInputCount, ref szInputValue, smgr, stationPO4P, "station_PO4P");

                Variable variable = (Variable)smgr.SystemVariables.GetVariable(strVariable);
                variable.Value = strInitValue;

                ScriptMaker sMaker = new ScriptMaker(smgr);
                varResult[3] = sMaker.RunScript(strVariable, strInitValue);

                if (varResult[3] == "ERROR" || smgr.DisasterType != strDisasterType)
                {
                    varResult[0] = "FAIL";
                }
                else
                {
                    varResult[0] = "OK";
                }

                varResult[2] = sMaker.ScriptResult;
                varResult[3] = smgr.GetEnumDescription(strTag);
            }
            szInputValue += "}";
            varResult[1] = szInputValue;
            return varResult;
        }

        private void AddVariable(ref int nInputCount, ref string szInputValue, ScenarioManager smgr, VariousData<double> value, string strVarName)
        {
            if (value == null)
                return;

            if (nInputCount++ != 0)
                szInputValue += ";";

            Variable variable = (Variable)smgr.SystemVariables.GetVariable(strVarName);
            variable.Value = value.Data;
            szInputValue += (strVarName + "=" + variable.ToStringValue());
        }

        private void SetParameter(ref VariousData<double> variable, string strParam)
        {
            double data;

            if (double.TryParse(strParam, out data))
            {
                variable = new VariousData<double>(data);
            }
            else
                variable = null;
        }
    }
}