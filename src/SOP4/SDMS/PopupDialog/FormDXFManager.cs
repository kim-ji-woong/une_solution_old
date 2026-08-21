using System;
using System.Net;
using System.Windows.Forms;
using UnE.Sensor;
using UnE.Spatial;

namespace SDMS
{
	public partial class FormDXFManager : Form
	{
		private string strFilePath = "";

		public FormDXFManager()
		{
			InitializeComponent();

			ComboHelper.InitBuildingGroupComboBox(cboBuildingGroup);
			button2.Enabled = false;
			mLabelFileName.Text = "도면 없음";
		}

		private void panel1_Paint(object sender, PaintEventArgs e)
		{
		}

		private void cboBuilding_SelectedIndexChanged(object sender, EventArgs e)
		{
			int nSelectedIndex = cboBuilding.SelectedIndex;
			if (nSelectedIndex < 0)
				return;

			Object obj = cboBuilding.Items[nSelectedIndex];
			if (obj.GetType() == typeof(Building))
			{
				ComboHelper.InitFloorComboBox(cboFloor, (Building)obj);
			}
			else
			{
				cboFloor.Items.Clear();
				cboFloor.Items.Add("-");
			}

			if (cboFloor.Items.Count > 0)
				cboFloor.SelectedIndex = 0;
		}

		private void cboBuildingGroup_SelectedIndexChanged(object sender, EventArgs e)
		{
			int nSelectedIndex = cboBuildingGroup.SelectedIndex;
			if (nSelectedIndex < 0)
				return;
			BuildingGroup buildingGroup = (BuildingGroup)cboBuildingGroup.Items[nSelectedIndex];
			ComboHelper.InitBuildingComboBox(cboBuilding, buildingGroup);
			if (cboBuilding.Items.Count > 0)
				cboBuilding.SelectedIndex = 0;
		}

		private void button3_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void cboFloor_SelectedIndexChanged_1(object sender, EventArgs e)
		{
			int nSelectedIndex = cboBuilding.SelectedIndex;
			if (nSelectedIndex < 0)
				return;

			Building building = (Building)cboBuilding.Items[nSelectedIndex];

			nSelectedIndex = cboFloor.SelectedIndex;
			if (nSelectedIndex < 0)
				return;

			Floor floor = (Floor)cboFloor.Items[nSelectedIndex];
			Zone zone = ZoneManager.Instance.GetZone(building.BuildingID, floor.FloorIndex);

			strFilePath = zone.DXFFilePath;

            
			mLabelFileName.Text = zone.DXFFileName;

            if( zone.DXFFileName == "blank.png")
                mLabelFileName.Text = "도면 없음";
			//Zone zoneSelected = null;
			//if (cboFloor.Text.Length == 0)
			//    return;
			//else if(cboFloor.Text == "-")
			//{
			//    zoneSelected = (Zone)cboBuilding.Items[cboBuilding.SelectedIndex];
			//}
			//else
			//{
			//    Floor floor = (Floor)cboFloor.Items[cboFloor.SelectedIndex];
			//    zoneSelected = floor.Zone;
			//}

			//if (zoneSelected == null)
			//    return;
		}

		private Uri FTPConnect(string ftpUrl)
		{
			Uri ftpUri = new Uri(ftpUrl);
			FtpWebRequest reqFtp = (FtpWebRequest)WebRequest.Create(ftpUrl);

			//사용 할 기능 설정
			reqFtp.Method = WebRequestMethods.Ftp.GetFileSize;
			reqFtp.Credentials = new NetworkCredential("sop_user", "9449966Ab$");

			//요청에 대한 응답을 받는다
			FtpWebResponse resFtp = (FtpWebResponse)reqFtp.GetResponse();
			long fileSize = resFtp.ContentLength;
			resFtp.Close();

			return ftpUri;
		}

		private void button1_Click(object sender, EventArgs e)
		{
			try
			{
				if (mLabelFileName.Text == "도면 없음")
				{
					MessageBox.Show("해당 존의 도면이 없습니다.");
					return;
				}

				string SavePath = "";
				saveFileDialog1.Filter = "AutoCAD Interchange File (*.dxf)|*.dxf";

				saveFileDialog1.FileName = mLabelFileName.Text;
				if (saveFileDialog1.ShowDialog() == DialogResult.OK)
				{
					SavePath = saveFileDialog1.FileName;
				}
				DBUtility.WebDBManager webDBManager = FormMain.Instance.DBManager;
				string ftpUrl = webDBManager.LoadIni("dxf_ftp_url");

				ftpUrl = ftpUrl + "/DXF" + "/" + strFilePath;
				ftpUrl = ftpUrl.Replace("\\", "/");

				Uri ftpUri = FTPConnect(ftpUrl);
				using (WebClient request = new WebClient())
				{
					request.Credentials = new NetworkCredential("sop_user", "9449966Ab$");

					request.DownloadFileAsync(ftpUri, SavePath);

					request.DownloadFileCompleted += request_DownloadFileCompleted;
				}
			}
			catch (Exception)
			{
			}
		}

		private void request_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
		{
			MessageBox.Show("Download Completed");
		}
	}
}