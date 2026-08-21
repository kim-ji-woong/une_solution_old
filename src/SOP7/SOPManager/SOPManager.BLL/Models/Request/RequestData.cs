using System;
using System.Collections.Generic;

namespace SOPManager.BLL.Models.Request
{
    using SOP;
    using SOPManager.BLL.Models.Response;
    using SOPManager.Model.Sop.Account;

    public class RequestData
    {
        public enum ContentsType { DB = 0, XML };

        private RequestDisasterCategories m_requestDisasterCategories = null;
        private RequestDefault m_requestDefault = null;
        private RequestDisasterVersions m_requestDisasterVersions = null;
        private RequestSave m_requestSave = null;
        private RequestOpen m_requestOpen = null;
        private RequestDelete m_requestDelete = null;
        private RequestExternalProgram m_requestExternalProgram = null;
        private RequestOption m_requestOption = null;
        private RequestSaveOption m_requestSaveOption = null;
        private RequestParseSpecialMessage m_requestParseSpecialMessage = null;
        private bool? m_requestSpecialMessageList = null;

        public RequestDisasterCategories RequestDisasterCategories
        {
            get { return m_requestDisasterCategories; }
            set { m_requestDisasterCategories = value; }
        }

        public RequestDefault RequestDefault
        {
            get { return m_requestDefault; }
            set { m_requestDefault = value; }
        }

        public RequestDisasterVersions RequestDisasterVersions
        {
            get { return m_requestDisasterVersions; }
            set { m_requestDisasterVersions = value; }
        }

        public RequestSave RequestSave
        {
            get { return m_requestSave; }
            set { m_requestSave = value; }
        }

        public RequestOpen RequestOpen
        {
            get { return m_requestOpen; }
            set { m_requestOpen = value; }
        }

        public RequestDelete RequestDelete
        {
            get { return m_requestDelete; }
            set { m_requestDelete = value; }
        }

        public RequestExternalProgram RequestExternalProgram
        {
            get { return m_requestExternalProgram; }
            set { m_requestExternalProgram = value; }
        }

        public RequestOption RequestOption
        {
            get { return m_requestOption; }
            set { m_requestOption = value; }
        }

        public RequestSaveOption RequestSaveOption
        {
            get { return m_requestSaveOption; }
            set { m_requestSaveOption = value; }
        }

        public RequestParseSpecialMessage RequestParseSpecialMessage
        {
            get { return m_requestParseSpecialMessage; }
            set { m_requestParseSpecialMessage = value; }
        }

        public bool? RequestSpecialMessageList
        {
            get { return m_requestSpecialMessageList; }
            set { m_requestSpecialMessageList = value; }
        }
    }

    public class RequestDisasterCategories
    {
        private bool m_isNormal = true;

        public bool IsNormal
        {
            get { return m_isNormal; }
            set { m_isNormal = value; }
        }
    }

    public class RequestDefault
    {
        private bool m_requestStepMember = false;
        private bool m_requestActionSteps = false;

        public bool RequestStepMember
        {
            get { return m_requestStepMember; }
            set { m_requestStepMember = value; }
        }

        public bool RequestActionSteps
        {
            get { return m_requestActionSteps; }
            set { m_requestActionSteps = value; }
        }
    }

    public class RequestDisasterVersions
    {
        // DisasterID와 isNormal이 서로 다를 경우
        // 주간/야간이 바뀌는 경우가 된다.
        private int m_nDisasterID = -1;
        private bool m_isNormal = true;

        public int DisasterID
        {
            get { return m_nDisasterID; }
            set { m_nDisasterID = value; }
        }

        public bool IsNormal
        {
            get { return m_isNormal; }
            set { m_isNormal = value; }
        }
    }

    public class RequestSave
    {
        private int m_nTarget = (int)RequestData.ContentsType.DB;
        private int m_nUserID = -1;
        private SOPData m_sopData = null;

        public int Target
        {
            get { return m_nTarget; }
            set { m_nTarget = value; }
        }

        public int UserID
        {
            get { return m_nUserID; }
            set { m_nUserID = value; }
        }

        public SOPData SOPData
        {
            get { return m_sopData; }
            set { m_sopData = value; }
        }
    }

    public class RequestOpen
    {
        private int m_nTarget = (int)RequestData.ContentsType.DB;

        // DB 옵션
        private int m_nVersionID = -1;

        // XML 옵션
        private string m_strXMLData = "";

        public int Target
        {
            get { return m_nTarget; }
            set { m_nTarget = value; }
        }

        public int VersionID
        {
            get { return m_nVersionID; }
            set { m_nVersionID = value; }
        }

        public string XMLData
        {
            get { return m_strXMLData; }
            set { m_strXMLData = value; }
        }
    }

    public class RequestDelete
    {
        private List<int> m_versionIDs = new List<int>();

        public List<int> VersionIDs
        {
            get { return m_versionIDs; }
            set { m_versionIDs = value; }
        }
    }

    public class RequestExternalProgram
    {
        // -1이면 전체 Program List를 요청한다.
        private int m_nProgramID = -1;

        // -1이면 전체 Program List를 요청한다.
        public int ProgramID
        {
            get { return m_nProgramID; }
            set { m_nProgramID = value; }
        }
    }

    public class RequestOption
    {
        private int m_nUserID = -1;
        public int UserID
        {
            get { return m_nUserID; }
            set { m_nUserID = value; }
        }

        private string m_strCategory = "";
        public string Category
        {
            get { return m_strCategory; }
            set { m_strCategory = value; }
        }
    }

    public class RequestSaveOption
    {
        private Option m_saveOption = null;
        public Option SaveOption
        {
            get { return m_saveOption; }
            set { m_saveOption = value; }
        }
    }

    public class RequestParseSpecialMessage
    {
        // 전체 메시지
        private string m_strMessage = "";
        // 재난발생 시간
        private string m_strTime = "";
        // 재난발생 장소
        private string m_strLocation = "";
        // true이면 실제모드, false이면 훈련모드
        private bool? m_isRealMode = null;
        // true이면 평일모드, false이면 야간 및 휴일모드
        private bool? m_isNormalMode = null;
        // 다양한 재난상황 및 데이터를 표현하기 위한 변수 List
        // 변수 : Key + ";" + Value
        //        Key => 재난 데이터 이름(대소문자를 구분하지 않는다.)
        //        Value => 재난 데이터
        private List<string> m_variables = new List<string>();

        public string Message
        {
            get { return m_strMessage; }
            set { m_strMessage = value; }
        }

        public string Time
        {
            get { return m_strTime; }
            set { m_strTime = value; }
        }

        public string Location
        {
            get { return m_strLocation; }
            set { m_strLocation = value; }
        }

        public bool? IsRealMode
        {
            get { return m_isRealMode; }
            set { m_isRealMode = value; }
        }

        public bool? IsNormalMode
        {
            get { return m_isNormalMode; }
            set { m_isNormalMode = value; }
        }

        public List<string> Variables
        {
            get { return m_variables; }
        }

        public void AddVariable(string strKey, string strValue)
        {
            m_variables.Add(strKey + ";" + strValue);
        }

        public static bool GetVariableData(string strVariable, out string strKey, out string strValue)
        {
            strKey = strValue = null;

            int nIndex = strVariable.IndexOf(';');

            if (nIndex < 0)
                return false;

            strKey = strVariable.Substring(0, nIndex).Trim();
            strValue = strValue.Substring(nIndex + 1).Trim();
            return true;
        }

        public DateTime? GetTime(out string strErrorMessage)
        {
            try
            {
                strErrorMessage = null;
                DateTime time = Convert.ToDateTime(m_strTime);
                return time;
            }
            catch (Exception)
            {
                strErrorMessage = string.Format("DateTime Instance를 생성할 수 없는 문자열입니다. : {0}", m_strTime);
                System.Diagnostics.Trace.WriteLine(strErrorMessage);
            }

            return null;
        }
    }

    public class RequestAccountUser
    {
        private List<AccountUser> m_accountUsers = null;

        public List<AccountUser> AccountUsers
        {
            get { return m_accountUsers; }
            set { m_accountUsers = value; }
        }

        // 권한 부여한 로그인 ID
        private int m_nAccessedUserID = -1;

        public int AccessedUserID
        {
            get { return m_nAccessedUserID; }
            set { m_nAccessedUserID = value; }
        }
    }
}
