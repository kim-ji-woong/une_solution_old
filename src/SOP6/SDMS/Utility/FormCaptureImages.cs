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

namespace SDMS.Utility
{
    public partial class FormCaptureImages : Form
    {
        public FormCaptureImages()
        {
            InitializeComponent();
        }

        private void btnOutdoorFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();

            if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                textBoxOutdoorFolder.Text = folderBrowserDialog.SelectedPath;
            }
        }

        private void btnIndoor_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();

            if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                textBoxIndoorFolder.Text = folderBrowserDialog.SelectedPath;
            }
        }

        private void btnRun_Click(object sender, EventArgs e)
        {
            //if (textBoxOutdoorFolder.Text.Length == 0 || textBoxIndoorFolder.Text.Length == 0)
            if (textBoxOutdoorFolder.Text.Length == 0)
                return;

            if (radioFire.Checked)
                CaptureEquipZoneImages(textBoxOutdoorFolder.Text, textBoxIndoorFolder.Text);
        }

        private void CaptureEquipZoneImages(string strOutdoorFolder, string strIndoorFolder)
        {
            int nCount = 0;
            int nTotalCount = ZoneManager.Instance.DicEquipZones.Count;

            foreach (KeyValuePair<int, EquipmentZone> pair in ZoneManager.Instance.DicEquipZones)
            {
                ZoomEquipZone(pair.Value);

                //CaptureImage(pair.Value, strOutdoorFolder, strIndoorFolder);
                CaptureImage(pair.Value, strOutdoorFolder);
                nCount++;
                System.Diagnostics.Trace.WriteLine(nCount.ToString() + " / " + nTotalCount.ToString());
                labelProcess.Text = "진행상황 : " + nCount.ToString() + " / " + nTotalCount.ToString();
                labelProcess.Refresh();
            }
        }

        private void CaptureImage(EquipmentZone equipZone, string strOutdoorFolder)
        {
            string szPath1 = FormMain.Instance.PageHome.ContentForm.SaveToTempImage();
            string szPath2 = szPath1.Replace("view1", "view2");


            System.Threading.Thread.Sleep(700);
            
            if (System.IO.File.Exists(szPath1))
            {
                if (!System.IO.Directory.Exists(strOutdoorFolder))
                    System.IO.Directory.CreateDirectory(strOutdoorFolder);

                string szFileName = strOutdoorFolder + "\\" + equipZone.ID.ToString() + ".png";
                System.Diagnostics.Trace.WriteLine("CopyFile : " + szFileName);
                System.IO.File.Copy(szPath1, szFileName, true);
                
            }

            //System.Threading.Thread.Sleep(700);

            //if (System.IO.File.Exists(szPath2))
            //{
            //    if (!System.IO.Directory.Exists(strIndoorFolder))
            //        System.IO.Directory.CreateDirectory(strIndoorFolder);
            //    System.IO.File.Copy(szPath2, strIndoorFolder + "\\" + equipZone.ID.ToString() + ".png", true);
            //}

            FormMain.Instance.PageHome.ContentForm.HideZoneVolume();
        }

        private void ZoomEquipZone(EquipmentZone equipZone)
        {
            Zone zone = equipZone.LinkedZone;
            if (zone == null)
                return;

            if (zone != null && zone.Building != null && zone.Building.BuildingID != "yhNONE")
            {
                string szName = zone.Building.BuildingID;

                FormMain.Instance.PageHome.ContentForm.ZoomBuilding(szName);

                FormMain.Instance.PageHome.ContentForm.SetCurrentBuilding(zone.Building, equipZone.LinkedZone);
                //FormMain.Instance.PageHome.ContentForm.ShowIndoor(equipZone.LinkedZone);

                FormMain.Instance.PageHome.ContentForm.ShowZoneVolume(equipZone.LinkedZone.ID, equipZone.LinkedZone.ID, true, true);
                FormMain.Instance.PageHome.ContentForm.ShowZoneVolume(equipZone.LinkedZone.ID, equipZone.ID, false, true);

            }
            else
            {
                if (equipZone.Polygon != null)
                {
                    UnE.Geometry.Vertex2D pos = equipZone.Polygon.CalcWeightCenter();
                    float dx = ZoneManager.Instance.Dx;
                    float dy = ZoneManager.Instance.Dy;


                    if (UnE.SOP.ProxySOP.Instance.SiteID == 2)
                    {
                        float x = (float)pos.x - dx;
                        float y = 0.0f;
                        float z = dy - (float)pos.y;

                        x /= 1000;
                        z /= 1000;
                        FormMain.Instance.PageHome.ContentForm.ZoomTarget(x, y, z, false);
                        FormMain.Instance.PageHome.ContentForm.ShowZoneVolume(equipZone.LinkedZone.ID, equipZone.ID, true, true);
                    }
                    else
                    {
                        float x = (float)pos.x - dx;
                        float y = 0.0f;
                        float z = dy - (float)pos.y;
                        FormMain.Instance.PageHome.ContentForm.ZoomTarget(x, y, z, false);
                        FormMain.Instance.PageHome.ContentForm.ShowZoneVolume(equipZone.LinkedZone.ID, equipZone.ID, true, true);
                    }
                }
            }
        }
    }
}
