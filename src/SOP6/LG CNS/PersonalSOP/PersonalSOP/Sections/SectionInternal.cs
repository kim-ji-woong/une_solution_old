using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PersonalSOP.Sections
{
    public class SectionInternal : Section
    {
        public SectionInternal()
        {
            m_data = new SectionDataInternal();
        }

        public override ComponentType GetComponentType()
        {
            return ComponentType.INTERNAL;
        }
    }
}