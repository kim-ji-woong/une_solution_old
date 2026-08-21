using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RtspUrlEditor
{
    public partial class FormZone : Form
    {
        public FormZone(Dictionary<int, List<Building>> dicBuildingGroups, List<Zone> outdoorZones)
        {
            InitializeComponent();
            InitTree(dicBuildingGroups, outdoorZones);
        }

        private void FormZone_Load(object sender, EventArgs e)
        {
            treeZones.ExpandAll();
        }

        private void InitTree(Dictionary<int, List<Building>> dicBuildingGroups, List<Zone> outdoorZones)
        {
            foreach (KeyValuePair<int, List<Building>> pair in dicBuildingGroups)
            {
                if (pair.Value.Count == 0)
                    continue;

                string strBuildingGroupName = pair.Value[0].BuildingGroupName;
                TreeNode node = treeZones.Nodes.Add(strBuildingGroupName);

                foreach (Building building in pair.Value)
                {
                    AddBuilding(building, node);
                }
            }

            if (outdoorZones.Count > 0)
            {
                TreeNode node = treeZones.Nodes.Add("외부영역");

                foreach (Zone zone in outdoorZones)
                {
                    AddZone(zone, node);
                }
            }
        }

        private void AddBuilding(Building building, TreeNode node)
        {
            TreeNode _node = node.Nodes.Add(building.BuildingName);
            _node.Tag = building;

            foreach (Zone zone in building.Zones)
            {
                AddZone(zone, _node);
            }
        }

        private void AddZone(Zone zone, TreeNode node)
        {
            TreeNode _node = node.Nodes.Add(zone.ZoneName);
            _node.Tag = zone;
        }

        private void treeZones_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (treeZones.SelectedNode == null)
                return;

            object tag = treeZones.SelectedNode.Tag;

            if (tag == null)
                return;

            if (tag is Zone)
            {
                Zone zone = (Zone)tag;
                FormMain.Instance.OnSelectZone(zone);
            }
        }
    }
}
