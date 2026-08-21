using System;

namespace Vacation.BLL.Models
{
    [Serializable]
    public class Result
    {
        private bool m_result = false;

        public bool Success
        {
            get { return m_result; }
            set { m_result = value; }
        }

        public Result()
        {
        }

        public Result(bool success)
        {
            m_result = success;
        }
    }

    [Serializable]
    public class MessageResult : Result
    {
        private string m_strMessage = "";

        public string Message
        {
            get { return m_strMessage; }
            set { m_strMessage = value; }
        }

        public MessageResult()
        {
        }

        public MessageResult(bool success, string strMessage)
            : base(success)
        {
            m_strMessage = strMessage;
        }
    }
}
