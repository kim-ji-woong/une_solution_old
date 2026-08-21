using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UnE.Spatial;

namespace SDMS.PopupDialog.DisasterPrevention
{
    public partial class FormLocationCfg : Form
    {
        public FormLocationCfg()
        {
            InitializeComponent();

            InitTreeView();
            treeView1.ExpandAll();
            treeView1.AfterSelect += treeView1_AfterSelect;
        }

        void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeNode node = e.Node;
            if (node == null)
                return;
            if (node.Tag == null)
                return;

            EquipmentZone zone = node.Tag as EquipmentZone;
            if (zone == null)
                return;

            FormDisasterPreventionManagement.Instance.SetLocation(zone); 
        } 

        private void InitTreeView()
        {  
            foreach (KeyValuePair<int, EquipmentZone> equipZone in ZoneManager.Instance.DicEquipZones)
            {
                AddTreeNode(equipZone.Value);
            } 
        }

        private void AddTreeNode(EquipmentZone paramEquipZone)
        {
            if (paramEquipZone == null)
                return;

            if (paramEquipZone.Building == null)
            {
                TreeNode groupNode = null;
                TreeNode[] nodes = treeView1.Nodes.Find("외부 영역", false);
                if (nodes == null || nodes.Length == 0)
                {
                    groupNode = treeView1.Nodes.Add("외부 영역", "외부 영역");
                }
                else
                {
                    groupNode = nodes[0];
                } 

                TreeNode zoneNode = null;
                TreeNode[] znodes = groupNode.Nodes.Find(paramEquipZone.DisplayText, false);
                if (znodes == null || znodes.Length == 0)
                {
                    zoneNode = groupNode.Nodes.Add(paramEquipZone.DisplayText, paramEquipZone.DisplayText);
                }
                else
                {
                    zoneNode = znodes[0];
                }
                zoneNode.Tag = paramEquipZone; 
            }
            else
            {           
                Zone zone = paramEquipZone.LinkedZone;
                Building building = zone.Building;
                BuildingGroup buildingGroup = building.BuildingGroup;

                if (buildingGroup != null)
                {
                    TreeNode groupNode = null;
                    TreeNode[] nodes = treeView1.Nodes.Find(buildingGroup.BuildingGroupName, false);
                    if (nodes == null || nodes.Length == 0)
                    {
                        groupNode = treeView1.Nodes.Add(buildingGroup.BuildingGroupName, buildingGroup.BuildingGroupName);
                    }
                    else
                    {
                        groupNode = nodes[0];
                    }

                    TreeNode buildingNode = null;
                    TreeNode[] bnodes = groupNode.Nodes.Find(building.BuildingName, false);
                    if (bnodes == null || bnodes.Length == 0)
                    {
                        buildingNode = groupNode.Nodes.Add(building.BuildingName, building.BuildingName);
                    }
                    else
                    {
                        buildingNode = bnodes[0];
                    }

                    TreeNode zoneNode = null;
                    TreeNode[] znodes = buildingNode.Nodes.Find(zone.DisplayText, false);
                    if (znodes == null || znodes.Length == 0)
                    {
                        zoneNode = buildingNode.Nodes.Add(zone.DisplayText, zone.DisplayText);
                    }
                    else
                    {
                        zoneNode = znodes[0];
                    }

                    TreeNode equipNode = null;
                    TreeNode[] equipNodes = zoneNode.Nodes.Find(paramEquipZone.DisplayText, false);
                    if (equipNodes == null || equipNodes.Length == 0)
                    {
                        equipNode = zoneNode.Nodes.Add(paramEquipZone.DisplayText, paramEquipZone.DisplayText);
                    }
                    else
                    {
                        equipNode = equipNodes[0];
                    }

                    equipNode.Tag = paramEquipZone;
                }
            }
        }
    }
}
