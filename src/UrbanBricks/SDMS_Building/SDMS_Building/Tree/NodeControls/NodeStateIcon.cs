using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using SDMS_Building.Properties;

namespace Aga.Controls.Tree.NodeControls
{
	public class NodeStateIcon: NodeIcon
	{
		private Image _leaf;
		private Image _opened;
		private Image _closed;

		public NodeStateIcon()
		{
            _leaf = null;//MakeTransparent(Resources.Leaf);
            _opened = Resources.down;//MakeTransparent(Resources.Folder);
            _closed = Resources.right;//MakeTransparent(Resources.FolderClosed);
		}

		private static Image MakeTransparent(Bitmap bitmap)
		{
			bitmap.MakeTransparent(bitmap.GetPixel(0,0));
			return bitmap;
		}

		protected override Image GetIcon(TreeNodeAdv node)
		{
            // 자식 노드가 없을 경우 StateIcon을 표시하지 않는다.
            if (node.CanExpand == false)
                return null;

			Image icon = base.GetIcon(node);
			if (icon != null)
				return icon;
			else if (node.IsLeaf)
				return _leaf;
			else if (node.CanExpand && node.IsExpanded)
				return _opened;
			else
				return _closed;
		}
	}
}
