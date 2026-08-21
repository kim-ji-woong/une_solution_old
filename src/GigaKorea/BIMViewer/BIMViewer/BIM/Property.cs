using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BIMViewer.BIM
{
    public class Property
    {
        private string m_strName = "";
        private string m_strValue = "";
        private string m_strDescription = null;
        
        public string Name
        {
            get { return m_strName; }
            set { m_strName = value; }
        }

        public string Value
        {
            get { return m_strValue; }
            set { m_strValue = value; }
        }

        public string Description
        {
            get { return m_strDescription; }
            set { m_strDescription = value; }
        }
    }
}
