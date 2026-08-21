using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Aga.Controls.Tree;
using SDMS;
using UnE.Sensor;
using UnE.Spatial;

namespace SDMS_Building.Edit
{
    public partial class FormCCTVLink : Form
    {
        private TreeModel m_modelSensor = null;
        private TreeModel m_modelCCTV = null;

        private List<EquipmentZone> m_editZones = new List<EquipmentZone>();

        /// <summary>
        /// Dictionary가 수정되어 TreeView를 만들 때 에러나는 것을 방지하기 위해 복사된 Dictionary를 사용한다.
        /// </summary>
        private Dictionary<int, CCTV> m_dicCCTVCopy = null;

        public FormCCTVLink()
        {
            InitializeComponent();

            RefreshForm();
        }
        
        public void RefreshForm()
        {
            if (m_dicCCTVCopy != null)
            {
                m_dicCCTVCopy.Clear();
                m_dicCCTVCopy = null;
            }

            m_dicCCTVCopy = new Dictionary<int, CCTV>(CCTVManager.Instance.DicCCTVs);

            LoadEquipZoneCCTV();
            InitTreeView();
        }

        private void InitTreeView()
        {
            //treeViewSensor.ImageList = imageList1;
            //treeViewCCTV.ImageList = imageList1;
            
            m_modelSensor = new TreeModel();
            m_modelCCTV = new TreeModel();
            
            foreach (KeyValuePair<int, Building> item in ZoneManager.Instance.DicBuildings)
            {
                MakeBuilding(item.Value, false, m_modelSensor);
                MakeBuilding(item.Value, true, m_modelCCTV);
            }

            treeViewSensor.Model = m_modelSensor;
            treeViewCCTV.Model = m_modelCCTV;
        }
        
        private Node AddRoot(Building building, TreeModel model)
        {
            TreeViewCustomNode node = new TreeViewCustomNode(building.BuildingName);
            node.NodeType = NodeType.Building;
            node.Tag = building;
            
            node.Image = imageList1.Images[0];
            model.Nodes.Add(node);
            return node;
        }

        private Node AddChild(string text, object tag, NodeType nodeType)
        {
            TreeViewCustomNode node = new TreeViewCustomNode(text);
            node.NodeType = nodeType;
            node.Tag = tag;
            node.Image = imageList1.Images[3];            
            //parent.Nodes.Add(node);
            return node;
        }

        private void MakeBuilding(Building building, bool isCCTV, TreeModel model)
        {
            Node node = AddRoot(building, model);
            
            foreach (KeyValuePair<int, Zone> item in ZoneManager.Instance.DicZones)
            {
                Zone zone = item.Value;
                if (zone.Building == building)
                    MakeZone(node, zone, isCCTV);
            }
        }
        private void MakeZone(Node parent, Zone zone, bool isCCTV)
        {
            Node node = AddChild(zone.Floor.ToString(), zone, NodeType.Zone);
            parent.Nodes.Add(node);

            if (!isCCTV)
            {
                List<EquipmentZone> equipZoneList = ZoneManager.Instance.GetEquipmentZoneList(zone);
                if (equipZoneList == null)
                    return;
                
                foreach (EquipmentZone equipment in equipZoneList)
                {
                    EquipmentZoneObjectList list = new EquipmentZoneObjectList();
                    if (SensorManager.Instance.DicSensorZone.ContainsKey(equipment.ID))
                    {
                        list = SensorManager.Instance.DicSensorZone[equipment.ID];
                        
                        Node equipZoneNode = AddChild(equipment.ZoneName, equipment, NodeType.EquipmentZone);
                        node.Nodes.Add(equipZoneNode);

                        Node category = AddChild("센서 목록", null, NodeType.SensorList);
                        Node category2 = AddChild("연결된 CCTV 목록", null, NodeType.CCTVList);
                        
                        equipZoneNode.Nodes.Add(category2);
                        
                        if (list.SensorList.Count > 0)
                        {
                            equipZoneNode.Nodes.Add(category);

                            foreach (ISensor sensor in list.SensorList)
                            {
                                Node sensorNode = AddChild(sensor.SensorName, sensor, NodeType.Sensor);
                                category.Nodes.Add(sensorNode);
                            }

                            CCTV[] cctvs = CCTVManager.Instance.GetCCTVArray(equipment);
                            if (cctvs != null)
                            {
                                foreach (CCTV cctv in cctvs)
                                {
                                    if (cctv == null)
                                        continue;

                                    Node sensorNode = AddChild(cctv.AccessKey, cctv, NodeType.CCTV);
                                    category2.Nodes.Add(sensorNode);
                                }
                            }   
                        }
                    }
                }                
            }
            else
            {
                foreach (KeyValuePair<int, CCTV> item in m_dicCCTVCopy)
                {
                    CCTV cctv = item.Value;

                    if (cctv.POI.Zone == zone)
                    {
                        // cctv 추가
                        Node addCCTVNode = AddChild(cctv.AccessKey, cctv, NodeType.CCTV);
                        node.Nodes.Add(addCCTVNode);

                        if (m_cctvZone.ContainsKey(cctv.ID) && m_cctvZone[cctv.ID].Count > 0)
                        {
                            List<int> equipzoneIDs = m_cctvZone[cctv.ID];
                            foreach (int equipzoneID in equipzoneIDs)
                            {
                                EquipmentZone equipmentZone = ZoneManager.Instance.GetEquipZone(equipzoneID);

                                Node zoneNode = AddChild(equipmentZone.ZoneName, equipmentZone, NodeType.EquipmentZone);
                                addCCTVNode.Nodes.Add(zoneNode);
                            }
                        }
                        // cctv에 연결된 equipzone 추가
                        //List<EquipmentZone> equipZoneList = ZoneManager.Instance.GetEquipmentZoneList(zone);
                        //foreach (EquipmentZone equipZone in equipZoneList)
                        //{
                        //    CCTV[] cctvs = CCTVManager.Instance.GetCCTVArray(equipZone);
                        //    if (cctvs != null)
                        //    {
                        //        foreach (CCTV cctv in cctvs)
                        //        {
                        //            if (cctv == null)
                        //                continue;

                        //            TreeNodeAdv findNode = FindNodeByTag(treeViewCCTV.Root, cctv);


                        //            Node sensorNode = AddChild(cctv.AccessKey, cctv, NodeType.CCTV);
                        //            findNode.Nodes.Add(sensorNode);
                        //            //category2.Nodes.Add(sensorNode);
                        //        }
                        //    }
                        //}
                    }
                }
            }
        }

        Dictionary<int, List<int>> m_cctvZone = new Dictionary<int, List<int>>();

        private void LoadEquipZoneCCTV()
        {
            ArrayList arrResult = FormMain.Instance.DBManager.GetResultData("SELECT EquipZoneID, CCTV1, CCTV2, CCTV3, CCTV4 FROM EquipZoneCCTV WHERE CCTV1 IS NOT NULL OR CCTV2 IS NOT NULL OR CCTV3 IS NOT NULL OR CCTV4 IS NOT NULL");
            if (arrResult == null || arrResult.Count == 0)
                return;

            for (int i = 0; i < arrResult.Count; i+=5)
            {
                int nEquipZoneID = DBUtility2.WebDBManager.GetIntField(arrResult[i].ToString(), -1);

                if (nEquipZoneID < 0)
                    continue;

                int[] nCCTV = new int[4];
                for (int j = 1; j <= 4; j++)
                {
                    int nCctvID = DBUtility2.WebDBManager.GetIntField(arrResult[i + j].ToString(), -1);
                    if (nCctvID < 0)
                        continue;

                    if (!m_cctvZone.ContainsKey(nCctvID))
                        m_cctvZone.Add(nCctvID, new List<int>());

                    m_cctvZone[nCctvID].Add(nEquipZoneID);
                }
            }
        }

        private TreeViewAdv m_dragTreeView = null;

        private void treeView_ItemDrag(object sender, ItemDragEventArgs e)
        {
            TreeViewAdv treeView = sender as TreeViewAdv;
            

            TreeNodeAdv[] selectedNodes = e.Item as TreeNodeAdv[];

            foreach (TreeNodeAdv item in selectedNodes)
            {
                TreeViewCustomNode node = item.Tag as TreeViewCustomNode;
                if (node.NodeType == NodeType.Building || node.NodeType == NodeType.Zone)
                    return;
                else if (treeView.Name == "treeViewSensor" && node.NodeType == NodeType.CCTV)
                    return;
                else if (treeView.Name == "treeViewCCTV" && node.NodeType == NodeType.Sensor)
                    return;
            }

            m_dragTreeView = treeView;
            treeView.DoDragDropSelectedNodes(DragDropEffects.Move);
        }

        private void treeView_DragOver(object sender, DragEventArgs e)
        {
            TreeViewAdv treeView = sender as TreeViewAdv;

            if (treeView == m_dragTreeView)
                return;

            if (e.Data.GetDataPresent(typeof(TreeNodeAdv[])) && treeView.DropPosition.Node != null)
            {
                TreeNodeAdv[] nodes = e.Data.GetData(typeof(TreeNodeAdv[])) as TreeNodeAdv[];
                TreeNodeAdv dropNode = treeView.DropPosition.Node;

                TreeViewCustomNode dropNode2 = dropNode.Tag as TreeViewCustomNode;

                bool check = false;
                if (treeView.Name == "treeViewSensor" && dropNode2.NodeType == NodeType.CCTVList && treeView.DropPosition.Position == NodePosition.Inside) 
                    check = true;
                if (treeView.Name == "treeViewCCTV" && dropNode2.NodeType == NodeType.CCTV && treeView.DropPosition.Position == NodePosition.Inside)
                    check = true;

                if (!check)
                {
                    e.Effect = DragDropEffects.None;
                    return;
                }

                if (treeView.DropPosition.Position != NodePosition.Inside)
                    dropNode = dropNode.Parent;

                foreach (TreeNodeAdv node in nodes)
                {
                    if (!CheckNodeParent(dropNode, node))
                    {
                        e.Effect = DragDropEffects.None;
                        return;
                    }
                }

                e.Effect = e.AllowedEffect;
            }
        }

        private bool CheckNodeParent(TreeNodeAdv parent, TreeNodeAdv node)
        {
            while (parent != null)
            {
                if (node == parent)
                    return false;
                else
                    parent = parent.Parent;
            }
            return true;
        }

        private void treeView_DragDrop(object sender, DragEventArgs e)
        {
            TreeViewAdv treeView = sender as TreeViewAdv;

            treeView.BeginUpdate();

            TreeNodeAdv[] nodes = (TreeNodeAdv[])e.Data.GetData(typeof(TreeNodeAdv[]));
            
            Node dropNode = treeView.DropPosition.Node.Tag as Node;
            if (treeView.DropPosition.Position == NodePosition.Inside)
            {
                foreach (TreeNodeAdv n in nodes)
                {
                    //(n.Tag as Node).Parent = dropNode;

                    bool overlap = false;
                    foreach (Node child in dropNode.Nodes)
                    {
                        if (child.Tag == (n.Tag as Node).Tag)
                        {
                            overlap = true;
                            break;
                        }
                    }

                    if (overlap)
                        continue;

                    Node orgParentNode = n.Parent.Tag as Node;
                    int orgIndex = n.Index;

                    Node item = n.Tag as Node;

                    List<Node> childs = new List<Node>();
                    foreach (Node childNode in item.Nodes)
                    {
                        childs.Add(childNode);
                    }

                    dropNode.Nodes.Add(item);

                    // drop하는 쪽 treeview에 추가되면서 drag한 treeview에 있던 node가 삭제되므로 다시 추가해준다
                    Node addOrgNode = AddChild(item.Text, item.Tag, (treeView.Name == "treeViewSensor") ? NodeType.CCTV : NodeType.Sensor);
                    orgParentNode.Nodes.Insert(orgIndex, addOrgNode);
                                       
                    if (childs.Count > 0)
                    {
                        foreach (Node childNode in childs)
                        {
                            addOrgNode.Nodes.Add(childNode);
                        }
                    }

                    if (treeView.Name == "treeViewSensor")
                    {
                        // CCTV에 연결된 EquipmentZone 추가
                        Node linkNode = dropNode.Parent;
                        
                        Node addChildNode = AddChild(linkNode.Text, linkNode.Tag, NodeType.EquipmentZone);
                        addOrgNode.Nodes.Add(addChildNode);

                        if (linkNode.Tag is EquipmentZone)
                        {
                            EquipmentZone equipZone = linkNode.Tag as EquipmentZone;
                            if (!m_editZones.Contains(equipZone))
                                m_editZones.Add(equipZone);
                        }
                    }
                    else
                    {                        
                        for (int i = 0; i < n.Children.Count; i++)
                        {
                            if ((n.Children[i].Tag as TreeViewCustomNode).NodeType == NodeType.CCTVList)
                            {
                                // 연결된 CCTV 목록에 Drop한 CCTV 추가
                                Node targetNode = AddChild(dropNode.Text, dropNode.Tag, NodeType.CCTV);
                                Node parentNode = n.Children[i].Tag as Node; 
                                
                                parentNode.Nodes.Add(targetNode);

                                if ((n.Tag as Node) != null)
                                {
                                    if ((n.Tag as Node).Tag is EquipmentZone)
                                    {
                                        EquipmentZone equipZone = (n.Tag as Node).Tag as EquipmentZone;
                                        if (!m_editZones.Contains(equipZone))
                                            m_editZones.Add(equipZone);
                                    }
                                }
                                
                                break;
                            }
                        }
                    }
                }
            }
            else
            {
                Node parent = dropNode.Parent;
                Node nextItem = dropNode;
                if (treeView.DropPosition.Position == NodePosition.After)
                    nextItem = dropNode.NextNode;

                //foreach (TreeNodeAdv node in nodes)
                //    (node.Tag as Node).Parent = null;

                int index = -1;
                index = parent.Nodes.IndexOf(nextItem);
                foreach (TreeNodeAdv node in nodes)
                {
                    bool overlap = false;
                    foreach (Node child in dropNode.Nodes)
                    {
                        if (child.Tag == (node.Tag as Node).Tag)
                        {
                            overlap = true;
                            break;
                        }
                    }

                    if (overlap)
                        continue;

                    Node orgParentNode = node.Parent.Tag as Node;
                    int orgIndex = node.Index;

                    Node item = node.Tag as Node;

                    List<Node> childs = new List<Node>();
                    foreach (Node childNode in item.Nodes)
                    {
                        childs.Add(childNode);
                    }
                    
                    if (index == -1)
                        parent.Nodes.Add(item);
                    else
                    {
                        parent.Nodes.Insert(index, item);
                        index++;
                    }

                    // drop하는 쪽 treeview에 추가되면서 drag한 treeview에 있던 node가 삭제되므로 다시 추가해준다
                    Node addOrgNode = AddChild(item.Text, item.Tag, (treeView.Name == "treeViewSensor") ? NodeType.CCTV : NodeType.Sensor);
                    orgParentNode.Nodes.Insert(orgIndex, addOrgNode);

                    if (childs.Count > 0)
                    {
                        foreach (Node childNode in childs)
                        {
                            addOrgNode.Nodes.Add(childNode);
                        }
                    }

                    //addOrgNode에 추가
                    Node addChildNode = AddChild(dropNode.Text, dropNode.Tag, (treeView.Name == "treeViewSensor") ? NodeType.Sensor : NodeType.CCTV);
                    addOrgNode.Nodes.Add(addChildNode);
                }
            }
            
            treeView.EndUpdate();
        }
        
        private void treeView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Delete)
                return;

            TreeViewAdv treeview = sender as TreeViewAdv;
            if (treeview.SelectedNodes == null || treeview.SelectedNodes.Count == 0)
                return;

            List<TreeNodeAdv> deleteNodes = new List<TreeNodeAdv>();
            foreach (TreeNodeAdv node in treeview.SelectedNodes)
            {
                Node node2 = node.Tag as Node;
                if (treeview.Name == "treeViewSensor" && node2.Tag is CCTV) 
                {
                    deleteNodes.Add(node);
                    
                    EquipmentZone equipZone = (node.Parent.Parent.Tag as Node).Tag as EquipmentZone;
                    TreeNodeAdv findNode = DeleteFindNode(treeViewCCTV, node2.Tag as CCTV, equipZone);
                    if (findNode != null)
                        deleteNodes.Add(findNode);

                    if (!m_editZones.Contains(equipZone))
                        m_editZones.Add(equipZone);
                }
                else if (treeview.Name == "treeViewCCTV" && node2.Tag is EquipmentZone)
                {
                    deleteNodes.Add(node);

                    EquipmentZone equipZone = node2.Tag as EquipmentZone;
                    TreeNodeAdv findNode = DeleteFindNode(treeViewSensor, equipZone, (node.Parent.Tag as Node).Tag as CCTV);
                    if (findNode != null)
                        deleteNodes.Add(findNode);

                    if (!m_editZones.Contains(equipZone))
                        m_editZones.Add(equipZone);
                }
            }

            foreach (TreeNodeAdv node in deleteNodes)
            {
                (node.Tag as Node).Parent = null;     
            }
        }

        private TreeNodeAdv DeleteFindNode(TreeViewAdv treeView, object filter1, object filter2)
        {
            TreeNodeAdv findNode = null;
            
            TreeNodeAdv findNode1 = FindNodeByTag(treeView.Root, filter1);
            if (findNode1 == null)
                return findNode;

            TreeNodeAdv findNode2 = FindNodeByTag(findNode1, filter2);
            if (findNode2 == null)
                return findNode;

            findNode = findNode2;

            return findNode;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            TreeNodeAdv findNode = FindNodeByText(textBox1.Text);
            if (findNode != null)
            {
                treeViewSensor.ClearSelection();

                TreeNodeAdv parentNode = findNode.Parent;

                while (true) // 부모 Node 모두 Expand
                {
                    parentNode.Expand(true);
                    parentNode = parentNode.Parent;
                    if (parentNode == null)
                        break;
                }

                findNode.IsSelected = true;
            }
        }

        private TreeNodeAdv FindNodeByText(string text)
        {
            return FindNodeByText(treeViewSensor.Root, text);
        }

        private TreeNodeAdv FindNodeByText(TreeNodeAdv root, string text)
        {
            foreach (TreeNodeAdv node in root.Nodes)
            {
                if (node.ToString().Replace(" ", "").Contains(text.Replace(" ", "")))
                    return node;
                TreeNodeAdv res = FindNodeByText(node, text);
                if (res != null)
                    return res;
            }
            return null;
        }
        
        private TreeNodeAdv FindNodeByTag(TreeNodeAdv root, object tag)
        {
            foreach (TreeNodeAdv node in root.Nodes)
            {
                if ((node.Tag as Node).Tag == tag)
                    return node;
                TreeNodeAdv res = FindNodeByTag(node, tag);
                if (res != null)
                    return res;
            }
            return null;
        }

        private void Save()
        {
            List<string> querys = new List<string>();

            foreach (EquipmentZone equipment in m_editZones)
            {
                TreeNodeAdv node = FindNodeByTag(treeViewSensor.Root, equipment);
                foreach (TreeNodeAdv item in node.Children)
                {
                    if ((item.Tag as TreeViewCustomNode).NodeType == NodeType.CCTVList)
                    {
                        List<int> cctvs = new List<int>();
                        foreach (TreeNodeAdv cctvNode in item.Nodes)
                        {
                            CCTV cctv = (cctvNode.Tag as Node).Tag as CCTV;
                            if (cctv != null)
                            {
                                cctvs.Add(cctv.ID);
                            }
                        }

                        StringBuilder sb = new StringBuilder();
                        sb.AppendFormat("Update EquipZoneCCTV Set CCTV1={1}, CCTV2={2}, CCTV3={3}, CCTV4={4} Where EquipZoneID={0} "
                            , equipment.ID
                            , (cctvs.Count > 0) ? cctvs[0].ToString() : "NULL"
                            , (cctvs.Count > 1) ? cctvs[1].ToString() : "NULL"
                            , (cctvs.Count > 2) ? cctvs[2].ToString() : "NULL"
                            , (cctvs.Count > 3) ? cctvs[3].ToString() : "NULL");

                        querys.Add(sb.ToString());
                    }
                }
            }

            foreach (string query in querys)
            {
                FormMain.Instance.DBManager.GetResultData(query);
            }

            m_editZones.Clear();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.No;
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.No;
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (m_editZones.Count == 0)
                return;

            this.Cursor = Cursors.WaitCursor;
            Save();
            this.Cursor = Cursors.Default;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            TreeNodeAdv findNode = FindNodeByText(textBox1.Text);
            if (findNode != null)
            {
                treeViewSensor.ClearSelection();

                TreeNodeAdv parentNode = findNode.Parent;

                while (true) // 부모 Node 모두 Expand
                {
                    parentNode.Expand(true);
                    parentNode = parentNode.Parent;
                    if (parentNode == null)
                        break;
                }

                findNode.IsSelected = true;
            }
        }
    }
}
