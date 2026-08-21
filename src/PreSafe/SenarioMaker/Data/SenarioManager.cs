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
                switch (m_nType)
                {
                    case 6:
                        m_szDisasterType = "공통";
                        break;
                    case 1:
                        m_szDisasterType = "강간형";
                        break;
                    case 2:
                        m_szDisasterType = "특수 강간형";
                        break;
                    case 3:
                        m_szDisasterType = "미성년자 강간형";
                        break;
                    case 4:
                        m_szDisasterType = "강제 추행형";
                        break;
                    case 5:
                        m_szDisasterType = "미성년자 강제 추행형";
                        break;
                    default:
                        break;
                }
            }
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

        private string m_szDisasterType = "공통";
        public string DisasterType
        {
            get { return m_szDisasterType; }
            set 
            {
                m_szDisasterType = value;               
                if(m_szDisasterType == "강간형")
                {
                    m_nType = 1;
                }
                else if (m_szDisasterType == "특수 강간형")
                {
                    m_nType = 2;
                }
                else if (m_szDisasterType == "미성년자 강간형")
                {
                    m_nType = 3;
                }
                else if (m_szDisasterType == "강제 추행형")
                {
                    m_nType = 4;
                }
                else if (m_szDisasterType == "미성년자 강제 추행형")
                {
                    m_nType = 5;
                }
                else if (m_szDisasterType == "공통")
                {
                    m_nType = 6;
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

        private void CreateSystemVars()
        {        
            string[] varArr = 
            {
                "ALC", "BOOLEAN", "", "음주 유무",
                "HB", "정수", "회/분", "분당 심장 박동수",
                "ACC", "ENUM", "", "가속도",
                "SND", "BOOLEAN", "", "비명 유무",
                "CL", "ENUM", "", "현재 위치",
                "NAME", "문자열",  "","이름",
                "AGE", "정수",  "","나이",
                //"VEL", "실수", "m/sec", "속도",
                "IMP", "BOOLEAN", "" , "충격 여부",
                "MALE", "BOOLEAN",  "","성별, True이면 남성, False이면 여성",
                "SNUM", "문자열",  "","주민번호 ('-'을 제외하고 숫자만 입력)",
                "TEL", "문자열",  "","전화번호 ('-'을 제외하고 숫자만 입력)",
                "CYEAR", "정수", "", "현재 시간의 년도",
                "CMON", "정수",  "","현재 시간의 달",
                "CDAY", "정수",  "","현재 시간의 날짜",
                "CHOUR", "정수",  "","현재 시간의 시간, 범위: 0 ~ 23",
                "CMIN", "정수",  "","현재 시간의 분, 범위 : 0~59",
                "CSEC", "정수",  "","현재 시간의 초, 범위: 0~59",
                "CWD", "ENUM", "", "현재 시간의 요일",
                "CR", "실수", "", "범죄 가능성 (%)",
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
