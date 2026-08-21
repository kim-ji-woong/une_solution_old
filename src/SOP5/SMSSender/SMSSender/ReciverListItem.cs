using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnE.SOP;

namespace SMSSender
{
    public class ReciverListItem : IComparer
    {

        public ReciverListItem(object o,int nType)
        {
            m_nType = nType;
            m_TargetObject = o;
        }

        // 1: 정규팀원, 2: 외부팀원, 3: 정규팀, 4: 외부팀, 5: 수동입력
        private int m_nType = -1;
        public int Type
        {
            get { return m_nType; }
        }

        private object m_TargetObject = null;
        public object TargetObject
        {
            get { return m_TargetObject; }
        }

        public int Compare(object x, object y)
        {
            return Compare((ReciverListItem)x, (ReciverListItem)y);
        }

        public int Compare(ReciverListItem x, ReciverListItem y)
        {
            if( x.m_nType == y.m_nType )
            {
                if (x.m_TargetObject == y.m_TargetObject)
                    return 0;
            }
            if (x.m_nType > y.m_nType)
                return 1;

            return -1;                
        }

        
        public override string ToString()
        {
            string szResult = "";
            switch(m_nType)
            {
                case 1:
                    Data_CompanyMember member = (Data_CompanyMember) m_TargetObject;
                    szResult = member.MemberName;
                    break;
                case 2:
                    ExternalCompanyMember member2 = (ExternalCompanyMember)m_TargetObject;
                    szResult = member2.MemberName;
                    break;
                case 3:
                    Data_RegularTeam team1 = (Data_RegularTeam)m_TargetObject;
                    szResult = team1.TeamName;
                    break;
                case 4:
                    ExternalCompanyTeam team2 = (ExternalCompanyTeam)m_TargetObject;
                    szResult = team2.TeamName;
                    break;
                case 5:
                    szResult = m_TargetObject.ToString();
                    break;
                default:
                    szResult = "";
                    break;
            }
            return szResult;
        }
    }
}
