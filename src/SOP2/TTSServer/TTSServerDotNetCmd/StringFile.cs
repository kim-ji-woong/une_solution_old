using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TTSServerDotNetCmd
{
    public class StringFile
    {
        private string m_strData = "";
        private int m_nCurrentIndex = 0;
        private bool m_isWorking = false;

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

        private void CheckWorking()
        {
            while (m_isWorking)
                System.Threading.Thread.Sleep(100);
        }

        public bool ReadLine(ref string strLine)
        {
            if (m_strData == null)
                return false;

            int nLen = m_strData.Length;
            if (m_nCurrentIndex >= nLen)
                return false;

            CheckWorking();
            m_isWorking = true;

            try
            {
                int nIndex = m_strData.IndexOf('\n', m_nCurrentIndex);

                if (nIndex < 0)
                {
                    m_nCurrentIndex = nLen;
                    //strLine = m_strData.Substring(m_nCurrentIndex);
                    strLine = "";
                    m_isWorking = false;
                    return true;
                }

                if (nIndex <= m_nCurrentIndex)
                {
                    m_isWorking = false;
                    return false;
                }

                strLine = m_strData.Substring(m_nCurrentIndex, nIndex - m_nCurrentIndex);
                strLine = Utility.TrimString(strLine);

                m_nCurrentIndex = nIndex + 1;
                m_isWorking = false;
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
    }
}
