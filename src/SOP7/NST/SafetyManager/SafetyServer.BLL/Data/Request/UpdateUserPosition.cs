using System;
using System.Collections.Generic;
using System.Text;

namespace SafetyServer.BLL.Data.Request
{
    public class UpdateUserPosition
    {
        private string m_strID = null;
        private int? m_nZoneID = null;
        private float? x = null;
        private float? y = null;

        public string UserID
        {
            get { return m_strID; }
            set { m_strID = value; }
        }

        public int? FieldID
        {
            get { return m_nZoneID; }
            set { m_nZoneID = value; }
        }

        public float? X
        {
            get { return x; }
            set { x = value; }
        }

        public float? Y
        {
            get { return y; }
            set { y = value; }
        }
    }
}
