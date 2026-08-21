using System.Collections.Generic;

namespace SensorMaker.BLL.Models.Request
{
    public class RequestUploadModelFile
    {
        private int m_nUserID = -1;
        private string m_strUserName = "";
        // Temp Folder에 있는 파일들을 모두 삭제하고 업로드를 진행하지 않는다.
        private bool m_cancelTempFiles = true;
        // Upload Folder에 있는 파일들은 그대로 둔 상태에서 Temp Folder에 있는 파일들을 모두 Upload Folder로 옮긴다.(같은 이름의 파일은 덮어쓴다.)
        private bool m_appendFiles = false;
        // Upload Folder에 있는 파일들을 모두 지우고, Temp Folder에 있는 파일들을 모두 Upload Folder로 옮긴다.
        private bool m_removeNCopy = true;
        private List<string> m_fileNames = new List<string>();

        public int UserID
        {
            get { return m_nUserID; }
            set { m_nUserID = value; }
        }

        public string UserName
        {
            get { return m_strUserName; }
            set { m_strUserName = value; }
        }

        // Temp Folder에 있는 파일들을 모두 삭제하고 업로드를 진행하지 않는다.
        public bool CancelTempFiles
        {
            get { return m_cancelTempFiles; }
            set { m_cancelTempFiles = value; }
        }

        // Upload Folder에 있는 파일들은 그대로 둔 상태에서 Temp Folder에 있는 파일들을 모두 Upload Folder로 옮긴다.(같은 이름의 파일은 덮어쓴다.)
        public bool AppendFiles
        {
            get { return m_appendFiles; }
            set { m_appendFiles = value; }
        }

        // Upload Folder에 있는 파일들을 모두 지우고, Temp Folder에 있는 파일들을 모두 Upload Folder로 옮긴다.
        public bool RemoveNCopy
        {
            get { return m_removeNCopy; }
            set { m_removeNCopy = value; }
        }

        public List<string> FileNames
        {
            get { return m_fileNames; }
            set { m_fileNames = value; }
        }
    }
}
