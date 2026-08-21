using System.Windows.Forms;

namespace SDMS
{
	public partial class DockingFormFacilityList : Form
	{
		public DockingFormFacilityList()
		{
            this.DoubleBuffered = true;
			InitializeComponent();
		}
		public void SetTitle(string szText)
		{
			this.lbTitle.Text = szText;
			this.Text = szText;
		}
	}
}