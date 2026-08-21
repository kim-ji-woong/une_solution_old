using System.Collections;

namespace PersonalSOP.Network
{
    public class InjuryMessage : Message
    {
        private int m_nActionStepHistoryID = -1;
        private int m_nProcessID = -1;

        public int nActionStepHistoryID
        {
            get { return m_nActionStepHistoryID; }
        }
        public int nProcessID
        {
            get { return m_nProcessID; }
        }

        public InjuryMessage(int nActionStepHistoryID, int nProcessID)
        {
            m_nActionStepHistoryID = nActionStepHistoryID;
            m_nProcessID = nProcessID;
        }

        public override int GetHeader()
        {
            return SOPWebServer.Header.SELECT_SOP_COMPONENT;
        }
        public override byte[] GetBytes()
        {
            ArrayList arrDatas = new ArrayList();

            arrDatas.Add(m_nActionStepHistoryID);
            arrDatas.Add("Process");
            arrDatas.Add(m_nProcessID);
            return SOPWebServer.BinaryHelper.MakeBytes(arrDatas);
        }

        public override bool SendToSOPSimulator()
        {
            return true;
        }
    }
}