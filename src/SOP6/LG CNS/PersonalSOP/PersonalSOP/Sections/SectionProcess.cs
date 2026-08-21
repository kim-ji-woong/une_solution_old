using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PersonalSOP.Sections
{
    public class SectionProcess : Section
    {
        private string m_strTextUp = "";
        private string m_strTextDown = "";

        public string TextUP
        {
            get { return m_strTextUp; }
            set { m_strTextUp = value; }
        }

        public string TextDown
        {
            get { return m_strTextDown; }
            set { m_strTextDown = value; }
        }

        public override string Title
        {
            get { return TextUP; }
            set { TextUP = value; }
        }

        public SectionProcess()
        {
            m_data = new SectionDataProcess();
        }

        public override ComponentType GetComponentType()
        {
            return ComponentType.PROCESS;
        }
    }
}