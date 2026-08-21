using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.IO;
using System.Collections;


namespace PreSafe
{
    /// <summary>
    /// Service1의 요약 설명입니다.
    /// </summary>
    [WebService(Namespace = "http://unes.iptime.org/presafe")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // ASP.NET AJAX를 사용하여 스크립트에서 이 웹 서비스를 호출하려면 다음 줄의 주석 처리를 제거합니다. 
    // [System.Web.Script.Services.ScriptService]
    public class PreSafe : System.Web.Services.WebService
    {
        static PreSafe()
        {           
            ScriptProxy proxy = ScriptProxy.Instance;
            proxy.UserObject.Script = ScriptProxy.Instance;
        }
             

        [WebMethod(MessageName = "SenarioList" , Description = "실행가능한 시나리오 목록을 알려줍니다.")]
        public string[] SenarioList()
        {
            string szPath = AppDomain.CurrentDomain.BaseDirectory + "App_Data\\";
            IEnumerable<string> files = Directory.EnumerateFiles(szPath, "*.xml");

            int nCount = files.Count<string>();
            if( nCount == 0)
                return null;
            string[] szResult = new string[nCount];
            int i =0;
            foreach(string path in files)
            {
                FileInfo info = new FileInfo(path);
                string szName = (info.Name).Replace(info.Extension, "" );

                szResult[i] = szName;
                i++;
            }
            return szResult;
        }

        [WebMethod(MessageName = "SenarioListForType", Description = "타입별 실행가능한 시나리오 목록을 알려줍니다.")]
        public string[] SenarioListForType(int[] nType)
        {
            string szPath = AppDomain.CurrentDomain.BaseDirectory + "App_Data\\";
            IEnumerable<string> files = Directory.EnumerateFiles(szPath, "*.xml");

            int nCount = files.Count<string>();
            if (nCount == 0)
                return null;

            if (nType == null || nType.Length == 0)
                return null;

            string[] szResult = new string[nCount];
            int i = 0;
            foreach (string path in files)
            {
                FileInfo info = new FileInfo(path);
                string szName = (info.Name).Replace(info.Extension, "");

                for (int j = 0; j < nType.Length; j++)
                {
                    if (IsMatchType(szName, nType[j]))
                    {
                        if (szResult.Contains(szName) == false)
                        {
                            szResult[i] = szName;
                            i++;
                        }
                    }
                }                    
            }
            return szResult;
        }

        private string[] SenarioTypes = 
          { "공통", "강간형", "특수", "미성년자강간", "강제추행", "미성년자강제추행"};

        private bool IsMatchType(string szName, int nType)
        {
            if (nType < 0)
                return false;

            if (nType >= SenarioTypes.Length)
                return false;

            if (szName.IndexOf(SenarioTypes[0]) != -1)
            {
                return true;
            }
            if(szName.StartsWith(SenarioTypes[nType]) == true)
            {
                return true;
            }            
            return false;
        }

        [WebMethod(MessageName = "RunSenario" , Description = "입력된 조건으로 시나리오를 실행하여 결과를 리턴해줍니다.")]
        public string[] RunSenario(string senarioName, 
            bool bUseSound, float nSoundLevel,
            bool bUseHeartBeat, int nHeartBeat,
            bool bUseAlcole, float nAlcole,
            bool bUseVelocity, float nVelocity,
            bool bUseAcc, float nAccelate, 
            bool bUseLocation, int nLocation, 
            bool bUseImpact, bool bImpact)
        {
           
            string szPath = AppDomain.CurrentDomain.BaseDirectory;
            string szFilePath = szPath + "App_Data\\" + senarioName + ".xml";


            string[] varResult = new string[4];
            string szInputValue = "INPUT={";
            varResult[0] = "OK";
            int nInputCount = 0;

            if (System.IO.File.Exists(szFilePath))
            {
                XMLManager mgr = new XMLManager();
                SenarioManager smgr = new SenarioManager();
                mgr.Load(szFilePath, smgr);

                if (bUseSound == true)
                {
                    if (nInputCount != 0)
                        szInputValue += ";";

                    Variable var = (Variable)smgr.SystemVariables.GetVariable("SND");
                    var.Value = nSoundLevel;
                    szInputValue += ("SND=" + var.ToStringValue());

                    nInputCount++;
                }



                if (bUseHeartBeat == true)
                {
                    if (nInputCount != 0)
                        szInputValue += ";";
                    Variable var = (Variable)smgr.SystemVariables.GetVariable("HB");
                    var.Value = nHeartBeat;
                    szInputValue += ("HB=" + var.ToStringValue());
                }
                if (bUseAlcole == true)
                {
                    if (nInputCount != 0)
                        szInputValue += ";";
                    Variable var = (Variable)smgr.SystemVariables.GetVariable("ALC");
                    var.Value = nAlcole;
                    szInputValue += ("ALC=" + var.ToStringValue());
                }

                if (bUseVelocity == true)
                {
                    if (nInputCount != 0)
                        szInputValue += ";";
                    Variable var = (Variable)smgr.SystemVariables.GetVariable("VEL");
                    var.Value = nVelocity;

                    szInputValue += ("VEL=" + var.ToStringValue());
                }
                if (bUseAcc == true)
                {
                    if (nInputCount != 0)
                        szInputValue += ";";
                    Variable var = (Variable)smgr.SystemVariables.GetVariable("ACC");
                    var.Value = nAccelate;
                    szInputValue += ("ACC=" + var.ToStringValue());
                }
                if (bUseLocation == true)
                {
                    if (nInputCount != 0)
                        szInputValue += ";";
                    Variable var = (Variable)smgr.SystemVariables.GetVariable("CL");
                    var.Value = nLocation;
                    szInputValue += ("CL=" + var.ToStringValue());
                }
                if (bUseImpact == true)
                {
                    if (nInputCount != 0)
                        szInputValue += ";";
                    Variable var = (Variable)smgr.SystemVariables.GetVariable("IMP");
                    var.Value = bImpact;
                    szInputValue += ("IMP=" + var.ToStringValue());
                }

                {
                    Variable var = (Variable)smgr.SystemVariables.GetVariable("CR");
                    var.Value = 0.0f;
                }

                DateTime time = DateTime.Now;
                {
                    Variable var1 = (Variable)smgr.SystemVariables.GetVariable("CYEAR");
                    var1.Value = time.Year;

                    Variable var2 = (Variable)smgr.SystemVariables.GetVariable("CMON");
                    var2.Value = time.Month;

                    Variable var3 = (Variable)smgr.SystemVariables.GetVariable("CDAY");
                    var3.Value = time.Day;

                    Variable var4 = (Variable)smgr.SystemVariables.GetVariable("CHOUR");
                    var4.Value = time.TimeOfDay;

                    Variable var5 = (Variable)smgr.SystemVariables.GetVariable("CMIN");
                    var5.Value = time.Minute;

                    Variable var6 = (Variable)smgr.SystemVariables.GetVariable("CSEC");
                    var6.Value = time.Second;
                }

                ScriptMaker sMaker = new ScriptMaker(smgr);
                varResult[3] = sMaker.RunScript();

                if (varResult[3] == "ERROR")
                {
                    varResult[0] = "FAIL";
                }
                else
                {
                    varResult[0] = "OK";
                }
                varResult[2] = sMaker.ScriptResult;

            }
            szInputValue += "}";
            varResult[1] = szInputValue;
            return varResult;
        }

        [WebMethod(MessageName = "RunSenario2", Description = "입력된 조건으로 시나리오를 실행하여 결과를 리턴해줍니다.")]
        public string[] RunSenario2(string senarioName,
            bool bUseLocation, int nLocation,
            bool bUseHeartBeat, int nHeartBeat,
            bool bUseAcc, int nAcc,
            bool bUseAlcohol, int nAlcohol,
            bool bUseSound, int nSound,
            bool bUseImpact, int nImpact)
        {
            string szPath = AppDomain.CurrentDomain.BaseDirectory;
            string szFilePath = szPath + "App_Data\\" + senarioName + ".xml";


            string[] varResult = new string[4];
            string szInputValue = "INPUT={";
            varResult[0] = "OK";
            int nInputCount = 0;

            if (System.IO.File.Exists(szFilePath))
            {
                XMLManager mgr = new XMLManager();
                SenarioManager smgr = new SenarioManager();
                mgr.Load(szFilePath, smgr);


                if (bUseLocation == true)
                {
                    if (nInputCount != 0)
                        szInputValue += ";";
                    Variable var = (Variable)smgr.SystemVariables.GetVariable("CL");
                    var.Value = nLocation;
                    szInputValue += ("CL=" + var.ToStringValue());
                }
                else
                {
                    Variable var = (Variable)smgr.SystemVariables.GetVariable("CL");
                    var.Value = -1;
                }


                if (bUseHeartBeat == true)
                {
                    if (nInputCount != 0)
                        szInputValue += ";";
                    Variable var = (Variable)smgr.SystemVariables.GetVariable("HB");
                    var.Value = nHeartBeat;
                    szInputValue += ("HB=" + var.ToStringValue());
                }
                else
                {
                    Variable var = (Variable)smgr.SystemVariables.GetVariable("HB");
                    var.Value = -1;
                }

                if (bUseAcc == true)
                {
                    if (nInputCount != 0)
                        szInputValue += ";";
                    Variable var = (Variable)smgr.SystemVariables.GetVariable("ACC");
                    var.Value = nAcc;
                    szInputValue += ("ACC=" + var.ToStringValue());
                }
                else
                {
                    Variable var = (Variable)smgr.SystemVariables.GetVariable("ACC");
                    var.Value = -1;
                }

                if (bUseAlcohol == true)
                {
                    if (nInputCount != 0)
                        szInputValue += ";";
                    Variable var = (Variable)smgr.SystemVariables.GetVariable("ALC");
                    var.Value = nAlcohol;
                    szInputValue += ("ALC=" + var.ToStringValue());
                }
                else
                {
                    Variable var = (Variable)smgr.SystemVariables.GetVariable("ALC");
                    var.Value = -1;
                }

                if (bUseImpact == true)
                {
                    if (nInputCount != 0)
                        szInputValue += ";";
                    Variable var = (Variable)smgr.SystemVariables.GetVariable("IMP");
                    var.Value = nImpact;
                    szInputValue += ("IMP=" + var.ToStringValue());
                }
                else
                {
                    Variable var = (Variable)smgr.SystemVariables.GetVariable("IMP");
                    var.Value = -1;
                }

                {
                    Variable var = (Variable)smgr.SystemVariables.GetVariable("CR");
                    var.Value = 0.0f;
                }

                DateTime time = DateTime.Now;
                {                    
                    Variable var1 = (Variable)smgr.SystemVariables.GetVariable("CYEAR");
                    var1.Value = time.Year;

                    Variable var2 = (Variable)smgr.SystemVariables.GetVariable("CMON");
                    var2.Value = time.Month;

                    Variable var3 = (Variable)smgr.SystemVariables.GetVariable("CDAY");
                    var3.Value = time.Day;

                    Variable var4 = (Variable)smgr.SystemVariables.GetVariable("CHOUR");
                    var4.Value = time.Hour;

                    Variable var5 = (Variable)smgr.SystemVariables.GetVariable("CMIN");
                    var5.Value = time.Minute;

                    Variable var6 = (Variable)smgr.SystemVariables.GetVariable("CSEC");
                    var6.Value = time.Second;
                }

                ScriptMaker sMaker = new ScriptMaker(smgr);
                varResult[3] = sMaker.RunScript();

                if (varResult[3] == "ERROR")
                {
                    varResult[0] = "FAIL";
                }
                else
                {
                    varResult[0] = "OK";
                }
                varResult[2] = sMaker.ScriptResult;

            }
            szInputValue += "}";
            varResult[1] = szInputValue;
            return varResult;
        }

        [WebMethod(MessageName = "SaveSenarioData", Description = "시나리오 데이터를 저장합니다.")]
        public bool SaveSenarioData(
            string szDeviceID, int nDeviceType,
            bool bUseLocation, int nLocation,
            bool bUseHeartBeat, int nHeartBeat,
            bool bUseAcc, int nAcc,
            bool bUseAlcohol, int nAlcohol,
            bool bUseSound, int nSound,
            bool bUseImpact, int nImpact,
            string szDescription)
        {
            bool bReturn = false;

            DataManager dbMrg = new DataManager();

            bReturn = dbMrg.AddData("UNES", 1, bUseLocation, nLocation, bUseHeartBeat, nHeartBeat, bUseAcc, nAcc, bUseAlcohol, nAlcohol, bUseSound, nSound, bUseImpact, nImpact, "");

            return bReturn;
        }

        [WebMethod(MessageName = "LoadSenarioData", Description = "시나리오 데이터를 불러옵니다.")]
        public object[] LoadSenarioData(string szDeviceID, int nDeviceType)
        {
            object[] arrResult = null;

            DataManager dbMrg = new DataManager();

            arrResult = dbMrg.LoadData("UNES", 1);

            return arrResult;
        }

        //[WebMethod(Description = "입력된 Python코드를 실행하여 결과를  리턴해줍니다.")]
        //public string RunSenario(string senarioName, string nSoundLevel, string nHeartBeat, string nAlcole, string nVelocity, string nAccelate, string szLocation, bool bImpact)
        //{
        //    string szMessage = "OK";
        //    string szPath = AppDomain.CurrentDomain.BaseDirectory;
        //    string szFilePath = szPath + "App_Data\\"+ senarioName + ".xml";
            
        //    if( System.IO.File.Exists(szFilePath))
        //    {
        //        XMLManager mgr = new XMLManager();
        //        SenarioManager smgr = new SenarioManager();
        //        mgr.Load(szFilePath, smgr);


        //        Variable var = (Variable)smgr.SystemVariables.GetVariable("CR");
        //        var.Value = 52.4f;

        //        ScriptMaker sMaker = new ScriptMaker(smgr);
        //        szMessage = sMaker.RunScript();
        //        szMessage = sMaker.ScriptResult;
        //    }
        //    return szMessage;
        //}


        private void InitializeComponent()
        {
        }
    }
}