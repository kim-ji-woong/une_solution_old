using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPReadServer.Data
{
    public class TeamInfoData
    {
        private string m_ORGEH = "";
        public string ORGEH
        {
            get { return m_ORGEH; }
            set { m_ORGEH = value; }
        }

        private string m_ORGTX = "";
        public string ORGTX
        {
            get { return m_ORGTX; }
            set { m_ORGTX = value; }
        }

        private string m_OWICH = "";
        public string OWICH
        {
            get { return m_OWICH; }
            set { m_OWICH = value; }
        }

        private string m_UPORGEH = "";
        public string UPORGEH
        {
            get { return m_UPORGEH; }
            set { m_UPORGEH = value; }
        }

        private string m_PARENT = "";
        public string PARENT
        {
            get { return m_PARENT; }
            set { m_PARENT = value; }
        }

        private string m_CHILD = "";
        public string CHILD
        {
            get { return m_CHILD; }
            set { m_CHILD = value; }
        }

        private string m_L_PERNR = "";
        public string L_PERNR
        {
            get { return m_L_PERNR; }
            set { m_L_PERNR = value; }
        }

        private string m_KOSTL = "";
        public string KOSTL
        {
            get { return m_KOSTL; }
            set { m_KOSTL = value; }
        }

        private string m_OLEVEL = "";
        public string OLEVEL
        {
            get { return m_OLEVEL; }
            set { m_OLEVEL = value; }
        }
    }

    public class RegularData : TeamEditor.Model.Sop.Team.Regular
    {
        bool m_bCheck = false;

        public bool Check
        {
            get { return m_bCheck; }
            set { m_bCheck = value; }
        }
    }
}
