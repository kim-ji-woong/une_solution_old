using System.Windows.Forms;
using UnE.Sensor;
using UnE.Spatial;

namespace SDMS
{
	public partial class FormCCTVGuide : Form
	{
		public FormCCTVGuide()
		{
			InitializeComponent();

			labelZone.Visible = labelCCTV.Visible = false;
		}

		public void SetCCTV(CCTV cctv)
		{
			if (cctv == null)
			{
				labelZone.Text = "연결된 CCTV 정보 없음";
				labelZone.Visible = true;
				labelCCTV.Visible = false;
			}
			else
			{
				labelZone.Text = cctv.ID.ToString();
				//labelZone.Text = cctv.POI.Zone.BroadcastName;
				labelCCTV.Text = cctv.AccessKey;
				//labelCCTV.Text = cctv.IPAddress;
				labelZone.Visible = labelCCTV.Visible = true;
			}
		}

		public void Clear()
		{
			labelZone.Visible = labelCCTV.Visible = false;
		}
	}
}