using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sections
{
    public class SectionDataTransSOP : SectionData
    {
        // 전환될 SOP
        protected int m_nLinkedActionStepID = -1;
        protected string m_Description = "";

        // Default 문자열을 사용하여 작성된 ID 개수
        protected static Dictionary<string, int> DEFAULT_ID_COUNT = new Dictionary<string, int>();

        public static void ClearIDCount()
        {
            DEFAULT_ID_COUNT.Clear();
        }

        public override void SetDefaultID(string strStepName, string strTeamName)
        {
            MakeDefaultID(strStepName, strTeamName, DEFAULT_ID_COUNT, "TransSOP");
        }

        protected override void AddDefaultID(string strTag, int nTagCount)
        {
            DEFAULT_ID_COUNT[strTag] = nTagCount;
        }

        // nTagCount가 strTag에 대한 최대값이면 최대값을 1만큼 낮춰준다.
        protected override void RemoveMaxDefaultCount(string strTag, int nTagCount)
        {
            if (DEFAULT_ID_COUNT.ContainsKey(strTag))
            {
                if (DEFAULT_ID_COUNT[strTag] == nTagCount)
                    DEFAULT_ID_COUNT[strTag] = nTagCount - 1;
            }
        }

        // 전환될 SOP
        public int LinkedActionStepID
        {
            get { return m_nLinkedActionStepID; }
            set { m_nLinkedActionStepID = value; }
        }

        public string Description
        {
            get { return m_Description; }
            set { m_Description = value; }
        }
    }
}
