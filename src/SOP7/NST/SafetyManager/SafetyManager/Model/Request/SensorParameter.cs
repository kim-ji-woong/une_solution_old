using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SafetyManager.Model.Request
{
    public class SensorParameter
    {
        private int m_nHeader = 0;
        private string m_strClientInfo = "";
        private List<string> m_values = new List<string>();

        public int Header
        {
            get { return m_nHeader; }
            set { m_nHeader = value; }
        }

        public string ClientInfo
        {
            get { return m_strClientInfo; }
            set { m_strClientInfo = value; }
        }

        public List<string> Values
        {
            get { return m_values; }
            set { m_values = value; }
        }
    }
}
