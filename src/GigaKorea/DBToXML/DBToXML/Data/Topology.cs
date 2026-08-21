using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBUtility2;
using System.Collections;

namespace DBToXML.Data
{
    public class Topology
    {
        public class Node
        {
            private string m_strID = "";
            private double m_dX = 0.0;
            private double m_dY = 0.0;
            private List<Node> m_linkedNodes = new List<Node>();
            private List<Property> m_properties = new List<Property>();

            public const string TopologyNodeIDTag = "tnode";

            public string ID
            {
                get { return m_strID; }
                set { m_strID = value; }
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
                set { m_properties = value; }
            }
        }

        private string m_strID = "";
        private List<Node> m_nodes = new List<Node>();
        private List<Property> m_properties = new List<Property>();

        public const string TopologyIDTag = "topology";

        public string ID
        {
            get { return m_strID; }
            set { m_strID = value; }
        }

        public List<Node> Nodes
        {
            get { return m_nodes; }
        }

        public List<Property> Properties
        {
            get { return m_properties; }
        }

        public static List<Topology> ReadTopology(int nLevelID, WebDBManager dbMgr)
        {
            string strSQL = "Select t.ID, tn.ID, tn.X, tn.Y ";
            strSQL += "from Topology as t, TopologyNode as tn ";
            strSQL += "where tn.TopologyID = t.ID and t.LevelID = " + nLevelID.ToString();

            Dictionary<int, Topology> dicTopology = new Dictionary<int, Topology>();
            Dictionary<int, Topology.Node> dicNodes = new Dictionary<int, Topology.Node>();

            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return dicTopology.Values.ToList();

            int nResultCount = arrResult.Count;
            string strTopologyIDs = "";

            for (int i=0;i<nResultCount-3;i+=4)
            {
                VariousData<int> topologyID = WebDBManager.GetIntField(arrResult[i].ToString());
                VariousData<int> nodeID = WebDBManager.GetIntField(arrResult[i + 1].ToString());
                VariousData<float> x = WebDBManager.GetFloatField(arrResult[i + 2].ToString());
                VariousData<float> y = WebDBManager.GetFloatField(arrResult[i + 3].ToString());

                if (topologyID == null || nodeID == null || x == null || y == null)
                    continue;

                Topology topology;
                Topology.Node node;

                if (dicTopology.TryGetValue(topologyID.Data, out topology) == false)
                {
                    topology = new Topology();
                    topology.m_strID = TopologyIDTag + topologyID.Data.ToString();
                    dicTopology[topologyID.Data] = topology;

                    if (strTopologyIDs.Length == 0)
                        strTopologyIDs = topologyID.Data.ToString();
                    else
                        strTopologyIDs += ", " + topologyID.Data.ToString();

                    //List<Property> properties = Property.ReadDB(dbMgr, "TopologyProperties", "TopologyProperty", "TopologyID", topologyID.Data);
                    //topology.m_properties = properties;
                }

                if (dicNodes.TryGetValue(nodeID.Data, out node) == false)
                {
                    node = new Topology.Node();

                    node.ID = Topology.Node.TopologyNodeIDTag + nodeID.Data.ToString();
                    node.X = x.Data;
                    node.Y = y.Data;

                    dicNodes[nodeID.Data] = node;

                    //List<Property> properties = Property.ReadDB(dbMgr, "TopologyNodeProperties", "TopologyNodeProperty", "NodeID", nodeID.Data);
                    //node.Properties = properties;
                }

                topology.Nodes.Add(node);
            }

            if (strTopologyIDs.Length == 0)
                return dicTopology.Values.ToList();

            Dictionary<int, List<Property>> dicProperties = Property.ReadDB(dbMgr, "TopologyProperties", "TopologyProperty", "TopologyID", "LevelID = " + nLevelID.ToString());

            foreach (KeyValuePair<int, List<Property>> pair in dicProperties)
            {
                Topology topology;

                if (dicTopology.TryGetValue(pair.Key, out topology) == false)
                    continue;

                topology.m_properties = pair.Value;
            }

            dicProperties = Property.ReadDB(dbMgr, "TopologyNodeProperties", "TopologyNodeProperty", "NodeID", "TopologyID in (" + strTopologyIDs + ")");

            foreach (KeyValuePair<int, List<Property>> pair in dicProperties)
            {
                Topology.Node node;

                if (dicNodes.TryGetValue(pair.Key, out node) == false)
                    continue;

                node.Properties = pair.Value;
            }

            strSQL = "Select NodeID, LinkedNodeID from TopologyNodeLink where TopologyID in (" + strTopologyIDs + ")";
            arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return dicTopology.Values.ToList();

            nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount-1;i+=2)
            {
                VariousData<int> nodeID = WebDBManager.GetIntField(arrResult[i].ToString());
                VariousData<int> linkID = WebDBManager.GetIntField(arrResult[i + 1].ToString());

                if (nodeID == null || linkID == null)
                    continue;

                Topology.Node node, link;

                if (dicNodes.TryGetValue(nodeID.Data, out node) && dicNodes.TryGetValue(linkID.Data, out link))
                {
                    node.LinkedNodes.Add(link);
                }
            }

            return dicTopology.Values.ToList();
        }
    }
}
