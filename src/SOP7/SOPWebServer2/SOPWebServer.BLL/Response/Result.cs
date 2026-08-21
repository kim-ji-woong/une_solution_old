using System.Collections.Generic;

namespace SOPWebServer.BLL.Response
{
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

        public MessageResult(bool success, string message)
        {
            Success = success;
            m_strMessage = message;
        }
    }

    public class MessageValueResult : MessageResult
    {
        private List<string> m_values = new List<string>();

        public List<string> Values
        {
            get { return m_values; }
            set { m_values = value; }
        }

        public MessageValueResult()
        {
        }

        public MessageValueResult(bool success, string message)
        {
            Success = success;
            Message = message;
        }
    }

    public class ValueResult : Result
    {
        private List<string> m_values = new List<string>();

        public List<string> Values
        {
            get { return m_values; }
            set { m_values = value; }
        }

        public ValueResult()
        {
        }

        public ValueResult(bool success)
        {
            Success = success;
        }
    }
}
