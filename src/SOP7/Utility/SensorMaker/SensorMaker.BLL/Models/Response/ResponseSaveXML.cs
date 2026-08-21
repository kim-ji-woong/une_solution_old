using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace SensorMaker.BLL.Models.Response
{
    public class ResponseSaveXML : MessageResult
    {
        private XDocument m_xDocument = null;
        public XDocument XDocument
        { 
            get { return m_xDocument; } 
            set { m_xDocument = value; }
        }
    }
}
