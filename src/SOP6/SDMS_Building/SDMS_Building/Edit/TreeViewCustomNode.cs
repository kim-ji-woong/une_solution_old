using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aga.Controls.Tree;

namespace SDMS_Building.Edit
{
    public enum NodeType { None, Building, Zone, EquipmentZone, Sensor, CCTV, SensorList, CCTVList }
    public class TreeViewCustomNode : Node
    {
        public NodeType NodeType = NodeType.Building;
        public TreeViewCustomNode(string text)
            : base(text)
        {
        }
    }
}
