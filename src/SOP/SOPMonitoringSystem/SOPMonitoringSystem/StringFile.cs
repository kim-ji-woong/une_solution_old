using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SOPMonitoringSystem
{
    public class StringFile
    {
        private string m_strData = "";
        int m_nCurrentIndex = 0;

        public StringFile()
        {
        }

        public StringFile(string strData)
        {
            m_strData = strData;
        }

        public void SetData(string strData)
        {
            m_strData = strData;
            m_nCurrentIndex = 0;
        }

        public bool ReadLine(ref string strLine)
        {
            if (m_strData == null)
                return false;

            int nLen = m_strData.Length;
            if (m_nCurrentIndex >= nLen)
                return false;

            int nIndex = m_strData.IndexOf('\n', m_nCurrentIndex);

            if (nIndex < 0)
            {
                m_nCurrentIndex = nLen;
                strLine = m_strData.Substring(m_nCurrentIndex);
                return true;
            }

            strLine = m_strData.Substring(m_nCurrentIndex, nIndex - m_nCurrentIndex);
            strLine = Utility.TrimString(strLine);

            m_nCurrentIndex = nIndex + 1;
            return true;
        }
    }
}
