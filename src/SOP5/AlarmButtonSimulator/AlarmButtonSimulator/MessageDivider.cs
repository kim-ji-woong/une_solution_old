using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace AlarmButtonSimulator
{
    public class MessageDivider
    {
        // 메시지 최대 개수의 자리수
        private int m_nMaxSegmentSize = 2;
        // 메시지의 길이제한 바이트 수
        private int m_nMessageLength = 80;

        // 메시지 최대 개수의 자리수
        // 메시지가 최대로 쪼개어질 수 있는 개수는 10 ^ MaxSegmentSize - 1
        public int MaxSegmentSize
        {
            get { return m_nMaxSegmentSize; }
            set { m_nMaxSegmentSize = value; }
        }

        // 메시지의 길이제한 바이트 수
        public int MessageLength
        {
            get { return m_nMessageLength; }
            set { m_nMessageLength = value; }
        }

        public MessageDivider()
        {
        }

        public MessageDivider(int nMessageLength)
        {
            m_nMessageLength = nMessageLength;
        }

        public ArrayList MakeMessageList(string strMsg)
        {
            ArrayList arrMessages = new ArrayList();

            if (JustOneMessage(strMsg))
                arrMessages.Add(strMsg);
            else
                SetMessageList(strMsg, arrMessages);

            foreach (string str in arrMessages)
            {
                System.Diagnostics.Trace.WriteLine(str);
            }

            return arrMessages;
        }

        private bool JustOneMessage(string strMsg)
        {
            int nByteLength = 0;
            int nLen = strMsg.Length;

            for (int i = 0; i < nLen; i++)
            {
                if (strMsg.ElementAt(i) < 256)
                    nByteLength++;
                else
                    nByteLength += 2;
            }

            if (nByteLength <= m_nMessageLength)
                return true;

            return false;
        }

        private void SetMessageList(string strMsg, ArrayList arrMessages)
        {
            char ch = (char)6;

            // m_nMaxSegmentSize가 3이면 999개까지 메시지가 쪼개어 지는 것이 가능하다.
            for (int i = 1; i <= m_nMaxSegmentSize; i++)
            {
                arrMessages.Clear();

                if (SetMessageList(strMsg, arrMessages, ch, i))
                    break;
            }
        }

        private bool SetMessageList(string strMsg, ArrayList arrMessages, char ch, int nDepth)
        {
            string strTag = ch.ToString();

            for (int i = 1; i < nDepth; i++)
            {
                strTag += ch.ToString();
            }

            int nIndex = 0;
            int nByteInit = strTag.Length + 3;
            int nByteLength = nByteInit + ((nIndex + 1) / 10) + 1;
            int nLen = strMsg.Length;
            int nBeginIndex = 0;

            for (int i = 0; i < nLen; i++)
            {
                if (strMsg.ElementAt(i) < 256)
                    nByteLength++;
                else
                    nByteLength += 2;

                if (nByteLength == m_nMessageLength ||
                ((nByteLength == (m_nMessageLength - 1)) && (i < nLen - 1 && strMsg.ElementAt(i + 1) >= 256)))
                {
                    if ((nIndex + 1) / 10 >= nDepth)
                    {
                        ReplaceMessage(arrMessages, strTag, nIndex);
                        return false;
                    }

                    string str = string.Format("[{0}/{1}]{2}", ++nIndex, strTag, strMsg.Substring(nBeginIndex, i - nBeginIndex + 1));
                    arrMessages.Add(str);
                    nBeginIndex = i + 1;
                    nByteLength = nByteInit + ((nIndex + 1) / 10) + 1;
                }
            }

            if (nByteLength > nByteInit + ((nIndex + 1) / 10) + 1)
            {
                if ((nIndex + 1) / 10 >= nDepth)
                {
                    ReplaceMessage(arrMessages, strTag, nIndex);
                    return false;
                }

                string str = string.Format("[{0}/{1}]{2}", ++nIndex, strTag, strMsg.Substring(nBeginIndex));
                arrMessages.Add(str);
            }

            ReplaceMessage(arrMessages, strTag, nIndex);
            return true;
        }

        private void ReplaceMessage(ArrayList arrMessages, string strTag, int nIndex)
        {
            int nMessageCount = arrMessages.Count;
            string strTag2 = nIndex.ToString();

            for (int i = 0; i < nMessageCount; i++)
            {
                arrMessages[i] = ((string)arrMessages[i]).Replace(strTag, strTag2);
            }
        }
    }
}
