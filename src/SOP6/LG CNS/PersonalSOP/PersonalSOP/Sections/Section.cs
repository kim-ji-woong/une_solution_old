using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;

namespace PersonalSOP.Sections
{
    using Models;

    public abstract class Section
    {
        protected string m_strText = "";

        protected SectionData m_data = null;
        protected Section m_sectionParent = null;
        protected ArrayList m_arrChildSection = new ArrayList();
        protected string m_strSectionName = "";

        protected ArrayList m_arrArrows = new ArrayList();

        public enum ColorTarget { LINE, FILL, TEXT };
        public enum ComponentType { PROCESS = 0, DECISION, ANNOTATION, ENDPOINT, LINK, TRANSSOP, INTERNAL, EXTERNAL, TRANSMISSION, GROUP, NONE }

        public virtual string Title
        {
            get
            {
                return m_strText;
            }
            set
            {
                m_strText = value;
            }
        }

        public ArrayList Arrows
        {
            get { return m_arrArrows; }
            set { m_arrArrows = value; }
        }

        public Section()
        {
        }

        public virtual bool AddArrow(Arrow arrow)
        {
            Section sectionBegin = arrow.BeginLink;
            if (sectionBegin == null)
                return false;

            Section sectionEnd = arrow.EndLink;
            if (sectionEnd == null)
                return false;

            foreach (Arrow _arrow in m_arrArrows)
            {
                if (_arrow.BeginLink == sectionBegin && _arrow.EndLink == sectionEnd)
                    return false;
            }

            m_arrArrows.Add(arrow);
            return true;
        }

        public abstract ComponentType GetComponentType();
        
        public Section GetParentSection()
        {
            return m_sectionParent;
        }

        public ArrayList GetChildSections()
        {
            return m_arrChildSection;
        }

        public virtual string SectionName
        {
            get
            {
                return m_strSectionName;
            }
            set
            {
                m_strSectionName = value;
            }
        }

        public SectionData Data
        {
            get { return m_data; }
            set
            {
                m_data = value;
            }
        }
    }
}