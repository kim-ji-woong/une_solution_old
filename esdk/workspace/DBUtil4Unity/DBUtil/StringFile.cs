using System;
using System.Collections.Generic;
using System.Text;

namespace DBUtility
{
    public class StringFile
    {
        private string m_strData = "";
        private int m_nCurrentIndex = 0;
        private bool m_isWorking = false;

        private int m_nColumn = 0;
        public int Column
        {
            get { return m_nColumn; }
        }
        private int m_nRow = 0;
        public int Row
        {
            get { return m_nRow; }
        }

        public StringFile()
        {
        }

        public StringFile(string strData)
        {
            m_strData = strData;
            FindDataInfo();
        }

        public void SetData(string strData)
        {
            m_strData = strData;
            m_nCurrentIndex = 0;

            FindDataInfo();
        }

        private void FindDataInfo()
        {
            int nStart = m_strData.IndexOf("Begin Info :", 0);
            if( nStart >= 0)
            {
                // 시작위치는 Begin Info : 다음부터 임
                nStart += 12;
                int nEnd = m_strData.IndexOf("End Info", 0);
                int nLength = nEnd - nStart;
                string szValue = m_strData.Substring(nStart, nLength);
                string []szValues = szValue.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (szValues == null || szValues.Length < 2)
                    return;

                m_nColumn = Convert.ToInt32(szValues[0]);
                m_nRow = Convert.ToInt32(szValues[1]);
            }
            else
            {
                m_nColumn = 0;
                m_nRow = 0;
            }
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

                int nTagLength;
                int a = GetBeginDataIndex(strLine, out nTagLength);

                if (a >= 0)
                {
                    int b = strLine.IndexOf("]:#$*_", a + nTagLength);
                    //int b = strLine.IndexOf(']', a + nTagLength);

                    if (b > 0)
                    {
                        if (a + nTagLength == b)
                            strLine = "";
                        else
                            strLine = strLine.Substring(a + nTagLength, b - (a + nTagLength));
                    }
                }

                /*int a = strLine.IndexOf('[');
                int b = strLine.LastIndexOf(']');

                if (a == -1 || b == -1)
                {

                }
                else if (a + 1 == b)
                {
                    strLine = "";
                }
                else
                    strLine = strLine.Substring(a + 1, strLine.Length - (a + 2));*/

                m_nCurrentIndex = nIndex + 1;
                m_isWorking = false;
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        private int GetBeginDataIndex(string strLine, out int nTagLength)
        {
            string strTag = "_*$#:[";
            nTagLength = strTag.Length;

            int nIndex = strLine.IndexOf(strTag);
            return nIndex;
            /*string[] tags = { "BYTE:[", "SHORT:[", "INT:[", "LONG:[", "FLOAT:[", "DOUBLE:[", "CHAR:[", "BOOLEAN:[", "STRING:[", "TEXT:[", "DATETIME:[",
                              "byte:[", "short:[", "int:[", "long:[", "float:[", "double:[", "char:[", "boolean:[", "string:[", "text:[", "datetime:[" };
            nTagLength = 0;

            foreach (string tag in tags)
            {
                int nIndex = strLine.IndexOf(tag);

                if (nIndex >= 0)
                {
                    nTagLength = tag.Length;
                    return nIndex;
                }
            }

            return -1;*/
        }
    }
}
