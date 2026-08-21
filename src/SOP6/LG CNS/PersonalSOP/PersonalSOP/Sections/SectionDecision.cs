using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PersonalSOP.Sections
{
    public class SectionDecision : Section
    {
        public SectionDecision()
        {
            m_data = new SectionDataDecision();
        }

        public override ComponentType GetComponentType()
        {
            return ComponentType.DECISION;
        }
    }
}