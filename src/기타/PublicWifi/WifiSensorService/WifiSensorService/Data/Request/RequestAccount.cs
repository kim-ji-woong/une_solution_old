using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WifiSensorService.Data.Request
{
    public class RequestAccount
    {
        private bool? m_requestManagerList = null;
        private RequestCreateManager m_requestCreateManager = null;
        private RequestUpdatePassword m_requestUpdatePassword = null;
        private RequestRemoveManager m_requestRemoveManager = null;

        public bool? RequestManagerList
        {
            get { return m_requestManagerList; }
            set { m_requestManagerList = value; }
        }

        public RequestCreateManager CreateManager
        {
            get { return m_requestCreateManager; }
            set { m_requestCreateManager = value; }
        }

        public RequestUpdatePassword UpdatePassword
        {
            get { return m_requestUpdatePassword; }
            set { m_requestUpdatePassword = value; }
        }

        public RequestRemoveManager RemoveManager
        {
            get { return m_requestRemoveManager; }
            set { m_requestRemoveManager = value; }
        }
    }

    public class RequestUpdatePassword
    {
        private string m_strID = "";
        private string m_strOldPass = "";
        private string m_strNewPass = "";

        public string Id
        {
            get { return m_strID; }
            set { m_strID = value; }
        }

        public string OldPass
        {
            get { return m_strOldPass; }
            set { m_strOldPass = value; }
        }

        public string NewPass
        {
            get { return m_strNewPass; }
            set { m_strNewPass = value; }
        }
    }

    public class RequestRemoveManager
    {
        private string m_strID = "";

        public string Id
        {
            get { return m_strID; }
            set { m_strID = value; }
        }
    }

    public class RequestCreateManager
    {
        private string m_strID = "";
        private string m_strName = "";
        private string m_strType = "";
        private string m_strNote = "";
        private string m_strPass = "";

        public string Id
        {
            get { return m_strID; }
            set { m_strID = value; }
        }

        public string Name
        {
            get { return m_strName; }
            set { m_strName = value; }
        }

        public string Type
        {
            get { return m_strType; }
            set { m_strType = value; }
        }

        public string Note
        {
            get { return m_strNote; }
            set { m_strNote = value; }
        }

        public string Pass
        {
            get { return m_strPass; }
            set { m_strPass = value; }
        }
    }
}
