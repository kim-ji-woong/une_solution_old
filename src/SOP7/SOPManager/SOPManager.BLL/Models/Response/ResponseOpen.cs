namespace SOPManager.BLL.Models.Response
{
    using SOP;

    public class ResponseOpen : MessageResult
    {
        private SOPData m_sopData = null;

        public SOPData SOPData
        {
            get { return m_sopData; }
            set { m_sopData = value; }
        }
    }
}
