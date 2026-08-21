namespace SOPManager.Model.Sop.Config
{
    public class ExternalProgram
    {
        public enum Fields { ID, ExeName, Description, InstallPath };

        private int m_nID = -1;
        // 확장자까지 포함한 전체 경로
        private string m_strExeName = "";
        private string m_strDescription = "";
        // 프로그램이 설치된 경로(ExeName을 제외한 폴더 위치만 알려준다.) 이 값이 NULL이면 기본 설정 위치를 사용한다.
        private string m_strInstallPath = null;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        // 확장자까지 포함한 전체 경로
        public string ExeName
        {
            get { return m_strExeName; }
            set { m_strExeName = value; }
        }

        public string Description
        {
            get { return m_strDescription; }
            set { m_strDescription = value; }
        }

        // 프로그램이 설치된 경로(ExeName을 제외한 폴더 위치만 알려준다.) 이 값이 NULL이면 기본 설정 위치를 사용한다.
        public string InstallPath
        {
            get { return m_strInstallPath; }
            set { m_strInstallPath = value; }
        }

        public static string TableName
        {
            get { return "SopConfigExternalProgram"; }
        }

        public static string GetFieldName(Fields field, out bool isNullable)
        {
            if (field == Fields.InstallPath)
                isNullable = true;
            else
                isNullable = false;

            return field.ToString();
        }
    }
}
