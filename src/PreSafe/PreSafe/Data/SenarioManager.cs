using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PreSafe
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
            set { m_nType = value; }
        }

        protected string m_szSenarioFilePath = "";
        internal string SenarioFilePath
        {
            get { return m_szSenarioFilePath; }
            set { m_szSenarioFilePath = value; }
        }


        private SenarioManager()
        {
            CreateSystemVars();
        }
                
        internal void NewSenario(string szSenarioName, int nType)
        {
            m_szSenarioName = szSenarioName;
            m_nType = nType;

            // 파일Path의 초기화
            m_szSenarioFilePath = "";
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
                "ALC", "실수", "", "알코올 수치, 범위: 0 ~ 10",
                "HB", "정수", "", "분당 심장 박동수",
                "ACC", "실수", "m/sec^2", "가속도",
                "SND", "실수", "dB", "소리",
                "CL", "ENUM", "", "현재 위치",
                "NAME", "문자열",  "","이름",
                "AGE", "정수",  "","나이",
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
}
