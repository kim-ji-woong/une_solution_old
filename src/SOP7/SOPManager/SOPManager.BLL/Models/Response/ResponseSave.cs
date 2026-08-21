namespace SOPManager.BLL.Models.Response
{
    using SOP;

    public class ResponseSave : MessageResult
    {
        private SOPData m_sopData = null;

        // XML 옵션
        private string m_strXMLData = "";
        private string m_strXMLFileName = "";

        public SOPData SOPData
        {
            get { return m_sopData; }
            set { m_sopData = value; }
        }

        public string XMLData
        {
            get { return m_strXMLData; }
            set { m_strXMLData = value; }
        }

        public string XMLFileName
        {
            get { return m_strXMLFileName; }
            set { m_strXMLFileName = value; }
        }
    }
}
