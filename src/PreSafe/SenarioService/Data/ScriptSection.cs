using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Web;
using System.Text;

using PreSafe;

namespace PreSafe
{

    public class ScriptSection
    {
        public enum ComponentType { PROCESS = 0, DECISION, ANNOTATION, ENDPOINT, LINK, TRANSSOP, INTERNAL, EXTERNAL, TRANSMISSION, GROUP, NONE }

        public enum ScriptType
        {
            Decision  = 1,
            Proecess = 2,
            End = 3,
            None = 4
        }

        private bool m_bBeginSection = false;
        public bool BeginSection
        {
            get { return m_bBeginSection; }
            set { m_bBeginSection = value; }
        }

        protected ScriptType m_nType = ScriptType.None;
        public ScriptType Type
        {
            get { return m_nType; }
            set { m_nType = value; }
        }

        protected bool m_bIncludeScript = false;
        public bool IsIncludeScript
        {
            get { return m_bIncludeScript; }
        }

        protected string m_TargetComponentID = "";
        public string TargetComponent
        {
            get { return m_TargetComponentID; }
            set { m_TargetComponentID = value; }
        }               

        protected ArrayList m_Links = new ArrayList();
        public ArrayList LinkList
        {
            get { return m_Links; }
        }

        protected string m_szScript = "";
        public string Script
        {
            get { return m_szScript; }
            set
            {
                if( value != null && value != "")
                {
                    m_bIncludeScript = true;
                    m_szScript = value; 
                }                
            }
        }

        private string m_szResult = "";
        public string Result
        {
            get { return m_szResult; }
        }

        public ScriptSection()
        {
        }

        public void AddLink(ScriptSectionLink link)
        {
            if( link.BeginSection == this)
            {
                if (!m_Links.Contains(link))
                    m_Links.Add(link);
            }
        }

        public ArrayList GetNextSection()
        {
            ArrayList arResult = new ArrayList();
            foreach(ScriptSectionLink link in m_Links)
            {
                arResult.Add(link.EndSection);            
            }
            return arResult;
        }
    }


    public class ScriptSectionLink
    {
        public enum LinkType
        {
            Yes = 1,
            No = 2,
            None = 3
        }

        private ScriptSection m_BeginSection = null;
        public ScriptSection BeginSection
        {
            get { return m_BeginSection; }
            set { m_BeginSection = value; }
        }
        private ScriptSection m_EndSection = null;
        public ScriptSection EndSection
        {
            get { return m_EndSection; }
            set { m_EndSection = value; }
        }

        private LinkType m_nType = LinkType.None;
        public LinkType Type
        {
            get { return m_nType; }
            set { m_nType = value; }
        }


    }
}