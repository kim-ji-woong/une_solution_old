using SensorMaker.BLL.Models.Response;

namespace SensorMaker.BLL.Models.Account
{
    public class RegisterResult : MessageResult
    {
        private bool m_isNewUser = false;
        private bool m_changePassword = false;
        private bool m_registerAdminUser = false;

        // 아직 등록되지 않은 사용자인가?
        // 이 값이 true이면 관리자의 승인을 기다리는 상태가 된다.
        public bool IsNewUser
        {
            get { return m_isNewUser; }
            set { m_isNewUser = value; }
        }

        // 비밀번호 변경을 하려고 하는가?
        public bool ChangePassword
        {
            get { return m_changePassword; }
            set { m_changePassword = value; }
        }

        // 관리자로 등록하는가?
        public bool RegisterAdminUser
        {
            get { return m_registerAdminUser; }
            set { m_registerAdminUser = value; }
        }

        public RegisterResult()
            : base()
        {
        }

        public RegisterResult(bool success, string strMessage)
            : base(success, strMessage)
        {
        }
    }
}
