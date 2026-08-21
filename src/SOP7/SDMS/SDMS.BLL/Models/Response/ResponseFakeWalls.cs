using System.Collections.Generic;

namespace SDMS.BLL.Models.Response
{
    using Model.Spatial;

    public class ResponseFakeWalls : MessageResult
    {
        private int m_nZoneID = -1;
        private List<FakeWall> m_fakeWalls = new List<FakeWall>();

        public int ZoneID
        {
            get { return m_nZoneID; }
            set { m_nZoneID = value; }
        }

        public List<FakeWall> FakeWalls
        {
            get { return m_fakeWalls; }
            set { m_fakeWalls = value; }
        }

        public ResponseFakeWalls()
        {
        }

        public ResponseFakeWalls(bool success, string strMessage)
        {
            Success = success;
            Message = strMessage;
        }
    }
}
