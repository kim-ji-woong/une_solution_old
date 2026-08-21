using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnE.SenarioMaker
{
    internal class SenarioManager
    {
        private static SenarioManager m_Instance = null;
        internal static SenarioManager Instance
        {
            get
            {
                if (m_Instance == null)
                    m_Instance = new SenarioManager();
                return m_Instance;
            }         
        }

        protected Variables<Variable> m_SystemVars = new Variables<Variable>();
        internal Variables<Variable> SystemVariables
        {
            get { return m_SystemVars; }
            set { m_SystemVars = value; }
        }

        protected Variables<UserVariable> m_UserVars = new Variables<UserVariable>();
        internal Variables<UserVariable> UserVariables
        {
            get { return m_UserVars; }
            set { m_UserVars = value; }
        }

        protected Variables<Enums> m_Enums = new Variables<Enums>();
        internal Variables<Enums> EnumList
        {
            get { return m_Enums; }
            set { m_Enums = value; }
        }        

        protected string m_szSenarioName = "";
        internal string SenarioName
        {
            get { return m_szSenarioName; }
            set { m_szSenarioName = value; }
        }

        protected int m_nType = 1;
        internal int SenarioType
        {
            get { return m_nType; }
            set 
            { 
                m_nType = value;
                m_szDisasterType = ToDisasterType(m_nType);
            }
        }

        public static string ToDisasterType(int nScenarioType)
        {
            if (nScenarioType == 1)
                return "모니터링";
            else if (nScenarioType == 2)
                return "예측 모델링";

            return "";
        }

        protected string m_szSenarioFilePath = "";
        internal string SenarioFilePath
        {
            get { return m_szSenarioFilePath; }
            set { m_szSenarioFilePath = value; }
        }


        private string m_szCategory = "UnE";
        public string Category
        {
            get { return m_szCategory; }
            set { m_szCategory = value; }
        }

        private string m_szSubCategory = "SOP";
        public string SubCategory
        {
            get { return m_szSubCategory; }
            set { m_szSubCategory = value; }
        }

        private string m_szDisasterType = "모니터링";
        public string DisasterType
        {
            get { return m_szDisasterType; }
            set 
            {
                if (value == "모니터링")
                {
                    m_nType = 1;
                    m_szDisasterType = value;
                }
                else if (value == "예측 모델링")
                {
                    m_nType = 2;
                    m_szDisasterType = value;
                }
                else
                {
                    m_nType = -1;
                }
            }
        }

        private string m_szVersionName = "";
        public string VersionName
        {
            get { return m_szVersionName; }
            set { m_szVersionName = value; }
        }

        private ArrayList m_ActionStepList = new ArrayList();
        public ArrayList ActionStepList
        {
            get { return m_ActionStepList; }
            set { m_ActionStepList = value; }
        }
        
        private ISectionPageContainer m_PageOwner = null;
        public ISectionPageContainer SectionPageOwner
        {
            get { return m_PageOwner; }
            set { m_PageOwner = value; }
        }


        private SenarioManager()
        {
            CreateSystemEnumVars();
            CreateSystemVars();
        }

        internal void NewSenario(string szSenarioName, int nType)
        {
            m_szSenarioName = szSenarioName;
            SenarioType = nType;

            // 파일Path의 초기화
            m_szSenarioFilePath = "";

            m_ActionStepList.Clear();

            ActionStep newActionStep = new ActionStep();
            newActionStep.StepName = szSenarioName;
            newActionStep.DisasterID = m_nType;
            newActionStep.Selected = true;
            newActionStep.TeamName = "Main";
            m_ActionStepList.Add(newActionStep);

            if (m_PageOwner != null)
            {
                m_PageOwner.OnAddActionStep(newActionStep);
                m_PageOwner.OnShowActionStep(newActionStep);
            }
            SetActionStepID();
        }

        private void SetActionStepID()
        {
            int id = 1;
            foreach(ActionStep step in m_ActionStepList)
            {
                step.ID = id;
                if (step.Panel != null)
                    step.Panel.ActionStepID = id;
                id++;
            }
        }

        internal System.Windows.Forms.TabPage RemoveActionStep(ActionStep step)
        {
            if(m_ActionStepList.Contains(step))
            {
                m_ActionStepList.Remove(step);
                if(m_PageOwner != null)
                {
                    return m_PageOwner.OnDeleteActionStep(step);                    
                }
            }
            return null;
        }

        internal ActionStep AddActionStep(string szName)
        {
            ActionStep newActionStep = new ActionStep();
            newActionStep.StepName = szName;
            newActionStep.DisasterID = m_nType;
            newActionStep.Selected = true;
            newActionStep.TeamName = "Sub";
            m_ActionStepList.Add(newActionStep);
            
            if (m_PageOwner != null)
            {
                m_PageOwner.OnAddActionStep(newActionStep);
                m_PageOwner.OnShowActionStep(newActionStep);
            }

            SetActionStepID();

            return newActionStep;
        }

        internal void SelectActionStep(Sections.PanelSection panel)
        {
            foreach (ActionStep step in m_ActionStepList)
            {               
                if (step.Panel != null && step.Panel == panel)
                {
                    step.Selected = true;                    
                }
                else if(step.Panel != null && step.Panel != panel)
                {
                    step.Selected = false;
                }
            }
        }

        internal void SelectActionStep(ActionStep step)
        {
            foreach (ActionStep astep in m_ActionStepList)
            {
                if (astep == step)
                {
                    astep.Selected = true;
                }
                else
                {
                    astep.Selected = false;

                    if(astep.Panel != null)
                    {
                        astep.Panel.ClearSelection();
                        astep.Panel.Refresh();
                    }                    
                }
            }
        }
        
        internal UnE.SenarioMaker.ActionStep FindActionStep(string szActionStepName, int nID)
        {
            foreach (ActionStep astep in m_ActionStepList)
            {
                if(astep.StepName == szActionStepName && astep.ID == nID)
                {
                    return astep;
                }
            }
            return null;
        }

		internal bool IsExistActionStepName(string szActionStepName)
		{
			foreach (ActionStep astep in m_ActionStepList)
			{
				if (astep.StepName == szActionStepName)
				{
					return true;
				}
			}
			return false;
		}      

        internal bool AddEnums(string szName, string szType, object szValue, string szDesc)
        {
            if (m_Enums.ContainsKey(szName))
                return false;

            Enums var = new Enums(szName, szType,szValue, szDesc);
            var.Unit = "";
            return m_Enums.AddVariable(var);
        }

        internal bool AddUserVariable(string szName, string szType, object szValue, object szMinVal, object szMaxVal, string szDesc)
        {
            if (m_UserVars.ContainsKey(szName))
                return false;

            UserVariable var = new UserVariable(szName, szType, szDesc);
            var.Value = szValue;
            var.MinValue = szMinVal;
            var.MaxValue = szMaxVal;
            var.Unit = "";

            return m_UserVars.AddVariable(var);
        }

        private void CreateSystemEnumVars()
        {
            string[] varArr = 
            {
                "매우좋음", "정수", "0", "수질등급",
                "좋음", "정수", "1", "수질등급",
                "약간좋음", "정수", "2", "수질등급",
                "보통", "정수", "3", "수질등급",
                "약간나쁨", "정수", "4", "수질등급",
                "나쁨", "정수", "5", "수질등급",
                "매우나쁨", "정수", "6", "수질등급",
                "레벨1", "정수", "1" , "녹조발생 레벨",
                "레벨2", "정수", "2" , "녹조발생 레벨",
                "레벨3", "정수", "3" , "녹조발생 레벨",
                "레벨4", "정수", "4" , "녹조발생 레벨",
                "레벨5", "정수", "5" , "녹조발생 레벨",
                "레벨6", "정수", "6" , "녹조발생 레벨",
                "레벨7", "정수", "7" , "녹조발생 레벨",
                "레벨8", "정수", "8" , "녹조발생 레벨",
                "레벨9", "정수", "9" , "녹조발생 레벨",
                "레벨10", "정수", "10" , "녹조발생 레벨",
                ""
            };

            try
            {
                for (int i = 3; i < varArr.Length; i += 4)
                {
                    AddEnums(varArr[i - 3], varArr[i - 2], varArr[i - 1], varArr[i]);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message, ex);
            }
        }

        private void CreateSystemVars()
        {        
            string[] varArr = 
            {
                "sensor_PH", "실수", "PH", "수소이온농도",
                "sensor_DO", "실수", "mg/L", "용존산소",
                "sensor_ORP", "실수", "", "ORP",
                "sensor_conductivity", "실수", "ms/cm", "전도도",
                "sensor_depth", "실수", "m", "수심",
                "sensor_temp", "실수",  "°C","수온",
                "sensor_NO3N", "실수",  "mg/L","질산성 질소",
                "sensor_NH4", "실수", "mg/L" , "암모니아성 질소",
                "sensor_TN", "실수",  "mg/L","총 질소",
                "sensor_PO4", "실수",  "mg/L","인산염 인",
                "sensor_TP", "실수",  "mg/L","총 인",
                "sensor_Turbidity", "실수", "", "혼탁도",
                "sensor_Chlorophyll", "실수",  "","염록소",
                "station_PH", "실수",  "PH","수소이온농도",
                "station_DO", "실수",  "mg/L","용존산소",
                "station_TN", "실수",  "mg/L","총 질소",
                "station_TP", "실수",  "mg/L","총 인(T-P)",
                "station_TOC", "실수", "mg/L", "TOC",
                "station_TEMP", "실수", "°C", "수온",
                "station_EC", "실수", "㎛hos/㎝", "전기전도도",
                "station_Chlorophyll_a", "실수", "㎎/㎥", "클로로필 a",
                "station_NH3N", "실수", "mg/L", "암모니아성 질소",
                "station_NO3N", "실수", "mg/L", "질산성 질소",
                "station_PO4P", "실수", "mg/L", "인산염 인",
                "moni_LEVEL", "ENUM", "", "수질등급",
                "pred_LEVEL", "ENUM", "", "모델링 결과 녹조발생 레벨",
                ""
            };

            try
            {
                for (int i = 3; i < varArr.Length; i += 4)
                {
                    Variable var = new Variable(varArr[i - 3], varArr[i - 2], varArr[i]);
                    var.Unit = varArr[i - 1];
                    m_SystemVars.AddVariable(var);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message, ex);
            }
        }
    }

    public interface ISectionPageContainer
    {
        System.Windows.Forms.TabPage OnAddActionStep(ActionStep step);
        System.Windows.Forms.TabPage OnDeleteActionStep(ActionStep step);
        System.Windows.Forms.TabPage OnShowActionStep(ActionStep step);

    }
}
