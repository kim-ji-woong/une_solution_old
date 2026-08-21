using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PersonalSOP.Sections
{
    public class SectionExternal : Section
    {
        public SectionExternal()
        {
            m_data = new SectionDataExternal();
        }

        public override ComponentType GetComponentType()
        {
            return ComponentType.EXTERNAL;
        }
    }
}