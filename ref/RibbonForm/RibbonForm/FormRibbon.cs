using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UnE.GUI.Contorl;

namespace UnE.GUI
{
	public partial class FormRibbon : FormRibbonTab
	{
		public FormRibbon()
		{
			InitializeComponent();
			TopLevel = false;
			Dock = DockStyle.Fill;
		}
	}
}
