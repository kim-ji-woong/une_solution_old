using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BIMViewer.BIM
{
    public class Topology
    {
        public class Node
        {
            private int m_nID = 0;
            private string m_strXMLID = "";
            private double m_dX = 0.0;
            private double m_dY = 0.0;
            private List<Node> m_linkedNodes = new List<Node>();
            private List<Property> m_properties = new List<Property>();

            public int ID
            {
                get { return m_nID; }
                set { m_nID = value; }
            }

            public string XMLID
            {
                get { return m_strXMLID; }
                set { m_strXMLID = value; }
            }

            public double X
            {
                get { return m_dX; }
                set { m_dX = value; }
            }

            public double Y
            {
                get { return m_dY; }
                set { m_dY = value; }
            }

            public List<Node> LinkedNodes
            {
                get { return m_linkedNodes; }
            }

            public List<Property> Properties
            {
                get { return m_properties; }
            }
        }

        private int m_nID = 0;
        private string m_strXMLID = "";
        private List<Node> m_nodes = new List<Node>();
        private List<Property> m_properties = new List<Property>();

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public string XMLID
        {
            get { return m_strXMLID; }
            set { m_strXMLID = value; }
        }

        public List<Node> Nodes
        {
            get { return m_nodes; }
        }

        public List<Property> Properties
        {
            get { return m_properties; }
        }
    }
}
