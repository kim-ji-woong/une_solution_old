using System.Collections.Generic;

namespace SDMS.BLL.Models.Response
{
    public class ResponseUpdateFakeWall : MessageResult
    {
        private int m_nID = -1;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public ResponseUpdateFakeWall()
        {
        }

        public ResponseUpdateFakeWall(bool success, string strMessage)
        {
            Success = success;
            Message = strMessage;
        }
    }

    public class ResponseUpdateFakeWalls : MessageResult
    {
        private List<int> m_ids = new List<int>();

        public List<int> IDs
        {
            get { return m_ids; }
            set { m_ids = value; }
        }

        public ResponseUpdateFakeWalls()
        {
        }

        public ResponseUpdateFakeWalls(bool success, string strMessage)
        {
            Success = success;
            Message = strMessage;
        }
    }
}
