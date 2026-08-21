using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PersonalSOP.Sections
{
    public class SectionEndPoint : Section
    {
        public SectionEndPoint()
        {
            m_data = new SectionDataEndPoint();
        }

        public override ComponentType GetComponentType()
        {
            return ComponentType.ENDPOINT;
        }
    }
}