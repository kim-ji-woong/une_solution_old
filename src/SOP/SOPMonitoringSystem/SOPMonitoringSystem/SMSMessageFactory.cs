using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace SOPMonitoringSystem
{
    public class SMSMessageFactory
    {
        private static int m_nMessageID = 0;

        // section 및 section의 하부 section들에 대하여 메시지 리스트를 만든다.
        // GROUP_SECTION 속성이 있는 section들만 해당된다.
        // Return 값 : 생성된 sms 메시지 개수
        public static int MakeMessageList(SectionEx section)
        {
            if (section == null)
                return 0;

            if (section.Type == SectionEx.SectionType.GROUP_SECTION)
            {
                ArrayList arrMissions = section.MissionData.Missions;

                string strHeader;
                int nID = GetMessageHeader(arrMissions, out strHeader);

                string strMsg = strHeader;
                string strCheckItemMsg = "";

                foreach (MemberofSection.MissionofSection mission in arrMissions)
                {
                    strMsg += string.Format("{0}\t{1}\t{2}\t{3}\t", mission.Division, mission.TaskName, mission.Report, mission.Location);

                    ArrayList arrCheckItems = mission.CheckItems;

                    foreach (MemberofSection.CheckofMission check in arrCheckItems)
                    {
                        strCheckItemMsg += string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t", check.Category, check.SubCategory, check.TaskName, check.Count == null ? "-1" : check.Count, check.Location);
                    }
                }

                strMsg += strCheckItemMsg;
                return DivideMessage(strHeader, strMsg, section.SMSMessages, nID, 80);
            }

            int nMessageCount = 0;
            ArrayList arrChilds = section.GetChildSections();

            foreach (SectionEx child in arrChilds)
            {
                nMessageCount += MakeMessageList(child);
            }

            return nMessageCount;
        }

        // strLongMsg를 nMessageSize 크기만큼 나누어서 arrMessages에 담는다.
        private static int DivideMessage(string strHeader, string strLongMsg, ArrayList arrMessages, int nID, int nMessageSize)
        {
            int nHeaderLength = strHeader.Length;
            int nByteCount = nHeaderLength;

            int nLen = strLongMsg.Length;
            int nPrevIndex = nHeaderLength;

            string strAdd = strHeader;
            int nMessageIndex = 1;

            for (int i = nHeaderLength; i <= nLen; i++)
            {
                if (i < nLen)
                {
                    if ((ushort)strLongMsg.ElementAt(i) < 256)
                        nByteCount++;
                    else
                        nByteCount += 2;
                }

                if (nByteCount > nMessageSize || i == nLen)
                {
                    string strMsg = strAdd + strLongMsg.Substring(nPrevIndex, i - nPrevIndex);
                    arrMessages.Add(strMsg);

                    strAdd = string.Format("{0:X3}{1:X3}", ++nMessageIndex, nID);
                    nByteCount = strAdd.Length;

                    nPrevIndex = i;
                }
            }

            return arrMessages.Count;
        }

        private static int NewMessageID()
        {
            if (m_nMessageID >= 0xfff)
                m_nMessageID = 1;
            else
                m_nMessageID++;

            return m_nMessageID;
        }

        // Return값 : 메시지 식별자
        private static int GetMessageHeader(ArrayList arrMissions, out string strHeader)
        {
            int nID = NewMessageID();
            int nMissionCount = 0, nCheckItemCount = 0;

            foreach (MemberofSection.MissionofSection mission in arrMissions)
            {
                nCheckItemCount += mission.CheckItems.Count;
            }

            nMissionCount += arrMissions.Count;

            // 메시지 번호는 Header 이므로 001이 된다. 위기 상황 단계는 S01이어야 하는데, 임무에 관한 메시지이므로
            // 001로 표기한다.
            strHeader = string.Format("001{0:X3}{1:X3}{2:X3}\t", nID, nMissionCount, nCheckItemCount);
            return nID;
        }
    }
}
